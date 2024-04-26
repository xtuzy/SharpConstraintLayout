using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using System.Collections.Generic;

namespace SharpConstraintLayout.Core.Benchmark
{
    internal class FlexLayoutControlTest
    {
        public MyFlexLayout LayoutLotsOfItemsNoWrap(int count = 100)
        {
            var layout = new MyFlexLayout { Wrap = FlexWrap.Wrap };

            var parent = new Grid();

            layout.Parent = parent;

            // Vary the size of the layout and the views
            double layoutWidth = 1000;
            double layoutHeight = 600;
            layout.WidthRequest = layoutWidth;
            layout.HeightRequest = layoutHeight;
            List<View> _views = new List<View>();
            for (int x = 0; x < count; x++)
            {
                var view = new Border();
                _views.Add(view);
                view.WidthRequest = 200;
                view.HeightRequest = 100;
                layout.Add(view);

                // Vary the properties of the views
                FlexLayout.SetBasis(view, (x % 2 == 0) ? FlexBasis.Auto : new FlexBasis(1, true));
                FlexLayout.SetOrder(view, x % 5);
                FlexLayout.SetGrow(view, x % 3);
                FlexLayout.SetShrink(view, x % 4);
            }
            //layout.Layout(new Rect(0, 0, layoutWidth, layoutHeight));

            var manager = new MyFlexLayoutManager(layout);

            // Measure and arrange the layout while the first view is visible
            var measure = manager.Measure(1000, 600);
            manager.ArrangeChildren(new Rect(Point.Zero, measure));
            return layout;
        }

        public void FlexLayoutRecognizesVisibilityChange()
        {
            var root = new Grid();
            var flexLayout = new FlexLayout() 
            { 
                AlignItems = FlexAlignItems.Center,
                AlignContent = FlexAlignContent.Center,
                JustifyContent = FlexJustify.Center,
                WidthRequest = 1000,
                HeightRequest = 1000
            } as IFlexLayout;
            var view = new TestLabel();
            var view2 = new TestLabel();

            root.Add(flexLayout);
            flexLayout.Add(view as IView);
            flexLayout.Add(view2 as IView);

            var manager = new FlexLayoutManager(flexLayout);

            // Measure and arrange the layout while the first view is visible
            var measure = manager.Measure(1000, 1000);
            manager.ArrangeChildren(new Rect(Point.Zero, measure));

            // Keep track of where the second view is arranged
            var whenVisible = view2.Frame.X;

            // Change the visibility
            view.IsVisible = false;

            // Measure and arrange againg
            measure = manager.Measure(1000, 1000);
            manager.ArrangeChildren(new Rect(Point.Zero, measure));

            var whenInvisible = view2.Frame.X;

            // The location of the second view should have changed
            // now that the first view is not visible
            var v = (whenInvisible != whenVisible);
        }

        public void PaddingOnLayout()
        //https://github.com/xamarin/Microsoft.Maui.Controls/issues/2663
        {
            var label0 = new TestLabel
            {
                IsPlatformEnabled = true,
            };
            var label1 = new TestLabel
            {
                IsPlatformEnabled = true,
            };
            var label2 = new Label
            {
                IsPlatformEnabled = true,
            };
            var g = new Grid();
            var layout = new MyFlexLayout
            {
                IsPlatformEnabled = true,
                JustifyContent = FlexJustify.SpaceBetween,
                AlignItems = FlexAlignItems.Start,

                Padding = new Thickness(20, 10, 20, 0),
                Children = {
                    label0,
                    label1,
                    label2,
                }
            };
            g.Add(layout); 
            var gs = g.Measure(500, 300);

            var s = layout.CrossPlatformMeasure(500, 300);
            layout.Layout(new Rect(0, 0, 500, 300));
            var c1 = layout.Children[0].Frame.Equals(new Rect(20, 10, 100, 20));
            var c2 = layout.Children[1].Frame.Equals(new Rect(20, 10, 100, 20));
            var c3 = layout.Children[2].Frame.Equals(new Rect(380, 10, 100, 20));
        }

        class TestLabel : Label
        {
            protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
            {
                return new Size(150, 100);
            }
        }
    }

    class MyFlexLayout : FlexLayout
    {
        protected override ILayoutManager CreateLayoutManager()
        {
            return new MyFlexLayoutManager(this);
        }
    }

    class MyFlexLayoutManager : FlexLayoutManager
    {
        public MyFlexLayoutManager(IFlexLayout flexLayout) : base(flexLayout)
        {
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            return base.Measure(widthConstraint, heightConstraint);
        }

        public Size ArrangeChildren(Rect bounds)
        {
            return base.ArrangeChildren(bounds);
        }
    }
}
