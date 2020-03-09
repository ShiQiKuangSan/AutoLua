using System;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AutoLua.Core.Common;
using Java.Lang;
using Object = Java.Lang.Object;

namespace AutoLua.Core.LuaScript.ApiCommon.MaterialDialogs
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class BlockedMaterialDialog : MaterialDialog
    {
        protected BlockedMaterialDialog(MaterialDialog.Builder builder) : base(builder)
        {
        }

        public override void Show()
        {
            if (!IsActivityContext(Context))
            {
                var type = Build.VERSION.SdkInt >= BuildVersionCodes.O
                    ? WindowManagerTypes.ApplicationOverlay
                    : WindowManagerTypes.Phone;
                Window.SetType(type);
            }

            base.Show();
        }


        private static bool IsActivityContext(Context context)
        {
            while (true)
            {
                switch (context)
                {
                    case null:
                        return false;
                    case Activity activity:
                        return !activity.IsFinishing;
                    case ContextWrapper wrapper:
                        context = wrapper.BaseContext;
                        continue;
                    default:
                        return false;
                }
            }
        }


        public new class Builder : MaterialDialog.Builder
        {
            private readonly VolatileDispose _resultBox;
            private bool _notified;

            public Builder(Context context) : base(context)
            {
                if (Looper.MainLooper != Looper.MyLooper())
                {
                    _resultBox = new VolatileDispose();
                }
            }

            public MaterialDialog.Builder Inputs(string hint, string prefill, bool allowEmptyInput,
                Action<string> callback = null)
            {
                var input = new Action<string>((r) =>
                {
                    if (_notified)
                        return;

                    _notified = true;
                    callback?.Invoke(r);
                    _resultBox?.setAndNotify(r);
                });

                Input(hint, prefill, allowEmptyInput, new InputCallbacks(i => input(i)));
                CancelListener(dialog => input(null));
                return this;
            }

            public Builder Alert(Action callback = null)
            {
                var alert = new Action<Object>((r) =>
                {
                    if (_notified)
                        return;

                    _notified = true;
                    callback?.Invoke();
                    _resultBox?.setAndNotify(r);
                });

                DismissListener(dialog => alert(null));
                OnAny((dialog, which) => alert(null));
                return this;
            }

            public Builder Confirm(Action<bool> callback = null)
            {
                var confirm = new Action<bool>((r) =>
                {
                    if (_notified)
                        return;

                    _notified = true;
                    callback?.Invoke(r);
                    _resultBox?.setAndNotify(r);
                });

                DismissListener(dialog => confirm(false));
                OnAny((dialog, which) => { confirm(which == DialogAction.Positive); });
                return this;
            }

            public MaterialDialog.Builder ItemsCallback(Action<string> callback = null)
            {
                var itemsCallback = new Action<string>((r) =>
                {
                    if (_notified)
                        return;

                    _notified = true;
                    callback?.Invoke(r);
                    _resultBox?.setAndNotify(r);
                });

                DismissListener(dialog => itemsCallback(string.Empty));
                base.ItemsCallback((dialog, itemView, position, text) => itemsCallback(text));
                return this;
            }

            public MaterialDialog.Builder ItemsCallbackMultiChoice(int[] selectedIndices,
                Action<int[], string[]> callback = null)
            {
                var itemsCallbackMultiChoice = new Func<int[], string[], bool>((which, text) =>
                {
                    if (_notified)
                        return true;

                    _notified = true;
                    callback?.Invoke(which, text);
                    _resultBox?.setAndNotify((int[])which.Clone());
                    return true;
                });

                DismissListener(dialog => itemsCallbackMultiChoice(new int[0], new string[0]));
                base.ItemsCallbackMultiChoice(selectedIndices,
                    (dialog, which, text) => itemsCallbackMultiChoice(which, text));
                return this;
            }

            public MaterialDialog.Builder ItemsCallbackSingleChoice(int selectedIndex,
                Action<int, string> callback = null)
            {
                var itemsCallbackSingleChoice = new Action<int, string>((which, text) =>
                {
                    if (_notified)
                        return;

                    _notified = true;
                    callback?.Invoke(which, text);
                    _resultBox?.setAndNotify(text);
                });

                DismissListener(dialog => itemsCallbackSingleChoice(-1, string.Empty));
                base.ItemsCallbackSingleChoice(selectedIndex, (dialog, itemView, which, text) =>
                {
                    itemsCallbackSingleChoice(which, text);
                    return true;
                });
                return this;
            }

            /// <summary>
            /// 显示数据。
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T ShowAndGet<T>()
            {
                if (Looper.MyLooper() == Looper.MainLooper)
                {
                    base.Show();
                }
                else
                {
                    AppUtils.RunOnUI((() => base.Show()));
                }

                return _resultBox != null ? _resultBox.blockedGetOrThrow<T>() : default;
            }

            private class InputCallbacks : Object, IInputCallback
            {
                private readonly Action<string> _action;

                public InputCallbacks(Action<string> action)
                {
                    this._action = action;
                }

                public void OnInput(MaterialDialog p0, ICharSequence p1)
                {
                    _action?.Invoke(p1.ToString());
                }
            }
        }
    }
}