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

namespace androidx.constraintlayout.core.state
{
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
	using Facade = androidx.constraintlayout.core.state.helpers.Facade;
	using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;


	//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
	//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.HORIZONTAL;
	using static androidx.constraintlayout.core.widgets.ConstraintWidget;
	//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
	//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;


	public class ConstraintReference : Reference
	{

		private object key;

		public virtual object Key
		{
			set
			{
				this.key = value;
			}
			get
			{
				return key;
			}
		}


		public virtual string Tag
		{
			set
			{
				mTag = value;
			}
			get
			{
				return mTag;
			}
		}


		public interface ConstraintReferenceFactory
		{
			ConstraintReference create(State state);
		}

		internal readonly State mState;

		internal string mTag = null;

		internal Facade mFacade = null;

		internal int mHorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD;
		internal int mVerticalChainStyle = ConstraintWidget.CHAIN_SPREAD;

		internal float mHorizontalBias = 0.5f;
		internal float mVerticalBias = 0.5f;

		internal int mMarginLeft = 0;
		internal int mMarginRight = 0;
		protected internal int mMarginStart = 0;
		protected internal int mMarginEnd = 0;
		internal int mMarginTop = 0;
		internal int mMarginBottom = 0;

		internal int mMarginLeftGone = 0;
		internal int mMarginRightGone = 0;
		internal int mMarginStartGone = 0;
		internal int mMarginEndGone = 0;
		internal int mMarginTopGone = 0;
		internal int mMarginBottomGone = 0;

		internal float mPivotX = float.NaN;
		internal float mPivotY = float.NaN;

		internal float mRotationX = float.NaN;
		internal float mRotationY = float.NaN;
		internal float mRotationZ = float.NaN;

		internal float mTranslationX = float.NaN;
		internal float mTranslationY = float.NaN;
		internal float mTranslationZ = float.NaN;

		internal float mAlpha = float.NaN;

		internal float mScaleX = float.NaN;
		internal float mScaleY = float.NaN;

		internal int mVisibility = ConstraintWidget.VISIBLE;

		internal object mLeftToLeft = null;
		internal object mLeftToRight = null;
		internal object mRightToLeft = null;
		internal object mRightToRight = null;
		protected internal object mStartToStart = null;
		protected internal object mStartToEnd = null;
		protected internal object mEndToStart = null;
		protected internal object mEndToEnd = null;
		protected internal object mTopToTop = null;
		protected internal object mTopToBottom = null;
		protected internal object mBottomToTop = null;
		protected internal object mBottomToBottom = null;
		internal object mBaselineToBaseline = null;
		internal object mCircularConstraint = null;
		private float mCircularAngle;
		private float mCircularDistance;

		internal State.Constraint? mLast = null;

		internal Dimension mHorizontalDimension = Dimension.Fixed(Dimension.WRAP_DIMENSION);
		internal Dimension mVerticalDimension = Dimension.Fixed(Dimension.WRAP_DIMENSION);

		private object mView;
		private ConstraintWidget mConstraintWidget;

		private Dictionary<string, int?> mCustomColors = new Dictionary<string, int?>();
		private Dictionary<string, float?> mCustomFloats = new Dictionary<string, float?>();

		public virtual object View
		{
			set
			{
				mView = value;
				if (mConstraintWidget != null)
				{
					mConstraintWidget.CompanionWidget = mView;
				}
			}
			get
			{
				return mView;
			}
		}


		public virtual Facade Facade
		{
			set
			{
				mFacade = value;
				if (value != null)
				{
					ConstraintWidget = value.ConstraintWidget;
				}
			}
			get
			{
				return mFacade;
			}
		}


		public virtual ConstraintWidget ConstraintWidget
		{
			set
			{
				if (value == null)
				{
					return;
				}
				mConstraintWidget = value;
				mConstraintWidget.CompanionWidget = mView;
			}
			get
			{
				if (mConstraintWidget == null)
				{
					mConstraintWidget = createConstraintWidget();
					mConstraintWidget.CompanionWidget = mView;
				}
				return mConstraintWidget;
			}
		}


		public virtual ConstraintWidget createConstraintWidget()
		{
			return new ConstraintWidget(Width.Value, Height.Value);
		}

