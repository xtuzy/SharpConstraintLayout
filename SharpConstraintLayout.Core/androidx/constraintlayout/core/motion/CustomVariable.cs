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
namespace androidx.constraintlayout.core.motion
{
    using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;

    /// <summary>
    /// Defines non standard Attributes
    /// </summary>
    public class CustomVariable
    {
        private const string TAG = "TransitionLayout";
        internal string mName;
        private int mType;
        private int mIntegerValue = int.MinValue;
        private float mFloatValue = float.NaN;
        private string mStringValue = null;
        internal bool mBooleanValue;

        public virtual CustomVariable copy()
        {
            return new CustomVariable(this);
        }

        public CustomVariable(CustomVariable c)
        {
            mName = c.mName;
            mType = c.mType;
            mIntegerValue = c.mIntegerValue;
            mFloatValue = c.mFloatValue;
            mStringValue = c.mStringValue;
            mBooleanValue = c.mBooleanValue;
        }

        public CustomVariable(string name, int type, string value)
        {
            mName = name;
            mType = type;
            mStringValue = value;
        }

        public CustomVariable(string name, int type, int value)
        {
            mName = name;
            mType = type;
            if (type == TypedValues.TypedValues_Custom.TYPE_FLOAT)
            { // catch int ment for float
                mFloatValue = value;
            }
            else
            {
                mIntegerValue = value;
            }
        }

        public CustomVariable(string name, int type, float value)
        {
            mName = name;
            mType = type;
            mFloatValue = value;
        }

        public CustomVariable(string name, int type, bool value)
        {
            mName = name;
            mType = type;
            mBooleanValue = value;
        }

        public static string colorString(int v)
        {
            string str = "00000000" + v.ToString("x");
            return "#" + str.Substring(str.Length - 8);
        }

        public override string ToString()
        {
            string str = mName + ':';
            switch (mType)
            {
                case TypedValues.TypedValues_Custom.TYPE_INT:
                    return str + mIntegerValue;
                case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                    return str + mFloatValue;
                case TypedValues.TypedValues_Custom.TYPE_COLOR:
                    return str + colorString(mIntegerValue);
                case TypedValues.TypedValues_Custom.TYPE_STRING:
                    return str + mStringValue;
                case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                    return str + (bool?) mBooleanValue;
                case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
                    return str + mFloatValue;
            }
            return str + "????";
        }

        public virtual int Type
        {
            get
            {
                return mType;
            }
        }

        public virtual bool BooleanValue
        {
            get
            {
                return mBooleanValue;
            }
            set
            {
                mBooleanValue = value;
            }
        }

        public virtual float FloatValue
        {
            get
            {
                return mFloatValue;
            }
            set
            {
                mFloatValue = value;
            }
        }

        public virtual int ColorValue
        {
            get
            {
                return mIntegerValue;
            }
        }

        public virtual int IntegerValue
        {
            get
            {
                return mIntegerValue;
            }
        }

        public virtual string StringValue
        {
            get
            {
                return mStringValue;
            }
            set
            {
                mStringValue = value;
            }
        }

        /// <summary>
        /// Continuous types are interpolated they are fired only at
        /// 
        /// @return
        /// </summary>
        public virtual bool Continuous
        {
            get
            {
                switch (mType)
                {
                    case TypedValues.TypedValues_Custom.TYPE_REFERENCE:
                    case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                    case TypedValues.TypedValues_Custom.TYPE_STRING:
                        return false;
                    default:
                        return true;
                }
            }
        }



        public virtual int IntValue
        {
            set
            {
                mIntegerValue = value;
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
                case TypedValues.TypedValues_Custom.TYPE_COLOR:
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
                    case TypedValues.TypedValues_Custom.TYPE_INT:
                        return mIntegerValue;
                    case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                        return mFloatValue;
                    case TypedValues.TypedValues_Custom.TYPE_COLOR:
    
                        throw new Exception("Color does not have a single color to interpolate");
                    case TypedValues.TypedValues_Custom.TYPE_STRING:
                        throw new Exception("Cannot interpolate String");
                    case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                        return mBooleanValue ? 1 : 0;
                    case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
                        return mFloatValue;
                }
                return float.NaN;
            }
        }

