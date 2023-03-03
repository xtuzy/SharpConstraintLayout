
using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SharpConstraintLayout.Maui.Helper.Widget;
using SharpConstraintLayout.Maui.Native.Example.Tool;
using SharpConstraintLayout.Maui.DebugTool;
#if __ANDROID__
using Android.Views;
using Android.Widget;
#elif __IOS__
using UIKit;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
#endif
namespace SharpConstraintLayout.Maui.Native.Example
{
    public partial class MainPage
    {
        void PlaceholderTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            Placeholder.IsEditMode = true;
            Placeholder placeHolder = null;
#if WINDOWS || __IOS__
            placeHolder = new Placeholder();
#elif __ANDROID__
            placeHolder = new Placeholder(page.Context);
#endif
            page.AddElement(placeHolder, FifthTextBox);
            placeHolder.SetContent(FifthTextBox);

            using (var constraintSet = new FluentConstraintSet())
            {
                constraintSet.Clone(page);
                constraintSet.Select(placeHolder)
                    .CenterTo()
                    .MinWidth(300).MinHeight(300)
                    .Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent);
                constraintSet.ApplyTo(page);
            }
        }

        void GroupTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            Group group = null;
#if WINDOWS || __IOS__
            group = new Group();
#elif __ANDROID__
            group = new Group(page.Context);
#endif
            page.AddElement(FirstButton, group, SecondButton, ThirdCanvas, FouthTextBlock);
            group.RefElement(SecondButton);
            group.RefElement(ThirdCanvas);
            group.RefElement(FouthTextBlock);
            using (var constraintSet = new FluentConstraintSet())
            {
                constraintSet.Clone(page);
                constraintSet.Select(FirstButton).CenterTo()
                    .Select(SecondButton).LeftToRight(FirstButton).CenterYTo(FirstButton)
                    .Select(ThirdCanvas).RightToLeft(FirstButton).CenterYTo(FirstButton)
                    .PercentWidth(0.2f).PercentWidth(0.3f)
                    .Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint)
                    .Select(FouthTextBlock).LeftToRight(FirstButton).TopToBottom(FirstButton);
                constraintSet.ApplyTo(page);
            }

#if WINDOWS || __ANDROID__
            FirstButton.Click += (s, e) =>
#elif __IOS__
            FirstButton.TouchUpInside += (s, e) =>
