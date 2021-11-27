using System;

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
	/// Store a set of variables and their values in an array.
	/// </summary>
	internal class ArrayBackedVariables
	{
		private const bool DEBUG = false;

		private SolverVariable[] variables = null;
		private float[] values = null;
		private int[] indexes = null;
		private readonly int threshold = 16;
		private int maxSize = 4;
		private int currentSize = 0;
		private int currentWriteSize = 0;
		private SolverVariable candidate = null;

		public ArrayBackedVariables(ArrayRow arrayRow, Cache cache)
		{
			variables = new SolverVariable[maxSize];
			values = new float[maxSize];
			indexes = new int[maxSize];
		}

		public virtual SolverVariable PivotCandidate
		{
			get
			{
				if (candidate == null)
				{
					for (int i = 0; i < currentSize; i++)
					{
						int idx = indexes[i];
						if (values[idx] < 0)
						{
							candidate = variables[idx];
							break;
						}
					}
				}
				return candidate;
			}
		}

		internal virtual void increaseSize()
		{
			maxSize *= 2;
			variables = variables.Copy<SolverVariable>(maxSize);
			values = values.Copy<float>(maxSize);
			indexes = indexes.Copy<int>(maxSize);
		}

		public int size()
		{
			return currentSize;
		}

		public SolverVariable getVariable(int index)
		{
			return variables[indexes[index]];
		}

		public float getVariableValue(int index)
		{
			return values[indexes[index]];
		}

		public void updateArray(ArrayBackedVariables target, float amount)
		{
			if (amount == 0)
			{
				return;
			}
			for (int i = 0; i < currentSize; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int idx = indexes[i];
				int idx = indexes[i];
				SolverVariable v = variables[idx];
				float value = values[idx];
				target.add(v, (value * amount));
			}
		}

		public virtual void setVariable(int index, float value)
		{
			int idx = indexes[index];
			values[idx] = value;
			if (value < 0)
			{
				candidate = variables[idx];
			}
		}

		public float get(SolverVariable v)
		{
			if (currentSize < threshold)
			{
				for (int i = 0; i < currentSize; i++)
				{
					int idx = indexes[i];
					if (variables[idx] == v)
					{
						return values[idx];
					}
				}
			}
			else
			{
				int start = 0;
				int end = currentSize - 1;
				while (start <= end)
				{
					int index = start + (end - start) / 2;
					int idx = indexes[index];
					SolverVariable current = variables[idx];
					if (current == v)
					{
						return values[idx];
					}
					else if (current.id < v.id)
					{
						start = index + 1;
					}
					else
					{
						end = index - 1;
					}
				}
			}
			return 0;
		}

		public virtual void put(SolverVariable variable, float value)
		{
			if (value == 0)
			{
				remove(variable);
				return;
			}
			while (true)
			{
				int firstEmptyIndex = -1;
				for (int i = 0; i < currentWriteSize; i++)
				{
					if (variables[i] == variable)
					{
						values[i] = value;
						if (value < 0)
						{
							candidate = variable;
						}
						return;
					}
					if (firstEmptyIndex == -1 && variables[i] == null)
					{
						firstEmptyIndex = i;
					}
				}
				if (firstEmptyIndex == -1 && currentWriteSize < maxSize)
				{
					firstEmptyIndex = currentWriteSize;
				}
				if (firstEmptyIndex != -1)
				{
					variables[firstEmptyIndex] = variable;
					values[firstEmptyIndex] = value;
					// insert the position...
					bool inserted = false;
					for (int j = 0; j < currentSize; j++)
					{
						int index = indexes[j];
						if (variables[index].id > variable.id)
						{
							// this is our insertion point
							Array.Copy(indexes, j, indexes, j + 1, (currentSize - j));
							indexes[j] = firstEmptyIndex;
							inserted = true;
							break;
						}
					}
					if (!inserted)
					{
						indexes[currentSize] = firstEmptyIndex;
					}
					currentSize++;
					if (firstEmptyIndex + 1 > currentWriteSize)
					{
						currentWriteSize = firstEmptyIndex + 1;
					}
					if (value < 0)
					{
						candidate = variable;
					}
					return;
				}
				else
				{
					increaseSize();
				}
			}
		}

		public virtual void add(SolverVariable variable, float value)
		{
			if (value == 0)
			{
				return;
			}
			while (true)
			{
				int firstEmptyIndex = -1;
				for (int i = 0; i < currentWriteSize; i++)
				{
					if (variables[i] == variable)
					{
						values[i] += value;
						if (value < 0)
						{
							candidate = variable;
						}
						if (values[i] == 0)
						{
							remove(variable);
						}
						return;
					}
					if (firstEmptyIndex == -1 && variables[i] == null)
					{
						firstEmptyIndex = i;
					}
				}
				if (firstEmptyIndex == -1 && currentWriteSize < maxSize)
				{
					firstEmptyIndex = currentWriteSize;
				}
				if (firstEmptyIndex != -1)
				{
					variables[firstEmptyIndex] = variable;
					values[firstEmptyIndex] = value;
					// insert the position...
					bool inserted = false;
					for (int j = 0; j < currentSize; j++)
					{
						int index = indexes[j];
						if (variables[index].id > variable.id)
						{
							// this is our insertion point
							Array.Copy(indexes, j, indexes, j + 1, (currentSize - j));
							indexes[j] = firstEmptyIndex;
							inserted = true;
							break;
						}
					}
					if (!inserted)
					{
						indexes[currentSize] = firstEmptyIndex;
					}
					currentSize++;
					if (firstEmptyIndex + 1 > currentWriteSize)
					{
						currentWriteSize = firstEmptyIndex + 1;
					}
					if (value < 0)
					{
						candidate = variable;
					}
					return;
				}
				else
				{
					increaseSize();
				}
			}
		}

		public virtual void clear()
		{
			for (int i = 0, length = variables.Length; i < length; i++)
			{
				variables[i] = null;
			}
			currentSize = 0;
			currentWriteSize = 0;
		}

		public virtual bool containsKey(SolverVariable variable)
		{
			if (currentSize < 8)
			{
				for (int i = 0; i < currentSize; i++)
				{
					if (variables[indexes[i]] == variable)
					{
						return true;
					}
				}
			}
			else
			{
				int start = 0;
				int end = currentSize - 1;
				while (start <= end)
				{
					int index = start + (end - start) / 2;
					SolverVariable current = variables[indexes[index]];
					if (current == variable)
					{
						return true;
					}
					else if (current.id < variable.id)
					{
						start = index + 1;
					}
					else
					{
						end = index - 1;
					}
				}
			}
			return false;
		}

		public virtual float remove(SolverVariable variable)
		{
			if (DEBUG)
			{
				Console.Write("BEFORE REMOVE " + variable + " -> ");
				display();
			}
			if (candidate == variable)
			{
				candidate = null;
			}
			for (int i = 0; i < currentWriteSize; i++)
			{
				int idx = indexes[i];
				if (variables[idx] == variable)
				{
					float amount = values[idx];
					variables[idx] = null;
					Array.Copy(indexes, i + 1, indexes, i, (currentWriteSize - i - 1));
					currentSize--;
					if (DEBUG)
					{
						Console.Write("AFTER REMOVE ");
						display();
					}
					return amount;
				}
			}
			return 0;
		}

		public virtual int sizeInBytes()
		{
			int size = 0;
			size += (maxSize * 4);
			size += (maxSize * 4);
			size += (maxSize * 4);
			size += 4 + 4 + 4 + 4;
			return size;
		}

		public virtual void display()
		{
			int count = size();
			Console.Write("{ ");
			for (int i = 0; i < count; i++)
			{
				Console.Write(getVariable(i) + " = " + getVariableValue(i) + " ");
			}
			Console.WriteLine(" }");
		}

		private string InternalArrays
		{
			get
			{
				string str = "";
				int count = size();
				str += "idx { ";
				for (int i = 0; i < count; i++)
				{
					str += indexes[i] + " ";
				}
				str += "}\n";
				str += "obj { ";
				for (int i = 0; i < count; i++)
				{
					str += variables[i] + ":" + values[i] + " ";
				}
				str += "}\n";
				return str;
			}
		}

		public virtual void displayInternalArrays()
		{
			int count = size();
			Console.Write("idx { ");
			for (int i = 0; i < count; i++)
			{
				Console.Write(indexes[i] + " ");
			}
			Console.WriteLine("}");
			Console.Write("obj { ");
			for (int i = 0; i < count; i++)
			{
				Console.Write(variables[i] + ":" + values[i] + " ");
			}
			Console.WriteLine("}");
		}

		public virtual void updateFromRow(ArrayRow arrayRow, ArrayRow definition)
		{
			// TODO -- only used when ArrayRow.USE_LINKED_VARIABLES is set to true
		}

		public virtual SolverVariable pickPivotCandidate()
		{
			// TODO -- only used when ArrayRow.USE_LINKED_VARIABLES is set to true
			return null;
		}

		public virtual void updateFromSystem(ArrayRow goal, ArrayRow[] mRows)
		{
			// TODO -- only used when ArrayRow.USE_LINKED_VARIABLES is set to true
		}

		public virtual void divideByAmount(float amount)
		{
			// TODO -- only used when ArrayRow.USE_LINKED_VARIABLES is set to true
		}

		public virtual void updateClientEquations(ArrayRow arrayRow)
		{
			// TODO -- only used when ArrayRow.USE_LINKED_VARIABLES is set to true
		}

		public virtual bool hasAtLeastOnePositiveVariable()
		{
			// TODO -- only used when ArrayRow.USE_LINKED_VARIABLES is set to true
			return false;
		}

		public virtual void invert()
		{
			// TODO -- only used when ArrayRow.USE_LINKED_VARIABLES is set to true
		}
	}

}