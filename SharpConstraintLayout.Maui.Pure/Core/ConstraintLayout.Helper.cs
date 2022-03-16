#if __F__
using System;
using System.Diagnostics;
#if __ANDROID__
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

using AndroidX.ConstraintLayout.Widget;
#elif __IOS__
using View = UIKit.UIView;
#elif WINDOWS
using View = Microsoft.UI.Xaml.UIElement;
#endif

//指定命名空间避免冲突
namespace SharpConstraintLayout.Maui.Pure.Core
{


    //https://developer.android.com/training/constraint-layout?hl=zh-cn
    /// <summary>
    /// 设置太多了,不好用
    /// </summary>
    public static partial class ConstraintLayoutHelper
    {
        /// <summary>
        /// Center widget between the other two widgets.<br/>
        /// View在任意两条边中心<br/>
        /// <see href="https://android.googlesource.com/platform/frameworks/opt/sherpa/+/studio-3.0/constraintlayout/src/main/java/android/support/constraint/ConstraintSet.java">参考:ConstraintSet.Center源码</see>
        /// </summary>
        public static ConstraintSet AddCenter(this ConstraintSet set, View view, View toFirstView, NSLayoutAttribute firstSide, View toSecondView, NSLayoutAttribute secondSide, int firstSideMargin = 0, int secondSideMargin = 0)
        {
            VerifyViewId(view);
            // Error checking
            if (firstSideMargin < 0)
            {
                throw new ArgumentException("margin must be > 0");
            }

            if (secondSideMargin < 0)
            {
                throw new ArgumentException("margin must be > 0");
            }
            /*if (bias <= 0 || bias > 1)
            {
                throw new ArgumentException("bias must be between 0 and 1 inclusive");
            }*/

            if (firstSide == NSLayoutAttribute.Left || firstSide == NSLayoutAttribute.Right)
            {
                set.Connect(view.GetId(), (int)NSLayoutAttribute.Left, toFirstView.GetId(), (int)firstSide, firstSideMargin);
                set.Connect(view.GetId(), (int)NSLayoutAttribute.Right, toSecondView.GetId(), (int)secondSide, secondSideMargin);
                //Constraint constraint = mConstraints.get(centerID);
                //constraint.horizontalBias = bias;
            }
            else if (firstSide == NSLayoutAttribute.Leading || firstSide == NSLayoutAttribute.Trailing)
            {
                set.Connect(view.GetId(), (int)NSLayoutAttribute.Leading, toFirstView.GetId(), (int)firstSide, firstSideMargin);
                set.Connect(view.GetId(), (int)NSLayoutAttribute.Trailing, toSecondView.GetId(), (int)secondSide, secondSideMargin);
                //Constraint constraint = mConstraints.get(centerID);
                //constraint.horizontalBias = bias;
            }
            else
            {
                set.Connect(view.GetId(), (int)NSLayoutAttribute.Top, toFirstView.GetId(), (int)firstSide, firstSideMargin);
                set.Connect(view.GetId(), (int)NSLayoutAttribute.Bottom, toSecondView.GetId(), (int)secondSide, secondSideMargin);
                //Constraint constraint = mConstraints.get(centerID);
                //constraint.verticalBias = bias;
            }
            return set;
        }

        /// <summary>
        /// View的某一边对齐某个View的中心
        /// </summary>
        public static ConstraintSet AddCenter(this ConstraintSet set, View view, NSLayoutAttribute firstSide, View toView, NSCenterAttribute orientation, int biasMargins = 0)
        {
            //获取顶层ConstraitLayout
            View parent = view;
            while (true)
            {
                //这里用循环获取ConstraintLayout是防止不按照每个组件只有一个父Layout的最佳原则
                var p = (View)parent.Parent;
                if (p == null)
                    throw new ArgumentException("No ConstraintLayout in Parents");
                parent = p as ConstraintLayout;
                if (parent != null)
                    break;
                else
                    parent = p;
            }

            //创建辅助线
            var line = new Space(parent.Context) { Id = View.GenerateViewId() };
            line.LayoutParameters = new ConstraintLayout.LayoutParams(0, 0);
            ((ConstraintLayout)parent).AddView(line);
            if (orientation is NSCenterAttribute.CenterX)
            {
                set.AddCenterH(line, toView);
                set.AddConnect(view, firstSide, line, firstSide, biasMargins);
            }
            else
            {
                set.AddCenterV(line, toView);
                set.AddConnect(view, firstSide, line, firstSide, biasMargins);
            }
            return set;
        }

