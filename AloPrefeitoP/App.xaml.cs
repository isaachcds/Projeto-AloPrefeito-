using AloPrefeitoP.Pages;
using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP
{
    public partial class App : Application
    {
        private readonly ApiServices _services;
        public App(ApiServices apiServices)
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            var vm = new LoginPageViewModel(apiServices);
            var page = new LoginPage(vm);
            MainPage = new NavigationPage(page);
        }
    }
}