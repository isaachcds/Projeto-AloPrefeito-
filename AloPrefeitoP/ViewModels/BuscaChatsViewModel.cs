using AloPrefeitoP.Models;
using AloPrefeitoP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AloPrefeitoP.ViewModels
{
    public partial class BuscaChatsViewModel : ObservableObject
    {
        private readonly ISQLiteDbServive _db;

        [ObservableProperty]
        private string textoBusca = string.Empty;

        [ObservableProperty]
        private ObservableCollection<ChatBuscaResultado> resultados = new();

        [ObservableProperty]
        private string mensagemVazia = "Digite algo para pesquisar";

        [ObservableProperty]
        private bool isBusy;

        public BuscaChatsViewModel(ISQLiteDbServive db)
        {
            _db = db;
        }

        [RelayCommand]
        private async Task Pesquisar()
        {
            if (IsBusy)
                return;

            var termo = TextoBusca?.Trim();

            if (string.IsNullOrWhiteSpace(termo))
            {
                Resultados.Clear();
                MensagemVazia = "Digite algo para pesquisar";
                return;
            }

            try
            {
                IsBusy = true;

                await _db.InitializeAsync();

                var encontrados = await _db.BuscarChatsPorPalavraChave(termo);

                Resultados.Clear();

                foreach (var item in encontrados)
                    Resultados.Add(item);

                MensagemVazia = Resultados.Count == 0
                    ? "Nenhuma conversa encontrada"
                    : string.Empty;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AbrirResultado(ChatBuscaResultado resultado)
        {
            if (resultado == null)
                return;

            Preferences.Set("chat_atual", resultado.ChatId);
            await Shell.Current.GoToAsync("///Home");
        }

        [RelayCommand]
        private async Task Voltar()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}