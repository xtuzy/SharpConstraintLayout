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
    using AndroidMeasureSpec = SharpConstraintLayout.Maui.Widget.MeasureSpec;
    using SharpConstraintLayout.Maui.Helper.Widget.Interface;

    /// <summary>
    /// Flow VirtualLayout. <b>Added in 2.0</b>
    /// <br/>
    /// Allows positioning of referenced widgets horizontally or vertically, similar to a Chain.
    /// <br/>
    /// The elements referenced are indicated via constraint_referenced_ids, as with other
    /// ConstraintHelper implementations.
    /// <br/>
    /// Those referenced widgets are then laid out by the Flow virtual layout in three possible
    /// ways: 
    /// <br/>
    /// <see cref="WrapNone"/> : simply create a chain out of the referenced elements<br/>
    /// <see cref="WrapChain"/> : create multiple chains (one after the other) if the referenced elements do not fit<br/>
    /// <see cref="WrapAligned"/> : similar to wrap chain, but will align the elements by creating rows and columns<br/>
    ///
    /// As VirtualLayouts are ConstraintHelpers, they are normal views; you can thus treat them as
    /// such, and setting up constraints on them (position, dimension) or some view attributes
    /// (background, padding) will work. The main difference between VirtualLayouts and ViewGroups
    /// is that: <br/>
    /// VirtualLayout keep the hierarchy flat<br/>
    /// Other views can thus reference / constrain to not only the VirtualLayout, but also the views laid out by the VirtualLayout
    /// VirtualLayout allow on the fly behavior modifications (e.g. for Flow, changing the orientation)
    /// <br/>
    /// <br/>
    /// <see cref="SetWrapMode(int)"/>(<see cref="WrapNone"/>)
    /// <br/>
    /// This will simply create an horizontal or vertical chain out of the referenced widgets. This
    /// is the default behavior of Flow.
    /// <br/>
    /// XML attributes that are allowed in this mode:
    /// <br/>
    /// <ul>
    /// <li>flow_horizontalStyle = "spread|spread_inside|packed"</li><br/>
    /// <li>flow_verticalStyle = "spread|spread_inside|packed"</li><br/>
    /// <li>flow_horizontalBias = "<i>float</i>"</li><br/>
    /// <li>flow_verticalBias = " <i>float</i>"</li><br/>
    /// <li>flow_horizontalGap = "<i>dimension</i>"</li><br/>
    /// <li>flow_verticalGap = "<i>dimension</i>"</li><br/>
    /// <li>flow_horizontalAlign = "start|end"</li><br/>
    /// <li>flow_verticalAlign = "top|bottom|center|baseline</li><br/>
    /// </ul>
    /// While the elements are laid out as a chain in the orientation defined, the way they are laid
    /// out in the other dimension is controlled by <see cref="SetHorizontalAlign(int)"/> and
    /// <see cref="SetVerticalAlign(int)"/>.
    /// <br/>
    /// <br/>
    /// <see cref="SetWrapMode(int)"/>(<see cref="WrapChain"/>)
    /// <br/>
    /// Similar to wrap none in terms of creating chains, but if the referenced widgets do not fit
    /// the horizontal or vertical dimension (depending on the orientation picked), they will wrap
    /// around to the next line / column.
    /// <br/>
    /// XML attributes are the same as in wrap_none, with the addition of attributes specifying
    /// chain style and chain bias applied to the first chain. This way, it is possible to specify
    /// different chain behavior between the first chain and the rest of the chains eventually
    /// created.
    /// <br/>
    /// <ul>
    /// <li>flow_firstHorizontalStyle = "spread|spread_inside|packed"</li><br/>
    /// <li>flow_firstVerticalStyle = "spread|spread_inside|packed"</li><br/>
    /// <li>flow_firstHorizontalBias = "<i>float</i>"</li><br/>
    /// <li>flow_firstVerticalBias = " <i>float</i>"</li><br/>
    /// </ul>
    /// One last important attribute is <see cref="SetMaxElementsWrap(int)"/>, which specify the number of
    /// elements before wrapping, regardless if they fit or not in the available space.
    /// <br/>
    /// <br/>
    /// <see cref="SetWrapMode(int)"/>(<see cref="WrapAligned"/>)
    /// <br/>
    /// Same XML attributes as for WRAP_CHAIN, with the difference that the elements are going to be
    /// laid out in a set of rows and columns instead of chains. The attribute specifying chains
    /// style and bias are thus not going to be applied.
    /// <br/>
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
#if __ANDROID__ && !__MAUI__
        public Flow(Android.Content.Context context) : base(context)
#else
        public Flow() : base()
#endif
        {
        }

        public override void ResolveRtl(ConstraintWidget widget, bool isRtl)
        {
            mFlow.applyRtl(isRtl);
        }

#if __ANDROID__ && !__MAUI__
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            onMeasure(mFlow, widthMeasureSpec, heightMeasureSpec);
        }
