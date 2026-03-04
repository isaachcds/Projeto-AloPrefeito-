using AloPrefeitoP.Models;
using ScheduleListUI.Models;
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
        Task<IEnumerable<Mensagens>> GetDestaques();
        
        Task<int> AddDestaque(Mensagens mensagen);
        Task<int> DeleteDestaque(Mensagens destaque);

       
    }
}
