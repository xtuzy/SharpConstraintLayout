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
namespace androidx.constraintlayout.core.motion.key
{
	using Oscillator = androidx.constraintlayout.core.motion.utils.Oscillator;
	using SplineSet = androidx.constraintlayout.core.motion.utils.SplineSet;
	using TimeCycleSplineSet = androidx.constraintlayout.core.motion.utils.TimeCycleSplineSet;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
	using Utils = androidx.constraintlayout.core.motion.utils.Utils;


	public class MotionKeyTimeCycle : MotionKey
	{
		private bool InstanceFieldsInitialized = false;

		public MotionKeyTimeCycle()
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

		internal const string NAME = "KeyTimeCycle";
		private const string TAG = NAME;

		private string mTransitionEasing;
		private int mCurveFit = -1;
		private float mAlpha = float.NaN;
		private float mElevation = float.NaN;
		private float mRotation = float.NaN;
		private float mRotationX = float.NaN;
		private float mRotationY = float.NaN;
		private float mTransitionPathRotate = float.NaN;
		private float mScaleX = float.NaN;
		private float mScaleY = float.NaN;
		private float mTranslationX = float.NaN;
		private float mTranslationY = float.NaN;
		private float mTranslationZ = float.NaN;
		private float mProgress = float.NaN;
		private int mWaveShape = 0;
		private string mCustomWaveShape = null; // TODO add support of custom wave shapes in KeyTimeCycle
		private float mWavePeriod = float.NaN;
		private float mWaveOffset = 0;
		public const int KEY_TYPE = 3;

