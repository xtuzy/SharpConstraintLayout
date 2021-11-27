using System;
using System.Collections.Generic;

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
namespace androidx.constraintlayout.core.motion
{
	using Easing = androidx.constraintlayout.core.motion.utils.Easing;
	using Rect = androidx.constraintlayout.core.motion.utils.Rect;
	using SplineSet = androidx.constraintlayout.core.motion.utils.SplineSet;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
	using Utils = androidx.constraintlayout.core.motion.utils.Utils;


	/// <summary>
	/// All the parameter it extracts from a ConstraintSet/View
	/// 
	/// @suppress
	/// </summary>
	internal class MotionConstrainedPoint : IComparable<MotionConstrainedPoint>
	{
		public const string TAG = "MotionPaths";
		public const bool DEBUG = false;

		private float alpha = 1;
		internal int mVisibilityMode = MotionWidget.VISIBILITY_MODE_NORMAL;
		internal int visibility;
		private bool applyElevation = false;
		private float elevation = 0;
		private float rotation = 0;
		private float rotationX = 0;
		public float rotationY = 0;
		private float scaleX = 1;
		private float scaleY = 1;
		private float mPivotX = float.NaN;
		private float mPivotY = float.NaN;
		private float translationX = 0;
		private float translationY = 0;
		private float translationZ = 0;
		private Easing mKeyFrameEasing;
		private int mDrawPath = 0;
		private float position;
		private float x;
		private float y;
		private float width;
		private float height;
		private float mPathRotate = float.NaN;
		private float mProgress = float.NaN;
		private int mAnimateRelativeTo = -1;

		internal const int PERPENDICULAR = 1;
		internal const int CARTESIAN = 2;
		internal static string[] names = new string[] {"position", "x", "y", "width", "height", "pathRotate"};

		internal LinkedHashMap<string, CustomVariable> mCustomVariable = new LinkedHashMap<string, CustomVariable>();
		internal int mMode = 0; // how was this point computed 1=perpendicular 2=deltaRelative

		public MotionConstrainedPoint()
		{

		}

		private bool diff(float a, float b)
		{
			if (float.IsNaN(a) || float.IsNaN(b))
			{
				return float.IsNaN(a) != float.IsNaN(b);
			}
			return Math.Abs(a - b) > 0.000001f;
		}