		internal class IncorrectConstraintException : Exception
		{

			internal readonly List<string> mErrors;

			public IncorrectConstraintException(List<string> errors)
			{
				mErrors = errors;
			}

			public virtual List<string> Errors
			{
				get
				{
					return mErrors;
				}
			}

			public override string ToString()
			{
				return "IncorrectConstraintException: " + mErrors.ToString();
			}
		}

		/// <summary>
		///  Validate the constraints
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void validate() throws IncorrectConstraintException
		public virtual void validate()
		{
			List<string> errors = new List<string>();
			if (mLeftToLeft != null && mLeftToRight != null)
			{
				errors.Add("LeftToLeft and LeftToRight both defined");
			}
			if (mRightToLeft != null && mRightToRight != null)
			{
				errors.Add("RightToLeft and RightToRight both defined");
			}
			if (mStartToStart != null && mStartToEnd != null)
			{
				errors.Add("StartToStart and StartToEnd both defined");
			}
			if (mEndToStart != null && mEndToEnd != null)
			{
				errors.Add("EndToStart and EndToEnd both defined");
			}
			if ((mLeftToLeft != null || mLeftToRight != null || mRightToLeft != null || mRightToRight != null) && (mStartToStart != null || mStartToEnd != null || mEndToStart != null || mEndToEnd != null))
			{
				errors.Add("Both left/right and start/end constraints defined");
			}
			if (errors.Count > 0)
			{
				throw new IncorrectConstraintException(errors);
			}
		}

		private object get(object reference)
		{
			if (reference == null)
			{
				return null;
			}
			if (!(reference is ConstraintReference))
			{
				return mState.reference(reference);
			}
			return reference;
		}

		public ConstraintReference(State state)
		{
			mState = state;
		}

		public virtual int HorizontalChainStyle
		{
			set
			{
				mHorizontalChainStyle = value;
			}
			get
			{
				return mHorizontalChainStyle;
			}
		}


		public virtual int VerticalChainStyle
		{
			set
			{
				mVerticalChainStyle = value;
			}
		}

		public virtual int getVerticalChainStyle(int chainStyle)
		{
			return mVerticalChainStyle;
		}

		public virtual ConstraintReference clearVertical()
		{
			top().clear();
			baseline().clear();
			bottom().clear();
			return this;
		}

		public virtual ConstraintReference clearHorizontal()
		{
			start().clear();
			end().clear();
			left().clear();
			right().clear();
			return this;
		}

		public virtual float TranslationX
		{
			get
			{
				return mTranslationX;
			}
		}
		public virtual float TranslationY
		{
			get
			{
				return mTranslationY;
			}
		}
		public virtual float TranslationZ
		{
			get
			{
				return mTranslationZ;
			}
		}
		public virtual float ScaleX
		{
			get
			{
				return mScaleX;
			}
		}
		public virtual float ScaleY
		{
			get
			{
				return mScaleY;
			}
		}
		public virtual float Alpha
		{
			get
			{
				return mAlpha;
			}
		}
		public virtual float PivotX
		{
			get
			{
				return mPivotX;
			}
		}
		public virtual float PivotY
		{
			get
			{
				return mPivotY;
			}
		}
		public virtual float RotationX
		{
			get
			{
				return mRotationX;
			}
		}
		public virtual float RotationY
		{
			get
			{
				return mRotationY;
			}
		}
		public virtual float RotationZ
		{
			get
			{
				return mRotationZ;
			}
		}

		public virtual ConstraintReference pivotX(float x)
		{
			mPivotX = x;
			return this;
		}

		public virtual ConstraintReference pivotY(float y)
		{
			mPivotY = y;
			return this;
		}

		public virtual ConstraintReference rotationX(float x)
		{
			mRotationX = x;
			return this;
		}

		public virtual ConstraintReference rotationY(float y)
		{
			mRotationY = y;
			return this;
		}

		public virtual ConstraintReference rotationZ(float z)
		{
			mRotationZ = z;
			return this;
		}

		public virtual ConstraintReference translationX(float x)
		{
			mTranslationX = x;
			return this;
		}

		public virtual ConstraintReference translationY(float y)
		{
			mTranslationY = y;
			return this;
		}