        /// <summary>
        /// Centers the widget horizontally to the left and right side on another widgets sides.
        /// </summary>
        public static ConstraintSet AddCenterH(this ConstraintSet set, View view, View toView)
        {
            VerifyViewId(view);
            if (view.Parent == toView)
            {
                set.CenterHorizontally(view.GetId(), (int)NSUnKnowAttribute.ParentId);
                return set;
            }
            set.Connect(view.GetId(), (int)NSLayoutAttribute.Leading, toView.GetId(), (int)NSLayoutAttribute.Leading);
            set.Connect(view.GetId(), (int)NSLayoutAttribute.Trailing, toView.GetId(), (int)NSLayoutAttribute.Trailing);
            return set;
        }

        /// <summary>
        /// 垂直方向上居中于某View
        /// </summary>
        /// <param name="set"></param>
        /// <param name="view"></param>
        /// <param name="toView"></param>
        /// <returns></returns>
        public static ConstraintSet AddCenterV(this ConstraintSet set, View view, View toView)
        {
            VerifyViewId(view);
            if (view.Parent == toView)
            {
                set.CenterVertically(view.GetId(), (int)NSUnKnowAttribute.ParentId);
                return set;
            }
            set.Connect(view.GetId(), (int)NSLayoutAttribute.Top, toView.GetId(), (int)NSLayoutAttribute.Top);
            set.Connect(view.GetId(), (int)NSLayoutAttribute.Bottom, toView.GetId(), (int)NSLayoutAttribute.Bottom);
            return set;
        }

        /// <summary>
        /// 包装native提供链式调用<br/>
        /// </summary>
        /// <param name="set"></param>
        /// <param name="guidLine"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static ConstraintSet AddGuidLinePercent(this ConstraintSet set, View guidLine, float percent)
        {
            VerifyViewId(guidLine);
            set.SetGuidelinePercent(guidLine.Id, percent);
            return set;
        }

        /// <summary>
        /// 包装native提供链式调用<br/>
        /// width可以是具体宽数值,NSSizeAttribute.WrapContent(-2,同ViewGroup.LayoutParams.WrapContent),NSSizeAttribute.MatchConstraint(0)
        /// </summary>
        public static ConstraintSet AddConstrainWidth(this ConstraintSet set, View view, int width)
        {
            VerifyViewId(view);
            set.ConstrainWidth(view.GetId(), width);
            return set;
        }

        /// <summary>
        /// 包装native提供链式调用<br/>
        /// </summary>
        /// <param name="set"></param>
        /// <param name="view"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static ConstraintSet AddConstrainMinWidth(this ConstraintSet set, View view, int width)
        {
            VerifyViewId(view);
            set.ConstrainMinWidth(view.GetId(), width);
            return set;
        }

        /// <summary>
        /// 包装native提供链式调用<br/>
        /// </summary>
        /// <param name="set"></param>
        /// <param name="view"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static ConstraintSet AddConstrainMaxWidth(this ConstraintSet set, View view, int width)
        {
            VerifyViewId(view);
            set.ConstrainMaxWidth(view.GetId(), width);
            return set;
        }

        /// <summary>
        /// 包装native提供链式调用<br/>
        /// height可以是具体高数值,NSSizeAttribute.WrapContent(-2,同ViewGroup.LayoutParams.WrapContent),NSSizeAttribute.MatchConstraint(0)
        /// </summary>
        public static ConstraintSet AddConstrainHeight(this ConstraintSet set, View view, int height)
        {
            VerifyViewId(view);
            set.ConstrainHeight(view.GetId(), height);
            return set;
        }

        /// <summary>
        /// 包装native提供链式调用<br/>
        /// </summary>
        /// <param name="set"></param>
        /// <param name="view"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static ConstraintSet AddConstrainMinHeight(this ConstraintSet set, View view, int height)
        {
            VerifyViewId(view, null);
            set.ConstrainMinHeight(view.GetId(), height);
            return set;
        }

