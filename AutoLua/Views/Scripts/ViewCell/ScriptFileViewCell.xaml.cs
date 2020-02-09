using AutoLua.Events;
using AutoLua.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoLua.Views.Scripts.ViewCell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScriptFileViewCell : ContentView
    {
        private readonly IToastService toastService = DependencyService.Get<IToastService>();

        private readonly ILuaScriptService luaScriptService = DependencyService.Get<ILuaScriptService>();

        public ScriptFileViewCell()
        {
            InitializeComponent();
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
                toastService.ShortAlert("发生未知错误");
                return;
            }

            if (luaLb == null)
            {
                toastService.ShortAlert("发生未知错误100");
                return;
            }

            if (string.IsNullOrWhiteSpace(luaLb.Text))
            {
                toastService.ShortAlert("脚本不存在");
                return;
            }

            LogEventDelegates.Instance.OnLog(new LogEventArgs("运行", $"运行 {label.Text ?? string.Empty} 脚本", Color.Blue));

            luaScriptService.RunFile(luaLb.Text);
        }
    }
}