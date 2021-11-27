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

namespace androidx.constraintlayout.core.widgets
{

	/// <summary>
	/// A container of ConstraintWidget
	/// </summary>
	public class WidgetContainer : ConstraintWidget
	{
		public List<ConstraintWidget> mChildren = new List<ConstraintWidget>();

		/*-----------------------------------------------------------------------*/
		// Construction
		/*-----------------------------------------------------------------------*/

		/// <summary>
		/// Default constructor
		/// </summary>
		public WidgetContainer()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="x">      x position </param>
		/// <param name="y">      y position </param>
		/// <param name="width">  width of the layout </param>
		/// <param name="height"> height of the layout </param>
		public WidgetContainer(int x, int y, int width, int height) : base(x, y, width, height)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="width">  width of the layout </param>
		/// <param name="height"> height of the layout </param>
		public WidgetContainer(int width, int height) : base(width, height)
		{
		}

		public override void reset()
		{
			mChildren.Clear();
			base.reset();
		}

		/// <summary>
		/// Add a child widget
		/// </summary>
		/// <param name="widget"> to add </param>
		public virtual void add(ConstraintWidget widget)
		{
			mChildren.Add(widget);
			if (widget.Parent != null)
			{
				WidgetContainer container = (WidgetContainer)widget.Parent;
				container.remove(widget);
			}
			widget.Parent = this;
		}

		/// <summary>
		/// Add multiple child widgets.
		/// </summary>
		/// <param name="widgets"> to add </param>
		public virtual void add(params ConstraintWidget[] widgets)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = widgets.length;
			int count = widgets.Length;
			for (int i = 0; i < count; i++)
			{
				add(widgets[i]);
			}
		}

		/// <summary>
		/// Remove a child widget
		/// </summary>
		/// <param name="widget"> to remove </param>
		public virtual void remove(ConstraintWidget widget)
		{
			mChildren.Remove(widget);
			widget.reset();
		}

		/// <summary>
		/// Access the children
		/// </summary>
		/// <returns> the array of children </returns>
		public virtual List<ConstraintWidget> Children
		{
			get
			{
				return mChildren;
			}
		}

		/// <summary>
		/// Return the top-level ConstraintWidgetContainer
		/// </summary>
		/// <returns> top-level ConstraintWidgetContainer </returns>
		public virtual ConstraintWidgetContainer RootConstraintContainer
		{
			get
			{
				ConstraintWidget item = this;
				ConstraintWidget parent = item.Parent;
				ConstraintWidgetContainer container = null;
				if (item is ConstraintWidgetContainer)
				{
					container = (ConstraintWidgetContainer)this;
				}
				while (parent != null)
				{
					item = parent;
					parent = item.Parent;
					if (item is ConstraintWidgetContainer)
					{
						container = (ConstraintWidgetContainer)item;
					}
				}
				return container;
			}
		}

		/*-----------------------------------------------------------------------*/
		// Overloaded methods from ConstraintWidget
		/*-----------------------------------------------------------------------*/

		/// <summary>
		/// Set the offset of this widget relative to the root widget.
		/// We then set the offset of our children as well.
		/// </summary>
		/// <param name="x"> horizontal offset </param>
		/// <param name="y"> vertical offset </param>
		public override void setOffset(int x, int y)
		{
			base.setOffset(x, y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildren.size();
			int count = mChildren.Count;
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget widget = mChildren[i];
				widget.setOffset(RootX, RootY);
			}
		}

		/// <summary>
		/// Function implemented by ConstraintWidgetContainer
		/// </summary>
		public virtual void layout()
		{
			if (mChildren == null)
			{
				return;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildren.size();
			int count = mChildren.Count;
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget widget = mChildren[i];
				if (widget is WidgetContainer)
				{
					((WidgetContainer)widget).layout();
				}
			}
		}

		public override void resetSolverVariables(Cache cache)
		{
			base.resetSolverVariables(cache);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildren.size();
			int count = mChildren.Count;
			for (int i = 0; i < count; i++)
			{
				ConstraintWidget widget = mChildren[i];
				widget.resetSolverVariables(cache);
			}
		}

		public virtual void removeAllChildren()
		{
			mChildren.Clear();
		}
	}
}