#endif
                        {
                            if (group.ConstrainVisibility == ConstraintSet.Gone)
                            {
                                group.ConstrainVisibility = ConstraintSet.Visible;
                            }
                            else
                            {
                                group.ConstrainVisibility = ConstraintSet.Gone;
                            }
                        };
        }

        void FlowPerformanceTest(ConstraintLayout rootPage)
        {
#if ANDROID
            var page = new ConstraintLayout(rootPage.Context);
#else
            var page = new ConstraintLayout();
#endif
            rootPage.AddElement(page);
            using(var rootSet = new FluentConstraintSet())
            {
                rootSet.Clone(rootPage);
                rootSet.Select(page)
                    .Width(SizeBehavier.MatchParent)
                    .Height(SizeBehavier.MatchParent);
                rootSet.ApplyTo(rootPage);
            }
            page.ConstrainWidth = (int)SizeBehavier.MatchParent;
            page.ConstrainHeight = (int)SizeBehavier.MatchParent;
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            int buttonCount = 50;
#if __ANDROID__
            var flow = new Flow(page.Context) { };
#else
            var flow = new Flow() { };
#endif
            flow.SetOrientation(Flow.Horizontal);
            flow.SetWrapMode(Flow.WrapChain);
            flow.SetHorizontalStyle(Flow.ChainPacked);
            page.AddElement(flow);

            page.AddElement(FifthTextBox);
            flow.RefElement(FifthTextBox);
            //Generate 1000 Button,all add to page
#if WINDOWS
            var buttonList = new List<Button>();
            for (int i = 0; i < buttonCount; i++)
            {
                var button = new Button();
                button.Content = "Button" + i;
                buttonList.Add(button);
                page.AddElement(button);
                flow.RefElement(button);
            }
#elif __ANDROID__
            var buttonList = new List<Button>();
            for (int i = 0; i < buttonCount; i++)
            {
                var button = new Button(page.Context);
                button.Text = "Button" + i;
                buttonList.Add(button);
                page.AddElement(button);
                flow.RefElement(button);
            }
#elif __IOS__
            var buttonList = new List<UIButton>();
            for (int i = 0; i < buttonCount; i++)
            {
                var button = new UIButton();
                button.SetTitle("Button" + i, UIControlState.Normal);
                buttonList.Add(button);
                page.AddElement(button);
                flow.RefElement(button);
            }
#endif
            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(page);
                layoutSet
                    .Select(flow)
                    .TopToTop().BottomToBottom().LeftToLeft()
                    .Width(SizeBehavier.WrapContent)
                    .Height(SizeBehavier.WrapContent);
                layoutSet.ApplyTo(page);
            }
        }

        void WrapPanelPerformanceTest(ConstraintLayout page)
        {
          /*  (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            ConstraintLayout.MEASURE_MEASURELAYOUT = false;
            //WrapPanel.DEBUG = true;
            WrapPanel.MEASURE = true;
            int buttonCount = 50;
#if __ANDROID__
            var wrapPanel = new WrapPanel(page.Context) { Orientation = WrapPanel.LayoutOrientation.X, Padding = new Microsoft.Maui.Graphics.Rect(10, 10, 10, 10), HorizontalSpacing = 10, VerticalSpacing = 10 };
#else
            var wrapPanel = new WrapPanel() { Orientation = WrapPanel.LayoutOrientation.X, Padding = new Microsoft.Maui.Graphics.Rect(10, 10, 10, 10), HorizontalSpacing = 10, VerticalSpacing = 10 };
#endif
            wrapPanel.AddElement(FifthTextBox);
            page.AddElement(wrapPanel);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(page);
                set
                    .Select(wrapPanel)
                    .TopToTop(page).BottomToBottom().LeftToLeft()
                    .Width(SizeBehavier.MatchParent)
                    .Height(SizeBehavier.WrapContent);
                set.ApplyTo(page);
            }
#if WINDOWS
            var buttonList = new List<Button>();
            for (int i = 0; i < buttonCount; i++)
            {

                var button = new Button();
                button.Content = "Button" + i;
                buttonList.Add(button);
                wrapPanel.AddElement(button);
            }
#elif __ANDROID__
            var buttonList = new List<Button>();
            for (int i = 0; i < buttonCount; i++)
            {
                var button = new Button(page.Context);
                button.Text = "Button" + i;
                buttonList.Add(button);
                wrapPanel.AddView(button);
            }
#elif __IOS__
            wrapPanel.UserInteractionEnabled = true;
            wrapPanel.BackgroundColor = UIColor.Gray;
            FifthTextBox.Enabled = true;
            FifthTextBox.UserInteractionEnabled = true;
            var buttonList = new List<UIButton>();
            for (int i = 0; i < buttonCount; i++)
            {
                var button = new UIButton();
                button.SetTitle("Button" + i, UIControlState.Normal);
                buttonList.Add(button);
                wrapPanel.AddElement(button);
            }
#endif*/
        }

        void CircleConstraintTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            var layout = page;
            layout.AddElement(FirstButton, SecondButton, FouthTextBlock);
            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(layout);
                layoutSet.Select(FirstButton).CenterTo()
                    .Select(SecondButton, FouthTextBlock)
                .CircleTo(FirstButton, new[] { 100, 100 }, new[] { 60f, 240 });
                layoutSet.ApplyTo(layout);
            }

            Task.Run(async () =>
            {
                int index = 0;
                while (index < 1)
                {
                    await Task.Delay(3000);//wait UI show
                    UIThread.Invoke(() =>
                    {
                        //The distance between SecondButton center and FirstButton center should equal to 50
                        SimpleTest.AreEqual(Math.Abs(SecondButton.GetBounds().Center.Y - FirstButton.GetBounds().Center.Y) * 2, 100, nameof(CircleConstraintTest), "The distence between SecondButton center and FirstButton center should equal to 100");
                        //The distance between FouthTextBlock center and FirstButton center should equal to 50
                        SimpleTest.AreEqual(Math.Abs(FouthTextBlock.GetBounds().Center.Y - FirstButton.GetBounds().Center.Y) * 2, 100, nameof(CircleConstraintTest), "The distence between FouthTextBlock center and FirstButton center should equal to 100");
                    }, page);

                    index++;
                }
            });
        }

        void PlatformLayoutInConstraintLayoutTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            page.ConstrainPaddingTop = 10;
            page.ConstrainPaddingBottom = 10;
            page.ConstrainPaddingLeft = 10;
            page.ConstrainPaddingRight = 10;