        /// <summary>
        /// 包装native提供链式调用<br/>
        /// </summary>
        /// <param name="set"></param>
        /// <param name="view"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static ConstraintSet AddConstrainMaxHeight(this ConstraintSet set, View view, int height)
        {
            VerifyViewId(view);
            set.ConstrainMaxHeight(view.GetId(), height);
            return set;
        }

        /// <summary>
        /// 包装native提供链式调用<br/>
        /// margin和native一样只能为正数
        /// </summary>
        /// <param name="set"></param>
        /// <param name="view"></param>
        /// <param name="startSide"></param>
        /// <param name="secondView"></param>
        /// <param name="endSide"></param>
        /// <param name="margins"></param>
        /// <returns></returns>
        public static ConstraintSet Connect(this ConstraintSet set, View view, NSLayoutAttribute startSide, View secondView, NSLayoutAttribute endSide, int margin = 0)
        {
            VerifyViewId(view, secondView);
            set.Connect(view.GetId(), (int)startSide, secondView.GetId(), (int)endSide, margin);
            return set;
        }

        public static void VerifyViewId(View view,View secondView=null)
        {
#if __ANDROID__
            if (view.GetId() == View.NoId)
            {
                Debug.WriteLine("{0} {1}", view, "No id,will auto generate.");
                view.Id = View.GenerateViewId();
            }
            if (secondView!=null && secondView.GetId() == View.NoId)
            {
                Debug.WriteLine("{0} {1}", secondView, "No id,will auto generate.");
                secondView.GetId() = View.GenerateViewId();
#endif
            }
        }

