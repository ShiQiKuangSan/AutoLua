using Android.OS;
using Android.Widget;
using AutoLua.Droid.Services;
using AutoLua.Services;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(ToastService))]
namespace AutoLua.Droid.Services
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class ToastService : IToastService
    {
        public void LongAlert(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}