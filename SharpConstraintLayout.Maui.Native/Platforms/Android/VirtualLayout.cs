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
	using Context = android.content.Context;
	using TypedArray = android.content.res.TypedArray;
	using AttributeSet = android.util.AttributeSet;
	using View = android.view.View;
	using ViewParent = android.view.ViewParent;

	/// <summary>
	///  <b>Added in 2.0</b>
	///  <para>
	///  </para>
	///  @suppress
	/// </summary>
	public abstract class VirtualLayout : ConstraintHelper
	{
		private bool mApplyVisibilityOnAttach;
		private bool mApplyElevationOnAttach;

		public VirtualLayout(Context context) : base(context)
		{
		}

		public VirtualLayout(Context context, AttributeSet attrs) : base(context, attrs)
		{
		}

		public VirtualLayout(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
		}

		protected internal override void init(AttributeSet attrs)
		{
			base.init(attrs);
			if (attrs != null)
			{
				TypedArray a = Context.obtainStyledAttributes(attrs, R.styleable.ConstraintLayout_Layout);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
				int N = a.IndexCount;
				for (int i = 0; i < N; i++)
				{
					int attr = a.getIndex(i);
					if (attr == R.styleable.ConstraintLayout_Layout_android_visibility)
					{
						mApplyVisibilityOnAttach = true;
					}
					else if (attr == R.styleable.ConstraintLayout_Layout_android_elevation)
					{
						mApplyElevationOnAttach = true;
					}
				}
				a.recycle();
			}
		}

		public virtual void onMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec)
		{
			// nothing
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public override void onAttachedToWindow()
		{
			base.onAttachedToWindow();
			if (mApplyVisibilityOnAttach || mApplyElevationOnAttach)
			{
				ViewParent parent = Parent;
				if (parent is ConstraintLayout)
				{
					ConstraintLayout container = (ConstraintLayout) parent;
					int visibility = Visibility;
					float elevation = 0;
					if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP)
					{
						elevation = Elevation;
					}
					for (int i = 0; i < mCount; i++)
					{
						int id = mIds[i];
						View view = container.getViewById(id);
						if (view != null)
						{
							if (mApplyVisibilityOnAttach)
							{
								view.Visibility = visibility;
							}
							if (mApplyElevationOnAttach)
							{
								if (elevation > 0 && android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP)
								{
									view.TranslationZ = view.TranslationZ + elevation;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public override int Visibility
		{
			set
			{
				base.Visibility = value;
				applyLayoutFeatures();
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
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
	}

}