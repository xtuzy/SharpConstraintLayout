using System;

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
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_PERCENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_RATIO;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_WRAP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;

	public abstract class WidgetRun : Dependency
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			dimension = new DimensionDependency(this);
			start = new DependencyNode(this);
			end = new DependencyNode(this);
		}

		public int matchConstraintsType;
		internal ConstraintWidget widget;
		internal RunGroup runGroup;
		protected internal ConstraintWidget.DimensionBehaviour dimensionBehavior;
		internal DimensionDependency dimension;

		public int orientation = HORIZONTAL;
		internal bool resolved = false;
		public DependencyNode start;
		public DependencyNode end;

		internal RunType mRunType = RunType.NONE;

		public WidgetRun(ConstraintWidget widget)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.widget = widget;
		}

		internal abstract void clear();
		internal abstract void apply();
		internal abstract void applyToWidget();
		internal abstract void reset();

		internal abstract bool supportsWrapComputation();

		public virtual bool DimensionResolved
		{
			get
			{
				return dimension.resolved;
			}
		}

		public virtual bool CenterConnection
		{
			get
			{
				int connections = 0;
				int count = start.targets.Count;
				for (int i = 0; i < count; i++)
				{
					DependencyNode dependency = start.targets[i];
					if (dependency.run != this)
					{
						connections++;
					}
				}
				count = end.targets.Count;
				for (int i = 0; i < count; i++)
				{
					DependencyNode dependency = end.targets[i];
					if (dependency.run != this)
					{
						connections++;
					}
				}
				return connections >= 2;
			}
		}

		public virtual long wrapSize(int direction)
		{
			if (dimension.resolved)
			{
				long size = dimension.value;
				if (CenterConnection)
				{ //start.targets.size() > 0 && end.targets.size() > 0) {
					size += start.margin - end.margin;
				}
				else
				{
					if (direction == RunGroup.START)
					{
						size += start.margin;
					}
					else
					{
						size -= end.margin;
					}
				}
				return size;
			}
			return 0;
		}

		protected internal DependencyNode getTarget(ConstraintAnchor anchor)
		{
			if (anchor.mTarget == null)
			{
				return null;
			}
			DependencyNode target = null;
			ConstraintWidget targetWidget = anchor.mTarget.mOwner;
			ConstraintAnchor.Type targetType = anchor.mTarget.mType;
			switch (targetType)
			{
				case ConstraintAnchor.Type.LEFT:
				{
					HorizontalWidgetRun run = targetWidget.horizontalRun;
					target = run.start;
				}
				break;
				case ConstraintAnchor.Type.RIGHT:
				{
					HorizontalWidgetRun run = targetWidget.horizontalRun;
					target = run.end;
				}
				break;
				case ConstraintAnchor.Type.TOP:
				{
					VerticalWidgetRun run = targetWidget.verticalRun;
					target = run.start;
				}
				break;
				case ConstraintAnchor.Type.BASELINE:
				{
					VerticalWidgetRun run = targetWidget.verticalRun;
					target = run.baseline;
				}
				break;
				case ConstraintAnchor.Type.BOTTOM:
				{
					VerticalWidgetRun run = targetWidget.verticalRun;
					target = run.end;
				}
				break;
				default:
					break;
			}
			return target;
		}

		protected internal virtual void updateRunCenter(Dependency dependency, ConstraintAnchor startAnchor, ConstraintAnchor endAnchor, int orientation)
		{
			DependencyNode startTarget = getTarget(startAnchor);
			DependencyNode endTarget = getTarget(endAnchor);

			if (!(startTarget.resolved && endTarget.resolved))
			{
				return;
			}

			int startPos = startTarget.value + startAnchor.Margin;
			int endPos = endTarget.value - endAnchor.Margin;
			int distance = endPos - startPos;

			if (!dimension.resolved && dimensionBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
			{
				resolveDimension(orientation, distance);
			}

			if (!dimension.resolved)
			{
				return;
			}

			if (dimension.value == distance)
			{
				start.resolve(startPos);
				end.resolve(endPos);
				return;
			}

			// Otherwise, we have to center
			float bias = orientation == HORIZONTAL ? widget.HorizontalBiasPercent : widget.VerticalBiasPercent;

			if (startTarget == endTarget)
			{
				startPos = startTarget.value;
				endPos = endTarget.value;
				// TODO: taking advantage of bias here would be a nice feature to support,
				// but for now let's stay compatible with 1.1
				bias = 0.5f;
			}

			int availableDistance = (endPos - startPos - dimension.value);
			start.resolve((int)(0.5f + startPos + availableDistance * bias));
			end.resolve(start.value + dimension.value);
		}

		private void resolveDimension(int orientation, int distance)
		{
			switch (matchConstraintsType)
			{
				case MATCH_CONSTRAINT_SPREAD:
				{
					dimension.resolve(getLimitedDimension(distance, orientation));
				}
				break;
				case MATCH_CONSTRAINT_PERCENT:
				{
					ConstraintWidget parent = widget.Parent;
					if (parent != null)
					{
						WidgetRun run = orientation == HORIZONTAL ? (WidgetRun)parent.horizontalRun : (WidgetRun)parent.verticalRun;
						if (run.dimension.resolved)
						{
							float percent = orientation == HORIZONTAL ? widget.mMatchConstraintPercentWidth : widget.mMatchConstraintPercentHeight;
							int targetDimensionValue = run.dimension.value;
							int size = (int)(0.5f + targetDimensionValue * percent);
							dimension.resolve(getLimitedDimension(size, orientation));
						}
					}
				}
				break;
				case MATCH_CONSTRAINT_WRAP:
				{
					int wrapValue = getLimitedDimension(dimension.wrapValue, orientation);
					dimension.resolve(Math.Min(wrapValue, distance));
				}
				break;
				case MATCH_CONSTRAINT_RATIO:
				{
					if (widget.horizontalRun.dimensionBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && widget.horizontalRun.matchConstraintsType == MATCH_CONSTRAINT_RATIO && widget.verticalRun.dimensionBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && widget.verticalRun.matchConstraintsType == MATCH_CONSTRAINT_RATIO)
					{
						// pof
					}
					else
					{
						WidgetRun run = (orientation == HORIZONTAL) ? (WidgetRun)widget.verticalRun : (WidgetRun)widget.horizontalRun;
						if (run.dimension.resolved)
						{
							float ratio = widget.getDimensionRatio();
							int value;
							if (orientation == VERTICAL)
							{
								value = (int)(0.5f + run.dimension.value / ratio);
							}
							else
							{
								value = (int)(0.5f + ratio * run.dimension.value);
							}
							dimension.resolve(value);
						}
					}
				}
				break;
				default:
					break;
			}
		}

		protected internal virtual void updateRunStart(Dependency dependency)
		{

		}

		protected internal virtual void updateRunEnd(Dependency dependency)
		{

		}

		public virtual void update(Dependency dependency)
		{
		}

		protected internal int getLimitedDimension(int dimension, int orientation)
		{
			if (orientation == HORIZONTAL)
			{
				int max = widget.mMatchConstraintMaxWidth;
				int min = widget.mMatchConstraintMinWidth;
				int value = Math.Max(min, dimension);
				if (max > 0)
				{
					value = Math.Min(max, dimension);
				}
				if (value != dimension)
				{
					dimension = value;
				}
			}
			else
			{
				int max = widget.mMatchConstraintMaxHeight;
				int min = widget.mMatchConstraintMinHeight;
				int value = Math.Max(min, dimension);
				if (max > 0)
				{
					value = Math.Min(max, dimension);
				}
				if (value != dimension)
				{
					dimension = value;
				}
			}
			return dimension;
		}

		protected internal DependencyNode getTarget(ConstraintAnchor anchor, int orientation)
		{
			if (anchor.mTarget == null)
			{
				return null;
			}
			DependencyNode target = null;
			ConstraintWidget targetWidget = anchor.mTarget.mOwner;
			WidgetRun run = (orientation == ConstraintWidget.HORIZONTAL) ? (WidgetRun)targetWidget.horizontalRun : (WidgetRun)targetWidget.verticalRun;
			ConstraintAnchor.Type targetType = anchor.mTarget.mType;
			switch (targetType)
			{
				case ConstraintAnchor.Type.TOP:
				case ConstraintAnchor.Type.LEFT:
				{
					target = run.start;
				}
				break;
				case ConstraintAnchor.Type.BOTTOM:
				case ConstraintAnchor.Type.RIGHT:
				{
					target = run.end;
				}
				break;
				default:
					break;
			}
			return target;
		}

		protected internal void addTarget(DependencyNode node, DependencyNode target, int margin)
		{
			node.targets.Add(target);
			node.margin = margin;
			target.dependencies.Add(node);
		}

		internal void addTarget(DependencyNode node, DependencyNode target, int marginFactor, DimensionDependency dimensionDependency)
		{
			node.targets.Add(target);
			node.targets.Add(dimension);
			node.marginFactor = marginFactor;
			node.marginDependency = dimensionDependency;
			target.dependencies.Add(node);
			dimensionDependency.dependencies.Add(node);
		}

		public virtual long WrapDimension
		{
			get
			{
				if (dimension.resolved)
				{
					return dimension.value;
				}
				return 0;
			}
		}

		public virtual bool Resolved
		{
			get
			{
				return resolved;
			}
		}

		internal enum RunType
		{
			NONE,
			START,
			END,
			CENTER
		}
	}

}