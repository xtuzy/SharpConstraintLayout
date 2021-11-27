using System;
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

namespace androidx.constraintlayout.core.widgets
{

	/// <summary>
	/// A Barrier takes multiple widgets
	/// </summary>
	public class Barrier : HelperWidget
	{

		public const int LEFT = 0;
		public const int RIGHT = 1;
		public const int TOP = 2;
		public const int BOTTOM = 3;
		private const bool USE_RESOLUTION = true;
		private const bool USE_RELAX_GONE = false;

		private int mBarrierType = LEFT;

		private bool mAllowsGoneWidget = true;
		private int mMargin = 0;
		internal bool resolved = false;

		public Barrier()
		{
		}
		public Barrier(string debugName)
		{
			DebugName = debugName;
		}

		public override bool allowedInBarrier()
		{
			return true;
		}

		public virtual int BarrierType
		{
			get
			{
				return mBarrierType;
			}
			set
			{
				mBarrierType = value;
			}
		}


		public virtual bool AllowsGoneWidget
		{
			set
			{
				mAllowsGoneWidget = value;
			}
			get
			{
				return mAllowsGoneWidget;
			}
		}

		/// <summary>
		/// Find if this barrier supports gone widgets.
		/// </summary>
		/// <returns> true if this barrier supports gone widgets, otherwise false
		/// </returns>
		/// @deprecated This method should be called {@code getAllowsGoneWidget} such that {@code allowsGoneWidget} 
		/// can be accessed as a property from Kotlin; {<seealso cref= https://android.github.io/kotlin-guides/interop.html#property-prefixes}.
		/// Use <seealso cref="#getAllowsGoneWidget()"/> instead. </seealso>
		[Obsolete("This method should be called {@code getAllowsGoneWidget} such that {@code allowsGoneWidget}")]
		public virtual bool allowsGoneWidget()
		{
			return mAllowsGoneWidget;
		}


		public override bool ResolvedHorizontally
		{
			get
			{
				return resolved;
			}
		}

		public override bool ResolvedVertically
		{
			get
			{
				return resolved;
			}
		}

		public override void copy(ConstraintWidget src, Dictionary<ConstraintWidget, ConstraintWidget> map)
		{
			base.copy(src, map);
			Barrier srcBarrier = (Barrier) src;
			mBarrierType = srcBarrier.mBarrierType;
			mAllowsGoneWidget = srcBarrier.mAllowsGoneWidget;
			mMargin = srcBarrier.mMargin;
		}

		public override string ToString()
		{
			string debug = "[Barrier] " + DebugName + " {";
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (i > 0)
				{
					debug += ", ";
				}
				debug += widget.DebugName;
			}
			debug += "}";
			return debug;
		}

