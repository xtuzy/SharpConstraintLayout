
using SharpConstraintLayout.Maui.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if __ANDROID__
using Android.Views;
using AndroidX.ConstraintLayout.Helper.Widget;
#else
using SharpConstraintLayout.Maui.Helper.Widget;
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
        void flowTest(ConstraintLayout page)
        {
#if __ANDROID__
            var flow = new Flow(page.Context)
            {
                Id = View.GenerateViewId(),
            };
            flow.SetBackgroundColor(Android.Graphics.Color.Cyan);
#else
            var flow = new Flow();
#if WINDOWS
           
#elif IOS
            flow.BackgroundColor = UIColor.Cyan;
#endif
#endif
                        flow.SetOrientation(Flow.Horizontal);
            flow.SetWrapMode(Flow.WrapChain);
            flow.SetHorizontalStyle(Flow.ChainPacked);
            layout = page;
            layout.AddView(ThirdCanvas);
            layout.AddView(FirstButton);
            layout.AddView(SecondButton);
            
            layout.AddView(FouthTextBlock);
            layout.AddView(FifthTextBox);
            layout.AddView(SixthRichTextBlock);
            
            layout.AddView(flow);

            flow.AddView(FirstButton);
            flow.AddView(SecondButton);
            //flow.AddView(ThirdCanvas);
            flow.AddView(FouthTextBlock);
            flow.AddView(FifthTextBox);
            flow.AddView(SixthRichTextBlock);

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);
            layoutSet.Connect(flow.GetId(), ConstraintSet.Left, layout.GetId(), ConstraintSet.Left);
            layoutSet.Connect(flow.GetId(), ConstraintSet.Right, layout.GetId(), ConstraintSet.Right);
            layoutSet.Connect(flow.GetId(), ConstraintSet.Top, layout.GetId(), ConstraintSet.Top);

            layoutSet.ConstrainWidth(FirstButton.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FirstButton.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainWidth(SecondButton.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(SecondButton.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainWidth(FouthTextBlock.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FouthTextBlock.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainWidth(FifthTextBox.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FifthTextBox.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WrapContent);
#if ANDROID
            layoutSet.ConstrainWidth(flow.GetId(), ConstraintSet.MatchConstraint);
#else
            layoutSet.ConstrainWidth(flow.GetId(), ConstraintSet.MatchConstraint);
# endif
            layoutSet.ConstrainHeight(flow.GetId(), ConstraintSet.WrapContent);

            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Left, flow.GetId(), ConstraintSet.Left);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Right, flow.GetId(), ConstraintSet.Right);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Top, flow.GetId(), ConstraintSet.Top);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Bottom, flow.GetId(), ConstraintSet.Bottom);
            layoutSet.ConstrainWidth(ThirdCanvas.GetId(), ConstraintSet.MatchConstraint);
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MatchConstraint);

            layoutSet.SetVisibility(ThirdCanvas.GetId(), ConstraintSet.Visible);
#if ANDROID
            ThirdCanvas.Visibility = ViewStates.Visible;
#endif
            layoutSet.ApplyTo(layout);
        }
        private void visibilityTest(ConstraintLayout page)
        {
            layout = page;

            layout.AddView(FirstButton);
            layout.AddView(SecondButton);
            layout.AddView(ThirdCanvas);

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);

            layoutSet.Connect(FirstButton.GetId(), ConstraintSet.Left, layout.GetId(), ConstraintSet.Left);
            layoutSet.Connect(FirstButton.GetId(), ConstraintSet.Right, layout.GetId(), ConstraintSet.Right);

            layoutSet.CenterVertically(FirstButton.GetId(), ConstraintSet.ParentId);

            layoutSet.ConstrainWidth(FirstButton.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FirstButton.GetId(), ConstraintSet.WrapContent);

            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.Left, FirstButton.GetId(), ConstraintSet.Right);
            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.Top, FirstButton.GetId(), ConstraintSet.Top);
            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.Bottom, FirstButton.GetId(), ConstraintSet.Bottom);

            layoutSet.ConstrainWidth(SecondButton.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(SecondButton.GetId(), ConstraintSet.WrapContent);

            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Left, SecondButton.GetId(), ConstraintSet.Right);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Right, layout.GetId(), ConstraintSet.Right);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Top, layout.GetId(), ConstraintSet.Top);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Bottom, layout.GetId(), ConstraintSet.Bottom);
            layoutSet.ConstrainWidth(ThirdCanvas.GetId(), ConstraintSet.MatchConstraint);
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MatchConstraint);

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
                    layoutSet.SetVisibility(SecondButton.GetId(), ConstraintSet.Visible);
                    index++;
                }
                else if (index == 2)
                {
                    layoutSet.SetVisibility(SecondButton.GetId(), ConstraintSet.Invisible);
                    index++;
                }
                else if (index == 3)
                {
                    layoutSet.SetVisibility(SecondButton.GetId(), ConstraintSet.Gone);
                    index = 1;
                }
                layoutSet.ApplyTo(layout);
#if ANDROID
                layout.Invalidate();
#endif
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
            layout.AddView(FifthTextBox);
            layout.AddView(ThirdCanvas);
            layout.AddView(barrier);

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);

            layoutSet.CenterVertically(FifthTextBox.GetId(), ConstraintSet.ParentId);
            layoutSet.CenterHorizontally(FifthTextBox.GetId(), ConstraintSet.ParentId);
            layoutSet.ConstrainWidth(FifthTextBox.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FifthTextBox.GetId(), ConstraintSet.WrapContent);

            layoutSet.CreateBarrier(barrier.GetId(), Barrier.RIGHT, 0, new[] { FifthTextBox.GetId() });

            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Left, barrier.GetId(), ConstraintSet.Right, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Right, layout.GetId(), ConstraintSet.Right, 50);

            layoutSet.ConstrainWidth(ThirdCanvas.GetId(), ConstraintSet.MatchConstraint);
