using AloPrefeitoP.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AloPrefeitoP.ViewModels
{
    public partial class ConfigViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task Voltar()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task AlterarSenha()
        {
            await Shell.Current.DisplayAlert(
                "Alterar senha",
                "Tela de alteração de senha será implementada em breve.",
                "OK");
        }

        [RelayCommand]
        private async Task Sobre()
        {
            await Shell.Current.DisplayAlert(
                "Sobre",
                "Alô Prefeito - versão 1.0",
                "OK");
        }

        [RelayCommand]
        private async Task Suporte()
        {
            await Shell.Current.DisplayAlert(
                "Suporte",
                "Nosso canal de suporte será disponibilizado em breve.",
                "OK");
        }

        [RelayCommand]
        private async Task Sair()
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Sair da conta",
                "Deseja realmente sair da sua conta?",
                "Sim",
                "Cancelar");

            if (!confirmar)
                return;

            Preferences.Remove("chat_atual");

            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}