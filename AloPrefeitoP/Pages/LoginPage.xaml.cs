using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}