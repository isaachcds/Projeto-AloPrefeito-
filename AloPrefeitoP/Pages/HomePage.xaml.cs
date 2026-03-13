using AloPrefeitoP.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AloPrefeitoP.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _vm;
    private bool _headerJaAnimou;
    private bool _menuAnimando;
    private bool _animandoOndas;

    private List<BoxView> Ondas => new()
    {
        Wave1, Wave2, Wave3, Wave4, Wave5, Wave6, Wave7,
        Wave8, Wave9, Wave10, Wave11, Wave12, Wave13, Wave14,
        Wave15, Wave16, Wave17, Wave18, Wave19, Wave20, Wave21,
        Wave22, Wave23, Wave24
    };

    public HomePage(HomePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;

        ConfigurarColunasOndas();

        _vm.ScrollToBottomRequested += ScrollToBottom;
        _vm.MenuStateChanged += OnMenuStateChanged;
        _vm.ListaMensagens.CollectionChanged += ListaMensagens_CollectionChanged;
        _vm.PropertyChanged += Vm_PropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _vm.LoadChatAtualAsync();
        await _vm.LoadHistoricoAsync();

        ScrollToBottom();

        if (_vm.TemMensagens)
        {
            HeaderBoasVindas.Opacity = 0;
            HeaderBoasVindas.TranslationY = -20;
            HeaderBoasVindas.IsVisible = false;
            _headerJaAnimou = true;
        }
        else
        {
            HeaderBoasVindas.Opacity = 1;
            HeaderBoasVindas.TranslationY = 0;
            HeaderBoasVindas.IsVisible = true;
            _headerJaAnimou = false;
        }

        if (_vm.EstaEscutando && !_animandoOndas)
            _ = AnimarOndasLoop();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _animandoOndas = false;

        foreach (var onda in Ondas)
            onda.CancelAnimations();
    }

    private void ConfigurarColunasOndas()
    {
        WaveGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < 28; i++)
            WaveGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HomePageViewModel.EstaEscutando) &&
            _vm.EstaEscutando &&
            !_animandoOndas)
        {
            _ = AnimarOndasLoop();
        }
    }

    private async Task AnimarOndasLoop()
    {
        _animandoOndas = true;
        var random = new Random();

        double[] basePattern =
        {
            1.0, 1.8, 1.3, 2.1, 0.9, 1.6, 1.1,
            2.0, 1.0, 1.7, 1.4, 1.9, 0.9, 1.5,
            1.2, 2.2, 1.0, 1.8, 1.3, 2.0, 0.9,
            1.6, 1.1, 1.9, 1.0, 1.7, 1.2, 2.0
        };

        while (_animandoOndas && _vm.EstaEscutando)
        {
            try
            {
                var tasks = new List<Task>();

                for (int i = 0; i < Ondas.Count; i++)
                {
                    var onda = Ondas[i];

                    double variacao = (random.NextDouble() * 0.8) - 0.4;
                    double alvo = Math.Max(0.8, basePattern[i] + variacao);
                    uint duracao = (uint)random.Next(90, 170);

                    tasks.Add(onda.ScaleYTo(alvo, duracao, Easing.SinInOut));
                }

                await Task.WhenAll(tasks);

                tasks.Clear();

                for (int i = 0; i < Ondas.Count; i++)
                {
                    var onda = Ondas[i];
                    uint duracao = (uint)random.Next(90, 170);

                    double retorno = 0.95 + (random.NextDouble() * 0.20);
                    tasks.Add(onda.ScaleYTo(retorno, duracao, Easing.SinInOut));
                }

                await Task.WhenAll(tasks);
            }
            catch
            {
                break;
            }
        }

        foreach (var onda in Ondas)
        {
            try
            {
                onda.CancelAnimations();
                onda.ScaleY = 1;
            }
            catch
            {
            }
        }

        _animandoOndas = false;
    }

    private async void ListaMensagens_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_headerJaAnimou)
            return;

        if (_vm.TemMensagens)
        {
            _headerJaAnimou = true;

            await HeaderBoasVindas.TranslateTo(0, -20, 220, Easing.CubicInOut);
            await HeaderBoasVindas.FadeTo(0, 180, Easing.CubicInOut);

            HeaderBoasVindas.IsVisible = false;
        }
    }

    private void ScrollToBottom()
    {
        try
        {
            if (_vm.ListaMensagens == null || _vm.ListaMensagens.Count == 0)
                return;

            var last = _vm.ListaMensagens[_vm.ListaMensagens.Count - 1];

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ChatList.ScrollTo(last, position: ScrollToPosition.End, animate: true);
            });
        }
        catch
        {
        }
    }

    private async void OnMenuStateChanged(bool abrir)
    {
        if (_menuAnimando)
            return;

        _menuAnimando = true;

        try
        {
            if (abrir)
            {
                OverlayMenu.IsVisible = true;
                OverlayMenu.InputTransparent = false;

                await Task.WhenAll(
                    OverlayMenu.FadeTo(1, 180, Easing.CubicOut),
                    MenuLateral.TranslateTo(0, 0, 240, Easing.CubicOut)
                );
            }
            else
            {
                await Task.WhenAll(
                    OverlayMenu.FadeTo(0, 180, Easing.CubicIn),
                    MenuLateral.TranslateTo(-320, 0, 220, Easing.CubicIn)
                );

                OverlayMenu.InputTransparent = true;
                OverlayMenu.IsVisible = false;
            }
        }
        finally
        {
            _menuAnimando = false;
        }
    }

    private void OverlayMenu_Tapped(object sender, TappedEventArgs e)
    {
        _vm.FecharMenuCommand.Execute(null);
    }
}