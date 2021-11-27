using System;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.AT_MOST;
using static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.EXACTLY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.UNSPECIFIED;

	/// <summary>
	/// Simple VirtualLayout that center the first referenced widget onto itself.
	/// </summary>
	public class Placeholder : VirtualLayout
	{

		public override void measure(int widthMode, int widthSize, int heightMode, int heightSize)
		{
			int width = 0;
			int height = 0;
			int paddingLeft = PaddingLeft;
			int paddingRight = PaddingRight;
			int paddingTop = PaddingTop;
			int paddingBottom = PaddingBottom;

			width += paddingLeft + paddingRight;
			height += paddingTop + paddingBottom;

			if (mWidgetsCount > 0)
			{
				// grab the first referenced widget size in case we are ourselves in wrap_content
				width += mWidgets[0].Width;
				height += mWidgets[0].Height;
			}
			width = Math.Max(MinWidth, width);
			height = Math.Max(MinHeight, height);

			int measuredWidth = 0;
			int measuredHeight = 0;

			if (widthMode == EXACTLY)
			{
				measuredWidth = widthSize;
			}
			else if (widthMode == AT_MOST)
			{
				measuredWidth = Math.Min(width, widthSize);
			}
			else if (widthMode == UNSPECIFIED)
			{
				measuredWidth = width;
			}

			if (heightMode == EXACTLY)
			{
				measuredHeight = heightSize;
			}
			else if (heightMode == AT_MOST)
			{
				measuredHeight = Math.Min(height, heightSize);
			}
			else if (heightMode == UNSPECIFIED)
			{
				measuredHeight = height;
			}

			setMeasure(measuredWidth, measuredHeight);
			Width = measuredWidth;
			Height = measuredHeight;
			needsCallbackFromSolver(mWidgetsCount > 0);
		}

		public override void addToSolver(LinearSystem system, bool optimize)
		{
			base.addToSolver(system, optimize);

			if (mWidgetsCount > 0)
			{
				ConstraintWidget widget = mWidgets[0];
				widget.resetAllConstraints();
				widget.connect(ConstraintAnchor.Type.LEFT, this, ConstraintAnchor.Type.LEFT);
				widget.connect(ConstraintAnchor.Type.RIGHT, this, ConstraintAnchor.Type.RIGHT);
				widget.connect(ConstraintAnchor.Type.TOP, this, ConstraintAnchor.Type.TOP);
				widget.connect(ConstraintAnchor.Type.BOTTOM, this, ConstraintAnchor.Type.BOTTOM);
			}
		}
	}

}