using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2017 The Android Open Source Project
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
namespace SharpConstraintLayout.Maui.Platforms.Androids
{
	using Context = android.content.Context;
	using TypedArray = android.content.res.TypedArray;
	using Color = android.graphics.Color;
	using ColorDrawable = android.graphics.drawable.ColorDrawable;
	using Drawable = android.graphics.drawable.Drawable;
	using AttributeSet = android.util.AttributeSet;
	using Log = android.util.Log;
	using TypedValue = android.util.TypedValue;
	using Xml = android.util.Xml;
	using View = android.view.View;

	using Debug = androidx.constraintlayout.motion.widget.Debug;

	using XmlPullParser = org.xmlpull.v1.XmlPullParser;


	/// <summary>
	/// Defines non standard Attributes
	/// 
	/// @suppress
	/// </summary>
	public class ConstraintAttribute
	{
		private const string TAG = "TransitionLayout";
		private bool mMethod = false;
		internal string mName;
		private AttributeType mType;
		private int mIntegerValue;
		private float mFloatValue;
		private string mStringValue;
		internal bool mBooleanValue;
		private int mColorValue;

		public enum AttributeType
		{
			INT_TYPE,
			FLOAT_TYPE,
			COLOR_TYPE,
			COLOR_DRAWABLE_TYPE,
			STRING_TYPE,
			BOOLEAN_TYPE,
			DIMENSION_TYPE,
			REFERENCE_TYPE
		}

		public virtual AttributeType Type
		{
			get
			{
				return mType;
			}
		}

		/// <summary>
		/// Continuous types are interpolated they are fired only at
		/// @return
		/// </summary>
		public virtual bool Continuous
		{
			get
			{
			   switch (mType)
			   {
				   case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.REFERENCE_TYPE:
				   case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
				   case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
					   return false;
				   default:
					   return true;
			   }
			}
		}

		public virtual float FloatValue
		{
			set
			{
				mFloatValue = value;
			}
		}

		public virtual int ColorValue
		{
			set
			{
				mColorValue = value;
			}
		}

		public virtual int IntValue
		{
			set
			{
				mIntegerValue = value;
			}
		}

		public virtual string StringValue
		{
			set
			{
				mStringValue = value;
			}
		}

		/// <summary>
		/// The number of interpolation values that need to be interpolated
		/// Typically 1 but 3 for colors.
		/// </summary>
		/// <returns> Typically 1 but 3 for colors. </returns>
		public virtual int numberOfInterpolatedValues()
		{
			switch (mType)
			{
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
					return 4;
				default:
					return 1;
			}
		}

		/// <summary>
		/// Transforms value to a float for the purpose of interpolation
		/// </summary>
		/// <returns> interpolation value </returns>
		public virtual float ValueToInterpolate
		{
			get
			{
				switch (mType)
				{
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.INT_TYPE:
						return mIntegerValue;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.FLOAT_TYPE:
						return mFloatValue;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
						throw new Exception("Color does not have a single color to interpolate");
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
						throw new Exception("Cannot interpolate String");
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
						return mBooleanValue ? 1 : 0;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.DIMENSION_TYPE:
						return mFloatValue;
				}
				return Float.NaN;
			}
		}

		public virtual void getValuesToInterpolate(float[] ret)
		{
			switch (mType)
			{
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.INT_TYPE:
					ret[0] = mIntegerValue;
					break;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.FLOAT_TYPE:
					ret[0] = mFloatValue;
					break;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
					int a = 0xFF & (mColorValue >> 24);
					int r = 0xFF & (mColorValue >> 16);
					int g = 0xFF & (mColorValue >> 8);
					int b = 0xFF & (mColorValue);
					float f_r = (float) Math.Pow(r / 255.0f, 2.2);
					float f_g = (float) Math.Pow(g / 255.0f, 2.2);
					float f_b = (float) Math.Pow(b / 255.0f, 2.2);
					ret[0] = f_r;
					ret[1] = f_g;
					ret[2] = f_b;
					ret[3] = a / 255f;
					break;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
					throw new Exception("Color does not have a single color to interpolate");
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
					ret[0] = mBooleanValue ? 1 : 0;
					break;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.DIMENSION_TYPE:
					ret[0] = mFloatValue;
					break;
			}
		}

