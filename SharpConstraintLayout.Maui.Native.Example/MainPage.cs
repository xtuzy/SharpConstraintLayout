
using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if __ANDROID__
using Android.Views;
using AndroidX.ConstraintLayout.Helper.Widget;
using Android.Widget;
using Orientation = SharpConstraintLayout.Maui.Widget.FluentConstraintSet.Orientation;
using SharpConstraintLayout.Maui.Native.Example.Tool;
#else
using SharpConstraintLayout.Maui.Helper.Widget;
using System.Diagnostics;
using Microsoft.Maui.Essentials;
using SharpConstraintLayout.Maui.Native.Example.Tool;
#endif

#if __ANDROID__
using AndroidX.ConstraintLayout.Widget;
#elif __IOS__
using UIKit;
#elif WINDOWS
using Microsoft.UI;
#endif
namespace SharpConstraintLayout.Maui.Native.Example
{
    public partial class MainPage
    {
        /// <summary>
        /// test nested layout warp content and it's child match parent
        /// </summary>
        /// <param name="page"></param>
        void nestedLayoutTest(ConstraintLayout page)
        {
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

            using (var pageSet = new FluentConstraintSet())
            {
                page.AddView(layout);
                pageSet.Clone(page);
                pageSet.Select(layout)
                    .CenterYTo(page)
                    .Width(ConstraintSet.WrapContent)
                    .Height(ConstraintSet.WrapContent);
                pageSet.ApplyTo(page);
                layout.AddView(ThirdCanvas);
                layout.AddView(FirstButton);
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

#if WINDOWS || ANDROID
            FirstButton.Click += (sender, e) =>
            {
#if ANDROID          
                Toast.MakeText(page.Context, $"Button Y {FirstButton.GetZ()} ,Canvas Y {ThirdCanvas.GetZ()}", ToastLength.Long).Show();
                //FirstButton.SetZ(ThirdCanvas.GetZ());
                FirstButton.Elevation = 0;
                layout.Invalidate();
                Toast.MakeText(page.Context, $"Button Y {FirstButton.GetZ()} ,Canvas Y {ThirdCanvas.GetZ()}", ToastLength.Long).Show();
#elif WINDOWS
                //Debug.WriteLine($"Button Y {FirstButton.Z} ,Canvas Y {ThirdCanvas. GetZ()}");
#endif
            };
#endif
        }
        void flowTest(ConstraintLayout page)
        {
#if __ANDROID__
            var flow = new Flow(page.Context)
            {
                Id = View.GenerateViewId(),
            };
#else
            var flow = new Flow();
#endif
            flow.SetOrientation(Flow.Horizontal);
            flow.SetWrapMode(Flow.WrapChain);
            flow.SetHorizontalStyle(Flow.ChainPacked);
            layout = page;
            layout.AddView(ThirdCanvas, FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock, flow);
            flow.AddView(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock);

            using (var layoutSet = new FluentConstraintSet())
            {
                layoutSet.Clone(layout);
                layoutSet
                    .Select(flow)
                    .CenterYTo()
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.WrapContent)
                    .Select(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock)
                    .Width(SizeBehavier.WrapContent)
                    .Height(SizeBehavier.WrapContent)
                    .Select(ThirdCanvas).EdgesTo(flow)
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(SizeBehavier.MatchConstraint)
                    .Visibility(FluentConstraintSet.Visibility.Visible);
                layoutSet.ApplyTo(layout);
            }

            Task.Run(async () =>
            {
                int index = 0;
                while (index < 5)//test 20 times,you can edit textbox
                {
                    await Task.Delay(3000);//wait ui show
                    UIThread.Invoke(() =>
                    {
                        SimpleTest.AreEqual(flow.GetBounds().Left, ThirdCanvas.GetBounds().Left, nameof(flowTest), "Canvas Left position should equal to Flow");
                        SimpleTest.AreEqual(flow.GetBounds().Right, ThirdCanvas.GetBounds().Right, nameof(flowTest), "Canvas Right position should equal to Flow");
                        SimpleTest.AreEqual(flow.GetBounds().Top, ThirdCanvas.GetBounds().Top, nameof(flowTest), "Canvas Top position should equal to Flow");
                        SimpleTest.AreEqual(flow.GetBounds().Bottom, ThirdCanvas.GetBounds().Bottom, nameof(flowTest), "Canvas Bottom position should equal to Flow");
                    });

                    index++;
                }
            });
        }

