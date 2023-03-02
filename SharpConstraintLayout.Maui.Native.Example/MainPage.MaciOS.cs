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
        public ConstraintLayout Page;

        public MainPage(CGRect frame)
        {
            ConstraintLayout.DEBUG = false;
            ConstraintLayout.MEASURE_MEASURELAYOUT = true;
            //ConstraintLayout.MEASUREEVERYWIDGET = true;
            //ConstraintLayout.MEASUREEVERYCHILD = true;

            Page = new ConstraintLayout()
            {
                AutosizesSubviews = true,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                BackgroundColor = UIColor.SystemGray
            };
            var buttonList = new UIStackView();
            var baseAlignBt = new UIButton(); baseAlignBt.SetTitle("BaseAlign", UIControlState.Normal);
            var baselineBt = new UIButton(); baselineBt.SetTitle("Baseline", UIControlState.Normal);
            var guidelineBt = new UIButton(); guidelineBt.SetTitle("Guideline", UIControlState.Normal);
            var barrierBt = new UIButton(); barrierBt.SetTitle("Barrier", UIControlState.Normal);
            var visibilityBt = new UIButton(); visibilityBt.SetTitle("Visibility", UIControlState.Normal);
            var flowBt = new UIButton(); flowBt.SetTitle("Flow", UIControlState.Normal);
            var platformLayoutInConstraintLayoutBt = new UIButton(); platformLayoutInConstraintLayoutBt.SetTitle("PlatformLayoutInConstraintLayout", UIControlState.Normal);
            var constraintLayoutInScrollViewBt = new UIButton(); constraintLayoutInScrollViewBt.SetTitle("ConstraintLayoutInScrollView", UIControlState.Normal);
            var circleConstraintBt = new UIButton(); circleConstraintBt.SetTitle("CircleConstraint", UIControlState.Normal);
            var flowPerformance = new UIButton(); flowPerformance.SetTitle("FlowPerformance", UIControlState.Normal);
            var wrapPanelPerformance = new UIButton(); wrapPanelPerformance.SetTitle("WrapPanelPerformance", UIControlState.Normal);
            var group = new UIButton(); group.SetTitle("Group", UIControlState.Normal);
            var placeholder = new UIButton(); placeholder.SetTitle("Placeholder", UIControlState.Normal);
            var size = new UIButton(); size.SetTitle("Size", UIControlState.Normal);
            buttonList.Add(baseAlignBt);
            buttonList.Add(baselineBt);
            buttonList.Add(guidelineBt);
            buttonList.Add(barrierBt);
            buttonList.Add(visibilityBt);
            buttonList.Add(flowBt);
            buttonList.Add(platformLayoutInConstraintLayoutBt);
            buttonList.Add(constraintLayoutInScrollViewBt);
            buttonList.Add(circleConstraintBt);
            buttonList.Add(flowPerformance);
            buttonList.Add(wrapPanelPerformance);
            buttonList.Add(group);
            buttonList.Add(placeholder);
            buttonList.Add(size);

            var scroll = new UIScrollView();
            scroll.Add(buttonList);
            var layout = new ConstraintLayout();
            Page.AddElement(scroll);
            Page.AddElement(layout);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(Page);
                set.Select(scroll).TopToTop().LeftToLeft().RightToRight().Width(SizeBehavier.MatchConstraint)
                    .Select(layout).TopToBottom(scroll).BottomToBottom().EdgesXTo().Height(SizeBehavier.MatchConstraint)
                    ;
                set.ApplyTo(Page);
            }

            baseAlignBt.TouchUpInside += (sender, e) =>
            {
                BaseAlignTest(layout);
            };
            baselineBt.TouchUpInside += (sender, e) =>
            {
                BaselineTest(layout);
            };
            guidelineBt.TouchUpInside += (sender, e) =>
            {
                GuidelineTest(layout);
            };
            barrierBt.TouchUpInside += (sender, e) =>
            {
                BarrierTest(layout);
            };
            visibilityBt.TouchUpInside += (sender, e) =>
            {
                VisibilityTest(layout);
            };
            flowBt.TouchUpInside += (sender, e) =>
            {
                FlowTest(layout);
            };
            platformLayoutInConstraintLayoutBt.TouchUpInside += (sender, e) =>
            {
                PlatformLayoutInConstraintLayoutTest(layout);
            };
            constraintLayoutInScrollViewBt.TouchUpInside += (sender, e) =>
            {
                ConstraintLayoutInScrollViewTest(layout);
            };
            circleConstraintBt.TouchUpInside += (sender, e) =>
            {
                CircleConstraintTest(layout);
            };
            flowPerformance.TouchUpInside += (sender, e) =>
            {
                FlowPerformanceTest(layout);
            };
            wrapPanelPerformance.TouchUpInside += (sender, e) =>
            {
                WrapPanelPerformanceTest(layout);
            };
            group.TouchUpInside += (sender, e) =>
            {
                GroupTest(layout);
            };
            placeholder.TouchUpInside += (sender, e) =>
            {
                PlaceholderTest(layout);
            };
            size.TouchUpInside += (sender, e) =>
            {
                SizeTest(layout);
            };

            //NestedConstraintLayoutTest(Page);
        }

        void ConstraintLayoutInScrollViewTest(ConstraintLayout page)
        {
            (UIButton FirstButton, UIButton SecondButton, UIView ThirdCanvas, UITextView FouthTextBlock, UITextField FifthTextBox, UITextView SixthRichTextBlock) = CreateControls();
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
                    .Width(SizeBehavier.MatchParent)
                    .Height(SizeBehavier.MatchParent);
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
                    .Width(SizeBehavier.WrapContent)
                    .Height(SizeBehavier.WrapContent)
                    .Select(firstConstraintLayoutPageBackground)
                    .EdgesTo()
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.MatchConstraint);
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

        private (UIButton FirstButton, UIButton SecondButton, UIView ThirdCanvas, UITextView FouthTextBlock, UITextField FifthTextBox, UITextView SixthRichTextBlock) CreateControls()
        {
            var FirstButton = new UIButton()
            {
                BackgroundColor = UIColor.Red
            };
            FirstButton.SetTitle("FirstButton", UIControlState.Normal);
            FirstButton.TouchUpInside += FirstButton_Click;

            var SecondButton = new UIButton()
            {
                BackgroundColor = UIColor.Black
            };
            SecondButton.SetTitle("SecondButton", UIControlState.Normal);

            var ThirdCanvas = new UIView()
            {
                BackgroundColor = UIColor.Green
            };

            var FouthTextBlock = new UITextView()
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

            var FifthTextBox = new UITextField()
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
            var SixthRichTextBlock = new UITextView()
            {
                ScrollEnabled = false,
                AttributedText = new NSAttributedString("RichTextBlock咩咩咩咩咩咩咩", UIFont.SystemFontOfSize(14))
            };
            SixthRichTextBlock.Changed += (sender, e) =>
            {
                SixthRichTextBlock.InvalidateIntrinsicContentSize();
                SixthRichTextBlock.Superview.SetNeedsLayout();
            };
            return (FirstButton, SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock);
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
