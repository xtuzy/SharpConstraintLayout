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

namespace androidx.constraintlayout.core.scout
{

    /// <summary>
    /// Inference Probability tables
    /// There are two major entry points in this class
    /// computeConstraints - which build the Inference tables
    /// applyConstraints - applies the constraints to the widgets
    /// </summary>
    public class ScoutProbabilities
    {

        private const bool DEBUG = false;
        private const float BASELINE_ERROR = 4.0f;
        private const int RESULT_PROBABILITY = 0;
        private const int RESULT_MARGIN = 1;
        private const bool SUPPORT_CENTER_TO_NON_ROOT = true;
        private const bool SUPPORT_WEAK_TO_CENTER = true;
        private const int NEGATIVE_GAP_FLAG = -3;
        private const int CONSTRAINT_FAILED_FLAG = -2;
        private const float CENTER_ERROR = 2;
        private const float SLOPE_CENTER_CONNECTION = 20;
        private const int MAX_DIST_FOR_CENTER_OVERLAP = 40;
        private const int ROOT_MARGIN_DISCOUNT = 16;
        private const int MAX_ROOT_OVERHANG = 10;
        private const bool SKIP_SPARSE_COLUMNS = true;

        internal float[][][] mProbability; // probability of a connection
        internal float[][][] mMargin; // margin needed for that connection
        internal float[][][][] mBinaryBias; // Ratio needed for binary connections (should be .5 for now)
        internal float[][][][] mBinaryProbability; // probability of a left_right/up_down
        internal int len;

        /// <summary>
        /// This calculates a constraint tables
        /// </summary>
        /// <param name="list"> ordered list of widgets root must be list[0] </param>
        public virtual void computeConstraints(ScoutWidget[] list)
        {
            if (list.Length < 2)
            {
                throw new System.ArgumentException("list must contain more than 1 widget");
            }
            if (!list[0].Root)
            {
                throw new System.ArgumentException("list[0] must be root");
            }

            len = list.Length;

            mProbability = new float[len][][];
            mMargin = new float[len][][];

            // calculate probability for normal connections
            float[] result = new float[2]; // estimation function return 2 values probability & margin

            for (int i = 1; i < len; i++)
            { // for all non root widgets
                Direction[] all = Direction.AllDirections;
                if (list[i].getGuideline())
                {
                    continue;
                }
                mProbability[i] = new float[all.Length][];
                mMargin[i] = new float[all.Length][];
                for (int dir = 0; dir < all.Length; dir++)
                { // for all possible connections
                    Direction direction = Direction.get(dir);
                    int connectTypes = direction.connectTypes();

                    // create the multidimensional array on the fly
                    // to account for the variying size of the probability space
                    mProbability[i][dir] = new float[len * connectTypes];
                    mMargin[i][dir] = new float[len * connectTypes];

                    // fill in all candidate connections
                    for (int candidate = 0; candidate < mMargin[i][dir].Length; candidate++)
                    {
                        int widgetNumber = candidate / connectTypes;
                        int opposite = candidate % connectTypes;
                        Direction connectTo = (opposite == 0) ? direction : direction.Opposite;

                        estimateProbability(list[i], direction, list[widgetNumber], connectTo, list, result);
                        mProbability[i][dir][candidate] = result[RESULT_PROBABILITY];
                        mMargin[i][dir][candidate] = result[RESULT_MARGIN];
                    }
                }
            }

            // calculate probability for "centered" connections
            //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            //ORIGINAL LINE: mBinaryProbability = new float[len][2][len * 2][len * 2];
            mBinaryProbability = RectangularArrays.ReturnRectangularFloatArray(len, 2, len * 2, len * 2);
            //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            //ORIGINAL LINE: mBinaryBias = new float[len][2][len * 2][len * 2];
            mBinaryBias = RectangularArrays.ReturnRectangularFloatArray(len, 2, len * 2, len * 2);
            Direction[][] directions = new Direction[][]
            {
                new Direction[] {Direction.NORTH, Direction.SOUTH},
                new Direction[] {Direction.WEST, Direction.EAST}
            };
            for (int i = 1; i < len; i++)
            {
                for (int horizontal = 0; horizontal < 2; horizontal++)
                { // vert=0 or horizantal=1
                    Direction[] sides = directions[horizontal];
                    for (int candidate1 = 0; candidate1 < len * 2; candidate1++)
                    {
                        for (int candidate2 = 0; candidate2 < len * 2; candidate2++)
                        {

                            // candidates are 2 per widget (left/right or above/below)
                            int widget1Number = candidate1 / 2;
                            int widget2Number = candidate2 / 2;

                            // pick the sides to connect
                            Direction widget1Side = sides[candidate1 & 0x1];
                            Direction widget2Side = sides[candidate2 & 0x1];

                            estimateBinaryProbability(list[i], horizontal, list[widget1Number], widget1Side, list[widget2Number], widget2Side, list, result);
                            mBinaryProbability[i][horizontal][candidate1][candidate2] = result[RESULT_PROBABILITY];
                            mBinaryBias[i][horizontal][candidate1][candidate2] = result[RESULT_MARGIN];
                        }
                    }
                }
            }
            if (DEBUG)
            {
                printTable(list);
            }
        }

