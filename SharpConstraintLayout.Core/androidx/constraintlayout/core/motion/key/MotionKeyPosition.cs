using System;
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
namespace androidx.constraintlayout.core.motion.key
{
	using FloatRect = androidx.constraintlayout.core.motion.utils.FloatRect;
	using SplineSet = androidx.constraintlayout.core.motion.utils.SplineSet;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;


	public class MotionKeyPosition : MotionKey
	{
		private bool InstanceFieldsInitialized = false;

		public MotionKeyPosition()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			mType = KEY_TYPE;
		}

		internal const string NAME = "KeyPosition";
		protected internal const float SELECTION_SLOPE = 20;
		public int mCurveFit = UNSET;
		public string mTransitionEasing = null;
		public int mPathMotionArc = UNSET; // -1 means not set
		public int mDrawPath = 0;
		public float mPercentWidth = float.NaN;
		public float mPercentHeight = float.NaN;
		public float mPercentX = float.NaN;
		public float mPercentY = float.NaN;
		public float mAltPercentX = float.NaN;
		public float mAltPercentY = float.NaN;
		public const int TYPE_SCREEN = 2;
		public const int TYPE_PATH = 1;
		public const int TYPE_CARTESIAN = 0;
		public int mPositionType = TYPE_CARTESIAN;

		private float mCalculatedPositionX = float.NaN;
		private float mCalculatedPositionY = float.NaN;
		internal const int KEY_TYPE = 2;

		// TODO this needs the views dimensions to be accurate
		private void calcScreenPosition(int layoutWidth, int layoutHeight)
		{
			int viewWidth = 0;
			int viewHeight = 0;
			mCalculatedPositionX = (layoutWidth - viewWidth) * mPercentX + viewWidth / 2;
			mCalculatedPositionY = (layoutHeight - viewHeight) * mPercentX + viewHeight / 2;
		}

		private void calcPathPosition(float start_x, float start_y, float end_x, float end_y)
		{
			float pathVectorX = end_x - start_x;
			float pathVectorY = end_y - start_y;
			float perpendicularX = -pathVectorY;
			float perpendicularY = pathVectorX;
			mCalculatedPositionX = start_x + pathVectorX * mPercentX + perpendicularX * mPercentY;
			mCalculatedPositionY = start_y + pathVectorY * mPercentX + perpendicularY * mPercentY;
		}

		private void calcCartesianPosition(float start_x, float start_y, float end_x, float end_y)
		{
			float pathVectorX = end_x - start_x;
			float pathVectorY = end_y - start_y;
			float dxdx = (float.IsNaN(mPercentX)) ? 0 : mPercentX;
			float dydx = (float.IsNaN(mAltPercentY)) ? 0 : mAltPercentY;
			float dydy = (float.IsNaN(mPercentY)) ? 0 : mPercentY;
			float dxdy = (float.IsNaN(mAltPercentX)) ? 0 : mAltPercentX;
			mCalculatedPositionX = (int)(start_x + pathVectorX * dxdx + pathVectorY * dxdy);
			mCalculatedPositionY = (int)(start_y + pathVectorX * dydx + pathVectorY * dydy);
		}

		internal virtual float PositionX
		{
			get
			{
				return mCalculatedPositionX;
			}
		}

		internal virtual float PositionY
		{
			get
			{
				return mCalculatedPositionY;
			}
		}

		public virtual void positionAttributes(MotionWidget view, FloatRect start, FloatRect end, float x, float y, string[] attribute, float[] value)
		{
			switch (mPositionType)
			{

				case TYPE_PATH:
					positionPathAttributes(start, end, x, y, attribute, value);
					return;
				case TYPE_SCREEN:
					positionScreenAttributes(view, start, end, x, y, attribute, value);
					return;
				case TYPE_CARTESIAN:
				default:
					positionCartAttributes(start, end, x, y, attribute, value);
					return;

			}
		}

		internal virtual void positionPathAttributes(FloatRect start, FloatRect end, float x, float y, string[] attribute, float[] value)
		{
			float startCenterX = start.centerX();
			float startCenterY = start.centerY();
			float endCenterX = end.centerX();
			float endCenterY = end.centerY();
			float pathVectorX = endCenterX - startCenterX;
			float pathVectorY = endCenterY - startCenterY;
			float distance = (float) MathExtension.hypot(pathVectorX, pathVectorY);
			if (distance < 0.0001)
			{
				Console.WriteLine("distance ~ 0");
				value[0] = 0;
				value[1] = 0;
				return;
			}

			float dx = pathVectorX / distance;
			float dy = pathVectorY / distance;
			float perpendicular = (dx * (y - startCenterY) - (x - startCenterX) * dy) / distance;
			float dist = (dx * (x - startCenterX) + dy * (y - startCenterY)) / distance;
			if (!string.ReferenceEquals(attribute[0], null))
			{
				if (TypedValues.TypedValues_Position.S_PERCENT_X.Equals(attribute[0]))
				{
					value[0] = dist;
					value[1] = perpendicular;
				}
			}
			else
			{
				attribute[0] = TypedValues.TypedValues_Position.S_PERCENT_X;
				attribute[1] = TypedValues.TypedValues_Position.S_PERCENT_Y;
				value[0] = dist;
				value[1] = perpendicular;
			}
		}

