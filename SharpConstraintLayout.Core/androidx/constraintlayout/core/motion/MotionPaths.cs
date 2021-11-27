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
namespace androidx.constraintlayout.core.motion
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.motion.MotionWidget.UNSET;
using static androidx.constraintlayout.core.motion.MotionWidget;

	using MotionKeyPosition = androidx.constraintlayout.core.motion.key.MotionKeyPosition;
	using Easing = androidx.constraintlayout.core.motion.utils.Easing;
	using Utils = androidx.constraintlayout.core.motion.utils.Utils;


	/// <summary>
	/// This is used to capture and play back path of the layout.
	/// It is used to set the bounds of the view (view.layout(l, t, r, b))
	/// 
	/// @suppress
	/// </summary>
	public class MotionPaths : IComparable<MotionPaths>
	{
		public const string TAG = "MotionPaths";
		public const bool DEBUG = false;
		public const bool OLD_WAY = false; // the computes the positions the old way
		internal const int OFF_POSITION = 0;
		internal const int OFF_X = 1;
		internal const int OFF_Y = 2;
		internal const int OFF_WIDTH = 3;
		internal const int OFF_HEIGHT = 4;
		internal const int OFF_PATH_ROTATE = 5;

		// mode and type have same numbering scheme
		public const int PERPENDICULAR = MotionKeyPosition.TYPE_PATH;
		public const int CARTESIAN = MotionKeyPosition.TYPE_CARTESIAN;
		public const int SCREEN = MotionKeyPosition.TYPE_SCREEN;
		internal static string[] names = new string[] {"position", "x", "y", "width", "height", "pathRotate"};
		internal Easing mKeyFrameEasing;
		internal int mDrawPath = 0;
		internal float time;
		internal float position;
		internal float x;
		internal float y;
		internal float width;
		internal float height;
		internal float mPathRotate = float.NaN;
		internal float mProgress = float.NaN;
		internal int mPathMotionArc = UNSET;
		internal int mAnimateRelativeTo = UNSET;
		internal float mRelativeAngle = float.NaN;
		internal Motion mRelativeToController = null;

		internal Dictionary<string, CustomVariable> customAttributes = new Dictionary<string, CustomVariable>();
		internal int mMode = 0; // how was this point computed 1=perpendicular 2=deltaRelative
		internal int mAnimateCircleAngleTo; // since angles loop there are 4 ways we can pic direction

		public MotionPaths()
		{

		}

		/// <summary>
		/// set up with Cartesian
		/// </summary>
		/// <param name="c"> </param>
		/// <param name="startTimePoint"> </param>
		/// <param name="endTimePoint"> </param>
		internal virtual void initCartesian(MotionKeyPosition c, MotionPaths startTimePoint, MotionPaths endTimePoint)
		{
			float position = c.mFramePosition / 100f;
			MotionPaths point = this;
			point.time = position;

			mDrawPath = c.mDrawPath;
			float scaleWidth = float.IsNaN(c.mPercentWidth) ? position : c.mPercentWidth;
			float scaleHeight = float.IsNaN(c.mPercentHeight) ? position : c.mPercentHeight;
			float scaleX = endTimePoint.width - startTimePoint.width;
			float scaleY = endTimePoint.height - startTimePoint.height;

			point.position = point.time;

			float path = position; // the position on the path

			float startCenterX = startTimePoint.x + startTimePoint.width / 2;
			float startCenterY = startTimePoint.y + startTimePoint.height / 2;
			float endCenterX = endTimePoint.x + endTimePoint.width / 2;
			float endCenterY = endTimePoint.y + endTimePoint.height / 2;
			float pathVectorX = endCenterX - startCenterX;
			float pathVectorY = endCenterY - startCenterY;
			point.x = (int)(startTimePoint.x + (pathVectorX) * path - scaleX * scaleWidth / 2);
			point.y = (int)(startTimePoint.y + (pathVectorY) * path - scaleY * scaleHeight / 2);
			point.width = (int)(startTimePoint.width + scaleX * scaleWidth);
			point.height = (int)(startTimePoint.height + scaleY * scaleHeight);

			float dxdx = (float.IsNaN(c.mPercentX)) ? position : c.mPercentX;
			float dydx = (float.IsNaN(c.mAltPercentY)) ? 0 : c.mAltPercentY;
			float dydy = (float.IsNaN(c.mPercentY)) ? position : c.mPercentY;
			float dxdy = (float.IsNaN(c.mAltPercentX)) ? 0 : c.mAltPercentX;
			point.mMode = MotionPaths.CARTESIAN;
			point.x = (int)(startTimePoint.x + pathVectorX * dxdx + pathVectorY * dxdy - scaleX * scaleWidth / 2);
			point.y = (int)(startTimePoint.y + pathVectorX * dydx + pathVectorY * dydy - scaleY * scaleHeight / 2);

			point.mKeyFrameEasing = Easing.getInterpolator(c.mTransitionEasing);
			point.mPathMotionArc = c.mPathMotionArc;
		}

		/// <summary>
		/// takes the new keyPosition
		/// </summary>
		/// <param name="c"> </param>
		/// <param name="startTimePoint"> </param>
		/// <param name="endTimePoint"> </param>
		public MotionPaths(int parentWidth, int parentHeight, MotionKeyPosition c, MotionPaths startTimePoint, MotionPaths endTimePoint)
		{
			if (startTimePoint.mAnimateRelativeTo != UNSET)
			{
				initPolar(parentWidth, parentHeight, c, startTimePoint, endTimePoint);
				return;
			}
			switch (c.mPositionType)
			{
				case MotionKeyPosition.TYPE_SCREEN:
					initScreen(parentWidth, parentHeight, c, startTimePoint, endTimePoint);
					return;
				case MotionKeyPosition.TYPE_PATH:
					initPath(c, startTimePoint, endTimePoint);
					return;
				default:
					goto case androidx.constraintlayout.core.motion.key.MotionKeyPosition.TYPE_CARTESIAN;
				case MotionKeyPosition.TYPE_CARTESIAN:
					initCartesian(c, startTimePoint, endTimePoint);
					return;
			}
		}

		internal virtual void initPolar(int parentWidth, int parentHeight, MotionKeyPosition c, MotionPaths s, MotionPaths e)
		{
			float position = c.mFramePosition / 100f;
			this.time = position;
			mDrawPath = c.mDrawPath;
			this.mMode = c.mPositionType; // mode and type have same numbering scheme
			float scaleWidth = float.IsNaN(c.mPercentWidth) ? position : c.mPercentWidth;
			float scaleHeight = float.IsNaN(c.mPercentHeight) ? position : c.mPercentHeight;
			float scaleX = e.width - s.width;
			float scaleY = e.height - s.height;
			this.position = this.time;
			width = (int)(s.width + scaleX * scaleWidth);
			height = (int)(s.height + scaleY * scaleHeight);
			float startfactor = 1 - position;
			float endfactor = position;
			switch (c.mPositionType)
			{
				case MotionKeyPosition.TYPE_SCREEN:
					this.x = float.IsNaN(c.mPercentX) ? (position * (e.x - s.x) + s.x) : c.mPercentX * Math.Min(scaleHeight, scaleWidth);
					this.y = float.IsNaN(c.mPercentY) ? (position * (e.y - s.y) + s.y) : c.mPercentY;
					break;

				case MotionKeyPosition.TYPE_PATH:
					this.x = (float.IsNaN(c.mPercentX) ? position : c.mPercentX) * (e.x - s.x) + s.x;
					this.y = (float.IsNaN(c.mPercentY) ? position : c.mPercentY) * (e.y - s.y) + s.y;
					break;
				default:
					goto case androidx.constraintlayout.core.motion.key.MotionKeyPosition.TYPE_CARTESIAN;
				case MotionKeyPosition.TYPE_CARTESIAN:
					this.x = (float.IsNaN(c.mPercentX) ? position : c.mPercentX) * (e.x - s.x) + s.x;
					this.y = (float.IsNaN(c.mPercentY) ? position : c.mPercentY) * (e.y - s.y) + s.y;
					break;
			}

			this.mAnimateRelativeTo = s.mAnimateRelativeTo;
			this.mKeyFrameEasing = Easing.getInterpolator(c.mTransitionEasing);
			this.mPathMotionArc = c.mPathMotionArc;
		}

		public virtual void setupRelative(Motion mc, MotionPaths relative)
		{
			double dx = x + width / 2 - relative.x - relative.width / 2;
			double dy = y + height / 2 - relative.y - relative.height / 2;
			mRelativeToController = mc;

			x = (float) MathExtension.hypot(dy, dx);
			if (float.IsNaN(mRelativeAngle))
			{
				y = (float)(Math.Atan2(dy, dx) + Math.PI / 2);
			}
			else
			{
				y = (float) MathExtension.toRadians(mRelativeAngle);

			}
		}

		internal virtual void initScreen(int parentWidth, int parentHeight, MotionKeyPosition c, MotionPaths startTimePoint, MotionPaths endTimePoint)
		{
			float position = c.mFramePosition / 100f;
			MotionPaths point = this;
			point.time = position;

			mDrawPath = c.mDrawPath;
			float scaleWidth = float.IsNaN(c.mPercentWidth) ? position : c.mPercentWidth;
			float scaleHeight = float.IsNaN(c.mPercentHeight) ? position : c.mPercentHeight;

			float scaleX = endTimePoint.width - startTimePoint.width;
			float scaleY = endTimePoint.height - startTimePoint.height;

			point.position = point.time;

			float path = position; // the position on the path

			float startCenterX = startTimePoint.x + startTimePoint.width / 2;
			float startCenterY = startTimePoint.y + startTimePoint.height / 2;
			float endCenterX = endTimePoint.x + endTimePoint.width / 2;
			float endCenterY = endTimePoint.y + endTimePoint.height / 2;
			float pathVectorX = endCenterX - startCenterX;
			float pathVectorY = endCenterY - startCenterY;
			point.x = (int)(startTimePoint.x + (pathVectorX) * path - scaleX * scaleWidth / 2);
			point.y = (int)(startTimePoint.y + (pathVectorY) * path - scaleY * scaleHeight / 2);
			point.width = (int)(startTimePoint.width + scaleX * scaleWidth);
			point.height = (int)(startTimePoint.height + scaleY * scaleHeight);

			point.mMode = MotionPaths.SCREEN;
			if (!float.IsNaN(c.mPercentX))
			{
				parentWidth -= (int)point.width;
				point.x = (int)(c.mPercentX * parentWidth);
			}
			if (!float.IsNaN(c.mPercentY))
			{
				parentHeight -= (int)point.height;
				point.y = (int)(c.mPercentY * parentHeight);
			}

			point.mAnimateRelativeTo = mAnimateRelativeTo;
			point.mKeyFrameEasing = Easing.getInterpolator(c.mTransitionEasing);
			point.mPathMotionArc = c.mPathMotionArc;
		}

		internal virtual void initPath(MotionKeyPosition c, MotionPaths startTimePoint, MotionPaths endTimePoint)
		{

			float position = c.mFramePosition / 100f;
			MotionPaths point = this;
			point.time = position;

			mDrawPath = c.mDrawPath;
			float scaleWidth = float.IsNaN(c.mPercentWidth) ? position : c.mPercentWidth;
			float scaleHeight = float.IsNaN(c.mPercentHeight) ? position : c.mPercentHeight;

			float scaleX = endTimePoint.width - startTimePoint.width;
			float scaleY = endTimePoint.height - startTimePoint.height;

			point.position = point.time;

			float path = float.IsNaN(c.mPercentX) ? position : c.mPercentX; // the position on the path

			float startCenterX = startTimePoint.x + startTimePoint.width / 2;
			float startCenterY = startTimePoint.y + startTimePoint.height / 2;
			float endCenterX = endTimePoint.x + endTimePoint.width / 2;
			float endCenterY = endTimePoint.y + endTimePoint.height / 2;
			float pathVectorX = endCenterX - startCenterX;
			float pathVectorY = endCenterY - startCenterY;
			point.x = (int)(startTimePoint.x + (pathVectorX) * path - scaleX * scaleWidth / 2);
			point.y = (int)(startTimePoint.y + (pathVectorY) * path - scaleY * scaleHeight / 2);
			point.width = (int)(startTimePoint.width + scaleX * scaleWidth);
			point.height = (int)(startTimePoint.height + scaleY * scaleHeight);
			float perpendicular = float.IsNaN(c.mPercentY) ? 0 : c.mPercentY; // the position on the path
			float perpendicularX = -pathVectorY;
			float perpendicularY = pathVectorX;

			float normalX = perpendicularX * perpendicular;
			float normalY = perpendicularY * perpendicular;
			point.mMode = MotionPaths.PERPENDICULAR;
			point.x = (int)(startTimePoint.x + (pathVectorX) * path - scaleX * scaleWidth / 2);
			point.y = (int)(startTimePoint.y + (pathVectorY) * path - scaleY * scaleHeight / 2);
			point.x += normalX;
			point.y += normalY;

			point.mAnimateRelativeTo = mAnimateRelativeTo;
			point.mKeyFrameEasing = Easing.getInterpolator(c.mTransitionEasing);
			point.mPathMotionArc = c.mPathMotionArc;
		}

		private static float xRotate(float sin, float cos, float cx, float cy, float x, float y)
		{
			x = x - cx;
			y = y - cy;
			return x * cos - y * sin + cx;
		}

		private static float yRotate(float sin, float cos, float cx, float cy, float x, float y)
		{
			x = x - cx;
			y = y - cy;
			return x * sin + y * cos + cy;
		}

		private bool diff(float a, float b)
		{
			if (float.IsNaN(a) || float.IsNaN(b))
			{
				return float.IsNaN(a) != float.IsNaN(b);
			}
			return Math.Abs(a - b) > 0.000001f;
		}

		internal virtual void different(MotionPaths points, bool[] mask, string[] custom, bool arcMode)
		{
			int c = 0;
			bool diffx = diff(x, points.x);
			bool diffy = diff(y, points.y);
			mask[c++] |= diff(position, points.position);
			mask[c++] |= diffx | diffy | arcMode;
			mask[c++] |= diffx | diffy | arcMode;
			mask[c++] |= diff(width, points.width);
			mask[c++] |= diff(height, points.height);

		}

		internal virtual void getCenter(double p, int[] toUse, double[] data, float[] point, int offset)
		{
			float v_x = x;
			float v_y = y;
			float v_width = width;
			float v_height = height;
			float translationX = 0, translationY = 0;
			for (int i = 0; i < toUse.Length; i++)
			{
				float value = (float) data[i];

				switch (toUse[i])
				{
					case OFF_X:
						v_x = value;
						break;
					case OFF_Y:
						v_y = value;
						break;
					case OFF_WIDTH:
						v_width = value;
						break;
					case OFF_HEIGHT:
						v_height = value;
						break;
				}
			}
			if (mRelativeToController != null)
			{
				float[] pos = new float[2];
				float[] vel = new float[2];

				mRelativeToController.getCenter(p, pos, vel);
				float rx = pos[0];
				float ry = pos[1];
				float radius = v_x;
				float angle = v_y;
				// TODO Debug angle
				v_x = (float)(rx + radius * Math.Sin(angle) - v_width / 2);
				v_y = (float)(ry - radius * Math.Cos(angle) - v_height / 2);
			}

			point[offset] = v_x + v_width / 2 + translationX;
			point[offset + 1] = v_y + v_height / 2 + translationY;
		}

		internal virtual void getCenter(double p, int[] toUse, double[] data, float[] point, double[] vdata, float[] velocity)
		{
			float v_x = x;
			float v_y = y;
			float v_width = width;
			float v_height = height;
			float dv_x = 0;
			float dv_y = 0;
			float dv_width = 0;
			float dv_height = 0;

			float translationX = 0, translationY = 0;
			for (int i = 0; i < toUse.Length; i++)
			{
				float value = (float) data[i];
				float dvalue = (float) vdata[i];

				switch (toUse[i])
				{
					case OFF_X:
						v_x = value;
						dv_x = dvalue;
						break;
					case OFF_Y:
						v_y = value;
						dv_y = dvalue;
						break;
					case OFF_WIDTH:
						v_width = value;
						dv_width = dvalue;
						break;
					case OFF_HEIGHT:
						v_height = value;
						dv_height = dvalue;
						break;
				}
			}
			float dpos_x = dv_x + dv_width / 2;
			float dpos_y = dv_y + dv_height / 2;

			if (mRelativeToController != null)
			{
				float[] pos = new float[2];
				float[] vel = new float[2];
				mRelativeToController.getCenter(p, pos, vel);
				float rx = pos[0];
				float ry = pos[1];
				float radius = v_x;
				float angle = v_y;
				float dradius = dv_x;
				float dangle = dv_y;
				float drx = vel[0];
				float dry = vel[1];
				// TODO Debug angle
				v_x = (float)(rx + radius * Math.Sin(angle) - v_width / 2);
				v_y = (float)(ry - radius * Math.Cos(angle) - v_height / 2);
				dpos_x = (float)(drx + dradius * Math.Sin(angle) + Math.Cos(angle) * dangle);
				dpos_y = (float)(dry - dradius * Math.Cos(angle) + Math.Sin(angle) * dangle);
			}

			point[0] = v_x + v_width / 2 + translationX;
			point[1] = v_y + v_height / 2 + translationY;
			velocity[0] = dpos_x;
			velocity[1] = dpos_y;
		}

		internal virtual void getCenterVelocity(double p, int[] toUse, double[] data, float[] point, int offset)
		{
			float v_x = x;
			float v_y = y;
			float v_width = width;
			float v_height = height;
			float translationX = 0, translationY = 0;
			for (int i = 0; i < toUse.Length; i++)
			{
				float value = (float) data[i];

				switch (toUse[i])
				{
					case OFF_X:
						v_x = value;
						break;
					case OFF_Y:
						v_y = value;
						break;
					case OFF_WIDTH:
						v_width = value;
						break;
					case OFF_HEIGHT:
						v_height = value;
						break;
				}
			}
			if (mRelativeToController != null)
			{
				float[] pos = new float[2];
				float[] vel = new float[2];
				mRelativeToController.getCenter(p, pos, vel);
				float rx = pos[0];
				float ry = pos[1];
				float radius = v_x;
				float angle = v_y;
				// TODO Debug angle
				v_x = (float)(rx + radius * Math.Sin(angle) - v_width / 2);
				v_y = (float)(ry - radius * Math.Cos(angle) - v_height / 2);
			}

			point[offset] = v_x + v_width / 2 + translationX;
			point[offset + 1] = v_y + v_height / 2 + translationY;
		}

		internal virtual void getBounds(int[] toUse, double[] data, float[] point, int offset)
		{
			float v_x = x;
			float v_y = y;
			float v_width = width;
			float v_height = height;
			float translationX = 0, translationY = 0;
			for (int i = 0; i < toUse.Length; i++)
			{
				float value = (float) data[i];

				switch (toUse[i])
				{
					case OFF_X:
						v_x = value;
						break;
					case OFF_Y:
						v_y = value;
						break;
					case OFF_WIDTH:
						v_width = value;
						break;
					case OFF_HEIGHT:
						v_height = value;
						break;
				}
			}
			point[offset] = v_width;
			point[offset + 1] = v_height;
		}

		internal double[] mTempValue = new double[18];
		internal double[] mTempDelta = new double[18];

		// Called on the start Time Point
		internal virtual void setView(float position, MotionWidget view, int[] toUse, double[] data, double[] slope, double[] cycle)
		{
			float v_x = x;
			float v_y = y;
			float v_width = width;
			float v_height = height;
			float dv_x = 0;
			float dv_y = 0;
			float dv_width = 0;
			float dv_height = 0;
			float delta_path = 0;
			float path_rotate = float.NaN;
			string mod;

			if (toUse.Length != 0 && mTempValue.Length <= toUse[toUse.Length - 1])
			{
				int scratch_data_length = toUse[toUse.Length - 1] + 1;
				mTempValue = new double[scratch_data_length];
				mTempDelta = new double[scratch_data_length];
			}
			//Arrays.fill(mTempValue, Double.NaN);
			mTempValue.Fill(Double.NaN);
			for (int i = 0; i < toUse.Length; i++)
			{
				mTempValue[toUse[i]] = data[i];
				mTempDelta[toUse[i]] = slope[i];
			}

			for (int i = 0; i < mTempValue.Length; i++)
			{
				if (double.IsNaN(mTempValue[i]) && (cycle == null || cycle[i] == 0.0))
				{
					continue;
				}
				double deltaCycle = (cycle != null) ? cycle[i] : 0.0;
				float value = (float)(double.IsNaN(mTempValue[i]) ? deltaCycle : mTempValue[i] + deltaCycle);
				float dvalue = (float) mTempDelta[i];

				switch (i)
				{
					case OFF_POSITION:
						delta_path = value;
						break;
					case OFF_X:
						v_x = value;
						dv_x = dvalue;

						break;
					case OFF_Y:
						v_y = value;
						dv_y = dvalue;
						break;
					case OFF_WIDTH:
						v_width = value;
						dv_width = dvalue;
						break;
					case OFF_HEIGHT:
						v_height = value;
						dv_height = dvalue;
						break;
					case OFF_PATH_ROTATE:
						path_rotate = value;
						break;
				}
			}

			if (mRelativeToController != null)
			{
				float[] pos = new float[2];
				float[] vel = new float[2];

				mRelativeToController.getCenter(position, pos, vel);
				float rx = pos[0];
				float ry = pos[1];
				float radius = v_x;
				float angle = v_y;
				float dradius = dv_x;
				float dangle = dv_y;
				float drx = vel[0];
				float dry = vel[1];

				// TODO Debug angle
				float pos_x = (float)(rx + radius * Math.Sin(angle) - v_width / 2);
				float pos_y = (float)(ry - radius * Math.Cos(angle) - v_height / 2);
				float dpos_x = (float)(drx + dradius * Math.Sin(angle) + radius * Math.Cos(angle) * dangle);
				float dpos_y = (float)(dry - dradius * Math.Cos(angle) + radius * Math.Sin(angle) * dangle);
				dv_x = dpos_x;
				dv_y = dpos_y;
				v_x = pos_x;
				v_y = pos_y;
				if (slope.Length >= 2)
				{
					slope[0] = dpos_x;
					slope[1] = dpos_y;
				}
				if (!float.IsNaN(path_rotate))
				{
					float rot = (float)(path_rotate + MathExtension.toDegrees(Math.Atan2(dv_y, dv_x)));
					view.RotationZ = rot;
				}

			}
			else
			{

				if (!float.IsNaN(path_rotate))
				{
					float rot = 0;
					float dx = dv_x + dv_width / 2;
					float dy = dv_y + dv_height / 2;
					if (DEBUG)
					{
						Utils.log(TAG, "dv_x       =" + dv_x);
						Utils.log(TAG, "dv_y       =" + dv_y);
						Utils.log(TAG, "dv_width   =" + dv_width);
						Utils.log(TAG, "dv_height  =" + dv_height);
					}
					rot += path_rotate + (float)MathExtension.toDegrees(Math.Atan2(dy, dx));
					view.RotationZ = rot;
					if (DEBUG)
					{
						Utils.log(TAG, "Rotated " + rot + "  = " + dx + "," + dy);
					}
				}
			}

		   // Todo: develop a concept of Float layout in MotionWidget widget.layout(float ...)
			int l = (int)(0.5f + v_x);
			int t = (int)(0.5f + v_y);
			int r = (int)(0.5f + v_x + v_width);
			int b = (int)(0.5f + v_y + v_height);
			int i_width = r - l;
			int i_height = b - t;
			if (OLD_WAY)
			{ // This way may produce more stable with and height but risk gaps
				l = (int)(v_x);
				t = (int)(v_y);
				i_width = (int)(v_width);
				i_height = (int)(v_height);
				r = l + i_width;
				b = t + i_height;
			}

			// MotionWidget must do Android View measure if layout changes
			view.layout(l, t, r, b);
			if (DEBUG)
			{
				if (toUse.Length > 0)
				{
					Utils.log(TAG, "setView " + mod);
				}
			}
		}

		internal virtual void getRect(int[] toUse, double[] data, float[] path, int offset)
		{
			float v_x = x;
			float v_y = y;
			float v_width = width;
			float v_height = height;
			float delta_path = 0;
			float rotation = 0;
			float alpha = 0;
			float rotationX = 0;
			float rotationY = 0;
			float scaleX = 1;
			float scaleY = 1;
			float pivotX = float.NaN;
			float pivotY = float.NaN;
			float translationX = 0;
			float translationY = 0;

			string mod;

			for (int i = 0; i < toUse.Length; i++)
			{
				float value = (float) data[i];

				switch (toUse[i])
				{
					case OFF_POSITION:
						delta_path = value;
						break;
					case OFF_X:
						v_x = value;
						break;
					case OFF_Y:
						v_y = value;
						break;
					case OFF_WIDTH:
						v_width = value;
						break;
					case OFF_HEIGHT:
						v_height = value;
						break;
				}
			}

			if (mRelativeToController != null)
			{
				float rx = mRelativeToController.CenterX;
				float ry = mRelativeToController.CenterY;
				float radius = v_x;
				float angle = v_y;
				// TODO Debug angle
				v_x = (float)(rx + radius * Math.Sin(angle) - v_width / 2);
				v_y = (float)(ry - radius * Math.Cos(angle) - v_height / 2);
			}

			float x1 = v_x;
			float y1 = v_y;

			float x2 = v_x + v_width;
			float y2 = y1;

			float x3 = x2;
			float y3 = v_y + v_height;

			float x4 = x1;
			float y4 = y3;

			float cx = x1 + v_width / 2;
			float cy = y1 + v_height / 2;

			if (!float.IsNaN(pivotX))
			{
				cx = x1 + (x2 - x1) * pivotX;
			}
			if (!float.IsNaN(pivotY))
			{

				cy = y1 + (y3 - y1) * pivotY;
			}
			if (scaleX != 1)
			{
				float midx = (x1 + x2) / 2;
				x1 = (x1 - midx) * scaleX + midx;
				x2 = (x2 - midx) * scaleX + midx;
				x3 = (x3 - midx) * scaleX + midx;
				x4 = (x4 - midx) * scaleX + midx;
			}
			if (scaleY != 1)
			{
				float midy = (y1 + y3) / 2;
				y1 = (y1 - midy) * scaleY + midy;
				y2 = (y2 - midy) * scaleY + midy;
				y3 = (y3 - midy) * scaleY + midy;
				y4 = (y4 - midy) * scaleY + midy;
			}
			if (rotation != 0)
			{
				float sin = (float) Math.Sin(MathExtension.toRadians(rotation));
				float cos = (float) Math.Cos(MathExtension.toRadians(rotation));
				float tx1 = xRotate(sin, cos, cx, cy, x1, y1);
				float ty1 = yRotate(sin, cos, cx, cy, x1, y1);
				float tx2 = xRotate(sin, cos, cx, cy, x2, y2);
				float ty2 = yRotate(sin, cos, cx, cy, x2, y2);
				float tx3 = xRotate(sin, cos, cx, cy, x3, y3);
				float ty3 = yRotate(sin, cos, cx, cy, x3, y3);
				float tx4 = xRotate(sin, cos, cx, cy, x4, y4);
				float ty4 = yRotate(sin, cos, cx, cy, x4, y4);
				x1 = tx1;
				y1 = ty1;
				x2 = tx2;
				y2 = ty2;
				x3 = tx3;
				y3 = ty3;
				x4 = tx4;
				y4 = ty4;
			}

			x1 += translationX;
			y1 += translationY;
			x2 += translationX;
			y2 += translationY;
			x3 += translationX;
			y3 += translationY;
			x4 += translationX;
			y4 += translationY;

			path[offset++] = x1;
			path[offset++] = y1;
			path[offset++] = x2;
			path[offset++] = y2;
			path[offset++] = x3;
			path[offset++] = y3;
			path[offset++] = x4;
			path[offset++] = y4;
		}

		/// <summary>
		/// mAnchorDpDt
		/// </summary>
		/// <param name="locationX"> </param>
		/// <param name="locationY"> </param>
		/// <param name="mAnchorDpDt"> </param>
		/// <param name="toUse"> </param>
		/// <param name="deltaData"> </param>
		/// <param name="data"> </param>
		internal virtual void setDpDt(float locationX, float locationY, float[] mAnchorDpDt, int[] toUse, double[] deltaData, double[] data)
		{

			float d_x = 0;
			float d_y = 0;
			float d_width = 0;
			float d_height = 0;

			float deltaScaleX = 0;
			float deltaScaleY = 0;

			float mPathRotate = float.NaN;
			float deltaTranslationX = 0;
			float deltaTranslationY = 0;

			string mod = " dd = ";
			for (int i = 0; i < toUse.Length; i++)
			{
				float deltaV = (float) deltaData[i];
				float value = (float) data[i];
				if (DEBUG)
				{
					mod += " , D" + names[toUse[i]] + "/Dt= " + deltaV;
				}
				switch (toUse[i])
				{
					case OFF_POSITION:
						break;
					case OFF_X:
						d_x = deltaV;
						break;
					case OFF_Y:
						d_y = deltaV;
						break;
					case OFF_WIDTH:
						d_width = deltaV;
						break;
					case OFF_HEIGHT:
						d_height = deltaV;
						break;

				}
			}
			if (DEBUG)
			{
				if (toUse.Length > 0)
				{
					Utils.log(TAG, "setDpDt " + mod);
				}
			}

			float deltaX = d_x - deltaScaleX * d_width / 2;
			float deltaY = d_y - deltaScaleY * d_height / 2;
			float deltaWidth = d_width * (1 + deltaScaleX);
			float deltaHeight = d_height * (1 + deltaScaleY);
			float deltaRight = deltaX + deltaWidth;
			float deltaBottom = deltaY + deltaHeight;
			if (DEBUG)
			{
				if (toUse.Length > 0)
				{

					Utils.log(TAG, "D x /dt           =" + d_x);
					Utils.log(TAG, "D y /dt           =" + d_y);
					Utils.log(TAG, "D width /dt       =" + d_width);
					Utils.log(TAG, "D height /dt      =" + d_height);
					Utils.log(TAG, "D deltaScaleX /dt =" + deltaScaleX);
					Utils.log(TAG, "D deltaScaleY /dt =" + deltaScaleY);
					Utils.log(TAG, "D deltaX /dt      =" + deltaX);
					Utils.log(TAG, "D deltaY /dt      =" + deltaY);
					Utils.log(TAG, "D deltaWidth /dt  =" + deltaWidth);
					Utils.log(TAG, "D deltaHeight /dt =" + deltaHeight);
					Utils.log(TAG, "D deltaRight /dt  =" + deltaRight);
					Utils.log(TAG, "D deltaBottom /dt =" + deltaBottom);
					Utils.log(TAG, "locationX         =" + locationX);
					Utils.log(TAG, "locationY         =" + locationY);
					Utils.log(TAG, "deltaTranslationX =" + deltaTranslationX);
					Utils.log(TAG, "deltaTranslationX =" + deltaTranslationX);
				}
			}

			mAnchorDpDt[0] = deltaX * (1 - locationX) + deltaRight * (locationX) + deltaTranslationX;
			mAnchorDpDt[1] = deltaY * (1 - locationY) + deltaBottom * (locationY) + deltaTranslationY;
		}

		internal virtual void fillStandard(double[] data, int[] toUse)
		{
			float[] set = new float[] {position, x, y, width, height, mPathRotate};
			int c = 0;
			for (int i = 0; i < toUse.Length; i++)
			{
				if (toUse[i] < set.Length)
				{
					data[c++] = set[toUse[i]];
				}
			}
		}

		internal virtual bool hasCustomData(string name)
		{
			return customAttributes.ContainsKey(name);
		}

		internal virtual int getCustomDataCount(string name)
		{
			CustomVariable a = customAttributes[name];
			if (a == null)
			{
				return 0;
			}
			return a.numberOfInterpolatedValues();
		}

		internal virtual int getCustomData(string name, double[] value, int offset)
		{
			CustomVariable a = customAttributes[name];
			if (a == null)
			{
				return 0;
			}
			else if (a.numberOfInterpolatedValues() == 1)
			{
				value[offset] = a.ValueToInterpolate;
				return 1;
			}
			else
			{
				int N = a.numberOfInterpolatedValues();
				float[] f = new float[N];
				a.getValuesToInterpolate(f);
				for (int i = 0; i < N; i++)
				{
					value[offset++] = f[i];
				}
				return N;
			}
		}

		internal virtual void setBounds(float x, float y, float w, float h)
		{
			this.x = x;
			this.y = y;
			width = w;
			height = h;
		}

		public virtual int CompareTo(MotionPaths o)
		{
			return position.CompareTo(o.position);
		}

		public virtual void applyParameters(MotionWidget c)
		{
			MotionPaths point = this;
			point.mKeyFrameEasing = Easing.getInterpolator(c.motion.mTransitionEasing);
			point.mPathMotionArc = c.motion.mPathMotionArc;
			point.mAnimateRelativeTo = c.motion.mAnimateRelativeTo;
			point.mPathRotate = c.motion.mPathRotate;
			point.mDrawPath = c.motion.mDrawPath;
			point.mAnimateCircleAngleTo = c.motion.mAnimateCircleAngleTo;
			point.mProgress = c.propertySet.mProgress;
			point.mRelativeAngle = 0; // c.layout.circleAngle;
			ICollection<string> at = c.CustomAttributeNames;
			foreach (string s in at)
			{
				CustomVariable attr = c.getCustomAttribute(s);
				if (attr != null && attr.Continuous)
				{
					this.customAttributes[s] = attr;
				}
			}
		}

		public virtual void configureRelativeTo(Motion toOrbit)
		{
			double[] p = toOrbit.getPos(mProgress); // get the position in the orbit
		}
	}

}