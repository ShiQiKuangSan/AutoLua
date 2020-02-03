using System;
using Android.App;
using Android.Content;

namespace AutoLua.Droid.Utils.App
{
    public interface IOnActivityResultDelegate
    {
        void OnActivityResult(int requestCode, Result resultCode, Intent data);

        
    }
}