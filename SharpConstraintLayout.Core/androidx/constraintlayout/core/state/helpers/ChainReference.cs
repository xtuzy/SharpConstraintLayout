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

	public class ChainReference : HelperReference
	{

		protected internal float mBias = 0.5f;
		protected internal State.Chain mStyle = State.Chain.SPREAD;

		public ChainReference(State state, State.Helper type) : base(state, type)
		{
		}

		public virtual State.Chain Style
		{
			get
			{
				return State.Chain.SPREAD;
			}
		}
		public virtual ChainReference style(State.Chain style)
		{
			mStyle = style;
			return this;
		}
		public virtual float Bias
		{
			get
			{
				return mBias;
			}
		}
		//public override ChainReference bias(float bias)
		public override ConstraintReference bias(float bias)
		{
			mBias = bias;
			return this;
		}

	}

}