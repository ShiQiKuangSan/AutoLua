using AutoLua.Views.Scripts.Models;
using AutoLua.Views.Scripts.ViewCell;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace AutoLua.Commons
{
    public class ScriptDirService : ScriptServiceBase
    {
        public ScriptDirService(ListView listView) 
            : base(listView)
        {
        }

        public override void UpdateScripts(ObservableCollection<ScriptFileModel> items)
        {
            listView.IsRefreshing = true;
            items.Clear();

            var files = GetDirScript();

            foreach (var item in files)
            {
                items.Add(item);
            }

            if (listView != null)
            {
                listView.ItemTemplate = new DataTemplate(() => new ViewCell { View = new ScriptDirViewCell() });
                listView.RowHeight = 60;
            }

            listView.EndRefresh();
        }
    }
}
