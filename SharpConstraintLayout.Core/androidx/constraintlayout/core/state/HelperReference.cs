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
	using Facade = androidx.constraintlayout.core.state.helpers.Facade;
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using HelperWidget = androidx.constraintlayout.core.widgets.HelperWidget;


	public class HelperReference : ConstraintReference, Facade
	{
		protected internal readonly new State mState;
		internal readonly State.Helper mType;
		protected internal List<object> mReferences = new List<object>();
		private HelperWidget mHelperWidget;

		public HelperReference(State state, State.Helper type) : base(state)
		{
			mState = state;
			mType = type;
		}

		public virtual State.Helper Type
		{
			get
			{
				return mType;
			}
		}

		public virtual HelperReference add(params object[] objects)
		{
			//Collections.addAll(mReferences, objects);
			mReferences.addAll(objects);
			return this;
		}

		public virtual HelperWidget HelperWidget
		{
			set
			{
				mHelperWidget = value;
			}
			get
			{
				return mHelperWidget;
			}
		}


		public override ConstraintWidget ConstraintWidget
		{
			get
			{
				return HelperWidget;
			}
		}

		public override void apply()
		{
			// nothing
		}
	}

}