using AloPrefeitoP.Models;
using AloPrefeitoP.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleListUI.Services
{
    public class SQLiteDbServive : ISQLiteDbServive
    {
        private SQLiteAsyncConnection _dbConnection;
        public async Task InitializeAsync()
        {
            await SetUpDb();
        }

        private async Task SetUpDb()
        {
            try
            {
                if (_dbConnection == null)
                {
                    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Prefeito.db");
                    _dbConnection = new SQLiteAsyncConnection(dbPath);
                    await _dbConnection.CreateTableAsync<Mensagens>();



                }
            }
            catch (Exception ex) {

                Console.WriteLine(ex);
            }
        }
        public async Task<int> AddMensagem(Mensagens mensagen)
        {
           return await _dbConnection.InsertAsync(mensagen);
        }

        public async Task<int> DeleteMensagem(Mensagens mensagen)
        {
            return await _dbConnection.DeleteAsync(mensagen);
        }
        public async Task<IEnumerable<Mensagens>> GetMensagem()
        {
            return await _dbConnection.Table<Mensagens>().ToListAsync();
        }

       
    }
}
