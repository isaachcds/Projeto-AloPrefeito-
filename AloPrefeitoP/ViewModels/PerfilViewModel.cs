using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;

namespace AloPrefeitoP.ViewModels
{
    public partial class PerfilViewModel : ObservableObject
    {
        [ObservableProperty]
        private string nomeExibicao = "Paulo Souza";

        [ObservableProperty]
        private string email = "seuemail@exemplo.com";

        [ObservableProperty]
        private string telefone = "+55 (71) 99999-0000";

        [ObservableProperty]
        private string endereco = "Rua das Flores, 23, Pituba";

        [ObservableProperty]
        private ImageSource fotoPerfil = "profile.svg";

        public PerfilViewModel()
        {
            CarregarDadosSalvos();
        }

        private void CarregarDadosSalvos()
        {
            NomeExibicao = Preferences.Get("perfil_nome", "Paulo Souza");
            Email = Preferences.Get("perfil_email", "seuemail@exemplo.com");
            Telefone = Preferences.Get("perfil_telefone", "+55 (71) 99999-0000");
            Endereco = Preferences.Get("perfil_endereco", "Rua das Flores, 23, Pituba");

            var caminhoFoto = Preferences.Get("perfil_foto", string.Empty);

            if (!string.IsNullOrWhiteSpace(caminhoFoto) && File.Exists(caminhoFoto))
                FotoPerfil = ImageSource.FromFile(caminhoFoto);
            else
                FotoPerfil = "profile.svg";
        }

        [RelayCommand]
        private async Task Voltar()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task AlterarFoto()
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    // Mesmo que a câmera não seja suportada, a galeria normalmente funciona.
                    // Então seguimos.
                }

                var foto = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selecione uma foto de perfil"
                });

                if (foto == null)
                    return;

                var extensao = Path.GetExtension(foto.FileName);
                var nomeArquivo = $"perfil_usuario{extensao}";
                var caminhoDestino = Path.Combine(FileSystem.AppDataDirectory, nomeArquivo);

                await using var origem = await foto.OpenReadAsync();
                await using var destino = File.Open(caminhoDestino, FileMode.Create, FileAccess.Write);

                await origem.CopyToAsync(destino);

                Preferences.Set("perfil_foto", caminhoDestino);
                FotoPerfil = ImageSource.FromFile(caminhoDestino);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Erro",
                    $"Não foi possível alterar a foto.\n\n{ex.Message}",
                    "OK");
            }
        }

        [RelayCommand]
        private async Task EditarTelefone()
        {
            var novoTelefone = await Shell.Current.DisplayPromptAsync(
                "Editar telefone",
                "Digite o novo telefone:",
                initialValue: Telefone,
                maxLength: 20,
                keyboard: Keyboard.Telephone);

            if (string.IsNullOrWhiteSpace(novoTelefone))
                return;

            Telefone = novoTelefone.Trim();
            Preferences.Set("perfil_telefone", Telefone);
        }

        [RelayCommand]
        private async Task EditarEndereco()
        {
            var novoEndereco = await Shell.Current.DisplayPromptAsync(
                "Editar endereço",
                "Digite o novo endereço:",
                initialValue: Endereco,
                maxLength: 120,
                keyboard: Keyboard.Text);

            if (string.IsNullOrWhiteSpace(novoEndereco))
                return;

            Endereco = novoEndereco.Trim();
            Preferences.Set("perfil_endereco", Endereco);
        }

        [RelayCommand]
        private async Task EditarEmail()
        {
            var novoEmail = await Shell.Current.DisplayPromptAsync(
                "Editar e-mail",
                "Digite o novo e-mail:",
                initialValue: Email,
                maxLength: 80,
                keyboard: Keyboard.Email);

            if (string.IsNullOrWhiteSpace(novoEmail))
                return;

            Email = novoEmail.Trim();
            Preferences.Set("perfil_email", Email);
        }
    }
}