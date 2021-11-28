﻿/*
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

	internal class HelperReferences : WidgetRun
	{
		public HelperReferences(ConstraintWidget widget) : base(widget)
		{
		}

		internal override void clear()
		{
			runGroup = null;
			start.clear();
		}

		internal override void reset()
		{
			start.resolved = false;
		}

		internal override bool supportsWrapComputation()
		{
			return false;
		}

		private void addDependency(DependencyNode node)
		{
			start.dependencies.Add(node);
			node.targets.Add(start);
		}

		internal override void apply()
		{
			if (widget is Barrier)
			{
				start.delegateToWidgetRun = true;
				Barrier barrier = (Barrier) widget;
				int type = barrier.BarrierType;
				bool allowsGoneWidget = barrier.AllowsGoneWidget;
				switch (type)
				{
					case Barrier.LEFT:
					{
						start.type = DependencyNode.Type.LEFT;
						for (int i = 0; i < barrier.mWidgetsCount; i++)
						{
							ConstraintWidget refWidget = barrier.mWidgets[i];
							if (!allowsGoneWidget && refWidget.Visibility == ConstraintWidget.GONE)
							{
								continue;
							}
							DependencyNode target = refWidget.horizontalRun.start;
							target.dependencies.Add(start);
							start.targets.Add(target);
							// FIXME -- if we move the DependencyNode directly in the ConstraintAnchor we'll be good.
						}
						addDependency(widget.horizontalRun.start);
						addDependency(widget.horizontalRun.end);
					}
					break;
					case Barrier.RIGHT:
					{
						start.type = DependencyNode.Type.RIGHT;
						for (int i = 0; i < barrier.mWidgetsCount; i++)
						{
							ConstraintWidget refWidget = barrier.mWidgets[i];
							if (!allowsGoneWidget && refWidget.Visibility == ConstraintWidget.GONE)
							{
								continue;
							}
							DependencyNode target = refWidget.horizontalRun.end;
							target.dependencies.Add(start);
							start.targets.Add(target);
							// FIXME -- if we move the DependencyNode directly in the ConstraintAnchor we'll be good.
						}
						addDependency(widget.horizontalRun.start);
						addDependency(widget.horizontalRun.end);
					}
					break;
					case Barrier.TOP:
					{
						start.type = DependencyNode.Type.TOP;
						for (int i = 0; i < barrier.mWidgetsCount; i++)
						{
							ConstraintWidget refwidget = barrier.mWidgets[i];
							if (!allowsGoneWidget && refwidget.Visibility == ConstraintWidget.GONE)
							{
								continue;
							}
							DependencyNode target = refwidget.verticalRun.start;
							target.dependencies.Add(start);
							start.targets.Add(target);
							// FIXME -- if we move the DependencyNode directly in the ConstraintAnchor we'll be good.
						}
						addDependency(widget.verticalRun.start);
						addDependency(widget.verticalRun.end);
					}
					break;
					case Barrier.BOTTOM:
					{
						start.type = DependencyNode.Type.BOTTOM;
						for (int i = 0; i < barrier.mWidgetsCount; i++)
						{
							ConstraintWidget refwidget = barrier.mWidgets[i];
							if (!allowsGoneWidget && refwidget.Visibility == ConstraintWidget.GONE)
							{
								continue;
							}
							DependencyNode target = refwidget.verticalRun.end;
							target.dependencies.Add(start);
							start.targets.Add(target);
							// FIXME -- if we move the DependencyNode directly in the ConstraintAnchor we'll be good.
						}
						addDependency(widget.verticalRun.start);
						addDependency(widget.verticalRun.end);
					}
					break;
				}
			}
		}

		public override void update(Dependency dependency)
		{
			Barrier barrier = (Barrier) widget;
			int type = barrier.BarrierType;

			int min = -1;
			int max = 0;
			foreach (DependencyNode node in start.targets)
			{
				int value = node.value;
				if (min == -1 || value < min)
				{
					min = value;
				}
				if (max < value)
				{
					max = value;
				}
			}
			if (type == Barrier.LEFT || type == Barrier.TOP)
			{
				start.resolve(min + barrier.Margin);
			}
			else
			{
				start.resolve(max + barrier.Margin);
			}
		}

		internal override void applyToWidget()
		{
			if (widget is Barrier)
			{
				Barrier barrier = (Barrier) widget;
				int type = barrier.BarrierType;
				if (type == Barrier.LEFT || type == Barrier.RIGHT)
				{
					widget.X = start.value;
				}
				else
				{
					widget.Y = start.value;
				}
			}
		}
	}

}