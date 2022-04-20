using System.Collections.Generic;

/*
 * Copyright (C) 2021 The Android Open Source Project
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

namespace androidx.constraintlayout.core.widgets
{
	using Grouping = androidx.constraintlayout.core.widgets.analyzer.Grouping;
	using WidgetGroup = androidx.constraintlayout.core.widgets.analyzer.WidgetGroup;


	/// <summary>
	/// HelperWidget class
	/// </summary>
	public class HelperWidget : ConstraintWidget, Helper
	{
		public ConstraintWidget[] mWidgets = new ConstraintWidget[4];
		public int mWidgetsCount = 0;

		public virtual void updateConstraints(ConstraintWidgetContainer container)
		{
			// nothing here
		}

		/// <summary>
		/// Add a widget to the helper
		/// </summary>
		/// <param name="widget"> a widget </param>
		public virtual void add(ConstraintWidget widget)
		{
			if (widget == this || widget == null)
			{
				return;
			}
			if (mWidgetsCount + 1 > mWidgets.Length)
			{
				mWidgets = mWidgets.Copy<ConstraintWidget>(mWidgets.Length * 2);
			}
			mWidgets[mWidgetsCount] = widget;
			mWidgetsCount++;
		}

		public override void copy(ConstraintWidget src, Dictionary<ConstraintWidget, ConstraintWidget> map)
		{
			base.copy(src, map);
			HelperWidget srcHelper = (HelperWidget) src;
			mWidgetsCount = 0;
//ORIGINAL LINE: final int count = srcHelper.mWidgetsCount;
			int count = srcHelper.mWidgetsCount;
			for (int i = 0; i < count; i++)
			{
				add(map[srcHelper.mWidgets[i]]);
			}
		}

		/// <summary>
		/// Reset the widgets list contained by this helper
		/// </summary>
		public virtual void removeAllIds()
		{
			mWidgetsCount = 0;
			//Arrays.fill(mWidgets, null);
			mWidgets.Fill(null);
		}

		public virtual void addDependents(List<WidgetGroup> dependencyLists, int orientation, WidgetGroup group)
		{
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				group.add(widget);
			}
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				Grouping.findDependents(widget, orientation, dependencyLists, group);
			}
		}

		public virtual int findGroupInDependents(int orientation)
		{
			for (int i = 0; i < mWidgetsCount; i++)
			{
				ConstraintWidget widget = mWidgets[i];
				if (orientation == HORIZONTAL && widget.horizontalGroup != -1)
				{
					return widget.horizontalGroup;
				}
				if (orientation == VERTICAL && widget.verticalGroup != -1)
				{
					return widget.verticalGroup;
				}
			}
			return -1;
		}
	}

}