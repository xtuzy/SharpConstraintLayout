﻿/*
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Core
{
    public partial class ConstraintSet
    {
        public virtual Constraint getParameters(int mId)
        {
            return get(mId);
        }

        public void Clone(ConstraintLayout constraintLayout)
        {
            //TODO:此处修改了谷歌实现的逻辑,注意需要测试

            mConstraints.Clear();
            //拿到原ConstraintSet
            var oldSet = constraintLayout.ConstraintSet;
            foreach (var c in oldSet.mConstraints)
            {
                int id = c.Key;

                if (!mConstraints.ContainsKey(id))
                {
                    mConstraints[id] = c.Value.clone();
                }
                else
                {
                    throw new NotImplementedException($"新字典不应该包含{id},难道是HashCode重复了?");
                }
            }

        }

       /* public void Clone(Constraints constraints)
        {
            throw new NotImplementedException();
        }
*/
        public virtual void Clone(ConstraintSet set)
        {
            mConstraints.Clear();
            foreach (int key in set.mConstraints.Keys)
            {
                Constraint constraint = set.mConstraints[key];
                if (constraint == null)
                {
                    continue;
                }
                mConstraints[key] = constraint.clone();
            }
        }

        public void ApplyTo(ConstraintLayout constraintLayout)
        {
            applyToInternal(constraintLayout, true);
            //constraintLayout.ConstraintSet = null;
            //constraintLayout.requestLayout();
            constraintLayout.UpdateLayout();
        }

        /// <summary>
        /// Used to set constraints when used by constraint layout
        /// </summary>
        internal virtual void applyToInternal(ConstraintLayout constraintLayout, bool applyPostLayout)
        {
            constraintLayout.ConstraintSet.Dispose();
            constraintLayout.ConstraintSet = null;
            constraintLayout.ConstraintSet = this;
        }

        /// <summary>
        /// Center widget between the other two widgets.
        /// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>, {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso cref="#RIGHT"/>)
        /// Note, sides must be all vertical or horizontal sides.
        /// </summary>
        /// <param name="centerID">     ID of the widget to be centered </param>
        /// <param name="firstID">      ID of the first widget to connect the left or top of the widget to </param>
        /// <param name="firstSide">    the side of the widget to connect to </param>
        /// <param name="firstMargin">  the connection margin </param>
        /// <param name="secondId">     the ID of the second widget to connect to right or top of the widget to </param>
        /// <param name="secondSide">   the side of the widget to connect to </param>
        /// <param name="secondMargin"> the connection margin </param>
        /// <param name="bias">         the ratio between two connections </param>
        public virtual void Center(int centerID, int firstID, int firstSide, int firstMargin, int secondId, int secondSide, int secondMargin, float bias)
        {
            // Error checking

            if (firstMargin < 0)
            {
                throw new System.ArgumentException("margin must be > 0");
            }
            if (secondMargin < 0)
            {
                throw new System.ArgumentException("margin must be > 0");
            }
            if (bias <= 0 || bias > 1)
            {
                throw new System.ArgumentException("bias must be between 0 and 1 inclusive");
            }

            if (firstSide == LEFT || firstSide == RIGHT)
            {
                Connect(centerID, LEFT, firstID, firstSide, firstMargin);
                Connect(centerID, RIGHT, secondId, secondSide, secondMargin);
                Constraint constraint = mConstraints[centerID];
                if (constraint != null)
                {
                    constraint.layout.horizontalBias = bias;
                }
            }
            else if (firstSide == START || firstSide == END)
            {
                Connect(centerID, START, firstID, firstSide, firstMargin);
                Connect(centerID, END, secondId, secondSide, secondMargin);
                Constraint constraint = mConstraints[centerID];
                if (constraint != null)
                {
                    constraint.layout.horizontalBias = bias;
                }
            }
            else
            {
                Connect(centerID, TOP, firstID, firstSide, firstMargin);
                Connect(centerID, BOTTOM, secondId, secondSide, secondMargin);
                Constraint constraint = mConstraints[centerID];
                if (constraint != null)
                {
                    constraint.layout.verticalBias = bias;
                }
            }
        }

        /// <summary>
        /// Centers the widget horizontally to the left and right side on another widgets sides.
        /// (for sides see: {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso cref="#RIGHT"/>)
        /// </summary>
        /// <param name="centerID">    ID of widget to be centered </param>
        /// <param name="leftId">      The Id of the widget on the left side </param>
        /// <param name="leftSide">    The side of the leftId widget to connect to </param>
        /// <param name="leftMargin">  The margin on the left side </param>
        /// <param name="rightId">     The Id of the widget on the right side </param>
        /// <param name="rightSide">   The side  of the rightId widget to connect to </param>
        /// <param name="rightMargin"> The margin on the right side </param>
        /// <param name="bias">        The ratio of the space on the left vs. right sides 0.5 is centered (default) </param>
        public virtual void CenterHorizontally(int centerID, int leftId, int leftSide, int leftMargin, int rightId, int rightSide, int rightMargin, float bias)
        {
            Connect(centerID, LEFT, leftId, leftSide, leftMargin);
            Connect(centerID, RIGHT, rightId, rightSide, rightMargin);
            Constraint constraint = mConstraints[centerID];
            if (constraint != null)
            {
                constraint.layout.horizontalBias = bias;
            }
        }

        /// <summary>
        /// Centers the widgets horizontally to the left and right side on another widgets sides.
        /// (for sides see: <seealso cref="#START"/>, <seealso cref="#END"/>,
        /// <seealso cref="#LEFT"/>, <seealso cref="#RIGHT"/>)
        /// </summary>
        /// <param name="centerID">    ID of widget to be centered </param>
        /// <param name="startId">     The Id of the widget on the start side (left in non rtl languages) </param>
        /// <param name="startSide">   The side of the startId widget to connect to </param>
        /// <param name="startMargin"> The margin on the start side </param>
        /// <param name="endId">       The Id of the widget on the start side (left in non rtl languages) </param>
        /// <param name="endSide">     The side of the endId widget to connect to </param>
        /// <param name="endMargin">   The margin on the end side </param>
        /// <param name="bias">        The ratio of the space on the start vs end side 0.5 is centered (default) </param>
        public virtual void CenterHorizontallyRtl(int centerID, int startId, int startSide, int startMargin, int endId, int endSide, int endMargin, float bias)
        {
            Connect(centerID, START, startId, startSide, startMargin);
            Connect(centerID, END, endId, endSide, endMargin);
            Constraint constraint = mConstraints[centerID];
            if (constraint != null)
            {
                constraint.layout.horizontalBias = bias;
            }
        }

        /// <summary>
        /// Centers the widgets vertically to the top and bottom side on another widgets sides.
        /// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>)
        /// </summary>
        /// <param name="centerID">     ID of widget to be centered </param>
        /// <param name="topId">        The Id of the widget on the top side </param>
        /// <param name="topSide">      The side of the leftId widget to connect to </param>
        /// <param name="topMargin">    The margin on the top side </param>
        /// <param name="bottomId">     The Id of the widget on the bottom side </param>
        /// <param name="bottomSide">   The side of the bottomId widget to connect to </param>
        /// <param name="bottomMargin"> The margin on the bottom side </param>
        /// <param name="bias">         The ratio of the space on the top vs. bottom sides 0.5 is centered (default) </param>
        public virtual void CenterVertically(int centerID, int topId, int topSide, int topMargin, int bottomId, int bottomSide, int bottomMargin, float bias)
        {
            Connect(centerID, TOP, topId, topSide, topMargin);
            Connect(centerID, BOTTOM, bottomId, bottomSide, bottomMargin);
            Constraint constraint = mConstraints[centerID];
            if (constraint != null)
            {
                constraint.layout.verticalBias = bias;
            }
        }

        /// <summary>
        /// Spaces a set of widgets vertically between the view topId and bottomId.
        /// Widgets can be spaced with weights.
        /// This operation sets all the related margins to 0.
        /// <para>
        /// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>)
        /// 
        /// </para>
        /// </summary>
        /// <param name="topId">      The id of the widget to connect to or PARENT_ID </param>
        /// <param name="topSide">    the side of the start to connect to </param>
        /// <param name="bottomId">   The id of the widget to connect to or PARENT_ID </param>
        /// <param name="bottomSide"> the side of the right to connect to </param>
        /// <param name="chainIds">   widgets to use as a chain </param>
        /// <param name="weights">    can be null </param>
        /// <param name="style">      set the style of the chain </param>
        public virtual void CreateVerticalChain(int topId, int topSide, int bottomId, int bottomSide, int[] chainIds, float[] weights, int style)
        {
            if (chainIds.Length < 2)
            {
                throw new System.ArgumentException("must have 2 or more widgets in a chain");
            }
            if (weights != null && weights.Length != chainIds.Length)
            {
                throw new System.ArgumentException("must have 2 or more widgets in a chain");
            }
            if (weights != null)
            {
                get(chainIds[0]).layout.verticalWeight = weights[0];
            }
            get(chainIds[0]).layout.verticalChainStyle = style;

            Connect(chainIds[0], TOP, topId, topSide, 0);
            for (int i = 1; i < chainIds.Length; i++)
            {
                int chainId = chainIds[i];
                Connect(chainIds[i], TOP, chainIds[i - 1], BOTTOM, 0);
                Connect(chainIds[i - 1], BOTTOM, chainIds[i], TOP, 0);
                if (weights != null)
                {
                    get(chainIds[i]).layout.verticalWeight = weights[i];
                }
            }
            Connect(chainIds[chainIds.Length - 1], BOTTOM, bottomId, bottomSide, 0);
        }

        /// <summary>
        /// Spaces a set of widgets horizontally between the view startID and endId.
        /// Widgets can be spaced with weights.
        /// This operation sets all the related margins to 0.
        /// <para>
        /// (for sides see: {@link #START, <seealso cref="#END"/>,
        /// {@link #LEFT, <seealso cref="#RIGHT"/>
        /// 
        /// </para>
        /// </summary>
        /// <param name="leftId">    The id of the widget to connect to or PARENT_ID </param>
        /// <param name="leftSide">  the side of the start to connect to </param>
        /// <param name="rightId">   The id of the widget to connect to or PARENT_ID </param>
        /// <param name="rightSide"> the side of the right to connect to </param>
        /// <param name="chainIds">  The widgets in the chain </param>
        /// <param name="weights">   The weight to assign to each element in the chain or null </param>
        /// <param name="style">     The type of chain </param>
        public virtual void CreateHorizontalChain(int leftId, int leftSide, int rightId, int rightSide, int[] chainIds, float[] weights, int style)
        {
            createHorizontalChain(leftId, leftSide, rightId, rightSide, chainIds, weights, style, LEFT, RIGHT);
        }

        /// <summary>
        /// Spaces a set of widgets horizontal between the view startID and endId.
        /// Widgets can be spaced with weights.
        /// (for sides see: {@link #START, <seealso cref="#END"/>,
        /// {@link #LEFT, <seealso cref="#RIGHT"/>)
        /// </summary>
        /// <param name="startId">   The id of the widget to connect to or PARENT_ID </param>
        /// <param name="startSide"> the side of the start to connect to </param>
        /// <param name="endId">     The id of the widget to connect to or PARENT_ID </param>
        /// <param name="endSide">   the side of the end to connect to </param>
        /// <param name="chainIds">  The widgets in the chain </param>
        /// <param name="weights">   The weight to assign to each element in the chain or null </param>
        /// <param name="style">     The type of chain </param>
        public virtual void CreateHorizontalChainRtl(int startId, int startSide, int endId, int endSide, int[] chainIds, float[] weights, int style)
        {
            createHorizontalChain(startId, startSide, endId, endSide, chainIds, weights, style, START, END);
        }

        private void createHorizontalChain(int leftId, int leftSide, int rightId, int rightSide, int[] chainIds, float[] weights, int style, int left, int right)
        {

            if (chainIds.Length < 2)
            {
                throw new System.ArgumentException("must have 2 or more widgets in a chain");
            }
            if (weights != null && weights.Length != chainIds.Length)
            {
                throw new System.ArgumentException("must have 2 or more widgets in a chain");
            }
            if (weights != null)
            {
                get(chainIds[0]).layout.horizontalWeight = weights[0];
            }
            get(chainIds[0]).layout.horizontalChainStyle = style;
            Connect(chainIds[0], left, leftId, leftSide, UNSET);
            for (int i = 1; i < chainIds.Length; i++)
            {
                int chainId = chainIds[i];
                Connect(chainIds[i], left, chainIds[i - 1], right, UNSET);
                Connect(chainIds[i - 1], right, chainIds[i], left, UNSET);
                if (weights != null)
                {
                    get(chainIds[i]).layout.horizontalWeight = weights[i];
                }
            }

            Connect(chainIds[chainIds.Length - 1], right, rightId, rightSide, UNSET);

        }

        /// <summary>
        /// Create a constraint between two widgets.
        /// (for sides see: {@link #TOP, <seealso cref="BOTTOM"/>, {@link #START, <seealso cref="END"/>,
        /// {@link #LEFT, <seealso cref="RIGHT"/>, <seealso cref="BASELINE"/>)
        /// </summary>
        /// <param name="startID">   the ID of the widget to be constrained </param>
        /// <param name="startSide"> the side of the widget to constrain </param>
        /// <param name="endID">     the id of the widget to constrain to </param>
        /// <param name="endSide">   the side of widget to constrain to </param>
        /// <param name="margin">    the margin to constrain (margin must be positive) </param>
        public virtual void Connect(int startID, int startSide, int endID, int endSide, int margin)
        {
            if (!mConstraints.ContainsKey(startID))
            {
                mConstraints[startID] = new Constraint();
            }
            Constraint constraint = mConstraints[startID];
            if (constraint == null)
            {
                return;
            }
            switch (startSide)
            {
                case LEFT:
                    if (endSide == LEFT)
                    {
                        constraint.layout.leftToLeft = endID;
                        constraint.layout.leftToRight = Layout.UNSET;
                    }
                    else if (endSide == RIGHT)
                    {
                        constraint.layout.leftToRight = endID;
                        constraint.layout.leftToLeft = Layout.UNSET;

                    }
                    else
                    {
                        throw new System.ArgumentException("Left to " + sideToString(endSide) + " undefined");
                    }
                    constraint.layout.leftMargin = margin;
                    break;
                case RIGHT:
                    if (endSide == LEFT)
                    {
                        constraint.layout.rightToLeft = endID;
                        constraint.layout.rightToRight = Layout.UNSET;

                    }
                    else if (endSide == RIGHT)
                    {
                        constraint.layout.rightToRight = endID;
                        constraint.layout.rightToLeft = Layout.UNSET;

                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    constraint.layout.rightMargin = margin;
                    break;
                case TOP:
                    if (endSide == TOP)
                    {
                        constraint.layout.topToTop = endID;
                        constraint.layout.topToBottom = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else if (endSide == BOTTOM)
                    {
                        constraint.layout.topToBottom = endID;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    constraint.layout.topMargin = margin;
                    break;
                case BOTTOM:
                    if (endSide == BOTTOM)
                    {
                        constraint.layout.bottomToBottom = endID;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else if (endSide == TOP)
                    {
                        constraint.layout.bottomToTop = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    constraint.layout.bottomMargin = margin;
                    break;
                case BASELINE:
                    if (endSide == BASELINE)
                    {
                        constraint.layout.baselineToBaseline = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else if (endSide == TOP)
                    {
                        constraint.layout.baselineToTop = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else if (endSide == BOTTOM)
                    {
                        constraint.layout.baselineToBottom = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;
                case START:
                    if (endSide == START)
                    {
                        constraint.layout.startToStart = endID;
                        constraint.layout.startToEnd = Layout.UNSET;
                    }
                    else if (endSide == END)
                    {
                        constraint.layout.startToEnd = endID;
                        constraint.layout.startToStart = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    constraint.layout.startMargin = margin;
                    break;
                case END:
                    if (endSide == END)
                    {
                        constraint.layout.endToEnd = endID;
                        constraint.layout.endToStart = Layout.UNSET;
                    }
                    else if (endSide == START)
                    {
                        constraint.layout.endToStart = endID;
                        constraint.layout.endToEnd = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    constraint.layout.endMargin = margin;
                    break;
                default:
                    throw new System.ArgumentException(sideToString(startSide) + " to " + sideToString(endSide) + " unknown");
            }
        }

        /// <summary>
        /// Create a constraint between two widgets.
        /// (for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>, {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso cref="#RIGHT"/>, <seealso cref="#BASELINE"/>)
        /// </summary>
        /// <param name="startID">   the ID of the widget to be constrained </param>
        /// <param name="startSide"> the side of the widget to constrain </param>
        /// <param name="endID">     the id of the widget to constrain to </param>
        /// <param name="endSide">   the side of widget to constrain to </param>
        public virtual void Connect(int startID, int startSide, int endID, int endSide)
        {
            if (!mConstraints.ContainsKey(startID))
            {
                mConstraints[startID] = new Constraint();
            }
            Constraint constraint = mConstraints[startID];
            if (constraint == null)
            {
                return;
            }
            switch (startSide)
            {
                case LEFT:
                    if (endSide == LEFT)
                    {
                        constraint.layout.leftToLeft = endID;
                        constraint.layout.leftToRight = Layout.UNSET;
                    }
                    else if (endSide == RIGHT)
                    {
                        constraint.layout.leftToRight = endID;
                        constraint.layout.leftToLeft = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("left to " + sideToString(endSide) + " undefined");
                    }
                    break;
                case RIGHT:
                    if (endSide == LEFT)
                    {
                        constraint.layout.rightToLeft = endID;
                        constraint.layout.rightToRight = Layout.UNSET;

                    }
                    else if (endSide == RIGHT)
                    {
                        constraint.layout.rightToRight = endID;
                        constraint.layout.rightToLeft = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;
                case TOP:
                    if (endSide == TOP)
                    {
                        constraint.layout.topToTop = endID;
                        constraint.layout.topToBottom = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else if (endSide == BOTTOM)
                    {
                        constraint.layout.topToBottom = endID;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;
                case BOTTOM:
                    if (endSide == BOTTOM)
                    {
                        constraint.layout.bottomToBottom = endID;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else if (endSide == TOP)
                    {
                        constraint.layout.bottomToTop = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;
                case BASELINE:
                    if (endSide == BASELINE)
                    {
                        constraint.layout.baselineToBaseline = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else if (endSide == TOP)
                    {
                        constraint.layout.baselineToTop = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else if (endSide == BOTTOM)
                    {
                        constraint.layout.baselineToBottom = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;
                case START:
                    if (endSide == START)
                    {
                        constraint.layout.startToStart = endID;
                        constraint.layout.startToEnd = Layout.UNSET;
                    }
                    else if (endSide == END)
                    {
                        constraint.layout.startToEnd = endID;
                        constraint.layout.startToStart = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;
                case END:
                    if (endSide == END)
                    {
                        constraint.layout.endToEnd = endID;
                        constraint.layout.endToStart = Layout.UNSET;
                    }
                    else if (endSide == START)
                    {
                        constraint.layout.endToStart = endID;
                        constraint.layout.endToEnd = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;
                default:
                    throw new System.ArgumentException(sideToString(startSide) + " to " + sideToString(endSide) + " unknown");
            }
        }

        /// <summary>
        /// Centers the view horizontally relative to toView's position.
        /// </summary>
        /// <param name="viewId"> ID of view to center Horizontally </param>
        /// <param name="toView"> ID of view to center on (or in) </param>
        public virtual void centerHorizontally(int viewId, int toView)
        {
            if (toView == PARENT_ID)
            {
                Center(viewId, PARENT_ID, ConstraintSet.LEFT, 0, PARENT_ID, ConstraintSet.RIGHT, 0, 0.5f);
            }
            else
            {
                Center(viewId, toView, ConstraintSet.RIGHT, 0, toView, ConstraintSet.LEFT, 0, 0.5f);
            }
        }

        /// <summary>
        /// Centers the view horizontally relative to toView's position.
        /// </summary>
        /// <param name="viewId"> ID of view to center Horizontally </param>
        /// <param name="toView"> ID of view to center on (or in) </param>
        public virtual void centerHorizontallyRtl(int viewId, int toView)
        {
            if (toView == PARENT_ID)
            {
                Center(viewId, PARENT_ID, ConstraintSet.START, 0, PARENT_ID, ConstraintSet.END, 0, 0.5f);
            }
            else
            {
                Center(viewId, toView, ConstraintSet.END, 0, toView, ConstraintSet.START, 0, 0.5f);
            }
        }

        /// <summary>
        /// Centers the view vertically relative to toView's position.
        /// </summary>
        /// <param name="viewId"> ID of view to center Horizontally </param>
        /// <param name="toView"> ID of view to center on (or in) </param>
        public virtual void centerVertically(int viewId, int toView)
        {
            if (toView == PARENT_ID)
            {
                Center(viewId, PARENT_ID, ConstraintSet.TOP, 0, PARENT_ID, ConstraintSet.BOTTOM, 0, 0.5f);
            }
            else
            {
                Center(viewId, toView, ConstraintSet.BOTTOM, 0, toView, ConstraintSet.TOP, 0, 0.5f);
            }
        }

        public void Clear(int viewId)
        {
            mConstraints.Remove(viewId);
        }

        public void Clear(int viewId, int anchor)
        {
            if (mConstraints.ContainsKey(viewId))
            {
                Constraint constraint = mConstraints[viewId];
                if (constraint == null)
                {
                    return;
                }
                switch (anchor)
                {
                    case LEFT:
                        constraint.layout.leftToRight = Layout.UNSET;
                        constraint.layout.leftToLeft = Layout.UNSET;
                        constraint.layout.leftMargin = Layout.UNSET;
                        constraint.layout.goneLeftMargin = Layout.UNSET_GONE_MARGIN;
                        break;
                    case RIGHT:
                        constraint.layout.rightToRight = Layout.UNSET;
                        constraint.layout.rightToLeft = Layout.UNSET;
                        constraint.layout.rightMargin = Layout.UNSET;
                        constraint.layout.goneRightMargin = Layout.UNSET_GONE_MARGIN;
                        break;
                    case TOP:
                        constraint.layout.topToBottom = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topMargin = 0;
                        constraint.layout.goneTopMargin = Layout.UNSET_GONE_MARGIN;
                        break;
                    case BOTTOM:
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomMargin = 0;
                        constraint.layout.goneBottomMargin = Layout.UNSET_GONE_MARGIN;
                        break;
                    case BASELINE:
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                        constraint.layout.baselineMargin = 0;
                        constraint.layout.goneBaselineMargin = Layout.UNSET_GONE_MARGIN;
                        break;
                    case START:
                        constraint.layout.startToEnd = Layout.UNSET;
                        constraint.layout.startToStart = Layout.UNSET;
                        constraint.layout.startMargin = 0;
                        constraint.layout.goneStartMargin = Layout.UNSET_GONE_MARGIN;
                        break;
                    case END:
                        constraint.layout.endToStart = Layout.UNSET;
                        constraint.layout.endToEnd = Layout.UNSET;
                        constraint.layout.endMargin = 0;
                        constraint.layout.goneEndMargin = Layout.UNSET_GONE_MARGIN;
                        break;
                    case CIRCLE_REFERENCE:
                        constraint.layout.circleAngle = Layout.UNSET;
                        constraint.layout.circleRadius = Layout.UNSET;
                        constraint.layout.circleConstraint = Layout.UNSET;
                        break;
                    default:
                        throw new System.ArgumentException("unknown constraint");
                }
            }
        }

        /// <summary>
        /// Sets the margin.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust the margin on </param>
        /// <param name="anchor"> The side to adjust the margin on </param>
        /// <param name="value">  The new value for the margin </param>
        public virtual void setMargin(int viewId, int anchor, int value)
        {
            Constraint constraint = get(viewId);
            switch (anchor)
            {
                case LEFT:
                    constraint.layout.leftMargin = value;
                    break;
                case RIGHT:
                    constraint.layout.rightMargin = value;
                    break;
                case TOP:
                    constraint.layout.topMargin = value;
                    break;
                case BOTTOM:
                    constraint.layout.bottomMargin = value;
                    break;
                case BASELINE:
                    constraint.layout.baselineMargin = value;
                    break;
                case START:
                    constraint.layout.startMargin = value;
                    break;
                case END:
                    constraint.layout.endMargin = value;
                    break;
                default:
                    throw new System.ArgumentException("unknown constraint");
            }
        }

        /// <summary>
        /// Sets the gone margin.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust the margin on </param>
        /// <param name="anchor"> The side to adjust the margin on </param>
        /// <param name="value">  The new value for the margin </param>
        public virtual void setGoneMargin(int viewId, int anchor, int value)
        {
            Constraint constraint = get(viewId);
            switch (anchor)
            {
                case LEFT:
                    constraint.layout.goneLeftMargin = value;
                    break;
                case RIGHT:
                    constraint.layout.goneRightMargin = value;
                    break;
                case TOP:
                    constraint.layout.goneTopMargin = value;
                    break;
                case BOTTOM:
                    constraint.layout.goneBottomMargin = value;
                    break;
                case BASELINE:
                    constraint.layout.goneBaselineMargin = value;
                    break;
                case START:
                    constraint.layout.goneStartMargin = value;
                    break;
                case END:
                    constraint.layout.goneEndMargin = value;
                    break;
                default:
                    throw new System.ArgumentException("unknown constraint");
            }
        }

        /// <summary>
        /// Adjust the horizontal bias of the view (used with views constrained on left and right).
        /// </summary>
        /// <param name="viewId"> ID of view to adjust the horizontal </param>
        /// <param name="bias">   the new bias 0.5 is in the middle </param>
        public virtual void setHorizontalBias(int viewId, float bias)
        {
            get(viewId).layout.horizontalBias = bias;
        }

        /// <summary>
        /// Adjust the vertical bias of the view (used with views constrained on left and right).
        /// </summary>
        /// <param name="viewId"> ID of view to adjust the vertical </param>
        /// <param name="bias">   the new bias 0.5 is in the middle </param>
        public virtual void setVerticalBias(int viewId, float bias)
        {
            get(viewId).layout.verticalBias = bias;
        }

        /// <summary>
        /// Constrains the views aspect ratio.
        /// For Example a HD screen is 16 by 9 = 16/(float)9 = 1.777f.
        /// </summary>
        /// <param name="viewId"> ID of view to constrain </param>
        /// <param name="ratio">  The ratio of the width to height (width / height) </param>
        public virtual void setDimensionRatio(int viewId, string ratio)
        {
            get(viewId).layout.dimensionRatio = ratio;
        }

        /// <summary>
        /// Adjust the visibility of a view.
        /// </summary>
        /// <param name="viewId">     ID of view to adjust the vertical </param>
        /// <param name="visibility"> the visibility </param>
        public virtual void setVisibility(int viewId, int visibility)
        {
            get(viewId).propertySet.visibility = visibility;
        }

        /// <summary>
        /// ConstraintSet will not setVisibility. <seealso cref="#VISIBILITY_MODE_IGNORE"/> or {@link
        /// #VISIBILITY_MODE_NORMAL}.
        /// </summary>
        /// <param name="viewId">         ID of view </param>
        /// <param name="visibilityMode"> </param>
        public virtual void setVisibilityMode(int viewId, int visibilityMode)
        {
            get(viewId).propertySet.mVisibilityMode = visibilityMode;
        }

        /// <summary>
        /// ConstraintSet will not setVisibility. <seealso cref="#VISIBILITY_MODE_IGNORE"/> or {@link
        /// #VISIBILITY_MODE_NORMAL}.
        /// </summary>
        /// <param name="viewId"> ID of view </param>
        public virtual int getVisibilityMode(int viewId)
        {
            return get(viewId).propertySet.mVisibilityMode;
        }

        /// <summary>
        /// Get the visibility flag set in this view
        /// </summary>
        /// <param name="viewId"> the id of the view </param>
        /// <returns> the visibility constraint for the view </returns>
        public virtual int getVisibility(int viewId)
        {
            return get(viewId).propertySet.visibility;
        }

        /// <summary>
        /// Get the height set in the view
        /// </summary>
        /// <param name="viewId"> the id of the view </param>
        /// <returns> return the height constraint of the view </returns>
        public virtual int getHeight(int viewId)
        {
            return get(viewId).layout.mHeight;
        }

        /// <summary>
        /// Get the width set in the view
        /// </summary>
        /// <param name="viewId"> the id of the view </param>
        /// <returns> return the width constraint of the view </returns>
        public virtual int getWidth(int viewId)
        {
            return get(viewId).layout.mWidth;
        }

        /// <summary>
        /// Adjust the alpha of a view.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust the vertical </param>
        /// <param name="alpha">  the alpha </param>
        public virtual void setAlpha(int viewId, float alpha)
        {
            get(viewId).propertySet.alpha = alpha;
        }

        /// <summary>
        /// return with the constraint set will apply elevation for the specified view.
        /// </summary>
        /// <returns> true if the elevation will be set on this view (default is false) </returns>
        public virtual bool getApplyElevation(int viewId)
        {
            return get(viewId).transform.applyElevation;
        }

        /// <summary>
        /// set if elevation will be applied to the view.
        /// Elevation logic is based on style and animation. By default it is not used because it would
        /// lead to unexpected results.
        /// </summary>
        /// <param name="apply"> true if this constraint set applies elevation to this view </param>
        public virtual void setApplyElevation(int viewId, bool apply)
        {
            //if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
            //{
                get(viewId).transform.applyElevation = apply;
            //}
        }

        /// <summary>
        /// Adjust the elevation of a view.
        /// </summary>
        /// <param name="viewId">    ID of view to adjust the elevation </param>
        /// <param name="elevation"> the elevation </param>
        public virtual void setElevation(int viewId, float elevation)
        {
            //if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
            //{
                get(viewId).transform.elevation = elevation;
                get(viewId).transform.applyElevation = true;
            //}
        }

        /// <summary>
        /// Adjust the post-layout rotation about the Z axis of a view.
        /// </summary>
        /// <param name="viewId">   ID of view to adjust th Z rotation </param>
        /// <param name="rotation"> the rotation about the X axis </param>
        public virtual void setRotation(int viewId, float rotation)
        {
            get(viewId).transform.rotation = rotation;
        }

        /// <summary>
        /// Adjust the post-layout rotation about the X axis of a view.
        /// </summary>
        /// <param name="viewId">    ID of view to adjust th X rotation </param>
        /// <param name="rotationX"> the rotation about the X axis </param>
        public virtual void setRotationX(int viewId, float rotationX)
        {
            get(viewId).transform.rotationX = rotationX;
        }

        /// <summary>
        /// Adjust the post-layout rotation about the Y axis of a view.
        /// </summary>
        /// <param name="viewId">    ID of view to adjust the Y rotation </param>
        /// <param name="rotationY"> the rotationY </param>
        public virtual void setRotationY(int viewId, float rotationY)
        {
            get(viewId).transform.rotationY = rotationY;
        }

        /// <summary>
        /// Adjust the post-layout scale in X of a view.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust the scale in X </param>
        /// <param name="scaleX"> the scale in X </param>
        public virtual void setScaleX(int viewId, float scaleX)
        {
            get(viewId).transform.scaleX = scaleX;
        }

        /// <summary>
        /// Adjust the post-layout scale in Y of a view.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust the scale in Y </param>
        /// <param name="scaleY"> the scale in Y </param>
        public virtual void setScaleY(int viewId, float scaleY)
        {
            get(viewId).transform.scaleY = scaleY;
        }

        /// <summary>
        /// Set X location of the pivot point around which the view will rotate and scale.
        /// use Float.NaN to clear the pivot value.
        /// Note: once an actual View has had its pivot set it cannot be cleared.
        /// </summary>
        /// <param name="viewId">          ID of view to adjust the transforms pivot point about X </param>
        /// <param name="transformPivotX"> X location of the pivot point. </param>
        public virtual void setTransformPivotX(int viewId, float transformPivotX)
        {
            get(viewId).transform.transformPivotX = transformPivotX;
        }

        /// <summary>
        /// Set Y location of the pivot point around which the view will rotate and scale.
        /// use Float.NaN to clear the pivot value.
        /// Note: once an actual View has had its pivot set it cannot be cleared.
        /// </summary>
        /// <param name="viewId">          ID of view to adjust the transforms pivot point about Y </param>
        /// <param name="transformPivotY"> Y location of the pivot point. </param>
        public virtual void setTransformPivotY(int viewId, float transformPivotY)
        {
            get(viewId).transform.transformPivotY = transformPivotY;
        }

        /// <summary>
        /// Set X,Y location of the pivot point around which the view will rotate and scale.
        /// use Float.NaN to clear the pivot value.
        /// Note: once an actual View has had its pivot set it cannot be cleared.
        /// </summary>
        /// <param name="viewId">          ID of view to adjust the transforms pivot point </param>
        /// <param name="transformPivotX"> X location of the pivot point. </param>
        /// <param name="transformPivotY"> Y location of the pivot point. </param>
        public virtual void setTransformPivot(int viewId, float transformPivotX, float transformPivotY)
        {
            Constraint constraint = get(viewId);
            constraint.transform.transformPivotY = transformPivotY;
            constraint.transform.transformPivotX = transformPivotX;
        }

        /// <summary>
        /// Adjust the post-layout X translation of a view.
        /// </summary>
        /// <param name="viewId">       ID of view to translate in X </param>
        /// <param name="translationX"> the translation in X </param>
        public virtual void setTranslationX(int viewId, float translationX)
        {
            get(viewId).transform.translationX = translationX;
        }

        /// <summary>
        /// Adjust the  post-layout Y translation of a view.
        /// </summary>
        /// <param name="viewId">       ID of view to to translate in Y </param>
        /// <param name="translationY"> the translation in Y </param>
        public virtual void setTranslationY(int viewId, float translationY)
        {
            get(viewId).transform.translationY = translationY;
        }

        /// <summary>
        /// Adjust the post-layout translation of a view.
        /// </summary>
        /// <param name="viewId">       ID of view to adjust its translation in X & Y </param>
        /// <param name="translationX"> the translation in X </param>
        /// <param name="translationY"> the translation in Y </param>
        public virtual void setTranslation(int viewId, float translationX, float translationY)
        {
            Constraint constraint = get(viewId);
            constraint.transform.translationX = translationX;
            constraint.transform.translationY = translationY;
        }

        /// <summary>
        /// Adjust the translation in Z of a view.
        /// </summary>
        /// <param name="viewId">       ID of view to adjust </param>
        /// <param name="translationZ"> the translationZ </param>
        public virtual void setTranslationZ(int viewId, float translationZ)
        {
            //if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
            //{
                get(viewId).transform.translationZ = translationZ;
            //}
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public virtual void setEditorAbsoluteX(int viewId, int position)
        {
            get(viewId).layout.editorAbsoluteX = position;
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public virtual void setEditorAbsoluteY(int viewId, int position)
        {
            get(viewId).layout.editorAbsoluteY = position;
        }

        /// <summary>
        /// Sets the wrap behavior of the widget in the parent's wrap computation
        /// </summary>
        public virtual void setLayoutWrapBehavior(int viewId, int behavior)
        {
            //if (behavior >= 0 && behavior <= ConstraintWidget.WRAP_BEHAVIOR_SKIPPED)
            //{
                get(viewId).layout.mWrapBehavior = behavior;
            //}
        }

        /// <summary>
        /// Sets the height of the view. It can be a dimension, <seealso cref="#WRAP_CONTENT"/> or {@link
        /// #MATCH_CONSTRAINT}.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its height </param>
        /// <param name="height"> the height of the view
        /// @since 1.1 </param>
        public virtual void constrainHeight(int viewId, int height)
        {
            get(viewId).layout.mHeight = height;
        }

        /// <summary>
        /// Sets the width of the view. It can be a dimension, <seealso cref="#WRAP_CONTENT"/> or {@link
        /// #MATCH_CONSTRAINT}.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its width </param>
        /// <param name="width">  the width of the view
        /// @since 1.1 </param>
        public virtual void constrainWidth(int viewId, int width)
        {
            get(viewId).layout.mWidth = width;
        }

        /// <summary>
        /// Constrain the view on a circle constraint
        /// </summary>
        /// <param name="viewId"> ID of the view we constrain </param>
        /// <param name="id">     ID of the view we constrain relative to </param>
        /// <param name="radius"> the radius of the circle in degrees </param>
        /// <param name="angle">  the angle
        /// @since 1.1 </param>
        public virtual void constrainCircle(int viewId, int id, int radius, float angle)
        {
            Constraint constraint = get(viewId);
            constraint.layout.circleConstraint = id;
            constraint.layout.circleRadius = radius;
            constraint.layout.circleAngle = angle;
        }

        /// <summary>
        /// Sets the maximum height of the view. It is a dimension, It is only applicable if height is
        /// #MATCH_CONSTRAINT}.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust it height </param>
        /// <param name="height"> the maximum height of the constraint
        /// @since 1.1 </param>
        public virtual void constrainMaxHeight(int viewId, int height)
        {
            get(viewId).layout.heightMax = height;
        }

        /// <summary>
        /// Sets the maximum width of the view. It is a dimension, It is only applicable if width is
        /// #MATCH_CONSTRAINT}.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its max height </param>
        /// <param name="width">  the maximum width of the view
        /// @since 1.1 </param>
        public virtual void constrainMaxWidth(int viewId, int width)
        {
            get(viewId).layout.widthMax = width;
        }

        /// <summary>
        /// Sets the height of the view. It is a dimension, It is only applicable if height is
        /// #MATCH_CONSTRAINT}.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its min height </param>
        /// <param name="height"> the minimum height of the view
        /// @since 1.1 </param>
        public virtual void constrainMinHeight(int viewId, int height)
        {
            get(viewId).layout.heightMin = height;
        }

        /// <summary>
        /// Sets the width of the view.  It is a dimension, It is only applicable if width is
        /// #MATCH_CONSTRAINT}.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its min height </param>
        /// <param name="width">  the minimum width of the view
        /// @since 1.1 </param>
        public virtual void constrainMinWidth(int viewId, int width)
        {
            get(viewId).layout.widthMin = width;
        }

        /// <summary>
        /// Sets the width of the view as a percentage of the parent.
        /// </summary>
        /// <param name="viewId"> </param>
        /// <param name="percent">
        /// @since 1.1 </param>
        public virtual void constrainPercentWidth(int viewId, float percent)
        {
            get(viewId).layout.widthPercent = percent;
        }

        /// <summary>
        /// Sets the height of the view as a percentage of the parent.
        /// </summary>
        /// <param name="viewId"> </param>
        /// <param name="percent">
        /// @since 1.1 </param>
        public virtual void constrainPercentHeight(int viewId, float percent)
        {
            get(viewId).layout.heightPercent = percent;
        }

        /// <summary>
        /// Sets how the height is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
        /// Default is spread.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its matchConstraintDefaultHeight </param>
        /// <param name="height"> MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD
        /// @since 1.1 </param>
        public virtual void constrainDefaultHeight(int viewId, int height)
        {
            get(viewId).layout.heightDefault = height;
        }

        /// <summary>
        /// Sets how the width is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
        /// Default is spread.
        /// </summary>
        /// <param name="viewId">      ID of view to adjust its matchConstraintDefaultWidth </param>
        /// <param name="constrained"> if true with will be constrained
        /// @since 1.1 </param>
        public virtual void constrainedWidth(int viewId, bool constrained)
        {
            get(viewId).layout.constrainedWidth = constrained;
        }

        /// <summary>
        /// Sets how the height is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
        /// Default is spread.
        /// </summary>
        /// <param name="viewId">      ID of view to adjust its matchConstraintDefaultHeight </param>
        /// <param name="constrained"> if true height will be constrained
        /// @since 1.1 </param>
        public virtual void constrainedHeight(int viewId, bool constrained)
        {
            get(viewId).layout.constrainedHeight = constrained;
        }

        /// <summary>
        /// Sets how the width is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
        /// Default is spread.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its matchConstraintDefaultWidth </param>
        /// <param name="width">  SPREAD or WRAP
        /// @since 1.1 </param>
        public virtual void constrainDefaultWidth(int viewId, int width)
        {
            get(viewId).layout.widthDefault = width;
        }

        /// <summary>
        /// The child's weight that we can use to distribute the available horizontal space
        /// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its HorizontalWeight </param>
        /// <param name="weight"> the weight that we can use to distribute the horizontal space </param>
        public virtual void setHorizontalWeight(int viewId, float weight)
        {
            get(viewId).layout.horizontalWeight = weight;
        }

        /// <summary>
        /// The child's weight that we can use to distribute the available vertical space
        /// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its VerticalWeight </param>
        /// <param name="weight"> the weight that we can use to distribute the vertical space </param>
        public virtual void setVerticalWeight(int viewId, float weight)
        {
            get(viewId).layout.verticalWeight = weight;
        }

        /// <summary>
        /// How the elements of the horizontal chain will be positioned. if the dimension
        /// behaviour is set to MATCH_CONSTRAINT. The possible values are:
        /// 
        /// <ul>
        /// <li><seealso cref="#CHAIN_SPREAD"/> -- the elements will be spread out</li>
        /// <li><seealso cref="#CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
        /// be spread out</li>
        /// <li><seealso cref="#CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
        /// horizontal bias attribute of the child will then affect the positioning of the packed
        /// elements</li>
        /// </ul>
        /// </summary>
        /// <param name="viewId">     ID of view to adjust its HorizontalChainStyle </param>
        /// <param name="chainStyle"> the weight that we can use to distribute the horizontal space </param>
        public virtual void setHorizontalChainStyle(int viewId, int chainStyle)
        {
            get(viewId).layout.horizontalChainStyle = chainStyle;
        }

        /// <summary>
        /// How the elements of the vertical chain will be positioned. in a chain, if the dimension
        /// behaviour is set to MATCH_CONSTRAINT
        /// 
        /// <ul>
        /// <li><seealso cref="#CHAIN_SPREAD"/> -- the elements will be spread out</li>
        /// <li><seealso cref="#CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
        /// be spread out</li>
        /// <li><seealso cref="#CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
        /// vertical bias attribute of the child will then affect the positioning of the packed
        /// elements</li>
        /// </ul>
        /// </summary>
        /// <param name="viewId">     ID of view to adjust its VerticalChainStyle </param>
        /// <param name="chainStyle"> the weight that we can use to distribute the horizontal space </param>
        public virtual void setVerticalChainStyle(int viewId, int chainStyle)
        {
            get(viewId).layout.verticalChainStyle = chainStyle;
        }

        /// <summary>
        /// Adds a view to a horizontal chain.
        /// </summary>
        /// <param name="viewId">  view to add </param>
        /// <param name="leftId">  view in chain to the left </param>
        /// <param name="rightId"> view in chain to the right </param>
        public virtual void addToHorizontalChain(int viewId, int leftId, int rightId)
        {
            Connect(viewId, LEFT, leftId, (leftId == PARENT_ID) ? LEFT : RIGHT, 0);
            Connect(viewId, RIGHT, rightId, (rightId == PARENT_ID) ? RIGHT : LEFT, 0);
            if (leftId != PARENT_ID)
            {
                Connect(leftId, RIGHT, viewId, LEFT, 0);
            }
            if (rightId != PARENT_ID)
            {
                Connect(rightId, LEFT, viewId, RIGHT, 0);
            }
        }

        /// <summary>
        /// Adds a view to a horizontal chain.
        /// </summary>
        /// <param name="viewId">  view to add </param>
        /// <param name="leftId">  view to the start side </param>
        /// <param name="rightId"> view to the end side </param>
        public virtual void addToHorizontalChainRTL(int viewId, int leftId, int rightId)
        {
            Connect(viewId, START, leftId, (leftId == PARENT_ID) ? START : END, 0);
            Connect(viewId, END, rightId, (rightId == PARENT_ID) ? END : START, 0);
            if (leftId != PARENT_ID)
            {
                Connect(leftId, END, viewId, START, 0);
            }
            if (rightId != PARENT_ID)
            {
                Connect(rightId, START, viewId, END, 0);
            }
        }

        /// <summary>
        /// Adds a view to a vertical chain.
        /// </summary>
        /// <param name="viewId">   view to add to a vertical chain </param>
        /// <param name="topId">    view above. </param>
        /// <param name="bottomId"> view below </param>
        public virtual void addToVerticalChain(int viewId, int topId, int bottomId)
        {
            Connect(viewId, TOP, topId, (topId == PARENT_ID) ? TOP : BOTTOM, 0);
            Connect(viewId, BOTTOM, bottomId, (bottomId == PARENT_ID) ? BOTTOM : TOP, 0);
            if (topId != PARENT_ID)
            {
                Connect(topId, BOTTOM, viewId, TOP, 0);
            }
            if (bottomId != PARENT_ID)
            {
                Connect(bottomId, TOP, viewId, BOTTOM, 0);
            }
        }

        /// <summary>
        /// Removes a view from a vertical chain.
        /// This assumes the view is connected to a vertical chain.
        /// Its behaviour is undefined if not part of a vertical chain.
        /// </summary>
        /// <param name="viewId"> the view to be removed </param>
        public virtual void removeFromVerticalChain(int viewId)
        {
            if (mConstraints.ContainsKey(viewId))
            {
                Constraint constraint = mConstraints[viewId];
                if (constraint == null)
                {
                    return;
                }
                int topId = constraint.layout.topToBottom;
                int bottomId = constraint.layout.bottomToTop;
                if (topId != Layout.UNSET || bottomId != Layout.UNSET)
                {
                    if (topId != Layout.UNSET && bottomId != Layout.UNSET)
                    {
                        // top and bottom connected to views
                        Connect(topId, BOTTOM, bottomId, TOP, 0);
                        Connect(bottomId, TOP, topId, BOTTOM, 0);
                    }
                    else if (constraint.layout.bottomToBottom != Layout.UNSET)
                    {
                        // top connected to view. Bottom connected to parent
                        Connect(topId, BOTTOM, constraint.layout.bottomToBottom, BOTTOM, 0);
                    }
                    else if (constraint.layout.topToTop != Layout.UNSET)
                    {
                        // bottom connected to view. Top connected to parent
                        Connect(bottomId, TOP, constraint.layout.topToTop, TOP, 0);
                    }
                }
            }
            Clear(viewId, TOP);
            Clear(viewId, BOTTOM);
        }

        /// <summary>
        /// Removes a view from a horizontal chain.
        /// This assumes the view is connected to a horizontal chain.
        /// Its behaviour is undefined if not part of a horizontal chain.
        /// </summary>
        /// <param name="viewId"> the view to be removed </param>
        public virtual void removeFromHorizontalChain(int viewId)
        {
            if (mConstraints.ContainsKey(viewId))
            {
                Constraint constraint = mConstraints[viewId];
                if (constraint == null)
                {
                    return;
                }
                int leftId = constraint.layout.leftToRight;
                int rightId = constraint.layout.rightToLeft;
                if (leftId != Layout.UNSET || rightId != Layout.UNSET)
                {
                    if (leftId != Layout.UNSET && rightId != Layout.UNSET)
                    {
                        // left and right connected to views
                        Connect(leftId, RIGHT, rightId, LEFT, 0);
                        Connect(rightId, LEFT, leftId, RIGHT, 0);
                    }
                    else if (constraint.layout.rightToRight != Layout.UNSET)
                    {
                        // left connected to view. right connected to parent
                        Connect(leftId, RIGHT, constraint.layout.rightToRight, RIGHT, 0);
                    }
                    else if (constraint.layout.leftToLeft != Layout.UNSET)
                    {
                        // right connected to view. left connected to parent
                        Connect(rightId, LEFT, constraint.layout.leftToLeft, LEFT, 0);
                    }
                    Clear(viewId, LEFT);
                    Clear(viewId, RIGHT);
                }
                else
                {

                    int startId = constraint.layout.startToEnd;
                    int endId = constraint.layout.endToStart;
                    if (startId != Layout.UNSET || endId != Layout.UNSET)
                    {
                        if (startId != Layout.UNSET && endId != Layout.UNSET)
                        {
                            // start and end connected to views
                            Connect(startId, END, endId, START, 0);
                            Connect(endId, START, leftId, END, 0);
                        }
                        else if (endId != Layout.UNSET)
                        {
                            if (constraint.layout.rightToRight != Layout.UNSET)
                            {
                                // left connected to view. right connected to parent
                                Connect(leftId, END, constraint.layout.rightToRight, END, 0);
                            }
                            else if (constraint.layout.leftToLeft != Layout.UNSET)
                            {
                                // right connected to view. left connected to parent
                                Connect(endId, START, constraint.layout.leftToLeft, START, 0);
                            }
                        }
                    }
                    Clear(viewId, START);
                    Clear(viewId, END);
                }
            }
        }

        /// <summary>
        /// Creates a ConstraintLayout virtual object. Currently only horizontal or vertical GuideLines.
        /// </summary>
        /// <param name="guidelineID"> ID of guideline to create </param>
        /// <param name="orientation"> the Orientation of the guideline </param>
        public virtual void create(int guidelineID, int orientation)
        {
            Constraint constraint = get(guidelineID);
            constraint.layout.mIsGuideline = true;
            constraint.layout.orientation = orientation;
        }

        /// <summary>
        /// Creates a ConstraintLayout Barrier object.
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="direction">  Barrier.{LEFT,RIGHT,TOP,BOTTOM,START,END} </param>
        /// <param name="referenced">
        /// @since 1.1 </param>
        public virtual void createBarrier(int id, int direction, int margin, params int[] referenced)
        {
            Constraint constraint = get(id);
            constraint.layout.mHelperType = BARRIER_TYPE;
            constraint.layout.mBarrierDirection = direction;
            constraint.layout.mBarrierMargin = margin;
            constraint.layout.mIsGuideline = false;
            constraint.layout.mReferenceIds = referenced;
        }

        /// <summary>
        /// Set the guideline's distance form the top or left edge.
        /// </summary>
        /// <param name="guidelineID"> ID of the guideline </param>
        /// <param name="margin">      the distance to the top or left edge </param>
        public virtual void setGuidelineBegin(int guidelineID, int margin)
        {
            get(guidelineID).layout.guideBegin = margin;
            get(guidelineID).layout.guideEnd = Layout.UNSET;
            get(guidelineID).layout.guidePercent = Layout.UNSET;

        }

        /// <summary>
        /// Set a guideline's distance to end.
        /// </summary>
        /// <param name="guidelineID"> ID of the guideline </param>
        /// <param name="margin">      the margin to the right or bottom side of container </param>
        public virtual void setGuidelineEnd(int guidelineID, int margin)
        {
            get(guidelineID).layout.guideEnd = margin;
            get(guidelineID).layout.guideBegin = Layout.UNSET;
            get(guidelineID).layout.guidePercent = Layout.UNSET;
        }

        /// <summary>
        /// Set a Guideline's percent.
        /// </summary>
        /// <param name="guidelineID"> ID of the guideline </param>
        /// <param name="ratio">       the ratio between the gap on the left and right 0.0 is top/left 0.5 is middle </param>
        public virtual void setGuidelinePercent(int guidelineID, float ratio)
        {
            get(guidelineID).layout.guidePercent = ratio;
            get(guidelineID).layout.guideEnd = Layout.UNSET;
            get(guidelineID).layout.guideBegin = Layout.UNSET;
        }

        /// <summary>
        /// get the reference id's of a helper.
        /// </summary>
        /// <param name="id"> </param>
        /// <returns> array of id's </returns>
        public virtual int[] getReferencedIds(int id)
        {
            Constraint constraint = get(id);
            if (constraint.layout.mReferenceIds == null)
            {
                return new int[0];
            }
            //return Arrays.copyOf(constraint.layout.mReferenceIds, constraint.layout.mReferenceIds.Length);
            return ArrayHelperClass.Copy(constraint.layout.mReferenceIds, constraint.layout.mReferenceIds.Length);
        }

        /// <summary>
        /// sets the reference id's of a barrier.
        /// </summary>
        /// <param name="id"> </param>
        /// <param name="referenced">
        /// @since 2.0 </param>
        public virtual void setReferencedIds(int id, params int[] referenced)
        {
            Constraint constraint = get(id);
            constraint.layout.mReferenceIds = referenced;
        }

        public virtual void setBarrierType(int id, int type)
        {
            Constraint constraint = get(id);
            constraint.layout.mHelperType = type;
        }

        private Constraint get(int id)
        {
            if (!mConstraints.ContainsKey(id))
            {
                mConstraints[id] = new Constraint();
            }
            return mConstraints[id];
        }

        private string sideToString(int side)
        {
            switch (side)
            {
                case LEFT:
                    return "left";
                case RIGHT:
                    return "right";
                case TOP:
                    return "top";
                case BOTTOM:
                    return "bottom";
                case BASELINE:
                    return "baseline";
                case START:
                    return "start";
                case END:
                    return "end";
            }
            return "undefined";
        }

        private static void setDeltaValue(Constraint c, int type, float value)
        {
            switch (type)
            {
                case GUIDE_PERCENT:
                    c.layout.guidePercent = value;
                    break;
                case CIRCLE_ANGLE:
                    c.layout.circleAngle = value;
                    break;
                case HORIZONTAL_BIAS:
                    c.layout.horizontalBias = value;
                    break;
                case VERTICAL_BIAS:
                    c.layout.verticalBias = value;
                    break;
                case ALPHA:
                    c.propertySet.alpha = value;
                    break;
                case ELEVATION:
                    c.transform.elevation = value;
                    c.transform.applyElevation = true;
                    break;
                case ROTATION:
                    c.transform.rotation = value;
                    break;
                case ROTATION_X:
                    c.transform.rotationX = value;
                    break;
                case ROTATION_Y:
                    c.transform.rotationY = value;
                    break;
                case SCALE_X:
                    c.transform.scaleX = value;
                    break;
                case SCALE_Y:
                    c.transform.scaleY = value;
                    break;
                case TRANSFORM_PIVOT_X:
                    c.transform.transformPivotX = value;
                    break;
                case TRANSFORM_PIVOT_Y:
                    c.transform.transformPivotY = value;
                    break;
                case TRANSLATION_X:
                    c.transform.translationX = value;
                    break;
                case TRANSLATION_Y:
                    c.transform.translationY = value;
                    break;
                case TRANSLATION_Z:
                    c.transform.translationZ = value;
                    break;
                case VERTICAL_WEIGHT:
                    c.layout.verticalWeight = value;
                    break;
                case HORIZONTAL_WEIGHT:
                    c.layout.horizontalWeight = value;
                    break;
                case WIDTH_PERCENT:
                    c.layout.widthPercent = value;
                    break;
                case HEIGHT_PERCENT:
                    c.layout.heightPercent = value;
                    break;
                case PROGRESS:
                    c.propertySet.mProgress = value;
                    break;
                case TRANSITION_PATH_ROTATE:
                    //c.motion.mPathRotate = value;
                    break;
                case MOTION_STAGGER:
                    //c.motion.mMotionStagger = value;
                    break;
                case QUANTIZE_MOTION_PHASE:
                    //c.motion.mQuantizeMotionPhase = value;
                    break;
                case UNUSED:
                    break;
                default:
                    Debug.WriteLine(TAG, "Unknown attribute 0x");
                    break;
            }
        }

        private static void setDeltaValue(Constraint c, int type, int value)
        {
            switch (type)
            {
                case EDITOR_ABSOLUTE_X:
                    c.layout.editorAbsoluteX = value;
                    break;
                case EDITOR_ABSOLUTE_Y:
                    c.layout.editorAbsoluteY = value;
                    break;
                case LAYOUT_WRAP_BEHAVIOR:
                    c.layout.mWrapBehavior = value;
                    break;
                case GUIDE_BEGIN:
                    c.layout.guideBegin = value;
                    break;
                case GUIDE_END:
                    c.layout.guideEnd = value;
                    break;
                case ORIENTATION:
                    c.layout.orientation = value;
                    break;
                case CIRCLE:
                    c.layout.circleConstraint = value;
                    break;
                case CIRCLE_RADIUS:
                    c.layout.circleRadius = value;
                    break;
                case GONE_LEFT_MARGIN:
                    c.layout.goneLeftMargin = value;
                    break;
                case GONE_TOP_MARGIN:
                    c.layout.goneTopMargin = value;
                    break;
                case GONE_RIGHT_MARGIN:
                    c.layout.goneRightMargin = value;
                    break;
                case GONE_BOTTOM_MARGIN:
                    c.layout.goneBottomMargin = value;
                    break;
                case GONE_START_MARGIN:
                    c.layout.goneStartMargin = value;
                    break;
                case GONE_END_MARGIN:
                    c.layout.goneEndMargin = value;
                    break;
                case GONE_BASELINE_MARGIN:
                    c.layout.goneBaselineMargin = value;
                    break;
                case LEFT_MARGIN:
                    c.layout.leftMargin = value;
                    break;
                case RIGHT_MARGIN:
                    c.layout.rightMargin = value;
                    break;
                case START_MARGIN:
                    c.layout.startMargin = value;
                    break;
                case END_MARGIN:
                    c.layout.endMargin = value;
                    break;
                case TOP_MARGIN:
                    c.layout.topMargin = value;
                    break;
                case BOTTOM_MARGIN:
                    c.layout.bottomMargin = value;
                    break;
                case BASELINE_MARGIN:
                    c.layout.baselineMargin = value;
                    break;
                case LAYOUT_WIDTH:
                    c.layout.mWidth = value;
                    break;
                case LAYOUT_HEIGHT:
                    c.layout.mHeight = value;
                    break;
                case WIDTH_DEFAULT:
                    c.layout.widthDefault = value;
                    break;
                case HEIGHT_DEFAULT:
                    c.layout.heightDefault = value;
                    break;
                case HEIGHT_MAX:
                    c.layout.heightMax = value;
                    break;
                case WIDTH_MAX:
                    c.layout.widthMax = value;
                    break;
                case HEIGHT_MIN:
                    c.layout.heightMin = value;
                    break;
                case WIDTH_MIN:
                    c.layout.widthMin = value;
                    break;
                case LAYOUT_VISIBILITY:
                    c.propertySet.visibility = value;
                    break;
                case VISIBILITY_MODE:
                    c.propertySet.mVisibilityMode = value;
                    break;
                case TRANSFORM_PIVOT_TARGET:
                    c.transform.transformPivotTarget = value;
                    break;
                case VERTICAL_STYLE:
                    c.layout.verticalChainStyle = value;
                    break;
                case HORIZONTAL_STYLE:
                    c.layout.horizontalChainStyle = value;
                    break;
                case VIEW_ID:
                    c.mViewId = value;
                    break;
                case ANIMATE_RELATIVE_TO:
                    //c.motion.mAnimateRelativeTo = value;
                    break;
                case ANIMATE_CIRCLE_ANGLE_TO:
                    //c.motion.mAnimateCircleAngleTo = value;
                    break;
                case PATH_MOTION_ARC:
                    //c.motion.mPathMotionArc = value;
                    break;
                case QUANTIZE_MOTION_STEPS:
                    //c.motion.mQuantizeMotionSteps = value;
                    break;
                case QUANTIZE_MOTION_INTERPOLATOR_TYPE:
                    //c.motion.mQuantizeInterpolatorType = value;
                    break;
                case QUANTIZE_MOTION_INTERPOLATOR_ID:
                    //c.motion.mQuantizeInterpolatorID = value;
                    break;
                case DRAW_PATH:
                    //c.motion.mDrawPath = value;
                    break;
                case BARRIER_DIRECTION:
                    c.layout.mBarrierDirection = value;
                    break;
                case BARRIER_MARGIN:
                    c.layout.mBarrierMargin = value;
                    break;
                case UNUSED:
                    break;
                default:
                    Debug.WriteLine(TAG, "Unknown attribute 0x");
                    break;
            }
        }

        private static void setDeltaValue(Constraint c, int type, string value)
        {
            switch (type)
            {
                case DIMENSION_RATIO:
                    c.layout.dimensionRatio = value;
                    break;
                case TRANSITION_EASING:
                    //c.motion.mTransitionEasing = value;
                    break;
                case QUANTIZE_MOTION_INTERPOLATOR_STR:
                    //c.motion.mQuantizeInterpolatorString = value;
                    break;
                case CONSTRAINT_REFERENCED_IDS:
                    c.layout.mReferenceIdString = value;
                    // If a string is defined, clear up the reference ids array
                    c.layout.mReferenceIds = null;
                    break;
                case CONSTRAINT_TAG:
                    c.layout.mConstraintTag = value;
                    break;
                case UNUSED:
                    break;
                default:
                    Debug.WriteLine(TAG, "Unknown attribute 0x");
                    break;
            }
        }

        private static void setDeltaValue(Constraint c, int type, bool value)
        {
            switch (type)
            {
                case CONSTRAINED_WIDTH:
                    c.layout.constrainedWidth = value;
                    break;
                case CONSTRAINED_HEIGHT:
                    c.layout.constrainedHeight = value;
                    break;
                case ELEVATION:
                    c.transform.applyElevation = value;
                    break;
                case BARRIER_ALLOWS_GONE_WIDGETS:
                    c.layout.mBarrierAllowsGoneWidgets = value;
                    break;
                case UNUSED:
                    break;
                default:
                    Debug.WriteLine(TAG, "Unknown attribute 0x");
                    break;
            }
        }

        public virtual Constraint getConstraint(int id)
        {
            if (mConstraints.ContainsKey(id))
            {
                return mConstraints[id];
            }
            return null;
        }
    }
}
