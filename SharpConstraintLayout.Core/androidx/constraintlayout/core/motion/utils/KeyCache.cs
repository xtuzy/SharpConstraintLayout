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

namespace androidx.constraintlayout.core.motion.utils
{

	/// <summary>
	/// Used by KeyTimeCycles (and any future time dependent behaviour) to cache its current parameters
	/// to maintain consistency across requestLayout type rebuilds.
	/// </summary>
	public class KeyCache
	{

		internal Dictionary<object, Dictionary<string, float[]>> map = new Dictionary<object, Dictionary<string, float[]>>();

		public virtual void setFloatValue(object view, string type, int element, float value)
		{
			if (!map.ContainsKey(view))
			{
				Dictionary<string, float[]> array = new Dictionary<string, float[]>();
				float[] vArray = new float[element + 1];
				vArray[element] = value;
				array[type] = vArray;
				map[view] = array;
			}
			else
			{
				Dictionary<string, float[]> array = map[view];
				if (array == null)
				{
					array = new Dictionary<string,float[]>();
				}

				if (!array.ContainsKey(type))
				{
					float[] vArray = new float[element + 1];
					vArray[element] = value;
					array[type] = vArray;
					map[view] = array;
				}
				else
				{
					float[] vArray = array[type];
					if (vArray == null)
					{
						vArray = new float[0];
					}
					if (vArray.Length <= element)
					{
						//vArray = Arrays.copyOf(vArray, element + 1);
						vArray = vArray.Copy<float>(element + 1);
					}
					vArray[element] = value;
					array[type] = vArray;
				}
			}
		}

		public virtual float getFloatValue(object view, string type, int element)
		{
			if (!map.ContainsKey(view))
			{
				return float.NaN;
			}
			else
			{
				Dictionary<string, float[]> array = map[view];
				if (array == null || !array.ContainsKey(type))
				{
					return float.NaN;
				}
				float[] vArray = array[type];
				if (vArray == null)
				{
					return float.NaN;
				}
				if (vArray.Length > element)
				{
					return vArray[element];
				}
				return float.NaN;
			}
		}
	}

}