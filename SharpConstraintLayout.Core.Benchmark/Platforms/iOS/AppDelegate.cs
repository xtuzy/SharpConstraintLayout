using Foundation;
using UIKit;

namespace SharpConstraintLayout.Core.Benchmark
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        //protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        public override UIWindow? Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // create a UIViewController with a single UILabel
            var vc = new MainController(Window);

            Window.RootViewController = vc;

            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }
    }
}