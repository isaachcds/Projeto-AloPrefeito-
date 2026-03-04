using AloPrefeitoP.Pages;
using System.Windows.Input;

namespace AloPrefeitoP.ViewModels;

public class LoginPageViewModel
{
    public string Email { get; set; }
    public string Senha { get; set; }

    public ICommand LoginCommand { get; }

    public LoginPageViewModel()
    {
        LoginCommand = new Command(async () => await Login());
    }

    private async Task Login()
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}
