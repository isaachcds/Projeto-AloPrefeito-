using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class PerfilPage : ContentPage
{
    public PerfilPage(PerfilViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}