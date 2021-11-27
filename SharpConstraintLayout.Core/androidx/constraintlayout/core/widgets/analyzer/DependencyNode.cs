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

	public class DependencyNode : Dependency
	{
		public Dependency updateDelegate = null;
		public bool delegateToWidgetRun = false;
		public bool readyToSolve = false;

		internal enum Type
		{
			UNKNOWN,
			HORIZONTAL_DIMENSION,
			VERTICAL_DIMENSION,
			LEFT,
			RIGHT,
			TOP,
			BOTTOM,
			BASELINE
		}

		internal WidgetRun run;
		internal Type type = Type.UNKNOWN;
		internal int margin;
		public int value;
		internal int marginFactor = 1;
		internal DimensionDependency marginDependency = null;
		public bool resolved = false;

		public DependencyNode(WidgetRun run)
		{
			this.run = run;
		}
		internal IList<Dependency> dependencies = new List<Dependency>();
		internal IList<DependencyNode> targets = new List<DependencyNode>();

		public override string ToString()
		{
			return run.widget.DebugName + ":" + type + "(" + (resolved? value.ToString() : "unresolved") + ") <t=" + targets.Count + ":d=" + dependencies.Count + ">";
		}

		public virtual void resolve(int value)
		{
			if (resolved)
			{
				return;
			}

			this.resolved = true;
			this.value = value;
			foreach (Dependency node in dependencies)
			{
				node.update(node);
			}
		}

		public virtual void update(Dependency node)
		{
			foreach (DependencyNode targetItem in targets)
			{
				if (!targetItem.resolved)
				{
					return;
				}
			}
			readyToSolve = true;
			if (updateDelegate != null)
			{
				updateDelegate.update(this);
			}
			if (delegateToWidgetRun)
			{
				run.update(this);
				return;
			}
			DependencyNode target = null;
			int numTargets = 0;
			foreach (DependencyNode t in targets)
			{
				if (t is DimensionDependency)
				{
					continue;
				}
				target = t;
				numTargets++;
			}
			if (target != null && numTargets == 1 && target.resolved)
			{
				if (marginDependency != null)
				{
					if (marginDependency.resolved)
					{
						margin = marginFactor * marginDependency.value;
					}
					else
					{
						return;
					}
				}
				resolve(target.value + margin);
			}
			if (updateDelegate != null)
			{
				updateDelegate.update(this);
			}
		}

		public virtual void addDependency(Dependency dependency)
		{
			dependencies.Add(dependency);
			if (resolved)
			{
				dependency.update(dependency);
			}
		}

		public virtual string name()
		{
			string definition = run.widget.DebugName;
			if (type == Type.LEFT || type == Type.RIGHT)
			{
				definition += "_HORIZONTAL";
			}
			else
			{
				definition += "_VERTICAL";
			}
			definition += ":" + type.ToString();
			return definition;
		}

		public virtual void clear()
		{
			targets.Clear();
			dependencies.Clear();
			resolved = false;
			value = 0;
			readyToSolve = false;
			delegateToWidgetRun = false;
		}
	}

}