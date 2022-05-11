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
        private Button FirstButton;

        public Button SecondButton;
        private Canvas ThirdCanvas;
        private TextBlock FouthTextBlock;
        private TextBox FifthTextBox;
        private RichTextBlock SixthRichTextBlock;
        public ConstraintLayout layout;

        public MainPage()
        {
            ConstraintLayout.DEBUG = false;
            ConstraintLayout.MEASURE_MEASURELAYOUT = true;
            //ConstraintLayout.MEASUREEVERYWIDGET = true;
            //ConstraintLayout.MEASUREEVERYCHILD = true;

            createControls();
            this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.HotPink);

            //BaseAlignTest(this);
            //BaselineTest(this); //bug:first time not align baseline
            //GuidelineTest(this);
            //BarrierTest(this);//bug:text box have false length
            //VisibilityTest(this);//bug:invisiable not correct
            //FlowTest(this);
            //NestedConstraintLayoutTest(this);
            //CircleConstraintTest(this);
            //PlatformLayoutInConstraintLayoutTest(this);//OK
            //FlowPerformanceTest(this);
            //WrapPanelPerformanceTest(this);
            //GroupTest(this);
            //PlaceholderTest(this);
            //SizeTest(this);
            ConstraintLayoutInScrollViewTest(this);
        }

        void ConstraintLayoutInScrollViewTest(ConstraintLayout page)
        {
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
                    .Width(FluentConstraintSet.SizeBehavier.MatchParent)
                    .Height(FluentConstraintSet.SizeBehavier.MatchParent);
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
                    .Width(FluentConstraintSet.SizeBehavier.MatchParent)
                    .Height(1000)
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
                    .Width(FluentConstraintSet.SizeBehavier.MatchParent)
                    .Height(FluentConstraintSet.SizeBehavier.MatchParent);
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
        private void createControls()
        {
            FirstButton = new Button()
            {
                Tag = nameof(FirstButton),
                ClickMode = ClickMode.Press,
                Content = "FirstButton",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Red)
            };

            SecondButton = new Button()
            {
                //Width = 100,Height = 100,
                Tag = nameof(SecondButton),
                Content = "SecondBotton",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
            };

            ThirdCanvas = new Canvas()
            {
                Tag = nameof(ThirdCanvas),
                //Width = 100,
                //Height = 100,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.LightGreen)
            };

            FouthTextBlock = new TextBlock()
            {
                Tag = nameof(FouthTextBlock),
                Text = "FourthTextBlock",
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            FifthTextBox = new TextBox()
            {
                Tag = nameof(FifthTextBox),
                Text = "FifthTextBox",
                AcceptsReturn = true
            };

            //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
            SixthRichTextBlock = new RichTextBlock()
            {
                Tag = nameof(SixthRichTextBlock),
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