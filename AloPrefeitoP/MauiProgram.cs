using AloPrefeitoP.Pages;
using AloPrefeitoP.Services;
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
            builder.Services.AddSingleton<ApiServices>();
            builder.Services.AddSingleton<ISQLiteDbServive, SQLiteDbServive>();
            builder.Services.AddHttpClient();
            builder.Logging.AddDebug();
            builder.UseSkiaSharp();
#endif

            return builder.Build();
        }
    }
}
