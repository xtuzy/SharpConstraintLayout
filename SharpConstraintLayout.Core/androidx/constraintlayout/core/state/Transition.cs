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

namespace androidx.constraintlayout.core.state
{
	using Motion = androidx.constraintlayout.core.motion.Motion;
	using MotionWidget = androidx.constraintlayout.core.motion.MotionWidget;
	using MotionKeyAttributes = androidx.constraintlayout.core.motion.key.MotionKeyAttributes;
	using MotionKeyCycle = androidx.constraintlayout.core.motion.key.MotionKeyCycle;
	using MotionKeyPosition = androidx.constraintlayout.core.motion.key.MotionKeyPosition;
	using Easing = androidx.constraintlayout.core.motion.utils.Easing;
	using KeyCache = androidx.constraintlayout.core.motion.utils.KeyCache;
	using TypedBundle = androidx.constraintlayout.core.motion.utils.TypedBundle;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;


	public class Transition
	{
		private Dictionary<string, WidgetState> state = new Dictionary<string, WidgetState>();
		internal Dictionary<int?, Dictionary<string, KeyPosition>> keyPositions = new Dictionary<int?, Dictionary<string, KeyPosition>>();

		public const int START = 0;
		public const int END = 1;
		public const int INTERPOLATED = 2;

		private int pathMotionArc = -1;
		// Interpolation
		private int mDefaultInterpolator = 0;
		private string mDefaultInterpolatorString = null;
		private const int SPLINE_STRING = -1;
		private const int INTERPOLATOR_REFERENCE_ID = -2;
		internal const int EASE_IN_OUT = 0;
		internal const int EASE_IN = 1;
		internal const int EASE_OUT = 2;
		internal const int LINEAR = 3;
		internal const int BOUNCE = 4;
		internal const int OVERSHOOT = 5;
		internal const int ANTICIPATE = 6;

		private int mAutoTransition = 0;
		private int mDuration = 400;
		private float mStagger = 0.0f;

		internal virtual KeyPosition findPreviousPosition(string target, int frameNumber)
		{
			while (frameNumber >= 0)
			{
				Dictionary<string, KeyPosition> map = keyPositions[frameNumber];
				if (map != null)
				{
					KeyPosition keyPosition = map[target];
					if (keyPosition != null)
					{
						return keyPosition;
					}
				}
				frameNumber--;
			}
			return null;
		}

		internal virtual KeyPosition findNextPosition(string target, int frameNumber)
		{
			while (frameNumber <= 100)
			{
				Dictionary<string, KeyPosition> map = keyPositions[frameNumber];
				if (map != null)
				{
					KeyPosition keyPosition = map[target];
					if (keyPosition != null)
					{
						return keyPosition;
					}
				}
				frameNumber++;
			}
			return null;
		}

		public virtual int getNumberKeyPositions(WidgetFrame frame)
		{
			int numKeyPositions = 0;
			int frameNumber = 0;
			while (frameNumber <= 100)
			{
				Dictionary<string, KeyPosition> map = keyPositions[frameNumber];
				if (map != null)
				{
					KeyPosition keyPosition = map[frame.widget.stringId];
					if (keyPosition != null)
					{
						numKeyPositions++;
					}
				}
				frameNumber++;
			}
			return numKeyPositions;
		}

		public virtual Motion getMotion(string id)
		{
			return getWidgetState(id, null, 0).motionControl;
		}

		public virtual void fillKeyPositions(WidgetFrame frame, float[] x, float[] y, float[] pos)
		{
			int numKeyPositions = 0;
			int frameNumber = 0;
			while (frameNumber <= 100)
			{
				Dictionary<string, KeyPosition> map = keyPositions[frameNumber];
				if (map != null)
				{
					KeyPosition keyPosition = map[frame.widget.stringId];
					if (keyPosition != null)
					{
						x[numKeyPositions] = keyPosition.x;
						y[numKeyPositions] = keyPosition.y;
						pos[numKeyPositions] = keyPosition.frame;
						numKeyPositions++;
					}
				}
				frameNumber++;
			}
		}

		public virtual bool hasPositionKeyframes()
		{
			return keyPositions.Count > 0;
		}

		public virtual TypedBundle TransitionProperties
		{
			set
			{
				pathMotionArc = value.getInteger(TypedValues.TypedValues_Position.TYPE_PATH_MOTION_ARC);
				mAutoTransition = value.getInteger(TypedValues.TypedValues_Transition.TYPE_AUTO_TRANSITION);
			}
		}

		internal class WidgetState
		{
			internal WidgetFrame start;
			internal WidgetFrame end;
			internal WidgetFrame interpolated;
			internal Motion motionControl;
			internal MotionWidget motionWidgetStart;
			internal MotionWidget motionWidgetEnd;
			internal MotionWidget motionWidgetInterpolated;
			internal KeyCache myKeyCache = new KeyCache();
			internal int myParentHeight = -1;
			internal int myParentWidth = -1;