        /// <summary>
        /// 简单实现和iOS AutoLayout一样的偏移概念<br/>
        /// View的startside相对SecondView的endside位置,偏移margin<br/>
        /// 只负责左右上下边界对齐,未实现居中和文字基线等的对齐,margin和iOS的一样,在左和上为负数,在右和下为正<br/>
        /// </summary>
        /// <param name="set"></param>
        /// <param name="view">要和别人对齐的View</param>
        /// <param name="startSide"></param>
        /// <param name="secondView">作为基准的View</param>
        /// <param name="endSide"></param>
        /// <param name="biasMargins">注意是偏差距离,有正负,左偏小于0,右偏大于0</param>
        /// <returns></returns>
        public static ConstraintSet AddConnect(this ConstraintSet set, View view, NSLayoutAttribute startSide, View secondView, NSLayoutAttribute endSide, int biasMargins = 0)
        {
            VerifyViewId(view,secondView);
            /*
             * 方向水平
             */
            if (startSide is NSLayoutAttribute.Left ||
                startSide is NSLayoutAttribute.Right ||
                startSide is NSLayoutAttribute.Leading ||
                startSide is NSLayoutAttribute.Trailing
                )
            {
                //根据bias创建辅助线
                if (biasMargins != 0)
                {
                    //获取顶层ConstraitLayout
                    View parent = view;
                    while (true)
                    {
                        //这里用循环获取ConstraintLayout是防止不按照每个组件只有一个父Layout的最佳原则
                        var p = (View)parent.Parent;
                        if (p == null)
                            throw new ArgumentException("No ConstraintLayout in Parents");
                        parent = p as ConstraintLayout;
                        if (parent != null)
                            break;
                        else
                            parent = p;
                    }

                    /*if (parent == null)
                        throw new ArgumentException("View Not ConstraintLayout");*/

                    //创建辅助线
                    var line = new Space(parent.Context) { Id = View.GenerateViewId() };
                    line.LayoutParameters = new ConstraintLayout.LayoutParams(0, 0);
                    ((ConstraintLayout)parent).AddView(line);
                    set.Connect(line.Id, (int)NSLayoutAttribute.Top, secondView.GetId(), (int)NSLayoutAttribute.Top);
                    if (biasMargins < 0)
                    {
                        //bias小于0,辅助线的右边在endside的左边
                        if (startSide == NSLayoutAttribute.Left || startSide == NSLayoutAttribute.Right)
                        {
                            set.Connect(line.Id, (int)NSLayoutAttribute.Right, secondView.GetId(), (int)endSide, -biasMargins);//TODO:测试Leading,Trailing有无问题
                            set.Connect(view.GetId(), (int)startSide, line.Id, (int)NSLayoutAttribute.Right);
                        }
                        else// if (startSide == NSLayoutAttribute.Leading || startSide == NSLayoutAttribute.Trailing)
                        {
                            set.Connect(line.Id, (int)NSLayoutAttribute.Trailing, secondView.GetId(), (int)endSide, -biasMargins);//TODO:测试Leading,Trailing有无问题
                            set.Connect(view.GetId(), (int)startSide, line.Id, (int)NSLayoutAttribute.Trailing);
                        }
                    }
                    else
                    {
                        //bias大于0,辅助线左边在endsize的右边
                        if (startSide == NSLayoutAttribute.Left || startSide == NSLayoutAttribute.Right)
                        {
                            set.Connect(line.Id, (int)NSLayoutAttribute.Left, secondView.GetId(), (int)endSide, biasMargins);
                            set.Connect(view.GetId(), (int)startSide, line.Id, (int)NSLayoutAttribute.Left);
                        }
                        else// if (startSide == NSLayoutAttribute.Leading || startSide == NSLayoutAttribute.Trailing || )
                        {
                            set.Connect(line.Id, (int)NSLayoutAttribute.Leading, secondView.GetId(), (int)endSide, biasMargins);
                            set.Connect(view.GetId(), (int)startSide, line.Id, (int)NSLayoutAttribute.Leading);
                        }
                    }
                }
                else
                {
                    set.Connect(view.GetId(), (int)startSide, secondView.GetId(), (int)endSide);
                }

                return set;
            }



            /*
             * 垂直方向
             */
            //根据bias创建辅助线
            if (biasMargins != 0)
            {
                //获取顶层ConstraitLayout
                View parent = view;
                while (true)
                {
                    //这里用循环获取ConstraintLayout是防止不按照每个组件只有一个父Layout的最佳原则
                    var p = (View)parent.Parent;
                    if (p == null)
                        throw new ArgumentException("No ConstraintLayout in Parents");
                    parent = p as ConstraintLayout;
                    if (parent != null)
                        break;
                    else
                        parent = p;
                }
                //创建辅助线
                var line = new Space(parent.Context) { Id = View.GenerateViewId() };
                line.LayoutParameters = new ConstraintLayout.LayoutParams(0, 0);
                ((ConstraintLayout)parent).AddView(line);

                set.Connect(line.Id, (int)NSLayoutAttribute.Leading, secondView.GetId(), (int)NSLayoutAttribute.Leading);
                if (biasMargins < 0)
                {
                    //bias小于0,辅助线的右边在endside的左边
                    set.Connect(line.Id, (int)NSLayoutAttribute.Bottom, secondView.GetId(), (int)endSide, -biasMargins);
                    set.Connect(view.GetId(), (int)startSide, line.Id, (int)NSLayoutAttribute.Bottom);
                }
                else
                {
                    //bias大于0,辅助线左边在endsize的右边
                    set.Connect(line.Id, (int)NSLayoutAttribute.Top, secondView.GetId(), (int)endSide, biasMargins);
                    set.Connect(view.GetId(), (int)startSide, line.Id, (int)NSLayoutAttribute.Top);
                }
                return set;

            }
            else
            {
                set.Connect(view.GetId(), (int)startSide, secondView.GetId(), (int)endSide);
            }
            return set;
        }
    }

    /// <summary>
    /// let ConstraintSet more simple => decrease use NSLayoutAttribute
    /// </summary>
    public static partial class ConstraintLayoutHelper
    {
        public static ConstraintSet LeftToLeft(this ConstraintSet set, View fromView, View toView, int margin = 0)
        {
            return set.AddConnect(fromView, NSLayoutAttribute.Left, toView, NSLayoutAttribute.Left, margin);
        }

        public static ConstraintSet LeftToRight(this ConstraintSet set, View fromView, View toView, int margin = 0)
        {
            return set.AddConnect(fromView, NSLayoutAttribute.Left, toView, NSLayoutAttribute.Right, margin);
        }

