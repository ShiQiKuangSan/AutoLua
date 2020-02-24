using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using AutoLua.Droid.Utils;
using NLua.Exceptions;
using Xamarin.Forms;

namespace AutoLua.Droid.LuaScript
{
    [Android.Runtime.Preserve(AllMembers = true)]
    [Activity(Theme = "@style/MainTheme", ExcludeFromRecents = true)]
    public class ScriptExecuteActivity : AppCompatActivity
    {
        /// <summary>
        /// 执行lua脚本
        /// </summary>
        /// <param name="script">lua 脚本</param>
        public static void Execute(string script)
        {
            var context = AppUtils.GetAppContext;
            if (context == null)
            {
                throw new LuaException("AppUtils.GetAppContext = null");
            }

            var intent = new Intent(context, Java.Lang.Class.FromType(typeof(ScriptExecuteActivity)))
                .AddFlags(ActivityFlags.NewTask);

            IntentExtras.NewExtras()
                .Put("luaScript", script)
                .PutInIntent(intent);
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var extras = IntentExtras.FromIntentAndRelease(Intent);
            if (extras == null)
            {
                Finish();
                return;
            }

            var script = extras.Get<string>("luaScript");
            if (string.IsNullOrWhiteSpace(script))
            {
                Finish();
                return;
            }

            //初始化lua全局函数
            LuaGlobal.Instance.Init();

            AppApplication.LuaThread = new Thread(() =>
            {
                try
                {
                    AppApplication.Lua?.DoFile(script);
                }
                catch (LuaException e)
                {
                    AppApplication.OnLog("异常", e.Message, Color.Red);
                }
            });

            AppApplication.LuaThread.Start();
        }

        public override void Finish()
        {
            Close();
            base.Finish();
        }

        private static void Close()
        {
            //初始化lua全局函数
            LuaGlobal.Instance.Close();
        }
    }
}