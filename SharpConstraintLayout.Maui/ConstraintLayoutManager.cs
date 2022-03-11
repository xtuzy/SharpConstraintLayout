using androidx.constraintlayout.core.widgets;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using Rectangle = Microsoft.Maui.Graphics.Rectangle;
using androidx.constraintlayout.core.widgets.analyzer;

namespace SharpConstraintLayout.Maui
{
    public class ConstraintLayoutManager : AbsoluteLayoutManager
    {
        IConstraintLayout ConstraintLayout;
        public ConstraintLayoutManager(IConstraintLayout constraintLayout) : base(constraintLayout)
        {
            ConstraintLayout = constraintLayout;
        }

        public override Size ArrangeChildren(Rectangle bounds)
        {
            return ArrangeOverride(bounds.Size);
        }

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            return MeasureOverride(new Size(widthConstraint, heightConstraint));
        }

        Stopwatch swTest = null;
        /// <summary>
        /// current times, sum of spend time,limited times
        /// </summary>
        (int, List<long>, int) countTestData = (0, new List<long>(), 10);
        bool isInfinity = false;

        protected Size MeasureOverride(Size availableSize)
        {
            if (ConstraintLayout.TEST)
            {
                if (swTest != null && countTestData.Item1 <= countTestData.Item3)
                {
                    countTestData.Item1++;
                    countTestData.Item2.Add(swTest.ElapsedTicks);
                    //swTest.Restart();
                }

                //if (swTest == null)
                {
                    swTest = Stopwatch.StartNew();
                }
            }

            //first measure all child size,we need know some default size.
            for (int n = 0; n < ConstraintLayout.Count; n++)
            {
                var child = ConstraintLayout[n];
                var widget = ConstraintLayout.GetWidget(child);
                //匹配约束的先不测量,因为没有固定的值
                if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT
                    && widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                    continue;
                //if(!IsMeasureValid)
                child.Measure(availableSize.Width, availableSize.Height);
            }

            //we have know some view default size, so we can calculate other view size that they should be.
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                ConstraintLayout.Root.Width = int.MaxValue;
                isInfinity = true;
            }
            else
            {
                ConstraintLayout.Root.Width = (int)availableSize.Width;
            }

