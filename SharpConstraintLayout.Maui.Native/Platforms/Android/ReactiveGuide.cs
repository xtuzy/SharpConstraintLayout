/*
 * Copyright (C) 2020 The Android Open Source Project
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

namespace SharpConstraintLayout.Maui.Platforms.Androids
{
	using SuppressLint = android.annotation.SuppressLint;
	using Context = android.content.Context;
	using TypedArray = android.content.res.TypedArray;
	using Canvas = android.graphics.Canvas;
	using AttributeSet = android.util.AttributeSet;
	using View = android.view.View;

	using MotionLayout = androidx.constraintlayout.motion.widget.MotionLayout;

	/// <summary>
	/// Utility class representing a reactive Guideline helper object for <seealso cref="ConstraintLayout"/>.
	/// </summary>
	public class ReactiveGuide : View, SharedValues.SharedValuesListener
	{
		private int mAttributeId = -1;
		private bool mAnimateChange = false;
		private int mApplyToConstraintSetId = 0;
		private bool mApplyToAllConstraintSets = true;

		public ReactiveGuide(Context context) : base(context)
		{
			base.Visibility = View.GONE;
			init(null);
		}

		public ReactiveGuide(Context context, AttributeSet attrs) : base(context, attrs)
		{
			base.Visibility = View.GONE;
			init(attrs);
		}

		public ReactiveGuide(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			base.Visibility = View.GONE;
			init(attrs);
		}

		public ReactiveGuide(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr)
		{
			base.Visibility = View.GONE;
			init(attrs);
		}

		private void init(AttributeSet attrs)
		{
			if (attrs != null)
			{
				TypedArray a = Context.obtainStyledAttributes(attrs, R.styleable.ConstraintLayout_ReactiveGuide);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
				int N = a.IndexCount;
				for (int i = 0; i < N; i++)
				{
					int attr = a.getIndex(i);
					if (attr == R.styleable.ConstraintLayout_ReactiveGuide_reactiveGuide_valueId)
					{
						mAttributeId = a.getResourceId(attr, mAttributeId);
					}
					else if (attr == R.styleable.ConstraintLayout_ReactiveGuide_reactiveGuide_animateChange)
					{
						mAnimateChange = a.getBoolean(attr, mAnimateChange);
					}
					else if (attr == R.styleable.ConstraintLayout_ReactiveGuide_reactiveGuide_applyToConstraintSet)
					{
						mApplyToConstraintSetId = a.getResourceId(attr, mApplyToConstraintSetId);
					}
					else if (attr == R.styleable.ConstraintLayout_ReactiveGuide_reactiveGuide_applyToAllConstraintSets)
					{
						mApplyToAllConstraintSets = a.getBoolean(attr, mApplyToAllConstraintSets);
					}
				}
				a.recycle();
			}
			if (mAttributeId != -1)
			{
				SharedValues sharedValues = ConstraintLayout.SharedValues;
				sharedValues.addListener(mAttributeId, this);
			}
		}

		public virtual int AttributeId
		{
			get
			{
				return mAttributeId;
			}
			set
			{
				SharedValues sharedValues = ConstraintLayout.SharedValues;
				if (mAttributeId != -1)
				{
					sharedValues.removeListener(mAttributeId, this);
				}
				mAttributeId = value;
				if (mAttributeId != -1)
				{
					sharedValues.addListener(mAttributeId, this);
				}
			}
		}


		public virtual int ApplyToConstraintSetId
		{
			get
			{
				return mApplyToConstraintSetId;
			}
			set
			{
				mApplyToConstraintSetId = value;
			}
		}


		public virtual bool AnimatingChange
		{
			get
			{
				return mAnimateChange;
			}
		}

		public virtual bool AnimateChange
		{
			set
			{
				mAnimateChange = value;
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public override int Visibility
		{
			set
			{
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressLint("MissingSuperCall") @Override public void draw(android.graphics.Canvas canvas)
		public override void draw(Canvas canvas)
		{
		}

		/// <summary>
		/// @suppress
		/// </summary>
		protected internal override void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			setMeasuredDimension(0, 0);
		}

		/// <summary>
		/// Set the guideline's distance from the top or left edge.
		/// </summary>
		/// <param name="margin"> the distance to the top or left edge </param>
		public virtual int GuidelineBegin
		{
			set
			{
				ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams) LayoutParams;
				@params.guideBegin = value;
				LayoutParams = @params;
			}
		}

		/// <summary>
		/// Set a guideline's distance to end.
		/// </summary>
		/// <param name="margin"> the margin to the right or bottom side of container </param>
		public virtual int GuidelineEnd
		{
			set
			{
				ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams) LayoutParams;
				@params.guideEnd = value;
				LayoutParams = @params;
			}
		}

		/// <summary>
		/// Set a Guideline's percent. </summary>
		/// <param name="ratio"> the ratio between the gap on the left and right 0.0 is top/left 0.5 is middle </param>
		public virtual float GuidelinePercent
		{
			set
			{
				ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams) LayoutParams;
				@params.guidePercent = value;
				LayoutParams = @params;
			}
		}

		public virtual void onNewValue(int key, int newValue, int oldValue)
		{
			GuidelineBegin = newValue;
			int id = Id;
			if (id <= 0)
			{
				return;
			}
			if (Parent is MotionLayout)
			{
				MotionLayout motionLayout = (MotionLayout) Parent;
				int currentState = motionLayout.CurrentState;
				if (mApplyToConstraintSetId != 0)
				{
					currentState = mApplyToConstraintSetId;
				}
				if (mAnimateChange)
				{
					if (mApplyToAllConstraintSets)
					{
						int[] ids = motionLayout.ConstraintSetIds;
						for (int i = 0; i < ids.Length; i++)
						{
							int cs = ids[i];
							if (cs != currentState)
							{
								changeValue(newValue, id, motionLayout, cs);
							}
						}
					}
					ConstraintSet constraintSet = motionLayout.cloneConstraintSet(currentState);
					constraintSet.setGuidelineEnd(id, newValue);
					motionLayout.updateStateAnimate(currentState, constraintSet, 1000);
				}
				else
				{
					if (mApplyToAllConstraintSets)
					{
						int[] ids = motionLayout.ConstraintSetIds;
						for (int i = 0; i < ids.Length; i++)
						{
							int cs = ids[i];
							changeValue(newValue, id, motionLayout, cs);
						}
					}
					else
					{
						changeValue(newValue, id, motionLayout, currentState);
					}
				}
			}
		}

		private void changeValue(int newValue, int id, MotionLayout motionLayout, int currentState)
		{
			ConstraintSet constraintSet = motionLayout.getConstraintSet(currentState);
			constraintSet.setGuidelineEnd(id, newValue);
			motionLayout.updateState(currentState, constraintSet);
		}
	}

}