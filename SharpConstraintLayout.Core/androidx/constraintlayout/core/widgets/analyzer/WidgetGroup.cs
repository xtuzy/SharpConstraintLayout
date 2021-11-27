using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2020 The Android Open Source Project
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
namespace androidx.constraintlayout.core.widgets.analyzer
{


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.BOTH;
using static androidx.constraintlayout.core.widgets.ConstraintWidget;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.HORIZONTAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;

	/// <summary>
	/// Represents a group of widget for the grouping mechanism.
	/// </summary>
	public class WidgetGroup
	{
		private const bool DEBUG = false;
		internal List<ConstraintWidget> widgets = new List<ConstraintWidget>();
		internal static int count = 0;
		internal int id = -1;
		internal bool authoritative = false;
		internal int orientation = HORIZONTAL;
		internal List<MeasureResult> results = null;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private int moveTo_Renamed = -1;

		public WidgetGroup(int orientation)
		{
			id = count++;
			this.orientation = orientation;
		}

		public virtual int Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				this.orientation = value;
			}
		}
		public virtual int Id
		{
			get
			{
				return id;
			}
		}

		public virtual bool add(ConstraintWidget widget)
		{
			if (widgets.Contains(widget))
			{
				return false;
			}
			widgets.Add(widget);
			return true;
		}

		public virtual bool Authoritative
		{
			set
			{
				authoritative = value;
			}
			get
			{
				return authoritative;
			}
		}

		private string OrientationString
		{
			get
			{
				if (orientation == HORIZONTAL)
				{
					return "Horizontal";
				}
				else if (orientation == VERTICAL)
				{
					return "Vertical";
				}
				else if (orientation == BOTH)
				{
					return "Both";
				}
				return "Unknown";
			}
		}

		public override string ToString()
		{
			string ret = OrientationString + " [" + id + "] <";
			foreach (ConstraintWidget widget in widgets)
			{
				ret += " " + widget.DebugName;
			}
			ret += " >";
			return ret;
		}

		public virtual void moveTo(int orientation, WidgetGroup widgetGroup)
		{
			if (DEBUG)
			{
				Console.WriteLine("Move all widgets (" + this + ") from " + id + " to " + widgetGroup.Id + "(" + widgetGroup + ")");
			}
			foreach (ConstraintWidget widget in widgets)
			{
				widgetGroup.add(widget);
				if (orientation == HORIZONTAL)
				{
					widget.horizontalGroup = widgetGroup.Id;
				}
				else
				{
					widget.verticalGroup = widgetGroup.Id;
				}
			}
			moveTo_Renamed = widgetGroup.id;
		}

		public virtual void clear()
		{
			widgets.Clear();
		}

		private int measureWrap(int orientation, ConstraintWidget widget)
		{
			ConstraintWidget.DimensionBehaviour behaviour = widget.getDimensionBehaviour(orientation).Value;
			if (behaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT || behaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT || behaviour == ConstraintWidget.DimensionBehaviour.FIXED)
			{
				int dimension;
				if (orientation == HORIZONTAL)
				{
					dimension = widget.Width;
				}
				else
				{
					dimension = widget.Height;
				}
				return dimension;
			}
			return -1;
		}

		public virtual int measureWrap(LinearSystem system, int orientation)
		{
			int count = widgets.Count;
			if (count == 0)
			{
				return 0;
			}
			// TODO: add direct wrap computation for simpler cases instead of calling the solver
			return solverMeasure(system, widgets, orientation);
		}

		private int solverMeasure(LinearSystem system, List<ConstraintWidget> widgets, int orientation)
		{
			ConstraintWidgetContainer container = (ConstraintWidgetContainer) widgets[0].Parent;
			system.reset();
			bool prevDebug = LinearSystem.FULL_DEBUG;
			container.addToSolver(system, false);
			for (int i = 0; i < widgets.Count; i++)
			{
				ConstraintWidget widget = widgets[i];
				widget.addToSolver(system, false);
			}
			if (orientation == HORIZONTAL)
			{
				if (container.mHorizontalChainsSize > 0)
				{
					Chain.applyChainConstraints(container, system, widgets, HORIZONTAL);
				}
			}
			if (orientation == VERTICAL)
			{
				if (container.mVerticalChainsSize > 0)
				{
					Chain.applyChainConstraints(container, system, widgets, VERTICAL);
				}
			}

			try
			{
				system.minimize();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			// save results
			results = new List<MeasureResult>();
			for (int i = 0; i < widgets.Count; i++)
			{
				ConstraintWidget widget = widgets[i];
				MeasureResult result = new MeasureResult(this, widget, system, orientation);
				results.Add(result);
			}

			if (orientation == HORIZONTAL)
			{
				int left = system.getObjectVariableValue(container.mLeft);
				int right = system.getObjectVariableValue(container.mRight);
				system.reset();
				return right - left;
			}
			else
			{
				int top = system.getObjectVariableValue(container.mTop);
				int bottom = system.getObjectVariableValue(container.mBottom);
				system.reset();
				return bottom - top;
			}
		}


		public virtual void apply()
		{
			if (results == null)
			{
				return;
			}
			if (!authoritative)
			{
				return;
			}
			for (int i = 0; i < results.Count; i++)
			{
				MeasureResult result = results[i];
				result.apply();
			}
		}

		public virtual bool intersectWith(WidgetGroup group)
		{
			for (int i = 0; i < widgets.Count; i++)
			{
				ConstraintWidget widget = widgets[i];
				if (group.contains(widget))
				{
					return true;
				}
			}
			return false;
		}

		private bool contains(ConstraintWidget widget)
		{
			return widgets.Contains(widget);
		}

		public virtual int size()
		{
			return widgets.Count;
		}

		public virtual void cleanup(List<WidgetGroup> dependencyLists)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = widgets.size();
			int count = widgets.Count;
			if (moveTo_Renamed != -1 && count > 0)
			{
				for (int i = 0; i < dependencyLists.Count; i++)
				{
					WidgetGroup group = dependencyLists[i];
					if (moveTo_Renamed == group.id)
					{
						moveTo(orientation, group);
					}
				}
			}
			if (count == 0)
			{
				dependencyLists.Remove(this);
				return;
			}
		}


		internal class MeasureResult
		{
			private readonly WidgetGroup outerInstance;

			internal WeakReference<ConstraintWidget> widgetRef;
			internal int left;
			internal int top;
			internal int right;
			internal int bottom;
			internal int baseline;
			internal int orientation;

			public MeasureResult(WidgetGroup outerInstance, ConstraintWidget widget, LinearSystem system, int orientation)
			{
				this.outerInstance = outerInstance;
				widgetRef = new WeakReference<ConstraintWidget>(widget);
				left = system.getObjectVariableValue(widget.mLeft);
				top = system.getObjectVariableValue(widget.mTop);
				right = system.getObjectVariableValue(widget.mRight);
				bottom = system.getObjectVariableValue(widget.mBottom);
				baseline = system.getObjectVariableValue(widget.mBaseline);
				this.orientation = orientation;
			}

			public virtual void apply()
			{
				ConstraintWidget widget;
				widgetRef.TryGetTarget(out widget);
				if (widget != null)
				{
					widget.setFinalFrame(left, top, right, bottom, baseline, orientation);
				}
			}
		}
	}

}