using AloPrefeitoP.Pages;
using System.Windows.Input;

namespace AloPrefeitoP.ViewModels;

public class LoginPageViewModel
{
    public LoginPageViewModel()
    {
        LoginCommand = new Command(() => App.GoToHome());
    }
    public string Email { get; set; }
    public string Senha { get; set; }

    public ICommand LoginCommand { get; }
}
