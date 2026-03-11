using AloPrefeitoP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AloPrefeitoP.ViewModels;

public partial class RecuperarSenhaViewModel : ObservableObject
{
    private readonly ApiServices _apiServices;

    [ObservableProperty] private string email;
    [ObservableProperty] private bool isBusy;

    public RecuperarSenhaViewModel(ApiServices apiServices)
    {
        _apiServices = apiServices;
    }

    //[RelayCommand]
    //private async Task EnviarRecuperacao()
    //{
    //    if (IsBusy) return;

    //    if (string.IsNullOrWhiteSpace(Email))
    //    {
    //        await Application.Current!.MainPage!.DisplayAlert(
    //            "Erro",
    //            "Informe o e-mail.",
    //            "OK");
    //        return;
    //    }

    //    try
    //    {
    //        IsBusy = true;

    //        // Ajustar para seu endpoint real
    //        await _apiServices.RecuperarSenha(Email);

    //        await Application.Current!.MainPage!.DisplayAlert(
    //            "Sucesso",
    //            "Se o e-mail estiver cadastrado, você receberá as instruções de recuperação.",
    //            "OK");

    //        await Application.Current!.MainPage!.Navigation.PopAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        await Application.Current!.MainPage!.DisplayAlert(
    //            "Erro",
    //            $"Falha ao solicitar recuperação: {ex.Message}",
    //            "OK");
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}

    //[RelayCommand]
    //private async Task Voltar()
    //{
    //    await Application.Current!.MainPage!.Navigation.PopAsync();
    //}
}