        public virtual void getValuesToInterpolate(float[] ret)
        {
            switch (mType)
            {
                case TypedValues.TypedValues_Custom.TYPE_INT:
                    ret[0] = mIntegerValue;
                    break;
                case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                    ret[0] = mFloatValue;
                    break;
                case TypedValues.TypedValues_Custom.TYPE_COLOR:
                    int a = 0xFF & (mIntegerValue >> 24);
                    int r = 0xFF & (mIntegerValue >> 16);
                    int g = 0xFF & (mIntegerValue >> 8);
                    int b = 0xFF & (mIntegerValue);
                    float f_r = (float) Math.Pow(r / 255.0f, 2.2);
                    float f_g = (float) Math.Pow(g / 255.0f, 2.2);
                    float f_b = (float) Math.Pow(b / 255.0f, 2.2);
                    ret[0] = f_r;
                    ret[1] = f_g;
                    ret[2] = f_b;
                    ret[3] = a / 255f;
                    break;
                case TypedValues.TypedValues_Custom.TYPE_STRING:
                    throw new Exception("Color does not have a single color to interpolate");
                case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                    ret[0] = mBooleanValue ? 1 : 0;
                    break;
                case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
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
                    case TypedValues.TypedValues_Custom.TYPE_REFERENCE:
                    case TypedValues.TypedValues_Custom.TYPE_INT:
                        mIntegerValue = (int) value[0];
                        break;
                    case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                    case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
                        mFloatValue = value[0];
                        break;
                    case TypedValues.TypedValues_Custom.TYPE_COLOR:
                        mIntegerValue = hsvToRgb(value[0], value[1], value[2]);
                        mIntegerValue = (mIntegerValue & 0xFFFFFF) | (clamp((int)(0xFF * value[3])) << 24);
                        break;
                    case TypedValues.TypedValues_Custom.TYPE_STRING:
                        throw new Exception("Color does not have a single color to interpolate");
                    case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                        mBooleanValue = value[0] > 0.5;
                        break;
                }
            }
        }

        public static int hsvToRgb(float hue, float saturation, float value)
        {
            int h = (int)(hue * 6);
            float f = hue * 6 - h;
            int p = (int)(0.5f + 255 * value * (1 - saturation));
            int q = (int)(0.5f + 255 * value * (1 - f * saturation));
            int t = (int)(0.5f + 255 * value * (1 - (1 - f) * saturation));
            int v = (int)(0.5f + 255 * value);
            switch (h)
            {
                case 0:
                    return (int)unchecked((int)0XFF000000) | (v << 16) + (t << 8) + p;
                case 1:
                    return (int)unchecked((int)0XFF000000) | (q << 16) + (v << 8) + p;
                case 2:
                    return (int)unchecked((int)0XFF000000) | (p << 16) + (v << 8) + t;
                case 3:
                    return (int)unchecked((int)0XFF000000) | (p << 16) + (q << 8) + v;
                case 4:
                    return (int)unchecked((int)0XFF000000) | (t << 16) + (p << 8) + v;
                case 5:
                    return (int)unchecked((int)0XFF000000) | (v << 16) + (p << 8) + q;

            }
            return 0;
        }

        /// <summary>
        /// test if the two attributes are different
        /// </summary>
        /// <param name="CustomAttribute">
        /// @return </param>
        public virtual bool diff(CustomVariable CustomAttribute)
        {
            if (CustomAttribute == null || mType != CustomAttribute.mType)
            {
                return false;
            }
            switch (mType)
            {
                case TypedValues.TypedValues_Custom.TYPE_INT:
                case TypedValues.TypedValues_Custom.TYPE_REFERENCE:
                    return mIntegerValue == CustomAttribute.mIntegerValue;
                case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                    return mFloatValue == CustomAttribute.mFloatValue;
                case TypedValues.TypedValues_Custom.TYPE_COLOR:
                    return mIntegerValue == CustomAttribute.mIntegerValue;
                case TypedValues.TypedValues_Custom.TYPE_STRING:
                    return mIntegerValue == CustomAttribute.mIntegerValue;
                case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                    return mBooleanValue == CustomAttribute.mBooleanValue;
                case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
                    return mFloatValue == CustomAttribute.mFloatValue;
            }
            return false;
        }

        public CustomVariable(string name, int attributeType)
        {
            mName = name;
            mType = attributeType;
        }

        public CustomVariable(string name, int attributeType, object value)
        {
            mName = name;
            mType = attributeType;
            setValue(value);
        }

        public CustomVariable(CustomVariable source, object value)
        {
            mName = source.mName;
            mType = source.mType;
            setValue(value);

        }

        public virtual void setValue(object value)
        {
            
                switch (mType)
                {
                    case TypedValues.TypedValues_Custom.TYPE_REFERENCE:
                    case TypedValues.TypedValues_Custom.TYPE_INT:
                        mIntegerValue = ((int?) value).Value;
                        break;
                    case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                        mFloatValue = ((float?) value).Value;
                        break;
                    case TypedValues.TypedValues_Custom.TYPE_COLOR:
                        mIntegerValue = ((int?) value).Value;
                        break;
                    case TypedValues.TypedValues_Custom.TYPE_STRING:
                        mStringValue = (string) value;
                        break;
                    case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                        mBooleanValue = ((bool?) value).Value;
                        break;
                    case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
                        mFloatValue = ((float?) value).Value;
                        break;
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

       public virtual int getInterpolatedColor(float[] value)
       {
            int r = clamp((int)((float) Math.Pow(value[0], 1.0 / 2.2) * 255.0f));
            int g = clamp((int)((float) Math.Pow(value[1], 1.0 / 2.2) * 255.0f));
            int b = clamp((int)((float) Math.Pow(value[2], 1.0 / 2.2) * 255.0f));
            int a = clamp((int)(value[3] * 255.0f));
            int color = (a << 24) | (r << 16) | (g << 8) | b;
            return color;
       }

        public virtual void setInterpolatedValue(MotionWidget view, float[] value)
        {

            switch (mType)
            {
                case TypedValues.TypedValues_Custom.TYPE_INT:
                    view.setCustomAttribute(mName, mType, (int) value[0]);
                    break;
                case TypedValues.TypedValues_Custom.TYPE_COLOR:
                    int r = clamp((int)((float) Math.Pow(value[0], 1.0 / 2.2) * 255.0f));
                    int g = clamp((int)((float) Math.Pow(value[1], 1.0 / 2.2) * 255.0f));
                    int b = clamp((int)((float) Math.Pow(value[2], 1.0 / 2.2) * 255.0f));
                    int a = clamp((int)(value[3] * 255.0f));
                    int color = (a << 24) | (r << 16) | (g << 8) | b;
                    view.setCustomAttribute(mName, mType, color);
                    break;
                case TypedValues.TypedValues_Custom.TYPE_REFERENCE:
                case TypedValues.TypedValues_Custom.TYPE_STRING:
                    throw new Exception("unable to interpolate " + mName);
                case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                    view.setCustomAttribute(mName, mType, value[0] > 0.5f);
                    break;
                case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
                case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                    view.setCustomAttribute(mName, mType, value[0]);
                    break;
            }
        }

        public static int rgbaTocColor(float r, float g, float b, float a)
        {
            int ir = clamp((int)(r * 255f));
            int ig = clamp((int)(g * 255f));
            int ib = clamp((int)(b * 255f));
            int ia = clamp((int)(a * 255f));
            int color = (ia << 24) | (ir << 16) | (ig << 8) | ib;
            return color;
        }

        public virtual void applyToWidget(MotionWidget view)
        {
            switch (mType)
            {
                case TypedValues.TypedValues_Custom.TYPE_INT:
                case TypedValues.TypedValues_Custom.TYPE_COLOR:
                case TypedValues.TypedValues_Custom.TYPE_REFERENCE:
                    view.setCustomAttribute(mName, mType, mIntegerValue);
                    break;
                case TypedValues.TypedValues_Custom.TYPE_STRING:
                    view.setCustomAttribute(mName, mType, mStringValue);
                    break;
                case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                    view.setCustomAttribute(mName, mType, mBooleanValue);
                    break;
                case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
                case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                    view.setCustomAttribute(mName, mType, mFloatValue);
                    break;
            }
        }

        public virtual string Name
        {
            get
            {
                return mName;
            }
        }
    }

}