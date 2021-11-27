using System;
using System.Collections.Generic;
using System.Drawing;

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
namespace androidx.constraintlayout.core.scout
{
	using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using Guideline = androidx.constraintlayout.core.widgets.Guideline;
	using WidgetContainer = androidx.constraintlayout.core.widgets.WidgetContainer;


	/// <summary>
	/// Main Wrapper class for Constraint Widgets
	/// </summary>
	public class ScoutWidget : IComparable<ScoutWidget>
	{
		private const bool DEBUG = false;
		private const float MAXIMUM_STRETCH_GAP = 0.6f; // percentage
		private float mX;
		private float mY;
		private float mWidth;
		private float mHeight;
		private float mBaseLine;
		private ScoutWidget mParent;
		private float mRootDistance;
		private float[] mDistToRootCache = new float[] {-1, -1, -1, -1};
		internal ConstraintWidget mConstraintWidget;
		private bool mKeepExistingConnections = true;
		private Rectangle mRectangle;

		public ScoutWidget(ConstraintWidget constraintWidget, ScoutWidget parent)
		{
			this.mConstraintWidget = constraintWidget;
			this.mParent = parent;
			this.mX = constraintWidget.X;
			this.mY = constraintWidget.Y;
			this.mWidth = constraintWidget.Width;
			this.mHeight = constraintWidget.Height;
			this.mBaseLine = mConstraintWidget.BaselineDistance + constraintWidget.Y;
			if (parent != null)
			{
				mRootDistance = distance(parent, this);
			}
		}

		/// <summary>
		/// Sets the order root first
		/// followed by outside to inside, top to bottom, left to right
		/// </summary>
		/// <param name="scoutWidget">
		/// @return </param>
		public virtual int CompareTo(ScoutWidget scoutWidget)
		{
			if (mParent == null)
			{
				return -1;
			}
			if (mRootDistance != scoutWidget.mRootDistance)
			{
				return mRootDistance.CompareTo(scoutWidget.mRootDistance);
			}
			if (mY != scoutWidget.mY)
			{
				return mY.CompareTo(scoutWidget.mY);
			}
			if (mX != scoutWidget.mX)
			{
				return mX.CompareTo(scoutWidget.mX);
			}
			return 0;
		}

		public override string ToString()
		{
			return mConstraintWidget.DebugName;
		}

		internal virtual bool Root
		{
			get
			{
				return mParent == null;
			}
		}

		/// <summary>
		/// is this a guideline
		/// 
		/// @return
		/// </summary>
		public virtual bool getGuideline()
		{
			return mConstraintWidget is Guideline;
		}

		/// <summary>
		/// is guideline vertical
		/// 
		/// @return
		/// </summary>
		public virtual bool VerticalGuideline
		{
			get
			{
				if (mConstraintWidget is Guideline)
				{
					Guideline g = (Guideline) mConstraintWidget;
					return g.Orientation == Guideline.VERTICAL;
				}
				return false;
			}
		}

		/// <summary>
		/// is this a horizontal guide line on the image
		/// 
		/// @return
		/// </summary>
		public virtual bool HorizontalGuideline
		{
			get
			{
				if (mConstraintWidget is Guideline)
				{
					Guideline g = (Guideline) mConstraintWidget;
					return g.Orientation == Guideline.HORIZONTAL;
				}
				return false;
			}
		}

