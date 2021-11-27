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

namespace androidx.constraintlayout.core.motion.utils
{
	using WidgetFrame = androidx.constraintlayout.core.state.WidgetFrame;


	/// <summary>
	/// This engine allows manipulation of attributes by Curves
	/// 
	/// @suppress
	/// </summary>

	public abstract class SplineSet
	{
		private const string TAG = "SplineSet";
		protected internal CurveFit mCurveFit;
		protected internal int[] mTimePoints = new int[10];
		protected internal float[] mValues = new float[10];
		private int count;
		private string mType;

		public virtual void setProperty(TypedValues widget, float t)
		{
			widget.setValue(TypedValues.TypedValues_Attributes.getId(mType), get(t));
		}

		public override string ToString()
		{
			string str = mType;
			DecimalFormat df = new DecimalFormat("##.##");
			for (int i = 0; i < count; i++)
			{
				str += "[" + mTimePoints[i] + " , " + df.format(mValues[i]) + "] ";

			}
			return str;
		}

		public virtual string Type
		{
			set
			{
				mType = value;
			}
		}

		public virtual float get(float t)
		{
			return (float) mCurveFit.getPos(t, 0);
		}

		public virtual float getSlope(float t)
		{
			return (float) mCurveFit.getSlope(t, 0);
		}

		public virtual CurveFit CurveFit
		{
			get
			{
				return mCurveFit;
			}
		}


		public virtual void setPoint(int position, float value)
		{
			if (mTimePoints.Length < count + 1)
			{
				//mTimePoints = Arrays.copyOf(mTimePoints, mTimePoints.Length * 2);
				mTimePoints = mTimePoints.Copy(mTimePoints.Length * 2);
				//mValues = Arrays.copyOf(mValues, mValues.Length * 2);
				mValues = mValues.Copy(mValues.Length * 2);
			}
			mTimePoints[count] = position;
			mValues[count] = value;
			count++;
		}

		public virtual void setup(int curveType)
		{
			if (count == 0)
			{
				return;
			}

			Sort.doubleQuickSort(mTimePoints, mValues, 0, count - 1);

			int unique = 1;

			for (int i = 1; i < count; i++)
			{
				if (mTimePoints[i - 1] != mTimePoints[i])
				{
					unique++;
				}
			}

			double[] time = new double[unique];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] values = new double[unique][1];
			double[][] values = RectangularArrays.ReturnRectangularDoubleArray(unique, 1);
			int k = 0;
			for (int i = 0; i < count; i++)
			{
				if (i > 0 && mTimePoints[i] == mTimePoints[i - 1])
				{
					continue;
				}

				time[k] = mTimePoints[i] * 1E-2;
				values[k][0] = mValues[i];
				k++;
			}
			mCurveFit = CurveFit.get(curveType, time, values);
		}

		public static SplineSet makeCustomSpline(string str, KeyFrameArray.CustomArray attrList)
		{
			return new CustomSet(str, attrList);
		}

		public static SplineSet makeCustomSplineSet(string str, KeyFrameArray.CustomVar attrList)
		{
			return new CustomSpline(str, attrList);
		}

		public static SplineSet makeSpline(string str, long currentTime)
		{

			return new CoreSpline(str, currentTime);
		}

		private class Sort
		{

			internal static void doubleQuickSort(int[] key, float[] value, int low, int hi)
			{
				int[] stack = new int[key.Length + 10];
				int count = 0;
				stack[count++] = hi;
				stack[count++] = low;
				while (count > 0)
				{
					low = stack[--count];
					hi = stack[--count];
					if (low < hi)
					{
						int p = partition(key, value, low, hi);
						stack[count++] = p - 1;
						stack[count++] = low;
						stack[count++] = hi;
						stack[count++] = p + 1;
					}
				}
			}

			internal static int partition(int[] array, float[] value, int low, int hi)
			{
				int pivot = array[hi];
				int i = low;
				for (int j = low; j < hi; j++)
				{
					if (array[j] <= pivot)
					{
						swap(array, value, i, j);
						i++;
					}
				}
				swap(array, value, i, hi);
				return i;
			}

