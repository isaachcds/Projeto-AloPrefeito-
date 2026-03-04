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

        [ObservableProperty]
        private string textoFalado;

        [ObservableProperty]
        private bool estaEscutando;

        public HomePageViewModel()
        {
            // pode usar direto sem DI:
            speechToText = SpeechToText.Default;

            speechToText.RecognitionResultUpdated += OnUpdated;
            speechToText.RecognitionResultCompleted += OnCompleted;
        }

        private void OnUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
        {
            // parcial (enquanto fala)
            TextoFalado = e.RecognitionResult;
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