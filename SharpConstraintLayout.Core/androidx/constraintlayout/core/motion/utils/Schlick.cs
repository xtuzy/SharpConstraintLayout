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
	/// Schlick’s bias and gain functions
	/// curve for use in an easing function including quantize functions
	/// 
	/// </summary>
	public class Schlick : Easing
	{
		private const bool DEBUG = false;
		internal double mS, mT;
		internal double eps;

		internal Schlick(string configString)
		{
			// done this way for efficiency

			str = configString;
			int start = configString.IndexOf('(');
			int off1 = configString.IndexOf(',', start);
			mS = double.Parse(configString.Substring(start + 1, off1 - (start + 1)).Trim());
			int off2 = configString.IndexOf(',', off1 + 1);
			mT = double.Parse(configString.Substring(off1 + 1, off2 - (off1 + 1)).Trim());
		}

		private double func(double x)
		{
			if (x < mT)
			{
				return mT * x / (x + mS * (mT - x));
			}
			return ((1 - mT) * (x - 1)) / (1 - x - mS * (mT - x));
		}

		private double dfunc(double x)
		{
			if (x < mT)
			{
				return (mS * mT * mT) / ((mS * (mT - x) + x) * (mS * (mT - x) + x));
			}
			return (mS * (mT - 1) * (mT - 1)) / ((-mS * (mT - x) - x + 1) * (-mS * (mT - x) - x + 1));
		}

		public override double getDiff(double x)
		{
			return dfunc(x);
		}

		public override double get(double x)
		{
			return func(x);
		}
	}

}