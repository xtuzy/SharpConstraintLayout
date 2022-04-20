using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ReloadPreview;
using SharpConstraintLayout.Maui.DebugTool;
using SharpConstraintLayout.Maui.Native.Example.Tool;
using System;
using System.Diagnostics;

namespace SharpConstraintLayout.Maui.Native.Example
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainController : AppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            SimpleDebug.WriteLine($"Window");
            SimpleDebug.WriteLine("App Start");
            base.OnCreate(savedInstanceState);
#if DEBUG
            HotReload.Instance.Reload += () =>
            {
                try
                {
                    var view = HotReload.Instance.ReloadClass<MainPage>(this) as View;
                    SetContentView(view);
                    SimpleDebug.WriteLine($"HotReload:{view} " + view.GetHashCode());
                }
                catch (Exception ex)
                {
                    SimpleDebug.WriteLine(ex.ToString());
                }

            };
            HotReload.Instance.Init("192.168.0.144");
#endif
            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.activity_main);

            SetContentView(new MainPage(this));
        }
    }
}