using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        BindingContext = new LoginPageViewModel();
    }
}