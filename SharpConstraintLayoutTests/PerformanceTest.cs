using NUnit.Framework;
using System;
using System.Diagnostics;

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

namespace androidx.constraintlayout.core
{
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;


    /// <summary>
    /// Basic performance test
    /// </summary>
    [TestFixture]
    public class PerformanceTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testBasic()
        [Test]
        public virtual void BasicTest()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget A = new ConstraintWidget(100, 40);
            ConstraintWidget B = new ConstraintWidget(100, 100);
            ConstraintWidget C = new ConstraintWidget(100, 20);
            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";
            root.add(A);
            root.add(B);
            root.add(C);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;

            root.layout();
            Console.WriteLine("root: " + root + "\n A: " + A + "\n B: " + B + "\n C: " + C);
        }

        [Test]
        public virtual void PressureTest()
        {
            int viewCount = 30;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
            ConstraintWidget preview = new ConstraintWidget(100, 40);
            root.add(preview);
            preview.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            preview.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            preview.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            preview.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            for (var i = 0; i < viewCount; i++)
            {
                var view =  new ConstraintWidget(100, 40);
                root.add(view);
                view.connect(ConstraintAnchor.Type.LEFT, preview, ConstraintAnchor.Type.LEFT,1);
                view.connect(ConstraintAnchor.Type.RIGHT, preview, ConstraintAnchor.Type.RIGHT,-1);
                view.connect(ConstraintAnchor.Type.TOP, preview, ConstraintAnchor.Type.TOP,-1);
                view.connect(ConstraintAnchor.Type.BOTTOM, preview, ConstraintAnchor.Type.BOTTOM);
                view.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                view.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                preview = view;
            }

            root.layout();
            sw.Stop();
            Debug.WriteLine("耗时:" + sw.ElapsedMilliseconds);
        }
    }
}