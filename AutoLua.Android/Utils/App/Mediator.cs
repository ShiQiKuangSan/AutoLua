using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Util;

namespace AutoLua.Droid.Utils.App
{
    public class Mediator : IOnActivityResultDelegate
    {
        private readonly SparseArray<IOnActivityResultDelegate> _specialDelegate =
            new SparseArray<IOnActivityResultDelegate>();

        private readonly List<IOnActivityResultDelegate> _delegates = new List<IOnActivityResultDelegate>();

        public void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            var delegates = _specialDelegate.Get(requestCode);
            delegates?.OnActivityResult(requestCode, resultCode, data);

            foreach (var d in _delegates)
            {
                d.OnActivityResult(requestCode, resultCode, data);
            }
        }

        public void AddDelegate(IOnActivityResultDelegate @delegate)
        {
            _delegates.Add(@delegate);
        }

        public void AddDelegate(int requestCode, IOnActivityResultDelegate @delegate)
        {
            _specialDelegate.Put(requestCode, @delegate);
        }
        
        public void RemoveDelegate(IOnActivityResultDelegate @delegate) {
            if (_delegates.Remove(@delegate)) {
                _specialDelegate.RemoveAt(_specialDelegate.IndexOfValue(@delegate));
            }
        }
    }
}