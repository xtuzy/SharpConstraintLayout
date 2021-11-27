using System;
using System.Collections.Generic;
using System.Text;

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

namespace androidx.constraintlayout.core.state
{
    using CustomVariable = androidx.constraintlayout.core.motion.CustomVariable;
    using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
    using androidx.constraintlayout.core.parser;
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;


    /// <summary>
    /// Utility class to encapsulate layout of a widget
    /// </summary>
    public class WidgetFrame
    {
        private const bool OLD_SYSTEM = true;
        public ConstraintWidget widget = null;
        public int left = 0;
        public int top = 0;
        public int right = 0;
        public int bottom = 0;

        // transforms

        public float pivotX = float.NaN;
        public float pivotY = float.NaN;

        public float rotationX = float.NaN;
        public float rotationY = float.NaN;
        public float rotationZ = float.NaN;

        public float translationX = float.NaN;
        public float translationY = float.NaN;
        public float translationZ = float.NaN;
        public static float phone_orientation = float.NaN;

        public float scaleX = float.NaN;
        public float scaleY = float.NaN;

        public float alpha = float.NaN;
        public float interpolatedPos = float.NaN;

        public int visibility = ConstraintWidget.VISIBLE;

        public readonly Dictionary<string, CustomVariable> mCustom = new Dictionary<string, CustomVariable>();

        public string name = null;

        public virtual int width()
        {
            return Math.Max(0, right - left);
        }

        public virtual int height()
        {
            return Math.Max(0, bottom - top);
        }

        public WidgetFrame()
        {
        }

        public WidgetFrame(ConstraintWidget widget)
        {
            this.widget = widget;
        }

        public WidgetFrame(WidgetFrame frame)
        {
            widget = frame.widget;
            left = frame.left;
            top = frame.top;
            right = frame.right;
            bottom = frame.bottom;
            updateAttributes(frame);
        }

        public virtual void updateAttributes(WidgetFrame frame)
        {
            pivotX = frame.pivotX;
            pivotY = frame.pivotY;
            rotationX = frame.rotationX;
            rotationY = frame.rotationY;
            rotationZ = frame.rotationZ;
            translationX = frame.translationX;
            translationY = frame.translationY;
            translationZ = frame.translationZ;
            scaleX = frame.scaleX;
            scaleY = frame.scaleY;
            alpha = frame.alpha;
            visibility = frame.visibility;

            mCustom.Clear();
            if (frame != null)
            {
                foreach (CustomVariable c in frame.mCustom.Values)
                {
                    mCustom[c.Name] = c.copy();
                }
            }
        }

        public virtual bool DefaultTransform
        {
            get
            {
                return float.IsNaN(rotationX) && float.IsNaN(rotationY) && float.IsNaN(rotationZ) && float.IsNaN(translationX) && float.IsNaN(translationY) && float.IsNaN(translationZ) && float.IsNaN(scaleX) && float.IsNaN(scaleY) && float.IsNaN(alpha);
            }
        }

