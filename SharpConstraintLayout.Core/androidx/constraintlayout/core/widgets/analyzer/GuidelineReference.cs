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

	internal class GuidelineReference : WidgetRun
	{

		public GuidelineReference(ConstraintWidget widget) : base(widget)
		{
			widget.horizontalRun.clear();
			widget.verticalRun.clear();
			this.orientation = ((Guideline) widget).Orientation;
		}

		internal override void clear()
		{
			start.clear();
		}

		internal override void reset()
		{
			start.resolved = false;
			end.resolved = false;
		}

		internal override bool supportsWrapComputation()
		{
			return false;
		}

		private void addDependency(androidx.constraintlayout.core.widgets.analyzer.DependencyNode node)
		{
			start.dependencies.Add(node);
			node.targets.Add(start);
		}

		public override void update(Dependency dependency)
		{
			if (!start.readyToSolve)
			{
				return;
			}
			if (start.resolved)
			{
				return;
			}
			// ready to solve, centering.
			androidx.constraintlayout.core.widgets.analyzer.DependencyNode startTarget = start.targets[0];
			Guideline guideline = (Guideline) widget;
			int startPos = (int)(0.5f + startTarget.value * guideline.RelativePercent);
			start.resolve(startPos);
		}

		internal override void apply()
		{
			Guideline guideline = (Guideline) widget;
			int relativeBegin = guideline.RelativeBegin;
			int relativeEnd = guideline.RelativeEnd;
			float percent = guideline.RelativePercent;
			if (guideline.Orientation == ConstraintWidget.VERTICAL)
			{
				if (relativeBegin != -1)
				{
					start.targets.Add(widget.mParent.horizontalRun.start);
					widget.mParent.horizontalRun.start.dependencies.Add(start);
					start.margin = relativeBegin;
				}
				else if (relativeEnd != -1)
				{
					start.targets.Add(widget.mParent.horizontalRun.end);
					widget.mParent.horizontalRun.end.dependencies.Add(start);
					start.margin = -relativeEnd;
				}
				else
				{
					start.delegateToWidgetRun = true;
					start.targets.Add(widget.mParent.horizontalRun.end);
					widget.mParent.horizontalRun.end.dependencies.Add(start);
				}
				// FIXME -- if we move the DependencyNode directly in the ConstraintAnchor we'll be good.
				addDependency(widget.horizontalRun.start);
				addDependency(widget.horizontalRun.end);
			}
			else
			{
				if (relativeBegin != -1)
				{
					start.targets.Add(widget.mParent.verticalRun.start);
					widget.mParent.verticalRun.start.dependencies.Add(start);
					start.margin = relativeBegin;
				}
				else if (relativeEnd != -1)
				{
					start.targets.Add(widget.mParent.verticalRun.end);
					widget.mParent.verticalRun.end.dependencies.Add(start);
					start.margin = -relativeEnd;
				}
				else
				{
					start.delegateToWidgetRun = true;
					start.targets.Add(widget.mParent.verticalRun.end);
					widget.mParent.verticalRun.end.dependencies.Add(start);
				}
				// FIXME -- if we move the DependencyNode directly in the ConstraintAnchor we'll be good.
				addDependency(widget.verticalRun.start);
				addDependency(widget.verticalRun.end);
			}
		}

		internal override void applyToWidget()
		{
			Guideline guideline = (Guideline) widget;
			if (guideline.Orientation == ConstraintWidget.VERTICAL)
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