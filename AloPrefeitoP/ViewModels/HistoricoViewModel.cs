using AloPrefeitoP.Models;
using AloPrefeitoP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AloPrefeitoP.ViewModels
{
    public partial class HistoricoViewModel : ObservableObject
    {
        private readonly ISQLiteDbServive _db;

        [ObservableProperty]
        private ObservableCollection<ChatResumo> chats = new();

        [ObservableProperty]
        private bool isBusy;

        public HistoricoViewModel(ISQLiteDbServive db)
        {
            _db = db;
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                await _db.InitializeAsync();

                // pega 1 registro representando cada chat (mais recente)
                var ultimasPorChat = (await _db.GetChatsAgrupados()).ToList();

                // para título, vamos pegar a primeira msg do usuário de cada chat
                // (mais simples: buscar todas do chat e pegar a 1ª não-bot)
                var lista = new List<ChatResumo>();

                foreach (var ultima in ultimasPorChat)
                {
                    var mensagens = (await _db.GetMensagensByChatId(ultima.ChatId)).ToList();

                    var primeiraUser = mensagens
                        .Where(m => !m.IsBot)
                        .OrderBy(m => m.Data)
                        .FirstOrDefault();

                    var tituloBase = primeiraUser?.Mensagem ?? "Conversa";
                    var titulo = tituloBase.Length > 38 ? tituloBase.Substring(0, 38) + "..." : tituloBase;

                    lista.Add(new ChatResumo
                    {
                        ChatId = ultima.ChatId,
                        Titulo = titulo,
                        UltimaData = mensagens.Max(m => m.Data)
                    });
                }

                // ordena por última atividade
                lista = lista.OrderByDescending(x => x.UltimaData).ToList();

                Chats.Clear();
                foreach (var c in lista)
                    Chats.Add(c);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NovoChat()
        {
            // limpa chat atual -> HomePage inicia vazia (Modo 2 cria na 1ª fala)
            Preferences.Remove("chat_atual");

            // navega para Home
            await Shell.Current.GoToAsync("//Home");
        }

        [RelayCommand]
        private async Task AbrirChat(ChatResumo chat)
        {
            if (chat == null) return;

            // define o chat atual e volta pra Home
            Preferences.Set("chat_atual", chat.ChatId);
            await Shell.Current.GoToAsync("//Home");
        }
    }
}