		public virtual void addTimeValues(Dictionary<string, TimeCycleSplineSet> splines)
		{
			foreach (string s in splines.Keys)
			{
				TimeCycleSplineSet splineSet = splines[s];
				if (splineSet == null)
				{
					continue;
				}
				if (s.StartsWith(CUSTOM, StringComparison.Ordinal))
				{
					string cKey = s.Substring(CUSTOM.Length + 1);
					CustomVariable cValue = mCustom[cKey];
					if (cValue != null)
					{
						((TimeCycleSplineSet.CustomVarSet) splineSet).setPoint(mFramePosition, cValue, mWavePeriod, mWaveShape, mWaveOffset);
					}
					continue;
				}
				switch (s)
				{
					case TypedValues.TypedValues_Attributes.S_ALPHA:
						if (!float.IsNaN(mAlpha))
						{
							splineSet.setPoint(mFramePosition, mAlpha, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;

					case TypedValues.TypedValues_Attributes.S_ROTATION_X:
						if (!float.IsNaN(mRotationX))
						{
							splineSet.setPoint(mFramePosition, mRotationX, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_ROTATION_Y:
						if (!float.IsNaN(mRotationY))
						{
							splineSet.setPoint(mFramePosition, mRotationY, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_ROTATION_Z:
						if (!float.IsNaN(mRotation))
						{
							splineSet.setPoint(mFramePosition, mRotation, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_PATH_ROTATE:
						if (!float.IsNaN(mTransitionPathRotate))
						{
							splineSet.setPoint(mFramePosition, mTransitionPathRotate, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;

					case TypedValues.TypedValues_Attributes.S_SCALE_X:
						if (!float.IsNaN(mScaleX))
						{
							splineSet.setPoint(mFramePosition, mScaleX, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_SCALE_Y:
						if (!float.IsNaN(mScaleY))
						{
							splineSet.setPoint(mFramePosition, mScaleY, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_X:
						if (!float.IsNaN(mTranslationX))
						{
							splineSet.setPoint(mFramePosition, mTranslationX, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_Y:
						if (!float.IsNaN(mTranslationY))
						{
							splineSet.setPoint(mFramePosition, mTranslationY, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_TRANSLATION_Z:
						if (!float.IsNaN(mTranslationZ))
						{
							splineSet.setPoint(mFramePosition, mTranslationZ, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_ELEVATION:
						if (!float.IsNaN(mTranslationZ))
						{
							splineSet.setPoint(mFramePosition, mTranslationZ, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					case TypedValues.TypedValues_Attributes.S_PROGRESS:
						if (!float.IsNaN(mProgress))
						{
							splineSet.setPoint(mFramePosition, mProgress, mWavePeriod, mWaveShape, mWaveOffset);
						}
						break;
					default:
						Utils.loge("KeyTimeCycles", "UNKNOWN addValues \"" + s + "\"");
					break;
				}
			}
		}

		public override void addValues(Dictionary<string, SplineSet> splines)
		{
		}

		public override bool setValue(int type, int value)
		{

			switch (type)
			{
				case TypedValues.TYPE_FRAME_POSITION:
					mFramePosition = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_WAVE_SHAPE:
					mWaveShape = value;
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}

		public override bool setValue(int type, float value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Cycle.TYPE_ALPHA:
					mAlpha = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_CURVE_FIT:
					mCurveFit = toInt(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_ELEVATION:
					mElevation = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_PROGRESS:
					mProgress = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_ROTATION_Z:
					mRotation = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_ROTATION_X:
					mRotationX = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_ROTATION_Y:
					mRotationY = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_SCALE_X:
					mScaleX = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_SCALE_Y:
					mScaleY = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_PATH_ROTATE:
					mTransitionPathRotate = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_X:
					mTranslationX = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_Y:
					mTranslationY = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_Z:
					mTranslationZ = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_WAVE_PERIOD:
					mWavePeriod = toFloat(value);
					break;
				case TypedValues.TypedValues_Cycle.TYPE_WAVE_OFFSET:
					mWaveOffset = toFloat(value);
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}

		public override bool setValue(int type, string value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Cycle.TYPE_WAVE_SHAPE:
					mWaveShape = Oscillator.CUSTOM;
					mCustomWaveShape = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_EASING:
					mTransitionEasing = value;
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}

		public override bool setValue(int type, bool value)
		{
			return base.setValue(type, value);
		}

		public override MotionKey copy(MotionKey src)
		{
			base.copy(src);
			MotionKeyTimeCycle k = (MotionKeyTimeCycle) src;
			mTransitionEasing = k.mTransitionEasing;
			mCurveFit = k.mCurveFit;
			mWaveShape = k.mWaveShape;
			mWavePeriod = k.mWavePeriod;
			mWaveOffset = k.mWaveOffset;
			mProgress = k.mProgress;
			mAlpha = k.mAlpha;
			mElevation = k.mElevation;
			mRotation = k.mRotation;
			mTransitionPathRotate = k.mTransitionPathRotate;
			mRotationX = k.mRotationX;
			mRotationY = k.mRotationY;
			mScaleX = k.mScaleX;
			mScaleY = k.mScaleY;
			mTranslationX = k.mTranslationX;
			mTranslationY = k.mTranslationY;
			mTranslationZ = k.mTranslationZ;
			return this;
		}

		public override void getAttributeNames(HashSet<string> attributes)
		{
			if (!float.IsNaN(mAlpha))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_ALPHA);
			}
			if (!float.IsNaN(mElevation))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_ELEVATION);
			}
			if (!float.IsNaN(mRotation))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_ROTATION_Z);
			}
			if (!float.IsNaN(mRotationX))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_ROTATION_X);
			}
			if (!float.IsNaN(mRotationY))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_ROTATION_Y);
			}
			if (!float.IsNaN(mScaleX))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_SCALE_X);
			}
			if (!float.IsNaN(mScaleY))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_SCALE_Y);
			}
			if (!float.IsNaN(mTransitionPathRotate))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_PATH_ROTATE);
			}
			if (!float.IsNaN(mTranslationX))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_TRANSLATION_X);
			}
			if (!float.IsNaN(mTranslationY))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_TRANSLATION_Y);
			}
			if (!float.IsNaN(mTranslationZ))
			{
				attributes.Add(TypedValues.TypedValues_Cycle.S_TRANSLATION_Z);
			}
			if (mCustom.Count > 0)
			{
				foreach (string s in mCustom.Keys)
				{
					attributes.Add(TypedValues.S_CUSTOM + "," + s);
				}
			}
		}

		public override MotionKey clone()
		{
			return (new MotionKeyTimeCycle()).copy(this);
		}

		public override int getId(string name)
		{
			return TypedValues.TypedValues_Cycle.getId(name);
		}
	}

}