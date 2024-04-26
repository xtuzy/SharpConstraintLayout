using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SharpConstraintLayout.Maui.Widget;
using System.Collections.Generic;
using System.Diagnostics;
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
            //don't measure time of create control.
            var constraintlayout = new ConstraintLayout()
            {
                ConstrainWidthDp = 1000,
                ConstrainHeightDp = 1000,
            };
            View preview = new ContentView();
            var views = new List<View>();
            for (var i = 1; i <= childCount - 1; i++)
            {
                var view = new ContentView() { };
                views.Add(view);
            }

            constraintlayout.Add(preview);
            foreach (var view in views)
            {
                constraintlayout.Add(view);
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
                var manager = new ConstraintLayoutManager(constraintlayout);
                var measure = manager.Measure(constraintlayout.ConstrainWidthDp, constraintlayout.ConstrainHeightDp);
                var arrange = manager.ArrangeChildren(new Rect(Point.Zero, measure));

            }, 10);
            _output.WriteLine(time);

            var children = constraintlayout.Children;

            for(var index = 0; index<children.Count; index++)
            {
                var child = children[index];
                Assert.Equal(0 + (index * space) , child.Frame.Left);
                Assert.Equal(0 + (index * space), child.Frame.Top);
                Assert.Equal(100, child.Frame.Width);
                Assert.Equal(100, child.Frame.Height);
            }
        }

        [Fact]
        public void BasisTest()
        {
            var root = new Grid()
            {
                WidthRequest = 1000,
                HeightRequest = 1000
            };
            var constraintlayout = new ConstraintLayout()
            {
                ConstrainWidth = SizeBehavier.MatchParent,
                ConstrainHeight = SizeBehavier.MatchParent,
            };
            var leftView = new ContentView();
            var rightView = new ContentView();

            root.Add(constraintlayout);
            constraintlayout.Add(leftView as IView);
            constraintlayout.Add(rightView as IView);

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

            var manager = new ConstraintLayoutManager(constraintlayout);
            var measure = manager.Measure(1000, 1000);
            manager.ArrangeChildren(new Rect(Point.Zero, measure));

            Assert.Equal(0, leftView.Bounds.Left);
            Assert.Equal(1000 / 2, leftView.Bounds.Right);
            Assert.Equal(1000 / 2 - fixedHeight / 2, leftView.Bounds.Top);
            Assert.Equal(1000 / 2 + fixedHeight / 2, leftView.Bounds.Bottom);
            Assert.Equal(1000 / 2, leftView.Bounds.Width);
            Assert.Equal(fixedHeight, leftView.Bounds.Height);

            Assert.Equal(500, rightView.Bounds.Left);
            Assert.Equal(1000, rightView.Bounds.Right);
            Assert.Equal(1000 / 2, rightView.Bounds.Top);
            Assert.Equal(1000 / 2, rightView.Bounds.Width);
            Assert.Equal(0, rightView.Bounds.Height);
        }
        
        [Fact]
        public void BarrierTest()
        {
            var constraintlayout = new ConstraintLayout()
            {
                ConstrainWidth = SizeBehavier.MatchParent,
                ConstrainHeight = SizeBehavier.MatchParent,
            };
            var left1View = new ContentView();
            var left2View = new ContentView();
            var rightView = new ContentView();
            var barrier = new Barrier();

            constraintlayout.Add(left1View as IView);
            constraintlayout.Add(left2View as IView);
            constraintlayout.Add(rightView as IView);
            constraintlayout.Add(barrier as IView);

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

            var manager = new ConstraintLayoutManager(constraintlayout);
            var measure = manager.Measure(1000, 1000);
            manager.ArrangeChildren(new Rect(Point.Zero, measure));

            Assert.Equal(150, barrier.Bounds.Left);
            Assert.Equal(150, rightView.Bounds.Left);
            Assert.Equal(1000, rightView.Bounds.Right);

            //test adjust position
            using (var set = new FluentConstraintSet())
            {
                set.Clone(constraintlayout);
                set.Select(left1View).Width(200);

                set.ApplyTo(constraintlayout);
            }

            measure = manager.Measure(1000, 1000);
            manager.ArrangeChildren(new Rect(Point.Zero, measure));

            Assert.Equal(200, barrier.Bounds.Left);
            Assert.Equal(200, rightView.Bounds.Left);
            Assert.Equal(1000, rightView.Bounds.Right);
        }
        
        [Fact]
        public void GuidlineTest()
        {
            var constraintlayout = new ConstraintLayout()
            {
                ConstrainWidth = SizeBehavier.MatchParent,
                ConstrainHeight = SizeBehavier.MatchParent,
            };
            var leftView = new ContentView();
            var guideline = new Guideline();

            constraintlayout.Add(leftView as IView);
            constraintlayout.Add(guideline as IView);

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

            var manager = new ConstraintLayoutManager(constraintlayout);
            var measure = manager.Measure(1000, 1000);
            manager.ArrangeChildren(new Rect(Point.Zero, measure));

            Assert.Equal(250, leftView.Bounds.Right);
        }
    }
}