		/// <summary>
		/// Given the start and end points define Keys that need to be built
		/// </summary>
		/// <param name="points"> </param>
		/// <param name="keySet"> </param>
		internal virtual void different(MotionConstrainedPoint points, HashSet<string> keySet)
		{
			if (diff(alpha, points.alpha))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_ALPHA);
			}
			if (diff(elevation, points.elevation))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_TRANSLATION_Z);
			}
			if (visibility != points.visibility && mVisibilityMode == MotionWidget.VISIBILITY_MODE_NORMAL && (visibility == MotionWidget.VISIBLE || points.visibility == MotionWidget.VISIBLE))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_ALPHA);
			}
			if (diff(rotation, points.rotation))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_ROTATION_Z);
			}
			if (!(float.IsNaN(mPathRotate) && float.IsNaN(points.mPathRotate)))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_PATH_ROTATE);
			}
			if (!(float.IsNaN(mProgress) && float.IsNaN(points.mProgress)))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_PROGRESS);
			}
			if (diff(rotationX, points.rotationX))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_ROTATION_X);
			}
			if (diff(rotationY, points.rotationY))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_ROTATION_Y);
			}
			if (diff(mPivotX, points.mPivotX))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_PIVOT_X);
			}
			if (diff(mPivotY, points.mPivotY))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_PIVOT_Y);
			}
			if (diff(scaleX, points.scaleX))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_SCALE_X);
			}
			if (diff(scaleY, points.scaleY))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_SCALE_Y);
			}
			if (diff(translationX, points.translationX))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_TRANSLATION_X);
			}
			if (diff(translationY, points.translationY))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_TRANSLATION_Y);
			}
			if (diff(translationZ, points.translationZ))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_TRANSLATION_Z);
			}
			if (diff(elevation, points.elevation))
			{
				keySet.Add(TypedValues.TypedValues_Attributes.S_ELEVATION);
			}
		}

		internal virtual void different(MotionConstrainedPoint points, bool[] mask, string[] custom)
		{
			int c = 0;
			mask[c++] |= diff(position, points.position);
			mask[c++] |= diff(x, points.x);
			mask[c++] |= diff(y, points.y);
			mask[c++] |= diff(width, points.width);
			mask[c++] |= diff(height, points.height);
		}

		internal double[] mTempValue = new double[18];
		internal double[] mTempDelta = new double[18];

		internal virtual void fillStandard(double[] data, int[] toUse)
		{
			float[] set = new float[] {position, x, y, width, height, alpha, elevation, rotation, rotationX, rotationY, scaleX, scaleY, mPivotX, mPivotY, translationX, translationY, translationZ, mPathRotate};
			int c = 0;
			for (int i = 0; i < toUse.Length; i++)
			{
				if (toUse[i] < set.Length)
				{
					data[c++] = set[toUse[i]];
				}
			}
		}

		internal virtual bool hasCustomData(string name)
		{
			return mCustomVariable.containsKey(name);
		}

		internal virtual int getCustomDataCount(string name)
		{
			return mCustomVariable.get(name).numberOfInterpolatedValues();
		}

		internal virtual int getCustomData(string name, double[] value, int offset)
		{
			CustomVariable a = mCustomVariable.get(name);
			if (a.numberOfInterpolatedValues() == 1)
			{
				value[offset] = a.ValueToInterpolate;
				return 1;
			}
			else
			{
				int N = a.numberOfInterpolatedValues();
				float[] f = new float[N];
				a.getValuesToInterpolate(f);
				for (int i = 0; i < N; i++)
				{
					value[offset++] = f[i];
				}
				return N;
			}
		}

		internal virtual void setBounds(float x, float y, float w, float h)
		{
			this.x = x;
			this.y = y;
			width = w;
			height = h;
		}

		public virtual int CompareTo(MotionConstrainedPoint o)
		{
			return position.CompareTo(o.position);
		}

		public virtual void applyParameters(MotionWidget view)
		{

			this.visibility = view.Visibility;
			this.alpha = (view.Visibility != MotionWidget.VISIBLE) ? 0.0f : view.Alpha;
			this.applyElevation = false; // TODO figure a way to cache parameters

			this.rotation = view.RotationZ;
			this.rotationX = view.RotationX;
			this.rotationY = view.RotationY;
			this.scaleX = view.ScaleX;
			this.scaleY = view.ScaleY;
			this.mPivotX = view.PivotX;
			this.mPivotY = view.PivotY;
			this.translationX = view.TranslationX;
			this.translationY = view.TranslationY;
			this.translationZ = view.TranslationZ;
			ICollection<string> at = view.CustomAttributeNames;
			foreach (string s in at)
			{
				CustomVariable attr = view.getCustomAttribute(s);
				if (attr != null && attr.Continuous)
				{
					this.mCustomVariable.put(s, attr);
				}
			}


		}

		public virtual void addValues(Dictionary<string, SplineSet> splines, int mFramePosition)
		{
			foreach (string s in splines.Keys)
			{
				SplineSet ViewSpline = splines[s];
				if (DEBUG)
				{
					Utils.log(TAG, "setPoint" + mFramePosition + "  spline set = " + s);
				}
				switch (s)
				{
					case TypedValues.TypedValues_Attributes.S_ALPHA:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(alpha) ? 1 : alpha);
						break;
					case TypedValues.TypedValues_Attributes.S_ROTATION_Z:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(rotation) ? 0 : rotation);
						break;
					case TypedValues.TypedValues_Attributes.S_ROTATION_X:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(rotationX) ? 0 : rotationX);
						break;
					case TypedValues.TypedValues_Attributes.S_ROTATION_Y:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(rotationY) ? 0 : rotationY);
						break;
					case TypedValues.TypedValues_Attributes.S_PIVOT_X:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(mPivotX) ? 0 : mPivotX);
						break;
					case TypedValues.TypedValues_Attributes.S_PIVOT_Y:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(mPivotY) ? 0 : mPivotY);
						break;
					case TypedValues.TypedValues_Attributes.S_PATH_ROTATE:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(mPathRotate) ? 0 : mPathRotate);
						break;
					case TypedValues.TypedValues_Attributes.S_PROGRESS:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(mProgress) ? 0 : mProgress);
						break;
					case TypedValues.TypedValues_Attributes.S_SCALE_X:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(scaleX) ? 1 : scaleX);
						break;
					case TypedValues.TypedValues_Attributes.S_SCALE_Y:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(scaleY) ? 1 : scaleY);
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_X:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(translationX) ? 0 : translationX);
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_Y:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(translationY) ? 0 : translationY);
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_Z:
						ViewSpline.setPoint(mFramePosition, float.IsNaN(translationZ) ? 0 : translationZ);
						break;
					default:
						if (s.StartsWith("CUSTOM", StringComparison.Ordinal))
						{
							string customName = s.Split(",", true)[1];
							if (mCustomVariable.containsKey(customName))
							{
								CustomVariable custom = mCustomVariable.get(customName);
								if (ViewSpline is SplineSet.CustomSpline)
								{
									((SplineSet.CustomSpline) ViewSpline).setPoint(mFramePosition, custom);
								}
								else
								{
									Utils.loge(TAG, s + " ViewSpline not a CustomSet frame = " + mFramePosition + ", value" + custom.ValueToInterpolate + ViewSpline);

								}

							}
						}
						else
						{
							Utils.loge(TAG, "UNKNOWN spline " + s);
						}
					break;
				}
			}

		}

		public virtual MotionWidget State
		{
			set
			{
				setBounds(value.X, value.Y, value.Width, value.Height);
				applyParameters(value);
			}
		}

		/// <param name="rect">     assumes pre rotated </param>
		/// <param name="view"> </param>
		/// <param name="rotation"> mode Surface.ROTATION_0,Surface.ROTATION_90... </param>
		public virtual void setState(Rect rect, MotionWidget view, int rotation, float prevous)
		{
			setBounds(rect.left, rect.top, rect.width(), rect.height());
			applyParameters(view);
			mPivotX = float.NaN;
			mPivotY = float.NaN;

			switch (rotation)
			{
				case MotionWidget.ROTATE_PORTRATE_OF_LEFT:
					this.rotation = prevous + 90;
					break;
				case MotionWidget.ROTATE_PORTRATE_OF_RIGHT:
					this.rotation = prevous - 90;
					break;
			}
		}

	//   TODO support Screen Rotation
	//    /**
	//     * Sets the state of the position given a rect, constraintset, rotation and viewid
	//     *
	//     * @param cw
	//     * @param constraintSet
	//     * @param rotation
	//     * @param viewId
	//     */
	//    public void setState(Rect cw, ConstraintSet constraintSet, int rotation, int viewId) {
	//        setBounds(cw.left, cw.top, cw.width(), cw.height());
	//        applyParameters(constraintSet.getParameters(viewId));
	//        switch (rotation) {
	//            case ConstraintSet.ROTATE_PORTRATE_OF_RIGHT:
	//            case ConstraintSet.ROTATE_RIGHT_OF_PORTRATE:
	//                this.rotation -= 90;
	//                break;
	//            case ConstraintSet.ROTATE_PORTRATE_OF_LEFT:
	//            case ConstraintSet.ROTATE_LEFT_OF_PORTRATE:
	//                this.rotation += 90;
	//                if (this.rotation > 180) this.rotation -= 360;
	//                break;
	//        }
	//    }
	}

}