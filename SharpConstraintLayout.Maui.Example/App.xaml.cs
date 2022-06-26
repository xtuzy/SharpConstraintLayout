using ReloadPreview;
using SharpConstraintLayout.Maui.DebugTool;

namespace SharpConstraintLayout.Maui.Example;

public partial class App : Application
{
    public static int WindowWidth;
    public static int WindowHeight;
    public App()
    {
        InitializeComponent();

        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if WINDOWS
            var mauiWindow = handler.VirtualView;
            var nativeWindow = handler.PlatformView;

            nativeWindow.Activate();
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32(500, 700));
            WindowWidth = appWindow.Size.Width;
            WindowHeight = appWindow.Size.Height;
#endif
        });

        MainPage = new AppShell();

#if DEBUG
        HotReload.Instance.Reload += () =>
        {
            try
            {
                this.Dispatcher.Dispatch(() =>
                {
                    var view = HotReload.Instance.ReloadClass<AppShell>() as Page;
                    MainPage = view;
                    SimpleDebug.WriteLine($"HotReload:{view} " + view.GetHashCode());
                });

            }
            catch (Exception ex)
            {
                SimpleDebug.WriteLine(ex.ToString());
            }

        };
        HotReload.Instance.Init("192.168.0.144");
#endif
    }
}
