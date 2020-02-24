using AutoLua.Commons;
using AutoLua.Events;
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
    public partial class ScriptFilePage : ContentPage
    {
        private readonly IToastService _toastService = DependencyService.Get<IToastService>();
        private readonly ILuaScriptService _luaScriptService = DependencyService.Get<ILuaScriptService>();

        private readonly ScriptFileService _fileService;

        private readonly ObservableCollection<ScriptFileModel> _items = new ObservableCollection<ScriptFileModel>();
        public ScriptFilePage()
        {
            InitializeComponent();
            _fileService = new ScriptFileService(ScriptItems);

            ScriptItems.RowHeight = 60;
            ScriptItems.ItemsSource = _items;
            ScriptItems.SelectionMode = ListViewSelectionMode.None;
            ScriptItems.RefreshControlColor = Color.Red;

            ScriptItems.Refreshing +=  (sender, e) => UpdateScript();
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


            var file = $"{_fileService.GetPath()}/{fileName}.lua";

            if (File.Exists(file))
            {
                _toastService.ShortAlert("文件已存在");
                return;
            }

            File.Create(file);

            //刷新目录
            UpdateScript();
        }

        /// <summary>
        /// 更新文件。
        /// </summary>
        private void UpdateScript()
        {
            //刷新目录
            _fileService.UpdateScripts(_items);
        }

        private void run_script(object sender, System.EventArgs e)
        {
            if (!(sender is ImageButton imgBut))
                return;

            var stack = imgBut.Parent;
            var grid = stack?.Parent;

            var label = grid?.FindByName<Label>("ScripeName");

            var luaLb = stack?.FindByName<Label>("LPath");

            if (label == null)
            {
                _toastService.ShortAlert("发生未知错误");
                return;
            }

            if (luaLb == null)
            {
                _toastService.ShortAlert("发生未知错误100");
                return;
            }

            if (string.IsNullOrWhiteSpace(luaLb.Text))
            {
                _toastService.ShortAlert("脚本不存在");
                return;
            }

            LogEventDelegates.Instance.OnLog(new LogEventArgs("运行", $"运行 {label.Text ?? string.Empty} 脚本", Color.Blue));

            _luaScriptService.RunFile(luaLb.Text);
        }
    }
}