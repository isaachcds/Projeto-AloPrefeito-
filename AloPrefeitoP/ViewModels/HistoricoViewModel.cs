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

                var ultimasPorChat = (await _db.GetChatsAgrupados()).ToList();

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
            Preferences.Remove("chat_atual");
            await Shell.Current.GoToAsync("//Home");
        }

        [RelayCommand]
        private async Task AbrirChat(ChatResumo chat)
        {
            if (chat == null)
                return;

            Preferences.Set("chat_atual", chat.ChatId);
            await Shell.Current.GoToAsync("//Home");
        }

        [RelayCommand]
        private async Task MostrarOpcoes(ChatResumo chat)
        {
            if (chat == null)
                return;

            var acao = await Shell.Current.DisplayActionSheet(
                "Conversa",
                "Cancelar",
                null,
                "Abrir",
                "Excluir");

            switch (acao)
            {
                case "Abrir":
                    await AbrirChat(chat);
                    break;

                case "Excluir":
                    var confirmar = await Shell.Current.DisplayAlert(
                        "Excluir conversa",
                        "Deseja realmente excluir esta conversa?",
                        "Excluir",
                        "Cancelar");

                    if (!confirmar)
                        return;

                    await _db.DeleteChatByChatId(chat.ChatId);

                    // se a conversa excluída era a atual, limpa a preferência
                    var chatAtual = Preferences.Get("chat_atual", "");
                    if (chatAtual == chat.ChatId)
                        Preferences.Remove("chat_atual");

                    await LoadAsync();
                    break;
            }
        }
    }
}