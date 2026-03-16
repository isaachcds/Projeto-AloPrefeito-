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
    }

    public async Task InicializarAsync()
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
            await Application.Current!.MainPage!.DisplayAlertAsync("Erro", "Informe o email", "Cancelar");
            return;
        }

        if (string.IsNullOrWhiteSpace(Senha))
        {
            await Application.Current!.MainPage!.DisplayAlertAsync("Erro", "Informe a senha", "Cancelar");
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
                Preferences.Set("chat_atual", Guid.NewGuid().ToString("N"));

                await PerguntarAtivacaoBiometriaAsync();

                Application.Current!.MainPage = new AppShell();
                return;
            }

            await Application.Current!.MainPage!.DisplayAlertAsync("Erro", "Senha ou e-mail incorretos", "Cancelar");
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlertAsync("Erro", $"Falha no login: {ex.Message}", "Cancelar");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PerguntarAtivacaoBiometriaAsync()
    {
        try
        {
            var jaConfigurada = Preferences.Get("biometria_ativada", false);
            if (jaConfigurada)
                return;

            var status = await BiometricAuthenticationService.Default.GetAuthenticationStatusAsync();

            if (status != BiometricHwStatus.Success)
                return;

            var desejaAtivar = await Application.Current!.MainPage!.DisplayAlertAsync(
                "Ativar biometria",
                "Deseja usar biometria nos próximos acessos?",
                "Sim",
                "Agora não");

            if (!desejaAtivar)
            {
                Preferences.Set("biometria_ativada", false);
                return;
            }

            var result = await BiometricAuthenticationService.Default.AuthenticateAsync(
                new AuthenticationRequest
                {
                    Title = "Confirmar ativação da biometria",
                    AllowPasswordAuth = true,
                    NegativeText = "Cancelar"
                },
                CancellationToken.None);

            Preferences.Set("biometria_ativada", result.Status == BiometricResponseStatus.Success);
        }
        catch
        {
            Preferences.Set("biometria_ativada", false);
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
                return;

            var availability = await BiometricAuthenticationService.Default.GetAuthenticationStatusAsync();

            if (availability != BiometricHwStatus.Success)
                return;

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

            await Application.Current!.MainPage!.DisplayAlertAsync(
                "Erro",
                "Não foi possível autenticar com biometria.",
                "OK");
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlertAsync(
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
    private Task IrParaRecuperarSenha()
    {
        Application.Current!.MainPage = new RecuperarSenhaPage(_apiServices);
        return Task.CompletedTask;
    }
}