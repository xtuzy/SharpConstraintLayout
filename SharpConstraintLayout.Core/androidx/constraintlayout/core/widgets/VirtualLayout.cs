using System.Collections.Generic;

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
namespace androidx.constraintlayout.core.widgets
{
	using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;

	/// <summary>
	/// @suppress
	/// 
	/// Base class for Virtual layouts
	/// 
	/// </summary>
	public class VirtualLayout : HelperWidget
	{

		private int mPaddingTop = 0;
		private int mPaddingBottom = 0;
		private int mPaddingLeft = 0;
		private int mPaddingRight = 0;
		private int mPaddingStart = 0;
		private int mPaddingEnd = 0;
		private int mResolvedPaddingLeft = 0;
		private int mResolvedPaddingRight = 0;

		private bool mNeedsCallFromSolver = false;
		private int mMeasuredWidth = 0;
		private int mMeasuredHeight = 0;

		protected internal BasicMeasure.Measure mMeasure = new BasicMeasure.Measure();

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Accessors
		/////////////////////////////////////////////////////////////////////////////////////////////

		public virtual int Padding
		{
			set
			{
				mPaddingLeft = value;
				mPaddingTop = value;
				mPaddingRight = value;
				mPaddingBottom = value;
				mPaddingStart = value;
				mPaddingEnd = value;
			}
		}

		public virtual int PaddingStart
		{
			set
			{
				mPaddingStart = value;
				mResolvedPaddingLeft = value;
				mResolvedPaddingRight = value;
			}
		}

		public virtual int PaddingEnd
		{
			set
			{
				mPaddingEnd = value;
			}
		}

		public virtual int PaddingLeft
		{
			set
			{
				mPaddingLeft = value;
				mResolvedPaddingLeft = value;
			}
			get
			{
				return mResolvedPaddingLeft;
			}
		}

		public virtual void applyRtl(bool isRtl)
		{
			if (mPaddingStart > 0 || mPaddingEnd > 0)
			{
				if (isRtl)
				{
					mResolvedPaddingLeft = mPaddingEnd;
					mResolvedPaddingRight = mPaddingStart;
				}
				else
				{
					mResolvedPaddingLeft = mPaddingStart;
					mResolvedPaddingRight = mPaddingEnd;
				}
			}
		}

		public virtual int PaddingTop
		{
			set
			{
				mPaddingTop = value;
			}
			get
			{
				return mPaddingTop;
			}
		}

		public virtual int PaddingRight
		{
			set
			{
				mPaddingRight = value;
				mResolvedPaddingRight = value;
			}
			get
			{
				return mResolvedPaddingRight;
			}
		}

		public virtual int PaddingBottom
		{
			set
			{
				mPaddingBottom = value;
			}
			get
			{
				return mPaddingBottom;
			}
		}





		/////////////////////////////////////////////////////////////////////////////////////////////
		// Solver callback
		/////////////////////////////////////////////////////////////////////////////////////////////

		protected internal virtual void needsCallbackFromSolver(bool value)
		{
			mNeedsCallFromSolver = value;
		}

		public virtual bool needSolverPass()
		{
			return mNeedsCallFromSolver;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Measure
		/////////////////////////////////////////////////////////////////////////////////////////////

		public virtual void measure(int widthMode, int widthSize, int heightMode, int heightSize)
		{
			// nothing
		}

		public override void updateConstraints(ConstraintWidgetContainer container)
		{
			captureWidgets();
		}

		public virtual void captureWidgets()
		{
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (widget != null)
				{
					widget.InVirtualLayout = true;
				}
			}
		}

		public virtual int MeasuredWidth
		{
			get
			{
				return mMeasuredWidth;
			}
		}

		public virtual int MeasuredHeight
		{
			get
			{
				return mMeasuredHeight;
			}
		}

		public virtual void setMeasure(int width, int height)
		{
			mMeasuredWidth = width;
			mMeasuredHeight = height;
		}

		protected internal virtual bool measureChildren()
		{
			BasicMeasure.Measurer measurer = null;
			if (mParent != null)
			{
				measurer = ((ConstraintWidgetContainer) mParent).Measurer;
			}
			if (measurer == null)
			{
				return false;
			}

			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (widget == null)
				{
					continue;
				}

				if (widget is Guideline)
				{
					continue;
				}

				//TODO:验证是否为null
				if (widget.getDimensionBehaviour(HORIZONTAL) == null || widget.getDimensionBehaviour(VERTICAL)==null)
					throw new AssertionError("Dimension is null");
				DimensionBehaviour widthBehavior = widget.getDimensionBehaviour(HORIZONTAL).Value ;
				DimensionBehaviour heightBehavior = widget.getDimensionBehaviour(VERTICAL).Value;

				bool skip = widthBehavior == DimensionBehaviour.MATCH_CONSTRAINT && widget.mMatchConstraintDefaultWidth != MATCH_CONSTRAINT_WRAP && heightBehavior == DimensionBehaviour.MATCH_CONSTRAINT && widget.mMatchConstraintDefaultHeight != MATCH_CONSTRAINT_WRAP;

				if (skip)
				{
					// we don't need to measure here as the dimension of the widget
					// will be completely computed by the solver.
					continue;
				}

				if (widthBehavior == DimensionBehaviour.MATCH_CONSTRAINT)
				{
					widthBehavior = DimensionBehaviour.WRAP_CONTENT;
				}
				if (heightBehavior == DimensionBehaviour.MATCH_CONSTRAINT)
				{
					heightBehavior = DimensionBehaviour.WRAP_CONTENT;
				}
				mMeasure.horizontalBehavior = widthBehavior;
				mMeasure.verticalBehavior = heightBehavior;
				mMeasure.horizontalDimension = widget.Width;
				mMeasure.verticalDimension = widget.Height;
				measurer.measure(widget, mMeasure);
				widget.Width = mMeasure.measuredWidth;
				widget.Height = mMeasure.measuredHeight;
				widget.BaselineDistance = mMeasure.measuredBaseline;
			}
			return true;
		}

		internal BasicMeasure.Measurer mMeasurer = null;

		protected internal virtual void measure(ConstraintWidget widget, ConstraintWidget.DimensionBehaviour horizontalBehavior, int horizontalDimension, ConstraintWidget.DimensionBehaviour verticalBehavior, int verticalDimension)
		{
			while (mMeasurer == null && Parent != null)
			{
				ConstraintWidgetContainer parent = (ConstraintWidgetContainer) Parent;
				mMeasurer = parent.Measurer;
			}
			mMeasure.horizontalBehavior = horizontalBehavior;
			mMeasure.verticalBehavior = verticalBehavior;
			mMeasure.horizontalDimension = horizontalDimension;
			mMeasure.verticalDimension = verticalDimension;
			mMeasurer.measure(widget, mMeasure);
			widget.Width = mMeasure.measuredWidth;
			widget.Height = mMeasure.measuredHeight;
			widget.HasBaseline = mMeasure.measuredHasBaseline;
			widget.BaselineDistance = mMeasure.measuredBaseline;
		}

		public virtual bool contains(HashSet<ConstraintWidget> widgets)
		{
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (widgets.Contains(widget))
				{
					return true;
				}
			}
			return false;
		}
	}

}