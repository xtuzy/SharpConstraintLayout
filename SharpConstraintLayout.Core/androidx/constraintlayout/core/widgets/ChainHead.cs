using System.Collections.Generic;

/*
 * Copyright (C) 2018 The Android Open Source Project
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
	using DimensionBehaviour = androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_PERCENT;
using static androidx.constraintlayout.core.widgets.ConstraintWidget;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_RATIO;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_SPREAD;

	/// <summary>
	/// Class to represent a chain by its main elements.
	/// </summary>
	public class ChainHead
	{

		protected internal ConstraintWidget mFirst;
		protected internal ConstraintWidget mFirstVisibleWidget;
		protected internal ConstraintWidget mLast;
		protected internal ConstraintWidget mLastVisibleWidget;
		protected internal ConstraintWidget mHead;
		protected internal ConstraintWidget mFirstMatchConstraintWidget;
		protected internal ConstraintWidget mLastMatchConstraintWidget;
		protected internal List<ConstraintWidget> mWeightedMatchConstraintsWidgets;
		protected internal int mWidgetsCount;
		protected internal int mWidgetsMatchCount;
		protected internal float mTotalWeight = 0f;
		internal int mVisibleWidgets;
		internal int mTotalSize;
		internal int mTotalMargins;
		internal bool mOptimizable;
		private int mOrientation;
		private bool mIsRtl = false;
		protected internal bool mHasUndefinedWeights;
		protected internal bool mHasDefinedWeights;
		protected internal bool mHasComplexMatchWeights;
		protected internal bool mHasRatio;
		private bool mDefined;

		/// <summary>
		/// Initialize variables, then determine visible widgets, the head of chain and
		/// matched constraint widgets.
		/// </summary>
		/// <param name="first">       first widget in a chain </param>
		/// <param name="orientation"> orientation of the chain (either Horizontal or Vertical) </param>
		/// <param name="isRtl">       Right-to-left layout flag to determine the actual head of the chain </param>
		public ChainHead(ConstraintWidget first, int orientation, bool isRtl)
		{
			mFirst = first;
			mOrientation = orientation;
			mIsRtl = isRtl;
		}

		/// <summary>
		/// Returns true if the widget should be part of the match equality rules in the chain
		/// </summary>
		/// <param name="widget">      the widget to test </param>
		/// <param name="orientation"> current orientation, HORIZONTAL or VERTICAL
		/// @return </param>
		private static bool isMatchConstraintEqualityCandidate(ConstraintWidget widget, int orientation)
		{
			return widget.Visibility != ConstraintWidget.GONE && widget.mListDimensionBehaviors[orientation] == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && (widget.mResolvedMatchConstraintDefault[orientation] == MATCH_CONSTRAINT_SPREAD || widget.mResolvedMatchConstraintDefault[orientation] == MATCH_CONSTRAINT_RATIO);
		}

		private void defineChainProperties()
		{
			int offset = mOrientation * 2;
			ConstraintWidget lastVisited = mFirst;
			mOptimizable = true;

			// TraverseChain
			ConstraintWidget widget = mFirst;
			ConstraintWidget next = mFirst;
			bool done = false;
			while (!done)
			{
				mWidgetsCount++;
				widget.mNextChainWidget[mOrientation] = null;
				widget.mListNextMatchConstraintsWidget[mOrientation] = null;
				if (widget.Visibility != ConstraintWidget.GONE)
				{
					mVisibleWidgets++;
					if (widget.getDimensionBehaviour(mOrientation) != DimensionBehaviour.MATCH_CONSTRAINT)
					{
						mTotalSize += widget.getLength(mOrientation);
					}
					mTotalSize += widget.mListAnchors[offset].Margin;
					mTotalSize += widget.mListAnchors[offset + 1].Margin;
					mTotalMargins += widget.mListAnchors[offset].Margin;
					mTotalMargins += widget.mListAnchors[offset + 1].Margin;
					// Visible widgets linked list.
					if (mFirstVisibleWidget == null)
					{
						mFirstVisibleWidget = widget;
					}
					mLastVisibleWidget = widget;

					// Match constraint linked list.
					if (widget.mListDimensionBehaviors[mOrientation] == DimensionBehaviour.MATCH_CONSTRAINT)
					{
						if (widget.mResolvedMatchConstraintDefault[mOrientation] == MATCH_CONSTRAINT_SPREAD || widget.mResolvedMatchConstraintDefault[mOrientation] == MATCH_CONSTRAINT_RATIO || widget.mResolvedMatchConstraintDefault[mOrientation] == MATCH_CONSTRAINT_PERCENT)
						{
							mWidgetsMatchCount++; // Note: Might cause an issue if we support MATCH_CONSTRAINT_RATIO_RESOLVED in chain optimization. (we currently don't)
							float weight = widget.mWeight[mOrientation];
							if (weight > 0)
							{
								mTotalWeight += widget.mWeight[mOrientation];
							}

							if (isMatchConstraintEqualityCandidate(widget, mOrientation))
							{
								if (weight < 0)
								{
									mHasUndefinedWeights = true;
								}
								else
								{
									mHasDefinedWeights = true;
								}
								if (mWeightedMatchConstraintsWidgets == null)
								{
									mWeightedMatchConstraintsWidgets = new List<ConstraintWidget>();
								}
								mWeightedMatchConstraintsWidgets.Add(widget);
							}

							if (mFirstMatchConstraintWidget == null)
							{
								mFirstMatchConstraintWidget = widget;
							}
							if (mLastMatchConstraintWidget != null)
							{
								mLastMatchConstraintWidget.mListNextMatchConstraintsWidget[mOrientation] = widget;
							}
							mLastMatchConstraintWidget = widget;
						}
						if (mOrientation == ConstraintWidget.HORIZONTAL)
						{
							if (widget.mMatchConstraintDefaultWidth != ConstraintWidget.MATCH_CONSTRAINT_SPREAD)
							{
								mOptimizable = false;
							}
							else if (widget.mMatchConstraintMinWidth != 0 || widget.mMatchConstraintMaxWidth != 0)
							{
								mOptimizable = false;
							}
						}
						else
						{
							if (widget.mMatchConstraintDefaultHeight != ConstraintWidget.MATCH_CONSTRAINT_SPREAD)
							{
								mOptimizable = false;
							}
							else if (widget.mMatchConstraintMinHeight != 0 || widget.mMatchConstraintMaxHeight != 0)
							{
								mOptimizable = false;
							}
						}
						if (widget.mDimensionRatio != 0.0f)
						{
							//TODO: Improve (Could use ratio optimization).
							mOptimizable = false;
							mHasRatio = true;
						}
					}
				}
				if (lastVisited != widget)
				{
					lastVisited.mNextChainWidget[mOrientation] = widget;
				}
				lastVisited = widget;

				// go to the next widget
				ConstraintAnchor nextAnchor = widget.mListAnchors[offset + 1].mTarget;
				if (nextAnchor != null)
				{
					next = nextAnchor.mOwner;
					if (next.mListAnchors[offset].mTarget == null || next.mListAnchors[offset].mTarget.mOwner != widget)
					{
						next = null;
					}
				}
				else
				{
					next = null;
				}
				if (next != null)
				{
					widget = next;
				}
				else
				{
					done = true;
				}
			}
			if (mFirstVisibleWidget != null)
			{
				mTotalSize -= mFirstVisibleWidget.mListAnchors[offset].Margin;
			}
			if (mLastVisibleWidget != null)
			{
				mTotalSize -= mLastVisibleWidget.mListAnchors[offset + 1].Margin;
			}
			mLast = widget;

			if (mOrientation == ConstraintWidget.HORIZONTAL && mIsRtl)
			{
				mHead = mLast;
			}
			else
			{
				mHead = mFirst;
			}

			mHasComplexMatchWeights = mHasDefinedWeights && mHasUndefinedWeights;
		}

		public virtual ConstraintWidget First
		{
			get
			{
				return mFirst;
			}
		}

		public virtual ConstraintWidget FirstVisibleWidget
		{
			get
			{
				return mFirstVisibleWidget;
			}
		}

		public virtual ConstraintWidget Last
		{
			get
			{
				return mLast;
			}
		}

		public virtual ConstraintWidget LastVisibleWidget
		{
			get
			{
				return mLastVisibleWidget;
			}
		}

		public virtual ConstraintWidget Head
		{
			get
			{
				return mHead;
			}
		}

		public virtual ConstraintWidget FirstMatchConstraintWidget
		{
			get
			{
				return mFirstMatchConstraintWidget;
			}
		}

		public virtual ConstraintWidget LastMatchConstraintWidget
		{
			get
			{
				return mLastMatchConstraintWidget;
			}
		}

		public virtual float TotalWeight
		{
			get
			{
				return mTotalWeight;
			}
		}

		public virtual void define()
		{
			if (!mDefined)
			{
				defineChainProperties();
			}
			mDefined = true;
		}
	}

}