#if ANDROID
            var stackPanel = new LinearLayout(page.Context)
            {
                Orientation = Android.Widget.Orientation.Horizontal,
                Background = new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Coral),
            };
            page.AddElement(stackPanel);
            stackPanel.AddView(new Button(page.Context) { Text = "InStackPanel" });
            stackPanel.AddView(new Button(page.Context) { Text = "InStackPanel" });
            stackPanel.AddView(new TextView(page.Context) { Text = "InStackPanel" });
            stackPanel.AddView(new EditText(page.Context) { Text = "InStackPanel" });
            var grid = new GridLayout(page.Context) { };
            page.AddElement(grid);
            grid.ColumnCount = 2;
            grid.RowCount = 2;
            grid.AddView(new TextView(page.Context) { Text = "InGrid", LayoutParameters = new GridLayout.LayoutParams(GridLayout.InvokeSpec(0, 1, 1), GridLayout.InvokeSpec(0, 1, 1)) });
            grid.AddView(new TextView(page.Context) { Text = "InGrid", LayoutParameters = new GridLayout.LayoutParams(GridLayout.InvokeSpec(0, 1, 1), GridLayout.InvokeSpec(1, 1, 1)) });
            grid.AddView(new TextView(page.Context) { Text = "InGrid", LayoutParameters = new GridLayout.LayoutParams(GridLayout.InvokeSpec(1, 1, 1), GridLayout.InvokeSpec(0, 1, 1)) });
            grid.AddView(new EditText(page.Context) { Text = "InGrid", LayoutParameters = new GridLayout.LayoutParams(GridLayout.InvokeSpec(1, 1, 1), GridLayout.InvokeSpec(1, 1, 1)) });
            var scrollView = new ScrollView(page.Context);
            page.AddElement(scrollView);
            scrollView.AddView(new LinearLayout(page.Context)
            {
                Orientation = Android.Widget.Orientation.Vertical,
                Background = new Android.Graphics.Drawables.ColorDrawable(Android.Graphics.Color.Coral),
            });
            (scrollView.GetChildAt(0) as LinearLayout).AddView(new Button(page.Context) { Text = "InScrollViewer" });
            (scrollView.GetChildAt(0) as LinearLayout).AddView(new Button(page.Context) { Text = "InScrollViewer" });
            (scrollView.GetChildAt(0) as LinearLayout).AddView(new TextView(page.Context) { Text = "InScrollViewer" });
            (scrollView.GetChildAt(0) as LinearLayout).AddView(new EditText(page.Context) { Text = "InScrollViewer" });

#elif __IOS__
            var stackPanel = new UIStackView()
            {
                Axis = UILayoutConstraintAxis.Horizontal,
                BackgroundColor = UIColor.SystemPink,
                Spacing = 10,
            };
            page.AddElement(stackPanel);
            var firstButtonInStackPanel = new UIButton() { BackgroundColor = UIColor.Green };
            firstButtonInStackPanel.SetTitle("InStackPanel", UIControlState.Normal);
            stackPanel.AddArrangedSubview(firstButtonInStackPanel);
            var secondButtonInStackPanel = new UIButton() { BackgroundColor = UIColor.Green };
            secondButtonInStackPanel.SetTitle("InStackPanel", UIControlState.Normal);
            stackPanel.AddArrangedSubview(secondButtonInStackPanel);
            stackPanel.AddArrangedSubview(new UILabel() { Text = "InStackPanel" });
            stackPanel.AddArrangedSubview(new UITextField() { Text = "InStackPanel" });

            var grid = new UIView()
            {
                BackgroundColor = UIColor.SystemPink,
                //TranslatesAutoresizingMaskIntoConstraints = false
            };
            page.AddSubview(grid);
            var label = new UILabel() { Text = "InGrid" };
            var textbox = new UITextField() { Text = "InGrid" };
            grid.AddSubview(label);
            grid.AddSubview(textbox);
            label.TranslatesAutoresizingMaskIntoConstraints = false;
            textbox.TranslatesAutoresizingMaskIntoConstraints = false;
            label.TopAnchor.ConstraintEqualTo(grid.TopAnchor).Active = true;
            label.LeftAnchor.ConstraintEqualTo(grid.LeftAnchor).Active = true;
            textbox.TopAnchor.ConstraintEqualTo(label.BottomAnchor).Active = true;
            textbox.LeftAnchor.ConstraintEqualTo(label.RightAnchor).Active = true;
            textbox.RightAnchor.ConstraintEqualTo(grid.RightAnchor).Active = true;
            textbox.BottomAnchor.ConstraintEqualTo(grid.BottomAnchor).Active = true;

            var scrollView = new UIScrollView()
            {
                BackgroundColor = UIColor.SystemPink,
                ContentSize = new CoreGraphics.CGSize(300, 500),
            };
            page.AddSubview(scrollView);
            scrollView.AddSubview(new UIStackView()
            {
                Axis = UILayoutConstraintAxis.Vertical,
                BackgroundColor = UIColor.Brown,
                Spacing = 20,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Frame = new CoreGraphics.CGRect(0, 0, 300, 500),
            });

            //scrollView.Subviews[0].LeftAnchor.ConstraintEqualTo(scrollView.LeftAnchor).Active = true;
            //scrollView.Subviews[0].RightAnchor.ConstraintEqualTo(scrollView.RightAnchor).Active = true;
            //scrollView.Subviews[0].TopAnchor.ConstraintEqualTo(scrollView.TopAnchor).Active = true;
            //scrollView.Subviews[0].BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor).Active = true;

            var firstButtonInScrollView = new UIButton() { BackgroundColor = UIColor.Gray };
            firstButtonInScrollView.SetTitle("InScrollViewer", UIControlState.Normal);
            (scrollView.Subviews[0] as UIStackView).AddArrangedSubview(firstButtonInScrollView);
            var secondButtonInScrollView = new UIButton() { BackgroundColor = UIColor.Gray };
            secondButtonInScrollView.SetTitle("InScrollViewer", UIControlState.Normal);
            (scrollView.Subviews[0] as UIStackView).AddArrangedSubview(secondButtonInScrollView);
            (scrollView.Subviews[0] as UIStackView).AddArrangedSubview(new UILabel() { Text = "InScrollViewer" });
            (scrollView.Subviews[0] as UIStackView).AddArrangedSubview(new UITextField() { Text = "InScrollViewer" });

            scrollView.AddSubview(new UILabel() { Text = "InScrollViewer" });

            Task.Run(async () =>
            {

                await Task.Delay(3000);//wait ui show
                UIThread.Invoke(() =>
                {
                    DebugTool.SimpleDebug.WriteLine("scrollView.ContentSize:" + scrollView.ContentSize);
                }, page);
            });
