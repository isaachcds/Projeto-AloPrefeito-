using AloPrefeitoP.Models;

namespace AloPrefeitoP.Services
{
    public interface ISQLiteDbServive
    {
        Task InitializeAsync();
        Task<IEnumerable<Mensagens>> GetMensagem(int usuarioId);
        Task<int> AddMensagem(Mensagens mensagen);
        Task<int> DeleteMensagem(Mensagens destaque);
        Task<IEnumerable<Mensagens>> GetMensagensByChatId(string chatId, int usuarioId);
        Task<IEnumerable<Mensagens>> GetChatsAgrupados(int usuarioId);
        Task<int> DeleteChatByChatId(string chatId, int usuarioId);
        Task<List<ChatBuscaResultado>> BuscarChatsPorPalavraChave(string termo, int usuarioId);
    }
}