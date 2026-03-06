using AloPrefeitoP.Models;
using AloPrefeitoP.Services;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using System.Collections.ObjectModel;
using System.Globalization;

namespace AloPrefeitoP.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        private readonly ISpeechToText speechToText;
        private CancellationTokenSource? cts;
        public Action? ScrollToBottomRequested;
        private readonly ISQLiteDbServive _db;
        private readonly ApiServices _api;

        [ObservableProperty] private string textoFalado = string.Empty;
        [ObservableProperty] private bool estaEscutando;
        [ObservableProperty] private string mensagemDigitada = string.Empty;
        [ObservableProperty]
        private bool iaEstaDigitando;
        [ObservableProperty] private ObservableCollection<Mensagens> listaMensagens = new();

        public string Nome => Preferences.Get("usuarionome", string.Empty);

        // Header + input só somem quando EstáEscutando = true
        public bool NaoEstaEscutando => !EstaEscutando;

        // Balão: durante escuta mostra "...", senão mostra a transcrição
        public string TextoTranscricaoUI => EstaEscutando ? "..." : (TextoFalado ?? string.Empty);

        // Balão aparece quando está escutando OU já tem texto transcrito
        public bool TemTranscricao => EstaEscutando || !string.IsNullOrWhiteSpace(TextoFalado);

        public bool TemMensagens => ListaMensagens?.Count > 0;
        public HomePageViewModel(ApiServices apiServices, ISQLiteDbServive sQLiteDbServive)
        {
            _api = apiServices;
            _db = sQLiteDbServive;

            speechToText = SpeechToText.Default;

            speechToText.RecognitionResultUpdated += (_, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TextoFalado = e.RecognitionResult;
                });
            };

            speechToText.RecognitionResultCompleted += async (_, e) =>
            {
                try
                {
                    var textoFinal = e.RecognitionResult.IsSuccessful
                        ? e.RecognitionResult.Text
                        : TextoFalado;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TextoFalado = textoFinal ?? "";
                        FinalizarEscutaUI();
                    });

                    if (string.IsNullOrWhiteSpace(textoFinal))
                        return;

                    await EnviarMensagemFinalAsync(textoFinal);
                }
                catch
                {
                    MainThread.BeginInvokeOnMainThread(FinalizarEscutaUI);
                }
            };

            ListaMensagens.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(TemMensagens));
            };
        }

        partial void OnEstaEscutandoChanged(bool value)
        {
            OnPropertyChanged(nameof(NaoEstaEscutando));
            OnPropertyChanged(nameof(TextoTranscricaoUI));
            OnPropertyChanged(nameof(TemTranscricao));
        }

        partial void OnTextoFaladoChanged(string value)
        {
            OnPropertyChanged(nameof(TextoTranscricaoUI));
            OnPropertyChanged(nameof(TemTranscricao));
        }

        // ✅ Carrega o chat atual (quando abre Home)
        public async Task LoadChatAtualAsync()
        {
            await _db.InitializeAsync();

            var chatId = Preferences.Get("chat_atual", "");

            // Sem chat ainda = Home vazia (Modo 2 cria na 1ª fala)
            if (string.IsNullOrWhiteSpace(chatId))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListaMensagens.Clear();
                });
                return;
            }

            var msgs = await _db.GetMensagensByChatId(chatId);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaMensagens.Clear();
                foreach (var m in msgs)
                    ListaMensagens.Add(m);
            });
        }

        private string GetOrCreateChatAtual()
        {
            var chatId = Preferences.Get("chat_atual", "");

            if (string.IsNullOrWhiteSpace(chatId))
            {
                chatId = Guid.NewGuid().ToString("N");
                Preferences.Set("chat_atual", chatId);
            }

            return chatId;
        }

        private async Task EnviarMensagemFinalAsync(string textoUsuario)
        {
            await _db.InitializeAsync();

            var chatId = GetOrCreateChatAtual();
            var userId = Preferences.Get("usuarioid", 0);
            var nomeUser = Preferences.Get("usuarionome", string.Empty);

            // 1) salva msg do usuário
            var msgUser = new Mensagens
            {
                Nome = nomeUser,
                Mensagem = textoUsuario,
                Data = DateTime.Now,
                ChatId = chatId,
                IsBot = false
            };

            await _db.AddMensagem(msgUser);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaMensagens.Add(msgUser);
                ScrollToBottomRequested?.Invoke(); // ✅ aqui entra o scroll
            });

            try
            {
                IaEstaDigitando = true; // ✅ mostra indicador

                // 2) chama IA
                var resposta = await _api.GetRespostaAgentContexto(textoUsuario, userId);
                if (string.IsNullOrWhiteSpace(resposta))
                    resposta = "Não consegui responder agora. Tente novamente.";

                // 3) salva msg do bot
                var msgBot = new Mensagens
                {
                    Nome = "Alô Prefeito",
                    Mensagem = resposta,
                    Data = DateTime.Now,
                    ChatId = chatId,
                    IsBot = true
                };

                await _db.AddMensagem(msgBot);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListaMensagens.Add(msgBot);
                    TextoFalado = "";
                    ScrollToBottomRequested?.Invoke(); // ✅ e aqui também
                });
            }
            finally
            {
                IaEstaDigitando = false; // ✅ some indicador
            }
        }

        [RelayCommand]
        private async Task ToggleEscuta()
        {
            if (EstaEscutando)
            {
                try
                {
                    await speechToText.StopListenAsync(CancellationToken.None);
                }
                catch
                {
                    FinalizarEscutaUI();
                }
                return;
            }

            cts?.Dispose();
            cts = new CancellationTokenSource();

            bool granted;
            try
            {
                granted = await speechToText.RequestPermissions(cts.Token);
            }
            catch
            {
                granted = false;
            }

            if (!granted)
            {
                cts.Dispose();
                cts = null;
                return;
            }

            TextoFalado = string.Empty;

            try
            {
                EstaEscutando = true;

                await speechToText.StartListenAsync(
                    new SpeechToTextOptions
                    {
                        Culture = CultureInfo.GetCultureInfo("pt-BR"),
                        ShouldReportPartialResults = true
                    },
                    cts.Token);
            }
            catch
            {
                FinalizarEscutaUI();
            }
        }

        private void FinalizarEscutaUI()
        {
            EstaEscutando = false;

            cts?.Dispose();
            cts = null;
        }


        [RelayCommand]
        private async Task EnviarTexto()
        {
            if (string.IsNullOrWhiteSpace(MensagemDigitada))
                return;

            var texto = MensagemDigitada.Trim();

            MensagemDigitada = string.Empty;

            await EnviarMensagemFinalAsync(texto);
        }
    }
}