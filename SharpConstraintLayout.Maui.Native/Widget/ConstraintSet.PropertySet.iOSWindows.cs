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
        public class PropertySet:IDisposable
        {
            public bool mApply = false;

            /// <summary>
            /// Copy Form Android.Views.View.Visibility Returns the visibility status for this view.
            /// <see cref="ConstraintSet.Visible"/>, <see cref="ConstraintSet.Invisible"/>,<see cref="ConstraintSet.Gone"/>.
            /// </summary>
            public int visibility = Visible;

            public int mVisibilityMode = VisibilityModeNormal;

            /// <summary>
            /// Copy Form Android.Views.View.Alpha The opacity of the view.This is a value from 0 to
            /// 1, where 0 means the view is completely transparent and 1 means the view is
            /// completely opaque. By default this is 1.0f.
            /// </summary>
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

            public void Dispose()
            {
               
            }
        }
    }
}