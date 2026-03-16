using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class AlterarSenhaPage : ContentPage
{
    public AlterarSenhaPage(ApiServices apiServices, string email)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = new AlterarSenhaViewModel(apiServices, email);
    }
}