			internal static void swap(int[] array, float[] value, int a, int b)
			{
				int tmp = array[a];
				array[a] = array[b];
				array[b] = tmp;
				float tmpv = value[a];
				value[a] = value[b];
				value[b] = tmpv;
			}
		}


		public class CustomSet : SplineSet
		{
			internal string mAttributeName;
			internal KeyFrameArray.CustomArray mConstraintAttributeList;
			internal float[] mTempValues;

			public CustomSet(string attribute, KeyFrameArray.CustomArray attrList)
			{
				mAttributeName = attribute.Split(",", true)[1];
				mConstraintAttributeList = attrList;
			}

			public override void setup(int curveType)
			{
				int size = mConstraintAttributeList.size();
				int dimensionality = mConstraintAttributeList.valueAt(0).numberOfInterpolatedValues();
				double[] time = new double[size];
				mTempValues = new float[dimensionality];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] values = new double[size][dimensionality];
				double[][] values = RectangularArrays.ReturnRectangularDoubleArray(size, dimensionality);
				for (int i = 0; i < size; i++)
				{

					int key = mConstraintAttributeList.keyAt(i);
					CustomAttribute ca = mConstraintAttributeList.valueAt(i);

					time[i] = key * 1E-2;
					ca.getValuesToInterpolate(mTempValues);
					for (int k = 0; k < mTempValues.Length; k++)
					{
						values[i][k] = mTempValues[k];
					}

				}
				mCurveFit = CurveFit.get(curveType, time, values);
			}

			public override void setPoint(int position, float value)
			{
				throw new Exception("don't call for custom attribute call setPoint(pos, ConstraintAttribute)");
			}

			public virtual void setPoint(int position, CustomAttribute value)
			{
				mConstraintAttributeList.append(position, value);
			}

			public virtual void setProperty(WidgetFrame view, float t)
			{
				mCurveFit.getPos(t, mTempValues);
				mConstraintAttributeList.valueAt(0).setInterpolatedValue(view, mTempValues);
			}
		}


		private class CoreSpline : SplineSet
		{
			internal string type;
			internal long start;

			public CoreSpline(string str, long currentTime)
			{
				type = str;
				start = currentTime;
			}

			public override void setProperty(TypedValues widget, float t)
			{
				int id = widget.getId(type);
				widget.setValue(id, get(t));
			}
		}

		public class CustomSpline : SplineSet
		{
			internal string mAttributeName;
			internal KeyFrameArray.CustomVar mConstraintAttributeList;
			internal float[] mTempValues;

			public CustomSpline(string attribute, KeyFrameArray.CustomVar attrList)
			{
				mAttributeName = attribute.Split(",", true)[1];
				mConstraintAttributeList = attrList;
			}

			public override void setup(int curveType)
			{
				int size = mConstraintAttributeList.size();
				int dimensionality = mConstraintAttributeList.valueAt(0).numberOfInterpolatedValues();
				double[] time = new double[size];
				mTempValues = new float[dimensionality];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] values = new double[size][dimensionality];
				double[][] values = RectangularArrays.ReturnRectangularDoubleArray(size, dimensionality);
				for (int i = 0; i < size; i++)
				{

					int key = mConstraintAttributeList.keyAt(i);
					CustomVariable ca = mConstraintAttributeList.valueAt(i);

					time[i] = key * 1E-2;
					ca.getValuesToInterpolate(mTempValues);
					for (int k = 0; k < mTempValues.Length; k++)
					{
						values[i][k] = mTempValues[k];
					}

				}
				mCurveFit = CurveFit.get(curveType, time, values);
			}

			public override void setPoint(int position, float value)
			{
				throw new Exception("don't call for custom attribute call setPoint(pos, ConstraintAttribute)");
			}

			public override void setProperty(TypedValues widget, float t)
			{
				setProperty((MotionWidget) widget, t);
			}

			public virtual void setPoint(int position, CustomVariable value)
			{
				mConstraintAttributeList.append(position, value);
			}

			public virtual void setProperty(MotionWidget view, float t)
			{
				mCurveFit.getPos(t, mTempValues);
				mConstraintAttributeList.valueAt(0).setInterpolatedValue(view, mTempValues);
			}
		}

	}

}