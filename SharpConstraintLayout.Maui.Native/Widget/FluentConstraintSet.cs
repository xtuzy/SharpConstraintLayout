﻿using System;
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
    /// 设置child
    /// </summary>
    public class FluentConstraintSet : ConstraintSet
    {
        public Element Select(params UIElement[] views)
        {
            ElementType type;
            return new Element(this, views);
        }

        private void ApiDesign()
        {
            using (var c = new FluentConstraintSet())
            {
                c.Clone(null);
                c.Select(null).LeftToLeft(null).LeftToRight(null)
                .Select(null).RightToLeft(null).RightToRight(null)
                .Select(null).TopToTop(null).TopToBottom(null)
                .Select(null).BottomToTop(null).BottomToBottom(null);
                c.ApplyTo(null);
            }
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
                if (type != ElementType.Guideline)
                    throw new ArgumentException($"You select a {type} ui element as Guideline");
            }

            void ValidateBarrier()
            {
                if (type != ElementType.Barrier)
                    throw new ArgumentException($"You select a {type} ui element as Barrier");
            }

            void ValidateHelper()
            {
                if (type != ElementType.ConstraintHelper)
                    throw new ArgumentException($"You select a {type} ui element as ConstraintHelper");
            }

            WeakReference<ConstraintSet> setReference;
            private int[] ids;
            private ElementType type;

            internal Element(ConstraintSet set, params UIElement[] views)
            {
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

            protected Element LeftToLeft(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Left, secondView, ConstraintSet.Left, margin);
                return this;
            }

            public Element LeftToLeft(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId, margin);
                }
                return this.LeftToLeft(secondView.GetId(), margin);
            }

            protected Element LeftToRight(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Left, secondView, ConstraintSet.Right, margin);
                return this;
            }

            public Element LeftToRight(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.LeftToRight(ConstraintSet.ParentId, margin);
                }
                return this.LeftToRight(secondView.GetId(), margin);
            }

            protected Element TopToTop(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Top, secondView, ConstraintSet.Top, margin);
                return this;
            }

            public Element TopToTop(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.TopToTop(ConstraintSet.ParentId, margin);
                }
                return this.TopToTop(secondView.GetId(), margin);
            }

            protected Element TopToBottom(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Top, secondView, ConstraintSet.Bottom, margin);
                return this;
            }

            public Element TopToBottom(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.TopToBottom(ConstraintSet.ParentId, margin);
                }
                return this.TopToBottom(secondView.GetId(), margin);
            }

            protected Element RightToLeft(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Right, secondView, ConstraintSet.Left, margin);
                return this;
            }

            public Element RightToLeft(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.RightToLeft(ConstraintSet.ParentId, margin);
                }
                return this.RightToLeft(secondView.GetId(), margin);
            }

            protected Element RightToRight(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Right, secondView, ConstraintSet.Right, margin);
                return this;
            }

            public Element RightToRight(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.RightToRight(ConstraintSet.ParentId, margin);
                }
                return this.RightToRight(secondView.GetId(), margin);
            }

            protected Element BottomToTop(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Bottom, secondView, ConstraintSet.Top, margin);
                return this;
            }

            public Element BottomToTop(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.BottomToTop(ConstraintSet.ParentId, margin);
                }
                return this.BottomToTop(secondView.GetId(), margin);
            }

            protected Element BottomToBottom(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Bottom, secondView, ConstraintSet.Bottom, margin);
                return this;
            }

            public Element BottomToBottom(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.BottomToBottom(ConstraintSet.ParentId, margin);
                }
                return this.BottomToBottom(secondView.GetId(), margin);
            }

            protected Element BaselineToBaseline(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Baseline, secondView, ConstraintSet.Baseline, margin);
                return this;
            }

            public Element BaselineToBaseline(UIElement secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.BaselineToBaseline(ConstraintSet.ParentId, margin);
                }
                return this.BaselineToBaseline(secondView.GetId(), margin);
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

            public Element EdgesTo(UIElement secondView = null, int xmargin = 0, int ymargin = 0)
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

            public Element EdgesXTo(UIElement secondView = null, int xmargin = 0)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId, xmargin).RightToRight(ConstraintSet.ParentId, xmargin);
                }
                return this.LeftToLeft(secondView, xmargin).RightToRight(secondView, xmargin);
            }

            public Element EdgesYTo(UIElement secondView = null, int ymargin = 0)
            {
                if (secondView == null)
                {
                    return this.TopToTop(ConstraintSet.ParentId, ymargin).BottomToBottom(ConstraintSet.ParentId, ymargin);
                }
                return this.TopToTop(secondView, ymargin).BottomToBottom(secondView, ymargin);
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.SetVisibility(int, int)"></see>
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
            /// Same as <see cref="ConstraintSet.Connect(int, int, int, int, int)"/>
            /// </summary>
            /// <param name="startSide"></param>
            /// <param name="endView"></param>
            /// <param name="endSide"></param>
            /// <param name="margin"></param>
            /// <returns></returns>
            public Element EdgeTo(Edge startSide, UIElement endView, Edge endSide, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, (int)startSide, endView.GetId(), (int)endSide, margin);
                return this;
            }

            /// <summary>
            /// Set views at centerView's Circle.
            /// Notice:The angle is not same as <see cref="ConstraintSet.ConstrainCircle(int, int, int, float)"/>.
            /// </summary>
            /// <param name="centerView"></param>
            /// <param name="radius"></param>
            /// <param name="angles">center of centerView is origin, the angle is positive value when rotate with a clockwise from x-axis. This is not same as ConstraintSet.ConstraintCirle</param>
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
                    set?.ConstrainCircle(ids[index], centerView.GetId(), radius[index], angles[index]);
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
            /// Same as <see cref="ConstraintSet.ConstrainHeight(int, int)(int, int)"/>
            /// </summary>
            /// <param name="height"></param>
            /// <returns></returns>
            public Element Height(int height)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainHeight(id, height);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.ConstrainHeight(int, int)(int, int)"/>
            /// </summary>
            /// <param name="behavier"></param>
            /// <returns></returns>
            public Element Height(SizeBehavier behavier)
            {
                return this.Height((int)behavier);
            }

            public Element MaxHeight(int height)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainMaxHeight(id, height);
                return this;
            }

            public Element MinHeight(int height)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainMinHeight(id, height);
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
            public Element Width(int width)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainWidth(id, width);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.ConstrainWidth(int, int)"/>
            /// </summary>
            /// <param name="behavier"></param>
            /// <returns></returns>
            public Element Width(SizeBehavier behavier)
            {
                return this.Width((int)behavier);
            }

            public Element MaxWidth(int width)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainMaxWidth(id, width);
                return this;
            }

            public Element MinWidth(int width)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainMinWidth(id, width);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.ConstrainPercentWidth(int, float)"/>
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
            public Element GuidelineBegin(int value)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGuidelineBegin(id, value);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.SetGuidelineEnd(int, int)"/>
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public Element GuidelineEnd(int value)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGuidelineEnd(id, value);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.CreateBarrier(int, int, int, int[])"/>
            /// </summary>
            /// <param name="direction"></param>
            /// <param name="margin"></param>
            /// <param name="referenced"></param>
            /// <returns></returns>
            public Element Barrier(Direction direction, int margin, params int[] referenced)
            {
                ValidateBarrier();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.CreateBarrier(id, (int)direction, margin, referenced);
                return this;
            }

            /// <summary>
            /// Same as <see cref="ConstraintSet.CreateBarrier(int, int, int, int[])"/>
            /// </summary>
            /// <param name="direction"></param>
            /// <param name="margin"></param>
            /// <param name="referenced"></param>
            /// <returns></returns>
            public Element Barrier(Direction direction, int margin, params UIElement[] referenced)
            {
                ValidateBarrier();

                setReference.TryGetTarget(out var set);
                int[] referencedIds = new int[referenced.Length];
                for (var index = 0; index < referenced.Length; index++)
                {
                    referencedIds[index] = referenced[index].GetId();
                }
                foreach (var id in ids)
                    set?.CreateBarrier(id, (int)direction, margin, referencedIds);
                return this;
            }

            #endregion ConstraintHelper

            #region Chain
            public Element AddToXChain(int leftId, int rightId)
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

            public Element AddToYChain(int topId, int bottomId)
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

            #endregion Chain

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
        public static void ReferenceElement(this ConstraintHelper helper, params FrameworkElement[] views)
#else
        public static void ReferenceElement(this ConstraintHelper helper, params UIElement[] views)
#endif
        {
            foreach (var view in views)
            {
                helper.ReferenceElement(view);
            }
        }
    }
}