		public virtual float[] Value
		{
			set
			{
				switch (mType)
				{
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.REFERENCE_TYPE:
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.INT_TYPE:
						mIntegerValue = (int) value[0];
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.FLOAT_TYPE:
						mFloatValue = value[0];
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
						mColorValue = Color.HSVToColor(value);
						mColorValue = (mColorValue & 0xFFFFFF) | (clamp((int)(0xFF * value[3])) << 24);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
						throw new Exception("Color does not have a single color to interpolate");
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
						mBooleanValue = value[0] > 0.5;
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.DIMENSION_TYPE:
						mFloatValue = value[0];
    
					break;
				}
			}
		}

		/// <summary>
		/// test if the two attributes are different
		/// </summary>
		/// <param name="constraintAttribute">
		/// @return </param>
		public virtual bool diff(ConstraintAttribute constraintAttribute)
		{
			if (constraintAttribute == null || mType != constraintAttribute.mType)
			{
				return false;
			}
			switch (mType)
			{
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.INT_TYPE:
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.REFERENCE_TYPE:
					return mIntegerValue == constraintAttribute.mIntegerValue;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.FLOAT_TYPE:
					return mFloatValue == constraintAttribute.mFloatValue;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
					return mColorValue == constraintAttribute.mColorValue;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
					return mIntegerValue == constraintAttribute.mIntegerValue;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
					return mBooleanValue == constraintAttribute.mBooleanValue;
				case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.DIMENSION_TYPE:
					return mFloatValue == constraintAttribute.mFloatValue;
			}
			return false;
		}

		public ConstraintAttribute(string name, AttributeType attributeType)
		{
			mName = name;
			mType = attributeType;
		}

		public ConstraintAttribute(string name, AttributeType attributeType, object value, bool method)
		{
			mName = name;
			mType = attributeType;
			mMethod = method;
			Value = value;
		}

		public ConstraintAttribute(ConstraintAttribute source, object value)
		{
			mName = source.mName;
			mType = source.mType;
			Value = value;

		}

		public virtual object Value
		{
			set
			{
				switch (mType)
				{
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.REFERENCE_TYPE:
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.INT_TYPE:
						mIntegerValue = (int?) value.Value;
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.FLOAT_TYPE:
						mFloatValue = (float?) value.Value;
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
						mColorValue = (int?) value.Value;
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
						mStringValue = (string) value;
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
						mBooleanValue = (bool?) value.Value;
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.DIMENSION_TYPE:
						mFloatValue = (float?) value.Value;
						break;
				}
			}
		}

