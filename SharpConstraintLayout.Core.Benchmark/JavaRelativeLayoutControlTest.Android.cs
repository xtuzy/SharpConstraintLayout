using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;
namespace SharpConstraintLayout.Core.Benchmark
{
    public class JavaRelativeLayoutControlTest
    {
        readonly ITestOutputHelper _output;

        public JavaRelativeLayoutControlTest(ITestOutputHelper output)
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
            var root = new RelativeLayout(context);
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
                var pre = preview;
                RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(100, 100);
                layoutParams.AddRule(LayoutRules.AlignParentLeft);
                layoutParams.AddRule(LayoutRules.AlignParentTop);
                layoutParams.LeftMargin = 0;
                layoutParams.TopMargin = 0;
                pre.LayoutParameters = layoutParams;
                foreach (var view in views)
                {
                    layoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
                    layoutParams.AddRule(LayoutRules.AlignLeft, pre.Id);
                    layoutParams.AddRule(LayoutRules.AlignRight, pre.Id);
                    layoutParams.AddRule(LayoutRules.AlignTop, pre.Id);
                    layoutParams.AddRule(LayoutRules.AlignBottom, pre.Id);
                    layoutParams.LeftMargin = space;
                    layoutParams.RightMargin = -space;
                    layoutParams.TopMargin = space;
                    layoutParams.BottomMargin = -space;
                    view.LayoutParameters = layoutParams;
                    pre = view;
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