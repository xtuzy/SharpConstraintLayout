using System;
using System.Collections.Generic;
using System.Text;

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
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.GONE;
using static androidx.constraintlayout.core.widgets.ConstraintWidget;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.HORIZONTAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.MATCH_CONSTRAINT_WRAP;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.ConstraintWidget.VERTICAL;

	/// <summary>
	/// Direct resolution engine
	/// 
	/// This walks through the graph of dependencies and infer final position. This allows
	/// us to skip the linear solver in many situations, as well as skipping intermediate measure passes.
	/// 
	/// Widgets are solved independently in horizontal and vertical. Any widgets not fully resolved
	/// will be computed later on by the linear solver.
	/// </summary>
	public class Direct
	{

		private const bool DEBUG = LinearSystem.FULL_DEBUG;
		private const bool APPLY_MATCH_PARENT = false;
		private static BasicMeasure.Measure measure = new BasicMeasure.Measure();
		private const bool EARLY_TERMINATION = true; // feature flag -- remove after release.

		private static int hcount = 0;
		private static int vcount = 0;

		/// <summary>
		/// Walk the dependency graph and solves it.
		/// </summary>
		/// <param name="layout"> the container we want to optimize </param>
		/// <param name="measurer"> the measurer used to measure the widget </param>
		public static void solvingPass(ConstraintWidgetContainer layout, BasicMeasure.Measurer measurer)
		{
			ConstraintWidget.DimensionBehaviour horizontal = layout.HorizontalDimensionBehaviour;
			ConstraintWidget.DimensionBehaviour vertical = layout.VerticalDimensionBehaviour;
			hcount = 0;
			vcount = 0;
			long time = 0;
			if (DEBUG)
			{
				time = DateTimeHelperClass.nanoTime();
				Console.WriteLine("#### SOLVING PASS (horiz " + horizontal + ", vert " + vertical + ") ####");
			}
			layout.resetFinalResolution();
			List<ConstraintWidget> children = layout.Children;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = children.size();
			int count = children.Count;
			if (DEBUG)
			{
				Console.WriteLine("#### SOLVING PASS on " + count + " widgeets ####");
			}
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget child = children[i];
				child.resetFinalResolution();
			}

			bool isRtl = layout.Rtl;

			// First, let's solve the horizontal dependencies, as it's a lot more common to have
			// a container with a fixed horizontal dimension (e.g. match_parent) than the opposite.

			// If we know our size, we can fully set the entire dimension, but if not we can
			// still solve what we can starting from the left.
			if (horizontal == ConstraintWidget.DimensionBehaviour.FIXED)
			{
				layout.setFinalHorizontal(0, layout.Width);
			}
			else
			{
				layout.FinalLeft = 0;
			}

			if (DEBUG)
			{
				Console.WriteLine("\n### Let's solve horizontal dependencies ###\n");
			}

			// Then let's first try to solve horizontal guidelines, as they only depends on the container
			bool hasGuideline = false;
			bool hasBarrier = false;
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget child = children[i];
				if (child is Guideline)
				{
					Guideline guideline = (Guideline) child;
					if (guideline.Orientation == Guideline.VERTICAL)
					{
						if (guideline.RelativeBegin != -1)
						{
							guideline.FinalValue = guideline.RelativeBegin;
						}
						else if (guideline.RelativeEnd != -1 && layout.ResolvedHorizontally)
						{
							guideline.FinalValue = layout.Width - guideline.RelativeEnd;
						}
						else if (layout.ResolvedHorizontally)
						{
							int position = (int)(0.5f + guideline.RelativePercent * layout.Width);
							guideline.FinalValue = position;
						}
						hasGuideline = true;
					}
				}
				else if (child is Barrier)
				{
					Barrier barrier = (Barrier) child;
					if (barrier.Orientation == HORIZONTAL)
					{
						hasBarrier = true;
					}
				}
			}
			if (hasGuideline)
			{
				if (DEBUG)
				{
					Console.WriteLine("\n#### VERTICAL GUIDELINES CHECKS ####");
				}
				for (int i = 0; i < count; i++)
				{
					ConstraintWidget child = children[i];
					if (child is Guideline)
					{
						Guideline guideline = (Guideline) child;
						if (guideline.Orientation == Guideline.VERTICAL)
						{
							horizontalSolvingPass(0, guideline, measurer, isRtl);
						}
					}
				}
				if (DEBUG)
				{
					Console.WriteLine("### Done solving guidelines.");
				}
			}

			if (DEBUG)
			{
				Console.WriteLine("\n#### HORIZONTAL SOLVING PASS ####");
			}

			// Now let's resolve what we can in the dependencies of the container
			horizontalSolvingPass(0, layout, measurer, isRtl);

			// Finally, let's go through barriers, as they depends on widgets that may have been solved.
			if (hasBarrier)
			{
				if (DEBUG)
				{
					Console.WriteLine("\n#### HORIZONTAL BARRIER CHECKS ####");
				}
				for (int i = 0; i < count; i++)
				{
					ConstraintWidget child = children[i];
					if (child is Barrier)
					{
						Barrier barrier = (Barrier) child;
						if (barrier.Orientation == HORIZONTAL)
						{
							solveBarrier(0, barrier, measurer, HORIZONTAL, isRtl);
						}
					}
				}
				if (DEBUG)
				{
					Console.WriteLine("#### DONE HORIZONTAL BARRIER CHECKS ####");
				}
			}

			if (DEBUG)
			{
				Console.WriteLine("\n### Let's solve vertical dependencies now ###\n");
			}

			// Now we are done with the horizontal axis, let's see what we can do vertically
			if (vertical == ConstraintWidget.DimensionBehaviour.FIXED)
			{
				layout.setFinalVertical(0, layout.Height);
			}
			else
			{
				layout.FinalTop = 0;
			}

			// Same thing as above -- let's start with guidelines...
			hasGuideline = false;
			hasBarrier = false;
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget child = children[i];
				if (child is Guideline)
				{
					Guideline guideline = (Guideline) child;
					if (guideline.Orientation == Guideline.HORIZONTAL)
					{
						if (guideline.RelativeBegin != -1)
						{
							guideline.FinalValue = guideline.RelativeBegin;
						}
						else if (guideline.RelativeEnd != -1 && layout.ResolvedVertically)
						{
							guideline.FinalValue = layout.Height - guideline.RelativeEnd;
						}
						else if (layout.ResolvedVertically)
						{
							int position = (int)(0.5f + guideline.RelativePercent * layout.Height);
							guideline.FinalValue = position;
						}
						hasGuideline = true;
					}
				}
				else if (child is Barrier)
				{
					Barrier barrier = (Barrier) child;
					if (barrier.Orientation == ConstraintWidget.VERTICAL)
					{
						hasBarrier = true;
					}
				}
			}
			if (hasGuideline)
			{
				if (DEBUG)
				{
					Console.WriteLine("\n#### HORIZONTAL GUIDELINES CHECKS ####");
				}
				for (int i = 0; i < count; i++)
				{
					ConstraintWidget child = children[i];
					if (child is Guideline)
					{
						Guideline guideline = (Guideline) child;
						if (guideline.Orientation == Guideline.HORIZONTAL)
						{
							verticalSolvingPass(1, guideline, measurer);
						}
					}
				}
				if (DEBUG)
				{
					Console.WriteLine("\n### Done solving guidelines.");
				}
			}

			if (DEBUG)
			{
				Console.WriteLine("\n#### VERTICAL SOLVING PASS ####");
			}

			// ...then solve the vertical dependencies...
			verticalSolvingPass(0, layout, measurer);

			// ...then deal with any barriers left.
			if (hasBarrier)
			{
				if (DEBUG)
				{
					Console.WriteLine("#### VERTICAL BARRIER CHECKS ####");
				}
				for (int i = 0; i < count; i++)
				{
					ConstraintWidget child = children[i];
					if (child is Barrier)
					{
						Barrier barrier = (Barrier) child;
						if (barrier.Orientation == ConstraintWidget.VERTICAL)
						{
							solveBarrier(0, barrier, measurer, VERTICAL, isRtl);
						}
					}
				}
			}

			if (DEBUG)
			{
				Console.WriteLine("\n#### LAST PASS ####");
			}
			// We can do a last pass to see any widget that could still be measured
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget child = children[i];
				if (child.MeasureRequested && canMeasure(0, child))
				{
					ConstraintWidgetContainer.measure(0, child, measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
					if (child is Guideline)
					{
						if (((Guideline) child).Orientation == Guideline.HORIZONTAL)
						{
							verticalSolvingPass(0, child, measurer);
						}
						else
						{
							horizontalSolvingPass(0, child, measurer, isRtl);
						}
					}
					else
					{
						horizontalSolvingPass(0, child, measurer, isRtl);
						verticalSolvingPass(0, child, measurer);
					}
				}
			}

			if (DEBUG)
			{
				time = DateTimeHelperClass.nanoTime() - time;
				Console.WriteLine("\n*** THROUGH WITH DIRECT PASS in " + time + " ns ***\n");
				Console.WriteLine("hcount: " + hcount + " vcount: " + vcount);
			}
		}

		/// <summary>
		/// Ask the barrier if it's resolved, and if so do a solving pass </summary>
		/// <param name="level"> </param>
		/// <param name="barrier"> </param>
		/// <param name="measurer"> </param>
		/// <param name="isRtl"> </param>
		private static void solveBarrier(int level, Barrier barrier, BasicMeasure.Measurer measurer, int orientation, bool isRtl)
		{
			if (barrier.allSolved())
			{
				if (orientation == HORIZONTAL)
				{
					horizontalSolvingPass(level + 1, barrier, measurer, isRtl);
				}
				else
				{
					verticalSolvingPass(level + 1, barrier, measurer);
				}
			}
		}

		/// <summary>
		/// Small utility function to indent logs depending on the level </summary>
		/// <param name="level"> </param>
		/// <returns> a formatted string for the indentation </returns>
		public static string ls(int level)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < level; i++)
			{
				builder.Append("  ");
			}
			builder.Append("+-(" + level + ") ");
			return builder.ToString();
		}

		/// <summary>
		/// Does an horizontal solving pass for the given widget. This will walk through the widget's
		/// horizontal dependencies and if they can be resolved directly, do so. </summary>
		/// <param name="level"> </param>
		/// <param name="layout"> the widget we want to solve the dependencies </param>
		/// <param name="measurer"> the measurer object to measure the widgets. </param>
		/// <param name="isRtl"> </param>
		private static void horizontalSolvingPass(int level, ConstraintWidget layout, BasicMeasure.Measurer measurer, bool isRtl)
		{
			if (EARLY_TERMINATION && layout.HorizontalSolvingPassDone)
			{
				if (DEBUG)
				{
					Console.WriteLine(ls(level) + "HORIZONTAL SOLVING PASS ON " + layout.DebugName + " ALREADY CALLED");
				}
				return;
			}
			hcount++;
			if (DEBUG)
			{
				Console.WriteLine(ls(level) + "HORIZONTAL SOLVING PASS ON " + layout.DebugName);
			}

			if (!(layout is ConstraintWidgetContainer) && layout.MeasureRequested && canMeasure(level + 1, layout))
			{
				BasicMeasure.Measure measure = new BasicMeasure.Measure();
				ConstraintWidgetContainer.measure(level + 1, layout, measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
			}

			ConstraintAnchor left = layout.getAnchor(ConstraintAnchor.Type.LEFT);
			ConstraintAnchor right = layout.getAnchor(ConstraintAnchor.Type.RIGHT);
			int l = left.FinalValue;
			int r = right.FinalValue;

			if (left.Dependents != null && left.hasFinalValue())
			{
				foreach (ConstraintAnchor first in left.Dependents)
				{
					ConstraintWidget widget = first.mOwner;
					int x1 = 0;
					int x2 = 0;
					bool isCanMeasure = canMeasure(level + 1, widget);
					if (widget.MeasureRequested && isCanMeasure)
					{
						BasicMeasure.Measure measure = new BasicMeasure.Measure();
						ConstraintWidgetContainer.measure(level + 1, widget, measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
					}

					if (widget.HorizontalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT || isCanMeasure)
					{
						if (widget.MeasureRequested)
						{
							// Widget needs to be measured
							if (DEBUG)
							{
								Console.WriteLine(ls(level + 1) + "(L) We didn't measure " + widget.DebugName + ", let's bail");
							}
							continue;
						}
						if (first == widget.mLeft && widget.mRight.mTarget == null)
						{
							x1 = l + widget.mLeft.Margin;
							x2 = x1 + widget.Width;
							widget.setFinalHorizontal(x1, x2);
							horizontalSolvingPass(level + 1, widget, measurer, isRtl);
						}
						else if (first == widget.mRight && widget.mLeft.mTarget == null)
						{
							x2 = l - widget.mRight.Margin;
							x1 = x2 - widget.Width;
							widget.setFinalHorizontal(x1, x2);
							horizontalSolvingPass(level + 1, widget, measurer, isRtl);
						}
						else if (first == widget.mLeft && widget.mRight.mTarget != null && widget.mRight.mTarget.hasFinalValue() && !widget.InHorizontalChain)
						{
							solveHorizontalCenterConstraints(level + 1, measurer, widget, isRtl);
						}
						else if (APPLY_MATCH_PARENT && widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
						{
							widget.setFinalHorizontal(0, widget.Width);
							horizontalSolvingPass(level + 1, widget, measurer, isRtl);
						}
					}
					else if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && widget.mMatchConstraintMaxWidth >= 0 && widget.mMatchConstraintMinWidth >= 0 && (widget.Visibility == ConstraintWidget.GONE || ((widget.mMatchConstraintDefaultWidth == ConstraintWidget.MATCH_CONSTRAINT_SPREAD) && widget.getDimensionRatio() == 0)) && !widget.InHorizontalChain && !widget.InVirtualLayout)
					{
						bool bothConnected = (first == widget.mLeft && widget.mRight.mTarget != null && widget.mRight.mTarget.hasFinalValue()) || (first == widget.mRight && widget.mLeft.mTarget != null && widget.mLeft.mTarget.hasFinalValue());
						if (bothConnected && !widget.InHorizontalChain)
						{
							solveHorizontalMatchConstraint(level + 1, layout, measurer, widget, isRtl);
						}
					}
				}
			}
			if (layout is Guideline)
			{
				return;
			}
			if (right.Dependents != null && right.hasFinalValue())
			{
				foreach (ConstraintAnchor first in right.Dependents)
				{
					ConstraintWidget widget = first.mOwner;
					bool isCanMeasure = canMeasure(level + 1, widget);
					if (widget.MeasureRequested && isCanMeasure)
					{
						BasicMeasure.Measure measure = new BasicMeasure.Measure();
						ConstraintWidgetContainer.measure(level + 1, widget, measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
					}

					int x1 = 0;
					int x2 = 0;
					bool bothConnected = (first == widget.mLeft && widget.mRight.mTarget != null && widget.mRight.mTarget.hasFinalValue()) || (first == widget.mRight && widget.mLeft.mTarget != null && widget.mLeft.mTarget.hasFinalValue());
					if (widget.HorizontalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT || isCanMeasure)
					{
						if (widget.MeasureRequested)
						{
							// Widget needs to be measured
							if (DEBUG)
							{
								Console.WriteLine(ls(level + 1) + "(R) We didn't measure " + widget.DebugName + ", le'ts bail");
							}
							continue;
						}
						if (first == widget.mLeft && widget.mRight.mTarget == null)
						{
							x1 = r + widget.mLeft.Margin;
							x2 = x1 + widget.Width;
							widget.setFinalHorizontal(x1, x2);
							horizontalSolvingPass(level + 1, widget, measurer, isRtl);
						}
						else if (first == widget.mRight && widget.mLeft.mTarget == null)
						{
							x2 = r - widget.mRight.Margin;
							x1 = x2 - widget.Width;
							widget.setFinalHorizontal(x1, x2);
							horizontalSolvingPass(level + 1, widget, measurer, isRtl);
						}
						else if (bothConnected && !widget.InHorizontalChain)
						{
							solveHorizontalCenterConstraints(level + 1, measurer, widget, isRtl);
						}
					}
					else if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && widget.mMatchConstraintMaxWidth >= 0 && widget.mMatchConstraintMinWidth >= 0 && (widget.Visibility == ConstraintWidget.GONE || ((widget.mMatchConstraintDefaultWidth == ConstraintWidget.MATCH_CONSTRAINT_SPREAD) && widget.getDimensionRatio() == 0)) && !widget.InHorizontalChain && !widget.InVirtualLayout)
					{
						if (bothConnected && !widget.InHorizontalChain)
						{
							solveHorizontalMatchConstraint(level + 1, layout, measurer, widget, isRtl);
						}
					}
				}
			}
			layout.markHorizontalSolvingPassDone();
		}

		/// <summary>
		/// Does an vertical solving pass for the given widget. This will walk through the widget's
		/// vertical dependencies and if they can be resolved directly, do so.
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="layout"> the widget we want to solve the dependencies </param>
		/// <param name="measurer"> the measurer object to measure the widgets. </param>
		private static void verticalSolvingPass(int level, ConstraintWidget layout, BasicMeasure.Measurer measurer)
		{
			if (EARLY_TERMINATION && layout.VerticalSolvingPassDone)
			{
				if (DEBUG)
				{
					Console.WriteLine(ls(level) + "VERTICAL SOLVING PASS ON " + layout.DebugName + " ALREADY CALLED");
				}
				return;
			}
			vcount++;
			if (DEBUG)
			{
				Console.WriteLine(ls(level) + "VERTICAL SOLVING PASS ON " + layout.DebugName);
			}

			if (!(layout is ConstraintWidgetContainer) && layout.MeasureRequested && canMeasure(level + 1, layout))
			{
				BasicMeasure.Measure measure = new BasicMeasure.Measure();
				ConstraintWidgetContainer.measure(level + 1, layout, measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
			}

			ConstraintAnchor top = layout.getAnchor(ConstraintAnchor.Type.TOP);
			ConstraintAnchor bottom = layout.getAnchor(ConstraintAnchor.Type.BOTTOM);
			int t = top.FinalValue;
			int b = bottom.FinalValue;

			if (top.Dependents != null && top.hasFinalValue())
			{
				foreach (ConstraintAnchor first in top.Dependents)
				{
					ConstraintWidget widget = first.mOwner;
					int y1 = 0;
					int y2 = 0;
					bool iscanMeasure = canMeasure(level + 1, widget);
					if (widget.MeasureRequested && iscanMeasure)
					{
						BasicMeasure.Measure measure = new BasicMeasure.Measure();
						ConstraintWidgetContainer.measure(level + 1, widget, measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
					}

					if (widget.VerticalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT || iscanMeasure)
					{
						if (widget.MeasureRequested)
						{
							// Widget needs to be measured
							if (DEBUG)
							{
								Console.WriteLine(ls(level + 1) + "(T) We didn't measure " + widget.DebugName + ", le'ts bail");
							}
							continue;
						}
						if (first == widget.mTop && widget.mBottom.mTarget == null)
						{
							y1 = t + widget.mTop.Margin;
							y2 = y1 + widget.Height;
							widget.setFinalVertical(y1, y2);
							verticalSolvingPass(level + 1, widget, measurer);
						}
						else if (first == widget.mBottom && widget.mBottom.mTarget == null)
						{
							y2 = t - widget.mBottom.Margin;
							y1 = y2 - widget.Height;
							widget.setFinalVertical(y1, y2);
							verticalSolvingPass(level + 1, widget, measurer);
						}
						else if (first == widget.mTop && widget.mBottom.mTarget != null && widget.mBottom.mTarget.hasFinalValue())
						{
							solveVerticalCenterConstraints(level + 1, measurer, widget);
						}
						else if (APPLY_MATCH_PARENT && widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
						{
							widget.setFinalVertical(0, widget.Height);
							verticalSolvingPass(level + 1, widget, measurer);
						}
					}
					else if (widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && widget.mMatchConstraintMaxHeight >= 0 && widget.mMatchConstraintMinHeight >= 0 && (widget.Visibility == ConstraintWidget.GONE || ((widget.mMatchConstraintDefaultHeight == ConstraintWidget.MATCH_CONSTRAINT_SPREAD) && widget.getDimensionRatio() == 0)) && !widget.InVerticalChain && !widget.InVirtualLayout)
					{
						bool bothConnected = (first == widget.mTop && widget.mBottom.mTarget != null && widget.mBottom.mTarget.hasFinalValue()) || (first == widget.mBottom && widget.mTop.mTarget != null && widget.mTop.mTarget.hasFinalValue());
						if (bothConnected && !widget.InVerticalChain)
						{
							solveVerticalMatchConstraint(level + 1, layout, measurer, widget);
						}
					}
				}
			}
			if (layout is Guideline)
			{
				return;
			}
			if (bottom.Dependents != null && bottom.hasFinalValue())
			{
				foreach (ConstraintAnchor first in bottom.Dependents)
				{
					ConstraintWidget widget = first.mOwner;
					bool iscanMeasure = canMeasure(level + 1, widget);
					if (widget.MeasureRequested && iscanMeasure)
					{
						BasicMeasure.Measure measure = new BasicMeasure.Measure();
						ConstraintWidgetContainer.measure(level + 1, widget, measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
					}

					int y1 = 0;
					int y2 = 0;
					bool bothConnected = (first == widget.mTop && widget.mBottom.mTarget != null && widget.mBottom.mTarget.hasFinalValue()) || (first == widget.mBottom && widget.mTop.mTarget != null && widget.mTop.mTarget.hasFinalValue());
					if (widget.VerticalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT || iscanMeasure)
					{
						if (widget.MeasureRequested)
						{
							// Widget needs to be measured
							if (DEBUG)
							{
								Console.WriteLine(ls(level + 1) + "(B) We didn't measure " + widget.DebugName + ", le'ts bail");
							}
							continue;
						}
						if (first == widget.mTop && widget.mBottom.mTarget == null)
						{
							y1 = b + widget.mTop.Margin;
							y2 = y1 + widget.Height;
							widget.setFinalVertical(y1, y2);
							verticalSolvingPass(level + 1, widget, measurer);
						}
						else if (first == widget.mBottom && widget.mTop.mTarget == null)
						{
							y2 = b - widget.mBottom.Margin;
							y1 = y2 - widget.Height;
							widget.setFinalVertical(y1, y2);
							verticalSolvingPass(level + 1, widget, measurer);
						}
						else if (bothConnected && !widget.InVerticalChain)
						{
							solveVerticalCenterConstraints(level + 1, measurer, widget);
						}
					}
					else if (widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && widget.mMatchConstraintMaxHeight >= 0 && widget.mMatchConstraintMinHeight >= 0 && (widget.Visibility == ConstraintWidget.GONE || ((widget.mMatchConstraintDefaultHeight == ConstraintWidget.MATCH_CONSTRAINT_SPREAD) && widget.getDimensionRatio() == 0)) && !widget.InVerticalChain && !widget.InVirtualLayout)
					{
						if (bothConnected && !widget.InVerticalChain)
						{
							solveVerticalMatchConstraint(level + 1, layout, measurer, widget);
						}
					}
				}
			}

			ConstraintAnchor baseline = layout.getAnchor(ConstraintAnchor.Type.BASELINE);
			if (baseline.Dependents != null && baseline.hasFinalValue())
			{
				int baselineValue = baseline.FinalValue;
				foreach (ConstraintAnchor first in baseline.Dependents)
				{
					ConstraintWidget widget = first.mOwner;
					bool iscanMeasure = canMeasure(level + 1, widget);
					if (widget.MeasureRequested && iscanMeasure)
					{
						BasicMeasure.Measure measure = new BasicMeasure.Measure();
						ConstraintWidgetContainer.measure(level + 1, widget, measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
					}
					if (widget.VerticalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT || iscanMeasure)
					{
						if (widget.MeasureRequested)
						{
							// Widget needs to be measured
							if (DEBUG)
							{
								Console.WriteLine(ls(level + 1) + "(B) We didn't measure " + widget.DebugName + ", le'ts bail");
							}
							continue;
						}
						if (first == widget.mBaseline)
						{
							widget.FinalBaseline = baselineValue + first.Margin;
							verticalSolvingPass(level + 1, widget, measurer);
						}
					}
				}
			}
			layout.markVerticalSolvingPassDone();
		}

		/// <summary>
		/// Solve horizontal centering constraints
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="measurer"> </param>
		/// <param name="widget"> </param>
		/// <param name="isRtl"> </param>
		private static void solveHorizontalCenterConstraints(int level, BasicMeasure.Measurer measurer, ConstraintWidget widget, bool isRtl)
		{
			int x1;
			int x2;
			float bias = widget.HorizontalBiasPercent;
			int start = widget.mLeft.mTarget.FinalValue;
			int end = widget.mRight.mTarget.FinalValue;
			int s1 = start + widget.mLeft.Margin;
			int s2 = end - widget.mRight.Margin;
			if (start == end)
			{
				bias = 0.5f;
				s1 = start;
				s2 = end;
			}
			int width = widget.Width;
			int distance = s2 - s1 - width;
			if (s1 > s2)
			{
				distance = s1 - s2 - width;
			}
			int d1;
			if (distance > 0)
			{
				d1 = (int)(0.5f + bias * distance);
			}
			else
			{
				d1 = (int)(bias * distance);
			}
			x1 = s1 + d1;
			x2 = x1 + width;
			if (s1 > s2)
			{
				x1 = s1 + d1;
				x2 = x1 - width;
			}
			widget.setFinalHorizontal(x1, x2);
			horizontalSolvingPass(level + 1, widget, measurer, isRtl);
		}

		/// <summary>
		/// Solve vertical centering constraints
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="measurer"> </param>
		/// <param name="widget"> </param>
		private static void solveVerticalCenterConstraints(int level, BasicMeasure.Measurer measurer, ConstraintWidget widget)
		{
			int y1;
			int y2;
			float bias = widget.VerticalBiasPercent;
			int start = widget.mTop.mTarget.FinalValue;
			int end = widget.mBottom.mTarget.FinalValue;
			int s1 = start + widget.mTop.Margin;
			int s2 = end - widget.mBottom.Margin;
			if (start == end)
			{
				bias = 0.5f;
				s1 = start;
				s2 = end;
			}
			int height = widget.Height;
			int distance = s2 - s1 - height;
			if (s1 > s2)
			{
				distance = s1 - s2 - height;
			}
			int d1;
			if (distance > 0)
			{
				d1 = (int)(0.5f + bias * distance);
			}
			else
			{
				d1 = (int)(bias * distance);
			}
			y1 = s1 + d1;
			y2 = y1 + height;
			if (s1 > s2)
			{
				y1 = s1 - d1;
				y2 = y1 - height;
			}
			widget.setFinalVertical(y1, y2);
			verticalSolvingPass(level + 1, widget, measurer);
		}

		/// <summary>
		/// Solve horizontal match constraints
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="measurer"> </param>
		/// <param name="widget"> </param>
		/// <param name="isRtl"> </param>
		private static void solveHorizontalMatchConstraint(int level, ConstraintWidget layout, BasicMeasure.Measurer measurer, ConstraintWidget widget, bool isRtl)
		{
			int x1;
			int x2;
			float bias = widget.HorizontalBiasPercent;
			int s1 = widget.mLeft.mTarget.FinalValue + widget.mLeft.Margin;
			int s2 = widget.mRight.mTarget.FinalValue - widget.mRight.Margin;
			if (s2 >= s1)
			{
				int width = widget.Width;
				if (widget.Visibility != ConstraintWidget.GONE)
				{
					if (widget.mMatchConstraintDefaultWidth == ConstraintWidget.MATCH_CONSTRAINT_PERCENT)
					{
						int parentWidth = 0;
						if (layout is ConstraintWidgetContainer)
						{
							parentWidth = layout.Width;
						}
						else
						{
							parentWidth = layout.Parent.Width;
						}
						width = (int)(0.5f * widget.HorizontalBiasPercent * parentWidth);
					}
					else if (widget.mMatchConstraintDefaultWidth == ConstraintWidget.MATCH_CONSTRAINT_SPREAD)
					{
						width = s2 - s1;
					}
					width = Math.Max(widget.mMatchConstraintMinWidth, width);
					if (widget.mMatchConstraintMaxWidth > 0)
					{
						width = Math.Min(widget.mMatchConstraintMaxWidth, width);
					}
				}
				int distance = s2 - s1 - width;
				int d1 = (int)(0.5f + bias * distance);
				x1 = s1 + d1;
				x2 = x1 + width;
				widget.setFinalHorizontal(x1, x2);
				horizontalSolvingPass(level + 1, widget, measurer, isRtl);
			}
		}

		/// <summary>
		/// Solve vertical match constraints
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="measurer"> </param>
		/// <param name="widget"> </param>
		private static void solveVerticalMatchConstraint(int level, ConstraintWidget layout, BasicMeasure.Measurer measurer, ConstraintWidget widget)
		{
			int y1;
			int y2;
			float bias = widget.VerticalBiasPercent;
			int s1 = widget.mTop.mTarget.FinalValue + widget.mTop.Margin;
			int s2 = widget.mBottom.mTarget.FinalValue - widget.mBottom.Margin;
			if (s2 >= s1)
			{
				int height = widget.Height;
				if (widget.Visibility != ConstraintWidget.GONE)
				{
					if (widget.mMatchConstraintDefaultHeight == ConstraintWidget.MATCH_CONSTRAINT_PERCENT)
					{
						int parentHeight = 0;
						if (layout is ConstraintWidgetContainer)
						{
							parentHeight = layout.Height;
						}
						else
						{
							parentHeight = layout.Parent.Height;
						}
						height = (int)(0.5f * bias * parentHeight);
					}
					else if (widget.mMatchConstraintDefaultHeight == ConstraintWidget.MATCH_CONSTRAINT_SPREAD)
					{
						height = s2 - s1;
					}
					height = Math.Max(widget.mMatchConstraintMinHeight, height);
					if (widget.mMatchConstraintMaxHeight > 0)
					{
						height = Math.Min(widget.mMatchConstraintMaxHeight, height);
					}
				}
				int distance = s2 - s1 - height;
				int d1 = (int)(0.5f + bias * distance);
				y1 = s1 + d1;
				y2 = y1 + height;
				widget.setFinalVertical(y1, y2);
				verticalSolvingPass(level + 1, widget, measurer);
			}
		}

		/// <summary>
		/// Returns true if the dimensions of the given widget are computable directly
		/// 
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="layout"> the widget to check </param>
		/// <returns> true if both dimensions are knowable by a single measure pass </returns>
		private static bool canMeasure(int level, ConstraintWidget layout)
		{
			ConstraintWidget.DimensionBehaviour horizontalBehaviour = layout.HorizontalDimensionBehaviour;
			ConstraintWidget.DimensionBehaviour verticalBehaviour = layout.VerticalDimensionBehaviour;
			ConstraintWidgetContainer parent = layout.Parent != null ? (ConstraintWidgetContainer) layout.Parent : null;
			bool isParentHorizontalFixed = parent != null && parent.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.FIXED;
			bool isParentVerticalFixed = parent != null && parent.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.FIXED;
			bool isHorizontalFixed = horizontalBehaviour == ConstraintWidget.DimensionBehaviour.FIXED || layout.ResolvedHorizontally || (APPLY_MATCH_PARENT && horizontalBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT && isParentHorizontalFixed) || horizontalBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT || (horizontalBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && layout.mMatchConstraintDefaultWidth == ConstraintWidget.MATCH_CONSTRAINT_SPREAD && layout.mDimensionRatio == 0 && layout.hasDanglingDimension(HORIZONTAL)) || (horizontalBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && layout.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP && layout.hasResolvedTargets(HORIZONTAL, layout.Width));
			bool isVerticalFixed = verticalBehaviour == ConstraintWidget.DimensionBehaviour.FIXED || layout.ResolvedVertically || (APPLY_MATCH_PARENT && verticalBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT && isParentVerticalFixed) || verticalBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT || (verticalBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && layout.mMatchConstraintDefaultHeight == ConstraintWidget.MATCH_CONSTRAINT_SPREAD && layout.mDimensionRatio == 0 && layout.hasDanglingDimension(ConstraintWidget.VERTICAL)) || (horizontalBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT && layout.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP && layout.hasResolvedTargets(VERTICAL, layout.Height));
			if (layout.mDimensionRatio > 0 && (isHorizontalFixed || isVerticalFixed))
			{
				return true;
			}
			if (DEBUG)
			{
				Console.WriteLine(ls(level) + "can measure " + layout.DebugName + " ? " + (isHorizontalFixed && isVerticalFixed) + "  [ " + isHorizontalFixed + " (horiz " + horizontalBehaviour + ") & " + isVerticalFixed + " (vert " + verticalBehaviour + ") ]");
			}
			return isHorizontalFixed && isVerticalFixed;
		}

		/// <summary>
		/// Try to directly resolve the chain
		/// </summary>
		/// <param name="container"> </param>
		/// <param name="system"> </param>
		/// <param name="orientation"> </param>
		/// <param name="offset"> </param>
		/// <param name="chainHead"> </param>
		/// <param name="isChainSpread"> </param>
		/// <param name="isChainSpreadInside"> </param>
		/// <param name="isChainPacked"> </param>
		/// <returns> true if fully resolved </returns>
		public static bool solveChain(ConstraintWidgetContainer container, LinearSystem system, int orientation, int offset, ChainHead chainHead, bool isChainSpread, bool isChainSpreadInside, bool isChainPacked)
		{
			if (LinearSystem.FULL_DEBUG)
			{
				Console.WriteLine("\n### SOLVE CHAIN ###");
			}
			if (isChainPacked)
			{
				return false;
			}
			if (orientation == HORIZONTAL)
			{
				if (!container.ResolvedHorizontally)
				{
					return false;
				}
			}
			else
			{
				if (!container.ResolvedVertically)
				{
					return false;
				}
			}
			int level = 0; // nested level (used for debugging)
			bool isRtl = container.Rtl;

			ConstraintWidget first = chainHead.First;
			ConstraintWidget last = chainHead.Last;
			ConstraintWidget firstVisibleWidget = chainHead.FirstVisibleWidget;
			ConstraintWidget lastVisibleWidget = chainHead.LastVisibleWidget;
			ConstraintWidget head = chainHead.Head;

			ConstraintWidget widget = first;
			ConstraintWidget next;
			bool done = false;

			ConstraintAnchor begin = first.mListAnchors[offset];
			ConstraintAnchor end = last.mListAnchors[offset + 1];
			if (begin.mTarget == null || end.mTarget == null)
			{
				return false;
			}
			if (!begin.mTarget.hasFinalValue() || !end.mTarget.hasFinalValue())
			{
				return false;
			}

			if (firstVisibleWidget == null || lastVisibleWidget == null)
			{
				return false;
			}

			int startPoint = begin.mTarget.FinalValue + firstVisibleWidget.mListAnchors[offset].Margin;
			int endPoint = end.mTarget.FinalValue - lastVisibleWidget.mListAnchors[offset + 1].Margin;

			int distance = endPoint - startPoint;
			if (distance <= 0)
			{
				return false;
			}
			int totalSize = 0;
			BasicMeasure.Measure measure = new BasicMeasure.Measure();

			int numWidgets = 0;
			int numVisibleWidgets = 0;

			while (!done)
			{
				bool iscanMeasure = canMeasure(level + 1, widget);
				if (!iscanMeasure)
				{
					return false;
				}
				if (widget.mListDimensionBehaviors[orientation] == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
				{
					return false;
				}

				if (widget.MeasureRequested)
				{
					ConstraintWidgetContainer.measure(level + 1, widget, container.Measurer, measure, BasicMeasure.Measure.SELF_DIMENSIONS);
				}

				totalSize += widget.mListAnchors[offset].Margin;
				if (orientation == HORIZONTAL)
				{
					totalSize += + widget.Width;
				}
				else
				{
					totalSize += widget.Height;
				}
				totalSize += widget.mListAnchors[offset + 1].Margin;

				numWidgets++;
				if (widget.Visibility != ConstraintWidget.GONE)
				{
					numVisibleWidgets++;
				}


				// go to the next widget
				ConstraintAnchor nextAnchor = widget.mListAnchors[offset + 1].mTarget;
				if (nextAnchor != null)
				{
					next = nextAnchor.mOwner;
					if (next.mListAnchors[offset].mTarget == null || next.mListAnchors[offset].mTarget.mOwner != widget)
					{
						next = null;
					}
				}
				else
				{
					next = null;
				}
				if (next != null)
				{
					widget = next;
				}
				else
				{
					done = true;
				}
			}

			if (numVisibleWidgets == 0)
			{
				return false;
			}

			if (numVisibleWidgets != numWidgets)
			{
				return false;
			}

			if (distance < totalSize)
			{
				return false;
			}

			int gap = distance - totalSize;
			if (isChainSpread)
			{
				gap = gap / (numVisibleWidgets + 1);
			}
			else if (isChainSpreadInside)
			{
				if (numVisibleWidgets > 2)
				{
					gap = gap / numVisibleWidgets - 1;
				}
			}

			if (numVisibleWidgets == 1)
			{
				float bias;
				if (orientation == ConstraintWidget.HORIZONTAL)
				{
					bias = head.HorizontalBiasPercent;
				}
				else
				{
					bias = head.VerticalBiasPercent;
				}
				int p1 = (int)(0.5f + startPoint + gap * bias);
				if (orientation == HORIZONTAL)
				{
					firstVisibleWidget.setFinalHorizontal(p1, p1 + firstVisibleWidget.Width);
				}
				else
				{
					firstVisibleWidget.setFinalVertical(p1, p1 + firstVisibleWidget.Height);
				}
				Direct.horizontalSolvingPass(level + 1, firstVisibleWidget, container.Measurer, isRtl);
				return true;
			}

			if (isChainSpread)
			{
				done = false;

				int current = startPoint + gap;
				widget = first;
				while (!done)
				{
					if (widget.Visibility == GONE)
					{
						if (orientation == HORIZONTAL)
						{
							widget.setFinalHorizontal(current, current);
							Direct.horizontalSolvingPass(level + 1, widget, container.Measurer, isRtl);
						}
						else
						{
							widget.setFinalVertical(current, current);
							Direct.verticalSolvingPass(level + 1, widget, container.Measurer);
						}
					}
					else
					{
						current += widget.mListAnchors[offset].Margin;
						if (orientation == HORIZONTAL)
						{
							widget.setFinalHorizontal(current, current + widget.Width);
							Direct.horizontalSolvingPass(level + 1, widget, container.Measurer, isRtl);
							current += widget.Width;
						}
						else
						{
							widget.setFinalVertical(current, current + widget.Height);
							Direct.verticalSolvingPass(level + 1, widget, container.Measurer);
							current += widget.Height;
						}
						current += widget.mListAnchors[offset + 1].Margin;
						current += gap;
					}

					widget.addToSolver(system, false);

					// go to the next widget
					ConstraintAnchor nextAnchor = widget.mListAnchors[offset + 1].mTarget;
					if (nextAnchor != null)
					{
						next = nextAnchor.mOwner;
						if (next.mListAnchors[offset].mTarget == null || next.mListAnchors[offset].mTarget.mOwner != widget)
						{
							next = null;
						}
					}
					else
					{
						next = null;
					}
					if (next != null)
					{
						widget = next;
					}
					else
					{
						done = true;
					}
				}
			}
			else if (isChainSpreadInside)
			{
				if (numVisibleWidgets == 2)
				{
					if (orientation == HORIZONTAL)
					{
						firstVisibleWidget.setFinalHorizontal(startPoint, startPoint + firstVisibleWidget.Width);
						lastVisibleWidget.setFinalHorizontal(endPoint - lastVisibleWidget.Width, endPoint);
						Direct.horizontalSolvingPass(level + 1, firstVisibleWidget, container.Measurer, isRtl);
						Direct.horizontalSolvingPass(level + 1, lastVisibleWidget, container.Measurer, isRtl);
					}
					else
					{
						firstVisibleWidget.setFinalVertical(startPoint, startPoint + firstVisibleWidget.Height);
						lastVisibleWidget.setFinalVertical(endPoint - lastVisibleWidget.Height, endPoint);
						Direct.verticalSolvingPass(level + 1, firstVisibleWidget, container.Measurer);
						Direct.verticalSolvingPass(level + 1, lastVisibleWidget, container.Measurer);
					}
					return true;
				}
				return false;
			}
			return true;
		}
	}

}