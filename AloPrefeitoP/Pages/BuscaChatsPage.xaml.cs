using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class BuscaChatsPage : ContentPage
{
    public BuscaChatsPage(BuscaChatsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BuscaEntry.Focus();
    }
}