/*
 * Copyright 2019 The Android Open Source Project
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

namespace androidx.constraintlayout.core.state.helpers
{
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using Guideline = androidx.constraintlayout.core.widgets.Guideline;

	public class GuidelineReference : Facade, Reference
	{

		internal readonly State mState;
		private int mOrientation;
		private Guideline mGuidelineWidget;
		private int mStart = -1;
		private int mEnd = -1;
		private float mPercent = 0;

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

		public GuidelineReference(State state)
		{
			mState = state;
		}

		public virtual GuidelineReference start(object margin)
		{
			mStart = mState.convertDimension(margin);
			mEnd = -1;
			mPercent = 0;
			return this;
		}

		public virtual GuidelineReference end(object margin)
		{
			mStart = -1;
			mEnd = mState.convertDimension(margin);
			mPercent = 0;
			return this;
		}

		public virtual GuidelineReference percent(float percent)
		{
			mStart = -1;
			mEnd = -1;
			mPercent = percent;
			return this;
		}

		public virtual int Orientation
		{
			set
			{
				mOrientation = value;
			}
			get
			{
				return mOrientation;
			}
		}


		public virtual void apply()
		{
			mGuidelineWidget.Orientation = mOrientation;
			if (mStart != -1)
			{
				mGuidelineWidget.GuideBegin = mStart;
			}
			else if (mEnd != -1)
			{
				mGuidelineWidget.GuideEnd = mEnd;
			}
			else
			{
				mGuidelineWidget.setGuidePercent(mPercent);
			}
		}

		public virtual Facade Facade
		{
			get
			{
				return null;
			}
		}

		public virtual ConstraintWidget ConstraintWidget
		{
			get
			{
				if (mGuidelineWidget == null)
				{
					mGuidelineWidget = new Guideline();
				}
				return mGuidelineWidget;
			}
			set
			{
				if (value is Guideline)
				{
					mGuidelineWidget = (Guideline) value;
				}
				else
				{
					mGuidelineWidget = null;
				}
			}
		}

	}
}