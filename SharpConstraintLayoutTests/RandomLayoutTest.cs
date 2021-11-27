using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;

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
namespace androidx.constraintlayout.core
{
    using Scout = androidx.constraintlayout.core.scout.Scout;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Guideline = androidx.constraintlayout.core.widgets.Guideline;
    //using Test = org.junit.Test;


    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.assertTrue;

    /// <summary>
    /// This test creates a random set of non overlapping rectangles uses the scout
    /// to add a sequence of constraints. Verify that the constraint engine will then layout the
    /// rectangles to within 12 pixels.
    /// It uses
    /// </summary>
    /// 
    [TestFixture]
    public class RandomLayoutTest
    {
        private const int ALLOWED_POSITION_ERROR = 12;

        public const int MIN_WIDTH = 100;
        public const int MIN_HEIGHT = 40;
        public const int MIN_GAP = 40;
        public const int MAX_TRIES = 100;
        public const int LAYOUT_WIDTH = 1024;
        public const int LAYOUT_HEIGHT = 512;
        public const int MAX_WIDGETS = 20;
        public const int PERCENT_BIG_WIDGETS = 70;
        public const int LOOP_FOR = 1000;

        /// <summary>
        /// Create a collection of rectangles
        /// </summary>
        /// <param name="count">     the number of rectangles to try and generate </param>
        /// <param name="sizeRatio"> 0 = all small ones, 100 = all big ones </param>
        /// <param name="width">     the width of the bounding rectangle </param>
        /// <param name="height">    the height of the bounding rectangle
        /// @return </param>
        internal static List<Rectangle> random(long seed, int count, int sizeRatio, int width, int height)
        {
            List<Rectangle> recs = new List<Rectangle>();
            int minWidth = MIN_WIDTH;
            int minHeight = MIN_HEIGHT;
            int minGap = MIN_GAP;
            int gapBy2 = MIN_GAP * 2;

            Random rand = new Random((int)seed);
            Rectangle test = new Rectangle();
            for (int i = 0; i < count; i++)
            {

                Rectangle rn = new Rectangle();
                bool found = false;

                int attempt = 0;
                while (!found)
                {
                    if (rand.Next(100) < sizeRatio)
                    {
                        rn.X = rand.Next(width - minWidth - gapBy2) + minGap;
                        rn.Y = rand.Next(height - minHeight - gapBy2) + minGap;
                        rn.Width = minWidth + rand.Next(width - rn.X - minWidth - minGap);
                        rn.Height = minHeight + rand.Next(height - rn.Y - minHeight - minGap);
                    }
                    else
                    {
                        rn.X = rand.Next(width - minWidth - gapBy2) + minGap;
                        rn.Y = rand.Next(height - minHeight - gapBy2) + minGap;
                        rn.Width = minWidth;
                        rn.Height = minHeight;
                    }
                    test.X = rn.X - minGap;
                    test.Y = rn.Y - minGap;
                    test.Width = rn.Width + gapBy2;
                    test.Height = rn.Height + gapBy2;

                    found = true;
                    int size = recs.Count;
                    for (int j = 0; j < size; j++)
                    {
                        //if (recs[j].intersects(test))
                        if (recs[j].IntersectsWith(test))
                        {
                            found = false;
                            break;
                        }
                    }
                    attempt++;
                    if (attempt > MAX_TRIES)
                    {
                        break;
                    }

                }
                if (found)
                {
                    recs.Add(rn);
                }
            }
            return recs;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testRandomLayouts()
        [Test]
        public virtual void testRandomLayouts()
        {
            Random r = new Random(4567890);
            for (int test = 0; test < LOOP_FOR; test++)
            {
                //long seed = r.nextLong();
                long seed = r.Next();
                Console.WriteLine("seed = " + seed);
                List<Rectangle> list = random(seed, MAX_WIDGETS, PERCENT_BIG_WIDGETS, LAYOUT_WIDTH, LAYOUT_HEIGHT);

                ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, LAYOUT_WIDTH, LAYOUT_HEIGHT);

                root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                root.Width = LAYOUT_WIDTH;
                root.Height = LAYOUT_HEIGHT;
                int k = 0;
                foreach (Rectangle rec in list)
                {
                    ConstraintWidget widget = new ConstraintWidget();
                    widget.Type = "TextView";
                    string text = ("TextView" + k++);
                    widget.DebugName = text;
                    widget.setOrigin(rec.X, rec.Y);

                    widget.Width = widget.MinWidth;
                    widget.Height = widget.MinHeight;
                    widget.Width = widget.Width;
                    widget.Height = widget.Height;
                    widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

                    root.add(widget);
                    widget.X = rec.X;
                    widget.Y = rec.Y;
                    if (widget.MinWidth < rec.Width)
                    {
                        widget.MinWidth = rec.Width;
                    }
                    if (widget.MinHeight < rec.Height)
                    {
                        widget.MinHeight = rec.Height;
                    }

                    widget.setDimension(rec.Width, rec.Height);
                    //                widget.setWrapHeight(rec.Height);
                    //                widget.setWrapHeight(rec.Width);
                }

                List<ConstraintWidget> widgetList = root.Children;

                Scout.inferConstraints(root);
                foreach (ConstraintWidget widget in widgetList)
                {
                    widget.setDimension(10, 10);
                    widget.setOrigin(10, 10);
                }
                bool allOk = true;
                root.layout();
                string layout = "\n";
                bool ok = true;

                for (int i = 0; i < widgetList.Count; i++)
                {
                    ConstraintWidget widget = widgetList[i];
                    Rectangle rect = list[i];
                    allOk &= ok = isSame(dim(widget), dim(rect));
                    layout += rightPad(dim(widget), 15) + ((ok) ? " == " : " != ") + dim(rect) + "\n";
                }
                //Assert.True(layout, allOk);
                Assert.True(allOk, layout, null);
            }
        }

        /// <summary>
        /// Compare two string containing comer separated integers
        /// </summary>
        /// <param name="a"> </param>
        /// <param name="b">
        /// @return </param>
        private bool isSame(string a, string b)
        {
            if (string.ReferenceEquals(a, null) || string.ReferenceEquals(b, null))
            {
                return false;
            }
            //string[] a_split = a.Split(",", true);
            string[] a_split = a.Split(",");
            //string[] b_split = b.Split(",", true);
            string[] b_split = b.Split(",");
            if (a_split.Length != b_split.Length)
            {
                return false;
            }
            for (int i = 0; i < a_split.Length; i++)
            {
                if (a_split[i].Length == 0)
                {
                    return false;
                }
                int error = ALLOWED_POSITION_ERROR;
                if (b_split[i].StartsWith("+", StringComparison.Ordinal))
                {
                    error += 10;
                }
                int a_value = int.Parse(a_split[i]);
                int b_value = int.Parse(b_split[i]);
                if (Math.Abs(a_value - b_value) > error)
                {
                    return false;
                }
            }
            return true;
        }

        private static string rightPad(string s, int n)
        {
            s = s + StringHelperClass.NewString(new sbyte[n]).Replace('\0', ' ');
            return s.Substring(0, n);
        }

        internal virtual string dim(Rectangle r)
        {
            return r.X + "," + r.Y + "," + r.Width + "," + r.Height;
        }

        internal virtual string dim(ConstraintWidget w)
        {
            if (w is Guideline)
            {
                return w.Left + "," + w.Top + "," + 0 + "," + 0;
            }
            if (w.Visibility == ConstraintWidget.GONE)
            {
                return 0 + "," + 0 + "," + 0 + "," + 0;
            }
            return w.Left + "," + w.Top + "," + w.Width + "," + w.Height;
        }
    }
}