/*
 * Copyright (C) 2015 The Android Open Source Project
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

namespace androidx.constraintlayout.core
{
	/// <summary>
	/// Represents the amount of a given <seealso cref="EquationVariable variable"/>, can be fractional.
	/// </summary>
	internal class Amount
	{
		private int mNumerator = 0;
		private int mDenominator = 1;

		/// <summary>
		/// Base constructor, set the numerator and denominator. </summary>
		/// <param name="numerator"> the numerator </param>
		/// <param name="denominator"> the denominator </param>
		public Amount(int numerator, int denominator)
		{
			mNumerator = numerator;
			mDenominator = denominator;
			simplify();
		}

		/// <summary>
		/// Alternate constructor, set the numerator, with the denominator set to one. </summary>
		/// <param name="numerator"> the amount's value </param>
		public Amount(int numerator)
		{
			mNumerator = numerator;
			mDenominator = 1;
		}

		public Amount(Amount amount)
		{
			mNumerator = amount.mNumerator;
			mDenominator = amount.mDenominator;
			simplify();
		}

		/// <summary>
		/// Set the numerator and denominator directly </summary>
		/// <param name="numerator"> numerator </param>
		/// <param name="denominator"> denominator </param>
		public virtual void set(int numerator, int denominator)
		{
			mNumerator = numerator;
			mDenominator = denominator;
			simplify();
		}

		/// <summary>
		/// Add an amount to the current one. </summary>
		/// <param name="amount"> amount to add </param>
		/// <returns> this </returns>
		public virtual Amount add(Amount amount)
		{
			if (mDenominator == amount.mDenominator)
			{
				mNumerator += amount.mNumerator;
			}
			else
			{
				mNumerator = mNumerator * amount.mDenominator + amount.mNumerator * mDenominator;
				mDenominator = mDenominator * amount.mDenominator;
			}
			simplify();
			return this;
		}

		/// <summary>
		/// Add an integer amount </summary>
		/// <param name="amount"> amount to add </param>
		/// <returns> this </returns>
		public virtual Amount add(int amount)
		{
			mNumerator += amount * mDenominator;
			return this;
		}

		/// <summary>
		/// Subtract an amount to the current one. </summary>
		/// <param name="amount"> amount to subtract </param>
		/// <returns> this </returns>
		public virtual Amount subtract(Amount amount)
		{
			if (mDenominator == amount.mDenominator)
			{
				mNumerator -= amount.mNumerator;
			}
			else
			{
				mNumerator = mNumerator * amount.mDenominator - amount.mNumerator * mDenominator;
				mDenominator = mDenominator * amount.mDenominator;
			}
			simplify();
			return this;
		}

		/// <summary>
		/// Multiply an amount with the current one. </summary>
		/// <param name="amount"> amount to multiply by </param>
		/// <returns> this </returns>
		public virtual Amount multiply(Amount amount)
		{
			mNumerator = mNumerator * amount.mNumerator;
			mDenominator = mDenominator * amount.mDenominator;
			simplify();
			return this;
		}

		/// <summary>
		/// Divide the current amount by the given amount. </summary>
		/// <param name="amount"> amount to divide by </param>
		/// <returns> this </returns>
		public virtual Amount divide(Amount amount)
		{
			int preN = mNumerator;
			int preD = mDenominator;
			mNumerator = mNumerator * amount.mDenominator;
			mDenominator = mDenominator * amount.mNumerator;
			simplify();
			return this;
		}

		/// <summary>
		/// Inverse the current amount as a fraction (e.g. a/b becomes b/a) </summary>
		/// <returns> this </returns>
		public virtual Amount inverseFraction()
		{
			int n = mNumerator;
			mNumerator = mDenominator;
			mDenominator = n;
			simplify();
			return this;
		}

		/// <summary>
		/// Inverse the current amount (positive to negative or negative to positive) </summary>
		/// <returns> this </returns>
		public virtual Amount inverse()
		{
			mNumerator *= -1;
			simplify();
			return this;
		}

		/// <summary>
		/// Accessor for the numerator </summary>
		/// <returns> the numerator </returns>
		public virtual int Numerator
		{
			get
			{
				return mNumerator;
			}
		}

		/// <summary>
		/// Accessor for the denominator </summary>
		/// <returns> the denominator </returns>
		public virtual int Denominator
		{
			get
			{
				return mDenominator;
			}
		}

		/// <summary>
		/// Override equals method </summary>
		/// <param name="o"> compared object </param>
		/// <returns> true if the compared object is equals to this one (same numerator and denominator) </returns>
		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is Amount))
			{
				return false;
			}
			Amount a = (Amount) o;
			return mNumerator == a.mNumerator && mDenominator == a.mDenominator;
		}

		/// <summary>
		/// Simplify the current amount. If the amount is fractional,
		/// we calculate the GCD and divide numerator and denominator by it.
		/// If both numerator and denominator are negative, turns things back
		/// to positive. If only the denominator is negative, make it positive
		/// and make the numerator negative instead.
		/// </summary>
		private void simplify()
		{
			if (mNumerator < 0 && mDenominator < 0)
			{
				mNumerator *= -1;
				mDenominator *= -1;
			}
			else if (mNumerator >= 0 && mDenominator < 0)
			{
				mNumerator *= -1;
				mDenominator *= -1;
			}
			if (mDenominator > 1)
			{
				int commonDenominator;
				if (mDenominator == 2 && mNumerator % 2 == 0)
				{
					commonDenominator = 2;
				}
				else
				{
					commonDenominator = gcd(mNumerator, mDenominator);
				}
				mNumerator /= commonDenominator;
				mDenominator /= commonDenominator;
			}
		}

		/// <summary>
		/// Iterative Binary GCD algorithm </summary>
		/// <param name="u"> first number </param>
		/// <param name="v"> second number </param>
		/// <returns> Greater Common Divisor </returns>
		private static int gcd(int u, int v)
		{
			int shift;

			if (u < 0)
			{
				u *= -1;
			}

			if (v < 0)
			{
				v *= -1;
			}

			if (u == 0)
			{
				return v;
			}

			if (v == 0)
			{
				return u;
			}

			for (shift = 0; ((u | v) & 1) == 0; shift++)
			{
				u >>= 1;
				v >>= 1;
			}

			while ((u & 1) == 0)
			{
				u >>= 1;
			}

			do
			{
				while ((v & 1) == 0)
				{
					v >>= 1;
				}
				if (u > v)
				{
					int t = v;
					v = u;
					u = t;
				}
				v = v - u;
			} while (v != 0);
			return u << shift;
		}

		/// <summary>
		/// Returns true if the Amount is equals to one </summary>
		/// <returns> true if the Amount is equals to one </returns>
		public virtual bool One
		{
			get
			{
				return (mNumerator == 1 && mDenominator == 1);
			}
		}

		/// <summary>
		/// Returns true if the Amount is equals to minus one </summary>
		/// <returns> true if the Amount is equals to minus one </returns>
		public virtual bool MinusOne
		{
			get
			{
				return (mNumerator == -1 && mDenominator == 1);
			}
		}

		/// <summary>
		/// Returns true if the Amount is positive. </summary>
		/// <returns> true if the Amount is positive. </returns>
		public virtual bool Positive
		{
			get
			{
				return (mNumerator >= 0 && mDenominator >= 0);
			}
		}

		/// <summary>
		/// Returns true if the Amount is negative. </summary>
		/// <returns> true if the Amount is negative. </returns>
		public virtual bool Negative
		{
			get
			{
				return (mNumerator < 0);
			}
		}

		/// <summary>
		/// Returns true if the value is zero </summary>
		/// <returns> true if the value is zero </returns>
		public virtual bool Null
		{
			get
			{
				return mNumerator == 0;
			}
		}

		/// <summary>
		/// Set the Amount to zero.
		/// </summary>
		public virtual void setToZero()
		{
			mNumerator = 0;
			mDenominator = 1;
		}

		/// <summary>
		/// Returns the float value of the Amount </summary>
		/// <returns> the float value </returns>
		public virtual float toFloat()
		{
			if (mDenominator >= 1)
			{
				return mNumerator / (float) mDenominator;
			}
			return 0;
		}

		/// <summary>
		/// Override the toString() method to display the amount (possibly as a fraction) </summary>
		/// <returns> formatted string </returns>
		public override string ToString()
		{
			if (mDenominator == 1)
			{
				if (mNumerator == 1 || mNumerator == -1)
				{
					return "";
				}
				if (mNumerator < 0)
				{
					return "" + (mNumerator * -1);
				}
				return "" + mNumerator;
			}
			if (mNumerator < 0)
			{
				return "" + (mNumerator * -1) + "/" + mDenominator;
			}
			return "" + mNumerator + "/" + mDenominator;
		}

		public virtual string valueString()
		{
			if (mDenominator == 1)
			{
				return "" + mNumerator;
			}
			return "" + mNumerator + "/" + mDenominator;
		}
	}

}