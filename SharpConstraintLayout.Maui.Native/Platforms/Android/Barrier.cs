using System;

/*
 * Copyright (C) 2017 The Android Open Source Project
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
	using Context = Android.Content.Context;
	using TypedArray = Android.content.res.TypedArray;
	using Build = Android.OS.Build;
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
	using HelperWidget = androidx.constraintlayout.core.widgets.HelperWidget;
	using AttributeSet = android.util.AttributeSet;
	using SparseArray = android.util.SparseArray;
	using View = android.view.View;

	/// <summary>
	/// <b>Added in 1.1</b>
	/// <para>
	/// A Barrier references multiple widgets as input, and creates a virtual guideline based on the most
	/// extreme widget on the specified side. For example, a left barrier will align to the left of all the referenced views.
	/// </para>
	/// <para>
	/// <h2>Example</h2>
	/// </para>
	///     <para><div align="center" >
	///       <img width="325px" src="resources/images/barrier-buttons.png">
	///     </div>
	///     Let's have two buttons, @id/button1 and @id/button2. The constraint_referenced_ids field will reference
	///     them by simply having them as comma-separated list:
	///     <pre>
	///     {@code
	///         <androidx.constraintlayout.widget.Barrier
	///              android:id="@+id/barrier"
	///              android:layout_width="wrap_content"
	///              android:layout_height="wrap_content"
	///              app:barrierDirection="start"
	///              app:constraint_referenced_ids="button1,button2" />
	///     }
	///     </pre>
	/// </para>
	///     <para>
	///         With the barrier direction set to start, we will have the following result:
	/// </para>
	///     <para><div align="center" >
	///       <img width="325px" src="resources/images/barrier-start.png">
	///     </div>
	/// </para>
	///     <para>
	///         Reversely, with the direction set to end, we will have:
	/// </para>
	///     <para><div align="center" >
	///       <img width="325px" src="resources/images/barrier-end.png">
	///     </div>
	/// </para>
	///     <para>
	///         If the widgets dimensions change, the barrier will automatically move according to its direction to get
	///         the most extreme widget:
	/// </para>
	///     <para><div align="center" >
	///       <img width="325px" src="resources/images/barrier-adapt.png">
	///     </div>
	/// 
	/// </para>
	///     <para>
	///         Other widgets can then be constrained to the barrier itself, instead of the individual widget. This allows a layout
	///         to automatically adapt on widget dimension changes (e.g. different languages will end up with different length for similar worlds).
	///     </para>
	/// <h2>GONE widgets handling</h2>
	/// <para>If the barrier references GONE widgets, the default behavior is to create a barrier on the resolved position of the GONE widget.
	/// If you do not want to have the barrier take GONE widgets into account, you can change this by setting the attribute <i>barrierAllowsGoneWidgets</i>
	/// to false (default being true).</para>
	///     </p>
	/// </p>
	/// 
	/// </summary>
	public class Barrier : ConstraintHelper
	{

		/// <summary>
		/// Left direction constant
		/// </summary>
		public const int LEFT = androidx.constraintlayout.core.widgets.Barrier.LEFT;

		/// <summary>
		/// Top direction constant
		/// </summary>
		public const int TOP = androidx.constraintlayout.core.widgets.Barrier.TOP;

		/// <summary>
		/// Right direction constant
		/// </summary>
		public const int RIGHT = androidx.constraintlayout.core.widgets.Barrier.RIGHT;

		/// <summary>
		/// Bottom direction constant
		/// </summary>
		public const int BOTTOM = androidx.constraintlayout.core.widgets.Barrier.BOTTOM;

		/// <summary>
		/// Start direction constant
		/// </summary>
		public static readonly int START = BOTTOM + 2;

		/// <summary>
		/// End Barrier constant
		/// </summary>
		public static readonly int END = START + 1;

		private int mIndicatedType;
		private int mResolvedType;
		private androidx.constraintlayout.core.widgets.Barrier mBarrier;

		public Barrier(Context context) : base(context)
		{
			base.Visibility = View.GONE;
		}

		public Barrier(Context context, AttributeSet attrs) : base(context, attrs)
		{
			base.Visibility = View.GONE;
		}

		public Barrier(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			base.Visibility = View.GONE;
		}

		/// <summary>
		/// Get the barrier type ({@code Barrier.LEFT}, {@code Barrier.TOP},
		/// {@code Barrier.RIGHT}, {@code Barrier.BOTTOM}, {@code Barrier.END},
		/// {@code Barrier.START})
		/// </summary>
		public virtual int Type
		{
			get
			{
				return mIndicatedType;
			}
			set
			{
				mIndicatedType = value;
			}
		}


		private void updateType(ConstraintWidget widget, int type, bool isRtl)
		{
			mResolvedType = type;
			if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1)
			{
				// Pre JB MR1, left/right should take precedence, unless they are
				// not defined and somehow a corresponding start/end constraint exists
				if (mIndicatedType == START)
				{
					mResolvedType = LEFT;
				}
				else if (mIndicatedType == END)
				{
					mResolvedType = RIGHT;
				}
			}
			else
			{
				// Post JB MR1, if start/end are defined, they take precedence over left/right
				if (isRtl)
				{
					if (mIndicatedType == START)
					{
						mResolvedType = RIGHT;
					}
					else if (mIndicatedType == END)
					{
						mResolvedType = LEFT;
					}
				}
				else
				{
					if (mIndicatedType == START)
					{
						mResolvedType = LEFT;
					}
					else if (mIndicatedType == END)
					{
						mResolvedType = RIGHT;
					}
				}
			}
			if (widget is androidx.constraintlayout.core.widgets.Barrier)
			{
				androidx.constraintlayout.core.widgets.Barrier barrier = (androidx.constraintlayout.core.widgets.Barrier) widget;
				barrier.BarrierType = mResolvedType;
			}
		}

		public override void resolveRtl(ConstraintWidget widget, bool isRtl)
		{
			updateType(widget, mIndicatedType, isRtl);
		}

		/// <param name="attrs">
		/// @suppress </param>
		protected internal override void init(AttributeSet attrs)
		{
			base.init(attrs);
			mBarrier = new androidx.constraintlayout.core.widgets.Barrier();
			if (attrs != null)
			{
				TypedArray a = Context.obtainStyledAttributes(attrs, R.styleable.ConstraintLayout_Layout);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
				int N = a.IndexCount;
				for (int i = 0; i < N; i++)
				{
					int attr = a.getIndex(i);
					if (attr == R.styleable.ConstraintLayout_Layout_barrierDirection)
					{
						Type = a.getInt(attr, LEFT);
					}
					else if (attr == R.styleable.ConstraintLayout_Layout_barrierAllowsGoneWidgets)
					{
						mBarrier.AllowsGoneWidget = a.getBoolean(attr, true);
					}
					else if (attr == R.styleable.ConstraintLayout_Layout_barrierMargin)
					{
						int margin = a.getDimensionPixelSize(attr, 0);
						mBarrier.Margin = margin;
					}
				}
				a.recycle();
			}
			mHelperWidget = mBarrier;
			validateParams();
		}

		public virtual bool AllowsGoneWidget
		{
			set
			{
				mBarrier.AllowsGoneWidget = value;
			}
			get
			{
				return mBarrier.AllowsGoneWidget;
			}
		}

		/// <summary>
		/// Find if this barrier supports gone widgets.
		/// </summary>
		/// <returns> true if this barrier supports gone widgets, otherwise false
		/// </returns>
		/// @deprecated This method should be called {@code getAllowsGoneWidget} such that {@code allowsGoneWidget} 
		/// can be accessed as a property from Kotlin; {<seealso cref= https://android.github.io/kotlin-guides/interop.html#property-prefixes}.
		/// Use <seealso cref="#getAllowsGoneWidget()"/> instead. </seealso>
		[Obsolete("This method should be called {@code getAllowsGoneWidget} such that {@code allowsGoneWidget}")]
		public virtual bool allowsGoneWidget()
		{
			return mBarrier.AllowsGoneWidget;
		}


		/// <summary>
		/// Set a margin on the barrier
		/// </summary>
		/// <param name="margin"> in dp </param>
		public virtual int DpMargin
		{
			set
			{
				float density = Resources.DisplayMetrics.density;
				int px = (int)(0.5f + value * density);
				mBarrier.Margin = px;
			}
		}

		/// <summary>
		/// Returns the barrier margin
		/// </summary>
		/// <returns> the barrier margin (in pixels) </returns>
		public virtual int Margin
		{
			get
			{
				return mBarrier.Margin;
			}
			set
			{
				mBarrier.Margin = value;
			}
		}


		public override void loadParameters(ConstraintSet.Constraint constraint, HelperWidget child, ConstraintLayout.LayoutParams layoutParams, SparseArray<ConstraintWidget> mapIdToWidget)
		{
			base.loadParameters(constraint, child, layoutParams, mapIdToWidget);
			if (child is androidx.constraintlayout.core.widgets.Barrier)
			{
				androidx.constraintlayout.core.widgets.Barrier barrier = (androidx.constraintlayout.core.widgets.Barrier) child;
				ConstraintWidgetContainer container = (ConstraintWidgetContainer) child.Parent;
				bool isRtl = container.Rtl;
				updateType(barrier, constraint.layout.mBarrierDirection, isRtl);
				barrier.AllowsGoneWidget = constraint.layout.mBarrierAllowsGoneWidgets;
				barrier.Margin = constraint.layout.mBarrierMargin;
			}
		}
	}

}