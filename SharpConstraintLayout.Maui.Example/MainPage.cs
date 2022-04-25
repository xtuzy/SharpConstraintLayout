using SharpConstraintLayout.Maui.Native.Example.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpConstraintLayout.Maui.Example.Tool;
using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace SharpConstraintLayout.Maui.Example
{
    public partial class MainPage
    {
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
