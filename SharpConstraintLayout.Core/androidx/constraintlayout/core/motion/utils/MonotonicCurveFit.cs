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
	/// This performs a spline interpolation in multiple dimensions
	/// 
	/// @suppress
	/// </summary>
	public class MonotonicCurveFit : CurveFit
	{
		private const string TAG = "MonotonicCurveFit";
		private double[] mT;
		private double[][] mY;
		private double[][] mTangent;
		private bool mExtrapolate = true;
		internal double[] mSlopeTemp;

		public MonotonicCurveFit(double[] time, double[][] y)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = time.length;
			int N = time.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = y[0].length;
			int dim = y[0].Length;
			mSlopeTemp = new double[dim];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] slope = new double[N - 1][dim]; // could optimize this out
			double[][] slope = RectangularArrays.ReturnRectangularDoubleArray(N - 1, dim); // could optimize this out
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] tangent = new double[N][dim];
			double[][] tangent = RectangularArrays.ReturnRectangularDoubleArray(N, dim);
			for (int j = 0; j < dim; j++)
			{
				for (int i = 0; i < N - 1; i++)
				{
					double dt = time[i + 1] - time[i];
					slope[i][j] = (y[i + 1][j] - y[i][j]) / dt;
					if (i == 0)
					{
						tangent[i][j] = slope[i][j];
					}
					else
					{
						tangent[i][j] = (slope[i - 1][j] + slope[i][j]) * 0.5f;
					}
				}
				tangent[N - 1][j] = slope[N - 2][j];
			}

			for (int i = 0; i < N - 1; i++)
			{
				for (int j = 0; j < dim; j++)
				{
					if (slope[i][j] == 0.0)
					{
						tangent[i][j] = 0.0;
						tangent[i + 1][j] = 0.0;
					}
					else
					{
						double a = tangent[i][j] / slope[i][j];
						double b = tangent[i + 1][j] / slope[i][j];
						double h = MathExtension.hypot(a, b);
						if (h > 9.0)
						{
							double t = 3.0 / h;
							tangent[i][j] = t * a * slope[i][j];
							tangent[i + 1][j] = t * b * slope[i][j];
						}
					}
				}
			}
			mT = time;
			mY = y;
			mTangent = tangent;
		}

		public override void getPos(double t, double[] v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = mT.length;
			int n = mT.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = mY[0].length;
			int dim = mY[0].Length;
			if (mExtrapolate)
			{
				if (t <= mT[0])
				{
					getSlope(mT[0], mSlopeTemp);
					for (int j = 0; j < dim; j++)
					{
						v[j] = mY[0][j] + (t - mT[0]) * mSlopeTemp[j];
					}
					return;
				}
				if (t >= mT[n - 1])
				{
					getSlope(mT[n - 1], mSlopeTemp);
					for (int j = 0; j < dim; j++)
					{
						v[j] = mY[n - 1][j] + (t - mT[n - 1]) * mSlopeTemp[j];
					}
					return;
				}
			}
			else
			{
				if (t <= mT[0])
				{
					for (int j = 0; j < dim; j++)
					{
						v[j] = mY[0][j];
					}
					return;
				}
				if (t >= mT[n - 1])
				{
					for (int j = 0; j < dim; j++)
					{
						v[j] = mY[n - 1][j];
					}
					return;
				}
			}

			for (int i = 0; i < n - 1; i++)
			{
				if (t == mT[i])
				{
					for (int j = 0; j < dim; j++)
					{
						v[j] = mY[i][j];
					}
				}
				if (t < mT[i + 1])
				{
					double h = mT[i + 1] - mT[i];
					double x = (t - mT[i]) / h;
					for (int j = 0; j < dim; j++)
					{
						double y1 = mY[i][j];
						double y2 = mY[i + 1][j];
						double t1 = mTangent[i][j];
						double t2 = mTangent[i + 1][j];
						v[j] = interpolate(h, x, y1, y2, t1, t2);
					}
					return;
				}
			}
		}

		public override void getPos(double t, float[] v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = mT.length;
			int n = mT.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = mY[0].length;
			int dim = mY[0].Length;
			if (mExtrapolate)
			{
				if (t <= mT[0])
				{
					getSlope(mT[0], mSlopeTemp);
					for (int j = 0; j < dim; j++)
					{
						v[j] = (float)(mY[0][j] + (t - mT[0]) * mSlopeTemp[j]);
					}
					return;
				}
				if (t >= mT[n - 1])
				{
					getSlope(mT[n - 1], mSlopeTemp);
					for (int j = 0; j < dim; j++)
					{
						v[j] = (float)(mY[n - 1][j] + (t - mT[n - 1]) * mSlopeTemp[j]);
					}
					return;
				}
			}
			else
			{
				if (t <= mT[0])
				{
					for (int j = 0; j < dim; j++)
					{
						v[j] = (float) mY[0][j];
					}
					return;
				}
				if (t >= mT[n - 1])
				{
					for (int j = 0; j < dim; j++)
					{
						v[j] = (float) mY[n - 1][j];
					}
					return;
				}
			}

			for (int i = 0; i < n - 1; i++)
			{
				if (t == mT[i])
				{
					for (int j = 0; j < dim; j++)
					{
						v[j] = (float) mY[i][j];
					}
				}
				if (t < mT[i + 1])
				{
					double h = mT[i + 1] - mT[i];
					double x = (t - mT[i]) / h;
					for (int j = 0; j < dim; j++)
					{
						double y1 = mY[i][j];
						double y2 = mY[i + 1][j];
						double t1 = mTangent[i][j];
						double t2 = mTangent[i + 1][j];
						v[j] = (float) interpolate(h, x, y1, y2, t1, t2);
					}
					return;
				}
			}
		}

		public override double getPos(double t, int j)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = mT.length;
			int n = mT.Length;
			if (mExtrapolate)
			{
				if (t <= mT[0])
				{
					return mY[0][j] + (t - mT[0]) * getSlope(mT[0], j);
				}
				if (t >= mT[n - 1])
				{
					return mY[n - 1][j] + (t - mT[n - 1]) * getSlope(mT[n - 1], j);
				}
			}
			else
			{
				if (t <= mT[0])
				{
					return mY[0][j];
				}
				if (t >= mT[n - 1])
				{
					return mY[n - 1][j];
				}
			}

			for (int i = 0; i < n - 1; i++)
			{
				if (t == mT[i])
				{
					return mY[i][j];
				}
				if (t < mT[i + 1])
				{
					double h = mT[i + 1] - mT[i];
					double x = (t - mT[i]) / h;
					double y1 = mY[i][j];
					double y2 = mY[i + 1][j];
					double t1 = mTangent[i][j];
					double t2 = mTangent[i + 1][j];
					return interpolate(h, x, y1, y2, t1, t2);

				}
			}
			return 0; // should never reach here
		}

		public override void getSlope(double t, double[] v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = mT.length;
			int n = mT.Length;
			int dim = mY[0].Length;
			if (t <= mT[0])
			{
				t = mT[0];
			}
			else if (t >= mT[n - 1])
			{
				t = mT[n - 1];
			}

			for (int i = 0; i < n - 1; i++)
			{
				if (t <= mT[i + 1])
				{
					double h = mT[i + 1] - mT[i];
					double x = (t - mT[i]) / h;
					for (int j = 0; j < dim; j++)
					{
						double y1 = mY[i][j];
						double y2 = mY[i + 1][j];
						double t1 = mTangent[i][j];
						double t2 = mTangent[i + 1][j];
						v[j] = diff(h, x, y1, y2, t1, t2) / h;
					}
					break;
				}
			}
			return;
		}

		public override double getSlope(double t, int j)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = mT.length;
			int n = mT.Length;

			if (t < mT[0])
			{
				t = mT[0];
			}
			else if (t >= mT[n - 1])
			{
				t = mT[n - 1];
			}
			for (int i = 0; i < n - 1; i++)
			{
				if (t <= mT[i + 1])
				{
					double h = mT[i + 1] - mT[i];
					double x = (t - mT[i]) / h;
					double y1 = mY[i][j];
					double y2 = mY[i + 1][j];
					double t1 = mTangent[i][j];
					double t2 = mTangent[i + 1][j];
					return diff(h, x, y1, y2, t1, t2) / h;
				}
			}
			return 0; // should never reach here
		}

		public override double[] TimePoints
		{
			get
			{
				return mT;
			}
		}

		/// <summary>
		/// Cubic Hermite spline
		/// 
		/// @return
		/// </summary>
		private static double interpolate(double h, double x, double y1, double y2, double t1, double t2)
		{
			double x2 = x * x;
			double x3 = x2 * x;
			return -2 * x3 * y2 + 3 * x2 * y2 + 2 * x3 * y1 - 3 * x2 * y1 + y1 + h * t2 * x3 + h * t1 * x3 - h * t2 * x2 - 2 * h * t1 * x2 + h * t1 * x;
		}

		/// <summary>
		/// Cubic Hermite spline slope differentiated
		/// 
		/// @return
		/// </summary>
		private static double diff(double h, double x, double y1, double y2, double t1, double t2)
		{
			double x2 = x * x;
			return -6 * x2 * y2 + 6 * x * y2 + 6 * x2 * y1 - 6 * x * y1 + 3 * h * t2 * x2 + 3 * h * t1 * x2 - 2 * h * t2 * x - 4 * h * t1 * x + h * t1;
		}

		/// <summary>
		/// This builds a monotonic spline to be used as a wave function
		/// </summary>
		/// <param name="configString">
		/// @return </param>
		public static MonotonicCurveFit buildWave(string configString)
		{
			// done this way for efficiency
			string str = configString;
			double[] values = new double[str.Length / 2];
			int start = configString.IndexOf('(') + 1;
			int off1 = configString.IndexOf(',', start);
			int count = 0;
			while (off1 != -1)
			{
				string tmp = configString.Substring(start, off1 - start).Trim();
				values[count++] = double.Parse(tmp);
				off1 = configString.IndexOf(',', start = off1 + 1);
			}
			off1 = configString.IndexOf(')', start);
			string tmpStr = configString.Substring(start, off1 - start).Trim();
			values[count++] = double.Parse(tmpStr);

			//return buildWave(Arrays.copyOf(values, count));
			return buildWave(values.Copy(count));
		}

		private static MonotonicCurveFit buildWave(double[] values)
		{
			int length = values.Length * 3 - 2;
			int len = values.Length - 1;
			double gap = 1.0 / len;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] points = new double[length][1];
			double[][] points = RectangularArrays.ReturnRectangularDoubleArray(length, 1);
			double[] time = new double[length];
			for (int i = 0; i < values.Length; i++)
			{
				double v = values[i];
				points[i + len][0] = v;
				time[i + len] = i * gap;
				if (i > 0)
				{
					points[i + len * 2][0] = v + 1;
					time[i + len * 2] = i * gap + 1;

					points[i - 1][0] = v - 1 - gap;
					time[i - 1] = i * gap + -1 - gap;
				}
			}

			return new MonotonicCurveFit(time, points);
		}
	}

}