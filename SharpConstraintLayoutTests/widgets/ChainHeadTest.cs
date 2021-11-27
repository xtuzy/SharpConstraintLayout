/*
 * Copyright (C) 2018 The Android Open Source Project
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
    using NUnit.Framework;
    using DimensionBehaviour = androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.AreEqual;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.assertTrue;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.assertFalse;

    [TestFixture]
    public class ChainHeadTest
    {

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void basicHorizontalChainHeadTest()
        [Test]
        public virtual void basicHorizontalChainHeadTest()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
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
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            ChainHead chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.Head, A);
            Assert.AreEqual(chainHead.First, A);
            Assert.AreEqual(chainHead.FirstVisibleWidget, A);
            Assert.AreEqual(chainHead.Last, C);
            Assert.AreEqual(chainHead.LastVisibleWidget, C);

            A.Visibility = ConstraintWidget.GONE;

            chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.Head, A);
            Assert.AreEqual(chainHead.First, A);
            Assert.AreEqual(chainHead.FirstVisibleWidget, B);


            chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, true);
            chainHead.define();

            Assert.AreEqual(chainHead.Head, C);
            Assert.AreEqual(chainHead.First, A);
            Assert.AreEqual(chainHead.FirstVisibleWidget, B);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void basicVerticalChainHeadTest()
        [Test]
        public virtual void basicVerticalChainHeadTest()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(100, 20);

            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";

            root.add(A);
            root.add(B);
            root.add(C);

            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.connect(ConstraintAnchor.Type.BOTTOM, B, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, A, ConstraintAnchor.Type.BOTTOM);
            B.connect(ConstraintAnchor.Type.BOTTOM, C, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, B, ConstraintAnchor.Type.BOTTOM);
            C.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            B.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);

            ChainHead chainHead = new ChainHead(A, ConstraintWidget.VERTICAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.Head, A);
            Assert.AreEqual(chainHead.First, A);
            Assert.AreEqual(chainHead.FirstVisibleWidget, A);
            Assert.AreEqual(chainHead.Last, C);
            Assert.AreEqual(chainHead.LastVisibleWidget, C);

            A.Visibility = ConstraintWidget.GONE;

            chainHead = new ChainHead(A, ConstraintWidget.VERTICAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.Head, A);
            Assert.AreEqual(chainHead.First, A);
            Assert.AreEqual(chainHead.FirstVisibleWidget, B);


            chainHead = new ChainHead(A, ConstraintWidget.VERTICAL, true);
            chainHead.define();

            Assert.AreEqual(chainHead.Head, A);
            Assert.AreEqual(chainHead.First, A);
            Assert.AreEqual(chainHead.FirstVisibleWidget, B);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void basicMatchConstraintTest()
        [Test]
        public virtual void basicMatchConstraintTest()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(100, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
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
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            A.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            B.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            C.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
            A.HorizontalWeight = 1f;
            B.HorizontalWeight = 2f;
            C.HorizontalWeight = 3f;

            ChainHead chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.FirstMatchConstraintWidget, A);
            Assert.AreEqual(chainHead.LastMatchConstraintWidget, C);
            Assert.AreEqual(chainHead.TotalWeight, 6f, 0f);

            C.Visibility = ConstraintWidget.GONE;

            chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.FirstMatchConstraintWidget, A);
            Assert.AreEqual(chainHead.LastMatchConstraintWidget, B);
            Assert.AreEqual(chainHead.TotalWeight, 3f, 0f);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void chainOptimizerValuesTest()
        [Test]
        public virtual void chainOptimizerValuesTest()
        {
            ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 600, 600);
            ConstraintWidget A = new ConstraintWidget(50, 20);
            ConstraintWidget B = new ConstraintWidget(100, 20);
            ConstraintWidget C = new ConstraintWidget(200, 20);

            root.DebugName = "root";
            A.DebugName = "A";
            B.DebugName = "B";
            C.DebugName = "C";

            root.add(A, B, C);

            A.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT, 5);
            A.connect(ConstraintAnchor.Type.RIGHT, B, ConstraintAnchor.Type.LEFT, 5);
            B.connect(ConstraintAnchor.Type.LEFT, A, ConstraintAnchor.Type.RIGHT, 1);
            B.connect(ConstraintAnchor.Type.RIGHT, C, ConstraintAnchor.Type.LEFT, 1);
            C.connect(ConstraintAnchor.Type.LEFT, B, ConstraintAnchor.Type.RIGHT, 10);
            C.connect(ConstraintAnchor.Type.RIGHT, root, ConstraintAnchor.Type.RIGHT, 10);
            A.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            B.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);
            C.connect(ConstraintAnchor.Type.TOP, root, ConstraintAnchor.Type.TOP);

            ChainHead chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, false);
            chainHead.define();

            //Assert.AreEqual(chainHead.mVisibleWidgets, 3);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mVisibleWidgets"), 3);
            //Assert.AreEqual(chainHead.mTotalSize, 367); // Takes all but first and last margins.
            Assert.AreEqual(chainHead.GetFieldValue<int>("mTotalSize"), 367); // Takes all but first and last margins.
                                                                                 //Assert.AreEqual(chainHead.mTotalMargins, 32);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mTotalMargins"), 32);
            //Assert.AreEqual(chainHead.mWidgetsMatchCount, 0);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mWidgetsMatchCount"), 0);
            //Assert.IsTrue(chainHead.mOptimizable);
            Assert.IsTrue(chainHead.GetFieldValue<bool>("mOptimizable"));

            B.Visibility = ConstraintWidget.GONE;

            chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.GetFieldValue<int>("mVisibleWidgets"), 2);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mTotalSize"), 265);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mTotalMargins"), 30);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mWidgetsMatchCount"), 0);
            Assert.IsTrue(chainHead.GetFieldValue<bool>("mOptimizable"));

            A.Visibility = ConstraintWidget.GONE;
            B.Visibility = ConstraintWidget.VISIBLE;
            C.Visibility = ConstraintWidget.GONE;

            chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.GetFieldValue<int>("mVisibleWidgets"), 1);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mTotalSize"), 100);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mTotalMargins"), 2);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mWidgetsMatchCount"), 0);
            Assert.IsTrue(chainHead.GetFieldValue<bool>("mOptimizable"));

            A.Visibility = ConstraintWidget.VISIBLE;
            B.Visibility = ConstraintWidget.VISIBLE;
            C.Visibility = ConstraintWidget.VISIBLE;
            A.HorizontalDimensionBehaviour = DimensionBehaviour.MATCH_CONSTRAINT;
            A.mMatchConstraintDefaultWidth = ConstraintWidget.MATCH_CONSTRAINT_PERCENT;

            chainHead = new ChainHead(A, ConstraintWidget.HORIZONTAL, false);
            chainHead.define();

            Assert.AreEqual(chainHead.GetFieldValue<int>("mVisibleWidgets"), 3);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mTotalSize"), 317);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mTotalMargins"), 32);
            Assert.AreEqual(chainHead.GetFieldValue<int>("mWidgetsMatchCount"), 1);
            Assert.False(chainHead.GetFieldValue<bool>("mOptimizable"));
        }

    }

}