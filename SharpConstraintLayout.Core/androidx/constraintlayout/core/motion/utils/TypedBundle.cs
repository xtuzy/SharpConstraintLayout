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

	public class TypedBundle
	{

		private const int INITIAL_BOOLEAN = 4;
		private const int INITIAL_INT = 10;
		private const int INITIAL_FLOAT = 10;
		private const int INITIAL_STRING = 5;

		internal int[] mTypeInt = new int[INITIAL_INT];
		internal int[] mValueInt = new int[INITIAL_INT];
		internal int mCountInt = 0;
		internal int[] mTypeFloat = new int[INITIAL_FLOAT];
		internal float[] mValueFloat = new float[INITIAL_FLOAT];
		internal int mCountFloat = 0;
		internal int[] mTypeString = new int[INITIAL_STRING];
		internal string[] mValueString = new string[INITIAL_STRING];
		internal int mCountString = 0;
		internal int[] mTypeBoolean = new int[INITIAL_BOOLEAN];
		internal bool[] mValueBoolean = new bool[INITIAL_BOOLEAN];
		internal int mCountBoolean = 0;

		public virtual int getInteger(int type)
		{
			for (int i = 0; i < mCountInt; i++)
			{
				if (mTypeInt[i] == type)
				{
					return mValueInt[i];
				}
			}
			return -1;
		}

		public virtual void add(int type, int value)
		{
			if (mCountInt >= mTypeInt.Length)
			{
				//mTypeInt = Arrays.copyOf(mTypeInt, mTypeInt.Length * 2);
				mTypeInt = mTypeInt.Copy(mTypeInt.Length * 2);
				//mValueInt = Arrays.copyOf(mValueInt, mValueInt.Length * 2);
				mValueInt = mValueInt.Copy(mValueInt.Length * 2);
			}
			mTypeInt[mCountInt] = type;
			mValueInt[mCountInt++] = value;
		}

		public virtual void add(int type, float value)
		{
			if (mCountFloat >= mTypeFloat.Length)
			{
				//mTypeFloat = Arrays.copyOf(mTypeFloat, mTypeFloat.Length * 2);
				mTypeFloat = mTypeFloat.Copy(mTypeFloat.Length * 2);
				//mValueFloat = Arrays.copyOf(mValueFloat, mValueFloat.Length * 2);
				mValueFloat = mValueFloat.Copy(mValueFloat.Length * 2);
			}
			mTypeFloat[mCountFloat] = type;
			mValueFloat[mCountFloat++] = value;
		}

		public virtual void addIfNotNull(int type, string value)
		{
			if (!string.ReferenceEquals(value, null))
			{
				add(type, value);
			}
		}

		public virtual void add(int type, string value)
		{
			if (mCountString >= mTypeString.Length)
			{
				//mTypeString = Arrays.copyOf(mTypeString, mTypeString.Length * 2);
				mTypeString = mTypeString.Copy(mTypeString.Length * 2);
				//mValueString = Arrays.copyOf(mValueString, mValueString.Length * 2);
				mValueString = mValueString.Copy(mValueString.Length * 2);
			}
			mTypeString[mCountString] = type;
			mValueString[mCountString++] = value;
		}

		public virtual void add(int type, bool value)
		{
			if (mCountBoolean >= mTypeBoolean.Length)
			{
				//mTypeBoolean = Arrays.copyOf(mTypeBoolean, mTypeBoolean.Length * 2);
				mTypeBoolean = mTypeBoolean.Copy(mTypeBoolean.Length * 2);
				//mValueBoolean = Arrays.copyOf(mValueBoolean, mValueBoolean.Length * 2);
				mValueBoolean = mValueBoolean.Copy(mValueBoolean.Length * 2);
			}
			mTypeBoolean[mCountBoolean] = type;
			mValueBoolean[mCountBoolean++] = value;
		}

		public virtual void applyDelta(TypedValues values)
		{
			for (int i = 0; i < mCountInt; i++)
			{
				values.setValue(mTypeInt[i], mValueInt[i]);
			}
			for (int i = 0; i < mCountFloat; i++)
			{
				values.setValue(mTypeFloat[i], mValueFloat[i]);
			}
			for (int i = 0; i < mCountString; i++)
			{
				values.setValue(mTypeString[i], mValueString[i]);
			}
			for (int i = 0; i < mCountBoolean; i++)
			{
				values.setValue(mTypeBoolean[i], mValueBoolean[i]);
			}
		}
		public virtual void applyDelta(TypedBundle values)
		{
			for (int i = 0; i < mCountInt; i++)
			{
				values.add(mTypeInt[i], mValueInt[i]);
			}
			for (int i = 0; i < mCountFloat; i++)
			{
				values.add(mTypeFloat[i], mValueFloat[i]);
			}
			for (int i = 0; i < mCountString; i++)
			{
				values.add(mTypeString[i], mValueString[i]);
			}
			for (int i = 0; i < mCountBoolean; i++)
			{
				values.add(mTypeBoolean[i], mValueBoolean[i]);
			}
		}


		public virtual void clear()
		{
			mCountBoolean = 0;
			mCountString = 0;
			mCountFloat = 0;
			mCountInt = 0;
		}
	}

}