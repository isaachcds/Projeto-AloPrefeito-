using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _vm;

    public HomePage(HomePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;

        // “evento” pro VM pedir scroll
        _vm.ScrollToBottomRequested += ScrollToBottom;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadChatAtualAsync();

        // opcional: ao entrar numa conversa, já desce pro fim
        ScrollToBottom();
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
            // não quebra se falhar (ex: lista ainda não renderizou)
        }
    }
}