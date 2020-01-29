using AutoLua.Views.Scripts.Models;
using AutoLua.Views.Scripts.ViewCell;
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

            if (listView != null)
            {
                listView.ItemTemplate = new DataTemplate(() => new ViewCell { View = new ScriptFileViewCell() });
                listView.RowHeight = 60;
            }

            listView.EndRefresh();
        }
    }
}
