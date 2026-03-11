using AloPrefeitoP.Pages;
using AloPrefeitoP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Biometric;

namespace AloPrefeitoP.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    private readonly ApiServices _apiServices;

    [ObservableProperty] private string email;
    [ObservableProperty] private string senha;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool senhaVisivel;
    [ObservableProperty] private bool temBiometria;
    [ObservableProperty] private bool exibirBotaoBiometria;

    public LoginPageViewModel(ApiServices apiServices)
    {
        _apiServices = apiServices;
        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await VerificarBiometriaAsync();
        await TentarLoginAutomaticoComBiometriaAsync();
    }

    private async Task VerificarBiometriaAsync()
    {
        try
        {
            var status = await BiometricAuthenticationService.Default.GetAuthenticationStatusAsync();

            TemBiometria = status == BiometricHwStatus.Success;

            var usuarioSalvo = Preferences.Get("usuario_salvo", false);
            var biometriaAtivada = Preferences.Get("biometria_ativada", false);

            ExibirBotaoBiometria = TemBiometria && usuarioSalvo && biometriaAtivada;
        }
        catch
        {
            TemBiometria = false;
            ExibirBotaoBiometria = false;
        }
    }

    private async Task TentarLoginAutomaticoComBiometriaAsync()
    {
        try
        {
            var usuarioSalvo = Preferences.Get("usuario_salvo", false);
            var biometriaAtivada = Preferences.Get("biometria_ativada", false);
            var emailSalvo = Preferences.Get("usuarioemail", string.Empty);

            if (!usuarioSalvo || !biometriaAtivada || string.IsNullOrWhiteSpace(emailSalvo))
                return;

            var status = await BiometricAuthenticationService.Default.GetAuthenticationStatusAsync();

            if (status != BiometricHwStatus.Success)
                return;

            await LoginBiometric();
        }
        catch
        {
            // evita quebrar a tela caso a biometria automática falhe
        }
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
                Preferences.Set("usuarioemail", Email);
                Preferences.Set("usuario_salvo", true);
                Preferences.Set("biometria_ativada", true); // por enquanto já deixa habilitado
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

    [RelayCommand]
    private async Task LoginBiometric()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var emailSalvo = Preferences.Get("usuarioemail", string.Empty);

            if (string.IsNullOrWhiteSpace(emailSalvo))
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Erro",
                    "Nenhum usuário salvo foi encontrado. Faça login com e-mail e senha primeiro.",
                    "OK");
                return;
            }

            var availability = await BiometricAuthenticationService.Default.GetAuthenticationStatusAsync();

            if (availability != BiometricHwStatus.Success)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Biometria",
                    $"Biometria indisponível no dispositivo.\nStatus: {availability}",
                    "OK");
                return;
            }

            var result = await BiometricAuthenticationService.Default.AuthenticateAsync(
                new AuthenticationRequest
                {
                    Title = "Autenticação Biométrica",
                    AllowPasswordAuth = true,
                    NegativeText = "Cancelar"
                },
                CancellationToken.None);

            if (result.Status != BiometricResponseStatus.Success)
                return;

            var response = await _apiServices.LoginBio(emailSalvo);

            if (!response.HasError)
            {
                Preferences.Set("chat_atual", Guid.NewGuid().ToString("N"));
                Application.Current!.MainPage = new AppShell();
                return;
            }

            await Application.Current!.MainPage!.DisplayAlert(
                "Erro",
                "Não foi possível autenticar com biometria.",
                "OK");
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Erro",
                $"Falha na biometria: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task IrParaRecuperarSenha()
    {
        await Application.Current!.MainPage!.Navigation.PushAsync(
            new RecuperarSenhaPage(_apiServices));
    }
}