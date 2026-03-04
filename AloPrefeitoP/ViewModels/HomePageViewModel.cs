using AloPrefeitoP.Models;
using AloPrefeitoP.Services;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using System.Globalization;

namespace AloPrefeitoP.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        private readonly ISpeechToText speechToText;
        private CancellationTokenSource? cts;
        private readonly ISQLiteDbServive _iSQLiteDbServive;
        private readonly ApiServices _apiServices;

        [ObservableProperty]
        private string textoFalado = string.Empty;

        [ObservableProperty]
        private bool estaEscutando;

        [ObservableProperty]
        private string mensagemDigitada;

        // Header + input só somem quando EstáEscutando = true
        public bool NaoEstaEscutando => !EstaEscutando;

        // Balão: durante escuta mostra "...", senão mostra a transcrição
        public string TextoTranscricaoUI => EstaEscutando ? "..." : (TextoFalado ?? string.Empty);

        // Balão aparece quando está escutando OU já tem texto transcrito
        public bool TemTranscricao => EstaEscutando || !string.IsNullOrWhiteSpace(TextoFalado);

        public HomePageViewModel(/*ApiServices apiServices, ISQLiteDbServive sQLiteDbServive*/)
        {
            speechToText = SpeechToText.Default;

            speechToText.RecognitionResultUpdated += (_, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // parcial enquanto fala
                    TextoFalado = e.RecognitionResult;
                });
            };

            speechToText.RecognitionResultCompleted += (_, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (e.RecognitionResult.IsSuccessful)
                        TextoFalado = e.RecognitionResult.Text;

                    FinalizarEscutaUI();
                });
            };

            //_apiServices = apiServices;
            //_iSQLiteDbServive = sQLiteDbServive;
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

        [RelayCommand]
        private async Task ToggleEscuta()
        {
            // 2º toque: para
            if (EstaEscutando)
            {
                try
                {
                    await speechToText.StopListenAsync(CancellationToken.None);
                }
                catch
                {
                    // se por algum motivo falhar o stop, garante UI de volta
                    FinalizarEscutaUI();
                }
                return;
            }

            // 1º toque: começa
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
                // não esconde nada se não tiver permissão
                cts.Dispose();
                cts = null;
                return;
            }

            // limpa transcrição anterior (opcional)
            TextoFalado = string.Empty;

            try
            {
                // 👇 agora sim: marca como escutando (a UI some aqui)
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
                // se não conseguiu iniciar, traz UI de volta
                FinalizarEscutaUI();
            }
        }

        private void FinalizarEscutaUI()
        {
            EstaEscutando = false;

            cts?.Dispose();
            cts = null;
        }



        // EXEMPLO DE USO: a cada atualização parcial do texto falado, salva no SQLite e envia para API 

        //  var fala = new Mensagens
        //  {
        //      Nome = Preferences.Get("usuarionome", string.Empty),
        //      Mensagem = TextoFalado,
        //      Data = DateTime.Now

        //  };
        //  int id = Preferences.Get("usuarioid", 0);
        //var response =  await _apiServices.GetRespostaAgentContexto(TextoFalado, id); // ENVIA PARA API A CADA ATUALIZAÇÃO PARCIAL
        //  await _iSQLiteDbServive.InitializeAsync(); // GARANTE QUE O SQLITE ESTEJA PRONTO

        //  await _iSQLiteDbServive.AddMensagem(fala); // SALVA NO SQLITE A CADA ATUALIZAÇÃO PARCIAL
        //  await _iSQLiteDbServive.GetMensagem(); // TRAS TODAS AS MENSAGENS DO SQLITE 
    }
}