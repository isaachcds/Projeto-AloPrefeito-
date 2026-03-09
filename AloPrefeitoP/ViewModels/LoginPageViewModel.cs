using AloPrefeitoP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AloPrefeitoP.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly ApiServices _apiServices;

    [ObservableProperty] private string email;
    [ObservableProperty] private string senha;
    [ObservableProperty] private bool isBusy;

    [ObservableProperty] private bool senhaVisivel;

    public LoginPageViewModel(ApiServices apiServices)
    {
        _apiServices = apiServices;
    }

    [RelayCommand]
    private void AlternarSenha()
    {
        SenhaVisivel = !SenhaVisivel;
    }

    [RelayCommand]
    private async Task Login()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email))
        {
            await Application.Current!.MainPage!.DisplayAlert("Erro", "Informe o email", "Cancelar");
            return;
        }

        if (string.IsNullOrWhiteSpace(Senha))
        {
            await Application.Current!.MainPage!.DisplayAlert("Erro", "Informe a senha", "Cancelar");
            return;
        }

        try
        {
            IsBusy = true;

            var response = await _apiServices.Login(Email, Senha);

            if (!response.HasError)
            {
                // 🔹 CRIA UM NOVO CHAT AO LOGAR
                Preferences.Set("chat_atual", Guid.NewGuid().ToString("N"));

                Application.Current!.MainPage = new AppShell();
                return;
            }

            await Application.Current!.MainPage!.DisplayAlert("Erro", "Senha ou e-mail incorretos", "Cancelar");
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Erro", $"Falha no login: {ex.Message}", "Cancelar");
        }
        finally
        {
            IsBusy = false;
        }
    }
}