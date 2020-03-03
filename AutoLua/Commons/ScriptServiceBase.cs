using AutoLua.Services;
using AutoLua.Views.Scripts.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace AutoLua.Commons
{
    public abstract class ScriptServiceBase
    {
        public ListView listView { set; protected get; }

        protected readonly string path;

        private readonly IFileService fileService = DependencyService.Get<IFileService>();

        protected ScriptServiceBase(ListView listView)
        {
            this.listView = listView;
            path = fileService.GetSdCard() + "/scriptlua/";
            if (!Directory.Exists(path))
            {
                //Directory.CreateDirectory(path);
            }
        }

        protected IList<ScriptFileModel> GetDirScript()
        {
            var items = new List<ScriptFileModel>();
            var dir = new DirectoryInfo(path);

            //添加文件夹
            //foreach (var item in dir.GetDirectories())
            //{
            //    var model = new ScriptFileModel
            //    {
            //        Name = item.Name,
            //        Path = item.FullName,
            //        IsDir = true
            //    };

            //    items.Add(model);
            //}

            return items;
        }

        protected IList<ScriptFileModel> GetFileScript()
        {
            var items = new List<ScriptFileModel>();
            var dir = new DirectoryInfo(path);
            //var files = dir.GetFiles();

            ////添加文件夹
            //foreach (var item in files.Where(x => x.Extension == ".lua" || x.Extension == ".luac"))
            //{
            //    var model = new ScriptFileModel
            //    {
            //        Name = item.Name,
            //        Path = item.FullName,
            //        IsDir = false
            //    };

            //    items.Add(model);
            //}

            return items;
        }

        public abstract void UpdateScripts(ObservableCollection<ScriptFileModel> items);

        public string GetPath()
        {
            return path;
        }
    }
}