        private void visibilityTest(ConstraintLayout page)
        {
            layout = page;

            layout.AddView(FirstButton, SecondButton, ThirdCanvas);

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
#elif IOS
            FirstButton.TouchUpInside += (sender, e) =>
#endif
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
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right + SecondButton.GetBounds().Width, ThirdCanvas.GetBounds().Left, nameof(barrierTest), "When Center Button Visiable, Canvas position should equal to FirstButton+SecondButton");
                        });
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
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right + SecondButton.GetBounds().Width, ThirdCanvas.GetBounds().Left, nameof(barrierTest), "When Center Button Invisiable, Canvas position should equal to FirstButton+SecondButton");
                        });
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
                            SimpleTest.AreEqual(FirstButton.GetBounds().Right, ThirdCanvas.GetBounds().Left, nameof(barrierTest), "When Center Button Gone, Canvas position should equal to FirstButton");
                        });
                    });
                }
                layoutSet.ApplyTo(layout);
            };
        }

        private void barrierTest(ConstraintLayout page)
        {
            layout = page;
#if __ANDROID__
            var barrier = new Barrier(page.Context)
            {
                Id = View.GenerateViewId(),
            };
#else
            var barrier = new Barrier();
#endif
            layout.AddView(FifthTextBox, ThirdCanvas, barrier);

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
                while (index < 20)//test 20 times,you can edit textbox
                {
                    await Task.Delay(3000);//wait ui show
                    UIThread.Invoke(() =>
                    {
                        SimpleTest.AreEqual(FifthTextBox.GetBounds().Right, barrier.GetBounds().Left, nameof(barrierTest), "Barrier position should equal to TextBox");
                        SimpleTest.AreEqual(ThirdCanvas.GetBounds().Left, barrier.GetBounds().Left, nameof(barrierTest), "Canvas position should equal to Barrier");
                    });

                    index++;
                }
            });
        }

        private void guidelineTest(ConstraintLayout page)
        {
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

            page.AddView(layout, guide);

            var pageSet = new FluentConstraintSet();
            pageSet.Clone(page);
            pageSet.Select(guide).GuidelineOrientation(Orientation.X).GuidelinePercent(0.5f)
            .Select(layout).LeftToRight(guide).RightToRight(page).TopToTop(page).BottomToBottom(page)
            .Width(SizeBehavier.MatchConstraint)
            .Height(SizeBehavier.MatchConstraint);
            pageSet.ApplyTo(page);

            Task.Run(async () =>
            {
                await Task.Delay(3000);//wait ui show
                UIThread.Invoke(() =>
                {
                    SimpleTest.AreEqual(page.GetSize().Width / 2, layout.GetBounds().X, nameof(guidelineTest), "Horiable guideline should at center");
                });
            });
        }

        private void baselineTest(ConstraintLayout page)
        {
            layout = page;

            layout.AddView(FouthTextBlock, SixthRichTextBlock);

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

        private void controlsTest(ConstraintLayout page)
        {
            layout = page;

            layout.AddView(FirstButton, SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock);

            var layoutSet = new FluentConstraintSet();
            layoutSet.Clone(layout);
            layoutSet
                .Select(FirstButton).CenterTo()
                .Select(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock).Width(SizeBehavier.WrapContent).Height(SizeBehavier.WrapContent)
                .Select(ThirdCanvas).Width(SizeBehavier.MatchConstraint).Height(SizeBehavier.MatchConstraint)
                .Select(SecondButton).RightToRight(FirstButton).TopToBottom(FirstButton, 10)
                .Select(ThirdCanvas).LeftToRight(FirstButton, 50).RightToRight(null, 50).EdgesYTo(null, 50)
                .Select(FouthTextBlock).RightToRight(SecondButton).TopToBottom(SecondButton, 10)
                .Select(FifthTextBox).BottomToTop(FirstButton, 50).LeftToLeft(FirstButton).RightToRight(FirstButton)
                .Select(SixthRichTextBlock).RightToLeft(FouthTextBlock, 50).BaselineToBaseline(FouthTextBlock, 50);
            layoutSet.ApplyTo(layout);
        }
    }
}
