using System;
using System.Collections.Generic;
using System.Linq;

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
namespace androidx.constraintlayout.core.motion.key
{
	using SplineSet = androidx.constraintlayout.core.motion.utils.SplineSet;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
	using Utils = androidx.constraintlayout.core.motion.utils.Utils;


	public class MotionKeyAttributes : MotionKey
	{
		private bool InstanceFieldsInitialized = false;

		public MotionKeyAttributes()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			mType = KEY_TYPE;
			mCustom = new Dictionary<string,CustomVariable>();
		}

		internal const string NAME = "KeyAttribute";
		private const string TAG = "KeyAttributes";
		private const bool DEBUG = false;
		private string mTransitionEasing;
		private int mCurveFit = -1;
		private int mVisibility = 0;
		private float mAlpha = float.NaN;
		private float mElevation = float.NaN;
		private float mRotation = float.NaN;
		private float mRotationX = float.NaN;
		private float mRotationY = float.NaN;
		private float mPivotX = float.NaN;
		private float mPivotY = float.NaN;
		private float mTransitionPathRotate = float.NaN;
		private float mScaleX = float.NaN;
		private float mScaleY = float.NaN;
		private float mTranslationX = float.NaN;
		private float mTranslationY = float.NaN;
		private float mTranslationZ = float.NaN;
		private float mProgress = float.NaN;

		public const int KEY_TYPE = 1;

