using AloPrefeitoP.Pages;

namespace AloPrefeitoP
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute("LoginPage", typeof(Pages.LoginPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            //Routing.RegisterRoute("HomePage", typeof(Pages.HomePage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(BuscaChatsPage), typeof(BuscaChatsPage));
            Routing.RegisterRoute(nameof(HistoricoPage), typeof(HistoricoPage));
            Routing.RegisterRoute(nameof(ConfigPage), typeof(ConfigPage));
            Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));
            Routing.RegisterRoute(nameof(AlterarSenhaPage), typeof(AlterarSenhaPage));
        }
    }
}
