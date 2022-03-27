/*
 * Copyright (C) 2019 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace SharpConstraintLayout.Maui.Helper.Widget
{
    using androidx.constraintlayout.core.widgets.analyzer;
    using SharpConstraintLayout.Maui.Widget;
    using System.Collections.Generic;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using HelperWidget = androidx.constraintlayout.core.widgets.HelperWidget;

    /// <summary>
    /// Flow VirtualLayout. <b>Added in 2.0</b>
    ///
    /// Allows positioning of referenced widgets horizontally or vertically, similar to a Chain.
    ///
    /// The elements referenced are indicated via constraint_referenced_ids, as with other
    /// ConstraintHelper implementations.
    ///
    /// Those referenced widgets are then laid out by the Flow virtual layout in three possible
    /// ways: <ul><li><a href="#wrap_none">wrap none</a> : simply create a chain out of the
    /// referenced elements</li><li><a href="#wrap_chain">wrap chain</a> : create multiple chains
    /// (one after the other) if the referenced elements do not fit</li><li><a
    /// href="#wrap_aligned">wrap aligned</a> : similar to wrap chain, but will align the elements
    /// by creating rows and columns</li></ul>
    ///
    /// As VirtualLayouts are ConstraintHelpers, they are normal views; you can thus treat them as
    /// such, and setting up constraints on them (position, dimension) or some view attributes
    /// (background, padding) will work. The main difference between VirtualLayouts and ViewGroups
    /// is that: <ul><li>VirtualLayout keep the hierarchy flat</li><li>Other views can thus
    /// reference / constrain to not only the VirtualLayout, but also the views laid out by the
    /// VirtualLayout</li><li>VirtualLayout allow on the fly behavior modifications (e.g. for Flow,
    /// changing the orientation)</li></ul><h4 id="wrap_none">flow_wrapMode = "none"</h4>
    ///
    /// This will simply create an horizontal or vertical chain out of the referenced widgets. This
    /// is the default behavior of Flow.
    ///
    /// XML attributes that are allowed in this mode:
    ///
    /// <ul><li>flow_horizontalStyle = "spread|spread_inside|packed"</li><li>flow_verticalStyle =
    /// "spread|spread_inside|packed"</li><li>flow_horizontalBias = "
    /// <i>float</i>"</li><li>flow_verticalBias = " <i>float</i>"</li><li>flow_horizontalGap = "
    /// <i>dimension</i>"</li><li>flow_verticalGap = "
    /// <i>dimension</i>"</li><li>flow_horizontalAlign = "start|end"</li><li>flow_verticalAlign =
    /// "top|bottom|center|baseline</li></ul>
    ///
    /// While the elements are laid out as a chain in the orientation defined, the way they are laid
    /// out in the other dimension is controlled by <i>flow_horizontalAlign</i> and
    /// <i>flow_verticalAlign</i> attributes.
    ///
    /// <h4 id="wrap_chain">flow_wrapMode = "chain"</h4>
    ///
    /// Similar to wrap none in terms of creating chains, but if the referenced widgets do not fit
    /// the horizontal or vertical dimension (depending on the orientation picked), they will wrap
    /// around to the next line / column.
    ///
    /// XML attributes are the same same as in wrap_none, with the addition of attributes specifying
    /// chain style and chain bias applied to the first chain. This way, it is possible to specify
    /// different chain behavior between the first chain and the rest of the chains eventually
    /// created.
    ///
    /// <ul><li>flow_firstHorizontalStyle =
    /// "spread|spread_inside|packed"</li><li>flow_firstVerticalStyle =
    /// "spread|spread_inside|packed"</li><li>flow_firstHorizontalBias = "
    /// <i>float</i>"</li><li>flow_firstVerticalBias = " <i>float</i>"</li></ul>
    ///
    /// One last important attribute is <i>flow_maxElementsWrap</i>, which specify the number of
    /// elements before wrapping, regardless if they fit or not in the available space.
    ///
    /// <h4 id="wrap_aligned">flow_wrapMode = "aligned"</h4>
    ///
    /// Same XML attributes as for WRAP_CHAIN, with the difference that the elements are going to be
    /// laid out in a set of rows and columns instead of chains. The attribute specifying chains
    /// style and bias are thus not going to be applied.
    /// </summary>
    public class Flow : VirtualLayout, IFlow
    {
        private const string TAG = "Flow";

        private androidx.constraintlayout.core.widgets.Flow mFlow;

        public const int Horizontal = androidx.constraintlayout.core.widgets.Flow.HORIZONTAL;
        public const int Vertical = androidx.constraintlayout.core.widgets.Flow.VERTICAL;
        public const int WrapNone = androidx.constraintlayout.core.widgets.Flow.WRAP_NONE;
        public const int WrapChain = androidx.constraintlayout.core.widgets.Flow.WRAP_CHAIN;
        public const int WrapAligned = androidx.constraintlayout.core.widgets.Flow.WRAP_ALIGNED;

        public const int ChainSpread = ConstraintWidget.CHAIN_SPREAD;
        public const int ChainSpreadInside = ConstraintWidget.CHAIN_SPREAD_INSIDE;
        public const int ChainPacked = ConstraintWidget.CHAIN_PACKED;

        public const int HorizontalAlignStart = androidx.constraintlayout.core.widgets.Flow.HORIZONTAL_ALIGN_START;
        public const int HorizontalAlignEnd = androidx.constraintlayout.core.widgets.Flow.HORIZONTAL_ALIGN_END;
        public const int HorizontalAlignCenter = androidx.constraintlayout.core.widgets.Flow.HORIZONTAL_ALIGN_CENTER;

        public const int VerticalAlignTop = androidx.constraintlayout.core.widgets.Flow.VERTICAL_ALIGN_TOP;
        public const int VerticalAlignBottom = androidx.constraintlayout.core.widgets.Flow.VERTICAL_ALIGN_BOTTOM;
        public const int VerticalAlignCenter = androidx.constraintlayout.core.widgets.Flow.VERTICAL_ALIGN_CENTER;
        public const int VerticalAlignBaseline = androidx.constraintlayout.core.widgets.Flow.VERTICAL_ALIGN_BASELINE;

        public Flow() : base()
        {
        }

        public override void ResolveRtl(ConstraintWidget widget, bool isRtl)
        {
            mFlow.applyRtl(isRtl);
        }

        protected internal void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            OnMeasure(mFlow, widthMeasureSpec, heightMeasureSpec);
        }

        public override void OnMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec)
        {
            int widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            int heightMode = MeasureSpec.GetMode(heightMeasureSpec);
            int heightSize = MeasureSpec.GetSize(heightMeasureSpec);

            #region Copy form FlowTest
            /*
            int widthMode = BasicMeasure.UNSPECIFIED;
            int heightMode = BasicMeasure.UNSPECIFIED;
            int widthSize = 0;
            int heightSize = 0;
            if (layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
            {
                widthSize = layout.Parent != null ? layout.Parent.Width : 0;
                widthMode = BasicMeasure.EXACTLY;
            }
            else if (layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.FIXED)
            {
                widthSize = widthMeasureSpec;
                widthMode = BasicMeasure.EXACTLY;
            }
            if (layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
            {
                heightSize = layout.Parent != null ? layout.Parent.Height : 0;
                heightMode = BasicMeasure.EXACTLY;
            }
            else if (layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.FIXED)
            {
                heightSize = heightMeasureSpec;
                heightMode = BasicMeasure.EXACTLY;
            }
            */
            #endregion

            if (layout != null)
            {
                if (ConstraintLayout.DEBUG) Debug.WriteLine(TAG, $"widthMode {widthMode}, widthSize {widthSize}, heightMode {heightMode}, heightSize {heightSize}");
                layout.measure(widthMode, widthSize, heightMode, heightSize);
                //setMeasuredDimension(layout.MeasuredWidth, layout.MeasuredHeight);//Android中这个的作用应该是设置flow的大小,我注释掉
            }
            else
            {
                //setMeasuredDimension(0, 0);
            }
        }

        public override void LoadParameters(ConstraintSet.Constraint constraint, HelperWidget child, Dictionary<int, ConstraintWidget> mapIdToWidget)
        {
            base.LoadParameters(constraint, child, mapIdToWidget);
            if (child is androidx.constraintlayout.core.widgets.Flow)
            {
                androidx.constraintlayout.core.widgets.Flow flow = (androidx.constraintlayout.core.widgets.Flow)child;
                if (constraint.layout.orientation != -1)
                {
                    flow.Orientation = constraint.layout.orientation;
                }
            }
        }

        protected internal override void init()
        {
            base.init();
            mFlow = new androidx.constraintlayout.core.widgets.Flow();

            mHelperWidget = mFlow;
            ValidateParams();
        }

        /// <summary>
        /// Set the orientation of the layout
        /// </summary>
        /// <param name="orientation">either Flow.HORIZONTAL or FLow.VERTICAL</param>
        public virtual void SetOrientation(int orientation)
        {
            mFlow.Orientation = orientation;
            requestLayout();
        }

        /// <summary>
        /// Set padding around the content
        /// </summary>
        /// <param name="padding"></param>
        public virtual void SetPadding(int value)
        {
            mFlow.Padding = value;
            requestLayout();
        }

        /// <summary>
        /// Set padding left around the content
        /// </summary>
        /// <param name="paddingLeft"></param>
        public virtual void SetPaddingLeft(int value)
        {
            mFlow.PaddingLeft = value;
            requestLayout();
        }

        /// <summary>
        /// Set padding top around the content
        /// </summary>
        /// <param name="paddingTop"></param>
        public virtual void SetPaddingTop(int value)
        {
            mFlow.PaddingTop = value;
            requestLayout();
        }

        /// <summary>
        /// Set padding right around the content
        /// </summary>
        /// <param name="paddingRight"></param>
        public virtual void SetPaddingRight(int value)
        {
            mFlow.PaddingRight = value;
            requestLayout();
        }

        /// <summary>
        /// Set padding bottom around the content
        /// </summary>
        /// <param name="paddingBottom"></param>
        public virtual void SetPaddingBottom(int value)
        {
            mFlow.PaddingBottom = value;
            requestLayout();
        }

        /// <summary>
        /// Set the style of the last Horizontal column.
        /// </summary>
        /// <param name="style">
        /// Flow.CHAIN_SPREAD, Flow.CHAIN_SPREAD_INSIDE, or Flow.CHAIN_PACKED
        /// </param>
        public virtual void SetLastHorizontalStyle(int style)
        {
            mFlow.LastHorizontalStyle = style;
            requestLayout();
        }

        /// <summary>
        /// Set the style of the last vertical row.
        /// </summary>
        /// <param name="style">
        /// Flow.CHAIN_SPREAD, Flow.CHAIN_SPREAD_INSIDE, or Flow.CHAIN_PACKED
        /// </param>
        public virtual void SetLastVerticalStyle(int style)
        {
            mFlow.LastVerticalStyle = style;
            requestLayout();
        }

        /// <summary>
        /// Set the bias of the last Horizontal column.
        /// </summary>
        /// <param name="bias"></param>
        public virtual void SetLastHorizontalBias(float value)
        {
            mFlow.LastHorizontalBias = value;
            requestLayout();
        }

        /// <summary>
        /// Set the bias of the last vertical row.
        /// </summary>
        /// <param name="bias"></param>
        public virtual void SetLastVerticalBias(float value)
        {
            mFlow.LastVerticalBias = value;
            requestLayout();
        }

        /// <summary>
        /// Set wrap mode for the layout. Can be:
        ///
        /// Flow.WRAP_NONE (default) -- no wrap behavior, create a single chain Flow.WRAP_CHAIN --
        /// if not enough space to fit the referenced elements, will create additional chains after
        /// the first one Flow.WRAP_ALIGNED -- if not enough space to fit the referenced elements,
        /// will wrap the elements, keeping them aligned (like a table)
        /// </summary>
        /// <param name="mode"></param>
        public virtual void SetWrapMode(int value)
        {
            mFlow.WrapMode = value;
            requestLayout();
        }

        /// <summary>
        /// Set horizontal chain style. Can be:
        ///
        /// Flow.CHAIN_SPREAD Flow.CHAIN_SPREAD_INSIDE Flow.CHAIN_PACKED
        /// </summary>
        /// <param name="style"></param>
        public virtual void SetHorizontalStyle(int value)
        {
            mFlow.HorizontalStyle = value;
            requestLayout();
        }

        /// <summary>
        /// Set vertical chain style. Can be:
        ///
        /// Flow.CHAIN_SPREAD Flow.CHAIN_SPREAD_INSIDE Flow.CHAIN_PACKED
        /// </summary>
        /// <param name="style"></param>
        public virtual void SetVerticalStyle(int value)
        {
            mFlow.VerticalStyle = value;
            requestLayout();
        }

        /// <summary>
        /// Set the horizontal bias applied to the chain
        /// </summary>
        /// <param name="bias">from 0 to 1</param>
        public virtual void SetHorizontalBias(float bias)
        {
            mFlow.HorizontalBias = bias;
            requestLayout();
        }

        /// <summary>
        /// Set the vertical bias applied to the chain
        /// </summary>
        /// <param name="bias">from 0 to 1</param>
        public virtual void SetVerticalBias(float bias)
        {
            mFlow.VerticalBias = bias;
            requestLayout();
        }

        /// <summary>
        /// Similar to setHorizontalStyle(), but only applies to the first chain.
        /// </summary>
        /// <param name="style"></param>
        public virtual void SetFirstHorizontalStyle(int value)
        {
            mFlow.FirstHorizontalStyle = value;
            requestLayout();
        }

        /// <summary>
        /// Similar to setVerticalStyle(), but only applies to the first chain.
        /// </summary>
        /// <param name="style"></param>
        public virtual void SetFirstVerticalStyle(int value)
        {
            mFlow.FirstVerticalStyle = value;
            requestLayout();
        }

        /// <summary>
        /// Similar to setHorizontalBias(), but only applied to the first chain.
        /// </summary>
        /// <param name="bias"></param>
        public virtual void SetFirstHorizontalBias(float value)
        {
            mFlow.FirstHorizontalBias = value;
            requestLayout();
        }

        /// <summary>
        /// Similar to setVerticalBias(), but only applied to the first chain.
        /// </summary>
        /// <param name="bias"></param>
        public virtual void SetFirstVerticalBias(float value)
        {
            mFlow.FirstVerticalBias = value;
            requestLayout();
        }

        /// <summary>
        /// Set up the horizontal alignment of the elements in the layout, if the layout orientation
        /// is set to Flow.VERTICAL
        ///
        /// Can be either: Flow.HORIZONTAL_ALIGN_START Flow.HORIZONTAL_ALIGN_END
        /// Flow.HORIZONTAL_ALIGN_CENTER
        /// </summary>
        /// <param name="align"></param>
        public virtual void SetHorizontalAlign(int value)
        {
            mFlow.HorizontalAlign = value;
            requestLayout();
        }

        /// <summary>
        /// Set up the vertical alignment of the elements in the layout, if the layout orientation
        /// is set to Flow.HORIZONTAL
        ///
        /// Can be either: Flow.VERTICAL_ALIGN_TOP Flow.VERTICAL_ALIGN_BOTTOM
        /// Flow.VERTICAL_ALIGN_CENTER Flow.VERTICAL_ALIGN_BASELINE
        /// </summary>
        /// <param name="align"></param>
        public virtual void SetVerticalAlign(int value)
        {
            mFlow.VerticalAlign = value;
            requestLayout();
        }

        /// <summary>
        /// Set up the horizontal gap between elements
        /// </summary>
        /// <param name="gap"></param>
        public virtual void SetHorizontalGap(int value)
        {
            mFlow.HorizontalGap = value;
            requestLayout();
        }

        /// <summary>
        /// Set up the vertical gap between elements
        /// </summary>
        /// <param name="gap"></param>
        public virtual void SetVerticalGap(int value)
        {
            mFlow.VerticalGap = value;
            requestLayout();
        }

        /// <summary>
        /// Set up the maximum number of elements before wrapping.
        /// </summary>
        /// <param name="max"></param>
        public virtual void SetMaxElementsWrap(int value)
        {
            mFlow.MaxElementsWrap = value;
            requestLayout();
        }
    }
}