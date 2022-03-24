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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintSet : IDisposable
    {
        /// <summary>
        /// 只对ConstraintLayout中的有效,用户设置中使用的ConstraintSet不使用该Tag
        /// </summary>
        public bool IsChanged = false;

        private const string TAG = "ConstraintSet";
        private const string ERROR_MESSAGE = "XML parser error must be within a Constraint ";

        private const int INTERNAL_MATCH_PARENT = -1;
        private const int INTERNAL_WRAP_CONTENT = -2;
        private const int INTERNAL_MATCH_CONSTRAINT = -3;
        private const int INTERNAL_WRAP_CONTENT_CONSTRAINED = -4;

        private bool mValidate;
        public string mIdString;
        public string derivedState = "";
        public const int ROTATE_NONE = 0;
        public const int ROTATE_PORTRATE_OF_RIGHT = 1;
        public const int ROTATE_PORTRATE_OF_LEFT = 2;
        public const int ROTATE_RIGHT_OF_PORTRATE = 3;
        public const int ROTATE_LEFT_OF_PORTRATE = 4;
        public int mRotate = 0;
        //private Dictionary<string, ConstraintAttribute> mSavedAttributes = new Dictionary<string, ConstraintAttribute>();

        /// <summary>
        /// require that all views have IDs to function
        /// </summary>
        private bool mForceId = true;
        /// <summary>
        /// Used to indicate a parameter is cleared or not set
        /// </summary>
        public const int Unset = -1;

        /// <summary>
        /// Dimension will be controlled by constraints
        /// </summary>
        public const int MatchConstraint = 0;

        /// <summary>
        /// Special value for the height or width requested by a View.
        /// MATCH_PARENT means that the view wants to be as big as its parent,
        /// minus the parent's padding, if any. Introduced in API Level 8.
        /// </summary>
        public const int MatchParent = -1;

        /// <summary>
        /// Special value for the height or width requested by a View.
        /// WRAP_CONTENT means that the view wants to be just large enough to fit
        /// its own internal content, taking its own padding into account.
        /// </summary>
        public const int WrapContent = -2;

        /// <summary>
        /// How to calculate the size of a view in 0 dp by using its wrap_content size
        /// </summary>
        public const int MatchConstraintWrap = ConstraintWidget.MATCH_CONSTRAINT_WRAP;

        /// <summary>
        /// Calculate the size of a view in 0 dp by reducing the constrains gaps as much as possible
        /// </summary>
        public const int MatchConstraintSpread = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;

        public const int MatchConstraintPercent = ConstraintWidget.MATCH_CONSTRAINT_PERCENT;

        /// <summary>
        /// References the id of the parent.
        /// Used in:
        /// <ul>
        /// <li><seealso cref="ConstraintSet.Connect(int, int, int, int, int)"/></li>
        /// <li><seealso cref="ConstraintSet.Center(int, int, int, int, int, int, int, float)"/></li>
        /// </ul>
        /// </summary>
        public const int ParentId = 0;

        /// <summary>
        /// The horizontal orientation.
        /// </summary>
        public const int Horizontal = ConstraintWidget.HORIZONTAL;

        /// <summary>
        /// The vertical orientation.
        /// </summary>
        public const int Vertical = ConstraintWidget.VERTICAL;

        /// <summary>
        /// Used to create a horizontal create guidelines.
        /// </summary>
        public const int HorizontalGuideline = 0;

        /// <summary>
        /// Used to create a vertical create guidelines.
        /// see <seealso cref="Create(int, int)"/>
        /// </summary>
        public const int VerticalGuideline = 1;

        /// <summary>
        /// This view is visible.
        /// Use with <see cref="SetVisibility"/> and <a href="#attr_android:visibility">{@code
        /// android:visibility}.
        /// </summary>
        public const int Visible = ConstraintWidget.VISIBLE;

        /// <summary>
        /// This view is invisible, but it still takes up space for layout purposes.
        /// Use with <seealso cref="SetVisibility"/> and <a href="#attr_android:visibility">{@code
        /// android:visibility}.
        /// </summary>
        public const int Invisible = ConstraintWidget.INVISIBLE;

        /// <summary>
        /// This view is gone, and will not take any space for layout
        /// purposes. Use with <seealso cref="#setVisibility"/> and <a href="#attr_android:visibility">{@code
        /// android:visibility}.
        /// </summary>
        public const int Gone = ConstraintWidget.GONE;

        /// <summary>
        /// The left side of a view.
        /// </summary>
        public const int Left = 1;

        /// <summary>
        /// The right side of a view.
        /// </summary>
        public const int Right = 2;

        /// <summary>
        /// The top of a view.
        /// </summary>
        public const int Top = 3;

        /// <summary>
        /// The bottom side of a view.
        /// </summary>
        public const int Bottom = 4;

        /// <summary>
        /// The baseline of the text in a view.
        /// </summary>
        public const int Baseline = 5;

        /// <summary>
        /// The left side of a view in left to right languages.
        /// In right to left languages it corresponds to the right side of the view
        /// </summary>
        public const int Start = 6;

        /// <summary>
        /// The right side of a view in right to left languages.
        /// In right to left languages it corresponds to the left side of the view
        /// </summary>
        public const int End = 7;

        /// <summary>
        /// Circle reference from a view.
        /// </summary>
        /// public static final int CIRCLE = 8;
        public const int CircleReference = 8;

        /// <summary>
        /// Chain spread style
        /// </summary>
        public const int ChainSpread = ConstraintWidget.CHAIN_SPREAD;

        /// <summary>
        /// Chain spread inside style
        /// </summary>
        public const int ChainSpreadInside = ConstraintWidget.CHAIN_SPREAD_INSIDE;

        public const int VisibilityModeNormal = 0;
        public const int VisibilityModeIgnore = 1;
        /// <summary>
        /// Chain packed style
        /// </summary>
        public const int ChainPacked = ConstraintWidget.CHAIN_PACKED;

        private const bool DEBUG = false;
        //private static readonly int[] VISIBILITY_FLAGS = new int[] { Visible, Invisible, Gone };
        private const int BARRIER_TYPE = 1;

        private Dictionary<int, Constraint> mConstraints = new Dictionary<int, Constraint>();

        //private static Dictionary<int, int> mapToConstant = new Dictionary<int, int>();
        //private static Dictionary<int, int> overrideMapToConstant = new Dictionary<int, int>();
        private const int BASELINE_TO_BASELINE = 1;
        private const int BOTTOM_MARGIN = 2;
        private const int BOTTOM_TO_BOTTOM = 3;
        private const int BOTTOM_TO_TOP = 4;
        private const int DIMENSION_RATIO = 5;
        private const int EDITOR_ABSOLUTE_X = 6;
        private const int EDITOR_ABSOLUTE_Y = 7;
        private const int END_MARGIN = 8;
        private const int END_TO_END = 9;
        private const int END_TO_START = 10;
        private const int GONE_BOTTOM_MARGIN = 11;
        private const int GONE_END_MARGIN = 12;
        private const int GONE_LEFT_MARGIN = 13;
        private const int GONE_RIGHT_MARGIN = 14;
        private const int GONE_START_MARGIN = 15;
        private const int GONE_TOP_MARGIN = 16;
        private const int GUIDE_BEGIN = 17;
        private const int GUIDE_END = 18;
        private const int GUIDE_PERCENT = 19;
        private const int HORIZONTAL_BIAS = 20;
        private const int LAYOUT_HEIGHT = 21;
        private const int LAYOUT_VISIBILITY = 22;
        private const int LAYOUT_WIDTH = 23;
        private const int LEFT_MARGIN = 24;
        private const int LEFT_TO_LEFT = 25;
        private const int LEFT_TO_RIGHT = 26;
        private const int ORIENTATION = 27;
        private const int RIGHT_MARGIN = 28;
        private const int RIGHT_TO_LEFT = 29;
        private const int RIGHT_TO_RIGHT = 30;
        private const int START_MARGIN = 31;
        private const int START_TO_END = 32;
        private const int START_TO_START = 33;
        private const int TOP_MARGIN = 34;
        private const int TOP_TO_BOTTOM = 35;
        private const int TOP_TO_TOP = 36;
        private const int VERTICAL_BIAS = 37;
        private const int VIEW_ID = 38;
        private const int HORIZONTAL_WEIGHT = 39;
        private const int VERTICAL_WEIGHT = 40;
        private const int HORIZONTAL_STYLE = 41;
        private const int VERTICAL_STYLE = 42;
        private const int ALPHA = 43;
        private const int ELEVATION = 44;
        private const int ROTATION_X = 45;
        private const int ROTATION_Y = 46;
        private const int SCALE_X = 47;
        private const int SCALE_Y = 48;
        private const int TRANSFORM_PIVOT_X = 49;
        private const int TRANSFORM_PIVOT_Y = 50;
        private const int TRANSLATION_X = 51;
        private const int TRANSLATION_Y = 52;
        private const int TRANSLATION_Z = 53;
        private const int WIDTH_DEFAULT = 54;
        private const int HEIGHT_DEFAULT = 55;
        private const int WIDTH_MAX = 56;
        private const int HEIGHT_MAX = 57;
        private const int WIDTH_MIN = 58;
        private const int HEIGHT_MIN = 59;
        private const int ROTATION = 60;
        private const int CIRCLE = 61;
        private const int CIRCLE_RADIUS = 62;
        private const int CIRCLE_ANGLE = 63;
        private const int ANIMATE_RELATIVE_TO = 64;
        private const int TRANSITION_EASING = 65;
        private const int DRAW_PATH = 66;
        private const int TRANSITION_PATH_ROTATE = 67;
        private const int PROGRESS = 68;
        private const int WIDTH_PERCENT = 69;
        private const int HEIGHT_PERCENT = 70;
        private const int CHAIN_USE_RTL = 71;
        private const int BARRIER_DIRECTION = 72;
        private const int BARRIER_MARGIN = 73;
        private const int CONSTRAINT_REFERENCED_IDS = 74;
        private const int BARRIER_ALLOWS_GONE_WIDGETS = 75;
        private const int PATH_MOTION_ARC = 76;
        private const int CONSTRAINT_TAG = 77;
        private const int VISIBILITY_MODE = 78;
        private const int MOTION_STAGGER = 79;
        private const int CONSTRAINED_WIDTH = 80;
        private const int CONSTRAINED_HEIGHT = 81;
        private const int ANIMATE_CIRCLE_ANGLE_TO = 82;
        private const int TRANSFORM_PIVOT_TARGET = 83;
        private const int QUANTIZE_MOTION_STEPS = 84;
        private const int QUANTIZE_MOTION_PHASE = 85;
        private const int QUANTIZE_MOTION_INTERPOLATOR = 86;
        private const int UNUSED = 87;
        private const int QUANTIZE_MOTION_INTERPOLATOR_TYPE = 88;
        private const int QUANTIZE_MOTION_INTERPOLATOR_ID = 89;
        private const int QUANTIZE_MOTION_INTERPOLATOR_STR = 90;
        private const int BASELINE_TO_TOP = 91;
        private const int BASELINE_TO_BOTTOM = 92;
        private const int BASELINE_MARGIN = 93;
        private const int GONE_BASELINE_MARGIN = 94;
        private const int LAYOUT_CONSTRAINT_WIDTH = 95;
        private const int LAYOUT_CONSTRAINT_HEIGHT = 96;
        private const int LAYOUT_WRAP_BEHAVIOR = 97;
        private const int MOTION_TARGET = 98;

        private const string KEY_WEIGHT = "weight";
        private const string KEY_RATIO = "ratio";
        private const string KEY_PERCENT_PARENT = "parent";

        public void Dispose()
        {
            mConstraints.Clear();
        }

        /// <summary>
        /// 定义了全部约束,测量时从其中取出分析
        /// </summary>
        public Dictionary<int, Constraint> Constraints { get { return mConstraints; } }
    }
}