#if __ANDROID__
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), LayoutParams.MatchParent);
#else
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MatchParent);
#endif

            layoutSet.ApplyTo(layout);
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

            page.AddView(layout);
            page.AddView(guide);

            var pageSet = new ConstraintSet();
            pageSet.Clone(page);
            pageSet.Create(guide.GetId(), ConstraintSet.Vertical);
            pageSet.SetGuidelinePercent(guide.GetId(), 0.5f);
            //pageSet.SetGuidelineEnd(guide.GetId(), 300);
            pageSet.Connect(layout.GetId(), ConstraintSet.Left, guide.GetId(), ConstraintSet.Right);
            pageSet.Connect(layout.GetId(), ConstraintSet.Right, page.GetId(), ConstraintSet.Right);
            pageSet.Connect(layout.GetId(), ConstraintSet.Top, page.GetId(), ConstraintSet.Top);
            pageSet.Connect(layout.GetId(), ConstraintSet.Bottom, page.GetId(), ConstraintSet.Bottom);
            pageSet.ConstrainWidth(layout.GetId(), ConstraintSet.MatchConstraint);
            pageSet.ConstrainHeight(layout.GetId(), ConstraintSet.MatchConstraint);
            pageSet.ApplyTo(page);
        }

        private void baselineTest(ConstraintLayout page)
        {
            layout = page;

            layout.AddView(FouthTextBlock);
            layout.AddView(SixthRichTextBlock);

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);

            layoutSet.ConstrainWidth(layout.GetId(), ConstraintSet.MatchConstraint);
            layoutSet.ConstrainHeight(layout.GetId(), ConstraintSet.MatchConstraint);

            layoutSet.CenterVertically(FouthTextBlock.GetId(), layout.GetId());
            layoutSet.CenterHorizontally(FouthTextBlock.GetId(), layout.GetId());
            layoutSet.ConstrainWidth(FouthTextBlock.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FouthTextBlock.GetId(), ConstraintSet.WrapContent);

            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.Right, FouthTextBlock.GetId(), ConstraintSet.Left, 50);
            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.Baseline, FouthTextBlock.GetId(), ConstraintSet.Baseline);
            layoutSet.ConstrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WrapContent);

            layoutSet.ApplyTo(layout);
        }

        private void controlsTest(ConstraintLayout page)
        {
            layout = page;

            layout.AddView(FirstButton);
            layout.AddView(SecondButton);
            layout.AddView(ThirdCanvas);
            layout.AddView(FouthTextBlock);
            layout.AddView(FifthTextBox);
            layout.AddView(SixthRichTextBlock);

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);

            //layoutSet.ConstrainWidth(layout.GetId(), ConstraintSet.MatchConstraint);
            //layoutSet.ConstrainHeight(layout.GetId(), ConstraintSet.MatchConstraint);

            layoutSet.Connect(FirstButton.GetId(), ConstraintSet.Left, layout.GetId(), ConstraintSet.Left);
            layoutSet.Connect(FirstButton.GetId(), ConstraintSet.Right, layout.GetId(), ConstraintSet.Right);

            //layoutSet.CenterVertically(FirstButton.GetId(), ConstraintSet.ParentId, ConstraintSet.Top, 0, ConstraintSet.ParentId, ConstraintSet.Bottom, 0, 0.5f);
            layoutSet.CenterVertically(FirstButton.GetId(), layout.GetId());
            layoutSet.ConstrainWidth(FirstButton.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FirstButton.GetId(), ConstraintSet.WrapContent);

            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.Right, FirstButton.GetId(), ConstraintSet.Right);
            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.Top, FirstButton.GetId(), ConstraintSet.Bottom, 10);
            layoutSet.ConstrainWidth(SecondButton.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(SecondButton.GetId(), ConstraintSet.WrapContent);

            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Left, FirstButton.GetId(), ConstraintSet.Right, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Right, layout.GetId(), ConstraintSet.Right, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Top, layout.GetId(), ConstraintSet.Top, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.Bottom, layout.GetId(), ConstraintSet.Bottom, 50);
            layoutSet.ConstrainWidth(ThirdCanvas.GetId(), ConstraintSet.MatchConstraint);
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MatchConstraint);

            layoutSet.Connect(FouthTextBlock.GetId(), ConstraintSet.Right, SecondButton.GetId(), ConstraintSet.Right);
            layoutSet.Connect(FouthTextBlock.GetId(), ConstraintSet.Top, SecondButton.GetId(), ConstraintSet.Bottom, 10);
            layoutSet.ConstrainWidth(FouthTextBlock.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FouthTextBlock.GetId(), ConstraintSet.WrapContent);

            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.Bottom, FirstButton.GetId(), ConstraintSet.Top, 50);
            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.Left, FirstButton.GetId(), ConstraintSet.Left);
            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.Right, FirstButton.GetId(), ConstraintSet.Right);
            layoutSet.ConstrainWidth(FifthTextBox.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(FifthTextBox.GetId(), ConstraintSet.WrapContent);

            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.Right, FouthTextBlock.GetId(), ConstraintSet.Left, 50);
            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.Baseline, FouthTextBlock.GetId(), ConstraintSet.Baseline, 50);
            layoutSet.ConstrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WrapContent);
            layoutSet.ConstrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WrapContent);

            layoutSet.ApplyTo(layout);
        }
    }
}
