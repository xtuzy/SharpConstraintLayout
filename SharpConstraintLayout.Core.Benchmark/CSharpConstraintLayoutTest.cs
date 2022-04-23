using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using androidx.constraintlayout.core.widgets;
namespace SharpConstraintLayout.Core.Benchmark
{
    public class CSharpConstraintLayoutTest
    {
        public void CSharpBasisConstraintTest(int childCount = 100)
        {
            int viewCount = childCount;
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget preview = new ConstraintWidget(100, 40);
            root.add(preview);
            preview.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            preview.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            preview.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            preview.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            for (var i = 0; i < viewCount; i++)
            {
                var view = new ConstraintWidget(100, 40);
                root.add(view);
                view.connect(ConstraintAnchor.Type.LEFT, preview, ConstraintAnchor.Type.LEFT, 1);
                view.connect(ConstraintAnchor.Type.RIGHT, preview, ConstraintAnchor.Type.RIGHT, -1);
                view.connect(ConstraintAnchor.Type.TOP, preview, ConstraintAnchor.Type.TOP, -1);
                view.connect(ConstraintAnchor.Type.BOTTOM, preview, ConstraintAnchor.Type.BOTTOM);
                view.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                view.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                preview = view;
            }
            root.layout();
        }

        public void CSharpFlowConstraintTest(int childCount = 100)
        {
            new CSharpFlowTest().TestFlowWrapNone(childCount);
        }
    }
}
