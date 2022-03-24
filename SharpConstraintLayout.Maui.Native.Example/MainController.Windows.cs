﻿using Microsoft.UI.Xaml;
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
        public static Window Current;
        public MainController()
        {
            Current = this;

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
                    SimpleDebug.WriteLine(ex.ToString());
                }

            };
            HotReload.Instance.Init("192.168.0.108", 100);
#endif
            SimpleDebug.WriteLine("App Start");
        }
    }
}
