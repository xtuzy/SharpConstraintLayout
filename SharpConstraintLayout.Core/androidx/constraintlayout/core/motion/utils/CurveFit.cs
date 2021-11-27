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
	/// Base class for curve fitting / interpolation
	/// Curve fits must be capable of being differentiable and extend beyond the points (extrapolate)
	/// 
	/// @suppress
	/// </summary>

	public abstract class CurveFit
	{
		public const int SPLINE = 0;
		public const int LINEAR = 1;
		public const int CONSTANT = 2;

		public static CurveFit get(int type, double[] time, double[][] y)
		{
			if (time.Length == 1)
			{
				type = CONSTANT;
			}
			switch (type)
			{
				case SPLINE:
					return new MonotonicCurveFit(time, y);
				case CONSTANT:
					return new Constant(time[0], y[0]);
				default:
					return new LinearCurveFit(time, y);
			}
		}

		public static CurveFit getArc(int[] arcModes, double[] time, double[][] y)
		{
			return new ArcCurveFit(arcModes, time, y);
		}

		public abstract void getPos(double t, double[] v);

		public abstract void getPos(double t, float[] v);

		public abstract double getPos(double t, int j);

		public abstract void getSlope(double t, double[] v);

		public abstract double getSlope(double t, int j);

		public abstract double[] TimePoints {get;}

		internal class Constant : CurveFit
		{
			internal double mTime;
			internal double[] mValue;

			internal Constant(double time, double[] value)
			{
				mTime = time;
				mValue = value;
			}

			public override void getPos(double t, double[] v)
			{
				Array.Copy(mValue, 0, v, 0, mValue.Length);
			}

			public override void getPos(double t, float[] v)
			{
				for (int i = 0; i < mValue.Length; i++)
				{
					v[i] = (float) mValue[i];
				}
			}

			public override double getPos(double t, int j)
			{
				return mValue[j];
			}

			public override void getSlope(double t, double[] v)
			{
				for (int i = 0; i < mValue.Length; i++)
				{
					v[i] = 0;
				}
			}

			public override double getSlope(double t, int j)
			{
				return 0;
			}

			public override double[] TimePoints
			{
				get
				{
					return new double[]{mTime};
				}
			}
		}
	}

}