			public WidgetState()
			{
				start = new WidgetFrame();
				end = new WidgetFrame();
				interpolated = new WidgetFrame();
				motionWidgetStart = new MotionWidget(start);
				motionWidgetEnd = new MotionWidget(end);
				motionWidgetInterpolated = new MotionWidget(interpolated);
				motionControl = new Motion(motionWidgetStart);
				motionControl.Start = motionWidgetStart;
				motionControl.End = motionWidgetEnd;
			}

			public virtual TypedBundle KeyPosition
			{
				set
				{
					MotionKeyPosition keyPosition = new MotionKeyPosition();
					value.applyDelta(keyPosition);
					motionControl.addKey(keyPosition);
				}
			}

			public virtual TypedBundle KeyAttribute
			{
				set
				{
					MotionKeyAttributes keyAttributes = new MotionKeyAttributes();
					value.applyDelta(keyAttributes);
					motionControl.addKey(keyAttributes);
				}
			}

			public virtual TypedBundle KeyCycle
			{
				set
				{
					MotionKeyCycle keyAttributes = new MotionKeyCycle();
					value.applyDelta(keyAttributes);
					motionControl.addKey(keyAttributes);
				}
			}

			public virtual void update(ConstraintWidget child, int state)
			{
				if (state == START)
				{
					start.update(child);
					motionControl.Start = motionWidgetStart;
				}
				else if (state == END)
				{
					end.update(child);
					motionControl.End = motionWidgetEnd;
				}
				myParentWidth = -1;
			}

			public virtual WidgetFrame getFrame(int type)
			{
				if (type == START)
				{
					return start;
				}
				else if (type == END)
				{
					return end;
				}
				return interpolated;
			}

			public virtual void interpolate(int parentWidth, int parentHeight, float progress, Transition transition)
			{
				if (true || parentHeight != myParentHeight || parentWidth != myParentWidth)
				{
					myParentHeight = parentHeight;
					myParentWidth = parentWidth;
					motionControl.setup(parentWidth, parentHeight, 1, DateTimeHelperClass.nanoTime());
				}
				WidgetFrame.interpolate(parentWidth, parentHeight, interpolated, start, end, transition, progress);
				interpolated.interpolatedPos = progress;
				motionControl.interpolate(motionWidgetInterpolated, progress, DateTimeHelperClass.nanoTime(), myKeyCache);
			}
		}

		internal class KeyPosition
		{
			internal int frame;
			internal string target;
			internal int type;
			internal float x;
			internal float y;

			public KeyPosition(string target, int frame, int type, float x, float y)
			{
				this.target = target;
				this.frame = frame;
				this.type = type;
				this.x = x;
				this.y = y;
			}
		}

		public virtual bool Empty
		{
			get
			{
				return state.Count == 0;
			}
		}

		public virtual void clear()
		{
			state.Clear();
		}

		public virtual bool contains(string key)
		{
			return state.ContainsKey(key);
		}

		public virtual void addKeyPosition(string target, TypedBundle bundle)
		{
			getWidgetState(target, null, 0).KeyPosition = bundle;
		}

		public virtual void addKeyAttribute(string target, TypedBundle bundle)
		{
			getWidgetState(target, null, 0).KeyAttribute = bundle;
		}

		public virtual void addKeyCycle(string target, TypedBundle bundle)
		{
			getWidgetState(target, null, 0).KeyCycle = bundle;
		}

		public virtual void addKeyPosition(string target, int frame, int type, float x, float y)
		{
			TypedBundle bundle = new TypedBundle();
			bundle.add(TypedValues.TypedValues_Position.TYPE_POSITION_TYPE, 2);
			bundle.add(TypedValues.TYPE_FRAME_POSITION, frame);
			bundle.add(TypedValues.TypedValues_Position.TYPE_PERCENT_X, x);
			bundle.add(TypedValues.TypedValues_Position.TYPE_PERCENT_Y, y);
			getWidgetState(target, null, 0).KeyPosition = bundle;

			KeyPosition keyPosition = new KeyPosition(target, frame, type, x, y);
			Dictionary<string, KeyPosition> map = keyPositions[frame];
			if (map == null)
			{
				map = new Dictionary<string, KeyPosition>();
				keyPositions[frame] = map;
			}
			map[target] = keyPosition;
		}

		public virtual void addCustomFloat(int state, string widgetId, string property, float value)
		{
			WidgetState widgetState = getWidgetState(widgetId, null, state);
			WidgetFrame frame = widgetState.getFrame(state);
			frame.addCustomFloat(property, value);
		}

