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
            _services = apiServices;
            Application.Current.UserAppTheme = AppTheme.Light;
            var vm = new LoginPageViewModel(apiServices);
            var page = new LoginPage( _services);
            NavigationPage.SetHasNavigationBar(page, false);
            MainPage = new NavigationPage(page);
        }
    }
}