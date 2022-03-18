
using SharpConstraintLayout.Maui.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private void visibilityTest(ConstraintLayout page)
        {
            layout = page;

            layout.AddView(FirstButton);
            layout.AddView(SecondButton);
            layout.AddView(ThirdCanvas);

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);

            layoutSet.ConstrainWidth(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);

            layoutSet.CenterHorizontally(FirstButton.GetId(), ConstraintSet.PARENT_ID);
            layoutSet.CenterVertically(FirstButton.GetId(), ConstraintSet.PARENT_ID);
            layoutSet.ConstrainWidth(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.RIGHT);
            layoutSet.CenterVertically(SecondButton.GetId(), FirstButton.GetId());
            layoutSet.ConstrainWidth(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, SecondButton.GetId(), ConstraintSet.RIGHT, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.TOP, layout.GetId(), ConstraintSet.TOP, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.BOTTOM, layout.GetId(), ConstraintSet.BOTTOM, 50);
            layoutSet.ConstrainWidth(ThirdCanvas.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_CONSTRAINT);


            layoutSet.ApplyTo(layout);
            int index = 2;
#if WINDOWS
            FirstButton.Click += (sender, e) =>
#elif __IOS__
            FirstButton.TouchUpInside+= (sender, e) =>
#endif
            {
                if (index == 1)
                {
                    layoutSet.SetVisibility(SecondButton.GetId(), ConstraintSet.VISIBLE);
                    index++;
                }
                else if (index == 2)
                {
                    layoutSet.SetVisibility(SecondButton.GetId(), ConstraintSet.INVISIBLE);
                    index++;
                }
                else if (index == 3)
                {
                    layoutSet.SetVisibility(SecondButton.GetId(), ConstraintSet.GONE);
                    index = 1;
                }
                layoutSet.ApplyTo(layout);
            };
        }

        private void barrierTest(ConstraintLayout page)
        {
            layout = page;

            var barrier = new Barrier()
            {

            };
            layout.AddView(FifthTextBox);
            layout.AddView(ThirdCanvas);
            layout.AddView(barrier);

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);

            layoutSet.ConstrainWidth(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);


            layoutSet.CenterVertically(FifthTextBox.GetId(), layout.GetId());
            layoutSet.CenterHorizontally(FifthTextBox.GetId(), layout.GetId());
            layoutSet.ConstrainWidth(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.CreateBarrier(barrier.GetId(), Barrier.RIGHT, 0, new[] { FifthTextBox.GetId() });

            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, barrier.GetId(), ConstraintSet.RIGHT, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT, 50);

            layoutSet.ConstrainWidth(ThirdCanvas.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_PARENT);

            layoutSet.ApplyTo(layout);
        }

        private void guidelineTest(ConstraintLayout page)
        {
            layout = new ConstraintLayout()
            {
#if WINDOWS
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
#elif __IOS__
                BackgroundColor = UIColor.Black
#endif
            };
            var guide = new Guideline()
            {
                //GuidelinePercent = 0.3f,
                
                //Orientation = ConstraintSet.VERTICAL,
                //GuidelineBegin = 200,
            };

            page.AddView(layout);
            page.AddView(guide);

            var pageSet = new ConstraintSet();
            pageSet.Clone(page);

            pageSet.Connect(layout.GetId(), ConstraintSet.LEFT, guide.GetId(), ConstraintSet.RIGHT);
            pageSet.Connect(layout.GetId(), ConstraintSet.RIGHT, page.GetId(), ConstraintSet.RIGHT);
            pageSet.Connect(layout.GetId(), ConstraintSet.TOP, page.GetId(), ConstraintSet.TOP);
            pageSet.Connect(layout.GetId(), ConstraintSet.BOTTOM, page.GetId(), ConstraintSet.BOTTOM);
            pageSet.ConstrainWidth(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            pageSet.ConstrainHeight(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            pageSet.ApplyTo(page);
        }

        private void baselineTest(ConstraintLayout page)
        {
            layout = page;

            layout.AddView(FouthTextBlock);
            layout.AddView(SixthRichTextBlock);

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);

            layoutSet.ConstrainWidth(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);

            layoutSet.CenterVertically(FouthTextBlock.GetId(), layout.GetId());
            layoutSet.CenterHorizontally(FouthTextBlock.GetId(), layout.GetId());
            layoutSet.ConstrainWidth(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.RIGHT, FouthTextBlock.GetId(), ConstraintSet.LEFT, 50);
            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.BASELINE, FouthTextBlock.GetId(), ConstraintSet.BASELINE);
            layoutSet.ConstrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);


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

            layoutSet.ConstrainWidth(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);

            layoutSet.Connect(FirstButton.GetId(), ConstraintSet.LEFT, layout.GetId(), ConstraintSet.LEFT);
            layoutSet.Connect(FirstButton.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT);
            layoutSet.CenterVertically(FirstButton.GetId(), ConstraintSet.PARENT_ID, ConstraintSet.TOP, 0, ConstraintSet.PARENT_ID, ConstraintSet.BOTTOM, 0, 0.5f);
            layoutSet.ConstrainWidth(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 10);
            layoutSet.ConstrainWidth(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);


            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.RIGHT, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.TOP, layout.GetId(), ConstraintSet.TOP, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.BOTTOM, layout.GetId(), ConstraintSet.BOTTOM, 50);
            layoutSet.ConstrainWidth(ThirdCanvas.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_CONSTRAINT);

            layoutSet.Connect(FouthTextBlock.GetId(), ConstraintSet.RIGHT, SecondButton.GetId(), ConstraintSet.RIGHT);
            layoutSet.Connect(FouthTextBlock.GetId(), ConstraintSet.TOP, SecondButton.GetId(), ConstraintSet.BOTTOM, 10);
            layoutSet.ConstrainWidth(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.BOTTOM, FirstButton.GetId(), ConstraintSet.TOP, 50);
            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.LEFT);
            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            layoutSet.ConstrainWidth(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.RIGHT, FouthTextBlock.GetId(), ConstraintSet.LEFT, 50);
            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.BASELINE, FouthTextBlock.GetId(), ConstraintSet.BASELINE, 50);
            layoutSet.ConstrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.ApplyTo(layout);
        }
    }
}