		public virtual ConstraintReference translationZ(float z)
		{
			mTranslationZ = z;
			return this;
		}

		public virtual ConstraintReference scaleX(float x)
		{
			mScaleX = x;
			return this;
		}

		public virtual ConstraintReference scaleY(float y)
		{
			mScaleY = y;
			return this;
		}

		public virtual ConstraintReference alpha(float alpha)
		{
			mAlpha = alpha;
			return this;
		}

		public virtual ConstraintReference visibility(int visibility)
		{
			mVisibility = visibility;
			return this;
		}

		public virtual ConstraintReference left()
		{
			if (mLeftToLeft != null)
			{
				mLast = State.Constraint.LEFT_TO_LEFT;
			}
			else
			{
				mLast = State.Constraint.LEFT_TO_RIGHT;
			}
			return this;
		}

		public virtual ConstraintReference right()
		{
			if (mRightToLeft != null)
			{
				mLast = State.Constraint.RIGHT_TO_LEFT;
			}
			else
			{
				mLast = State.Constraint.RIGHT_TO_RIGHT;
			}
			return this;
		}

		public virtual ConstraintReference start()
		{
			if (mStartToStart != null)
			{
				mLast = State.Constraint.START_TO_START;
			}
			else
			{
				mLast = State.Constraint.START_TO_END;
			}
			return this;
		}

		public virtual ConstraintReference end()
		{
			if (mEndToStart != null)
			{
				mLast = State.Constraint.END_TO_START;
			}
			else
			{
				mLast = State.Constraint.END_TO_END;
			}
			return this;
		}

		public virtual ConstraintReference top()
		{
			if (mTopToTop != null)
			{
				mLast = State.Constraint.TOP_TO_TOP;
			}
			else
			{
				mLast = State.Constraint.TOP_TO_BOTTOM;
			}
			return this;
		}

		public virtual ConstraintReference bottom()
		{
			if (mBottomToTop != null)
			{
				mLast = State.Constraint.BOTTOM_TO_TOP;
			}
			else
			{
				mLast = State.Constraint.BOTTOM_TO_BOTTOM;
			}
			return this;
		}

		public virtual ConstraintReference baseline()
		{
			mLast = State.Constraint.BASELINE_TO_BASELINE;
			return this;
		}

		public virtual void addCustomColor(string name, int color)
		{
			mCustomColors[name] = color;
		}

		public virtual void addCustomFloat(string name, float value)
		{
			if (mCustomFloats == null)
			{
				mCustomFloats = new Dictionary<string,float?>();
			}
			mCustomFloats[name] = value;
		}

		private void dereference()
		{
			mLeftToLeft = get(mLeftToLeft);
			mLeftToRight = get(mLeftToRight);
			mRightToLeft = get(mRightToLeft);
			mRightToRight = get(mRightToRight);
			mStartToStart = get(mStartToStart);
			mStartToEnd = get(mStartToEnd);
			mEndToStart = get(mEndToStart);
			mEndToEnd = get(mEndToEnd);
			mTopToTop = get(mTopToTop);
			mTopToBottom = get(mTopToBottom);
			mBottomToTop = get(mBottomToTop);
			mBottomToBottom = get(mBottomToBottom);
			mBaselineToBaseline = get(mBaselineToBaseline);
		}

		public virtual ConstraintReference leftToLeft(object reference)
		{
			mLast = State.Constraint.LEFT_TO_LEFT;
			mLeftToLeft = reference;
			return this;
		}

		public virtual ConstraintReference leftToRight(object reference)
		{
			mLast = State.Constraint.LEFT_TO_RIGHT;
			mLeftToRight = reference;
			return this;
		}

		public virtual ConstraintReference rightToLeft(object reference)
		{
			mLast = State.Constraint.RIGHT_TO_LEFT;
			mRightToLeft = reference;
			return this;
		}

		public virtual ConstraintReference rightToRight(object reference)
		{
			mLast = State.Constraint.RIGHT_TO_RIGHT;
			mRightToRight = reference;
			return this;
		}

		public virtual ConstraintReference startToStart(object reference)
		{
			mLast = State.Constraint.START_TO_START;
			mStartToStart = reference;
			return this;
		}

