using System;
using System.Collections.Generic;
using System.Drawing;

/*
 * Copyright (C) 2016 The Android Open Source Project
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

namespace androidx.constraintlayout.core.scout
{
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using Guideline = androidx.constraintlayout.core.widgets.Guideline;


	/// <summary>
	/// Simple Utilities used by the Inference system
	/// </summary>
	public class Utils
	{
		private static DecimalFormat df = new DecimalFormat("0.0#####");
		/// <summary>
		/// Calculate the maximum of an array </summary>
		/// <param name="array"> </param>
		/// <returns> the index of the maximum </returns>
		internal static int max(float[] array)
		{
			int max = 0;
			float val = array[0];
			for (int i = 1; i < array.Length; i++)
			{
				if (val < array[i])
				{
					max = i;
					val = array[i];
				}
			}
			return max;
		}

		/// <summary>
		/// Calculate the maximum of a 2D array
		/// </summary>
		/// <param name="array"> </param>
		/// <param name="result"> the index of the maximum filled by the function </param>
		/// <returns> the value of the maximum probabilities </returns>
		internal static float max(float[][] array, int[] result)
		{
			int max1 = 0;
			int max2 = 0;
			float val = array[max1][max2];
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array[0].Length; j++)
				{
					if (val < array[i][j])
					{
						max1 = i;
						max2 = j;
						val = array[max1][max2];
					}
				}
			}
			result[0] = max1;
			result[1] = max2;
			return val;
		}

		/// <summary>
		/// convert an array of floats to fixed length strings </summary>
		/// <param name="a">
		/// @return </param>
		internal static string toS(float[] a)
		{
			string s = "[";
			if (a == null)
			{
				return "[null]";
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (i != 0)
				{
					s += " , ";
				}
				string t = df.format(a[i]) + "       ";
				s += t.Substring(0, 7);

			}
			s += "]";
			return s;
		}

		/// <summary>
		/// Left trim a string to a fixed length
		/// </summary>
		/// <param name="str"> String to trim </param>
		/// <param name="len"> length to trim to </param>
		/// <returns> the trimmed string </returns>
		internal static string leftTrim(string str, int len)
		{
			return str.Substring(str.Length - len);
		}

		/// <summary>
		/// Fill a 2D array of floats with 0.0 </summary>
		/// <param name="array"> </param>
		internal static void zero(float[][] array)
		{
			foreach (float[] aFloat in array)
			{
				aFloat.Fill( -1);
			}
		}

		/// <summary>
		/// Calculate the number of gaps + 1 given a start and end range
		/// </summary>
		/// <param name="start"> table of range starts </param>
		/// <param name="end">   table of range ends
		/// @return </param>
		public static int gaps(int[] start, int[] end)
		{
			Array.Sort(start);
			Array.Sort(end);
			int overlap = 0;
			int gaps = 0;
			for (int i = 0, j = 0; j < end.Length;)
			{
				if (i < start.Length && start[i] < end[j])
				{
					overlap++;
					i++;
				}
				else
				{
					j++;
					overlap--;
				}
				if (overlap == 0)
				{
					gaps++;
				}
			}
			return gaps;
		}

		/// <summary>
		/// calculate the ranges for the cells
		/// </summary>
		/// <param name="start"> table of range starts </param>
		/// <param name="end">   table of range ends </param>
		/// <returns> array of integers 2 for each cell </returns>
		public static int[] cells(int[] start, int[] end)
		{
			Array.Sort(start);
			Array.Sort(end);

			int overlap = 0;
			int gaps = 0;
			for (int i = 0, j = 0; j < end.Length;)
			{
				if (i < start.Length && start[i] < end[j])
				{
					overlap++;
					i++;
				}
				else
				{
					j++;
					overlap--;
				}
				if (overlap == 0)
				{
					gaps++;
				}
			}
			int[] cells = new int[gaps * 2];
			overlap = 0;
			gaps = 0;
			int previousOverlap = 0;
			for (int i = 0, j = 0; j < end.Length;)
			{
				if (i < start.Length && start[i] < end[j])
				{
					overlap++;
					if (previousOverlap == 0)
					{
						cells[gaps++] = start[i];
					}
					i++;
				}
				else
				{
					overlap--;
					if (overlap == 0)
					{
						cells[gaps++] = end[j];
					}
					j++;
				}
				previousOverlap = overlap;
			}

			return cells;
		}

		/// <summary>
		/// Search within the collection of ranges for the position
		/// </summary>
		/// <param name="pos"> range pairs </param>
		/// <param name="p1">  start of widget </param>
		/// <param name="p2">  end of widget </param>
		/// <returns> the pair of ranges it is within </returns>
		public static int getPosition(int[] pos, int p1, int p2)
		{
			for (int j = 0; j < pos.Length; j += 2)
			{ // linear search is best because N typically < 10
				if (pos[j] <= p1 && p2 <= pos[j + 1])
				{
					return j / 2;
				}
			}
			return -1;
		}

		/// <summary>
		/// Sort a list of integers and remove duplicates
		/// </summary>
		/// <param name="list">
		/// @return </param>
		internal static int[] sortUnique(int[] list)
		{
			Array.Sort(list);
			int count = 1;
			for (int i = 1; i < list.Length; i++)
			{
				if (list[i] != list[i - 1])
				{
					count++;
				}
			}
			int[] ret = new int[count];
			count = 1;
			ret[0] = list[0];
			for (int i = 1; i < list.Length; i++)
			{
				if (list[i] != list[i - 1])
				{
					ret[count++] = list[i];
				}
			}
			return ret;
		}

		/// <summary>
		/// print a string that is a fixed width of size used in debugging
		/// </summary>
		/// <param name="s"> </param>
		/// <param name="size"> </param>
		internal static void fwPrint(string s, int size)
		{
			s += "                                             ";
			s = s.Substring(0, size);
			Console.Write(s);
		}


		/// <summary>
		/// Get the bounding box around a list of widgets
		/// </summary>
		/// <param name="widgets">
		/// @return </param>
		internal static Rectangle getBoundingBox(List<ConstraintWidget> widgets)
		{
			Rectangle all = Rectangle.Empty;
			Rectangle tmp = new Rectangle();
			foreach (ConstraintWidget widget in widgets)
			{
				if (widget is Guideline)
				{
					continue;
				}
				tmp.X = widget.X;
				tmp.Y = widget.Y;
				tmp.Width = widget.Width;
				tmp.Height = widget.Height;
				if (all == Rectangle.Empty)
				{
					//all = new Rectangle(tmp);
					all = new Rectangle(tmp.X, tmp.Y, tmp.Width, tmp.Height);
				}
				else
				{
					//all = all.union(tmp);
					all.Intersect(tmp);
				}
			}
			return all;
		}

	}

}