#endif

        public override void onMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec)
        {
            int widthMode = AndroidMeasureSpec.GetMode(widthMeasureSpec);
            int widthSize = AndroidMeasureSpec.GetSize(widthMeasureSpec);
            int heightMode = AndroidMeasureSpec.GetMode(heightMeasureSpec);
            int heightSize = AndroidMeasureSpec.GetSize(heightMeasureSpec);

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
#if __ANDROID__ && !__MAUI__
                SetMeasuredDimension(layout.MeasuredWidth, layout.MeasuredHeight);//Android中这个的作用应该是设置flow的大小
#elif __IOS__ && !__MAUI__
                this.Bounds = new CoreGraphics.CGRect(this.Bounds.X, this.Bounds.Y, layout.MeasuredWidth, layout.MeasuredHeight);
#endif
            }
            else
            {
#if __ANDROID__ && !__MAUI__
                SetMeasuredDimension(0, 0);
#endif
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

        protected override void init()
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
            this.RequestReLayout();
        }

        /// <summary>
        /// Set padding around the content
        /// </summary>
        /// <param name="padding"></param>
        public virtual void SetPadding(int value)
        {
            mFlow.Padding = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Set padding left around the content
        /// </summary>
        /// <param name="paddingLeft"></param>
        public virtual void SetPaddingLeft(int value)
        {
            mFlow.PaddingLeft = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Set padding top around the content
        /// </summary>
        /// <param name="paddingTop"></param>
        public virtual void SetPaddingTop(int value)
        {
            mFlow.PaddingTop = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Set padding right around the content
        /// </summary>
        /// <param name="paddingRight"></param>
        public virtual void SetPaddingRight(int value)
        {
            mFlow.PaddingRight = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Set padding bottom around the content
        /// </summary>
        /// <param name="paddingBottom"></param>
        public virtual void SetPaddingBottom(int value)
        {
            mFlow.PaddingBottom = value;
            this.RequestReLayout();
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
            this.RequestReLayout();
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
            this.RequestReLayout();
        }

        /// <summary>
        /// Set the bias of the last Horizontal column.
        /// </summary>
        /// <param name="bias"></param>
        public virtual void SetLastHorizontalBias(float value)
        {
            mFlow.LastHorizontalBias = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Set the bias of the last vertical row.
        /// </summary>
        /// <param name="bias"></param>
        public virtual void SetLastVerticalBias(float value)
        {
            mFlow.LastVerticalBias = value;
            this.RequestReLayout();
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
            this.RequestReLayout();
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
            this.RequestReLayout();
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
            this.RequestReLayout();
        }

        /// <summary>
        /// Set the horizontal bias applied to the chain
        /// </summary>
        /// <param name="bias">from 0 to 1</param>
        public virtual void SetHorizontalBias(float bias)
        {
            mFlow.HorizontalBias = bias;
            this.RequestReLayout();
        }

        /// <summary>
        /// Set the vertical bias applied to the chain
        /// </summary>
        /// <param name="bias">from 0 to 1</param>
        public virtual void SetVerticalBias(float bias)
        {
            mFlow.VerticalBias = bias;
            this.RequestReLayout();
        }

        /// <summary>
        /// Similar to setHorizontalStyle(), but only applies to the first chain.
        /// </summary>
        /// <param name="style"></param>
        public virtual void SetFirstHorizontalStyle(int value)
        {
            mFlow.FirstHorizontalStyle = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Similar to setVerticalStyle(), but only applies to the first chain.
        /// </summary>
        /// <param name="style"></param>
        public virtual void SetFirstVerticalStyle(int value)
        {
            mFlow.FirstVerticalStyle = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Similar to setHorizontalBias(), but only applied to the first chain.
        /// </summary>
        /// <param name="bias"></param>
        public virtual void SetFirstHorizontalBias(float value)
        {
            mFlow.FirstHorizontalBias = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Similar to setVerticalBias(), but only applied to the first chain.
        /// </summary>
        /// <param name="bias"></param>
        public virtual void SetFirstVerticalBias(float value)
        {
            mFlow.FirstVerticalBias = value;
            this.RequestReLayout();
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
            this.RequestReLayout();
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
            this.RequestReLayout();
        }

        /// <summary>
        /// Set up the horizontal gap between elements
        /// </summary>
        /// <param name="gap"></param>
        public virtual void SetHorizontalGap(int value)
        {
            mFlow.HorizontalGap = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Set up the vertical gap between elements
        /// </summary>
        /// <param name="gap"></param>
        public virtual void SetVerticalGap(int value)
        {
            mFlow.VerticalGap = value;
            this.RequestReLayout();
        }

        /// <summary>
        /// Set up the maximum number of elements before wrapping.
        /// </summary>
        /// <param name="max"></param>
        public virtual void SetMaxElementsWrap(int value)
        {
            mFlow.MaxElementsWrap = value;
            this.RequestReLayout();
        }
    }
}