using System.Collections.Generic;

/*
 * Copyright (C) 2018 The Android Open Source Project
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
namespace androidx.constraintlayout.core.motion.key
{
	using SplineSet = androidx.constraintlayout.core.motion.utils.SplineSet;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;


	/// <summary>
	/// Base class in an element in a KeyFrame
	/// 
	/// @suppress
	/// </summary>

	public abstract class MotionKey : TypedValues
	{
		//public abstract int getId(string name);
		public static int UNSET = -1;
		public int mFramePosition = UNSET;
		internal int mTargetId = UNSET;
		internal string mTargetString = null;
		public int mType;
		public Dictionary<string, CustomVariable> mCustom;

		public abstract void getAttributeNames(HashSet<string> attributes);

		public const string ALPHA = "alpha";
		public const string ELEVATION = "elevation";
		public const string ROTATION = "rotationZ";
		public const string ROTATION_X = "rotationX";

		public const string TRANSITION_PATH_ROTATE = "transitionPathRotate";
		public const string SCALE_X = "scaleX";
		public const string SCALE_Y = "scaleY";


		public const string TRANSLATION_X = "translationX";
		public const string TRANSLATION_Y = "translationY";

		public const string CUSTOM = "CUSTOM";

		public const string VISIBILITY = "visibility";

		internal virtual bool matches(string constraintTag)
		{
			if (string.ReferenceEquals(mTargetString, null) || string.ReferenceEquals(constraintTag, null))
			{
				return false;
			}
			return constraintTag.Matches(mTargetString);
		}

		/// <summary>
		/// Defines method to add a a view to splines derived form this key frame.
		/// The values are written to the spline
		/// </summary>
		/// <param name="splines"> splines to write values to
		/// @suppress </param>
		public abstract void addValues(Dictionary<string, SplineSet> splines);

		/// <summary>
		/// Return the float given a value. If the value is a "Float" object it is casted
		/// </summary>
		/// <param name="value">
		/// @return
		/// @suppress </param>
		internal virtual float toFloat(object value)
		{
			return (value is float?) ? ((float?) value).Value : float.Parse(value.ToString());
		}

		/// <summary>
		/// Return the int version of an object if the value is an Integer object it is casted.
		/// </summary>
		/// <param name="value">
		/// @return
		/// @suppress </param>
		internal virtual int toInt(object value)
		{
			return (value is int?) ? ((int?) value).Value : int.Parse(value.ToString());
		}

		/// <summary>
		/// Return the boolean version this object if the object is a Boolean it is casted.
		/// </summary>
		/// <param name="value">
		/// @return
		/// @suppress </param>
		internal virtual bool toBoolean(object value)
		{
			return (value is bool?) ? ((bool?) value).Value : bool.Parse(value.ToString());
		}

		/// <summary>
		/// Key frame can specify the type of interpolation it wants on various attributes
		/// For each string it set it to -1, CurveFit.LINEAR or  CurveFit.SPLINE
		/// </summary>
		/// <param name="interpolation"> </param>
		public virtual Dictionary<string, int?> Interpolation
		{
			set
			{
			}
		}

		public virtual MotionKey copy(MotionKey src)
		{
			mFramePosition = src.mFramePosition;
			mTargetId = src.mTargetId;
			mTargetString = src.mTargetString;
			mType = src.mType;
			return this;
		}

		public abstract MotionKey clone();

		public virtual MotionKey setViewId(int id)
		{
			mTargetId = id;
			return this;
		}

		/// <summary>
		/// sets the frame position
		/// </summary>
		/// <param name="pos"> </param>
		public virtual int FramePosition
		{
			set
			{
				mFramePosition = value;
			}
			get
			{
				return mFramePosition;
			}
		}


		public override bool setValue(int type, int value)
		{

			switch (type)
			{
				case TypedValues.TYPE_FRAME_POSITION:
					mFramePosition = value;
					return true;
			}
			return false;
		}

		public override bool setValue(int type, float value)
		{
			return false;
		}

		public override bool setValue(int type, string value)
		{
			switch (type)
			{
				case TypedValues.TYPE_TARGET:
					mTargetString = value;
					return true;
			}
			return false;
		}

		public override bool setValue(int type, bool value)
		{
			return false;
		}

		public virtual void setCustomAttribute(string name, int type, float value)
		{
			mCustom[name] = new CustomVariable(name, type, value);
		}

		public virtual void setCustomAttribute(string name, int type, int value)
		{
			mCustom[name] = new CustomVariable(name, type, value);
		}

		public virtual void setCustomAttribute(string name, int type, bool value)
		{
			mCustom[name] = new CustomVariable(name, type, value);
		}

		public virtual void setCustomAttribute(string name, int type, string value)
		{
			mCustom[name] = new CustomVariable(name, type, value);
		}
	}

}