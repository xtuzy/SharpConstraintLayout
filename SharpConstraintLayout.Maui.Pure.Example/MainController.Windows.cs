using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ReloadPreview;
using System;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SharpConstraintLayout.Maui.Pure.Example
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainController : Window
    {
        public MainController()
        {
            this.Content = new MainPage();
#if DEBUG
            HotReload.Instance.Reload += () =>
            {
                try
                {
                    this.DispatcherQueue.TryEnqueue(() =>
                    {
                        var view = HotReload.Instance.ReloadClass<MainPage>() as FrameworkElement;
                        Content = view;
                    });
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

            };
            HotReload.Instance.Init("192.168.0.108");
#endif

        }
    }
}
