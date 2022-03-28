using ReloadPreview;
using SharpConstraintLayout.Maui.DebugTool;
using SharpConstraintLayout.Maui.Native.Example.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace SharpConstraintLayout.Maui.Native.Example
{
    internal class MainController : UIViewController
    {
        public MainController(UIWindow window)
        {
            HotReload.Instance.Init("192.168.0.144", 200);
            HotReload.Instance.Reload += () =>
            {
                try
                {
                    CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        dynamic view = HotReload.Instance.ReloadClass<MainPage>(window!.Frame);
                        this.View = view.GetPage();
                        SimpleDebug.WriteLine($"HotReload:{this.View} " + this.View.GetHashCode());
                    });
                }
                catch (Exception ex)
                {
                    CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        View = new UIView() { BackgroundColor = UIColor.White };
                    });

                    SimpleDebug.WriteLine(ex.ToString());
                }
            };
            SimpleDebug.WriteLine($"Window {window!.Frame}");
            SimpleDebug.WriteLine("App Start");
            this.View = new MainPage(window!.Frame).Page;
        }
    }
}
