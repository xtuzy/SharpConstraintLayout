using Android.Content;
using Android.Views;
using Android.Widget;

//using BenchmarkDotNet.Analysers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using AndroidX.ConstraintLayout.Core.Widgets;
using AndroidX.Legacy.Widget;
namespace SharpConstraintLayout.Core.Benchmark
{
    public class JavaConstraintLayoutTest
    {
        readonly ITestOutputHelper _output;

        public JavaConstraintLayoutTest(ITestOutputHelper output)
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
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 1000);
            ConstraintWidget preview = new ConstraintWidget();

            var views = new List<ConstraintWidget>();
            for (var i = 0; i < childCount - 1; i++)
            {
                var view = new ConstraintWidget();
                views.Add(view);
            }

            root.Add(preview);
            foreach (var view in views)
            {
                root.Add(view);
            }
            int space = 2;
            var time = SimpleClock.BenchmarkTime(() =>
            {
                var pre = preview;
                pre.Connect(ConstraintAnchor.Type.Left, root, ConstraintAnchor.Type.Left);
                pre.Connect(ConstraintAnchor.Type.Top, root, ConstraintAnchor.Type.Top);
                pre.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.Fixed;
                pre.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.Fixed;
                pre.Width = 100;
                pre.Height = 100;
                foreach (var view in views)
                {
                    view.Connect(ConstraintAnchor.Type.Left, pre, ConstraintAnchor.Type.Left, space);
                    view.Connect(ConstraintAnchor.Type.Right, pre, ConstraintAnchor.Type.Right, -space);
                    view.Connect(ConstraintAnchor.Type.Top, pre, ConstraintAnchor.Type.Top, space);
                    view.Connect(ConstraintAnchor.Type.Bottom, pre, ConstraintAnchor.Type.Bottom, -space);
                    view.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MatchConstraint;
                    view.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MatchConstraint;
                    pre = view;
                }

                root.Layout();
            });
            _output.WriteLine(time);

            var children = root.Children;
            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];
                Assert.Equal(0 + (index * space), child.Left);
                Assert.Equal(0 + (index * space), child.Top);
                Assert.Equal(100, child.Width);
                Assert.Equal(100, child.Height);
            }
        }
    }
}
