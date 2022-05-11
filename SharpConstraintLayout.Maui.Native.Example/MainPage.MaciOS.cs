using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using SharpConstraintLayout.Maui.Widget;
using Foundation;
using Microsoft.Maui.Storage;
using SharpConstraintLayout.Maui.DebugTool;
using SharpConstraintLayout.Maui.Native.Example.Tool;

namespace SharpConstraintLayout.Maui.Native.Example
{
    public partial class MainPage
    {
        private ConstraintLayout layout;
        public ConstraintLayout Page;
        private UIButton FirstButton;
        private UIButton SecondButton;
        private UIView ThirdCanvas;
        private UITextView FouthTextBlock;
        private UITextField FifthTextBox;
        private UITextView SixthRichTextBlock;

        public MainPage(CGRect frame)
        {
            ConstraintLayout.DEBUG = true;
            //ConstraintLayout.MEASURE_MEASURELAYOUT = true;
            //ConstraintLayout.MEASUREEVERYWIDGET = true;
            //ConstraintLayout.MEASUREEVERYCHILD = true;

            Page = new ConstraintLayout()
            {
                AutosizesSubviews = true,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                BackgroundColor = UIColor.SystemGray
            };

            createControls();

            //BaseAlignTest(Page);
            //BaselineTest(Page);
            //GuidelineTest(Page);//OK
            //BarrierTest(Page);//OK
            //VisibilityTest(Page);//OK
            //FlowTest(Page);
            //PlatformLayoutInConstraintLayoutTest(Page);
            ConstraintLayoutInPlatformLayoutTest(Page);//Bug
            //CircleConstraintTest(Page);
            //FlowPerformanceTest(Page);//Bad performance,need 16~20ms.
            //WrapPanelPerformanceTest(Page);
            //GroupTest(Page);
            //PlaceholderTest(Page);
            //SizeTest(Page);
        }

        void ConstraintLayoutInPlatformLayoutTest(ConstraintLayout page)
        {
            page.ConstrainPaddingLeft = 10;
            page.ConstrainPaddingRight = 10;
            page.ConstrainPaddingTop = 10;
            page.ConstrainPaddingBottom = 10;

            var scrollView = new UIScrollView()
            {
                //AutosizesSubviews = true,
                //AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                BackgroundColor = UIColor.White,
                //ContentSize = new CGSize(350, 153),
            };
            page.AddSubview(scrollView);

            using (var set = new FluentConstraintSet())
            {
                set.Clone(page);
                set.Select(scrollView)
                    .LeftToLeft().TopToTop()
                    .Width(FluentConstraintSet.SizeBehavier.MatchParent)
                    .Height(FluentConstraintSet.SizeBehavier.MatchParent);
                set.ApplyTo(page);
            }

            var firstConstraintLayoutPage = new ConstraintLayout()
            {
                //Frame = new CGRect(0, 0, 200, 100),
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.SystemPink,
                ConstrainWidth = ConstraintSet.WrapContent,
                ConstrainHeight = ConstraintSet.WrapContent,
                ConstrainPaddingLeft = 10,
                ConstrainPaddingTop = 10,
                ConstrainPaddingRight = 10,
                ConstrainPaddingBottom = 10,
            };
            scrollView.AddSubview(firstConstraintLayoutPage);
            firstConstraintLayoutPage.TopAnchor.ConstraintEqualTo(scrollView.CenterYAnchor).Active = true;
            firstConstraintLayoutPage.LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            firstConstraintLayoutPage.RightAnchor.ConstraintEqualTo(scrollView.RightAnchor).Active = true;
            firstConstraintLayoutPage.BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor).Active = true;

            var firstConstraintLayoutPageBackground = new UIView()
            {
                BackgroundColor = UIColor.SystemYellow,
            };

            firstConstraintLayoutPage.AddElement(firstConstraintLayoutPageBackground);
            firstConstraintLayoutPage.AddElement(FirstButton);
            firstConstraintLayoutPage.AddElement(SecondButton);
            firstConstraintLayoutPage.AddElement(ThirdCanvas);
            firstConstraintLayoutPage.AddElement(FouthTextBlock);
            firstConstraintLayoutPage.AddElement(FifthTextBox);
            firstConstraintLayoutPage.AddElement(SixthRichTextBlock);

