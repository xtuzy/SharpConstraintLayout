using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
namespace SharpConstraintLayout.Core.Benchmark
{
    public class CSharpRelativeLayoutControlTest
    {
        readonly ITestOutputHelper _output;

        public CSharpRelativeLayoutControlTest(ITestOutputHelper output)
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
            var relativeLayout = new RelativeLayout()
            {
                WidthRequest = 1000,
                HeightRequest = 1000,
            };
            View preview = new ContentView() { IsPlatformEnabled = true };
            var views = new List<View>();
            for (var i = 0; i < childCount - 1; i++)
            {
                var view = new ContentView() { IsPlatformEnabled = true };
                views.Add(view);
            }

            relativeLayout.Children.Add(preview, Constraint.RelativeToParent((p) => 0));
            foreach (var view in views)
            {
                relativeLayout.Children.Add(view, Constraint.RelativeToParent((p) => 0));
            }
            int space = 2;
            var time = SimpleClock.BenchmarkTime(() =>
            {
                var pre = preview;
                RelativeLayout.SetXConstraint(pre, Constraint.RelativeToParent((p) => 0));
                RelativeLayout.SetYConstraint(pre, Constraint.RelativeToParent((p) => 0));
                RelativeLayout.SetWidthConstraint(pre, Constraint.RelativeToParent((p) => 100));
                RelativeLayout.SetHeightConstraint(pre, Constraint.RelativeToParent((p) => 100));

                for (var index = 0; index < views.Count; index++)
                {
                    var view = views[index];
                    RelativeLayout.SetXConstraint(view, Constraint.RelativeToView(pre, (p, v) => v.X+space));
                    RelativeLayout.SetYConstraint(view, Constraint.RelativeToView(pre, (p, v) => v.Y+space));
                    RelativeLayout.SetWidthConstraint(view, Constraint.RelativeToView(pre, (p, v) => v.Width));
                    RelativeLayout.SetHeightConstraint(view, Constraint.RelativeToView(pre, (p, v) => v.Height));

                    pre = view;
                }
                relativeLayout.Layout(new Rect(0, 0, relativeLayout.WidthRequest, relativeLayout.HeightRequest));
            }, 10);
            _output.WriteLine(time);

            var children = relativeLayout.Children;
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
