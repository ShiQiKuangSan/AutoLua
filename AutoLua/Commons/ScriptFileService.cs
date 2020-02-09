using AutoLua.Views.Scripts.Models;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AutoLua.Commons
{
    public class ScriptFileService : ScriptServiceBase
    {
        public ScriptFileService(ListView listView) : base(listView)
        {
        }

        public override void UpdateScripts(ObservableCollection<ScriptFileModel> items)
        {
            listView.IsRefreshing = true;
            
            items.Clear();
            var files = GetFileScript();
            foreach (var item in files)
            {
                items.Add(item);
            }

            listView.EndRefresh();
        }
    }
}