        public static ConstraintSet TopToTop(this ConstraintSet set, View fromView, View toView, int margin = 0)
        {
            return set.AddConnect(fromView, NSLayoutAttribute.Top, toView, NSLayoutAttribute.Top, margin);
        }

        public static ConstraintSet TopToBottom(this ConstraintSet set, View fromView, View toView, int margin = 0)
        {
            return set.AddConnect(fromView, NSLayoutAttribute.Top, toView, NSLayoutAttribute.Bottom, margin);
        }

        public static ConstraintSet RightToLeft(this ConstraintSet set, View fromView, View toView, int margin = 0)
        {
            return set.AddConnect(fromView, NSLayoutAttribute.Right, toView, NSLayoutAttribute.Left, margin);
        }

        public static ConstraintSet RightToRight(this ConstraintSet set, View fromView, View toView, int margin = 0)
        {
            return set.AddConnect(fromView, NSLayoutAttribute.Right, toView, NSLayoutAttribute.Right, margin);
        }

        public static ConstraintSet BottomToTop(this ConstraintSet set, View fromView, View toView, int margin = 0)
        {
            return set.AddConnect(fromView, NSLayoutAttribute.Bottom, toView, NSLayoutAttribute.Top, margin);
        }

        public static ConstraintSet BottomToBottom(this ConstraintSet set, View fromView, View toView, int margin = 0)
        {
            return set.AddConnect(fromView, NSLayoutAttribute.Bottom, toView, NSLayoutAttribute.Bottom, margin);
        }

        public static ConstraintSet BaselineToBaseline(this ConstraintSet set, View fromView, View toView)
        {
            return set.Connect(fromView, NSLayoutAttribute.Baseline, toView, NSLayoutAttribute.Baseline);
        }
    }

        public enum NSSizeAttribute
        {
            //[Register("WRAP_CONTENT")]
            /// <summary>
            /// 视图仅在需要时扩展以适应其内容
            /// </summary>
            WrapContent = ConstraintSet.WrapContent,// - 2,

            //[Register("MATCH_CONSTRAINT")]
            /// <summary>
            /// 视图会尽可能扩展，以满足每侧的约束条件（在考虑视图的外边距之后）
            /// </summary>
            MatchConstraint = ConstraintSet.MatchConstraint,// 0,

            //[Register("MATCH_CONSTRAINT_PERCENT")]
            MatchConstraintPercent = ConstraintSet.MatchConstraintPercent,// 2,

            //[Register("MATCH_CONSTRAINT_SPREAD")]
            /// <summary>
            /// 这些属性仅在您将视图宽度设置为“match constraints”时才会生效
            /// <para/>
            /// layout_constraintWidth_default<br/>
            /// 尽可能扩展视图以满足每侧的约束条件
            /// </summary>
            MatchConstraintSpread = ConstraintSet.MatchConstraintSpread,// 0,

            //[Register("MATCH_CONSTRAINT_WRAP")]
            /// <summary>
            /// 这些属性仅在您将视图宽度设置为“match constraints”时才会生效
            /// <para/>
            /// layout_constraintWidth_default<br/>
            /// 仅在需要时扩展视图以适应其内容，但如有约束条件限制，视图仍然可以小于其内容。<br/>
            /// 因此，它与使用 Wrap Content（上面）之间的区别在于，将宽度设为 Wrap Content 会强行使宽度始终与内容宽度完全匹配;<br/>
            /// 而使用 layout_constraintWidth_default 设置为 wrap 的 Match Constraints 时，视图可以小于内容宽度。
            /// <para></para>
            /// <see href="https://developer.android.com/reference/androidx/constraintlayout/widget/ConstraintSet?hl=zh-cn#MATCH_CONSTRAINT_SPREAD">MATCH_CONSTRAINT_WRAP</see>
            /// </summary>
            MatchConstraintWrap = ConstraintSet.MatchConstraintWrap,// 1,


            //根据AutoLayout添加
            Width = 101,
            Height = 102,
        }

