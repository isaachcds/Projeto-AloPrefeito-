using AloPrefeitoP.Pages;
using AloPrefeitoP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AloPrefeitoP.ViewModels;

public partial class AlterarSenhaViewModel : ObservableObject
{
    private readonly ApiServices _apiServices;
    private readonly string _email;

    [ObservableProperty]
    private string novaSenha;

    [ObservableProperty]
    private string confirmarSenha;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool novaSenhaVisivel;

    [ObservableProperty]
    private bool confirmarSenhaVisivel;

    public AlterarSenhaViewModel(ApiServices apiServices, string email)
    {
        _apiServices = apiServices;
        _email = email;
    }

    [RelayCommand]
    private void AlternarNovaSenha()
    {
        NovaSenhaVisivel = !NovaSenhaVisivel;
    }

    [RelayCommand]
    private void AlternarConfirmarSenha()
    {
        ConfirmarSenhaVisivel = !ConfirmarSenhaVisivel;
    }

    [RelayCommand]
    private Task Voltar()
    {
        Application.Current!.MainPage = new RecuperarSenhaPage(_apiServices);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task Salvar()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(NovaSenha))
        {
            await Application.Current!.MainPage!.DisplayAlertAsync("Erro", "Informe a nova senha.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(ConfirmarSenha))
        {
            await Application.Current!.MainPage!.DisplayAlertAsync("Erro", "Confirme a senha.", "OK");
            return;
        }

        if (NovaSenha != ConfirmarSenha)
        {
            await Application.Current!.MainPage!.DisplayAlertAsync("Erro", "As senhas não coincidem.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // sua lógica real
            // await _apiServices.AlterarSenha(_email, NovaSenha);

            await Application.Current!.MainPage!.DisplayAlertAsync(
                "Sucesso",
                "Senha alterada com sucesso.",
                "OK");

            Application.Current!.MainPage = new LoginPage(_apiServices);
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlertAsync(
                "Erro",
                $"Falha ao alterar senha: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}