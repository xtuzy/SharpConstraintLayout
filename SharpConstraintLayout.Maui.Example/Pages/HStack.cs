using SharpConstraintLayout.Maui.Widget;

namespace SharpConstraintLayout.Maui.Example.Pages
{
    internal class HStackLayout : androidx.constraintlayout.core.widgets.VirtualLayout
    {
        public override void measure(int widthMode, int widthSize, int heightMode, int heightSize)
        {
            base.measure(widthMode, widthSize, heightMode, heightSize);
            if (mWidgetsCount > 0 && measureChildren())
            {
                var totleW = 0;
                var maxHeight = 0;
                for(int i = 0; i <mWidgetsCount; i++)
                {
                    var w = mWidgets[i];
                    totleW += w.Width;
                    if (w.Height > maxHeight)
                        maxHeight = w.Height;
                }
                setMeasure(totleW, maxHeight);
                return;
            }
        }
    }

    internal class HStack : VirtualLayout
    {
        HStackLayout mHStack;
        protected override void init()
        {
            base.init();
            mHStack = new HStackLayout();

            mHelperWidget = mHStack;
        }

        public override void onMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec)
        {
            base.onMeasure(layout, widthMeasureSpec, heightMeasureSpec);
            int widthMode = (int)MeasureSpec.GetMode(widthMeasureSpec);
            int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            int heightMode = (int)MeasureSpec.GetMode(heightMeasureSpec);
            int heightSize = MeasureSpec.GetSize(heightMeasureSpec);
            if (layout != null)
            {
                layout.measure(widthMode, widthSize, heightMode, heightSize);
                MeasuredSize = new Size(layout.MeasuredWidth, layout.MeasuredHeight);
            }
        }

        public override void UpdatePostMeasure(ConstraintLayout container)
        {
            base.UpdatePostMeasure(container);
        }

        public override void UpdatePreLayout(ConstraintLayout container)
        {
            base.UpdatePreLayout(container);
            var thisId = this.GetId();
            var childs = ReferencedIds;
            for (var index = 0; index < childs.Length; index++)
            {
                var id = childs[index];
                container.mConstraintSet.Connect(id, ConstraintSet.Top, thisId, ConstraintSet.Top);
                container.mConstraintSet.Connect(id, ConstraintSet.Bottom, thisId, ConstraintSet.Bottom);
                if (index == 0 && index == childs.Length - 1)
                {
                    container.mConstraintSet.Connect(id, ConstraintSet.Left, thisId, ConstraintSet.Left);
                    container.mConstraintSet.Connect(id, ConstraintSet.Right, thisId, ConstraintSet.Right);
                }
                else if (index == 0)
                {
                    container.mConstraintSet.Connect(id, ConstraintSet.Left, thisId, ConstraintSet.Left);
                    container.mConstraintSet.Connect(id, ConstraintSet.Right, childs[index + 1], ConstraintSet.Left);
                }
                else if (index == childs.Length - 1)
                {
                    container.mConstraintSet.Connect(id, ConstraintSet.Left, childs[index - 1], ConstraintSet.Right);
                    container.mConstraintSet.Connect(id, ConstraintSet.Right, thisId, ConstraintSet.Right);
                }
                else
                {
                    container.mConstraintSet.Connect(id, ConstraintSet.Left, childs[index - 1], ConstraintSet.Right);
                    container.mConstraintSet.Connect(id, ConstraintSet.Right, childs[index + 1], ConstraintSet.Left);
                }
            }
        }
    }
}