#else
            var stackPanel = new StackPanel()
            {
                Orientation = Microsoft.UI.Xaml.Controls.Orientation.Horizontal,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Coral),
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
            };
            page.AddElement(stackPanel);
            stackPanel.Children.Add(new Button() { Content = "InStackPanel" });
            stackPanel.Children.Add(new Button() { Content = "InStackPanel" });
            stackPanel.Children.Add(new TextBlock() { Text = "InStackPanel" });
            stackPanel.Children.Add(new TextBox() { Text = "InStackPanel" });

            var grid = new Grid();
            page.AddElement(grid);
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new Microsoft.UI.Xaml.GridLength(1, Microsoft.UI.Xaml.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new Microsoft.UI.Xaml.GridLength(1, Microsoft.UI.Xaml.GridUnitType.Star) });
            var textblock = new TextBlock() { Text = "InGrid" };
            var textbox = new TextBox() { Text = "InGrid" };
            textblock.SetValue(Grid.ColumnProperty, 0);//https://stackoverflow.com/a/27756061/13254773
            textbox.SetValue(Grid.ColumnProperty, 1);//https://stackoverflow.com/a/27756061/13254773
            grid.Children.Add(textblock);
            grid.Children.Add(textbox);

            var scrollView = new ScrollViewer();
            page.AddElement(scrollView);
            scrollView.Content = new StackPanel()
            {
                Orientation = Microsoft.UI.Xaml.Controls.Orientation.Vertical,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Coral),
            };
            (scrollView.Content as StackPanel).Children.Add(new Button() { Content = "InScrollViewer" });
            (scrollView.Content as StackPanel).Children.Add(new Button() { Content = "InScrollViewer" });
            (scrollView.Content as StackPanel).Children.Add(new TextBlock() { Text = "InScrollViewer" });
            (scrollView.Content as StackPanel).Children.Add(new TextBox() { Text = "InScrollViewer" });

#endif
            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(page);
                layoutSet.Select(stackPanel)
                    .CenterYTo().LeftToLeft()
                    .Width(SizeBehavier.WrapContent)
                    .Height(SizeBehavier.WrapContent)
                    .Select(grid)
                    .TopToBottom(stackPanel, 10).LeftToLeft()
                    .Width(SizeBehavier.WrapContent)
                    .Height(SizeBehavier.WrapContent)
                    .Select(scrollView)
                    .TopToBottom(grid, 10).BottomToBottom().LeftToLeft()
                    .Width(SizeBehavier.WrapContent)//ios don't know size
                    .Height(SizeBehavier.MatchConstraint);
                layoutSet.ApplyTo(page);
            }
        }

        void AnimationTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();
            ConstraintLayout layout;
#if __ANDROID__
            layout = new ConstraintLayout(page.Context)
            {
                Id = View.GenerateViewId(),
            };
            layout.SetBackgroundColor(Android.Graphics.Color.Black);
#else
            layout = new ConstraintLayout()
            {
#if WINDOWS
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
#elif __IOS__
                BackgroundColor = UIColor.Black
#endif
            };
#endif

#if __ANDROID__
            var guide = new Guideline(page.Context) { Id = View.GenerateViewId() };
#else
            var guide = new Guideline();
#endif

            page.AddElement(layout, guide);

            var pageSet = new FluentConstraintSet();
            pageSet.Clone(page);

