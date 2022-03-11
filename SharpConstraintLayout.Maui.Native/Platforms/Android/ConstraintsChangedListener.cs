/*
 * Copyright (C) 2018 The Android Open Source Project
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
	/// <summary>
	/// <b>Added in 2.0</b>
	/// <para>
	/// Callbacks on state change
	/// </para>
	/// </summary>
	public abstract class ConstraintsChangedListener
	{

		/// <summary>
		/// called before layout happens </summary>
		/// <param name="stateId"> -1 if state unknown, otherwise the state we will transition to </param>
		/// <param name="constraintId"> the constraintSet id that we will transition to </param>
		public virtual void preLayoutChange(int stateId, int constraintId)
		{
			// nothing
		}

		/// <summary>
		/// called after layout happens </summary>
		/// <param name="stateId"> -1 if state unknown, otherwise the current state </param>
		/// <param name="constraintId"> the current constraintSet id we transitioned to </param>
		public virtual void postLayoutChange(int stateId, int constraintId)
		{
			// nothing
		}
	}

}