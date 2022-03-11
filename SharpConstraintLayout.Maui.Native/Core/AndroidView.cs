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
    internal class AndroidView
    {

        /**
         * This view is visible.
         * Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int VISIBLE = 0x00000000;

        /**
         * This view is invisible, but it still takes up space for layout purposes.
         * Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int INVISIBLE = 0x00000004;

        /**
         * This view is invisible, and it doesn't take any space for layout
         * purposes. Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int GONE = 0x00000008;



        /**
         * A flag to indicate that the layout direction of this view has not been defined yet.
         * @hide
         */
        public const int LAYOUT_DIRECTION_UNDEFINED = -1;//?LayoutDirection.UNDEFINED;

        /**
         * Horizontal layout direction of this view is from Left to Right.
         * Use with {@link #setLayoutDirection}.
         */
        public const int LAYOUT_DIRECTION_LTR = 0;//LayoutDirection.LTR;

        /**
         * Horizontal layout direction of this view is from Right to Left.
         * Use with {@link #setLayoutDirection}.
         */
        public const int LAYOUT_DIRECTION_RTL = 1;// LayoutDirection.RTL;

        /**
         * Horizontal layout direction of this view is inherited from its parent.
         * Use with {@link #setLayoutDirection}.
         */
        public const int LAYOUT_DIRECTION_INHERIT = 2;// LayoutDirection.INHERIT;

        /**
         * Horizontal layout direction of this view is from deduced from the default language
         * script for the locale. Use with {@link #setLayoutDirection}.
         */
        public const int LAYOUT_DIRECTION_LOCALE = 3;// LayoutDirection.LOCALE;

        private static bool sUseBrokenMakeMeasureSpec = false;
        public static class MeasureSpec
        {
            private const int MODE_SHIFT = 30;
            private const int MODE_MASK = 0x3 << MODE_SHIFT;

            /** @hide */
            /*@IntDef({ UNSPECIFIED, EXACTLY, AT_MOST})
            @Retention(RetentionPolicy.SOURCE)
            public @interface MeasureSpecMode { }*/

            /**
             * Measure specification mode: The parent has not imposed any constraint
             * on the child. It can be whatever size it wants.
             */
            public const int UNSPECIFIED = 0 << MODE_SHIFT;

            /**
             * Measure specification mode: The parent has determined an exact size
             * for the child. The child is going to be given those bounds regardless
             * of how big it wants to be.
             */
            public const int EXACTLY = 1 << MODE_SHIFT;

            /**
             * Measure specification mode: The child can be as large as it wants up
             * to the specified size.
             */
            public const int AT_MOST = 2 << MODE_SHIFT;

            /**
        * Creates a measure specification based on the supplied size and mode.
        *
        * The mode must always be one of the following:
        * <ul>
        *  <li>{@link android.view.View.MeasureSpec#UNSPECIFIED}</li>
        *  <li>{@link android.view.View.MeasureSpec#EXACTLY}</li>
        *  <li>{@link android.view.View.MeasureSpec#AT_MOST}</li>
        * </ul>
        *
        * <p><strong>Note:</strong> On API level 17 and lower, makeMeasureSpec's
        * implementation was such that the order of arguments did not matter
        * and overflow in either value could impact the resulting MeasureSpec.
        * {@link android.widget.RelativeLayout} was affected by this bug.
        * Apps targeting API levels greater than 17 will get the fixed, more strict
        * behavior.</p>
        *
        * @param size the size of the measure specification
        * @param mode the mode of the measure specification
        * @return the measure specification based on size and mode
        */
            //public static int makeMeasureSpec(@IntRange(from = 0, to = (1 << MeasureSpec.MODE_SHIFT) - 1) int size,@MeasureSpecMode int mode) 
            public static int makeMeasureSpec(int size, int mode)
            {
                if (sUseBrokenMakeMeasureSpec)
                {
                    return size + mode;
                }
                else
                {
                    return (size & ~MODE_MASK) | (mode & MODE_MASK);
                }
            }
        }
    }
}
