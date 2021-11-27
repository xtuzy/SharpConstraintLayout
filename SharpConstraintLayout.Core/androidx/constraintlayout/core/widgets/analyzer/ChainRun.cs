using System;
using System.Collections.Generic;
using System.Text;

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
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
using static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.GONE;
using static androidx.constraintlayout.core.widgets.ConstraintWidget;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.HORIZONTAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_WRAP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;

	public class ChainRun : WidgetRun
	{
		internal List<WidgetRun> widgets = new List<WidgetRun>();
		private int chainStyle;

		public ChainRun(ConstraintWidget widget, int orientation) : base(widget)
		{
			this.orientation = orientation;
			build();
		}

		public override string ToString()
		{
			StringBuilder log = new StringBuilder("ChainRun ");
			log.Append((orientation == HORIZONTAL ? "horizontal : " : "vertical : "));
			foreach (WidgetRun run in widgets)
			{
				log.Append("<");
				log.Append(run);
				log.Append("> ");
			}
			return log.ToString();
		}

		internal override bool supportsWrapComputation()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = widgets.size();
			int count = widgets.Count;
			for (int i = 0; i < count; i++)
			{
				WidgetRun run = widgets[i];
				if (!run.supportsWrapComputation())
				{
					return false;
				}
			}
			return true;
		}

		public override long WrapDimension
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int count = widgets.size();
				int count = widgets.Count;
				long wrapDimension = 0;
				for (int i = 0; i < count; i++)
				{
					WidgetRun run = widgets[i];
					wrapDimension += run.start.margin;
					wrapDimension += run.WrapDimension;
					wrapDimension += run.end.margin;
				}
				return wrapDimension;
			}
		}

		private void build()
		{
			ConstraintWidget current = widget;
			ConstraintWidget previous = current.getPreviousChainMember(orientation);
			while (previous != null)
			{
				current = previous;
				previous = current.getPreviousChainMember(orientation);
			}
			widget = current; // first element of the chain
			widgets.Add(current.getRun(orientation));
			ConstraintWidget next = current.getNextChainMember(orientation);
			while (next != null)
			{
				current = next;
				widgets.Add(current.getRun(orientation));
				next = current.getNextChainMember(orientation);
			}
			foreach (WidgetRun run in widgets)
			{
				if (orientation == HORIZONTAL)
				{
					run.widget.horizontalChainRun = this;
				}
				else if (orientation == ConstraintWidget.VERTICAL)
				{
					run.widget.verticalChainRun = this;
				}
			}
			bool isInRtl = (orientation == HORIZONTAL) && ((ConstraintWidgetContainer) widget.Parent).Rtl;
			if (isInRtl && widgets.Count > 1)
			{
				widget = widgets[widgets.Count - 1].widget;
			}
			chainStyle = orientation == HORIZONTAL ? widget.HorizontalChainStyle : widget.VerticalChainStyle;
		}


		internal override void clear()
		{
			runGroup = null;
			foreach (WidgetRun run in widgets)
			{
				run.clear();
			}
		}

		internal override void reset()
		{
			start.resolved = false;
			end.resolved = false;
		}

		public override void update(Dependency dependency)
		{
			if (!(start.resolved && end.resolved))
			{
				return;
			}

			ConstraintWidget parent = widget.Parent;
			bool isInRtl = false;
			if (parent is ConstraintWidgetContainer)
			{
				isInRtl = ((ConstraintWidgetContainer) parent).Rtl;
			}
			int distance = end.value - start.value;
			int size = 0;
			int numMatchConstraints = 0;
			float weights = 0;
			int numVisibleWidgets = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = widgets.size();
			int count = widgets.Count;
			// let's find the first visible widget...
			int firstVisibleWidget = -1;
			for (int i = 0; i < count; i++)
			{
				WidgetRun run = widgets[i];
				if (run.widget.Visibility == GONE)
				{
					continue;
				}
				firstVisibleWidget = i;
				break;
			}
			// now the last visible widget...
			int lastVisibleWidget = -1;
			for (int i = count - 1; i >= 0; i--)
			{
				WidgetRun run = widgets[i];
				if (run.widget.Visibility == GONE)
				{
					continue;
				}
				lastVisibleWidget = i;
				break;
			}
			for (int j = 0; j < 2; j++)
			{
				for (int i = 0; i < count; i++)
				{
					WidgetRun run = widgets[i];
					if (run.widget.Visibility == GONE)
					{
						continue;
					}
					numVisibleWidgets++;
					if (i > 0 && i >= firstVisibleWidget)
					{
						size += run.start.margin;
					}
					int dimension = run.dimension.value;
					bool treatAsFixed = run.dimensionBehavior != MATCH_CONSTRAINT;
					if (treatAsFixed)
					{
						if (orientation == HORIZONTAL && !run.widget.horizontalRun.dimension.resolved)
						{
							return;
						}
						if (orientation == VERTICAL && !run.widget.verticalRun.dimension.resolved)
						{
							return;
						}
					}
					else if (run.matchConstraintsType == MATCH_CONSTRAINT_WRAP && j == 0)
					{
						treatAsFixed = true;
						dimension = run.dimension.wrapValue;
						numMatchConstraints++;
					}
					else if (run.dimension.resolved)
					{
						treatAsFixed = true;
					}
					if (!treatAsFixed)
					{ // only for the first pass
						numMatchConstraints++;
						float weight = run.widget.mWeight[orientation];
						if (weight >= 0)
						{
							weights += weight;
						}
					}
					else
					{
						size += dimension;
					}
					if (i < count - 1 && i < lastVisibleWidget)
					{
						size += -run.end.margin;
					}
				}
				if (size < distance || numMatchConstraints == 0)
				{
					break; // we are good to go!
				}
				// otherwise, let's do another pass with using match_constraints
				numVisibleWidgets = 0;
				numMatchConstraints = 0;
				size = 0;
				weights = 0;
			}

			int position = start.value;
			if (isInRtl)
			{
				position = end.value;
			}
			if (size > distance)
			{
				if (isInRtl)
				{
					position += (int)(0.5f + (size - distance) / 2f);
				}
				else
				{
					position -= (int)(0.5f + (size - distance) / 2f);
				}
			}
			int matchConstraintsDimension = 0;
			if (numMatchConstraints > 0)
			{
				matchConstraintsDimension = (int)(0.5f + (distance - size) / (float) numMatchConstraints);

				int appliedLimits = 0;
				for (int i = 0; i < count; i++)
				{
					WidgetRun run = widgets[i];
					if (run.widget.Visibility == GONE)
					{
						continue;
					}
					if (run.dimensionBehavior == MATCH_CONSTRAINT && !run.dimension.resolved)
					{
						int dimension = matchConstraintsDimension;
						if (weights > 0)
						{
							float weight = run.widget.mWeight[orientation];
							dimension = (int)(0.5f + weight * (distance - size) / weights);
						}
						int max;
						int min;
						int value = dimension;
						if (orientation == HORIZONTAL)
						{
							max = run.widget.mMatchConstraintMaxWidth;
							min = run.widget.mMatchConstraintMinWidth;
						}
						else
						{
							max = run.widget.mMatchConstraintMaxHeight;
							min = run.widget.mMatchConstraintMinHeight;
						}
						if (run.matchConstraintsType == MATCH_CONSTRAINT_WRAP)
						{
							value = Math.Min(value, run.dimension.wrapValue);
						}
						value = Math.Max(min, value);
						if (max > 0)
						{
							value = Math.Min(max, value);
						}
						if (value != dimension)
						{
							appliedLimits++;
							dimension = value;
						}
						run.dimension.resolve(dimension);
					}
				}
				if (appliedLimits > 0)
				{
					numMatchConstraints -= appliedLimits;
					// we have to recompute the sizes
					size = 0;
					for (int i = 0; i < count; i++)
					{
						WidgetRun run = widgets[i];
						if (run.widget.Visibility == GONE)
						{
							continue;
						}
						if (i > 0 && i >= firstVisibleWidget)
						{
							size += run.start.margin;
						}
						size += run.dimension.value;
						if (i < count - 1 && i < lastVisibleWidget)
						{
							size += -run.end.margin;
						}
					}
				}
				if (chainStyle == ConstraintWidget.CHAIN_PACKED && appliedLimits == 0)
				{
					chainStyle = ConstraintWidget.CHAIN_SPREAD;
				}
			}

			if (size > distance)
			{
				chainStyle = ConstraintWidget.CHAIN_PACKED;
			}

			if (numVisibleWidgets > 0 && numMatchConstraints == 0 && firstVisibleWidget == lastVisibleWidget)
			{
				// only one widget of fixed size to display...
				chainStyle = ConstraintWidget.CHAIN_PACKED;
			}

			if (chainStyle == ConstraintWidget.CHAIN_SPREAD_INSIDE)
			{
				int gap = 0;
				if (numVisibleWidgets > 1)
				{
					gap = (distance - size) / (numVisibleWidgets - 1);
				}
				else if (numVisibleWidgets == 1)
				{
					gap = (distance - size) / 2;
				}
				if (numMatchConstraints > 0)
				{
					gap = 0;
				}
				for (int i = 0; i < count; i++)
				{
					int index = i;
					if (isInRtl)
					{
						index = count - (i + 1);
					}
					WidgetRun run = widgets[index];
					if (run.widget.Visibility == GONE)
					{
						run.start.resolve(position);
						run.end.resolve(position);
						continue;
					}
					if (i > 0)
					{
						if (isInRtl)
						{
							position -= gap;
						}
						else
						{
							position += gap;
						}
					}
					if (i > 0 && i >= firstVisibleWidget)
					{
						if (isInRtl)
						{
							position -= run.start.margin;
						}
						else
						{
							position += run.start.margin;
						}
					}

					if (isInRtl)
					{
						run.end.resolve(position);
					}
					else
					{
						run.start.resolve(position);
					}

					int dimension = run.dimension.value;
					if (run.dimensionBehavior == MATCH_CONSTRAINT && run.matchConstraintsType == MATCH_CONSTRAINT_WRAP)
					{
						dimension = run.dimension.wrapValue;
					}
					if (isInRtl)
					{
						position -= dimension;
					}
					else
					{
						position += dimension;
					}

					if (isInRtl)
					{
						run.start.resolve(position);
					}
					else
					{
						run.end.resolve(position);
					}
					run.resolved = true;
					if (i < count - 1 && i < lastVisibleWidget)
					{
						if (isInRtl)
						{
							position -= -run.end.margin;
						}
						else
						{
							position += -run.end.margin;
						}
					}
				}
			}
			else if (chainStyle == ConstraintWidget.CHAIN_SPREAD)
			{
				int gap = (distance - size) / (numVisibleWidgets + 1);
				if (numMatchConstraints > 0)
				{
					gap = 0;
				}
				for (int i = 0; i < count; i++)
				{
					int index = i;
					if (isInRtl)
					{
						index = count - (i + 1);
					}
					WidgetRun run = widgets[index];
					if (run.widget.Visibility == GONE)
					{
						run.start.resolve(position);
						run.end.resolve(position);
						continue;
					}
					if (isInRtl)
					{
						position -= gap;
					}
					else
					{
						position += gap;
					}
					if (i > 0 && i >= firstVisibleWidget)
					{
						if (isInRtl)
						{
							position -= run.start.margin;
						}
						else
						{
							position += run.start.margin;
						}
					}

					if (isInRtl)
					{
						run.end.resolve(position);
					}
					else
					{
						run.start.resolve(position);
					}

					int dimension = run.dimension.value;
					if (run.dimensionBehavior == MATCH_CONSTRAINT && run.matchConstraintsType == MATCH_CONSTRAINT_WRAP)
					{
						dimension = Math.Min(dimension, run.dimension.wrapValue);
					}

					if (isInRtl)
					{
						position -= dimension;
					}
					else
					{
						position += dimension;
					}

					if (isInRtl)
					{
						run.start.resolve(position);
					}
					else
					{
						run.end.resolve(position);
					}
					if (i < count - 1 && i < lastVisibleWidget)
					{
						if (isInRtl)
						{
							position -= -run.end.margin;
						}
						else
						{
							position += -run.end.margin;
						}
					}
				}
			}
			else if (chainStyle == ConstraintWidget.CHAIN_PACKED)
			{
				float bias = (orientation == HORIZONTAL) ? widget.HorizontalBiasPercent : widget.VerticalBiasPercent;
				if (isInRtl)
				{
					bias = 1 - bias;
				}
				int gap = (int)(0.5f + (distance - size) * bias);
				if (gap < 0 || numMatchConstraints > 0)
				{
					gap = 0;
				}
				if (isInRtl)
				{
					position -= gap;
				}
				else
				{
					position += gap;
				}
				for (int i = 0; i < count; i++)
				{
					int index = i;
					if (isInRtl)
					{
						index = count - (i + 1);
					}
					WidgetRun run = widgets[index];
					if (run.widget.Visibility == GONE)
					{
						run.start.resolve(position);
						run.end.resolve(position);
						continue;
					}
					if (i > 0 && i >= firstVisibleWidget)
					{
						if (isInRtl)
						{
							position -= run.start.margin;
						}
						else
						{
							position += run.start.margin;
						}
					}
					if (isInRtl)
					{
						run.end.resolve(position);
					}
					else
					{
						run.start.resolve(position);
					}

					int dimension = run.dimension.value;
					if (run.dimensionBehavior == MATCH_CONSTRAINT && run.matchConstraintsType == MATCH_CONSTRAINT_WRAP)
					{
						dimension = run.dimension.wrapValue;
					}
					if (isInRtl)
					{
						position -= dimension;
					}
					else
					{
						position += dimension;
					}

					if (isInRtl)
					{
						run.start.resolve(position);
					}
					else
					{
						run.end.resolve(position);
					}
					if (i < count - 1 && i < lastVisibleWidget)
					{
						if (isInRtl)
						{
							position -= -run.end.margin;
						}
						else
						{
							position += -run.end.margin;
						}
					}
				}
			}
		}

		internal override void applyToWidget()
		{
			for (int i = 0; i < widgets.Count; i++)
			{
				WidgetRun run = widgets[i];
				run.applyToWidget();
			}
		}

		private ConstraintWidget FirstVisibleWidget
		{
			get
			{
				for (int i = 0; i < widgets.Count; i++)
				{
					WidgetRun run = widgets[i];
					if (run.widget.Visibility != GONE)
					{
						return run.widget;
					}
				}
				return null;
			}
		}

		private ConstraintWidget LastVisibleWidget
		{
			get
			{
				for (int i = widgets.Count - 1; i >= 0; i--)
				{
					WidgetRun run = widgets[i];
					if (run.widget.Visibility != GONE)
					{
						return run.widget;
					}
				}
				return null;
			}
		}


		internal override void apply()
		{
			foreach (WidgetRun run in widgets)
			{
				run.apply();
			}
			int count = widgets.Count;
			if (count < 1)
			{
				return;
			}

			// get the first and last element of the chain
			ConstraintWidget firstWidget = widgets[0].widget;
			ConstraintWidget lastWidget = widgets[count - 1].widget;

			if (orientation == HORIZONTAL)
			{
				ConstraintAnchor startAnchor = firstWidget.mLeft;
				ConstraintAnchor endAnchor = lastWidget.mRight;
				DependencyNode startTarget = getTarget(startAnchor, HORIZONTAL);
				int startMargin = startAnchor.Margin;
				ConstraintWidget firstVisibleWidget = FirstVisibleWidget;
				if (firstVisibleWidget != null)
				{
					startMargin = firstVisibleWidget.mLeft.Margin;
				}
				if (startTarget != null)
				{
					addTarget(start, startTarget, startMargin);
				}
				DependencyNode endTarget = getTarget(endAnchor, HORIZONTAL);
				int endMargin = endAnchor.Margin;
				ConstraintWidget lastVisibleWidget = LastVisibleWidget;
				if (lastVisibleWidget != null)
				{
					endMargin = lastVisibleWidget.mRight.Margin;
				}
				if (endTarget != null)
				{
					addTarget(end, endTarget, -endMargin);
				}
			}
			else
			{
				ConstraintAnchor startAnchor = firstWidget.mTop;
				ConstraintAnchor endAnchor = lastWidget.mBottom;
				DependencyNode startTarget = getTarget(startAnchor, VERTICAL);
				int startMargin = startAnchor.Margin;
				ConstraintWidget firstVisibleWidget = FirstVisibleWidget;
				if (firstVisibleWidget != null)
				{
					startMargin = firstVisibleWidget.mTop.Margin;
				}
				if (startTarget != null)
				{
					addTarget(start, startTarget, startMargin);
				}
				DependencyNode endTarget = getTarget(endAnchor, VERTICAL);
				int endMargin = endAnchor.Margin;
				ConstraintWidget lastVisibleWidget = LastVisibleWidget;
				if (lastVisibleWidget != null)
				{
					endMargin = lastVisibleWidget.mBottom.Margin;
				}
				if (endTarget != null)
				{
					addTarget(end, endTarget, -endMargin);
				}
			}
			start.updateDelegate = this;
			end.updateDelegate = this;
		}

	}

}