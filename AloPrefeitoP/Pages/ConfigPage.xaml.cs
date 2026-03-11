using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class ConfigPage : ContentPage
{
    public ConfigPage(ConfigViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}