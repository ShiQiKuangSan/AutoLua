using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoLua.Views.Scripts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScriptHomePage : TabbedPage
    {
        public ScriptHomePage()
        {
            InitializeComponent();
            App.Pages.Add("ScriptHomePage", this);
        }
    }
}