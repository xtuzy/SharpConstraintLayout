using System.Collections.Generic;

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
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
	using WidgetFrame = androidx.constraintlayout.core.state.WidgetFrame;
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;


	public class MotionWidget : TypedValues
	{
		internal WidgetFrame widgetFrame = new WidgetFrame();
		internal Motion motion = new Motion();
		internal PropertySet propertySet = new PropertySet();
		private float mProgress;
		internal float mTransitionPathRotate;

		public const int VISIBILITY_MODE_NORMAL = 0;
		public const int VISIBILITY_MODE_IGNORE = 1;
		private const int INTERNAL_MATCH_PARENT = -1;
		private const int INTERNAL_WRAP_CONTENT = -2;
		public const int INVISIBLE = 0;
		public const int VISIBLE = 4;
		private const int INTERNAL_MATCH_CONSTRAINT = -3;
		private const int INTERNAL_WRAP_CONTENT_CONSTRAINED = -4;

		public const int ROTATE_NONE = 0;
		public const int ROTATE_PORTRATE_OF_RIGHT = 1;
		public const int ROTATE_PORTRATE_OF_LEFT = 2;
		public const int ROTATE_RIGHT_OF_PORTRATE = 3;
		public const int ROTATE_LEFT_OF_PORTRATE = 4;
		public const int UNSET = -1;
		public const int MATCH_CONSTRAINT = 0;
		public const int PARENT_ID = 0;
		public const int FILL_PARENT = -1;
		public const int MATCH_PARENT = -1;
		public const int WRAP_CONTENT = -2;
		public static readonly int GONE_UNSET = int.MinValue;
		public const int MATCH_CONSTRAINT_WRAP = ConstraintWidget.MATCH_CONSTRAINT_WRAP;

		/// <summary>
		/// @suppress
		/// </summary>
		public class Motion
		{
			public int mAnimateRelativeTo = UNSET;
			public int mAnimateCircleAngleTo = 0;
			public string mTransitionEasing = null;
			public int mPathMotionArc = UNSET;
			public int mDrawPath = 0;
			public float mMotionStagger = float.NaN;
			public int mPolarRelativeTo = UNSET;
			public float mPathRotate = float.NaN;
			public float mQuantizeMotionPhase = float.NaN;
			public int mQuantizeMotionSteps = UNSET;
			public string mQuantizeInterpolatorString = null;
			public int mQuantizeInterpolatorType = INTERPOLATOR_UNDEFINED; // undefined
			public int mQuantizeInterpolatorID = -1;
			internal const int INTERPOLATOR_REFERENCE_ID = -2;
			internal const int SPLINE_STRING = -1;
			internal const int INTERPOLATOR_UNDEFINED = -3;
		}

		public class PropertySet
		{
			public int visibility = VISIBLE;
			public int mVisibilityMode = VISIBILITY_MODE_NORMAL;
			public float alpha = 1;
			public float mProgress = float.NaN;
		}

		public MotionWidget()
		{

		}

		public virtual MotionWidget Parent
		{
			get
			{
				return null;
			}
		}

		public virtual MotionWidget findViewById(int mTransformPivotTarget)
		{
			return null;
		}

		public virtual int Visibility
		{
			set
			{
				propertySet.visibility = value;
			}
			get
			{
				return propertySet.visibility;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.GetType().Name;
			}
		}

		public virtual void layout(int l, int t, int r, int b)
		{
			setBounds(l, t, r, b);
		}

		public override string ToString()
		{
			return widgetFrame.left + ", " + widgetFrame.top + ", " + widgetFrame.right + ", " + widgetFrame.bottom;
		}

		public virtual void setBounds(int left, int top, int right, int bottom)
		{
			if (widgetFrame == null)
			{
				widgetFrame = new WidgetFrame((ConstraintWidget) null);
			}
			widgetFrame.top = top;
			widgetFrame.left = left;
			widgetFrame.right = right;
			widgetFrame.bottom = bottom;
		}

		public MotionWidget(WidgetFrame f)
		{
			widgetFrame = f;
		}

		public override bool setValue(int id, int value)
		{
			return setValueAttributes(id, value);
		}

		public override bool setValue(int id, float value)
		{
			bool set = setValueAttributes(id, value);
			if (set)
			{
				return true;
			}
			return setValueMotion(id, value);
		}

		public override bool setValue(int id, string value)
		{
			return setValueMotion(id, value);
		}

		public override bool setValue(int id, bool value)
		{
			return false;
		}

		public virtual bool setValueMotion(int id, int value)
		{
			switch (id)
			{
				case TypedValues.TypedValues_Motion.TYPE_ANIMATE_RELATIVE_TO:
					motion.mAnimateRelativeTo = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_ANIMATE_CIRCLEANGLE_TO:
					motion.mAnimateCircleAngleTo = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_PATHMOTION_ARC:
					motion.mPathMotionArc = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_DRAW_PATH:
					motion.mDrawPath = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_POLAR_RELATIVETO:
					motion.mPolarRelativeTo = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_QUANTIZE_MOTIONSTEPS:
					motion.mQuantizeMotionSteps = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_QUANTIZE_INTERPOLATOR_TYPE:
					motion.mQuantizeInterpolatorType = value;
					break; // undefined
					goto case TypedValues.TypedValues_Motion.TYPE_QUANTIZE_INTERPOLATOR_ID;
				case TypedValues.TypedValues_Motion.TYPE_QUANTIZE_INTERPOLATOR_ID:
					motion.mQuantizeInterpolatorID = value;
					break;
				default:
					return false;
			}
			return true;
		}

		public virtual bool setValueMotion(int id, string value)
		{
			switch (id)
			{

				case TypedValues.TypedValues_Motion.TYPE_EASING:
					motion.mTransitionEasing = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_QUANTIZE_INTERPOLATOR:
					motion.mQuantizeInterpolatorString = value;
					break;
				default:
					return false;
			}
			return true;
		}

		public virtual bool setValueMotion(int id, float value)
		{
			switch (id)
			{
				case TypedValues.TypedValues_Motion.TYPE_STAGGER:
					motion.mMotionStagger = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_PATH_ROTATE:
					motion.mPathRotate = value;
					break;
				case TypedValues.TypedValues_Motion.TYPE_QUANTIZE_MOTION_PHASE:
					motion.mQuantizeMotionPhase = value;
					break;
				default:
					return false;
			}
			return true;
		}

		/// <summary>
		/// Sets the attributes
		/// </summary>
		/// <param name="id"> </param>
		/// <param name="value"> </param>
		public virtual bool setValueAttributes(int id, float value)
		{
			switch (id)
			{
				case TypedValues.TypedValues_Attributes.TYPE_ALPHA:
					widgetFrame.alpha = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_X:
					widgetFrame.translationX = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_Y:
					widgetFrame.translationY = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_Z:
					widgetFrame.translationZ = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_X:
					widgetFrame.rotationX = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_Y:
					widgetFrame.rotationY = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_Z:
					widgetFrame.rotationZ = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_SCALE_X:
					widgetFrame.scaleX = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_SCALE_Y:
					widgetFrame.scaleY = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_PIVOT_X:
					widgetFrame.pivotX = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_PIVOT_Y:
					widgetFrame.pivotY = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_PROGRESS:
					mProgress = value;
					break;
				case TypedValues.TypedValues_Attributes.TYPE_PATH_ROTATE:
					mTransitionPathRotate = value;
					break;
				default:
					return false;
			}
			return true;
		}
		/// <summary>
		/// Sets the attributes
		/// </summary>
		/// <param name="id"> </param>
		public virtual float getValueAttributes(int id)
		{
			switch (id)
			{
				case TypedValues.TypedValues_Attributes.TYPE_ALPHA:
					return widgetFrame.alpha;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_X:
					return widgetFrame.translationX;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_Y:
					return widgetFrame.translationY;
				case TypedValues.TypedValues_Attributes.TYPE_TRANSLATION_Z:
					return widgetFrame.translationZ;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_X:
					return widgetFrame.rotationX;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_Y:
					return widgetFrame.rotationY;
				case TypedValues.TypedValues_Attributes.TYPE_ROTATION_Z:
					return widgetFrame.rotationZ;
				case TypedValues.TypedValues_Attributes.TYPE_SCALE_X:
					return widgetFrame.scaleX;
				case TypedValues.TypedValues_Attributes.TYPE_SCALE_Y:
					return widgetFrame.scaleY;
				case TypedValues.TypedValues_Attributes.TYPE_PIVOT_X:
					return widgetFrame.pivotX;
				case TypedValues.TypedValues_Attributes.TYPE_PIVOT_Y:
					return widgetFrame.pivotY;
				case TypedValues.TypedValues_Attributes.TYPE_PROGRESS:
					return mProgress;
				case TypedValues.TypedValues_Attributes.TYPE_PATH_ROTATE:
					return mTransitionPathRotate;
				default:
					return float.NaN;
			}

		}
		public override int getId(string name)
		{
			int ret = TypedValues.TypedValues_Attributes.getId(name);
			if (ret != -1)
			{
				return ret;
			}
			return TypedValues.TypedValues_Motion.getId(name);
		}

		public virtual int Top
		{
			get
			{
				return widgetFrame.top;
			}
		}

		public virtual int Left
		{
			get
			{
				return widgetFrame.left;
			}
		}

		public virtual int Bottom
		{
			get
			{
				return widgetFrame.bottom;
			}
		}

		public virtual int Right
		{
			get
			{
				return widgetFrame.right;
			}
		}

		public virtual float PivotX
		{
			set
			{
				widgetFrame.pivotX = value;
			}
			get
			{
				return widgetFrame.pivotX;
			}
		}

		public virtual float PivotY
		{
			set
			{
				widgetFrame.pivotY = value;
			}
			get
			{
				return widgetFrame.pivotY;
			}
		}

		public virtual float RotationX
		{
			get
			{
				return widgetFrame.rotationX;
			}
			set
			{
				widgetFrame.rotationX = value;
			}
		}


		public virtual float RotationY
		{
			get
			{
				return widgetFrame.rotationY;
			}
			set
			{
				widgetFrame.rotationY = value;
			}
		}


		public virtual float RotationZ
		{
			get
			{
				return widgetFrame.rotationZ;
			}
			set
			{
				widgetFrame.rotationZ = value;
			}
		}


		public virtual float TranslationX
		{
			get
			{
				return widgetFrame.translationX;
			}
			set
			{
				widgetFrame.translationX = value;
			}
		}


		public virtual float TranslationY
		{
			get
			{
				return widgetFrame.translationY;
			}
			set
			{
				widgetFrame.translationY = value;
			}
		}


		public virtual float TranslationZ
		{
			set
			{
				widgetFrame.translationZ = value;
			}
			get
			{
				return widgetFrame.translationZ;
			}
		}


		public virtual float ScaleX
		{
			get
			{
				return widgetFrame.scaleX;
			}
			set
			{
				widgetFrame.scaleX = value;
			}
		}


		public virtual float ScaleY
		{
			get
			{
				return widgetFrame.scaleY;
			}
			set
			{
				widgetFrame.scaleY = value;
			}
		}





		public virtual float Alpha
		{
			get
			{
				return propertySet.alpha;
			}
		}

		public virtual int X
		{
			get
			{
				return widgetFrame.left;
			}
		}

		public virtual int Y
		{
			get
			{
				return widgetFrame.top;
			}
		}

		public virtual int Width
		{
			get
			{
				return widgetFrame.right - widgetFrame.left;
			}
		}

		public virtual int Height
		{
			get
			{
				return widgetFrame.bottom - widgetFrame.top;
			}
		}

		public virtual WidgetFrame WidgetFrame
		{
			get
			{
				return widgetFrame;
			}
		}

		public virtual ICollection<string> CustomAttributeNames
		{
			get
			{
				return widgetFrame.CustomAttributeNames;
			}
		}

		public virtual void setCustomAttribute(string name, int type, float value)
		{
			widgetFrame.setCustomAttribute(name, type, value);
		}

		public virtual void setCustomAttribute(string name, int type, int value)
		{
			widgetFrame.setCustomAttribute(name, type, value);
		}

		public virtual void setCustomAttribute(string name, int type, bool value)
		{
			widgetFrame.setCustomAttribute(name, type, value);
		}

		public virtual void setCustomAttribute(string name, int type, string value)
		{
			widgetFrame.setCustomAttribute(name, type, value);
		}

		public virtual CustomVariable getCustomAttribute(string name)
		{
			return widgetFrame.getCustomAttribute(name);
		}
	}

}