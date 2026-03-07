using AloPrefeitoP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AloPrefeitoP.Services
{
    public interface ISQLiteDbServive
    {
        Task InitializeAsync(); 
        Task<IEnumerable<Mensagens>> GetMensagem();
        
        Task<int> AddMensagem(Mensagens mensagen);
        Task<int> DeleteMensagem(Mensagens destaque);
        Task<IEnumerable<Mensagens>> GetMensagensByChatId(string chatId);
        Task<IEnumerable<Mensagens>> GetChatsAgrupados();
        Task<int> DeleteChatByChatId(string chatId);
    }
}
