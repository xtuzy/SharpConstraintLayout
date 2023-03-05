using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SharpConstraintLayout.Maui.DebugTool;
using SharpConstraintLayout.Maui.Native.Example.Tool;
using SharpConstraintLayout.Maui.Widget;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
namespace SharpConstraintLayout.Maui.Native.Example
{
    public partial class MainPage : ConstraintLayout
    {
        public ConstraintLayout layout;

        public MainPage()
        {
            //ConstraintLayout.DEBUG = false;
            ConstraintLayout.MEASURE_MEASURELAYOUT = true;
            //ConstraintLayout.MEASUREEVERYWIDGET = true;
            //ConstraintLayout.MEASUREEVERYCHILD = true;
            var textview = new TextBlock();
#if WINDOWS
            var fr = new BlogFrameRate.FrameRateCalculator();
            fr.FrameRateUpdated += (value) =>
            {
                this.DispatcherQueue.TryEnqueue(() => textview.Text = value.Frames.ToString());
            };
            fr.Start();
#endif
            this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.HotPink);
            var buttonList = new StackPanel() { Orientation = Microsoft.UI.Xaml.Controls.Orientation.Horizontal };
            var baseAlignBt = new Button() { Content = "BaseAlign" };
            var baselineBt = new Button() { Content = "Baseline" };
            var guidelineBt = new Button() { Content = "Guideline" };
            var barrierBt = new Button() { Content = "Barrier" };
            var visibilityBt = new Button() { Content = "Visibility" };
            var flowBt = new Button() { Content = "Flow" };
            var platformLayoutInConstraintLayoutBt = new Button() { Content = "PlatformLayoutInConstraintLayout" };
            var constraintLayoutInScrollViewBt = new Button() { Content = "ConstraintLayoutInScrollView" };
            var constraintLayoutInListViewBt = new Button() { Content = "ConstraintLayoutInList" };
            var circleConstraintBt = new Button() { Content = "CircleConstraint" };
            var flowPerformanceBt = new Button() { Content = "FlowPerformance" };
            var wrapPanelPerformanceBt = new Button() { Content = "WrapPanelPerformance" };
            var groupBt = new Button() { Content = "Group" };
            var placeholderBt = new Button() { Content = "Placeholder" };
            var sizeBt = new Button() { Content = "Size" };
            var nestedConstraintLayoutBt = new Button() { Content = "NestedConstraintLayout" };
            buttonList.Children.Add(textview);
            buttonList.Children.Add(baseAlignBt);
            buttonList.Children.Add(baselineBt);
            buttonList.Children.Add(guidelineBt);
            buttonList.Children.Add(barrierBt);
            buttonList.Children.Add(visibilityBt);
            buttonList.Children.Add(flowBt);
            buttonList.Children.Add(platformLayoutInConstraintLayoutBt);
            buttonList.Children.Add(constraintLayoutInScrollViewBt);
            buttonList.Children.Add(constraintLayoutInListViewBt);

            buttonList.Children.Add(circleConstraintBt);
            buttonList.Children.Add(flowPerformanceBt);
            buttonList.Children.Add(wrapPanelPerformanceBt);
            buttonList.Children.Add(groupBt);
            buttonList.Children.Add(placeholderBt);
            buttonList.Children.Add(sizeBt);
            buttonList.Children.Add(nestedConstraintLayoutBt);

