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
	using Barrier = androidx.constraintlayout.core.widgets.Barrier;
	using HelperWidget = androidx.constraintlayout.core.widgets.HelperWidget;

	public class BarrierReference : HelperReference
	{

		private State.Direction mDirection;
		private int mMargin;
		private Barrier mBarrierWidget;

		public BarrierReference(State state) : base(state, State.Helper.BARRIER)
		{
		}

		public virtual State.Direction BarrierDirection
		{
			set
			{
				mDirection = value;
			}
		}

		public override ConstraintReference margin(object value)
		{
			margin(mState.convertDimension(value));
			return this;
		}

		public override ConstraintReference margin(int value)
		{
			mMargin = value;
			return this;
		}

		/// <summary>
		/// getHelperWidget();
		/// </summary>
		public override HelperWidget HelperWidget
		{
			get
			{
				if (mBarrierWidget == null)
				{
					mBarrierWidget = new Barrier();
				}
				return mBarrierWidget;
			}
		}

		public override void apply()
		{
			var temp = HelperWidget;
			int direction = Barrier.LEFT;
			switch (mDirection)
			{
				case State.Direction.LEFT:
				case State.Direction.START:
				{
					// TODO: handle RTL
				}
				break;
				case State.Direction.RIGHT:
				case State.Direction.END:
				{
					// TODO: handle RTL
					direction = Barrier.RIGHT;
				}
				break;
				case State.Direction.TOP:
				{
					direction = Barrier.TOP;
				}
				break;
				case State.Direction.BOTTOM:
				{
					direction = Barrier.BOTTOM;
				}
			break;
			}
			mBarrierWidget.BarrierType = direction;
			mBarrierWidget.Margin = mMargin;
		}
	}

}