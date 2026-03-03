using CommunityToolkit.Maui.Media;
using System.Globalization;

namespace AloPrefeitoP
{
    public partial class HomePage : ContentPage
    {
        private readonly ISpeechToText _speechToText = SpeechToText.Default;

        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            // 1) Permissão do microfone (MAUI) + permissão do SpeechToText (Toolkit)
            var micStatus = await Permissions.RequestAsync<Permissions.Microphone>();
            var sttGranted = await _speechToText.RequestPermissions(CancellationToken.None);

            if (micStatus != PermissionStatus.Granted || !sttGranted)
            {
                ResultLabel.Text = "Permissão negada (microfone/voz).";
                return;
            }

            // 2) Assina eventos (parciais e final)
            _speechToText.RecognitionResultUpdated += OnUpdated;
            _speechToText.RecognitionResultCompleted += OnCompleted;

            ResultLabel.Text = "Ouvindo... fale algo 👂";

            // 3) Começa a ouvir
            await _speechToText.StartListenAsync(
                new SpeechToTextOptions
                {
                    Culture = new CultureInfo("pt-BR"),
                    ShouldReportPartialResults = true
                },
                CancellationToken.None
            );
        }

        private async void OnStopClicked(object sender, EventArgs e)
        {
            await _speechToText.StopListenAsync(CancellationToken.None);

            _speechToText.RecognitionResultUpdated -= OnUpdated;
            _speechToText.RecognitionResultCompleted -= OnCompleted;
        }

        private void OnUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs args)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // vai “digitando” enquanto você fala
                ResultLabel.Text = args.RecognitionResult;
            });
        }

        private void OnCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs args)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // resultado final
                ResultLabel.Text = args.RecognitionResult;
            });
        }
    }
}
