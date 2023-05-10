using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpConstraintLayout.Maui.Example.Tool;
using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;
using SharpConstraintLayout.Maui.Helper.Widget;
using SharpConstraintLayout.Maui.Example.Models;
using SharpConstraintLayout.Maui.Example.ViewModels;
using SharpConstraintLayout.Maui.Native.Example.Tool;

namespace SharpConstraintLayout.Maui.Example
{
    public partial class MainPage
    {
        void GroupTest(ConstraintLayout page)
        {
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();

            var group = new Group();
            var layout = page;
            layout.AddElement(group, FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock);
            group.RefElement(FirstButton, FouthTextBlock);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(layout);
                set.Select(SecondButton).CenterTo()
                    .Select(FirstButton).RightToLeft(SecondButton).CenterYTo(SecondButton)
                    .Select(FouthTextBlock).LeftToRight(SecondButton).CenterYTo(SecondButton)
                    .Select(FifthTextBox).LeftToRight(FouthTextBlock).CenterYTo(FouthTextBlock)
                    .Select(SixthRichTextBlock).RightToLeft(FirstButton).CenterYTo(FirstButton)
                    ;
                set.ApplyTo(layout);

            }
            SecondButton.Clicked += (sender, e) =>
            {
                if (group.ConstrainVisibility == ConstraintSet.Gone)
                {
                    group.ConstrainVisibility = ConstraintSet.Visible;
                }
                else if (group.ConstrainVisibility == ConstraintSet.Visible)
                {
                    group.ConstrainVisibility = ConstraintSet.Invisible;
                }
                else//Invisible
                {
                    group.ConstrainVisibility = ConstraintSet.Gone;
                }
            };
        }
        void FlowPerformanceTest(ConstraintLayout rootpage)
        {
            var page = new ConstraintLayout(new Log());
            rootpage.AddElement(page);
            page.ConstrainWidth = ConstraintSet.MatchParent;
            page.ConstrainHeight = ConstraintSet.MatchParent;
            using(var rootSet = new FluentConstraintSet())
            {
                rootSet.Clone(rootpage);
                rootSet.Select(page).EdgesTo(rootpage).Width(SizeBehavier.MatchParent).Height(SizeBehavier.MatchParent);
                rootSet.ApplyTo(rootpage);
            }
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();

            int buttonCount = 50;

            var flow = new Flow() { };
            page.AddElement(FirstButton);
            page.AddElement(SecondButton);

            FirstButton.Text = "Add Button";
            FirstButton.Clicked += (sender, e) =>
            {
                var newbutton = new Button();
                newbutton.Text = flow.ReferencedIds.Length.ToString();
                newbutton.BackgroundColor = Colors.Gray;
                page.AddElement(newbutton);
                flow.RefElement(newbutton);
            };

            flow.SetOrientation(SharpConstraintLayout.Maui.Helper.Widget.Flow.Horizontal);
            flow.SetWrapMode(SharpConstraintLayout.Maui.Helper.Widget.Flow.WrapChain);
            flow.SetHorizontalStyle(SharpConstraintLayout.Maui.Helper.Widget.Flow.ChainPacked);
            page.AddElement(flow);

            //page.AddElement(FifthTextBox);
            //flow.RefElement(FifthTextBox);
            //Generate 1000 Button,all add to page

            var buttonList = new List<Button>();
            for (int i = 0; i < buttonCount; i++)
            {

                var button = new Button();
                button.Text = "Button" + i;
                buttonList.Add(button);
                page.AddElement(button);
                flow.RefElement(button);
            }

            SecondButton.Text = "Add Char";
            SecondButton.Clicked += (sender, e) =>
            {
                buttonList[0].Text = buttonList[0].Text + "A";//not use entry to add char, because entry will measure twice
            };

            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(page);
                layoutSet
                    .Select(flow)
                    .TopToTop().BottomToBottom().LeftToLeft()
                    .Width(SizeBehavier.WrapContent)
                    .Height(SizeBehavier.WrapContent)
                    .Select(FirstButton).EdgesXTo()
                    .Select(SecondButton).CenterYTo(FirstButton).LeftToRight(FirstButton)
                    ;
                layoutSet.ApplyTo(page);
            }
        }

        void CircleConstraintTest(ConstraintLayout page)
        {
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();
            FirstButton.Text = "0°";
            SecondButton.Text = "60°";
            FouthTextBlock.Text = "180°";
            FifthTextBox.Text = "240°";
            SixthRichTextBlock.Text = "300°";
            var centerButton = new Button() { Text = "Center" };
            var layout = page;
            layout.AddElement(centerButton,FirstButton, SecondButton,ThirdCanvas, FouthTextBlock,FifthTextBox,SixthRichTextBlock);
            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(layout);
                layoutSet.Select(centerButton).CenterTo()
                    .Select(FirstButton,SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock)
                .CircleTo(centerButton, new[] { 100, 120,140,160,180,200 }, new[] { 0f,60, 120,180,240,300})
                .Select(ThirdCanvas).Width(30).Height(30)
                ;
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
                        SimpleTest.AreEqual(Math.Abs(SecondButton.GetBounds().Center.Y - centerButton.GetBounds().Center.Y) * 2, 100, nameof(CircleConstraintTest));
                        //The distance between FouthTextBlock center and FirstButton center should equal to 50
                        SimpleTest.AreEqual(Math.Abs(FouthTextBlock.GetBounds().Center.Y - centerButton.GetBounds().Center.Y) * 2, 180, nameof(CircleConstraintTest));
                    }, page);

                    index++;
                }
            });
        }

        void ScrollViewInConstraintLayoutTest(ConstraintLayout page)
        {
            var scrollView = new ScrollView();
            page.AddElement(scrollView);
            scrollView.Content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Background = new SolidColorBrush(Colors.Aqua),
            };
            (scrollView.Content as StackLayout).Add(new Button() { Text = "ScrollViewInConstraintLayout" });
            (scrollView.Content as StackLayout).Add(new Editor() { Text = "InScrollViewer", HorizontalOptions = LayoutOptions.Center });
            for (var index = 1; index < 20; index++)
            {
                (scrollView.Content as StackLayout).Add(new Label() { Text = index.ToString(), HorizontalOptions = LayoutOptions.Center });
            }
            var grid = new Grid();
            page.AddElement(grid);
            var normalScrollView = new ScrollView();
            grid.Add(normalScrollView);
            normalScrollView.Content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Background = new SolidColorBrush(Colors.Aqua),
            };

            (normalScrollView.Content as StackLayout).Add(new Button() { Text = "ScrollViewInGrid" });
            (normalScrollView.Content as StackLayout).Add(new Editor() { Text = "InScrollViewer", HorizontalOptions = LayoutOptions.Center });
            for (var index = 1; index < 20; index++)
            {
                (normalScrollView.Content as StackLayout).Add(new Label() { Text = index.ToString(), HorizontalOptions = LayoutOptions.Center });
            }

            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(page);
                var guideline = new Guideline();
                page.AddElement(guideline);
                layoutSet
                    .Select(guideline).GuidelineOrientation(Orientation.Y).GuidelinePercent(0.5f)
                    .Select(scrollView)
                    .TopToTop()
                    .BottomToBottom()
                    .LeftToLeft()
                    .RightToRight(guideline, 10)
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.MatchConstraint)
                    .Select(grid)
                    .LeftToRight(guideline, 10).RightToRight().TopToTop().BottomToBottom()
                    .Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint);
                layoutSet.ApplyTo(page);
            }

        }

        void StackLayoutInConstraintLayoutTest(ConstraintLayout page)
        {
            
            var stackPanel = new HorizontalStackLayout()
            {
                Background = new SolidColorBrush(Colors.Aqua),
            };
            page.AddElement(stackPanel);
            stackPanel.Add(new Button() { Text = "InHorizontalStackPanel" });
            stackPanel.Add(new Label() { Text = "InHorizontalStackPanel" });
            stackPanel.Add(new Editor() { Text = "InHorizontalStackPanel" });

            var grid = new Grid();
            page.AddElement(grid);
            var normalStackLayout = new HorizontalStackLayout();
            grid.Add(normalStackLayout);
            normalStackLayout.Add(new Button() { Text = "InHorizontalStackPanel" });
            normalStackLayout.Add(new Label() { Text = "InHorizontalStackPanel" });
            normalStackLayout.Add(new Editor() { Text = "InHorizontalStackPanel" });

            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(page);
                layoutSet.Select(stackPanel)
                    .TopToTop().LeftToLeft()
                    .Width(SizeBehavier.WrapContent)
                    //.Height(SizeBehavier.WrapContent)//HorizontalStackLayout's height be decide by parent, WrapContent will let it as big as possible
                    .Height(SizeBehavier.MatchConstraint)
                    .Select(grid)
                    .LeftToLeft().TopToBottom(stackPanel).RightToRight().BottomToBottom()
                    .Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint);
                layoutSet.ApplyTo(page);
            }
        }

        void NestedConstraintLayoutTest(ConstraintLayout page)
        {
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();

            var leftLayout = new ConstraintLayout()
            {
                ConstrainPaddingTopDp = 10,
                ConstrainPaddingBottomDp = 10,
                ConstrainPaddingLeftDp = 10,
                ConstrainPaddingRightDp = 10,
                DebugName = "left",
                Background = new SolidColorBrush(Colors.Red)
            };

            var rightLayout = new ConstraintLayout()
            {
                ConstrainPaddingTopDp = 10,
                ConstrainPaddingBottomDp = 10,
                ConstrainPaddingLeftDp = 10,
                ConstrainPaddingRightDp = 10,
                DebugName = "right",
                Background = new SolidColorBrush(Colors.Green)
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
                var leftChildView = new ConstraintLayout()
                {
                    DebugName = "Second",
                    Background = new SolidColorBrush(Colors.Yellow)
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

                var rightChildView = new ConstraintLayout()
                {
                    DebugName = "Second",
                    Background = new SolidColorBrush(Colors.Yellow),
                    ConstrainWidth = ConstraintSet.WrapContent,
                };
                rightLayout.AddElement(rightChildView);
                rightChildView.Add(FirstButton);
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
            }
        }

        void FlowTest(ConstraintLayout page)
        {
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();
            FifthTextBox.BackgroundColor = Colors.Red;
            SixthRichTextBlock.BackgroundColor = Colors.Green;
            var flow = new Flow();

            flow.SetOrientation(SharpConstraintLayout.Maui.Helper.Widget.Flow.Vertical);
            flow.SetWrapMode(SharpConstraintLayout.Maui.Helper.Widget.Flow.WrapChain);
            flow.SetHorizontalStyle(SharpConstraintLayout.Maui.Helper.Widget.Flow.ChainSpreadInside);
            var layout = page;
            layout.AddElement(ThirdCanvas, FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock, flow);
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
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();

            var layout = page;

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
                    layoutSet.Select(SecondButton).Visibility(Widget.Visibility.Visible);
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
                    layoutSet.Select(SecondButton).Visibility(Widget.Visibility.Invisible);
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
                    layoutSet.Select(SecondButton).Visibility(Widget.Visibility.Gone);
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
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();

            var layout = page;

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
            pageSet.Select(guide).GuidelineOrientation(Orientation.Y).GuidelinePercent(0.5f)
            .Select(layout).LeftToRight(guide).RightToRight(page).TopToTop(page).BottomToBottom(page)
            .Width(SizeBehavier.MatchConstraint)
            .Height(SizeBehavier.MatchConstraint);
            pageSet.ApplyTo(page);

            Task.Run(async () =>
            {
                await Task.Delay(3000);//wait ui show
                UIThread.Invoke(() =>
                {
                    System.Diagnostics.Trace.WriteLine($"{page} position={page.GetBounds()}");
                    SimpleTest.AreEqual(page.GetSize().Width / 2, layout.GetBounds().X, nameof(GuidelineTest), "Horiable guideline should at center");
                }, page);
            });
        }

        private void BaseAlignTest(ConstraintLayout page)
        {
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();

            FirstButton.Text = "CenterTo Width=100";
            SecondButton.Text = "RightToRight & TopToBottom";
            FouthTextBlock.Text = "CenterXTo & BottomToTop(100)";
            FifthTextBox.Text = "CenterXTo & TopToBottom";
            SixthRichTextBlock.Text = "CenterXTo & TopToBottom";
            var layout = page;

            layout.AddElement(FirstButton, SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock);

            var layoutSet = new FluentConstraintSet();
            layoutSet.Clone(layout);
            layoutSet
                .Select(FirstButton).CenterTo()
                .Select(ThirdCanvas).Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint)
                .Select(SecondButton).RightToRight(FirstButton).TopToBottom(FirstButton)
                .Select(ThirdCanvas).LeftToRight(FirstButton).RightToRight().EdgesYTo()
                .Select(FouthTextBlock).CenterXTo().BottomToTop(FirstButton, 100).Width(SizeBehavier.MatchConstraint)
                .Select(FifthTextBox).CenterXTo().TopToBottom(SecondButton).Width(SizeBehavier.MatchConstraint)
                .Select(SixthRichTextBlock).CenterXTo().TopToBottom(FifthTextBox).Width(SizeBehavier.MatchConstraint);
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

        ListViewViewModel listViewViewModel;
        private void ConstraintLayoutInListViewTest(ListView listView)
        {
            var list = new List<MicrosoftNews>();
            list.Add(new MicrosoftNews()
            {
                Title = "央视 | 秘鲁北部发生长途客车坠崖事故 致至少11死34伤",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAXjc2n?w=300&h=174&q=60&m=6&f=jpg&u=t",
                SourceForm = "MSN",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BBVQ5Hs.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "上海16个区当中15个区实现社会面清零",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BB19W3O1?w=300&h=174&q=60&m=6&f=jpg&u=t",
                SourceForm = "每日经济新闻",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAO4ppI.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "袁隆平离开我们一年了",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BB19n8rU.img?w=406&h=304&q=90&m=6&f=jpg&x=2277&y=1318&u=t",
                SourceForm = "光明网",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AARPRel.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "就Android应用支付系统 Match Group和Google达成临时妥协",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAXxMr9.img?w=300&h=225&q=90&m=6&f=jpg&u=t",
                SourceForm = "cnBeta",
            });
            list.Add(new MicrosoftNews()
            {
                Title = "央视 | 秘鲁北部发生长途客车坠崖事故 致至少11死34伤",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAXjc2n?w=300&h=174&q=60&m=6&f=jpg&u=t",
                SourceForm = "MSN",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BBVQ5Hs.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "上海16个区当中15个区实现社会面清零",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BB19W3O1?w=300&h=174&q=60&m=6&f=jpg&u=t",
                SourceForm = "每日经济新闻",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAO4ppI.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "袁隆平离开我们一年了",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BB19n8rU.img?w=406&h=304&q=90&m=6&f=jpg&x=2277&y=1318&u=t",
                SourceForm = "光明网",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AARPRel.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "就Android应用支付系统 Match Group和Google达成临时妥协",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAXxMr9.img?w=300&h=225&q=90&m=6&f=jpg&u=t",
                SourceForm = "cnBeta",
            });
            list.Add(new MicrosoftNews()
            {
                Title = "央视 | 秘鲁北部发生长途客车坠崖事故 致至少11死34伤",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAXjc2n?w=300&h=174&q=60&m=6&f=jpg&u=t",
                SourceForm = "MSN",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BBVQ5Hs.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "上海16个区当中15个区实现社会面清零",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BB19W3O1?w=300&h=174&q=60&m=6&f=jpg&u=t",
                SourceForm = "每日经济新闻",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAO4ppI.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "袁隆平离开我们一年了",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/BB19n8rU.img?w=406&h=304&q=90&m=6&f=jpg&x=2277&y=1318&u=t",
                SourceForm = "光明网",
                SourceFormImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AARPRel.img?w=36&h=36&q=60&m=6&f=png&u=t"
            });
            list.Add(new MicrosoftNews()
            {
                Title = "就Android应用支付系统 Match Group和Google达成临时妥协",
                ImageUrl = "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AAXxMr9.img?w=300&h=225&q=90&m=6&f=jpg&u=t",
                SourceForm = "cnBeta",
            });

            var dataTemplate = new DataTemplate(() =>
            {
                var viewCell = new ViewCell();
                var layout = new ConstraintLayout(new Log()) { ConstrainWidth = ConstraintSet.MatchParent, ConstrainHeight = ConstraintSet.WrapContent, BackgroundColor = Color.FromRgb(66, 66, 66) };
                var title = new Label() { TextColor = Colors.White, FontSize = 30, FontAttributes = FontAttributes.Bold };
                title.SetBinding(Label.TextProperty, nameof(Models.MicrosoftNews.Title));
                var image = new Image();
                image.SetBinding(Image.SourceProperty, nameof(Models.MicrosoftNews.ImageUrl));
                var sourceFrom = new Label() { TextColor = Color.FromRgb(175, 165, 136), FontSize = 12, FontAttributes = FontAttributes.Bold };
                sourceFrom.SetBinding(Label.TextProperty, nameof(Models.MicrosoftNews.SourceForm));
                var sourceFromeImage = new Image();
                sourceFromeImage.SetBinding(Image.SourceProperty, nameof(Models.MicrosoftNews.SourceForm));
                layout.AddElement(
                    image,
                    title,
                    sourceFromeImage,
                    sourceFrom);

                var guideLine = new Guideline() { };
                layout.AddElement(guideLine);

                var littleWindow = new FluentConstraintSet();
                littleWindow.Clone(layout);
                littleWindow
                .Select(guideLine,
                image, sourceFromeImage,
                sourceFrom, title).Clear()//需要移除之前的约束
                .Select(guideLine).GuidelineOrientation(Orientation.X).GuidelinePercent(0.6f)
                .Select(image).EdgesXTo().BottomToTop(guideLine).Width(SizeBehavier.MatchParent).Height(SizeBehavier.WrapContent)
                .Select(sourceFromeImage).LeftToLeft(null, 20).BottomToTop(title, 20).Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                .Select(sourceFrom).LeftToRight(sourceFromeImage, 20).CenterYTo(sourceFromeImage)
                .Select(title).LeftToLeft(null, 20).RightToRight(null, 20).BottomToBottom(null, 20).Width(SizeBehavier.MatchConstraint);

                var bigWindow = new FluentConstraintSet();
                bigWindow.Clone(layout);
                bigWindow
                .Select(guideLine,
                image, sourceFromeImage,
                sourceFrom, title).Clear()//需要移除之前的约束
                                          .Select(image).RightToRight(null, 20).TopToTop(null, 20).Width(140).Height(140)
                                          .Select(sourceFromeImage).LeftToLeft(null, 20).TopToTop(image).Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                .Select(sourceFrom).LeftToRight(sourceFromeImage, 20).CenterYTo(sourceFromeImage)
                .Select(title).LeftToLeft(null, 20).RightToLeft(image, 20).TopToBottom(sourceFromeImage, 20).Width(SizeBehavier.MatchConstraint);
                ;
                double oldValue = -1;
                layout.SizeChanged += (sender, e) =>
                        {
                            if (oldValue == -1)
                            {
                                if ((sender as View).Bounds.Width > 744)
                                {
                                    bigWindow.ApplyTo(layout);
                                }
                                else
                                {
                                    littleWindow.ApplyTo(layout);
                                }
                            }

                            if ((sender as View).Bounds.Width > 744)
                            {
                                if (oldValue < 744)
                                    bigWindow.ApplyTo(layout);

                            }
                            else
                            {
                                if (oldValue > 744)
                                    littleWindow.ApplyTo(layout);
                            }
                            oldValue = (sender as View).Bounds.Width;
                        };

                viewCell.View = layout;
                return viewCell;
            });

            listView.ItemTemplate = dataTemplate;
            listViewViewModel = new ListViewViewModel() { News = list };
            this.BindingContext = listViewViewModel;
            listView.HasUnevenRows = true;
            listView.ItemsSource = listViewViewModel.News;
        }

        private void ConstraintLayoutInScrollViewTest(ScrollView scrollViewForConstraintLayout, ScrollView scrollViewForVerticalStackLayout)
        {
            void buildUIForConstraintLayout()
            {
                scrollViewForConstraintLayout.Orientation = ScrollOrientation.Vertical;
                (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();
                FirstButton.Text = "ConstraintLayout InScrollView";
                var constraintlayout = new ConstraintLayout(new Log()) { ConstrainWidth = ConstraintSet.MatchParent, ConstrainHeight = ConstraintSet.WrapContent, BackgroundColor = Colors.Pink };
                scrollViewForConstraintLayout.Content = constraintlayout;
                constraintlayout.AddElement(FirstButton, SecondButton, ThirdCanvas);
                using (var set = new FluentConstraintSet())
                {
                    set.Clone(constraintlayout);
                    set.Select(FirstButton).CenterXTo().TopToTop()
                        .Select(ThirdCanvas).CenterXTo().TopToBottom(FirstButton).Width(60).Height(300)
                        .Select(SecondButton).CenterXTo().TopToBottom(ThirdCanvas);
                    set.ApplyTo(constraintlayout);
                }
            }

            void buildUIForVerticalStackLayout()
            {
                scrollViewForVerticalStackLayout.Orientation = ScrollOrientation.Vertical;
                (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();
                FirstButton.Text = "VerticalStackLayout InScrollView";
                var verticalStackLayout = new VerticalStackLayout() { BackgroundColor = Colors.DeepPink };
                scrollViewForVerticalStackLayout.Content = verticalStackLayout;
                verticalStackLayout.Add(FirstButton);
                verticalStackLayout.Add(ThirdCanvas);
                verticalStackLayout.Add(SecondButton);
                FirstButton.HorizontalOptions = LayoutOptions.Center;
                ThirdCanvas.HorizontalOptions = LayoutOptions.Center;
                SecondButton.HorizontalOptions = LayoutOptions.Center;
                ThirdCanvas.WidthRequest = 60;
                ThirdCanvas.HeightRequest = 300;
            }

            buildUIForConstraintLayout();
            buildUIForVerticalStackLayout();
        }

        private void RemoveAndAddTest(ConstraintLayout content)
        {
            (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) = CreateControls();
            content.AddElement(FirstButton, SecondButton, ThirdCanvas);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(content);
                set.Select(FirstButton).CenterXTo().TopToTop()
                    .Select(ThirdCanvas).CenterXTo().TopToBottom(FirstButton).Width(60).Height(60)
                    .Select(SecondButton).CenterXTo().TopToBottom(ThirdCanvas);
                set.ApplyTo(content);
            }
            FirstButton.Clicked += (sender, e) =>
            {
                content.RemoveElement(ThirdCanvas);
            };
            SecondButton.Clicked += (sender, e) =>
            {
                if (content.Contains(ThirdCanvas))
                    return;
                content.AddElement(ThirdCanvas);
                using (var set = new FluentConstraintSet())
                {
                    set.Clone(content);
                    set//.Select(FirstButton).CenterXTo().TopToTop()
                        .Select(ThirdCanvas).CenterXTo().TopToBottom(FirstButton).Width(60).Height(60)
                        //.Select(SecondButton).CenterXTo().TopToBottom(ThirdCanvas)
                        ;
                    set.ApplyTo(content);
                }
            };
        }
    }
}
