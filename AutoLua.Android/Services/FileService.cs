using AutoLua.Droid.Services;
using AutoLua.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileService))]
namespace AutoLua.Droid.Services
{
    public class FileService : IFileService
    {
        public string GetSdCard()
        {
            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        }
    }
}