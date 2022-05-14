using ReloadPreview;
using SharpConstraintLayout.Maui.DebugTool;

namespace SharpConstraintLayout.Maui.Example;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

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
        HotReload.Instance.Init("192.168.0.144", 100);
#endif
    }
}
