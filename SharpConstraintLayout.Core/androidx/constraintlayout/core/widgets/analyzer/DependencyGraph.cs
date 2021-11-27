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
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.FIXED;
using static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.GONE;
using static androidx.constraintlayout.core.widgets.ConstraintWidget;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.HORIZONTAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_PERCENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_RATIO;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_WRAP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.UNKNOWN;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;

	public class DependencyGraph
	{
		private const bool USE_GROUPS = true;
		private ConstraintWidgetContainer container;
		private bool mNeedBuildGraph = true;
		private bool mNeedRedoMeasures = true;
		private ConstraintWidgetContainer mContainer;
		private List<WidgetRun> mRuns = new List<WidgetRun>();

		// TODO: Unused, should we delete?
		private List<RunGroup> runGroups = new List<RunGroup>();

		public DependencyGraph(ConstraintWidgetContainer container)
		{
			this.container = container;
			mContainer = container;
		}

		private BasicMeasure.Measurer mMeasurer = null;
		private BasicMeasure.Measure mMeasure = new BasicMeasure.Measure();

		public virtual BasicMeasure.Measurer Measurer
		{
			set
			{
				mMeasurer = value;
			}
		}

		private int computeWrap(ConstraintWidgetContainer container, int orientation)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mGroups.size();
			int count = mGroups.Count;
			long wrapSize = 0;
			for (int i = 0; i < count; i++)
			{
				RunGroup run = mGroups[i];
				long size = run.computeWrapSize(container, orientation);
				wrapSize = Math.Max(wrapSize, size);
			}
			return (int) wrapSize;
		}

		/// <summary>
		/// Find and mark terminal widgets (trailing widgets) -- they are the only
		/// ones we need to care for wrap_content checks </summary>
		/// <param name="horizontalBehavior"> </param>
		/// <param name="verticalBehavior"> </param>
		public virtual void defineTerminalWidgets(ConstraintWidget.DimensionBehaviour horizontalBehavior, ConstraintWidget.DimensionBehaviour verticalBehavior)
		{
			if (mNeedBuildGraph)
			{
				buildGraph();

				if (USE_GROUPS)
				{
					bool hasBarrier = false;
					foreach (ConstraintWidget widget in container.mChildren)
					{
						widget.isTerminalWidget[HORIZONTAL] = true;
						widget.isTerminalWidget[VERTICAL] = true;
						if (widget is Barrier)
						{
							hasBarrier = true;
						}
					}
					if (!hasBarrier)
					{
						foreach (RunGroup group in mGroups)
						{
							group.defineTerminalWidgets(horizontalBehavior == WRAP_CONTENT, verticalBehavior == WRAP_CONTENT);
						}
					}
				}
			}
		}

		/// <summary>
		/// Try to measure the layout by solving the graph of constraints directly
		/// </summary>
		/// <param name="optimizeWrap"> use the wrap_content optimizer </param>
		/// <returns> true if all widgets have been resolved </returns>
		public virtual bool directMeasure(bool optimizeWrap)
		{
			optimizeWrap &= USE_GROUPS;

			if (mNeedBuildGraph || mNeedRedoMeasures)
			{
				foreach (ConstraintWidget widget in container.mChildren)
				{
					widget.ensureWidgetRuns();
					widget.measured = false;
					widget.horizontalRun.reset();
					widget.verticalRun.reset();
				}
				container.ensureWidgetRuns();
				container.measured = false;
				container.horizontalRun.reset();
				container.verticalRun.reset();
				mNeedRedoMeasures = false;
			}

			bool avoid = basicMeasureWidgets(mContainer);
			if (avoid)
			{
				return false;
			}

			container.X = 0;
			container.Y = 0;

			ConstraintWidget.DimensionBehaviour? originalHorizontalDimension = container.getDimensionBehaviour(HORIZONTAL);
			ConstraintWidget.DimensionBehaviour? originalVerticalDimension = container.getDimensionBehaviour(VERTICAL);

			if (mNeedBuildGraph)
			{
				buildGraph();
			}

			int x1 = container.X;
			int y1 = container.Y;

			container.horizontalRun.start.resolve(x1);
			container.verticalRun.start.resolve(y1);

			// Let's do the easy steps first -- anything that can be immediately measured
			// Whatever is left for the dimension will be match_constraints.
			measureWidgets();

			// If we have to support wrap, let's see if we can compute it directly
			if (originalHorizontalDimension == WRAP_CONTENT || originalVerticalDimension == WRAP_CONTENT)
			{
				if (optimizeWrap)
				{
					foreach (WidgetRun run in mRuns)
					{
						if (!run.supportsWrapComputation())
						{
							optimizeWrap = false;
							break;
						}
					}
				}

				if (optimizeWrap && originalHorizontalDimension == WRAP_CONTENT)
				{
					container.HorizontalDimensionBehaviour = FIXED;
					container.Width = computeWrap(container, HORIZONTAL);
					container.horizontalRun.dimension.resolve(container.Width);
				}
				if (optimizeWrap && originalVerticalDimension == WRAP_CONTENT)
				{
					container.VerticalDimensionBehaviour = FIXED;
					container.Height = computeWrap(container, VERTICAL);
					container.verticalRun.dimension.resolve(container.Height);
				}
			}

			bool checkRoot = false;

			// Now, depending on our own dimension behavior, we may want to solve
			// one dimension before the other

			if (container.mListDimensionBehaviors[HORIZONTAL] == ConstraintWidget.DimensionBehaviour.FIXED || container.mListDimensionBehaviors[HORIZONTAL] == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
			{

				// solve horizontal dimension
				int x2 = x1 + container.Width;
				container.horizontalRun.end.resolve(x2);
				container.horizontalRun.dimension.resolve(x2 - x1);
				measureWidgets();
				if (container.mListDimensionBehaviors[VERTICAL] == FIXED || container.mListDimensionBehaviors[VERTICAL] == MATCH_PARENT)
				{
					int y2 = y1 + container.Height;
					container.verticalRun.end.resolve(y2);
					container.verticalRun.dimension.resolve(y2 - y1);
				}
				measureWidgets();
				checkRoot = true;
			}
			else
			{
				// we'll bail out to the solver...
			}

			// Let's apply what we did resolve
			foreach (WidgetRun run in mRuns)
			{
				if (run.widget == container && !run.resolved)
				{
					continue;
				}
				run.applyToWidget();
			}

			bool allResolved = true;
			foreach (WidgetRun run in mRuns)
			{
				if (!checkRoot && run.widget == container)
				{
					continue;
				}
				if (!run.start.resolved)
				{
					allResolved = false;
					break;
				}
				if (!run.end.resolved && !(run is GuidelineReference))
				{
					allResolved = false;
					break;
				}
				if (!run.dimension.resolved && !(run is ChainRun) && !(run is GuidelineReference))
				{
					allResolved = false;
					break;
				}
			}

			//TODO:验证是否存在null的情况
			if(originalHorizontalDimension == null)
				throw new AssertionError("Dimension is null");
			container.HorizontalDimensionBehaviour = originalHorizontalDimension.Value;
			container.VerticalDimensionBehaviour = originalVerticalDimension.Value;

			return allResolved;
		}

		public virtual bool directMeasureSetup(bool optimizeWrap)
		{
			if (mNeedBuildGraph)
			{
				foreach (ConstraintWidget widget in container.mChildren)
				{
					widget.ensureWidgetRuns();
					widget.measured = false;
					widget.horizontalRun.dimension.resolved = false;
					widget.horizontalRun.resolved = false;
					widget.horizontalRun.reset();
					widget.verticalRun.dimension.resolved = false;
					widget.verticalRun.resolved = false;
					widget.verticalRun.reset();
				}
				container.ensureWidgetRuns();
				container.measured = false;
				container.horizontalRun.dimension.resolved = false;
				container.horizontalRun.resolved = false;
				container.horizontalRun.reset();
				container.verticalRun.dimension.resolved = false;
				container.verticalRun.resolved = false;
				container.verticalRun.reset();
				buildGraph();
			}

			bool avoid = basicMeasureWidgets(mContainer);
			if (avoid)
			{
				return false;
			}

			container.X = 0;
			container.Y = 0;
			container.horizontalRun.start.resolve(0);
			container.verticalRun.start.resolve(0);
			return true;
		}

		public virtual bool directMeasureWithOrientation(bool optimizeWrap, int orientation)
		{
			optimizeWrap &= USE_GROUPS;

			//TODO:验证是否存在null的情况
			if (container.getDimensionBehaviour(HORIZONTAL) == null)
				throw new AssertionError("Dimension is null");
			ConstraintWidget.DimensionBehaviour originalHorizontalDimension = container.getDimensionBehaviour(HORIZONTAL).Value;
			ConstraintWidget.DimensionBehaviour originalVerticalDimension = container.getDimensionBehaviour(VERTICAL).Value;

			int x1 = container.X;
			int y1 = container.Y;

			// If we have to support wrap, let's see if we can compute it directly
			if (optimizeWrap && (originalHorizontalDimension == WRAP_CONTENT || originalVerticalDimension == WRAP_CONTENT))
			{
				foreach (WidgetRun run in mRuns)
				{
					if (run.orientation == orientation && !run.supportsWrapComputation())
					{
						optimizeWrap = false;
						break;
					}
				}

				if (orientation == HORIZONTAL)
				{
					if (optimizeWrap && originalHorizontalDimension == WRAP_CONTENT)
					{
						container.HorizontalDimensionBehaviour = FIXED;
						container.Width = computeWrap(container, HORIZONTAL);
						container.horizontalRun.dimension.resolve(container.Width);
					}
				}
				else
				{
					if (optimizeWrap && originalVerticalDimension == WRAP_CONTENT)
					{
						container.VerticalDimensionBehaviour = FIXED;
						container.Height = computeWrap(container, VERTICAL);
						container.verticalRun.dimension.resolve(container.Height);
					}
				}
			}

			bool checkRoot = false;

			// Now, depending on our own dimension behavior, we may want to solve
			// one dimension before the other

			if (orientation == HORIZONTAL)
			{
				if (container.mListDimensionBehaviors[HORIZONTAL] == FIXED || container.mListDimensionBehaviors[HORIZONTAL] == MATCH_PARENT)
				{
					int x2 = x1 + container.Width;
					container.horizontalRun.end.resolve(x2);
					container.horizontalRun.dimension.resolve(x2 - x1);
					checkRoot = true;
				}
			}
			else
			{
				if (container.mListDimensionBehaviors[VERTICAL] == FIXED || container.mListDimensionBehaviors[VERTICAL] == MATCH_PARENT)
				{
					int y2 = y1 + container.Height;
					container.verticalRun.end.resolve(y2);
					container.verticalRun.dimension.resolve(y2 - y1);
					checkRoot = true;
				}
			}
			measureWidgets();

			// Let's apply what we did resolve
			foreach (WidgetRun run in mRuns)
			{
				if (run.orientation != orientation)
				{
					continue;
				}
				if (run.widget == container && !run.resolved)
				{
					continue;
				}
				run.applyToWidget();
			}

			bool allResolved = true;
			foreach (WidgetRun run in mRuns)
			{
				if (run.orientation != orientation)
				{
					continue;
				}
				if (!checkRoot && run.widget == container)
				{
					continue;
				}
				if (!run.start.resolved)
				{
					allResolved = false;
					break;
				}
				if (!run.end.resolved)
				{
					allResolved = false;
					break;
				}
				if (!(run is ChainRun) && !run.dimension.resolved)
				{
					allResolved = false;
					break;
				}
			}

			container.HorizontalDimensionBehaviour = originalHorizontalDimension;
			container.VerticalDimensionBehaviour = originalVerticalDimension;

			return allResolved;
		}

		/// <summary>
		/// Convenience function to fill in the measure spec
		/// </summary>
		/// <param name="widget"> the widget to measure </param>
		/// <param name="horizontalBehavior"> </param>
		/// <param name="horizontalDimension"> </param>
		/// <param name="verticalBehavior"> </param>
		/// <param name="verticalDimension"> </param>
		private void measure(ConstraintWidget widget, ConstraintWidget.DimensionBehaviour horizontalBehavior, int horizontalDimension, ConstraintWidget.DimensionBehaviour verticalBehavior, int verticalDimension)
		{
			mMeasure.horizontalBehavior = horizontalBehavior;
			mMeasure.verticalBehavior = verticalBehavior;
			mMeasure.horizontalDimension = horizontalDimension;
			mMeasure.verticalDimension = verticalDimension;
			mMeasurer.measure(widget, mMeasure);
			widget.Width = mMeasure.measuredWidth;
			widget.Height = mMeasure.measuredHeight;
			widget.HasBaseline = mMeasure.measuredHasBaseline;
			widget.BaselineDistance = mMeasure.measuredBaseline;
		}

		private bool basicMeasureWidgets(ConstraintWidgetContainer constraintWidgetContainer)
		{
			foreach (ConstraintWidget widget in constraintWidgetContainer.mChildren)
			{
				ConstraintWidget.DimensionBehaviour horizontal = widget.mListDimensionBehaviors[HORIZONTAL];
				ConstraintWidget.DimensionBehaviour vertical = widget.mListDimensionBehaviors[VERTICAL];

				if (widget.Visibility == GONE)
				{
					widget.measured = true;
					continue;
				}

				// Basic validation
				// TODO: might move this earlier in the process
				if (widget.mMatchConstraintPercentWidth < 1 && horizontal == MATCH_CONSTRAINT)
				{
					widget.mMatchConstraintDefaultWidth = MATCH_CONSTRAINT_PERCENT;
				}
				if (widget.mMatchConstraintPercentHeight < 1 && vertical == MATCH_CONSTRAINT)
				{
					widget.mMatchConstraintDefaultHeight = MATCH_CONSTRAINT_PERCENT;
				}
				if (widget.getDimensionRatio() > 0)
				{
					if (horizontal == MATCH_CONSTRAINT && (vertical == WRAP_CONTENT || vertical == FIXED))
					{
						widget.mMatchConstraintDefaultWidth = MATCH_CONSTRAINT_RATIO;
					}
					else if (vertical == MATCH_CONSTRAINT && (horizontal == WRAP_CONTENT || horizontal == FIXED))
					{
						widget.mMatchConstraintDefaultHeight = MATCH_CONSTRAINT_RATIO;
					}
					else if (horizontal == MATCH_CONSTRAINT && vertical == MATCH_CONSTRAINT)
					{
						if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
						{
							widget.mMatchConstraintDefaultWidth = MATCH_CONSTRAINT_RATIO;
						}
						if (widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
						{
							widget.mMatchConstraintDefaultHeight = MATCH_CONSTRAINT_RATIO;
						}
					}
				}

				if (horizontal == MATCH_CONSTRAINT && widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
				{
					if (widget.mLeft.mTarget == null || widget.mRight.mTarget == null)
					{
						horizontal = WRAP_CONTENT;
					}
				}
				if (vertical == MATCH_CONSTRAINT && widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
				{
					if (widget.mTop.mTarget == null || widget.mBottom.mTarget == null)
					{
						vertical = WRAP_CONTENT;
					}
				}

				widget.horizontalRun.dimensionBehavior = horizontal;
				widget.horizontalRun.matchConstraintsType = widget.mMatchConstraintDefaultWidth;
				widget.verticalRun.dimensionBehavior = vertical;
				widget.verticalRun.matchConstraintsType = widget.mMatchConstraintDefaultHeight;

				if ((horizontal == MATCH_PARENT || horizontal == FIXED || horizontal == WRAP_CONTENT) && (vertical == MATCH_PARENT || vertical == FIXED || vertical == WRAP_CONTENT))
				{
					int width = widget.Width;
					if (horizontal == MATCH_PARENT)
					{
						width = constraintWidgetContainer.Width - widget.mLeft.mMargin - widget.mRight.mMargin;
						horizontal = FIXED;
					}
					int height = widget.Height;
					if (vertical == MATCH_PARENT)
					{
						height = constraintWidgetContainer.Height - widget.mTop.mMargin - widget.mBottom.mMargin;
						vertical = FIXED;
					}
					measure(widget, horizontal, width, vertical, height);
					widget.horizontalRun.dimension.resolve(widget.Width);
					widget.verticalRun.dimension.resolve(widget.Height);
					widget.measured = true;
					continue;
				}

				if (horizontal == MATCH_CONSTRAINT && (vertical == WRAP_CONTENT || vertical == FIXED))
				{
					if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_RATIO)
					{
						if (vertical == WRAP_CONTENT)
						{
							measure(widget, WRAP_CONTENT, 0, WRAP_CONTENT, 0);
						}
						int height = widget.Height;
						int width = (int)(height * widget.mDimensionRatio + 0.5f);
						measure(widget, FIXED, width, FIXED, height);
						widget.horizontalRun.dimension.resolve(widget.Width);
						widget.verticalRun.dimension.resolve(widget.Height);
						widget.measured = true;
						continue;
					}
					else if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
					{
						measure(widget, WRAP_CONTENT, 0, vertical, 0);
						widget.horizontalRun.dimension.wrapValue = widget.Width;
						continue;
					}
					else if (widget.mMatchConstraintDefaultWidth == ConstraintWidget.MATCH_CONSTRAINT_PERCENT)
					{
						if (constraintWidgetContainer.mListDimensionBehaviors[HORIZONTAL] == FIXED || constraintWidgetContainer.mListDimensionBehaviors[HORIZONTAL] == MATCH_PARENT)
						{
							float percent = widget.mMatchConstraintPercentWidth;
							int width = (int)(0.5f + percent * constraintWidgetContainer.Width);
							int height = widget.Height;
							measure(widget, FIXED, width, vertical, height);
							widget.horizontalRun.dimension.resolve(widget.Width);
							widget.verticalRun.dimension.resolve(widget.Height);
							widget.measured = true;
							continue;
						}
					}
					else
					{
						// let's verify we have both constraints
						if (widget.mListAnchors[ConstraintWidget.ANCHOR_LEFT].mTarget == null || widget.mListAnchors[ConstraintWidget.ANCHOR_RIGHT].mTarget == null)
						{
							measure(widget, WRAP_CONTENT, 0, vertical, 0);
							widget.horizontalRun.dimension.resolve(widget.Width);
							widget.verticalRun.dimension.resolve(widget.Height);
							widget.measured = true;
							continue;
						}
					}
				}
				if (vertical == MATCH_CONSTRAINT && (horizontal == WRAP_CONTENT || horizontal == FIXED))
				{
					if (widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_RATIO)
					{
						if (horizontal == WRAP_CONTENT)
						{
							measure(widget, WRAP_CONTENT, 0, WRAP_CONTENT, 0);
						}
						int width = widget.Width;
						float ratio = widget.mDimensionRatio;
						if (widget.DimensionRatioSide == UNKNOWN)
						{
							ratio = 1f / ratio;
						}
						int height = (int)(width * ratio + 0.5f);

						measure(widget, FIXED, width, FIXED, height);
						widget.horizontalRun.dimension.resolve(widget.Width);
						widget.verticalRun.dimension.resolve(widget.Height);
						widget.measured = true;
						continue;
					}
					else if (widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
					{
						measure(widget, horizontal, 0, WRAP_CONTENT, 0);
						widget.verticalRun.dimension.wrapValue = widget.Height;
						continue;
					}
					else if (widget.mMatchConstraintDefaultHeight == ConstraintWidget.MATCH_CONSTRAINT_PERCENT)
					{
						if (constraintWidgetContainer.mListDimensionBehaviors[VERTICAL] == FIXED || constraintWidgetContainer.mListDimensionBehaviors[VERTICAL] == MATCH_PARENT)
						{
							float percent = widget.mMatchConstraintPercentHeight;
							int width = widget.Width;
							int height = (int)(0.5f + percent * constraintWidgetContainer.Height);
							measure(widget, horizontal, width, FIXED, height);
							widget.horizontalRun.dimension.resolve(widget.Width);
							widget.verticalRun.dimension.resolve(widget.Height);
							widget.measured = true;
							continue;
						}
					}
					else
					{
						// let's verify we have both constraints
						if (widget.mListAnchors[ConstraintWidget.ANCHOR_TOP].mTarget == null || widget.mListAnchors[ConstraintWidget.ANCHOR_BOTTOM].mTarget == null)
						{
							measure(widget, WRAP_CONTENT, 0, vertical, 0);
							widget.horizontalRun.dimension.resolve(widget.Width);
							widget.verticalRun.dimension.resolve(widget.Height);
							widget.measured = true;
							continue;
						}
					}
				}
				if (horizontal == MATCH_CONSTRAINT && vertical == MATCH_CONSTRAINT)
				{
					if (widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP || widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
					{
						measure(widget, WRAP_CONTENT, 0, WRAP_CONTENT, 0);
						widget.horizontalRun.dimension.wrapValue = widget.Width;
						widget.verticalRun.dimension.wrapValue = widget.Height;
					}
					else if (widget.mMatchConstraintDefaultHeight == ConstraintWidget.MATCH_CONSTRAINT_PERCENT && widget.mMatchConstraintDefaultWidth == ConstraintWidget.MATCH_CONSTRAINT_PERCENT && constraintWidgetContainer.mListDimensionBehaviors[HORIZONTAL] == FIXED && constraintWidgetContainer.mListDimensionBehaviors[VERTICAL] == FIXED)
					{
						float horizPercent = widget.mMatchConstraintPercentWidth;
						float vertPercent = widget.mMatchConstraintPercentHeight;
						int width = (int)(0.5f + horizPercent * constraintWidgetContainer.Width);
						int height = (int)(0.5f + vertPercent * constraintWidgetContainer.Height);
						measure(widget, FIXED, width, FIXED, height);
						widget.horizontalRun.dimension.resolve(widget.Width);
						widget.verticalRun.dimension.resolve(widget.Height);
						widget.measured = true;
					}
				}
			}
			return false;
		}

		public virtual void measureWidgets()
		{
			foreach (ConstraintWidget widget in container.mChildren)
			{
				if (widget.measured)
				{
					continue;
				}
				ConstraintWidget.DimensionBehaviour horiz = widget.mListDimensionBehaviors[HORIZONTAL];
				ConstraintWidget.DimensionBehaviour vert = widget.mListDimensionBehaviors[VERTICAL];
				int horizMatchConstraintsType = widget.mMatchConstraintDefaultWidth;
				int vertMatchConstraintsType = widget.mMatchConstraintDefaultHeight;

				bool horizWrap = horiz == WRAP_CONTENT || (horiz == MATCH_CONSTRAINT && horizMatchConstraintsType == MATCH_CONSTRAINT_WRAP);

				bool vertWrap = vert == WRAP_CONTENT || (vert == MATCH_CONSTRAINT && vertMatchConstraintsType == MATCH_CONSTRAINT_WRAP);

				bool horizResolved = widget.horizontalRun.dimension.resolved;
				bool vertResolved = widget.verticalRun.dimension.resolved;

				if (horizResolved && vertResolved)
				{
					measure(widget, FIXED, widget.horizontalRun.dimension.value, FIXED, widget.verticalRun.dimension.value);
					widget.measured = true;
				}
				else if (horizResolved && vertWrap)
				{
					measure(widget, FIXED, widget.horizontalRun.dimension.value, WRAP_CONTENT, widget.verticalRun.dimension.value);
					if (vert == MATCH_CONSTRAINT)
					{
						widget.verticalRun.dimension.wrapValue = widget.Height;
					}
					else
					{
						widget.verticalRun.dimension.resolve(widget.Height);
						widget.measured = true;
					}
				}
				else if (vertResolved && horizWrap)
				{
					measure(widget, WRAP_CONTENT, widget.horizontalRun.dimension.value, FIXED, widget.verticalRun.dimension.value);
					if (horiz == MATCH_CONSTRAINT)
					{
						widget.horizontalRun.dimension.wrapValue = widget.Width;
					}
					else
					{
						widget.horizontalRun.dimension.resolve(widget.Width);
						widget.measured = true;
					}
				}
				if (widget.measured && widget.verticalRun.baselineDimension != null)
				{
					widget.verticalRun.baselineDimension.resolve(widget.BaselineDistance);
				}
			}
		}

		/// <summary>
		/// Invalidate the graph of constraints
		/// </summary>
		public virtual void invalidateGraph()
		{
			mNeedBuildGraph = true;
		}

		/// <summary>
		/// Mark the widgets as needing to be remeasured
		/// </summary>
		public virtual void invalidateMeasures()
		{
			mNeedRedoMeasures = true;
		}

		internal List<RunGroup> mGroups = new List<RunGroup>();

		public virtual void buildGraph()
		{
			// First, let's identify the overall dependency graph
			buildGraph(mRuns);

			if (USE_GROUPS)
			{
				mGroups.Clear();
				// Then get the horizontal and vertical groups
				RunGroup.index = 0;
				findGroup(container.horizontalRun, HORIZONTAL, mGroups);
				findGroup(container.verticalRun, VERTICAL, mGroups);
			}
			mNeedBuildGraph = false;
		}

		public virtual void buildGraph(List<WidgetRun> runs)
		{
			runs.Clear();
			mContainer.horizontalRun.clear();
			mContainer.verticalRun.clear();
			runs.Add(mContainer.horizontalRun);
			runs.Add(mContainer.verticalRun);
			HashSet<ChainRun> chainRuns = null;
			foreach (ConstraintWidget widget in mContainer.mChildren)
			{
				if (widget is Guideline)
				{
					runs.Add(new GuidelineReference(widget));
					continue;
				}
				if (widget.InHorizontalChain)
				{
					if (widget.horizontalChainRun == null)
					{
						// build the horizontal chain
						widget.horizontalChainRun = new ChainRun(widget, HORIZONTAL);
					}
					if (chainRuns == null)
					{
						chainRuns = new HashSet<ChainRun>();
					}
					chainRuns.Add(widget.horizontalChainRun);
				}
				else
				{
					runs.Add(widget.horizontalRun);
				}
				if (widget.InVerticalChain)
				{
					if (widget.verticalChainRun == null)
					{
						// build the vertical chain
						widget.verticalChainRun = new ChainRun(widget, VERTICAL);
					}
					if (chainRuns == null)
					{
						chainRuns = new HashSet<ChainRun>();
					}
					chainRuns.Add(widget.verticalChainRun);
				}
				else
				{
					runs.Add(widget.verticalRun);
				}
				if (widget is HelperWidget)
				{
					runs.Add(new HelperReferences(widget));
				}
			}
			if (chainRuns != null)
			{
				runs.AddRange(chainRuns);
			}
			foreach (WidgetRun run in runs)
			{
				run.clear();
			}
			foreach (WidgetRun run in runs)
			{
				if (run.widget == mContainer)
				{
					continue;
				}
				run.apply();
			}

	//        displayGraph();
		}



		private void displayGraph()
		{
			string content = "digraph {\n";
			foreach (WidgetRun run in mRuns)
			{
				content = generateDisplayGraph(run, content);
			}
			content += "\n}\n";
			Console.WriteLine("content:<<\n" + content + "\n>>");
		}

		private void applyGroup(DependencyNode node, int orientation, int direction, DependencyNode end, List<RunGroup> groups, RunGroup group)
		{
			WidgetRun run = node.run;
			if (run.runGroup != null || run == container.horizontalRun || run == container.verticalRun)
			{
				return;
			}

			if (group == null)
			{
				group = new RunGroup(run, direction);
				groups.Add(group);
			}

			run.runGroup = group;
			group.add(run);
			foreach (Dependency dependent in run.start.dependencies)
			{
				if (dependent is DependencyNode)
				{
					applyGroup((DependencyNode) dependent, orientation, RunGroup.START, end, groups, group);
				}
			}
			foreach (Dependency dependent in run.end.dependencies)
			{
				if (dependent is DependencyNode)
				{
					applyGroup((DependencyNode) dependent, orientation, RunGroup.END, end, groups, group);
				}
			}
			if (orientation == VERTICAL && run is VerticalWidgetRun)
			{
				foreach (Dependency dependent in ((VerticalWidgetRun) run).baseline.dependencies)
				{
					if (dependent is DependencyNode)
					{
						applyGroup((DependencyNode) dependent, orientation, RunGroup.BASELINE, end, groups, group);
					}
				}
			}
			foreach (DependencyNode target in run.start.targets)
			{
				if (target == end)
				{
					group.dual = true;
				}
				applyGroup(target, orientation, RunGroup.START, end, groups, group);
			}
			foreach (DependencyNode target in run.end.targets)
			{
				if (target == end)
				{
					group.dual = true;
				}
				applyGroup(target, orientation, RunGroup.END, end, groups, group);
			}
			if (orientation == VERTICAL && run is VerticalWidgetRun)
			{
				foreach (DependencyNode target in ((VerticalWidgetRun) run).baseline.targets)
				{
					applyGroup(target, orientation, RunGroup.BASELINE, end, groups, group);
				}
			}
		}

		private void findGroup(WidgetRun run, int orientation, List<RunGroup> groups)
		{
			foreach (Dependency dependent in run.start.dependencies)
			{
				if (dependent is DependencyNode)
				{
					DependencyNode node = (DependencyNode) dependent;
					applyGroup(node, orientation, RunGroup.START, run.end, groups, null);
				}
				else if (dependent is WidgetRun)
				{
					WidgetRun dependentRun = (WidgetRun) dependent;
					applyGroup(dependentRun.start, orientation, RunGroup.START, run.end, groups, null);
				}
			}
			foreach (Dependency dependent in run.end.dependencies)
			{
				if (dependent is DependencyNode)
				{
					DependencyNode node = (DependencyNode) dependent;
					applyGroup(node, orientation, RunGroup.END, run.start, groups, null);
				}
				else if (dependent is WidgetRun)
				{
					WidgetRun dependentRun = (WidgetRun) dependent;
					applyGroup(dependentRun.end, orientation, RunGroup.END, run.start, groups, null);
				}
			}
			if (orientation == VERTICAL)
			{
				foreach (Dependency dependent in ((VerticalWidgetRun) run).baseline.dependencies)
				{
					if (dependent is DependencyNode)
					{
						DependencyNode node = (DependencyNode) dependent;
						applyGroup(node, orientation, RunGroup.BASELINE, null, groups, null);
					}
				}
			}
		}


		private string generateDisplayNode(DependencyNode node, bool centeredConnection, string content)
		{
			StringBuilder contentBuilder = new StringBuilder(content);
			foreach (DependencyNode target in node.targets)
			{
				string constraint = "\n" + node.name();
				constraint += " -> " + target.name();
				if (node.margin > 0 || centeredConnection || node.run is HelperReferences)
				{
					constraint += "[";
					if (node.margin > 0)
					{
						constraint += "label=\"" + node.margin + "\"";
						if (centeredConnection)
						{
							constraint += ",";
						}
					}
					if (centeredConnection)
					{
						constraint += " style=dashed ";
					}
					if (node.run is HelperReferences)
					{
						constraint += " style=bold,color=gray ";
					}
					constraint += "]";
				}
				constraint += "\n";
				contentBuilder.Append(constraint);
			}
			content = contentBuilder.ToString();
	//        for (DependencyNode dependency : node.dependencies) {
	//            content = generateDisplayNode(dependency, content);
	//        }
			return content;
		}

		private string nodeDefinition(WidgetRun run)
		{
			int orientation = run is VerticalWidgetRun ? VERTICAL : HORIZONTAL;
			string name = run.widget.DebugName;
			StringBuilder definition = new StringBuilder(name);
			ConstraintWidget.DimensionBehaviour behaviour = orientation == HORIZONTAL ? run.widget.HorizontalDimensionBehaviour : run.widget.VerticalDimensionBehaviour;
			RunGroup runGroup = run.runGroup;

			if (orientation == HORIZONTAL)
			{
				definition.Append("_HORIZONTAL");
			}
			else
			{
				definition.Append("_VERTICAL");
			}
			definition.Append(" [shape=none, label=<");
			definition.Append("<TABLE BORDER=\"0\" CELLSPACING=\"0\" CELLPADDING=\"2\">");
			definition.Append("  <TR>");
			if (orientation == HORIZONTAL)
			{
				definition.Append("    <TD ");
				if (run.start.resolved)
				{
					definition.Append(" BGCOLOR=\"green\"");
				}
				definition.Append(" PORT=\"LEFT\" BORDER=\"1\">L</TD>");
			}
			else
			{
				definition.Append("    <TD ");
				if (run.start.resolved)
				{
					definition.Append(" BGCOLOR=\"green\"");
				}
				definition.Append(" PORT=\"TOP\" BORDER=\"1\">T</TD>");
			}
			definition.Append("    <TD BORDER=\"1\" ");
			if (run.dimension.resolved && !run.widget.measured)
			{
				definition.Append(" BGCOLOR=\"green\" ");
			}
			else if (run.dimension.resolved)
			{
				definition.Append(" BGCOLOR=\"lightgray\" ");
			}
			else if (run.widget.measured)
			{
				definition.Append(" BGCOLOR=\"yellow\" ");
			}
			if (behaviour == MATCH_CONSTRAINT)
			{
				definition.Append("style=\"dashed\"");
			}
			definition.Append(">");
			definition.Append(name);
			if (runGroup != null)
			{
				definition.Append(" [");
				definition.Append(runGroup.groupIndex + 1);
				definition.Append("/");
				definition.Append(RunGroup.index);
				definition.Append("]");
			}
			definition.Append(" </TD>");
			if (orientation == HORIZONTAL)
			{
				definition.Append("    <TD ");
				if (run.end.resolved)
				{
					definition.Append(" BGCOLOR=\"green\"");
				}
				definition.Append(" PORT=\"RIGHT\" BORDER=\"1\">R</TD>");
			}
			else
			{
				definition.Append("    <TD ");
				if (((VerticalWidgetRun) run).baseline.resolved)
				{
					definition.Append(" BGCOLOR=\"green\"");
				}
				definition.Append(" PORT=\"BASELINE\" BORDER=\"1\">b</TD>");
				definition.Append("    <TD ");
				if (run.end.resolved)
				{
					definition.Append(" BGCOLOR=\"green\"");
				}
				definition.Append(" PORT=\"BOTTOM\" BORDER=\"1\">B</TD>");
			}
			definition.Append("  </TR></TABLE>");
			definition.Append(">];\n");
			return definition.ToString();
		}

		private string generateChainDisplayGraph(ChainRun chain, string content)
		{
			int orientation = chain.orientation;
			StringBuilder subgroup = new StringBuilder("subgraph ");
			subgroup.Append("cluster_");
			subgroup.Append(chain.widget.DebugName);
			if (orientation == HORIZONTAL)
			{
				subgroup.Append("_h");
			}
			else
			{
				subgroup.Append("_v");
			}
			subgroup.Append(" {\n");
			string definitions = "";
			foreach (WidgetRun run in chain.widgets)
			{
				subgroup.Append(run.widget.DebugName);
				if (orientation == HORIZONTAL)
				{
					subgroup.Append("_HORIZONTAL");
				}
				else
				{
					subgroup.Append("_VERTICAL");
				}
				subgroup.Append(";\n");
				definitions = generateDisplayGraph(run, definitions);
			}
			subgroup.Append("}\n");
			return content + definitions + subgroup;
		}

		private bool isCenteredConnection(DependencyNode start, DependencyNode end)
		{
			int startTargets = 0;
			int endTargets = 0;
			foreach (DependencyNode s in start.targets)
			{
				if (s != end)
				{
					startTargets++;
				}
			}
			foreach (DependencyNode e in end.targets)
			{
				if (e != start)
				{
					endTargets++;
				}
			}
			return startTargets > 0 && endTargets > 0;
		}

		private string generateDisplayGraph(WidgetRun root, string content)
		{
			DependencyNode start = root.start;
			DependencyNode end = root.end;
			StringBuilder sb = new StringBuilder(content);

			if (!(root is HelperReferences) && start.dependencies.Count == 0 && end.dependencies.Count == 0 & start.targets.Count == 0 && end.targets.Count == 0)
			{
				return content;
			}
			sb.Append(nodeDefinition(root));

			bool centeredConnection = isCenteredConnection(start, end);
			content = generateDisplayNode(start, centeredConnection, content);
			content = generateDisplayNode(end, centeredConnection, content);
			if (root is VerticalWidgetRun)
			{
				DependencyNode baseline = ((VerticalWidgetRun) root).baseline;
				content = generateDisplayNode(baseline, centeredConnection, content);
			}

			if (root is HorizontalWidgetRun || (root is ChainRun && ((ChainRun) root).orientation == HORIZONTAL))
			{
				ConstraintWidget.DimensionBehaviour behaviour = root.widget.HorizontalDimensionBehaviour;
				if (behaviour == ConstraintWidget.DimensionBehaviour.FIXED || behaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
				{
					if (start.targets.Count > 0 && end.targets.Count == 0)
					{
						sb.Append("\n");
						sb.Append(end.name());
						sb.Append(" -> ");
						sb.Append(start.name());
						sb.Append("\n");
					}
					else if (start.targets.Count == 0 && end.targets.Count > 0)
					{
						sb.Append("\n");
						sb.Append(start.name());
						sb.Append(" -> ");
						sb.Append(end.name());
						sb.Append("\n");
					}
				}
				else
				{
					if (behaviour == MATCH_CONSTRAINT && root.widget.getDimensionRatio() > 0)
					{
						sb.Append("\n");
						sb.Append(root.widget.DebugName);
						sb.Append("_HORIZONTAL -> ");
						sb.Append(root.widget.DebugName);
						sb.Append("_VERTICAL;\n");
					}
				}
			}
			else if (root is VerticalWidgetRun || (root is ChainRun && ((ChainRun) root).orientation == VERTICAL))
			{
				ConstraintWidget.DimensionBehaviour behaviour = root.widget.VerticalDimensionBehaviour;
				if (behaviour == ConstraintWidget.DimensionBehaviour.FIXED || behaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
				{
					if (start.targets.Count > 0 && end.targets.Count == 0)
					{
						sb.Append("\n");
						sb.Append(end.name());
						sb.Append(" -> ");
						sb.Append(start.name());
						sb.Append("\n");
					}
					else if (start.targets.Count == 0 && end.targets.Count > 0)
					{
						sb.Append("\n");
						sb.Append(start.name());
						sb.Append(" -> ");
						sb.Append(end.name());
						sb.Append("\n");
					}
				}
				else
				{
					if (behaviour == MATCH_CONSTRAINT && root.widget.getDimensionRatio() > 0)
					{
						sb.Append("\n");
						sb.Append(root.widget.DebugName);
						sb.Append("_VERTICAL -> ");
						sb.Append(root.widget.DebugName);
						sb.Append("_HORIZONTAL;\n");
					}
				}
			}
			if (root is ChainRun)
			{
				return generateChainDisplayGraph((ChainRun) root, content);
			}
			return sb.ToString();
		}

	}

}