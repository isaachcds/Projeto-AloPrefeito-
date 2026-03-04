using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		BindingContext = new HomePageViewModel();
	}
}