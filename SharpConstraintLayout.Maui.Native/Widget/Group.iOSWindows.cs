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

namespace SharpConstraintLayout.Maui.Widget
{

    /// <summary>
    /// Control the visibility and elevation of the referenced views
    /// 
    /// <b>Added in 1.1</b>
    /// <para>
    ///     This class controls the visibility of a set of referenced widgets.
    ///     Widgets are referenced by being added to a comma separated list of ids, e.g:
    ///     <pre>
    ///     {@code
    ///          <androidx.constraintlayout.widget.Group
    ///              android:id="@+id/group"
    ///              android:layout_width="wrap_content"
    ///              android:layout_height="wrap_content"
    ///              android:visibility="visible"
    ///              app:constraint_referenced_ids="button4,button9" />
    ///     }
    ///     </pre>
    /// </para>
    ///     <para>
    ///         The visibility of the group will be applied to the referenced widgets.
    ///         It's a convenient way to easily hide/show a set of widgets without having to maintain this set
    ///         programmatically.
    /// </para>
    ///     <para>
    ///     <h2>Multiple groups</h2>
    /// </para>
    ///     <para>
    ///         Multiple groups can reference the same widgets -- in that case, the XML declaration order will
    ///         define the final visibility state (the group declared last will have the last word).
    /// </para>
    /// </summary>
    public class Group : ConstraintHelper
    {

        public Group() : base()
        {
        }

        protected internal override void init()
        {
            base.init();
            mUseViewMeasure = false;
        }

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
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

        public override void updatePostLayout(ConstraintLayout container)
        {
            ConstraintWidget widget = container.idsToConstraintWidgets[this.GetId()];
            widget.Width = 0;
            widget.Height = 0;
        }
    }
}