		public static Dictionary<string, ConstraintAttribute> extractAttributes(Dictionary<string, ConstraintAttribute> @base, View view)
		{
			Dictionary<string, ConstraintAttribute> ret = new Dictionary<string, ConstraintAttribute>();
			Type viewClass = view.GetType();
			foreach (string name in @base.Keys)
			{
				ConstraintAttribute constraintAttribute = @base[name];

				try
				{
					if (name.Equals("BackgroundColor"))
					{ // hack for getMap set background color
						ColorDrawable viewColor = (ColorDrawable) view.Background;
						object val = viewColor.Color;
						ret[name] = new ConstraintAttribute(constraintAttribute, val);
					}
					else
					{
						Method method = viewClass.GetMethod("getMap" + name);
						object val = method.invoke(view);
						ret[name] = new ConstraintAttribute(constraintAttribute, val);
					}

				}
				catch (NoSuchMethodException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				catch (IllegalAccessException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				catch (InvocationTargetException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
			return ret;
		}

		public static void setAttributes(View view, Dictionary<string, ConstraintAttribute> map)
		{
			Type viewClass = view.GetType();
			foreach (string name in map.Keys)
			{
				ConstraintAttribute constraintAttribute = map[name];
				string methodName = name;
				if (!constraintAttribute.mMethod)
				{
					methodName = "set" + methodName;
				}
				try
				{
					Method method;
					switch (constraintAttribute.mType)
					{
						case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.INT_TYPE:
							method = viewClass.GetMethod(methodName, Integer.TYPE);
							method.invoke(view, constraintAttribute.mIntegerValue);
							break;
						case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.FLOAT_TYPE:
							method = viewClass.GetMethod(methodName, Float.TYPE);
							method.invoke(view, constraintAttribute.mFloatValue);
							break;
						case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
							method = viewClass.GetMethod(methodName, typeof(Drawable));
							ColorDrawable drawable = new ColorDrawable(); // TODO cache
							drawable.Color = constraintAttribute.mColorValue;
							method.invoke(view, drawable);
							break;
						case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
							method = viewClass.GetMethod(methodName, Integer.TYPE);
							method.invoke(view, constraintAttribute.mColorValue);
							break;
						case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
							method = viewClass.GetMethod(methodName, typeof(CharSequence));
							method.invoke(view, constraintAttribute.mStringValue);
							break;
						case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
							method = viewClass.GetMethod(methodName, Boolean.TYPE);
							method.invoke(view, constraintAttribute.mBooleanValue);
							break;
						case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.DIMENSION_TYPE:
							method = viewClass.GetMethod(methodName, Float.TYPE);
							method.invoke(view, constraintAttribute.mFloatValue);
							break;
						case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.REFERENCE_TYPE:
							method = viewClass.GetMethod(methodName, Integer.TYPE);
							method.invoke(view, constraintAttribute.mIntegerValue);
						break;
					}
				}
				catch (NoSuchMethodException e)
				{
					Log.e(TAG, e.Message);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Log.e(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Log.e(TAG, viewClass.FullName + " must have a method " + methodName);
				}
				catch (IllegalAccessException e)
				{
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Log.e(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				catch (InvocationTargetException e)
				{
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Log.e(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		public virtual void applyCustom(View view)
		{
			Type viewClass = view.GetType();
			string name = this.mName;
			string methodName = name;
			if (!mMethod)
			{
				methodName = "set" + methodName;
			}
			try
			{
				Method method;
				switch (this.mType)
				{
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.INT_TYPE:
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.REFERENCE_TYPE:
						method = viewClass.GetMethod(methodName, Integer.TYPE);
						method.invoke(view, this.mIntegerValue);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.FLOAT_TYPE:
						method = viewClass.GetMethod(methodName, Float.TYPE);
						method.invoke(view, this.mFloatValue);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
						method = viewClass.GetMethod(methodName, typeof(Drawable));
						ColorDrawable drawable = new ColorDrawable(); // TODO cache
						drawable.Color = this.mColorValue;
						method.invoke(view, drawable);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
						method = viewClass.GetMethod(methodName, Integer.TYPE);
						method.invoke(view, this.mColorValue);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
						method = viewClass.GetMethod(methodName, typeof(CharSequence));
						method.invoke(view, this.mStringValue);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
						method = viewClass.GetMethod(methodName, Boolean.TYPE);
						method.invoke(view, this.mBooleanValue);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.DIMENSION_TYPE:
						method = viewClass.GetMethod(methodName, Float.TYPE);
						method.invoke(view, this.mFloatValue);
						break;
				}
			}
			catch (NoSuchMethodException e)
			{
				Log.e(TAG, e.Message);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Log.e(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Log.e(TAG, viewClass.FullName + " must have a method " + methodName);
			}
			catch (IllegalAccessException e)
			{
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Log.e(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (InvocationTargetException e)
			{
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Log.e(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

		}

		private static int clamp(int c)
		{
			int N = 255;
			c &= ~(c >> 31);
			c -= N;
			c &= (c >> 31);
			c += N;
			return c;
		}

		public virtual void setInterpolatedValue(View view, float[] value)
		{
			Type viewClass = view.GetType();

			string methodName = "set" + mName;
			try
			{
				Method method;
				switch (mType)
				{
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.INT_TYPE:
						method = viewClass.GetMethod(methodName, Integer.TYPE);
						method.invoke(view, (int) value[0]);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.FLOAT_TYPE:
						method = viewClass.GetMethod(methodName, Float.TYPE);
						method.invoke(view, value[0]);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
					{
						method = viewClass.GetMethod(methodName, typeof(Drawable));
						int r = clamp((int)((float) Math.Pow(value[0], 1.0 / 2.2) * 255.0f));
						int g = clamp((int)((float) Math.Pow(value[1], 1.0 / 2.2) * 255.0f));
						int b = clamp((int)((float) Math.Pow(value[2], 1.0 / 2.2) * 255.0f));
						int a = clamp((int)(value[3] * 255.0f));
						int color = a << 24 | (r << 16) | (g << 8) | b;
						ColorDrawable drawable = new ColorDrawable(); // TODO cache
						drawable.Color = color;
						method.invoke(view, drawable);
					}
					break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.COLOR_TYPE:
						method = viewClass.GetMethod(methodName, Integer.TYPE);
						int r = clamp((int)((float) Math.Pow(value[0], 1.0 / 2.2) * 255.0f));
						int g = clamp((int)((float) Math.Pow(value[1], 1.0 / 2.2) * 255.0f));
						int b = clamp((int)((float) Math.Pow(value[2], 1.0 / 2.2) * 255.0f));
						int a = clamp((int)(value[3] * 255.0f));
						int color = a << 24 | (r << 16) | (g << 8) | b;
						method.invoke(view, color);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.STRING_TYPE:
						throw new Exception("unable to interpolate strings " + mName);

					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.BOOLEAN_TYPE:
						method = viewClass.GetMethod(methodName, Boolean.TYPE);
						method.invoke(view, value[0] > 0.5f);
						break;
					case androidx.constraintlayout.widget.ConstraintAttribute.AttributeType.DIMENSION_TYPE:
						method = viewClass.GetMethod(methodName, Float.TYPE);
						method.invoke(view, value[0]);
						break;
				}
			}
			catch (NoSuchMethodException e)
			{
				Log.e(TAG, "no method " + methodName + " on View \"" + Debug.getName(view) + "\"");
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (IllegalAccessException e)
			{
				Log.e(TAG, "cannot access method " + methodName + " on View \"" + Debug.getName(view) + "\"");
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (InvocationTargetException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		public static void parse(Context context, XmlPullParser parser, Dictionary<string, ConstraintAttribute> custom)
		{
			AttributeSet attributeSet = Xml.asAttributeSet(parser);
			TypedArray a = context.obtainStyledAttributes(attributeSet, R.styleable.CustomAttribute);
			string name = null;
			bool method = false;
			object value = null;
			AttributeType type = null;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
			int N = a.IndexCount;
			for (int i = 0; i < N; i++)
			{
				int attr = a.getIndex(i);
				if (attr == R.styleable.CustomAttribute_attributeName)
				{
					name = a.getString(attr);
					if (!string.ReferenceEquals(name, null) && name.Length > 0)
					{
						name = char.ToUpper(name[0]) + name.Substring(1);
					}
				}
				else if (attr == R.styleable.CustomAttribute_methodName)
				{
					method = true;
					name = a.getString(attr);
				}
				else if (attr == R.styleable.CustomAttribute_customBoolean)
				{
					value = a.getBoolean(attr, false);
					type = AttributeType.BOOLEAN_TYPE;
				}
				else if (attr == R.styleable.CustomAttribute_customColorValue)
				{
					type = AttributeType.COLOR_TYPE;
					value = a.getColor(attr, 0);
				}
				else if (attr == R.styleable.CustomAttribute_customColorDrawableValue)
				{
					type = AttributeType.COLOR_DRAWABLE_TYPE;
					value = a.getColor(attr, 0);
				}
				else if (attr == R.styleable.CustomAttribute_customPixelDimension)
				{
					type = AttributeType.DIMENSION_TYPE;
					value = TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, a.getDimension(attr, 0), context.Resources.DisplayMetrics);
				}
				else if (attr == R.styleable.CustomAttribute_customDimension)
				{
					type = AttributeType.DIMENSION_TYPE;
					value = a.getDimension(attr, 0);
				}
				else if (attr == R.styleable.CustomAttribute_customFloatValue)
				{
					type = AttributeType.FLOAT_TYPE;
					value = a.getFloat(attr, Float.NaN);
				}
				else if (attr == R.styleable.CustomAttribute_customIntegerValue)
				{
					type = AttributeType.INT_TYPE;
					value = a.getInteger(attr, -1);
				}
				else if (attr == R.styleable.CustomAttribute_customStringValue)
				{
					type = AttributeType.STRING_TYPE;
					value = a.getString(attr);
				}
				else if (attr == R.styleable.CustomAttribute_customReference)
				{
					type = AttributeType.REFERENCE_TYPE;
					int tmp = a.getResourceId(attr, -1);
					if (tmp == -1)
					{
						tmp = a.getInt(attr, -1);
					}
					value = tmp;
				}
			}
			if (!string.ReferenceEquals(name, null) && value != null)
			{
				custom[name] = new ConstraintAttribute(name, type, value, method);
			}
			a.recycle();
		}

	}

}