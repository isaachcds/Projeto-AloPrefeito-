using AloPrefeitoP.Pages;
using AloPrefeitoP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AloPrefeitoP.ViewModels;

public partial class RecuperarSenhaViewModel : ObservableObject
{
    private readonly ApiServices _apiServices;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private bool isBusy;

    public RecuperarSenhaViewModel(ApiServices apiServices)
    {
        _apiServices = apiServices;
    }

    [RelayCommand]
    private Task Cancelar()
    {
        Application.Current!.MainPage = new LoginPage(_apiServices);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task EnviarCodigo()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(Email))
        {
            await Application.Current!.MainPage!.DisplayAlertAsync("Erro", "Informe o email.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // sua lógica real aqui
            // await _apiServices.RecuperarSenha(Email);

            Application.Current!.MainPage = new AlterarSenhaPage(_apiServices, Email);
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlertAsync(
                "Erro",
                $"Falha ao enviar código: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