		public override void getAttributeNames(HashSet<string> attributes)
		{

			if (!float.IsNaN(mAlpha))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_ALPHA);
			}
			if (!float.IsNaN(mElevation))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_ELEVATION);
			}
			if (!float.IsNaN(mRotation))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_ROTATION_Z);
			}
			if (!float.IsNaN(mRotationX))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_ROTATION_X);
			}
			if (!float.IsNaN(mRotationY))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_ROTATION_Y);
			}
			if (!float.IsNaN(mPivotX))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_PIVOT_X);
			}
			if (!float.IsNaN(mPivotY))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_PIVOT_Y);
			}
			if (!float.IsNaN(mTranslationX))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_TRANSLATION_X);
			}
			if (!float.IsNaN(mTranslationY))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_TRANSLATION_Y);
			}
			if (!float.IsNaN(mTranslationZ))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_TRANSLATION_Z);
			}
			if (!float.IsNaN(mTransitionPathRotate))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_PATH_ROTATE);
			}
			if (!float.IsNaN(mScaleX))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_SCALE_X);
			}
			if (!float.IsNaN(mScaleY))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_SCALE_Y);
			}
			if (!float.IsNaN(mProgress))
			{
				attributes.Add(TypedValues.TypedValues_Attributes.S_PROGRESS);
			}
			if (mCustom.Count > 0)
			{
				foreach (string s in mCustom.Keys)
				{
					attributes.Add(TypedValues.S_CUSTOM + "," + s);
				}
			}
		}

		public override void addValues(Dictionary<string, SplineSet> splines)
		{
			foreach (string s in splines.Keys)
			{
				SplineSet splineSet = splines[s];
				if (splineSet == null)
				{
					continue;
				}
				// TODO support custom
				if (s.StartsWith(TypedValues.TypedValues_Attributes.S_CUSTOM, StringComparison.Ordinal))
				{
					string cKey = s.Substring(TypedValues.TypedValues_Attributes.S_CUSTOM.Length + 1);
					CustomVariable cValue = mCustom[cKey];
					if (cValue != null)
					{
						((SplineSet.CustomSpline)splineSet).setPoint(mFramePosition, cValue);
					}
					continue;
				}
				switch (s)
				{
					case TypedValues.TypedValues_Attributes.S_ALPHA:
						if (!float.IsNaN(mAlpha))
						{
							splineSet.setPoint(mFramePosition, mAlpha);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_ELEVATION:
						if (!float.IsNaN(mElevation))
						{
							splineSet.setPoint(mFramePosition, mElevation);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_ROTATION_Z:
						if (!float.IsNaN(mRotation))
						{
							splineSet.setPoint(mFramePosition, mRotation);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_ROTATION_X:
						if (!float.IsNaN(mRotationX))
						{
							splineSet.setPoint(mFramePosition, mRotationX);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_ROTATION_Y:
						if (!float.IsNaN(mRotationY))
						{
							splineSet.setPoint(mFramePosition, mRotationY);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_PIVOT_X:
						if (!float.IsNaN(mRotationX))
						{
							splineSet.setPoint(mFramePosition, mPivotX);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_PIVOT_Y:
						if (!float.IsNaN(mRotationY))
						{
							splineSet.setPoint(mFramePosition, mPivotY);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_PATH_ROTATE:
						if (!float.IsNaN(mTransitionPathRotate))
						{
							splineSet.setPoint(mFramePosition, mTransitionPathRotate);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_SCALE_X:
						if (!float.IsNaN(mScaleX))
						{
							splineSet.setPoint(mFramePosition, mScaleX);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_SCALE_Y:
						if (!float.IsNaN(mScaleY))
						{
							splineSet.setPoint(mFramePosition, mScaleY);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_X:
						if (!float.IsNaN(mTranslationX))
						{
							splineSet.setPoint(mFramePosition, mTranslationX);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_Y:
						if (!float.IsNaN(mTranslationY))
						{
							splineSet.setPoint(mFramePosition, mTranslationY);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_Z:
						if (!float.IsNaN(mTranslationZ))
						{
							splineSet.setPoint(mFramePosition, mTranslationZ);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_PROGRESS:
						if (!float.IsNaN(mProgress))
						{
							splineSet.setPoint(mFramePosition, mProgress);
						}
						break;
					default:
						//Console.Error.WriteLine("not supported by KeyAttributes " + s);
						Console.Fail("not supported by KeyAttributes " + s);
					break;
				}
			}
		}

		public override MotionKey clone()
		{
			return null;
		}

		public override bool setValue(int type, int value)
		{

			switch (type)
			{
				case TypedValues.TypedValues_Attributes.TYPE_VISIBILITY:
					mVisibility = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_CURVE_FIT:
					mCurveFit = value;
					break;
				case TypedValues.TYPE_FRAME_POSITION:
					mFramePosition = value;
					break;
				default:
					if (!setValue(type, value))
					{
						return base.setValue(type, value);
					}
					break;
			}
			return true;
		}

		public override bool setValue(int type, float value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Attributes.TYPE_ALPHA:
					mAlpha = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_X:
					mTranslationX = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_Y:
					mTranslationY = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_Z:
					mTranslationZ = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_ELEVATION:
					mElevation = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_X:
					mRotationX = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_Y:
					mRotationY = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_Z:
					mRotation = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_SCALE_X:
					mScaleX = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_SCALE_Y:
					mScaleY = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_PIVOT_X:
					mPivotX = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_PIVOT_Y:
					mPivotY = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_PROGRESS:
					mProgress = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_PATH_ROTATE:
					mTransitionPathRotate = value;
					break;
				case TypedValues.TYPE_FRAME_POSITION:
					mTransitionPathRotate = value;
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}

		public override Dictionary<string, int?> Interpolation
		{
			set
			{
				if (!float.IsNaN(mAlpha))
				{
					value[TypedValues.TypedValues_Attributes.S_ALPHA] = mCurveFit;
				}
				if (!float.IsNaN(mElevation))
				{
					value[TypedValues.TypedValues_Attributes.S_ELEVATION] = mCurveFit;
				}
				if (!float.IsNaN(mRotation))
				{
					value[TypedValues.TypedValues_Attributes.S_ROTATION_Z] = mCurveFit;
				}
				if (!float.IsNaN(mRotationX))
				{
					value[TypedValues.TypedValues_Attributes.S_ROTATION_X] = mCurveFit;
				}
				if (!float.IsNaN(mRotationY))
				{
					value[TypedValues.TypedValues_Attributes.S_ROTATION_Y] = mCurveFit;
				}
				if (!float.IsNaN(mPivotX))
				{
					value[TypedValues.TypedValues_Attributes.S_PIVOT_X] = mCurveFit;
				}
				if (!float.IsNaN(mPivotY))
				{
					value[TypedValues.TypedValues_Attributes.S_PIVOT_Y] = mCurveFit;
				}
				if (!float.IsNaN(mTranslationX))
				{
					value[TypedValues.TypedValues_Attributes.S_TRANSLATION_X] = mCurveFit;
				}
				if (!float.IsNaN(mTranslationY))
				{
					value[TypedValues.TypedValues_Attributes.S_TRANSLATION_Y] = mCurveFit;
				}
				if (!float.IsNaN(mTranslationZ))
				{
					value[TypedValues.TypedValues_Attributes.S_TRANSLATION_Z] = mCurveFit;
				}
				if (!float.IsNaN(mTransitionPathRotate))
				{
					value[TypedValues.TypedValues_Attributes.S_PATH_ROTATE] = mCurveFit;
				}
				if (!float.IsNaN(mScaleX))
				{
					value[TypedValues.TypedValues_Attributes.S_SCALE_X] = mCurveFit;
				}
				if (!float.IsNaN(mScaleY))
				{
					value[TypedValues.TypedValues_Attributes.S_SCALE_Y] = mCurveFit;
				}
				if (!float.IsNaN(mProgress))
				{
					value[TypedValues.TypedValues_Attributes.S_PROGRESS] = mCurveFit;
				}
				if (mCustom.Count > 0)
				{
					foreach (string s in mCustom.Keys)
					{
						value[TypedValues.TypedValues_Attributes.S_CUSTOM + "," + s] = mCurveFit;
					}
				}
			}
		}

		public override bool setValue(int type, string value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Attributes.TYPE_EASING:
					mTransitionEasing = value;
					break;

				case TypedValues.TYPE_TARGET:
					mTargetString = value;
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}

		public override int getId(string name)
		{
			return TypedValues.TypedValues_Attributes.getId(name);
		}

		public virtual int CurveFit
		{
			get
			{
				return mCurveFit;
			}
		}

		public virtual void printAttributes()
		{
			HashSet<string> nameSet = new HashSet<string>();
			getAttributeNames(nameSet);

			Console.WriteLine(" ------------- " + mFramePosition + " -------------");
			//string[] names = nameSet.toArray(new string[0]);
			string[] names = nameSet.ToArray();
			for (int i = 0; i < names.Length; i++)
			{
				int id = TypedValues.TypedValues_Attributes.getId(names[i]);
				Console.WriteLine(names[i] + ":" + getfloatValue(id));
			}
		}

		private float getfloatValue(int id)
		{
			switch (id)
			{
				case TypedValues.TypedValues_Attributes.TYPE_ALPHA:
					return mAlpha;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_X:
					return mTranslationX;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_Y:
					return mTranslationY;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_Z:
					return mTranslationZ;
				case TypedValues.TypedValues_Attributes.TYPE_ELEVATION:
					return mElevation;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_X:
					return mRotationX;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_Y:
					return mRotationY;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_Z:
					return mRotation;
				case TypedValues.TypedValues_Attributes.TYPE_SCALE_X:
					return mScaleX;
				case TypedValues.TypedValues_Attributes.TYPE_SCALE_Y:
					return mScaleY;
				case TypedValues.TypedValues_Attributes.TYPE_PIVOT_X:
					return mPivotX;
				case TypedValues.TypedValues_Attributes.TYPE_PIVOT_Y:
					return mPivotY;
				case TypedValues.TypedValues_Attributes.TYPE_PROGRESS:
					return mProgress;
				case TypedValues.TypedValues_Attributes.TYPE_PATH_ROTATE:
					return mTransitionPathRotate;
				case TypedValues.TYPE_FRAME_POSITION:
					return mFramePosition;
				default:
					return float.NaN;
			}
		}
	}

}