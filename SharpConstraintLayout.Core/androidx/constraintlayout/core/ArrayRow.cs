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

	using static androidx.constraintlayout.core.SolverVariable;

	public class ArrayRow : LinearSystem.Row
	{
		private const bool DEBUG = false;

		internal SolverVariable variable = null;
		internal float constantValue = 0;
		internal bool used = false;
		private const bool FULL_NEW_CHECK = false; // full validation (debug purposes)

		internal List<SolverVariable> variablesToUpdate = new List<SolverVariable>();

		public ArrayRowVariables variables;

		public interface ArrayRowVariables
		{
			int CurrentSize {get;}
			SolverVariable getVariable(int i);
			float getVariableValue(int i);
			float get(SolverVariable variable);
			int indexOf(SolverVariable variable);
			void display();
			void clear();
			bool contains(SolverVariable v);
			void put(SolverVariable variable, float value);
			int sizeInBytes();
			void invert();
			float remove(SolverVariable v, bool removeFromDefinition);
			void divideByAmount(float amount);
			void add(SolverVariable @var, float value, bool removeFromDefinition);
			float use(ArrayRow definition, bool removeFromDefinition);
		}

		internal bool isSimpleDefinition = false;

		public ArrayRow()
		{
		}

		public ArrayRow(Cache cache)
		{
			variables = new ArrayLinkedVariables(this, cache);
			//variables = new OptimizedSolverVariableValues(this, cache);
		}

		internal virtual bool hasKeyVariable()
		{
			return !((variable == null) || (variable.mType != SolverVariable.Type.UNRESTRICTED && constantValue < 0));
		}

		public override string ToString()
		{
			return toReadableString();
		}

		internal virtual string toReadableString()
		{
			string s = "";
			if (variable == null)
			{
				s += "0";
			}
			else
			{
				s += variable;
			}
			s += " = ";
			bool addedVariable = false;
			if (constantValue != 0)
			{
				s += constantValue;
				addedVariable = true;
			}
			int count = variables.CurrentSize;
			for (int i = 0; i < count; i++)
			{
				SolverVariable v = variables.getVariable(i);
				if (v == null)
				{
					continue;
				}
				float amount = variables.getVariableValue(i);
				if (amount == 0)
				{
					continue;
				}
				string name = v.ToString();
				if (!addedVariable)
				{
					if (amount < 0)
					{
						s += "- ";
						amount *= -1;
					}
				}
				else
				{
					if (amount > 0)
					{
						s += " + ";
					}
					else
					{
						s += " - ";
						amount *= -1;
					}
				}
				if (amount == 1)
				{
					s += name;
				}
				else
				{
					s += amount + " " + name;
				}
				addedVariable = true;
			}
			if (!addedVariable)
			{
				s += "0.0";
			}
			if (DEBUG)
			{
				variables.display();
			}
			return s;
		}

		public virtual void reset()
		{
			variable = null;
			variables.clear();
			constantValue = 0;
			isSimpleDefinition = false;
		}

		internal virtual bool hasVariable(SolverVariable v)
		{
			return variables.contains(v);
		}

		internal virtual ArrayRow createRowDefinition(SolverVariable variable, int value)
		{
			this.variable = variable;
			variable.computedValue = value;
			constantValue = value;
			isSimpleDefinition = true;
			return this;
		}

		public virtual ArrayRow createRowEquals(SolverVariable variable, int value)
		{
			if (value < 0)
			{
				constantValue = -1 * value;
				variables.put(variable, 1);
			}
			else
			{
				constantValue = value;
				variables.put(variable, -1);
			}
			return this;
		}

		public virtual ArrayRow createRowEquals(SolverVariable variableA, SolverVariable variableB, int margin)
		{
			bool inverse = false;
			if (margin != 0)
			{
				int m = margin;
				if (m < 0)
				{
					m = -1 * m;
					inverse = true;
				}
				constantValue = m;
			}
			if (!inverse)
			{
				variables.put(variableA, -1);
				variables.put(variableB, 1);
			}
			else
			{
				variables.put(variableA, 1);
				variables.put(variableB, -1);
			}
			return this;
		}

		internal virtual ArrayRow addSingleError(SolverVariable error, int sign)
		{
			variables.put(error, (float) sign);
			return this;
		}

		public virtual ArrayRow createRowGreaterThan(SolverVariable variableA, SolverVariable variableB, SolverVariable slack, int margin)
		{
			bool inverse = false;
			if (margin != 0)
			{
				int m = margin;
				if (m < 0)
				{
					m = -1 * m;
					inverse = true;
				}
				constantValue = m;
			}
			if (!inverse)
			{
				variables.put(variableA, -1);
				variables.put(variableB, 1);
				variables.put(slack, 1);
			}
			else
			{
				variables.put(variableA, 1);
				variables.put(variableB, -1);
				variables.put(slack, -1);
			}
			return this;
		}

		public virtual ArrayRow createRowGreaterThan(SolverVariable a, int b, SolverVariable slack)
		{
			constantValue = b;
			variables.put(a, -1);
			return this;
		}

		public virtual ArrayRow createRowLowerThan(SolverVariable variableA, SolverVariable variableB, SolverVariable slack, int margin)
		{
			bool inverse = false;
			if (margin != 0)
			{
				int m = margin;
				if (m < 0)
				{
					m = -1 * m;
					inverse = true;
				}
				constantValue = m;
			}
			if (!inverse)
			{
				variables.put(variableA, -1);
				variables.put(variableB, 1);
				variables.put(slack, -1);
			}
			else
			{
				variables.put(variableA, 1);
				variables.put(variableB, -1);
				variables.put(slack, 1);
			}
			return this;
		}

		public virtual ArrayRow createRowEqualMatchDimensions(float currentWeight, float totalWeights, float nextWeight, SolverVariable variableStartA, SolverVariable variableEndA, SolverVariable variableStartB, SolverVariable variableEndB)
		{
			constantValue = 0;
			if (totalWeights == 0 || (currentWeight == nextWeight))
			{
				// endA - startA == endB - startB
				// 0 = startA - endA + endB - startB
				variables.put(variableStartA, 1);
				variables.put(variableEndA, -1);
				variables.put(variableEndB, 1);
				variables.put(variableStartB, -1);
			}
			else
			{
				if (currentWeight == 0)
				{
					variables.put(variableStartA, 1);
					variables.put(variableEndA, -1);
				}
				else if (nextWeight == 0)
				{
					variables.put(variableStartB, 1);
					variables.put(variableEndB, -1);
				}
				else
				{
					float cw = currentWeight / totalWeights;
					float nw = nextWeight / totalWeights;
					float w = cw / nw;

					// endA - startA == w * (endB - startB)
					// 0 = startA - endA + w * (endB - startB)
					variables.put(variableStartA, 1);
					variables.put(variableEndA, -1);
					variables.put(variableEndB, w);
					variables.put(variableStartB, -w);
				}
			}
			return this;
		}

		public virtual ArrayRow createRowEqualDimension(float currentWeight, float totalWeights, float nextWeight, SolverVariable variableStartA, int marginStartA, SolverVariable variableEndA, int marginEndA, SolverVariable variableStartB, int marginStartB, SolverVariable variableEndB, int marginEndB)
		{
			if (totalWeights == 0 || (currentWeight == nextWeight))
			{
				// endA - startA + marginStartA + marginEndA == endB - startB + marginStartB + marginEndB
				// 0 = startA - endA - marginStartA - marginEndA + endB - startB + marginStartB + marginEndB
				// 0 = (- marginStartA - marginEndA + marginStartB + marginEndB) + startA - endA + endB - startB
				constantValue = -marginStartA - marginEndA + marginStartB + marginEndB;
				variables.put(variableStartA, 1);
				variables.put(variableEndA, -1);
				variables.put(variableEndB, 1);
				variables.put(variableStartB, -1);
			}
			else
			{
				float cw = currentWeight / totalWeights;
				float nw = nextWeight / totalWeights;
				float w = cw / nw;
				// (endA - startA + marginStartA + marginEndA) = w * (endB - startB) + marginStartB + marginEndB;
				// 0 = (startA - endA - marginStartA - marginEndA) + w * (endB - startB) + marginStartB + marginEndB
				// 0 = (- marginStartA - marginEndA + marginStartB + marginEndB) + startA - endA + w * endB - w * startB
				constantValue = - marginStartA - marginEndA + w * marginStartB + w * marginEndB;
				variables.put(variableStartA, 1);
				variables.put(variableEndA, -1);
				variables.put(variableEndB, w);
				variables.put(variableStartB, -w);
			}
			return this;
		}

		internal virtual ArrayRow createRowCentering(SolverVariable variableA, SolverVariable variableB, int marginA, float bias, SolverVariable variableC, SolverVariable variableD, int marginB)
		{
			if (variableB == variableC)
			{
				// centering on the same position
				// B - A == D - B
				// 0 = A + D - 2 * B
				variables.put(variableA, 1);
				variables.put(variableD, 1);
				variables.put(variableB, -2);
				return this;
			}
			if (bias == 0.5f)
			{
				// don't bother applying the bias, we are centered
				// A - B = C - D
				// 0 = A - B - C + D
				// with margin:
				// A - B - Ma = C - D - Mb
				// 0 = A - B - C + D - Ma + Mb
				variables.put(variableA, 1f);
				variables.put(variableB, -1f);
				variables.put(variableC, -1f);
				variables.put(variableD, 1f);
				if (marginA > 0 || marginB > 0)
				{
					constantValue = -marginA + marginB;
				}
			}
			else if (bias <= 0)
			{
				// A = B + m
				variables.put(variableA, -1);
				variables.put(variableB, 1);
				constantValue = marginA;
			}
			else if (bias >= 1)
			{
				// D = C - m
				variables.put(variableD, -1);
				variables.put(variableC, 1);
				constantValue = -marginB;
			}
			else
			{
				variables.put(variableA, 1 * (1 - bias));
				variables.put(variableB, -1 * (1 - bias));
				variables.put(variableC, -1 * bias);
				variables.put(variableD, 1 * bias);
				if (marginA > 0 || marginB > 0)
				{
					constantValue = - marginA * (1 - bias) + marginB * bias;
				}
			}
			return this;
		}

		public virtual ArrayRow addError(LinearSystem system, int strength)
		{
			variables.put(system.createErrorVariable(strength, "ep"), 1);
			variables.put(system.createErrorVariable(strength, "em"), -1);
			return this;
		}

		internal virtual ArrayRow createRowDimensionPercent(SolverVariable variableA, SolverVariable variableC, float percent)
		{
			variables.put(variableA, -1);
			variables.put(variableC, percent);
			return this;
		}

		/// <summary>
		/// Create a constraint to express {@code A = B + (C - D)} * ratio
		/// We use this for ratio, where for example {@code Right = Left + (Bottom - Top) * percent}
		/// </summary>
		/// <param name="variableA"> variable A </param>
		/// <param name="variableB"> variable B </param>
		/// <param name="variableC"> variable C </param>
		/// <param name="variableD"> variable D </param>
		/// <param name="ratio"> ratio between AB and CD </param>
		/// <returns> the row </returns>
		public virtual ArrayRow createRowDimensionRatio(SolverVariable variableA, SolverVariable variableB, SolverVariable variableC, SolverVariable variableD, float ratio)
		{
			// A = B + (C - D) * ratio
			variables.put(variableA, -1);
			variables.put(variableB, 1);
			variables.put(variableC, ratio);
			variables.put(variableD, -ratio);
			return this;
		}

		/// <summary>
		/// Create a constraint to express At + (Ab-At)/2 = Bt + (Bb-Bt)/2 - angle
		/// </summary>
		/// <param name="at"> </param>
		/// <param name="ab"> </param>
		/// <param name="bt"> </param>
		/// <param name="bb"> </param>
		/// <param name="angleComponent">
		/// @return </param>
		public virtual ArrayRow createRowWithAngle(SolverVariable at, SolverVariable ab, SolverVariable bt, SolverVariable bb, float angleComponent)
		{
			variables.put(bt, 0.5f);
			variables.put(bb, 0.5f);
			variables.put(at, -0.5f);
			variables.put(ab, -0.5f);
			constantValue = - angleComponent;
			return this;
		}

		internal virtual int sizeInBytes()
		{
			int size = 0;
			if (variable != null)
			{
				size += 4; // object
			}
			size += 4; // constantValue
			size += 4; // used

			size += variables.sizeInBytes();
			return size;
		}

		internal virtual void ensurePositiveConstant()
		{
			// Ensure that if we have a constant it's positive
			if (constantValue < 0)
			{
				// If not, simply multiply the equation by -1
				constantValue *= -1;
				variables.invert();
			}
		}

		/// <summary>
		/// Pick a subject variable out of the existing ones.
		/// - if a variable is unrestricted
		/// - or if it's a negative new variable (not found elsewhere)
		/// - otherwise we have to add a new additional variable
		/// </summary>
		/// <returns> true if we added an extra variable to the system </returns>
		internal virtual bool chooseSubject(LinearSystem system)
		{
			bool addedExtra = false;
			SolverVariable pivotCandidate = chooseSubjectInVariables(system);
			if (pivotCandidate == null)
			{
				// need to add extra variable
				addedExtra = true;
			}
			else
			{
				pivot(pivotCandidate);
			}
			if (variables.CurrentSize == 0)
			{
				isSimpleDefinition = true;
			}
			return addedExtra;
		}

		/// <summary>
		/// Pick a subject variable out of the existing ones.
		/// - if a variable is unrestricted
		/// - or if it's a negative new variable (not found elsewhere)
		/// - otherwise we return null
		/// </summary>
		/// <returns> a candidate variable we can pivot on or null if not found </returns>
		internal virtual SolverVariable chooseSubjectInVariables(LinearSystem system)
		{
			// if unrestricted, pick it
			// if restricted, needs to be < 0 and new
			//
			SolverVariable restrictedCandidate = null;
			SolverVariable unrestrictedCandidate = null;
			float unrestrictedCandidateAmount = 0;
			float restrictedCandidateAmount = 0;
			bool unrestrictedCandidateIsNew = false;
			bool restrictedCandidateIsNew = false;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int currentSize = variables.getCurrentSize();
			int currentSize = variables.CurrentSize;
			for (int i = 0; i < currentSize; i++)
			{
				float amount = variables.getVariableValue(i);
				SolverVariable variable = variables.getVariable(i);
				if (variable.mType == SolverVariable.Type.UNRESTRICTED)
				{
					if (unrestrictedCandidate == null)
					{
						unrestrictedCandidate = variable;
						unrestrictedCandidateAmount = amount;
						unrestrictedCandidateIsNew = isNew(variable, system);
					}
					else if (unrestrictedCandidateAmount > amount)
					{
						unrestrictedCandidate = variable;
						unrestrictedCandidateAmount = amount;
						unrestrictedCandidateIsNew = isNew(variable, system);
					}
					else if (!unrestrictedCandidateIsNew && isNew(variable, system))
					{
						unrestrictedCandidate = variable;
						unrestrictedCandidateAmount = amount;
						unrestrictedCandidateIsNew = true;
					}
				}
				else if (unrestrictedCandidate == null)
				{
					if (amount < 0)
					{
						if (restrictedCandidate == null)
						{
							restrictedCandidate = variable;
							restrictedCandidateAmount = amount;
							restrictedCandidateIsNew = isNew(variable, system);
						}
						else if (restrictedCandidateAmount > amount)
						{
							restrictedCandidate = variable;
							restrictedCandidateAmount = amount;
							restrictedCandidateIsNew = isNew(variable, system);
						}
						else if (!restrictedCandidateIsNew && isNew(variable, system))
						{
							restrictedCandidate = variable;
							restrictedCandidateAmount = amount;
							restrictedCandidateIsNew = true;
						}
					}
				}
			}

			if (unrestrictedCandidate != null)
			{
				return unrestrictedCandidate;
			}
			return restrictedCandidate;
		}

		/// <summary>
		/// Returns true if the variable is new to the system, i.e. is already present
		/// in one of the rows. This function is called while choosing the subject of a new row.
		/// </summary>
		/// <param name="variable"> the variable to check for </param>
		/// <param name="system"> the linear system we check
		/// @return </param>
		private bool isNew(SolverVariable variable, LinearSystem system)
		{
			if (FULL_NEW_CHECK)
			{
				bool isNew = true;
				for (int i = 0; i < system.mNumRows; i++)
				{
					ArrayRow row = system.mRows[i];
					if (row.hasVariable(variable))
					{
						isNew = false;
					}
				}
				if (variable.usageInRowCount <= 1 != isNew)
				{
					Console.WriteLine("Problem with usage tracking");
				}
				return isNew;
			}
			// We maintain a usage count -- variables are ref counted if they are present
			// in the right side of a row or not. If the count is zero or one, the variable
			// is new (if one, it means it exist in a row, but this is the row we insert)
			return variable.usageInRowCount <= 1;
		}

		internal virtual void pivot(SolverVariable v)
		{
			if (variable != null)
			{
				// first, move back the variable to its column
				variables.put(variable, -1f);
				variable.definitionId = -1;
				variable = null;
			}

			float amount = variables.remove(v, true) * -1;
			variable = v;
			if (amount == 1)
			{
				return;
			}
			constantValue = constantValue / amount;
			variables.divideByAmount(amount);
		}

		// Row compatibility

		public virtual bool Empty
		{
			get
			{
				return (variable == null && constantValue == 0 && variables.CurrentSize == 0);
			}
		}

		public virtual void updateFromRow(LinearSystem system, ArrayRow definition, bool removeFromDefinition)
		{
			float value = variables.use(definition, removeFromDefinition);

			constantValue += definition.constantValue * value;
			if (removeFromDefinition)
			{
				definition.variable.removeFromRow(this);
			}
			if (LinearSystem.SIMPLIFY_SYNONYMS && variable != null && variables.CurrentSize == 0)
			{
				isSimpleDefinition = true;
				system.hasSimpleDefinition = true;
			}
		}

		public virtual void updateFromFinalVariable(LinearSystem system, SolverVariable variable, bool removeFromDefinition)
		{
			if (variable == null || !variable.isFinalValue)
			{
				return;
			}
			float value = variables.get(variable);
			constantValue += variable.computedValue * value;
			variables.remove(variable, removeFromDefinition);
			if (removeFromDefinition)
			{
				variable.removeFromRow(this);
			}
			if (LinearSystem.SIMPLIFY_SYNONYMS && variables.CurrentSize == 0)
			{
				isSimpleDefinition = true;
				system.hasSimpleDefinition = true;
			}
		}

		public virtual void updateFromSynonymVariable(LinearSystem system, SolverVariable variable, bool removeFromDefinition)
		{
			if (variable == null || !variable.isSynonym)
			{
				return;
			}
			float value = variables.get(variable);
			constantValue += variable.synonymDelta * value;
			variables.remove(variable, removeFromDefinition);
			if (removeFromDefinition)
			{
				variable.removeFromRow(this);
			}
			variables.add(system.mCache.mIndexedVariables[variable.synonym], value, removeFromDefinition);
			if (LinearSystem.SIMPLIFY_SYNONYMS && variables.CurrentSize == 0)
			{
				isSimpleDefinition = true;
				system.hasSimpleDefinition = true;
			}
		}

		private SolverVariable pickPivotInVariables(bool[] avoid, SolverVariable exclude)
		{
			bool all = true;
			float value = 0;
			SolverVariable pivot = null;
			SolverVariable pivotSlack = null;
			float valueSlack = 0;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int currentSize = variables.getCurrentSize();
			int currentSize = variables.CurrentSize;
			for (int i = 0; i < currentSize; i++)
			{
				float currentValue = variables.getVariableValue(i);
				if (currentValue < 0)
				{
					// We can return the first negative candidate as in ArrayLinkedVariables
					// they are already sorted by id
					SolverVariable v = variables.getVariable(i);
					if (!((avoid != null && avoid[v.id]) || (v == exclude)))
					{
						if (all)
						{
							if (v.mType == SolverVariable.Type.SLACK || v.mType == SolverVariable.Type.ERROR)
							{
								if (currentValue < value)
								{
									value = currentValue;
									pivot = v;
								}
							}
						}
						else
						{
							if (v.mType == SolverVariable.Type.SLACK)
							{
								if (currentValue < valueSlack)
								{
									valueSlack = currentValue;
									pivotSlack = v;
								}
							}
							else if (v.mType == SolverVariable.Type.ERROR)
							{
								if (currentValue < value)
								{
									value = currentValue;
									pivot = v;
								}
							}
						}
					}
				}
			}
			if (all)
			{
				return pivot;
			}
			return pivot != null ? pivot : pivotSlack;
		}

		public virtual SolverVariable pickPivot(SolverVariable exclude)
		{
			return pickPivotInVariables(null, exclude);
		}

		public virtual SolverVariable getPivotCandidate(LinearSystem system, bool[] avoid)
		{
			return pickPivotInVariables(avoid, null);
		}

		public virtual void clear()
		{
			variables.clear();
			variable = null;
			constantValue = 0;
		}

		/// <summary>
		/// Used to initiate a goal from a given row (to see if we can remove an extra var) </summary>
		/// <param name="row"> </param>
		public virtual void initFromRow(LinearSystem.Row row)
		{
			if (row is ArrayRow)
			{
				ArrayRow copiedRow = (ArrayRow) row;
				variable = null;
				variables.clear();
				for (int i = 0; i < copiedRow.variables.CurrentSize; i++)
				{
					SolverVariable @var = copiedRow.variables.getVariable(i);
					float val = copiedRow.variables.getVariableValue(i);
					variables.add(@var, val, true);
				}
			}
		}

		public virtual void addError(SolverVariable error)
		{
			float weight = 1;
			if (error.strength == STRENGTH_LOW)
			{
				weight = 1F;
			}
			else if (error.strength == STRENGTH_MEDIUM)
			{
				weight = 1E3F;
			}
			else if (error.strength == STRENGTH_HIGH)
			{
				weight = 1E6F;
			}
			else if (error.strength == STRENGTH_HIGHEST)
			{
				weight = 1E9F;
			}
			else if (error.strength == STRENGTH_EQUALITY)
			{
				weight = 1E12F;
			}
			variables.put(error, weight);
		}

		public virtual SolverVariable Key
		{
			get
			{
				return variable;
			}
		}

		public virtual void updateFromSystem(LinearSystem system)
		{
			if (system.mRows.Length == 0)
			{
				return;
			}

			bool done = false;
			while (!done)
			{
				int currentSize = variables.CurrentSize;
				for (int i = 0; i < currentSize; i++)
				{
					SolverVariable variable = variables.getVariable(i);
					if (variable.definitionId != -1 || variable.isFinalValue || variable.isSynonym)
					{
						variablesToUpdate.Add(variable);
					}
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = variablesToUpdate.size();
				int size = variablesToUpdate.Count;
				if (size > 0)
				{
					for (int i = 0; i < size; i++)
					{
						SolverVariable variable = variablesToUpdate[i];
						if (variable.isFinalValue)
						{
							updateFromFinalVariable(system, variable, true);
						}
						else if (variable.isSynonym)
						{
							updateFromSynonymVariable(system, variable, true);
						}
						else
						{
							updateFromRow(system, system.mRows[variable.definitionId], true);
						}
					}
					variablesToUpdate.Clear();
				}
				else
				{
					done = true;
				}
			}
			if (LinearSystem.SIMPLIFY_SYNONYMS && variable != null && variables.CurrentSize == 0)
			{
				isSimpleDefinition = true;
				system.hasSimpleDefinition = true;
			}
		}
	}

}