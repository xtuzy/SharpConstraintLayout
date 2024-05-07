using Android.Views;
using Microsoft.Maui.Graphics;
using SharpConstraintLayout.Maui.Widget;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
namespace SharpConstraintLayout.Core.Benchmark
{
    public class CSharpConstraintLayoutControlTest
    {
        readonly ITestOutputHelper _output;

        public CSharpConstraintLayoutControlTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(5)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public void PerformanceTest(int childCount)
        {
            var context = Microsoft.Maui.ApplicationModel.Platform.AppContext;
            //don't measure time of create control.
            var constraintlayout = new ConstraintLayout(context)
            {
                ConstrainWidthDp = 1000,
                ConstrainHeightDp = 1000,
            };
            View preview = new View(context);
            var views = new List<View>();
            for (var i = 1; i <= childCount - 1; i++)
            {
                var view = new View(context) { };
                views.Add(view);
            }

            constraintlayout.AddView(preview);
            foreach (var view in views)
            {
                constraintlayout.AddView(view);
            }

            int space = 2;
            var time = SimpleClock.BenchmarkTime(() =>
            {
                var pre = preview;
                using (var set = new FluentConstraintSet())
                {
                    set.Clone(constraintlayout);
                    set.Select(pre)
                        .LeftToLeft()
                        .TopToTop()
                        .Width(100)
                        .Height(100);
                    foreach (var view in views)
                    {
                        set.Select(view)
                            .LeftToLeft(pre, space)
                            .RightToRight(pre, -space)
                            .TopToTop(pre, space)
                            .BottomToBottom(pre, -space)
                            .Width(SizeBehavier.MatchConstraint)
                            .Height(SizeBehavier.MatchConstraint);
                        pre = view;
                    }
                    set.ApplyTo(constraintlayout);
                }
                var px = (int)(1000 * constraintlayout.Density);
                constraintlayout.Measure(px, px);
                constraintlayout.Layout(0, 0, px, px);
            }, 10);
            _output.WriteLine(time);

            for (var index = 0; index < constraintlayout.ChildCount; index++)
            {
                var child = constraintlayout.GetChildAt(index);
                var id = child.Id;
                Assert.Equal(0 + (index * space), GetDp(child.Left));
                Assert.Equal(0 + (index * space), GetDp(child.Top));
                Assert.Equal(100, GetDp(child.Width));
                Assert.Equal(100, GetDp(child.Height));
            }
        }

        [Fact]
        public void BasisTest()
        {
            var context = Microsoft.Maui.ApplicationModel.Platform.AppContext;
            var constraintlayout = new ConstraintLayout(context)
            {
                ConstrainWidth = SizeBehavier.MatchParent,
                ConstrainHeight = SizeBehavier.MatchParent,
            };
            var leftView = new View(context);
            var rightView = new View(context);

            constraintlayout.AddView(leftView);
            constraintlayout.AddView(rightView);

            var fixedHeight = 400;
            using (var set = new FluentConstraintSet())
            {
                set.Clone(constraintlayout);
                set.Select(leftView)
                    .LeftToLeft(constraintlayout)
                    .RightToLeft(rightView)
                    .CenterYTo()
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(fixedHeight);

                set.Select(rightView)
                   .LeftToRight(leftView)
                   .RightToRight()
                   .CenterYTo()
                   .Width(SizeBehavier.MatchConstraint)
                   ;

                set.ApplyTo(constraintlayout);
            }

            var px = (int)(1000 * constraintlayout.Density);
            constraintlayout.Measure(px, px);
            constraintlayout.Layout(0, 0, px, px);

            Assert.Equal(0, GetDp(leftView.Left));
            Assert.Equal(1000 / 2, GetDp(leftView.Right));
            Assert.Equal(1000 / 2 - fixedHeight / 2, GetDp(leftView.Top));
            Assert.Equal(1000 / 2 + fixedHeight / 2, GetDp(leftView.Bottom));
            Assert.Equal(1000 / 2, GetDp(leftView.Width));
            Assert.Equal(fixedHeight, GetDp(leftView.Height));

            Assert.Equal(500, GetDp(rightView.Left));
            Assert.Equal(1000, GetDp(rightView.Right));
            Assert.Equal(1000 / 2, GetDp(rightView.Top));
            Assert.Equal(1000 / 2, GetDp(rightView.Width));
            Assert.Equal(0, GetDp(rightView.Height));
        }

