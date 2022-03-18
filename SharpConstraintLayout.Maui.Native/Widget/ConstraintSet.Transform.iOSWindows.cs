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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintSet
    {
        public class Transform
        {
            public bool mApply = false;
            public float rotation = 0;
            public float rotationX = 0;
            public float rotationY = 0;
            public float scaleX = 1;
            public float scaleY = 1;
            public float transformPivotX = float.NaN;
            public float transformPivotY = float.NaN;
            public int transformPivotTarget = Unset;
            public float translationX = 0;
            public float translationY = 0;
            public float translationZ = 0;
            public bool applyElevation = false;
            public float elevation = 0;

            public virtual void copyFrom(Transform src)
            {
                mApply = src.mApply;
                rotation = src.rotation;
                rotationX = src.rotationX;
                rotationY = src.rotationY;
                scaleX = src.scaleX;
                scaleY = src.scaleY;
                transformPivotX = src.transformPivotX;
                transformPivotY = src.transformPivotY;
                transformPivotTarget = src.transformPivotTarget;
                translationX = src.translationX;
                translationY = src.translationY;
                translationZ = src.translationZ;
                applyElevation = src.applyElevation;
                elevation = src.elevation;
            }

            internal static Dictionary<int,int> mapToConstant = new Dictionary<int, int>();
            internal const int ROTATION = 1;
            internal const int ROTATION_X = 2;
            internal const int ROTATION_Y = 3;
            internal const int SCALE_X = 4;
            internal const int SCALE_Y = 5;
            internal const int TRANSFORM_PIVOT_X = 6;
            internal const int TRANSFORM_PIVOT_Y = 7;
            internal const int TRANSLATION_X = 8;
            internal const int TRANSLATION_Y = 9;
            internal const int TRANSLATION_Z = 10;
            internal const int ELEVATION = 11;
            internal const int TRANSFORM_PIVOT_TARGET = 12;

            /*internal virtual void fillFromAttributeList(Context context, AttributeSet attrs)
            {
                TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.Transform);
                mApply = true;
                //ORIGINAL LINE: final int N = a.getIndexCount();
                int N = a.IndexCount;
                for (int i = 0; i < N; i++)
                {
                    int attr = a.getIndex(i);

                    switch (mapToConstant.get(attr))
                    {
                        case ROTATION:
                            rotation = a.getFloat(attr, rotation);
                            break;
                        case ROTATION_X:
                            rotationX = a.getFloat(attr, rotationX);
                            break;
                        case ROTATION_Y:
                            rotationY = a.getFloat(attr, rotationY);
                            break;
                        case SCALE_X:
                            scaleX = a.getFloat(attr, scaleX);
                            break;
                        case SCALE_Y:
                            scaleY = a.getFloat(attr, scaleY);
                            break;
                        case TRANSFORM_PIVOT_X:
                            transformPivotX = a.getDimension(attr, transformPivotX);
                            break;
                        case TRANSFORM_PIVOT_Y:
                            transformPivotY = a.getDimension(attr, transformPivotY);
                            break;
                        case TRANSFORM_PIVOT_TARGET:
                            transformPivotTarget = lookupID(a, attr, transformPivotTarget);
                            break;
                        case TRANSLATION_X:
                            translationX = a.getDimension(attr, translationX);
                            break;
                        case TRANSLATION_Y:
                            translationY = a.getDimension(attr, translationY);
                            break;
                        case TRANSLATION_Z:
                            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
                            {
                                translationZ = a.getDimension(attr, translationZ);
                            }
                            break;
                        case ELEVATION:
                            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
                            {
                                applyElevation = true;
                                elevation = a.getDimension(attr, elevation);
                            }
                            break;
                    }
                }
                a.recycle();
            }*/
        }
    }
}