		protected internal virtual void markWidgets()
		{
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (!mAllowsGoneWidget && !widget.allowedInBarrier())
				{
					continue;
				}
				if (mBarrierType == LEFT || mBarrierType == RIGHT)
				{
					widget.setInBarrier(HORIZONTAL, true);
				}
				else if (mBarrierType == TOP || mBarrierType == BOTTOM)
				{
					widget.setInBarrier(VERTICAL, true);
				}
			}
		}

		/// <summary>
		/// Add this widget to the solver
		/// </summary>
		/// <param name="system"> the solver we want to add the widget to </param>
		/// <param name="optimize"> true if <seealso cref="Optimizer#OPTIMIZATION_GRAPH"/> is on </param>
		public override void addToSolver(LinearSystem system, bool optimize)
		{
			if (LinearSystem.FULL_DEBUG)
			{
				Console.WriteLine("\n----------------------------------------------");
				Console.WriteLine("-- adding " + DebugName + " to the solver");
				Console.WriteLine("----------------------------------------------\n");
			}

			ConstraintAnchor position;
			mListAnchors[LEFT] = mLeft;
			mListAnchors[TOP] = mTop;
			mListAnchors[RIGHT] = mRight;
			mListAnchors[BOTTOM] = mBottom;
			for (int i = 0; i < mListAnchors.Length; i++)
			{
				mListAnchors[i].mSolverVariable = system.createObjectVariable(mListAnchors[i]);
			}
			if (mBarrierType >= 0 && mBarrierType < 4)
			{
				position = mListAnchors[mBarrierType];
			}
			else
			{
				return;
			}

			if (USE_RESOLUTION)
			{
				if (!resolved)
				{
					allSolved();
				}
				if (resolved)
				{
					resolved = false;
					if (mBarrierType == LEFT || mBarrierType == RIGHT)
					{
						system.addEquality(mLeft.mSolverVariable, mX);
						system.addEquality(mRight.mSolverVariable, mX);
					}
					else if (mBarrierType == TOP || mBarrierType == BOTTOM)
					{
						system.addEquality(mTop.mSolverVariable, mY);
						system.addEquality(mBottom.mSolverVariable, mY);
					}
					return;
				}
			}

			// We have to handle the case where some of the elements referenced in the barrier are set as
			// match_constraint; we have to take it in account to set the strength of the barrier.
			bool hasMatchConstraintWidgets = false;
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (!mAllowsGoneWidget && !widget.allowedInBarrier())
				{
					continue;
				}
				if ((mBarrierType == LEFT || mBarrierType == RIGHT) && (widget.HorizontalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT) && widget.mLeft.mTarget != null && widget.mRight.mTarget != null)
				{
					hasMatchConstraintWidgets = true;
					break;
				}
				else if ((mBarrierType == TOP || mBarrierType == BOTTOM) && (widget.VerticalDimensionBehaviour == DimensionBehaviour.MATCH_CONSTRAINT) && widget.mTop.mTarget != null && widget.mBottom.mTarget != null)
				{
					hasMatchConstraintWidgets = true;
					break;
				}
			}

			bool mHasHorizontalCenteredDependents = mLeft.hasCenteredDependents() || mRight.hasCenteredDependents();
			bool mHasVerticalCenteredDependents = mTop.hasCenteredDependents() || mBottom.hasCenteredDependents();
			bool applyEqualityOnReferences = !hasMatchConstraintWidgets && ((mBarrierType == LEFT && mHasHorizontalCenteredDependents) || (mBarrierType == TOP && mHasVerticalCenteredDependents) || (mBarrierType == RIGHT && mHasHorizontalCenteredDependents) || (mBarrierType == BOTTOM && mHasVerticalCenteredDependents));

			int equalityOnReferencesStrength = SolverVariable.STRENGTH_EQUALITY;
			if (!applyEqualityOnReferences)
			{
				equalityOnReferencesStrength = SolverVariable.STRENGTH_HIGHEST;
			}
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (!mAllowsGoneWidget && !widget.allowedInBarrier())
				{
					continue;
				}
				SolverVariable target = system.createObjectVariable(widget.mListAnchors[mBarrierType]);
				widget.mListAnchors[mBarrierType].mSolverVariable = target;
				int margin = 0;
				if (widget.mListAnchors[mBarrierType].mTarget != null && widget.mListAnchors[mBarrierType].mTarget.mOwner == this)
				{
					margin += widget.mListAnchors[mBarrierType].mMargin;
				}
				if (mBarrierType == LEFT || mBarrierType == TOP)
				{
					system.addLowerBarrier(position.mSolverVariable, target, mMargin - margin, hasMatchConstraintWidgets);
				}
				else
				{
					system.addGreaterBarrier(position.mSolverVariable, target, mMargin + margin, hasMatchConstraintWidgets);
				}
				if (USE_RELAX_GONE)
				{
					if (widget.Visibility != GONE || widget is Guideline || widget is Barrier)
					{
						system.addEquality(position.mSolverVariable, target, mMargin + margin, equalityOnReferencesStrength);
					}
				}
				else
				{
					system.addEquality(position.mSolverVariable, target, mMargin + margin, equalityOnReferencesStrength);
				}
			}

			int barrierParentStrength = SolverVariable.STRENGTH_HIGHEST;
			int barrierParentStrengthOpposite = SolverVariable.STRENGTH_NONE;

			if (mBarrierType == LEFT)
			{
				system.addEquality(mRight.mSolverVariable, mLeft.mSolverVariable, 0, SolverVariable.STRENGTH_FIXED);
				system.addEquality(mLeft.mSolverVariable, mParent.mRight.mSolverVariable, 0, barrierParentStrength);
				system.addEquality(mLeft.mSolverVariable, mParent.mLeft.mSolverVariable, 0, barrierParentStrengthOpposite);
			}
			else if (mBarrierType == RIGHT)
			{
				system.addEquality(mLeft.mSolverVariable, mRight.mSolverVariable, 0, SolverVariable.STRENGTH_FIXED);
				system.addEquality(mLeft.mSolverVariable, mParent.mLeft.mSolverVariable, 0, barrierParentStrength);
				system.addEquality(mLeft.mSolverVariable, mParent.mRight.mSolverVariable, 0, barrierParentStrengthOpposite);
			}
			else if (mBarrierType == TOP)
			{
				system.addEquality(mBottom.mSolverVariable, mTop.mSolverVariable, 0, SolverVariable.STRENGTH_FIXED);
				system.addEquality(mTop.mSolverVariable, mParent.mBottom.mSolverVariable, 0, barrierParentStrength);
				system.addEquality(mTop.mSolverVariable, mParent.mTop.mSolverVariable, 0, barrierParentStrengthOpposite);
			}
			else if (mBarrierType == BOTTOM)
			{
				system.addEquality(mTop.mSolverVariable, mBottom.mSolverVariable, 0, SolverVariable.STRENGTH_FIXED);
				system.addEquality(mTop.mSolverVariable, mParent.mTop.mSolverVariable, 0, barrierParentStrength);
				system.addEquality(mTop.mSolverVariable, mParent.mBottom.mSolverVariable, 0, barrierParentStrengthOpposite);
			}
		}

		public virtual int Margin
		{
			set
			{
				mMargin = value;
			}
			get
			{
				return mMargin;
			}
		}


		public virtual int Orientation
		{
			get
			{
				switch (mBarrierType)
				{
					case LEFT:
					case RIGHT:
						return HORIZONTAL;
					case TOP:
					case BOTTOM:
						return VERTICAL;
				}
				return UNKNOWN;
			}
		}

		public virtual bool allSolved()
		{
			if (!USE_RESOLUTION)
			{
				return false;
			}
			bool hasAllWidgetsResolved = true;
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (!mAllowsGoneWidget && !widget.allowedInBarrier())
				{
					continue;
				}
				if ((mBarrierType == LEFT || mBarrierType == RIGHT) && !widget.ResolvedHorizontally)
				{
					hasAllWidgetsResolved = false;
				}
				else if ((mBarrierType == TOP || mBarrierType == BOTTOM) && !widget.ResolvedVertically)
				{
					hasAllWidgetsResolved = false;
				}
			}

			if (hasAllWidgetsResolved && mWidgetsCount > 0)
			{
				// we're done!
				int barrierPosition = 0;
				bool initialized = false;
				for (int i = 0; i < mWidgetsCount; i++)
				{
					ConstraintWidget widget = mWidgets[i];
					if (!mAllowsGoneWidget && !widget.allowedInBarrier())
					{
						continue;
					}
					if (!initialized)
					{
						if (mBarrierType == LEFT)
						{
							barrierPosition = widget.getAnchor(ConstraintAnchor.Type.LEFT).FinalValue;
						}
						else if (mBarrierType == RIGHT)
						{
							barrierPosition = widget.getAnchor(ConstraintAnchor.Type.RIGHT).FinalValue;
						}
						else if (mBarrierType == TOP)
						{
							barrierPosition = widget.getAnchor(ConstraintAnchor.Type.TOP).FinalValue;
						}
						else if (mBarrierType == BOTTOM)
						{
							barrierPosition = widget.getAnchor(ConstraintAnchor.Type.BOTTOM).FinalValue;
						}
						initialized = true;
					}
					if (mBarrierType == LEFT)
					{
						barrierPosition = Math.Min(barrierPosition, widget.getAnchor(ConstraintAnchor.Type.LEFT).FinalValue);
					}
					else if (mBarrierType == RIGHT)
					{
						barrierPosition = Math.Max(barrierPosition, widget.getAnchor(ConstraintAnchor.Type.RIGHT).FinalValue);
					}
					else if (mBarrierType == TOP)
					{
						barrierPosition = Math.Min(barrierPosition, widget.getAnchor(ConstraintAnchor.Type.TOP).FinalValue);
					}
					else if (mBarrierType == BOTTOM)
					{
						barrierPosition = Math.Max(barrierPosition, widget.getAnchor(ConstraintAnchor.Type.BOTTOM).FinalValue);
					}
				}
				barrierPosition += mMargin;
				if (mBarrierType == LEFT || mBarrierType == RIGHT)
				{
					setFinalHorizontal(barrierPosition, barrierPosition);
				}
				else
				{
					setFinalVertical(barrierPosition, barrierPosition);
				}
				if (LinearSystem.FULL_DEBUG)
				{
					Console.WriteLine("*** BARRIER " + DebugName + " SOLVED TO " + barrierPosition + " ***");
				}
				resolved = true;
				return true;
			}
			return false;
		}
	}

}