        public static void interpolate(int parentWidth, int parentHeight, WidgetFrame frame, WidgetFrame start, WidgetFrame end, Transition transition, float progress)
        {
            int frameNumber = (int)(progress * 100);
            int startX = start.left;
            int startY = start.top;
            int endX = end.left;
            int endY = end.top;
            int startWidth = start.right - start.left;
            int startHeight = start.bottom - start.top;
            int endWidth = end.right - end.left;
            int endHeight = end.bottom - end.top;

            float progressPosition = progress;

            float startAlpha = start.alpha;
            float endAlpha = end.alpha;

            if (start.visibility == ConstraintWidget.GONE)
            {
                // On visibility gone, keep the same size to do an alpha to zero
                startX -= (int)(endWidth / 2f);
                startY -= (int)(endHeight / 2f);
                startWidth = endWidth;
                startHeight = endHeight;
                if (float.IsNaN(startAlpha))
                {
                    // override only if not defined...
                    startAlpha = 0f;
                }
            }

            if (end.visibility == ConstraintWidget.GONE)
            {
                // On visibility gone, keep the same size to do an alpha to zero
                endX -= (int)(startWidth / 2f);
                endY -= (int)(startHeight / 2f);
                endWidth = startWidth;
                endHeight = startHeight;
                if (float.IsNaN(endAlpha))
                {
                    // override only if not defined...
                    endAlpha = 0f;
                }
            }

            if (float.IsNaN(startAlpha) && !float.IsNaN(endAlpha))
            {
                startAlpha = 1f;
            }
            if (!float.IsNaN(startAlpha) && float.IsNaN(endAlpha))
            {
                endAlpha = 1f;
            }

            if (start.visibility == ConstraintWidget.INVISIBLE)
            {
                startAlpha = 0f;
            }

            if (end.visibility == ConstraintWidget.INVISIBLE)
            {
                endAlpha = 0f;
            }

            if (frame.widget != null && transition.hasPositionKeyframes())
            {
                Transition.KeyPosition firstPosition = transition.findPreviousPosition(frame.widget.stringId, frameNumber);
                Transition.KeyPosition lastPosition = transition.findNextPosition(frame.widget.stringId, frameNumber);

                if (firstPosition == lastPosition)
                {
                    lastPosition = null;
                }
                int interpolateStartFrame = 0;
                int interpolateEndFrame = 100;

                if (firstPosition != null)
                {
                    startX = (int)(firstPosition.x * parentWidth);
                    startY = (int)(firstPosition.y * parentHeight);
                    interpolateStartFrame = firstPosition.frame;
                }
                if (lastPosition != null)
                {
                    endX = (int)(lastPosition.x * parentWidth);
                    endY = (int)(lastPosition.y * parentHeight);
                    interpolateEndFrame = lastPosition.frame;
                }

                progressPosition = (progress * 100f - interpolateStartFrame) / (float)(interpolateEndFrame - interpolateStartFrame);
            }

            frame.widget = start.widget;

            frame.left = (int)(startX + progressPosition * (endX - startX));
            frame.top = (int)(startY + progressPosition * (endY - startY));
            int width = (int)((1 - progress) * startWidth + (progress * endWidth));
            int height = (int)((1 - progress) * startHeight + (progress * endHeight));
            frame.right = frame.left + width;
            frame.bottom = frame.top + height;

            frame.pivotX = interpolate(start.pivotX, end.pivotX, 0.5f, progress);
            frame.pivotY = interpolate(start.pivotY, end.pivotY, 0.5f, progress);

            frame.rotationX = interpolate(start.rotationX, end.rotationX, 0f, progress);
            frame.rotationY = interpolate(start.rotationY, end.rotationY, 0f, progress);
            frame.rotationZ = interpolate(start.rotationZ, end.rotationZ, 0f, progress);

            frame.scaleX = interpolate(start.scaleX, end.scaleX, 1f, progress);
            frame.scaleY = interpolate(start.scaleY, end.scaleY, 1f, progress);

            frame.translationX = interpolate(start.translationX, end.translationX, 0f, progress);
            frame.translationY = interpolate(start.translationY, end.translationY, 0f, progress);
            frame.translationZ = interpolate(start.translationZ, end.translationZ, 0f, progress);

            frame.alpha = interpolate(startAlpha, endAlpha, 1f, progress);
        }

        private static float interpolate(float start, float end, float defaultValue, float progress)
        {
            bool isStartUnset = float.IsNaN(start);
            bool isEndUnset = float.IsNaN(end);
            if (isStartUnset && isEndUnset)
            {
                return float.NaN;
            }
            if (isStartUnset)
            {
                start = defaultValue;
            }
            if (isEndUnset)
            {
                end = defaultValue;
            }
            return (start + progress * (end - start));
        }

        public virtual float centerX()
        {
            return left + (right - left) / 2f;
        }

        public virtual float centerY()
        {
            return top + (bottom - top) / 2f;
        }

        public virtual WidgetFrame update()
        {
            if (widget != null)
            {
                left = widget.Left;
                top = widget.Top;
                right = widget.Right;
                bottom = widget.Bottom;
                WidgetFrame frame = widget.frame;
                updateAttributes(frame);
            }
            return this;
        }

        public virtual WidgetFrame update(ConstraintWidget widget)
        {
            if (widget == null)
            {
                return this;
            }

            this.widget = widget;
            update();
            return this;
        }

        public virtual void addCustomColor(string name, int color)
        {
            setCustomAttribute(name, TypedValues.TypedValues_Custom.TYPE_COLOR, color);
        }

        public virtual int getCustomColor(string name)
        {
            if (mCustom.ContainsKey(name))
            {
                int color = mCustom[name].ColorValue;
                return color;
            }
            return unchecked((int)0xFFFFAA88);
        }

        public virtual void addCustomFloat(string name, float value)
        {
            setCustomAttribute(name, TypedValues.TypedValues_Custom.TYPE_FLOAT, value);
        }

        public virtual float getCustomFloat(string name)
        {
            if (mCustom.ContainsKey(name))
            {
                return mCustom[name].FloatValue;
            }
            return float.NaN;
        }

        public virtual void setCustomAttribute(string name, int type, float value)
        {
            if (mCustom.ContainsKey(name))
            {
                mCustom[name].FloatValue = value;
            }
            else
            {
                mCustom[name] = new CustomVariable(name, type, value);
            }
        }

