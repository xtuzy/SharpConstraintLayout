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

	public class VerticalChainReference : ChainReference
	{

		public VerticalChainReference(State state) : base(state, State.Helper.VERTICAL_CHAIN)
		{
		}

		public override void apply()
		{
			ConstraintReference first = null;
			ConstraintReference previous = null;
			foreach (object key in mReferences)
			{
				ConstraintReference reference = mState.constraints(key);
				reference.clearVertical();
			}

			foreach (object key in mReferences)
			{
				ConstraintReference reference = mState.constraints(key);
				if (first == null)
				{
					first = reference;
					if (mTopToTop != null)
					{
						first.topToTop(mTopToTop);
					}
					else if (mTopToBottom != null)
					{
						first.topToBottom(mTopToBottom);
					}
					else
					{
						first.topToTop(State.PARENT);
					}
				}
				if (previous != null)
				{
					previous.bottomToTop(reference.Key);
					reference.topToBottom(previous.Key);
				}
				previous = reference;
			}

			if (previous != null)
			{
				if (mBottomToTop != null)
				{
					previous.bottomToTop(mBottomToTop);
				}
				else if (mBottomToBottom != null)
				{
					previous.bottomToBottom(mBottomToBottom);
				}
				else
				{
					previous.bottomToBottom(State.PARENT);
				}
			}

			if (first == null)
			{
				return;
			}

			if (mBias != 0.5f)
			{
				first.verticalBias(mBias);
			}

			switch (mStyle)
			{
				case State.Chain.SPREAD:
				{
					first.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD;
				}
				break;
				case State.Chain.SPREAD_INSIDE:
				{
					first.VerticalChainStyle = ConstraintWidget.CHAIN_SPREAD_INSIDE;
				}
				break;
				case State.Chain.PACKED:
				{
					first.VerticalChainStyle = ConstraintWidget.CHAIN_PACKED;
				}
			break;
			}
		}

	}

}