		public virtual ConstraintReference startToEnd(object reference)
		{
			mLast = State.Constraint.START_TO_END;
			mStartToEnd = reference;
			return this;
		}

		public virtual ConstraintReference endToStart(object reference)
		{
			mLast = State.Constraint.END_TO_START;
			mEndToStart = reference;
			return this;
		}

		public virtual ConstraintReference endToEnd(object reference)
		{
			mLast = State.Constraint.END_TO_END;
			mEndToEnd = reference;
			return this;
		}

		public virtual ConstraintReference topToTop(object reference)
		{
			mLast = State.Constraint.TOP_TO_TOP;
			mTopToTop = reference;
			return this;
		}

		public virtual ConstraintReference topToBottom(object reference)
		{
			mLast = State.Constraint.TOP_TO_BOTTOM;
			mTopToBottom = reference;
			return this;
		}

		public virtual ConstraintReference bottomToTop(object reference)
		{
			mLast = State.Constraint.BOTTOM_TO_TOP;
			mBottomToTop = reference;
			return this;
		}

		public virtual ConstraintReference bottomToBottom(object reference)
		{
			mLast = State.Constraint.BOTTOM_TO_BOTTOM;
			mBottomToBottom = reference;
			return this;
		}

		public virtual ConstraintReference baselineToBaseline(object reference)
		{
			mLast = State.Constraint.BASELINE_TO_BASELINE;
			mBaselineToBaseline = reference;
			return this;
		}

		public virtual ConstraintReference centerHorizontally(object reference)
		{
			object @ref = get(reference);
			mStartToStart = @ref;
			mEndToEnd = @ref;
			mLast = State.Constraint.CENTER_HORIZONTALLY;
			mHorizontalBias = 0.5f;
			return this;
		}

		public virtual ConstraintReference centerVertically(object reference)
		{
			object @ref = get(reference);
			mTopToTop = @ref;
			mBottomToBottom = @ref;
			mLast = State.Constraint.CENTER_VERTICALLY;
			mVerticalBias = 0.5f;
			return this;
		}

		public virtual ConstraintReference circularConstraint(object reference, float angle, float distance)
		{
			object @ref = get(reference);
			mCircularConstraint = @ref;
			mCircularAngle = angle;
			mCircularDistance = distance;
			mLast = State.Constraint.CIRCULAR_CONSTRAINT;
			return this;
		}

		public virtual ConstraintReference width(Dimension dimension)
		{
			return setWidth(dimension);
		}

		public virtual ConstraintReference height(Dimension dimension)
		{
			return setHeight(dimension);
		}

		public virtual Dimension Width
		{
			get
			{
				return mHorizontalDimension;
			}
		}

		public virtual ConstraintReference setWidth(Dimension dimension)
		{
			mHorizontalDimension = dimension;
			return this;
		}

		public virtual Dimension Height
		{
			get
			{
				return mVerticalDimension;
			}
		}
		public virtual ConstraintReference setHeight(Dimension dimension)
		{
			mVerticalDimension = dimension;
			return this;
		}

		public virtual ConstraintReference margin(object marginValue)
		{
			return margin(mState.convertDimension(marginValue));
		}