        double GetDp(int px)
        {
            return UIElementExtension.PxToDp(px);
        }

        [Fact]
        public void BarrierTest()
        {
            var context = Microsoft.Maui.ApplicationModel.Platform.AppContext;
            var constraintlayout = new ConstraintLayout(context)
            {
                ConstrainWidth = SizeBehavier.MatchParent,
                ConstrainHeight = SizeBehavier.MatchParent,
            };
            var left1View = new View(context);
            var left2View = new View(context);
            var rightView = new View(context);
            var barrier = new Barrier(context);

            constraintlayout.AddView(left1View);
            constraintlayout.AddView(left2View);
            constraintlayout.AddView(rightView);
            constraintlayout.AddView(barrier);

            var fixedHeight = 400;
            using (var set = new FluentConstraintSet())
            {
                set.Clone(constraintlayout);
                set.Select(left1View)
                    .LeftToLeft(constraintlayout)
                    .CenterYTo()
                    .Width(100)
                    .Height(fixedHeight);
                set.Select(left2View)
                    .LeftToLeft(constraintlayout)
                    .CenterYTo()
                    .Width(150)
                    .Height(fixedHeight);

                set.Select(barrier).Barrier(Direction.Right, 0, left1View, left2View);

                set.Select(rightView)
                   .LeftToRight(barrier)
                   .RightToRight()
                   .CenterYTo()
                   .Width(SizeBehavier.MatchConstraint)
                   ;

                set.ApplyTo(constraintlayout);
            }
            var px = (int)(1000 * constraintlayout.Density);
            constraintlayout.Measure(px, px);
            constraintlayout.Layout(0, 0, px, px);

            Assert.Equal(100, GetDp(left1View.Right));
            Assert.Equal(150, GetDp(left2View.Right));
            Assert.Equal(150, GetDp(barrier.Left));
            Assert.Equal(150, GetDp(rightView.Left));
            Assert.Equal(1000, GetDp(rightView.Right));

            //test adjust position
            using (var set = new FluentConstraintSet())
            {
                set.Clone(constraintlayout);
                set.Select(left1View).Width(200);

                set.ApplyTo(constraintlayout);
            }

            constraintlayout.Measure(px, px);
            constraintlayout.Layout(0, 0, px, px);
            constraintlayout.RequestLayout();
            Assert.Equal(200, GetDp(left1View.Right));
            Assert.Equal(150, GetDp(left2View.Right));
            Assert.Equal(200, GetDp(barrier.Left));
            Assert.Equal(200, GetDp(rightView.Left));
            Assert.Equal(1000, GetDp(rightView.Right));
        }

        [Fact]
        public void GuidlineTest()
        {
            var context = Microsoft.Maui.ApplicationModel.Platform.AppContext;
            var constraintlayout = new ConstraintLayout(context)
            {
                ConstrainWidth = SizeBehavier.MatchParent,
                ConstrainHeight = SizeBehavier.MatchParent,
            };
            var leftView = new View(context);
            var guideline = new Guideline(context);

            constraintlayout.AddView(leftView);
            constraintlayout.AddView(guideline);

            var fixedHeight = 400;
            using (var set = new FluentConstraintSet())
            {
                set.Clone(constraintlayout);
                set.Select(leftView)
                    .LeftToLeft(constraintlayout)
                    .RightToLeft(guideline)
                    .CenterYTo()
                    .Width(SizeBehavier.MatchConstraint)
                    .Height(fixedHeight);

                set.Select(guideline).GuidelineOrientation(Orientation.Y)
                    .GuidelinePercent(0.25f);

                set.ApplyTo(constraintlayout);
            }

            var px = (int)(1000 * constraintlayout.Density);
            constraintlayout.Measure(px, px);
            constraintlayout.Layout(0, 0, px, px);

            Assert.Equal(250, GetDp(leftView.Right));
        }
    }
}
