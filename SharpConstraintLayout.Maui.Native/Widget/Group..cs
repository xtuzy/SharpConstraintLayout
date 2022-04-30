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
using androidx.constraintlayout.core.widgets;
using SharpConstraintLayout.Maui.Widget.Interface;

namespace SharpConstraintLayout.Maui.Widget
{

    /// <summary>
    /// Control the visibility and elevation of the referenced views.<b>Added in 1.1</b>
    /// <br/>
    /// This class controls the visibility of a set of referenced widgets.
    /// Widgets are referenced by being added to a comma separated list of ids, e.g:<code>group.Visiable = ConstraintSet.Gone</code>
    /// <br/>
    /// The visibility of the group will be applied to the referenced widgets.
    /// It's a convenient way to easily hide/show a set of widgets without having to maintain this set
    /// programmatically.
    /// <br/>
    /// <h2>Multiple groups</h2>
    /// <br/>
    /// Multiple groups can reference the same widgets -- in that case, the XML declaration order will
    /// define the final visibility state (the group declared last will have the last word).
    /// <br/>
    /// 
    /// <see href="https://developer.android.com/reference/androidx/constraintlayout/widget/Group">androidx.constraintlayout.widget.Group</see>
    /// </summary>
    public class Group : ConstraintHelper, IGroup
    {
#if __ANDROID__  && !__MAUI__
        public Group(Android.Content.Context context) : base(context)
#else
        public Group() : base()
#endif
        {
        }

        protected override void init()
        {
            base.init();
            mUseViewMeasure = false;
        }

        protected override void WhenAttachedToWindow()
        {
            base.WhenAttachedToWindow();
            applyLayoutFeatures();
        }

        public override int Visible
        {
            set
            {
                base.Visible = value;
                applyLayoutFeatures();
            }
        }

        public override float Elevation
        {
            set
            {
                base.Elevation = value;
                applyLayoutFeatures();
            }
        }

        protected internal override void applyLayoutFeaturesInConstraintSet(ConstraintLayout container)
        {
            applyLayoutFeatures(container);
        }

        public override void UpdatePostLayout(ConstraintLayout container)
        {
            ConstraintWidget widget = container.GetWidgetByElement(this);
            widget.Width = 0;
            widget.Height = 0;
        }
    }
}