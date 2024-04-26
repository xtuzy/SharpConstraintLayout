using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;
namespace SharpConstraintLayout.Core.Benchmark
{
    public class JavaConstraintLayoutControlTest
    {
        readonly ITestOutputHelper _output;

        public JavaConstraintLayoutControlTest(ITestOutputHelper output)
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
            int WidthRequest = 1000;
            int HeightRequest = 1000;

            var context = Android.App.Application.Context;
            int viewCount = childCount;
            var root = new ConstraintLayout(context);
            root.Id = View.GenerateViewId();

            View preview = new TextView(context)
            {
                Id = View.GenerateViewId(),
            };
            root.AddView(preview);

            var views = new List<View>();
            for (var i = 0; i < viewCount - 1; i++)
            {
                var view = new TextView(context)
                {
                    Id = View.GenerateViewId(),
                };
                root.AddView(view);
                views.Add(view);
            }
            int space = 2;
            var time = SimpleClock.BenchmarkTime(() =>
            {
                using (var set = new ConstraintSet())
                {
                    var pre = preview;
                    set.Clone(root);
                    set.Connect(pre.Id, ConstraintSet.Left, root.Id, ConstraintSet.Left);
                    set.Connect(pre.Id, ConstraintSet.Top, root.Id, ConstraintSet.Top);
                    set.ConstrainWidth(pre.Id, 100);
                    set.ConstrainHeight(pre.Id, 100);

                    foreach (var view in views)
                    {
                        set.Connect(view.Id, ConstraintSet.Left, pre.Id, ConstraintSet.Left, space);
                        set.Connect(view.Id, ConstraintSet.Right, pre.Id, ConstraintSet.Right, -space);
                        set.Connect(view.Id, ConstraintSet.Top, pre.Id, ConstraintSet.Top, space);
                        set.Connect(view.Id, ConstraintSet.Bottom, pre.Id, ConstraintSet.Bottom, -space);
                        set.ConstrainWidth(view.Id, ConstraintSet.MatchConstraint);
                        set.ConstrainHeight(view.Id, ConstraintSet.MatchConstraint);
                        pre = view;
                    }
                    set.ApplyTo(root);
                }

                root.Measure(WidthRequest, HeightRequest);
                root.Layout(0, 0, WidthRequest, HeightRequest);
            }, 10);
            _output.WriteLine(time);

            for (var index = 0; index < root.ChildCount; index++)
            {
                var child = root.GetChildAt(index);
                Debug.WriteLine($"[{child.Left},{child.Top},{child.Right},{child.Bottom}][{child.Width},{child.Height}]");
                Assert.Equal(0 + (index * space), child.Left);
                Assert.Equal(0 + (index * space), child.Top);
                Assert.Equal(100, child.Width);
                Assert.Equal(100, child.Height);
            }
        }
    }
}