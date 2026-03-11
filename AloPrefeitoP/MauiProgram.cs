using AloPrefeitoP.Pages;
using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;
using CommunityToolkit.Maui;
using DevExpress.Maui;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Biometric;
using ScheduleListUI.Services;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AloPrefeitoP
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseDevExpress()
                .UseDevExpressControls()
                .UseSkiaSharp()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            builder.Services.AddSingleton<IBiometric>(BiometricAuthenticationService.Default);

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<ISQLiteDbServive, SQLiteDbServive>();
            builder.Services.AddHttpClient<ApiServices>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HistoricoViewModel>();
            builder.Services.AddTransient<HistoricoPage>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<BuscaChatsPage>();
            builder.Services.AddTransient<BuscaChatsViewModel>();
            builder.Services.AddTransient<ConfigPage>();
            builder.Services.AddTransient<ConfigViewModel>();
            builder.Services.AddTransient<HistoricoPage>();
            builder.Services.AddTransient<HistoricoViewModel>();
            return builder.Build();
        }
    }
}