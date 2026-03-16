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
    private async Task EnviarCodigo()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email))
        {
            await Application.Current!.MainPage!.DisplayAlertAsync(
                "Erro",
                "Informe o e-mail.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var response = await _apiServices.ResetPassWord(Email);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                await Application.Current!.MainPage!.DisplayAlertAsync(
                    "Erro",
                    response.ErrorMessage,
                    "OK");
                return;
            }

            await Application.Current!.MainPage!.DisplayAlertAsync(
                "Sucesso",
                "Se o e-mail estiver cadastrado, o processo de redefinição foi iniciado.",
                "OK");

            await Application.Current!.MainPage!.Navigation.PushAsync(
                new AlterarSenhaPage(_apiServices, Email));
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlertAsync(
                "Erro",
                $"Falha ao solicitar recuperação: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancelar()
    {
        await Application.Current!.MainPage!.Navigation.PopAsync();
    }
}