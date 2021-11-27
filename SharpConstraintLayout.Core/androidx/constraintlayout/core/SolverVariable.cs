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

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.LinearSystem.FULL_DEBUG;
using static androidx.constraintlayout.core.LinearSystem;

	/// <summary>
	/// Represents a given variable used in the <seealso cref="LinearSystem linear expression solver"/>.
	/// </summary>
	public class SolverVariable : IComparable<SolverVariable>
	{

		private static readonly bool INTERNAL_DEBUG = FULL_DEBUG;
		private const bool VAR_USE_HASH = false;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static final int STRENGTH_NONE = 0;
		public const int STRENGTH_NONE = 0;
		public const int STRENGTH_LOW = 1;
		public const int STRENGTH_MEDIUM = 2;
		public const int STRENGTH_HIGH = 3;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("WeakerAccess") public static final int STRENGTH_HIGHEST = 4;
		public const int STRENGTH_HIGHEST = 4;
		public const int STRENGTH_EQUALITY = 5;
		public const int STRENGTH_BARRIER = 6;
		public const int STRENGTH_CENTERING = 7;
		public const int STRENGTH_FIXED = 8;

		private static int uniqueSlackId = 1;
		private static int uniqueErrorId = 1;
		private static int uniqueUnrestrictedId = 1;
		private static int uniqueConstantId = 1;
		private static int uniqueId = 1;
		public bool inGoal;

		private string mName;

		public int id = -1;
		internal int definitionId = -1;
		public int strength = 0;
		public float computedValue;
		public bool isFinalValue = false;

		internal const int MAX_STRENGTH = 9;
		internal float[] strengthVector = new float[MAX_STRENGTH];
		internal float[] goalStrengthVector = new float[MAX_STRENGTH];

		internal Type mType;

		internal ArrayRow[] mClientEquations = new ArrayRow[16];
		internal int mClientEquationsCount = 0;
		public int usageInRowCount = 0;
		internal bool isSynonym = false;
		internal int synonym = -1;
		internal float synonymDelta = 0;

		/// <summary>
		/// Type of variables
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// The variable can take negative or positive values
			/// </summary>
			UNRESTRICTED,
			/// <summary>
			/// The variable is actually not a variable :) , but a constant number
			/// </summary>
			CONSTANT,
			/// <summary>
			/// The variable is restricted to positive values and represents a slack
			/// </summary>
			SLACK,
			/// <summary>
			/// The variable is restricted to positive values and represents an error
			/// </summary>
			ERROR,
			/// <summary>
			/// Unknown (invalid) type.
			/// </summary>
			UNKNOWN
		}

		internal static void increaseErrorId()
		{
			uniqueErrorId++;
		}

		private static string getUniqueName(Type type, string prefix)
		{
			if (!string.ReferenceEquals(prefix, null))
			{
				return prefix + uniqueErrorId;
			}
			switch (type)
			{
				case androidx.constraintlayout.core.SolverVariable.Type.UNRESTRICTED:
					return "U" + ++uniqueUnrestrictedId;
				case androidx.constraintlayout.core.SolverVariable.Type.CONSTANT:
					return "C" + ++uniqueConstantId;
				case androidx.constraintlayout.core.SolverVariable.Type.SLACK:
					return "S" + ++uniqueSlackId;
				case androidx.constraintlayout.core.SolverVariable.Type.ERROR:
				{
					return "e" + ++uniqueErrorId;
				}
				case androidx.constraintlayout.core.SolverVariable.Type.UNKNOWN:
					return "V" + ++uniqueId;
			}
			throw new AssertionError(type.ToString());
		}

		/// <summary>
		/// Base constructor </summary>
		///  <param name="name"> the variable name </param>
		/// <param name="type"> the type of the variable </param>
		public SolverVariable(string name, Type type)
		{
			mName = name;
			mType = type;
		}

		public SolverVariable(Type type, string prefix)
		{
			mType = type;
			if (INTERNAL_DEBUG)
			{
				//mName = getUniqueName(type, prefix);
			}
		}

		internal virtual void clearStrengths()
		{
			for (int i = 0; i < MAX_STRENGTH; i++)
			{
				strengthVector[i] = 0;
			}
		}

		internal virtual string strengthsToString()
		{
			string representation = this + "[";
			bool negative = false;
			bool empty = true;
			for (int j = 0; j < strengthVector.Length; j++)
			{
				representation += strengthVector[j];
				if (strengthVector[j] > 0)
				{
					negative = false;
				}
				else if (strengthVector[j] < 0)
				{
					negative = true;
				}
				if (strengthVector[j] != 0)
				{
					empty = false;
				}
				if (j < strengthVector.Length - 1)
				{
					representation += ", ";
				}
				else
				{
					representation += "] ";
				}
			}
			if (negative)
			{
				representation += " (-)";
			}
			if (empty)
			{
				representation += " (*)";
			}
			// representation += " {id: " + id + "}";
			return representation;
		}

		internal HashSet<ArrayRow> inRows = VAR_USE_HASH ? new HashSet<ArrayRow>() : null;

		public void addToRow(ArrayRow row)
		{
			if (VAR_USE_HASH)
			{
				inRows.Add(row);
			}
			else
			{
				for (int i = 0; i < mClientEquationsCount; i++)
				{
					if (mClientEquations[i] == row)
					{
						return;
					}
				}
				if (mClientEquationsCount >= mClientEquations.Length)
				{
					var newMClientEquations = new ArrayRow[mClientEquations.Length * 2];
					Array.Copy(mClientEquations,newMClientEquations, mClientEquations.Length);
					mClientEquations = newMClientEquations;
				}
				mClientEquations[mClientEquationsCount] = row;
				mClientEquationsCount++;
			}
		}

		public void removeFromRow(ArrayRow row)
		{
			if (VAR_USE_HASH)
			{
				inRows.Remove(row);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mClientEquationsCount;
				int count = mClientEquationsCount;
				for (int i = 0; i < count; i++)
				{
					if (mClientEquations[i] == row)
					{
						for (int j = i; j < count - 1; j++)
						{
							mClientEquations[j] = mClientEquations[j + 1];
						}
						mClientEquationsCount--;
						return;
					}
				}
			}
		}

		public void updateReferencesWithNewDefinition(LinearSystem system, ArrayRow definition)
		{
			if (VAR_USE_HASH)
			{
				foreach (ArrayRow row in inRows)
				{
					row.updateFromRow(system, definition, false);
				}
				inRows.Clear();
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mClientEquationsCount;
				int count = mClientEquationsCount;
				for (int i = 0; i < count; i++)
				{
					mClientEquations[i].updateFromRow(system, definition, false);
				}
				mClientEquationsCount = 0;
			}
		}

		public virtual void setFinalValue(LinearSystem system, float value)
		{
			if (false && INTERNAL_DEBUG)
			{
				Console.WriteLine("Set final value for " + this + " of " + value);
			}
			computedValue = value;
			isFinalValue = true;
			isSynonym = false;
			synonym = -1;
			synonymDelta = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mClientEquationsCount;
			int count = mClientEquationsCount;
			definitionId = -1;
			for (int i = 0; i < count; i++)
			{
				mClientEquations[i].updateFromFinalVariable(system,this, false);
			}
			mClientEquationsCount = 0;
		}

		public virtual void setSynonym(LinearSystem system, SolverVariable synonymVariable, float value)
		{
			if (INTERNAL_DEBUG)
			{
				Console.WriteLine("Set synonym for " + this + " = " + synonymVariable + " + " + value);
			}
			isSynonym = true;
			synonym = synonymVariable.id;
			synonymDelta = value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mClientEquationsCount;
			int count = mClientEquationsCount;
			definitionId = -1;
			for (int i = 0; i < count; i++)
			{
				mClientEquations[i].updateFromSynonymVariable(system,this, false);
			}
			mClientEquationsCount = 0;
			system.displayReadableRows();
		}

		public virtual void reset()
		{
			mName = null;
			mType = Type.UNKNOWN;
			strength = SolverVariable.STRENGTH_NONE;
			id = -1;
			definitionId = -1;
			computedValue = 0;
			isFinalValue = false;
			isSynonym = false;
			synonym = -1;
			synonymDelta = 0;
			if (VAR_USE_HASH)
			{
				inRows.Clear();
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mClientEquationsCount;
				int count = mClientEquationsCount;
				for (int i = 0; i < count; i++)
				{
					mClientEquations[i] = null;
				}
				mClientEquationsCount = 0;
			}
			usageInRowCount = 0;
			inGoal = false;
			for(var index=0;index<goalStrengthVector.Length;index++)
			 goalStrengthVector[index]= 0;
		}

		/// <summary>
		/// Accessor for the name
		/// </summary>
		/// <returns> the name of the variable </returns>
		public virtual string Name
		{
			get
			{
				return mName;
			}
			set
			{
				mName = value;
			}
		}

		public virtual void setType(Type type, string prefix)
		{
			mType = type;
			if (INTERNAL_DEBUG && string.ReferenceEquals(mName, null))
			{
				mName = getUniqueName(type, prefix);
			}
		}

		public virtual int CompareTo(SolverVariable v)
		{
			return this.id - v.id;
		}

		/// <summary>
		/// Override the toString() method to display the variable
		/// </summary>
		public override string ToString()
		{
			string result = "";
			if (INTERNAL_DEBUG)
			{
				result += mName + "(" + id + "):" + strength;
				if (isSynonym)
				{
					result += ":S(" + synonym + ")";
				}
				if (isFinalValue)
				{
					result += ":F(" + computedValue + ")";
				}
			}
			else
			{
				if (!string.ReferenceEquals(mName, null))
				{
					result += mName;
				}
				else
				{
					result += id;
				}
			}
			return result;
		}

	}

}