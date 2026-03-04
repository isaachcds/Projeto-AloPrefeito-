using AloPrefeitoP.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace AloPrefeitoP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}