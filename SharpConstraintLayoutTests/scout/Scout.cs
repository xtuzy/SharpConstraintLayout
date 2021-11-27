using System.Collections.Generic;

/*
 * Copyright (C) 2016 The Android Open Source Project
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
namespace androidx.constraintlayout.core.scout
{
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
	using WidgetContainer = androidx.constraintlayout.core.widgets.WidgetContainer;

	/// <summary>
	/// Main entry for the Scout Inference engine.
	/// All external access should be through this class
	/// TODO support Stash / merge constraints table etc.
	/// </summary>
	public class Scout
	{


		/// <summary>
		/// Given a collection of widgets evaluates probability of a connection
		/// and makes connections
		/// </summary>
		/// <param name="list"> collection of widgets to connect </param>

		/// <summary>
		/// Recursive decent of widget tree inferring constraints on ConstraintWidgetContainer
		/// </summary>
		/// <param name="base"> </param>
		public static void inferConstraints(WidgetContainer @base)
		{
			if (@base == null)
			{
				return;
			}
			if (@base is ConstraintWidgetContainer && ((ConstraintWidgetContainer) @base).handlesInternalConstraints())
			{
				return;
			}
			int preX = @base.X;
			int preY = @base.Y;
			@base.X = 0;
			@base.Y = 0;
			foreach (ConstraintWidget constraintWidget in @base.Children)
			{
				if (constraintWidget is ConstraintWidgetContainer)
				{
					ConstraintWidgetContainer container = (ConstraintWidgetContainer)constraintWidget;
					if (container.Children.Count > 0)
					{
						inferConstraints(container);
					}
				}
			}

			List<ConstraintWidget> list = new List<ConstraintWidget>(@base.Children);
			list.Insert(0, @base);

			ConstraintWidget[] widgets = list.ToArray();
			ScoutWidget.computeConstraints(ScoutWidget.create(widgets));
			@base.X = preX;
			@base.Y = preY;
		}

	}

}