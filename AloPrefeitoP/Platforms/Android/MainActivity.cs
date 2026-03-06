using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace AloPrefeitoP
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            AndroidEnvironment.UnhandledExceptionRaiser += (sender, e) =>
            {
                Android.Util.Log.Error("MAUI-UNHANDLED", e.Exception.ToString());
                e.Handled = false;
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Android.Util.Log.Error("MAUI-DOMAIN", e.ExceptionObject?.ToString() ?? "null");
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Android.Util.Log.Error("MAUI-TASK", e.Exception.ToString());
            };

            base.OnCreate(savedInstanceState);
        }
    }
}
