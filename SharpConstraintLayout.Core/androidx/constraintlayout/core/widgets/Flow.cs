using System;
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


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.AT_MOST;
using static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.EXACTLY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.UNSPECIFIED;

	/// <summary>
	/// Implements the Flow virtual layout.
	/// </summary>
	public class Flow : VirtualLayout
	{

		public const int HORIZONTAL_ALIGN_START = 0;
		public const int HORIZONTAL_ALIGN_END = 1;
		public const int HORIZONTAL_ALIGN_CENTER = 2;

		public const int VERTICAL_ALIGN_TOP = 0;
		public const int VERTICAL_ALIGN_BOTTOM = 1;
		public const int VERTICAL_ALIGN_CENTER = 2;
		public const int VERTICAL_ALIGN_BASELINE = 3;

		public const int WRAP_NONE = 0;
		public const int WRAP_CHAIN = 1;
		public const int WRAP_ALIGNED = 2;

		private int mHorizontalStyle = UNKNOWN;
		private int mVerticalStyle = UNKNOWN;
		private int mFirstHorizontalStyle = UNKNOWN;
		private int mFirstVerticalStyle = UNKNOWN;
		private int mLastHorizontalStyle = UNKNOWN;
		private int mLastVerticalStyle = UNKNOWN;

		private float mHorizontalBias = 0.5f;
		private float mVerticalBias = 0.5f;
		private float mFirstHorizontalBias = 0.5f;
		private float mFirstVerticalBias = 0.5f;
		private float mLastHorizontalBias = 0.5f;
		private float mLastVerticalBias = 0.5f;

		private int mHorizontalGap = 0;
		private int mVerticalGap = 0;

		private int mHorizontalAlign = HORIZONTAL_ALIGN_CENTER;
		private int mVerticalAlign = VERTICAL_ALIGN_CENTER;
		private int mWrapMode = WRAP_NONE;

		private int mMaxElementsWrap = UNKNOWN;

		private int mOrientation = HORIZONTAL;

		private List<WidgetsList> mChainList = new List<WidgetsList>();

		// Aligned management

		private ConstraintWidget[] mAlignedBiggestElementsInRows = null;
		private ConstraintWidget[] mAlignedBiggestElementsInCols = null;
		private int[] mAlignedDimensions = null;
		private ConstraintWidget[] mDisplayedWidgets;
		private int mDisplayedWidgetsCount = 0;


		public override void copy(ConstraintWidget src, Dictionary<ConstraintWidget, ConstraintWidget> map)
		{
			base.copy(src, map);
			Flow srcFLow = (Flow) src;

			mHorizontalStyle = srcFLow.mHorizontalStyle;
			mVerticalStyle = srcFLow.mVerticalStyle;
			mFirstHorizontalStyle = srcFLow.mFirstHorizontalStyle;
			mFirstVerticalStyle = srcFLow.mFirstVerticalStyle;
			mLastHorizontalStyle = srcFLow.mLastHorizontalStyle;
			mLastVerticalStyle = srcFLow.mLastVerticalStyle;

			mHorizontalBias = srcFLow.mHorizontalBias;
			mVerticalBias = srcFLow.mVerticalBias;
			mFirstHorizontalBias = srcFLow.mFirstHorizontalBias;
			mFirstVerticalBias = srcFLow.mFirstVerticalBias;
			mLastHorizontalBias = srcFLow.mLastHorizontalBias;
			mLastVerticalBias = srcFLow.mLastVerticalBias;

			mHorizontalGap = srcFLow.mHorizontalGap;
			mVerticalGap = srcFLow.mVerticalGap;

			mHorizontalAlign = srcFLow.mHorizontalAlign;
			mVerticalAlign = srcFLow.mVerticalAlign;
			mWrapMode = srcFLow.mWrapMode;

			mMaxElementsWrap = srcFLow.mMaxElementsWrap;

			mOrientation = srcFLow.mOrientation;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Accessors
		/////////////////////////////////////////////////////////////////////////////////////////////

		public virtual int Orientation
		{
			set
			{
				mOrientation = value;
			}
		}

		public virtual int FirstHorizontalStyle
		{
			set
			{
				mFirstHorizontalStyle = value;
			}
		}

		public virtual int FirstVerticalStyle
		{
			set
			{
				mFirstVerticalStyle = value;
			}
		}

		public virtual int LastHorizontalStyle
		{
			set
			{
				mLastHorizontalStyle = value;
			}
		}

		public virtual int LastVerticalStyle
		{
			set
			{
				mLastVerticalStyle = value;
			}
		}

		public virtual int HorizontalStyle
		{
			set
			{
				mHorizontalStyle = value;
			}
		}

		public virtual int VerticalStyle
		{
			set
			{
				mVerticalStyle = value;
			}
		}

		public virtual float HorizontalBias
		{
			set
			{
				mHorizontalBias = value;
			}
		}

		public virtual float VerticalBias
		{
			set
			{
				mVerticalBias = value;
			}
		}

		public virtual float FirstHorizontalBias
		{
			set
			{
				mFirstHorizontalBias = value;
			}
		}

		public virtual float FirstVerticalBias
		{
			set
			{
				mFirstVerticalBias = value;
			}
		}

		public virtual float LastHorizontalBias
		{
			set
			{
				mLastHorizontalBias = value;
			}
		}

		public virtual float LastVerticalBias
		{
			set
			{
				mLastVerticalBias = value;
			}
		}

		public virtual int HorizontalAlign
		{
			set
			{
				mHorizontalAlign = value;
			}
		}

		public virtual int VerticalAlign
		{
			set
			{
				mVerticalAlign = value;
			}
		}

		public virtual int WrapMode
		{
			set
			{
				mWrapMode = value;
			}
		}

		public virtual int HorizontalGap
		{
			set
			{
				mHorizontalGap = value;
			}
		}

		public virtual int VerticalGap
		{
			set
			{
				mVerticalGap = value;
			}
		}

		public virtual int MaxElementsWrap
		{
			set
			{
				mMaxElementsWrap = value;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Utility methods
		/////////////////////////////////////////////////////////////////////////////////////////////

		private int getWidgetWidth(ConstraintWidget widget, int max)
		{
			if (widget == null)
			{
				return 0;
			}
			if (widget.HorizontalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT)
			{
				if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
				{
					return 0;
				}
				else if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_PERCENT)
				{
					int value = (int)(widget.mMatchConstraintPercentWidth * max);
					if (value != widget.Width)
					{
						widget.MeasureRequested = true;
						measure(widget, DimensionBehaviour.FIXED, value, widget.VerticalDimensionBehaviour, widget.Height);
					}
					return value;
				}
				else if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
				{
					return widget.Width;
				}
				else if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_RATIO)
				{
					return (int)(widget.Height * widget.mDimensionRatio + 0.5f);
				}
			}
			return widget.Width;
		}

		private int getWidgetHeight(ConstraintWidget widget, int max)
		{
			if (widget == null)
			{
				return 0;
			}
			if (widget.VerticalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT)
			{
				if (widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
				{
					return 0;
				}
				else if (widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_PERCENT)
				{
					int value = (int)(widget.mMatchConstraintPercentHeight * max);
					if (value != widget.Height)
					{
						widget.MeasureRequested = true;
						measure(widget, widget.HorizontalDimensionBehaviour, widget.Width, DimensionBehaviour.FIXED, value);
					}
					return value;
				}
				else if (widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
				{
					return widget.Height;
				}
				else if (widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_RATIO)
				{
					return (int)(widget.Width * widget.mDimensionRatio + 0.5f);
				}
			}
			return widget.Height;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Measure
		/////////////////////////////////////////////////////////////////////////////////////////////

		public override void measure(int widthMode, int widthSize, int heightMode, int heightSize)
		{
			if (mWidgetsCount > 0 && !measureChildren())
			{
				setMeasure(0, 0);
				needsCallbackFromSolver(false);
				return;
			}

			int width = 0;
			int height = 0;
			int paddingLeft = PaddingLeft;
			int paddingRight = PaddingRight;
			int paddingTop = PaddingTop;
			int paddingBottom = PaddingBottom;

			int[] measured = new int[2];
			int max = widthSize - paddingLeft - paddingRight;
			if (mOrientation == VERTICAL)
			{
				max = heightSize - paddingTop - paddingBottom;
			}

			if (mOrientation == HORIZONTAL)
			{
				if (mHorizontalStyle == UNKNOWN)
				{
					mHorizontalStyle = CHAIN_SPREAD;
				}
				if (mVerticalStyle == UNKNOWN)
				{
					mVerticalStyle = CHAIN_SPREAD;
				}
			}
			else
			{
				if (mHorizontalStyle == UNKNOWN)
				{
					mHorizontalStyle = CHAIN_SPREAD;
				}
				if (mVerticalStyle == UNKNOWN)
				{
					mVerticalStyle = CHAIN_SPREAD;
				}
			}

			ConstraintWidget[] widgets = mWidgets;

			int gone = 0;
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (widget.Visibility == GONE)
				{
					gone++;
				}
			}
			int count = mWidgetsCount;
			if (gone > 0)
			{
				widgets = new ConstraintWidget[mWidgetsCount - gone];
				int j = 0;
				for (int i = 0; i < mWidgetsCount; i++)
				{
					ConstraintWidget widget = mWidgets[i];
					if (widget.Visibility != GONE)
					{
						widgets[j] = widget;
						j++;
					}
				}
				count = j;
			}
			mDisplayedWidgets = widgets;
			mDisplayedWidgetsCount = count;
			switch (mWrapMode)
			{
				case WRAP_ALIGNED:
				{
					measureAligned(widgets, count, mOrientation, max, measured);
				}
				break;
				case WRAP_CHAIN:
				{
					measureChainWrap(widgets, count, mOrientation, max, measured);
				}
				break;
				case WRAP_NONE:
				{
					measureNoWrap(widgets, count, mOrientation, max, measured);
				}
				break;
			}

			width = measured[HORIZONTAL] + paddingLeft + paddingRight;
			height = measured[VERTICAL] + paddingTop + paddingBottom;

			int measuredWidth = 0;
			int measuredHeight = 0;

			if (widthMode == EXACTLY)
			{
				measuredWidth = widthSize;
			}
			else if (widthMode == AT_MOST)
			{
				measuredWidth = Math.Min(width, widthSize);
			}
			else if (widthMode == UNSPECIFIED)
			{
				measuredWidth = width;
			}

			if (heightMode == EXACTLY)
			{
				measuredHeight = heightSize;
			}
			else if (heightMode == AT_MOST)
			{
				measuredHeight = Math.Min(height, heightSize);
			}
			else if (heightMode == UNSPECIFIED)
			{
				measuredHeight = height;
			}

			setMeasure(measuredWidth, measuredHeight);
			Width = measuredWidth;
			Height = measuredHeight;
			needsCallbackFromSolver(mWidgetsCount > 0);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Utility class representing a single chain
		/////////////////////////////////////////////////////////////////////////////////////////////

		private class WidgetsList
		{
			private readonly Flow outerInstance;

			internal int mOrientation = HORIZONTAL;
			internal ConstraintWidget biggest = null;
			internal int biggestDimension = 0;
			internal ConstraintAnchor mLeft;
			internal ConstraintAnchor mTop;
			internal ConstraintAnchor mRight;
			internal ConstraintAnchor mBottom;
			internal int mPaddingLeft = 0;
			internal int mPaddingTop = 0;
			internal int mPaddingRight = 0;
			internal int mPaddingBottom = 0;
			internal int mWidth = 0;
			internal int mHeight = 0;
			internal int mStartIndex = 0;
			internal int mCount = 0;
			internal int mNbMatchConstraintsWidgets = 0;
			internal int mMax = 0;

			public WidgetsList(Flow outerInstance, int orientation, ConstraintAnchor left, ConstraintAnchor top, ConstraintAnchor right, ConstraintAnchor bottom, int max)
			{
				this.outerInstance = outerInstance;
				mOrientation = orientation;
				mLeft = left;
				mTop = top;
				mRight = right;
				mBottom = bottom;
				mPaddingLeft = outerInstance.PaddingLeft;
				mPaddingTop = outerInstance.PaddingTop;
				mPaddingRight = outerInstance.PaddingRight;
				mPaddingBottom = outerInstance.PaddingBottom;
				mMax = max;
			}

			public virtual void setup(int orientation, ConstraintAnchor left, ConstraintAnchor top, ConstraintAnchor right, ConstraintAnchor bottom, int paddingLeft, int paddingTop, int paddingRight, int paddingBottom, int max)
			{
				mOrientation = orientation;
				mLeft = left;
				mTop = top;
				mRight = right;
				mBottom = bottom;
				mPaddingLeft = paddingLeft;
				mPaddingTop = paddingTop;
				mPaddingRight = paddingRight;
				mPaddingBottom = paddingBottom;
				mMax = max;
			}

			public virtual void clear()
			{
				biggestDimension = 0;
				biggest = null;
				mWidth = 0;
				mHeight = 0;
				mStartIndex = 0;
				mCount = 0;
				mNbMatchConstraintsWidgets = 0;
			}

			public virtual int StartIndex
			{
				set
				{
					mStartIndex = value;
				}
			}

			public virtual int Width
			{
				get
				{
					if (mOrientation == HORIZONTAL)
					{
						return mWidth - outerInstance.mHorizontalGap;
					}
					return mWidth;
				}
			}

			public virtual int Height
			{
				get
				{
					if (mOrientation == VERTICAL)
					{
						return mHeight - outerInstance.mVerticalGap;
					}
					return mHeight;
				}
			}

			public virtual void add(ConstraintWidget widget)
			{
				if (mOrientation == HORIZONTAL)
				{
					int width = outerInstance.getWidgetWidth(widget, mMax);
					if (widget.HorizontalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT)
					{
						mNbMatchConstraintsWidgets++;
						width = 0;
					}
					int gap = outerInstance.mHorizontalGap;
					if (widget.Visibility == GONE)
					{
						gap = 0;
					}
					mWidth += width + gap;
					int height = outerInstance.getWidgetHeight(widget, mMax);
					if (biggest == null || biggestDimension < height)
					{
						biggest = widget;
						biggestDimension = height;
						mHeight = height;
					}
				}
				else
				{
					int width = outerInstance.getWidgetWidth(widget, mMax);
					int height = outerInstance.getWidgetHeight(widget, mMax);
					if (widget.VerticalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT)
					{
						mNbMatchConstraintsWidgets++;
						height = 0;
					}
					int gap = outerInstance.mVerticalGap;
					if (widget.Visibility == GONE)
					{
						gap = 0;
					}
					mHeight += height + gap;
					if (biggest == null || biggestDimension < width)
					{
						biggest = widget;
						biggestDimension = width;
						mWidth = width;
					}
				}
				mCount++;
			}

			public virtual void createConstraints(bool isInRtl, int chainIndex, bool isLastChain)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
				int count = mCount;
				for (int i = 0; i < count; i++)
				{
					if (mStartIndex + i >= outerInstance.mDisplayedWidgetsCount)
					{
						break;
					}
					ConstraintWidget widget = outerInstance.mDisplayedWidgets[mStartIndex + i];
					if (widget != null)
					{
						widget.resetAnchors();
					}
				}
				if (count == 0 || biggest == null)
				{
					return;
				}

				bool singleChain = isLastChain && chainIndex == 0;
				int firstVisible = -1;
				int lastVisible = -1;
				for (int i = 0; i < count; i++)
				{
					int index = i;
					if (isInRtl)
					{
						index = count - 1 - i;
					}
					if (mStartIndex + index >= outerInstance.mDisplayedWidgetsCount)
					{
						break;
					}
					ConstraintWidget widget = outerInstance.mDisplayedWidgets[mStartIndex + index];
					if (widget.Visibility == VISIBLE)
					{
						if (firstVisible == -1)
						{
							firstVisible = i;
						}
						lastVisible = i;
					}
				}

				ConstraintWidget previous = null;
				if (mOrientation == HORIZONTAL)
				{
					ConstraintWidget verticalWidget = biggest;
					verticalWidget.VerticalChainStyle = outerInstance.mVerticalStyle;
					int padding = mPaddingTop;
					if (chainIndex > 0)
					{
						padding += outerInstance.mVerticalGap;
					}
					verticalWidget.mTop.connect(mTop, padding);
					if (isLastChain)
					{
						verticalWidget.mBottom.connect(mBottom, mPaddingBottom);
					}
					if (chainIndex > 0)
					{
						ConstraintAnchor bottom = mTop.mOwner.mBottom;
						bottom.connect(verticalWidget.mTop, 0);
					}

					ConstraintWidget baselineVerticalWidget = verticalWidget;
					if (outerInstance.mVerticalAlign == VERTICAL_ALIGN_BASELINE && !verticalWidget.hasBaseline())
					{
						for (int i = 0; i < count; i++)
						{
							int index = i;
							if (isInRtl)
							{
								index = count - 1 - i;
							}
							if (mStartIndex + index >= outerInstance.mDisplayedWidgetsCount)
							{
								break;
							}
							ConstraintWidget widget = outerInstance.mDisplayedWidgets[mStartIndex + index];
							if (widget.hasBaseline())
							{
								baselineVerticalWidget = widget;
								break;
							}
						}
					}

					for (int i = 0; i < count; i++)
					{
						int index = i;
						if (isInRtl)
						{
							index = count - 1 - i;
						}
						if (mStartIndex + index >= outerInstance.mDisplayedWidgetsCount)
						{
							break;
						}
						ConstraintWidget widget = outerInstance.mDisplayedWidgets[mStartIndex + index];
						if (i == 0)
						{
							widget.connect(widget.mLeft, mLeft, mPaddingLeft);
						}

						// ChainHead is always based on index, not i.
						// E.g. RTL would have head at the right most widget.
						if (index == 0)
						{
							int style = outerInstance.mHorizontalStyle;
							float bias = isInRtl ? (1 - outerInstance.mHorizontalBias) : outerInstance.mHorizontalBias;
							if (mStartIndex == 0 && outerInstance.mFirstHorizontalStyle != UNKNOWN)
							{
								style = outerInstance.mFirstHorizontalStyle;
								bias = isInRtl ? (1 - outerInstance.mFirstHorizontalBias) : outerInstance.mFirstHorizontalBias;
							}
							else if (isLastChain && outerInstance.mLastHorizontalStyle != UNKNOWN)
							{
								style = outerInstance.mLastHorizontalStyle;
								bias = isInRtl ? (1 - outerInstance.mLastHorizontalBias) : outerInstance.mLastHorizontalBias;
							}
							widget.HorizontalChainStyle = style;
							widget.HorizontalBiasPercent = bias;
						}
						if (i == count - 1)
						{
							widget.connect(widget.mRight, mRight, mPaddingRight);
						}
						if (previous != null)
						{
							widget.mLeft.connect(previous.mRight, outerInstance.mHorizontalGap);
							if (i == firstVisible)
							{
								widget.mLeft.GoneMargin = mPaddingLeft;
							}
							previous.mRight.connect(widget.mLeft, 0);
							if (i == lastVisible + 1)
							{
								previous.mRight.GoneMargin = mPaddingRight;
							}
						}
						if (widget != verticalWidget)
						{
							if (outerInstance.mVerticalAlign == VERTICAL_ALIGN_BASELINE && baselineVerticalWidget.hasBaseline() && widget != baselineVerticalWidget && widget.hasBaseline())
							{
								widget.mBaseline.connect(baselineVerticalWidget.mBaseline, 0);
							}
							else
							{
								switch (outerInstance.mVerticalAlign)
								{
									case VERTICAL_ALIGN_TOP:
									{
										widget.mTop.connect(verticalWidget.mTop, 0);
									}
									break;
									case VERTICAL_ALIGN_BOTTOM:
									{
										widget.mBottom.connect(verticalWidget.mBottom, 0);
									}
									break;
									case VERTICAL_ALIGN_CENTER:
									default:
									{
										if (singleChain)
										{
											widget.mTop.connect(mTop, mPaddingTop);
											widget.mBottom.connect(mBottom, mPaddingBottom);
										}
										else
										{
											widget.mTop.connect(verticalWidget.mTop, 0);
											widget.mBottom.connect(verticalWidget.mBottom, 0);
										}
									}
								break;
								}
							}
						}
						previous = widget;
					}
				}
				else
				{
					ConstraintWidget horizontalWidget = biggest;
					horizontalWidget.HorizontalChainStyle = outerInstance.mHorizontalStyle;
					int padding = mPaddingLeft;
					if (chainIndex > 0)
					{
						padding += outerInstance.mHorizontalGap;
					}
					if (isInRtl)
					{
						horizontalWidget.mRight.connect(mRight, padding);
						if (isLastChain)
						{
							horizontalWidget.mLeft.connect(mLeft, mPaddingRight);
						}
						if (chainIndex > 0)
						{
							ConstraintAnchor left = mRight.mOwner.mLeft;
							left.connect(horizontalWidget.mRight, 0);
						}
					}
					else
					{
						horizontalWidget.mLeft.connect(mLeft, padding);
						if (isLastChain)
						{
							horizontalWidget.mRight.connect(mRight, mPaddingRight);
						}
						if (chainIndex > 0)
						{
							ConstraintAnchor right = mLeft.mOwner.mRight;
							right.connect(horizontalWidget.mLeft, 0);
						}
					}
					for (int i = 0; i < count; i++)
					{
						if (mStartIndex + i >= outerInstance.mDisplayedWidgetsCount)
						{
							break;
						}
						ConstraintWidget widget = outerInstance.mDisplayedWidgets[mStartIndex + i];
						if (i == 0)
						{
							widget.connect(widget.mTop, mTop, mPaddingTop);
							int style = outerInstance.mVerticalStyle;
							float bias = outerInstance.mVerticalBias;
							if (mStartIndex == 0 && outerInstance.mFirstVerticalStyle != UNKNOWN)
							{
								style = outerInstance.mFirstVerticalStyle;
								bias = outerInstance.mFirstVerticalBias;
							}
							else if (isLastChain && outerInstance.mLastVerticalStyle != UNKNOWN)
							{
								style = outerInstance.mLastVerticalStyle;
								bias = outerInstance.mLastVerticalBias;
							}
							widget.VerticalChainStyle = style;
							widget.VerticalBiasPercent = bias;
						}
						if (i == count - 1)
						{
							widget.connect(widget.mBottom, mBottom, mPaddingBottom);
						}
						if (previous != null)
						{
							widget.mTop.connect(previous.mBottom, outerInstance.mVerticalGap);
							if (i == firstVisible)
							{
								widget.mTop.GoneMargin = mPaddingTop;
							}
							previous.mBottom.connect(widget.mTop, 0);
							if (i == lastVisible + 1)
							{
								previous.mBottom.GoneMargin = mPaddingBottom;
							}
						}
						if (widget != horizontalWidget)
						{
							if (isInRtl)
							{
								switch (outerInstance.mHorizontalAlign)
								{
									case HORIZONTAL_ALIGN_START:
									{
										widget.mRight.connect(horizontalWidget.mRight, 0);
									}
									break;
									case HORIZONTAL_ALIGN_CENTER:
									{
										widget.mLeft.connect(horizontalWidget.mLeft, 0);
										widget.mRight.connect(horizontalWidget.mRight, 0);
									}
									break;
									case HORIZONTAL_ALIGN_END:
									{
										widget.mLeft.connect(horizontalWidget.mLeft, 0);
									}
									break;
								}
							}
							else
							{
								switch (outerInstance.mHorizontalAlign)
								{
									case HORIZONTAL_ALIGN_START:
									{
										widget.mLeft.connect(horizontalWidget.mLeft, 0);
									}
									break;
									case HORIZONTAL_ALIGN_CENTER:
									{
										if (singleChain)
										{
											widget.mLeft.connect(mLeft, mPaddingLeft);
											widget.mRight.connect(mRight, mPaddingRight);
										}
										else
										{
											widget.mLeft.connect(horizontalWidget.mLeft, 0);
											widget.mRight.connect(horizontalWidget.mRight, 0);
										}
									}
									break;
									case HORIZONTAL_ALIGN_END:
									{
										widget.mRight.connect(horizontalWidget.mRight, 0);
									}
									break;
								}
							}
						}
						previous = widget;
					}
				}
			}

			public virtual void measureMatchConstraints(int availableSpace)
			{
				if (mNbMatchConstraintsWidgets == 0)
				{
					return;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
				int count = mCount;
				int widgetSize = availableSpace / mNbMatchConstraintsWidgets; // that's completely incorrect and only works for spread with no weights?
				for (int i = 0; i < count; i++)
				{
					if (mStartIndex + i >= outerInstance.mDisplayedWidgetsCount)
					{
						break;
					}
					ConstraintWidget widget = outerInstance.mDisplayedWidgets[mStartIndex + i];
					if (mOrientation == HORIZONTAL)
					{
						if (widget != null && widget.HorizontalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT)
						{
							if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
							{
								outerInstance.measure(widget, DimensionBehaviour.FIXED, widgetSize, widget.VerticalDimensionBehaviour, widget.Height);
							}
						}
					}
					else
					{
						if (widget != null && widget.VerticalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT)
						{
							if (widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
							{
								outerInstance.measure(widget, widget.HorizontalDimensionBehaviour, widget.Width, DimensionBehaviour.FIXED, widgetSize);
							}
						}
					}
				}
				recomputeDimensions();
			}

			internal virtual void recomputeDimensions()
			{
				mWidth = 0;
				mHeight = 0;
				biggest = null;
				biggestDimension = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
				int count = mCount;
				for (int i = 0; i < count; i++)
				{
					if (mStartIndex + i >= outerInstance.mDisplayedWidgetsCount)
					{
						break;
					}
					ConstraintWidget widget = outerInstance.mDisplayedWidgets[mStartIndex + i];
					if (mOrientation == HORIZONTAL)
					{
						int width = widget.Width;
						int gap = outerInstance.mHorizontalGap;
						if (widget.Visibility == GONE)
						{
							gap = 0;
						}
						mWidth += width + gap;
						int height = outerInstance.getWidgetHeight(widget, mMax);
						if (biggest == null || biggestDimension < height)
						{
							biggest = widget;
							biggestDimension = height;
							mHeight = height;
						}
					}
					else
					{
						int width = outerInstance.getWidgetWidth(widget, mMax);
						int height = outerInstance.getWidgetHeight(widget, mMax);
						int gap = outerInstance.mVerticalGap;
						if (widget.Visibility == GONE)
						{
							gap = 0;
						}
						mHeight += height + gap;
						if (biggest == null || biggestDimension < width)
						{
							biggest = widget;
							biggestDimension = width;
							mWidth = width;
						}
					}
				}
			}

		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Measure Chain Wrap
		/////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Measure the virtual layout using a list of chains for the children </summary>
		///  <param name="widgets">     list of widgets </param>
		/// <param name="count"> </param>
		/// <param name="orientation"> the layout orientation (horizontal or vertical) </param>
		/// <param name="max">         the maximum available space </param>
		/// <param name="measured">    output parameters -- will contain the resulting measure </param>
		private void measureChainWrap(ConstraintWidget[] widgets, int count, int orientation, int max, int[] measured)
		{
			if (count == 0)
			{
				return;
			}

			mChainList.Clear();
			WidgetsList list = new WidgetsList(this, orientation, mLeft, mTop, mRight, mBottom, max);
			mChainList.Add(list);

			int nbMatchConstraintsWidgets = 0;

			if (orientation == HORIZONTAL)
			{
				int width = 0;
				for (int i = 0; i < count; i++)
				{
					ConstraintWidget widget = widgets[i];
					int w = getWidgetWidth(widget, max);
					if (widget.HorizontalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT)
					{
						nbMatchConstraintsWidgets++;
					}
					bool doWrap = (width == max || (width + mHorizontalGap + w) > max) && list.biggest != null;
					if (!doWrap && i > 0 && mMaxElementsWrap > 0 && (i % mMaxElementsWrap == 0))
					{
						doWrap = true;
					}
					if (doWrap)
					{
						width = w;
						list = new WidgetsList(this, orientation, mLeft, mTop, mRight, mBottom, max);
						list.StartIndex = i;
						mChainList.Add(list);
					}
					else
					{
						if (i > 0)
						{
							width += mHorizontalGap + w;
						}
						else
						{
							width = w;
						}
					}
					list.add(widget);
				}
			}
			else
			{
				int height = 0;
				for (int i = 0; i < count; i++)
				{
					ConstraintWidget widget = widgets[i];
					int h = getWidgetHeight(widget, max);
					if (widget.VerticalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT)
					{
						nbMatchConstraintsWidgets++;
					}
					bool doWrap = (height == max || (height + mVerticalGap + h) > max) && list.biggest != null;
					if (!doWrap && i > 0 && mMaxElementsWrap > 0 && (i % mMaxElementsWrap == 0))
					{
						doWrap = true;
					}
					if (doWrap)
					{
						height = h;
						list = new WidgetsList(this, orientation, mLeft, mTop, mRight, mBottom, max);
						list.StartIndex = i;
						mChainList.Add(list);
					}
					else
					{
						if (i > 0)
						{
							height += mVerticalGap + h;
						}
						else
						{
							height = h;
						}
					}
					list.add(widget);
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int listCount = mChainList.size();
			int listCount = mChainList.Count;

			ConstraintAnchor left = mLeft;
			ConstraintAnchor top = mTop;
			ConstraintAnchor right = mRight;
			ConstraintAnchor bottom = mBottom;

			int paddingLeft = PaddingLeft;
			int paddingTop = PaddingTop;
			int paddingRight = PaddingRight;
			int paddingBottom = PaddingBottom;

			int maxWidth = 0;
			int maxHeight = 0;

			bool needInternalMeasure = HorizontalDimensionBehaviour == DimensionBehaviour.WRAP_CONTENT || VerticalDimensionBehaviour == DimensionBehaviour.WRAP_CONTENT;

			if (nbMatchConstraintsWidgets > 0 && needInternalMeasure)
			{
				// we have to remeasure them.
				for (int i = 0; i < listCount; i++)
				{
					WidgetsList current = mChainList[i];
					if (orientation == HORIZONTAL)
					{
						current.measureMatchConstraints(max - current.Width);
					}
					else
					{
						current.measureMatchConstraints(max - current.Height);
					}
				}
			}

			for (int i = 0; i < listCount; i++)
			{
				WidgetsList current = mChainList[i];
				if (orientation == HORIZONTAL)
				{
					if (i < listCount - 1)
					{
						WidgetsList next = mChainList[i + 1];
						bottom = next.biggest.mTop;
						paddingBottom = 0;
					}
					else
					{
						bottom = mBottom;
						paddingBottom = PaddingBottom;
					}
					ConstraintAnchor currentBottom = current.biggest.mBottom;
					current.setup(orientation, left, top, right, bottom, paddingLeft, paddingTop, paddingRight, paddingBottom, max);
					top = currentBottom;
					paddingTop = 0;
					maxWidth = Math.Max(maxWidth, current.Width);
					maxHeight += current.Height;
					if (i > 0)
					{
						maxHeight += mVerticalGap;
					}
				}
				else
				{
					if (i < listCount - 1)
					{
						WidgetsList next = mChainList[i + 1];
						right = next.biggest.mLeft;
						paddingRight = 0;
					}
					else
					{
						right = mRight;
						paddingRight = PaddingRight;
					}
					ConstraintAnchor currentRight = current.biggest.mRight;
					current.setup(orientation, left, top, right, bottom, paddingLeft, paddingTop, paddingRight, paddingBottom, max);
					left = currentRight;
					paddingLeft = 0;
					maxWidth += current.Width;
					maxHeight = Math.Max(maxHeight, current.Height);
					if (i > 0)
					{
						maxWidth += mHorizontalGap;
					}
				}
			}
			measured[HORIZONTAL] = maxWidth;
			measured[VERTICAL] = maxHeight;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Measure No Wrap
		/////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Measure the virtual layout using a single chain for the children </summary>
		///  <param name="widgets">     list of widgets </param>
		/// <param name="count"> </param>
		/// <param name="orientation"> the layout orientation (horizontal or vertical) </param>
		/// <param name="max">         the maximum available space </param>
		/// <param name="measured">    output parameters -- will contain the resulting measure </param>
		private void measureNoWrap(ConstraintWidget[] widgets, int count, int orientation, int max, int[] measured)
		{
			if (count == 0)
			{
				return;
			}
			WidgetsList list = null;
			if (mChainList.Count == 0)
			{
				list = new WidgetsList(this, orientation, mLeft, mTop, mRight, mBottom, max);
				mChainList.Add(list);
			}
			else
			{
				list = mChainList[0];
				list.clear();
				list.setup(orientation, mLeft, mTop, mRight, mBottom, PaddingLeft, PaddingTop, PaddingRight, PaddingBottom, max);
			}

			for (int i = 0; i < count; i++)
			{
				ConstraintWidget widget = widgets[i];
				list.add(widget);
			}

			measured[HORIZONTAL] = list.Width;
			measured[VERTICAL] = list.Height;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Measure Aligned
		/////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Measure the virtual layout arranging the children in a regular grid
		/// </summary>
		/// <param name="widgets">     list of widgets </param>
		/// <param name="orientation"> the layout orientation (horizontal or vertical) </param>
		/// <param name="max">         the maximum available space </param>
		/// <param name="measured">    output parameters -- will contain the resulting measure </param>
		private void measureAligned(ConstraintWidget[] widgets, int count, int orientation, int max, int[] measured)
		{
			bool done = false;
			int rows = 0;
			int cols = 0;

			if (orientation == HORIZONTAL)
			{
				cols = mMaxElementsWrap;
				if (cols <= 0)
				{
					// let's initialize cols with an acceptable value
					int w = 0;
					cols = 0;
					for (int i = 0; i < count; i++)
					{
						if (i > 0)
						{
							w += mHorizontalGap;
						}
						ConstraintWidget widget = widgets[i];
						if (widget == null)
						{
							continue;
						}
						w += getWidgetWidth(widget, max);
						if (w > max)
						{
							break;
						}
						cols++;
					}
				}
			}
			else
			{
				rows = mMaxElementsWrap;
				if (rows <= 0)
				{
					// let's initialize rows with an acceptable value
					int h = 0;
					rows = 0;
					for (int i = 0; i < count; i++)
					{
						if (i > 0)
						{
							h += mVerticalGap;
						}
						ConstraintWidget widget = widgets[i];
						if (widget == null)
						{
							continue;
						}
						h += getWidgetHeight(widget, max);
						if (h > max)
						{
							break;
						}
						rows++;
					}
				}
			}

			if (mAlignedDimensions == null)
			{
				mAlignedDimensions = new int[2];
			}

			if ((rows == 0 && orientation == VERTICAL) || (cols == 0 && orientation == HORIZONTAL))
			{
				done = true;
			}

			while (!done)
			{
				// get a num of rows (or cols)
				// get for each row and cols the chain of biggest elements

				if (orientation == HORIZONTAL)
				{
					rows = (int)(Math.Ceiling(count / (float) cols));
				}
				else
				{
					cols = (int)(Math.Ceiling(count / (float) rows));
				}

				if (mAlignedBiggestElementsInCols == null || mAlignedBiggestElementsInCols.Length < cols)
				{
					mAlignedBiggestElementsInCols = new ConstraintWidget[cols];
				}
				else
				{
					//Arrays.fill(mAlignedBiggestElementsInCols, null);
					mAlignedBiggestElementsInCols.Fill(null);
				}
				if (mAlignedBiggestElementsInRows == null || mAlignedBiggestElementsInRows.Length < rows)
				{
					mAlignedBiggestElementsInRows = new ConstraintWidget[rows];
				}
				else
				{
					//Arrays.fill(mAlignedBiggestElementsInRows, null);
					mAlignedBiggestElementsInRows.Fill(null);
				}

				for (int i = 0; i < cols; i++)
				{
					for (int j = 0; j < rows; j++)
					{
						int index = j * cols + i;
						if (orientation == VERTICAL)
						{
							index = i * rows + j;
						}
						if (index >= widgets.Length)
						{
							continue;
						}
						ConstraintWidget widget = widgets[index];
						if (widget == null)
						{
							continue;
						}
						int tempw = getWidgetWidth(widget, max);
						if (mAlignedBiggestElementsInCols[i] == null || mAlignedBiggestElementsInCols[i].Width < tempw)
						{
							mAlignedBiggestElementsInCols[i] = widget;
						}
						int temph = getWidgetHeight(widget, max);
						if (mAlignedBiggestElementsInRows[j] == null || mAlignedBiggestElementsInRows[j].Height < temph)
						{
							mAlignedBiggestElementsInRows[j] = widget;
						}
					}
				}

				int w = 0;
				for (int i = 0; i < cols; i++)
				{
					ConstraintWidget widget = mAlignedBiggestElementsInCols[i];
					if (widget != null)
					{
						if (i > 0)
						{
							w += mHorizontalGap;
						}
						w += getWidgetWidth(widget, max);
					}
				}
				int h = 0;
				for (int j = 0; j < rows; j++)
				{
					ConstraintWidget widget = mAlignedBiggestElementsInRows[j];
					if (widget != null)
					{
						if (j > 0)
						{
							h += mVerticalGap;
						}
						h += getWidgetHeight(widget, max);
					}
				}
				measured[HORIZONTAL] = w;
				measured[VERTICAL] = h;

				if (orientation == HORIZONTAL)
				{
					if (w > max)
					{
						if (cols > 1)
						{
							cols--;
						}
						else
						{
							done = true;
						}
					}
					else
					{
						done = true;
					}
				}
				else
				{ // VERTICAL
					if (h > max)
					{
						if (rows > 1)
						{
							rows--;
						}
						else
						{
							done = true;
						}
					}
					else
					{
						done = true;
					}
				}
			}
			mAlignedDimensions[HORIZONTAL] = cols;
			mAlignedDimensions[VERTICAL] = rows;
		}

		private void createAlignedConstraints(bool isInRtl)
		{
			if (mAlignedDimensions == null || mAlignedBiggestElementsInCols == null || mAlignedBiggestElementsInRows == null)
			{
				return;
			}

			for (int i = 0; i < mDisplayedWidgetsCount; i++)
			{
				ConstraintWidget widget = mDisplayedWidgets[i];
				widget.resetAnchors();
			}

			int cols = mAlignedDimensions[HORIZONTAL];
			int rows = mAlignedDimensions[VERTICAL];

			ConstraintWidget previous = null;
			float horizontalBias = mHorizontalBias;
			for (int i = 0; i < cols; i++)
			{
				int index = i;
				if (isInRtl)
				{
					index = cols - i - 1;
					horizontalBias = 1 - mHorizontalBias;
				}
				ConstraintWidget widget = mAlignedBiggestElementsInCols[index];
				if (widget == null || widget.Visibility == GONE)
				{
					continue;
				}
				if (i == 0)
				{
					widget.connect(widget.mLeft, mLeft, PaddingLeft);
					widget.HorizontalChainStyle = mHorizontalStyle;
					widget.HorizontalBiasPercent = horizontalBias;
				}
				if (i == cols - 1)
				{
					widget.connect(widget.mRight, mRight, PaddingRight);
				}
				if (i > 0 && previous != null)
				{
					widget.connect(widget.mLeft, previous.mRight, mHorizontalGap);
					previous.connect(previous.mRight, widget.mLeft, 0);
				}
				previous = widget;
			}
			for (int j = 0; j < rows; j++)
			{
				ConstraintWidget widget = mAlignedBiggestElementsInRows[j];
				if (widget == null || widget.Visibility == GONE)
				{
					continue;
				}
				if (j == 0)
				{
					widget.connect(widget.mTop, mTop, PaddingTop);
					widget.VerticalChainStyle = mVerticalStyle;
					widget.VerticalBiasPercent = mVerticalBias;
				}
				if (j == rows - 1)
				{
					widget.connect(widget.mBottom, mBottom, PaddingBottom);
				}
				if (j > 0 && previous != null)
				{
					widget.connect(widget.mTop, previous.mBottom, mVerticalGap);
					previous.connect(previous.mBottom, widget.mTop, 0);
				}
				previous = widget;
			}

			for (int i = 0; i < cols; i++)
			{
				for (int j = 0; j < rows; j++)
				{
					int index = j * cols + i;
					if (mOrientation == VERTICAL)
					{
						index = i * rows + j;
					}
					if (index >= mDisplayedWidgets.Length)
					{
						continue;
					}
					ConstraintWidget widget = mDisplayedWidgets[index];
					if (widget == null || widget.Visibility == GONE)
					{
						continue;
					}
					ConstraintWidget biggestInCol = mAlignedBiggestElementsInCols[i];
					ConstraintWidget biggestInRow = mAlignedBiggestElementsInRows[j];
					if (widget != biggestInCol)
					{
						widget.connect(widget.mLeft, biggestInCol.mLeft, 0);
						widget.connect(widget.mRight, biggestInCol.mRight, 0);
					}
					if (widget != biggestInRow)
					{
						widget.connect(widget.mTop, biggestInRow.mTop, 0);
						widget.connect(widget.mBottom, biggestInRow.mBottom,0);
					}
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		// Add constraints to solver
		/////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Add this widget to the solver
		/// </summary>
		/// <param name="system"> the solver we want to add the widget to </param>
		/// <param name="optimize"> true if <seealso cref="Optimizer#OPTIMIZATION_GRAPH"/> is on </param>
		public override void addToSolver(LinearSystem system, bool optimize)
		{
			base.addToSolver(system, optimize);

			bool isInRtl = Parent != null && ((ConstraintWidgetContainer) Parent).Rtl;
			switch (mWrapMode)
			{
				case WRAP_CHAIN:
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChainList.size();
					int count = mChainList.Count;
					for (int i = 0; i < count; i++)
					{
						WidgetsList list = mChainList[i];
						list.createConstraints(isInRtl, i, i == count - 1);
					}
				}
				break;
				case WRAP_NONE:
				{
					if (mChainList.Count > 0)
					{
						WidgetsList list = mChainList[0];
						list.createConstraints(isInRtl, 0, true);
					}
				}
				break;
				case WRAP_ALIGNED:
				{
					createAlignedConstraints(isInRtl);
				}
			break;
			}
			needsCallbackFromSolver(false);
		}
	}

}