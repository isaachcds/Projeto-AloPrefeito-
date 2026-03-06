using AloPrefeitoP.Pages;
using AloPrefeitoP.Services;
using AloPrefeitoP.ViewModels;
using CommunityToolkit.Maui;
using DevExpress.Maui;
using Microsoft.Extensions.Logging;
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

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<ISQLiteDbServive, SQLiteDbServive>();
            builder.Services.AddHttpClient<ApiServices>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<LoginPage>();

            return builder.Build();
        }
    }
}