            using (var set = new FluentConstraintSet())
            {
                set.Clone(firstConstraintLayoutPage);
                set.Select(FirstButton)
                    .TopToTop().CenterXTo()
                    .Select(SecondButton).TopToBottom(FirstButton).CenterXTo()
                    .Select(ThirdCanvas).TopToBottom(SecondButton).CenterXTo().Width(400).Height(1000)
                    .Select(FouthTextBlock).TopToBottom(ThirdCanvas).CenterXTo()
                    .Select(FifthTextBox).TopToBottom(FouthTextBlock).CenterXTo()
                    .Select(SixthRichTextBlock).TopToBottom(FifthTextBox).CenterXTo()
                    .Select(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock)
                    .Width(FluentConstraintSet.SizeBehavier.WrapContent)
                    .Height(FluentConstraintSet.SizeBehavier.WrapContent)
                    .Select(firstConstraintLayoutPageBackground)
                    .EdgesTo()
                    .Width(FluentConstraintSet.SizeBehavier.MatchConstraint)
                    .Height(FluentConstraintSet.SizeBehavier.MatchConstraint);
                set.ApplyTo(firstConstraintLayoutPage);
            }

            Task.Run(async () =>
                  {
                      await Task.Delay(3000);//wait ui show
                      UIThread.Invoke(() =>
                         {
                             //SimpleDebug.WriteLine("firstConstraintLayoutPage:" + firstConstraintLayoutPage.GetViewLayoutInfo());
                             //SimpleDebug.WriteLine("FirstButton:" + FirstButton.GetViewLayoutInfo());
                             //SimpleDebug.WriteLine("scrollView:" + scrollView.GetViewLayoutInfo());
                             SimpleDebug.WriteLine("scrollView ContentSize:" + scrollView.ContentSize);
                         }, page);
                  });
        }

        private void createControls()
        {
            FirstButton = new UIButton()
            {
                BackgroundColor = UIColor.Red
            };
            FirstButton.SetTitle("FirstButton", UIControlState.Normal);
            FirstButton.TouchUpInside += FirstButton_Click;

            SecondButton = new UIButton()
            {
                BackgroundColor = UIColor.Black
            };
            SecondButton.SetTitle("SecondButton", UIControlState.Normal);
            SecondButton.TouchUpInside += SecondButton_Click;

            ThirdCanvas = new UIView()
            {
                BackgroundColor = UIColor.Green
            };

            FouthTextBlock = new UITextView()
            {
                ScrollEnabled = false,
                Text = "FouthTextBlock",
                BackgroundColor = UIColor.Green,
            };
            FouthTextBlock.Changed += (sender, e) =>
            {
                FouthTextBlock.InvalidateIntrinsicContentSize();
                FouthTextBlock.Superview.SetNeedsLayout();
            };

            FifthTextBox = new UITextField()
            {
                Enabled = true,
                EnablesReturnKeyAutomatically = true,
                ClearButtonMode = UITextFieldViewMode.Always,
                Text = "FifthTextBox",
            };
            FifthTextBox.EditingChanged += (sender, e) =>
            {
                FifthTextBox.InvalidateIntrinsicContentSize();
                FifthTextBox.Superview.SetNeedsLayout();
            };

            //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
            SixthRichTextBlock = new UITextView()
            {
                ScrollEnabled = false,
                AttributedText = new NSAttributedString("RichTextBlock咩咩咩咩咩咩咩咩咩咩咩咩咩咩咩咩咩咩咩  咩咩咩咩咩咩咩", UIFont.SystemFontOfSize(14))
            };
            SixthRichTextBlock.Changed += (sender, e) =>
            {
                SixthRichTextBlock.InvalidateIntrinsicContentSize();
                SixthRichTextBlock.Superview.SetNeedsLayout();
            };

        }

        private void SecondButton_Click(object sender, EventArgs e)
        {
            /*Stopwatch sw = new Stopwatch();
            sw.Start();*/
            var size = FifthTextBox.IntrinsicContentSize;

            /*sw.Stop();
            SimpleDebug.WriteLine($"IntrinsicContentSize Spend: {sw.Elapsed.TotalMilliseconds}");*/
        }

        private void FirstButton_Click(object sender, EventArgs e)
        {
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
