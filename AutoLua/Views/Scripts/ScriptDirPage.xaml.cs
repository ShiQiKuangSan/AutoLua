using AutoLua.Commons;
using AutoLua.Services;
using AutoLua.Views.Scripts.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoLua.Views.Scripts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScriptDirPage : ContentPage
    {
        private readonly IToastService toastService = DependencyService.Get<IToastService>();
        private readonly ScriptDirService dirService;

        private readonly ObservableCollection<ScriptFileModel> items = new ObservableCollection<ScriptFileModel>();

        public ScriptDirPage()
        {
            InitializeComponent();
            App.Pages.Add("ScriptDirPage", this);
            dirService = new ScriptDirService(ScriptItems);

            ScriptItems.ItemsSource = items;
            ScriptItems.SelectionMode = ListViewSelectionMode.None;
            ScriptItems.RefreshControlColor = Color.Red;

            ScriptItems.Refreshing += (sender, e) => UpdateScript();
            add.Clicked += PopupMenu;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //刷新目录
            UpdateScript();
        }

        /// <summary>
        /// 弹出菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PopupMenu(object sender, EventArgs e)
        {
            var fileName = await DisplayPromptAsync("名称", null, "确定", "取消");

            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var file = $"{dirService.GetPath()}/{fileName}/";

            if (Directory.Exists(file))
            {
                toastService.ShortAlert("项目已存在");
                return;
            }

            Directory.CreateDirectory(file);

            UpdateScript();
        }

        /// <summary>
        /// 更新文件。
        /// </summary>
        private void UpdateScript()
        {
            //刷新目录
            dirService.UpdateScripts(items);
        }
    }
}