        public enum NSStateAttribute
        {
            //[Register("GONE")]
            /// <summary>
            /// This view is gone, and will not take any space for layout purposes.
            /// </summary>
            Gone = ConstraintSet.Gone,// 8,

            //[Register("INVISIBLE")]
            /// <summary>
            /// This view is invisible, but it still takes up space for layout purposes.
            /// </summary>
            Invisible = ConstraintSet.Invisible,// 4,

            //[Register("VISIBILITY_MODE_IGNORE")]
            VisibilityModeIgnore = ConstraintSet.VisibilityModeIgnore,// 1,

            //[Register("VISIBILITY_MODE_NORMAL")]
            VisibilityModeNormal = ConstraintSet.VisibilityModeNormal,// 0,

            //[Register("VISIBLE")]
            Visible = ConstraintSet.Visible,// 0,
        }

        public enum NSChainAttribute
        {
            //[Register("CHAIN_PACKED")]
            ChainPacked = ConstraintSet.ChainPacked,// 2,

            //[Register("CHAIN_SPREAD")]
            ChainSpread = ConstraintSet.ChainSpread,// 0,

            //[Register("CHAIN_SPREAD_INSIDE")]
            ChainSpreadInside = ConstraintSet.ChainSpreadInside,// 1,
        }

        public enum NSOrientationAttribute
        {
            //[Register("HORIZONTAL")]
            Horizontal = ConstraintSet.Horizontal,// 0,

            //[Register("VERTICAL")]
            Vertical = ConstraintSet.Vertical,// 1,
        }

        public enum NSGuideLineAttribute
        {
            //[Register("HORIZONTAL_GUIDELINE")]
            HorizontalGuideline = ConstraintSet.HorizontalGuideline,// 0,

            //[Register("VERTICAL_GUIDELINE")]
            VerticalGuideline = ConstraintSet.VerticalGuideline,// 1,
        }


        public enum NSRotateAttribute
        {
            //[Register("ROTATE_NONE")]
            RotateNone = ConstraintSet.RotateNone,// 0,

            //[Register("ROTATE_PORTRATE_OF_RIGHT")]
            RotatePortrateOfRight = ConstraintSet.RotatePortrateOfRight,// 1,

            //[Register("ROTATE_PORTRATE_OF_LEFT")]
            RotatePortrateOfLeft = ConstraintSet.RotatePortrateOfLeft,// 2,


            //[Register("ROTATE_RIGHT_OF_PORTRATE")]
            RotateRightOfPortrate = ConstraintSet.RotateRightOfPortrate,// 3,

            //[Register("ROTATE_LEFT_OF_PORTRATE")]
            RotateLeftOfPortrate = ConstraintSet.RotateLeftOfPortrate,// 4,

        }

        public enum NSLayoutAttribute
        {
            //[Register("LEFT")]
            Left = ConstraintSet.Left,// 1,

            //[Register("RIGHT")]
            Right = ConstraintSet.Right,// 2,

            //[Register("TOP")]
            Top = ConstraintSet.Top,// 3,

            //[Register("BOTTOM")] 
            Bottom = ConstraintSet.Bottom,//4,

            //[Register("BASELINE")] 
            /// <summary>
            /// The baseline of the text in a view.
            /// </summary>
            Baseline = ConstraintSet.Baseline,//5 ,

            //[Register("START")]
            /// <summary>
            /// The left side of a view in left to right languages.
            /// </summary>
            Leading = ConstraintSet.Start,// 6,

            //[Register("END")]
            /// <summary>
            /// The right side of a view in right to left languages.
            /// </summary>
            Trailing = ConstraintSet.End,// 7,
        }

        public enum NSCenterAttribute
        {
            //根据AutoLayout添加
            CenterX = 103,
            CenterY = 104,
        }

        public enum NSUnKnowAttribute
        {
            //[Register("CIRCLE_REFERENCE")]
            /// <summary>
            /// Circle reference from a view.
            /// </summary>
            CircleReference = ConstraintSet.CircleReference,// 8,

            //[Register("PARENT_ID")]
            /// <summary>
            /// References the id of the parent.
            /// </summary>
            ParentId = ConstraintSet.ParentId,// 0,

