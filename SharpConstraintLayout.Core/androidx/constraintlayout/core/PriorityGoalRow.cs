using System;
using System.Collections.Generic;

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

namespace androidx.constraintlayout.core
{

	/// <summary>
	/// Implements a row containing goals taking in account priorities.
	/// </summary>
	public class PriorityGoalRow : ArrayRow
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			arrayGoals = new SolverVariable[TABLE_SIZE];
			sortArray = new SolverVariable[TABLE_SIZE];
			accessor = new GoalVariableAccessor(this, this);
		}

		private const float epsilon = 0.0001f;
		private const bool DEBUG = false;

		private int TABLE_SIZE = 128;
		private SolverVariable[] arrayGoals;
		private SolverVariable[] sortArray;
		private int numGoals = 0;
		internal GoalVariableAccessor accessor;

		internal class GoalVariableAccessor
		{
			private readonly PriorityGoalRow outerInstance;

			internal SolverVariable variable;
			internal PriorityGoalRow row;

			public GoalVariableAccessor(PriorityGoalRow outerInstance, PriorityGoalRow row)
			{
				this.outerInstance = outerInstance;
				this.row = row;
			}

			public virtual void init(SolverVariable variable)
			{
				this.variable = variable;
			}

			public virtual bool addToGoal(SolverVariable other, float value)
			{
				if (variable.inGoal)
				{
					bool empty = true;
					for (int i = 0; i < SolverVariable.MAX_STRENGTH; i++)
					{
						variable.goalStrengthVector[i] += other.goalStrengthVector[i] * value;
						float v = variable.goalStrengthVector[i];
						if (Math.Abs(v) < epsilon)
						{
							variable.goalStrengthVector[i] = 0;
						}
						else
						{
							empty = false;
						}
					}
					if (empty)
					{
						outerInstance.removeGoal(variable);
					}
				}
				else
				{
					for (int i = 0; i < SolverVariable.MAX_STRENGTH; i++)
					{
						float strength = other.goalStrengthVector[i];
						if (strength != 0)
						{
							float v = value * strength;
							if (Math.Abs(v) < epsilon)
							{
								v = 0;
							}
							variable.goalStrengthVector[i] = v;
						}
						else
						{
							variable.goalStrengthVector[i] = 0;
						}
					}
					return true;
				}
				return false;
			}

			public virtual void add(SolverVariable other)
			{
				for (int i = 0; i < SolverVariable.MAX_STRENGTH; i++)
				{
					variable.goalStrengthVector[i] += other.goalStrengthVector[i];
					float value = variable.goalStrengthVector[i];
					if (Math.Abs(value) < epsilon)
					{
						variable.goalStrengthVector[i] = 0;
					}
				}
			}

			public bool Negative
			{
				get
				{
					for (int i = SolverVariable.MAX_STRENGTH - 1; i >= 0; i--)
					{
						float value = variable.goalStrengthVector[i];
						if (value > 0)
						{
							return false;
						}
						if (value < 0)
						{
							return true;
						}
					}
					return false;
				}
			}

			public bool isSmallerThan(SolverVariable other)
			{
				for (int i = SolverVariable.MAX_STRENGTH - 1; i >= 0 ; i--)
				{
					float comparedValue = other.goalStrengthVector[i];
					float value = variable.goalStrengthVector[i];
					if (value == comparedValue)
					{
						continue;
					}
					if (value < comparedValue)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				return false;
			}

			public bool Null
			{
				get
				{
					for (int i = 0; i < SolverVariable.MAX_STRENGTH; i++)
					{
						if (variable.goalStrengthVector[i] != 0)
						{
							return false;
						}
					}
					return true;
				}
			}

			public virtual void reset()
			{
				//Array.fill(variable.goalStrengthVector, 0);
				for(var index= 0; index<variable.goalStrengthVector.Length-1;index++)
					variable.goalStrengthVector[index] = 0;
			}

			public override string ToString()
			{
				string result = "[ ";
				if (variable != null)
				{
					for (int i = 0; i < SolverVariable.MAX_STRENGTH; i++)
					{
						result += variable.goalStrengthVector[i] + " ";
					}
				}
				result += "] " + variable;
				return result;
			}

		}

		public override void clear()
		{
			numGoals = 0;
			constantValue = 0;
		}

		internal Cache mCache;

		public PriorityGoalRow(Cache cache) : base(cache)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			mCache = cache;
		}

		public override bool Empty
		{
			get
			{
				return numGoals == 0;
			}
		}

		internal const int NOT_FOUND = -1;

		public override SolverVariable getPivotCandidate(LinearSystem system, bool[] avoid)
		{
			int pivot = NOT_FOUND;
			for (int i = 0; i < numGoals; i++)
			{
				SolverVariable variable = arrayGoals[i];
				if (avoid[variable.id])
				{
					continue;
				}
				accessor.init(variable);
				if (pivot == NOT_FOUND)
				{
					if (accessor.Negative)
					{
						pivot = i;
					}
				}
				else if (accessor.isSmallerThan(arrayGoals[pivot]))
				{
					pivot = i;
				}
			}
			if (pivot == NOT_FOUND)
			{
				return null;
			}
			return arrayGoals[pivot];
		}

		public override void addError(SolverVariable error)
		{
			accessor.init(error);
			accessor.reset();
			error.goalStrengthVector[error.strength] = 1;
			addToGoal(error);
		}

		private void addToGoal(SolverVariable variable)
		{
			if (numGoals + 1 > arrayGoals.Length)
			{
				arrayGoals = arrayGoals.Copy<SolverVariable>( arrayGoals.Length * 2);
				sortArray = arrayGoals.Copy<SolverVariable>(arrayGoals.Length * 2);
			}
			arrayGoals[numGoals] = variable;
			numGoals++;

			if (numGoals > 1 && arrayGoals[numGoals - 1].id > variable.id)
			{
				for (int i = 0; i < numGoals; i++)
				{
					sortArray[i] = arrayGoals[i];
				}
				Array.Sort(sortArray, 0, numGoals, new ComparatorAnonymousInnerClass(this));
				for (int i = 0; i < numGoals; i++)
				{
					arrayGoals[i] = sortArray[i];
				}
			}

			variable.inGoal = true;
			variable.addToRow(this);
		}

		private class ComparatorAnonymousInnerClass : IComparer<SolverVariable>
		{
			private readonly PriorityGoalRow outerInstance;

			public ComparatorAnonymousInnerClass(PriorityGoalRow outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual int Compare(SolverVariable variable1, SolverVariable variable2)
			{
				return variable1.id - variable2.id;
			}
		}

		private void removeGoal(SolverVariable variable)
		{
			for (int i = 0; i < numGoals; i++)
			{
				if (arrayGoals[i] == variable)
				{
					for (int j = i; j < numGoals - 1; j++)
					{
						arrayGoals[j] = arrayGoals[j + 1];
					}
					numGoals--;
					variable.inGoal = false;
					return;
				}
			}
		}

		public override void updateFromRow(LinearSystem system, ArrayRow definition, bool removeFromDefinition)
		{
			SolverVariable goalVariable = definition.variable;
			if (goalVariable == null)
			{
				return;
			}

			ArrayRowVariables rowVariables = definition.variables;
			int currentSize = rowVariables.CurrentSize;
			for (int i = 0; i < currentSize; i++)
			{
				SolverVariable solverVariable = rowVariables.getVariable(i);
				float value = rowVariables.getVariableValue(i);
				accessor.init(solverVariable);
				if (accessor.addToGoal(goalVariable, value))
				{
					addToGoal(solverVariable);
				}
				constantValue += definition.constantValue * value;
			}
			removeGoal(goalVariable);
		}

		public override string ToString()
		{
			string result = "";
			result += " goal -> (" + constantValue + ") : ";
			for (int i = 0; i < numGoals; i++)
			{
				SolverVariable v = arrayGoals[i];
				accessor.init(v);
				result += accessor + " ";
			}
			return result;
		}
	}
}