        public virtual void setCustomAttribute(string name, int type, int value)
        {
            if (mCustom.ContainsKey(name))
            {
                mCustom[name].IntValue = value;
            }
            else
            {
                mCustom[name] = new CustomVariable(name, type, value);
            }
        }

        public virtual void setCustomAttribute(string name, int type, bool value)
        {
            if (mCustom.ContainsKey(name))
            {
                mCustom[name].BooleanValue = value;
            }
            else
            {
                mCustom[name] = new CustomVariable(name, type, value);
            }
        }

        public virtual void setCustomAttribute(string name, int type, string value)
        {
            if (mCustom.ContainsKey(name))
            {
                mCustom[name].StringValue = value;
            }
            else
            {
                mCustom[name] = new CustomVariable(name, type, value);
            }
        }

        public virtual CustomVariable getCustomAttribute(string name)
        {
            return mCustom[name];
        }

        public virtual ICollection<string> CustomAttributeNames
        {
            get
            {
                return mCustom.Keys;
            }
        }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean setValue(String key, CLElement value) throws CLParsingException
        public virtual bool setValue(string key, CLElement value)
        {
            switch (key)
            {
                case "pivotX":
                    pivotX = value.Float;
                    break;
                case "pivotY":
                    pivotY = value.Float;
                    break;
                case "rotationX":
                    rotationX = value.Float;
                    break;
                case "rotationY":
                    rotationY = value.Float;
                    break;
                case "rotationZ":
                    rotationZ = value.Float;
                    break;
                case "translationX":
                    translationX = value.Float;
                    break;
                case "translationY":
                    translationY = value.Float;
                    break;
                case "translationZ":
                    translationZ = value.Float;
                    break;
                case "scaleX":
                    scaleX = value.Float;
                    break;
                case "scaleY":
                    scaleY = value.Float;
                    break;
                case "alpha":
                    alpha = value.Float;
                    break;
                case "interpolatedPos":
                    interpolatedPos = value.Float;
                    break;
                case "phone_orientation":
                    phone_orientation = value.Float;
                    break;
                case "top":
                    top = value.Int;
                    break;
                case "left":
                    left = value.Int;
                    break;
                case "right":
                    right = value.Int;
                    break;
                case "bottom":
                    bottom = value.Int;
                    break;
                case "custom":
                    parseCustom(value);
                    break;

                default:
                    return false;
            }
            return true;
        }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void parseCustom(CLElement custom) throws CLParsingException
        internal virtual void parseCustom(CLElement custom)
        {
            CLObject obj = ((CLObject) custom);
            int n = obj.size();
            for (int i = 0; i < n; i++)
            {
                CLElement tmp = obj.get(i);
                CLKey k = ((CLKey) tmp);
                string name = k.content();
                CLElement v = k.Value;
                string vStr = v.content();
                //if (vStr.matches("#[0-9a-fA-F]+"))
                if (vStr.Matches("#[0-9a-fA-F]+"))
                {
                    int color = Convert.ToInt32(vStr.Substring(1), 16);
                    setCustomAttribute(k.content(), TypedValues.TypedValues_Custom.TYPE_COLOR, color);
                }
                else if (v is CLNumber)
                {
                    setCustomAttribute(k.content(), TypedValues.TypedValues_Custom.TYPE_FLOAT, v.Float);
                }
                else
                {
                    setCustomAttribute(k.content(), TypedValues.TypedValues_Custom.TYPE_STRING, vStr);

                }
            }
        }


        public virtual StringBuilder serialize(StringBuilder ret)
        {
            return serialize(ret, false);
        }

