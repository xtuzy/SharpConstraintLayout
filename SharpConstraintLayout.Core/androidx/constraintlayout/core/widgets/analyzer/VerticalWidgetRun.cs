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
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.FIXED;
using static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_PERCENT;
using static androidx.constraintlayout.core.widgets.ConstraintWidget;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_RATIO;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_WRAP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.WidgetRun.RunType.CENTER;
using static androidx.constraintlayout.core.widgets.analyzer.WidgetRun.RunType;

	public class VerticalWidgetRun : WidgetRun
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			baseline = new DependencyNode(this);
		}

		public DependencyNode baseline;
		internal androidx.constraintlayout.core.widgets.analyzer.DimensionDependency baselineDimension = null;

		public VerticalWidgetRun(ConstraintWidget widget) : base(widget)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			start.type = DependencyNode.Type.TOP;
			end.type = DependencyNode.Type.BOTTOM;
			baseline.type = DependencyNode.Type.BASELINE;
			this.orientation = VERTICAL;
		}

		public override string ToString()
		{
			return "VerticalRun " + widget.DebugName;
		}

		internal override void clear()
		{
			runGroup = null;
			start.clear();
			end.clear();
			baseline.clear();
			dimension.clear();
			resolved = false;
		}

		internal override void reset()
		{
			resolved = false;
			start.clear();
			start.resolved = false;
			end.clear();
			end.resolved = false;
			baseline.clear();
			baseline.resolved = false;
			dimension.resolved = false;
		}

		internal override bool supportsWrapComputation()
		{
			if (base.dimensionBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
			{
				if (base.widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		public override void update(Dependency dependency)
		{
			switch (mRunType)
			{
				case androidx.constraintlayout.core.widgets.analyzer.WidgetRun.RunType.START:
				{
					updateRunStart(dependency);
				}
				break;
				case androidx.constraintlayout.core.widgets.analyzer.WidgetRun.RunType.END:
				{
					updateRunEnd(dependency);
				}
				break;
				case androidx.constraintlayout.core.widgets.analyzer.WidgetRun.RunType.CENTER:
				{
					updateRunCenter(dependency, widget.mTop, widget.mBottom, VERTICAL);
					return;
				}
				default:
					break;
			}
			if (true || dependency == dimension)
			{
				if (dimension.readyToSolve && !dimension.resolved)
				{
					if (dimensionBehavior == MATCH_CONSTRAINT)
					{
						switch (widget.mMatchConstraintDefaultHeight)
						{
							case MATCH_CONSTRAINT_RATIO:
							{
								if (widget.horizontalRun.dimension.resolved)
								{
									int size = 0;
									int ratioSide = widget.DimensionRatioSide;
									switch (ratioSide)
									{
										case ConstraintWidget.HORIZONTAL:
										{
											size = (int)(0.5f + widget.horizontalRun.dimension.value * widget.getDimensionRatio());
										}
										break;
										case ConstraintWidget.VERTICAL:
										{
											size = (int)(0.5f + widget.horizontalRun.dimension.value / widget.getDimensionRatio());
										}
										break;
										case ConstraintWidget.UNKNOWN:
										{
											size = (int)(0.5f + widget.horizontalRun.dimension.value / widget.getDimensionRatio());
										}
										break;
										default:
											break;
									}
									dimension.resolve(size);
								}
							}
							break;
							case MATCH_CONSTRAINT_PERCENT:
							{
								ConstraintWidget parent = widget.Parent;
								if (parent != null)
								{
									if (parent.verticalRun.dimension.resolved)
									{
										float percent = widget.mMatchConstraintPercentHeight;
										int targetDimensionValue = parent.verticalRun.dimension.value;
										int size = (int)(0.5f + targetDimensionValue * percent);
										dimension.resolve(size);
									}
								}
							}
							break;
							default:
								break;
						}
					}
				}
			}
			if (!(start.readyToSolve && end.readyToSolve))
			{
				return;
			}
			if (start.resolved && end.resolved && dimension.resolved)
			{
				return;
			}

			if (!dimension.resolved && dimensionBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD && !widget.InVerticalChain)
			{

				DependencyNode startTarget = start.targets[0];
				DependencyNode endTarget = end.targets[0];
				int startPos = startTarget.value + start.margin;
				int endPos = endTarget.value + end.margin;

				int distance = endPos - startPos;
				start.resolve(startPos);
				end.resolve(endPos);
				dimension.resolve(distance);
				return;
			}

			if (!dimension.resolved && dimensionBehavior == MATCH_CONSTRAINT && matchConstraintsType == MATCH_CONSTRAINT_WRAP)
			{
				if (start.targets.Count > 0 && end.targets.Count > 0)
				{
					DependencyNode startTarget = start.targets[0];
					DependencyNode endTarget = end.targets[0];
					int startPos = startTarget.value + start.margin;
					int endPos = endTarget.value + end.margin;
					int availableSpace = endPos - startPos;
					if (availableSpace < dimension.wrapValue)
					{
						dimension.resolve(availableSpace);
					}
					else
					{
						dimension.resolve(dimension.wrapValue);
					}
				}
			}

			if (!dimension.resolved)
			{
				return;
			}
			// ready to solve, centering.
			if (start.targets.Count > 0 && end.targets.Count > 0)
			{
				DependencyNode startTarget = start.targets[0];
				DependencyNode endTarget = end.targets[0];
				int startPos = startTarget.value + start.margin;
				int endPos = endTarget.value + end.margin;
				float bias = widget.VerticalBiasPercent;
				if (startTarget == endTarget)
				{
					startPos = startTarget.value;
					endPos = endTarget.value;
					// TODO: this might be a nice feature to support, but I guess for now let's stay
					// compatible with 1.1
					bias = 0.5f;
				}
				int distance = (endPos - startPos - dimension.value);
				start.resolve((int)(0.5f + startPos + distance * bias));
				end.resolve(start.value + dimension.value);
			}
		}

		internal override void apply()
		{
			if (widget.measured)
			{
				dimension.resolve(widget.Height);
			}
			if (!dimension.resolved)
			{
				base.dimensionBehavior = widget.VerticalDimensionBehaviour;
				if (widget.hasBaseline())
				{
					baselineDimension = new BaselineDimensionDependency(this);
				}
				if (base.dimensionBehavior != ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
				{
					if (dimensionBehavior == MATCH_PARENT)
					{
						ConstraintWidget parent = widget.Parent;
						if (parent != null && parent.VerticalDimensionBehaviour == FIXED)
						{
							int resolvedDimension = parent.Height - widget.mTop.Margin - widget.mBottom.Margin;
							addTarget(start, parent.verticalRun.start, widget.mTop.Margin);
							addTarget(end, parent.verticalRun.end, -widget.mBottom.Margin);
							dimension.resolve(resolvedDimension);
							return;
						}
					}
					if (dimensionBehavior == FIXED)
					{
						dimension.resolve(widget.Height);
					}
				}
			}
			else
			{
				if (dimensionBehavior == MATCH_PARENT)
				{
					ConstraintWidget parent = widget.Parent;
					if (parent != null && parent.VerticalDimensionBehaviour == FIXED)
					{
						addTarget(start, parent.verticalRun.start, widget.mTop.Margin);
						addTarget(end, parent.verticalRun.end, -widget.mBottom.Margin);
						return;
					}
				}
			}
			// three basic possibilities:
			// <-s-e->
			// <-s-e
			//   s-e->
			// and a variation if the dimension is not yet known:
			// <-s-d-e->
			// <-s<-d<-e
			//   s->d->e->

			if (dimension.resolved && widget.measured)
			{
				if (widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].mTarget != null && widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].mTarget != null)
				{ // <-s-e->
					if (widget.InVerticalChain)
					{
						start.margin = widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].Margin;
						end.margin = -widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].Margin;
					}
					else
					{
						DependencyNode startTarget = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_TOP]);
						if (startTarget != null)
						{
							addTarget(start, startTarget, widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].Margin);
						}
						DependencyNode endTarget = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM]);
						if (endTarget != null)
						{
							addTarget(end, endTarget, -widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].Margin);
						}
						start.delegateToWidgetRun = true;
						end.delegateToWidgetRun = true;
					}
					if (widget.hasBaseline())
					{
						addTarget(baseline, start, widget.BaselineDistance);
					}
				}
				else if (widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].mTarget != null)
				{ // <-s-e
					DependencyNode target = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_TOP]);
					if (target != null)
					{
						addTarget(start, target, widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].Margin);
						addTarget(end, start, dimension.value);
						if (widget.hasBaseline())
						{
							addTarget(baseline, start, widget.BaselineDistance);
						}
					}
				}
				else if (widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].mTarget != null)
				{ //   s-e->
					DependencyNode target = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM]);
					if (target != null)
					{
						addTarget(end, target, -widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].Margin);
						addTarget(start, end, -dimension.value);
					}
					if (widget.hasBaseline())
					{
						addTarget(baseline, start, widget.BaselineDistance);
					}
				}
				else if (widget.mListAnchors[ConstraintWidget.ANCHOR_BASELINE].mTarget != null)
				{
					DependencyNode target = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_BASELINE]);
					if (target != null)
					{
						addTarget(baseline, target, 0);
						addTarget(start, baseline, -widget.BaselineDistance);
						addTarget(end, start, dimension.value);
					}
				}
				else
				{
					// no connections, nothing to do.
					if (!(widget is Helper) && widget.Parent != null && widget.getAnchor(ConstraintAnchor.Type.CENTER).mTarget == null)
					{
						DependencyNode top = widget.Parent.verticalRun.start;
						addTarget(start, top, widget.Y);
						addTarget(end, start, dimension.value);
						if (widget.hasBaseline())
						{
							addTarget(baseline, start, widget.BaselineDistance);
						}
					}
				}
			}
			else
			{
				if (!dimension.resolved && dimensionBehavior == MATCH_CONSTRAINT)
				{
					switch (widget.mMatchConstraintDefaultHeight)
					{
						case MATCH_CONSTRAINT_RATIO:
						{
							if (!widget.InVerticalChain)
							{
								if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_RATIO)
								{
									// need to look into both side
									// do nothing here -- let the HorizontalWidgetRun::update() deal with it.
									break;
								}
								// we have a ratio, but we depend on the other side computation
								DependencyNode targetDimension = widget.horizontalRun.dimension;
								dimension.targets.Add(targetDimension);
								targetDimension.dependencies.Add(dimension);
								dimension.delegateToWidgetRun = true;
								dimension.dependencies.Add(start);
								dimension.dependencies.Add(end);
							}
						}
						break;
						case MATCH_CONSTRAINT_PERCENT:
						{
							// we need to look up the parent dimension
							ConstraintWidget parent = widget.Parent;
							if (parent == null)
							{
								break;
							}
							DependencyNode targetDimension = parent.verticalRun.dimension;
							dimension.targets.Add(targetDimension);
							targetDimension.dependencies.Add(dimension);
							dimension.delegateToWidgetRun = true;
							dimension.dependencies.Add(start);
							dimension.dependencies.Add(end);
						}
						break;
						case MATCH_CONSTRAINT_SPREAD:
						{
							// the work is done in the update()
						}
						break;
						default:
							break;
					}
				}
				else
				{
					dimension.addDependency(this);
				}
				if (widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].mTarget != null && widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].mTarget != null)
				{ // <-s-d-e->
					if (widget.InVerticalChain)
					{
						start.margin = widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].Margin;
						end.margin = -widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].Margin;
					}
					else
					{
						DependencyNode startTarget = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_TOP]);
						DependencyNode endTarget = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM]);
						if (false)
						{
							if (startTarget != null)
							{
								addTarget(start, startTarget, widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].Margin);
							}
							if (endTarget != null)
							{
								addTarget(end, endTarget, -widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].Margin);
							}
						}
						else
						{
							if (startTarget != null)
							{
								startTarget.addDependency(this);
							}
							if (endTarget != null)
							{
								endTarget.addDependency(this);
							}
						}
						mRunType = CENTER;
					}
					if (widget.hasBaseline())
					{
						addTarget(baseline, start, 1, baselineDimension);
					}
				}
				else if (widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].mTarget != null)
				{ // <-s<-d<-e
					DependencyNode target = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_TOP]);
					if (target != null)
					{
						addTarget(start, target, widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].Margin);
						addTarget(end, start, 1, dimension);
						if (widget.hasBaseline())
						{
							addTarget(baseline, start, 1, baselineDimension);
						}
						if (dimensionBehavior == MATCH_CONSTRAINT)
						{
							if (widget.getDimensionRatio() > 0)
							{
								if (widget.horizontalRun.dimensionBehavior == MATCH_CONSTRAINT)
								{
									widget.horizontalRun.dimension.dependencies.Add(dimension);
									dimension.targets.Add(widget.horizontalRun.dimension);
									dimension.updateDelegate = this;
								}
							}
						}
					}
				}
				else if (widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].mTarget != null)
				{ //   s->d->e->
					DependencyNode target = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM]);
					if (target != null)
					{
						addTarget(end, target, -widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].Margin);
						addTarget(start, end, -1, dimension);
						if (widget.hasBaseline())
						{
							addTarget(baseline, start, 1, baselineDimension);
						}
					}
				}
				else if (widget.mListAnchors[ConstraintWidget.ANCHOR_BASELINE].mTarget != null)
				{
					DependencyNode target = getTarget(widget.mListAnchors[ConstraintWidget.ANCHOR_BASELINE]);
					if (target != null)
					{
						addTarget(baseline, target, 0);
						addTarget(start, baseline, -1, baselineDimension);
						addTarget(end, start, 1, dimension);
					}
				}
				else
				{
					// no connections, nothing to do.
					if (!(widget is Helper) && widget.Parent != null)
					{
						DependencyNode top = widget.Parent.verticalRun.start;
						addTarget(start, top, widget.Y);
						addTarget(end, start, 1, dimension);
						if (widget.hasBaseline())
						{
							addTarget(baseline, start, 1, baselineDimension);
						}
						if (dimensionBehavior == MATCH_CONSTRAINT)
						{
							if (widget.getDimensionRatio() > 0)
							{
								if (widget.horizontalRun.dimensionBehavior == MATCH_CONSTRAINT)
								{
									widget.horizontalRun.dimension.dependencies.Add(dimension);
									dimension.targets.Add(widget.horizontalRun.dimension);
									dimension.updateDelegate = this;
								}
							}
						}
					}
				}

				// if dimension has no dependency, mark it as ready to solve
				if (dimension.targets.Count == 0)
				{
					dimension.readyToSolve = true;
				}
			}
		}

		internal override void applyToWidget()
		{
			if (start.resolved)
			{
				widget.Y = start.value;
			}
		}
	}

}