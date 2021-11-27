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
	/// EquationVariable is used to represent a variable in a <seealso cref="LinearEquation LinearEquation"/>
	/// </summary>
	internal class EquationVariable
	{

		private Amount mAmount = null;
		private SolverVariable mVariable = null;
		/// <summary>
		/// Base constructor </summary>
		/// <param name="system"> the <seealso cref="LinearSystem linear system"/> this equation variable belongs to </param>
		/// <param name="amount"> the amount associated with this variable </param>
		/// <param name="name"> the variable name </param>
		/// <param name="type"> the variable type </param>
		public EquationVariable(LinearSystem system, Amount amount, string name, SolverVariable.Type type)
		{
			mAmount = amount;
			mVariable = (SolverVariable)system.RunMethod("getVariable",new object[] { name, type });
		}

		/// <summary>
		/// Alternate constructor, will set the type to be <seealso cref="SolverVariable.Type CONSTANT"/> </summary>
		/// <param name="amount"> the amount associated with this variable </param>
		public EquationVariable(Amount amount)
		{
			mAmount = amount;
		}

		/// <summary>
		/// Alternate constructor, will construct an amount given an integer number </summary>
		/// <param name="system"> the <seealso cref="LinearSystem linear system"/> this equation variable belongs to </param>
		/// <param name="amount"> the amount associated with this variable </param>
		/// <param name="name"> the variable name </param>
		/// <param name="type"> the variable type </param>
		public EquationVariable(LinearSystem system, int amount, string name, SolverVariable.Type type)
		{
			mAmount = new Amount(amount);
			//mVariable = system.getVariable(name, type);
			mVariable = (SolverVariable)system.RunMethod("getVariable", new object[] { name, type });
		}

		/// <summary>
		/// Alternate constructor, will set the type to be <seealso cref="SolverVariable.Type CONSTANT"/> </summary>
		/// <param name="system"> the <seealso cref="LinearSystem linear system"/> this equation variable belongs to </param>
		/// <param name="amount"> the amount associated with this variable </param>
		public EquationVariable(LinearSystem system, int amount)
		{
			mAmount = new Amount(amount);
		}

		/// <summary>
		/// Alternate constructor, will set the factor to be one by default </summary>
		/// <param name="system"> the <seealso cref="LinearSystem linear system"/> this equation variable belongs to </param>
		/// <param name="name"> the variable name </param>
		/// <param name="type"> the variable type </param>
		public EquationVariable(LinearSystem system, string name, SolverVariable.Type type)
		{
			mAmount = new Amount(1);
			//mVariable = system.getVariable(name, type);
			mVariable = (SolverVariable)system.RunMethod("getVariable", new object[] { name, type });
		}

		/// <summary>
		/// Alternate constructor, will multiply an amount to a given <seealso cref="EquationVariable"/> </summary>
		/// <param name="amount"> the amount given </param>
		/// <param name="variable"> the variable we'll multiply </param>
		public EquationVariable(Amount amount, EquationVariable variable)
		{
			mAmount = new Amount(amount);
			mAmount.multiply(variable.mAmount);
			mVariable = variable.SolverVariable;
		}

		/// <summary>
		/// Copy constructor </summary>
		/// <param name="v"> variable to copy </param>
		public EquationVariable(EquationVariable v)
		{
			mAmount = new Amount(v.mAmount);
			mVariable = v.SolverVariable;
		}

		/// <summary>
		/// Accessor for the variable's name </summary>
		/// <returns> the variable's name </returns>
		public virtual string Name
		{
			get
			{
				if (mVariable == null)
				{
					return null;
				}
				return mVariable.Name;
			}
		}

		/// <summary>
		/// Accessor for the variable's type </summary>
		/// <returns> the variable's type </returns>
		public virtual SolverVariable.Type Type
		{
			get
			{
				if (mVariable == null)
				{
					return SolverVariable.Type.CONSTANT;
				}
				return mVariable.GetFieldValue<SolverVariable.Type>("mType");
			}
		}

		/// <summary>
		/// Accessor for the <seealso cref="SolverVariable"/> </summary>
		/// <returns> the <seealso cref="SolverVariable"/> </returns>
		public virtual SolverVariable SolverVariable
		{
			get
			{
				return mVariable;
			}
		}

		/// <summary>
		/// Returns true if this is a constant </summary>
		/// <returns> true if a constant </returns>
		public virtual bool Constant
		{
			get
			{
				return (mVariable == null);
			}
		}

		/// <summary>
		/// Accessor to retrieve the amount associated with this variable </summary>
		/// <returns> amount </returns>
		public virtual Amount Amount
		{
			get
			{
				return mAmount;
			}
			set
			{
				mAmount = value;
			}
		}


		/// <summary>
		/// Inverse the current amount (from negative to positive or the reverse) </summary>
		/// <returns> this </returns>
		public virtual EquationVariable inverse()
		{
			mAmount.inverse();
			return this;
		}

		/// <summary>
		/// Returns true if the variables are isCompatible (same type, same name) </summary>
		/// <param name="variable"> another variable to compare this one to </param>
		/// <returns> true if isCompatible. </returns>
		public virtual bool isCompatible(EquationVariable variable)
		{
			if (Constant)
			{
				return variable.Constant;
			}
			else if (variable.Constant)
			{
				return false;
			}
			return (variable.SolverVariable == SolverVariable);
		}

		/// <summary>
		/// Add an amount from another variable to this variable </summary>
		/// <param name="variable"> variable added </param>
		public virtual void add(EquationVariable variable)
		{
			if (variable.isCompatible(this))
			{
				mAmount.add(variable.mAmount);
			}
		}

		/// <summary>
		/// Subtract an amount from another variable to this variable </summary>
		/// <param name="variable"> variable added </param>
		public virtual void subtract(EquationVariable variable)
		{
			if (variable.isCompatible(this))
			{
				mAmount.subtract(variable.mAmount);
			}
		}

		/// <summary>
		/// Multiply an amount from another variable to this variable </summary>
		/// <param name="variable"> variable multiplied </param>
		public virtual void multiply(EquationVariable variable)
		{
			multiply(variable.Amount);
		}

		/// <summary>
		/// Multiply this variable by a given amount </summary>
		/// <param name="amount"> specified amount multiplied </param>
		public virtual void multiply(Amount amount)
		{
			mAmount.multiply(amount);
		}

		/// <summary>
		/// Divide an amount from another variable to this variable </summary>
		/// <param name="variable"> variable dividing </param>
		public virtual void divide(EquationVariable variable)
		{
			mAmount.divide(variable.mAmount);
		}

		/// <summary>
		/// Override the toString() method to display the variable
		/// </summary>
		public override string ToString()
		{
			if (Constant)
			{
				return "" + mAmount;
			}
			if (mAmount.One || mAmount.MinusOne)
			{
				return "" + mVariable;
			}
			return "" + mAmount + " " + mVariable;
		}

		/// <summary>
		/// Returns a string displaying the sign of the variable (positive or negative, e.g. + or -) </summary>
		/// <returns> sign of the variable as a string, either + or - </returns>
		public virtual string signString()
		{
			if (mAmount.Positive)
			{
				return "+";
			}
			return "-";
		}

	}

}