            //[Register("UNSET")]
            /// <summary>
            /// Used to indicate a parameter is cleared or not set
            /// </summary>
            Unset = ConstraintSet.Unset,// - 1,
        }

        /// <summary>
        /// 提取自iOS,参考,不使用
        /// </summary>
        public enum NSLayoutAttribute1 : long
        {
            NoAttribute = 0L,
            Left = 1L,
            Right = 2L,
            Top = 3L,
            Bottom = 4L,
            Leading = 5L,
            Trailing = 6L,
            Width = 7L,
            Height = 8L,
            CenterX = 9L,
            CenterY = 10L,
            Baseline = 11L,

            LastBaseline = 11L,

            FirstBaseline = 12L,

            LeftMargin = 13L,

            RightMargin = 14L,

            TopMargin = 0xFL,

            BottomMargin = 0x10L,

            LeadingMargin = 17L,

            TrailingMargin = 18L,

            CenterXWithinMargins = 19L,

            CenterYWithinMargins = 20L
        }

        /* public static class ConstrantLayoutParamsHelper
         {
             /// <summary>
             /// TODO:Test
             /// </summary>
             /// <param name="view"></param>
             /// <param name="secondView"></param>
             /// <param name="margin"></param>
             /// <returns></returns>
             /// <exception cref="ArgumentException"></exception>
             public static View LeftToLeft(this View view, View secondView, int margin = 0)
             {
                 if (margin == 0)
                 {
                     ((ConstraintLayout.LayoutParams)view.LayoutParameters).LeftToLeft = secondView.GetId();
                     return view;
                 }

                 //获取布局树里最近的ConstraitLayout
                 View parent = view;
                 while (true)
                 {
                     //这里用循环获取ConstraintLayout是防止不按照每个组件只有一个父Layout的最佳原则
                     var p = (View)parent.Parent;
                     if (p == null)
                         throw new ArgumentException("No ConstraintLayout in Parents");
                     parent = p as ConstraintLayout;
                     if (parent != null)
                         break;
                     else
                         parent = p;
                 }

                 //创建辅助线
                 var line = new Space(parent.Context) { Id = View.GenerateViewId() };
                 var lp = new ConstraintLayout.LayoutParams(0, 0);
                 if (margin < 0)
                 {
                     lp.RightToLeft = secondView.GetId();
                     lp.SetRightMargins(-margin);
                 }
                 else
                 {
                     lp.LeftToLeft = secondView.GetId();
                     lp.SetLeftMargins(margin);
                 }

                  ((ConstraintLayout)parent).AddView(line, lp);
                 //和辅助线对齐
                 ((ConstraintLayout.LayoutParams)view.LayoutParameters).LeftToLeft = line.Id;
                 return view;
             }


             /// <summary>
             /// TODO:Test
             /// </summary>
             /// <param name="view"></param>
             /// <param name="secondView"></param>
             /// <param name="margin"></param>
             /// <returns></returns>
             /// <exception cref="ArgumentException"></exception>
             public static View LeftToRight(this View view, View secondView, int margin = 0)
             {
                 if (margin == 0)
                 {
                     ((ConstraintLayout.LayoutParams)view.LayoutParameters).LeftToRight = secondView.GetId();
                     return view;
                 }

                 //获取布局树里最近的ConstraitLayout
                 View parent = view;
                 while (true)
                 {
                     //这里用循环获取ConstraintLayout是防止不按照每个组件只有一个父Layout的最佳原则
                     var p = (View)parent.Parent;
                     if (p == null)
                         throw new ArgumentException("No ConstraintLayout in Parents");
                     parent = p as ConstraintLayout;
                     if (parent != null)
                         break;
                     else
                         parent = p;
                 }

                 //创建辅助线
                 var line = new Space(parent.Context) { Id = View.GenerateViewId() };
                 var lp = new ConstraintLayout.LayoutParams(0, 0);
                 if (margin < 0)
                 {
                     lp.RightToRight = secondView.GetId();
                     lp.SetRightMargins(-margin);
                 }
                 else
                 {
                     lp.LeftToRight = secondView.GetId();
                     lp.SetLeftMargins(margin);
                 }

                  ((ConstraintLayout)parent).AddView(line, lp);
                 //和辅助线对齐
                 ((ConstraintLayout.LayoutParams)view.LayoutParameters).LeftToLeft = line.Id;
                 return view;
             }


             /// <summary>
             /// TODO:Test
             /// </summary>
             /// <param name="view"></param>
             /// <param name="secondView"></param>
             /// <param name="margin"></param>
             /// <returns></returns>
             /// <exception cref="ArgumentException"></exception>
             public static View TopToTop(this View view, View secondView, int margin = 0)
             {
                 if (margin == 0)
                 {
                     ((ConstraintLayout.LayoutParams)view.LayoutParameters).TopToTop = secondView.GetId();
                     return view;
                 }

                 //获取布局树里最近的ConstraitLayout
                 View parent = view;
                 while (true)
                 {
                     //这里用循环获取ConstraintLayout是防止不按照每个组件只有一个父Layout的最佳原则
                     var p = (View)parent.Parent;
                     if (p == null)
                         throw new ArgumentException("No ConstraintLayout in Parents");
                     parent = p as ConstraintLayout;
                     if (parent != null)
                         break;
                     else
                         parent = p;
                 }

                 //创建辅助线
                 var line = new Space(parent.Context) { Id = View.GenerateViewId() };
                 var lp = new ConstraintLayout.LayoutParams(0, 0);
                 if (margin < 0)
                 {
                     lp.BottomToTop = secondView.GetId();
                     lp.SetRightMargins(-margin);
                 }
                 else
                 {
                     lp.TopToTop = secondView.GetId();
                     lp.SetLeftMargins(margin);
                 }

                  ((ConstraintLayout)parent).AddView(line, lp);
                 //和辅助线对齐
                 ((ConstraintLayout.LayoutParams)view.LayoutParameters).TopToTop = line.Id;
                 return view;
             }

             public static View TopToBottom(this View view, View secondView, int margin = 0)
             {
                 var first = ((ConstraintLayout.LayoutParams)view.LayoutParameters);
                 first.TopToBottom = secondView.GetId();
                 first.TopMargin = margin;
                 return view;
             }

             public static View RightToLeft(this View view, View secondView, int margin = 0)
             {
                 var first = ((ConstraintLayout.LayoutParams)view.LayoutParameters);
                 first.RightToLeft = secondView.GetId();
                 first.RightMargin = margin;
                 return view;
             }

             public static View RightToRight(this View view, View secondView, int margin = 0)
             {
                 var first = ((ConstraintLayout.LayoutParams)view.LayoutParameters);
                 first.RightToRight = secondView.GetId();
                 first.RightMargin = margin;
                 return view;
             }

             public static View BottomToTop(this View view, View secondView, int margin = 0)
             {
                 var first = ((ConstraintLayout.LayoutParams)view.LayoutParameters);
                 first.BottomToTop = secondView.GetId();
                 first.BottomMargin = margin;
                 return view;
             }

             public static View BottomTBottom(this View view, View secondView, int margin = 0)
             {
                 var first = ((ConstraintLayout.LayoutParams)view.LayoutParameters);
                 first.BottomToBottom = secondView.GetId();
                 first.BottomMargin = margin;
                 return view;
             }

             public static View  CenterTo(this View view, View secondView)
             {
                 var first = ((ConstraintLayout.LayoutParams)view.LayoutParameters);
                 first.LeftToLeft = secondView.GetId();
                 first.RightToRight = secondView.GetId();
                 first.TopToTop = secondView.GetId();
                 first.BottomToBottom = secondView.GetId();
                 return view;
             }

             public static View CenterXTo(this View view, View secondView)
             {
                 var first = ((ConstraintLayout.LayoutParams)view.LayoutParameters);
                 first.LeftToLeft = secondView.GetId();
                 first.RightToRight = secondView.GetId();
                 return view;
             }

             public static View CenterYTo(this View view, View secondView)
             {
                 var first = ((ConstraintLayout.LayoutParams)view.LayoutParameters);
                 first.TopToTop = secondView.GetId();
                 first.BottomToBottom = secondView.GetId();
                 return view;
             }
         }*/
}
#endif