		/// <summary>
		/// Wrap an array of ConstraintWidgets into an array of InferWidgets
		/// </summary>
		/// <param name="array">
		/// @return </param>
		public static ScoutWidget[] create(ConstraintWidget[] array)
		{
			ScoutWidget[] ret = new ScoutWidget[array.Length];
			ConstraintWidget root = array[0];

			ScoutWidget rootwidget = new ScoutWidget(root, null);
			ret[0] = rootwidget;
			int count = 1;
			for (int i = 0; i < ret.Length; i++)
			{
				if (array[i] != root)
				{
					ret[count++] = new ScoutWidget(array[i], rootwidget);
				}
			}
			Array.Sort(ret);
			if (DEBUG)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					Console.WriteLine("[" + i + "] -> " + ret[i].mConstraintWidget + "    " + ret[i].mRootDistance);
				}
			}
			return ret;
		}

		// above = 0, below = 1, left = 2, right = 3
		internal virtual float getLocation(Direction dir)
		{
			switch (dir.innerEnumValue)
			{
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.NORTH:
					return mY;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.SOUTH:
					return mY + mHeight;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.WEST:
					return mX;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.EAST:
					return mX + mWidth;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.BASE:
					return mBaseLine;
			}
			return mBaseLine;
		}

		/// <summary>
		/// simple accessor for the height
		/// </summary>
		/// <returns> the height of the widget </returns>
		public virtual float getHeight()
		{
			return mHeight;
		}

		/// <summary>
		/// simple accessor for the width
		/// </summary>
		/// <returns> the width of the widget </returns>
		public virtual float getWidth()
		{
			return mWidth;
		}

		/// <summary>
		/// simple accessor for the X position
		/// </summary>
		/// <returns> the X position of the widget </returns>
		internal float getX()
		{
			return mX;
		}

		/// <summary>
		/// simple accessor for the Y position
		/// </summary>
		/// <returns> the Y position of the widget </returns>
		internal float getY()
		{
			return mY;
		}

		/// <summary>
		/// This calculates a constraint tables and applies them to the widgets
		/// TODO break up into creation of a constraint table and apply
		/// </summary>
		/// <param name="list"> ordered list of widgets root must be list[0] </param>
		public static void computeConstraints(ScoutWidget[] list)
		{
			ScoutProbabilities table = new ScoutProbabilities();
			table.computeConstraints(list);
			table.applyConstraints(list);
		}

		private static ConstraintAnchor.Type lookupType(int dir)
		{
			return lookupType(Direction.get(dir));
		}

		/// <summary>
		/// map integer direction to ConstraintAnchor.Type
		/// </summary>
		/// <param name="dir"> integer direction
		/// @return </param>
		private static ConstraintAnchor.Type lookupType(Direction dir)
		{
			switch (dir.innerEnumValue)
			{
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.NORTH:
					return ConstraintAnchor.Type.TOP;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.SOUTH:
					return ConstraintAnchor.Type.BOTTOM;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.WEST:
					return ConstraintAnchor.Type.LEFT;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.EAST:
					return ConstraintAnchor.Type.RIGHT;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.BASE:
					return ConstraintAnchor.Type.BASELINE;
			}
			return ConstraintAnchor.Type.NONE;
		}

		/// <summary>
		/// set a centered constraint if possible return true if it did
		/// </summary>
		/// <param name="dir">   direction 0 = vertical </param>
		/// <param name="to1">   first widget  to connect to </param>
		/// <param name="to2">   second widget to connect to </param>
		/// <param name="cDir1"> the side of first widget to connect to </param>
		/// <param name="cDir2"> the sed of the second widget to connect to </param>
		/// <param name="gap">   the gap </param>
		/// <returns> true if it was able to connect </returns>
		internal virtual bool setCentered(int dir, ScoutWidget to1, ScoutWidget to2, Direction cDir1, Direction cDir2, float gap)
		{
			Direction ori = (dir == 0) ? Direction.NORTH : Direction.WEST;
			ConstraintAnchor anchor1 = mConstraintWidget.getAnchor(lookupType(ori));
			ConstraintAnchor anchor2 = mConstraintWidget.getAnchor(lookupType(ori.Opposite));

			if (mKeepExistingConnections && (anchor1.Connected || anchor2.Connected))
			{
				if (anchor1.Connected ^ anchor2.Connected)
				{
					return false;
				}
				if (anchor1.Connected && (anchor1.Target.Owner != to1.mConstraintWidget))
				{
					return false;
				}
				if (anchor2.Connected && (anchor2.Target.Owner != to2.mConstraintWidget))
				{
					return false;
				}
			}

			if (anchor1.isConnectionAllowed(to1.mConstraintWidget) && anchor2.isConnectionAllowed(to2.mConstraintWidget))
			{
				// Resize
				if (!isResizable(dir))
				{
					if (dir == 0)
					{
						int height = mConstraintWidget.Height;
						float stretchRatio = (gap * 2) / (float) height;
						if (isCandidateResizable(dir) && stretchRatio < MAXIMUM_STRETCH_GAP)
						{
							mConstraintWidget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
						}
						else
						{
							gap = 0;
						}
					}
					else
					{
						int width = mConstraintWidget.Width;
						float stretchRatio = (gap * 2) / (float) width;
						if (isCandidateResizable(dir) && stretchRatio < MAXIMUM_STRETCH_GAP)
						{
							mConstraintWidget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
						}
						else
						{
							gap = 0;
						}
					}
				}

				if (to1.Equals(to2))
				{
					connect(mConstraintWidget, lookupType(cDir1), to1.mConstraintWidget, lookupType(cDir1), (int) gap);
					connect(mConstraintWidget, lookupType(cDir2), to2.mConstraintWidget, lookupType(cDir2), (int) gap);
				}
				else
				{

					float pos1 = to1.getLocation(cDir1);
					float pos2 = to2.getLocation(cDir2);
					Direction c1 = (pos1 < pos2) ? (ori) : (ori.Opposite);
					Direction c2 = (pos1 > pos2) ? (ori) : (ori.Opposite);
					int gap1 = Gap(mConstraintWidget, c1, to1.mConstraintWidget, cDir1);
					int gap2 = Gap(mConstraintWidget, c2, to2.mConstraintWidget, cDir2);

					connect(mConstraintWidget, lookupType(c1), to1.mConstraintWidget, lookupType(cDir1), Math.Max(0, gap1));
					connect(mConstraintWidget, lookupType(c2), to2.mConstraintWidget, lookupType(cDir2), Math.Max(0, gap2));
				}
				return true;
			}
			else
			{

				return false;
			}
		}

		/// <summary>
		/// Get the gap between two specific edges of widgets </summary>
		/// <param name="widget1"> </param>
		/// <param name="direction1"> </param>
		/// <param name="widget2"> </param>
		/// <param name="direction2"> </param>
		/// <returns> distance in dp </returns>
		private static int Gap(ConstraintWidget widget1, Direction direction1, ConstraintWidget widget2, Direction direction2)
		{
			switch (direction1.innerEnumValue)
			{
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.NORTH:
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.WEST:
					return getPos(widget1, direction1) - getPos(widget2, direction2);
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.SOUTH:
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.EAST:
					return getPos(widget2, direction2) - getPos(widget1, direction1);
			}
			return 0;
		}

		/// <summary>
		/// Get the position of a edge of a widget </summary>
		/// <param name="widget"> </param>
		/// <param name="direction">
		/// @return </param>
		private static int getPos(ConstraintWidget widget, Direction direction)
		{
			switch (direction.innerEnumValue)
			{
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.NORTH:
					return widget.Y;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.SOUTH:
					return widget.Y + widget.Height;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.WEST:
					return widget.X;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.EAST:
					return widget.X + widget.Width;
			}
			return 0;
		}

		/// <summary>
		/// set a centered constraint if possible return true if it did
		/// </summary>
		/// <param name="dir">   direction 0 = vertical </param>
		/// <param name="to1">   first widget  to connect to </param>
		/// <param name="cDir1"> the side of first widget to connect to </param>
		/// <returns> true if it was able to connect </returns>
		internal virtual bool setEdgeCentered(int dir, ScoutWidget to1, Direction cDir1)
		{
			Direction ori = (dir == 0) ? Direction.NORTH : Direction.WEST;
			ConstraintAnchor anchor1 = mConstraintWidget.getAnchor(lookupType(ori));
			ConstraintAnchor anchor2 = mConstraintWidget.getAnchor(lookupType(ori.Opposite));

			if (mKeepExistingConnections && (anchor1.Connected || anchor2.Connected))
			{
				if (anchor1.Connected ^ anchor2.Connected)
				{
					return false;
				}
				if (anchor1.Connected && (anchor1.Target.Owner != to1.mConstraintWidget))
				{
					return false;
				}
			}

			if (anchor1.isConnectionAllowed(to1.mConstraintWidget))
			{
				connect(mConstraintWidget, lookupType(ori), to1.mConstraintWidget, lookupType(cDir1), 0);
				connect(mConstraintWidget, lookupType(ori.Opposite), to1.mConstraintWidget, lookupType(cDir1), 0);
			}
			return true;
		}


		private static void connect(ConstraintWidget fromWidget, ConstraintAnchor.Type fromType, ConstraintWidget toWidget, ConstraintAnchor.Type toType, int gap)
		{
			fromWidget.connect(fromType, toWidget, toType, gap);
	//        fromWidget.getAnchor(fromType).setConnectionCreator(ConstraintAnchor.SCOUT_CREATOR);
		}

		private static void connectWeak(ConstraintWidget fromWidget, ConstraintAnchor.Type fromType, ConstraintWidget toWidget, ConstraintAnchor.Type toType, int gap)
		{
			fromWidget.connect(fromType, toWidget, toType, gap);
	//        fromWidget.connect(fromType, toWidget, toType, gap, ConstraintAnchor.Strength.WEAK);
	//        fromWidget.getAnchor(fromType).setConnectionCreator(ConstraintAnchor.SCOUT_CREATOR);
		}

		/// <summary>
		/// set a constraint if possible return true if it did
		/// </summary>
		/// <param name="dir">  the direction of the connection </param>
		/// <param name="to">   the widget to connect to </param>
		/// <param name="cDir"> the direction of </param>
		/// <param name="gap"> </param>
		/// <returns> false if unable to apply </returns>
		internal virtual bool setConstraint(int dir, ScoutWidget to, int cDir, float gap)
		{
			ConstraintAnchor.Type anchorType = lookupType(dir);
			if (to.getGuideline())
			{
				cDir &= 0x2;
			}
			ConstraintAnchor anchor = mConstraintWidget.getAnchor(anchorType);

			if (mKeepExistingConnections)
			{
				if (anchor.Connected)
				{
					if (anchor.Target.Owner != to.mConstraintWidget)
					{
						return false;
					}
					return true;
				}
				if (dir == Direction.BASE.getDirection())
				{
					if (mConstraintWidget.getAnchor(ConstraintAnchor.Type.BOTTOM).Connected)
					{
						return false;
					}
					if (mConstraintWidget.getAnchor(ConstraintAnchor.Type.TOP).Connected)
					{
						return false;
					}
				}
				else if (dir == Direction.NORTH.getDirection())
				{
					if (mConstraintWidget.getAnchor(ConstraintAnchor.Type.BOTTOM).Connected)
					{
						return false;
					}
					if (mConstraintWidget.getAnchor(ConstraintAnchor.Type.BASELINE).Connected)
					{
						return false;
					}
				}
				else if (dir == Direction.SOUTH.getDirection())
				{
					if (mConstraintWidget.getAnchor(ConstraintAnchor.Type.TOP).Connected)
					{
						return false;
					}
					if (mConstraintWidget.getAnchor(ConstraintAnchor.Type.BASELINE).Connected)
					{
						return false;
					}
				}
				else if (dir == Direction.WEST.getDirection())
				{
					if (mConstraintWidget.getAnchor(ConstraintAnchor.Type.RIGHT).Connected)
					{
						return false;
					}
				}
				else if (dir == Direction.EAST.getDirection())
				{
					if (mConstraintWidget.getAnchor(ConstraintAnchor.Type.LEFT).Connected)
					{
						return false;
					}

				}
			}

			if (anchor.isConnectionAllowed(to.mConstraintWidget))
			{
				connect(mConstraintWidget, lookupType(dir), to.mConstraintWidget, lookupType(cDir), (int) gap);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// set a Weak constraint if possible return true if it did
		/// </summary>
		/// <param name="dir">  the direction of the connection </param>
		/// <param name="to">   the widget to connect to </param>
		/// <param name="cDir"> the direction of </param>
		/// <returns> false if unable to apply </returns>
		internal virtual bool setWeakConstraint(int dir, ScoutWidget to, int cDir)
		{
			ConstraintAnchor anchor = mConstraintWidget.getAnchor(lookupType(dir));
			float gap = 8f;

			if (mKeepExistingConnections && anchor.Connected)
			{
				if (anchor.Target.Owner != to.mConstraintWidget)
				{
					return false;
				}
				return true;
			}

			if (anchor.isConnectionAllowed(to.mConstraintWidget))
			{
				if (DEBUG)
				{
					Console.WriteLine("WEAK CONSTRAINT " + mConstraintWidget + " to " + to.mConstraintWidget);
				}
				connectWeak(mConstraintWidget, lookupType(dir), to.mConstraintWidget, lookupType(cDir), (int) gap);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// calculates the distance between two widgets (assumed to be rectangles)
		/// </summary>
		/// <param name="a"> </param>
		/// <param name="b"> </param>
		/// <returns> the distance between two widgets at there closest point to each other </returns>
		internal static float distance(ScoutWidget a, ScoutWidget b)
		{
			float ax1, ax2, ay1, ay2;
			float bx1, bx2, by1, by2;
			ax1 = a.mX;
			ax2 = a.mX + a.mWidth;
			ay1 = a.mY;
			ay2 = a.mY + a.mHeight;

			bx1 = b.mX;
			bx2 = b.mX + b.mWidth;
			by1 = b.mY;
			by2 = b.mY + b.mHeight;
			float xdiff11 = Math.Abs(ax1 - bx1);
			float xdiff12 = Math.Abs(ax1 - bx2);
			float xdiff21 = Math.Abs(ax2 - bx1);
			float xdiff22 = Math.Abs(ax2 - bx2);

			float ydiff11 = Math.Abs(ay1 - by1);
			float ydiff12 = Math.Abs(ay1 - by2);
			float ydiff21 = Math.Abs(ay2 - by1);
			float ydiff22 = Math.Abs(ay2 - by2);

			float xmin = Math.Min(Math.Min(xdiff11, xdiff12), Math.Min(xdiff21, xdiff22));
			float ymin = Math.Min(Math.Min(ydiff11, ydiff12), Math.Min(ydiff21, ydiff22));

			bool yOverlap = ay1 <= by2 && by1 <= ay2;
			bool xOverlap = ax1 <= bx2 && bx1 <= ax2;
			float xReturn = (yOverlap) ? xmin : (float)MathExtension.hypot(xmin, ymin);
			float yReturn = (xOverlap) ? ymin : (float)MathExtension.hypot(xmin, ymin);
			return Math.Min(xReturn, yReturn);
		}

		public virtual ScoutWidget Parent
		{
			get
			{
				return mParent;
			}
		}

		/// <summary>
		/// Return true if the widget is a candidate to be marked
		/// as resizable (ANY) -- i.e. if the current dimension is bigger than its minimum.
		/// </summary>
		/// <param name="dimension"> the dimension (vertical == 0, horizontal == 1) we are looking at </param>
		/// <returns> true if the widget is a good candidate for resize </returns>
		public virtual bool isCandidateResizable(int dimension)
		{
			if (dimension == 0)
			{
				return mConstraintWidget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT || ((mConstraintWidget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.FIXED) && mConstraintWidget.Height > mConstraintWidget.MinHeight);
			}
			else
			{
				return (mConstraintWidget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT) || ((mConstraintWidget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.FIXED) && mConstraintWidget.Width > mConstraintWidget.MinWidth);
			}
		}

		public virtual bool isResizable(int horizontal)
		{
			if (horizontal == 0)
			{
				return mConstraintWidget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
			}
			else
			{
				return mConstraintWidget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

			}
		}

		public virtual bool hasBaseline()
		{
			return mConstraintWidget.hasBaseline();
		}

		/// <summary>
		/// Gets the neighbour in that direction or root
		/// TODO better support for large widgets with several neighbouring widgets
		/// </summary>
		/// <param name="dir"> </param>
		/// <param name="list">
		/// @return </param>
		public virtual ScoutWidget getNeighbor(Direction dir, ScoutWidget[] list)
		{
			ScoutWidget neigh = list[0];
			float minDist = float.MaxValue;

			switch (dir.innerEnumValue)
			{
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.WEST:
				{
					float ay1 = this.getLocation(Direction.NORTH);
					float ay2 = this.getLocation(Direction.SOUTH);
					float ax = this.getLocation(Direction.WEST);

					for (int i = 1; i < list.Length; i++)
					{
						ScoutWidget iw = list[i];
						if (iw == this)
						{
							continue;
						}
						float by1 = iw.getLocation(Direction.NORTH);
						float by2 = iw.getLocation(Direction.SOUTH);
						if (Math.Max(ay1, by1) <= Math.Min(ay2, by2))
						{ // overlap
							float bx = iw.getLocation(Direction.EAST);
							if (bx < ax && (ax - bx) < minDist)
							{
								minDist = (ax - bx);
								neigh = iw;
							}
						}
					}
					return neigh;
				}
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.EAST:
				{
					float ay1 = this.getLocation(Direction.NORTH);
					float ay2 = this.getLocation(Direction.SOUTH);
					float ax = this.getLocation(Direction.EAST);

					for (int i = 1; i < list.Length; i++)
					{
						ScoutWidget iw = list[i];
						if (iw == this)
						{
							continue;
						}
						float by1 = iw.getLocation(Direction.NORTH);
						float by2 = iw.getLocation(Direction.SOUTH);
						if (Math.Max(ay1, by1) <= Math.Min(ay2, by2))
						{ // overlap
							float bx = iw.getLocation(Direction.WEST);
							if (bx > ax && (bx - ax) < minDist)
							{
								minDist = (bx - ax);
								neigh = iw;
							}
						}
					}
					return neigh;
				}
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.SOUTH:
				{
					float ax1 = this.getLocation(Direction.WEST);
					float ax2 = this.getLocation(Direction.EAST);
					float ay = this.getLocation(Direction.SOUTH);

					for (int i = 1; i < list.Length; i++)
					{
						ScoutWidget iw = list[i];
						if (iw == this)
						{
							continue;
						}
						float bx1 = iw.getLocation(Direction.WEST);
						float bx2 = iw.getLocation(Direction.EAST);
						if (Math.Max(ax1, bx1) <= Math.Min(ax2, bx2))
						{ // overlap
							float by = iw.getLocation(Direction.NORTH);
							if (by > ay && (by - ay) < minDist)
							{
								minDist = (by - ay);
								neigh = iw;
							}
						}
					}
					return neigh;
				}
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.NORTH:
				{
					float ax1 = this.getLocation(Direction.WEST);
					float ax2 = this.getLocation(Direction.EAST);
					float ay = this.getLocation(Direction.NORTH);

					for (int i = 1; i < list.Length; i++)
					{
						ScoutWidget iw = list[i];
						if (iw == this)
						{
							continue;
						}
						float bx1 = iw.getLocation(Direction.WEST);
						float bx2 = iw.getLocation(Direction.EAST);
						if (Math.Max(ax1, bx1) <= Math.Min(ax2, bx2))
						{ // overlap
							float by = iw.getLocation(Direction.SOUTH);
							if (ay > by && (ay - by) < minDist)
							{
								minDist = (ay - by);
								neigh = iw;
							}
						}
					}
					return neigh;
				}
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.BASE:
				default:
					return null;
			}
		}

		/// <summary>
		/// is the widet connected in that direction
		/// </summary>
		/// <param name="direction"> </param>
		/// <returns> true if connected </returns>
		public virtual bool isConnected(Direction direction)
		{
			return mConstraintWidget.getAnchor(lookupType(direction)).Connected;
		}

		/// <summary>
		/// is the distance to the Root Cached
		/// </summary>
		/// <param name="direction"> </param>
		/// <returns> true if distance to root has been cached </returns>
		private bool isDistanceToRootCache(Direction direction)
		{
			int directionOrdinal = direction.getDirection();
			float? f = mDistToRootCache[directionOrdinal];
			if (f < 0)
			{ // depends on any comparison involving Float.NaN returns false
				return false;
			}
			return true;
		}

		/// <summary>
		/// Get the cache distance to the root
		/// </summary>
		/// <param name="d"> </param>
		/// <param name="value"> </param>
		private void cacheRootDistance(Direction d, float value)
		{
			mDistToRootCache[d.getDirection()] = value;
		}

		/// <summary>
		/// get distance to the container in a direction
		/// caches the distance
		/// </summary>
		/// <param name="list">      list of widgets (container is list[0] </param>
		/// <param name="direction"> direction to check in </param>
		/// <returns> distance root or NaN if no connection available </returns>
		public virtual float connectedDistanceToRoot(ScoutWidget[] list, Direction direction)
		{
			float value = recursiveConnectedDistanceToRoot(list, direction);
			cacheRootDistance(direction, value);
			return value;
		}

		/// <summary>
		/// Walk the widget connections to get the distance to the container in a direction
		/// </summary>
		/// <param name="list">      list of widgets (container is list[0] </param>
		/// <param name="direction"> direction to check in </param>
		/// <returns> distance root or NaN if no connection available </returns>
		private float recursiveConnectedDistanceToRoot(ScoutWidget[] list, Direction direction)
		{

			if (isDistanceToRootCache(direction))
			{
				return mDistToRootCache[direction.getDirection()];
			}
			ConstraintAnchor.Type anchorType = lookupType(direction);
			ConstraintAnchor anchor = mConstraintWidget.getAnchor(anchorType);

			if (anchor == null || !anchor.Connected)
			{
				return float.NaN;
			}
			float margin = anchor.Margin;
			ConstraintAnchor toAnchor = anchor.Target;

			ConstraintWidget toWidget = toAnchor.Owner;
			if (list[0].mConstraintWidget == toWidget)
			{ // found the base return;
				return margin;
			}

			// if atached to the same side
			if (toAnchor.getType() == anchorType)
			{
				foreach (ScoutWidget scoutWidget in list)
				{
					if (scoutWidget.mConstraintWidget == toWidget)
					{
						float dist = scoutWidget.recursiveConnectedDistanceToRoot(list, direction);
						scoutWidget.cacheRootDistance(direction, dist);
						return margin + dist;
					}
				}
			}
			// if atached to the other side (you will need to add the length of the widget
			if (toAnchor.getType() == lookupType(direction.Opposite))
			{
				foreach (ScoutWidget scoutWidget in list)
				{
					if (scoutWidget.mConstraintWidget == toWidget)
					{
						margin += scoutWidget.getLength(direction);
						float dist = scoutWidget.recursiveConnectedDistanceToRoot(list, direction);
						scoutWidget.cacheRootDistance(direction, dist);
						return margin + dist;
					}
				}
			}
			return float.NaN;
		}

		/// <summary>
		/// Get size of widget
		/// </summary>
		/// <param name="direction"> the direction north/south gets height east/west gets width </param>
		/// <returns> size of widget in that dimension </returns>
		private float getLength(Direction direction)
		{
			switch (direction.innerEnumValue)
			{
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.NORTH:
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.SOUTH:
					return mHeight;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.EAST:
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.WEST:
					return mWidth;
				default:
					return 0;
			}
		}

		/// <summary>
		/// is the widget centered
		/// </summary>
		/// <param name="orientationVertical"> 1 = checking if vertical </param>
		/// <returns> true if centered </returns>
		public virtual bool isCentered(int orientationVertical)
		{
			if (getGuideline())
			{
				return false;
			}
			if (orientationVertical == Direction.ORIENTATION_VERTICAL)
			{
				return mConstraintWidget.getAnchor(ConstraintAnchor.Type.TOP).Connected && mConstraintWidget.getAnchor(ConstraintAnchor.Type.BOTTOM).Connected;
			}
			return mConstraintWidget.getAnchor(ConstraintAnchor.Type.LEFT).Connected && mConstraintWidget.getAnchor(ConstraintAnchor.Type.RIGHT).Connected;
		}

		public virtual bool hasConnection(Direction dir)
		{
			ConstraintAnchor anchor = mConstraintWidget.getAnchor(lookupType(dir));
			return (anchor != null && anchor.Connected);
		}

		public virtual Rectangle Rectangle
		{
			get
			{
				if (mRectangle == null)
				{
					mRectangle = new Rectangle();
				}
				mRectangle.X = mConstraintWidget.X;
				mRectangle.Y = mConstraintWidget.Y;
				mRectangle.Width = mConstraintWidget.Width;
				mRectangle.Height = mConstraintWidget.Height;
				return mRectangle;
			}
		}

		internal static ScoutWidget[] getWidgetArray(WidgetContainer @base)
		{
			List<ConstraintWidget> list = new List<ConstraintWidget>(@base.Children);
			list.Insert(0, @base);
			return create(list.ToArray());
		}

		/// <summary>
		/// Calculate the gap in to the nearest widget
		/// </summary>
		/// <param name="direction">  the direction to check </param>
		/// <param name="list"> list of other widgets (root == list[0]) </param>
		/// <returns> the distance on that side </returns>
		public virtual int gap(Direction direction, ScoutWidget[] list)
		{
			int rootWidth = list[0].mConstraintWidget.Width;
			int rootHeight = list[0].mConstraintWidget.Height;
			Rectangle rect = new Rectangle();

			switch (direction.innerEnumValue)
			{
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.NORTH:
				{
					rect.Y = 0;
					rect.X = mConstraintWidget.X + 1;
					rect.Width = mConstraintWidget.Width - 2;
					rect.Height = mConstraintWidget.Y;
				}
				break;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.SOUTH:
				{
					rect.Y = mConstraintWidget.Y + mConstraintWidget.Height;
					rect.X = mConstraintWidget.X + 1;
					rect.Width = mConstraintWidget.Width - 2;
					rect.Height = rootHeight - rect.Y;
				}
				break;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.WEST:
				{
					rect.Y = mConstraintWidget.Y + 1;
					rect.X = 0;
					rect.Width = mConstraintWidget.X;
					rect.Height = mConstraintWidget.Height - 2;

				}
				break;
				case androidx.constraintlayout.core.scout.Direction.InnerEnum.EAST:
				{
					rect.Y = mConstraintWidget.Y + 1;
					rect.X = mConstraintWidget.X + mConstraintWidget.Width;
					rect.Width = rootWidth - rect.X;
					rect.Height = mConstraintWidget.Height - 2;
				}
				break;

			}
			int min = int.MaxValue;
			for (int i = 1; i < list.Length; i++)
			{
				ScoutWidget scoutWidget = list[i];
				if (scoutWidget == this)
				{
					continue;
				}
				Rectangle r = scoutWidget.Rectangle;
				if (r.IntersectsWith(rect))
				{
					int dist = (int) distance(scoutWidget, this);
					if (min > dist)
					{
						min = dist;
					}
				}
			}

			if (min > Math.Max(rootHeight, rootWidth))
			{
				switch (direction.innerEnumValue)
				{
					case androidx.constraintlayout.core.scout.Direction.InnerEnum.NORTH:
						return mConstraintWidget.Y;
					case androidx.constraintlayout.core.scout.Direction.InnerEnum.SOUTH:
						return rootHeight - (mConstraintWidget.Y + mConstraintWidget.Height);

					case androidx.constraintlayout.core.scout.Direction.InnerEnum.WEST:
						return mConstraintWidget.X;

					case androidx.constraintlayout.core.scout.Direction.InnerEnum.EAST:
						return rootWidth - (mConstraintWidget.X + mConstraintWidget.Width);
				}
			}
			return min;
		}

		public virtual void setX(int x)
		{
			mConstraintWidget.X = x;
			mX = mConstraintWidget.X;
		}

		public virtual void setY(int y)
		{
			mConstraintWidget.Y = y;
			mY = mConstraintWidget.Y;
		}

		public virtual void setWidth(int width)
		{
			mConstraintWidget.Width = width;
			mWidth = mConstraintWidget.Width;
		}

		public virtual void setHeight(int height)
		{
			mConstraintWidget.Height = height;
			mHeight = mConstraintWidget.Height;
		}

		/// <summary>
		/// Comparator to sort widgets by y
		/// </summary>
		internal static IComparer<ScoutWidget> sSortY = new ComparatorAnonymousInnerClass();

		private class ComparatorAnonymousInnerClass : IComparer<ScoutWidget>
		{
			public ComparatorAnonymousInnerClass()
			{
			}

			public virtual int Compare(ScoutWidget o1, ScoutWidget o2)
			{
				return o1.mConstraintWidget.Y - o2.mConstraintWidget.Y;
			}
		}

		public virtual int rootDistanceY()
		{
			if (mConstraintWidget == null || mConstraintWidget.Parent == null)
			{
				return 0;
			}
			int rootHeight = mConstraintWidget.Parent.Height;
			int aY = mConstraintWidget.Y;
			int aHeight = mConstraintWidget.Height;
			return Math.Min(aY, rootHeight - (aY + aHeight));
		}
	}

}