            if (double.IsPositiveInfinity(availableSize.Height))
            {
                ConstraintLayout.Root.Height = int.MaxValue;
                isInfinity = true;
            }
            else
            {
                ConstraintLayout.Root.Height = (int)availableSize.Height;
            }
            ConstraintLayout.Root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
            ConstraintLayout.Root.layout();
            ConstraintLayout.Root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.EXACTLY, ConstraintLayout.Root.Width, BasicMeasure.EXACTLY, ConstraintLayout.Root.Height, 0, 0, 0, 0);
            double w;
            double h;
            //now we know all view corrected size in constaint, so give it to child,let them to caculate their child.
            for (int n = 0; n < ConstraintLayout.Count; n++)
            {
                var child = ConstraintLayout[n];
                var widget = ConstraintLayout.GetWidget(child);

                if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT
                    || widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                {
                    if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                        //newMeasureSize.Width = (int)child.DesiredSize.Width;
                        w = availableSize.Width;
                    else
                        w = widget.Width;
                    if (widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                        //newMeasureSize.Height = (int)child.DesiredSize.Height;
                        h = availableSize.Height;
                    else
                        h = widget.Height;
                    child.Measure(w, h);
                }
                if (ConstraintLayout.DEBUG)
                {
                    Debug.WriteLine($"{child},Size {widget.Width},{widget.Height} ,Baseline {widget.BaselineDistance} ");
                }
            }

            if (ConstraintLayout.DEBUG)
            {
                Debug.WriteLine($"{ConstraintLayout},availableSize {availableSize},DesiredSize {ConstraintLayout.DesiredSize},ContainerWidgetSize {ConstraintLayout.Root.Width},{ConstraintLayout.Root.Height}");
                for (int n = 0; n < ConstraintLayout.Count; n++)
                {
                    var child = ConstraintLayout[n];
                    var view = child as FrameworkElement;
                    // Debug.WriteLine($"{view?.Tag as string},Size {view?.Width},{view?.Height}, DesiredSize {view?.DesiredSize}");
                }
            }

            //return availableSize;
            //自身的位置绘制由父控件决定,所以这里要传的正确.
            //如果自身是要Match_Parent,而父控件传进来MaxValue,那么应该传回MaxValue?
            //如果自身是Match_Parent,而父控件能传来特定值,那么直接传回特定值
            //如果自身是Warp_Content和Match_Constraint,那么应该能从子控件约束中计算出特定值传回
            /*if(mLayout.HorizontalDimensionBehaviour==ConstraintWidget.DimensionBehaviour.MATCH_PARENT&&double.IsInfinity(availableSize.Width))
                w= 0;
            else*/
            w = ConstraintLayout.Root.Width;
            /*if(mLayout.VerticalDimensionBehaviour==ConstraintWidget.DimensionBehaviour.MATCH_PARENT&&double.IsInfinity(availableSize.Height))
                h= 0;
            else*/
            h = ConstraintLayout.Root.Height;
            return new Size(w, h);
        }


        protected Size ArrangeOverride(Size finalSize)
        {

            if (ConstraintLayout.DEBUG)
            {
                Debug.WriteLine($"{ConstraintLayout},finalSize {finalSize},DesiredSize {ConstraintLayout.DesiredSize}");
            }
            //recalculate?,because when constraintlayout size be define by parent,it size need parent to arrange
            //such as it as child of listview,listview will send double.infinity to measure,if you set listview's content to strenth,
            //you need get that size at Arrage. 
            if (isInfinity && finalSize.Width != 0 && finalSize.Height != 0)//only when parent give me measure size is size isInfinity and finalSize can use(not 0),we recalculate layout.
            {
                if (ConstraintLayout.DEBUG)
                {
                    Debug.WriteLine($"{ConstraintLayout} Re layout");
                }
                ConstraintLayout.Root.Width = (int)finalSize.Width;
                ConstraintLayout.Root.Height = (int)finalSize.Height;
                ConstraintLayout.Root.layout();
                ConstraintLayout.Root.OptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
                ConstraintLayout.Root.measure(Optimizer.OPTIMIZATION_STANDARD, BasicMeasure.EXACTLY, (int)finalSize.Width, BasicMeasure.EXACTLY, (int)finalSize.Height, 0, 0, 0, 0);
                //isInfinity = false;//Wpf's ArrangeOverride can be load mutiple times, not by measure.
            }

            //layout child
            foreach (ConstraintWidget child in ConstraintLayout.Root.Children)
            {
                UIElement component = (UIElement)child.CompanionWidget;
                if (component != null)
                {
                    if (ConstraintLayout.DEBUG)
                    {
                         Debug.WriteLine($"{component} arrange " + child.X + " " + child.Y + " " + child.Width + " " + child.Height);
                    }
                    component.Arrange(new Microsoft.Maui.Graphics.Rectangle(child.X, child.Y, child.Width, child.Height));
                }
            }

            if (ConstraintLayout.TEST)
            {
                swTest.Stop();
                if (countTestData.Item1 == countTestData.Item3)
                {
                    long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
                    long count = 0;
                    foreach (var item in countTestData.Item2)
                    {
                        count += item;

                    }
                    // Debug.WriteLine($"{this.Tag as string}, Count {countTestData.Item1}Times, Single Measure+Layout Average Spend Time: {(count * nanosecPerTick * 1.0) / 1000000 / countTestData.Item3}ms");
                    countTestData.Item1 = 0;
                    countTestData.Item2.Clear();
                }
            }
            return finalSize;
        }
    }
}
