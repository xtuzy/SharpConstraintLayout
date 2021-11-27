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
	/// This class translates a series of floating point values into a continuous
	/// curve for use in an easing function including quantize functions
	/// it is used with the "spline(0,0.3,0.3,0.5,...0.9,1)" it should start at 0 and end at one 1
	/// </summary>
	public class StepCurve : Easing
	{
		private const bool DEBUG = false;
		internal MonotonicCurveFit mCurveFit;

		internal StepCurve(string configString)
		{
			// done this way for efficiency
			str = configString;
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

			//mCurveFit = genSpline(Arrays.copyOf(values, count));
			mCurveFit = genSpline(values.Copy(count));
		}

		private static MonotonicCurveFit genSpline(string str)
		{
			string wave = str;
			string[] sp = wave.Split("\\s+", true);
			double[] values = new double[sp.Length];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = double.Parse(sp[i]);
			}
			return genSpline(values);
		}

		private static MonotonicCurveFit genSpline(double[] values)
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
			if (DEBUG)
			{
				string t = "t ";
				string v = "v ";
				DecimalFormat df = new DecimalFormat("#.00");
				for (int i = 0; i < time.Length; i++)
				{
					t += df.format(time[i]) + " ";
					v += df.format(points[i][0]) + " ";
				}
				Console.WriteLine(t);
				Console.WriteLine(v);
			}
			MonotonicCurveFit ms = new MonotonicCurveFit(time, points);
			Console.WriteLine(" 0 " + ms.getPos(0, 0));
			Console.WriteLine(" 1 " + ms.getPos(1, 0));
			return ms;
		}

		public override double getDiff(double x)
		{
			return mCurveFit.getSlope(x, 0);
		}

		public override double get(double x)
		{
			return mCurveFit.getPos(x, 0);
		}
	}

}