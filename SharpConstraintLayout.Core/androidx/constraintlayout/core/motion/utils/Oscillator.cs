using System;

/*
 * Copyright (C) 2020 The Android Open Source Project
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
namespace androidx.constraintlayout.core.motion.utils
{

	/// <summary>
	/// This generates variable frequency oscillation curves
	/// 
	/// @suppress
	/// </summary>
	public class Oscillator
	{
		public static string TAG = "Oscillator";
		internal float[] mPeriod = new float[] {};
		internal double[] mPosition = new double[] {};
		internal double[] mArea;
		public const int SIN_WAVE = 0; // theses must line up with attributes
		public const int SQUARE_WAVE = 1;
		public const int TRIANGLE_WAVE = 2;
		public const int SAW_WAVE = 3;
		public const int REVERSE_SAW_WAVE = 4;
		public const int COS_WAVE = 5;
		public const int BOUNCE = 6;
		public const int CUSTOM = 7;
		internal string mCustomType;
		internal MonotonicCurveFit mCustomCurve;
		internal int mType;
		internal double PI2 = Math.PI * 2;
		private bool mNormalized = false;

		public Oscillator()
		{
		}

		public override string ToString()
		{
			//return "pos =" + Arrays.ToString(mPosition) + " period=" + Arrays.ToString(mPeriod);
			return "pos =" + mPosition.ToString() + " period=" + mPeriod.ToString();
		}

		public virtual void setType(int type, string customType)
		{
			mType = type;
			mCustomType = customType;
			if (!string.ReferenceEquals(mCustomType, null))
			{
				mCustomCurve = MonotonicCurveFit.buildWave(customType);
			}
		}

		public virtual void addPoint(double position, float period)
		{
			int len = mPeriod.Length + 1;
			//int j = Arrays.binarySearch(mPosition, position);
			int j = Array.BinarySearch(mPosition,position);
			if (j < 0)
			{
				j = -j - 1;
			}
			//mPosition = Arrays.copyOf(mPosition, len);
			mPosition = mPosition.Copy(len);
			//mPeriod = Arrays.copyOf(mPeriod, len);
			mPeriod = mPeriod.Copy(len);
			mArea = new double[len];
			Array.Copy(mPosition, j, mPosition, j + 1, len - j - 1);
			mPosition[j] = position;
			mPeriod[j] = period;
			mNormalized = false;
		}

		/// <summary>
		/// After adding point every thing must be normalized
		/// </summary>
		public virtual void normalize()
		{
			double totalArea = 0;
			double totalCount = 0;
			for (int i = 0; i < mPeriod.Length; i++)
			{
				totalCount += mPeriod[i];
			}
			for (int i = 1; i < mPeriod.Length; i++)
			{
				float h = (mPeriod[i - 1] + mPeriod[i]) / 2;
				double w = mPosition[i] - mPosition[i - 1];
				totalArea = totalArea + w * h;
			}
			// scale periods to normalize it
			for (int i = 0; i < mPeriod.Length; i++)
			{
				mPeriod[i] *= (float)(totalCount / totalArea);
			}
			mArea[0] = 0;
			for (int i = 1; i < mPeriod.Length; i++)
			{
				float h = (mPeriod[i - 1] + mPeriod[i]) / 2;
				double w = mPosition[i] - mPosition[i - 1];
				mArea[i] = mArea[i - 1] + w * h;
			}
			mNormalized = true;
		}

		internal virtual double getP(double time)
		{
			if (time < 0)
			{
				time = 0;
			}
			else if (time > 1)
			{
				time = 1;
			}
			//int index = Arrays.binarySearch(mPosition, time);
			int index = Array.BinarySearch(mPosition, time);
			double p = 0;
			if (index > 0)
			{
				p = 1;
			}
			else if (index != 0)
			{
				index = -index - 1;
				double t = time;
				double m = (mPeriod[index] - mPeriod[index - 1]) / (mPosition[index] - mPosition[index - 1]);
				p = mArea[index - 1] + (mPeriod[index - 1] - m * mPosition[index - 1]) * (t - mPosition[index - 1]) + m * (t * t - mPosition[index - 1] * mPosition[index - 1]) / 2;
			}
			return p;
		}

		public virtual double getValue(double time, double phase)
		{
			double angle = phase + getP(time); // angle is / by 360
			switch (mType)
			{
				default:
					goto case SIN_WAVE;
				case SIN_WAVE:
					return Math.Sin(PI2 * (angle));
				case SQUARE_WAVE:
					return Math.Sign(0.5 - angle % 1);
				case TRIANGLE_WAVE:
					return 1 - Math.Abs(((angle) * 4 + 1) % 4 - 2);
				case SAW_WAVE:
					return ((angle * 2 + 1) % 2) - 1;
				case REVERSE_SAW_WAVE:
					return (1 - ((angle * 2 + 1) % 2));
				case COS_WAVE:
					return Math.Cos(PI2 * (phase + angle));
				case BOUNCE:
					double x = 1 - Math.Abs(((angle) * 4) % 4 - 2);
					return 1 - x * x;
				case CUSTOM:
					return mCustomCurve.getPos(angle % 1, 0);
			}
		}

		internal virtual double getDP(double time)
		{
			if (time <= 0)
			{
				time = 0.00001;
			}
			else if (time >= 1)
			{
				time = .999999;
			}
			//int index = Arrays.binarySearch(mPosition, time);
			int index = Array.BinarySearch(mPosition, time);
			double p = 0;
			if (index > 0)
			{
				return 0;
			}
			if (index != 0)
			{
				index = -index - 1;
				double t = time;
				double m = (mPeriod[index] - mPeriod[index - 1]) / (mPosition[index] - mPosition[index - 1]);
				p = m * t + (mPeriod[index - 1] - m * mPosition[index - 1]);
			}
			return p;
		}

		public virtual double getSlope(double time, double phase, double dphase)
		{
			double angle = phase + getP(time);

			double dangle_dtime = getDP(time) + dphase;
			switch (mType)
			{
				default:
					goto case SIN_WAVE;
				case SIN_WAVE:
					return PI2 * dangle_dtime * Math.Cos(PI2 * angle);
				case SQUARE_WAVE:
					return 0;
				case TRIANGLE_WAVE:
					return 4 * dangle_dtime * Math.Sign(((angle) * 4 + 3) % 4 - 2);
				case SAW_WAVE:
					return dangle_dtime * 2;
				case REVERSE_SAW_WAVE:
					return -dangle_dtime * 2;
				case COS_WAVE:
					return -PI2 * dangle_dtime * Math.Sin(PI2 * angle);
				case BOUNCE:
					return 4 * dangle_dtime * (((angle) * 4 + 2) % 4 - 2);
				case CUSTOM:
					return mCustomCurve.getSlope(angle % 1, 0);
			}
		}
	}

}