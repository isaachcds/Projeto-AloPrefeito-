using AloPrefeitoP.Pages;

namespace AloPrefeitoP
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("LoginPage", typeof(Pages.LoginPage));
            Routing.RegisterRoute("HomePage", typeof(Pages.HomePage));
            Routing.RegisterRoute(nameof(BuscaChatsPage), typeof(BuscaChatsPage));
        }
    }
}
