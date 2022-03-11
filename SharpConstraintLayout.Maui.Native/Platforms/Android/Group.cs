/*
 * Copyright (C) 2021 The Android Open Source Project
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
	using Context = android.content.Context;
	using AttributeSet = android.util.AttributeSet;
	using View = android.view.View;
	using ViewParent = android.view.ViewParent;

	/// <summary>
	/// Control the visibility and elevation of the referenced views
	/// 
	/// <b>Added in 1.1</b>
	/// <para>
	///     This class controls the visibility of a set of referenced widgets.
	///     Widgets are referenced by being added to a comma separated list of ids, e.g:
	///     <pre>
	///     {@code
	///          <androidx.constraintlayout.widget.Group
	///              android:id="@+id/group"
	///              android:layout_width="wrap_content"
	///              android:layout_height="wrap_content"
	///              android:visibility="visible"
	///              app:constraint_referenced_ids="button4,button9" />
	///     }
	///     </pre>
	/// </para>
	///     <para>
	///         The visibility of the group will be applied to the referenced widgets.
	///         It's a convenient way to easily hide/show a set of widgets without having to maintain this set
	///         programmatically.
	/// </para>
	///     <para>
	///     <h2>Multiple groups</h2>
	/// </para>
	///     <para>
	///         Multiple groups can reference the same widgets -- in that case, the XML declaration order will
	///         define the final visibility state (the group declared last will have the last word).
	/// </para>
	/// </summary>
	public class Group : ConstraintHelper
	{

		public Group(Context context) : base(context)
		{
		}

		public Group(Context context, AttributeSet attrs) : base(context, attrs)
		{
		}

		public Group(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
		}

		/// <summary>
		/// @suppress </summary>
		/// <param name="attrs"> </param>
		protected internal override void init(AttributeSet attrs)
		{
			base.init(attrs);
			mUseViewMeasure = false;
		}

		public override void onAttachedToWindow()
		{
			base.onAttachedToWindow();
			applyLayoutFeatures();
		}

		public override int Visibility
		{
			set
			{
				base.Visibility = value;
				applyLayoutFeatures();
			}
		}

		public override float Elevation
		{
			set
			{
				base.Elevation = value;
				applyLayoutFeatures();
			}
		}

		/// <summary>
		/// @suppress </summary>
		/// <param name="container"> </param>
		protected internal override void applyLayoutFeaturesInConstraintSet(ConstraintLayout container)
		{
			applyLayoutFeatures(container);
		}

		/// <summary>
		/// @suppress </summary>
		/// <param name="container"> </param>
		public override void updatePostLayout(ConstraintLayout container)
		{
			ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams) LayoutParams;
			@params.widget.Width = 0;
			@params.widget.Height = 0;
		}
	}

}