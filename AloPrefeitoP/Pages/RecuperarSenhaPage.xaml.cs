using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class RecuperarSenhaPage : ContentPage
{
    public RecuperarSenhaPage(ApiServices apiServices)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = new RecuperarSenhaViewModel(apiServices);
    }
}