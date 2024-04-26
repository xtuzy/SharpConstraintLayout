using androidx.constraintlayout.core.widgets;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
namespace SharpConstraintLayout.Core.Benchmark
{
    public class CSharpConstraintLayoutTest
    {
        readonly ITestOutputHelper _output;

        public CSharpConstraintLayoutTest(ITestOutputHelper output)
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
                var view = new ConstraintWidget();//在Maui中时在AddView时调用
                views.Add(view);
            }

            root.add(preview);
            foreach (var view in views)
            {
                root.add(view);//在Maui中是在Measure方法下调用
            }

            int space = 2;
            var time = SimpleClock.BenchmarkTime(() =>
            {
                var pre = preview;
                pre.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
                pre.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
                pre.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                pre.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                pre.Width = 100;
                pre.Height = 100;
                foreach (var view in views)
                {
                    view.connect(ConstraintAnchor.Type.LEFT, pre, ConstraintAnchor.Type.LEFT, space);
                    view.connect(ConstraintAnchor.Type.RIGHT, pre, ConstraintAnchor.Type.RIGHT, -space);
                    view.connect(ConstraintAnchor.Type.TOP, pre, ConstraintAnchor.Type.TOP, space);
                    view.connect(ConstraintAnchor.Type.BOTTOM, pre, ConstraintAnchor.Type.BOTTOM, -space);
                    view.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                    view.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                    pre = view;
                }

                root.layout();
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
