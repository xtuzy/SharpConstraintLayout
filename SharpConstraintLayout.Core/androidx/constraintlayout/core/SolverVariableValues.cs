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
namespace androidx.constraintlayout.core
{

	/// <summary>
	/// Store a set of variables and their values in an array-based linked list coupled
	/// with a custom hashmap.
	/// </summary>
	public class SolverVariableValues : ArrayRow.ArrayRowVariables
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			keys = new int[SIZE];
			nextKeys = new int[SIZE];
			variables = new int[SIZE];
			values = new float[SIZE];
			previous = new int[SIZE];
			next = new int[SIZE];
		}


		private const bool DEBUG = false;
		private const bool HASH = true;
		private static float epsilon = 0.001f;
		private readonly int NONE = -1;
		private int SIZE = 16;
		private int HASH_SIZE = 16;

		internal int[] keys;
		internal int[] nextKeys;

		internal int[] variables;
		internal float[] values;
		internal int[] previous;
		internal int[] next;
		internal int mCount = 0;
		internal int head = -1;

		private readonly ArrayRow mRow; // our owner
		protected internal readonly Cache mCache; // pointer to the system-wide cache, allowing access to SolverVariables

		public SolverVariableValues(ArrayRow row, Cache cache)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			mRow = row;
			mCache = cache;
			clear();
		}

		public virtual int CurrentSize
		{
			get
			{
				return mCount;
			}
		}

		public virtual SolverVariable getVariable(int index)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
			int count = mCount;
			if (count == 0)
			{
				return null;
			}
			int j = head;
			for (int i = 0; i < count; i++)
			{
				if (i == index && j != NONE)
				{
					return mCache.mIndexedVariables[variables[j]];
				}
				j = next[j];
				if (j == NONE)
				{
					break;
				}
			}
			return null;
		}

		public virtual float getVariableValue(int index)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
			int count = mCount;
			int j = head;
			for (int i = 0; i < count; i++)
			{
				if (i == index)
				{
					return values[j];
				}
				j = next[j];
				if (j == NONE)
				{
					break;
				}
			}
			return 0;
		}

		public virtual bool contains(SolverVariable variable)
		{
			return indexOf(variable) != NONE;
		}

		public virtual int indexOf(SolverVariable variable)
		{
			if (mCount == 0 || variable == null)
			{
				return NONE;
			}
			int id = variable.id;
			int key = id % HASH_SIZE;
			key = keys[key];
			if (key == NONE)
			{
				return NONE;
			}
			if (variables[key] == id)
			{
				return key;
			}
			while (nextKeys[key] != NONE && variables[nextKeys[key]] != id)
			{
				key = nextKeys[key];
			}
			if (nextKeys[key] == NONE)
			{
				return NONE;
			}
			if (variables[nextKeys[key]] == id)
			{
				return nextKeys[key];
			}
			return NONE;
		}

		public virtual float get(SolverVariable variable)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = indexOf(variable);
			int index = indexOf(variable);
			if (index != NONE)
			{
				return values[index];
			}
			return 0;
		}

		public virtual void display()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
			int count = mCount;
			Console.Write("{ ");
			for (int i = 0; i < count; i++)
			{
				SolverVariable v = getVariable(i);
				if (v == null)
				{
					continue;
				}
				Console.Write(v + " = " + getVariableValue(i) + " ");
			}
			Console.WriteLine(" }");
		}

		public override string ToString()
		{
			string str = GetHashCode() + " { ";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
			int count = mCount;
			for (int i = 0; i < count; i++)
			{
				SolverVariable v = getVariable(i);
				if (v == null)
				{
					continue;
				}
				str += v + " = " + getVariableValue(i) + " ";
				int index = indexOf(v);
				str += "[p: ";
				if (previous[index] != NONE)
				{
					str += mCache.mIndexedVariables[variables[previous[index]]];
				}
				else
				{
					str += "none";
				}
				str += ", n: ";
				if (next[index] != NONE)
				{
					str += mCache.mIndexedVariables[variables[next[index]]];
				}
				else
				{
					str += "none";
				}
				str += "]";
			}
			str += " }";
			return str;
		}

		public virtual void clear()
		{
			if (DEBUG)
			{
				Console.WriteLine(this + " <clear>");
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
			int count = mCount;
			for (int i = 0; i < count; i++)
			{
				SolverVariable v = getVariable(i);
				if (v != null)
				{
					v.removeFromRow(mRow);
				}
			}
			for (int i = 0; i < SIZE; i++)
			{
				variables[i] = NONE;
				nextKeys[i] = NONE;
			}
			for (int i = 0; i < HASH_SIZE; i++)
			{
				keys[i] = NONE;
			}
			mCount = 0;
			head = -1;
		}

		private void increaseSize()
		{
			int size = SIZE * 2;
			variables = variables.Copy<int>( size);
			values = values.Copy<float>(size);
			previous = previous.Copy<int>(size);
			next = next.Copy<int>(size);
			nextKeys = nextKeys.Copy<int>(size);
			for (int i = SIZE; i < size; i++)
			{
				variables[i] = NONE;
				nextKeys[i] = NONE;
			}
			SIZE = size;
		}

		private void addToHashMap(SolverVariable variable, int index)
		{
			if (DEBUG)
			{
				Console.WriteLine(this.GetHashCode() + " hash add " + variable.id + " @ " + index);
			}
			int hash = variable.id % HASH_SIZE;
			int key = keys[hash];
			if (key == NONE)
			{
				keys[hash] = index;
				if (DEBUG)
				{
					Console.WriteLine(this.GetHashCode() + " hash add " + variable.id + " @ " + index + " directly on keys " + hash);
				}
			}
			else
			{
				while (nextKeys[key] != NONE)
				{
					key = nextKeys[key];
				}
				nextKeys[key] = index;
				if (DEBUG)
				{
					Console.WriteLine(this.GetHashCode() + " hash add " + variable.id + " @ " + index + " as nextkey of " + key);
				}
			}
			nextKeys[index] = NONE;
			if (DEBUG)
			{
				displayHash();
			}
		}

		private void displayHash()
		{
			for (int i = 0; i < HASH_SIZE; i++)
			{
				if (keys[i] != NONE)
				{
					string str = this.GetHashCode() + " hash [" + i + "] => ";
					int key = keys[i];
					bool done = false;
					while (!done)
					{
						str += " " + variables[key];
						if (nextKeys[key] != NONE)
						{
							key = nextKeys[key];
						}
						else
						{
							done = true;
						}
					}
					Console.WriteLine(str);
				}
			}
		}
		private void removeFromHashMap(SolverVariable variable)
		{
			if (DEBUG)
			{
				Console.WriteLine(this.GetHashCode() + " hash remove " + variable.id);
			}
			int hash = variable.id % HASH_SIZE;
			int key = keys[hash];
			if (key == NONE)
			{
				if (DEBUG)
				{
					displayHash();
				}
				return;
			}
			int id = variable.id;
			// let's first find it
			if (variables[key] == id)
			{
				keys[hash] = nextKeys[key];
				nextKeys[key] = NONE;
			}
			else
			{
				while (nextKeys[key] != NONE && variables[nextKeys[key]] != id)
				{
					key = nextKeys[key];
				}
				int currentKey = nextKeys[key];
				if (currentKey != NONE && variables[currentKey] == id)
				{
					nextKeys[key] = nextKeys[currentKey];
					nextKeys[currentKey] = NONE;
				}
			}
			if (DEBUG)
			{
				displayHash();
			}
		}

		private void addVariable(int index, SolverVariable variable, float value)
		{
			variables[index] = variable.id;
			values[index] = value;
			previous[index] = NONE;
			next[index] = NONE;
			variable.addToRow(mRow);
			variable.usageInRowCount++;
			mCount++;
		}

		private int findEmptySlot()
		{
			for (int i = 0; i < SIZE; i++)
			{
				if (variables[i] == NONE)
				{
					return i;
				}
			}
			return -1;
		}

		private void insertVariable(int index, SolverVariable variable, float value)
		{
			int availableSlot = findEmptySlot();
			addVariable(availableSlot, variable, value);
			if (index != NONE)
			{
				previous[availableSlot] = index;
				next[availableSlot] = next[index];
				next[index] = availableSlot;
			}
			else
			{
				previous[availableSlot] = NONE;
				if (mCount > 0)
				{
					next[availableSlot] = head;
					head = availableSlot;
				}
				else
				{
					next[availableSlot] = NONE;
				}
			}
			if (next[availableSlot] != NONE)
			{
				previous[next[availableSlot]] = availableSlot;
			}
			addToHashMap(variable, availableSlot);
		}

		public virtual void put(SolverVariable variable, float value)
		{
			if (DEBUG)
			{
				Console.WriteLine(this + " <put> " + variable.id + " = " + value);
			}
			if (value > -epsilon && value < epsilon)
			{
				remove(variable, true);
				return;
			}
			if (mCount == 0)
			{
				addVariable(0, variable, value);
				addToHashMap(variable, 0);
				head = 0;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = indexOf(variable);
				int index = indexOf(variable);
				if (index != NONE)
				{
					values[index] = value;
				}
				else
				{
					if (mCount + 1 >= SIZE)
					{
						increaseSize();
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
					int count = mCount;
					int previousItem = -1;
					int j = head;
					for (int i = 0; i < count; i++)
					{
						if (variables[j] == variable.id)
						{
							values[j] = value;
							return;
						}
						if (variables[j] < variable.id)
						{
							previousItem = j;
						}
						j = next[j];
						if (j == NONE)
						{
							break;
						}
					}
					insertVariable(previousItem, variable, value);
				}
			}
		}

		public virtual int sizeInBytes()
		{
			return 0;
		}

		public virtual float remove(SolverVariable v, bool removeFromDefinition)
		{
			if (DEBUG)
			{
				Console.WriteLine(this + " <remove> " + v.id);
			}
			int index = indexOf(v);
			if (index == NONE)
			{
				return 0;
			}
			removeFromHashMap(v);
			float value = values[index];
			if (head == index)
			{
				head = next[index];
			}
			variables[index] = NONE;
			if (previous[index] != NONE)
			{
				next[previous[index]] = next[index];
			}
			if (next[index] != NONE)
			{
				previous[next[index]] = previous[index];
			}
			mCount--;
			v.usageInRowCount--;
			if (removeFromDefinition)
			{
				v.removeFromRow(mRow);
			}
			return value;
		}

		public virtual void add(SolverVariable v, float value, bool removeFromDefinition)
		{
			if (DEBUG)
			{
				Console.WriteLine(this + " <add> " + v.id + " = " + value);
			}
			if (value > -epsilon && value < epsilon)
			{
				return;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = indexOf(v);
			int index = indexOf(v);
			if (index == NONE)
			{
				put(v, value);
			}
			else
			{
				values[index] += value;
				if (values[index] > -epsilon && values[index] < epsilon)
				{
					values[index] = 0;
					remove(v, removeFromDefinition);
				}
			}
		}

		public virtual float use(ArrayRow def, bool removeFromDefinition)
		{
			float value = get(def.variable);
			remove(def.variable, removeFromDefinition);
			int definitionSize;
			if (false)
			{
				ArrayRow.ArrayRowVariables definitionVariables = def.variables;
				definitionSize = definitionVariables.CurrentSize;
				for (int i = 0; i < definitionSize; i++)
				{
					SolverVariable definitionVariable = definitionVariables.getVariable(i);
					float definitionValue = definitionVariables.get(definitionVariable);
					this.add(definitionVariable, definitionValue * value, removeFromDefinition);
				}
				return value;
			}
			SolverVariableValues definition = (SolverVariableValues) def.variables;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int definitionSize = definition.getCurrentSize();
			definitionSize = definition.CurrentSize;
			int j = definition.head;
			if (false)
			{
				for (int i = 0; i < definitionSize; i++)
				{
					float definitionValue = definition.values[j];
					SolverVariable definitionVariable = mCache.mIndexedVariables[definition.variables[j]];
					add(definitionVariable, definitionValue * value, removeFromDefinition);
					j = definition.next[j];
					if (j == NONE)
					{
						break;
					}
				}
			}
			else
			{
				j = 0;
				for (int i = 0; j < definitionSize; i++)
				{
					if (definition.variables[i] != NONE)
					{
						float definitionValue = definition.values[i];
						SolverVariable definitionVariable = mCache.mIndexedVariables[definition.variables[i]];
						add(definitionVariable, definitionValue * value, removeFromDefinition);
						j++;
					}
				}
			}
			return value;
		}

		public virtual void invert()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
			int count = mCount;
			int j = head;
			for (int i = 0; i < count; i++)
			{
				values[j] *= -1;
				j = next[j];
				if (j == NONE)
				{
					break;
				}
			}
		}

		public virtual void divideByAmount(float amount)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mCount;
			int count = mCount;
			int j = head;
			for (int i = 0; i < count; i++)
			{
				values[j] /= amount;
				j = next[j];
				if (j == NONE)
				{
					break;
				}
			}
		}

	}

}