#if ANDROID
            Android.Animation.ValueAnimator animator = Android.Animation.ValueAnimator.OfInt(0, 50);
            animator.SetDuration(5000);
            animator.Start();
            animator.Update += (object sender, Android.Animation.ValueAnimator.AnimatorUpdateEventArgs e) =>
            {
                int newValue = (int)e.Animation.AnimatedValue;
                // Apply this new value to the object being animated.
                pageSet.Select(guide)
                    .GuidelineOrientation(Widget.Orientation.X)
                    .GuidelinePercent(newValue / 100f)
                     .Select(layout).LeftToRight(guide).RightToRight(page).TopToTop(page).BottomToBottom(page)
                     .Width(SizeBehavier.MatchConstraint)
                     .Height(SizeBehavier.MatchConstraint);
                pageSet.ApplyTo(page);
            };
#elif IOS

#elif WINDOWS
            //TODO:WinUI value animation
            Microsoft.UI.Xaml.Media.CompositionTarget.Rendering += (sender, e) =>
            {

            };
#endif
        }

        /// <summary>
        /// test nested layout warp content and it's child match parent
        /// </summary>
        /// <param name="page"></param>
        void NestedConstraintLayoutTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();
#if __ANDROID__
            var leftLayout = new ConstraintLayout(page.Context)
#else
            var leftLayout = new ConstraintLayout()
#endif
            {
                ConstrainPaddingTop = 10,
                ConstrainPaddingBottom = 10,
                ConstrainPaddingLeft = 10,
                ConstrainPaddingRight = 10,
                DebugName = "left",
            };
#if __ANDROID__
            var rightLayout = new ConstraintLayout(page.Context)
#else
            var rightLayout = new ConstraintLayout()
#endif
            {
                ConstrainPaddingTop = 10,
                ConstrainPaddingBottom = 10,
                ConstrainPaddingLeft = 10,
                ConstrainPaddingRight = 10,
                DebugName = "right",
            };

            using (var pageSet = new FluentConstraintSet())
            {
                page.AddElement(leftLayout, rightLayout);
                pageSet.Clone(page);
                pageSet.Select(leftLayout)
                    .EdgesYTo(null, 10).LeftToLeft(null, 10).RightToLeft(rightLayout, 5)
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.MatchParent)
                    .Select(rightLayout).EdgesYTo(null, 10).LeftToRight(leftLayout, 5).RightToRight(null, 10)
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.MatchConstraint)
                    ;

                pageSet.ApplyTo(page);
#if __ANDROID__
                var leftChildView = new ConstraintLayout(page.Context)
#else
                var leftChildView = new ConstraintLayout()
#endif
                {
                    DebugName = "Second",
                };
                leftLayout.AddElement(leftChildView);
                using (var layoutSet = new FluentConstraintSet())
                {
                    layoutSet.Clone(leftLayout);
                    layoutSet
                        .Select(leftChildView).EdgesTo(null, 20, 20)
                        .Width(SizeBehavier.MatchConstraint)
                        .Height(SizeBehavier.MatchConstraint);
                    layoutSet.ApplyTo(leftLayout);
                }
#if __ANDROID__
                var rightChildView = new ConstraintLayout(page.Context)
#else
                var rightChildView = new ConstraintLayout()
#endif
                {
                    DebugName = "Second",
                    ConstrainWidth = ConstraintSet.WrapContent,
                };
                rightLayout.AddElement(rightChildView);
                rightChildView.AddElement(FirstButton);
                using (var layoutSet = new FluentConstraintSet())
                {
                    layoutSet.Clone(rightLayout);
                    layoutSet
                        .Select(rightChildView).EdgesTo(null, 20, 20)
                        .Width(SizeBehavier.WrapContent)
                        .Height(SizeBehavier.MatchConstraint);
                    layoutSet.ApplyTo(rightLayout);

                    using (var set = new FluentConstraintSet())
                    {
                        set.Clone(rightChildView);
                        set.Select(FirstButton).CenterTo();
                        set.ApplyTo(rightChildView);
                    }
                }

#if __ANDROID__
                leftLayout.SetBackgroundColor(Android.Graphics.Color.Red);
                rightLayout.SetBackgroundColor(Android.Graphics.Color.Green);
                leftChildView.SetBackgroundColor(Android.Graphics.Color.Yellow);
                rightChildView.SetBackgroundColor(Android.Graphics.Color.Yellow);

#elif __IOS__
                leftLayout.BackgroundColor = UIColor.Red;
                rightLayout.BackgroundColor = UIColor.Green;
                leftChildView.BackgroundColor = UIColor.Yellow;
                rightChildView.BackgroundColor = UIColor.Yellow;

#else
                leftLayout.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Red);
                rightLayout.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Green);
                leftChildView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Yellow);
                rightChildView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Yellow);
