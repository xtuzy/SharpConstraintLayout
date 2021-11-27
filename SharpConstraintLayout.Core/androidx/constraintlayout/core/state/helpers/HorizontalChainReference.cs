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

namespace androidx.constraintlayout.core.state.helpers
{
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;

	public class HorizontalChainReference : ChainReference
	{

		public HorizontalChainReference(State state) : base(state, State.Helper.HORIZONTAL_CHAIN)
		{
		}

		public override void apply()
		{
			ConstraintReference first = null;
			ConstraintReference previous = null;
			foreach (object key in mReferences)
			{
				ConstraintReference reference = mState.constraints(key);
				reference.clearHorizontal();
			}

			foreach (object key in mReferences)
			{
				ConstraintReference reference = mState.constraints(key);
				if (first == null)
				{
					first = reference;
					if (mStartToStart != null)
					{
						first.startToStart(mStartToStart).margin(mMarginStart);
					}
					else if (mStartToEnd != null)
					{
						first.startToEnd(mStartToEnd).margin(mMarginStart);
					}
					else
					{
						first.startToStart(State.PARENT);
					}
				}
				if (previous != null)
				{
					previous.endToStart(reference.Key);
					reference.startToEnd(previous.Key);
				}
				previous = reference;
			}

			if (previous != null)
			{
				if (mEndToStart != null)
				{
					previous.endToStart(mEndToStart).margin(mMarginEnd);
				}
				else if (mEndToEnd != null)
				{
					previous.endToEnd(mEndToEnd).margin(mMarginEnd);
				}
				else
				{
					previous.endToEnd(State.PARENT);
				}
			}

			if (first == null)
			{
				return;
			}

			if (mBias != 0.5f)
			{
				first.horizontalBias(mBias);
			}

			switch (mStyle)
			{
				case State.Chain.SPREAD:
				{
					first.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD;
				}
				break;
				case State.Chain.SPREAD_INSIDE:
				{
					first.HorizontalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
				}
				break;
				case State.Chain.PACKED:
				{
					first.HorizontalChainStyle = ConstraintWidget.CHAIN_PACKED;
				}
			break;
			}
		}

	}

}