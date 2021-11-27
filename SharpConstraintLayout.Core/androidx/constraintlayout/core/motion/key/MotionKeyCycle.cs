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
	using KeyCycleOscillator = androidx.constraintlayout.core.motion.utils.KeyCycleOscillator;
	using Oscillator = androidx.constraintlayout.core.motion.utils.Oscillator;
	using SplineSet = androidx.constraintlayout.core.motion.utils.SplineSet;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
	using Utils = androidx.constraintlayout.core.motion.utils.Utils;


	public class MotionKeyCycle : MotionKey
	{
		private bool InstanceFieldsInitialized = false;

		public MotionKeyCycle()
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

		private const string TAG = "KeyCycle";
		internal const string NAME = "KeyCycle";
		public const string WAVE_PERIOD = "wavePeriod";
		public const string WAVE_OFFSET = "waveOffset";
		public const string WAVE_PHASE = "wavePhase";
		public const string WAVE_SHAPE = "waveShape";
		public const int SHAPE_SIN_WAVE = Oscillator.SIN_WAVE;
		public const int SHAPE_SQUARE_WAVE = Oscillator.SQUARE_WAVE;
		public const int SHAPE_TRIANGLE_WAVE = Oscillator.TRIANGLE_WAVE;
		public const int SHAPE_SAW_WAVE = Oscillator.SAW_WAVE;
		public const int SHAPE_REVERSE_SAW_WAVE = Oscillator.REVERSE_SAW_WAVE;
		public const int SHAPE_COS_WAVE = Oscillator.COS_WAVE;
		public const int SHAPE_BOUNCE = Oscillator.BOUNCE;

		private string mTransitionEasing = null;
		private int mCurveFit = 0;
		private int mWaveShape = -1;
		private string mCustomWaveShape = null;
		private float mWavePeriod = float.NaN;
		private float mWaveOffset = 0;
		private float mWavePhase = 0;
		private float mProgress = float.NaN;
		private float mAlpha = float.NaN;
		private float mElevation = float.NaN;
		private float mRotation = float.NaN;
		private float mTransitionPathRotate = float.NaN;
		private float mRotationX = float.NaN;
		private float mRotationY = float.NaN;
		private float mScaleX = float.NaN;
		private float mScaleY = float.NaN;
		private float mTranslationX = float.NaN;
		private float mTranslationY = float.NaN;
		private float mTranslationZ = float.NaN;
		public const int KEY_TYPE = 4;

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

		public override void addValues(Dictionary<string, SplineSet> splines)
		{

		}

		public override bool setValue(int type, int value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Cycle.TYPE_CURVE_FIT:
					mCurveFit = value;
					return true;
				case TypedValues.TypedValues_Cycle.TYPE_WAVE_SHAPE:
					mWaveShape = value;
					return true;
				default:
					bool ret = setValue(type, (float) value);
					if (ret)
					{
						return true;
					}
					return base.setValue(type, value);
			}
		}

		public override bool setValue(int type, string value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Cycle.TYPE_EASING:
					mTransitionEasing = value;
					return true;
				case TypedValues.TypedValues_Cycle.TYPE_CUSTOM_WAVE_SHAPE:
					mCustomWaveShape = value;
					return true;
				default:
					return base.setValue(type, value);
			}

		}

		public override bool setValue(int type, float value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Cycle.TYPE_ALPHA:
					mAlpha = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_X:
					mTranslationX = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_Y:
					mTranslationY = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_Z:
					mTranslationZ = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_ELEVATION:
					mElevation = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_ROTATION_X:
					mRotationX = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_ROTATION_Y:
					mRotationY = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_ROTATION_Z:
					mRotation = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_SCALE_X:
					mScaleX = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_SCALE_Y:
					mScaleY = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_PROGRESS:
					mProgress = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_PATH_ROTATE:
					mTransitionPathRotate = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_WAVE_PERIOD:
					mWavePeriod = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_WAVE_OFFSET:
					mWaveOffset = value;
					break;
				case TypedValues.TypedValues_Cycle.TYPE_WAVE_PHASE:
					mWavePhase = value;
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}


		public virtual float getValue(string key)
		{
			switch (key)
			{
				case TypedValues.TypedValues_Cycle.S_ALPHA:
					return mAlpha;
				case TypedValues.TypedValues_Cycle.S_ELEVATION:
					return mElevation;
				case TypedValues.TypedValues_Cycle.S_ROTATION_Z:
					return mRotation;
				case TypedValues.TypedValues_Cycle.S_ROTATION_X:
					return mRotationX;
				case TypedValues.TypedValues_Cycle.S_ROTATION_Y:
					return mRotationY;
				case TypedValues.TypedValues_Cycle.S_PATH_ROTATE:
					return mTransitionPathRotate;
				case TypedValues.TypedValues_Cycle.S_SCALE_X:
					return mScaleX;
				case TypedValues.TypedValues_Cycle.S_SCALE_Y:
					return mScaleY;
				case TypedValues.TypedValues_Cycle.S_TRANSLATION_X:
					return mTranslationX;
				case TypedValues.TypedValues_Cycle.S_TRANSLATION_Y:
					return mTranslationY;
				case TypedValues.TypedValues_Cycle.S_TRANSLATION_Z:
					return mTranslationZ;
				case TypedValues.TypedValues_Cycle.S_WAVE_OFFSET:
					return mWaveOffset;
				case TypedValues.TypedValues_Cycle.S_WAVE_PHASE:
					return mWavePhase;
				case TypedValues.TypedValues_Cycle.S_PROGRESS:
					return mProgress;
				default:
					return float.NaN;
			}
		}

		public override MotionKey clone()
		{
			return null;
		}

		public override int getId(string name)
		{
			switch (name)
			{
				case TypedValues.TypedValues_Cycle.S_CURVE_FIT:
					return TypedValues.TypedValues_Cycle.TYPE_CURVE_FIT;
				case TypedValues.TypedValues_Cycle.S_VISIBILITY:
					return TypedValues.TypedValues_Cycle.TYPE_VISIBILITY;
				case TypedValues.TypedValues_Cycle.S_ALPHA:
					return TypedValues.TypedValues_Cycle.TYPE_ALPHA;
				case TypedValues.TypedValues_Cycle.S_TRANSLATION_X:
					return TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_X;
				case TypedValues.TypedValues_Cycle.S_TRANSLATION_Y:
					return TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_Y;
				case TypedValues.TypedValues_Cycle.S_TRANSLATION_Z:
					return TypedValues.TypedValues_Cycle.TYPE_TRANSLATION_Z;
				case TypedValues.TypedValues_Cycle.S_ROTATION_X:
					return TypedValues.TypedValues_Cycle.TYPE_ROTATION_X;
				case TypedValues.TypedValues_Cycle.S_ROTATION_Y:
					return TypedValues.TypedValues_Cycle.TYPE_ROTATION_Y;
				case TypedValues.TypedValues_Cycle.S_ROTATION_Z:
					return TypedValues.TypedValues_Cycle.TYPE_ROTATION_Z;
				case TypedValues.TypedValues_Cycle.S_SCALE_X:
					return TypedValues.TypedValues_Cycle.TYPE_SCALE_X;
				case TypedValues.TypedValues_Cycle.S_SCALE_Y:
					return TypedValues.TypedValues_Cycle.TYPE_SCALE_Y;
				case TypedValues.TypedValues_Cycle.S_PIVOT_X:
					return TypedValues.TypedValues_Cycle.TYPE_PIVOT_X;
				case TypedValues.TypedValues_Cycle.S_PIVOT_Y:
					return TypedValues.TypedValues_Cycle.TYPE_PIVOT_Y;
				case TypedValues.TypedValues_Cycle.S_PROGRESS:
					return TypedValues.TypedValues_Cycle.TYPE_PROGRESS;
				case TypedValues.TypedValues_Cycle.S_PATH_ROTATE:
					return TypedValues.TypedValues_Cycle.TYPE_PATH_ROTATE;
				case TypedValues.TypedValues_Cycle.S_EASING:
					return TypedValues.TypedValues_Cycle.TYPE_EASING;
				case TypedValues.TypedValues_Cycle.S_WAVE_PERIOD:
					return TypedValues.TypedValues_Cycle.TYPE_WAVE_PERIOD;
				case TypedValues.TypedValues_Cycle.S_WAVE_SHAPE:
					return TypedValues.TypedValues_Cycle.TYPE_WAVE_SHAPE;
				case TypedValues.TypedValues_Cycle.S_WAVE_PHASE:
					return TypedValues.TypedValues_Cycle.TYPE_WAVE_PHASE;
				case TypedValues.TypedValues_Cycle.S_WAVE_OFFSET:
					return TypedValues.TypedValues_Cycle.TYPE_WAVE_OFFSET;
				case TypedValues.TypedValues_Cycle.S_CUSTOM_WAVE_SHAPE:
					return TypedValues.TypedValues_Cycle.TYPE_CUSTOM_WAVE_SHAPE;

			}
			return -1;
		}

		public virtual void addCycleValues(Dictionary<string, KeyCycleOscillator> oscSet)
		{

			foreach (string key in oscSet.Keys)
			{
				if (key.StartsWith(TypedValues.S_CUSTOM, StringComparison.Ordinal))
				{
					string customKey = key.Substring(TypedValues.S_CUSTOM.Length + 1);
					CustomVariable cValue = mCustom[customKey];
					if (cValue == null || cValue.Type != TypedValues_Custom.TYPE_FLOAT)
					{
						continue;
					}

					KeyCycleOscillator temposc = oscSet[key];
					if (temposc == null)
					{
						continue;
					}

					temposc.setPoint(mFramePosition, mWaveShape, mCustomWaveShape, -1, mWavePeriod, mWaveOffset, mWavePhase, cValue.ValueToInterpolate, cValue);
					continue;
				}
				float value = getValue(key);
				if (float.IsNaN(value))
				{
					continue;
				}

				KeyCycleOscillator osc = oscSet[key];
				if (osc == null)
				{
					continue;
				}

				osc.setPoint(mFramePosition, mWaveShape, mCustomWaveShape, -1, mWavePeriod, mWaveOffset, mWavePhase, value);
			}
		}



		public virtual void dump()
		{
			Console.WriteLine("MotionKeyCycle{" + "mWaveShape=" + mWaveShape + ", mWavePeriod=" + mWavePeriod + ", mWaveOffset=" + mWaveOffset + ", mWavePhase=" + mWavePhase + ", mRotation=" + mRotation + '}');
		}

		public virtual void printAttributes()
		{
			HashSet<string> nameSet = new HashSet<string>();
			getAttributeNames(nameSet);

			Utils.log(" ------------- " + mFramePosition + " -------------");
			Utils.log("MotionKeyCycle{" + "Shape=" + mWaveShape + ", Period=" + mWavePeriod + ", Offset=" + mWaveOffset + ", Phase=" + mWavePhase + '}');
			//string[] names = nameSet.ToArray(new string[0]);
			string[] names = nameSet.ToArray();
			for (int i = 0; i < names.Length; i++)
			{
				int id = TypedValues.TypedValues_Attributes.getId(names[i]);
				Utils.log(names[i] + ":" + getValue(names[i]));
			}
		}


	}

}