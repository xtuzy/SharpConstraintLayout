using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2021 The Android Open Source Project
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

namespace androidx.constraintlayout.core.widgets
{


	public class BasicSolverVariableValues : ArrayRow.ArrayRowVariables
	{

		private static float epsilon = 0.001f;

		internal class Item
		{
			private readonly BasicSolverVariableValues outerInstance;

			public Item(BasicSolverVariableValues outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal SolverVariable variable;
			internal float value;
		}

		private readonly ArrayRow mRow; // our owner
		internal List<Item> list = new List<Item>();
		//LinkedList<Item> list = new LinkedList<>();

		internal IComparer<Item> comparator = new ComparatorAnonymousInnerClass();

		private class ComparatorAnonymousInnerClass : IComparer<Item>
		{
			public ComparatorAnonymousInnerClass()
			{
			}

			public virtual int Compare(Item s1, Item s2)
			{
				return s1.variable.id - s2.variable.id;
			}
		}

		internal BasicSolverVariableValues(ArrayRow row, Cache cache)
		{
			mRow = row;
		}

		public virtual int CurrentSize
		{
			get
			{
				return list.Count;
			}
		}

		public virtual SolverVariable getVariable(int i)
		{
			return list[i].variable;
		}

		public virtual float getVariableValue(int i)
		{
			return list[i].value;
		}

		public virtual bool contains(SolverVariable variable)
		{
			foreach (Item item in list)
			{
				if (item.variable.id == variable.id)
				{
					return true;
				}
			}
			return false;
		}

		public virtual int indexOf(SolverVariable variable)
		{
			for (int i = 0; i < CurrentSize; i++)
			{
				Item item = list[i];
				if (item.variable.id == variable.id)
				{
					return i;
				}
			}
			return -1;
		}

		public virtual float get(SolverVariable variable)
		{
			if (contains(variable))
			{
				return list[indexOf(variable)].value;
			}
			return 0;
		}

		public virtual void display()
		{
			int count = CurrentSize;
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

		public virtual void clear()
		{
			int count = CurrentSize;
			for (int i = 0; i < count; i++)
			{
				SolverVariable v = getVariable(i);
				v.removeFromRow(mRow);
			}
			list.Clear();
		}

		public virtual void put(SolverVariable variable, float value)
		{
			if (value > -epsilon && value < epsilon)
			{
				remove(variable, true);
				return;
			}
	//        System.out.println("Put " + variable + " [" + value + "] in " + mRow);
			//list.add(item);

			if (list.Count == 0)
			{
				Item item = new Item(this);
				item.variable = variable;
				item.value = value;
				list.Add(item);
				variable.addToRow(mRow);
				variable.usageInRowCount++;
			}
			else
			{
				if (contains(variable))
				{
					Item currentItem = list[indexOf(variable)];
					currentItem.value = value;
					return;
				}
				else
				{
					Item item = new Item(this);
					item.variable = variable;
					item.value = value;
					list.Add(item);
					variable.usageInRowCount++;
					variable.addToRow(mRow);
					list.Sort(comparator);
				}
	//            if (false) {
	//                int previousItem = -1;
	//                int n = 0;
	//                for (Item currentItem : list) {
	//                    if (currentItem.variable.id == variable.id) {
	//                        currentItem.value = value;
	//                        return;
	//                    }
	//                    if (currentItem.variable.id < variable.id) {
	//                        previousItem = n;
	//                    }
	//                    n++;
	//                }
	//                Item item = new Item();
	//                item.variable = variable;
	//                item.value = value;
	//                list.add(previousItem + 1, item);
	//                variable.usageInRowCount++;
	//                variable.addToRow(mRow);
	//            }
			}
		}

		public virtual int sizeInBytes()
		{
			return 0;
		}

		public virtual float remove(SolverVariable v, bool removeFromDefinition)
		{
			if (!contains(v))
			{
				return 0;
			}
			int index = indexOf(v);
			float value = list[indexOf(v)].value;
			list.RemoveAt(index);
			v.usageInRowCount--;
			if (removeFromDefinition)
			{
				v.removeFromRow(mRow);
			}
			return value;
		}

		public virtual void add(SolverVariable v, float value, bool removeFromDefinition)
		{
			if (value > -epsilon && value < epsilon)
			{
				return;
			}
			if (!contains(v))
			{
				put(v, value);
			}
			else
			{
				Item item = list[indexOf(v)];
				item.value += value;
				if (item.value > -epsilon && item.value < epsilon)
				{
					item.value = 0;
					list.Remove(item);
					v.usageInRowCount--;
					if (removeFromDefinition)
					{
						v.removeFromRow(mRow);
					}
				}
			}
		}

		public virtual float use(ArrayRow definition, bool removeFromDefinition)
		{
			return 0;
		}

		public virtual void invert()
		{
			foreach (Item item in list)
			{
				item.value *= -1;
			}
		}

		public virtual void divideByAmount(float amount)
		{
			foreach (Item item in list)
			{
				item.value /= amount;
			}
		}

	}

}