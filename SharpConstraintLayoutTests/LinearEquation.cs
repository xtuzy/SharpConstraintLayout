using System;
using System.Collections.Generic;

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
    /// LinearEquation is used to represent the linear equations fed into the solver.<br>
    /// A linear equation can be an equality or an inequation (left term &le; or &ge; to the right term).<br>
    /// The general form will be similar to {@code a0x0 + a1x1 + ... = C + a2x2 + a3x3 + ... ,} where {@code a0x0} is a term representing
    /// a variable x0 of an amount {@code a0}, and {@code C} represent a constant term. The amount of terms on the left side or the right
    /// side of the equation is arbitrary.
    /// </summary>
    internal class LinearEquation
    {

        private List<EquationVariable> mLeftSide = new List<EquationVariable>();
        private List<EquationVariable> mRightSide = new List<EquationVariable>();
        private List<EquationVariable> mCurrentSide = null;

        public virtual bool Null
        {
            get
            {
                if (mLeftSide.Count == 0 && mRightSide.Count == 0)
                {
                    return true;
                }
                if (mLeftSide.Count == 1 && mRightSide.Count == 1)
                {
                    EquationVariable v1 = mLeftSide[0];
                    EquationVariable v2 = mRightSide[0];
                    if (v1.Constant && v2.Constant && v1.Amount.Null && v2.Amount.Null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private enum Type
        {
            EQUALS,
            LOWER_THAN,
            GREATER_THAN
        }

        private Type mType = Type.EQUALS;

        private LinearSystem mSystem = null;

        private static int artificialIndex = 0;
        private static int slackIndex = 0;
        private static int errorIndex = 0;

        internal static string NextArtificialVariableName
        {
            get
            {
                return "a" + ++artificialIndex;
            }
        }
        internal static string NextSlackVariableName
        {
            get
            {
                return "s" + ++slackIndex;
            }
        }
        internal static string NextErrorVariableName
        {
            get
            {
                return "e" + ++errorIndex;
            }
        }

        /// <summary>
        /// Reset the counters for the automatic slack and error variable naming
        /// </summary>
        public static void resetNaming()
        {
            artificialIndex = 0;
            slackIndex = 0;
            errorIndex = 0;
        }

        /// <summary>
        /// Copy constructor </summary>
        /// <param name="equation"> to copy </param>
        public LinearEquation(LinearEquation equation)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.util.ArrayList<EquationVariable> mLeftSide1 = equation.mLeftSide;
            List<EquationVariable> mLeftSide1 = equation.mLeftSide;
            for (int i = 0, mLeftSide1Size = mLeftSide1.Count; i < mLeftSide1Size; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mLeftSide1.get(i);
                EquationVariable v = mLeftSide1[i];
                EquationVariable v2 = new EquationVariable(v);
                mLeftSide.Add(v2);
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.util.ArrayList<EquationVariable> mRightSide1 = equation.mRightSide;
            List<EquationVariable> mRightSide1 = equation.mRightSide;
            for (int i = 0, mRightSide1Size = mRightSide1.Count; i < mRightSide1Size; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mRightSide1.get(i);
                EquationVariable v = mRightSide1[i];
                EquationVariable v2 = new EquationVariable(v);
                mRightSide.Add(v2);
            }
            mCurrentSide = mRightSide;
        }

        /// <summary>
        /// Transform a LinearEquation into a Row </summary>
        /// <param name="linearSystem"> </param>
        /// <param name="e"> linear equation </param>
        /// <returns> a Row object </returns>
        internal static ArrayRow createRowFromEquation(LinearSystem linearSystem, LinearEquation e)
        {
            e.normalize();
            e.moveAllToTheRight();
            // Let's build a row from the LinearEquation
            ArrayRow row = linearSystem.createRow();
            List<EquationVariable> eq = e.RightSide;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int count = eq.size();
            int count = eq.Count;
            for (int i = 0; i < count; i++)
            {
                EquationVariable v = eq[i];
                SolverVariable sv = v.SolverVariable;
                if (sv != null)
                {
                    float previous = row.variables.get(sv);
                    row.variables.put(sv, previous + v.Amount.toFloat());
                }
                else
                {
                    row.SetFieldValue<float>("constantValue", v.Amount.toFloat());
                }
            }
            return row;
        }

        /// <summary>
        /// Insert the equation in the system
        /// </summary>
        public virtual void i()
        {
            if (mSystem == null)
            {
                return;
            }
            ArrayRow row = createRowFromEquation(mSystem, this);
            mSystem.addConstraint(row);
        }

        /// <summary>
        /// Set the current side to be the left side
        /// </summary>
        public virtual void setLeftSide()
        {
            mCurrentSide = mLeftSide;
        }

        /// <summary>
        /// Remove any terms on the left side of the equation
        /// </summary>
        public virtual void clearLeftSide()
        {
            mLeftSide.Clear();
        }

        /// <summary>
        /// Remove <seealso cref="EquationVariable"/> pointing to <seealso cref="SolverVariable"/> </summary>
        /// <param name="v"> the <seealso cref="SolverVariable"/> we want to remove from the equation </param>
        public virtual void remove(SolverVariable v)
        {
            EquationVariable ev = find(v, mLeftSide);
            if (ev != null)
            {
                mLeftSide.Remove(ev);
            }
            ev = find(v, mRightSide);
            if (ev != null)
            {
                mRightSide.Remove(ev);
            }
        }

        /// <summary>
        /// Base constructor, set the current side to the left side.
        /// </summary>
        public LinearEquation()
        {
            mCurrentSide = mLeftSide;
        }

        /// <summary>
        /// Base constructor, set the current side to the left side.
        /// </summary>
        public LinearEquation(LinearSystem system)
        {
            mCurrentSide = mLeftSide;
            mSystem = system;
        }

        /// <summary>
        /// Set the current equation system for this equation </summary>
        /// <param name="system"> the equation system this equation belongs to </param>
        public virtual LinearSystem System
        {
            set
            {
                mSystem = value;
            }
        }

        /// <summary>
        /// Set the equality operator for the equation, and switch the current side to the right side </summary>
        /// <returns> this </returns>
        public virtual LinearEquation equalsTo()
        {
            mCurrentSide = mRightSide;
            return this;
        }

        /// <summary>
        /// Set the greater than operator for the equation, and switch the current side to the right side </summary>
        /// <returns> this </returns>
        public virtual LinearEquation greaterThan()
        {
            mCurrentSide = mRightSide;
            mType = Type.GREATER_THAN;
            return this;
        }

        /// <summary>
        /// Set the lower than operator for the equation, and switch the current side to the right side </summary>
        /// <returns> this </returns>
        public virtual LinearEquation lowerThan()
        {
            mCurrentSide = mRightSide;
            mType = Type.LOWER_THAN;
            return this;
        }

        /// <summary>
        /// Normalize the linear equation. If the equation is an equality, transforms it into
        /// an equality, adding automatically slack or error variables.
        /// </summary>
        public virtual void normalize()
        {
            if (mType == Type.EQUALS)
            {
                return;
            }
            mCurrentSide = mLeftSide;
            if (mType == Type.LOWER_THAN)
            {
                withSlack(1);
            }
            else if (mType == Type.GREATER_THAN)
            {
                withSlack(-1);
            }
            mType = Type.EQUALS;
            mCurrentSide = mRightSide;
        }

        /// <summary>
        /// Will simplify the equation per side -- regroup similar variables into one.
        /// E.g. 2a + b + 3a = b - c will be turned into 5a + b = b - c.
        /// </summary>
        public virtual void simplify()
        {
            simplifySide(mLeftSide);
            simplifySide(mRightSide);
        }

        /// <summary>
        /// Simplify an array of <seealso cref="EquationVariable"/> </summary>
        /// <param name="side"> Array of EquationVariable </param>
        private void simplifySide(List<EquationVariable> side)
        {
            EquationVariable constant = null;
            Dictionary<string, EquationVariable> variables = new Dictionary<string, EquationVariable>();
            List<string> variablesNames = new List<string>();
            for (int i = 0, sideSize = side.Count; i < sideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = side.get(i);
                EquationVariable v = side[i];
                if (v.Constant)
                {
                    if (constant == null)
                    {
                        constant = v;
                    }
                    else
                    {
                        constant.add(v);
                    }
                }
                else
                {
                    if (variables.ContainsKey(v.Name))
                    {
                        EquationVariable original = variables[v.Name];
                        original.add(v);
                    }
                    else
                    {
                        variables[v.Name] = v;
                        variablesNames.Add(v.Name);
                    }
                }
            }
            side.Clear();
            if (constant != null)
            {
                side.Add(constant);
            }
            variablesNames.Sort();
            for (int i = 0, variablesNamesSize = variablesNames.Count; i < variablesNamesSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final String name = variablesNames.get(i);
                string name = variablesNames[i];
                EquationVariable v = variables[name];
                side.Add(v);
            }
            removeNullTerms(side);
        }

        public virtual void moveAllToTheRight()
        {
            for (int i = 0, mLeftSideSize = mLeftSide.Count; i < mLeftSideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mLeftSide.get(i);
                EquationVariable v = mLeftSide[i];
                mRightSide.Add(v.inverse());
            }
            mLeftSide.Clear();
        }

        /// <summary>
        /// Balance an equation to have only one term on the left side.
        /// The preference is to first pick an unconstrained variable, then a slack variable, then an error variable.
        /// </summary>
        public virtual void balance()
        {
            if (mLeftSide.Count == 0 && mRightSide.Count == 0)
            {
                return;
            }
            mCurrentSide = mLeftSide;
            for (int i = 0, mLeftSideSize = mLeftSide.Count; i < mLeftSideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mLeftSide.get(i);
                EquationVariable v = mLeftSide[i];
                mRightSide.Add(v.inverse());
            }
            mLeftSide.Clear();
            simplifySide(mRightSide);
            EquationVariable found = null;
            for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                EquationVariable v = mRightSide[i];
                if (v.Type == SolverVariable.Type.UNRESTRICTED)
                {
                    found = v;
                    break;
                }
            }
            if (found == null)
            {
                for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                    EquationVariable v = mRightSide[i];
                    if (v.Type == SolverVariable.Type.SLACK)
                    {
                        found = v;
                        break;
                    }
                }
            }
            if (found == null)
            {
                for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                    EquationVariable v = mRightSide[i];
                    if (v.Type == SolverVariable.Type.ERROR)
                    {
                        found = v;
                        break;
                    }
                }
            }
            if (found == null)
            {
                return;
            }
            mRightSide.Remove(found);
            found.inverse();
            if (!found.Amount.One)
            {
                Amount foundAmount = found.Amount;
                for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                    EquationVariable v = mRightSide[i];
                    v.Amount.divide(foundAmount);
                }
                found.Amount = new Amount(1);
            }
            simplifySide(mRightSide);
            mLeftSide.Add(found);
        }

        /// <summary>
        /// Check the equation to possibly remove null terms
        /// </summary>
        private void removeNullTerms(List<EquationVariable> list)
        {
            bool hasNullTerm = false;
            for (int i = 0, listSize = list.Count; i < listSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = list.get(i);
                EquationVariable v = list[i];
                if (v.Amount.Null)
                {
                    hasNullTerm = true;
                    break;
                }
            }
            if (hasNullTerm)
            {
                // if some elements are now zero, we need to remove them from the right side
                List<EquationVariable> newSide;
                newSide = new List<EquationVariable>();
                for (int i = 0, listSize = list.Count; i < listSize; i++)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final EquationVariable v = list.get(i);
                    EquationVariable v = list[i];
                    if (!v.Amount.Null)
                    {
                        newSide.Add(v);
                    }
                }
                list.Clear();
                list.AddRange(newSide);
            }
        }

        /// <summary>
        /// Pivot this equation on the variable -- e.g. the variable will be the only term on the left side of the equation. </summary>
        /// <param name="variable"> variable pivoted on </param>
        public virtual void pivot(SolverVariable variable)
        {
            if (mLeftSide.Count == 1 && mLeftSide[0].SolverVariable == variable)
            {
                // no-op, we're already pivoted.
                return;
            }
            for (int i = 0, mLeftSideSize = mLeftSide.Count; i < mLeftSideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mLeftSide.get(i);
                EquationVariable v = mLeftSide[i];
                mRightSide.Add(v.inverse());
            }
            mLeftSide.Clear();
            simplifySide(mRightSide);
            EquationVariable found = null;
            for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                EquationVariable v = mRightSide[i];
                if (v.SolverVariable == variable)
                {
                    found = v;
                    break;
                }
            }
            if (found != null)
            {
                mRightSide.Remove(found);
                found.inverse();
                if (!found.Amount.One)
                {
                    Amount foundAmount = found.Amount;
                    for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
                    {
                        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                        //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                        EquationVariable v = mRightSide[i];
                        v.Amount.divide(foundAmount);
                    }
                    found.Amount = new Amount(1);
                }
                mLeftSide.Add(found);
            }
        }

        /// <summary>
        /// Returns true if the constant is negative </summary>
        /// <returns> true if the constant is negative. </returns>
        public virtual bool hasNegativeConstant()
        {
            for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                EquationVariable v = mRightSide[i];
                if (v.Constant)
                {
                    if (v.Amount.Negative)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// If present, returns the constant on the right side of the equation.
        /// The equation is expected to be balanced before using this function. </summary>
        /// <returns> The equation constant </returns>
        public virtual Amount Constant
        {
            get
            {
                for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                    EquationVariable v = mRightSide[i];
                    if (v.Constant)
                    {
                        return v.Amount;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Inverse the equation (multiply both left and right terms by -1)
        /// </summary>
        public virtual void inverse()
        {
            Amount amount = new Amount(-1);
            for (int i = 0, mLeftSideSize = mLeftSide.Count; i < mLeftSideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mLeftSide.get(i);
                EquationVariable v = mLeftSide[i];
                v.multiply(amount);
            }
            for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                EquationVariable v = mRightSide[i];
                v.multiply(amount);
            }
        }

        /// <summary>
        /// Returns the first unconstrained variable encountered in this equation </summary>
        /// <returns> an unconstrained variable or null if none are found </returns>
        public virtual EquationVariable FirstUnconstrainedVariable
        {
            get
            {
                for (int i = 0, mLeftSideSize = mLeftSide.Count; i < mLeftSideSize; i++)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final EquationVariable v = mLeftSide.get(i);
                    EquationVariable v = mLeftSide[i];
                    if (v.Type == SolverVariable.Type.UNRESTRICTED)
                    {
                        return v;
                    }
                }
                for (int i = 0, mRightSideSize = mRightSide.Count; i < mRightSideSize; i++)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final EquationVariable v = mRightSide.get(i);
                    EquationVariable v = mRightSide[i];
                    if (v.Type == SolverVariable.Type.UNRESTRICTED)
                    {
                        return v;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the basic variable of the equation </summary>
        /// <returns> basic variable </returns>
        public virtual EquationVariable LeftVariable
        {
            get
            {
                if (mLeftSide.Count == 1)
                {
                    return mLeftSide[0];
                }
                return null;
            }
        }

        /// <summary>
        /// Replace the variable v in this equation (left or right side) by the right side of the equation l </summary>
        /// <param name="v"> the variable to replace </param>
        /// <param name="l"> the equation we use to replace it with </param>
        public virtual void replace(SolverVariable v, LinearEquation l)
        {
            replace(v, l, mLeftSide);
            replace(v, l, mRightSide);
        }

        /// <summary>
        /// Convenience function to replace the variable v possibly contained inside list
        /// by the right side of the equation l </summary>
        /// <param name="v"> the variable to replace </param>
        /// <param name="l"> the equation we use to replace it with </param>
        /// <param name="list"> the list of <seealso cref="EquationVariable"/> to work on </param>
        private void replace(SolverVariable v, LinearEquation l, List<EquationVariable> list)
        {
            EquationVariable toReplace = find(v, list);
            if (toReplace != null)
            {
                list.Remove(toReplace);
                Amount amount = toReplace.Amount;
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final java.util.ArrayList<EquationVariable> mRightSide1 = l.mRightSide;
                List<EquationVariable> mRightSide1 = l.mRightSide;
                for (int i = 0, mRightSide1Size = mRightSide1.Count; i < mRightSide1Size; i++)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final EquationVariable lv = mRightSide1.get(i);
                    EquationVariable lv = mRightSide1[i];
                    list.Add(new EquationVariable(amount, lv));
                }
            }
        }

        /// <summary>
        /// Returns the <seealso cref="EquationVariable"/> associated to
        /// the <seealso cref="SolverVariable"/> found in the
        /// list of <seealso cref="EquationVariable"/> </summary>
        /// <param name="v"> the variable to find </param>
        /// <param name="list"> list the list of <seealso cref="EquationVariable"/> to search in </param>
        /// <returns> the associated <seealso cref="EquationVariable"/> </returns>
        private EquationVariable find(SolverVariable v, List<EquationVariable> list)
        {
            for (int i = 0, listSize = list.Count; i < listSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable ev = list.get(i);
                EquationVariable ev = list[i];
                if (ev.SolverVariable == v)
                {
                    return ev;
                }
            }
            return null;
        }

        /// <summary>
        /// Accessor for the right side of the equation. </summary>
        /// <returns> the equation's right side. </returns>
        public virtual List<EquationVariable> RightSide
        {
            get
            {
                return mRightSide;
            }
        }

        /// <summary>
        /// Returns true if this equation contains a give variable </summary>
        /// <param name="solverVariable"> the variable we are looking for </param>
        /// <returns> true if found, false if not. </returns>
        public virtual bool contains(SolverVariable solverVariable)
        {
            if (find(solverVariable, mLeftSide) != null)
            {
                return true;
            }
            if (find(solverVariable, mRightSide) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the <seealso cref="EquationVariable"/> associated with a given
        /// <seealso cref="SolverVariable"/> in this equation </summary>
        /// <param name="solverVariable"> the variable we are looking for </param>
        /// <returns> the <seealso cref="EquationVariable"/> associated if found, otherwise null </returns>
        public virtual EquationVariable getVariable(SolverVariable solverVariable)
        {
            EquationVariable variable = find(solverVariable, mRightSide);
            if (variable != null)
            {
                return variable;
            }
            return find(solverVariable, mLeftSide);
        }

        /// <summary>
        /// Add a constant to the current side of the equation
        /// </summary>
        /// <param name="amount"> the value of the constant </param>
        /// <returns> this </returns>
        public virtual LinearEquation @var(int amount)
        {
            EquationVariable e = new EquationVariable(mSystem, amount);
            mCurrentSide.Add(e);
            return this;
        }

        /// <summary>
        /// Add a fractional constant to the current side of the equation
        /// </summary>
        /// <param name="numerator">   the value of the constant's numerator </param>
        /// <param name="denominator"> the value of the constant's denominator </param>
        /// <returns> this </returns>
        public virtual LinearEquation @var(int numerator, int denominator)
        {
            EquationVariable e = new EquationVariable(new Amount(numerator, denominator));
            mCurrentSide.Add(e);
            return this;
        }

        /// <summary>
        /// Add an unrestricted variable to the current side of the equation
        /// </summary>
        /// <param name="name"> the name of the variable </param>
        /// <returns> this </returns>
        public virtual LinearEquation @var(string name)
        {
            EquationVariable e = new EquationVariable(mSystem, name, SolverVariable.Type.UNRESTRICTED);
            mCurrentSide.Add(e);
            return this;
        }

        /// <summary>
        /// Add an unrestricted variable to the current side of the equation
        /// </summary>
        /// <param name="amount"> the amount of the variable </param>
        /// <param name="name">   the name of the variable </param>
        /// <returns> this </returns>
        public virtual LinearEquation @var(int amount, string name)
        {
            EquationVariable e = new EquationVariable(mSystem, amount, name, SolverVariable.Type.UNRESTRICTED);
            mCurrentSide.Add(e);
            return this;
        }

        /// <summary>
        /// Add an unrestricted fractional variable to the current side of the equation
        /// </summary>
        /// <param name="numerator">   the value of the variable's numerator </param>
        /// <param name="denominator"> the value of the variable's denominator </param>
        /// <param name="name">        the name of the variable </param>
        /// <returns> this </returns>
        public virtual LinearEquation @var(int numerator, int denominator, string name)
        {
            Amount amount = new Amount(numerator, denominator);
            EquationVariable e = new EquationVariable(mSystem, amount, name, SolverVariable.Type.UNRESTRICTED);
            mCurrentSide.Add(e);
            return this;
        }

        /// <summary>
        /// Convenience function to add a variable, based on <seealso cref="LinearEquation#var(String) var)"/>
        /// </summary>
        /// <param name="name"> the variable's name </param>
        /// <returns> this </returns>
        public virtual LinearEquation plus(string name)
        {
            @var(name);
            return this;
        }

        /// <summary>
        /// Convenience function to add a variable, based on <seealso cref="LinearEquation#var(String) var)"/>
        /// </summary>
        /// <param name="amount"> the variable's amount </param>
        /// <param name="name"> the variable's name </param>
        /// <returns> this </returns>
        public virtual LinearEquation plus(int amount, string name)
        {
            @var(amount, name);
            return this;
        }

        /// <summary>
        /// Convenience function to add a negative variable, based on <seealso cref="LinearEquation#var(String) var)"/>
        /// </summary>
        /// <param name="name"> the variable's name </param>
        /// <returns> this </returns>
        public virtual LinearEquation minus(string name)
        {
            @var(-1, name);
            return this;
        }

        /// <summary>
        /// Convenience function to add a negative variable, based on <seealso cref="LinearEquation#var(String) var)"/>
        /// </summary>
        /// <param name="amount"> the variable's amount </param>
        /// <param name="name"> the variable's name </param>
        /// <returns> this </returns>
        public virtual LinearEquation minus(int amount, string name)
        {
            @var(-1 * amount, name);
            return this;
        }

        /// <summary>
        /// Convenience function to add a constant, based on <seealso cref="LinearEquation#var(int) var)"/>
        /// </summary>
        /// <param name="amount"> the constant's amount </param>
        /// <returns> this </returns>
        public virtual LinearEquation plus(int amount)
        {
            @var(amount);
            return this;
        }

        /// <summary>
        /// Convenience function to add a negative constant, based on <seealso cref="LinearEquation#var(int) var)"/>
        /// </summary>
        /// <param name="amount"> the constant's amount </param>
        /// <returns> this </returns>
        public virtual LinearEquation minus(int amount)
        {
            @var(amount * -1);
            return this;
        }

        /// <summary>
        /// Convenience function to add a fractional constant, based on <seealso cref="LinearEquation#var(int) var)"/>
        /// </summary>
        /// <param name="numerator">   the value of the variable's numerator </param>
        /// <param name="denominator"> the value of the variable's denominator </param>
        /// <returns> this </returns>
        public virtual LinearEquation plus(int numerator, int denominator)
        {
            @var(numerator, denominator);
            return this;
        }

        /// <summary>
        /// Convenience function to add a negative fractional constant, based on <seealso cref="LinearEquation#var(int) var)"/>
        /// </summary>
        /// <param name="numerator">   the value of the constant's numerator </param>
        /// <param name="denominator"> the value of the constant's denominator </param>
        /// <returns> this </returns>
        public virtual LinearEquation minus(int numerator, int denominator)
        {
            @var(numerator * -1, denominator);
            return this;
        }

        /// <summary>
        /// Add an error variable to the current side
        /// </summary>
        /// <param name="name">     the name of the error variable </param>
        /// <param name="strength"> the strength of the error variable </param>
        /// <returns> this </returns>
        public virtual LinearEquation withError(string name, int strength)
        {
            EquationVariable e = new EquationVariable(mSystem, strength, name, SolverVariable.Type.ERROR);
            mCurrentSide.Add(e);
            return this;
        }

        public virtual LinearEquation withError(Amount amount, string name)
        {
            EquationVariable e = new EquationVariable(mSystem, amount, name, SolverVariable.Type.ERROR);
            mCurrentSide.Add(e);
            return this;
        }

        /// <summary>
        /// Add an error variable to the current side </summary>
        /// <returns> this </returns>
        public virtual LinearEquation withError()
        {
            string name = NextErrorVariableName;
            withError(name + "+", 1);
            withError(name + "-", -1);
            return this;
        }

        public virtual LinearEquation withPositiveError()
        {
            string name = NextErrorVariableName;
            withError(name + "+", 1);
            return this;
        }

        public virtual EquationVariable addArtificialVar()
        {
            EquationVariable e = new EquationVariable(mSystem, 1, NextArtificialVariableName, SolverVariable.Type.ERROR);
            mCurrentSide.Add(e);
            return e;
        }

        /// <summary>
        /// Add an error variable to the current side
        /// </summary>
        /// <param name="strength"> the strength of the error variable </param>
        /// <returns> this </returns>
        public virtual LinearEquation withError(int strength)
        {
            withError(NextErrorVariableName, strength);
            return this;
        }

        /// <summary>
        /// Add a slack variable to the current side
        /// </summary>
        /// <param name="name">     the name of the slack variable </param>
        /// <param name="strength"> the strength of the slack variable </param>
        /// <returns> this </returns>
        public virtual LinearEquation withSlack(string name, int strength)
        {
            EquationVariable e = new EquationVariable(mSystem, strength, name, SolverVariable.Type.SLACK);
            mCurrentSide.Add(e);
            return this;
        }

        public virtual LinearEquation withSlack(Amount amount, string name)
        {
            EquationVariable e = new EquationVariable(mSystem, amount, name, SolverVariable.Type.SLACK);
            mCurrentSide.Add(e);
            return this;
        }

        /// <summary>
        /// Add a slack variable to the current side </summary>
        /// <returns> this </returns>
        public virtual LinearEquation withSlack()
        {
            withSlack(NextSlackVariableName, 1);
            return this;
        }

        /// <summary>
        /// Add a slack variable to the current side
        /// </summary>
        /// <param name="strength"> the strength of the slack variable </param>
        /// <returns> this </returns>
        public virtual LinearEquation withSlack(int strength)
        {
            withSlack(NextSlackVariableName, strength);
            return this;
        }

        /// <summary>
        /// Override the toString() method to display the linear equation
        /// </summary>
        public override string ToString()
        {
            string result = "";
            result = sideToString(mLeftSide);
            switch (mType)
            {
                case androidx.constraintlayout.core.LinearEquation.Type.EQUALS:
                    {
                        result += "= ";
                        break;
                    }
                case androidx.constraintlayout.core.LinearEquation.Type.LOWER_THAN:
                    {
                        result += "<= ";
                        break;
                    }
                case androidx.constraintlayout.core.LinearEquation.Type.GREATER_THAN:
                    {
                        result += ">= ";
                        break;
                    }
            }
            result += sideToString(mRightSide);
            return result.Trim();
        }

        /// <summary>
        /// Returns a string representation of an array of <seealso cref="EquationVariable"/> </summary>
        /// <param name="side"> array of <seealso cref="EquationVariable"/> </param>
        /// <returns> a String representation of the array of variables </returns>
        private string sideToString(List<EquationVariable> side)
        {
            string result = "";
            bool first = true;
            for (int i = 0, sideSize = side.Count; i < sideSize; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final EquationVariable v = side.get(i);
                EquationVariable v = side[i];
                if (first)
                {
                    if (v.Amount.Positive)
                    {
                        result += v + " ";
                    }
                    else
                    {
                        result += v.signString() + " " + v + " ";
                    }
                    first = false;
                }
                else
                {
                    result += v.signString() + " " + v + " ";
                }
            }
            if (side.Count == 0)
            {
                result = "0";
            }
            return result;
        }
    }

}