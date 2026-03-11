using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;
using Plugin.Maui.Biometric;

namespace AloPrefeitoP.Pages;

public partial class LoginPage : ContentPage
{
    private readonly LoginPageViewModel vm;
    private readonly ApiServices _services;
    private bool _jaInicializou;
    public LoginPage(ApiServices services)
    {
        _services = services;
        vm = new LoginPageViewModel(_services);
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_jaInicializou)
            return;
        _jaInicializou = true;
        await vm.InicializarAsync();
    }
}
