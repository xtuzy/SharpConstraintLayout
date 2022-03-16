using System;
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
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.GONE;
using static androidx.constraintlayout.core.widgets.ConstraintWidget;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.HORIZONTAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_SPREAD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_WRAP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;

	/// <summary>
	/// Implements basic measure for linear resolution
	/// </summary>
	public class BasicMeasure
	{

		private const bool DEBUG = false;
		private const int MODE_SHIFT = 30;
		/// <summary>
		/// The parent has not imposed any constraint on the child. It can be whatever size it wants.
		/// <see cref="https://developer.android.com/reference/android/view/View.MeasureSpec"/>
		/// </summary>
		public const int UNSPECIFIED = 0;
		/// <summary>
		/// The parent has determined an exact size for the child. The child is going to be given those bounds regardless of how big it wants to be.
		/// <see cref="https://developer.android.com/reference/android/view/View.MeasureSpec"/>
		/// </summary>
		public static readonly int EXACTLY = 1 << MODE_SHIFT;
		/// <summary>
		/// The child can be as large as it wants up to the specified size.
		/// <see cref="https://developer.android.com/reference/android/view/View.MeasureSpec"/>
		/// </summary>
		public static readonly int AT_MOST = 2 << MODE_SHIFT;

		public const int MATCH_PARENT = -1;
		public const int WRAP_CONTENT = -2;
		public const int FIXED = -3;

		private readonly List<ConstraintWidget> mVariableDimensionsWidgets = new List<ConstraintWidget>();
		private Measure mMeasure = new Measure();

		public virtual void updateHierarchy(ConstraintWidgetContainer layout)
		{
			mVariableDimensionsWidgets.Clear();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childCount = layout.mChildren.size();
			int childCount = layout.mChildren.Count;
			for (int i = 0; i < childCount; i++)
			{
				ConstraintWidget widget = layout.mChildren[i];
				if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT || widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
				{
					mVariableDimensionsWidgets.Add(widget);
				}
			}
			layout.invalidateGraph();
		}

		private ConstraintWidgetContainer constraintWidgetContainer;

		public BasicMeasure(ConstraintWidgetContainer constraintWidgetContainer)
		{
			this.constraintWidgetContainer = constraintWidgetContainer;
		}

		private void measureChildren(ConstraintWidgetContainer layout)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childCount = layout.mChildren.size();
			int childCount = layout.mChildren.Count;
			bool optimize = layout.optimizeFor(Optimizer.OPTIMIZATION_GRAPH);
			Measurer measurer = layout.Measurer;
			for (int i = 0; i < childCount; i++)
			{
				ConstraintWidget child = layout.mChildren[i];
				if (child is Guideline)
				{
					continue;
				}
				if (child is Barrier)
				{
					continue;
				}
				if (child.InVirtualLayout)
				{
					continue;
				}

				if (optimize && child.horizontalRun != null && child.verticalRun != null && child.horizontalRun.dimension.resolved && child.verticalRun.dimension.resolved)
				{
					continue;
				}

				ConstraintWidget.DimensionBehaviour? widthBehavior = child.getDimensionBehaviour(HORIZONTAL);
				ConstraintWidget.DimensionBehaviour? heightBehavior = child.getDimensionBehaviour(VERTICAL);

				bool skip = widthBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && child.mMatchConstraintDefaultWidth != MATCH_CONSTRAINT_WRAP && heightBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && child.mMatchConstraintDefaultHeight != MATCH_CONSTRAINT_WRAP;

				if (!skip && layout.optimizeFor(Optimizer.OPTIMIZATION_DIRECT) && !(child is VirtualLayout))
				{
					if (widthBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && child.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD && heightBehavior != ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && !child.InHorizontalChain)
					{
						skip = true;
					}

					if (heightBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && child.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD && widthBehavior != ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && !child.InHorizontalChain)
					{
						skip = true;
					}

					// Don't measure yet -- let the direct solver have a shot at it.
					if ((widthBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT || heightBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT) && child.mDimensionRatio > 0)
					{
						skip = true;
					}
				}

				if (skip)
				{
					// we don't need to measure here as the dimension of the widget
					// will be completely computed by the solver.
					continue;
				}

				measure(measurer, child, Measure.SELF_DIMENSIONS);
				if (layout.mMetrics != null)
				{
					layout.mMetrics.measuredWidgets++;
				}
			}
			measurer.didMeasures();
		}

		private void solveLinearSystem(ConstraintWidgetContainer layout, string reason, int pass, int w, int h)
		{
			long startLayout;
			if (LinearSystem.MEASURE)
			{
				startLayout = DateTimeHelperClass.nanoTime();
			}

			int minWidth = layout.MinWidth;
			int minHeight = layout.MinHeight;
			layout.MinWidth = 0;
			layout.MinHeight = 0;
			layout.Width = w;
			layout.Height = h;
			layout.MinWidth = minWidth;
			layout.MinHeight = minHeight;
			if (DEBUG)
			{
				Console.WriteLine("### Solve <" + reason + "> ###");
			}
			constraintWidgetContainer.Pass = pass;
			constraintWidgetContainer.layout();
			if (LinearSystem.MEASURE && layout.mMetrics != null)
			{
					long endLayout = DateTimeHelperClass.nanoTime();
				layout.mMetrics.measuresLayoutDuration += (endLayout - startLayout);
			}
		}

		/// <summary>
		/// Called by ConstraintLayout onMeasure()
		/// </summary>
		/// <param name="layout"> </param>
		/// <param name="optimizationLevel"> </param>
		/// <param name="widthMode"> </param>
		/// <param name="widthSize"> </param>
		/// <param name="heightMode"> </param>
		/// <param name="heightSize"> </param>
		/// <param name="lastMeasureWidth"> </param>
		/// <param name="lastMeasureHeight"> </param>
		public virtual long solverMeasure(ConstraintWidgetContainer layout, int optimizationLevel, int paddingX, int paddingY, int widthMode, int widthSize, int heightMode, int heightSize, int lastMeasureWidth, int lastMeasureHeight)
		{
			Measurer measurer = layout.Measurer;
			long layoutTime = 0;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childCount = layout.mChildren.size();
			int childCount = layout.mChildren.Count;
			int startingWidth = layout.Width;
			int startingHeight = layout.Height;

			bool optimizeWrap = Optimizer.enabled(optimizationLevel, Optimizer.OPTIMIZATION_GRAPH_WRAP);
			bool optimize = optimizeWrap || Optimizer.enabled(optimizationLevel, Optimizer.OPTIMIZATION_GRAPH);

			if (optimize)
			{
				for (int i = 0; i < childCount; i++)
				{
					ConstraintWidget child = layout.mChildren[i];
					bool matchWidth = child.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
					bool matchHeight = child.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
					bool ratio = matchWidth && matchHeight && child.getDimensionRatio() > 0;
					if (child.InHorizontalChain && (ratio))
					{
						optimize = false;
						break;
					}
					if (child.InVerticalChain && (ratio))
					{
						optimize = false;
						break;
					}
					if (child is VirtualLayout)
					{
						optimize = false;
						break;
					}
					if (child.InHorizontalChain || child.InVerticalChain)
					{
						optimize = false;
						break;
					}
				}
			}

			if (optimize && LinearSystem.sMetrics != null)
			{
				LinearSystem.sMetrics.measures++;
			}

			bool allSolved = false;

			optimize &= (widthMode == EXACTLY && heightMode == EXACTLY) || optimizeWrap;

			int computations = 0;

			if (optimize)
			{
				// For non-optimizer this doesn't seem to be a problem.
				// For both cases, having the width address max size early seems to work (which makes sense).
				// Putting it specific to optimizer to reduce unnecessary risk.
				widthSize = Math.Min(layout.MaxWidth, widthSize);
				heightSize = Math.Min(layout.MaxHeight, heightSize);

				if (widthMode == EXACTLY && layout.Width != widthSize)
				{
					layout.Width = widthSize;
					layout.invalidateGraph();
				}
				if (heightMode == EXACTLY && layout.Height != heightSize)
				{
					layout.Height = heightSize;
					layout.invalidateGraph();
				}
				if (widthMode == EXACTLY && heightMode == EXACTLY)
				{
					allSolved = layout.directMeasure(optimizeWrap);
					computations = 2;
				}
				else
				{
					allSolved = layout.directMeasureSetup(optimizeWrap);
					if (widthMode == EXACTLY)
					{
						allSolved &= layout.directMeasureWithOrientation(optimizeWrap, HORIZONTAL);
						computations++;
					}
					if (heightMode == EXACTLY)
					{
						allSolved &= layout.directMeasureWithOrientation(optimizeWrap, VERTICAL);
						computations++;
					}
				}
				if (allSolved)
				{
					layout.updateFromRuns(widthMode == EXACTLY, heightMode == EXACTLY);
				}
			}
			else
			{
				if (false)
				{
					layout.horizontalRun.clear();
					layout.verticalRun.clear();
					foreach (ConstraintWidget child in layout.Children)
					{
						child.horizontalRun.clear();
						child.verticalRun.clear();
					}
				}
			}

			if (!allSolved || computations != 2)
			{
				int optimizations = layout.OptimizationLevel;
				if (childCount > 0)
				{
					measureChildren(layout);
				}
				if (LinearSystem.MEASURE)
				{
					layoutTime = DateTimeHelperClass.nanoTime();
				}

				updateHierarchy(layout);

				// let's update the size dependent widgets if any...
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int sizeDependentWidgetsCount = mVariableDimensionsWidgets.size();
				int sizeDependentWidgetsCount = mVariableDimensionsWidgets.Count;

				// let's solve the linear system.
				if (childCount > 0)
				{
					solveLinearSystem(layout, "First pass", 0, startingWidth, startingHeight);
				}

				if (DEBUG)
				{
					Console.WriteLine("size dependent widgets: " + sizeDependentWidgetsCount);
				}

				if (sizeDependentWidgetsCount > 0)
				{
					bool needSolverPass = false;
					bool containerWrapWidth = layout.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
					bool containerWrapHeight = layout.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
					int minWidth = Math.Max(layout.Width, constraintWidgetContainer.MinWidth);
					int minHeight = Math.Max(layout.Height, constraintWidgetContainer.MinHeight);

					////////////////////////////////////////////////////////////////////////////////////
					// Let's first apply sizes for VirtualLayouts if any
					////////////////////////////////////////////////////////////////////////////////////
					for (int i = 0; i < sizeDependentWidgetsCount; i++)
					{
						ConstraintWidget widget = mVariableDimensionsWidgets[i];
						if (!(widget is VirtualLayout))
						{
							continue;
						}
						int preWidth = widget.Width;
						int preHeight = widget.Height;
						needSolverPass |= measure(measurer, widget, Measure.TRY_GIVEN_DIMENSIONS);
						if (layout.mMetrics != null)
						{
							layout.mMetrics.measuredMatchWidgets++;
						}
						int measuredWidth = widget.Width;
						int measuredHeight = widget.Height;
						if (measuredWidth != preWidth)
						{
							widget.Width = measuredWidth;
							if (containerWrapWidth && widget.Right > minWidth)
							{
								int w = widget.Right + widget.getAnchor(ConstraintAnchor.Type.RIGHT).Margin;
								minWidth = Math.Max(minWidth, w);
							}
							needSolverPass = true;
						}
						if (measuredHeight != preHeight)
						{
							widget.Height = measuredHeight;
							if (containerWrapHeight && widget.Bottom > minHeight)
							{
								int h = widget.Bottom + widget.getAnchor(ConstraintAnchor.Type.BOTTOM).Margin;
								minHeight = Math.Max(minHeight, h);
							}
							needSolverPass = true;
						}
						VirtualLayout virtualLayout = (VirtualLayout) widget;
						needSolverPass |= virtualLayout.needSolverPass();
					}
					////////////////////////////////////////////////////////////////////////////////////

					int maxIterations = 2;
					for (int j = 0; j < maxIterations; j++)
					{
						for (int i = 0; i < sizeDependentWidgetsCount; i++)
						{
							ConstraintWidget widget = mVariableDimensionsWidgets[i];
							if ((widget is Helper && !(widget is VirtualLayout)) || widget is Guideline)
							{
								continue;
							}
							if (widget.Visibility == GONE)
							{
								continue;
							}
							if (optimize && widget.horizontalRun.dimension.resolved && widget.verticalRun.dimension.resolved)
							{
								continue;
							}
							if (widget is VirtualLayout)
							{
								continue;
							}

							int preWidth = widget.Width;
							int preHeight = widget.Height;
							int preBaselineDistance = widget.BaselineDistance;

							int measureStrategy = Measure.TRY_GIVEN_DIMENSIONS;
							if (j == maxIterations - 1)
							{
								measureStrategy = Measure.USE_GIVEN_DIMENSIONS;
							}
							bool hasMeasure = measure(measurer, widget, measureStrategy);
							if (false && !widget.hasDependencies())
							{
								hasMeasure = false;
							}
							needSolverPass |= hasMeasure;
							if (DEBUG && hasMeasure)
							{
								Console.WriteLine("{#} Needs Solver pass as measure true for " + widget.DebugName);
							}
							if (layout.mMetrics != null)
							{
								layout.mMetrics.measuredMatchWidgets++;
							}

							int measuredWidth = widget.Width;
							int measuredHeight = widget.Height;

							if (measuredWidth != preWidth)
							{
								widget.Width = measuredWidth;
								if (containerWrapWidth && widget.Right > minWidth)
								{
									int w = widget.Right + widget.getAnchor(ConstraintAnchor.Type.RIGHT).Margin;
									minWidth = Math.Max(minWidth, w);
								}
								if (DEBUG)
								{
									Console.WriteLine("{#} Needs Solver pass as Width for " + widget.DebugName + " changed: " + measuredWidth + " != " + preWidth);
								}
								needSolverPass = true;
							}
							if (measuredHeight != preHeight)
							{
								widget.Height = measuredHeight;
								if (containerWrapHeight && widget.Bottom > minHeight)
								{
									int h = widget.Bottom + widget.getAnchor(ConstraintAnchor.Type.BOTTOM).Margin;
									minHeight = Math.Max(minHeight, h);
								}
								if (DEBUG)
								{
									Console.WriteLine("{#} Needs Solver pass as Height for " + widget.DebugName + " changed: " + measuredHeight + " != " + preHeight);
								}
								needSolverPass = true;
							}
							if (widget.hasBaseline() && preBaselineDistance != widget.BaselineDistance)
							{
								if (DEBUG)
								{
									Console.WriteLine("{#} Needs Solver pass as Baseline for " + widget.DebugName + " changed: " + widget.BaselineDistance + " != " + preBaselineDistance);
								}
								needSolverPass = true;
							}
						}
						if (needSolverPass)
						{
							solveLinearSystem(layout, "intermediate pass", 1 + j, startingWidth, startingHeight);
							needSolverPass = false;
						}
						else
						{
							break;
						}
					}
				}
				layout.OptimizationLevel = optimizations;
			}
			if (LinearSystem.MEASURE)
			{
				layoutTime = (DateTimeHelperClass.nanoTime() - layoutTime);
			}
			return layoutTime;
		}

		/// <summary>
		/// Convenience function to fill in the measure spec
		/// </summary>
		/// <param name="measurer"> the measurer callback </param>
		/// <param name="widget"> the widget to measure </param>
		/// <param name="measureStrategy"> how to use the current ConstraintWidget dimensions during the measure </param>
		/// <returns> true if needs another solver pass </returns>
		private bool measure(Measurer measurer, ConstraintWidget widget, int measureStrategy)
		{
			mMeasure.horizontalBehavior = widget.HorizontalDimensionBehaviour;
			mMeasure.verticalBehavior = widget.VerticalDimensionBehaviour;
			mMeasure.horizontalDimension = widget.Width;
			mMeasure.verticalDimension = widget.Height;
			mMeasure.measuredNeedsSolverPass = false;
			mMeasure.measureStrategy = measureStrategy;

			bool horizontalMatchConstraints = (mMeasure.horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
			bool verticalMatchConstraints = (mMeasure.verticalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
			bool horizontalUseRatio = horizontalMatchConstraints && widget.mDimensionRatio > 0;
			bool verticalUseRatio = verticalMatchConstraints && widget.mDimensionRatio > 0;

			if (horizontalUseRatio)
			{
				if (widget.mResolvedMatchConstraintDefault[HORIZONTAL] == ConstraintWidget.MATCH_CONSTRAINT_RATIO_RESOLVED)
				{
					mMeasure.horizontalBehavior = ConstraintWidget.DimensionBehaviour.FIXED;
				}
			}
			if (verticalUseRatio)
			{
				if (widget.mResolvedMatchConstraintDefault[VERTICAL] == ConstraintWidget.MATCH_CONSTRAINT_RATIO_RESOLVED)
				{
					mMeasure.verticalBehavior = ConstraintWidget.DimensionBehaviour.FIXED;
				}
			}

			measurer.measure(widget, mMeasure);
			widget.Width = mMeasure.measuredWidth;
			widget.Height = mMeasure.measuredHeight;
			widget.HasBaseline = mMeasure.measuredHasBaseline;
			widget.BaselineDistance = mMeasure.measuredBaseline;
			mMeasure.measureStrategy = Measure.SELF_DIMENSIONS;
			return mMeasure.measuredNeedsSolverPass;
		}

		public interface Measurer
		{
			void measure(ConstraintWidget widget, Measure measure);
			void didMeasures();
		}

		public class Measure
		{
			public static int SELF_DIMENSIONS = 0;
			public static int TRY_GIVEN_DIMENSIONS = 1;
			public static int USE_GIVEN_DIMENSIONS = 2;
			public ConstraintWidget.DimensionBehaviour horizontalBehavior;
			public ConstraintWidget.DimensionBehaviour verticalBehavior;
			public int horizontalDimension;
			public int verticalDimension;
			public int measuredWidth;
			public int measuredHeight;
			public int measuredBaseline;
			public bool measuredHasBaseline;
			public bool measuredNeedsSolverPass;
			public int measureStrategy;
		}
	}

}