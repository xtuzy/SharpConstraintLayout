using System;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet.Element;

#if __MAUI__
using UIElement = Microsoft.Maui.Controls.View;
#elif WINDOWS
using UIElement = Microsoft.UI.Xaml.UIElement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
#elif __IOS__
using UIElement = UIKit.UIView;
#elif __ANDROID__
using UIElement = Android.Views.View;
using Android.Content;
#endif

namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// Set size how to match constraint when set SizeBehavior is <see cref="SizeBehavier.MatchConstraint"/>
    /// </summary>
    public enum ConstrainedSizeBehavier
    {
        MatchConstraintSpread = ConstraintSet.MatchConstraintSpread,
        MatchConstraintWrap = ConstraintSet.MatchConstraintWrap,
        MatchConstraintPercent = ConstraintSet.MatchConstraintPercent,
    }

    public enum SizeBehavier
    {
        WrapContent = ConstraintSet.WrapContent,
        MatchParent = ConstraintSet.MatchParent,
        MatchConstraint = ConstraintSet.MatchConstraint,
    }

    public enum Edge
    {
        Left = ConstraintSet.Left,
        Right = ConstraintSet.Right,
        Top = ConstraintSet.Top,
        Bottom = ConstraintSet.Bottom,
        Baseline = ConstraintSet.Baseline,
        Start = ConstraintSet.Start,
        End = ConstraintSet.End,
    }

    public enum ConstrainedEdge
    {
        /// <summary>
        /// such as <see cref="Element.LeftToLeft(UIElement, int)"/> and <see cref="Element.LeftToRight(UIElement, int)"/>
        /// </summary>
        Left = ConstraintSet.Left,
        Right = ConstraintSet.Right,
        Top = ConstraintSet.Top,
        Bottom = ConstraintSet.Bottom,
        Baseline = ConstraintSet.Baseline,
        Start = ConstraintSet.Start,
        End = ConstraintSet.End,
        Clrcle = ConstraintSet.CircleReference,
        Center = 9,
        CenterX = 10,
        CenterY = 11,
    }

    /// <summary>
    /// Guideline Orientation of line.
    /// </summary>
    public enum Orientation
    {
        X = ConstraintSet.Horizontal,
        Y = ConstraintSet.Vertical,
    }

    /// <summary>
    /// Barrier
    /// </summary>
    public enum Direction
    {
        Left = Barrier.LEFT,
        Right = Barrier.RIGHT,
        Top = Barrier.TOP,
        Bottom = Barrier.BOTTOM,
    }

    public enum Visibility
    {
        Visible = ConstraintSet.Visible,
        Invisible = ConstraintSet.Invisible,
        Gone = ConstraintSet.Gone,
    }

    public enum ChainStyle
    {
        Spread = ConstraintSet.ChainSpread,
        SpreadInside = ConstraintSet.ChainSpreadInside,
        Packed = ConstraintSet.ChainPacked,
    }

    /// <summary>
    /// 设置child
    /// </summary>
    public class FluentConstraintSet : ConstraintSet
    {
        double density;
        public double Density
        {
            get{
                if(density == 0)
                    density = DeviceDisplay.MainDisplayInfo.Density;
                return density;
            }
        }

        public Element Select(params UIElement[] views)
        {
            ElementType type;
            return new Element(this, views);
        }

        private void ApiDesign()
        {
            using (var c = new FluentConstraintSet())
            {
                // c.Clone(null);
                c.Select(null).LeftToLeft(null).LeftToRight(null)
                .Select(null).RightToLeft(null).RightToRight(null)
                .Select(null).TopToTop(null).TopToBottom(null)
                .Select(null).BottomToTop(null).BottomToBottom(null);
                c.ApplyTo(null);
            }
        }

        public FluentConstraintSet CreateXChain(UIElement leftView, Edge leftSide, UIElement rightView, Edge rightSide, UIElement[] chainViews, float[] weights, ChainStyle style)
        {
            int[] chainIds = new int[chainViews.Length];
            for (var index = 0; index < chainViews.Length; index++)
            {
                chainIds[index] = chainViews[index].GetId();
            }
            this.CreateHorizontalChain(leftView.GetId(), (int)leftSide, rightView.GetId(), (int)rightSide, chainIds, weights, (int)style);
            return this;
        }
        public FluentConstraintSet CreateYChain(UIElement topView, Edge topSide, UIElement bottomView, Edge bottomSide, UIElement[] chainViews, float[] weights, ChainStyle style)
        {
            int[] chainIds = new int[chainViews.Length];
            for (var index = 0; index < chainViews.Length; index++)
            {
                chainIds[index] = chainViews[index].GetId();
            }
            this.CreateVerticalChain(topView.GetId(), (int)topSide, bottomView.GetId(), (int)bottomSide, chainIds, weights, (int)style);
            return this;
        }

        public class Element : IDisposable//,IElement
        {
            internal enum ElementType
            {
                Normal,
                Guideline,
                Barrier,
                ConstraintHelper,
            }

            void ValidateGuideline()
            {
                if (ids.Length > 1)
                {
                    throw new NotImplementedException("You select multiple elements when you set Guideline, please only select one at a time");
                }
                if (type != ElementType.Guideline)
                    throw new ArgumentException($"You select a {type} ui element when you set Guideline");
            }

            void ValidateBarrier()
            {
                if (ids.Length > 1)
                {
                    throw new NotImplementedException("You select multiple elements when you set Barrier, please only select one at a time");
                }
                if (type != ElementType.Barrier)
                    throw new ArgumentException($"You select a {type} ui element as Barrier");
            }

            void ValidateHelper()
            {
                if (ids.Length > 1)
                {
                    throw new NotImplementedException("You select multiple elements when you set ConstraintHelper, please only select one at a time");
                }
                if (type != ElementType.ConstraintHelper)
                    throw new ArgumentException($"You select a {type} ui element as ConstraintHelper");
            }

            int Dp2Px(float value)
            {
                if (value == 0)
                    return 0;
                return (int)(value * density + 0.5);
            }

            WeakReference<ConstraintSet> setReference;
            private int[] ids;
            private ElementType type;
            double density;
            internal Element(FluentConstraintSet set, params UIElement[] views)
            {
                density = set.Density;
                setReference = new WeakReference<ConstraintSet>(set);
                Select(views);
            }

            /// <summary>
            /// It mean reselect, same as <see cref="FluentConstraintSet.Select(UIElement)"/>, only for not recreate element object waste memory and more fluent api.
            /// </summary>
            /// <param name="views"></param>
            /// <returns></returns>
            public Element Select(params UIElement[] views)
            {
                int[] ids = new int[views.Length];
                for (var index = 0; index < views.Length; index++)
                {
                    ids[index] = views[index].GetId();
                }
                this.ids = ids;

                //For validate Guideline,Barrier,ConstraintHepler 
                UIElement view = views[0];
                if (view is Guideline)
                {
                    type = ElementType.Guideline;
                }
                else if (view is Barrier)
                {
                    type = ElementType.Barrier;
                }
                else if (view is ConstraintHelper)
                {
                    type = ElementType.ConstraintHelper;
                }
                else
                {
                    type = ElementType.Normal;
                }

                return this;
            }

            #region Position

            protected Element LeftToLeft(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Left, secondView, ConstraintSet.Left, Dp2Px(margin));
                return this;
            }

            public Element LeftToLeft(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId, (margin));
                }
                return this.LeftToLeft(secondView.GetId(), (margin));
            }

            protected Element LeftToRight(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Left, secondView, ConstraintSet.Right, Dp2Px(margin));
                return this;
            }

            public Element LeftToRight(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.LeftToRight(ConstraintSet.ParentId, (margin));
                }
                return this.LeftToRight(secondView.GetId(), (margin));
            }

            protected Element TopToTop(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Top, secondView, ConstraintSet.Top, Dp2Px(margin));
                return this;
            }

            public Element TopToTop(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.TopToTop(ConstraintSet.ParentId, (margin));
                }
                return this.TopToTop(secondView.GetId(), (margin));
            }

            protected Element TopToBottom(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Top, secondView, ConstraintSet.Bottom, Dp2Px(margin));
                return this;
            }

            public Element TopToBottom(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.TopToBottom(ConstraintSet.ParentId, (margin));
                }
                return this.TopToBottom(secondView.GetId(), (margin));
            }

            protected Element RightToLeft(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Right, secondView, ConstraintSet.Left, Dp2Px(margin));
                return this;
            }

            public Element RightToLeft(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.RightToLeft(ConstraintSet.ParentId, (margin));
                }
                return this.RightToLeft(secondView.GetId(), (margin));
            }

            protected Element RightToRight(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Right, secondView, ConstraintSet.Right, Dp2Px(margin));
                return this;
            }

            public Element RightToRight(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.RightToRight(ConstraintSet.ParentId, (margin));
                }
                return this.RightToRight(secondView.GetId(), (margin));
            }

            protected Element BottomToTop(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Bottom, secondView, ConstraintSet.Top, Dp2Px(margin));
                return this;
            }

            public Element BottomToTop(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.BottomToTop(ConstraintSet.ParentId, (margin));
                }
                return this.BottomToTop(secondView.GetId(), (margin));
            }

            protected Element BottomToBottom(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Bottom, secondView, ConstraintSet.Bottom, Dp2Px(margin));
                return this;
            }

            public Element BottomToBottom(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.BottomToBottom(ConstraintSet.ParentId, (margin));
                }
                return this.BottomToBottom(secondView.GetId(), (margin));
            }

            [Obsolete("Maui have a complex text align, generate bug easily, so i avoid use baseline")]
            protected Element BaselineToBaseline(int secondView, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Baseline, secondView, ConstraintSet.Baseline, Dp2Px(margin));
                return this;
            }

            [Obsolete("Maui have a complex text align, generate bug easily, so i avoid use baseline")]
            public Element BaselineToBaseline(UIElement secondView = null, float margin = 0)
            {
                if (secondView == null)
                {
                    return this.BaselineToBaseline(ConstraintSet.ParentId, (margin));
                }
                return this.BaselineToBaseline(secondView.GetId(), (margin));
            }

            public Element CenterTo(UIElement secondView = null)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId)
                        .RightToRight(ConstraintSet.ParentId)
                        .TopToTop(ConstraintSet.ParentId)
                        .BottomToBottom(ConstraintSet.ParentId);
                }
                return this.CenterXTo(secondView).CenterYTo(secondView);
            }

            public Element CenterXTo(UIElement secondView = null)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId).RightToRight(ConstraintSet.ParentId);
                }
                return this.LeftToLeft(secondView).RightToRight(secondView);
            }

            public Element CenterYTo(UIElement secondView = null)
            {
                if (secondView == null)
                {
                    return this.TopToTop(ConstraintSet.ParentId).BottomToBottom(ConstraintSet.ParentId);
                }
                return this.TopToTop(secondView).BottomToBottom(secondView);
            }

            public Element EdgesTo(UIElement secondView = null, float xmargin = 0, float ymargin = 0)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId, xmargin)
                        .RightToRight(ConstraintSet.ParentId, xmargin)
                        .TopToTop(ConstraintSet.ParentId, ymargin)
                        .BottomToBottom(ConstraintSet.ParentId, ymargin);
                }
                return this.EdgesXTo(secondView, xmargin).EdgesYTo(secondView, ymargin);
            }

            public Element EdgesXTo(UIElement secondView = null, float xmargin = 0)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId, xmargin).RightToRight(ConstraintSet.ParentId, xmargin);
                }
                return this.LeftToLeft(secondView, xmargin).RightToRight(secondView, xmargin);
            }

            public Element EdgesYTo(UIElement secondView = null, float ymargin = 0)
            {
                if (secondView == null)
                {
                    return this.TopToTop(ConstraintSet.ParentId, ymargin).BottomToBottom(ConstraintSet.ParentId, ymargin);
                }
                return this.TopToTop(secondView, ymargin).BottomToBottom(secondView, ymargin);
            }

            /// <summary>
            /// See <see cref="ConstraintSet.SetMargin(int, int, int)"/>
            /// </summary>
            /// <param name="edge"></param>
            /// <param name="margin"></param>
            /// <returns></returns>
            public Element Margin(Edge edge, float margin)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetMargin(id, (int)edge, Dp2Px(margin));
                return this;
            }

            /// <summary>
            /// See <see cref="ConstraintSet.SetGoneMargin(int, int, int)"/>
            /// </summary>
            /// <param name="edge"></param>
            /// <param name="margin"></param>
            /// <returns></returns>
            public Element GoneMargin(Edge edge, float margin)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGoneMargin(id, (int)edge, Dp2Px(margin));
                return this;
            }

            /// <summary>
            /// See <see cref="ConstraintSet.SetVisibility(int, int)"></see>
            /// </summary>
            /// <param name="visibility"></param>
            /// <returns></returns>
            public Element Visibility(Visibility visibility)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetVisibility(id, (int)visibility);
                return this;
            }

            /// <summary>
            /// See <see cref="ConstraintSet.Connect(int, int, int, int, int)"/>
            /// </summary>
            /// <param name="startSide"></param>
            /// <param name="endView"></param>
            /// <param name="endSide"></param>
            /// <param name="margin"></param>
            /// <returns></returns>
            public Element EdgeTo(Edge startSide, UIElement endView, Edge endSide, float margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, (int)startSide, endView.GetId(), (int)endSide, Dp2Px(margin));
                return this;
            }

            /// <summary>
            /// Set views at centerView's Circle.
            /// See <see cref="ConstraintSet.ConstrainCircle(int, int, int, float)"/>.
            /// </summary>
            /// <param name="centerView"></param>
            /// <param name="radius"></param>
            /// <param name="angles">center of centerView is origin, the angle is positive value when rotate with a clockwise from y-axis. That means at top of centerView, angle is 0, at right of centerView angle is 90</param>
            /// <returns></returns>
            /// <exception cref="ArgumentException">radius' and angles' count must be equal to View's count</exception>
            public Element CircleTo(UIElement centerView, int[] radius, float[] angles)
            {
                if (radius.Length != ids.Length)
                    throw new ArgumentException($"{nameof(CircleTo)}:radius' count must be equal to View's count");
                if (angles.Length != ids.Length)
                    throw new ArgumentException($"{nameof(CircleTo)}:angles' count must be equal to View's count");
                setReference.TryGetTarget(out var set);
                for (var index = 0; index < ids.Length; index++)
                {
                    set?.ConstrainCircle(ids[index], centerView.GetId(), Dp2Px(radius[index]), angles[index]);
                }
                return this;
            }

            public Element XBias(float bias)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetHorizontalBias(id, bias);
                return this;
            }

            public Element YBias(float bias)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetVerticalBias(id, bias);
                return this;
            }

            #endregion Position

            #region Size

            /// <summary>
            /// See <see cref="ConstraintSet.ConstrainedWidth(int, bool)"/>
            /// </summary>
            /// <param name="isConstrained"></param>
            /// <returns></returns>
            public Element HasConstrainedHeight(bool isConstrained)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainedHeight(id, isConstrained);
                return this;
            }

            /// <summary>
            /// See <see cref="ConstraintSet.ConstrainedHeight(int, bool)"/>
            /// </summary>
            /// <param name="isConstrained"></param>
            /// <returns></returns>
            public Element HasConstrainedWidth(bool isConstrained)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainedWidth(id, isConstrained);
                return this;
            }

            public Element DefultHeight(ConstrainedSizeBehavier behavier)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainDefaultHeight(id, (int)behavier);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.ConstrainHeight(int, int)(int, int)"/>
            /// </summary>
            /// <param name="height"></param>
            /// <returns></returns>
            public Element Height(float height)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainHeight(id, Dp2Px(height));
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.ConstrainHeight(int, int)(int, int)"/>
            /// </summary>
            /// <param name="behavier"></param>
            /// <returns></returns>
            public Element Height(SizeBehavier behavier)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainHeight(id, (int)behavier);
                return this;
            }

            public Element MaxHeight(float height)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainMaxHeight(id, Dp2Px(height));
                return this;
            }

            public Element MinHeight(float height)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainMinHeight(id, Dp2Px(height));
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.ConstrainPercentHeight(int, float)"/>
            /// </summary>
            /// <param name="percent">view = percent * parent</param>
            /// <returns></returns>
            public Element PercentHeight(float percent)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainPercentHeight(id, percent);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.SetHorizontalWeight(int, float)"/>
            /// </summary>
            /// <param name="weight">in chain</param>
            /// <returns></returns>
            public Element XWeight(float weight)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetHorizontalWeight(id, weight);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.ConstrainWidth(int, int)"/>
            /// </summary>
            /// <param name="width"></param>
            /// <returns></returns>
            public Element Width(float width)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainWidth(id, Dp2Px(width));
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.ConstrainWidth(int, int)"/>
            /// </summary>
            /// <param name="behavier"></param>
            /// <returns></returns>
            public Element Width(SizeBehavier behavier)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainWidth(id, (int)behavier);
                return this;
            }

            public Element MaxWidth(float width)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainMaxWidth(id, Dp2Px(width));
                return this;
            }

            public Element MinWidth(float width)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainMinWidth(id, Dp2Px(width));
                return this;
            }

            /// <summary>
            /// See <see cref="ConstraintSet.ConstrainPercentWidth(int, float)"/>
            /// </summary>
            /// <param name="percent">view = percent * parent</param>
            /// <returns></returns>
            public Element PercentWidth(float percent)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainPercentWidth(id, percent);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.SetVerticalWeight(int, float)(int, float)"/>
            /// </summary>
            /// <param name="weight">in chain</param>
            /// <returns></returns>
            public Element YWeight(float weight)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetVerticalWeight(id, weight);
                return this;
            }

            /// <summary>
            /// "w:h", it calculate according to <see cref="Width(int)"/> or <see cref="Height(int)"/>, not other like <see cref="MinWidth(int)"/>.
            /// </summary>
            /// <param name="ratio">notice it is a string, it should like "w:h", such as "16:9"</param>
            /// <returns></returns>
            public Element WHRatio(string ratio)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetDimensionRatio(id, ratio);
                return this;
            }

            #endregion Size

            #region ConstraintHelper
            public Element GuidelineOrientation(Orientation orientation)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Create(id, (int)orientation);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.SetGuidelinePercent(int, float)"
            /// </summary>
            /// <param name="percent"></param>
            /// <returns></returns>
            public Element GuidelinePercent(float percent)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGuidelinePercent(id, percent);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.SetGuidelineBegin(int, int)"></see>
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public Element GuidelineBegin(float value)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGuidelineBegin(id, Dp2Px(value));
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.SetGuidelineEnd(int, int)"/>
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public Element GuidelineEnd(float value)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGuidelineEnd(id, Dp2Px(value));
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.CreateBarrier(int, int, int, int[])"/>
            /// </summary>
            /// <param name="direction"></param>
            /// <param name="margin"></param>
            /// <param name="referenced"></param>
            /// <returns></returns>
            public Element Barrier(Direction direction, float margin, params int[] referenced)
            {
                ValidateBarrier();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.CreateBarrier(id, (int)direction, Dp2Px(margin), referenced);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.CreateBarrier(int, int, int, int[])"/>
            /// </summary>
            /// <param name="direction"></param>
            /// <param name="margin"></param>
            /// <param name="referenced"></param>
            /// <returns></returns>
            public Element Barrier(Direction direction, float margin, params UIElement[] referenced)
            {
                ValidateBarrier();

                setReference.TryGetTarget(out var set);
                int[] referencedIds = new int[referenced.Length];
                for (var index = 0; index < referenced.Length; index++)
                {
                    referencedIds[index] = referenced[index].GetId();
                }
                foreach (var id in ids)
                    set?.CreateBarrier(id, (int)direction, Dp2Px(margin), referencedIds);
                return this;
            }

            public Element ReferenceElement(params UIElement[] referenced)
            {
                ValidateHelper();
                setReference.TryGetTarget(out var set);
                int[] referencedIds = new int[referenced.Length];
                for (var index = 0; index < referenced.Length; index++)
                {
                    referencedIds[index] = referenced[index].GetId();
                }
                foreach (var id in ids)
                    set?.SetReferencedIds(id, referencedIds);
                return this;
            }

            #endregion ConstraintHelper

            #region Chain
            protected Element AddToXChain(int leftId, int rightId)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.AddToHorizontalChain(id, leftId, rightId);
                return this;
            }

            public Element AddToXChain(UIElement leftView, UIElement rightView)
            {
                return AddToXChain(leftView.GetId(), rightView.GetId());
            }

            protected Element AddToYChain(int topId, int bottomId)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.AddToHorizontalChain(id, topId, bottomId);
                return this;
            }

            public Element AddToYChain(UIElement topView, UIElement bottomView)
            {
                return AddToYChain(topView.GetId(), bottomView.GetId());
            }

            /// <summary>
            /// See <see cref="ConstraintSet.RemoveFromHorizontalChain(int)"/>
            /// </summary>
            /// <returns></returns>
            public Element RemoveFromXChain()
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.RemoveFromHorizontalChain(id);
                return this;
            }

            /// <summary>
            /// See <see cref="ConstraintSet.RemoveFromVerticalChain(int)"/>
            /// </summary>
            /// <returns></returns>
            public Element RemoveFromYChain()
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.RemoveFromVerticalChain(id);
                return this;
            }

            protected Element CreateXChain(int leftId, int leftSide, int rightId, int rightSide, int[] chainIds, float[] weights, int style)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.CreateHorizontalChain(leftId, leftSide, rightId, rightSide, chainIds, weights, style);
                return this;
            }

            public Element CreateXChain(UIElement leftView, Edge leftSide, UIElement rightView, Edge rightSide, ChainStyle chainStyle, params (UIElement, float)[] chainViewsAndWeights)
            {
                int[] chainIds = new int[chainViewsAndWeights.Length];
                float[] weights = new float[chainViewsAndWeights.Length];
                for (var index = 0; index < chainViewsAndWeights.Length; index++)
                {
                    chainIds[index] = chainViewsAndWeights[index].Item1.GetId();
                    weights[index] = chainViewsAndWeights[index].Item2;
                }
                return CreateXChain(leftView.GetId(), (int)leftSide, rightView.GetId(), (int)rightSide, chainIds, weights, (int)chainStyle);
            }

            protected Element CreateYChain(int topId, int topSide, int bottomId, int bottomSide, int[] chainIds, float[] weights, int style)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.CreateVerticalChain(topId, topSide, bottomId, bottomSide, chainIds, weights, style);
                return this;
            }

            public Element CreateYChain(UIElement topView, Edge topSide, UIElement bottomView, Edge bottomSide, ChainStyle chainStyle, params (UIElement, float)[] chainViewsAndWeights)
            {
                int[] chainIds = new int[chainViewsAndWeights.Length];
                float[] weights = new float[chainViewsAndWeights.Length];
                for (var index = 0; index < chainViewsAndWeights.Length; index++)
                {
                    chainIds[index] = chainViewsAndWeights[index].Item1.GetId();
                    weights[index] = chainViewsAndWeights[index].Item2;
                }
                return CreateYChain(topView.GetId(), (int)topSide, bottomView.GetId(), (int)bottomSide, chainIds, weights, (int)chainStyle);
            }

            /// <summary>
            /// See <see cref="ConstraintSet.SetHorizontalChainStyle(int, int)"/>
            /// </summary>
            /// <param name="chainStyle"></param>
            /// <returns></returns>
            public Element XChainStyle(ChainStyle chainStyle)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetHorizontalChainStyle(id, (int)chainStyle);
                return this;
            }

            /// <summary>
            /// See <see cref="ConstraintSet.SetVerticalChainStyle(int, int)"/>
            /// </summary>
            /// <param name="chainStyle"></param>
            /// <returns></returns>
            public Element YChainStyle(ChainStyle chainStyle)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetVerticalChainStyle(id, (int)chainStyle);
                return this;
            }

            #endregion Chain

            #region Set View Property

            /// <summary>
            /// See <see cref="ConstraintSet.SetRotation(int, float)"/>
            /// </summary>
            /// <param name="rotation"></param>
            /// <returns></returns>
            public Element Rotation(float rotation)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetRotation(id, rotation);
                return this;
            }

            public Element RotationX(float rotation)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetRotationX(id, rotation);
                return this;
            }

            public Element RotationY(float rotation)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetRotationY(id, rotation);
                return this;
            }

            public Element Translation(float translationX, float translationY)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetTranslation(id, translationX, translationY);
                return this;
            }

            public Element TranslationX(float translationX)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetTranslationX(id, translationX);
                return this;
            }

            public Element TranslationY(float translationY)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetTranslationY(id, translationY);
                return this;
            }

            public Element Scale(float scale)
            {
                return ScaleX(scale).ScaleY(scale);
            }

            public Element ScaleX(float scaleX)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetScaleX(id, scaleX);
                return this;
            }

            public Element ScaleY(float scaleY)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetScaleY(id, scaleY);
                return this;
            }

            public Element Alpha(float alpha)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetAlpha(id, alpha);
                return this;
            }

            #endregion

            #region Clear

            public Element Clear()
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Clear(id);
                return this;
            }

            public Element Clear(ConstrainedEdge type)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                {
                    switch (type)
                    {
                        case ConstrainedEdge.Left:
                        case ConstrainedEdge.Right:
                        case ConstrainedEdge.Top:
                        case ConstrainedEdge.Bottom:
                        case ConstrainedEdge.Baseline:
                        case ConstrainedEdge.Start:
                        case ConstrainedEdge.End:
                        case ConstrainedEdge.Clrcle:
                            set?.Clear(id, (int)type);
                            break;
                        case ConstrainedEdge.Center:
                            Clear(ConstrainedEdge.CenterX);
                            Clear(ConstrainedEdge.CenterY);
                            break;
                        case ConstrainedEdge.CenterX:
                            set?.Clear(id, (int)ConstrainedEdge.Left);
                            set?.Clear(id, (int)ConstrainedEdge.Right);
                            set?.Clear(id, (int)ConstrainedEdge.Start);
                            set?.Clear(id, (int)ConstrainedEdge.End);
                            break;
                        case ConstrainedEdge.CenterY:
                            set?.Clear(id, (int)ConstrainedEdge.Top);
                            set?.Clear(id, (int)ConstrainedEdge.Bottom);
                            break;
                        default:
                            break;
                    }
                }
                return this;
            }

            #endregion

            public void Dispose()
            {
                setReference = null;
                ids = null;
            }
        }
    }

    public static class FluentConstraintLayoutExtension
    {
        public static void AddElement(this ConstraintLayout layout, params UIElement[] views)
        {
            foreach (var view in views)
            {
                layout.AddElement(view);
            }
        }
    }

    public static class FluentConstraintHelperExtension
    {
#if WINDOWS && !__MAUI__
        public static void RefElement(this ConstraintHelper helper, params FrameworkElement[] views)
#else
        public static void RefElement(this ConstraintHelper helper, params UIElement[] views)
#endif
        {
            foreach (var view in views)
            {
                helper.RefElement(view);
            }
        }

#if WINDOWS && !__MAUI__
        public static void RemoveRefElement(this ConstraintHelper helper, params FrameworkElement[] views)
#else
        public static void RemoveRefElement(this ConstraintHelper helper, params UIElement[] views)
#endif
        {
            foreach (var view in views)
            {
                helper.RemoveRefElement(view);
            }
        }
    }
}