        /// <summary>
        /// This applies a constraint set suggested by the Inference tables
        /// </summary>
        /// <param name="list"> </param>
        public virtual void applyConstraints(ScoutWidget[] list)
        {

            // this provides the sequence of connections
            pickColumnWidgets(list);
            pickCenterOverlap(list);
            pickBaseLineConnections(list); // baseline first
            pickCenteredConnections(list, true); // centered connections that stretch
            pickMarginConnections(list, 10); // regular margin connections that are close
            pickCenteredConnections(list, false); // general centered connections
            pickMarginConnections(list, 100); // all remaining margins
                                              //pickWeakConstraints(list); // weak constraints for ensuring wrap content

            if (DEBUG)
            {
                printBaseTable(list);
            }
        }

        /// <summary>
        /// Find and connect widgets centered over other widgets </summary>
        /// <param name="list"> </param>
        private void pickCenterOverlap(ScoutWidget[] list)
        {
            // find any widget centered over the edge of another
            for (int i = 0; i < list.Length; i++)
            {
                ScoutWidget scoutWidget = list[i];
                float centerX = scoutWidget.getX() + scoutWidget.getWidth() / 2;
                float centerY = scoutWidget.getY() + scoutWidget.getHeight() / 2;
                for (int j = 0; j < list.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    ScoutWidget widget = list[j];
                    if (scoutWidget.getGuideline())
                    {
                        continue;
                    }
                    if (!widget.getGuideline() && ScoutWidget.distance(scoutWidget, widget) > MAX_DIST_FOR_CENTER_OVERLAP)
                    {
                        continue;
                    }
                    if (!widget.getGuideline() || widget.VerticalGuideline)
                    {
                        if (Math.Abs(widget.getX() - centerX) < CENTER_ERROR)
                        {
                            scoutWidget.setEdgeCentered(1, widget, Direction.WEST);
                        }
                        if (Math.Abs(widget.getX() + widget.getWidth() - centerX) < CENTER_ERROR)
                        {
                            scoutWidget.setEdgeCentered(1, widget, Direction.EAST);
                        }
                    }
                    if (!widget.getGuideline() || widget.HorizontalGuideline)
                    {
                        if (Math.Abs(widget.getY() - centerY) < CENTER_ERROR)
                        {
                            scoutWidget.setEdgeCentered(0, widget, Direction.NORTH);
                        }

                        if (Math.Abs(widget.getY() + widget.getHeight() - centerY) < CENTER_ERROR)
                        {
                            scoutWidget.setEdgeCentered(0, widget, Direction.SOUTH);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// force structure for column cases
        /// </summary>
        /// <param name="list"> </param>
        private void pickColumnWidgets(ScoutWidget[] list)
        {
            ScoutWidget[] w = new ScoutWidget[list.Length - 1];
            for (int i = 0; i < list.Length - 1; i++)
            {
                w[i] = list[i + 1];
            }
            Array.Sort(w, new ComparatorAnonymousInnerClass(this));

            List<List<ScoutWidget>> groups = new List<List<ScoutWidget>>();
            List<ScoutWidget> current = new List<ScoutWidget>();
            for (int i = 2; i < w.Length; i++)
            {
                ScoutWidget scoutWidget = w[i];
                if (sameCol(w[i], w[i - 1]))
                {
                    if (current.Count == 0)
                    {
                        groups.Add(current);
                        current.Add(w[i - 1]);
                        current.Add(w[i]);
                    }
                    else
                    {
                        if (sameCol(current[0], w[i]))
                        {
                            current.Add(w[i]);
                        }
                        else
                        {
                            current = new List<ScoutWidget>();
                            groups.Add(current);
                            current.Add(w[i - 1]);
                            current.Add(w[i]);
                        }
                    }
                }
            }
            int[] dualIndex = new int[2];

            foreach (List<ScoutWidget> group in groups)
            {
                if (SKIP_SPARSE_COLUMNS)
                { // skip columns that have lot of space to reject accidental columns.
                    Rectangle union = Rectangle.Empty;
                    int area = 0;
                    foreach (ScoutWidget scoutWidget in group)
                    {
                        Rectangle r = scoutWidget.Rectangle;
                        area += r.Width * r.Height;
                        if (union == Rectangle.Empty)
                        {
                            union = r;
                        }
                        else
                        {
                            //union = union.union(r);
                            union.Intersect(r);
                        }
                    }
                    int unionArea = union.Width * union.Height;
                    if (unionArea > 2 * area)
                    { // more than have the area is empty
                        continue;
                    }
                }

                ScoutWidget[] widgets = (ScoutWidget[])group.ToArray();
                Array.Sort(widgets, ScoutWidget.sSortY);
                bool reverse = widgets[0].rootDistanceY() > widgets[widgets.Length - 1].rootDistanceY();
                float[] max = new float[widgets.Length];
                int[] map = new int[widgets.Length];

                for (int i = 0; i < widgets.Length; i++)
                {
                    for (int j = 1; j < list.Length; j++)
                    {
                        if (widgets[i] == list[j])
                        {
                            map[i] = j;
                        }
                    }
                }
                // zero out probabilities of connecting to each other we are going to take care of it here
                for (int i = 0; i < widgets.Length; i++)
                {
                    for (int j = 0; j < widgets.Length; j++)
                    {
                        int l = map[j] * 2;
                        for (int k = 2; k < 2 * list.Length; k++)
                        {
                            mBinaryProbability[map[i]][1][l][k] = -1;
                            mBinaryProbability[map[i]][1][k][l] = -1;
                            mBinaryProbability[map[i]][1][l + 1][k] = -1;
                            mBinaryProbability[map[i]][1][k][l + 1] = -1;
                        }
                    }
                }

                int bestToConnect = -1;
                float maxVal = -1;
                for (int i = 0; i < widgets.Length; i++)
                {
                    max[i] = Utils.max(mBinaryProbability[map[i]][1], dualIndex);
                    if (maxVal < max[i])
                    {
                        bestToConnect = i;
                        maxVal = max[i];
                    }
                }

                if (reverse)
                {
                    for (int i = 1; i < widgets.Length; i++)
                    {
                        int gap = widgets[i].mConstraintWidget.Y;
                        gap -= widgets[i - 1].mConstraintWidget.Y;
                        gap -= widgets[i - 1].mConstraintWidget.Height;
                        widgets[i - 1].setConstraint(Direction.SOUTH.getDirection(), widgets[i], Direction.NORTH.getDirection(), gap);
                    }
                }
                else
                {
                    for (int i = 1; i < widgets.Length; i++)
                    {
                        int gap = widgets[i].mConstraintWidget.Y;
                        gap -= widgets[i - 1].mConstraintWidget.Y;
                        gap -= widgets[i - 1].mConstraintWidget.Height;
                        widgets[i].setConstraint(Direction.NORTH.getDirection(), widgets[i - 1], Direction.SOUTH.getDirection(), gap);
                    }
                }

                if (bestToConnect >= 0)
                {
                    Utils.max(mBinaryProbability[map[bestToConnect]][1], dualIndex);
                    ScoutWidget w1 = list[dualIndex[0] / 2];
                    ScoutWidget w2 = list[dualIndex[1] / 2];
                    Direction dir1 = ((dualIndex[0] & 0x1) == 0) ? Direction.WEST : Direction.EAST;
                    Direction dir2 = ((dualIndex[1] & 0x1) == 0) ? Direction.WEST : Direction.EAST;
                    widgets[bestToConnect].setCentered(0, w1, w2, dir1, dir2, 0);

                    for (int i = bestToConnect + 1; i < widgets.Length; i++)
                    {
                        widgets[i].setCentered(1, widgets[i - 1], widgets[i - 1], Direction.WEST, Direction.EAST, 0);
                    }
                    for (int i = 1; i <= bestToConnect; i++)
                    {
                        widgets[i - 1].setCentered(1, widgets[i], widgets[i], Direction.WEST, Direction.EAST, 0);
                    }
                }
                else
                {
                    if (reverse)
                    {
                        for (int i = 1; i < widgets.Length; i++)
                        {
                            widgets[i - 1].setCentered(0, widgets[i], widgets[i], Direction.WEST, Direction.EAST, 0);
                        }
                    }
                    else
                    {
                        for (int i = 1; i < widgets.Length; i++)
                        {
                            widgets[i].setCentered(0, widgets[i - 1], widgets[i - 1], Direction.WEST, Direction.EAST, 0);
                        }
                    }
                }
            }
        }

        private class ComparatorAnonymousInnerClass : IComparer<ScoutWidget>
        {
            private readonly ScoutProbabilities outerInstance;

            public ComparatorAnonymousInnerClass(ScoutProbabilities outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual int Compare(ScoutWidget w1, ScoutWidget w2)
            {
                //int n = int.Compare(w1.mConstraintWidget.X, w2.mConstraintWidget.X);
                int n = w1.mConstraintWidget.X - w2.mConstraintWidget.X;
                if (n == 0)
                {
                    //n = Integer.compare(w1.mConstraintWidget.Width, w2.mConstraintWidget.Width);
                    n = w1.mConstraintWidget.Width - w2.mConstraintWidget.Width;
                }
                return n;
            }
        }

        private static bool sameCol(ScoutWidget a, ScoutWidget b)
        {
            return a.mConstraintWidget.X == b.mConstraintWidget.X && a.mConstraintWidget.Width == b.mConstraintWidget.Width;
        }

        /// <summary>
        /// This searches for baseline connections with a very narrow tolerance
        /// </summary>
        /// <param name="list"> </param>
        private void pickBaseLineConnections(ScoutWidget[] list)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int baseline = Direction.BASE.getDirection();
            int baseline = Direction.BASE.getDirection();
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int north = Direction.NORTH.getDirection();
            int north = Direction.NORTH.getDirection();
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int south = Direction.SOUTH.getDirection();
            int south = Direction.SOUTH.getDirection();
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int east = Direction.EAST.getDirection();
            int east = Direction.EAST.getDirection();
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int west = Direction.WEST.getDirection();
            int west = Direction.WEST.getDirection();

            // Search for baseline connections
            for (int i = 1; i < len; i++)
            {
                float[][] widgetProbability = mProbability[i];

                if (widgetProbability == null || widgetProbability[baseline] == null)
                {
                    continue;
                }
                float maxValue = 0.0f;

                float maxNorth = widgetProbability[north][Utils.max(widgetProbability[north])];
                float maxSouth = widgetProbability[south][Utils.max(widgetProbability[south])];

                int maxIndex = Utils.max(widgetProbability[baseline]);
                float maxBaseline = widgetProbability[baseline][maxIndex];
                if (maxBaseline < maxNorth || maxBaseline < maxSouth)
                {
                    continue;
                }

                string s;
                if (DEBUG)
                {
                    Console.WriteLine(" b check " + list[i] + " " + widgetProbability[4][maxIndex]);
                    //s = list[i] + "(" + Direction.ToString(baseline) + ") -> " + list[maxIndex] + " " + Direction.ToString(baseline);
                    s = list[i] + "(" + baseline.ToString() + ") -> " + list[maxIndex] + " " + baseline.ToString();
                    Console.WriteLine("try " + s);
                }

                if (list[i].setConstraint(baseline, list[maxIndex], baseline, 0))
                {
                    Utils.zero(mBinaryProbability[i][Direction.ORIENTATION_VERTICAL]);
                    widgetProbability[baseline].Fill(0.0f);
                    widgetProbability[north] = null;
                    widgetProbability[south].Fill(0.0f);

                    if (DEBUG)
                    {
                        Console.WriteLine("connect " + s);
                    }
                }
            }
        }

        /// <summary>
        /// This searches for centered connections
        /// </summary>
        /// <param name="list"> widgets (0 is root) </param>
        /// <param name="checkResizeable"> if true will attempt to make a stretchable widget </param>
        private void pickCenteredConnections(ScoutWidget[] list, bool checkResizeable)
        {
            Direction[][] side = new Direction[][]
            {
                new Direction[] {Direction.NORTH, Direction.SOUTH},
                new Direction[] {Direction.WEST, Direction.EAST}
            };
            int[] dualIndex = new int[2];
            for (int i = 1; i < len; i++)
            {
                float[][][] widgetBinaryProbability = mBinaryProbability[i];
                float[][][] widgetBinaryBias = mBinaryBias[i];

                for (int horizontal = 0; horizontal < widgetBinaryProbability.Length; horizontal++)
                { // vert=0 or horizontals=1
                    float[][] pmatrix = widgetBinaryProbability[horizontal];
                    float[][] bias = widgetBinaryBias[horizontal];
                    if (pmatrix == null)
                    {
                        continue;
                    }
                    bool worked = false;
                    while (!worked)
                    {
                        Utils.max(pmatrix, dualIndex);
                        int max1 = dualIndex[0];
                        int max2 = dualIndex[1];
                        int wNo1 = max1 / 2;
                        int wNo2 = max2 / 2;
                        Direction widget1Side = side[horizontal][max1 & 0x1];
                        Direction widget2Side = side[horizontal][max2 & 0x1];

                        // pick the sides to connect
                        float centerProbability = pmatrix[max1][max2];
                        worked = true;
                        if (centerProbability > .9)
                        {
                            if (checkResizeable && !list[i].isCandidateResizable(horizontal))
                            {
                                continue;
                            }

                            worked = list[i].setCentered(horizontal * 2, list[wNo1], list[wNo2], widget1Side, widget2Side, bias[max1][max2]);
                            if (worked)
                            {
                                mProbability[i][horizontal * 2] = null;
                                mProbability[i][horizontal * 2 + 1] = null;
                            }
                            else
                            {
                                pmatrix[max1][max2] = 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This searches for Normal margin connections
        /// </summary>
        /// <param name="list">             list of scouts </param>
        /// <param name="maxMarginPercent"> only margins less than that percent will be connected </param>
        private void pickMarginConnections(ScoutWidget[] list, int maxMarginPercent)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int baseline = Direction.BASE.getDirection();
            int baseline = Direction.BASE.getDirection();
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int north = Direction.NORTH.getDirection();
            int north = Direction.NORTH.getDirection();
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int south = Direction.SOUTH.getDirection();
            int south = Direction.SOUTH.getDirection();
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int east = Direction.EAST.getDirection();
            int east = Direction.EAST.getDirection();
            int width = list[0].mConstraintWidget.Width;
            int height = list[0].mConstraintWidget.Width;
            int maxWidthMargin = (width * maxMarginPercent) / 100;
            int maxHeightMargin = (height * maxMarginPercent) / 100;
            int[] maxMargin = new int[] { maxHeightMargin, maxWidthMargin };
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int west = Direction.WEST.getDirection();
            int west = Direction.WEST.getDirection();
            // pick generic connections
            int[][] dirTypes = new int[][]
            {
                new int[] {north, south},
                new int[] {west, east}
            };
            for (int i = len - 1; i > 0; i--)
            {
                float[][] widgetProbability = mProbability[i];

                for (int horizontal = 0; horizontal < 2; horizontal++)
                {
                    int[] dirs = dirTypes[horizontal];
                    bool found = false;
                    while (!found)
                    {
                        found = true;
                        int setlen = dirs.Length;
                        if (DEBUG)
                        {
                            Console.WriteLine(" check " + list[i] + " " + horizontal);
                        }
                        int dir = dirs[0];
                        if (widgetProbability == null || widgetProbability[dir] == null)
                        {
                            continue;
                        }
                        int maxIndex = 0;
                        int maxDirection = 0;
                        float maxValue = 0.0f;
                        int rowType = 0;
                        for (int j = 0; j < setlen; j++)
                        {
                            int rowMaxIndex = Utils.max(widgetProbability[dirs[j]]);
                            if (maxValue < widgetProbability[dirs[j]][rowMaxIndex])
                            {
                                maxDirection = dirs[j];
                                maxIndex = rowMaxIndex;
                                maxValue = widgetProbability[dirs[j]][rowMaxIndex];
                            }
                        }
                        if (widgetProbability[maxDirection] == null)
                        {
                            Console.WriteLine(list[i] + " " + maxDirection);
                            continue;
                        }
                        int m, cDir;
                        if (maxDirection == baseline)
                        { // baseline connection
                            m = maxIndex;
                            cDir = baseline; // always baseline
                        }
                        else
                        {
                            m = maxIndex / 2;
                            cDir = maxDirection;
                            if (maxIndex % 2 == 1)
                            {
                                cDir = cDir ^ 1;
                            }
                        }
                        if (mMargin[i][maxDirection][maxIndex] > maxMargin[horizontal])
                        {
                            continue;
                        }
                        //string s = list[i] + "(" + Direction.ToString(maxDirection) + ") -> " + list[m] + " " + Direction.ToString(cDir);
                        string s = list[i] + "(" + maxDirection.ToString() + ") -> " + list[m] + " " + cDir.ToString();
                        if (DEBUG)
                        {
                            Console.WriteLine("try " + s);
                        }
                        if (!list[i].setConstraint(maxDirection, list[m], cDir, mMargin[i][maxDirection][maxIndex]))
                        {
                            if (widgetProbability[maxDirection][maxIndex] >= 0)
                            {
                                widgetProbability[maxDirection][maxIndex] = CONSTRAINT_FAILED_FLAG;
                                found = false;
                            }
                        }
                        else
                        {
                            mBinaryProbability[i][horizontal] = null;
                            if (DEBUG)
                            {
                                Console.WriteLine("connect " + s);
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// pick weak constraints
        /// </summary>
        /// <param name="list"> </param>
        private void pickWeakConstraints(ScoutWidget[] list)
        {
            Direction[] directions = new Direction[] { Direction.NORTH, Direction.SOUTH, Direction.WEST, Direction.EAST };
            ScoutWidget[][] candidates = new ScoutWidget[directions.Length][]; // no arrays of generics
            ScoutWidget[] maxCandidate = new ScoutWidget[directions.Length];
            ScoutWidget centeredVertical = null;
            ScoutWidget centeredHorizontal = null;
            float[] maxDist = new float[] { -1, -1, -1, -1 };

            // find the biggest widget centered
            for (int i = 1; i < list.Length; i++)
            {
                ScoutWidget widget = list[i];
                if (widget.isCentered(Direction.ORIENTATION_VERTICAL))
                {
                    if (centeredVertical == null || centeredVertical.getHeight() > widget.getHeight())
                    {
                        centeredVertical = widget;
                    }
                }
                if (widget.isCentered(Direction.ORIENTATION_HORIZONTAL))
                {
                    if (centeredHorizontal == null || centeredHorizontal.getWidth() > widget.getWidth())
                    {
                        centeredHorizontal = widget;
                    }
                }
            }
            ScoutWidget[] centeredMax = new ScoutWidget[] { centeredVertical, centeredVertical, centeredHorizontal, centeredHorizontal };
            // build table of widgets open from each direction
            for (int j = 0; j < directions.Length; j++)
            {
                Direction direction = directions[j];
                List<ScoutWidget> tmp = new List<ScoutWidget>();
                for (int i = 1; i < list.Length; i++)
                {
                    ScoutWidget widget = list[i];
                    if (widget.getGuideline())
                    {
                        continue;
                    }
                    if (!widget.isConnected(directions[j].Opposite))
                    {
                        float dist = widget.connectedDistanceToRoot(list, direction);
                        if (!float.IsNaN(dist))
                        {
                            if (dist > maxDist[j])
                            {
                                maxDist[j] = dist;
                                maxCandidate[j] = widget;
                            }
                            tmp.Add(widget);
                        }
                    }
                }
                candidates[j] = tmp.ToArray();
                if (DEBUG)
                {
                    string s = "[" + direction + "]";
                    s += "max=" + maxCandidate[j] + " ";
                    for (int i = 0; i < candidates[j].Length; i++)
                    {
                        ScoutWidget c = candidates[j][i];
                        s += " " + c + " " + c.connectedDistanceToRoot(list, direction);
                    }
                    Console.WriteLine(s);
                }
            }

            // when there is nothing on the other side add a constraint to the longest
            // optionally attaching to the center if it fits
            for (int j = 0; j < directions.Length; j++)
            {
                if ((candidates[j].Length > 0) && (candidates[j ^ 1].Length == 0))
                {

                    int dirInt = directions[j].Opposite.getDirection();
                    int rootDirInt = directions[j].Opposite.getDirection();
                    ScoutWidget connect = list[0];
                    int connectSide = rootDirInt;

                    if (SUPPORT_WEAK_TO_CENTER)
                    {
                        if (centeredMax[j] != null)
                        {
                            float centerPos = centeredMax[j].getLocation(directions[j]);
                            float maxPos = maxCandidate[j].getLocation(directions[j].Opposite);
                            float delta = centerPos - maxPos;
                            if (directions[j] == Direction.EAST || directions[j] == Direction.SOUTH)
                            {
                                delta = -delta;
                            }
                            if (delta > 0)
                            {
                                connectSide = directions[j].getDirection();
                                connect = centeredMax[j];
                            }
                        }
                    }
                    maxCandidate[j].setWeakConstraint(dirInt, connect, connectSide);
                    candidates[j] = new ScoutWidget[0]; // prevent next step from using
                }
            }

            // Where there is no overlap
            for (int j = 0; j < directions.Length; j += 2)
            {
                if ((candidates[j].Length > 0) && (candidates[j + 1].Length > 0))
                {
                    Direction side = directions[j].Opposite;
                    Direction otherSide = directions[j];
                    if (maxCandidate[j].getLocation(side) < maxCandidate[j + 1].getLocation(otherSide))
                    {
                        maxCandidate[j].setWeakConstraint(side.getDirection(), maxCandidate[j + 1], otherSide.getDirection());
                        candidates[j] = new ScoutWidget[0]; // prevent next step from using
                        maxCandidate[j] = null;
                    }
                }
            }

            //  pick the closest clean shot to the other side if it is a maximum
            for (int j = 0; j < directions.Length; j += 2)
            {
                if ((candidates[j].Length > 0) && (candidates[j + 1].Length > 0))
                {
                    Direction side = directions[j].Opposite;
                    Direction otherSide = directions[j];

                    bool clearToRoot1 = maxCandidate[j].getNeighbor(otherSide, list).Root;
                    bool clearToRoot2 = maxCandidate[j].getNeighbor(side, list).Root;
                    if (clearToRoot1 && clearToRoot2)
                    {
                        if (maxDist[j] > maxDist[j + 1])
                        {
                            maxCandidate[j].setWeakConstraint(side.getDirection(), list[0], side.getDirection());
                        }
                        else
                        {
                            maxCandidate[j + 1].setWeakConstraint(otherSide.getDirection(), list[0], otherSide.getDirection());
                        }
                    }
                }
            }
        }

        /*-----------------------------------------------------------------------*/
        // core probability estimators
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// This defines the "probability" of a constraint between two widgets.
        /// </summary>
        /// <param name="from">    source widget </param>
        /// <param name="fromDir"> direction on that widget </param>
        /// <param name="to">      destination widget </param>
        /// <param name="toDir">   destination side to connect </param>
        /// <param name="result">  populates results with probability and offset </param>
        private static void estimateProbability(ScoutWidget from, Direction fromDir, ScoutWidget to, Direction toDir, ScoutWidget[] list, float[] result)
        {
            result[RESULT_PROBABILITY] = 0;
            result[RESULT_MARGIN] = 0;

            if (from == to)
            { // 0 probability of connecting to yourself
                return;
            }
            if (from.getGuideline())
            {
                return;
            }

            if (to.getGuideline())
            {
                if ((toDir == Direction.NORTH || toDir == Direction.SOUTH) && to.VerticalGuideline)
                {
                    return;
                }
                if ((toDir == Direction.EAST || toDir == Direction.WEST) && to.HorizontalGuideline)
                {
                    return;
                }
            }

            // if it already has a baseline do not connect to it
            if ((toDir == Direction.NORTH || toDir == Direction.SOUTH) & from.hasBaseline())
            {
                if (from.hasConnection(Direction.BASE))
                {
                    return;
                }
            }

            if (fromDir == Direction.BASE)
            { // if baseline 0  probability of connecting to non baseline
                if (!from.hasBaseline() || !to.hasBaseline())
                { // no base line
                    return;
                }
            }

            float fromLocation = from.getLocation(fromDir);
            float toLocation = to.getLocation(toDir);
            float positionDiff = (fromDir.reverse()) ? fromLocation - toLocation : toLocation - fromLocation;
            float distance = 2 * ScoutWidget.distance(from, to);
            if (to.Root)
            {
                distance = Math.Abs(distance - ROOT_MARGIN_DISCOUNT);
            }
            // probability decreases with distance and margin distance
            float probability = 1 / (1 + distance * distance + positionDiff * positionDiff);
            if (fromDir == Direction.BASE)
            { // prefer baseline
                if (Math.Abs(positionDiff) > BASELINE_ERROR)
                {
                    return;
                }
                probability *= 2;
            }
            if (to.Root)
            {
                probability *= 2;
            }
            result[RESULT_PROBABILITY] = (positionDiff >= 0) ? probability : NEGATIVE_GAP_FLAG;
            result[RESULT_MARGIN] = positionDiff;
        }

        /// <summary>
        /// This defines the constraint between a widget and two widgets to the left and right of it.
        /// Currently only encourages probability between widget and root for center purposes.
        /// </summary>
        /// <param name="from">        source widget </param>
        /// <param name="orientation"> horizontal or vertical connections (1 is horizontal) </param>
        /// <param name="to1">         connect to on one side </param>
        /// <param name="toDir1">      direction on that widget </param>
        /// <param name="to2">         connect to on other side </param>
        /// <param name="toDir2">      direction on that widget </param>
        /// <param name="result">      populates results with probability and offset </param>
        private static void estimateBinaryProbability(ScoutWidget from, int orientation, ScoutWidget to1, Direction toDir1, ScoutWidget to2, Direction toDir2, ScoutWidget[] list, float[] result)
        { // 0 = north/south 1 = east/west

            result[RESULT_PROBABILITY] = 0;
            result[RESULT_MARGIN] = 0;
            if (from == to1 || from == to2)
            { // cannot center on yourself
                return;
            }
            if (from.getGuideline())
            {
                return;
            }
            // if it already has a baseline do not connect to it
            if ((orientation == Direction.ORIENTATION_VERTICAL) & from.hasBaseline())
            {
                if (from.hasConnection(Direction.BASE))
                {
                    return;
                }
            }
            // distance normalizing scale factor
            float scale = 0.5f * ((orientation == Direction.ORIENTATION_VERTICAL) ? from.Parent.getHeight() : from.Parent.getWidth());
            Direction fromLeft = Direction.getDirections(orientation)[0];
            Direction fromRight = Direction.getDirections(orientation)[1];

            float location1 = from.getLocation(fromLeft);
            float location2 = from.getLocation(fromRight);
            float toLoc1 = to1.getLocation(toDir1);
            float toLoc2 = to2.getLocation(toDir2);
            float positionDiff1 = location1 - toLoc1;
            float positionDiff2 = toLoc2 - location2;

            if (positionDiff1 < 0 || positionDiff2 < 0)
            { // do not center if not aligned
                bool badCandidate = true;
                if (positionDiff2 < 0 && to2.Root && positionDiff2 > -MAX_ROOT_OVERHANG)
                {
                    badCandidate = false;
                    positionDiff2 = 0;
                }
                if (positionDiff1 < 0 && to1.Root && positionDiff2 > -MAX_ROOT_OVERHANG)
                {
                    badCandidate = false;
                    positionDiff2 = 0;
                }
                if (badCandidate)
                {
                    result[RESULT_PROBABILITY] = NEGATIVE_GAP_FLAG;
                    return;
                }
            }

            float distance1 = ScoutWidget.distance(from, to1) / scale;
            float distance2 = ScoutWidget.distance(from, to2) / scale;
            float diff = Math.Abs(positionDiff1 - positionDiff2);
            float probability = ((diff < SLOPE_CENTER_CONNECTION) ? 1 : 0); // favor close distance
            probability = probability / (1 + distance1 + distance2);
            probability += 1 / (1 + Math.Abs(positionDiff1 - positionDiff2));
            probability *= (to1.Root && to2.Root) ? 2 : ((SUPPORT_CENTER_TO_NON_ROOT) ? 1f : 0);

            result[RESULT_PROBABILITY] = probability;
            result[RESULT_MARGIN] = Math.Min(positionDiff1, positionDiff2);
        }

        /*-----------------------------------------------------------------------*/
        // Printing fuctions (for use in debugging)
        /*-----------------------------------------------------------------------*/

        /// <summary>
        /// Print the Tables
        /// </summary>
        /// <param name="list"> </param>
        public virtual void printTable(ScoutWidget[] list)
        {
            printCenterTable(list);
            printBaseTable(list);
        }

        /// <summary>
        /// Print the tables involved int centering the widgets
        /// </summary>
        /// <param name="list"> </param>
        public virtual void printCenterTable(ScoutWidget[] list)
        {
            // PRINT DEBUG
            Console.WriteLine("----------------- BASE TABLE --------------------");
            const int SIZE = 10;
            string padd = (new string(new char[SIZE])).Replace('\0', ' ');

            Console.Write("  ");
            for (int i = 0; i < len; i++)
            {
                string dbg = "[" + i + "] " + list[i] + "-------------------------";
                dbg = dbg.Substring(0, 20);
                Console.Write(dbg + ((i == len - 1) ? "\n" : ""));
            }

            string str = "[";
            for (int con = 0; con < len * 2; con++)
            {
                int opposite = con & 0x1;
                str += (con / 2 + ((opposite == 0) ? "->" : "<-") + "           ").Substring(0, 10);
            }

            Console.WriteLine("  " + str);

            for (int i = 1; i < len; i++)
            {
                for (int dir = 0; dir < mBinaryProbability[i].Length; dir++)
                { // above, below, left, right
                    string tab = "";
                    for (int k = 0; k < mBinaryProbability[i][dir].Length; k++)
                    {
                        tab += Utils.toS(mBinaryProbability[i][dir][k]) + "\n  ";
                    }
                    //Console.WriteLine(Direction.ToString(dir) + " " + tab);
                    Console.WriteLine(dir.ToString() + " " + tab);
                }
            }
        }

        /// <summary>
        /// Prints the tables involved in the normal widget asociations.
        /// </summary>
        /// <param name="list"> </param>
        public virtual void printBaseTable(ScoutWidget[] list)
        {
            // PRINT DEBUG
            Console.WriteLine("----------------- CENTER TABLE --------------------");

            const int SIZE = 10;
            string padd = (new string(new char[SIZE])).Replace('\0', ' ');

            Console.Write(" ");
            for (int i = 0; i < len; i++)
            {
                string dbg = "[" + i + "] " + list[i] + "-------------------------";
                if (i == 0)
                {
                    dbg = padd + dbg.Substring(0, 20);
                }
                else
                {
                    dbg = dbg.Substring(0, 20);
                }
                Console.Write(dbg + ((i == len - 1) ? "\n" : ""));
            }

            string str = "[";
            for (int con = 0; con < len * 2; con++)
            {
                int opposite = con & 0x1;
                str += (con / 2 + ((opposite == 0) ? "->" : "<-") + "           ").Substring(0, 10);
            }

            string header = ("Connection " + padd).Substring(0, SIZE);

            Console.WriteLine(header + " " + str);

            for (int i = 1; i < len; i++)
            {
                if (mProbability[i] == null)
                {
                    continue;
                }
                for (int dir = 0; dir < mProbability[i].Length; dir++)
                { // above, below, left, right

                    //Console.WriteLine(Utils.leftTrim(padd + i + " " + Direction.ToString(dir), SIZE) + " " + Utils.toS(mProbability[i][dir]));
                    Console.WriteLine(Utils.leftTrim(padd + i + " " + dir.ToString(), SIZE) + " " + Utils.toS(mProbability[i][dir]));
                    Console.WriteLine(padd + " " + Utils.toS(mMargin[i][dir]));

                }
            }
        }
    }

}