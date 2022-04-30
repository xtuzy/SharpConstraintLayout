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
            ConstraintLayout.DEBUG = false;
            ConstraintLayout.MEASURE = true;

            Page = new ConstraintLayout()
            {
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
            //ConstraintLayoutInPlatformLayoutTest(Page);//Bug
            //CircleConstraintTest(Page);
            //FlowPerformanceTest(Page);
            //WrapPanelPerformanceTest(Page);
            //GroupTest(Page);
            //PlaceholderTest(Page);
            //SizeTest(Page);
        }

        void ConstraintLayoutInPlatformLayoutTest(ConstraintLayout page)
        {
            var scrollView = new UIScrollView()
            {
                BackgroundColor = UIColor.White,
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
            var BaseAlignTestPage = new ConstraintLayout()
            {
                BackgroundColor = UIColor.SystemPink,
            };
            scrollView.AddSubview(BaseAlignTestPage);
            BaseAlignTestPage.AddElement(FirstButton);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(BaseAlignTestPage);
                set.Select(BaseAlignTestPage)
                    .MinWidth(100)
                    .MinHeight(100)
                    .Width(200)
                    .Height(200)
                    .Select(FirstButton)
                    .LeftToLeft().RightToRight().TopToTop().BottomToBottom()
                    .Width(FluentConstraintSet.SizeBehavier.WrapContent)
                    .Height(FluentConstraintSet.SizeBehavier.WrapContent);
                set.ApplyTo(BaseAlignTestPage);
            }
            //BaseAlignTest(BaseAlignTestPage);

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
                AttributedText = new NSAttributedString("RichTextBlock", UIFont.SystemFontOfSize(14))
            };
            SixthRichTextBlock.Changed += (sender, e) =>
            {
                SixthRichTextBlock.InvalidateIntrinsicContentSize();
                SixthRichTextBlock.Superview.SetNeedsLayout();
            };

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
