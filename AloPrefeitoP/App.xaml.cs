using AloPrefeitoP.Pages;
using AloPrefeitoP.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AloPrefeitoP
{
    public partial class App : Application
    {
        private readonly ApiServices _services;

        public App(ApiServices apiServices)
        {
            InitializeComponent();
            _services = apiServices;

            MainPage = new NavigationPage(new Pages.LoginPage(_services));
        }

        public static void GoToHome()
        {
            Current.MainPage = new AppShell();
        }
    }
}