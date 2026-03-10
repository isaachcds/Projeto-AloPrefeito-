using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;
using Plugin.Maui.Biometric;

namespace AloPrefeitoP.Pages;

public partial class LoginPage : ContentPage
{
    private readonly LoginPageViewModel vm;
    private readonly ApiServices _services;
    public LoginPage(ApiServices services)
    {
        _services = services;
        vm = new LoginPageViewModel(_services);
        InitializeComponent();
        BindingContext = vm;
    }

  

    }
