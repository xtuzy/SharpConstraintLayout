using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS
using View = Microsoft.UI.Xaml.FrameworkElement;
using UIElement = Microsoft.UI.Xaml.UIElement;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
#elif __IOS__

using View = UIKit.UIView;

using UIElement = UIKit.UIView;
#elif __ANDROID__
using Android.Content;
using View = Android.Views.View;
using AndroidX.ConstraintLayout.Widget;
#endif

namespace SharpConstraintLayout.Maui.Widget
{
    public class FluentConstraintSet : ConstraintSet
    {
        private ConstraintSet ConstraintSet;

        public FluentConstraintSet()
        {
            ConstraintSet = this;
        }

        public Element Select(View view)
        {
            return new Element(ConstraintSet, view);
        }

        public FluentConstraintSet ApplyTo(ConstraintLayout constraintLayout)
        {
            ConstraintSet.ApplyTo(constraintLayout);
            return this;
        }

        public void Dispose()
        {
            ConstraintSet?.Dispose();
            ConstraintSet = null;
        }

        public void ApiTest()
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

        public class Element : IDisposable
        {
            WeakReference<ConstraintSet> setReference;
            private int id;

            public Element(ConstraintSet set, View view)
            {
                setReference = new WeakReference<ConstraintSet>(set);
                id = view.GetId();
            }

            /// <summary>
            /// It mean reselect, same as <see cref="FluentConstraintSet.Select(View)"/>, only for not recreate element object waste memory and more fluent api.
            /// </summary>
            /// <param name="view"></param>
            /// <returns></returns>
            public Element Select(View view)
            {
                id = view.GetId();
                return this;
            }

            public Element LeftToLeft(View secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                set?.Connect(id, ConstraintSet.Left, secondView.GetId(), ConstraintSet.Left, margin);
                return this;
            }

            public Element LeftToRight(View secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                set?.Connect(id, ConstraintSet.Left, secondView.GetId(), ConstraintSet.Right, margin);
                return this;
            }

            public Element TopToTop(View secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                set?.Connect(id, ConstraintSet.Top, secondView.GetId(), ConstraintSet.Top, margin);
                return this;
            }

            public Element TopToBottom(View secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                set?.Connect(id, ConstraintSet.Top, secondView.GetId(), ConstraintSet.Bottom, margin);
                return this;
            }

            public Element RightToLeft(View secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                set?.Connect(id, ConstraintSet.Right, secondView.GetId(), ConstraintSet.Left, margin);
                return this;
            }

            public Element RightToRight(View secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                set?.Connect(id, ConstraintSet.Right, secondView.GetId(), ConstraintSet.Right, margin);
                return this;
            }

            public Element BottomToTop(View secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                set?.Connect(id, ConstraintSet.Bottom, secondView.GetId(), ConstraintSet.Top, margin);
                return this;
            }

            public Element BottomToBottom(View secondView, int margin = 0)
            {
                setReference.TryGetTarget(out var set);
                set?.Connect(id, ConstraintSet.Bottom, secondView.GetId(), ConstraintSet.Bottom, margin);
                return this;
            }

            public Element CenterTo(View secondView)
            {
                this.CenterXTo(secondView);
                this.CenterYTo(secondView);
                return this;
            }

            public Element CenterXTo(View secondView)
            {
                this.LeftToLeft(secondView);
                this.RightToRight(secondView);
                return this;
            }

            public Element CenterYTo(View secondView)
            {
                this.TopToTop(secondView);
                this.BottomToBottom(secondView);
                return this;
            }

            public Element Height(int height)
            {
                setReference.TryGetTarget(out var set);
                set?.ConstrainHeight(id, height);
                return this;
            }

            public Element Width(int width)
            {
                setReference.TryGetTarget(out var set);
                set?.ConstrainWidth(id, width);
                return this;
            }

            public Element GuidelineOrientation(int orientation)
            {
                setReference.TryGetTarget(out var set);
                set?.Create(id, orientation);
                return this;
            }

            public Element GuidelinePercent(float percent)
            {
                setReference.TryGetTarget(out var set);
                set?.SetGuidelinePercent(id, percent);
                return this;
            }

            public Element GuidelineBegin(int value)
            {
                setReference.TryGetTarget(out var set);
                set?.SetGuidelineBegin(id, value);
                return this;
            }

            public Element GuidelineEnd(int value)
            {
                setReference.TryGetTarget(out var set);
                set?.SetGuidelineEnd(id, value);
                return this;
            }

            public Element Barrier(int direction, int margin, params int[] referenced)
            {
                setReference.TryGetTarget(out var set);
                set?.CreateBarrier(id, direction, margin, referenced);
                return this;
            }

            public void Dispose()
            {
                setReference = null;
            }
        }
    }
}