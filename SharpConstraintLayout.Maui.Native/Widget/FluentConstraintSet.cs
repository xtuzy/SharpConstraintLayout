using System;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet.Element;

#if WINDOWS
using View = Microsoft.UI.Xaml.UIElement;
using UIElement = Microsoft.UI.Xaml.UIElement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
#elif __IOS__
using View = UIKit.UIView;
using UIElement = UIKit.UIView;
#elif __ANDROID__
using UIElement = Android.Views.View;
using Android.Content;
using View = Android.Views.View;
//using AndroidX.ConstraintLayout.Widget;
//using static Android.Views.ViewGroup;
#endif

namespace SharpConstraintLayout.Maui.Widget
{
    public class FluentConstraintSet : ConstraintSet
    {
        public Element Select(params View[] views)
        {
            ElementType type;
            return new Element(this, views);
        }

        public void ApiDesign()
        {
            using (var c = new FluentConstraintSet())
            {
                //c.Clone(null);
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
        /// Guideline
        /// </summary>
        public enum Orientation
        {
            Y = ConstraintSet.Horizontal,
            X = ConstraintSet.Vertical,
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

            internal Element(ConstraintSet set, params View[] views)
            {
                setReference = new WeakReference<ConstraintSet>(set);
                Select(views);
            }

            /// <summary>
            /// It mean reselect, same as <see cref="FluentConstraintSet.Select(View)"/>, only for not recreate element object waste memory and more fluent api.
            /// </summary>
            /// <param name="views"></param>
            /// <returns></returns>
            public Element Select(params View[] views)
            {
                int[] ids = new int[views.Length];
                for (var index = 0; index < views.Length; index++)
                {
                    ids[index] = views[index].GetId();
                }
                this.ids = ids;

                //For validate Guideline,Barrier,ConstraintHepler 
                View view = views[0];
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

            protected Element LeftToLeft(int secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, ConstraintSet.Left, secondView, ConstraintSet.Left, margin);
                return this;
            }

            public Element LeftToLeft(View secondView = null, int margin = 0)
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

            public Element LeftToRight(View secondView = null, int margin = 0)
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

            public Element TopToTop(View secondView = null, int margin = 0)
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

            public Element TopToBottom(View secondView = null, int margin = 0)
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

            public Element RightToLeft(View secondView = null, int margin = 0)
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

            public Element RightToRight(View secondView = null, int margin = 0)
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

            public Element BottomToTop(View secondView = null, int margin = 0)
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

            public Element BottomToBottom(View secondView = null, int margin = 0)
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

            public Element BaselineToBaseline(View secondView = null, int margin = 0)
            {
                if (secondView == null)
                {
                    return this.BaselineToBaseline(ConstraintSet.ParentId, margin);
                }
                return this.BaselineToBaseline(secondView.GetId(), margin);
            }

            public Element CenterTo(View secondView = null)
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

            public Element CenterXTo(View secondView = null)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId).RightToRight(ConstraintSet.ParentId);
                }
                return this.LeftToLeft(secondView).RightToRight(secondView);
            }

            public Element CenterYTo(View secondView = null)
            {
                if (secondView == null)
                {
                    return this.TopToTop(ConstraintSet.ParentId).BottomToBottom(ConstraintSet.ParentId);
                }
                return this.TopToTop(secondView).BottomToBottom(secondView);
            }

            public Element Height(int height)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainHeight(id, height);
                return this;
            }

            public Element Height(SizeBehavier behavier)
            {
                return this.Height((int)behavier);
            }

            public Element Width(int width)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.ConstrainWidth(id, width);
                return this;
            }

            public Element Width(SizeBehavier behavier)
            {
                return this.Width((int)behavier);
            }

            public Element GuidelineOrientation(Orientation orientation)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Create(id, (int)orientation);
                return this;
            }

            public Element GuidelinePercent(float percent)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGuidelinePercent(id, percent);
                return this;
            }

            public Element GuidelineBegin(int value)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGuidelineBegin(id, value);
                return this;
            }

            public Element GuidelineEnd(int value)
            {
                ValidateGuideline();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.SetGuidelineEnd(id, value);
                return this;
            }

            public Element Barrier(Direction direction, int margin, params int[] referenced)
            {
                ValidateBarrier();

                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.CreateBarrier(id, (int)direction, margin, referenced);
                return this;
            }

            public Element Barrier(Direction direction, int margin, params View[] referenced)
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

            public Element EdgesTo(View secondView = null, int xmargin = 0, int ymargin = 0)
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

            public Element EdgesXTo(View secondView = null, int xmargin = 0)
            {
                if (secondView == null)
                {
                    return this.LeftToLeft(ConstraintSet.ParentId, xmargin).RightToRight(ConstraintSet.ParentId, xmargin);
                }
                return this.LeftToLeft(secondView, xmargin).RightToRight(secondView, xmargin);
            }

            public Element EdgesYTo(View secondView = null, int ymargin = 0)
            {
                if (secondView == null)
                {
                    return this.TopToTop(ConstraintSet.ParentId, ymargin).BottomToBottom(ConstraintSet.ParentId, ymargin);
                }
                return this.TopToTop(secondView, ymargin).BottomToBottom(secondView, ymargin);
            }

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
            public Element EdgeTo(Edge startSide, View endView, Edge endSide, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                foreach (var id in ids)
                    set?.Connect(id, (int)startSide, endView.GetId(), (int)endSide, margin);
                return this;
            }
            /// <summary>
            /// Set views at centerView's Circle.
            /// Notice:The angle is not same as ConstraintSet.ConstraintCirle.
            /// </summary>
            /// <param name="centerView"></param>
            /// <param name="radius"></param>
            /// <param name="angles">center of centerView is origin, the angle is positive value when rotate with a clockwise from x-axis. This is not same as ConstraintSet.ConstraintCirle</param>
            /// <returns></returns>
            /// <exception cref="ArgumentException">radius' and angles' count must be equal to View's count</exception>
            public Element CircleTo(View centerView, int[] radius, float[] angles)
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

            public void Dispose()
            {
                setReference = null;
                ids = null;
            }
        }
    }

    public static class FluentConstraintLayoutExtension
    {
        public static void AddView(this ConstraintLayout layout, params View[] views)
        {
            foreach (var view in views)
            {
                layout.AddView(view);
            }
        }
    }

    public static class FluentConstraintHelperExtension
    {
#if WINDOWS
        public static void AddView(this ConstraintHelper helper, params FrameworkElement[] views)
#else
        public static void AddView(this ConstraintHelper helper, params View[] views)
#endif
        {
            foreach (var view in views)
            {
                helper.AddView(view);
            }
        }
    }
}