#endif
            }
        }

        void FlowTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

#if __ANDROID__
            var flow = new Flow(page.Context)
            {
                Id = View.GenerateViewId(),
            };
#else
            var flow = new Flow();
#endif
#if __IOS__
            // Create a new tap gesture
            var flowClickGesture = new UITapGestureRecognizer();
            // Wire up the event handler (have to use a selector)
            flowClickGesture.AddTarget(() => { SimpleDebug.WriteLine("Touch iOS Flow!"); });  // to be defined                                                       // Add the gesture recognizer to the view
            flow.AddGestureRecognizer(flowClickGesture);
            var canvasClickGesture = new UITapGestureRecognizer();
            // Wire up the event handler (have to use a selector)
            canvasClickGesture.AddTarget(() => { SimpleDebug.WriteLine("Touch iOS Canvas!"); });  // to be defined                                                                              // Add the gesture recognizer to the view
            ThirdCanvas.AddGestureRecognizer(canvasClickGesture);
#endif
            flow.SetOrientation(Flow.Vertical);
            flow.SetWrapMode(Flow.WrapChain);
            flow.SetHorizontalStyle(Flow.ChainSpreadInside);
            ConstraintLayout layout = page;
            layout.AddElement(flow, ThirdCanvas, FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock);
            flow.RefElement(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock);

            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(layout);
                layoutSet
                    .Select(flow)
                    .EdgesXTo()
                    .CenterYTo()
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.WrapContent)
                    .Select(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock)
                    .Width(SizeBehavier.WrapContent)
                    .Height(SizeBehavier.WrapContent)
                    .Select(ThirdCanvas).EdgesTo(flow)
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.MatchConstraint);
                //.Visibility(FluentConstraintSet.Visibility.Visible);
                layoutSet.ApplyTo(layout);
            }

            Task.Run(async () =>
            {
                int index = 0;
                while (index < 1)//test 20 times,you can edit textbox
                {
                    await Task.Delay(3000);//wait ui show
                    UIThread.Invoke(() =>
                    {
                        SimpleTest.AreEqual(flow.GetBounds().Left, ThirdCanvas.GetBounds().Left, nameof(FlowTest), "Canvas Left position should equal to Flow");
                        SimpleTest.AreEqual(flow.GetBounds().Right, ThirdCanvas.GetBounds().Right, nameof(FlowTest), "Canvas Right position should equal to Flow");
                        SimpleTest.AreEqual(flow.GetBounds().Top, ThirdCanvas.GetBounds().Top, nameof(FlowTest), "Canvas Top position should equal to Flow");
                        SimpleTest.AreEqual(flow.GetBounds().Bottom, ThirdCanvas.GetBounds().Bottom, nameof(FlowTest), "Canvas Bottom position should equal to Flow");
                    }, page);

                    index++;
                }
            });
        }

        private void VisibilityTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            ConstraintLayout layout = page;

            layout.AddElement(FirstButton, SecondButton, ThirdCanvas);

            var layoutSet = new FluentConstraintSet();
            layoutSet.Clone(layout);
            layoutSet
                .Select(FirstButton).CenterTo()
                .Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                .Select(SecondButton).LeftToRight(FirstButton).CenterYTo(FirstButton)
                .Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                .Select(ThirdCanvas).LeftToRight(SecondButton).RightToRight().EdgesYTo()
                .Width(SizeBehavier.MatchConstraint)
                .Height(SizeBehavier.MatchConstraint);
            layoutSet.ApplyTo(layout);

            int index = 2;
#if WINDOWS || ANDROID
            FirstButton.Click += (sender, e) =>
#elif __IOS__
            FirstButton.TouchUpInside += (sender, e) =>
#endif
            {
                if (index == 1)
                {
                    layoutSet.Select(SecondButton).Visibility(Widget.Visibility.Visible);
                    index++;
                    Task.Run(async () =>
                    {
                        await Task.Delay(300);//wait ui show
                        UIThread.Invoke(() =>
                        {
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right + SecondButton.GetBounds().Width, ThirdCanvas.GetBounds().Left, nameof(BarrierTest), "When Center Button Visiable, Canvas position should equal to FirstButton+SecondButton");
                        }, page);
                    });
                }
                else if (index == 2)
                {
                    layoutSet.Select(SecondButton).Visibility(Widget.Visibility.Invisible);
                    index++;
                    Task.Run(async () =>
                    {
                        await Task.Delay(300);//wait ui show
                        UIThread.Invoke(() =>
                        {
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right + SecondButton.GetBounds().Width, ThirdCanvas.GetBounds().Left, nameof(BarrierTest), "When Center Button Invisiable, Canvas position should equal to FirstButton+SecondButton");
                        }, page);
                    });
                }
                else if (index == 3)
                {
                    layoutSet.Select(SecondButton).Visibility(Widget.Visibility.Gone);
                    index = 1;
                    Task.Run(async () =>
                    {
                        await Task.Delay(300);//wait ui show
                        UIThread.Invoke(() =>
                        {
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right, ThirdCanvas.GetBounds().Left, nameof(BarrierTest), "When Center Button Gone, Canvas position should equal to FirstButton");
                        }, page);
                    });
                }
                layoutSet.ApplyTo(layout);
            };
        }

        private void BarrierTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();
            ConstraintLayout layout;
            layout = page;
