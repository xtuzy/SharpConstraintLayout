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
	using Context = android.content.Context;
	using TypedArray = android.content.res.TypedArray;
	using Build = android.os.Build;
	using LayoutParams = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams;
	using AttributeSet = android.util.AttributeSet;
	using Log = android.util.Log;
	using View = android.view.View;
	using ViewGroup = android.view.ViewGroup;
	using ViewParent = android.view.ViewParent;

	/// <summary>
	/// @suppress
	/// This defines the internally defined Constraint set
	/// It allows you to have a group of References which point to other views and provide them with
	/// constraint attributes
	/// 
	/// </summary>
	public class Constraints : ViewGroup
	{

	  public const string TAG = "Constraints";
	  internal ConstraintSet myConstraintSet;

	  public Constraints(Context context) : base(context)
	  {
		base.Visibility = View.GONE;
	  }

	  public Constraints(Context context, AttributeSet attrs) : base(context, attrs)
	  {
		init(attrs);
		base.Visibility = View.GONE;
	  }

	  public Constraints(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
	  {
		init(attrs);
		base.Visibility = View.GONE;
	  }

	  /// <summary>
	  /// {@inheritDoc}
	  /// </summary>
	  public override LayoutParams generateLayoutParams(AttributeSet attrs)
	  {
		return new LayoutParams(Context, attrs);
	  }

	  public class LayoutParams : ConstraintLayout.LayoutParams
	  {

		public float alpha = 1;
		public bool applyElevation = false;
		public float elevation = 0;
		public float rotation = 0;
		public float rotationX = 0;
		public float rotationY = 0;
		public float scaleX = 1;
		public float scaleY = 1;
		public float transformPivotX = 0;
		public float transformPivotY = 0;
		public float translationX = 0;
		public float translationY = 0;
		public float translationZ = 0;

		public LayoutParams(int width, int height) : base(width, height)
		{
		}

		public LayoutParams(LayoutParams source) : base(source)
		{
		}

		public LayoutParams(Context c, AttributeSet attrs) : base(c, attrs)
		{
		  TypedArray a = c.obtainStyledAttributes(attrs, R.styleable.ConstraintSet);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
		  int N = a.IndexCount;
		  for (int i = 0; i < N; i++)
		  {
			int attr = a.getIndex(i);
			if (attr == R.styleable.ConstraintSet_android_alpha)
			{
			  alpha = a.getFloat(attr, alpha);
			}
			else if (attr == R.styleable.ConstraintSet_android_elevation)
			{
			  if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
			  {
				elevation = a.getFloat(attr, elevation);
				applyElevation = true;
			  }
			}
			else if (attr == R.styleable.ConstraintSet_android_rotationX)
			{
			  rotationX = a.getFloat(attr, rotationX);
			}
			else if (attr == R.styleable.ConstraintSet_android_rotationY)
			{
			  rotationY = a.getFloat(attr, rotationY);
			}
			else if (attr == R.styleable.ConstraintSet_android_rotation)
			{
			  rotation = a.getFloat(attr, rotation);
			}
			else if (attr == R.styleable.ConstraintSet_android_scaleX)
			{
			  scaleX = a.getFloat(attr, scaleX);
			}
			else if (attr == R.styleable.ConstraintSet_android_scaleY)
			{
			  scaleY = a.getFloat(attr, scaleY);
			}
			else if (attr == R.styleable.ConstraintSet_android_transformPivotX)
			{
			  transformPivotX = a.getFloat(attr, transformPivotX);
			}
			else if (attr == R.styleable.ConstraintSet_android_transformPivotY)
			{
			  transformPivotY = a.getFloat(attr, transformPivotY);
			}
			else if (attr == R.styleable.ConstraintSet_android_translationX)
			{
			  translationX = a.getFloat(attr, translationX);
			}
			else if (attr == R.styleable.ConstraintSet_android_translationY)
			{
			  translationY = a.getFloat(attr, translationY);
			}
			else if (attr == R.styleable.ConstraintSet_android_translationZ)
			{
			  if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
			  {
				translationZ = a.getFloat(attr, translationZ);
			  }
			}
		  }
		  a.recycle();
		}
	  }

	  /// <summary>
	  /// @suppress
	  /// {@inheritDoc}
	  /// </summary>
	  protected internal override LayoutParams generateDefaultLayoutParams()
	  {
		return new LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
	  }

	  private void init(AttributeSet attrs)
	  {
		Log.v(TAG, " ################# init");
	  }

	  /// <summary>
	  /// {@inheritDoc}
	  /// </summary>
	  protected internal override ViewGroup.LayoutParams generateLayoutParams(ViewGroup.LayoutParams p)
	  {
		return new ConstraintLayout.LayoutParams(p);
	  }

	  public virtual ConstraintSet ConstraintSet
	  {
		  get
		  {
			if (myConstraintSet == null)
			{
			  myConstraintSet = new ConstraintSet();
			}
			// TODO -- could be more efficient...
			myConstraintSet.clone(this);
			return myConstraintSet;
		  }
	  }

	  protected internal override void onLayout(bool changed, int l, int t, int r, int b)
	  {
		// do nothing
	  }
	}

}