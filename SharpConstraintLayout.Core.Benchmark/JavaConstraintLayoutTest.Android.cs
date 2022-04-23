using Android.Content;
using Android.Views;
using Android.Widget;

using BenchmarkDotNet.Analysers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Core.Benchmark
{
    public class JavaConstraintLayoutTest
    {
        public void JavaBasisConstraintTest(int childCount = 100)
        {
            int viewCount = childCount;
            AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidgetContainer root = new AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidgetContainer(0, 0, 1000, 600);
            AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget preview = new AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget(100, 40);
            root.Add(preview);
            preview.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Left, root, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Left);
            preview.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Bottom, root, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Bottom);
            preview.HorizontalDimensionBehaviour = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget.DimensionBehaviour.MatchConstraint;
            preview.VerticalDimensionBehaviour = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget.DimensionBehaviour.MatchConstraint;
            for (var i = 0; i < viewCount; i++)
            {
                var view = new AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget(100, 40);
                root.Add(view);
                view.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Left, preview, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Left, 1);
                view.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Right, preview, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Right, -1);
                view.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Top, preview, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Top, -1);
                view.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Bottom, preview, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Bottom);
                view.HorizontalDimensionBehaviour = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget.DimensionBehaviour.MatchConstraint;
                view.VerticalDimensionBehaviour = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget.DimensionBehaviour.MatchConstraint;
                preview = view;
            }

            root.Layout();
        }

    }
}