#if __ANDROID__
            var barrier = new Barrier(page.Context)
            {
                Id = View.GenerateViewId(),
            };
            FifthTextBox.LayoutParameters = new ConstraintLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
#else
            var barrier = new Barrier();
#endif
            layout.AddElement(FifthTextBox, ThirdCanvas, barrier);

            var layoutSet = new FluentConstraintSet();
            layoutSet.Clone(layout);
            layoutSet
                .Select(FifthTextBox).CenterYTo().CenterXTo()
                .Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                .Select(barrier).Barrier(Direction.Right, 0, new[] { FifthTextBox })
                .Select(ThirdCanvas).LeftToRight(barrier).RightToRight()
                .Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchParent);
            layoutSet.ApplyTo(layout);

            Task.Run(async () =>
            {
                int index = 0;
                while (index < 5)//test 20 times,you can edit textbox
                {
                    await Task.Delay(3000);//wait ui show
                    UIThread.Invoke(() =>
                    {
                        SimpleTest.AreEqual(FifthTextBox.GetBounds().Right, barrier.GetBounds().Left, nameof(BarrierTest), "Barrier position should equal to TextBox");
                        SimpleTest.AreEqual(ThirdCanvas.GetBounds().Left, barrier.GetBounds().Left, nameof(BarrierTest), "Canvas position should equal to Barrier");
#if __IOS__
                        SimpleTest.AreEqual(FifthTextBox.Subviews.Length, 0, nameof(BarrierTest), "FifthTextBox should not have subview");
#endif
                    }, page);

                    index++;
                }
            });
        }

        private void GuidelineTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

#if __ANDROID__
            var layout = new ConstraintLayout(page.Context)
            {
                Id = View.GenerateViewId(),
            };
            layout.SetBackgroundColor(Android.Graphics.Color.Black);

#elif WINDOWS
            var layout = new ConstraintLayout()
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
            };
#elif __IOS__
            var layout = new UIView()
            {
                BackgroundColor = UIColor.Black
            };
#endif

#if __ANDROID__
            var guide = new Guideline(page.Context) { Id = View.GenerateViewId() };
#else
            var guide = new Guideline();
