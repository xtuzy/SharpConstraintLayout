using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
namespace SharpConstraintLayout.Core.Benchmark
{
    public class CSharpAbsoluteLayoutControlTest
    {
        readonly ITestOutputHelper _output;

        public CSharpAbsoluteLayoutControlTest(ITestOutputHelper output)
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
            var absoluteLayout = new AbsoluteLayout()
            {
                WidthRequest = 1000,
                HeightRequest = 1000,
            };
            View preview = new ContentView();
            var views = new List<View>();
            for (var i = 1; i <= childCount - 1; i++)
            {
                var view = new ContentView() { };
                views.Add(view);
            }

            absoluteLayout.Add(preview);
            foreach (var view in views)
            {
                absoluteLayout.Add(view);
            }
            int space = 2;
            var time = SimpleClock.BenchmarkTime(() =>
            {
                var pre = preview;
                AbsoluteLayout.SetLayoutBounds(pre, new Rect(0, 0, 100, 100));
                //AbsoluteLayout.SetLayoutFlags(pre, AbsoluteLayoutFlags.PositionProportional);

                for (var index = 0; index < views.Count; index++)
                {
                    var view = views[index]; 
                    AbsoluteLayout.SetLayoutBounds(view, new Rect((index+1)*space, (index + 1) * space, 100, 100));
                    //AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.PositionProportional);
                    pre = view;
                }

                var manager = new AbsoluteLayoutManager(absoluteLayout);
                var measure = manager.Measure(absoluteLayout.WidthRequest, absoluteLayout.HeightRequest);
                var arrange = manager.ArrangeChildren(new Rect(Point.Zero, measure));
            }, 10);
            _output.WriteLine(time);

            var children = absoluteLayout.Children;
            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];
                Assert.Equal(0 + (index * space), child.Frame.Left);
                Assert.Equal(0 + (index * space), child.Frame.Top);
                Assert.Equal(100, child.Frame.Width);
                Assert.Equal(100, child.Frame.Height);
            }
        }
    }
}
