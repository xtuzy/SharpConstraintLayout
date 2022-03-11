using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

/*
 * Copyright (C) 2016 The Android Open Source Project
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

namespace SharpConstraintLayout.Maui.Platforms.Androids
{
	using SuppressLint = Android.Annotation.SuppressLint;
	using Context = Android.Content.Context;
	using Resources = Android.Content.Res.Resources;
	using TypedArray = Android.Content.Res.TypedArray;
	using XmlResourceParser = Android.Content.Res.XmlResourceParser;
	using Color = Android.Graphics.Color;
	using Build = Android.OS.Build;
	using VERSION_CODES = Android.OS.Build.VERSION_CODES;
	using AttributeSet = Android.Util.IAttributeSet;
	using Log = Android.Util.Log;
	using SparseArray = Android.Util.SparseArray;
	using SparseIntArray = Android.Util.SparseIntArray;
	using TypedValue = Android.Util.TypedValue;
	using Xml = Android.Util.Xml;
	using LayoutInflater = Android.Views.LayoutInflater;
	using View = Android.Views.View;

	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using HelperWidget = androidx.constraintlayout.core.widgets.HelperWidget;
	using Easing = androidx.constraintlayout.core.motion.utils.Easing;
	using Debug = androidx.constraintlayout.motion.widget.Debug;
	using MotionLayout = androidx.constraintlayout.motion.widget.MotionLayout;
	using MotionScene = androidx.constraintlayout.motion.widget.MotionScene;
	using AttributeType = androidx.constraintlayout.widget.ConstraintAttribute.AttributeType;
	using LayoutParams = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams;

	using XmlPullParser = org.xmlpull.v1.XmlPullParser;
	using XmlPullParserException = org.xmlpull.v1.XmlPullParserException;


	/// <summary>
	/// This class allows you to define programmatically a set of constraints to be used with <seealso cref="ConstraintLayout"/>.
	/// <para>
	/// For details about Constraint behaviour see <seealso cref="ConstraintLayout"/>.
	/// It lets you create and save constraints, and apply them to an existing ConstraintLayout. ConstraintsSet can be created in various ways:
	/// <ul>
	/// <li>
	/// Manually <br> {@code c = new ConstraintSet(); c.connect(....);}
	/// </li>
	/// <li>
	/// from a R.layout.* object <br> {@code c.clone(context, R.layout.layout1);}
	/// </li>
	/// <li>
	/// from a ConstraintLayout <br> {@code c.clone(constraintLayout);}
	/// </li>
	/// </para>
	/// </ul><para>
	/// Example code:<br>
	/// {@sample resources/examples/ExampleConstraintSet.java Example}
	/// </para>
	/// </summary>
	public class ConstraintSet
	{
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
		private Dictionary<string, ConstraintAttribute> mSavedAttributes = new Dictionary<string, ConstraintAttribute>();

		/// <summary>
		/// require that all views have IDs to function
		/// </summary>
		private bool mForceId = true;
		/// <summary>
		/// Used to indicate a parameter is cleared or not set
		/// </summary>
		public const int UNSET = LayoutParams.UNSET;

		/// <summary>
		/// Dimension will be controlled by constraints
		/// </summary>
		public const int MATCH_CONSTRAINT = ConstraintLayout.LayoutParams.MATCH_CONSTRAINT;

		/// <summary>
		/// Dimension will set by the view's content
		/// </summary>
		public static readonly int WRAP_CONTENT = ConstraintLayout.LayoutParams.WRAP_CONTENT;

		/// <summary>
		/// How to calculate the size of a view in 0 dp by using its wrap_content size
		/// </summary>
		public static readonly int MATCH_CONSTRAINT_WRAP = ConstraintLayout.LayoutParams.MATCH_CONSTRAINT_WRAP;

		/// <summary>
		/// Calculate the size of a view in 0 dp by reducing the constrains gaps as much as possible
		/// </summary>
		public static readonly int MATCH_CONSTRAINT_SPREAD = ConstraintLayout.LayoutParams.MATCH_CONSTRAINT_SPREAD;

		public static readonly int MATCH_CONSTRAINT_PERCENT = ConstraintLayout.LayoutParams.MATCH_CONSTRAINT_PERCENT;

		/// <summary>
		/// References the id of the parent.
		/// Used in:
		/// <ul>
		/// <li><seealso cref="#connect(int, int, int, int, int)"/></li>
		/// <li><seealso cref="#center(int, int, int, int, int, int, int, float)"/></li>
		/// </ul>
		/// </summary>
		public const int PARENT_ID = ConstraintLayout.LayoutParams.PARENT_ID;

		/// <summary>
		/// The horizontal orientation.
		/// </summary>
		public static readonly int HORIZONTAL = ConstraintLayout.LayoutParams.HORIZONTAL;

		/// <summary>
		/// The vertical orientation.
		/// </summary>
		public static readonly int VERTICAL = ConstraintLayout.LayoutParams.VERTICAL;

		/// <summary>
		/// Used to create a horizontal create guidelines.
		/// </summary>
		public const int HORIZONTAL_GUIDELINE = 0;

		/// <summary>
		/// Used to create a vertical create guidelines.
		/// see <seealso cref="#create(int, int)"/>
		/// </summary>
		public const int VERTICAL_GUIDELINE = 1;

		/// <summary>
		/// This view is visible.
		/// Use with <seealso cref="#setVisibility"/> and <a href="#attr_android:visibility">{@code
		/// android:visibility}.
		/// </summary>
		public static readonly int VISIBLE = View.VISIBLE;

		/// <summary>
		/// This view is invisible, but it still takes up space for layout purposes.
		/// Use with <seealso cref="#setVisibility"/> and <a href="#attr_android:visibility">{@code
		/// android:visibility}.
		/// </summary>
		public static readonly int INVISIBLE = View.INVISIBLE;

		/// <summary>
		/// This view is gone, and will not take any space for layout
		/// purposes. Use with <seealso cref="#setVisibility"/> and <a href="#attr_android:visibility">{@code
		/// android:visibility}.
		/// </summary>
		public static readonly int GONE = View.GONE;

		/// <summary>
		/// The left side of a view.
		/// </summary>
		public const int LEFT = ConstraintLayout.LayoutParams.LEFT;

		/// <summary>
		/// The right side of a view.
		/// </summary>
		public const int RIGHT = ConstraintLayout.LayoutParams.RIGHT;

		/// <summary>
		/// The top of a view.
		/// </summary>
		public const int TOP = ConstraintLayout.LayoutParams.TOP;

		/// <summary>
		/// The bottom side of a view.
		/// </summary>
		public const int BOTTOM = ConstraintLayout.LayoutParams.BOTTOM;

		/// <summary>
		/// The baseline of the text in a view.
		/// </summary>
		public const int BASELINE = ConstraintLayout.LayoutParams.BASELINE;

		/// <summary>
		/// The left side of a view in left to right languages.
		/// In right to left languages it corresponds to the right side of the view
		/// </summary>
		public const int START = ConstraintLayout.LayoutParams.START;

		/// <summary>
		/// The right side of a view in right to left languages.
		/// In right to left languages it corresponds to the left side of the view
		/// </summary>
		public const int END = ConstraintLayout.LayoutParams.END;

		/// <summary>
		/// Circle reference from a view.
		/// </summary>
		public const int CIRCLE_REFERENCE = ConstraintLayout.LayoutParams.CIRCLE;

		/// <summary>
		/// Chain spread style
		/// </summary>
		public static readonly int CHAIN_SPREAD = ConstraintLayout.LayoutParams.CHAIN_SPREAD;

		/// <summary>
		/// Chain spread inside style
		/// </summary>
		public static readonly int CHAIN_SPREAD_INSIDE = ConstraintLayout.LayoutParams.CHAIN_SPREAD_INSIDE;

		public const int VISIBILITY_MODE_NORMAL = 0;
		public const int VISIBILITY_MODE_IGNORE = 1;
		/// <summary>
		/// Chain packed style
		/// </summary>
		public static readonly int CHAIN_PACKED = ConstraintLayout.LayoutParams.CHAIN_PACKED;

		private const bool DEBUG = false;
		private static readonly int[] VISIBILITY_FLAGS = new int[]{VISIBLE, INVISIBLE, GONE};
		private const int BARRIER_TYPE = 1;

		private Dictionary<int?, Constraint> mConstraints = new Dictionary<int?, Constraint>();

		private static SparseIntArray mapToConstant = new SparseIntArray();
		private static SparseIntArray overrideMapToConstant = new SparseIntArray();
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

		static ConstraintSet()
		{
			mapToConstant.append(R.styleable.Constraint_layout_constraintLeft_toLeftOf, LEFT_TO_LEFT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintLeft_toRightOf, LEFT_TO_RIGHT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintRight_toLeftOf, RIGHT_TO_LEFT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintRight_toRightOf, RIGHT_TO_RIGHT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintTop_toTopOf, TOP_TO_TOP);
			mapToConstant.append(R.styleable.Constraint_layout_constraintTop_toBottomOf, TOP_TO_BOTTOM);
			mapToConstant.append(R.styleable.Constraint_layout_constraintBottom_toTopOf, BOTTOM_TO_TOP);
			mapToConstant.append(R.styleable.Constraint_layout_constraintBottom_toBottomOf, BOTTOM_TO_BOTTOM);
			mapToConstant.append(R.styleable.Constraint_layout_constraintBaseline_toBaselineOf, BASELINE_TO_BASELINE);
			mapToConstant.append(R.styleable.Constraint_layout_constraintBaseline_toTopOf, BASELINE_TO_TOP);
			mapToConstant.append(R.styleable.Constraint_layout_constraintBaseline_toBottomOf, BASELINE_TO_BOTTOM);

			mapToConstant.append(R.styleable.Constraint_layout_editor_absoluteX, EDITOR_ABSOLUTE_X);
			mapToConstant.append(R.styleable.Constraint_layout_editor_absoluteY, EDITOR_ABSOLUTE_Y);
			mapToConstant.append(R.styleable.Constraint_layout_constraintGuide_begin, GUIDE_BEGIN);
			mapToConstant.append(R.styleable.Constraint_layout_constraintGuide_end, GUIDE_END);
			mapToConstant.append(R.styleable.Constraint_layout_constraintGuide_percent, GUIDE_PERCENT);
			mapToConstant.append(R.styleable.Constraint_android_orientation, ORIENTATION);
			mapToConstant.append(R.styleable.Constraint_layout_constraintStart_toEndOf, START_TO_END);
			mapToConstant.append(R.styleable.Constraint_layout_constraintStart_toStartOf, START_TO_START);
			mapToConstant.append(R.styleable.Constraint_layout_constraintEnd_toStartOf, END_TO_START);
			mapToConstant.append(R.styleable.Constraint_layout_constraintEnd_toEndOf, END_TO_END);
			mapToConstant.append(R.styleable.Constraint_layout_goneMarginLeft, GONE_LEFT_MARGIN);
			mapToConstant.append(R.styleable.Constraint_layout_goneMarginTop, GONE_TOP_MARGIN);
			mapToConstant.append(R.styleable.Constraint_layout_goneMarginRight, GONE_RIGHT_MARGIN);
			mapToConstant.append(R.styleable.Constraint_layout_goneMarginBottom, GONE_BOTTOM_MARGIN);
			mapToConstant.append(R.styleable.Constraint_layout_goneMarginStart, GONE_START_MARGIN);
			mapToConstant.append(R.styleable.Constraint_layout_goneMarginEnd, GONE_END_MARGIN);
			mapToConstant.append(R.styleable.Constraint_layout_constraintVertical_weight, VERTICAL_WEIGHT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintHorizontal_weight, HORIZONTAL_WEIGHT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintHorizontal_chainStyle, HORIZONTAL_STYLE);
			mapToConstant.append(R.styleable.Constraint_layout_constraintVertical_chainStyle, VERTICAL_STYLE);

			mapToConstant.append(R.styleable.Constraint_layout_constraintHorizontal_bias, HORIZONTAL_BIAS);
			mapToConstant.append(R.styleable.Constraint_layout_constraintVertical_bias, VERTICAL_BIAS);
			mapToConstant.append(R.styleable.Constraint_layout_constraintDimensionRatio, DIMENSION_RATIO);
			mapToConstant.append(R.styleable.Constraint_layout_constraintLeft_creator, UNUSED);
			mapToConstant.append(R.styleable.Constraint_layout_constraintTop_creator, UNUSED);
			mapToConstant.append(R.styleable.Constraint_layout_constraintRight_creator, UNUSED);
			mapToConstant.append(R.styleable.Constraint_layout_constraintBottom_creator, UNUSED);
			mapToConstant.append(R.styleable.Constraint_layout_constraintBaseline_creator, UNUSED);
			mapToConstant.append(R.styleable.Constraint_android_layout_marginLeft, LEFT_MARGIN);
			mapToConstant.append(R.styleable.Constraint_android_layout_marginRight, RIGHT_MARGIN);
			mapToConstant.append(R.styleable.Constraint_android_layout_marginStart, START_MARGIN);
			mapToConstant.append(R.styleable.Constraint_android_layout_marginEnd, END_MARGIN);
			mapToConstant.append(R.styleable.Constraint_android_layout_marginTop, TOP_MARGIN);
			mapToConstant.append(R.styleable.Constraint_android_layout_marginBottom, BOTTOM_MARGIN);
			mapToConstant.append(R.styleable.Constraint_android_layout_width, LAYOUT_WIDTH);
			mapToConstant.append(R.styleable.Constraint_android_layout_height, LAYOUT_HEIGHT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintWidth, LAYOUT_CONSTRAINT_WIDTH);
			mapToConstant.append(R.styleable.Constraint_layout_constraintHeight, LAYOUT_CONSTRAINT_HEIGHT);
			mapToConstant.append(R.styleable.Constraint_android_visibility, LAYOUT_VISIBILITY);
			mapToConstant.append(R.styleable.Constraint_android_alpha, ALPHA);
			mapToConstant.append(R.styleable.Constraint_android_elevation, ELEVATION);
			mapToConstant.append(R.styleable.Constraint_android_rotationX, ROTATION_X);
			mapToConstant.append(R.styleable.Constraint_android_rotationY, ROTATION_Y);
			mapToConstant.append(R.styleable.Constraint_android_rotation, ROTATION);
			mapToConstant.append(R.styleable.Constraint_android_scaleX, SCALE_X);
			mapToConstant.append(R.styleable.Constraint_android_scaleY, SCALE_Y);
			mapToConstant.append(R.styleable.Constraint_android_transformPivotX, TRANSFORM_PIVOT_X);
			mapToConstant.append(R.styleable.Constraint_android_transformPivotY, TRANSFORM_PIVOT_Y);
			mapToConstant.append(R.styleable.Constraint_android_translationX, TRANSLATION_X);
			mapToConstant.append(R.styleable.Constraint_android_translationY, TRANSLATION_Y);
			mapToConstant.append(R.styleable.Constraint_android_translationZ, TRANSLATION_Z);
			mapToConstant.append(R.styleable.Constraint_layout_constraintWidth_default, WIDTH_DEFAULT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintHeight_default, HEIGHT_DEFAULT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintWidth_max, WIDTH_MAX);
			mapToConstant.append(R.styleable.Constraint_layout_constraintHeight_max, HEIGHT_MAX);
			mapToConstant.append(R.styleable.Constraint_layout_constraintWidth_min, WIDTH_MIN);
			mapToConstant.append(R.styleable.Constraint_layout_constraintHeight_min, HEIGHT_MIN);
			mapToConstant.append(R.styleable.Constraint_layout_constraintCircle, CIRCLE);
			mapToConstant.append(R.styleable.Constraint_layout_constraintCircleRadius, CIRCLE_RADIUS);
			mapToConstant.append(R.styleable.Constraint_layout_constraintCircleAngle, CIRCLE_ANGLE);
			mapToConstant.append(R.styleable.Constraint_animateRelativeTo, ANIMATE_RELATIVE_TO);
			mapToConstant.append(R.styleable.Constraint_transitionEasing, TRANSITION_EASING);
			mapToConstant.append(R.styleable.Constraint_drawPath, DRAW_PATH);
			mapToConstant.append(R.styleable.Constraint_transitionPathRotate, TRANSITION_PATH_ROTATE);
			mapToConstant.append(R.styleable.Constraint_motionStagger, MOTION_STAGGER);
			mapToConstant.append(R.styleable.Constraint_android_id, VIEW_ID);
			mapToConstant.append(R.styleable.Constraint_motionProgress, PROGRESS);
			mapToConstant.append(R.styleable.Constraint_layout_constraintWidth_percent, WIDTH_PERCENT);
			mapToConstant.append(R.styleable.Constraint_layout_constraintHeight_percent, HEIGHT_PERCENT);
			mapToConstant.append(R.styleable.Constraint_layout_wrapBehaviorInParent, LAYOUT_WRAP_BEHAVIOR);

			mapToConstant.append(R.styleable.Constraint_chainUseRtl, CHAIN_USE_RTL);
			mapToConstant.append(R.styleable.Constraint_barrierDirection, BARRIER_DIRECTION);
			mapToConstant.append(R.styleable.Constraint_barrierMargin, BARRIER_MARGIN);
			mapToConstant.append(R.styleable.Constraint_constraint_referenced_ids, CONSTRAINT_REFERENCED_IDS);
			mapToConstant.append(R.styleable.Constraint_barrierAllowsGoneWidgets, BARRIER_ALLOWS_GONE_WIDGETS);
			mapToConstant.append(R.styleable.Constraint_pathMotionArc, PATH_MOTION_ARC);
			mapToConstant.append(R.styleable.Constraint_layout_constraintTag, CONSTRAINT_TAG);
			mapToConstant.append(R.styleable.Constraint_visibilityMode, VISIBILITY_MODE);
			mapToConstant.append(R.styleable.Constraint_layout_constrainedWidth, CONSTRAINED_WIDTH);
			mapToConstant.append(R.styleable.Constraint_layout_constrainedHeight, CONSTRAINED_HEIGHT);
			mapToConstant.append(R.styleable.Constraint_polarRelativeTo, ANIMATE_CIRCLE_ANGLE_TO);
			mapToConstant.append(R.styleable.Constraint_transformPivotTarget, TRANSFORM_PIVOT_TARGET);
			mapToConstant.append(R.styleable.Constraint_quantizeMotionSteps, QUANTIZE_MOTION_STEPS);
			mapToConstant.append(R.styleable.Constraint_quantizeMotionPhase, QUANTIZE_MOTION_PHASE);
			mapToConstant.append(R.styleable.Constraint_quantizeMotionInterpolator, QUANTIZE_MOTION_INTERPOLATOR);


			/*
			The tags not available in constraintOverride
			Left here to help with documentation and understanding
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintLeft_toLeftOf, LEFT_TO_LEFT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintLeft_toRightOf, LEFT_TO_RIGHT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintRight_toLeftOf, RIGHT_TO_LEFT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintRight_toRightOf, RIGHT_TO_RIGHT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintTop_toTopOf, TOP_TO_TOP);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintTop_toBottomOf, TOP_TO_BOTTOM);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintBottom_toTopOf, BOTTOM_TO_TOP);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintBottom_toBottomOf, BOTTOM_TO_BOTTOM);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintBaseline_toBaselineOf, BASELINE_TO_BASELINE);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintGuide_begin, GUIDE_BEGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintGuide_end, GUIDE_END);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintGuide_percent, GUIDE_PERCENT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintStart_toEndOf, START_TO_END);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintStart_toStartOf, START_TO_START);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintEnd_toStartOf, END_TO_START);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintEnd_toEndOf, END_TO_END);
			*/
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_editor_absoluteY, EDITOR_ABSOLUTE_X);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_editor_absoluteY, EDITOR_ABSOLUTE_Y);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_orientation, ORIENTATION);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_goneMarginLeft, GONE_LEFT_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_goneMarginTop, GONE_TOP_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_goneMarginRight, GONE_RIGHT_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_goneMarginBottom, GONE_BOTTOM_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_goneMarginStart, GONE_START_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_goneMarginEnd, GONE_END_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintVertical_weight, VERTICAL_WEIGHT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintHorizontal_weight, HORIZONTAL_WEIGHT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintHorizontal_chainStyle, HORIZONTAL_STYLE);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintVertical_chainStyle, VERTICAL_STYLE);

			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintHorizontal_bias, HORIZONTAL_BIAS);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintVertical_bias, VERTICAL_BIAS);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintDimensionRatio, DIMENSION_RATIO);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintLeft_creator, UNUSED);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintTop_creator, UNUSED);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintRight_creator, UNUSED);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintBottom_creator, UNUSED);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintBaseline_creator, UNUSED);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_layout_marginLeft, LEFT_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_layout_marginRight, RIGHT_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_layout_marginStart, START_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_layout_marginEnd, END_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_layout_marginTop, TOP_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_layout_marginBottom, BOTTOM_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_layout_width, LAYOUT_WIDTH);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_layout_height, LAYOUT_HEIGHT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintWidth, LAYOUT_CONSTRAINT_WIDTH);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintHeight, LAYOUT_CONSTRAINT_HEIGHT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_visibility, LAYOUT_VISIBILITY);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_alpha, ALPHA);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_elevation, ELEVATION);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_rotationX, ROTATION_X);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_rotationY, ROTATION_Y);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_rotation, ROTATION);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_scaleX, SCALE_X);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_scaleY, SCALE_Y);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_transformPivotX, TRANSFORM_PIVOT_X);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_transformPivotY, TRANSFORM_PIVOT_Y);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_translationX, TRANSLATION_X);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_translationY, TRANSLATION_Y);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_translationZ, TRANSLATION_Z);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintWidth_default, WIDTH_DEFAULT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintHeight_default, HEIGHT_DEFAULT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintWidth_max, WIDTH_MAX);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintHeight_max, HEIGHT_MAX);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintWidth_min, WIDTH_MIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintHeight_min, HEIGHT_MIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintCircleRadius, CIRCLE_RADIUS);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintCircleAngle, CIRCLE_ANGLE);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_animateRelativeTo, ANIMATE_RELATIVE_TO);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_transitionEasing, TRANSITION_EASING);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_drawPath, DRAW_PATH);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_transitionPathRotate, TRANSITION_PATH_ROTATE);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_motionStagger, MOTION_STAGGER);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_android_id, VIEW_ID);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_motionTarget, MOTION_TARGET);

			overrideMapToConstant.append(R.styleable.ConstraintOverride_motionProgress, PROGRESS);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintWidth_percent, WIDTH_PERCENT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintHeight_percent, HEIGHT_PERCENT);

			overrideMapToConstant.append(R.styleable.ConstraintOverride_chainUseRtl, CHAIN_USE_RTL);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_barrierDirection, BARRIER_DIRECTION);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_barrierMargin, BARRIER_MARGIN);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_constraint_referenced_ids, CONSTRAINT_REFERENCED_IDS);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_barrierAllowsGoneWidgets, BARRIER_ALLOWS_GONE_WIDGETS);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_pathMotionArc, PATH_MOTION_ARC);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constraintTag, CONSTRAINT_TAG);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_visibilityMode, VISIBILITY_MODE);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constrainedWidth, CONSTRAINED_WIDTH);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_constrainedHeight, CONSTRAINED_HEIGHT);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_polarRelativeTo, ANIMATE_CIRCLE_ANGLE_TO);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_transformPivotTarget, TRANSFORM_PIVOT_TARGET);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_quantizeMotionSteps, QUANTIZE_MOTION_STEPS);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_quantizeMotionPhase, QUANTIZE_MOTION_PHASE);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_quantizeMotionInterpolator, QUANTIZE_MOTION_INTERPOLATOR);
			overrideMapToConstant.append(R.styleable.ConstraintOverride_layout_wrapBehaviorInParent, LAYOUT_WRAP_BEHAVIOR);

				mapToConstant.append(R.styleable.Layout_layout_constraintLeft_toLeftOf, LEFT_TO_LEFT);
				mapToConstant.append(R.styleable.Layout_layout_constraintLeft_toRightOf, LEFT_TO_RIGHT);
				mapToConstant.append(R.styleable.Layout_layout_constraintRight_toLeftOf, RIGHT_TO_LEFT);
				mapToConstant.append(R.styleable.Layout_layout_constraintRight_toRightOf, RIGHT_TO_RIGHT);
				mapToConstant.append(R.styleable.Layout_layout_constraintTop_toTopOf, TOP_TO_TOP);
				mapToConstant.append(R.styleable.Layout_layout_constraintTop_toBottomOf, TOP_TO_BOTTOM);
				mapToConstant.append(R.styleable.Layout_layout_constraintBottom_toTopOf, BOTTOM_TO_TOP);
				mapToConstant.append(R.styleable.Layout_layout_constraintBottom_toBottomOf, BOTTOM_TO_BOTTOM);
				mapToConstant.append(R.styleable.Layout_layout_constraintBaseline_toBaselineOf, BASELINE_TO_BASELINE);

				mapToConstant.append(R.styleable.Layout_layout_editor_absoluteX, EDITOR_ABSOLUTE_X);
				mapToConstant.append(R.styleable.Layout_layout_editor_absoluteY, EDITOR_ABSOLUTE_Y);
				mapToConstant.append(R.styleable.Layout_layout_constraintGuide_begin, GUIDE_BEGIN);
				mapToConstant.append(R.styleable.Layout_layout_constraintGuide_end, GUIDE_END);
				mapToConstant.append(R.styleable.Layout_layout_constraintGuide_percent, GUIDE_PERCENT);
				mapToConstant.append(R.styleable.Layout_android_orientation, ORIENTATION);
				mapToConstant.append(R.styleable.Layout_layout_constraintStart_toEndOf, START_TO_END);
				mapToConstant.append(R.styleable.Layout_layout_constraintStart_toStartOf, START_TO_START);
				mapToConstant.append(R.styleable.Layout_layout_constraintEnd_toStartOf, END_TO_START);
				mapToConstant.append(R.styleable.Layout_layout_constraintEnd_toEndOf, END_TO_END);
				mapToConstant.append(R.styleable.Layout_layout_goneMarginLeft, GONE_LEFT_MARGIN);
				mapToConstant.append(R.styleable.Layout_layout_goneMarginTop, GONE_TOP_MARGIN);
				mapToConstant.append(R.styleable.Layout_layout_goneMarginRight, GONE_RIGHT_MARGIN);
				mapToConstant.append(R.styleable.Layout_layout_goneMarginBottom, GONE_BOTTOM_MARGIN);
				mapToConstant.append(R.styleable.Layout_layout_goneMarginStart, GONE_START_MARGIN);
				mapToConstant.append(R.styleable.Layout_layout_goneMarginEnd, GONE_END_MARGIN);
				mapToConstant.append(R.styleable.Layout_layout_constraintVertical_weight, VERTICAL_WEIGHT);
				mapToConstant.append(R.styleable.Layout_layout_constraintHorizontal_weight, HORIZONTAL_WEIGHT);
				mapToConstant.append(R.styleable.Layout_layout_constraintHorizontal_chainStyle, HORIZONTAL_STYLE);
				mapToConstant.append(R.styleable.Layout_layout_constraintVertical_chainStyle, VERTICAL_STYLE);

				mapToConstant.append(R.styleable.Layout_layout_constraintHorizontal_bias, HORIZONTAL_BIAS);
				mapToConstant.append(R.styleable.Layout_layout_constraintVertical_bias, VERTICAL_BIAS);
				mapToConstant.append(R.styleable.Layout_layout_constraintDimensionRatio, DIMENSION_RATIO);
				mapToConstant.append(R.styleable.Layout_layout_constraintLeft_creator, UNUSED);
				mapToConstant.append(R.styleable.Layout_layout_constraintTop_creator, UNUSED);
				mapToConstant.append(R.styleable.Layout_layout_constraintRight_creator, UNUSED);
				mapToConstant.append(R.styleable.Layout_layout_constraintBottom_creator, UNUSED);
				mapToConstant.append(R.styleable.Layout_layout_constraintBaseline_creator, UNUSED);
				mapToConstant.append(R.styleable.Layout_android_layout_marginLeft, LEFT_MARGIN);
				mapToConstant.append(R.styleable.Layout_android_layout_marginRight, RIGHT_MARGIN);
				mapToConstant.append(R.styleable.Layout_android_layout_marginStart, START_MARGIN);
				mapToConstant.append(R.styleable.Layout_android_layout_marginEnd, END_MARGIN);
				mapToConstant.append(R.styleable.Layout_android_layout_marginTop, TOP_MARGIN);
				mapToConstant.append(R.styleable.Layout_android_layout_marginBottom, BOTTOM_MARGIN);
				mapToConstant.append(R.styleable.Layout_android_layout_width, LAYOUT_WIDTH);
				mapToConstant.append(R.styleable.Layout_android_layout_height, LAYOUT_HEIGHT);
				mapToConstant.append(R.styleable.Layout_layout_constraintWidth, LAYOUT_CONSTRAINT_WIDTH);
				mapToConstant.append(R.styleable.Layout_layout_constraintHeight, LAYOUT_CONSTRAINT_HEIGHT);
				mapToConstant.append(R.styleable.Layout_layout_constrainedWidth, LAYOUT_CONSTRAINT_WIDTH);
				mapToConstant.append(R.styleable.Layout_layout_constrainedHeight, LAYOUT_CONSTRAINT_HEIGHT);
				mapToConstant.append(R.styleable.Layout_layout_wrapBehaviorInParent, LAYOUT_WRAP_BEHAVIOR);

				mapToConstant.append(R.styleable.Layout_layout_constraintCircle, CIRCLE);
				mapToConstant.append(R.styleable.Layout_layout_constraintCircleRadius, CIRCLE_RADIUS);
				mapToConstant.append(R.styleable.Layout_layout_constraintCircleAngle, CIRCLE_ANGLE);
				mapToConstant.append(R.styleable.Layout_layout_constraintWidth_percent, WIDTH_PERCENT);
				mapToConstant.append(R.styleable.Layout_layout_constraintHeight_percent, HEIGHT_PERCENT);

				mapToConstant.append(R.styleable.Layout_chainUseRtl, CHAIN_USE_RTL);
				mapToConstant.append(R.styleable.Layout_barrierDirection, BARRIER_DIRECTION);
				mapToConstant.append(R.styleable.Layout_barrierMargin, BARRIER_MARGIN);
				mapToConstant.append(R.styleable.Layout_constraint_referenced_ids, CONSTRAINT_REFERENCED_IDS);
				mapToConstant.append(R.styleable.Layout_barrierAllowsGoneWidgets, BARRIER_ALLOWS_GONE_WIDGETS);
				mapToConstant.append(R.styleable.Transform_android_rotation, ROTATION);
				mapToConstant.append(R.styleable.Transform_android_rotationX, ROTATION_X);
				mapToConstant.append(R.styleable.Transform_android_rotationY, ROTATION_Y);
				mapToConstant.append(R.styleable.Transform_android_scaleX, SCALE_X);
				mapToConstant.append(R.styleable.Transform_android_scaleY, SCALE_Y);
				mapToConstant.append(R.styleable.Transform_android_transformPivotX, TRANSFORM_PIVOT_X);
				mapToConstant.append(R.styleable.Transform_android_transformPivotY, TRANSFORM_PIVOT_Y);
				mapToConstant.append(R.styleable.Transform_android_translationX, TRANSLATION_X);
				mapToConstant.append(R.styleable.Transform_android_translationY, TRANSLATION_Y);
				mapToConstant.append(R.styleable.Transform_android_translationZ, TRANSLATION_Z);
				mapToConstant.append(R.styleable.Transform_android_elevation, ELEVATION);
				mapToConstant.append(R.styleable.Transform_transformPivotTarget, TRANSFORM_PIVOT_TARGET);

				mapToConstant.append(R.styleable.Motion_motionPathRotate, TRANSITION_PATH_ROTATE);
				mapToConstant.append(R.styleable.Motion_pathMotionArc, PATH_MOTION_ARC);
				mapToConstant.append(R.styleable.Motion_transitionEasing, TRANSITION_EASING);
				mapToConstant.append(R.styleable.Motion_drawPath, MOTION_DRAW_PATH);
				mapToConstant.append(R.styleable.Motion_animateRelativeTo, ANIMATE_RELATIVE_TO);
				mapToConstant.append(R.styleable.Motion_animateCircleAngleTo, ANIMATE_CIRCLE_ANGLE_TO);
				mapToConstant.append(R.styleable.Motion_motionStagger, MOTION_STAGGER);
				mapToConstant.append(R.styleable.Motion_quantizeMotionSteps, QUANTIZE_MOTION_STEPS);
				mapToConstant.append(R.styleable.Motion_quantizeMotionPhase, QUANTIZE_MOTION_PHASE);
				mapToConstant.append(R.styleable.Motion_quantizeMotionInterpolator, QUANTIZE_MOTION_INTERPOLATOR);
		}

		public virtual Dictionary<string, ConstraintAttribute> CustomAttributeSet
		{
			get
			{
				return mSavedAttributes;
			}
		}

		public virtual Constraint getParameters(int mId)
		{
			return get(mId);
		}

		/// <summary>
		/// This will copy Constraints from the ConstraintSet
		/// </summary>
		/// <param name="set"> </param>
		public virtual void readFallback(ConstraintSet set)
		{

			foreach (int? key in set.mConstraints.Keys)
			{
				int id = key.Value;
				Constraint parent = set.mConstraints[key];

				if (!mConstraints.ContainsKey(id))
				{
					mConstraints[id] = new Constraint();
				}
				Constraint constraint = mConstraints[id];
				if (constraint == null)
				{
					continue;
				}
				if (!constraint.layout.mApply)
				{
					constraint.layout.copyFrom(parent.layout);
				}
				if (!constraint.propertySet.mApply)
				{
					constraint.propertySet.copyFrom(parent.propertySet);
				}
				if (!constraint.transform.mApply)
				{
					constraint.transform.copyFrom(parent.transform);
				}
				if (!constraint.motion.mApply)
				{
					constraint.motion.copyFrom(parent.motion);
				}
				foreach (string s in parent.mCustomConstraints.Keys)
				{
					if (!constraint.mCustomConstraints.ContainsKey(s))
					{
						constraint.mCustomConstraints[s] = parent.mCustomConstraints[s];
					}
				}
			}
		}

		/// <summary>
		/// This will copy Constraints from the ConstraintLayout if it does not have parameters
		/// </summary>
		/// <param name="constraintLayout"> </param>
		public virtual void readFallback(ConstraintLayout constraintLayout)
		{
			int count = constraintLayout.ChildCount;
			for (int i = 0; i < count; i++)
			{
				View view = constraintLayout.getChildAt(i);
				ConstraintLayout.LayoutParams param = (ConstraintLayout.LayoutParams) view.LayoutParams;

				int id = view.Id;
				if (mForceId && id == -1)
				{
					throw new Exception("All children of ConstraintLayout must have ids to use ConstraintSet");
				}
				if (!mConstraints.ContainsKey(id))
				{
					mConstraints[id] = new Constraint();
				}
				Constraint constraint = mConstraints[id];
				if (constraint == null)
				{
					continue;
				}
				if (!constraint.layout.mApply)
				{
					constraint.fillFrom(id, param);
					if (view is ConstraintHelper)
					{
						constraint.layout.mReferenceIds = ((ConstraintHelper) view).ReferencedIds;
						if (view is Barrier)
						{
							Barrier barrier = (Barrier) view;
							constraint.layout.mBarrierAllowsGoneWidgets = barrier.AllowsGoneWidget;
							constraint.layout.mBarrierDirection = barrier.Type;
							constraint.layout.mBarrierMargin = barrier.Margin;
						}
					}
					constraint.layout.mApply = true;
				}
				if (!constraint.propertySet.mApply)
				{
					constraint.propertySet.visibility = view.Visibility;
					constraint.propertySet.alpha = view.Alpha;
					constraint.propertySet.mApply = true;
				}
				if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
				{

					if (!constraint.transform.mApply)
					{
						constraint.transform.mApply = true;
						constraint.transform.rotation = view.Rotation;
						constraint.transform.rotationX = view.RotationX;
						constraint.transform.rotationY = view.RotationY;
						constraint.transform.scaleX = view.ScaleX;
						constraint.transform.scaleY = view.ScaleY;

						float pivotX = view.PivotX; // we assume it is not set if set to 0.0
						float pivotY = view.PivotY; // we assume it is not set if set to 0.0

						if (pivotX != 0.0 || pivotY != 0.0)
						{
							constraint.transform.transformPivotX = pivotX;
							constraint.transform.transformPivotY = pivotY;
						}

						constraint.transform.translationX = view.TranslationX;
						constraint.transform.translationY = view.TranslationY;
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
						{
							constraint.transform.translationZ = view.TranslationZ;
							if (constraint.transform.applyElevation)
							{
								constraint.transform.elevation = view.Elevation;
							}
						}
					}
				}
			}

		}

		public virtual void applyDeltaFrom(ConstraintSet cs)
		{
			foreach (Constraint from in cs.mConstraints.Values)
			{
				if (from.mDelta != null)
				{
					if (!string.ReferenceEquals(from.mTargetString, null))
					{
						int count = 0;
						foreach (int key in mConstraints.Keys)
						{
							Constraint potential = getConstraint(key);
							if (!string.ReferenceEquals(potential.layout.mConstraintTag, null))
							{
								if (from.mTargetString.matches(potential.layout.mConstraintTag))
								{
									from.mDelta.applyDelta(potential);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
									potential.mCustomConstraints.putAll((Hashtable)from.mCustomConstraints.clone());
								}
							}
						}
					}
					else
					{
						Constraint constraint = getConstraint(from.mViewId);
						from.mDelta.applyDelta(constraint);
					}

				}
			}
		}

		/// <summary>
		/// Parse the constraint dimension attribute
		/// </summary>
		/// <param name="a"> </param>
		/// <param name="attr"> </param>
		/// <param name="orientation"> </param>
		internal static void parseDimensionConstraints(object data, TypedArray a, int attr, int orientation)
		{
			if (data == null)
			{
				return;
			}
			// data can be of:
			//
			// ConstraintLayout.LayoutParams
			// ConstraintSet.Layout
			// Constraint.Delta

			TypedValue v = a.peekValue(attr);
			int type = v.type;
			int finalValue = 0;
			bool finalConstrained = false;
			switch (type)
			{
				case TypedValue.TYPE_DIMENSION:
				{
					finalValue = a.getDimensionPixelSize(attr, 0);
				}
				break;
				case TypedValue.TYPE_STRING:
				{
					string value = a.getString(attr);
					parseDimensionConstraintsString(data, value, orientation);
					return;
				}
				default:
				{
					int value = a.getInt(attr, 0);
					switch (value)
					{
						case INTERNAL_WRAP_CONTENT:
						case INTERNAL_MATCH_PARENT:
						{
							finalValue = value;
						}
						break;
						case INTERNAL_MATCH_CONSTRAINT:
						{
							finalValue = MATCH_CONSTRAINT;
						}
						break;
						case INTERNAL_WRAP_CONTENT_CONSTRAINED:
						{
							finalValue = WRAP_CONTENT;
							finalConstrained = true;
						}
						break;
					}
				}
			break;
			}

			if (data is ConstraintLayout.LayoutParams)
			{
				ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams) data;
				if (orientation == HORIZONTAL)
				{
					@params.width = finalValue;
					@params.constrainedWidth = finalConstrained;
				}
				else
				{
					@params.height = finalValue;
					@params.constrainedHeight = finalConstrained;
				}
			}
			else if (data is Layout)
			{
				Layout @params = (Layout) data;
				if (orientation == HORIZONTAL)
				{
					@params.mWidth = finalValue;
					@params.constrainedWidth = finalConstrained;
				}
				else
				{
					@params.mHeight = finalValue;
					@params.constrainedHeight = finalConstrained;
				}
			}
			else if (data is Constraint.Delta)
			{
				Constraint.Delta @params = (Constraint.Delta) data;
				if (orientation == HORIZONTAL)
				{
					@params.add(LAYOUT_WIDTH, finalValue);
					@params.add(CONSTRAINED_WIDTH, finalConstrained);
				}
				else
				{
					@params.add(LAYOUT_HEIGHT, finalValue);
					@params.add(CONSTRAINED_HEIGHT, finalConstrained);
				}
			}
		}

		/// <summary>
		/// Parse the dimension ratio string
		/// </summary>
		/// <param name="value"> </param>
		internal static void parseDimensionRatioString(ConstraintLayout.LayoutParams @params, string value)
		{
			string dimensionRatio = value;
			float dimensionRatioValue = Float.NaN;
			int dimensionRatioSide = UNSET;
			if (!string.ReferenceEquals(dimensionRatio, null))
			{
				int len = dimensionRatio.Length;
				int commaIndex = dimensionRatio.IndexOf(',');
				if (commaIndex > 0 && commaIndex < len - 1)
				{
					string dimension = dimensionRatio.Substring(0, commaIndex);
					if (dimension.Equals("W", StringComparison.CurrentCultureIgnoreCase))
					{
						dimensionRatioSide = HORIZONTAL;
					}
					else if (dimension.Equals("H", StringComparison.CurrentCultureIgnoreCase))
					{
						dimensionRatioSide = VERTICAL;
					}
					commaIndex++;
				}
				else
				{
					commaIndex = 0;
				}
				int colonIndex = dimensionRatio.IndexOf(':');
				if (colonIndex >= 0 && colonIndex < len - 1)
				{
					string nominator = dimensionRatio.Substring(commaIndex, colonIndex - commaIndex);
					string denominator = dimensionRatio.Substring(colonIndex + 1);
					if (nominator.Length > 0 && denominator.Length > 0)
					{
						try
						{
							float nominatorValue = float.Parse(nominator);
							float denominatorValue = float.Parse(denominator);
							if (nominatorValue > 0 && denominatorValue > 0)
							{
								if (dimensionRatioSide == VERTICAL)
								{
									dimensionRatioValue = Math.Abs(denominatorValue / nominatorValue);
								}
								else
								{
									dimensionRatioValue = Math.Abs(nominatorValue / denominatorValue);
								}
							}
						}
						catch (System.FormatException)
						{
							// Ignore
						}
					}
				}
				else
				{
					string r = dimensionRatio.Substring(commaIndex);
					if (r.Length > 0)
					{
						try
						{
							dimensionRatioValue = float.Parse(r);
						}
						catch (System.FormatException)
						{
							// Ignore
						}
					}
				}
			}
			@params.dimensionRatio = dimensionRatio;
			@params.dimensionRatioValue = dimensionRatioValue;
			@params.dimensionRatioSide = dimensionRatioSide;
		}

		/// <summary>
		/// Parse the constraints string dimension
		/// </summary>
		/// <param name="value"> </param>
		/// <param name="orientation"> </param>
		internal static void parseDimensionConstraintsString(object data, string value, int orientation)
		{
			// data can be of:
			//
			// ConstraintLayout.LayoutParams
			// ConstraintSet.Layout
			// Constraint.Delta

			// String should be of the form
			//
			// "<Key>=<Value>"
			// supported Keys are:
			// "weight=<value>"
			// "ratio=<value>"
			// "parent=<value>"
			if (string.ReferenceEquals(value, null))
			{
				return;
			}

			int equalIndex = value.IndexOf('=');
			int len = value.Length;
			if (equalIndex > 0 && equalIndex < len - 1)
			{
				string key = value.Substring(0, equalIndex);
				string val = value.Substring(equalIndex + 1);
				if (val.Length > 0)
				{
					key = key.Trim();
					val = val.Trim();
					if (KEY_RATIO.Equals(key, StringComparison.CurrentCultureIgnoreCase))
					{
						if (data is ConstraintLayout.LayoutParams)
						{
							ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams) data;
							if (orientation == HORIZONTAL)
							{
								@params.width = MATCH_CONSTRAINT;
							}
							else
							{
								@params.height = MATCH_CONSTRAINT;
							}
							parseDimensionRatioString(@params, val);
						}
						else if (data is Layout)
						{
							Layout @params = (Layout) data;
							@params.dimensionRatio = val;
						}
						else if (data is Constraint.Delta)
						{
							Constraint.Delta @params = (Constraint.Delta) data;
							@params.add(DIMENSION_RATIO, val);
						}
					}
					else if (KEY_WEIGHT.Equals(key, StringComparison.CurrentCultureIgnoreCase))
					{
						try
						{
							float weight = float.Parse(val);
							if (data is ConstraintLayout.LayoutParams)
							{
								ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams) data;
								if (orientation == HORIZONTAL)
								{
									@params.width = MATCH_CONSTRAINT;
									@params.horizontalWeight = weight;
								}
								else
								{
									@params.height = MATCH_CONSTRAINT;
									@params.verticalWeight = weight;
								}
							}
							else if (data is Layout)
							{
								Layout @params = (Layout) data;
								if (orientation == HORIZONTAL)
								{
									@params.mWidth = MATCH_CONSTRAINT;
									@params.horizontalWeight = weight;
								}
								else
								{
									@params.mHeight = MATCH_CONSTRAINT;
									@params.verticalWeight = weight;
								}
							}
							else if (data is Constraint.Delta)
							{
								Constraint.Delta @params = (Constraint.Delta) data;
								if (orientation == HORIZONTAL)
								{
									@params.add(LAYOUT_WIDTH, MATCH_CONSTRAINT);
									@params.add(HORIZONTAL_WEIGHT, weight);
								}
								else
								{
									@params.add(LAYOUT_HEIGHT, MATCH_CONSTRAINT);
									@params.add(VERTICAL_WEIGHT, weight);
								}
							}
						}
						catch (System.FormatException)
						{
							// nothing
						}
					}
					else if (KEY_PERCENT_PARENT.Equals(key, StringComparison.CurrentCultureIgnoreCase))
					{
						try
						{
							float percent = Math.Min(1, float.Parse(val));
							percent = Math.Max(0, percent);
							if (data is ConstraintLayout.LayoutParams)
							{
								ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams) data;
								if (orientation == HORIZONTAL)
								{
									@params.width = MATCH_CONSTRAINT;
									@params.matchConstraintPercentWidth = percent;
									@params.matchConstraintDefaultWidth = MATCH_CONSTRAINT_PERCENT;
								}
								else
								{
									@params.height = MATCH_CONSTRAINT;
									@params.matchConstraintPercentHeight = percent;
									@params.matchConstraintDefaultHeight = MATCH_CONSTRAINT_PERCENT;
								}
							}
							else if (data is Layout)
							{
								Layout @params = (Layout) data;
								if (orientation == HORIZONTAL)
								{
									@params.mWidth = MATCH_CONSTRAINT;
									@params.widthPercent = percent;
									@params.widthDefault = MATCH_CONSTRAINT_PERCENT;
								}
								else
								{
									@params.mHeight = MATCH_CONSTRAINT;
									@params.heightPercent = percent;
									@params.heightDefault = MATCH_CONSTRAINT_PERCENT;
								}
							}
							else if (data is Constraint.Delta)
							{
								Constraint.Delta @params = (Constraint.Delta) data;
								if (orientation == HORIZONTAL)
								{
									@params.add(LAYOUT_WIDTH, MATCH_CONSTRAINT);
									@params.add(WIDTH_DEFAULT, MATCH_CONSTRAINT_PERCENT);
								}
								else
								{
									@params.add(LAYOUT_HEIGHT, MATCH_CONSTRAINT);
									@params.add(HEIGHT_DEFAULT, MATCH_CONSTRAINT_PERCENT);
								}
							}
						}
						catch (System.FormatException)
						{
							// nothing
						}
					}
				}
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public class Layout
		{
			public bool mIsGuideline = false;
			public bool mApply = false;
			public bool mOverride = false;
			public int mWidth;
			public int mHeight;
			public const int UNSET = ConstraintSet.UNSET;
			public static readonly int UNSET_GONE_MARGIN = int.MinValue;
			public int guideBegin = UNSET;
			public int guideEnd = UNSET;
			public float guidePercent = UNSET;
			public int leftToLeft = UNSET;
			public int leftToRight = UNSET;
			public int rightToLeft = UNSET;
			public int rightToRight = UNSET;
			public int topToTop = UNSET;
			public int topToBottom = UNSET;
			public int bottomToTop = UNSET;
			public int bottomToBottom = UNSET;
			public int baselineToBaseline = UNSET;
			public int baselineToTop = UNSET;
			public int baselineToBottom = UNSET;
			public int startToEnd = UNSET;
			public int startToStart = UNSET;
			public int endToStart = UNSET;
			public int endToEnd = UNSET;
			public float horizontalBias = 0.5f;
			public float verticalBias = 0.5f;
			public string dimensionRatio = null;
			public int circleConstraint = UNSET;
			public int circleRadius = 0;
			public float circleAngle = 0;
			public int editorAbsoluteX = UNSET;
			public int editorAbsoluteY = UNSET;
			public int orientation = UNSET;
			public int leftMargin = 0;
			public int rightMargin = 0;
			public int topMargin = 0;
			public int bottomMargin = 0;
			public int endMargin = 0;
			public int startMargin = 0;
			public int baselineMargin = 0;
			public int goneLeftMargin = UNSET_GONE_MARGIN;
			public int goneTopMargin = UNSET_GONE_MARGIN;
			public int goneRightMargin = UNSET_GONE_MARGIN;
			public int goneBottomMargin = UNSET_GONE_MARGIN;
			public int goneEndMargin = UNSET_GONE_MARGIN;
			public int goneStartMargin = UNSET_GONE_MARGIN;
			public int goneBaselineMargin = UNSET_GONE_MARGIN;
			public float verticalWeight = UNSET;
			public float horizontalWeight = UNSET;
			public int horizontalChainStyle = CHAIN_SPREAD;
			public int verticalChainStyle = CHAIN_SPREAD;
			public int widthDefault = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
			public int heightDefault = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
			public int widthMax = UNSET;
			public int heightMax = UNSET;
			public int widthMin = UNSET;
			public int heightMin = UNSET;
			public float widthPercent = 1;
			public float heightPercent = 1;
			public int mBarrierDirection = UNSET;
			public int mBarrierMargin = 0;
			public int mHelperType = UNSET;
			public int[] mReferenceIds;
			public string mReferenceIdString;
			public string mConstraintTag;
			public bool constrainedWidth = false;
			public bool constrainedHeight = false;
			// TODO public boolean mChainUseRtl = false;
			public bool mBarrierAllowsGoneWidgets = true;
			public int mWrapBehavior = ConstraintWidget.WRAP_BEHAVIOR_INCLUDED;

			public virtual void copyFrom(Layout src)
			{
				mIsGuideline = src.mIsGuideline;
				mWidth = src.mWidth;
				mApply = src.mApply;
				mHeight = src.mHeight;
				guideBegin = src.guideBegin;
				guideEnd = src.guideEnd;
				guidePercent = src.guidePercent;
				leftToLeft = src.leftToLeft;
				leftToRight = src.leftToRight;
				rightToLeft = src.rightToLeft;
				rightToRight = src.rightToRight;
				topToTop = src.topToTop;
				topToBottom = src.topToBottom;
				bottomToTop = src.bottomToTop;
				bottomToBottom = src.bottomToBottom;
				baselineToBaseline = src.baselineToBaseline;
				baselineToTop = src.baselineToTop;
				baselineToBottom = src.baselineToBottom;
				startToEnd = src.startToEnd;
				startToStart = src.startToStart;
				endToStart = src.endToStart;
				endToEnd = src.endToEnd;
				horizontalBias = src.horizontalBias;
				verticalBias = src.verticalBias;
				dimensionRatio = src.dimensionRatio;
				circleConstraint = src.circleConstraint;
				circleRadius = src.circleRadius;
				circleAngle = src.circleAngle;
				editorAbsoluteX = src.editorAbsoluteX;
				editorAbsoluteY = src.editorAbsoluteY;
				orientation = src.orientation;
				leftMargin = src.leftMargin;
				rightMargin = src.rightMargin;
				topMargin = src.topMargin;
				bottomMargin = src.bottomMargin;
				endMargin = src.endMargin;
				startMargin = src.startMargin;
				baselineMargin = src.baselineMargin;
				goneLeftMargin = src.goneLeftMargin;
				goneTopMargin = src.goneTopMargin;
				goneRightMargin = src.goneRightMargin;
				goneBottomMargin = src.goneBottomMargin;
				goneEndMargin = src.goneEndMargin;
				goneStartMargin = src.goneStartMargin;
				goneBaselineMargin = src.goneBaselineMargin;
				verticalWeight = src.verticalWeight;
				horizontalWeight = src.horizontalWeight;
				horizontalChainStyle = src.horizontalChainStyle;
				verticalChainStyle = src.verticalChainStyle;
				widthDefault = src.widthDefault;
				heightDefault = src.heightDefault;
				widthMax = src.widthMax;
				heightMax = src.heightMax;
				widthMin = src.widthMin;
				heightMin = src.heightMin;
				widthPercent = src.widthPercent;
				heightPercent = src.heightPercent;
				mBarrierDirection = src.mBarrierDirection;
				mBarrierMargin = src.mBarrierMargin;
				mHelperType = src.mHelperType;
				mConstraintTag = src.mConstraintTag;

				if (src.mReferenceIds != null && string.ReferenceEquals(src.mReferenceIdString, null))
				{
					mReferenceIds = Arrays.copyOf(src.mReferenceIds, src.mReferenceIds.Length);
				}
				else
				{
					mReferenceIds = null;
				}
				mReferenceIdString = src.mReferenceIdString;
				constrainedWidth = src.constrainedWidth;
				constrainedHeight = src.constrainedHeight;
				// TODO mChainUseRtl = t.mChainUseRtl;
				mBarrierAllowsGoneWidgets = src.mBarrierAllowsGoneWidgets;
				mWrapBehavior = src.mWrapBehavior;
			}

			internal static SparseIntArray mapToConstant = new SparseIntArray();
			internal const int BASELINE_TO_BASELINE = 1;
			internal const int BOTTOM_MARGIN = 2;
			internal const int BOTTOM_TO_BOTTOM = 3;
			internal const int BOTTOM_TO_TOP = 4;
			internal const int DIMENSION_RATIO = 5;
			internal const int EDITOR_ABSOLUTE_X = 6;
			internal const int EDITOR_ABSOLUTE_Y = 7;
			internal const int END_MARGIN = 8;
			internal const int END_TO_END = 9;
			internal const int END_TO_START = 10;
			internal const int GONE_BOTTOM_MARGIN = 11;
			internal const int GONE_END_MARGIN = 12;
			internal const int GONE_LEFT_MARGIN = 13;
			internal const int GONE_RIGHT_MARGIN = 14;
			internal const int GONE_START_MARGIN = 15;
			internal const int GONE_TOP_MARGIN = 16;
			internal const int GUIDE_BEGIN = 17;
			internal const int GUIDE_END = 18;
			internal const int GUIDE_PERCENT = 19;
			internal const int HORIZONTAL_BIAS = 20;
			internal const int LAYOUT_HEIGHT = 21;
			internal const int LAYOUT_WIDTH = 22;
			internal const int LEFT_MARGIN = 23;
			internal const int LEFT_TO_LEFT = 24;
			internal const int LEFT_TO_RIGHT = 25;
			internal const int ORIENTATION = 26;
			internal const int RIGHT_MARGIN = 27;
			internal const int RIGHT_TO_LEFT = 28;
			internal const int RIGHT_TO_RIGHT = 29;
			internal const int START_MARGIN = 30;
			internal const int START_TO_END = 31;
			internal const int START_TO_START = 32;
			internal const int TOP_MARGIN = 33;
			internal const int TOP_TO_BOTTOM = 34;
			internal const int TOP_TO_TOP = 35;
			internal const int VERTICAL_BIAS = 36;
			internal const int HORIZONTAL_WEIGHT = 37;
			internal const int VERTICAL_WEIGHT = 38;
			internal const int HORIZONTAL_STYLE = 39;
			internal const int VERTICAL_STYLE = 40;
			internal const int LAYOUT_CONSTRAINT_WIDTH = 41;
			internal const int LAYOUT_CONSTRAINT_HEIGHT = 42;

			internal const int CIRCLE = 61;
			internal const int CIRCLE_RADIUS = 62;
			internal const int CIRCLE_ANGLE = 63;
			internal const int WIDTH_PERCENT = 69;
			internal const int HEIGHT_PERCENT = 70;
			internal const int CHAIN_USE_RTL = 71;
			internal const int BARRIER_DIRECTION = 72;
			internal const int BARRIER_MARGIN = 73;
			internal const int CONSTRAINT_REFERENCED_IDS = 74;
			internal const int BARRIER_ALLOWS_GONE_WIDGETS = 75;
			internal const int UNUSED = 76;


			internal virtual void fillFromAttributeList(Context context, AttributeSet attrs)
			{
				TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.Layout);
				mApply = true;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
				int N = a.IndexCount;
				for (int i = 0; i < N; i++)
				{
					int attr = a.getIndex(i);

					switch (mapToConstant.get(attr))
					{
						case LEFT_TO_LEFT:
							leftToLeft = lookupID(a, attr, leftToLeft);
							break;
						case LEFT_TO_RIGHT:
							leftToRight = lookupID(a, attr, leftToRight);
							break;
						case RIGHT_TO_LEFT:
							rightToLeft = lookupID(a, attr, rightToLeft);
							break;
						case RIGHT_TO_RIGHT:
							rightToRight = lookupID(a, attr, rightToRight);
							break;
						case TOP_TO_TOP:
							topToTop = lookupID(a, attr, topToTop);
							break;
						case TOP_TO_BOTTOM:
							topToBottom = lookupID(a, attr, topToBottom);
							break;
						case BOTTOM_TO_TOP:
							bottomToTop = lookupID(a, attr, bottomToTop);
							break;
						case BOTTOM_TO_BOTTOM:
							bottomToBottom = lookupID(a, attr, bottomToBottom);
							break;
						case BASELINE_TO_BASELINE:
							baselineToBaseline = lookupID(a, attr, baselineToBaseline);
							break;
						case BASELINE_TO_TOP:
							baselineToTop = lookupID(a, attr, baselineToTop);
							break;
						case BASELINE_TO_BOTTOM:
							baselineToBottom = lookupID(a, attr, baselineToBottom);
							break;
						case EDITOR_ABSOLUTE_X:
							editorAbsoluteX = a.getDimensionPixelOffset(attr, editorAbsoluteX);
							break;
						case EDITOR_ABSOLUTE_Y:
							editorAbsoluteY = a.getDimensionPixelOffset(attr, editorAbsoluteY);
							break;
						case GUIDE_BEGIN:
							guideBegin = a.getDimensionPixelOffset(attr, guideBegin);
							break;
						case GUIDE_END:
							guideEnd = a.getDimensionPixelOffset(attr, guideEnd);
							break;
						case GUIDE_PERCENT:
							guidePercent = a.getFloat(attr, guidePercent);
							break;
						case ORIENTATION:
							orientation = a.getInt(attr, orientation);
							break;
						case START_TO_END:
							startToEnd = lookupID(a, attr, startToEnd);
							break;
						case START_TO_START:
							startToStart = lookupID(a, attr, startToStart);
							break;
						case END_TO_START:
							endToStart = lookupID(a, attr, endToStart);
							break;
						case END_TO_END:
							endToEnd = lookupID(a, attr, endToEnd);
							break;
						case CIRCLE:
							circleConstraint = lookupID(a, attr, circleConstraint);
							break;
						case CIRCLE_RADIUS:
							circleRadius = a.getDimensionPixelSize(attr, circleRadius);
							break;
						case CIRCLE_ANGLE:
							circleAngle = a.getFloat(attr, circleAngle);
							break;
						case GONE_LEFT_MARGIN:
							goneLeftMargin = a.getDimensionPixelSize(attr, goneLeftMargin);
							break;
						case GONE_TOP_MARGIN:
							goneTopMargin = a.getDimensionPixelSize(attr, goneTopMargin);
							break;
						case GONE_RIGHT_MARGIN:
							goneRightMargin = a.getDimensionPixelSize(attr, goneRightMargin);
							break;
						case GONE_BOTTOM_MARGIN:
							goneBottomMargin = a.getDimensionPixelSize(attr, goneBottomMargin);
							break;
						case GONE_START_MARGIN:
							goneStartMargin = a.getDimensionPixelSize(attr, goneStartMargin);
							break;
						case GONE_END_MARGIN:
							goneEndMargin = a.getDimensionPixelSize(attr, goneEndMargin);
							break;
						case GONE_BASELINE_MARGIN:
							goneBaselineMargin = a.getDimensionPixelSize(attr, goneBaselineMargin);
							break;
						case HORIZONTAL_BIAS:
							horizontalBias = a.getFloat(attr, horizontalBias);
							break;
						case VERTICAL_BIAS:
							verticalBias = a.getFloat(attr, verticalBias);
							break;
						case LEFT_MARGIN:
							leftMargin = a.getDimensionPixelSize(attr, leftMargin);
							break;
						case RIGHT_MARGIN:
							rightMargin = a.getDimensionPixelSize(attr, rightMargin);
							break;
						case START_MARGIN:
							if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
							{
								startMargin = a.getDimensionPixelSize(attr, startMargin);
							}
							break;
						case END_MARGIN:
							if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
							{
								endMargin = a.getDimensionPixelSize(attr, endMargin);
							}
							break;
						case TOP_MARGIN:
							topMargin = a.getDimensionPixelSize(attr, topMargin);
							break;
						case BOTTOM_MARGIN:
							bottomMargin = a.getDimensionPixelSize(attr, bottomMargin);
							break;
						case BASELINE_MARGIN:
							baselineMargin = a.getDimensionPixelSize(attr, baselineMargin);
							break;
						case LAYOUT_WIDTH:
							mWidth = a.getLayoutDimension(attr, mWidth);
							break;
						case LAYOUT_HEIGHT:
							mHeight = a.getLayoutDimension(attr, mHeight);
							break;
						case LAYOUT_CONSTRAINT_WIDTH:
							ConstraintSet.parseDimensionConstraints(this, a, attr, HORIZONTAL);
							break;
						case LAYOUT_CONSTRAINT_HEIGHT:
							ConstraintSet.parseDimensionConstraints(this, a, attr, VERTICAL);
							break;
						case WIDTH_DEFAULT:
							widthDefault = a.getInt(attr, widthDefault);
							break;
						case HEIGHT_DEFAULT:
							heightDefault = a.getInt(attr, heightDefault);
							break;
						case VERTICAL_WEIGHT:
							verticalWeight = a.getFloat(attr, verticalWeight);
							break;
						case HORIZONTAL_WEIGHT:
							horizontalWeight = a.getFloat(attr, horizontalWeight);
							break;
						case VERTICAL_STYLE:
							verticalChainStyle = a.getInt(attr, verticalChainStyle);
							break;
						case HORIZONTAL_STYLE:
							horizontalChainStyle = a.getInt(attr, horizontalChainStyle);
							break;
						case DIMENSION_RATIO:
							dimensionRatio = a.getString(attr);
							break;
						case HEIGHT_MAX:
							heightMax = a.getDimensionPixelSize(attr, heightMax);
							break;
						case WIDTH_MAX:
							widthMax = a.getDimensionPixelSize(attr, widthMax);
							break;
						case HEIGHT_MIN:
							heightMin = a.getDimensionPixelSize(attr, heightMin);
							break;
						case WIDTH_MIN:
							widthMin = a.getDimensionPixelSize(attr, widthMin);
							break;
						case WIDTH_PERCENT:
							widthPercent = a.getFloat(attr, 1);
							break;
						case HEIGHT_PERCENT:
							heightPercent = a.getFloat(attr, 1);
							break;
						case CONSTRAINED_WIDTH:
							constrainedWidth = a.getBoolean(attr, constrainedWidth);
							break;
						case CONSTRAINED_HEIGHT:
							constrainedHeight = a.getBoolean(attr, constrainedHeight);
							break;
						case CHAIN_USE_RTL:
							Log.e(TAG, "CURRENTLY UNSUPPORTED"); // TODO add support or remove
							//  TODO add support or remove  c.mChainUseRtl = a.getBoolean(attr,c.mChainUseRtl);
							break;
						case BARRIER_DIRECTION:
							mBarrierDirection = a.getInt(attr, mBarrierDirection);
							break;
						case LAYOUT_WRAP_BEHAVIOR:
							mWrapBehavior = a.getInt(attr, mWrapBehavior);
							break;
						case BARRIER_MARGIN:
							mBarrierMargin = a.getDimensionPixelSize(attr, mBarrierMargin);
							break;
						case CONSTRAINT_REFERENCED_IDS:
							mReferenceIdString = a.getString(attr);
							break;
						case BARRIER_ALLOWS_GONE_WIDGETS:
							mBarrierAllowsGoneWidgets = a.getBoolean(attr, mBarrierAllowsGoneWidgets);
							break;
						case CONSTRAINT_TAG:
							mConstraintTag = a.getString(attr);
							break;
						case UNUSED:
							Log.w(TAG, "unused attribute 0x" + attr.ToString("x") + "   " + mapToConstant.get(attr));
							break;
						default:
							Log.w(TAG, "Unknown attribute 0x" + attr.ToString("x") + "   " + mapToConstant.get(attr));

						break;
					}
				}
				a.recycle();
			}

			public virtual void dump(MotionScene scene, StringBuilder stringBuilder)
			{
				Field[] fields = this.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
				stringBuilder.Append("\n");
				for (int i = 0; i < fields.Length; i++)
				{
					Field field = fields[i];
					string name = field.Name;
					if (Modifier.isStatic(field.Modifiers))
					{
						continue;
					}
	//                 if (!field.isAccessible()) {
	//                    continue;
	//                }       if (!field.isAccessible()) {
	//                    continue;
	//                }

					try
					{
						object value = field.get(this);
						Type type = field.Type;
						if (type == Integer.TYPE)
						{
							int? iValue = (int?) value;
							if (iValue.Value != UNSET)
							{
								string stringId = scene.lookUpConstraintName(iValue.Value);
								stringBuilder.Append("    ");
								stringBuilder.Append(name);
								stringBuilder.Append(" = \"");
								stringBuilder.Append((string.ReferenceEquals(stringId, null)) ? iValue : stringId);
								stringBuilder.Append("\"\n");
							}
						}
						else if (type == Float.TYPE)
						{
							float? fValue = (float?) value;
							if (fValue != UNSET)
							{
								stringBuilder.Append("    ");
								stringBuilder.Append(name);
								stringBuilder.Append(" = \"");
								stringBuilder.Append(fValue);
								stringBuilder.Append("\"\n");

							}
						}

					}
					catch (IllegalAccessException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}


				}
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public class Transform
		{
			public bool mApply = false;
			public float rotation = 0;
			public float rotationX = 0;
			public float rotationY = 0;
			public float scaleX = 1;
			public float scaleY = 1;
			public float transformPivotX = Float.NaN;
			public float transformPivotY = Float.NaN;
			public int transformPivotTarget = UNSET;
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

			internal static SparseIntArray mapToConstant = new SparseIntArray();
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



			internal virtual void fillFromAttributeList(Context context, AttributeSet attrs)
			{
				TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.Transform);
				mApply = true;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
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
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public class PropertySet
		{
			public bool mApply = false;
			public int visibility = View.VISIBLE;
			public int mVisibilityMode = VISIBILITY_MODE_NORMAL;
			public float alpha = 1;
			public float mProgress = Float.NaN;

			public virtual void copyFrom(PropertySet src)
			{
				mApply = src.mApply;
				visibility = src.visibility;
				alpha = src.alpha;
				mProgress = src.mProgress;
				mVisibilityMode = src.mVisibilityMode;
			}

			internal virtual void fillFromAttributeList(Context context, AttributeSet attrs)
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
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public class Motion
		{
			public bool mApply = false;
			public int mAnimateRelativeTo = Layout.UNSET;
			public int mAnimateCircleAngleTo = 0;
			public string mTransitionEasing = null;
			public int mPathMotionArc = Layout.UNSET;
			public int mDrawPath = 0;
			public float mMotionStagger = Float.NaN;
			public int mPolarRelativeTo = Layout.UNSET;
			public float mPathRotate = Float.NaN;
			public float mQuantizeMotionPhase = Float.NaN;
			public int mQuantizeMotionSteps = Layout.UNSET;
			public string mQuantizeInterpolatorString = null;
			public int mQuantizeInterpolatorType = INTERPOLATOR_UNDEFINED; // undefined
			public int mQuantizeInterpolatorID = -1;
			internal const int INTERPOLATOR_REFERENCE_ID = -2;
			internal const int SPLINE_STRING = -1;
			internal const int INTERPOLATOR_UNDEFINED = -3;


			public virtual void copyFrom(Motion src)
			{
				mApply = src.mApply;
				mAnimateRelativeTo = src.mAnimateRelativeTo;
				mTransitionEasing = src.mTransitionEasing;
				mPathMotionArc = src.mPathMotionArc;
				mDrawPath = src.mDrawPath;
				mPathRotate = src.mPathRotate;
				mMotionStagger = src.mMotionStagger;
				mPolarRelativeTo = src.mPolarRelativeTo;
			}

			internal static SparseIntArray mapToConstant = new SparseIntArray();
			internal const int TRANSITION_PATH_ROTATE = 1;
			internal const int PATH_MOTION_ARC = 2;
			internal const int TRANSITION_EASING = 3;
			internal const int MOTION_DRAW_PATH = 4;
			internal const int ANIMATE_RELATIVE_TO = 5;
			internal const int ANIMATE_CIRCLE_ANGLE_TO = 6;
			internal const int MOTION_STAGGER = 7;
			internal const int QUANTIZE_MOTION_STEPS = 8;
			internal const int QUANTIZE_MOTION_PHASE = 9;
			internal const int QUANTIZE_MOTION_INTERPOLATOR = 10;



			internal virtual void fillFromAttributeList(Context context, AttributeSet attrs)
			{
				TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.Motion);
				mApply = true;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
				int N = a.IndexCount;
				for (int i = 0; i < N; i++)
				{
					int attr = a.getIndex(i);

					switch (mapToConstant.get(attr))
					{
						case TRANSITION_PATH_ROTATE:
							mPathRotate = a.getFloat(attr, mPathRotate);
							break;
						case PATH_MOTION_ARC:
							mPathMotionArc = a.getInt(attr, mPathMotionArc);
							break;
						case TRANSITION_EASING:
							TypedValue type = a.peekValue(attr);
							if (type.type == TypedValue.TYPE_STRING)
							{
								mTransitionEasing = a.getString(attr);
							}
							else
							{
								mTransitionEasing = Easing.NAMED_EASING[a.getInteger(attr, 0)];
							}
							break;
						case MOTION_DRAW_PATH:
							mDrawPath = a.getInt(attr, 0);
							break;
						case ANIMATE_RELATIVE_TO:
							mAnimateRelativeTo = lookupID(a, attr, mAnimateRelativeTo);
							break;
						case ANIMATE_CIRCLE_ANGLE_TO:
							mAnimateCircleAngleTo = a.getInteger(attr, mAnimateCircleAngleTo);
							break;
						case MOTION_STAGGER:
							mMotionStagger = a.getFloat(attr, mMotionStagger);
							break;
						case QUANTIZE_MOTION_STEPS:
							mQuantizeMotionSteps = a.getInteger(attr, mQuantizeMotionSteps);
							break;
						case QUANTIZE_MOTION_PHASE:
							mQuantizeMotionPhase = a.getFloat(attr, mQuantizeMotionPhase);
							break;
						case QUANTIZE_MOTION_INTERPOLATOR:
							type = a.peekValue(attr);

							if (type.type == TypedValue.TYPE_REFERENCE)
							{
								mQuantizeInterpolatorID = a.getResourceId(attr, -1);
								if (mQuantizeInterpolatorID != -1)
								{
									mQuantizeInterpolatorType = INTERPOLATOR_REFERENCE_ID;
								}
							}
							else if (type.type == TypedValue.TYPE_STRING)
							{
								mQuantizeInterpolatorString = a.getString(attr);
								if (mQuantizeInterpolatorString.IndexOf("/", StringComparison.Ordinal) > 0)
								{
									mQuantizeInterpolatorID = a.getResourceId(attr, -1);
									mQuantizeInterpolatorType = INTERPOLATOR_REFERENCE_ID;
								}
								else
								{
									mQuantizeInterpolatorType = SPLINE_STRING;
								}
							}
							else
							{
								mQuantizeInterpolatorType = a.getInteger(attr, mQuantizeInterpolatorID);
							}

							break;
					}
				}
				a.recycle();
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public class Constraint
		{
			internal int mViewId;
			internal string mTargetString;
			public readonly PropertySet propertySet = new PropertySet();
			public readonly Motion motion = new Motion();
			public readonly Layout layout = new Layout();
			public readonly Transform transform = new Transform();
			public Dictionary<string, ConstraintAttribute> mCustomConstraints = new Dictionary<string, ConstraintAttribute>();
			internal Delta mDelta;

			internal class Delta
			{
				internal const int INITIAL_BOOLEAN = 4;
				internal const int INITIAL_INT = 10;
				internal const int INITIAL_FLOAT = 10;
				internal const int INITIAL_STRING = 5;
				internal int[] mTypeInt = new int[INITIAL_INT];
				internal int[] mValueInt = new int[INITIAL_INT];
				internal int mCountInt = 0;

				internal virtual void add(int type, int value)
				{
					if (mCountInt >= mTypeInt.Length)
					{
						mTypeInt = Arrays.copyOf(mTypeInt, mTypeInt.Length * 2);
						mValueInt = Arrays.copyOf(mValueInt, mValueInt.Length * 2);
					}
					mTypeInt[mCountInt] = type;
					mValueInt[mCountInt++] = value;
				}

				internal int[] mTypeFloat = new int[INITIAL_FLOAT];
				internal float[] mValueFloat = new float[INITIAL_FLOAT];
				internal int mCountFloat = 0;

				internal virtual void add(int type, float value)
				{
					if (mCountFloat >= mTypeFloat.Length)
					{
						mTypeFloat = Arrays.copyOf(mTypeFloat, mTypeFloat.Length * 2);
						mValueFloat = Arrays.copyOf(mValueFloat, mValueFloat.Length * 2);
					}
					mTypeFloat[mCountFloat] = type;
					mValueFloat[mCountFloat++] = value;
				}

				internal int[] mTypeString = new int[INITIAL_STRING];
				internal string[] mValueString = new string[INITIAL_STRING];
				internal int mCountString = 0;

				internal virtual void add(int type, string value)
				{
					if (mCountString >= mTypeString.Length)
					{
						mTypeString = Arrays.copyOf(mTypeString, mTypeString.Length * 2);
						mValueString = Arrays.copyOf(mValueString, mValueString.Length * 2);
					}
					mTypeString[mCountString] = type;
					mValueString[mCountString++] = value;
				}

				internal int[] mTypeBoolean = new int[INITIAL_BOOLEAN];
				internal bool[] mValueBoolean = new bool[INITIAL_BOOLEAN];
				internal int mCountBoolean = 0;

				internal virtual void add(int type, bool value)
				{
					if (mCountBoolean >= mTypeBoolean.Length)
					{
						mTypeBoolean = Arrays.copyOf(mTypeBoolean, mTypeBoolean.Length * 2);
						mValueBoolean = Arrays.copyOf(mValueBoolean, mValueBoolean.Length * 2);
					}
					mTypeBoolean[mCountBoolean] = type;
					mValueBoolean[mCountBoolean++] = value;
				}

				internal virtual void applyDelta(Constraint c)
				{
					for (int i = 0; i < mCountInt; i++)
					{
						setDeltaValue(c, mTypeInt[i], mValueInt[i]);
					}
					for (int i = 0; i < mCountFloat; i++)
					{
						setDeltaValue(c, mTypeFloat[i], mValueFloat[i]);
					}
					for (int i = 0; i < mCountString; i++)
					{
						setDeltaValue(c, mTypeString[i], mValueString[i]);
					}
					for (int i = 0; i < mCountBoolean; i++)
					{
						setDeltaValue(c, mTypeBoolean[i], mValueBoolean[i]);
					}
				}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressLint("LogConditional") void printDelta(String tag)
				internal virtual void printDelta(string tag)
				{
					Log.v(tag, "int");

					for (int i = 0; i < mCountInt; i++)
					{
						Log.v(tag, mTypeInt[i] + " = " + mValueInt[i]);
					}
					Log.v(tag, "float");

					for (int i = 0; i < mCountFloat; i++)
					{
						Log.v(tag, mTypeFloat[i] + " = " + mValueFloat[i]);
					}
					Log.v(tag, "strings");

					for (int i = 0; i < mCountString; i++)
					{
						Log.v(tag, mTypeString[i] + " = " + mValueString[i]);
					}
					Log.v(tag, "boolean");
					for (int i = 0; i < mCountBoolean; i++)
					{
						Log.v(tag, mTypeBoolean[i] + " = " + mValueBoolean[i]);
					}
				}
			}

			public virtual void applyDelta(Constraint c)
			{
				if (mDelta != null)
				{
					mDelta.applyDelta(c);
				}
			}

			public virtual void printDelta(string tag)
			{
				if (mDelta != null)
				{
					mDelta.printDelta(tag);
				}
				else
				{
					Log.v(tag, "DELTA IS NULL");
				}
			}

			internal virtual ConstraintAttribute get(string attributeName, AttributeType attributeType)
			{
				ConstraintAttribute ret;
				if (mCustomConstraints.ContainsKey(attributeName))
				{
					ret = mCustomConstraints[attributeName];
					if (ret.Type != attributeType)
					{
						throw new System.ArgumentException("ConstraintAttribute is already a " + ret.Type.name());
					}
				}
				else
				{
					ret = new ConstraintAttribute(attributeName, attributeType);
					mCustomConstraints[attributeName] = ret;
				}
				return ret;
			}

			internal virtual void setStringValue(string attributeName, string value)
			{
				get(attributeName, AttributeType.STRING_TYPE).StringValue = value;
			}

			internal virtual void setFloatValue(string attributeName, float value)
			{
				get(attributeName, AttributeType.FLOAT_TYPE).FloatValue = value;
			}

			internal virtual void setIntValue(string attributeName, int value)
			{
				get(attributeName, AttributeType.INT_TYPE).IntValue = value;
			}

			internal virtual void setColorValue(string attributeName, int value)
			{
				get(attributeName, AttributeType.COLOR_TYPE).ColorValue = value;
			}

			public virtual Constraint clone()
			{
				Constraint clone = new Constraint();
				clone.layout.copyFrom(layout);
				clone.motion.copyFrom(motion);
				clone.propertySet.copyFrom(propertySet);
				clone.transform.copyFrom(transform);
				clone.mViewId = mViewId;
				clone.mDelta = mDelta;
				return clone;
			}

			internal virtual void fillFromConstraints(ConstraintHelper helper, int viewId, Constraints.LayoutParams param)
			{
				fillFromConstraints(viewId, param);
				if (helper is Barrier)
				{
					layout.mHelperType = BARRIER_TYPE;
					Barrier barrier = (Barrier) helper;
					layout.mBarrierDirection = barrier.Type;
					layout.mReferenceIds = barrier.ReferencedIds;
					layout.mBarrierMargin = barrier.Margin;
				}
			}

			internal virtual void fillFromConstraints(int viewId, Constraints.LayoutParams param)
			{
				fillFrom(viewId, param);
				propertySet.alpha = param.alpha;
				transform.rotation = param.rotation;
				transform.rotationX = param.rotationX;
				transform.rotationY = param.rotationY;
				transform.scaleX = param.scaleX;
				transform.scaleY = param.scaleY;
				transform.transformPivotX = param.transformPivotX;
				transform.transformPivotY = param.transformPivotY;
				transform.translationX = param.translationX;
				transform.translationY = param.translationY;
				transform.translationZ = param.translationZ;
				transform.elevation = param.elevation;
				transform.applyElevation = param.applyElevation;
			}

			internal virtual void fillFrom(int viewId, ConstraintLayout.LayoutParams param)
			{
				mViewId = viewId;
				layout.leftToLeft = param.leftToLeft;
				layout.leftToRight = param.leftToRight;
				layout.rightToLeft = param.rightToLeft;
				layout.rightToRight = param.rightToRight;
				layout.topToTop = param.topToTop;
				layout.topToBottom = param.topToBottom;
				layout.bottomToTop = param.bottomToTop;
				layout.bottomToBottom = param.bottomToBottom;
				layout.baselineToBaseline = param.baselineToBaseline;
				layout.baselineToTop = param.baselineToTop;
				layout.baselineToBottom = param.baselineToBottom;
				layout.startToEnd = param.startToEnd;
				layout.startToStart = param.startToStart;
				layout.endToStart = param.endToStart;
				layout.endToEnd = param.endToEnd;

				layout.horizontalBias = param.horizontalBias;
				layout.verticalBias = param.verticalBias;
				layout.dimensionRatio = param.dimensionRatio;

				layout.circleConstraint = param.circleConstraint;
				layout.circleRadius = param.circleRadius;
				layout.circleAngle = param.circleAngle;

				layout.editorAbsoluteX = param.editorAbsoluteX;
				layout.editorAbsoluteY = param.editorAbsoluteY;
				layout.orientation = param.orientation;
				layout.guidePercent = param.guidePercent;
				layout.guideBegin = param.guideBegin;
				layout.guideEnd = param.guideEnd;
				layout.mWidth = param.width;
				layout.mHeight = param.height;
				layout.leftMargin = param.leftMargin;
				layout.rightMargin = param.rightMargin;
				layout.topMargin = param.topMargin;
				layout.bottomMargin = param.bottomMargin;
				layout.baselineMargin = param.baselineMargin;
				layout.verticalWeight = param.verticalWeight;
				layout.horizontalWeight = param.horizontalWeight;
				layout.verticalChainStyle = param.verticalChainStyle;
				layout.horizontalChainStyle = param.horizontalChainStyle;
				layout.constrainedWidth = param.constrainedWidth;
				layout.constrainedHeight = param.constrainedHeight;
				layout.widthDefault = param.matchConstraintDefaultWidth;
				layout.heightDefault = param.matchConstraintDefaultHeight;
				layout.widthMax = param.matchConstraintMaxWidth;
				layout.heightMax = param.matchConstraintMaxHeight;
				layout.widthMin = param.matchConstraintMinWidth;
				layout.heightMin = param.matchConstraintMinHeight;
				layout.widthPercent = param.matchConstraintPercentWidth;
				layout.heightPercent = param.matchConstraintPercentHeight;
				layout.mConstraintTag = param.constraintTag;
				layout.goneTopMargin = param.goneTopMargin;
				layout.goneBottomMargin = param.goneBottomMargin;
				layout.goneLeftMargin = param.goneLeftMargin;
				layout.goneRightMargin = param.goneRightMargin;
				layout.goneStartMargin = param.goneStartMargin;
				layout.goneEndMargin = param.goneEndMargin;
				layout.goneBaselineMargin = param.goneBaselineMargin;
				layout.mWrapBehavior = param.wrapBehaviorInParent;

				int currentApiVersion = Build.VERSION.SDK_INT;
				if (currentApiVersion >= Build.VERSION_CODES.JELLY_BEAN_MR1)
				{
					layout.endMargin = param.MarginEnd;
					layout.startMargin = param.MarginStart;
				}
			}

			public virtual void applyTo(ConstraintLayout.LayoutParams param)
			{
				param.leftToLeft = layout.leftToLeft;
				param.leftToRight = layout.leftToRight;
				param.rightToLeft = layout.rightToLeft;
				param.rightToRight = layout.rightToRight;

				param.topToTop = layout.topToTop;
				param.topToBottom = layout.topToBottom;
				param.bottomToTop = layout.bottomToTop;
				param.bottomToBottom = layout.bottomToBottom;

				param.baselineToBaseline = layout.baselineToBaseline;
				param.baselineToTop = layout.baselineToTop;
				param.baselineToBottom = layout.baselineToBottom;

				param.startToEnd = layout.startToEnd;
				param.startToStart = layout.startToStart;
				param.endToStart = layout.endToStart;
				param.endToEnd = layout.endToEnd;

				param.leftMargin = layout.leftMargin;
				param.rightMargin = layout.rightMargin;
				param.topMargin = layout.topMargin;
				param.bottomMargin = layout.bottomMargin;
				param.goneStartMargin = layout.goneStartMargin;
				param.goneEndMargin = layout.goneEndMargin;
				param.goneTopMargin = layout.goneTopMargin;
				param.goneBottomMargin = layout.goneBottomMargin;

				param.horizontalBias = layout.horizontalBias;
				param.verticalBias = layout.verticalBias;

				param.circleConstraint = layout.circleConstraint;
				param.circleRadius = layout.circleRadius;
				param.circleAngle = layout.circleAngle;

				param.dimensionRatio = layout.dimensionRatio;
				param.editorAbsoluteX = layout.editorAbsoluteX;
				param.editorAbsoluteY = layout.editorAbsoluteY;
				param.verticalWeight = layout.verticalWeight;
				param.horizontalWeight = layout.horizontalWeight;
				param.verticalChainStyle = layout.verticalChainStyle;
				param.horizontalChainStyle = layout.horizontalChainStyle;
				param.constrainedWidth = layout.constrainedWidth;
				param.constrainedHeight = layout.constrainedHeight;
				param.matchConstraintDefaultWidth = layout.widthDefault;
				param.matchConstraintDefaultHeight = layout.heightDefault;
				param.matchConstraintMaxWidth = layout.widthMax;
				param.matchConstraintMaxHeight = layout.heightMax;
				param.matchConstraintMinWidth = layout.widthMin;
				param.matchConstraintMinHeight = layout.heightMin;
				param.matchConstraintPercentWidth = layout.widthPercent;
				param.matchConstraintPercentHeight = layout.heightPercent;
				param.orientation = layout.orientation;
				param.guidePercent = layout.guidePercent;
				param.guideBegin = layout.guideBegin;
				param.guideEnd = layout.guideEnd;
				param.width = layout.mWidth;
				param.height = layout.mHeight;
				if (!string.ReferenceEquals(layout.mConstraintTag, null))
				{
					param.constraintTag = layout.mConstraintTag;
				}
				param.wrapBehaviorInParent = layout.mWrapBehavior;

				if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
				{
					param.MarginStart = layout.startMargin;
					param.MarginEnd = layout.endMargin;
				}

				param.validate();
			}

		}

		/// <summary>
		/// Copy the constraints from a layout.
		/// </summary>
		/// <param name="context">            the context for the layout inflation </param>
		/// <param name="constraintLayoutId"> the id of the layout file </param>
		public virtual void clone(Context context, int constraintLayoutId)
		{
			clone((ConstraintLayout) LayoutInflater.from(context).inflate(constraintLayoutId, null));
		}

		/// <summary>
		/// Copy the constraints from a layout.
		/// </summary>
		/// <param name="set"> constraint set to copy </param>
		public virtual void clone(ConstraintSet set)
		{
			mConstraints.Clear();
			foreach (int? key in set.mConstraints.Keys)
			{
				Constraint constraint = set.mConstraints[key];
				if (constraint == null)
				{
					continue;
				}
				mConstraints[key] = constraint.clone();
			}
		}

		/// <summary>
		/// Copy the layout parameters of a ConstraintLayout.
		/// </summary>
		/// <param name="constraintLayout"> The ConstraintLayout to be copied </param>
		public virtual void clone(ConstraintLayout constraintLayout)
		{
			int count = constraintLayout.ChildCount;
			mConstraints.Clear();
			for (int i = 0; i < count; i++)
			{
				View view = constraintLayout.getChildAt(i);
				ConstraintLayout.LayoutParams param = (ConstraintLayout.LayoutParams) view.LayoutParams;

				int id = view.Id;
				if (mForceId && id == -1)
				{
					throw new Exception("All children of ConstraintLayout must have ids to use ConstraintSet");
				}
				if (!mConstraints.ContainsKey(id))
				{
					mConstraints[id] = new Constraint();
				}
				Constraint constraint = mConstraints[id];
				if (constraint == null)
				{
					continue;
				}
				constraint.mCustomConstraints = ConstraintAttribute.extractAttributes(mSavedAttributes, view);
				constraint.fillFrom(id, param);
				constraint.propertySet.visibility = view.Visibility;
				if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
				{
					constraint.propertySet.alpha = view.Alpha;
					constraint.transform.rotation = view.Rotation;
					constraint.transform.rotationX = view.RotationX;
					constraint.transform.rotationY = view.RotationY;
					constraint.transform.scaleX = view.ScaleX;
					constraint.transform.scaleY = view.ScaleY;

					float pivotX = view.PivotX; // we assume it is not set if set to 0.0
					float pivotY = view.PivotY; // we assume it is not set if set to 0.0

					if (pivotX != 0.0 || pivotY != 0.0)
					{
						constraint.transform.transformPivotX = pivotX;
						constraint.transform.transformPivotY = pivotY;
					}

					constraint.transform.translationX = view.TranslationX;
					constraint.transform.translationY = view.TranslationY;
					if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
					{
						constraint.transform.translationZ = view.TranslationZ;
						if (constraint.transform.applyElevation)
						{
							constraint.transform.elevation = view.Elevation;
						}
					}
				}
				if (view is Barrier)
				{
					Barrier barrier = ((Barrier) view);
					constraint.layout.mBarrierAllowsGoneWidgets = barrier.AllowsGoneWidget;
					constraint.layout.mReferenceIds = barrier.ReferencedIds;
					constraint.layout.mBarrierDirection = barrier.Type;
					constraint.layout.mBarrierMargin = barrier.Margin;
				}
			}
		}

		/// <summary>
		/// Copy the layout parameters of a ConstraintLayout.
		/// </summary>
		/// <param name="constraints"> The ConstraintLayout to be copied </param>
		public virtual void clone(Constraints constraints)
		{
			int count = constraints.ChildCount;
			mConstraints.Clear();
			for (int i = 0; i < count; i++)
			{
				View view = constraints.getChildAt(i);
				Constraints.LayoutParams param = (Constraints.LayoutParams) view.LayoutParams;

				int id = view.Id;
				if (mForceId && id == -1)
				{
					throw new Exception("All children of ConstraintLayout must have ids to use ConstraintSet");
				}
				if (!mConstraints.ContainsKey(id))
				{
					mConstraints[id] = new Constraint();
				}
				Constraint constraint = mConstraints[id];
				if (constraint == null)
				{
					continue;
				}
				if (view is ConstraintHelper)
				{
					ConstraintHelper helper = (ConstraintHelper) view;
					constraint.fillFromConstraints(helper, id, param);
				}
				constraint.fillFromConstraints(id, param);
			}
		}

		/// <summary>
		/// Apply the constraints to a ConstraintLayout.
		/// </summary>
		/// <param name="constraintLayout"> to be modified </param>
		public virtual void applyTo(ConstraintLayout constraintLayout)
		{
			applyToInternal(constraintLayout, true);
			constraintLayout.ConstraintSet = null;
			constraintLayout.requestLayout();
		}


		/// <summary>
		/// Apply the constraints to a ConstraintLayout.
		/// </summary>
		/// <param name="constraintLayout"> to be modified </param>
		public virtual void applyToWithoutCustom(ConstraintLayout constraintLayout)
		{
			applyToInternal(constraintLayout, false);
			constraintLayout.ConstraintSet = null;
		}

		/// <summary>
		/// Apply custom attributes alone
		/// </summary>
		/// <param name="constraintLayout"> </param>
		public virtual void applyCustomAttributes(ConstraintLayout constraintLayout)
		{
			int count = constraintLayout.ChildCount;
			for (int i = 0; i < count; i++)
			{
				View view = constraintLayout.getChildAt(i);
				int id = view.Id;
				if (!mConstraints.ContainsKey(id))
				{
					Log.w(TAG, "id unknown " + Debug.getName(view));
					continue;
				}
				if (mForceId && id == -1)
				{
					throw new Exception("All children of ConstraintLayout must have ids to use ConstraintSet");
				}

				if (mConstraints.ContainsKey(id))
				{
					Constraint constraint = mConstraints[id];
					if (constraint == null)
					{
						continue;
					}
					ConstraintAttribute.setAttributes(view, constraint.mCustomConstraints);
				}
			}
		}

		/// <summary>
		/// Apply Layout to Helper widget
		/// </summary>
		/// <param name="helper"> </param>
		/// <param name="child"> </param>
		/// <param name="layoutParams"> </param>
		/// <param name="mapIdToWidget"> </param>
		public virtual void applyToHelper(ConstraintHelper helper, ConstraintWidget child, LayoutParams layoutParams, SparseArray<ConstraintWidget> mapIdToWidget)
		{
			int id = helper.Id;
			if (mConstraints.ContainsKey(id))
			{
				Constraint constraint = mConstraints[id];
				if (constraint != null && child is HelperWidget)
				{
					HelperWidget helperWidget = (HelperWidget) child;
					helper.loadParameters(constraint, helperWidget, layoutParams, mapIdToWidget);
				}
			}
		}

		/// <summary>
		/// Fill in a ConstraintLayout LayoutParam based on the id.
		/// </summary>
		/// <param name="id">           Id of the view </param>
		/// <param name="layoutParams"> LayoutParams to be filled </param>
		public virtual void applyToLayoutParams(int id, ConstraintLayout.LayoutParams layoutParams)
		{
			if (mConstraints.ContainsKey(id))
			{
				Constraint constraint = mConstraints[id];
				if (constraint != null)
				{
					constraint.applyTo(layoutParams);
				}
			}
		}

		/// <summary>
		/// Used to set constraints when used by constraint layout
		/// </summary>
		internal virtual void applyToInternal(ConstraintLayout constraintLayout, bool applyPostLayout)
		{
			int count = constraintLayout.ChildCount;
			Dictionary<int?, Constraint>.KeyCollection used = new HashSet<int?>(mConstraints.Keys);
			for (int i = 0; i < count; i++)
			{
				View view = constraintLayout.getChildAt(i);
				int id = view.Id;
				if (!mConstraints.ContainsKey(id))
				{
					Log.w(TAG, "id unknown " + Debug.getName(view));
					continue;
				}

				if (mForceId && id == -1)
				{
					throw new Exception("All children of ConstraintLayout must have ids to use ConstraintSet");
				}
				if (id == -1)
				{
					continue;
				}

				if (mConstraints.ContainsKey(id))
				{
					used.remove(id);
					Constraint constraint = mConstraints[id];
					if (constraint == null)
					{
						continue;
					}
					if (view is Barrier)
					{
						constraint.layout.mHelperType = BARRIER_TYPE;
						Barrier barrier = (Barrier) view;
						barrier.Id = id;
						barrier.Type = constraint.layout.mBarrierDirection;
						barrier.Margin = constraint.layout.mBarrierMargin;

						barrier.AllowsGoneWidget = constraint.layout.mBarrierAllowsGoneWidgets;
						if (constraint.layout.mReferenceIds != null)
						{
							barrier.ReferencedIds = constraint.layout.mReferenceIds;
						}
						else if (!string.ReferenceEquals(constraint.layout.mReferenceIdString, null))
						{
							constraint.layout.mReferenceIds = convertReferenceString(barrier, constraint.layout.mReferenceIdString);
							barrier.ReferencedIds = constraint.layout.mReferenceIds;
						}
					}
					ConstraintLayout.LayoutParams param = (ConstraintLayout.LayoutParams) view.LayoutParams;
					param.validate();
					constraint.applyTo(param);

					if (applyPostLayout)
					{
						ConstraintAttribute.setAttributes(view, constraint.mCustomConstraints);
					}
					view.LayoutParams = param;
					if (constraint.propertySet.mVisibilityMode == VISIBILITY_MODE_NORMAL)
					{
						view.Visibility = constraint.propertySet.visibility;
					}
					if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
					{
						view.Alpha = constraint.propertySet.alpha;
						view.Rotation = constraint.transform.rotation;
						view.RotationX = constraint.transform.rotationX;
						view.RotationY = constraint.transform.rotationY;
						view.ScaleX = constraint.transform.scaleX;
						view.ScaleY = constraint.transform.scaleY;
						if (constraint.transform.transformPivotTarget != UNSET)
						{
							View layout = (View) view.Parent;
							View center = layout.findViewById(constraint.transform.transformPivotTarget);
							if (center != null)
							{
								float cy = (center.Top + center.Bottom) / 2.0f;
								float cx = (center.Left + center.Right) / 2.0f;
								if (view.Right - view.Left > 0 && view.Bottom - view.Top > 0)
								{
									float px = (cx - view.Left);
									float py = (cy - view.Top);
									view.PivotX = px;
									view.PivotY = py;
								}
							}
						}
						else
						{
							if (!float.IsNaN(constraint.transform.transformPivotX))
							{
								view.PivotX = constraint.transform.transformPivotX;
							}
							if (!float.IsNaN(constraint.transform.transformPivotY))
							{
								view.PivotY = constraint.transform.transformPivotY;
							}
						}
						view.TranslationX = constraint.transform.translationX;
						view.TranslationY = constraint.transform.translationY;
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
						{
							view.TranslationZ = constraint.transform.translationZ;
							if (constraint.transform.applyElevation)
							{
								view.Elevation = constraint.transform.elevation;
							}
						}
					}
				}
				else
				{
					Log.v(TAG, "WARNING NO CONSTRAINTS for view " + id);
				}
			}
			foreach (int? id in used)
			{
				Constraint constraint = mConstraints[id];
				if (constraint == null)
				{
					continue;
				}
				if (constraint.layout.mHelperType == BARRIER_TYPE)
				{
					Barrier barrier = new Barrier(constraintLayout.Context);
					barrier.Id = id;
					if (constraint.layout.mReferenceIds != null)
					{
						barrier.ReferencedIds = constraint.layout.mReferenceIds;
					}
					else if (!string.ReferenceEquals(constraint.layout.mReferenceIdString, null))
					{
						constraint.layout.mReferenceIds = convertReferenceString(barrier, constraint.layout.mReferenceIdString);
						barrier.ReferencedIds = constraint.layout.mReferenceIds;
					}
					barrier.Type = constraint.layout.mBarrierDirection;
					barrier.Margin = constraint.layout.mBarrierMargin;
					LayoutParams param = constraintLayout.generateDefaultLayoutParams();
					barrier.validateParams();
					constraint.applyTo(param);
					constraintLayout.addView(barrier, param);
				}
				if (constraint.layout.mIsGuideline)
				{
					Guideline g = new Guideline(constraintLayout.Context);
					g.Id = id;
					ConstraintLayout.LayoutParams param = constraintLayout.generateDefaultLayoutParams();
					constraint.applyTo(param);
					constraintLayout.addView(g, param);
				}
			}
			for (int i = 0; i < count; i++)
			{
				View view = constraintLayout.getChildAt(i);
				if (view is ConstraintHelper)
				{
					ConstraintHelper constraintHelper = (ConstraintHelper) view;
					constraintHelper.applyLayoutFeaturesInConstraintSet(constraintLayout);
				}
			}
		}

		/// <summary>
		/// Center widget between the other two widgets.
		/// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>, {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso cref="#RIGHT"/>)
		/// Note, sides must be all vertical or horizontal sides.
		/// </summary>
		/// <param name="centerID">     ID of the widget to be centered </param>
		/// <param name="firstID">      ID of the first widget to connect the left or top of the widget to </param>
		/// <param name="firstSide">    the side of the widget to connect to </param>
		/// <param name="firstMargin">  the connection margin </param>
		/// <param name="secondId">     the ID of the second widget to connect to right or top of the widget to </param>
		/// <param name="secondSide">   the side of the widget to connect to </param>
		/// <param name="secondMargin"> the connection margin </param>
		/// <param name="bias">         the ratio between two connections </param>
		public virtual void center(int centerID, int firstID, int firstSide, int firstMargin, int secondId, int secondSide, int secondMargin, float bias)
		{
			// Error checking

			if (firstMargin < 0)
			{
				throw new System.ArgumentException("margin must be > 0");
			}
			if (secondMargin < 0)
			{
				throw new System.ArgumentException("margin must be > 0");
			}
			if (bias <= 0 || bias > 1)
			{
				throw new System.ArgumentException("bias must be between 0 and 1 inclusive");
			}

			if (firstSide == LEFT || firstSide == RIGHT)
			{
				connect(centerID, LEFT, firstID, firstSide, firstMargin);
				connect(centerID, RIGHT, secondId, secondSide, secondMargin);
				Constraint constraint = mConstraints[centerID];
				if (constraint != null)
				{
					constraint.layout.horizontalBias = bias;
				}
			}
			else if (firstSide == START || firstSide == END)
			{
				connect(centerID, START, firstID, firstSide, firstMargin);
				connect(centerID, END, secondId, secondSide, secondMargin);
				Constraint constraint = mConstraints[centerID];
				if (constraint != null)
				{
					constraint.layout.horizontalBias = bias;
				}
			}
			else
			{
				connect(centerID, TOP, firstID, firstSide, firstMargin);
				connect(centerID, BOTTOM, secondId, secondSide, secondMargin);
				Constraint constraint = mConstraints[centerID];
				if (constraint != null)
				{
					constraint.layout.verticalBias = bias;
				}
			}
		}

		/// <summary>
		/// Centers the widget horizontally to the left and right side on another widgets sides.
		/// (for sides see: {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso cref="#RIGHT"/>)
		/// </summary>
		/// <param name="centerID">    ID of widget to be centered </param>
		/// <param name="leftId">      The Id of the widget on the left side </param>
		/// <param name="leftSide">    The side of the leftId widget to connect to </param>
		/// <param name="leftMargin">  The margin on the left side </param>
		/// <param name="rightId">     The Id of the widget on the right side </param>
		/// <param name="rightSide">   The side  of the rightId widget to connect to </param>
		/// <param name="rightMargin"> The margin on the right side </param>
		/// <param name="bias">        The ratio of the space on the left vs. right sides 0.5 is centered (default) </param>
		public virtual void centerHorizontally(int centerID, int leftId, int leftSide, int leftMargin, int rightId, int rightSide, int rightMargin, float bias)
		{
			connect(centerID, LEFT, leftId, leftSide, leftMargin);
			connect(centerID, RIGHT, rightId, rightSide, rightMargin);
			Constraint constraint = mConstraints[centerID];
			if (constraint != null)
			{
				constraint.layout.horizontalBias = bias;
			}
		}

		/// <summary>
		/// Centers the widgets horizontally to the left and right side on another widgets sides.
		/// (for sides see: <seealso cref="#START"/>, <seealso cref="#END"/>,
		/// <seealso cref="#LEFT"/>, <seealso cref="#RIGHT"/>)
		/// </summary>
		/// <param name="centerID">    ID of widget to be centered </param>
		/// <param name="startId">     The Id of the widget on the start side (left in non rtl languages) </param>
		/// <param name="startSide">   The side of the startId widget to connect to </param>
		/// <param name="startMargin"> The margin on the start side </param>
		/// <param name="endId">       The Id of the widget on the start side (left in non rtl languages) </param>
		/// <param name="endSide">     The side of the endId widget to connect to </param>
		/// <param name="endMargin">   The margin on the end side </param>
		/// <param name="bias">        The ratio of the space on the start vs end side 0.5 is centered (default) </param>
		public virtual void centerHorizontallyRtl(int centerID, int startId, int startSide, int startMargin, int endId, int endSide, int endMargin, float bias)
		{
			connect(centerID, START, startId, startSide, startMargin);
			connect(centerID, END, endId, endSide, endMargin);
			Constraint constraint = mConstraints[centerID];
			if (constraint != null)
			{
				constraint.layout.horizontalBias = bias;
			}
		}

		/// <summary>
		/// Centers the widgets vertically to the top and bottom side on another widgets sides.
		/// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>)
		/// </summary>
		/// <param name="centerID">     ID of widget to be centered </param>
		/// <param name="topId">        The Id of the widget on the top side </param>
		/// <param name="topSide">      The side of the leftId widget to connect to </param>
		/// <param name="topMargin">    The margin on the top side </param>
		/// <param name="bottomId">     The Id of the widget on the bottom side </param>
		/// <param name="bottomSide">   The side of the bottomId widget to connect to </param>
		/// <param name="bottomMargin"> The margin on the bottom side </param>
		/// <param name="bias">         The ratio of the space on the top vs. bottom sides 0.5 is centered (default) </param>
		public virtual void centerVertically(int centerID, int topId, int topSide, int topMargin, int bottomId, int bottomSide, int bottomMargin, float bias)
		{
			connect(centerID, TOP, topId, topSide, topMargin);
			connect(centerID, BOTTOM, bottomId, bottomSide, bottomMargin);
			Constraint constraint = mConstraints[centerID];
			if (constraint != null)
			{
				constraint.layout.verticalBias = bias;
			}
		}

		/// <summary>
		/// Spaces a set of widgets vertically between the view topId and bottomId.
		/// Widgets can be spaced with weights.
		/// This operation sets all the related margins to 0.
		/// <para>
		/// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>)
		/// 
		/// </para>
		/// </summary>
		/// <param name="topId">      The id of the widget to connect to or PARENT_ID </param>
		/// <param name="topSide">    the side of the start to connect to </param>
		/// <param name="bottomId">   The id of the widget to connect to or PARENT_ID </param>
		/// <param name="bottomSide"> the side of the right to connect to </param>
		/// <param name="chainIds">   widgets to use as a chain </param>
		/// <param name="weights">    can be null </param>
		/// <param name="style">      set the style of the chain </param>
		public virtual void createVerticalChain(int topId, int topSide, int bottomId, int bottomSide, int[] chainIds, float[] weights, int style)
		{
			if (chainIds.Length < 2)
			{
				throw new System.ArgumentException("must have 2 or more widgets in a chain");
			}
			if (weights != null && weights.Length != chainIds.Length)
			{
				throw new System.ArgumentException("must have 2 or more widgets in a chain");
			}
			if (weights != null)
			{
				get(chainIds[0]).layout.verticalWeight = weights[0];
			}
			get(chainIds[0]).layout.verticalChainStyle = style;

			connect(chainIds[0], TOP, topId, topSide, 0);
			for (int i = 1; i < chainIds.Length; i++)
			{
				int chainId = chainIds[i];
				connect(chainIds[i], TOP, chainIds[i - 1], BOTTOM, 0);
				connect(chainIds[i - 1], BOTTOM, chainIds[i], TOP, 0);
				if (weights != null)
				{
					get(chainIds[i]).layout.verticalWeight = weights[i];
				}
			}
			connect(chainIds[chainIds.Length - 1], BOTTOM, bottomId, bottomSide, 0);
		}

		/// <summary>
		/// Spaces a set of widgets horizontally between the view startID and endId.
		/// Widgets can be spaced with weights.
		/// This operation sets all the related margins to 0.
		/// <para>
		/// (for sides see: {@link #START, <seealso cref="#END"/>,
		/// {@link #LEFT, <seealso cref="#RIGHT"/>
		/// 
		/// </para>
		/// </summary>
		/// <param name="leftId">    The id of the widget to connect to or PARENT_ID </param>
		/// <param name="leftSide">  the side of the start to connect to </param>
		/// <param name="rightId">   The id of the widget to connect to or PARENT_ID </param>
		/// <param name="rightSide"> the side of the right to connect to </param>
		/// <param name="chainIds">  The widgets in the chain </param>
		/// <param name="weights">   The weight to assign to each element in the chain or null </param>
		/// <param name="style">     The type of chain </param>
		public virtual void createHorizontalChain(int leftId, int leftSide, int rightId, int rightSide, int[] chainIds, float[] weights, int style)
		{
			createHorizontalChain(leftId, leftSide, rightId, rightSide, chainIds, weights, style, LEFT, RIGHT);
		}

		/// <summary>
		/// Spaces a set of widgets horizontal between the view startID and endId.
		/// Widgets can be spaced with weights.
		/// (for sides see: {@link #START, <seealso cref="#END"/>,
		/// {@link #LEFT, <seealso cref="#RIGHT"/>)
		/// </summary>
		/// <param name="startId">   The id of the widget to connect to or PARENT_ID </param>
		/// <param name="startSide"> the side of the start to connect to </param>
		/// <param name="endId">     The id of the widget to connect to or PARENT_ID </param>
		/// <param name="endSide">   the side of the end to connect to </param>
		/// <param name="chainIds">  The widgets in the chain </param>
		/// <param name="weights">   The weight to assign to each element in the chain or null </param>
		/// <param name="style">     The type of chain </param>
		public virtual void createHorizontalChainRtl(int startId, int startSide, int endId, int endSide, int[] chainIds, float[] weights, int style)
		{
			createHorizontalChain(startId, startSide, endId, endSide, chainIds, weights, style, START, END);
		}

		private void createHorizontalChain(int leftId, int leftSide, int rightId, int rightSide, int[] chainIds, float[] weights, int style, int left, int right)
		{

			if (chainIds.Length < 2)
			{
				throw new System.ArgumentException("must have 2 or more widgets in a chain");
			}
			if (weights != null && weights.Length != chainIds.Length)
			{
				throw new System.ArgumentException("must have 2 or more widgets in a chain");
			}
			if (weights != null)
			{
				get(chainIds[0]).layout.horizontalWeight = weights[0];
			}
			get(chainIds[0]).layout.horizontalChainStyle = style;
			connect(chainIds[0], left, leftId, leftSide, UNSET);
			for (int i = 1; i < chainIds.Length; i++)
			{
				int chainId = chainIds[i];
				connect(chainIds[i], left, chainIds[i - 1], right, UNSET);
				connect(chainIds[i - 1], right, chainIds[i], left, UNSET);
				if (weights != null)
				{
					get(chainIds[i]).layout.horizontalWeight = weights[i];
				}
			}

			connect(chainIds[chainIds.Length - 1], right, rightId, rightSide, UNSET);

		}

		/// <summary>
		/// Create a constraint between two widgets.
		/// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>, {@link #START, <seealso cref="#END"/>,
		/// {@link #LEFT, <seealso cref="#RIGHT"/>, <seealso cref="#BASELINE"/>)
		/// </summary>
		/// <param name="startID">   the ID of the widget to be constrained </param>
		/// <param name="startSide"> the side of the widget to constrain </param>
		/// <param name="endID">     the id of the widget to constrain to </param>
		/// <param name="endSide">   the side of widget to constrain to </param>
		/// <param name="margin">    the margin to constrain (margin must be positive) </param>
		public virtual void connect(int startID, int startSide, int endID, int endSide, int margin)
		{
			if (!mConstraints.ContainsKey(startID))
			{
				mConstraints[startID] = new Constraint();
			}
			Constraint constraint = mConstraints[startID];
			if (constraint == null)
			{
				return;
			}
			switch (startSide)
			{
				case LEFT:
					if (endSide == LEFT)
					{
						constraint.layout.leftToLeft = endID;
						constraint.layout.leftToRight = Layout.UNSET;
					}
					else if (endSide == RIGHT)
					{
						constraint.layout.leftToRight = endID;
						constraint.layout.leftToLeft = Layout.UNSET;

					}
					else
					{
						throw new System.ArgumentException("Left to " + sideToString(endSide) + " undefined");
					}
					constraint.layout.leftMargin = margin;
					break;
				case RIGHT:
					if (endSide == LEFT)
					{
						constraint.layout.rightToLeft = endID;
						constraint.layout.rightToRight = Layout.UNSET;

					}
					else if (endSide == RIGHT)
					{
						constraint.layout.rightToRight = endID;
						constraint.layout.rightToLeft = Layout.UNSET;

					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					constraint.layout.rightMargin = margin;
					break;
				case TOP:
					if (endSide == TOP)
					{
						constraint.layout.topToTop = endID;
						constraint.layout.topToBottom = Layout.UNSET;
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
					}
					else if (endSide == BOTTOM)
					{
						constraint.layout.topToBottom = endID;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					constraint.layout.topMargin = margin;
					break;
				case BOTTOM:
					if (endSide == BOTTOM)
					{
						constraint.layout.bottomToBottom = endID;
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
					}
					else if (endSide == TOP)
					{
						constraint.layout.bottomToTop = endID;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					constraint.layout.bottomMargin = margin;
					break;
				case BASELINE:
					if (endSide == BASELINE)
					{
						constraint.layout.baselineToBaseline = endID;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.topToBottom = Layout.UNSET;
					}
					else if (endSide == TOP)
					{
						constraint.layout.baselineToTop = endID;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.topToBottom = Layout.UNSET;
					}
					else if (endSide == BOTTOM)
					{
						constraint.layout.baselineToBottom = endID;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.topToBottom = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					break;
				case START:
					if (endSide == START)
					{
						constraint.layout.startToStart = endID;
						constraint.layout.startToEnd = Layout.UNSET;
					}
					else if (endSide == END)
					{
						constraint.layout.startToEnd = endID;
						constraint.layout.startToStart = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					constraint.layout.startMargin = margin;
					break;
				case END:
					if (endSide == END)
					{
						constraint.layout.endToEnd = endID;
						constraint.layout.endToStart = Layout.UNSET;
					}
					else if (endSide == START)
					{
						constraint.layout.endToStart = endID;
						constraint.layout.endToEnd = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					constraint.layout.endMargin = margin;
					break;
				default:
					throw new System.ArgumentException(sideToString(startSide) + " to " + sideToString(endSide) + " unknown");
			}
		}

		/// <summary>
		/// Create a constraint between two widgets.
		/// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>, {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso cref="#RIGHT"/>, <seealso cref="#BASELINE"/>)
		/// </summary>
		/// <param name="startID">   the ID of the widget to be constrained </param>
		/// <param name="startSide"> the side of the widget to constrain </param>
		/// <param name="endID">     the id of the widget to constrain to </param>
		/// <param name="endSide">   the side of widget to constrain to </param>
		public virtual void connect(int startID, int startSide, int endID, int endSide)
		{
			if (!mConstraints.ContainsKey(startID))
			{
				mConstraints[startID] = new Constraint();
			}
			Constraint constraint = mConstraints[startID];
			if (constraint == null)
			{
				return;
			}
			switch (startSide)
			{
				case LEFT:
					if (endSide == LEFT)
					{
						constraint.layout.leftToLeft = endID;
						constraint.layout.leftToRight = Layout.UNSET;
					}
					else if (endSide == RIGHT)
					{
						constraint.layout.leftToRight = endID;
						constraint.layout.leftToLeft = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("left to " + sideToString(endSide) + " undefined");
					}
					break;
				case RIGHT:
					if (endSide == LEFT)
					{
						constraint.layout.rightToLeft = endID;
						constraint.layout.rightToRight = Layout.UNSET;

					}
					else if (endSide == RIGHT)
					{
						constraint.layout.rightToRight = endID;
						constraint.layout.rightToLeft = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					break;
				case TOP:
					if (endSide == TOP)
					{
						constraint.layout.topToTop = endID;
						constraint.layout.topToBottom = Layout.UNSET;
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
					}
					else if (endSide == BOTTOM)
					{
						constraint.layout.topToBottom = endID;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					break;
				case BOTTOM:
					if (endSide == BOTTOM)
					{
						constraint.layout.bottomToBottom = endID;
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
					}
					else if (endSide == TOP)
					{
						constraint.layout.bottomToTop = endID;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					break;
				case BASELINE:
					if (endSide == BASELINE)
					{
						constraint.layout.baselineToBaseline = endID;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.topToBottom = Layout.UNSET;
					}
					else if (endSide == TOP)
					{
						constraint.layout.baselineToTop = endID;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.topToBottom = Layout.UNSET;
					}
					else if (endSide == BOTTOM)
					{
						constraint.layout.baselineToBottom = endID;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.topToBottom = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					break;
				case START:
					if (endSide == START)
					{
						constraint.layout.startToStart = endID;
						constraint.layout.startToEnd = Layout.UNSET;
					}
					else if (endSide == END)
					{
						constraint.layout.startToEnd = endID;
						constraint.layout.startToStart = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					break;
				case END:
					if (endSide == END)
					{
						constraint.layout.endToEnd = endID;
						constraint.layout.endToStart = Layout.UNSET;
					}
					else if (endSide == START)
					{
						constraint.layout.endToStart = endID;
						constraint.layout.endToEnd = Layout.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					break;
				default:
					throw new System.ArgumentException(sideToString(startSide) + " to " + sideToString(endSide) + " unknown");
			}
		}

		/// <summary>
		/// Centers the view horizontally relative to toView's position.
		/// </summary>
		/// <param name="viewId"> ID of view to center Horizontally </param>
		/// <param name="toView"> ID of view to center on (or in) </param>
		public virtual void centerHorizontally(int viewId, int toView)
		{
			if (toView == PARENT_ID)
			{
				center(viewId, PARENT_ID, ConstraintSet.LEFT, 0, PARENT_ID, ConstraintSet.RIGHT, 0, 0.5f);
			}
			else
			{
				center(viewId, toView, ConstraintSet.RIGHT, 0, toView, ConstraintSet.LEFT, 0, 0.5f);
			}
		}

		/// <summary>
		/// Centers the view horizontally relative to toView's position.
		/// </summary>
		/// <param name="viewId"> ID of view to center Horizontally </param>
		/// <param name="toView"> ID of view to center on (or in) </param>
		public virtual void centerHorizontallyRtl(int viewId, int toView)
		{
			if (toView == PARENT_ID)
			{
				center(viewId, PARENT_ID, ConstraintSet.START, 0, PARENT_ID, ConstraintSet.END, 0, 0.5f);
			}
			else
			{
				center(viewId, toView, ConstraintSet.END, 0, toView, ConstraintSet.START, 0, 0.5f);
			}
		}

		/// <summary>
		/// Centers the view vertically relative to toView's position.
		/// </summary>
		/// <param name="viewId"> ID of view to center Horizontally </param>
		/// <param name="toView"> ID of view to center on (or in) </param>
		public virtual void centerVertically(int viewId, int toView)
		{
			if (toView == PARENT_ID)
			{
				center(viewId, PARENT_ID, ConstraintSet.TOP, 0, PARENT_ID, ConstraintSet.BOTTOM, 0, 0.5f);
			}
			else
			{
				center(viewId, toView, ConstraintSet.BOTTOM, 0, toView, ConstraintSet.TOP, 0, 0.5f);
			}
		}

		/// <summary>
		/// Remove all constraints from this view.
		/// </summary>
		/// <param name="viewId"> ID of view to remove all connections to </param>
		public virtual void clear(int viewId)
		{
			mConstraints.Remove(viewId);
		}

		/// <summary>
		/// Remove a constraint from this view.
		/// </summary>
		/// <param name="viewId"> ID of view to center on (or in) </param>
		/// <param name="anchor"> the Anchor to remove constraint from </param>
		public virtual void clear(int viewId, int anchor)
		{
			if (mConstraints.ContainsKey(viewId))
			{
				Constraint constraint = mConstraints[viewId];
				if (constraint == null)
				{
					return;
				}
				switch (anchor)
				{
					case LEFT:
						constraint.layout.leftToRight = Layout.UNSET;
						constraint.layout.leftToLeft = Layout.UNSET;
						constraint.layout.leftMargin = Layout.UNSET;
						constraint.layout.goneLeftMargin = Layout.UNSET_GONE_MARGIN;
						break;
					case RIGHT:
						constraint.layout.rightToRight = Layout.UNSET;
						constraint.layout.rightToLeft = Layout.UNSET;
						constraint.layout.rightMargin = Layout.UNSET;
						constraint.layout.goneRightMargin = Layout.UNSET_GONE_MARGIN;
						break;
					case TOP:
						constraint.layout.topToBottom = Layout.UNSET;
						constraint.layout.topToTop = Layout.UNSET;
						constraint.layout.topMargin = 0;
						constraint.layout.goneTopMargin = Layout.UNSET_GONE_MARGIN;
						break;
					case BOTTOM:
						constraint.layout.bottomToTop = Layout.UNSET;
						constraint.layout.bottomToBottom = Layout.UNSET;
						constraint.layout.bottomMargin = 0;
						constraint.layout.goneBottomMargin = Layout.UNSET_GONE_MARGIN;
						break;
					case BASELINE:
						constraint.layout.baselineToBaseline = Layout.UNSET;
						constraint.layout.baselineToTop = Layout.UNSET;
						constraint.layout.baselineToBottom = Layout.UNSET;
						constraint.layout.baselineMargin = 0;
						constraint.layout.goneBaselineMargin = Layout.UNSET_GONE_MARGIN;
						break;
					case START:
						constraint.layout.startToEnd = Layout.UNSET;
						constraint.layout.startToStart = Layout.UNSET;
						constraint.layout.startMargin = 0;
						constraint.layout.goneStartMargin = Layout.UNSET_GONE_MARGIN;
						break;
					case END:
						constraint.layout.endToStart = Layout.UNSET;
						constraint.layout.endToEnd = Layout.UNSET;
						constraint.layout.endMargin = 0;
						constraint.layout.goneEndMargin = Layout.UNSET_GONE_MARGIN;
						break;
					case CIRCLE_REFERENCE:
						constraint.layout.circleAngle = Layout.UNSET;
						constraint.layout.circleRadius = Layout.UNSET;
						constraint.layout.circleConstraint = Layout.UNSET;
						break;
					default:
						throw new System.ArgumentException("unknown constraint");
				}
			}
		}

		/// <summary>
		/// Sets the margin.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust the margin on </param>
		/// <param name="anchor"> The side to adjust the margin on </param>
		/// <param name="value">  The new value for the margin </param>
		public virtual void setMargin(int viewId, int anchor, int value)
		{
			Constraint constraint = get(viewId);
			switch (anchor)
			{
				case LEFT:
					constraint.layout.leftMargin = value;
					break;
				case RIGHT:
					constraint.layout.rightMargin = value;
					break;
				case TOP:
					constraint.layout.topMargin = value;
					break;
				case BOTTOM:
					constraint.layout.bottomMargin = value;
					break;
				case BASELINE:
					constraint.layout.baselineMargin = value;
					break;
				case START:
					constraint.layout.startMargin = value;
					break;
				case END:
					constraint.layout.endMargin = value;
					break;
				default:
					throw new System.ArgumentException("unknown constraint");
			}
		}

		/// <summary>
		/// Sets the gone margin.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust the margin on </param>
		/// <param name="anchor"> The side to adjust the margin on </param>
		/// <param name="value">  The new value for the margin </param>
		public virtual void setGoneMargin(int viewId, int anchor, int value)
		{
			Constraint constraint = get(viewId);
			switch (anchor)
			{
				case LEFT:
					constraint.layout.goneLeftMargin = value;
					break;
				case RIGHT:
					constraint.layout.goneRightMargin = value;
					break;
				case TOP:
					constraint.layout.goneTopMargin = value;
					break;
				case BOTTOM:
					constraint.layout.goneBottomMargin = value;
					break;
				case BASELINE:
					constraint.layout.goneBaselineMargin = value;
					break;
				case START:
					constraint.layout.goneStartMargin = value;
					break;
				case END:
					constraint.layout.goneEndMargin = value;
					break;
				default:
					throw new System.ArgumentException("unknown constraint");
			}
		}

		/// <summary>
		/// Adjust the horizontal bias of the view (used with views constrained on left and right).
		/// </summary>
		/// <param name="viewId"> ID of view to adjust the horizontal </param>
		/// <param name="bias">   the new bias 0.5 is in the middle </param>
		public virtual void setHorizontalBias(int viewId, float bias)
		{
			get(viewId).layout.horizontalBias = bias;
		}

		/// <summary>
		/// Adjust the vertical bias of the view (used with views constrained on left and right).
		/// </summary>
		/// <param name="viewId"> ID of view to adjust the vertical </param>
		/// <param name="bias">   the new bias 0.5 is in the middle </param>
		public virtual void setVerticalBias(int viewId, float bias)
		{
			get(viewId).layout.verticalBias = bias;
		}

		/// <summary>
		/// Constrains the views aspect ratio.
		/// For Example a HD screen is 16 by 9 = 16/(float)9 = 1.777f.
		/// </summary>
		/// <param name="viewId"> ID of view to constrain </param>
		/// <param name="ratio">  The ratio of the width to height (width / height) </param>
		public virtual void setDimensionRatio(int viewId, string ratio)
		{
			get(viewId).layout.dimensionRatio = ratio;
		}

		/// <summary>
		/// Adjust the visibility of a view.
		/// </summary>
		/// <param name="viewId">     ID of view to adjust the vertical </param>
		/// <param name="visibility"> the visibility </param>
		public virtual void setVisibility(int viewId, int visibility)
		{
			get(viewId).propertySet.visibility = visibility;
		}

		/// <summary>
		/// ConstraintSet will not setVisibility. <seealso cref="#VISIBILITY_MODE_IGNORE"/> or {@link
		/// #VISIBILITY_MODE_NORMAL}.
		/// </summary>
		/// <param name="viewId">         ID of view </param>
		/// <param name="visibilityMode"> </param>
		public virtual void setVisibilityMode(int viewId, int visibilityMode)
		{
			get(viewId).propertySet.mVisibilityMode = visibilityMode;
		}

		/// <summary>
		/// ConstraintSet will not setVisibility. <seealso cref="#VISIBILITY_MODE_IGNORE"/> or {@link
		/// #VISIBILITY_MODE_NORMAL}.
		/// </summary>
		/// <param name="viewId"> ID of view </param>
		public virtual int getVisibilityMode(int viewId)
		{
			return get(viewId).propertySet.mVisibilityMode;
		}

		/// <summary>
		/// Get the visibility flag set in this view
		/// </summary>
		/// <param name="viewId"> the id of the view </param>
		/// <returns> the visibility constraint for the view </returns>
		public virtual int getVisibility(int viewId)
		{
			return get(viewId).propertySet.visibility;
		}

		/// <summary>
		/// Get the height set in the view
		/// </summary>
		/// <param name="viewId"> the id of the view </param>
		/// <returns> return the height constraint of the view </returns>
		public virtual int getHeight(int viewId)
		{
			return get(viewId).layout.mHeight;
		}

		/// <summary>
		/// Get the width set in the view
		/// </summary>
		/// <param name="viewId"> the id of the view </param>
		/// <returns> return the width constraint of the view </returns>
		public virtual int getWidth(int viewId)
		{
			return get(viewId).layout.mWidth;
		}

		/// <summary>
		/// Adjust the alpha of a view.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust the vertical </param>
		/// <param name="alpha">  the alpha </param>
		public virtual void setAlpha(int viewId, float alpha)
		{
			get(viewId).propertySet.alpha = alpha;
		}

		/// <summary>
		/// return with the constraint set will apply elevation for the specified view.
		/// </summary>
		/// <returns> true if the elevation will be set on this view (default is false) </returns>
		public virtual bool getApplyElevation(int viewId)
		{
			return get(viewId).transform.applyElevation;
		}

		/// <summary>
		/// set if elevation will be applied to the view.
		/// Elevation logic is based on style and animation. By default it is not used because it would
		/// lead to unexpected results.
		/// </summary>
		/// <param name="apply"> true if this constraint set applies elevation to this view </param>
		public virtual void setApplyElevation(int viewId, bool apply)
		{
			if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
			{
				get(viewId).transform.applyElevation = apply;
			}
		}

		/// <summary>
		/// Adjust the elevation of a view.
		/// </summary>
		/// <param name="viewId">    ID of view to adjust the elevation </param>
		/// <param name="elevation"> the elevation </param>
		public virtual void setElevation(int viewId, float elevation)
		{
			if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
			{
				get(viewId).transform.elevation = elevation;
				get(viewId).transform.applyElevation = true;
			}
		}

		/// <summary>
		/// Adjust the post-layout rotation about the Z axis of a view.
		/// </summary>
		/// <param name="viewId">   ID of view to adjust th Z rotation </param>
		/// <param name="rotation"> the rotation about the X axis </param>
		public virtual void setRotation(int viewId, float rotation)
		{
			get(viewId).transform.rotation = rotation;
		}

		/// <summary>
		/// Adjust the post-layout rotation about the X axis of a view.
		/// </summary>
		/// <param name="viewId">    ID of view to adjust th X rotation </param>
		/// <param name="rotationX"> the rotation about the X axis </param>
		public virtual void setRotationX(int viewId, float rotationX)
		{
			get(viewId).transform.rotationX = rotationX;
		}

		/// <summary>
		/// Adjust the post-layout rotation about the Y axis of a view.
		/// </summary>
		/// <param name="viewId">    ID of view to adjust the Y rotation </param>
		/// <param name="rotationY"> the rotationY </param>
		public virtual void setRotationY(int viewId, float rotationY)
		{
			get(viewId).transform.rotationY = rotationY;
		}

		/// <summary>
		/// Adjust the post-layout scale in X of a view.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust the scale in X </param>
		/// <param name="scaleX"> the scale in X </param>
		public virtual void setScaleX(int viewId, float scaleX)
		{
			get(viewId).transform.scaleX = scaleX;
		}

		/// <summary>
		/// Adjust the post-layout scale in Y of a view.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust the scale in Y </param>
		/// <param name="scaleY"> the scale in Y </param>
		public virtual void setScaleY(int viewId, float scaleY)
		{
			get(viewId).transform.scaleY = scaleY;
		}

		/// <summary>
		/// Set X location of the pivot point around which the view will rotate and scale.
		/// use Float.NaN to clear the pivot value.
		/// Note: once an actual View has had its pivot set it cannot be cleared.
		/// </summary>
		/// <param name="viewId">          ID of view to adjust the transforms pivot point about X </param>
		/// <param name="transformPivotX"> X location of the pivot point. </param>
		public virtual void setTransformPivotX(int viewId, float transformPivotX)
		{
			get(viewId).transform.transformPivotX = transformPivotX;
		}

		/// <summary>
		/// Set Y location of the pivot point around which the view will rotate and scale.
		/// use Float.NaN to clear the pivot value.
		/// Note: once an actual View has had its pivot set it cannot be cleared.
		/// </summary>
		/// <param name="viewId">          ID of view to adjust the transforms pivot point about Y </param>
		/// <param name="transformPivotY"> Y location of the pivot point. </param>
		public virtual void setTransformPivotY(int viewId, float transformPivotY)
		{
			get(viewId).transform.transformPivotY = transformPivotY;
		}

		/// <summary>
		/// Set X,Y location of the pivot point around which the view will rotate and scale.
		/// use Float.NaN to clear the pivot value.
		/// Note: once an actual View has had its pivot set it cannot be cleared.
		/// </summary>
		/// <param name="viewId">          ID of view to adjust the transforms pivot point </param>
		/// <param name="transformPivotX"> X location of the pivot point. </param>
		/// <param name="transformPivotY"> Y location of the pivot point. </param>
		public virtual void setTransformPivot(int viewId, float transformPivotX, float transformPivotY)
		{
			Constraint constraint = get(viewId);
			constraint.transform.transformPivotY = transformPivotY;
			constraint.transform.transformPivotX = transformPivotX;
		}

		/// <summary>
		/// Adjust the post-layout X translation of a view.
		/// </summary>
		/// <param name="viewId">       ID of view to translate in X </param>
		/// <param name="translationX"> the translation in X </param>
		public virtual void setTranslationX(int viewId, float translationX)
		{
			get(viewId).transform.translationX = translationX;
		}

		/// <summary>
		/// Adjust the  post-layout Y translation of a view.
		/// </summary>
		/// <param name="viewId">       ID of view to to translate in Y </param>
		/// <param name="translationY"> the translation in Y </param>
		public virtual void setTranslationY(int viewId, float translationY)
		{
			get(viewId).transform.translationY = translationY;
		}

		/// <summary>
		/// Adjust the post-layout translation of a view.
		/// </summary>
		/// <param name="viewId">       ID of view to adjust its translation in X & Y </param>
		/// <param name="translationX"> the translation in X </param>
		/// <param name="translationY"> the translation in Y </param>
		public virtual void setTranslation(int viewId, float translationX, float translationY)
		{
			Constraint constraint = get(viewId);
			constraint.transform.translationX = translationX;
			constraint.transform.translationY = translationY;
		}

		/// <summary>
		/// Adjust the translation in Z of a view.
		/// </summary>
		/// <param name="viewId">       ID of view to adjust </param>
		/// <param name="translationZ"> the translationZ </param>
		public virtual void setTranslationZ(int viewId, float translationZ)
		{
			if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
			{
				get(viewId).transform.translationZ = translationZ;
			}
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public virtual void setEditorAbsoluteX(int viewId, int position)
		{
			get(viewId).layout.editorAbsoluteX = position;
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public virtual void setEditorAbsoluteY(int viewId, int position)
		{
			get(viewId).layout.editorAbsoluteY = position;
		}

		/// <summary>
		/// Sets the wrap behavior of the widget in the parent's wrap computation
		/// </summary>
		public virtual void setLayoutWrapBehavior(int viewId, int behavior)
		{
			if (behavior >= 0 && behavior <= ConstraintWidget.WRAP_BEHAVIOR_SKIPPED)
			{
				get(viewId).layout.mWrapBehavior = behavior;
			}
		}

		/// <summary>
		/// Sets the height of the view. It can be a dimension, <seealso cref="#WRAP_CONTENT"/> or {@link
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its height </param>
		/// <param name="height"> the height of the view
		/// @since 1.1 </param>
		public virtual void constrainHeight(int viewId, int height)
		{
			get(viewId).layout.mHeight = height;
		}

		/// <summary>
		/// Sets the width of the view. It can be a dimension, <seealso cref="#WRAP_CONTENT"/> or {@link
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its width </param>
		/// <param name="width">  the width of the view
		/// @since 1.1 </param>
		public virtual void constrainWidth(int viewId, int width)
		{
			get(viewId).layout.mWidth = width;
		}

		/// <summary>
		/// Constrain the view on a circle constraint
		/// </summary>
		/// <param name="viewId"> ID of the view we constrain </param>
		/// <param name="id">     ID of the view we constrain relative to </param>
		/// <param name="radius"> the radius of the circle in degrees </param>
		/// <param name="angle">  the angle
		/// @since 1.1 </param>
		public virtual void constrainCircle(int viewId, int id, int radius, float angle)
		{
			Constraint constraint = get(viewId);
			constraint.layout.circleConstraint = id;
			constraint.layout.circleRadius = radius;
			constraint.layout.circleAngle = angle;
		}

		/// <summary>
		/// Sets the maximum height of the view. It is a dimension, It is only applicable if height is
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust it height </param>
		/// <param name="height"> the maximum height of the constraint
		/// @since 1.1 </param>
		public virtual void constrainMaxHeight(int viewId, int height)
		{
			get(viewId).layout.heightMax = height;
		}

		/// <summary>
		/// Sets the maximum width of the view. It is a dimension, It is only applicable if width is
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its max height </param>
		/// <param name="width">  the maximum width of the view
		/// @since 1.1 </param>
		public virtual void constrainMaxWidth(int viewId, int width)
		{
			get(viewId).layout.widthMax = width;
		}

		/// <summary>
		/// Sets the height of the view. It is a dimension, It is only applicable if height is
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its min height </param>
		/// <param name="height"> the minimum height of the view
		/// @since 1.1 </param>
		public virtual void constrainMinHeight(int viewId, int height)
		{
			get(viewId).layout.heightMin = height;
		}

		/// <summary>
		/// Sets the width of the view.  It is a dimension, It is only applicable if width is
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its min height </param>
		/// <param name="width">  the minimum width of the view
		/// @since 1.1 </param>
		public virtual void constrainMinWidth(int viewId, int width)
		{
			get(viewId).layout.widthMin = width;
		}

		/// <summary>
		/// Sets the width of the view as a percentage of the parent.
		/// </summary>
		/// <param name="viewId"> </param>
		/// <param name="percent">
		/// @since 1.1 </param>
		public virtual void constrainPercentWidth(int viewId, float percent)
		{
			get(viewId).layout.widthPercent = percent;
		}

		/// <summary>
		/// Sets the height of the view as a percentage of the parent.
		/// </summary>
		/// <param name="viewId"> </param>
		/// <param name="percent">
		/// @since 1.1 </param>
		public virtual void constrainPercentHeight(int viewId, float percent)
		{
			get(viewId).layout.heightPercent = percent;
		}

		/// <summary>
		/// Sets how the height is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
		/// Default is spread.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its matchConstraintDefaultHeight </param>
		/// <param name="height"> MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD
		/// @since 1.1 </param>
		public virtual void constrainDefaultHeight(int viewId, int height)
		{
			get(viewId).layout.heightDefault = height;
		}

		/// <summary>
		/// Sets how the width is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
		/// Default is spread.
		/// </summary>
		/// <param name="viewId">      ID of view to adjust its matchConstraintDefaultWidth </param>
		/// <param name="constrained"> if true with will be constrained
		/// @since 1.1 </param>
		public virtual void constrainedWidth(int viewId, bool constrained)
		{
			get(viewId).layout.constrainedWidth = constrained;
		}

		/// <summary>
		/// Sets how the height is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
		/// Default is spread.
		/// </summary>
		/// <param name="viewId">      ID of view to adjust its matchConstraintDefaultHeight </param>
		/// <param name="constrained"> if true height will be constrained
		/// @since 1.1 </param>
		public virtual void constrainedHeight(int viewId, bool constrained)
		{
			get(viewId).layout.constrainedHeight = constrained;
		}

		/// <summary>
		/// Sets how the width is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
		/// Default is spread.
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its matchConstraintDefaultWidth </param>
		/// <param name="width">  SPREAD or WRAP
		/// @since 1.1 </param>
		public virtual void constrainDefaultWidth(int viewId, int width)
		{
			get(viewId).layout.widthDefault = width;
		}

		/// <summary>
		/// The child's weight that we can use to distribute the available horizontal space
		/// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its HorizontalWeight </param>
		/// <param name="weight"> the weight that we can use to distribute the horizontal space </param>
		public virtual void setHorizontalWeight(int viewId, float weight)
		{
			get(viewId).layout.horizontalWeight = weight;
		}

		/// <summary>
		/// The child's weight that we can use to distribute the available vertical space
		/// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
		/// </summary>
		/// <param name="viewId"> ID of view to adjust its VerticalWeight </param>
		/// <param name="weight"> the weight that we can use to distribute the vertical space </param>
		public virtual void setVerticalWeight(int viewId, float weight)
		{
			get(viewId).layout.verticalWeight = weight;
		}

		/// <summary>
		/// How the elements of the horizontal chain will be positioned. if the dimension
		/// behaviour is set to MATCH_CONSTRAINT. The possible values are:
		/// 
		/// <ul>
		/// <li><seealso cref="#CHAIN_SPREAD"/> -- the elements will be spread out</li>
		/// <li><seealso cref="#CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
		/// be spread out</li>
		/// <li><seealso cref="#CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
		/// horizontal bias attribute of the child will then affect the positioning of the packed
		/// elements</li>
		/// </ul>
		/// </summary>
		/// <param name="viewId">     ID of view to adjust its HorizontalChainStyle </param>
		/// <param name="chainStyle"> the weight that we can use to distribute the horizontal space </param>
		public virtual void setHorizontalChainStyle(int viewId, int chainStyle)
		{
			get(viewId).layout.horizontalChainStyle = chainStyle;
		}

		/// <summary>
		/// How the elements of the vertical chain will be positioned. in a chain, if the dimension
		/// behaviour is set to MATCH_CONSTRAINT
		/// 
		/// <ul>
		/// <li><seealso cref="#CHAIN_SPREAD"/> -- the elements will be spread out</li>
		/// <li><seealso cref="#CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
		/// be spread out</li>
		/// <li><seealso cref="#CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
		/// vertical bias attribute of the child will then affect the positioning of the packed
		/// elements</li>
		/// </ul>
		/// </summary>
		/// <param name="viewId">     ID of view to adjust its VerticalChainStyle </param>
		/// <param name="chainStyle"> the weight that we can use to distribute the horizontal space </param>
		public virtual void setVerticalChainStyle(int viewId, int chainStyle)
		{
			get(viewId).layout.verticalChainStyle = chainStyle;
		}

		/// <summary>
		/// Adds a view to a horizontal chain.
		/// </summary>
		/// <param name="viewId">  view to add </param>
		/// <param name="leftId">  view in chain to the left </param>
		/// <param name="rightId"> view in chain to the right </param>
		public virtual void addToHorizontalChain(int viewId, int leftId, int rightId)
		{
			connect(viewId, LEFT, leftId, (leftId == PARENT_ID) ? LEFT : RIGHT, 0);
			connect(viewId, RIGHT, rightId, (rightId == PARENT_ID) ? RIGHT : LEFT, 0);
			if (leftId != PARENT_ID)
			{
				connect(leftId, RIGHT, viewId, LEFT, 0);
			}
			if (rightId != PARENT_ID)
			{
				connect(rightId, LEFT, viewId, RIGHT, 0);
			}
		}

		/// <summary>
		/// Adds a view to a horizontal chain.
		/// </summary>
		/// <param name="viewId">  view to add </param>
		/// <param name="leftId">  view to the start side </param>
		/// <param name="rightId"> view to the end side </param>
		public virtual void addToHorizontalChainRTL(int viewId, int leftId, int rightId)
		{
			connect(viewId, START, leftId, (leftId == PARENT_ID) ? START : END, 0);
			connect(viewId, END, rightId, (rightId == PARENT_ID) ? END : START, 0);
			if (leftId != PARENT_ID)
			{
				connect(leftId, END, viewId, START, 0);
			}
			if (rightId != PARENT_ID)
			{
				connect(rightId, START, viewId, END, 0);
			}
		}

		/// <summary>
		/// Adds a view to a vertical chain.
		/// </summary>
		/// <param name="viewId">   view to add to a vertical chain </param>
		/// <param name="topId">    view above. </param>
		/// <param name="bottomId"> view below </param>
		public virtual void addToVerticalChain(int viewId, int topId, int bottomId)
		{
			connect(viewId, TOP, topId, (topId == PARENT_ID) ? TOP : BOTTOM, 0);
			connect(viewId, BOTTOM, bottomId, (bottomId == PARENT_ID) ? BOTTOM : TOP, 0);
			if (topId != PARENT_ID)
			{
				connect(topId, BOTTOM, viewId, TOP, 0);
			}
			if (bottomId != PARENT_ID)
			{
				connect(bottomId, TOP, viewId, BOTTOM, 0);
			}
		}

		/// <summary>
		/// Removes a view from a vertical chain.
		/// This assumes the view is connected to a vertical chain.
		/// Its behaviour is undefined if not part of a vertical chain.
		/// </summary>
		/// <param name="viewId"> the view to be removed </param>
		public virtual void removeFromVerticalChain(int viewId)
		{
			if (mConstraints.ContainsKey(viewId))
			{
				Constraint constraint = mConstraints[viewId];
				if (constraint == null)
				{
					return;
				}
				int topId = constraint.layout.topToBottom;
				int bottomId = constraint.layout.bottomToTop;
				if (topId != Layout.UNSET || bottomId != Layout.UNSET)
				{
					if (topId != Layout.UNSET && bottomId != Layout.UNSET)
					{
						// top and bottom connected to views
						connect(topId, BOTTOM, bottomId, TOP, 0);
						connect(bottomId, TOP, topId, BOTTOM, 0);
					}
					else if (constraint.layout.bottomToBottom != Layout.UNSET)
					{
						// top connected to view. Bottom connected to parent
						connect(topId, BOTTOM, constraint.layout.bottomToBottom, BOTTOM, 0);
					}
					else if (constraint.layout.topToTop != Layout.UNSET)
					{
						// bottom connected to view. Top connected to parent
						connect(bottomId, TOP, constraint.layout.topToTop, TOP, 0);
					}
				}
			}
			clear(viewId, TOP);
			clear(viewId, BOTTOM);
		}

		/// <summary>
		/// Removes a view from a horizontal chain.
		/// This assumes the view is connected to a horizontal chain.
		/// Its behaviour is undefined if not part of a horizontal chain.
		/// </summary>
		/// <param name="viewId"> the view to be removed </param>
		public virtual void removeFromHorizontalChain(int viewId)
		{
			if (mConstraints.ContainsKey(viewId))
			{
				Constraint constraint = mConstraints[viewId];
				if (constraint == null)
				{
					return;
				}
				int leftId = constraint.layout.leftToRight;
				int rightId = constraint.layout.rightToLeft;
				if (leftId != Layout.UNSET || rightId != Layout.UNSET)
				{
					if (leftId != Layout.UNSET && rightId != Layout.UNSET)
					{
						// left and right connected to views
						connect(leftId, RIGHT, rightId, LEFT, 0);
						connect(rightId, LEFT, leftId, RIGHT, 0);
					}
					else if (constraint.layout.rightToRight != Layout.UNSET)
					{
						// left connected to view. right connected to parent
						connect(leftId, RIGHT, constraint.layout.rightToRight, RIGHT, 0);
					}
					else if (constraint.layout.leftToLeft != Layout.UNSET)
					{
						// right connected to view. left connected to parent
						connect(rightId, LEFT, constraint.layout.leftToLeft, LEFT, 0);
					}
					clear(viewId, LEFT);
					clear(viewId, RIGHT);
				}
				else
				{

					int startId = constraint.layout.startToEnd;
					int endId = constraint.layout.endToStart;
					if (startId != Layout.UNSET || endId != Layout.UNSET)
					{
						if (startId != Layout.UNSET && endId != Layout.UNSET)
						{
							// start and end connected to views
							connect(startId, END, endId, START, 0);
							connect(endId, START, leftId, END, 0);
						}
						else if (endId != Layout.UNSET)
						{
							if (constraint.layout.rightToRight != Layout.UNSET)
							{
								// left connected to view. right connected to parent
								connect(leftId, END, constraint.layout.rightToRight, END, 0);
							}
							else if (constraint.layout.leftToLeft != Layout.UNSET)
							{
								// right connected to view. left connected to parent
								connect(endId, START, constraint.layout.leftToLeft, START, 0);
							}
						}
					}
					clear(viewId, START);
					clear(viewId, END);
				}
			}
		}

		/// <summary>
		/// Creates a ConstraintLayout virtual object. Currently only horizontal or vertical GuideLines.
		/// </summary>
		/// <param name="guidelineID"> ID of guideline to create </param>
		/// <param name="orientation"> the Orientation of the guideline </param>
		public virtual void create(int guidelineID, int orientation)
		{
			Constraint constraint = get(guidelineID);
			constraint.layout.mIsGuideline = true;
			constraint.layout.orientation = orientation;
		}

		/// <summary>
		/// Creates a ConstraintLayout Barrier object.
		/// </summary>
		/// <param name="id"> </param>
		/// <param name="direction">  Barrier.{LEFT,RIGHT,TOP,BOTTOM,START,END} </param>
		/// <param name="referenced">
		/// @since 1.1 </param>
		public virtual void createBarrier(int id, int direction, int margin, params int[] referenced)
		{
			Constraint constraint = get(id);
			constraint.layout.mHelperType = BARRIER_TYPE;
			constraint.layout.mBarrierDirection = direction;
			constraint.layout.mBarrierMargin = margin;
			constraint.layout.mIsGuideline = false;
			constraint.layout.mReferenceIds = referenced;
		}

		/// <summary>
		/// Set the guideline's distance form the top or left edge.
		/// </summary>
		/// <param name="guidelineID"> ID of the guideline </param>
		/// <param name="margin">      the distance to the top or left edge </param>
		public virtual void setGuidelineBegin(int guidelineID, int margin)
		{
			get(guidelineID).layout.guideBegin = margin;
			get(guidelineID).layout.guideEnd = Layout.UNSET;
			get(guidelineID).layout.guidePercent = Layout.UNSET;

		}

		/// <summary>
		/// Set a guideline's distance to end.
		/// </summary>
		/// <param name="guidelineID"> ID of the guideline </param>
		/// <param name="margin">      the margin to the right or bottom side of container </param>
		public virtual void setGuidelineEnd(int guidelineID, int margin)
		{
			get(guidelineID).layout.guideEnd = margin;
			get(guidelineID).layout.guideBegin = Layout.UNSET;
			get(guidelineID).layout.guidePercent = Layout.UNSET;
		}

		/// <summary>
		/// Set a Guideline's percent.
		/// </summary>
		/// <param name="guidelineID"> ID of the guideline </param>
		/// <param name="ratio">       the ratio between the gap on the left and right 0.0 is top/left 0.5 is middle </param>
		public virtual void setGuidelinePercent(int guidelineID, float ratio)
		{
			get(guidelineID).layout.guidePercent = ratio;
			get(guidelineID).layout.guideEnd = Layout.UNSET;
			get(guidelineID).layout.guideBegin = Layout.UNSET;
		}

		/// <summary>
		/// get the reference id's of a helper.
		/// </summary>
		/// <param name="id"> </param>
		/// <returns> array of id's </returns>
		public virtual int[] getReferencedIds(int id)
		{
			Constraint constraint = get(id);
			if (constraint.layout.mReferenceIds == null)
			{
				return new int[0];
			}
			return Arrays.copyOf(constraint.layout.mReferenceIds, constraint.layout.mReferenceIds.Length);
		}

		/// <summary>
		/// sets the reference id's of a barrier.
		/// </summary>
		/// <param name="id"> </param>
		/// <param name="referenced">
		/// @since 2.0 </param>
		public virtual void setReferencedIds(int id, params int[] referenced)
		{
			Constraint constraint = get(id);
			constraint.layout.mReferenceIds = referenced;
		}

		public virtual void setBarrierType(int id, int type)
		{
			Constraint constraint = get(id);
			constraint.layout.mHelperType = type;
		}

		public virtual void removeAttribute(string attributeName)
		{
			mSavedAttributes.Remove(attributeName);
		}

		public virtual void setIntValue(int viewId, string attributeName, int value)
		{
			get(viewId).setIntValue(attributeName, value);
		}

		public virtual void setColorValue(int viewId, string attributeName, int value)
		{
			get(viewId).setColorValue(attributeName, value);
		}

		public virtual void setFloatValue(int viewId, string attributeName, float value)
		{
			get(viewId).setFloatValue(attributeName, value);
		}

		public virtual void setStringValue(int viewId, string attributeName, string value)
		{
			get(viewId).setStringValue(attributeName, value);
		}

		private void addAttributes(AttributeType attributeType, params string[] attributeName)
		{
			ConstraintAttribute constraintAttribute = null;
			for (int i = 0; i < attributeName.Length; i++)
			{
				if (mSavedAttributes.ContainsKey(attributeName[i]))
				{
					constraintAttribute = mSavedAttributes[attributeName[i]];
					if (constraintAttribute == null)
					{
						continue;
					}
					if (constraintAttribute.Type != attributeType)
					{
						throw new System.ArgumentException("ConstraintAttribute is already a " + constraintAttribute.Type.name());
					}
				}
				else
				{
					constraintAttribute = new ConstraintAttribute(attributeName[i], attributeType);
					mSavedAttributes[attributeName[i]] = constraintAttribute;
				}
			}
		}

		public virtual void parseIntAttributes(Constraint set, string attributes)
		{
			string[] sp = attributes.Split(",", true);
			for (int i = 0; i < sp.Length; i++)
			{
				string[] attr = sp[i].Split("=", true);
				if (attr.Length != 2)
				{
					Log.w(TAG, " Unable to parse " + sp[i]);
				}
				else
				{
					set.setFloatValue(attr[0], Integer.decode(attr[1]));
				}
			}
		}

		public virtual void parseColorAttributes(Constraint set, string attributes)
		{
			string[] sp = attributes.Split(",", true);
			for (int i = 0; i < sp.Length; i++)
			{
				string[] attr = sp[i].Split("=", true);
				if (attr.Length != 2)
				{
					Log.w(TAG, " Unable to parse " + sp[i]);
				}
				else
				{
					set.setColorValue(attr[0], Color.parseColor(attr[1]));
				}
			}
		}

		public virtual void parseFloatAttributes(Constraint set, string attributes)
		{
			string[] sp = attributes.Split(",", true);
			for (int i = 0; i < sp.Length; i++)
			{
				string[] attr = sp[i].Split("=", true);
				if (attr.Length != 2)
				{
					Log.w(TAG, " Unable to parse " + sp[i]);
				}
				else
				{
					set.setFloatValue(attr[0], float.Parse(attr[1]));
				}
			}
		}

		public virtual void parseStringAttributes(Constraint set, string attributes)
		{
			string[] sp = splitString(attributes);
			for (int i = 0; i < sp.Length; i++)
			{
				string[] attr = sp[i].Split("=", true);
				Log.w(TAG, " Unable to parse " + sp[i]);
				set.setStringValue(attr[0], attr[1]);
			}
		}

		private static string[] splitString(string str)
		{
			char[] chars = str.ToCharArray();
			List<string> list = new List<string>();
			bool inDouble = false;
			int start = 0;
			for (int i = 0; i < chars.Length; i++)
			{
				if (chars[i] == ',' && !inDouble)
				{
					list.Add(new string(chars, start, i - start));
					start = i + 1;
				}
				else if (chars[i] == '"')
				{
					inDouble = !inDouble;
				}
			}
			list.Add(new string(chars, start, chars.Length - start));
			return list.ToArray();
		}

		public virtual void addIntAttributes(params string[] attributeName)
		{
			addAttributes(AttributeType.INT_TYPE, attributeName);
		}

		public virtual void addColorAttributes(params string[] attributeName)
		{
			addAttributes(AttributeType.COLOR_TYPE, attributeName);
		}

		public virtual void addFloatAttributes(params string[] attributeName)
		{
			addAttributes(AttributeType.FLOAT_TYPE, attributeName);
		}

		public virtual void addStringAttributes(params string[] attributeName)
		{
			addAttributes(AttributeType.STRING_TYPE, attributeName);
		}

		private Constraint get(int id)
		{
			if (!mConstraints.ContainsKey(id))
			{
				mConstraints[id] = new Constraint();
			}
			return mConstraints[id];
		}

		private string sideToString(int side)
		{
			switch (side)
			{
				case LEFT:
					return "left";
				case RIGHT:
					return "right";
				case TOP:
					return "top";
				case BOTTOM:
					return "bottom";
				case BASELINE:
					return "baseline";
				case START:
					return "start";
				case END:
					return "end";
			}
			return "undefined";
		}

		/// <summary>
		/// Load a constraint set from a constraintSet.xml file.
		/// Note. Do NOT use this to load a layout file.
		/// It will fail silently as there is no efficient way to differentiate.
		/// </summary>
		/// <param name="context">    the context for the inflation </param>
		/// <param name="resourceId"> id of xml file in res/xml/ </param>
		public virtual void load(Context context, int resourceId)
		{
			Resources res = context.Resources;
			XmlPullParser parser = res.getXml(resourceId);
			string document = null;
			string tagName = null;
			try
			{

				for (int eventType = parser.EventType; eventType != XmlResourceParser.END_DOCUMENT; eventType = parser.next())
				{
					switch (eventType)
					{
						case XmlResourceParser.START_DOCUMENT:
							document = parser.Name;
							break;
						case XmlResourceParser.START_TAG:
							tagName = parser.Name;
							Constraint constraint = fillFromAttributeList(context, Xml.asAttributeSet(parser), false);
							if (tagName.Equals("Guideline", StringComparison.CurrentCultureIgnoreCase))
							{
								constraint.layout.mIsGuideline = true;
							}
							if (DEBUG)
							{
								Log.v(TAG, Debug.Loc + " cache " + Debug.getName(context, constraint.mViewId) + " " + constraint.mViewId);
							}
							mConstraints[constraint.mViewId] = constraint;
							break;
						case XmlResourceParser.END_TAG:
							tagName = null;
							break;
						case XmlResourceParser.TEXT:
							break;
					}
				}
			}
			catch (XmlPullParserException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		/// <summary>
		/// Load a constraint set from a constraintSet.xml file
		/// </summary>
		/// <param name="context"> the context for the inflation </param>
		/// <param name="parser">  id of xml file in res/xml/ </param>
		public virtual void load(Context context, XmlPullParser parser)
		{
			string tagName = null;
			try
			{
				Constraint constraint = null;
				for (int eventType = parser.EventType; eventType != XmlResourceParser.END_DOCUMENT; eventType = parser.next())
				{
					switch (eventType)
					{
						case XmlResourceParser.START_DOCUMENT:
							string document = parser.Name;

							break;
						case XmlResourceParser.START_TAG:
							tagName = parser.Name;
							if (DEBUG)
							{
								Log.v(TAG, Debug.Loc + " view .... tagName=" + tagName);
							}
							switch (tagName)
							{
								case "Constraint":
									constraint = fillFromAttributeList(context, Xml.asAttributeSet(parser), false);
									break;
								case "ConstraintOverride":
									constraint = fillFromAttributeList(context, Xml.asAttributeSet(parser), true);
									break;
								case "Guideline":
									constraint = fillFromAttributeList(context, Xml.asAttributeSet(parser), false);
									constraint.layout.mIsGuideline = true;
									constraint.layout.mApply = true;
									break;
								case "Barrier":
									constraint = fillFromAttributeList(context, Xml.asAttributeSet(parser), false);
									constraint.layout.mHelperType = BARRIER_TYPE;
									break;
								case "PropertySet":
									if (constraint == null)
									{
										throw new Exception(ERROR_MESSAGE + parser.LineNumber);
									}
									constraint.propertySet.fillFromAttributeList(context, Xml.asAttributeSet(parser));
									break;
								case "Transform":
									if (constraint == null)
									{
										throw new Exception(ERROR_MESSAGE + parser.LineNumber);
									}
									constraint.transform.fillFromAttributeList(context, Xml.asAttributeSet(parser));
									break;
								case "Layout":
									if (constraint == null)
									{
										throw new Exception(ERROR_MESSAGE + parser.LineNumber);
									}
									constraint.layout.fillFromAttributeList(context, Xml.asAttributeSet(parser));
									break;
								case "Motion":
									if (constraint == null)
									{
										throw new Exception(ERROR_MESSAGE + parser.LineNumber);
									}
									constraint.motion.fillFromAttributeList(context, Xml.asAttributeSet(parser));
									break;
								case "CustomAttribute":
								case "CustomMethod":
									if (constraint == null)
									{
										throw new Exception(ERROR_MESSAGE + parser.LineNumber);
									}
									ConstraintAttribute.parse(context, parser, constraint.mCustomConstraints);
									break;
							}
	//                        if (tagName.equalsIgnoreCase("Constraint")) {
	//                            constraint = fillFromAttributeList(context, Xml.asAttributeSet(parser));
	//                        } else if (tagName.equalsIgnoreCase("Guideline")) {
	//                            constraint = fillFromAttributeList(context, Xml.asAttributeSet(parser));
	//                            constraint.layout.mIsGuideline = true;
	//                        } else if (tagName.equalsIgnoreCase("CustomAttribute")) {
	//                            ConstraintAttribute.parse(context, parser, constraint.mCustomConstraints);
	//                        }
							break;
						case XmlResourceParser.END_TAG:
							tagName = parser.Name;
							switch (tagName.ToLowerInvariant())
							{
								case "constraintset":
									return;
								case "constraint":
								case "constraintoverride":
								case "guideline":
									mConstraints[constraint.mViewId] = constraint;
									constraint = null;
								break;
							}
							tagName = null;
							break;
						case XmlResourceParser.TEXT:
							break;
					}
				}
			}
			catch (XmlPullParserException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		private static int lookupID(TypedArray a, int index, int def)
		{
			int ret = a.getResourceId(index, def);
			if (ret == Layout.UNSET)
			{
				ret = a.getInt(index, Layout.UNSET);
			}
			return ret;
		}

		private Constraint fillFromAttributeList(Context context, AttributeSet attrs, bool @override)
		{
			Constraint c = new Constraint();
			TypedArray a = context.obtainStyledAttributes(attrs, @override ? R.styleable.ConstraintOverride : R.styleable.Constraint);
			populateConstraint(context, c, a, @override);
			a.recycle();
			return c;
		}

		/// <summary>
		/// Used to read a ConstraintDelta
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="parser">
		/// @return </param>
		public static Constraint buildDelta(Context context, XmlPullParser parser)
		{
			AttributeSet attrs = Xml.asAttributeSet(parser);
			Constraint c = new Constraint();
			TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.ConstraintOverride);
			populateOverride(context, c, a);
			a.recycle();
			return c;
		}

		private static void populateOverride(Context ctx, Constraint c, TypedArray a)
		{

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
			int N = a.IndexCount;
			TypedValue type;
			Constraint.Delta delta = new Constraint.Delta();
			c.mDelta = delta;
			c.motion.mApply = false;
			c.layout.mApply = false;
			c.propertySet.mApply = false;
			c.transform.mApply = false;
			for (int i = 0; i < N; i++)
			{
				int attr = a.getIndex(i);


				int attrType = overrideMapToConstant.get(attr);
				if (DEBUG)
				{
					Log.v(TAG, Debug.Loc + " > " + attrType + " " + getDebugName(attrType));
				}

				switch (attrType)
				{

					case EDITOR_ABSOLUTE_X:
						delta.add(EDITOR_ABSOLUTE_X, a.getDimensionPixelOffset(attr, c.layout.editorAbsoluteX));
						break;
					case EDITOR_ABSOLUTE_Y:
						delta.add(EDITOR_ABSOLUTE_Y, a.getDimensionPixelOffset(attr, c.layout.editorAbsoluteY));
						break;
					case GUIDE_BEGIN:
						delta.add(GUIDE_BEGIN, a.getDimensionPixelOffset(attr, c.layout.guideBegin));
						break;
					case GUIDE_END:
						delta.add(GUIDE_END, a.getDimensionPixelOffset(attr, c.layout.guideEnd));
						break;
					case GUIDE_PERCENT:
						delta.add(GUIDE_PERCENT, a.getFloat(attr, c.layout.guidePercent));
						break;
					case ORIENTATION:
						delta.add(ORIENTATION, a.getInt(attr, c.layout.orientation));
						break;
					case CIRCLE_RADIUS:
						delta.add(CIRCLE_RADIUS, a.getDimensionPixelSize(attr, c.layout.circleRadius));
						break;
					case CIRCLE_ANGLE:
						delta.add(CIRCLE_ANGLE, a.getFloat(attr, c.layout.circleAngle));
						break;
					case GONE_LEFT_MARGIN:
						delta.add(GONE_LEFT_MARGIN, a.getDimensionPixelSize(attr, c.layout.goneLeftMargin));
						break;
					case GONE_TOP_MARGIN:
						delta.add(GONE_TOP_MARGIN, a.getDimensionPixelSize(attr, c.layout.goneTopMargin));
						break;
					case GONE_RIGHT_MARGIN:
						delta.add(GONE_RIGHT_MARGIN, a.getDimensionPixelSize(attr, c.layout.goneRightMargin));
						break;
					case GONE_BOTTOM_MARGIN:
						delta.add(GONE_BOTTOM_MARGIN, a.getDimensionPixelSize(attr, c.layout.goneBottomMargin));
						break;
					case GONE_START_MARGIN:
						delta.add(GONE_START_MARGIN, a.getDimensionPixelSize(attr, c.layout.goneStartMargin));
						break;
					case GONE_END_MARGIN:
						delta.add(GONE_END_MARGIN, a.getDimensionPixelSize(attr, c.layout.goneEndMargin));
						break;
					case GONE_BASELINE_MARGIN:
						delta.add(GONE_BASELINE_MARGIN, a.getDimensionPixelSize(attr, c.layout.goneBaselineMargin));
						break;
					case HORIZONTAL_BIAS:
						delta.add(HORIZONTAL_BIAS, a.getFloat(attr, c.layout.horizontalBias));
						break;
					case VERTICAL_BIAS:
						delta.add(VERTICAL_BIAS, a.getFloat(attr, c.layout.verticalBias));
						break;
					case LEFT_MARGIN:
						delta.add(LEFT_MARGIN, a.getDimensionPixelSize(attr, c.layout.leftMargin));
						break;
					case RIGHT_MARGIN:
						delta.add(RIGHT_MARGIN, a.getDimensionPixelSize(attr, c.layout.rightMargin));
						break;
					case START_MARGIN:
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
						{
							delta.add(START_MARGIN, a.getDimensionPixelSize(attr, c.layout.startMargin));
						}
						break;
					case END_MARGIN:
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
						{
							delta.add(END_MARGIN, a.getDimensionPixelSize(attr, c.layout.endMargin));
						}
						break;
					case TOP_MARGIN:
						delta.add(TOP_MARGIN, a.getDimensionPixelSize(attr, c.layout.topMargin));
						break;
					case BOTTOM_MARGIN:
						delta.add(BOTTOM_MARGIN, a.getDimensionPixelSize(attr, c.layout.bottomMargin));
						break;
					case BASELINE_MARGIN:
						delta.add(BASELINE_MARGIN, a.getDimensionPixelSize(attr, c.layout.baselineMargin));
						break;
					case LAYOUT_WIDTH:
						delta.add(LAYOUT_WIDTH, a.getLayoutDimension(attr, c.layout.mWidth));
						break;
					case LAYOUT_HEIGHT:
						delta.add(LAYOUT_HEIGHT, a.getLayoutDimension(attr, c.layout.mHeight));
						break;
					case LAYOUT_CONSTRAINT_WIDTH:
						ConstraintSet.parseDimensionConstraints(delta, a, attr, HORIZONTAL);
						break;
					case LAYOUT_CONSTRAINT_HEIGHT:
						ConstraintSet.parseDimensionConstraints(delta, a, attr, VERTICAL);
						break;
					case LAYOUT_WRAP_BEHAVIOR:
						delta.add(LAYOUT_WRAP_BEHAVIOR, a.getInt(attr, c.layout.mWrapBehavior));
						break;
					case WIDTH_DEFAULT:
						delta.add(WIDTH_DEFAULT, a.getInt(attr, c.layout.widthDefault));
						break;
					case HEIGHT_DEFAULT:
						delta.add(HEIGHT_DEFAULT, a.getInt(attr, c.layout.heightDefault));
						break;
					case HEIGHT_MAX:
						delta.add(HEIGHT_MAX, a.getDimensionPixelSize(attr, c.layout.heightMax));
						break;
					case WIDTH_MAX:
						delta.add(WIDTH_MAX, a.getDimensionPixelSize(attr, c.layout.widthMax));
						break;
					case HEIGHT_MIN:
						delta.add(HEIGHT_MIN, a.getDimensionPixelSize(attr, c.layout.heightMin));
						break;
					case WIDTH_MIN:
						delta.add(WIDTH_MIN, a.getDimensionPixelSize(attr, c.layout.widthMin));
						break;
					case CONSTRAINED_WIDTH:
						delta.add(CONSTRAINED_WIDTH, a.getBoolean(attr, c.layout.constrainedWidth));
						break;
					case CONSTRAINED_HEIGHT:
						delta.add(CONSTRAINED_HEIGHT, a.getBoolean(attr, c.layout.constrainedHeight));
						break;
					case LAYOUT_VISIBILITY:
						delta.add(LAYOUT_VISIBILITY, VISIBILITY_FLAGS[a.getInt(attr, c.propertySet.visibility)]);
						break;
					case VISIBILITY_MODE:
						delta.add(VISIBILITY_MODE, a.getInt(attr, c.propertySet.mVisibilityMode));
						break;
					case ALPHA:
						delta.add(ALPHA, a.getFloat(attr, c.propertySet.alpha));
						break;
					case ELEVATION:
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
						{
							delta.add(ELEVATION, true);
							delta.add(ELEVATION, a.getDimension(attr, c.transform.elevation));
						}
						break;
					case ROTATION:
						delta.add(ROTATION, a.getFloat(attr, c.transform.rotation));
						break;
					case ROTATION_X:
						delta.add(ROTATION_X, a.getFloat(attr, c.transform.rotationX));
						break;
					case ROTATION_Y:
						delta.add(ROTATION_Y, a.getFloat(attr, c.transform.rotationY));
						break;
					case SCALE_X:
						delta.add(SCALE_X, a.getFloat(attr, c.transform.scaleX));
						break;
					case SCALE_Y:
						delta.add(SCALE_Y, a.getFloat(attr, c.transform.scaleY));
						break;
					case TRANSFORM_PIVOT_X:
						delta.add(TRANSFORM_PIVOT_X, a.getDimension(attr, c.transform.transformPivotX));
						break;
					case TRANSFORM_PIVOT_Y:
						delta.add(TRANSFORM_PIVOT_Y, a.getDimension(attr, c.transform.transformPivotY));
						break;
					case TRANSLATION_X:
						delta.add(TRANSLATION_X, a.getDimension(attr, c.transform.translationX));
						break;
					case TRANSLATION_Y:
						delta.add(TRANSLATION_Y, a.getDimension(attr, c.transform.translationY));
						break;
					case TRANSLATION_Z:
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
						{
							delta.add(TRANSLATION_Z, a.getDimension(attr, c.transform.translationZ));
						}
						break;
					case TRANSFORM_PIVOT_TARGET:
						delta.add(TRANSFORM_PIVOT_TARGET, lookupID(a, attr, c.transform.transformPivotTarget));
						break;
					case VERTICAL_WEIGHT:
						delta.add(VERTICAL_WEIGHT, a.getFloat(attr, c.layout.verticalWeight));
						break;
					case HORIZONTAL_WEIGHT:
						delta.add(HORIZONTAL_WEIGHT, a.getFloat(attr, c.layout.horizontalWeight));
						break;
					case VERTICAL_STYLE:
						delta.add(VERTICAL_STYLE, a.getInt(attr, c.layout.verticalChainStyle));
						break;
					case HORIZONTAL_STYLE:
						delta.add(HORIZONTAL_STYLE, a.getInt(attr, c.layout.horizontalChainStyle));
						break;
					case VIEW_ID:
						c.mViewId = a.getResourceId(attr, c.mViewId);
						delta.add(VIEW_ID, c.mViewId);
						break;
					case MOTION_TARGET:
						if (MotionLayout.IS_IN_EDIT_MODE)
						{
							c.mViewId = a.getResourceId(attr, c.mViewId);
							if (c.mViewId == -1)
							{
								c.mTargetString = a.getString(attr);
							}
						}
						else
						{
							if (a.peekValue(attr).type == TypedValue.TYPE_STRING)
							{
								c.mTargetString = a.getString(attr);
							}
							else
							{
								c.mViewId = a.getResourceId(attr, c.mViewId);
							}
						}
						break;
					case DIMENSION_RATIO:
						delta.add(DIMENSION_RATIO, a.getString(attr));
						break;
					case WIDTH_PERCENT:
						delta.add(WIDTH_PERCENT, a.getFloat(attr, 1));
						break;
					case HEIGHT_PERCENT:
						delta.add(HEIGHT_PERCENT, a.getFloat(attr, 1));
						break;
					case PROGRESS:
						delta.add(PROGRESS, a.getFloat(attr, c.propertySet.mProgress));
						break;
					case ANIMATE_RELATIVE_TO:
						delta.add(ANIMATE_RELATIVE_TO, lookupID(a, attr, c.motion.mAnimateRelativeTo));
						break;
					case ANIMATE_CIRCLE_ANGLE_TO:
						delta.add(ANIMATE_CIRCLE_ANGLE_TO, a.getInteger(attr, c.motion.mAnimateCircleAngleTo));
						break;
					case TRANSITION_EASING:
						type = a.peekValue(attr);
						if (type.type == TypedValue.TYPE_STRING)
						{
							delta.add(TRANSITION_EASING, a.getString(attr));
						}
						else
						{
							delta.add(TRANSITION_EASING, Easing.NAMED_EASING[a.getInteger(attr, 0)]);
						}
						break;
					case PATH_MOTION_ARC:
						delta.add(PATH_MOTION_ARC, a.getInt(attr, c.motion.mPathMotionArc));
						break;
					case TRANSITION_PATH_ROTATE:
						delta.add(TRANSITION_PATH_ROTATE, a.getFloat(attr, c.motion.mPathRotate));
						break;
					case MOTION_STAGGER:
						delta.add(MOTION_STAGGER, a.getFloat(attr, c.motion.mMotionStagger));
						break;

					case QUANTIZE_MOTION_STEPS:
						delta.add(QUANTIZE_MOTION_STEPS, a.getInteger(attr, c.motion.mQuantizeMotionSteps));
						break;
					case QUANTIZE_MOTION_PHASE:
						delta.add(QUANTIZE_MOTION_PHASE, a.getFloat(attr, c.motion.mQuantizeMotionPhase));
						break;
					case QUANTIZE_MOTION_INTERPOLATOR:
						type = a.peekValue(attr);
						if (type.type == TypedValue.TYPE_REFERENCE)
						{
							c.motion.mQuantizeInterpolatorID = a.getResourceId(attr, -1);
							delta.add(QUANTIZE_MOTION_INTERPOLATOR_ID, c.motion.mQuantizeInterpolatorID);
							if (c.motion.mQuantizeInterpolatorID != -1)
							{
								c.motion.mQuantizeInterpolatorType = Motion.INTERPOLATOR_REFERENCE_ID;
								delta.add(QUANTIZE_MOTION_INTERPOLATOR_TYPE, c.motion.mQuantizeInterpolatorType);
							}
						}
						else if (type.type == TypedValue.TYPE_STRING)
						{
							c.motion.mQuantizeInterpolatorString = a.getString(attr);
							delta.add(QUANTIZE_MOTION_INTERPOLATOR_STR, c.motion.mQuantizeInterpolatorString);

							if (c.motion.mQuantizeInterpolatorString.IndexOf("/", StringComparison.Ordinal) > 0)
							{
								c.motion.mQuantizeInterpolatorID = a.getResourceId(attr, -1);
								delta.add(QUANTIZE_MOTION_INTERPOLATOR_ID, c.motion.mQuantizeInterpolatorID);

								c.motion.mQuantizeInterpolatorType = Motion.INTERPOLATOR_REFERENCE_ID;
								delta.add(QUANTIZE_MOTION_INTERPOLATOR_TYPE, c.motion.mQuantizeInterpolatorType);

							}
							else
							{
								c.motion.mQuantizeInterpolatorType = Motion.SPLINE_STRING;
								delta.add(QUANTIZE_MOTION_INTERPOLATOR_TYPE, c.motion.mQuantizeInterpolatorType);
							}
						}
						else
						{
							c.motion.mQuantizeInterpolatorType = a.getInteger(attr, c.motion.mQuantizeInterpolatorID);
							delta.add(QUANTIZE_MOTION_INTERPOLATOR_TYPE, c.motion.mQuantizeInterpolatorType);
						}
						break;
					case DRAW_PATH:
						delta.add(DRAW_PATH, a.getInt(attr, 0));
						break;
					case CHAIN_USE_RTL:
						Log.e(TAG, "CURRENTLY UNSUPPORTED"); // TODO add support or remove
						//  TODO add support or remove  c.mChainUseRtl = a.getBoolean(attr,c.mChainUseRtl);
						break;
					case BARRIER_DIRECTION:
						delta.add(BARRIER_DIRECTION, a.getInt(attr, c.layout.mBarrierDirection));
						break;
					case BARRIER_MARGIN:
						delta.add(BARRIER_MARGIN, a.getDimensionPixelSize(attr, c.layout.mBarrierMargin));
						break;
					case CONSTRAINT_REFERENCED_IDS:
						delta.add(CONSTRAINT_REFERENCED_IDS, a.getString(attr));
						break;
					case CONSTRAINT_TAG:
						delta.add(CONSTRAINT_TAG, a.getString(attr));
						break;
					case BARRIER_ALLOWS_GONE_WIDGETS:
						delta.add(BARRIER_ALLOWS_GONE_WIDGETS, a.getBoolean(attr, c.layout.mBarrierAllowsGoneWidgets));
						break;
					case UNUSED:
						Log.w(TAG, "unused attribute 0x" + attr.ToString("x") + "   " + mapToConstant.get(attr));
						break;
					default:
						Log.w(TAG, "Unknown attribute 0x" + attr.ToString("x") + "   " + mapToConstant.get(attr));
					break;
				}
			}
		}

		private static void setDeltaValue(Constraint c, int type, float value)
		{
			switch (type)
			{
				case GUIDE_PERCENT:
					c.layout.guidePercent = value;
					break;
				case CIRCLE_ANGLE:
					c.layout.circleAngle = value;
					break;
				case HORIZONTAL_BIAS:
					c.layout.horizontalBias = value;
					break;
				case VERTICAL_BIAS:
					c.layout.verticalBias = value;
					break;
				case ALPHA:
					c.propertySet.alpha = value;
					break;
				case ELEVATION:
					c.transform.elevation = value;
					c.transform.applyElevation = true;
					break;
				case ROTATION:
					c.transform.rotation = value;
					break;
				case ROTATION_X:
					c.transform.rotationX = value;
					break;
				case ROTATION_Y:
					c.transform.rotationY = value;
					break;
				case SCALE_X:
					c.transform.scaleX = value;
					break;
				case SCALE_Y:
					c.transform.scaleY = value;
					break;
				case TRANSFORM_PIVOT_X:
					c.transform.transformPivotX = value;
					break;
				case TRANSFORM_PIVOT_Y:
					c.transform.transformPivotY = value;
					break;
				case TRANSLATION_X:
					c.transform.translationX = value;
					break;
				case TRANSLATION_Y:
					c.transform.translationY = value;
					break;
				case TRANSLATION_Z:
					c.transform.translationZ = value;
					break;
				case VERTICAL_WEIGHT:
					c.layout.verticalWeight = value;
					break;
				case HORIZONTAL_WEIGHT:
					c.layout.horizontalWeight = value;
					break;
				case WIDTH_PERCENT:
					c.layout.widthPercent = value;
					break;
				case HEIGHT_PERCENT:
					c.layout.heightPercent = value;
					break;
				case PROGRESS:
					c.propertySet.mProgress = value;
					break;
				case TRANSITION_PATH_ROTATE:
					c.motion.mPathRotate = value;
					break;
				case MOTION_STAGGER:
					c.motion.mMotionStagger = value;
					break;
				case QUANTIZE_MOTION_PHASE:
					c.motion.mQuantizeMotionPhase = value;
					break;
				case UNUSED:
					break;
				default:
					Log.w(TAG, "Unknown attribute 0x");
				break;
			}
		}

		private static void setDeltaValue(Constraint c, int type, int value)
		{
			switch (type)
			{
				case EDITOR_ABSOLUTE_X:
					c.layout.editorAbsoluteX = value;
					break;
				case EDITOR_ABSOLUTE_Y:
					c.layout.editorAbsoluteY = value;
					break;
				case LAYOUT_WRAP_BEHAVIOR:
					c.layout.mWrapBehavior = value;
					break;
				case GUIDE_BEGIN:
					c.layout.guideBegin = value;
					break;
				case GUIDE_END:
					c.layout.guideEnd = value;
					break;
				case ORIENTATION:
					c.layout.orientation = value;
					break;
				case CIRCLE:
					c.layout.circleConstraint = value;
					break;
				case CIRCLE_RADIUS:
					c.layout.circleRadius = value;
					break;
				case GONE_LEFT_MARGIN:
					c.layout.goneLeftMargin = value;
					break;
				case GONE_TOP_MARGIN:
					c.layout.goneTopMargin = value;
					break;
				case GONE_RIGHT_MARGIN:
					c.layout.goneRightMargin = value;
					break;
				case GONE_BOTTOM_MARGIN:
					c.layout.goneBottomMargin = value;
					break;
				case GONE_START_MARGIN:
					c.layout.goneStartMargin = value;
					break;
				case GONE_END_MARGIN:
					c.layout.goneEndMargin = value;
					break;
				case GONE_BASELINE_MARGIN:
					c.layout.goneBaselineMargin = value;
					break;
				case LEFT_MARGIN:
					c.layout.leftMargin = value;
					break;
				case RIGHT_MARGIN:
					c.layout.rightMargin = value;
					break;
				case START_MARGIN:
					c.layout.startMargin = value;
					break;
				case END_MARGIN:
					c.layout.endMargin = value;
					break;
				case TOP_MARGIN:
					c.layout.topMargin = value;
					break;
				case BOTTOM_MARGIN:
					c.layout.bottomMargin = value;
					break;
				case BASELINE_MARGIN:
					c.layout.baselineMargin = value;
					break;
				case LAYOUT_WIDTH:
					c.layout.mWidth = value;
					break;
				case LAYOUT_HEIGHT:
					c.layout.mHeight = value;
					break;
				case WIDTH_DEFAULT:
					c.layout.widthDefault = value;
					break;
				case HEIGHT_DEFAULT:
					c.layout.heightDefault = value;
					break;
				case HEIGHT_MAX:
					c.layout.heightMax = value;
					break;
				case WIDTH_MAX:
					c.layout.widthMax = value;
					break;
				case HEIGHT_MIN:
					c.layout.heightMin = value;
					break;
				case WIDTH_MIN:
					c.layout.widthMin = value;
					break;
				case LAYOUT_VISIBILITY:
					c.propertySet.visibility = value;
					break;
				case VISIBILITY_MODE:
					c.propertySet.mVisibilityMode = value;
					break;
				case TRANSFORM_PIVOT_TARGET:
					c.transform.transformPivotTarget = value;
					break;
				case VERTICAL_STYLE:
					c.layout.verticalChainStyle = value;
					break;
				case HORIZONTAL_STYLE:
					c.layout.horizontalChainStyle = value;
					break;
				case VIEW_ID:
					c.mViewId = value;
					break;
				case ANIMATE_RELATIVE_TO:
					c.motion.mAnimateRelativeTo = value;
					break;
				case ANIMATE_CIRCLE_ANGLE_TO:
					c.motion.mAnimateCircleAngleTo = value;
					break;
				case PATH_MOTION_ARC:
					c.motion.mPathMotionArc = value;
					break;
				case QUANTIZE_MOTION_STEPS:
					c.motion.mQuantizeMotionSteps = value;
					break;
				case QUANTIZE_MOTION_INTERPOLATOR_TYPE:
					c.motion.mQuantizeInterpolatorType = value;
					break;
				case QUANTIZE_MOTION_INTERPOLATOR_ID:
					c.motion.mQuantizeInterpolatorID = value;
					break;
				case DRAW_PATH:
					c.motion.mDrawPath = value;
					break;
				case BARRIER_DIRECTION:
					c.layout.mBarrierDirection = value;
					break;
				case BARRIER_MARGIN:
					c.layout.mBarrierMargin = value;
					break;
				case UNUSED:
					break;
				default:
					Log.w(TAG, "Unknown attribute 0x");
				break;
			}
		}

		private static void setDeltaValue(Constraint c, int type, string value)
		{
			switch (type)
			{
				case DIMENSION_RATIO:
					c.layout.dimensionRatio = value;
					break;
				case TRANSITION_EASING:
					c.motion.mTransitionEasing = value;
					break;
				case QUANTIZE_MOTION_INTERPOLATOR_STR:
					c.motion.mQuantizeInterpolatorString = value;
					break;
				case CONSTRAINT_REFERENCED_IDS:
					c.layout.mReferenceIdString = value;
					// If a string is defined, clear up the reference ids array
					c.layout.mReferenceIds = null;
					break;
				case CONSTRAINT_TAG:
					c.layout.mConstraintTag = value;
					break;
				case UNUSED:
					break;
				default:
					Log.w(TAG, "Unknown attribute 0x");
				break;
			}
		}

		private static void setDeltaValue(Constraint c, int type, bool value)
		{
			switch (type)
			{
				case CONSTRAINED_WIDTH:
					c.layout.constrainedWidth = value;
					break;
				case CONSTRAINED_HEIGHT:
					c.layout.constrainedHeight = value;
					break;
				case ELEVATION:
					c.transform.applyElevation = value;
					break;
				case BARRIER_ALLOWS_GONE_WIDGETS:
					c.layout.mBarrierAllowsGoneWidgets = value;
					break;
				case UNUSED:
					break;
				default:
					Log.w(TAG, "Unknown attribute 0x");
				break;
			}
		}

		private void populateConstraint(Context ctx, Constraint c, TypedArray a, bool @override)
		{
			if (@override)
			{
				populateOverride(ctx, c, a);
				return;
			}
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
			int N = a.IndexCount;
			for (int i = 0; i < N; i++)
			{
				int attr = a.getIndex(i);
				if (DEBUG)
				{ // USEFUL when adding features to track tags being parsed
					try
					{
						Field[] campos = typeof(R.styleable).GetFields();
						bool found = false;
						foreach (Field f in campos)
						{
							try
							{
								if (f.Type.Primitive && attr == f.getInt(null) && f.Name.contains("Constraint_"))
								{
									found = true;
									if (DEBUG)
									{
										Log.v(TAG, "L id " + f.Name + " #" + attr);
									}
									break;
								}
							}
							catch (Exception)
							{

							}
						}
						if (!found)
						{
							campos = typeof(Android.R.attr).GetFields();
							foreach (Field f in campos)
							{
								try
								{
									if (f.Type.Primitive && attr == f.getInt(null) && f.Name.contains("Constraint_"))
									{
										found = false;
										if (DEBUG)
										{
											Log.v(TAG, "x id " + f.Name);
										}
										break;
									}
								}
								catch (Exception)
								{

								}
							}
						}
						if (!found)
						{
							Log.v(TAG, " ? " + attr);
						}
					}
					catch (Exception e)
					{
						Log.v(TAG, " " + e.ToString());
					}
				}


				if (attr != R.styleable.Constraint_android_id && R.styleable.Constraint_android_layout_marginStart != attr && R.styleable.Constraint_android_layout_marginEnd != attr)
				{
					c.motion.mApply = true;
					c.layout.mApply = true;
					c.propertySet.mApply = true;
					c.transform.mApply = true;
				}

				switch (mapToConstant.get(attr))
				{
					case LEFT_TO_LEFT:
						c.layout.leftToLeft = lookupID(a, attr, c.layout.leftToLeft);
						break;
					case LEFT_TO_RIGHT:
						c.layout.leftToRight = lookupID(a, attr, c.layout.leftToRight);
						break;
					case RIGHT_TO_LEFT:
						c.layout.rightToLeft = lookupID(a, attr, c.layout.rightToLeft);
						break;
					case RIGHT_TO_RIGHT:
						c.layout.rightToRight = lookupID(a, attr, c.layout.rightToRight);
						break;
					case TOP_TO_TOP:
						c.layout.topToTop = lookupID(a, attr, c.layout.topToTop);
						break;
					case TOP_TO_BOTTOM:
						c.layout.topToBottom = lookupID(a, attr, c.layout.topToBottom);
						break;
					case BOTTOM_TO_TOP:
						c.layout.bottomToTop = lookupID(a, attr, c.layout.bottomToTop);
						break;
					case BOTTOM_TO_BOTTOM:
						c.layout.bottomToBottom = lookupID(a, attr, c.layout.bottomToBottom);
						break;
					case BASELINE_TO_BASELINE:
						c.layout.baselineToBaseline = lookupID(a, attr, c.layout.baselineToBaseline);
						break;
					case BASELINE_TO_TOP:
						c.layout.baselineToTop = lookupID(a, attr, c.layout.baselineToTop);
						break;
					case BASELINE_TO_BOTTOM:
						c.layout.baselineToBottom = lookupID(a, attr, c.layout.baselineToBottom);
						break;
					case EDITOR_ABSOLUTE_X:
						c.layout.editorAbsoluteX = a.getDimensionPixelOffset(attr, c.layout.editorAbsoluteX);
						break;
					case EDITOR_ABSOLUTE_Y:
						c.layout.editorAbsoluteY = a.getDimensionPixelOffset(attr, c.layout.editorAbsoluteY);
						break;
					case GUIDE_BEGIN:
						c.layout.guideBegin = a.getDimensionPixelOffset(attr, c.layout.guideBegin);
						break;
					case GUIDE_END:
						c.layout.guideEnd = a.getDimensionPixelOffset(attr, c.layout.guideEnd);
						break;
					case GUIDE_PERCENT:
						c.layout.guidePercent = a.getFloat(attr, c.layout.guidePercent);
						break;
					case ORIENTATION:
						c.layout.orientation = a.getInt(attr, c.layout.orientation);
						break;
					case START_TO_END:
						c.layout.startToEnd = lookupID(a, attr, c.layout.startToEnd);
						break;
					case START_TO_START:
						c.layout.startToStart = lookupID(a, attr, c.layout.startToStart);
						break;
					case END_TO_START:
						c.layout.endToStart = lookupID(a, attr, c.layout.endToStart);
						break;
					case END_TO_END:
						c.layout.endToEnd = lookupID(a, attr, c.layout.endToEnd);
						break;
					case CIRCLE:
						c.layout.circleConstraint = lookupID(a, attr, c.layout.circleConstraint);
						break;
					case CIRCLE_RADIUS:
						c.layout.circleRadius = a.getDimensionPixelSize(attr, c.layout.circleRadius);
						break;
					case CIRCLE_ANGLE:
						c.layout.circleAngle = a.getFloat(attr, c.layout.circleAngle);
						break;
					case GONE_LEFT_MARGIN:
						c.layout.goneLeftMargin = a.getDimensionPixelSize(attr, c.layout.goneLeftMargin);
						break;
					case GONE_TOP_MARGIN:
						c.layout.goneTopMargin = a.getDimensionPixelSize(attr, c.layout.goneTopMargin);
						break;
					case GONE_RIGHT_MARGIN:
						c.layout.goneRightMargin = a.getDimensionPixelSize(attr, c.layout.goneRightMargin);
						break;
					case GONE_BOTTOM_MARGIN:
						c.layout.goneBottomMargin = a.getDimensionPixelSize(attr, c.layout.goneBottomMargin);
						break;
					case GONE_START_MARGIN:
						c.layout.goneStartMargin = a.getDimensionPixelSize(attr, c.layout.goneStartMargin);
						break;
					case GONE_END_MARGIN:
						c.layout.goneEndMargin = a.getDimensionPixelSize(attr, c.layout.goneEndMargin);
						break;
					case GONE_BASELINE_MARGIN:
						c.layout.goneBaselineMargin = a.getDimensionPixelSize(attr, c.layout.goneBaselineMargin);
						break;
					case HORIZONTAL_BIAS:
						c.layout.horizontalBias = a.getFloat(attr, c.layout.horizontalBias);
						break;
					case VERTICAL_BIAS:
						c.layout.verticalBias = a.getFloat(attr, c.layout.verticalBias);
						break;
					case LEFT_MARGIN:
						c.layout.leftMargin = a.getDimensionPixelSize(attr, c.layout.leftMargin);
						break;
					case RIGHT_MARGIN:
						c.layout.rightMargin = a.getDimensionPixelSize(attr, c.layout.rightMargin);
						break;
					case START_MARGIN:
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
						{
							c.layout.startMargin = a.getDimensionPixelSize(attr, c.layout.startMargin);
						}
						break;
					case END_MARGIN:
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
						{
							c.layout.endMargin = a.getDimensionPixelSize(attr, c.layout.endMargin);
						}
						break;
					case TOP_MARGIN:
						c.layout.topMargin = a.getDimensionPixelSize(attr, c.layout.topMargin);
						break;
					case BOTTOM_MARGIN:
						c.layout.bottomMargin = a.getDimensionPixelSize(attr, c.layout.bottomMargin);
						break;
					case BASELINE_MARGIN:
						c.layout.baselineMargin = a.getDimensionPixelSize(attr, c.layout.baselineMargin);
						break;
					case LAYOUT_WIDTH:
						c.layout.mWidth = a.getLayoutDimension(attr, c.layout.mWidth);
						break;
					case LAYOUT_HEIGHT:
						c.layout.mHeight = a.getLayoutDimension(attr, c.layout.mHeight);
						break;
					case LAYOUT_CONSTRAINT_WIDTH:
						ConstraintSet.parseDimensionConstraints(c.layout, a, attr, HORIZONTAL);
						break;
					case LAYOUT_CONSTRAINT_HEIGHT:
						ConstraintSet.parseDimensionConstraints(c.layout, a, attr, VERTICAL);
						break;
					case LAYOUT_WRAP_BEHAVIOR:
						c.layout.mWrapBehavior = a.getInt(attr, c.layout.mWrapBehavior);
						break;
					case WIDTH_DEFAULT:
						c.layout.widthDefault = a.getInt(attr, c.layout.widthDefault);
						break;
					case HEIGHT_DEFAULT:
						c.layout.heightDefault = a.getInt(attr, c.layout.heightDefault);
						break;
					case HEIGHT_MAX:
						c.layout.heightMax = a.getDimensionPixelSize(attr, c.layout.heightMax);
						break;
					case WIDTH_MAX:
						c.layout.widthMax = a.getDimensionPixelSize(attr, c.layout.widthMax);
						break;
					case HEIGHT_MIN:
						c.layout.heightMin = a.getDimensionPixelSize(attr, c.layout.heightMin);
						break;
					case WIDTH_MIN:
						c.layout.widthMin = a.getDimensionPixelSize(attr, c.layout.widthMin);
						break;
					case CONSTRAINED_WIDTH:
						c.layout.constrainedWidth = a.getBoolean(attr, c.layout.constrainedWidth);
						break;
					case CONSTRAINED_HEIGHT:
						c.layout.constrainedHeight = a.getBoolean(attr, c.layout.constrainedHeight);
						break;
					case LAYOUT_VISIBILITY:
						c.propertySet.visibility = a.getInt(attr, c.propertySet.visibility);
						c.propertySet.visibility = VISIBILITY_FLAGS[c.propertySet.visibility];
						break;
					case VISIBILITY_MODE:
						c.propertySet.mVisibilityMode = a.getInt(attr, c.propertySet.mVisibilityMode);
						break;
					case ALPHA:
						c.propertySet.alpha = a.getFloat(attr, c.propertySet.alpha);
						break;
					case ELEVATION:
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
						{
							c.transform.applyElevation = true;
							c.transform.elevation = a.getDimension(attr, c.transform.elevation);
						}
						break;
					case ROTATION:
						c.transform.rotation = a.getFloat(attr, c.transform.rotation);
						break;
					case ROTATION_X:
						c.transform.rotationX = a.getFloat(attr, c.transform.rotationX);
						break;
					case ROTATION_Y:
						c.transform.rotationY = a.getFloat(attr, c.transform.rotationY);
						break;
					case SCALE_X:
						c.transform.scaleX = a.getFloat(attr, c.transform.scaleX);
						break;
					case SCALE_Y:
						c.transform.scaleY = a.getFloat(attr, c.transform.scaleY);
						break;
					case TRANSFORM_PIVOT_X:
						c.transform.transformPivotX = a.getDimension(attr, c.transform.transformPivotX);
						break;
					case TRANSFORM_PIVOT_Y:
						c.transform.transformPivotY = a.getDimension(attr, c.transform.transformPivotY);
						break;
					case TRANSLATION_X:
						c.transform.translationX = a.getDimension(attr, c.transform.translationX);
						break;
					case TRANSLATION_Y:
						c.transform.translationY = a.getDimension(attr, c.transform.translationY);
						break;
					case TRANSLATION_Z:
						if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
						{
							c.transform.translationZ = a.getDimension(attr, c.transform.translationZ);
						}
						break;
					case TRANSFORM_PIVOT_TARGET:
						c.transform.transformPivotTarget = lookupID(a, attr, c.transform.transformPivotTarget);
						break;
					case VERTICAL_WEIGHT:
						c.layout.verticalWeight = a.getFloat(attr, c.layout.verticalWeight);
						break;
					case HORIZONTAL_WEIGHT:
						c.layout.horizontalWeight = a.getFloat(attr, c.layout.horizontalWeight);
						break;
					case VERTICAL_STYLE:
						c.layout.verticalChainStyle = a.getInt(attr, c.layout.verticalChainStyle);
						break;
					case HORIZONTAL_STYLE:
						c.layout.horizontalChainStyle = a.getInt(attr, c.layout.horizontalChainStyle);
						break;
					case VIEW_ID:
						c.mViewId = a.getResourceId(attr, c.mViewId);
						break;
					case DIMENSION_RATIO:
						c.layout.dimensionRatio = a.getString(attr);
						break;
					case WIDTH_PERCENT:
						c.layout.widthPercent = a.getFloat(attr, 1);
						break;
					case HEIGHT_PERCENT:
						c.layout.heightPercent = a.getFloat(attr, 1);
						break;
					case PROGRESS:
						c.propertySet.mProgress = a.getFloat(attr, c.propertySet.mProgress);
						break;
					case ANIMATE_RELATIVE_TO:
						c.motion.mAnimateRelativeTo = lookupID(a, attr, c.motion.mAnimateRelativeTo);
						break;
					case ANIMATE_CIRCLE_ANGLE_TO:
						c.motion.mAnimateCircleAngleTo = a.getInteger(attr, c.motion.mAnimateCircleAngleTo);
						break;
					case TRANSITION_EASING:
						TypedValue type = a.peekValue(attr);
						if (type.type == TypedValue.TYPE_STRING)
						{
							c.motion.mTransitionEasing = a.getString(attr);
						}
						else
						{
							c.motion.mTransitionEasing = Easing.NAMED_EASING[a.getInteger(attr, 0)];
						}
						break;
					case PATH_MOTION_ARC:
						c.motion.mPathMotionArc = a.getInt(attr, c.motion.mPathMotionArc);
						break;
					case TRANSITION_PATH_ROTATE:
						c.motion.mPathRotate = a.getFloat(attr, c.motion.mPathRotate);
						break;
					case MOTION_STAGGER:
						c.motion.mMotionStagger = a.getFloat(attr, c.motion.mMotionStagger);
						break;

					case QUANTIZE_MOTION_STEPS:
						c.motion.mQuantizeMotionSteps = a.getInteger(attr, c.motion.mQuantizeMotionSteps);
						break;
					case QUANTIZE_MOTION_PHASE:
						c.motion.mQuantizeMotionPhase = a.getFloat(attr, c.motion.mQuantizeMotionPhase);
						break;
					case QUANTIZE_MOTION_INTERPOLATOR:
						type = a.peekValue(attr);

						if (type.type == TypedValue.TYPE_REFERENCE)
						{
							c.motion.mQuantizeInterpolatorID = a.getResourceId(attr, -1);
							if (c.motion.mQuantizeInterpolatorID != -1)
							{
								c.motion.mQuantizeInterpolatorType = Motion.INTERPOLATOR_REFERENCE_ID;
							}
						}
						else if (type.type == TypedValue.TYPE_STRING)
						{
							c.motion.mQuantizeInterpolatorString = a.getString(attr);
							if (c.motion.mQuantizeInterpolatorString.IndexOf("/", StringComparison.Ordinal) > 0)
							{
								c.motion.mQuantizeInterpolatorID = a.getResourceId(attr, -1);
								c.motion.mQuantizeInterpolatorType = Motion.INTERPOLATOR_REFERENCE_ID;
							}
							else
							{
								c.motion.mQuantizeInterpolatorType = Motion.SPLINE_STRING;
							}
						}
						else
						{
							c.motion.mQuantizeInterpolatorType = a.getInteger(attr, c.motion.mQuantizeInterpolatorID);
						}

						break;


					case DRAW_PATH:
						c.motion.mDrawPath = a.getInt(attr, 0);
						break;
					case CHAIN_USE_RTL:
						Log.e(TAG, "CURRENTLY UNSUPPORTED"); // TODO add support or remove
						//  TODO add support or remove  c.mChainUseRtl = a.getBoolean(attr,c.mChainUseRtl);
						break;
					case BARRIER_DIRECTION:
						c.layout.mBarrierDirection = a.getInt(attr, c.layout.mBarrierDirection);
						break;
					case BARRIER_MARGIN:
						c.layout.mBarrierMargin = a.getDimensionPixelSize(attr, c.layout.mBarrierMargin);
						break;
					case CONSTRAINT_REFERENCED_IDS:
						c.layout.mReferenceIdString = a.getString(attr);
						break;
					case CONSTRAINT_TAG:
						c.layout.mConstraintTag = a.getString(attr);
						break;
					case BARRIER_ALLOWS_GONE_WIDGETS:
						c.layout.mBarrierAllowsGoneWidgets = a.getBoolean(attr, c.layout.mBarrierAllowsGoneWidgets);
						break;
					case UNUSED:
						Log.w(TAG, "unused attribute 0x" + attr.ToString("x") + "   " + mapToConstant.get(attr));
						break;
					default:
						Log.w(TAG, "Unknown attribute 0x" + attr.ToString("x") + "   " + mapToConstant.get(attr));
					break;
				}
			}
			if (!string.ReferenceEquals(c.layout.mReferenceIdString, null))
			{
				// in case the strings are set, make sure to clear up the cached ids
				c.layout.mReferenceIds = null;
			};
		}

		private int[] convertReferenceString(View view, string referenceIdString)
		{
			string[] split = referenceIdString.Split(",", true);
			Context context = view.Context;
			int[] tags = new int[split.Length];
			int count = 0;
			for (int i = 0; i < split.Length; i++)
			{
				string idString = split[i];
				idString = idString.Trim();
				int tag = 0;
				try
				{
					Type res = typeof(R.id);
					Field field = res.GetField(idString);
					tag = field.getInt(null);
				}
				catch (Exception)
				{
					// Do nothing
				}
				if (tag == 0)
				{
					tag = context.Resources.getIdentifier(idString, "id", context.PackageName);
				}

				if (tag == 0 && view.InEditMode && view.Parent is ConstraintLayout)
				{
					ConstraintLayout constraintLayout = (ConstraintLayout) view.Parent;
					object value = constraintLayout.getDesignInformation(0, idString);
					if (value != null && value is int?)
					{
						tag = (int?) value.Value;
					}
				}
				tags[count++] = tag;
			}
			if (count != split.Length)
			{
				tags = Arrays.copyOf(tags, count);
			}
			return tags;
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public virtual Constraint getConstraint(int id)
		{
			if (mConstraints.ContainsKey(id))
			{
				return mConstraints[id];
			}
			return null;
		}

		/// <summary>
		/// @suppress
		/// </summary>
		public virtual int[] KnownIds
		{
			get
			{
				int?[] arr = mConstraints.Keys.toArray(new int?[0]);
				int[] array = new int[arr.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = arr[i].Value;
				}
				return array;
			}
		}

		/// <summary>
		/// Enforce id are required for all ConstraintLayout children to use ConstraintSet.
		/// default = true;
		/// </summary>
		public virtual bool ForceId
		{
			get
			{
				return mForceId;
			}
			set
			{
				this.mForceId = value;
			}
		}


		/// <summary>
		/// If true perform validation checks when parsing ConstraintSets
		/// This will slow down parsing and should only be used for debugging
		/// </summary>
		/// <param name="validate"> </param>
		public virtual bool ValidateOnParse
		{
			set
			{
				mValidate = value;
			}
		}

		/// <summary>
		/// Dump the contents
		/// </summary>
		/// <param name="scene"> </param>
		/// <param name="ids"> </param>
		public virtual void dump(MotionScene scene, params int[] ids)
		{
			ISet<int?> keys = mConstraints.Keys;
			HashSet<int?> set;
			if (ids.Length != 0)
			{
				set = new HashSet<int?>();
				foreach (int id in ids)
				{
					set.Add(id);
				}
			}
			else
			{
				set = new HashSet<>(keys);
			}
			Console.WriteLine(set.Count + " constraints");
			StringBuilder stringBuilder = new StringBuilder();

			foreach (int? id in set.toArray(new int?[0]))
			{
				Constraint constraint = mConstraints[id];
				if (constraint == null)
				{
					continue;
				}

				stringBuilder.Append("<Constraint id=");
				stringBuilder.Append(id);
				stringBuilder.Append(" \n");
				constraint.layout.dump(scene, stringBuilder);
				stringBuilder.Append("/>\n");
			}
			Console.WriteLine(stringBuilder.ToString());

		}

		/// <summary>
		/// Construct a user friendly error string
		/// </summary>
		/// <param name="context">    the context </param>
		/// <param name="resourceId"> the xml being parsed </param>
		/// <param name="pullParser"> the XML parser
		/// @return </param>
		internal static string getLine(Context context, int resourceId, XmlPullParser pullParser)
		{
			return ".(" + Debug.getName(context, resourceId) + ".xml:" + pullParser.LineNumber + ") \"" + pullParser.Name + "\"";
		}

		internal static string getDebugName(int v)
		{
			foreach (Field field in typeof(ConstraintSet).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
			{
				if (field.Name.contains("_") && field.Type == typeof(int) && java.lang.reflect.Modifier.isStatic(field.Modifiers) && java.lang.reflect.Modifier.isFinal(field.Modifiers))
				{
					int val = 0;
					try
					{
						val = field.getInt(null);
						if (val == v)
						{
							return field.Name;
						}
					}
					catch (IllegalAccessException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}

				}
			}
			return "UNKNOWN";
		}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeState(java.io.Writer writer, ConstraintLayout layout, int flags) throws java.io.IOException
		public virtual void writeState(Writer writer, ConstraintLayout layout, int flags)
		{
			writer.write("\n---------------------------------------------\n");
			if ((flags & 1) == 1)
			{
				(new WriteXmlEngine(this, writer, layout, flags)).writeLayout();
			}
			else
			{
				(new WriteJsonEngine(this, writer, layout, flags)).writeLayout();
			}
			writer.write("\n---------------------------------------------\n");

		}

		internal class WriteXmlEngine
		{
			private readonly ConstraintSet outerInstance;

			internal Writer writer;
			internal ConstraintLayout layout;
			internal Context context;
			internal int flags;
			internal int unknownCount = 0;
			internal readonly string LEFT = "'left'";
			internal readonly string RIGHT = "'right'";
			internal readonly string BASELINE = "'baseline'";
			internal readonly string BOTTOM = "'bottom'";
			internal readonly string TOP = "'top'";
			internal readonly string START = "'start'";
			internal readonly string END = "'end'";

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: WriteXmlEngine(java.io.Writer writer, ConstraintLayout layout, int flags) throws java.io.IOException
			internal WriteXmlEngine(ConstraintSet outerInstance, Writer writer, ConstraintLayout layout, int flags)
			{
				this.outerInstance = outerInstance;
				this.writer = writer;
				this.layout = layout;
				this.context = layout.Context;
				this.flags = flags;
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeLayout() throws java.io.IOException
			internal virtual void writeLayout()
			{
				writer.write("\n<ConstraintSet>\n");
				foreach (int? id in outerInstance.mConstraints.Keys)
				{
					Constraint c = outerInstance.mConstraints[id];
					string idName = getName(id.Value);
					writer.write("  <Constraint");
					writer.write(SPACE + "android:id" + "=\"" + idName + "\"");
					Layout l = c.layout;
					writeBaseDimension("android:layout_width", l.mWidth, -5); // nodefault
					writeBaseDimension("android:layout_height", l.mHeight, -5); // nodefault

					writeVariable("app:layout_constraintGuide_begin", l.guideBegin, UNSET);
					writeVariable("app:layout_constraintGuide_end", l.guideEnd, UNSET);
					writeVariable("app:layout_constraintGuide_percent", l.guidePercent, UNSET);

					writeVariable("app:layout_constraintHorizontal_bias", l.horizontalBias, 0.5f);
					writeVariable("app:layout_constraintVertical_bias", l.verticalBias, 0.5f);
					writeVariable("app:layout_constraintDimensionRatio", l.dimensionRatio, null);
					writeXmlConstraint("app:layout_constraintCircle", l.circleConstraint);
					writeVariable("app:layout_constraintCircleRadius", l.circleRadius, 0);
					writeVariable("app:layout_constraintCircleAngle", l.circleAngle, 0);

					writeVariable("android:orientation", l.orientation, UNSET);

					writeVariable("app:layout_constraintVertical_weight", l.verticalWeight, UNSET);
					writeVariable("app:layout_constraintHorizontal_weight", l.horizontalWeight, UNSET);
					writeVariable("app:layout_constraintHorizontal_chainStyle", l.horizontalChainStyle, CHAIN_SPREAD);
					writeVariable("app:layout_constraintVertical_chainStyle", l.verticalChainStyle, CHAIN_SPREAD);

					writeVariable("app:barrierDirection", l.mBarrierDirection, UNSET);
					writeVariable("app:barrierMargin", l.mBarrierMargin, 0);

					writeDimension("app:layout_marginLeft", l.leftMargin, 0);
					writeDimension("app:layout_goneMarginLeft", l.goneLeftMargin, Layout.UNSET_GONE_MARGIN);
					writeDimension("app:layout_marginRight", l.rightMargin, 0);
					writeDimension("app:layout_goneMarginRight", l.goneRightMargin, Layout.UNSET_GONE_MARGIN);
					writeDimension("app:layout_marginStart", l.startMargin, 0);
					writeDimension("app:layout_goneMarginStart", l.goneStartMargin, Layout.UNSET_GONE_MARGIN);
					writeDimension("app:layout_marginEnd", l.endMargin, 0);
					writeDimension("app:layout_goneMarginEnd", l.goneEndMargin, Layout.UNSET_GONE_MARGIN);
					writeDimension("app:layout_marginTop", l.topMargin, 0);
					writeDimension("app:layout_goneMarginTop", l.goneTopMargin, Layout.UNSET_GONE_MARGIN);
					writeDimension("app:layout_marginBottom", l.bottomMargin, 0);
					writeDimension("app:layout_goneMarginBottom", l.goneBottomMargin, Layout.UNSET_GONE_MARGIN);
					writeDimension("app:goneBaselineMargin", l.goneBaselineMargin, Layout.UNSET_GONE_MARGIN);
					writeDimension("app:baselineMargin", l.baselineMargin, 0);

					writeBoolen("app:layout_constrainedWidth", l.constrainedWidth, false);
					writeBoolen("app:layout_constrainedHeight", l.constrainedHeight, false);
					writeBoolen("app:barrierAllowsGoneWidgets", l.mBarrierAllowsGoneWidgets, true);
					writeVariable("app:layout_wrapBehaviorInParent", l.mWrapBehavior, ConstraintWidget.WRAP_BEHAVIOR_INCLUDED);

					writeXmlConstraint("app:baselineToBaseline", l.baselineToBaseline);
					writeXmlConstraint("app:baselineToBottom", l.baselineToBottom);
					writeXmlConstraint("app:baselineToTop", l.baselineToTop);
					writeXmlConstraint("app:layout_constraintBottom_toBottomOf", l.bottomToBottom);
					writeXmlConstraint("app:layout_constraintBottom_toTopOf", l.bottomToTop);
					writeXmlConstraint("app:layout_constraintEnd_toEndOf", l.endToEnd);
					writeXmlConstraint("app:layout_constraintEnd_toStartOf", l.endToStart);
					writeXmlConstraint("app:layout_constraintLeft_toLeftOf", l.leftToLeft);
					writeXmlConstraint("app:layout_constraintLeft_toRightOf", l.leftToRight);
					writeXmlConstraint("app:layout_constraintRight_toLeftOf", l.rightToLeft);
					writeXmlConstraint("app:layout_constraintRight_toRightOf", l.rightToRight);
					writeXmlConstraint("app:layout_constraintStart_toEndOf", l.startToEnd);
					writeXmlConstraint("app:layout_constraintStart_toStartOf", l.startToStart);
					writeXmlConstraint("app:layout_constraintTop_toBottomOf", l.topToBottom);
					writeXmlConstraint("app:layout_constraintTop_toTopOf", l.topToTop);

					string[] typesConstraintDefault = new string[] {"spread", "wrap", "percent"};
					writeEnum("app:layout_constraintHeight_default", l.heightDefault, typesConstraintDefault, ConstraintWidget.MATCH_CONSTRAINT_SPREAD);
					writeVariable("app:layout_constraintHeight_percent", l.heightPercent, 1);
					writeDimension("app:layout_constraintHeight_min", l.heightMin, 0);
					writeDimension("app:layout_constraintHeight_max", l.heightMax, 0);
					writeBoolen("android:layout_constrainedHeight", l.constrainedHeight, false);

					writeEnum("app:layout_constraintWidth_default", l.widthDefault, typesConstraintDefault, ConstraintWidget.MATCH_CONSTRAINT_SPREAD);
					writeVariable("app:layout_constraintWidth_percent", l.widthPercent, 1);
					writeDimension("app:layout_constraintWidth_min", l.widthMin, 0);
					writeDimension("app:layout_constraintWidth_max", l.widthMax, 0);
					writeBoolen("android:layout_constrainedWidth", l.constrainedWidth, false);

					writeVariable("app:layout_constraintVertical_weight", l.verticalWeight, UNSET);
					writeVariable("app:layout_constraintHorizontal_weight", l.horizontalWeight, UNSET);
					writeVariable("app:layout_constraintHorizontal_chainStyle", l.horizontalChainStyle);
					writeVariable("app:layout_constraintVertical_chainStyle", l.verticalChainStyle);
					string[] barrierDir = new string[] {"left", "right", "top", "bottom", "start", "end"};
					writeEnum("app:barrierDirection", l.mBarrierDirection, barrierDir, UNSET);
					writeVariable("app:layout_constraintTag", l.mConstraintTag, null);

					if (l.mReferenceIds != null)
					{
						writeVariable("'ReferenceIds'", l.mReferenceIds);
					}
					writer.write(" />\n");
				}
				writer.write("</ConstraintSet>\n");
			}

			internal const string SPACE = "\n       ";

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeBoolen(String dimString, boolean val, boolean def) throws java.io.IOException
			internal virtual void writeBoolen(string dimString, bool val, bool def)
			{
				if (val != def)
				{
					writer.write(SPACE + dimString + "=\"" + val + "dp\"");
				}
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeEnum(String dimString, int val, String[] types, int def) throws java.io.IOException
			internal virtual void writeEnum(string dimString, int val, string[] types, int def)
			{
				if (val != def)
				{
					writer.write(SPACE + dimString + "=\"" + types[val] + "\"");
				}
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeDimension(String dimString, int dim, int def) throws java.io.IOException
			internal virtual void writeDimension(string dimString, int dim, int def)
			{
				if (dim != def)
				{
					writer.write(SPACE + dimString + "=\"" + dim + "dp\"");
				}
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeBaseDimension(String dimString, int dim, int def) throws java.io.IOException
			internal virtual void writeBaseDimension(string dimString, int dim, int def)
			{
				if (dim != def)
				{
					if (dim == -2)
					{
						writer.write(SPACE + dimString + "=\"wrap_content\"");

					}
					else if (dim == -1)
					{
						writer.write(SPACE + dimString + "=\"match_parent\"");

					}
					else
					{
						writer.write(SPACE + dimString + "=\"" + dim + "dp\"");
					}
				}
			}

			internal Dictionary<int?, string> idMap = new Dictionary<int?, string>();

			internal virtual string getName(int id)
			{
				if (idMap.ContainsKey(id))
				{
					return "@+id/" + idMap[id] + "";
				}
				if (id == 0)
				{
					return "parent";
				}
				string name = lookup(id);
				idMap[id] = name;
				return "@+id/" + name + "";
			}

			internal virtual string lookup(int id)
			{
				try
				{
					if (id != -1)
					{
						return context.Resources.getResourceEntryName(id);
					}
					else
					{
						return "unknown" + (++unknownCount);
					}
				}
				catch (Exception)
				{
					return "unknown" + (++unknownCount);
				}
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeXmlConstraint(String str, int leftToLeft) throws java.io.IOException
			internal virtual void writeXmlConstraint(string str, int leftToLeft)
			{
				if (leftToLeft == UNSET)
				{
					return;
				}
				writer.write(SPACE + str);
				writer.write("=\"" + getName(leftToLeft) + "\"");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeConstraint(String my, int leftToLeft, String other, int margin, int goneMargin) throws java.io.IOException
			internal virtual void writeConstraint(string my, int leftToLeft, string other, int margin, int goneMargin)
			{
				if (leftToLeft == UNSET)
				{
					return;
				}
				writer.write(SPACE + my);
				writer.write(":[");
				writer.write(getName(leftToLeft));
				writer.write(" , ");
				writer.write(other);
				if (margin != 0)
				{
					writer.write(" , " + margin);
				}
				writer.write("],\n");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeCircle(int circleConstraint, float circleAngle, int circleRadius) throws java.io.IOException
			internal virtual void writeCircle(int circleConstraint, float circleAngle, int circleRadius)
			{
				if (circleConstraint == UNSET)
				{
					return;
				}
				writer.write("circle");
				writer.write(":[");
				writer.write(getName(circleConstraint));
				writer.write(", " + circleAngle);
				writer.write(circleRadius + "]");
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, int value) throws java.io.IOException
			internal virtual void writeVariable(string name, int value)
			{
				if (value == 0 || value == -1)
				{
					return;
				}
				writer.write(SPACE + name + "=\"" + value + "\"\n");
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, float value, float def) throws java.io.IOException
			internal virtual void writeVariable(string name, float value, float def)
			{
				if (value == def)
				{
					return;
				}
				writer.write(SPACE + name);
				writer.write("=\"" + value + "\"");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, String value, String def) throws java.io.IOException
			internal virtual void writeVariable(string name, string value, string def)
			{
				if (string.ReferenceEquals(value, null) || value.Equals(def))
				{
					return;
				}
				writer.write(SPACE + name);
				writer.write("=\"" + value + "\"");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, int[] value) throws java.io.IOException
			internal virtual void writeVariable(string name, int[] value)
			{
				if (value == null)
				{
					return;
				}
				writer.write(SPACE + name);
				writer.write(":");
				for (int i = 0; i < value.Length; i++)
				{
					writer.write(((i == 0) ? "[" : ", ") + getName(value[i]));
				}
				writer.write("],\n");
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, String value) throws java.io.IOException
			internal virtual void writeVariable(string name, string value)
			{
				if (string.ReferenceEquals(value, null))
				{
					return;
				}
				writer.write(name);
				writer.write(":");
				writer.write(", " + value);
				writer.write("\n");

			}
		}

		// ================================== JSON ===============================================
		internal class WriteJsonEngine
		{
			private readonly ConstraintSet outerInstance;

			internal Writer writer;
			internal ConstraintLayout layout;
			internal Context context;
			internal int flags;
			internal int unknownCount = 0;
			internal readonly string LEFT = "'left'";
			internal readonly string RIGHT = "'right'";
			internal readonly string BASELINE = "'baseline'";
			internal readonly string BOTTOM = "'bottom'";
			internal readonly string TOP = "'top'";
			internal readonly string START = "'start'";
			internal readonly string END = "'end'";
			internal const string SPACE = "       ";

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: WriteJsonEngine(java.io.Writer writer, ConstraintLayout layout, int flags) throws java.io.IOException
			internal WriteJsonEngine(ConstraintSet outerInstance, Writer writer, ConstraintLayout layout, int flags)
			{
				this.outerInstance = outerInstance;
				this.writer = writer;
				this.layout = layout;
				this.context = layout.Context;
				this.flags = flags;
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeLayout() throws java.io.IOException
			internal virtual void writeLayout()
			{
				writer.write("\n\'ConstraintSet\':{\n");
				foreach (int? id in outerInstance.mConstraints.Keys)
				{
					Constraint c = outerInstance.mConstraints[id];
					string idName = getName(id.Value);
					writer.write(idName + ":{\n");
					Layout l = c.layout;

					writeDimension("height", l.mHeight, l.heightDefault, l.heightPercent, l.heightMin, l.heightMax, l.constrainedHeight);
					writeDimension("width", l.mWidth, l.widthDefault, l.widthPercent, l.widthMin, l.widthMax, l.constrainedWidth);

					writeConstraint(LEFT, l.leftToLeft, LEFT, l.leftMargin, l.goneLeftMargin);
					writeConstraint(LEFT, l.leftToRight, RIGHT, l.leftMargin, l.goneLeftMargin);
					writeConstraint(RIGHT, l.rightToLeft, LEFT, l.rightMargin, l.goneRightMargin);
					writeConstraint(RIGHT, l.rightToRight, RIGHT, l.rightMargin, l.goneRightMargin);
					writeConstraint(BASELINE, l.baselineToBaseline, BASELINE, UNSET, l.goneBaselineMargin);
					writeConstraint(BASELINE, l.baselineToTop, TOP, UNSET, l.goneBaselineMargin);
					writeConstraint(BASELINE, l.baselineToBottom, BOTTOM, UNSET, l.goneBaselineMargin);

					writeConstraint(TOP, l.topToBottom, BOTTOM, l.topMargin, l.goneTopMargin);
					writeConstraint(TOP, l.topToTop, TOP, l.topMargin, l.goneTopMargin);
					writeConstraint(BOTTOM, l.bottomToBottom, BOTTOM, l.bottomMargin, l.goneBottomMargin);
					writeConstraint(BOTTOM, l.bottomToTop, TOP, l.bottomMargin, l.goneBottomMargin);
					writeConstraint(START, l.startToStart, START, l.startMargin, l.goneStartMargin);
					writeConstraint(START, l.startToEnd, END, l.startMargin, l.goneStartMargin);
					writeConstraint(END, l.endToStart, START, l.endMargin, l.goneEndMargin);
					writeConstraint(END, l.endToEnd, END, l.endMargin, l.goneEndMargin);
					writeVariable("'horizontalBias'", l.horizontalBias, 0.5f);
					writeVariable("'verticalBias'", l.verticalBias, 0.5f);

					writeCircle(l.circleConstraint, l.circleAngle, l.circleRadius);

					writeGuideline(l.orientation, l.guideBegin, l.guideEnd, l.guidePercent);
					writeVariable("'dimensionRatio'", l.dimensionRatio);
					writeVariable("'barrierMargin'", l.mBarrierMargin);
					writeVariable("'type'", l.mHelperType);
					writeVariable("'ReferenceId'", l.mReferenceIdString);
					writeVariable("'mBarrierAllowsGoneWidgets'", l.mBarrierAllowsGoneWidgets,true);
					writeVariable("'WrapBehavior'", l.mWrapBehavior);

					writeVariable("'verticalWeight'", l.verticalWeight);
					writeVariable("'horizontalWeight'", l.horizontalWeight);
					writeVariable("'horizontalChainStyle'", l.horizontalChainStyle);
					writeVariable("'verticalChainStyle'", l.verticalChainStyle);
					writeVariable("'barrierDirection'", l.mBarrierDirection);
					if (l.mReferenceIds != null)
					{
						writeVariable("'ReferenceIds'", l.mReferenceIds);
					}
					writer.write("}\n");
				}
				writer.write("}\n");
			}

			internal virtual void writeGuideline(int orientation, int guideBegin, int guideEnd, float guidePercent)
			{
			}



//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeDimension(String dimString, int dim, int dimDefault, float dimPercent, int dimMin, int dimMax, boolean constrainedDim) throws java.io.IOException
			internal virtual void writeDimension(string dimString, int dim, int dimDefault, float dimPercent, int dimMin, int dimMax, bool constrainedDim)
			{




					if (dim == 0)
					{
						if (dimMax != UNSET || dimMin != UNSET)
						{
							switch (dimDefault)
							{
								case 0: // spread
									writer.write(SPACE + dimString + ": {'spread' ," + dimMin + ", " + dimMax + "}\n");
									break;
								case 1: //  wrap
									writer.write(SPACE + dimString + ": {'wrap' ," + dimMin + ", " + dimMax + "}\n");
									return;
								case 2: // percent
									writer.write(SPACE + dimString + ": {'" + dimPercent + "'% ," + dimMin + ", " + dimMax + "}\n");
									return;
							}
							return;
						}

						switch (dimDefault)
						{
							case 0: // spread is the default
								break;
							case 1: //  wrap
								writer.write(SPACE + dimString + ": '???????????',\n");
								return;
							case 2: // percent
								writer.write(SPACE + dimString + ": '" + dimPercent + "%',\n");
								return;
						}



					}
					else if (dim == -2)
					{
						writer.write(SPACE + dimString + ": 'wrap'\n");
					}
					else if (dim == -1)
					{
						writer.write(SPACE + dimString + ": 'parent'\n");

					}
					else
					{
						writer.write(SPACE + dimString + ": " + dim + ",\n");
					}

			}

			internal Dictionary<int?, string> idMap = new Dictionary<int?, string>();

			internal virtual string getName(int id)
			{
				if (idMap.ContainsKey(id))
				{
					return "\'" + idMap[id] + "\'";
				}
				if (id == 0)
				{
					return "'parent'";
				}
				string name = lookup(id);
				idMap[id] = name;
				return "\'" + name + "\'";
			}

			internal virtual string lookup(int id)
			{
				try
				{
					if (id != -1)
					{
						return context.Resources.getResourceEntryName(id);
					}
					else
					{
						return "unknown" + (++unknownCount);
					}
				}
				catch (Exception)
				{
					return "unknown" + (++unknownCount);
				}
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeConstraint(String my, int leftToLeft, String other, int margin, int goneMargin) throws java.io.IOException
			internal virtual void writeConstraint(string my, int leftToLeft, string other, int margin, int goneMargin)
			{
				if (leftToLeft == UNSET)
				{
					return;
				}
				writer.write(SPACE + my);
				writer.write(":[");
				writer.write(getName(leftToLeft));
				writer.write(" , ");
				writer.write(other);
				if (margin != 0)
				{
					writer.write(" , " + margin);
				}
				writer.write("],\n");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeCircle(int circleConstraint, float circleAngle, int circleRadius) throws java.io.IOException
			internal virtual void writeCircle(int circleConstraint, float circleAngle, int circleRadius)
			{
				if (circleConstraint == UNSET)
				{
					return;
				}
				writer.write(SPACE + "circle");
				writer.write(":[");
				writer.write(getName(circleConstraint));
				writer.write(", " + circleAngle);
				writer.write(circleRadius + "]");
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, int value) throws java.io.IOException
			internal virtual void writeVariable(string name, int value)
			{
				if (value == 0 || value == -1)
				{
					return;
				}
				writer.write(SPACE + name);
				writer.write(":");

				writer.write(", " + value);
				writer.write("\n");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, float value) throws java.io.IOException
			internal virtual void writeVariable(string name, float value)
			{
				if (value == UNSET)
				{
					return;
				}
				writer.write(SPACE + name);

				writer.write(": " + value);
				writer.write(",\n");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, float value, float def) throws java.io.IOException
			internal virtual void writeVariable(string name, float value, float def)
			{
				if (value == def)
				{
					return;
				}
				writer.write(SPACE + name);

				writer.write(": " + value);
				writer.write(",\n");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, boolean value) throws java.io.IOException
			internal virtual void writeVariable(string name, bool value)
			{
				if (value == false)
				{
					return;
				}
				writer.write(SPACE + name);

				writer.write(": " + value);
				writer.write(",\n");

			}
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, boolean value, boolean def) throws java.io.IOException
			internal virtual void writeVariable(string name, bool value, bool def)
			{
				if (value == def)
				{
					return;
				}
				writer.write(SPACE + name);

				writer.write(": " + value);
				writer.write(",\n");

			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, int[] value) throws java.io.IOException
			internal virtual void writeVariable(string name, int[] value)
			{
				if (value == null)
				{
					return;
				}
				writer.write(SPACE + name);
				writer.write(": ");
				for (int i = 0; i < value.Length; i++)
				{
					writer.write(((i == 0) ? "[" : ", ") + getName(value[i]));
				}
				writer.write("],\n");
			}

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void writeVariable(String name, String value) throws java.io.IOException
			internal virtual void writeVariable(string name, string value)
			{
				if (string.ReferenceEquals(value, null))
				{
					return;
				}
				writer.write(SPACE + name);
				writer.write(":");
				writer.write(", " + value);
				writer.write("\n");

			}
		}

	}

}