using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class RecuperarSenhaPage : ContentPage
{
    public RecuperarSenhaPage(ApiServices apiServices)
    {
        InitializeComponent();
        BindingContext = new RecuperarSenhaViewModel(apiServices);
    }
}