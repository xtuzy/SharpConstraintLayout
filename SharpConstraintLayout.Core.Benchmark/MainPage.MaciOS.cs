using CoreGraphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace SharpConstraintLayout.Core.Benchmark
{
    public partial class MainPage
    {
        public UIView Page;
        private UIButton TestCsharpConstraintLayoutButton;
        private UIButton TestJavaConstraintLayoutButton;
        private UIButton TestSleepButton;
        UITextView TestCSharpSummary;
        UITextView TestJavaSummary;
        UITextView TestSleepSummary;
        public MainPage(CGRect frame)
        {
            var label = new UILabel(new CGRect(100, 100, 100, 50))
            {
                BackgroundColor = UIColor.SystemPink,
                TextAlignment = UITextAlignment.Center,
                Text = "Hello, iOS!!"
            };
            var ButtonContainer = new UIStackView(frame)
            {
                BackgroundColor = UIColor.SystemYellow,
                Axis = UILayoutConstraintAxis.Horizontal,
            };
            TestCsharpConstraintLayoutButton = new UIButton(new CGRect(100, 200, 100, 50))
            {
            };
            TestCsharpConstraintLayoutButton.SetTitle("Start CSharp", UIControlState.Normal);
            TestCsharpConstraintLayoutButton.TouchUpInside += (sender, e) =>
            {
                TestCsharpConstraintLayoutButton_Click(sender, e);
            };

            TestSleepButton = new UIButton(new CGRect(100, 200, 100, 50))
            {
            };
            TestSleepButton.SetTitle("Start Sleep", UIControlState.Normal);
            TestSleepButton.TouchUpInside += (sender, e) =>
            {
                TestSleepButton_Clicked(sender, e);
            };

            ButtonContainer.Add(TestCsharpConstraintLayoutButton);
            ButtonContainer.Add(TestSleepButton);

            TestCSharpSummary = new UITextView() { };
            TestSleepSummary = new UITextView() { };
            Page = new UIStackView(frame)
            {
                BackgroundColor = UIColor.SystemYellow,
                Axis = UILayoutConstraintAxis.Vertical,
            };

            Page.AddSubview(ButtonContainer);
            Page.AddSubview(TestCSharpSummary);
            Page.AddSubview(TestSleepSummary);

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
