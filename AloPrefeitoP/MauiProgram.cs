using CommunityToolkit.Maui;
using DevExpress.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Microsoft.Extensions.Logging;

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
            builder.UseSkiaSharp();
#endif

            return builder.Build();
        }
    }
}
