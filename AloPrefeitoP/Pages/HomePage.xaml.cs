using AloPrefeitoP.ViewModels;
using System.Collections.Specialized;

namespace AloPrefeitoP.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _vm;
    private bool _headerJaAnimou;
    private bool _menuAnimando;

    public HomePage(HomePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;

        _vm.ScrollToBottomRequested += ScrollToBottom;
        _vm.MenuStateChanged += OnMenuStateChanged;
        _vm.ListaMensagens.CollectionChanged += ListaMensagens_CollectionChanged;
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