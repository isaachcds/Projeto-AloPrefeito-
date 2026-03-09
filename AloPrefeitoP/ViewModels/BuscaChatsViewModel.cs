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
        private CancellationTokenSource? _debounceCts;

        [ObservableProperty]
        private string textoBusca = string.Empty;

        [ObservableProperty]
        private ObservableCollection<GrupoBuscaResultado> resultadosAgrupados = new();

        [ObservableProperty]
        private string mensagemVazia = "Digite algo para pesquisar";

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool agruparPorConversa = true;

        [ObservableProperty]
        private bool agruparPorData;

        public BuscaChatsViewModel(ISQLiteDbServive db)
        {
            _db = db;
        }

        partial void OnTextoBuscaChanged(string value)
        {
            _debounceCts?.Cancel();
            _debounceCts = new CancellationTokenSource();
            var token = _debounceCts.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(350, token);

                    if (token.IsCancellationRequested)
                        return;

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Pesquisar();
                    });
                }
                catch (TaskCanceledException)
                {
                }
            }, token);
        }

        [RelayCommand]
        private async Task Pesquisar()
        {
            if (IsBusy)
                return;

            var termo = TextoBusca?.Trim();
            var usuarioId = Preferences.Get("usuarioid", 0);

            if (string.IsNullOrWhiteSpace(termo))
            {
                ResultadosAgrupados.Clear();
                MensagemVazia = "Digite algo para pesquisar";
                return;
            }

            try
            {
                IsBusy = true;

                await _db.InitializeAsync();

                var encontrados = await _db.BuscarChatsPorPalavraChave(termo, usuarioId);

                AplicarAgrupamento(encontrados);

                MensagemVazia = ResultadosAgrupados.Count == 0
                    ? "Nenhuma conversa encontrada"
                    : string.Empty;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AgruparConversa()
        {
            AgruparPorConversa = true;
            AgruparPorData = false;
            await Pesquisar();
        }

        [RelayCommand]
        private async Task AgruparData()
        {
            AgruparPorConversa = false;
            AgruparPorData = true;
            await Pesquisar();
        }

        private void AplicarAgrupamento(List<ChatBuscaResultado> encontrados)
        {
            ResultadosAgrupados.Clear();

            IEnumerable<IGrouping<string, ChatBuscaResultado>> grupos;

            if (AgruparPorData)
            {
                grupos = encontrados
                    .GroupBy(x => x.GrupoPorData)
                    .OrderByDescending(g => g.Max(x => x.DataMensagem));
            }
            else
            {
                grupos = encontrados
                    .GroupBy(x => x.GrupoPorConversa)
                    .OrderByDescending(g => g.Max(x => x.DataMensagem));
            }

            foreach (var grupo in grupos)
            {
                ResultadosAgrupados.Add(new GrupoBuscaResultado(grupo.Key, grupo));
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