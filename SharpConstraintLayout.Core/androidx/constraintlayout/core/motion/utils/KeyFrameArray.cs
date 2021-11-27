using System;

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
namespace androidx.constraintlayout.core.motion.utils
{

	public class KeyFrameArray
	{

		// =================================== CustomAttribute =================================
		public class CustomArray
		{
			internal int[] keys = new int[101];
			internal CustomAttribute[] values = new CustomAttribute[101];
			internal int count;
			internal const int EMPTY = 999;

			public CustomArray()
			{
				clear();
			}

			public virtual void clear()
			{
				//Arrays.fill(keys, EMPTY);
				keys.Fill(EMPTY);
				//Arrays.fill(values, null);
				values.Fill(null);
				count = 0;
			}

			public virtual void dump()
			{
				//Console.WriteLine("V: " + Arrays.ToString(Arrays.copyOf(keys, count)));
				Console.WriteLine("V: " + keys.Copy(count).ToString());
				Console.Write("K: [");
				for (int i = 0; i < count; i++)
				{
					Console.Write(((i == 0 ? "" : ", ")) + valueAt(i));
				}
				Console.WriteLine("]");
			}

			public virtual int size()
			{
				return count;
			}

			public virtual CustomAttribute valueAt(int i)
			{
				return values[keys[i]];
			}

			public virtual int keyAt(int i)
			{
				return keys[i];
			}

			public virtual void append(int position, CustomAttribute value)
			{
				if (values[position] != null)
				{
					remove(position);
				}
				values[position] = value;
				keys[count++] = position;
				//Arrays.sort(keys);
				Array.Sort(keys);//TODO:验证效果是否一样
			}

			public virtual void remove(int position)
			{
				values[position] = null;
				for (int j = 0, i = 0; i < count; i++)
				{
					if (position == keys[i])
					{
						keys[i] = EMPTY;
						j++;
					}
					if (i != j)
					{
						keys[i] = keys[j];
					}
					j++;

				}
				count--;
			}
		}
		// =================================== CustomVar =================================
		public class CustomVar
		{
			internal int[] keys = new int[101];
			internal CustomVariable[] values = new CustomVariable[101];
			internal int count;
			internal const int EMPTY = 999;

			public CustomVar()
			{
				clear();
			}

			public virtual void clear()
			{
				//Arrays.fill(keys, EMPTY);
				keys.Fill(EMPTY);
				//Arrays.fill(values, null);
				values.Fill(null);
				count = 0;
			}

			public virtual void dump()
			{
				//Console.WriteLine("V: " + Arrays.ToString(Arrays.copyOf(keys, count)));
				Console.WriteLine("V: " + keys.Copy(count).ToString());
				Console.Write("K: [");
				for (int i = 0; i < count; i++)
				{
					Console.Write(((i == 0 ? "" : ", ")) + valueAt(i));
				}
				Console.WriteLine("]");
			}

			public virtual int size()
			{
				return count;
			}

			public virtual CustomVariable valueAt(int i)
			{
				return values[keys[i]];
			}

			public virtual int keyAt(int i)
			{
				return keys[i];
			}

			public virtual void append(int position, CustomVariable value)
			{
				if (values[position] != null)
				{
					remove(position);
				}
				values[position] = value;
				keys[count++] = position;
				//Arrays.sort(keys);
				Array.Sort(keys);
			}

			public virtual void remove(int position)
			{
				values[position] = null;
				for (int j = 0, i = 0; i < count; i++)
				{
					if (position == keys[i])
					{
						keys[i] = EMPTY;
						j++;
					}
					if (i != j)
					{
						keys[i] = keys[j];
					}
					j++;

				}
				count--;
			}
		}
		// =================================== FloatArray ======================================
	   internal class FloatArray
	   {
			internal int[] keys = new int[101];
			internal float[][] values = new float[101][];
			internal int count;
			internal const int EMPTY = 999;

			public FloatArray()
			{
				clear();
			}

			public virtual void clear()
			{
				//Arrays.fill(keys, EMPTY);
				keys.Fill(EMPTY);
				//Arrays.fill(values, null);
				values.Fill(null);
				count = 0;
			}

			public virtual void dump()
			{
				//Console.WriteLine("V: " + Arrays.ToString(Arrays.copyOf(keys, count)));
				Console.WriteLine("V: " + keys.Copy(count).ToString());
				Console.Write("K: [");
				for (int i = 0; i < count; i++)
				{
					//Console.Write(((i == 0 ? "" : ", ")) + Arrays.ToString(valueAt(i)));
					Console.Write(((i == 0 ? "" : ", ")) + valueAt(i).ToString());
				}
				Console.WriteLine("]");
			}

			public virtual int size()
			{
				return count;
			}

			public virtual float[] valueAt(int i)
			{
				return values[keys[i]];
			}

			public virtual int keyAt(int i)
			{
				return keys[i];
			}

			public virtual void append(int position, float[] value)
			{
				if (values[position] != null)
				{
					remove(position);
				}
				values[position] = value;
				keys[count++] = position;
				//Arrays.sort(keys);
				Array.Sort(keys);
			}

			public virtual void remove(int position)
			{
				values[position] = null;
				for (int j = 0, i = 0; i < count; i++)
				{
					if (position == keys[i])
					{
						keys[i] = EMPTY;
						j++;
					}
					if (i != j)
					{
						keys[i] = keys[j];
					}
					j++;

				}
				count--;
			}
	   }
	}

}