﻿using System;

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
	/// Provides spline interpolation code.
	/// Currently not used but it is anticipated that we will be using it in the
	/// KeyMotion
	/// 
	/// @suppress
	/// </summary>

	public class HyperSpline
	{
		internal int mPoints;
		internal Cubic[][] mCurve;
		internal int mDimensionality;
		internal double[] mCurveLength;
		internal double mTotalLength;
		internal double[][] mCtl;

		/// <summary>
		/// Spline in N dimensions
		/// </summary>
		/// <param name="points"> [mPoints][dimensionality] </param>
		public HyperSpline(double[][] points)
		{
			setup(points);
		}

		public HyperSpline()
		{
		}

		public virtual void setup(double[][] points)
		{
			mDimensionality = points[0].Length;
			mPoints = points.Length;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: mCtl = new double[mDimensionality][mPoints];
			mCtl = RectangularArrays.ReturnRectangularDoubleArray(mDimensionality, mPoints);
			mCurve = new Cubic[mDimensionality][];
			for (int d = 0; d < mDimensionality; d++)
			{
				for (int p = 0; p < mPoints; p++)
				{
					mCtl[d][p] = points[p][d];
				}
			}

			for (int d = 0; d < mDimensionality; d++)
			{
				mCurve[d] = calcNaturalCubic(mCtl[d].Length, mCtl[d]);
			}

			mCurveLength = new double[mPoints - 1];
			mTotalLength = 0;
			Cubic[] temp = new Cubic[mDimensionality];
			for (int p = 0; p < mCurveLength.Length; p++)
			{
				for (int d = 0; d < mDimensionality; d++)
				{

					temp[d] = mCurve[d][p];

				}
				mTotalLength += mCurveLength[p] = approxLength(temp);
			}
		}

		public virtual void getVelocity(double p, double[] v)
		{
			double pos = p * mTotalLength;
			double sum = 0;
			int k = 0;
			for (; k < mCurveLength.Length - 1 && mCurveLength[k] < pos; k++)
			{
				pos -= mCurveLength[k];
			}
			for (int i = 0; i < v.Length; i++)
			{
				v[i] = mCurve[i][k].vel(pos / mCurveLength[k]);
			}
		}

		public virtual void getPos(double p, double[] x)
		{
			double pos = p * mTotalLength;
			double sum = 0;
			int k = 0;
			for (; k < mCurveLength.Length - 1 && mCurveLength[k] < pos; k++)
			{
				pos -= mCurveLength[k];
			}
			for (int i = 0; i < x.Length; i++)
			{
				x[i] = mCurve[i][k].eval(pos / mCurveLength[k]);
			}
		}

		public virtual void getPos(double p, float[] x)
		{
			double pos = p * mTotalLength;
			double sum = 0;
			int k = 0;
			for (; k < mCurveLength.Length - 1 && mCurveLength[k] < pos; k++)
			{
				pos -= mCurveLength[k];
			}
			for (int i = 0; i < x.Length; i++)
			{
				x[i] = (float) mCurve[i][k].eval(pos / mCurveLength[k]);
			}
		}

		public virtual double getPos(double p, int splineNumber)
		{
			double pos = p * mTotalLength;
			double sum = 0;
			int k = 0;
			for (; k < mCurveLength.Length - 1 && mCurveLength[k] < pos; k++)
			{
				pos -= mCurveLength[k];
			}
			return mCurve[splineNumber][k].eval(pos / mCurveLength[k]);
		}

		public virtual double approxLength(Cubic[] curve)
		{
			double sum = 0;

			int N = curve.Length;
			double[] old = new double[curve.Length];
			for (double i = 0; i < 1; i += .1)
			{
				double temps = 0;
				for (int j = 0; j < curve.Length; j++)
				{
					double tmp = old[j];
					tmp -= old[j] = curve[j].eval(i);
					temps += tmp * tmp;
				}
				if (i > 0)
				{
					sum += Math.Sqrt(temps);
				}

			}
			double s = 0;
			for (int j = 0; j < curve.Length; j++)
			{
				double tmp = old[j];
				tmp -= old[j] = curve[j].eval(1);
				s += tmp * tmp;
			}
			sum += Math.Sqrt(s);
			return sum;
		}

		internal static Cubic[] calcNaturalCubic(int n, double[] x)
		{
			double[] gamma = new double[n];
			double[] delta = new double[n];
			double[] D = new double[n];
			n -= 1;

			gamma[0] = 1.0f / 2.0f;
			for (int i = 1; i < n; i++)
			{
				gamma[i] = 1 / (4 - gamma[i - 1]);
			}
			gamma[n] = 1 / (2 - gamma[n - 1]);

			delta[0] = 3 * (x[1] - x[0]) * gamma[0];
			for (int i = 1; i < n; i++)
			{
				delta[i] = (3 * (x[i + 1] - x[i - 1]) - delta[i - 1]) * gamma[i];
			}
			delta[n] = (3 * (x[n] - x[n - 1]) - delta[n - 1]) * gamma[n];

			D[n] = delta[n];
			for (int i = n - 1; i >= 0; i--)
			{
				D[i] = delta[i] - gamma[i] * D[i + 1];
			}

			Cubic[] C = new Cubic[n];
			for (int i = 0; i < n; i++)
			{
				C[i] = new Cubic((float) x[i], D[i], 3 * (x[i + 1] - x[i]) - 2 * D[i] - D[i + 1], 2 * (x[i] - x[i + 1]) + D[i] + D[i + 1]);
			}
			return C;
		}

		public class Cubic
		{
			internal double mA, mB, mC, mD;

			public Cubic(double a, double b, double c, double d)
			{
				mA = a;
				mB = b;
				mC = c;
				mD = d;
			}

			public virtual double eval(double u)
			{
				return (((mD * u) + mC) * u + mB) * u + mA;
			}

			public virtual double vel(double v)
			{
				//  (((mD * u) + mC) * u + mB) * u + mA
				//  =  "mA + u*mB + u*u*mC+u*u*u*mD" a cubic expression
				// diff with respect to u = mB + u*mC/2+ u*u*mD/3
				// made efficient (mD*u/3+mC/2)*u+mB;

				return (mD * 3 * v + mC * 2) * v + mB;
			}
		}
	}

}