		public virtual ConstraintReference margin(int value)
		{
			if (mLast != null)
			{
				switch (mLast)
				{
					case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_LEFT:
					case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_RIGHT:
					{
						mMarginLeft = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_LEFT:
					case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_RIGHT:
					{
						mMarginRight = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.START_TO_START:
					case androidx.constraintlayout.core.state.State.Constraint.START_TO_END:
					{
						mMarginStart = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.END_TO_START:
					case androidx.constraintlayout.core.state.State.Constraint.END_TO_END:
					{
						mMarginEnd = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_TOP:
					case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_BOTTOM:
					{
						mMarginTop = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_TOP:
					case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_BOTTOM:
					{
						mMarginBottom = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.BASELINE_TO_BASELINE:
					{
						// nothing
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.CIRCULAR_CONSTRAINT:
					{
						mCircularDistance = value;
					}
					break;
					default:
						break;
				}
			}
			else
			{
				mMarginLeft = value;
				mMarginRight = value;
				mMarginStart = value;
				mMarginEnd = value;
				mMarginTop = value;
				mMarginBottom = value;
			}
			return this;
		}

		public virtual ConstraintReference marginGone(int value)
		{
			if (mLast != null)
			{
				switch (mLast)
				{
					case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_LEFT:
					case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_RIGHT:
					{
						mMarginLeftGone = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_LEFT:
					case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_RIGHT:
					{
						mMarginRightGone = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.START_TO_START:
					case androidx.constraintlayout.core.state.State.Constraint.START_TO_END:
					{
						mMarginStartGone = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.END_TO_START:
					case androidx.constraintlayout.core.state.State.Constraint.END_TO_END:
					{
						mMarginEndGone = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_TOP:
					case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_BOTTOM:
					{
						mMarginTopGone = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_TOP:
					case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_BOTTOM:
					{
						mMarginBottomGone = value;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.BASELINE_TO_BASELINE:
					{
						// nothing
					}
					break;
					default:
						break;
				}
			}
			else
			{
				mMarginLeftGone = value;
				mMarginRightGone = value;
				mMarginStartGone = value;
				mMarginEndGone = value;
				mMarginTopGone = value;
				mMarginBottomGone = value;
			}
			return this;
		}

		public virtual ConstraintReference horizontalBias(float value)
		{
			mHorizontalBias = value;
			return this;
		}

		public virtual ConstraintReference verticalBias(float value)
		{
			mVerticalBias = value;
			return this;
		}

		public virtual ConstraintReference bias(float value)
		{
			if (mLast == null)
			{
				return this;
			}
			switch (mLast)
			{
				case androidx.constraintlayout.core.state.State.Constraint.CENTER_HORIZONTALLY:
				case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_LEFT:
				case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_RIGHT:
				case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_LEFT:
				case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_RIGHT:
				case androidx.constraintlayout.core.state.State.Constraint.START_TO_START:
				case androidx.constraintlayout.core.state.State.Constraint.START_TO_END:
				case androidx.constraintlayout.core.state.State.Constraint.END_TO_START:
				case androidx.constraintlayout.core.state.State.Constraint.END_TO_END:
				{
					mHorizontalBias = value;
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.CENTER_VERTICALLY:
				case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_TOP:
				case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_BOTTOM:
				case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_TOP:
				case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_BOTTOM:
				{
					mVerticalBias = value;
				}
				break;
				default:
					break;
			}
			return this;
		}

		public virtual ConstraintReference clear()
		{
			if (mLast != null)
			{
				switch (mLast)
				{
					case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_LEFT:
					case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_RIGHT:
					{
						mLeftToLeft = null;
						mLeftToRight = null;
						mMarginLeft = 0;
						mMarginLeftGone = 0;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_LEFT:
					case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_RIGHT:
					{
						mRightToLeft = null;
						mRightToRight = null;
						mMarginRight = 0;
						mMarginRightGone = 0;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.START_TO_START:
					case androidx.constraintlayout.core.state.State.Constraint.START_TO_END:
					{
						mStartToStart = null;
						mStartToEnd = null;
						mMarginStart = 0;
						mMarginStartGone = 0;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.END_TO_START:
					case androidx.constraintlayout.core.state.State.Constraint.END_TO_END:
					{
						mEndToStart = null;
						mEndToEnd = null;
						mMarginEnd = 0;
						mMarginEndGone = 0;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_TOP:
					case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_BOTTOM:
					{
						mTopToTop = null;
						mTopToBottom = null;
						mMarginTop = 0;
						mMarginTopGone = 0;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_TOP:
					case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_BOTTOM:
					{
						mBottomToTop = null;
						mBottomToBottom = null;
						mMarginBottom = 0;
						mMarginBottomGone = 0;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.BASELINE_TO_BASELINE:
					{
						mBaselineToBaseline = null;
					}
					break;
					case androidx.constraintlayout.core.state.State.Constraint.CIRCULAR_CONSTRAINT:
					{
						mCircularConstraint = null;
					}
					break;
					default:
						break;
				}
			}
			else
			{
				mLeftToLeft = null;
				mLeftToRight = null;
				mMarginLeft = 0;
				mRightToLeft = null;
				mRightToRight = null;
				mMarginRight = 0;
				mStartToStart = null;
				mStartToEnd = null;
				mMarginStart = 0;
				mEndToStart = null;
				mEndToEnd = null;
				mMarginEnd = 0;
				mTopToTop = null;
				mTopToBottom = null;
				mMarginTop = 0;
				mBottomToTop = null;
				mBottomToBottom = null;
				mMarginBottom = 0;
				mBaselineToBaseline = null;
				mCircularConstraint = null;
				mHorizontalBias = 0.5f;
				mVerticalBias = 0.5f;
				mMarginLeftGone = 0;
				mMarginRightGone = 0;
				mMarginStartGone = 0;
				mMarginEndGone = 0;
				mMarginTopGone = 0;
				mMarginBottomGone = 0;
			}
			return this;
		}

		private ConstraintWidget getTarget(object target)
		{
			if (target is Reference)
			{
				Reference referenceTarget = (Reference) target;
				return referenceTarget.ConstraintWidget;
			}
			return null;
		}

		private void applyConnection(ConstraintWidget widget, object opaqueTarget, State.Constraint type)
		{
			ConstraintWidget target = getTarget(opaqueTarget);
			if (target == null)
			{
				return;
			}
			switch (type)
			{
				// TODO: apply RTL
				default:
					break;
			}
			switch (type)
			{
				case androidx.constraintlayout.core.state.State.Constraint.START_TO_START:
				{
					widget.getAnchor(ConstraintAnchor.Type.LEFT).connect(target.getAnchor(ConstraintAnchor.Type.LEFT), mMarginStart, mMarginStartGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.START_TO_END:
				{
					widget.getAnchor(ConstraintAnchor.Type.LEFT).connect(target.getAnchor(ConstraintAnchor.Type.RIGHT), mMarginStart, mMarginStartGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.END_TO_START:
				{
					widget.getAnchor(ConstraintAnchor.Type.RIGHT).connect(target.getAnchor(ConstraintAnchor.Type.LEFT), mMarginEnd, mMarginEndGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.END_TO_END:
				{
					widget.getAnchor(ConstraintAnchor.Type.RIGHT).connect(target.getAnchor(ConstraintAnchor.Type.RIGHT), mMarginEnd, mMarginEndGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_LEFT:
				{
					widget.getAnchor(ConstraintAnchor.Type.LEFT).connect(target.getAnchor(ConstraintAnchor.Type.LEFT), mMarginLeft, mMarginLeftGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.LEFT_TO_RIGHT:
				{
					widget.getAnchor(ConstraintAnchor.Type.LEFT).connect(target.getAnchor(ConstraintAnchor.Type.RIGHT), mMarginLeft, mMarginLeftGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_LEFT:
				{
					widget.getAnchor(ConstraintAnchor.Type.RIGHT).connect(target.getAnchor(ConstraintAnchor.Type.LEFT), mMarginRight, mMarginRightGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.RIGHT_TO_RIGHT:
				{
					widget.getAnchor(ConstraintAnchor.Type.RIGHT).connect(target.getAnchor(ConstraintAnchor.Type.RIGHT), mMarginRight, mMarginRightGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_TOP:
				{
					widget.getAnchor(ConstraintAnchor.Type.TOP).connect(target.getAnchor(ConstraintAnchor.Type.TOP), mMarginTop, mMarginTopGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.TOP_TO_BOTTOM:
				{
					widget.getAnchor(ConstraintAnchor.Type.TOP).connect(target.getAnchor(ConstraintAnchor.Type.BOTTOM), mMarginTop, mMarginTopGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_TOP:
				{
					widget.getAnchor(ConstraintAnchor.Type.BOTTOM).connect(target.getAnchor(ConstraintAnchor.Type.TOP), mMarginBottom, mMarginBottomGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.BOTTOM_TO_BOTTOM:
				{
					widget.getAnchor(ConstraintAnchor.Type.BOTTOM).connect(target.getAnchor(ConstraintAnchor.Type.BOTTOM), mMarginBottom, mMarginBottomGone, false);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.BASELINE_TO_BASELINE:
				{
					widget.immediateConnect(ConstraintAnchor.Type.BASELINE, target, ConstraintAnchor.Type.BASELINE, 0, 0);
				}
				break;
				case androidx.constraintlayout.core.state.State.Constraint.CIRCULAR_CONSTRAINT:
				{
					widget.connectCircularConstraint(target, mCircularAngle, (int) mCircularDistance);
				}
				break;
				default:
					break;
			}
		}

		public virtual void apply()
		{
			if (mConstraintWidget == null)
			{
				return;
			}
			if (mFacade != null)
			{
				mFacade.apply();
			}
			mHorizontalDimension.apply(mState, mConstraintWidget, HORIZONTAL);
			mVerticalDimension.apply(mState, mConstraintWidget, VERTICAL);
			dereference();

			applyConnection(mConstraintWidget, mLeftToLeft, State.Constraint.LEFT_TO_LEFT);
			applyConnection(mConstraintWidget, mLeftToRight, State.Constraint.LEFT_TO_RIGHT);
			applyConnection(mConstraintWidget, mRightToLeft, State.Constraint.RIGHT_TO_LEFT);
			applyConnection(mConstraintWidget, mRightToRight, State.Constraint.RIGHT_TO_RIGHT);
			applyConnection(mConstraintWidget, mStartToStart, State.Constraint.START_TO_START);
			applyConnection(mConstraintWidget, mStartToEnd, State.Constraint.START_TO_END);
			applyConnection(mConstraintWidget, mEndToStart, State.Constraint.END_TO_START);
			applyConnection(mConstraintWidget, mEndToEnd, State.Constraint.END_TO_END);
			applyConnection(mConstraintWidget, mTopToTop, State.Constraint.TOP_TO_TOP);
			applyConnection(mConstraintWidget, mTopToBottom, State.Constraint.TOP_TO_BOTTOM);
			applyConnection(mConstraintWidget, mBottomToTop, State.Constraint.BOTTOM_TO_TOP);
			applyConnection(mConstraintWidget, mBottomToBottom, State.Constraint.BOTTOM_TO_BOTTOM);
			applyConnection(mConstraintWidget, mBaselineToBaseline, State.Constraint.BASELINE_TO_BASELINE);
			applyConnection(mConstraintWidget, mCircularConstraint, State.Constraint.CIRCULAR_CONSTRAINT);

			if (mHorizontalChainStyle != ConstraintWidget.CHAIN_SPREAD)
			{
				mConstraintWidget.HorizontalChainStyle = mHorizontalChainStyle;
			}
			if (mVerticalChainStyle != ConstraintWidget.CHAIN_SPREAD)
			{
				mConstraintWidget.VerticalChainStyle = mVerticalChainStyle;
			}

			mConstraintWidget.HorizontalBiasPercent = mHorizontalBias;
			mConstraintWidget.VerticalBiasPercent = mVerticalBias;

			mConstraintWidget.frame.pivotX = mPivotX;
			mConstraintWidget.frame.pivotY = mPivotY;
			mConstraintWidget.frame.rotationX = mRotationX;
			mConstraintWidget.frame.rotationY = mRotationY;
			mConstraintWidget.frame.rotationZ = mRotationZ;
			mConstraintWidget.frame.translationX = mTranslationX;
			mConstraintWidget.frame.translationY = mTranslationY;
			mConstraintWidget.frame.translationZ = mTranslationZ;
			mConstraintWidget.frame.scaleX = mScaleX;
			mConstraintWidget.frame.scaleY = mScaleY;
			mConstraintWidget.frame.alpha = mAlpha;
			mConstraintWidget.frame.visibility = mVisibility;
			mConstraintWidget.Visibility = mVisibility;
			if (mCustomColors != null)
			{
				foreach (string key in mCustomColors.Keys)
				{
					int? color = mCustomColors[key];
					mConstraintWidget.frame.setCustomAttribute(key, TypedValues.TypedValues_Custom.TYPE_COLOR, color.Value);
				}
			}
			if (mCustomFloats != null)
			{
				foreach (string key in mCustomFloats.Keys)
				{
					float value = mCustomFloats[key].Value;
					mConstraintWidget.frame.setCustomAttribute(key, TypedValues.TypedValues_Custom.TYPE_FLOAT, value);
				}
			}
		}
	}

}