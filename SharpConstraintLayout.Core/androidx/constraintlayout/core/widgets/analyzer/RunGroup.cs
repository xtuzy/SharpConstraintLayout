﻿using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2019 The Android Open Source Project
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
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.HORIZONTAL;
using static androidx.constraintlayout.core.widgets.ConstraintWidget;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;

	internal class RunGroup
	{
		public const int START = 0;
		public const int END = 1;
		public const int BASELINE = 2;

		public static int index;

		public int position = 0;
		public bool dual = false;

		internal WidgetRun firstRun = null;
		internal WidgetRun lastRun = null;
		internal List<WidgetRun> runs = new List<WidgetRun>();

		internal int groupIndex = 0;
		internal int direction;

		public RunGroup(WidgetRun run, int dir)
		{
			groupIndex = index;
			index++;
			firstRun = run;
			lastRun = run;
			direction = dir;
		}

		public virtual void add(WidgetRun run)
		{
			runs.Add(run);
			lastRun = run;
		}

		private long traverseStart(DependencyNode node, long startPosition)
		{
			WidgetRun run = node.run;
			if (run is HelperReferences)
			{
				return startPosition;
			}
			long position = startPosition;

			// first, compute stuff dependent on this node.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = node.dependencies.size();
			int count = node.dependencies.Count;
			for (int i = 0; i < count; i++)
			{
					Dependency dependency = node.dependencies[i];
					if (dependency is DependencyNode)
					{
						DependencyNode nextNode = (DependencyNode) dependency;
						if (nextNode.run == run)
						{
							// skip our own sibling node
							continue;
						}
						position = Math.Max(position, traverseStart(nextNode, startPosition + nextNode.margin));
					}
			}

			if (node == run.start)
			{
				// let's go for our sibling
				long dimension = run.WrapDimension;
				position = Math.Max(position, traverseStart(run.end, startPosition + dimension));
				position = Math.Max(position, startPosition + dimension - run.end.margin);
			}

			return position;
		}

		private long traverseEnd(DependencyNode node, long startPosition)
		{
			WidgetRun run = node.run;
			if (run is HelperReferences)
			{
				return startPosition;
			}
			long position = startPosition;

			// first, compute stuff dependent on this node.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = node.dependencies.size();
			int count = node.dependencies.Count;
			for (int i = 0; i < count; i++)
			{
				Dependency dependency = node.dependencies[i];
				if (dependency is DependencyNode)
				{
					DependencyNode nextNode = (DependencyNode) dependency;
					if (nextNode.run == run)
					{
						// skip our own sibling node
						continue;
					}
					position = Math.Min(position, traverseEnd(nextNode, startPosition + nextNode.margin));
				}
			}

			if (node == run.end)
			{
				// let's go for our sibling
				long dimension = run.WrapDimension;
				position = Math.Min(position, traverseEnd(run.start, startPosition - dimension));
				position = Math.Min(position, startPosition - dimension - run.start.margin);
			}

			return position;
		}

		public virtual long computeWrapSize(ConstraintWidgetContainer container, int orientation)
		{
			if (firstRun is ChainRun)
			{
				ChainRun chainRun = (ChainRun) firstRun;
				if (chainRun.orientation != orientation)
				{
					return 0;
				}
			}
			else
			{
				if (orientation == HORIZONTAL)
				{
					if (!(firstRun is HorizontalWidgetRun))
					{
						return 0;
					}
				}
				else
				{
					if (!(firstRun is VerticalWidgetRun))
					{
						return 0;
					}
				}
			}
			DependencyNode containerStart = orientation == HORIZONTAL ? container.horizontalRun.start : container.verticalRun.start;
			DependencyNode containerEnd = orientation == HORIZONTAL ? container.horizontalRun.end : container.verticalRun.end;

			bool runWithStartTarget = firstRun.start.targets.Contains(containerStart);
			bool runWithEndTarget = firstRun.end.targets.Contains(containerEnd);

			long dimension = firstRun.WrapDimension;

			if (runWithStartTarget && runWithEndTarget)
			{
				long maxPosition = traverseStart(firstRun.start, 0);
				long minPosition = traverseEnd(firstRun.end, 0);

				// to compute the gaps, we subtract the margins
				long endGap = maxPosition - dimension;
				if (endGap >= -firstRun.end.margin)
				{
					endGap += firstRun.end.margin;
				}
				long startGap = -minPosition - dimension - firstRun.start.margin;
				if (startGap >= firstRun.start.margin)
				{
					startGap -= firstRun.start.margin;
				}
				float bias = firstRun.widget.getBiasPercent(orientation);
				long gap = 0;
				if (bias > 0)
				{
					gap = (long)((startGap / bias) + (endGap / (1f - bias)));
				}

				startGap = (long)(0.5f + (gap * bias));
				endGap = (long)(0.5f + (gap * (1f - bias)));

				long runDimension = startGap + dimension + endGap;
				dimension = firstRun.start.margin + runDimension - firstRun.end.margin;

			}
			else if (runWithStartTarget)
			{
				long maxPosition = traverseStart(firstRun.start, firstRun.start.margin);
				long runDimension = firstRun.start.margin + dimension;
				dimension = Math.Max(maxPosition, runDimension);
			}
			else if (runWithEndTarget)
			{
				long minPosition = traverseEnd(firstRun.end, firstRun.end.margin);
				long runDimension = -firstRun.end.margin + dimension;
				dimension = Math.Max(-minPosition, runDimension);
			}
			else
			{
				dimension = firstRun.start.margin + firstRun.WrapDimension - firstRun.end.margin;
			}

			return dimension;
		}

		private bool defineTerminalWidget(WidgetRun run, int orientation)
		{
			if (!run.widget.isTerminalWidget[orientation])
			{
				return false;
			}
			foreach (Dependency dependency in run.start.dependencies)
			{
				if (dependency is DependencyNode)
				{
					DependencyNode node = (DependencyNode) dependency;
					if (node.run == run)
					{
						continue;
					}
					if (node == node.run.start)
					{
						if (run is ChainRun)
						{
							ChainRun chainRun = (ChainRun) run;
							foreach (WidgetRun widgetChainRun in chainRun.widgets)
							{
								defineTerminalWidget(widgetChainRun, orientation);
							}
						}
						else
						{
							if (!(run is HelperReferences))
							{
								run.widget.isTerminalWidget[orientation] = false;
							}
						}
						defineTerminalWidget(node.run, orientation);
					}
				}
			}
			foreach (Dependency dependency in run.end.dependencies)
			{
				if (dependency is DependencyNode)
				{
					DependencyNode node = (DependencyNode) dependency;
					if (node.run == run)
					{
						continue;
					}
					if (node == node.run.start)
					{
						if (run is ChainRun)
						{
							ChainRun chainRun = (ChainRun) run;
							foreach (WidgetRun widgetChainRun in chainRun.widgets)
							{
								defineTerminalWidget(widgetChainRun, orientation);
							}
						}
						else
						{
							if (!(run is HelperReferences))
							{
							   run.widget.isTerminalWidget[orientation] = false;
							}
						}
						defineTerminalWidget(node.run, orientation);
					}
				}
			}
			return false;
		}


		public virtual void defineTerminalWidgets(bool horizontalCheck, bool verticalCheck)
		{
			if (horizontalCheck && firstRun is HorizontalWidgetRun)
			{
				defineTerminalWidget(firstRun, HORIZONTAL);
			}
			if (verticalCheck && firstRun is VerticalWidgetRun)
			{
				defineTerminalWidget(firstRun, VERTICAL);
			}
		}
	}

}