#endif

            page.AddElement(layout, guide);

            var pageSet = new FluentConstraintSet();
            pageSet.Clone(page);
            pageSet.Select(guide).GuidelineOrientation(Widget.Orientation.Y).GuidelinePercent(0.5f)
            .Select(layout).LeftToRight(guide).RightToRight(page).TopToTop(page).BottomToBottom(page)
            .Width(SizeBehavier.MatchConstraint)
            .Height(SizeBehavier.MatchConstraint);
            pageSet.ApplyTo(page);

            Task.Run(async () =>
            {
                await Task.Delay(3000);//wait ui show
                UIThread.Invoke(() =>
                {
                    SimpleTest.AreEqual(page.GetSize().Width / 2, layout.GetBounds().X, nameof(GuidelineTest), "Horiable guideline should at center");
                }, page);
            });
        }

        private void BaselineTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();
            ConstraintLayout layout;
            layout = page;

            layout.AddElement(FouthTextBlock, SixthRichTextBlock);

            var layoutSet = new FluentConstraintSet();
            layoutSet.Clone(layout);
            layoutSet
                .Select(layout)
                .Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint)
                .Select(FouthTextBlock).CenterYTo().CenterXTo()
                .Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                .Select(SixthRichTextBlock).RightToLeft(FouthTextBlock, 50).BaselineToBaseline(FouthTextBlock)
                .Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent);
            layoutSet.ApplyTo(layout);
        }

        private void BaseAlignTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            ConstraintLayout layout = page;

            layout.AddElement(FirstButton, SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock);

            var layoutSet = new FluentConstraintSet();
            layoutSet.Clone(layout);
            layoutSet
                .Select(FirstButton).CenterTo()
                .Select(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock)
                .Width(SizeBehavier.WrapContent)
                .Height(SizeBehavier.WrapContent)
                .Select(ThirdCanvas).Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint)
                .Select(SecondButton).RightToRight(FirstButton).TopToBottom(FirstButton)
                .Select(ThirdCanvas).LeftToRight(FirstButton).RightToRight().EdgesYTo()
                .Select(FouthTextBlock).RightToRight(SecondButton).TopToBottom(SecondButton)
                .Select(FifthTextBox).BottomToTop(FirstButton).LeftToLeft(FirstButton).RightToRight(FirstButton)
                .Select(SixthRichTextBlock).RightToLeft(FouthTextBlock).BaselineToBaseline(FouthTextBlock);
            layoutSet.ApplyTo(layout);

            Task.Run(async () =>
            {
                await Task.Delay(3000);//wait UI change or show
                UIThread.Invoke(() =>
                {
                    //FirstButton at page's Center
                    SimpleTest.AreEqual(page.GetSize().Width / 2, FirstButton.GetBounds().X + FirstButton.GetSize().Width / 2, nameof(BaseAlignTest), "FirstButton should at horizontal center");
                    SimpleTest.AreEqual(page.GetSize().Height / 2, FirstButton.GetBounds().Y + FirstButton.GetSize().Height / 2, nameof(BaseAlignTest), "FirstButton should at vertical center");
                    //SecondButton's Right equal to FirstButton's Right. SecondButton's Top equal to FirstButton's Bottom
                    SimpleTest.AreEqual(FirstButton.GetBounds().Right, SecondButton.GetBounds().Right, nameof(BaseAlignTest), "SecondButton's Right should equal to FirstButton's Right");
                    SimpleTest.AreEqual(FirstButton.GetBounds().Bottom, SecondButton.GetBounds().Top, nameof(BaseAlignTest), "SecondButton's Top should equal to FirstButton's Bottom");
                    //ThirdCanvas's Left equal to FirstButton's Right. ThirdCanvas's Right equal to page's Right. ThirdCanvas's Top equal to page's Top, ThirdCanvas's Bottom equal to page's Bottom
                    SimpleTest.AreEqual(FirstButton.GetBounds().Right, ThirdCanvas.GetBounds().Left, nameof(BaseAlignTest), "ThirdCanvas's Left should equal to FirstButton's Right");
                    SimpleTest.AreEqual(page.GetBounds().Right, ThirdCanvas.GetBounds().Right, nameof(BaseAlignTest), "ThirdCanvas's Right should equal to page's Right");
                    SimpleTest.AreEqual(page.GetBounds().Top, ThirdCanvas.GetBounds().Top, nameof(BaseAlignTest), "ThirdCanvas's Top should equal to page's Top");
                    SimpleTest.AreEqual(page.GetBounds().Bottom, ThirdCanvas.GetBounds().Bottom, nameof(BaseAlignTest), "ThirdCanvas's Bottom should equal to page's Bottom");
                }, page);
            });
        }

        private void SizeTest(ConstraintLayout page)
        {
            (var FirstButton, var SecondButton, var ThirdCanvas, var FouthTextBlock, var FifthTextBox, var SixthRichTextBlock) = CreateControls();

            page.ConstrainPaddingTop = 10;
            page.ConstrainPaddingBottom = 10;
            page.ConstrainPaddingLeft = 10;
            page.ConstrainPaddingRight = 10;
            page.AddElement(FirstButton, SecondButton, ThirdCanvas, FifthTextBox);
            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(page);
                layoutSet
                    .Select(FirstButton).CenterTo()
                    .Select(SecondButton).LeftToLeft(FirstButton).TopToBottom(FirstButton)
                    .MinWidth(200).MinHeight(100)
                    .MaxHeight(200)
                    .Width(100).Height(300)
                    .Select(ThirdCanvas)
                    .LeftToRight(FirstButton).RightToRight().TopToTop(FirstButton)
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.MatchConstraint)
                    .WHRatio("2:1")
                    .Select(FifthTextBox)
                    .LeftToLeft(FirstButton).BottomToTop(FirstButton)
                    .MinWidth(100)
                    .MaxWidth(200);
                layoutSet.ApplyTo(page);
            }

            Task.Run(async () =>
            {
                await Task.Delay(3000);//wait UI change or show
                UIThread.Invoke(() =>
                {
                    //SecondButton Height shound be equal to 200
                    SimpleTest.AreEqual(200, SecondButton.GetSize().Height, nameof(SizeTest), "SecondButton's height should be 200");
                    //SecondButton Width shound be equal to 200
                    SimpleTest.AreEqual(200, SecondButton.GetSize().Width, nameof(SizeTest), "SecondButton's width should be 200");
                    //The height of ThirdCanvas should be equal to the width of ThirdCanvas divided by 2
                    SimpleTest.AreEqual(ThirdCanvas.GetSize().Width / 2, ThirdCanvas.GetSize().Height, nameof(SizeTest), "ThirdCanvas's height should be equal to ThirdCanvas's width divided by 2");

                }, page);
            });
        }
    }
}
