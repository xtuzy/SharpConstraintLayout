using SharpConstraintLayout.Maui.Native.Example.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpConstraintLayout.Maui.Example.Tool;
using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;
using SharpConstraintLayout.Maui.Helper.Widget;

namespace SharpConstraintLayout.Maui.Example
{
    public partial class MainPage
    {
        void FlowPerformanceTest(ConstraintLayout page)
        {
            int buttonCount = 10;

            var flow = new Flow() { };

            flow.SetOrientation(Flow.Horizontal);
            flow.SetWrapMode(Flow.WrapChain);
            flow.SetHorizontalStyle(Flow.ChainPacked);
            page.AddElement(flow);

            page.AddElement(FifthTextBox);
            flow.AddElement(FifthTextBox);
            //Generate 1000 Button,all add to page

            var buttonList = new List<Button>();
            for (int i = 0; i < buttonCount; i++)
            {

                var button = new Button();
                button.Text = "Button" + i;
                buttonList.Add(button);
                page.AddElement(button);
                flow.AddElement(button);
            }

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

        void CircleConstraintTest(ConstraintLayout page)
        {
            layout = page;
            layout.AddElement(FirstButton, SecondButton, FouthTextBlock);
            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(layout);
                layoutSet.Select(FirstButton).CenterTo()
                    .Select(SecondButton, FouthTextBlock)
                .CircleTo(FirstButton, new[] { 50, 100 }, new[] { 60f, 240 });
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
                        SimpleTest.AreEqual(Math.Abs(SecondButton.GetBounds().Center.Y - FirstButton.GetBounds().Center.Y) * 2, 50, nameof(CircleConstraintTest), "The distence between SecondButton center and FirstButton center should equal to 50");
                        //The distance between FouthTextBlock center and FirstButton center should equal to 50
                        SimpleTest.AreEqual(Math.Abs(FouthTextBlock.GetBounds().Center.Y - FirstButton.GetBounds().Center.Y) * 2, 50, nameof(CircleConstraintTest), "The distence between FouthTextBlock center and FirstButton center should equal to 50");
                    }, page);

