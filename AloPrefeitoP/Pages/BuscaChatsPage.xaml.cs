using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class BuscaChatsPage : ContentPage
{
    private readonly BuscaChatsViewModel _vm;

    public BuscaChatsPage(BuscaChatsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BuscaEntry.Focus();
    }
}