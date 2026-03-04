using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;

namespace AloPrefeitoP.Pages;

public partial class LoginPage : ContentPage
{
    private readonly ApiServices _apiServices;

    public LoginPage(ApiServices apiServices)
	{
		InitializeComponent();
        BindingContext = new LoginPageViewModel();
       _apiServices = apiServices;

    }

    //override async protected void OnAppearing()
    //{
    //    base.OnAppearing();
    //    string per = "Quais produtos estão abaixo do minimo?";
    //    //var response = await _apiServices.GetRespostaAgentContexto(per, 5);
    //    var response = await _apiServices.Login("comercial@facilitycontrol.com.br", "abcd");


    //}
    private async void Button_Clicked(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(EntEmail.Text))
        {

            await DisplayAlert("Erro", "Informe o email", "Cancelar");
            return;
        }
        if (string.IsNullOrEmpty(EntSenha.Text))
        {

            await DisplayAlert("Erro", "Informe o senha", "Cancelar");
            return;
        }

        var response = await _apiServices.Login(EntEmail.Text, EntSenha.Text);
        if (!response.HasError)
        {
            // entra no Shell
            Application.Current!.MainPage = new AppShell();

        }
        else
        {
            IsBusy = false;
            IsEnabled = true;
            await DisplayAlert("Erro", "Algo deu errado: senha ou e-mail incorretos", "Cancelar");
        }
    }
}