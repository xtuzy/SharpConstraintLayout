using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ReloadPreview;
using SharpConstraintLayout.Maui.DebugTool;
using SharpConstraintLayout.Maui.Native.Example.Tool;
using SharpConstraintLayout.Maui.Widget;
using System;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SharpConstraintLayout.Maui.Native.Example
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainController : Window
    {
        public MainController()
        {
            //WinUI Set window special size
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            //appWindow.Resize(new Windows.Graphics.SizeInt32(390, 844));
            appWindow.Resize(new Windows.Graphics.SizeInt32(844, 390));

#if DEBUG
            HotReload.Instance.Reload += () =>
            {
                try
                {
                    this.DispatcherQueue.TryEnqueue(() =>
                    {
                        var view = HotReload.Instance.ReloadClass<MainPage>() as FrameworkElement;
                        Content = view;
                        SimpleDebug.WriteLine($"HotReload:{view} " + view.GetHashCode());
                    });

                }
                catch (Exception ex)
                {
                    SimpleDebug.WriteLine(ex.ToString());
                }

            };
            HotReload.Instance.Init("192.168.0.144", 100);
#endif
            this.Content = new MainPage();
            //this.Content = new Grid() { };
            //(this.Content as Grid).Children.Add(new TextBox() { Text = "TextBox" });
            SimpleDebug.WriteLine($"Window {this.Bounds}");
            SimpleDebug.WriteLine("App Start");
        }
    }
}