                    index++;
                }
            });
        }

        void PlatformLayoutInConstraintLayoutTest(ConstraintLayout page)
        {
            var stackPanel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Background = new SolidColorBrush(Colors.Coral),
            };
            page.AddElement(stackPanel);
            stackPanel.Add(new Button() { Text = "InStackPanel" });
            stackPanel.Add(new Button() { Text = "InStackPanel" });
            stackPanel.Add(new Label() { Text = "InStackPanel" });
            stackPanel.Add(new Editor() { Text = "InStackPanel" });

            var grid = new Grid();
            page.AddElement(grid);
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            var textblock = new Label() { Text = "InGrid" };
            var textbox = new Editor() { Text = "InGrid" };
            textblock.SetValue(Grid.ColumnProperty, 0);//https://stackoverflow.com/a/27756061/13254773
            textbox.SetValue(Grid.ColumnProperty, 1);//https://stackoverflow.com/a/27756061/13254773
            grid.Add(textblock);
            grid.Add(textbox);

            var scrollView = new ScrollView();
            page.AddElement(scrollView);
            scrollView.Content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Background = new SolidColorBrush(Colors.Coral),
            };
            (scrollView.Content as StackLayout).Add(new Button() { Text = "InScrollViewer" });
            (scrollView.Content as StackLayout).Add(new Button() { Text = "InScrollViewer" });
            (scrollView.Content as StackLayout).Add(new Label() { Text = "InScrollViewer" });
            (scrollView.Content as StackLayout).Add(new Editor() { Text = "InScrollViewer" });

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
                    .Height(100)
                    .Select(scrollView)
                    .TopToBottom(grid, 10)
                    .BottomToBottom()
                    .LeftToLeft()
                    .Width(SizeBehavier.WrapContent)
                    .Height(SizeBehavier.WrapContent);
                layoutSet.ApplyTo(page);
            }
        }

        void NestedConstraintLayoutTest(ConstraintLayout page)
        {
            layout = new ConstraintLayout()
            {
                Background = new SolidColorBrush(Colors.Black)
            };

            using (var pageSet = new FluentConstraintSet())
            {
                page.AddElement(layout);
                pageSet.Clone(page);
                pageSet.Select(layout)
                    .CenterTo()
                    .Width(ConstraintSet.WrapContent)
                    .Height(ConstraintSet.WrapContent);
                pageSet.ApplyTo(page);
                layout.AddElement(ThirdCanvas);
                layout.AddElement(FirstButton);
                using (var layoutSet = new FluentConstraintSet())
                {
                    layoutSet.Clone(layout);
                    layoutSet
                        .Select(FirstButton).CenterXTo().CenterYTo()
                        .Width(FluentConstraintSet.SizeBehavier.WrapContent)
                        .Height(FluentConstraintSet.SizeBehavier.WrapContent)
                        .Select(ThirdCanvas).EdgesTo(null, 20, 20)
                        .Width(FluentConstraintSet.SizeBehavier.MatchParent)
                        .Height(FluentConstraintSet.SizeBehavier.MatchParent);
                    layoutSet.ApplyTo(layout);
                }
            }
        }

        void FlowTest(ConstraintLayout page)
        {
            var flow = new Flow();

            flow.SetOrientation(Flow.Vertical);
            flow.SetWrapMode(Flow.WrapChain);
            flow.SetHorizontalStyle(Flow.ChainSpreadInside);
            layout = page;
            layout.AddElement(ThirdCanvas, FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock, flow);
            flow.AddElement(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock);

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
            layout = page;

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

            FirstButton.Clicked += (sender, e) =>
            {
                if (index == 1)
                {
                    layoutSet.Select(SecondButton).Visibility(FluentConstraintSet.Visibility.Visible);
                    index++;
                    Task.Run(async () =>
                    {
                        await Task.Delay(300);//wait ui show
                        UIThread.Invoke(() =>
                        {
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right + SecondButton.GetBounds().Width, ThirdCanvas.GetBounds().Left, nameof(VisibilityTest), "When Center Button Visiable, Canvas position should equal to FirstButton+SecondButton");
                        }, page);
                    });
                }
                else if (index == 2)
                {
                    layoutSet.Select(SecondButton).Visibility(FluentConstraintSet.Visibility.Invisible);
                    index++;
                    Task.Run(async () =>
                    {
                        await Task.Delay(300);//wait ui show
                        UIThread.Invoke(() =>
                        {
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right + SecondButton.GetBounds().Width, ThirdCanvas.GetBounds().Left, nameof(VisibilityTest), "When Center Button Invisiable, Canvas position should equal to FirstButton+SecondButton");
                        }, page);
                    });
                }
                else if (index == 3)
                {
                    layoutSet.Select(SecondButton).Visibility(FluentConstraintSet.Visibility.Gone);
                    index = 1;
                    Task.Run(async () =>
                    {
                        await Task.Delay(300);//wait ui show
                        UIThread.Invoke(() =>
                        {
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right, ThirdCanvas.GetBounds().Left, nameof(VisibilityTest), "When Center Button Gone, Canvas position should equal to FirstButton");
                        }, page);
                    });
                }
                layoutSet.ApplyTo(layout);
            };
        }

        private void BarrierTest(ConstraintLayout page)
        {
            layout = page;

            var barrier = new Widget.Barrier();

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
                    }, page);

                    index++;
                }
            });
        }

        private void GuidelineTest(ConstraintLayout page)
        {

            var layout = new ConstraintLayout()
            {
                Background = new SolidColorBrush(Colors.Black)
            };

            var guide = new Guideline();

            page.AddElement(layout, guide);

            var pageSet = new FluentConstraintSet();
            pageSet.Clone(page);
            pageSet.Select(guide).GuidelineOrientation(FluentConstraintSet.Orientation.X).GuidelinePercent(0.5f)
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

        private void BaseAlignTest(ConstraintLayout page)
        {
            layout = page;

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
    }
}
