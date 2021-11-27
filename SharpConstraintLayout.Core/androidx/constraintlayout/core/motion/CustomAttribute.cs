using System;
using System.Collections.Generic;
using System.Reflection;

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

    using Utils = androidx.constraintlayout.core.motion.utils.Utils;


    /// <summary>
    /// Defines non standard Attributes
    /// 
    /// @suppress
    /// </summary>
    public class CustomAttribute
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
        /// 
        /// @return
        /// </summary>
        public virtual bool Continuous
        {
            get
            {
                switch (mType)
                {
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.REFERENCE_TYPE:
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
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
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
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
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.INT_TYPE:
                        return mIntegerValue;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.FLOAT_TYPE:
                        return mFloatValue;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
                        throw new Exception("Color does not have a single color to interpolate");
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
                        throw new Exception("Cannot interpolate String");
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                        return mBooleanValue ? 1 : 0;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.DIMENSION_TYPE:
                        return mFloatValue;
                    default:
                        return float.NaN;
                }
            }
        }

        public virtual void getValuesToInterpolate(float[] ret)
        {
            switch (mType)
            {
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.INT_TYPE:
                    ret[0] = mIntegerValue;
                    break;
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.FLOAT_TYPE:
                    ret[0] = mFloatValue;
                    break;
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
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
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
                    throw new Exception("Color does not have a single color to interpolate");
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                    ret[0] = mBooleanValue ? 1 : 0;
                    break;
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.DIMENSION_TYPE:
                    ret[0] = mFloatValue;
                    break;
                default:
                    break;
            }
        }

        public virtual float[] Value
        {
            set
            {
                switch (mType)
                {
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.REFERENCE_TYPE:
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.INT_TYPE:
                        mIntegerValue = (int) value[0];
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.FLOAT_TYPE:
                        mFloatValue = value[0];
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
                        mColorValue = hsvToRgb(value[0], value[1], value[2]);
                        mColorValue = (mColorValue & 0xFFFFFF) | (clamp((int)(0xFF * value[3])) << 24);
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
                        throw new Exception("Color does not have a single color to interpolate");
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                        mBooleanValue = value[0] > 0.5;
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.DIMENSION_TYPE:
                        mFloatValue = value[0];
                        break;
                    default:
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
                default:
                    return 0;
            }
        }

        /// <summary>
        /// test if the two attributes are different
        /// </summary>
        /// <param name="CustomAttribute">
        /// @return </param>
        public virtual bool diff(CustomAttribute CustomAttribute)
        {
            if (CustomAttribute == null || mType != CustomAttribute.mType)
            {
                return false;
            }
            switch (mType)
            {
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.INT_TYPE:
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.REFERENCE_TYPE:
                    return mIntegerValue == CustomAttribute.mIntegerValue;
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.FLOAT_TYPE:
                    return mFloatValue == CustomAttribute.mFloatValue;
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
                    return mColorValue == CustomAttribute.mColorValue;
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
                    return mIntegerValue == CustomAttribute.mIntegerValue;
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                    return mBooleanValue == CustomAttribute.mBooleanValue;
                case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.DIMENSION_TYPE:
                    return mFloatValue == CustomAttribute.mFloatValue;
                default:
                    return false;
            }
        }

        public CustomAttribute(string name, AttributeType attributeType)
        {
            mName = name;
            mType = attributeType;
        }

        public CustomAttribute(string name, AttributeType attributeType, object value, bool method)
        {
            mName = name;
            mType = attributeType;
            mMethod = method;
            setValue(value);
        }

        public CustomAttribute(CustomAttribute source, object value)
        {
            mName = source.mName;
            mType = source.mType;
            setValue(value);

        }

        public virtual void setValue(object value)
        {
            
                switch (mType)
                {
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.REFERENCE_TYPE:
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.INT_TYPE:
                        mIntegerValue = ((int?) value).Value;
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.FLOAT_TYPE:
                        mFloatValue = ((float?) value).Value;
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_DRAWABLE_TYPE:
                        mColorValue = ((int?) value).Value;
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
                        mStringValue = (string) value;
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                        mBooleanValue = ((bool?) value).Value;
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.DIMENSION_TYPE:
                        mFloatValue = ((float?) value).Value;
                        break;
                    default:
                        break;
                }
            
        }

        public static Dictionary<string, CustomAttribute> extractAttributes(Dictionary<string, CustomAttribute> @base, object view)
        {
            Dictionary<string, CustomAttribute> ret = new Dictionary<string, CustomAttribute>();
            Type viewClass = view.GetType();
            foreach (string name in @base.Keys)
            {
                CustomAttribute CustomAttribute = @base[name];

                try
                {

                    System.Reflection.MethodInfo method = viewClass.GetMethod("getMap" + name);//TODO:验证object是否存在getMap
                    object val = method.Invoke(view,new object[] { });
                    ret[name] = new CustomAttribute(CustomAttribute, val);

                }
                //catch (NoSuchMethodException e)
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                }
                //catch (IllegalAccessException e)
                //{
                //    Console.WriteLine(e.ToString());
                //    Console.Write(e.StackTrace);
                //}
                //catch (InvocationTargetException e)
                //{
                //    Console.WriteLine(e.ToString());
                //    Console.Write(e.StackTrace);
                //}
            }
            return ret;
        }

        public static void setAttributes(object view, Dictionary<string, CustomAttribute> map)
        {
            Type viewClass = view.GetType();
            foreach (string name in map.Keys)
            {
                CustomAttribute CustomAttribute = map[name];
                string methodName = name;
                if (!CustomAttribute.mMethod)
                {
                    methodName = "set" + methodName;
                }
                try
                {
                    System.Reflection.MethodInfo method;
                    switch (CustomAttribute.mType)
                    {
                        case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.INT_TYPE:
                            method = viewClass.GetMethod(methodName, new Type[] { typeof(int) });
                            method.Invoke(view, new object[] { CustomAttribute.mIntegerValue });
                            break;
                        case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.FLOAT_TYPE:
                            method = viewClass.GetMethod(methodName, new Type[] { typeof(float) });
                            method.Invoke(view, new object[] { CustomAttribute.mFloatValue });
                            break;
                        case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
                            method = viewClass.GetMethod(methodName, new Type[] { typeof(int) });
                            method.Invoke(view, new object[] { CustomAttribute.mColorValue });
                            break;
                        case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
                            //method = viewClass.GetMethod(methodName, typeof(CharSequence));
                            method = viewClass.GetMethod(methodName, new Type[] { typeof(bool) });
                            method.Invoke(view, new object[] { CustomAttribute.mStringValue });
                            break;
                        case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                            method = viewClass.GetMethod(methodName, new Type[] { typeof(bool) });
                            method.Invoke(view, new object[] { CustomAttribute.mBooleanValue });
                            break;
                        case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.DIMENSION_TYPE:
                            method = viewClass.GetMethod(methodName, new Type[] { typeof(float) });
                            method.Invoke(view, new object[] { CustomAttribute.mFloatValue });
                            break;
                        case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.REFERENCE_TYPE:
                            method = viewClass.GetMethod(methodName, new Type[] { typeof(int) });
                            method.Invoke(view, new object[] { CustomAttribute.mIntegerValue });
                            break;
                        default:
                            break;
                    }
                }
                //catch (NoSuchMethodException e)
                catch (Exception e)
                {
                    Utils.loge(TAG, e.Message);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                    Utils.loge(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                    Utils.loge(TAG, viewClass.FullName + " must have a method " + methodName);
                }
//                catch (IllegalAccessException e)
//                {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//                    Utils.loge(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
//                    Console.WriteLine(e.ToString());
//                    Console.Write(e.StackTrace);
//                }
//                catch (InvocationTargetException e)
//                {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//                    Utils.loge(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
//                    Console.WriteLine(e.ToString());
//                    Console.Write(e.StackTrace);
//                }
            }
        }

        public virtual void applyCustom(object view)
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
                MethodInfo method;
                switch (this.mType)
                {
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.INT_TYPE:
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.REFERENCE_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(int) });
                        method.Invoke(view, new object[] { this.mIntegerValue });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.FLOAT_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(float) });
                        method.Invoke(view, new object[] { this.mFloatValue });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(int) });
                        method.Invoke(view, new object[] { this.mColorValue });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
                        //method = viewClass.GetMethod(methodName, typeof(CharSequence));
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(string) });
                        method.Invoke(view, new object[] { this.mStringValue });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(bool) });
                        method.Invoke(view, new object[] { this.mBooleanValue });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.DIMENSION_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(float) });
                        method.Invoke(view, new object[] { this.mFloatValue });
                        break;
                    default:
                        break;
                }
            }
            //catch (NoSuchMethodException e)
            catch (Exception e)
            {
                Utils.loge(TAG, e.Message);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                Utils.loge(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                Utils.loge(TAG, viewClass.FullName + " must have a method " + methodName);
            }
//            catch (IllegalAccessException e)
//            {
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//                Utils.loge(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
//                Console.WriteLine(e.ToString());
//                Console.Write(e.StackTrace);
//            }
//            catch (InvocationTargetException e)
//            {
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//                Utils.loge(TAG, " Custom Attribute \"" + name + "\" not found on " + viewClass.FullName);
//                Console.WriteLine(e.ToString());
//                Console.Write(e.StackTrace);
//            }

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

        public virtual void setInterpolatedValue(object view, float[] value)
        {
            Type viewClass = view.GetType();

            string methodName = "set" + mName;
            try
            {
                MethodInfo method;
                switch (mType)
                {
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.INT_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(int) });
                        method.Invoke(view,new object[] { (int)value[0] });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.FLOAT_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(float) });
                        method.Invoke(view, new object[] { value[0] });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.COLOR_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(int) });
                        int r = clamp((int)((float) Math.Pow(value[0], 1.0 / 2.2) * 255.0f));
                        int g = clamp((int)((float) Math.Pow(value[1], 1.0 / 2.2) * 255.0f));
                        int b = clamp((int)((float) Math.Pow(value[2], 1.0 / 2.2) * 255.0f));
                        int a = clamp((int)(value[3] * 255.0f));
                        int color = a << 24 | (r << 16) | (g << 8) | b;
                        method.Invoke(view,new object[] { color });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.STRING_TYPE:
                        throw new Exception("unable to interpolate strings " + mName);

                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.BOOLEAN_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(bool) });
                        method.Invoke(view, new object[] { value[0] > 0.5f });
                        break;
                    case androidx.constraintlayout.core.motion.CustomAttribute.AttributeType.DIMENSION_TYPE:
                        method = viewClass.GetMethod(methodName, new Type[] { typeof(float) });
                        method.Invoke(view, new object[] { value[0] });
                        break;
                    default:
                        break;
                }
            }
            //catch (NoSuchMethodException e)
            catch (Exception e)
            {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                Utils.loge(TAG, "no method " + methodName + " on View \"" + view.GetType().FullName + "\"");
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
//            catch (IllegalAccessException e)
//            {
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//                Utils.loge(TAG, "cannot access method " + methodName + " on View \"" + view.GetType().FullName + "\"");
//                Console.WriteLine(e.ToString());
//                Console.Write(e.StackTrace);
//            }
//            catch (InvocationTargetException e)
//            {
//                Console.WriteLine(e.ToString());
//                Console.Write(e.StackTrace);
//            }
        }

    }

}