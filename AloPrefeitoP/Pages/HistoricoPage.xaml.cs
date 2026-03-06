using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class HistoricoPage : ContentPage
{
    private readonly HistoricoViewModel _vm;

    public HistoricoPage(HistoricoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}