		internal virtual void positionScreenAttributes(MotionWidget view, FloatRect start, FloatRect end, float x, float y, string[] attribute, float[] value)
		{
			float startCenterX = start.centerX();
			float startCenterY = start.centerY();
			float endCenterX = end.centerX();
			float endCenterY = end.centerY();
			float pathVectorX = endCenterX - startCenterX;
			float pathVectorY = endCenterY - startCenterY;
			MotionWidget viewGroup = ((MotionWidget) view.Parent);
			int width = viewGroup.Width;
			int height = viewGroup.Height;

			if (!string.ReferenceEquals(attribute[0], null))
			{ // they are saying what to use
				if (TypedValues.TypedValues_Position.S_PERCENT_X.Equals(attribute[0]))
				{
					value[0] = x / width;
					value[1] = y / height;
				}
				else
				{
					value[1] = x / width;
					value[0] = y / height;
				}
			}
			else
			{ // we will use what we want to
				attribute[0] = TypedValues.TypedValues_Position.S_PERCENT_X;
				value[0] = x / width;
				attribute[1] = TypedValues.TypedValues_Position.S_PERCENT_Y;
				value[1] = y / height;
			}
		}

		internal virtual void positionCartAttributes(FloatRect start, FloatRect end, float x, float y, string[] attribute, float[] value)
		{
			float startCenterX = start.centerX();
			float startCenterY = start.centerY();
			float endCenterX = end.centerX();
			float endCenterY = end.centerY();
			float pathVectorX = endCenterX - startCenterX;
			float pathVectorY = endCenterY - startCenterY;
			if (!string.ReferenceEquals(attribute[0], null))
			{ // they are saying what to use
				if (TypedValues.TypedValues_Position.S_PERCENT_X.Equals(attribute[0]))
				{
					value[0] = (x - startCenterX) / pathVectorX;
					value[1] = (y - startCenterY) / pathVectorY;
				}
				else
				{
					value[1] = (x - startCenterX) / pathVectorX;
					value[0] = (y - startCenterY) / pathVectorY;
				}
			}
			else
			{ // we will use what we want to
				attribute[0] = TypedValues.TypedValues_Position.S_PERCENT_X;
				value[0] = (x - startCenterX) / pathVectorX;
				attribute[1] = TypedValues.TypedValues_Position.S_PERCENT_Y;
				value[1] = (y - startCenterY) / pathVectorY;
			}
		}

		public virtual bool intersects(int layoutWidth, int layoutHeight, FloatRect start, FloatRect end, float x, float y)
		{
			calcPosition(layoutWidth, layoutHeight, start.centerX(), start.centerY(), end.centerX(), end.centerY());
			if ((Math.Abs(x - mCalculatedPositionX) < SELECTION_SLOPE) && (Math.Abs(y - mCalculatedPositionY) < SELECTION_SLOPE))
			{
				return true;
			}
			return false;
		}

		public override MotionKey copy(MotionKey src)
		{
			base.copy(src);
			MotionKeyPosition k = (MotionKeyPosition) src;
			mTransitionEasing = k.mTransitionEasing;
			mPathMotionArc = k.mPathMotionArc;
			mDrawPath = k.mDrawPath;
			mPercentWidth = k.mPercentWidth;
			mPercentHeight = float.NaN;
			mPercentX = k.mPercentX;
			mPercentY = k.mPercentY;
			mAltPercentX = k.mAltPercentX;
			mAltPercentY = k.mAltPercentY;
			mCalculatedPositionX = k.mCalculatedPositionX;
			mCalculatedPositionY = k.mCalculatedPositionY;
			return this;
		}

		public override MotionKey clone()
		{
			return (new MotionKeyPosition()).copy(this);
		}

		internal virtual void calcPosition(int layoutWidth, int layoutHeight, float start_x, float start_y, float end_x, float end_y)
		{
			switch (mPositionType)
			{
				case TYPE_SCREEN:
					calcScreenPosition(layoutWidth, layoutHeight);
					return;

				case TYPE_PATH:
					calcPathPosition(start_x, start_y, end_x, end_y);
					return;
				case TYPE_CARTESIAN:
				default:
					calcCartesianPosition(start_x, start_y, end_x, end_y);
					return;
			}
		}

		public override void getAttributeNames(HashSet<string> attributes)
		{

		}

		public override void addValues(Dictionary<string, SplineSet> splines)
		{
		}

		public override bool setValue(int type, int value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Position.TYPE_POSITION_TYPE:
					mPositionType = value;
					break;
				case TypedValues.TYPE_FRAME_POSITION:
					mFramePosition = value;
					break;
				case TypedValues.TypedValues_Position.TYPE_CURVE_FIT:
					mCurveFit = value;
					break;

				default:
					return base.setValue(type, value);
			}
			return true;

		}

		public override bool setValue(int type, float value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Position.TYPE_PERCENT_WIDTH:
					mPercentWidth = value;
					break;
				case TypedValues.TypedValues_Position.TYPE_PERCENT_HEIGHT:
					mPercentHeight = value;
					break;
				case TypedValues.TypedValues_Position.TYPE_SIZE_PERCENT:
					mPercentHeight = mPercentWidth = value;
					break;
				case TypedValues.TypedValues_Position.TYPE_PERCENT_X:
					mPercentX = value;
					break;
				case TypedValues.TypedValues_Position.TYPE_PERCENT_Y:
					mPercentY = value;
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}

		public override bool setValue(int type, string value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Position.TYPE_TRANSITION_EASING:
					mTransitionEasing = value.ToString();
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}

		public override int getId(string name)
		{
			return TypedValues.TypedValues_Position.getId(name);
		}

	}

}