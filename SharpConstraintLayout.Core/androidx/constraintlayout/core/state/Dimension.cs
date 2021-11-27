using System;

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
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;

	using static androidx.constraintlayout.core.widgets.ConstraintWidget;

	/// <summary>
	/// Represents a dimension (width or height) of a constrained widget
	/// </summary>
	public class Dimension
	{

		public static readonly object FIXED_DIMENSION = new object();
		public static readonly object WRAP_DIMENSION = new object();
		public static readonly object SPREAD_DIMENSION = new object();
		public static readonly object PARENT_DIMENSION = new object();
		public static readonly object PERCENT_DIMENSION = new object();
		public static readonly object RATIO_DIMENSION = new object();

		private readonly int WRAP_CONTENT = -2;

		internal int mMin = 0;
		internal int mMax = int.MaxValue;
		internal float mPercent = 1f;
		internal int mValue = 0;
		internal string mRatioString = null;
		internal object mInitialValue = WRAP_DIMENSION;
		internal bool mIsSuggested = false;

		/// <summary>
		/// Returns true if the dimension is a fixed dimension of
		/// the same given value
		/// </summary>
		/// <param name="value">
		/// @return </param>
		public virtual bool equalsFixedValue(int value)
		{
			if (mInitialValue == null && mValue == value)
			{
				return true;
			}
			return false;
		}

		public enum Type
		{
			FIXED,
			WRAP,
			MATCH_PARENT,
			MATCH_CONSTRAINT
		}

		private Dimension()
		{
		}
		private Dimension(object type)
		{
			mInitialValue = type;
		}

		public static Dimension Suggested(int value)
		{
			Dimension dimension = new Dimension();
			dimension.suggested(value);
			return dimension;
		}

		public static Dimension Suggested(object startValue)
		{
			Dimension dimension = new Dimension();
			dimension.suggested(startValue);
			return dimension;
		}

		public static Dimension Fixed(int value)
		{
			Dimension dimension = new Dimension(FIXED_DIMENSION);
			dimension.@fixed(value);
			return dimension;
		}

		public static Dimension Fixed(object value)
		{
			Dimension dimension = new Dimension(FIXED_DIMENSION);
			dimension.@fixed(value);
			return dimension;
		}

		public static Dimension Percent(object key, float value)
		{
			Dimension dimension = new Dimension(PERCENT_DIMENSION);
			dimension.percent(key, value);
			return dimension;
		}

		public static Dimension Parent()
		{
			return new Dimension(PARENT_DIMENSION);
		}

		public static Dimension Wrap()
		{
			return new Dimension(WRAP_DIMENSION);
		}

		public static Dimension Spread()
		{
			return new Dimension(SPREAD_DIMENSION);
		}

		public static Dimension Ratio(string ratio)
		{
			Dimension dimension = new Dimension(RATIO_DIMENSION);
			dimension.ratio(ratio);
			return dimension;
		}

		public virtual Dimension percent(object key, float value)
		{
			mPercent = value;
			return this;
		}

		public virtual Dimension min(int value)
		{
			if (value >= 0)
			{
				mMin = value;
			}
			return this;
		}

		public virtual Dimension min(object value)
		{
			if (value == WRAP_DIMENSION)
			{
				mMin = WRAP_CONTENT;
			}
			return this;
		}

		public virtual Dimension max(int value)
		{
			if (mMax >= 0)
			{
				mMax = value;
			}
			return this;
		}

		public virtual Dimension max(object value)
		{
			if (value == WRAP_DIMENSION && mIsSuggested)
			{
				mInitialValue = WRAP_DIMENSION;
				mMax = int.MaxValue;
			}
			return this;
		}

		public virtual Dimension suggested(int value)
		{
			mIsSuggested = true;
			return this;
		}

		public virtual Dimension suggested(object value)
		{
			mInitialValue = value;
			mIsSuggested = true;
			return this;
		}

		public virtual Dimension @fixed(object value)
		{
			mInitialValue = value;
			if (value is int?)
			{
				mValue = ((int?) value).Value;
				mInitialValue = null;
			}
			return this;
		}

		public virtual Dimension @fixed(int value)
		{
			mInitialValue = null;
			mValue = value;
			return this;
		}

		public virtual Dimension ratio(string ratio)
		{ // WxH ratio
			mRatioString = ratio;
			return this;
		}

		internal virtual int Value
		{
			set
			{
				mIsSuggested = false; // fixed value
				mInitialValue = null;
				mValue = value;
			}
			get
			{
				return mValue;
			}
		}


		/// <summary>
		/// Apply the dimension to the given constraint widget </summary>
		/// <param name="constraintWidget"> </param>
		/// <param name="orientation"> </param>
		public virtual void apply(State state, ConstraintWidget constraintWidget, int orientation)
		{
			if (!string.ReferenceEquals(mRatioString, null))
			{
				constraintWidget.setDimensionRatio(mRatioString);
			}
			if (orientation == HORIZONTAL)
			{
				if (mIsSuggested)
				{
					constraintWidget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
					int type = MATCH_CONSTRAINT_SPREAD;
					if (mInitialValue == WRAP_DIMENSION)
					{
						type = MATCH_CONSTRAINT_WRAP;
					}
					else if (mInitialValue == PERCENT_DIMENSION)
					{
						type = MATCH_CONSTRAINT_PERCENT;
					}
					constraintWidget.setHorizontalMatchStyle(type, mMin, mMax, mPercent);
				}
				else
				{ // fixed
					if (mMin > 0)
					{
						constraintWidget.MinWidth = mMin;
					}
					if (mMax < int.MaxValue)
					{
						constraintWidget.MaxWidth = mMax;
					}
					if (mInitialValue == WRAP_DIMENSION)
					{
						constraintWidget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
					}
					else if (mInitialValue == PARENT_DIMENSION)
					{
						constraintWidget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
					}
					else if (mInitialValue == null)
					{
						constraintWidget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
						constraintWidget.Width = mValue;
					}
				}
			}
			else
			{
				if (mIsSuggested)
				{
					constraintWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
					int type = MATCH_CONSTRAINT_SPREAD;
					if (mInitialValue == WRAP_DIMENSION)
					{
						type = MATCH_CONSTRAINT_WRAP;
					}
					else if (mInitialValue == PERCENT_DIMENSION)
					{
						type = MATCH_CONSTRAINT_PERCENT;
					}
					constraintWidget.setVerticalMatchStyle(type, mMin, mMax, mPercent);
				}
				else
				{ // fixed
					if (mMin > 0)
					{
						constraintWidget.MinHeight = mMin;
					}
					if (mMax < int.MaxValue)
					{
						constraintWidget.MaxHeight = mMax;
					}
					if (mInitialValue == WRAP_DIMENSION)
					{
						constraintWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
					}
					else if (mInitialValue == PARENT_DIMENSION)
					{
						constraintWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
					}
					else if (mInitialValue == null)
					{
						constraintWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
						constraintWidget.Height = mValue;
					}
				}
			}
		}

	}

}