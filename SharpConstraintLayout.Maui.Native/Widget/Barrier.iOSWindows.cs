﻿using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2017 The Android Open Source Project
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

namespace SharpConstraintLayout.Maui.Widget
{
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using HelperWidget = androidx.constraintlayout.core.widgets.HelperWidget;

    /// <summary>
    /// <b>Added in 1.1</b>
    /// <para>
    /// A Barrier references multiple widgets as input, and creates a virtual guideline based on the most
    /// extreme widget on the specified side. For example, a left barrier will align to the left of all the referenced views.
    /// </para>
    /// <para>
    /// <h2>Example</h2>
    /// </para>
    ///     <para><div align="center" >
    ///       <img width="325px" src="resources/images/barrier-buttons.png">
    ///     </div>
    ///     Let's have two buttons, @id/button1 and @id/button2. The constraint_referenced_ids field will reference
    ///     them by simply having them as comma-separated list:
    ///     <pre>
    ///     {@code
    ///         <androidx.constraintlayout.widget.Barrier
    ///              android:id="@+id/barrier"
    ///              android:layout_width="wrap_content"
    ///              android:layout_height="wrap_content"
    ///              app:barrierDirection="start"
    ///              app:constraint_referenced_ids="button1,button2" />
    ///     }
    ///     </pre>
    /// </para>
    ///     <para>
    ///         With the barrier direction set to start, we will have the following result:
    /// </para>
    ///     <para><div align="center" >
    ///       <img width="325px" src="resources/images/barrier-start.png">
    ///     </div>
    /// </para>
    ///     <para>
    ///         Reversely, with the direction set to end, we will have:
    /// </para>
    ///     <para><div align="center" >
    ///       <img width="325px" src="resources/images/barrier-end.png">
    ///     </div>
    /// </para>
    ///     <para>
    ///         If the widgets dimensions change, the barrier will automatically move according to its direction to get
    ///         the most extreme widget:
    /// </para>
    ///     <para><div align="center" >
    ///       <img width="325px" src="resources/images/barrier-adapt.png">
    ///     </div>
    /// 
    /// </para>
    ///     <para>
    ///         Other widgets can then be constrained to the barrier itself, instead of the individual widget. This allows a layout
    ///         to automatically adapt on widget dimension changes (e.g. different languages will end up with different length for similar worlds).
    ///     </para>
    /// <h2>GONE widgets handling</h2>
    /// <para>If the barrier references GONE widgets, the default behavior is to create a barrier on the resolved position of the GONE widget.
    /// If you do not want to have the barrier take GONE widgets into account, you can change this by setting the attribute <i>barrierAllowsGoneWidgets</i>
    /// to false (default being true).</para>
    ///     </p>
    /// </p>
    /// 
    /// </summary>
    public class Barrier : ConstraintHelper, IBarrier
    {

        /// <summary>
        /// Left direction constant
        /// </summary>
        public const int LEFT = androidx.constraintlayout.core.widgets.Barrier.LEFT;

        /// <summary>
        /// Top direction constant
        /// </summary>
        public const int TOP = androidx.constraintlayout.core.widgets.Barrier.TOP;

        /// <summary>
        /// Right direction constant
        /// </summary>
        public const int RIGHT = androidx.constraintlayout.core.widgets.Barrier.RIGHT;

        /// <summary>
        /// Bottom direction constant
        /// </summary>
        public const int BOTTOM = androidx.constraintlayout.core.widgets.Barrier.BOTTOM;

        /// <summary>
        /// Start direction constant
        /// </summary>
        public static readonly int Start = BOTTOM + 2;

        /// <summary>
        /// End Barrier constant
        /// </summary>
        public static readonly int End = Start + 1;

        private int mIndicatedType;
        private int mResolvedType;
        private androidx.constraintlayout.core.widgets.Barrier mBarrier;

        public Barrier() : base()
        {
            Visible = ConstraintSet.Gone;
        }

        /// <summary>
        /// Get the barrier type ({@code Barrier.LEFT}, {@code Barrier.TOP},
        /// {@code Barrier.RIGHT}, {@code Barrier.BOTTOM}, {@code Barrier.END},
        /// {@code Barrier.START})
        /// </summary>
        public virtual int Type
        {
            get
            {
                return mIndicatedType;
            }
            set
            {
                mIndicatedType = value;
            }
        }

        private void updateType(ConstraintWidget widget, int type, bool isRtl)
        {
            mResolvedType = type;
            if (ConstraintLayout.ISSUPPORTRTLPLATFORM)
            {
                // Pre JB MR1, left/right should take precedence, unless they are
                // not defined and somehow a corresponding start/end constraint exists
                if (mIndicatedType == Start)
                {
                    mResolvedType = LEFT;
                }
                else if (mIndicatedType == End)
                {
                    mResolvedType = RIGHT;
                }
            }
            else
            {
                // Post JB MR1, if start/end are defined, they take precedence over left/right
                if (isRtl)
                {
                    if (mIndicatedType == Start)
                    {
                        mResolvedType = RIGHT;
                    }
                    else if (mIndicatedType == End)
                    {
                        mResolvedType = LEFT;
                    }
                }
                else
                {
                    if (mIndicatedType == Start)
                    {
                        mResolvedType = LEFT;
                    }
                    else if (mIndicatedType == End)
                    {
                        mResolvedType = RIGHT;
                    }
                }
            }
            if (widget is androidx.constraintlayout.core.widgets.Barrier)
            {
                androidx.constraintlayout.core.widgets.Barrier barrier = (androidx.constraintlayout.core.widgets.Barrier)widget;
                barrier.BarrierType = mResolvedType;
            }
        }

        public override void ResolveRtl(ConstraintWidget widget, bool isRtl)
        {
            updateType(widget, mIndicatedType, isRtl);
        }

        protected internal override void init()
        {
            base.init();
            mBarrier = new androidx.constraintlayout.core.widgets.Barrier();

            mHelperWidget = mBarrier;
            ValidateParams();
        }

        public virtual bool AllowsGoneWidget
        {
            set
            {
                mBarrier.AllowsGoneWidget = value;
            }
            get
            {
                return mBarrier.AllowsGoneWidget;
            }
        }

        /// <summary>
        /// Set a margin on the barrier
        /// </summary>
        /// <param name="margin"> in dp </param>
        public virtual void SetDpMargin(int margin)
        {
            //float density = Resources.DisplayMetrics.density;
            float density = (float)Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo.Density;
            int px = (int)(0.5f + margin * density);
            mBarrier.Margin = px;
        }

        /// <summary>
        /// Returns the barrier margin
        /// </summary>
        /// <returns> the barrier margin (in pixels) </returns>
        public new virtual int Margin
        {
            get
            {
                return mBarrier.Margin;
            }
            set
            {
                mBarrier.Margin = value;
            }
        }

        public override void LoadParameters(ConstraintSet.Constraint constraint, HelperWidget child, Dictionary<int, ConstraintWidget> mapIdToWidget)
        {
            base.LoadParameters(constraint, child, mapIdToWidget);
            if (child is androidx.constraintlayout.core.widgets.Barrier)
            {
                androidx.constraintlayout.core.widgets.Barrier barrier = (androidx.constraintlayout.core.widgets.Barrier)child;
                ConstraintWidgetContainer container = (ConstraintWidgetContainer)child.Parent;
                bool isRtl = container.Rtl;
                updateType(barrier, constraint.layout.mBarrierDirection, isRtl);
                barrier.AllowsGoneWidget = constraint.layout.mBarrierAllowsGoneWidgets;
                barrier.Margin = constraint.layout.mBarrierMargin;
            }
        }
    }

}