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

namespace SharpConstraintLayout.Maui.Core
{
    public partial class ConstraintSet
    {
        public class PropertySet
        {
            #region Add Form LayoutPamas
            public bool isInPlaceholder = false;
            #endregion
            public bool mApply = false;
            public int visibility = AndroidView.VISIBLE;
            public int mVisibilityMode = VISIBILITY_MODE_NORMAL;
            public float alpha = 1;
            public float mProgress = float.NaN;

            public virtual void copyFrom(PropertySet src)
            {
                mApply = src.mApply;
                visibility = src.visibility;
                alpha = src.alpha;
                mProgress = src.mProgress;
                mVisibilityMode = src.mVisibilityMode;
            }

            /*internal virtual void fillFromAttributeList(Context context, AttributeSet attrs)
            {
                TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.PropertySet);
                mApply = true;
                //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int N = a.getIndexCount();
                int N = a.IndexCount;
                for (int i = 0; i < N; i++)
                {
                    int attr = a.getIndex(i);

                    if (attr == R.styleable.PropertySet_android_alpha)
                    {
                        alpha = a.getFloat(attr, alpha);
                    }
                    else if (attr == R.styleable.PropertySet_android_visibility)
                    {
                        visibility = a.getInt(attr, visibility);
                        visibility = VISIBILITY_FLAGS[visibility];
                    }
                    else if (attr == R.styleable.PropertySet_visibilityMode)
                    {
                        mVisibilityMode = a.getInt(attr, mVisibilityMode);
                    }
                    else if (attr == R.styleable.PropertySet_motionProgress)
                    {
                        mProgress = a.getFloat(attr, mProgress);
                    }
                }
                a.recycle();
            }*/
        }
    }
}
