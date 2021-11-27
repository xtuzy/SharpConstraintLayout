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
	/// Provide the engine for cubic spline easing
	/// 
	/// @suppress
	/// </summary>
	public class Easing
	{
		internal static Easing sDefault = new Easing();
		internal string str = "identity";
		private const string STANDARD = "cubic(0.4, 0.0, 0.2, 1)";
		private const string ACCELERATE = "cubic(0.4, 0.05, 0.8, 0.7)";
		private const string DECELERATE = "cubic(0.0, 0.0, 0.2, 0.95)";
		private const string LINEAR = "cubic(1, 1, 0, 0)";
		private const string ANTICIPATE = "cubic(0.36, 0, 0.66, -0.56)";
		private const string OVERSHOOT = "cubic(0.34, 1.56, 0.64, 1)";

		private const string DECELERATE_NAME = "decelerate";
		private const string ACCELERATE_NAME = "accelerate";
		private const string STANDARD_NAME = "standard";
		private const string LINEAR_NAME = "linear";
		private const string ANTICIPATE_NAME = "anticipate";
		private const string OVERSHOOT_NAME = "overshoot";

		public static string[] NAMED_EASING = new string[] {STANDARD_NAME, ACCELERATE_NAME, DECELERATE_NAME, LINEAR_NAME};

		public static Easing getInterpolator(string configString)
		{
			if (string.ReferenceEquals(configString, null))
			{
				return null;
			}
			if (configString.StartsWith("cubic", StringComparison.Ordinal))
			{
				return new CubicEasing(configString);
			}
			else if (configString.StartsWith("spline", StringComparison.Ordinal))
			{
				return new StepCurve(configString);
			}
			else if (configString.StartsWith("Schlick", StringComparison.Ordinal))
			{
				return new Schlick(configString);
			}
			else
			{
				switch (configString)
				{
					case STANDARD_NAME:
						return new CubicEasing(STANDARD);
					case ACCELERATE_NAME:
						return new CubicEasing(ACCELERATE);
					case DECELERATE_NAME:
						return new CubicEasing(DECELERATE);
					case LINEAR_NAME:
						return new CubicEasing(LINEAR);
					case ANTICIPATE_NAME:
						return new CubicEasing(ANTICIPATE);
					case OVERSHOOT_NAME:
						return new CubicEasing(OVERSHOOT);
					default:
						//Console.Error.WriteLine("transitionEasing syntax error syntax:" + "transitionEasing=\"cubic(1.0,0.5,0.0,0.6)\" or " + NAMED_EASING.ToString());
						Console.Fail("transitionEasing syntax error syntax:" + "transitionEasing=\"cubic(1.0,0.5,0.0,0.6)\" or " + NAMED_EASING.ToString());
					break;
				}

			}
			return sDefault;
		}

		public virtual double get(double x)
		{
			return x;
		}

		public override string ToString()
		{
			return str;
		}

		public virtual double getDiff(double x)
		{
			return 1;
		}

		internal class CubicEasing : Easing
		{

			internal static double error = 0.01;
			internal static double d_error = 0.0001;
			internal double x1, y1, x2, y2;

			internal CubicEasing(string configString)
			{
				// done this way for efficiency
				str = configString;
				int start = configString.IndexOf('(');
				int off1 = configString.IndexOf(',', start);
				x1 = double.Parse(configString.Substring(start + 1, off1 - (start + 1)).Trim());
				int off2 = configString.IndexOf(',', off1 + 1);
				y1 = double.Parse(configString.Substring(off1 + 1, off2 - (off1 + 1)).Trim());
				int off3 = configString.IndexOf(',', off2 + 1);
				x2 = double.Parse(configString.Substring(off2 + 1, off3 - (off2 + 1)).Trim());
				int end = configString.IndexOf(')', off3 + 1);
				y2 = double.Parse(configString.Substring(off3 + 1, end - (off3 + 1)).Trim());
			}

			public CubicEasing(double x1, double y1, double x2, double y2)
			{
				setup(x1, y1, x2, y2);
			}

			internal virtual void setup(double x1, double y1, double x2, double y2)
			{
				this.x1 = x1;
				this.y1 = y1;
				this.x2 = x2;
				this.y2 = y2;
			}

			internal virtual double getX(double t)
			{
				double t1 = 1 - t;
				// no need for because start at 0,0 double f0 = (1 - t) * (1 - t) * (1 - t);
				double f1 = 3 * t1 * t1 * t;
				double f2 = 3 * t1 * t * t;
				double f3 = t * t * t;
				return x1 * f1 + x2 * f2 + f3;
			}

			internal virtual double getY(double t)
			{
				double t1 = 1 - t;
				// no need for because start at 0,0 double f0 = (1 - t) * (1 - t) * (1 - t);
				double f1 = 3 * t1 * t1 * t;
				double f2 = 3 * t1 * t * t;
				double f3 = t * t * t;
				return y1 * f1 + y2 * f2 + f3;
			}

			internal virtual double getDiffX(double t)
			{
				double t1 = 1 - t;
				return 3 * t1 * t1 * x1 + 6 * t1 * t * (x2 - x1) + 3 * t * t * (1 - x2);
			}

			internal virtual double getDiffY(double t)
			{
				double t1 = 1 - t;
				return 3 * t1 * t1 * y1 + 6 * t1 * t * (y2 - y1) + 3 * t * t * (1 - y2);
			}

			/// <summary>
			/// binary search for the region
			/// and linear interpolate the answer
			/// </summary>
			public override double getDiff(double x)
			{
				double t = 0.5;
				double range = 0.5;
				while (range > d_error)
				{
					double tx = getX(t);
					range *= 0.5;
					if (tx < x)
					{
						t += range;
					}
					else
					{
						t -= range;
					}
				}

				double x1 = getX(t - range);
				double x2 = getX(t + range);
				double y1 = getY(t - range);
				double y2 = getY(t + range);

				return (y2 - y1) / (x2 - x1);
			}

			/// <summary>
			/// binary search for the region
			/// and linear interpolate the answer
			/// </summary>
			public override double get(double x)
			{
				if (x <= 0.0)
				{
					return 0;
				}
				if (x >= 1.0)
				{
					return 1.0;
				}
				double t = 0.5;
				double range = 0.5;
				while (range > error)
				{
					double tx = getX(t);
					range *= 0.5;
					if (tx < x)
					{
						t += range;
					}
					else
					{
						t -= range;
					}
				}

				double x1 = getX(t - range);
				double x2 = getX(t + range);
				double y1 = getY(t - range);
				double y2 = getY(t + range);

				return (y2 - y1) * (x - x1) / (x2 - x1) + y1;
			}
		}
	}

}