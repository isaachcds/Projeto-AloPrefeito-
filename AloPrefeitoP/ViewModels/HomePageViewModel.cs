using AloPrefeitoP.Models;
using AloPrefeitoP.Pages;
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
        private CancellationTokenSource? _falaCts;

        private readonly ISQLiteDbServive _db;
        private readonly ApiServices _api;

        private bool _ultimaEntradaFoiPorVoz;
        private bool _iaEstaFalando;
        private List<ChatResumo> _todosChats = new();

        public Action? ScrollToBottomRequested;
        public Action<bool>? MenuStateChanged;

        [ObservableProperty]
        private string textoFalado = string.Empty;

        [ObservableProperty]
        private bool estaEscutando;

        [ObservableProperty]
        private string mensagemDigitada = string.Empty;

        [ObservableProperty]
        private bool iaEstaDigitando;

        [ObservableProperty]
        private bool fala;

        [ObservableProperty]
        private bool menuAberto;

        [ObservableProperty]
        private bool isBusyHistorico;

        [ObservableProperty]
        private ObservableCollection<Mensagens> listaMensagens = new();

        [ObservableProperty]
        private ObservableCollection<ChatResumo> chats = new();

        public string Nome => Preferences.Get("usuarionome", string.Empty);

        public bool NaoEstaEscutando => !EstaEscutando;

        public bool TemMensagens => ListaMensagens?.Count > 0;

        public bool TemMaisDeTresChats => _todosChats.Count > 3;

        public HomePageViewModel(ApiServices apiServices, ISQLiteDbServive sQLiteDbServive)
        {
            _api = apiServices;
            _db = sQLiteDbServive;

            speechToText = SpeechToText.Default;

            Fala = Preferences.Get("fala_ativa", true);

            ListaMensagens.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(TemMensagens));
            };

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

                    MainThread.BeginInvokeOnMainThread(FinalizarEscutaUI);

                    if (string.IsNullOrWhiteSpace(textoFinal))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            TextoFalado = string.Empty;
                        });
                        return;
                    }

                    _ultimaEntradaFoiPorVoz = true;

                    await EnviarMensagemFinalAsync(textoFinal);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TextoFalado = string.Empty;
                    });
                }
                catch
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        FinalizarEscutaUI();
                        TextoFalado = string.Empty;
                    });
                }
            };
        }

        partial void OnEstaEscutandoChanged(bool value)
        {
            OnPropertyChanged(nameof(NaoEstaEscutando));
        }

        partial void OnFalaChanged(bool value)
        {
            Preferences.Set("fala_ativa", value);

            if (!value && _iaEstaFalando)
            {
                try
                {
                    _falaCts?.Cancel();
                }
                catch
                {
                }
            }
        }

        public async Task LoadChatAtualAsync()
        {
            await _db.InitializeAsync();

            var chatId = Preferences.Get("chat_atual", "");
            var usuarioId = Preferences.Get("usuarioid", 0);

            if (string.IsNullOrWhiteSpace(chatId))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListaMensagens.Clear();
                    ScrollToBottomRequested?.Invoke();
                });
                return;
            }

            var msgs = await _db.GetMensagensByChatId(chatId, usuarioId);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaMensagens.Clear();

                foreach (var m in msgs)
                    ListaMensagens.Add(m);

                ScrollToBottomRequested?.Invoke();
            });
        }

        public async Task LoadHistoricoAsync()
        {
            if (IsBusyHistorico)
                return;

            try
            {
                IsBusyHistorico = true;

                await _db.InitializeAsync();

                var usuarioId = Preferences.Get("usuarioid", 0);
                var ultimasPorChat = (await _db.GetChatsAgrupados(usuarioId)).ToList();

                var lista = new List<ChatResumo>();

                foreach (var ultima in ultimasPorChat)
                {
                    var mensagens = (await _db.GetMensagensByChatId(ultima.ChatId, usuarioId)).ToList();

                    if (mensagens.Count == 0)
                        continue;

                    var primeiraUser = mensagens
                        .Where(m => !m.IsBot)
                        .OrderBy(m => m.Data)
                        .FirstOrDefault();

                    var tituloBase = primeiraUser?.Mensagem ?? "Conversa";
                    var titulo = tituloBase.Length > 38
                        ? tituloBase.Substring(0, 38) + "..."
                        : tituloBase;

                    lista.Add(new ChatResumo
                    {
                        ChatId = ultima.ChatId,
                        Titulo = titulo,
                        UltimaData = mensagens.Max(m => m.Data)
                    });
                }

                _todosChats = lista
                    .OrderByDescending(x => x.UltimaData)
                    .ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Chats.Clear();

                    foreach (var c in _todosChats.Take(3))
                        Chats.Add(c);

                    OnPropertyChanged(nameof(TemMaisDeTresChats));
                });
            }
            finally
            {
                IsBusyHistorico = false;
            }
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

            var msgUser = new Mensagens
            {
                Nome = nomeUser,
                Mensagem = textoUsuario,
                Data = DateTime.Now,
                ChatId = chatId,
                IsBot = false,
                UsuarioId = userId
            };

            await _db.AddMensagem(msgUser);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaMensagens.Add(msgUser);
                ScrollToBottomRequested?.Invoke();
            });

            await LoadHistoricoAsync();

            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IaEstaDigitando = true;
                    ScrollToBottomRequested?.Invoke();
                });

                var resposta = await _api.GetRespostaAgentContexto(textoUsuario, userId);

                if (string.IsNullOrWhiteSpace(resposta))
                    resposta = "Não consegui responder agora. Tente novamente.";

                if (Fala && _ultimaEntradaFoiPorVoz)
                {
                    IEnumerable<Locale> locales = await TextToSpeech.Default.GetLocalesAsync();

                    Locale ptBR = locales.FirstOrDefault(l =>
                        l.Language.Equals("pt", StringComparison.OrdinalIgnoreCase) &&
                        l.Country.Equals("BR", StringComparison.OrdinalIgnoreCase))
                        ?? locales.FirstOrDefault();

                    SpeechOptions options = new SpeechOptions
                    {
                        Pitch = 1.0f,
                        Volume = 1.0f,
                        Rate = 0.9f,
                        Locale = ptBR
                    };

                    _falaCts?.Dispose();
                    _falaCts = new CancellationTokenSource();

                    try
                    {
                        _iaEstaFalando = true;
                        await TextToSpeech.Default.SpeakAsync(resposta, options, _falaCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    finally
                    {
                        _iaEstaFalando = false;
                    }
                }

                var msgBot = new Mensagens
                {
                    Nome = "Alô Prefeito",
                    Mensagem = resposta,
                    Data = DateTime.Now,
                    ChatId = chatId,
                    IsBot = true,
                    UsuarioId = userId
                };

                await _db.AddMensagem(msgBot);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListaMensagens.Add(msgBot);
                    ScrollToBottomRequested?.Invoke();
                });

                await LoadHistoricoAsync();
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IaEstaDigitando = false;
                });
            }
        }

        [RelayCommand]
        private void AlternarMenu()
        {
            if (MenuAberto)
                FecharMenu();
            else
                AbrirMenu();
        }

        [RelayCommand]
        private async Task AbrirMenu()
        {
            MenuAberto = true;
            await LoadHistoricoAsync();
            MenuStateChanged?.Invoke(true);
        }

        [RelayCommand]
        public void FecharMenu()
        {
            MenuAberto = false;
            MenuStateChanged?.Invoke(false);
        }

        [RelayCommand]
        private async Task NovoChat()
        {
            Preferences.Set("chat_atual", Guid.NewGuid().ToString("N"));

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaMensagens.Clear();
                OnPropertyChanged(nameof(TemMensagens));
                ScrollToBottomRequested?.Invoke();
            });

            await LoadHistoricoAsync();
            FecharMenu();
        }

        [RelayCommand]
        private async Task AbrirChat(ChatResumo chat)
        {
            if (chat == null)
                return;

            Preferences.Set("chat_atual", chat.ChatId);

            await LoadChatAtualAsync();
            await LoadHistoricoAsync();

            FecharMenu();
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

                    await _db.DeleteChatByChatId(chat.ChatId, Preferences.Get("usuarioid", 0));

                    var chatAtual = Preferences.Get("chat_atual", "");
                    if (chatAtual == chat.ChatId)
                    {
                        Preferences.Remove("chat_atual");

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ListaMensagens.Clear();
                            OnPropertyChanged(nameof(TemMensagens));
                            ScrollToBottomRequested?.Invoke();
                        });
                    }

                    await LoadHistoricoAsync();
                    break;
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
                    TextoFalado = string.Empty;
                }
                return;
            }

            if (_iaEstaFalando)
            {
                try
                {
                    _falaCts?.Cancel();
                }
                catch
                {
                }
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
                TextoFalado = string.Empty;
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

            _ultimaEntradaFoiPorVoz = false;

            await EnviarMensagemFinalAsync(texto);
        }

        [RelayCommand]
        private async Task IrParaBusca()
        {
            FecharMenu();
            await Shell.Current.GoToAsync(nameof(BuscaChatsPage));
        }

        [RelayCommand]
        private async Task IrParaHistorico()
        {
            FecharMenu();
            await Shell.Current.GoToAsync(nameof(HistoricoPage));
        }

        [RelayCommand]
        private async Task AbrirConfiguracoes()
        {
            FecharMenu();
            await Shell.Current.GoToAsync(nameof(ConfigPage));
        }

        [RelayCommand]
        private async Task ConfirmarLogoff()
        {
            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Sair",
                "Deseja realmente sair da sua conta?",
                "Sim",
                "Cancelar");

            if (!confirmar)
                return;

            FecharMenu();
            Preferences.Remove("chat_atual");

            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}