		public virtual void addCustomColor(int state, string widgetId, string property, int color)
		{
			WidgetState widgetState = getWidgetState(widgetId, null, state);
			WidgetFrame frame = widgetState.getFrame(state);
			frame.addCustomColor(property, color);
		}

		public virtual void updateFrom(ConstraintWidgetContainer container, int state)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<androidx.constraintlayout.core.widgets.ConstraintWidget> children = container.getChildren();
			List<ConstraintWidget> children = container.Children;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = children.size();
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget child = children[i];
				WidgetState widgetState = getWidgetState(child.stringId, null, state);
				widgetState.update(child, state);
			}
		}

		public virtual void interpolate(int parentWidth, int parentHeight, float progress)
		{
			foreach (string key in state.Keys)
			{
				WidgetState widget = state[key];
				widget.interpolate(parentWidth, parentHeight, progress, this);
			}
		}

		public virtual WidgetFrame getStart(string id)
		{
			WidgetState widgetState = state[id];
			if (widgetState == null)
			{
				return null;
			}
			return widgetState.start;
		}

		public virtual WidgetFrame getEnd(string id)
		{
			WidgetState widgetState = state[id];
			if (widgetState == null)
			{
				return null;
			}
			return widgetState.end;
		}

		public virtual WidgetFrame getInterpolated(string id)
		{
			WidgetState widgetState = state[id];
			if (widgetState == null)
			{
				return null;
			}
			return widgetState.interpolated;
		}

		public virtual float[] getPath(string id)
		{
			WidgetState widgetState = state[id];
			int duration = 1000;
			int frames = duration / 16;
			float[] mPoints = new float[frames * 2];
			widgetState.motionControl.buildPath(mPoints, frames);
			return mPoints;
		}

		public virtual int getKeyFrames(string id, float[] rectangles, int[] pathMode, int[] position)
		{
			WidgetState widgetState = state[id];
			return widgetState.motionControl.buildKeyFrames(rectangles, pathMode, position);
		}

		private WidgetState getWidgetState(string widgetId)
		{
			return this.state[widgetId];
		}

		private WidgetState getWidgetState(string widgetId, ConstraintWidget child, int transitionState)
		{
			WidgetState widgetState = this.state[widgetId];
			if (widgetState == null)
			{
				widgetState = new WidgetState();
				if (pathMotionArc != -1)
				{
					widgetState.motionControl.PathMotionArc = pathMotionArc;
				}
				state[widgetId] = widgetState;
				if (child != null)
				{
					widgetState.update(child, transitionState);
				}
			}
			return widgetState;
		}

		/// <summary>
		/// Used in debug draw
		/// </summary>
		/// <param name="child">
		/// @return </param>
		public virtual WidgetFrame getStart(ConstraintWidget child)
		{
			return getWidgetState(child.stringId, null, Transition.START).start;
		}

		/// <summary>
		/// Used in debug draw
		/// </summary>
		/// <param name="child">
		/// @return </param>
		public virtual WidgetFrame getEnd(ConstraintWidget child)
		{
			return getWidgetState(child.stringId, null, Transition.END).end;
		}

		/// <summary>
		/// Used after the interpolation
		/// </summary>
		/// <param name="child">
		/// @return </param>
		public virtual WidgetFrame getInterpolated(ConstraintWidget child)
		{
			return getWidgetState(child.stringId, null, Transition.INTERPOLATED).interpolated;
		}

		public virtual Interpolator Interpolator
		{
			get
			{
				return getInterpolator(mDefaultInterpolator, mDefaultInterpolatorString);
			}
		}

		public static Interpolator getInterpolator(int interpolator, string interpolatorString)
		{
			switch (interpolator)
			{
				case SPLINE_STRING:
					return v => (float) Easing.getInterpolator(interpolatorString).get(v);
				case EASE_IN_OUT:
					return v => (float) Easing.getInterpolator("standard").get(v);
				case EASE_IN:
					return v => (float) Easing.getInterpolator("accelerate").get(v);
				case EASE_OUT:
					return v => (float) Easing.getInterpolator("decelerate").get(v);
				case LINEAR:
					return v => (float) Easing.getInterpolator("linear").get(v);
				case ANTICIPATE:
					return v => (float) Easing.getInterpolator("anticipate").get(v);
				case OVERSHOOT:
					return v => (float) Easing.getInterpolator("overshoot").get(v);
				case BOUNCE: // TODO make a better bounce
					return v => (float) Easing.getInterpolator("spline(0.0, 0.2, 0.4, 0.6, 0.8 ,1.0, 0.8, 1.0, 0.9, 1.0)").get(v);
			}
			return null;
		}

		public virtual int AutoTransition
		{
			get
			{
				return mAutoTransition;
			}
		}
	}

}