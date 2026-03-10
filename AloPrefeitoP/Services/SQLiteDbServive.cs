using AloPrefeitoP.Models;
using AloPrefeitoP.Services;
using SQLite;

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
                    string dbPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "Prefeito.db");

                    _dbConnection = new SQLiteAsyncConnection(dbPath);

                    await _dbConnection.CreateTableAsync<Mensagens>();

                    await EnsureColumnExistsAsync("Mensagens", "ChatId", "TEXT", "''");
                    await EnsureColumnExistsAsync("Mensagens", "IsBot", "INTEGER", "0");
                    await EnsureColumnExistsAsync("Mensagens", "UsuarioId", "INTEGER", "0");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao configurar banco: {ex.Message}");
            }
        }

        private async Task EnsureColumnExistsAsync(
            string tableName,
            string columnName,
            string columnType,
            string defaultValueSql)
        {
            var columns = await _dbConnection.QueryAsync<TableInfoRow>($"PRAGMA table_info({tableName});");

            var columnExists = columns.Any(c =>
                string.Equals(c.name, columnName, StringComparison.OrdinalIgnoreCase));

            if (columnExists)
                return;

            string sql = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType} DEFAULT {defaultValueSql};";
            await _dbConnection.ExecuteAsync(sql);
        }

        private class TableInfoRow
        {
            public int cid { get; set; }
            public string name { get; set; } = string.Empty;
            public string type { get; set; } = string.Empty;
            public int notnull { get; set; }
            public string dflt_value { get; set; } = string.Empty;
            public int pk { get; set; }
        }

        public async Task<int> AddMensagem(Mensagens mensagen)
        {
            await InitializeAsync();
            return await _dbConnection.InsertAsync(mensagen);
        }

        public async Task<int> DeleteMensagem(Mensagens mensagen)
        {
            await InitializeAsync();
            return await _dbConnection.DeleteAsync(mensagen);
        }

        public async Task<IEnumerable<Mensagens>> GetMensagem(int usuarioId)
        {
            await InitializeAsync();

            return await _dbConnection.Table<Mensagens>()
                                      .Where(x => x.UsuarioId == usuarioId)
                                      .OrderBy(x => x.Data)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Mensagens>> GetMensagensByChatId(string chatId, int usuarioId)
        {
            await InitializeAsync();

            return await _dbConnection.Table<Mensagens>()
                                      .Where(x => x.ChatId == chatId && x.UsuarioId == usuarioId)
                                      .OrderBy(x => x.Data)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Mensagens>> GetChatsAgrupados(int usuarioId)
        {
            await InitializeAsync();

            var todas = await _dbConnection.Table<Mensagens>()
                                           .Where(x => x.UsuarioId == usuarioId)
                                           .OrderByDescending(x => x.Data)
                                           .ToListAsync();

            return todas.GroupBy(x => x.ChatId)
                        .Select(g => g.OrderByDescending(x => x.Data).First())
                        .OrderByDescending(x => x.Data)
                        .ToList();
        }

        public async Task<int> DeleteChatByChatId(string chatId, int usuarioId)
        {
            await InitializeAsync();

            return await _dbConnection.ExecuteAsync(
                "DELETE FROM Mensagens WHERE ChatId = ? AND UsuarioId = ?",
                chatId,
                usuarioId);
        }

        public async Task<List<ChatBuscaResultado>> BuscarChatsPorPalavraChave(string termo, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return new List<ChatBuscaResultado>();

            await InitializeAsync();

            var termoLower = termo.Trim().ToLowerInvariant();

            var todasMensagens = await _dbConnection.Table<Mensagens>()
                                                    .Where(x => x.UsuarioId == usuarioId)
                                                    .ToListAsync();

            var mensagensEncontradas = todasMensagens
                .Where(m => !string.IsNullOrWhiteSpace(m.Mensagem) &&
                            m.Mensagem.ToLowerInvariant().Contains(termoLower))
                .OrderByDescending(m => m.Data)
                .ToList();

            var resultados = new List<ChatBuscaResultado>();

            foreach (var mensagemEncontrada in mensagensEncontradas)
            {
                var chatId = mensagemEncontrada.ChatId;

                var mensagensDoChat = todasMensagens
                    .Where(m => m.ChatId == chatId)
                    .OrderBy(m => m.Data)
                    .ToList();

                if (!mensagensDoChat.Any())
                    continue;

                var primeiraUser = mensagensDoChat
                    .Where(m => !m.IsBot)
                    .FirstOrDefault();

                var tituloBase = primeiraUser?.Mensagem ?? "Conversa";
                var titulo = tituloBase.Length > 38
                    ? tituloBase.Substring(0, 38) + "..."
                    : tituloBase;

                var trecho = mensagemEncontrada.Mensagem ?? string.Empty;

                if (trecho.Length > 120)
                    trecho = trecho.Substring(0, 120) + "...";

                resultados.Add(new ChatBuscaResultado
                {
                    ChatId = chatId,
                    Titulo = titulo,
                    Trecho = trecho,
                    DataMensagem = mensagemEncontrada.Data,
                    UltimaDataChat = mensagensDoChat.Max(m => m.Data),
                    IsBot = mensagemEncontrada.IsBot
                });
            }

            return resultados
                .OrderByDescending(x => x.DataMensagem)
                .ToList();
        }
    }
}