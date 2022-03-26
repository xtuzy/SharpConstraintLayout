using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using ReloadPreview;
using System;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;

namespace SharpConstraintLayout.Core.Benchmark
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainController : AppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
#if DEBUG
            HotReload.Instance.Reload += () =>
            {
                try
                {
                    var view = HotReload.Instance.ReloadClass<MainPage>(this) as View;
                    SetContentView(view);
                }catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                
            };
            HotReload.Instance.Init("192.168.0.108");
#endif
            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.activity_main);

            SetContentView(new MainPage(this));
            //AddContentView(new MainPage(this), new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }
    }
}