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
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.FIXED;
using static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.HORIZONTAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;

	/// <summary>
	/// Implements a simple grouping mechanism, to group interdependent widgets together.
	/// 
	/// TODO: we should move towards a more leaner implementation -- this is more expensive as it could be.
	/// </summary>
	public class Grouping
	{

		private const bool DEBUG = false;
		private const bool DEBUG_GROUPING = false;


		public static bool validInGroup(ConstraintWidget.DimensionBehaviour layoutHorizontal, ConstraintWidget.DimensionBehaviour layoutVertical, ConstraintWidget.DimensionBehaviour widgetHorizontal, ConstraintWidget.DimensionBehaviour widgetVertical)
		{
			bool fixedHorizontal = widgetHorizontal == FIXED || widgetHorizontal == WRAP_CONTENT || (widgetHorizontal == MATCH_PARENT && layoutHorizontal != WRAP_CONTENT);
			bool fixedVertical = widgetVertical == FIXED || widgetVertical == WRAP_CONTENT || (widgetVertical == MATCH_PARENT && layoutVertical != WRAP_CONTENT);
			if (fixedHorizontal || fixedVertical)
			{
				return true;
			}
			return false;
		}

		public static bool simpleSolvingPass(ConstraintWidgetContainer layout, BasicMeasure.Measurer measurer)
		{

			if (DEBUG)
			{
				Console.WriteLine("*** GROUP SOLVING ***");
			}
			List<ConstraintWidget> children = layout.Children;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = children.size();
			int count = children.Count;

			List<Guideline> verticalGuidelines = null;
			List<Guideline> horizontalGuidelines = null;
			List<HelperWidget> horizontalBarriers = null;
			List<HelperWidget> verticalBarriers = null;
			List<ConstraintWidget> isolatedHorizontalChildren = null;
			List<ConstraintWidget> isolatedVerticalChildren = null;

			for (int i = 0; i < count; i++)
			{
				ConstraintWidget child = children[i];
				if (!validInGroup(layout.HorizontalDimensionBehaviour, layout.VerticalDimensionBehaviour, child.HorizontalDimensionBehaviour, child.VerticalDimensionBehaviour))
				{
					if (DEBUG)
					{
						Console.WriteLine("*** NO GROUP SOLVING ***");
					}
					return false;
				}
				if (child is Flow)
				{
					return false;
				}
			}
			if (layout.mMetrics != null)
			{
				layout.mMetrics.grouping++;
			}
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget child = children[i];
				if (!validInGroup(layout.HorizontalDimensionBehaviour, layout.VerticalDimensionBehaviour, child.HorizontalDimensionBehaviour, child.VerticalDimensionBehaviour))
				{
					ConstraintWidgetContainer.measure(0, child, measurer, layout.mMeasure, BasicMeasure.Measure.SELF_DIMENSIONS);
				}
				if (child is Guideline)
				{
					Guideline guideline = (Guideline) child;
					if (guideline.Orientation == HORIZONTAL)
					{
						if (horizontalGuidelines == null)
						{
							horizontalGuidelines = new List<Guideline>();
						}
						horizontalGuidelines.Add(guideline);
					}
					if (guideline.Orientation == VERTICAL)
					{
						if (verticalGuidelines == null)
						{
							verticalGuidelines = new List<Guideline>();
						}
						verticalGuidelines.Add(guideline);
					}
				}
				if (child is HelperWidget)
				{
					if (child is Barrier)
					{
						Barrier barrier = (Barrier) child;
						if (barrier.Orientation == HORIZONTAL)
						{
							if (horizontalBarriers == null)
							{
								horizontalBarriers = new List<HelperWidget>();
							}
							horizontalBarriers.Add(barrier);
						}
						if (barrier.Orientation == VERTICAL)
						{
							if (verticalBarriers == null)
							{
								verticalBarriers = new List<HelperWidget>();
							}
							verticalBarriers.Add(barrier);
						}
					}
					else
					{
						HelperWidget helper = (HelperWidget) child;
						if (horizontalBarriers == null)
						{
							horizontalBarriers = new List<HelperWidget>();
						}
						horizontalBarriers.Add(helper);
						if (verticalBarriers == null)
						{
							verticalBarriers = new List<HelperWidget>();
						}
						verticalBarriers.Add(helper);
					}
				}
				if (child.mLeft.mTarget == null && child.mRight.mTarget == null && !(child is Guideline) && !(child is Barrier))
				{
					if (isolatedHorizontalChildren == null)
					{
						isolatedHorizontalChildren = new List<ConstraintWidget>();
					}
					isolatedHorizontalChildren.Add(child);
				}
				if (child.mTop.mTarget == null && child.mBottom.mTarget == null && child.mBaseline.mTarget == null && !(child is Guideline) && !(child is Barrier))
				{
					if (isolatedVerticalChildren == null)
					{
						isolatedVerticalChildren = new List<ConstraintWidget>();
					}
					isolatedVerticalChildren.Add(child);
				}
			}
			List<WidgetGroup> allDependencyLists = new List<WidgetGroup>();

			if (true || layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
			{
				List<WidgetGroup> dependencyLists = allDependencyLists; //horizontalDependencyLists; //new ArrayList<>();

				if (verticalGuidelines != null)
				{
					foreach (Guideline guideline in verticalGuidelines)
					{
						findDependents(guideline, HORIZONTAL, dependencyLists, null);
					}
				}
				if (horizontalBarriers != null)
				{
					foreach (HelperWidget barrier in horizontalBarriers)
					{
						WidgetGroup group = findDependents(barrier, HORIZONTAL, dependencyLists, null);
						barrier.addDependents(dependencyLists, HORIZONTAL, group);
						group.cleanup(dependencyLists);
					}
				}

				ConstraintAnchor left = layout.getAnchor(ConstraintAnchor.Type.LEFT);
				if (left.Dependents != null)
				{
					foreach (ConstraintAnchor first in left.Dependents)
					{
						findDependents(first.mOwner, ConstraintWidget.HORIZONTAL, dependencyLists, null);
					}
				}

				ConstraintAnchor right = layout.getAnchor(ConstraintAnchor.Type.RIGHT);
				if (right.Dependents != null)
				{
					foreach (ConstraintAnchor first in right.Dependents)
					{
						findDependents(first.mOwner, ConstraintWidget.HORIZONTAL, dependencyLists, null);
					}
				}

				ConstraintAnchor center = layout.getAnchor(ConstraintAnchor.Type.CENTER);
				if (center.Dependents != null)
				{
					foreach (ConstraintAnchor first in center.Dependents)
					{
						findDependents(first.mOwner, ConstraintWidget.HORIZONTAL, dependencyLists, null);
					}
				}

				if (isolatedHorizontalChildren != null)
				{
					foreach (ConstraintWidget widget in isolatedHorizontalChildren)
					{
						findDependents(widget, HORIZONTAL, dependencyLists, null);
					}
				}
			}

			if (true || layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
			{
				List<WidgetGroup> dependencyLists = allDependencyLists; //verticalDependencyLists; //new ArrayList<>();

				if (horizontalGuidelines != null)
				{
					foreach (Guideline guideline in horizontalGuidelines)
					{
						findDependents(guideline, VERTICAL, dependencyLists, null);
					}
				}
				if (verticalBarriers != null)
				{
					foreach (HelperWidget barrier in verticalBarriers)
					{
						WidgetGroup group = findDependents(barrier, VERTICAL, dependencyLists, null);
						barrier.addDependents(dependencyLists, VERTICAL, group);
						group.cleanup(dependencyLists);
					}
				}

				ConstraintAnchor top = layout.getAnchor(ConstraintAnchor.Type.TOP);
				if (top.Dependents != null)
				{
					foreach (ConstraintAnchor first in top.Dependents)
					{
						findDependents(first.mOwner, VERTICAL, dependencyLists, null);
					}
				}

				ConstraintAnchor baseline = layout.getAnchor(ConstraintAnchor.Type.BASELINE);
				if (baseline.Dependents != null)
				{
					foreach (ConstraintAnchor first in baseline.Dependents)
					{
						findDependents(first.mOwner, VERTICAL, dependencyLists, null);
					}
				}

				ConstraintAnchor bottom = layout.getAnchor(ConstraintAnchor.Type.BOTTOM);
				if (bottom.Dependents != null)
				{
					foreach (ConstraintAnchor first in bottom.Dependents)
					{
						findDependents(first.mOwner, VERTICAL, dependencyLists, null);
					}
				}

				ConstraintAnchor center = layout.getAnchor(ConstraintAnchor.Type.CENTER);
				if (center.Dependents != null)
				{
					foreach (ConstraintAnchor first in center.Dependents)
					{
						findDependents(first.mOwner, VERTICAL, dependencyLists, null);
					}
				}

				if (isolatedVerticalChildren != null)
				{
					foreach (ConstraintWidget widget in isolatedVerticalChildren)
					{
						findDependents(widget, VERTICAL, dependencyLists, null);
					}
				}
			}
			// Now we may have to merge horizontal/vertical dependencies
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget child = children[i];
				if (child.oppositeDimensionsTied())
				{
					WidgetGroup horizontalGroup = findGroup(allDependencyLists, child.horizontalGroup);
					WidgetGroup verticalGroup = findGroup(allDependencyLists, child.verticalGroup);
					if (horizontalGroup != null && verticalGroup != null)
					{
						if (DEBUG_GROUPING)
						{
							Console.WriteLine("Merging " + horizontalGroup + " to " + verticalGroup + " for " + child);
						}
						horizontalGroup.moveTo(HORIZONTAL, verticalGroup);
						verticalGroup.Orientation = BOTH;
						allDependencyLists.Remove(horizontalGroup);
					}
				}
				if (DEBUG_GROUPING)
				{
					Console.WriteLine("Widget " + child + " => " + child.horizontalGroup + " : " + child.verticalGroup);
				}
			}

			if (allDependencyLists.Count <= 1)
			{
				return false;
			}

			if (DEBUG)
			{
				Console.WriteLine("----------------------------------");
				Console.WriteLine("-- Horizontal dependency lists:");
				Console.WriteLine("----------------------------------");
				foreach (WidgetGroup list in allDependencyLists)
				{
					if (list.Orientation != VERTICAL)
					{
						Console.WriteLine("list: " + list);
					}
				}
				Console.WriteLine("----------------------------------");
				Console.WriteLine("-- Vertical dependency lists:");
				Console.WriteLine("----------------------------------");
				foreach (WidgetGroup list in allDependencyLists)
				{
					if (list.Orientation != HORIZONTAL)
					{
						Console.WriteLine("list: " + list);
					}
				}
				Console.WriteLine("----------------------------------");
			}

			WidgetGroup horizontalPick = null;
			WidgetGroup verticalPick = null;

			if (layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
			{
				int maxWrap = 0;
				WidgetGroup picked = null;
				foreach (WidgetGroup list in allDependencyLists)
				{
					if (list.Orientation == VERTICAL)
					{
						continue;
					}
					list.Authoritative = false;
					int wrap = list.measureWrap(layout.System, HORIZONTAL);
					if (wrap > maxWrap)
					{
						picked = list;
						maxWrap = wrap;
					}
					if (DEBUG)
					{
						Console.WriteLine("list: " + list + " => " + wrap);
					}
				}
				if (picked != null)
				{
					if (DEBUG)
					{
						Console.WriteLine("Horizontal MaxWrap : " + maxWrap + " with group " + picked);
					}
					layout.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
					layout.Width = maxWrap;
					picked.Authoritative = true;
					horizontalPick = picked;
				}
			}

			if (layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
			{
				int maxWrap = 0;
				WidgetGroup picked = null;
				foreach (WidgetGroup list in allDependencyLists)
				{
					if (list.Orientation == HORIZONTAL)
					{
						continue;
					}
					list.Authoritative = false;
					int wrap = list.measureWrap(layout.System, VERTICAL);
					if (wrap > maxWrap)
					{
						picked = list;
						maxWrap = wrap;
					}
					if (DEBUG)
					{
						Console.WriteLine("      " + list + " => " + wrap);
					}
				}
				if (picked != null)
				{
					if (DEBUG)
					{
						Console.WriteLine("Vertical MaxWrap : " + maxWrap + " with group " + picked);
					}
					layout.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
					layout.Height = maxWrap;
					picked.Authoritative = true;
					verticalPick = picked;
				}
			}
			return horizontalPick != null || verticalPick != null;
		}

		private static WidgetGroup findGroup(List<WidgetGroup> horizontalDependencyLists, int groupId)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = horizontalDependencyLists.size();
			int count = horizontalDependencyLists.Count;
			for (int i = 0; i < count; i++)
			{
				WidgetGroup group = horizontalDependencyLists[i];
				if (groupId == group.id)
				{
					return group;
				}
			}
			return null;
		}

		public static WidgetGroup findDependents(ConstraintWidget constraintWidget, int orientation, List<WidgetGroup> list, WidgetGroup group)
		{
			int groupId = -1;
			if (orientation == ConstraintWidget.HORIZONTAL)
			{
				groupId = constraintWidget.horizontalGroup;
			}
			else
			{
				groupId = constraintWidget.verticalGroup;
			}
			if (DEBUG_GROUPING)
			{
				Console.WriteLine("--- find " + (orientation == HORIZONTAL ? "Horiz" : "Vert") + " dependents of " + constraintWidget.DebugName + " group " + group + " widget group id " + groupId);
			}
			if (groupId != -1 && (group == null || (groupId != group.id)))
			{
				// already in a group!
				if (DEBUG_GROUPING)
				{
					Console.WriteLine("widget " + constraintWidget.DebugName + " already in group " + groupId + " group: " + group);
				}
				for (int i = 0; i < list.Count; i++)
				{
					WidgetGroup widgetGroup = list[i];
					if (widgetGroup.Id == groupId)
					{
						if (group != null)
						{
							if (DEBUG_GROUPING)
							{
								Console.WriteLine("Move group " + group + " to " + widgetGroup);
							}
							group.moveTo(orientation, widgetGroup);
							list.Remove(group);
						}
						group = widgetGroup;
						break;
					}
				}
			}
			else if (groupId != -1)
			{
				return group;
			}
			if (group == null)
			{
				if (constraintWidget is HelperWidget)
				{
					HelperWidget helper = (HelperWidget) constraintWidget;
					groupId = helper.findGroupInDependents(orientation);
					if (groupId != -1)
					{
						for (int i = 0; i < list.Count; i++)
						{
							WidgetGroup widgetGroup = list[i];
							if (widgetGroup.Id == groupId)
							{
								group = widgetGroup;
								break;
							}
						}
					}
				}
				if (group == null)
				{
					group = new WidgetGroup(orientation);
				}
				if (DEBUG_GROUPING)
				{
					Console.WriteLine("Create group " + group + " for widget " + constraintWidget.DebugName);
				}
				list.Add(group);
			}
			if (group.add(constraintWidget))
			{
				if (constraintWidget is Guideline)
				{
					Guideline guideline = (Guideline) constraintWidget;
					guideline.Anchor.findDependents(guideline.Orientation == Guideline.HORIZONTAL ? VERTICAL : HORIZONTAL, list, group);
				}
				if (orientation == ConstraintWidget.HORIZONTAL)
				{
					constraintWidget.horizontalGroup = group.Id;
					if (DEBUG_GROUPING)
					{
						Console.WriteLine("Widget " + constraintWidget.DebugName + " H group is " + constraintWidget.horizontalGroup);
					}
					constraintWidget.mLeft.findDependents(orientation, list, group);
					constraintWidget.mRight.findDependents(orientation, list, group);
				}
				else
				{
					constraintWidget.verticalGroup = group.Id;
					if (DEBUG_GROUPING)
					{
						Console.WriteLine("Widget " + constraintWidget.DebugName + " V group is " + constraintWidget.verticalGroup);
					}
					constraintWidget.mTop.findDependents(orientation, list, group);
					constraintWidget.mBaseline.findDependents(orientation, list, group);
					constraintWidget.mBottom.findDependents(orientation, list, group);
				}
				constraintWidget.mCenter.findDependents(orientation, list, group);
			}
			return group;
		}
	}

}