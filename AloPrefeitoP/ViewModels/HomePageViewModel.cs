using AloPrefeitoP.Models;
using AloPrefeitoP.Services;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private string textoFalado;

        [ObservableProperty]
        private bool estaEscutando;

        public HomePageViewModel(ISQLiteDbServive iSQLiteDbServive, ApiServices apiServices)
        {
            // pode usar direto sem DI:
            speechToText = SpeechToText.Default;

            speechToText.RecognitionResultUpdated += OnUpdated;
            speechToText.RecognitionResultCompleted += OnCompleted;
            _iSQLiteDbServive = iSQLiteDbServive;
            _apiServices = apiServices;
        }

        private async void OnUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
        {
            // parcial (enquanto fala)
            TextoFalado = e.RecognitionResult;
            var fala = new Mensagens
            {
                Nome = Preferences.Get("usuarionome", string.Empty),
                Mensagem = TextoFalado,
                Data = DateTime.Now

            };
            int id = Preferences.Get("usuarioid", 0);
          var response =  await _apiServices.GetRespostaAgentContexto(TextoFalado, id); // ENVIA PARA API A CADA ATUALIZAÇÃO PARCIAL
            await _iSQLiteDbServive.AddMensagem(fala); // SALVA NO SQLITE A CADA ATUALIZAÇÃO PARCIAL
            await _iSQLiteDbServive.GetMensagem(); // TRAS TODAS AS MENSAGENS DO SQLITE 


        }

        private void OnCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e)
        {
            // final (quando termina)
            if (e.RecognitionResult.IsSuccessful)
                TextoFalado = e.RecognitionResult.Text;

            EstaEscutando = false;
            cts?.Dispose();
            cts = null;
        }

        [RelayCommand]
        private async Task Falar()
        {
            if (EstaEscutando)
                return;

            cts = new CancellationTokenSource();

            var granted = await speechToText.RequestPermissions(cts.Token);
            if (!granted)
            {
                cts.Dispose();
                cts = null;
                return;
            }

            EstaEscutando = true;

            await speechToText.StartListenAsync(
                new SpeechToTextOptions
                {
                    Culture = CultureInfo.GetCultureInfo("pt-BR"),
                    ShouldReportPartialResults = true
                },
                cts.Token);
        }

        [RelayCommand]
        private async Task Parar()
        {
            if (!EstaEscutando)
                return;

            await speechToText.StopListenAsync(CancellationToken.None);
            // o Completed vai disparar e finalizar estado
        }
    }
}