        /// <summary>
        /// If true also send the phone orientation </summary>
        /// <param name="ret"> </param>
        /// <param name="sendPhoneOrientation">
        /// @return </param>
        public virtual StringBuilder serialize(StringBuilder ret, bool sendPhoneOrientation)
        {
            WidgetFrame frame = this;
            ret.Append("{\n");
            add(ret, "left", frame.left);
            add(ret, "top", frame.top);
            add(ret, "right", frame.right);
            add(ret, "bottom", frame.bottom);
            add(ret, "pivotX", frame.pivotX);
            add(ret, "pivotY", frame.pivotY);
            add(ret, "rotationX", frame.rotationX);
            add(ret, "rotationY", frame.rotationY);
            add(ret, "rotationZ", frame.rotationZ);
            add(ret, "translationX", frame.translationX);
            add(ret, "translationY", frame.translationY);
            add(ret, "translationZ", frame.translationZ);
            add(ret, "scaleX", frame.scaleX);
            add(ret, "scaleY", frame.scaleY);
            add(ret, "alpha", frame.alpha);
            add(ret, "visibility", frame.left);
            add(ret, "interpolatedPos", frame.interpolatedPos);
            if (widget != null)
            {
                foreach (ConstraintAnchor.Type side in Enum.GetValues(typeof(ConstraintAnchor.Type)))
                {
                    serializeAnchor(ret, side);
                }
            }
            if (sendPhoneOrientation)
            {
                add(ret, "phone_orientation", phone_orientation);
            }
            if (sendPhoneOrientation)
            {
                add(ret, "phone_orientation", phone_orientation);
            }

            if (frame.mCustom.Count != 0)
            {
                ret.Append("custom : {\n");
                foreach (string s in frame.mCustom.Keys)
                {
                    CustomVariable value = frame.mCustom[s];
                    ret.Append(s);
                    ret.Append(": ");
                    switch (value.Type)
                    {
                        case TypedValues.TypedValues_Custom.TYPE_INT:
                            ret.Append(value.IntegerValue);
                            ret.Append(",\n");
                            break;
                        case TypedValues.TypedValues_Custom.TYPE_FLOAT:
                        case TypedValues.TypedValues_Custom.TYPE_DIMENSION:
                            ret.Append(value.FloatValue);
                            ret.Append(",\n");
                            break;
                        case TypedValues.TypedValues_Custom.TYPE_COLOR:
                            ret.Append("'");
                            ret.Append(CustomVariable.colorString(value.IntegerValue));
                            ret.Append("',\n");
                            break;
                        case TypedValues.TypedValues_Custom.TYPE_STRING:
                            ret.Append("'");
                            ret.Append(value.StringValue);
                            ret.Append("',\n");
                            break;
                        case TypedValues.TypedValues_Custom.TYPE_BOOLEAN:
                            ret.Append("'");
                            ret.Append(value.BooleanValue);
                            ret.Append("',\n");
                            break;
                    }
                }
                ret.Append("}\n");
            }

            ret.Append("}\n");
            return ret;
        }
        private void serializeAnchor(StringBuilder ret, ConstraintAnchor.Type type)
        {
             ConstraintAnchor anchor = widget.getAnchor(type);
            if (anchor == null || anchor.mTarget == null)
            {
                return;
            }
            ret.Append("Anchor");
            ret.Append(type.ToString());
            ret.Append(": ['");
            string str = anchor.mTarget.Owner.stringId;
            ret.Append(string.ReferenceEquals(str, null)?"#PARENT":str);
            ret.Append("', '");
            ret.Append(anchor.mTarget.getType().ToString());
            ret.Append("', '");
            ret.Append(anchor.mMargin);
            ret.Append("'],\n");

        }
        private static void add(StringBuilder s, string title, int value)
        {
            s.Append(title);
            s.Append(": ");
            s.Append(value);
            s.Append(",\n");
        }

        private static void add(StringBuilder s, string title, float value)
        {
            if (float.IsNaN(value))
            {
                return;
            }
            s.Append(title);
            s.Append(": ");
            s.Append(value);
            s.Append(",\n");
        }

        internal virtual void printCustomAttributes()
        {
            //StackTraceElement s = (new Exception()).StackTrace[1];
            System.Diagnostics.StackFrame s = new System.Diagnostics.StackTrace(new Exception(), true).GetFrame(0);//需要pdb文件,https://stackoverflow.com/questions/3328990/how-can-i-get-the-line-number-which-threw-exception
            string ss = ".(" + s.GetFileName() + ":" + s.GetFileLineNumber() + ") " + s.GetMethod().Name;
            ss += " " + (this.GetHashCode() % 1000);
            if (widget != null)
            {
                ss += "/" + (widget.GetHashCode() % 1000) + " ";
            }
            else
            {
                ss += "/NULL ";
            }
            if (mCustom != null)
            {
                foreach (string key in mCustom.Keys)
                {
                    Console.WriteLine(ss + mCustom[key].ToString());
                }
            }
        }

        internal virtual void logv(string str)
        {
            //StackTraceElement s = (new Exception()).StackTrace[1];
            System.Diagnostics.StackFrame s = new System.Diagnostics.StackTrace(new Exception(), true).GetFrame(0);//需要pdb文件,https://stackoverflow.com/questions/3328990/how-can-i-get-the-line-number-which-threw-exception
            string ss = ".(" + s.GetFileName() + ":" + s.GetFileLineNumber() + ") " + s.GetMethod().Name;
            ss += " " + (this.GetHashCode() % 1000);
            if (widget != null)
            {
                ss += "/" + (widget.GetHashCode() % 1000);
            }
            else
            {
                ss += "/NULL";
            }

            Console.WriteLine(ss + " " + str);
        }

    }

}