using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoLua.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingHomePage : ContentPage
    {
        public SettingHomePage()
        {
            InitializeComponent();
            App.Pages.Add("SettingHomePage", this);
        }
    }
}