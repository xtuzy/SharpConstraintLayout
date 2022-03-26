using CoreGraphics;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace SharpConstraintLayout.Core.Benchmark
{
    public class MainPage
    {
        public UIView Page;
        public MainPage(CGRect frame)
        {
            var label = new UILabel(new CGRect(100, 100, 100, 50))
            {
                BackgroundColor = UIColor.SystemPink,
                TextAlignment = UITextAlignment.Center,
                Text = "Hello, iOS!!"
            };

            var button = new UIButton(new CGRect(100, 200, 100, 50))
            {
            };
            button.SetTitle("Click", UIControlState.Normal);
            button.TouchUpInside += (sender, e) =>
            {
                button.BackgroundColor = UIColor.LightGray;
            };

            Page = new UIStackView(frame)
            {
                BackgroundColor = UIColor.SystemYellow,
                Axis = UILayoutConstraintAxis.Vertical
            };
            Page.AddSubview(label);
            Page.AddSubview(button);
            LoadMauiAsset();
        }

        async Task LoadMauiAsset()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("favorite_black_24dp.svg");
            Debug.WriteLine($"svg size:{stream.Length}");
        }

        public UIView GetPage()
        {
            return Page;
        }
    }
}
