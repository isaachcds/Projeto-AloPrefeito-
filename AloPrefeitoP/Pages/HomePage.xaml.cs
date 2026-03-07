using AloPrefeitoP.ViewModels;
using System.Collections.Specialized;

namespace AloPrefeitoP.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _vm;
    private bool _headerJaAnimou;

    public HomePage(HomePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;

        _vm.ScrollToBottomRequested += ScrollToBottom;
        _vm.ListaMensagens.CollectionChanged += ListaMensagens_CollectionChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadChatAtualAsync();
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
}