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
	internal class DimensionDependency : DependencyNode
	{

		public int wrapValue;

		public DimensionDependency(WidgetRun run) : base(run)
		{
			if (run is HorizontalWidgetRun)
			{
				type = Type.HORIZONTAL_DIMENSION;
			}
			else
			{
				type = Type.VERTICAL_DIMENSION;
			}
		}

		public override void resolve(int value)
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

	}

}