            var scroll = new ScrollViewer() { Content = buttonList, HorizontalScrollMode = ScrollMode.Enabled, VerticalScrollMode = ScrollMode.Disabled, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };
            var layout = new ConstraintLayout();
            this.AddElement(scroll);
            this.AddElement(layout);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(this);
                set.Select(scroll).TopToTop().LeftToLeft().RightToRight().Width(SizeBehavier.MatchConstraint)
                    .Select(layout).TopToBottom(scroll).BottomToBottom().EdgesXTo().Height(SizeBehavier.MatchConstraint)
                    ;
                set.ApplyTo(this);
            }
            baseAlignBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                BaseAlignTest(layout);
            };
            baselineBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                BaselineTest(layout);
            };
            guidelineBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                GuidelineTest(layout);
            };
            barrierBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                BarrierTest(layout);
            };
            visibilityBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                VisibilityTest(layout);
            };
            flowBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                FlowTest(layout);
            };
            platformLayoutInConstraintLayoutBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                PlatformLayoutInConstraintLayoutTest(layout);
            };
            constraintLayoutInScrollViewBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                ConstraintLayoutInScrollViewTest(layout);
            };
            constraintLayoutInListViewBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                ConstraintLayoutInListViewTest(layout);
            };
            circleConstraintBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                CircleConstraintTest(layout);
            };
            flowPerformanceBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                FlowPerformanceTest(layout);
            };
            wrapPanelPerformanceBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                WrapPanelPerformanceTest(layout);
            };
            groupBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                GroupTest(layout);
            };
            placeholderBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                PlaceholderTest(layout);
            };
            sizeBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                SizeTest(layout);
            };
            nestedConstraintLayoutBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                NestedConstraintLayoutTest(layout);
            };

        }

        void ConstraintLayoutInScrollViewTest(ConstraintLayout page)
        {
            (Button FirstButton, Button SecondButton, Canvas ThirdCanvas, TextBlock FouthTextBlock, TextBox FifthTextBox, RichTextBlock SixthRichTextBlock) = CreateControls();
            var scrollView = new ScrollViewer()
            {
                Background = new SolidColorBrush(Colors.White),
            };
            page.AddElement(scrollView);

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
                Background = new SolidColorBrush(Colors.Pink),
                ConstrainWidth = ConstraintSet.MatchParent,
                ConstrainHeight = ConstraintSet.WrapContent,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                ConstrainPaddingLeft = 10,
                ConstrainPaddingTop = 10,
                ConstrainPaddingRight = 10,
                ConstrainPaddingBottom = 10,
            };

            scrollView.Content = firstConstraintLayoutPage;
            var firstConstraintLayoutPageBackground = new Canvas()
            {
                Background = new SolidColorBrush(Colors.Yellow),
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
                    .Select(ThirdCanvas).TopToBottom(SecondButton).CenterXTo()
                    .Width(SizeBehavier.MatchParent)
                    .Height(1000)
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
                             SimpleDebug.WriteLine("firstConstraintLayoutPage:" + firstConstraintLayoutPage.GetViewLayoutInfo());
                             SimpleDebug.WriteLine("FirstButton:" + FirstButton.GetViewLayoutInfo());
                             SimpleDebug.WriteLine("scrollView:" + scrollView.GetViewLayoutInfo());
                             SimpleDebug.WriteLine("page:" + page.GetViewLayoutInfo());
                         }, page);
                  });
        }

        void ConstraintLayoutInListViewTest(ConstraintLayout page)
        {
            var listView = new ListView()
            {
                Background = new SolidColorBrush(Colors.White),
            };
            page.AddElement(listView);

            using (var set = new FluentConstraintSet())
            {
                set.Clone(page);
                set.Select(listView)
                    .LeftToLeft().TopToTop()
                    .Width(SizeBehavier.MatchParent)
                    .Height(SizeBehavier.MatchParent);
                set.ApplyTo(page);
            }

            var firstConstraintLayoutPage = new ConstraintLayout()
            {
                Background = new SolidColorBrush(Colors.Pink),
                ConstrainWidth = ConstraintSet.MatchParent,
                ConstrainHeight = ConstraintSet.WrapContent,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                ConstrainPaddingLeft = 10,
                ConstrainPaddingTop = 10,
                ConstrainPaddingRight = 10,
                ConstrainPaddingBottom = 10,
            };
        }
        private (Button FirstButton, Button SecondButton, Canvas ThirdCanvas, TextBlock FouthTextBlock, TextBox FifthTextBox, RichTextBlock SixthRichTextBlock) CreateControls()
        {
            var FirstButton = new Button()
            {
                Tag = "FirstButton",
                ClickMode = ClickMode.Press,
                Content = "FirstButton",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Red)
            };

            var SecondButton = new Button()
            {
                //Width = 100,Height = 100,
                Tag = "SecondButton",
                Content = "SecondBotton",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
            };

            var ThirdCanvas = new Canvas()
            {
                Tag = "ThirdCanvas",
                //Width = 100,
                //Height = 100,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.LightGreen)
            };

            var FouthTextBlock = new TextBlock()
            {
                Tag = "FouthTextBlock",
                Text = "FourthTextBlock",
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            var FifthTextBox = new TextBox()
            {
                Tag = "FifthTextBox",
                Text = "FifthTextBox",
                AcceptsReturn = true
            };

            //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
            var SixthRichTextBlock = new RichTextBlock()
            {
                Tag = "SixthRichTextBlock",
                IsTextSelectionEnabled = true,
                TextWrapping = TextWrapping.Wrap
            };
            Paragraph paragraph = new Paragraph()
            {
                FontSize = 18
            };
            Run run = new Run()
            {
                Text = "RichTextBlock"
            };
            paragraph.Inlines.Add(run);
            SixthRichTextBlock.Blocks.Add(paragraph);
            return (FirstButton, SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock);
        }

        private void MainButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine("Hello");
        }

        async Task LoadMauiAsset()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("favorite_black_24dp.svg");
            System.Diagnostics.Debug.WriteLine($"svg size:{stream.Length}");
        }

    }
}