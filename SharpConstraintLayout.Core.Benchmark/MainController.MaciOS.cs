﻿using ReloadPreview;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace SharpConstraintLayout.Core.Benchmark
{
    internal class MainController : UIViewController
    {
        public MainController(UIWindow window)
        {
            HotReload.Instance.Init("192.168.0.108", 300);
            HotReload.Instance.Reload += () =>
            {
                try
                {
                    dynamic view = HotReload.Instance.ReloadClass<MainPage>(window!.Frame);
                    this.View = view.GetPage();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            };
            this.View = new MainPage(window!.Frame).Page;
        }
    }
}
