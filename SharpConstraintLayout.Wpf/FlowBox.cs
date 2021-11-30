using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpConstraintLayout.Wpf
{
    /// <summary>
    /// <see href="https://developer.android.com/reference/androidx/constraintlayout/helper/widget/Flow">Android Flow</see><br/>
    /// Flow VirtualLayout. Added in 2.0 Allows positioning of referenced widgets horizontally or vertically, similar to a Chain. The elements referenced are indicated via constraint_referenced_ids, as with other ConstraintHelper implementations. Those referenced widgets are then laid out by the Flow virtual layout in three possible ways:
    ///    wrap none : simply create a chain out of the referenced elements
    /// wrap chain : create multiple chains(one after the other) if the referenced elements do not fit
    /// wrap aligned : similar to wrap chain, but will align the elements by creating rows and columns
    /// As VirtualLayouts are ConstraintHelpers, they are normal views; you can thus treat them as such, and setting up constraints on them(position, dimension) or some view attributes(background, padding) will work.The main difference between VirtualLayouts and ViewGroups is that:
    /// VirtualLayout keep the hierarchy flat
    /// Other views can thus reference / constrain to not only the VirtualLayout, but also the views laid out by the VirtualLayout
    /// VirtualLayout allow on the fly behavior modifications (e.g. for Flow, changing the orientation)
    /// flow_wrapMode = "none"
    /// This will simply create an horizontal or vertical chain out of the referenced widgets.This is the default behavior of Flow.XML attributes that are allowed in this mode:
    /// flow_horizontalStyle = "spread|spread_inside|packed"
    /// flow_verticalStyle = "spread|spread_inside|packed"
    /// flow_horizontalBias = "float"
    /// flow_verticalBias = "float"
    /// flow_horizontalGap = "dimension"
    /// flow_verticalGap = "dimension"
    /// flow_horizontalAlign = "start|end"
    /// flow_verticalAlign = "top|bottom|center|baseline
    /// While the elements are laid out as a chain in the orientation defined, the way they are laid out in the other dimension is controlled by flow_horizontalAlign and flow_verticalAlign attributes.
    /// flow_wrapMode = "chain"
    /// Similar to wrap none in terms of creating chains, but if the referenced widgets do not fit the horizontal or vertical dimension (depending on the orientation picked), they will wrap around to the next line / column.XML attributes are the same same as in wrap_none, with the addition of attributes specifying chain style and chain bias applied to the first chain.This way, it is possible to specify different chain behavior between the first chain and the rest of the chains eventually created.
    /// flow_firstHorizontalStyle = "spread|spread_inside|packed"
    /// flow_firstVerticalStyle = "spread|spread_inside|packed"
    /// flow_firstHorizontalBias = "float"
    /// flow_firstVerticalBias = "float"
    /// One last important attribute is flow_maxElementsWrap, which specify the number of elements before wrapping, regardless if they fit or not in the available space.
    /// flow_wrapMode = "aligned"
    /// Same XML attributes as for WRAP_CHAIN, with the difference that the elements are going to be laid out in a set of rows and columns instead of chains.The attribute specifying chains style and bias are thus not going to be applied.
    /// </summary>
    public class FlowBox : FrameworkElement
    {
        public readonly Flow Flow = new Flow();

        public enum OrientationType
        {
            Horizontal = 0,
            Vertical
        }

        /// <summary>
        /// default is Horizontal
        /// </summary>
        public virtual OrientationType Orientation
        {
            set
            {
                Flow.Orientation = (int)value;
            }
        }
        /// <summary>
        /// Similar to HorizontalStyle, but only applies to the first chain.
        /// </summary>
        public virtual ConstraintSet.LayoutStyle FirstHorizontalStyle
        {
            set
            {
                Flow.FirstHorizontalStyle = (int)value;
            }
        }

        /// <summary>
        /// Similar to VerticalStyle, but only applies to the first chain.
        /// </summary>
        public virtual ConstraintSet.LayoutStyle FirstVerticalStyle
        {
            set
            {
                Flow.FirstVerticalStyle = (int)value;
            }
        }

        public virtual ConstraintSet.LayoutStyle LastHorizontalStyle
        {
            set
            {
                Flow.LastHorizontalStyle = (int)value;
            }
        }

        public virtual ConstraintSet.LayoutStyle LastVerticalStyle
        {
            set
            {
                Flow.LastVerticalStyle = (int)value;
            }
        }
        /// <summary>
        /// Set horizontal chain style.
        /// </summary>
        public virtual ConstraintSet.LayoutStyle HorizontalStyle
        {
            set
            {
                Flow.HorizontalStyle = (int)value;
            }
        }

        public virtual ConstraintSet.LayoutStyle VerticalStyle
        {
            set
            {
                Flow.VerticalStyle = (int)value;
            }
        }
        /// <summary>
        /// Set the horizontal bias applied to the chain
        /// </summary>
        public virtual float HorizontalBias
        {
            set
            {
                Flow.HorizontalBias = value;
            }
        }

        public virtual float VerticalBias
        {
            set
            {
                Flow.VerticalBias = value;
            }
        }

        /// <summary>
        /// Similar to HorizontalBias, but only applied to the first chain.
        /// </summary>
        public virtual float FirstHorizontalBias
        {
            set
            {
                Flow.FirstHorizontalBias = value;
            }
        }
        /// <summary>
        /// Similar to VerticalBias, but only applied to the first chain.
        /// </summary>
        public virtual float FirstVerticalBias
        {
            set
            {
                Flow.FirstVerticalBias = value;
            }
        }

        public virtual float LastHorizontalBias
        {
            set
            {
                Flow.LastHorizontalBias = value;
            }
        }

        public virtual float LastVerticalBias
        {
            set
            {
                Flow.LastVerticalBias = value;
            }
        }

        public enum HorizontalAlignType
        {
            Start,
            End,
            Center
        }

        /// <summary>
        /// Set up the horizontal alignment of the elements in the layout, 
        /// if the layout orientation is set to Flow.VERTICAL Can be either: 
        /// Flow.HORIZONTAL_ALIGN_START 
        /// Flow.HORIZONTAL_ALIGN_END 
        /// Flow.HORIZONTAL_ALIGN_CENTER
        /// </summary>
        public virtual int HorizontalAlign
        {
            set
            {
                Flow.HorizontalAlign = value;
            }
        }

        public enum VerticalAlignType
        {
            Top, Bottom, Center, Baseline
        }
        /// <summary>
        /// Set up the vertical alignment of the elements in the layout,
        /// if the layout orientation is set to Flow.HORIZONTAL Can be either: 
        /// Flow.VERTICAL_ALIGN_TOP 
        /// Flow.VERTICAL_ALIGN_BOTTOM 
        /// Flow.VERTICAL_ALIGN_CENTER 
        /// Flow.VERTICAL_ALIGN_BASELINE
        /// </summary>
        public virtual int VerticalAlign
        {
            set
            {
                Flow.VerticalAlign = value;
            }
        }

        public enum Mode
        {
            /*public const int WRAP_NONE = 0;
            public const int WRAP_CHAIN = 1;
            public const int WRAP_ALIGNED = 2;*/
            /// <summary>
            /// simply create a chain out of the referenced elements
            /// </summary>
            None,
            /// <summary>
            /// simply create a chain out of the referenced elements
            /// </summary>
            Chain,
            /// <summary>
            /// similar to wrap chain, but will align the elements by creating rows and columns
            /// </summary>
            Aligned
        }
        /// <summary>
        /// Set wrap mode for the layout.
        /// </summary>
        public virtual Mode WrapMode
        {
            set
            {
                Flow.WrapMode = (int)value;
            }
        }
        /// <summary>
        /// Set up the horizontal gap between elements
        /// </summary>
        public virtual int HorizontalGap
        {
            set
            {
                Flow.HorizontalGap = value;
            }
        }
        /// <summary>
        /// Set up the vertical gap between elements
        /// </summary>
        public virtual int VerticalGap
        {
            set
            {
                Flow.VerticalGap = value;
            }
        }
        /// <summary>
        /// Set up the maximum number of elements before wrapping.
        /// </summary>
        public virtual int MaxElementsWrap
        {
            set
            {
                Flow.MaxElementsWrap = value;
            }
        }

        /// <summary>
        /// Add view in flow,it not add to visualtree, just add in flow to calculate layout.So you also should add view to ConstraintLayout.
        /// </summary>
        /// <param name="view"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddView(FrameworkElement view)
        {
            var parent = Parent is ConstraintLayout ? Parent as ConstraintLayout : throw new ArgumentException($"{this} is not constraintlayout child");
            var widget = parent.GetWidget(view);
            Flow.add(widget);
        }

        /// <summary>
        /// Add views in flow,it not add to visualtree, just add in flow to calculate layout.So you also should add view to ConstraintLayout.
        /// </summary>
        /// <param name="views"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddViews(FrameworkElement[] views)
        {
            var parent = Parent is ConstraintLayout ? Parent as ConstraintLayout : throw new ArgumentException($"{this} is not constraintlayout child");
            foreach (FrameworkElement view in views)
            {
                var widget = parent.GetWidget(view);
                Flow.add(widget);
            }
        }
    }
}
