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
using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS
using View = Microsoft.UI.Xaml.UIElement;
#elif __IOS__
using View = UIKit.UIView;
#endif
namespace SharpConstraintLayout.Maui.Pure.Core
{
    public partial class ConstraintSet
    {
        public virtual Constraint getParameters(int mId)
        {
            return get(mId);
        }

        /// <summary>
        /// 该方案是和Android的思路一样,在ConstraintLayout中存储一份不变的Constraints去替代LayoutParams,
        /// LayoutParams是不变的,因此模仿Android思路Constraints也是不变的字典,不在字典中创建新的Constraint替换,只是改原有的属性
        /// </summary>
        /// <param name="constraintLayout"></param>
        /// <exception cref="Exception"></exception>
        public virtual void Clone(ConstraintLayout constraintLayout)
        {
            int count = constraintLayout.ChildCount;
            mConstraints.Clear();
            for (int i = 0; i < count; i++)
            {
#if WINDOWS
                View view = constraintLayout.Children[i];
#elif __IOS__
                View view = constraintLayout.Subviews[i];
#endif
                //ConstraintLayout.LayoutParams param = (ConstraintLayout.LayoutParams)view.LayoutParams;
                int id = view.GetHashCode();
                var param = constraintLayout.ConstraintSet.Constraints[id];//获取旧的constraint

                if (mForceId && id == -1)
                {
                    throw new Exception("All children of ConstraintLayout must have ids to use ConstraintSet");
                }
                if (!mConstraints.ContainsKey(id))
                {
                    //mConstraints[id] = new Constraint();
                    mConstraints.Add(id, param.clone());
                }

                Constraint constraint = mConstraints[id];
                //我认为这些多余,直接clone constraint就够了
                /*
                if (constraint == null)
                {
                    continue;
                }
                //constraint.mCustomConstraints = ConstraintAttribute.extractAttributes(mSavedAttributes, view);
                constraint.fillFrom(id, param.layout);
                //constraint.propertySet.visibility = view.Visibility;//从View获取visibility不对
                constraint.propertySet.visibility = param.propertySet.visibility;
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                {
                    constraint.propertySet.alpha = view.Alpha;
                    constraint.transform.rotation = view.Rotation;
                    constraint.transform.rotationX = view.RotationX;
                    constraint.transform.rotationY = view.RotationY;
                    constraint.transform.scaleX = view.ScaleX;
                    constraint.transform.scaleY = view.ScaleY;

                    float pivotX = view.PivotX; // we assume it is not set if set to 0.0
                    float pivotY = view.PivotY; // we assume it is not set if set to 0.0

                    if (pivotX != 0.0 || pivotY != 0.0)
                    {
                        constraint.transform.transformPivotX = pivotX;
                        constraint.transform.transformPivotY = pivotY;
                    }

                    constraint.transform.translationX = view.TranslationX;
                    constraint.transform.translationY = view.TranslationY;
                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
                    {
                        constraint.transform.translationZ = view.TranslationZ;
                        if (constraint.transform.applyElevation)
                        {
                            constraint.transform.elevation = view.Elevation;
                        }
                    }
                }*/
                if (view is Barrier)
                {
                    Barrier barrier = ((Barrier)view);
                    constraint.layout.mBarrierAllowsGoneWidgets = barrier.AllowsGoneWidget;
                    constraint.layout.mReferenceIds = barrier.ReferencedIds;
                    constraint.layout.mBarrierDirection = barrier.Type;
                    constraint.layout.mBarrierMargin = barrier.Margin;
                }
            }
        }


        /// <summary>
        /// Used to set constraints when used by constraint layout.
        /// 再Clone后新的ConstraintSet有新的约束字典,把这些约束更新到旧字典上去.
        /// 注意:这里还用到了统一替换PARENT_ID
        /// </summary>
        public virtual void ApplyTo(ConstraintLayout constraintLayout)
        {
            int parentID = constraintLayout.GetHashCode();

            int count = constraintLayout.ChildCount;
            List<int> used = mConstraints.Keys.ToList();//已经设置了约束的id
            for (int i = 0; i < count; i++)//查看layout的child
            {
#if WINDOWS
                View view = constraintLayout.Children[i];
#elif __IOS__
                View view = constraintLayout.Subviews[i];
#endif
                int id = view.GetHashCode();
                if (!mConstraints.ContainsKey(id))
                {
                    Debug.WriteLine(TAG, $"id unknown {view}");
                    continue;
                }

                if (mForceId && id == -1)
                {
                    throw new Exception("All children of ConstraintLayout must have ids to use ConstraintSet");
                }
                if (id == -1)
                {
                    continue;
                }

                if (mConstraints.ContainsKey(id))
                {
                    used.Remove(id);
                    Constraint constraint = mConstraints[id];
                    if (constraint == null)
                    {
                        continue;
                    }
                    if (view is Barrier)
                    {
                        constraint.layout.mHelperType = BARRIER_TYPE;
                        Barrier barrier = (Barrier)view;
                        //barrier.Id = id;
                        barrier.Type = constraint.layout.mBarrierDirection;
                        barrier.Margin = constraint.layout.mBarrierMargin;

                        barrier.AllowsGoneWidget = constraint.layout.mBarrierAllowsGoneWidgets;
                        if (constraint.layout.mReferenceIds != null)
                        {
                            barrier.ReferencedIds = constraint.layout.mReferenceIds;
                        }
                    }

                    //将新的约束更新到ConstraintLayout的ConstraintSet中
                    var param = constraintLayout.ConstraintSet.Constraints[id];
                    param.layout.validate();
                    constraint.applyTo(param.layout);

                    if (view is Guideline)//不像Android一样用户提供指定约束是isGuideline就创建Guideline,必须通过Guideline控件实现
                    {
                        if (constraint.layout.orientation != UNSET)//如果用户设置了新的orientation
                        {
                            //Orientation在这里设置的原因是在Measure中当约束update到widget时,其他的widget需要对齐Guideline的widget,如果guideline没有设置orientation,那么widget找不到对应的边的Anchor,因为Guideline需要方向才有正确的Anchor
                            (view as Guideline).Orientation = constraint.layout.orientation;
                        }
                    }

                    /*//if (applyPostLayout)
                    if (true)
                    {
                        ConstraintAttribute.setAttributes(view, constraint.mCustomConstraints);
                    }
                    view.LayoutParams = param;
                    if (constraint.propertySet.mVisibilityMode == VISIBILITY_MODE_NORMAL)
                    {
                        view.Visibility = constraint.propertySet.visibility;
                    }
                    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                    {
                        view.Alpha = constraint.propertySet.alpha;
                        view.Rotation = constraint.transform.rotation;
                        view.RotationX = constraint.transform.rotationX;
                        view.RotationY = constraint.transform.rotationY;
                        view.ScaleX = constraint.transform.scaleX;
                        view.ScaleY = constraint.transform.scaleY;
                        if (constraint.transform.transformPivotTarget != UNSET)
                        {
                            View layout = (View)view.Parent;
                            View center = layout.findViewById(constraint.transform.transformPivotTarget);
                            if (center != null)
                            {
                                float cy = (center.Top + center.Bottom) / 2.0f;
                                float cx = (center.Left + center.Right) / 2.0f;
                                if (view.Right - view.Left > 0 && view.Bottom - view.Top > 0)
                                {
                                    float px = (cx - view.Left);
                                    float py = (cy - view.Top);
                                    view.PivotX = px;
                                    view.PivotY = py;
                                }
                            }
                        }
                        else
                        {
                            if (!float.IsNaN(constraint.transform.transformPivotX))
                            {
                                view.PivotX = constraint.transform.transformPivotX;
                            }
                            if (!float.IsNaN(constraint.transform.transformPivotY))
                            {
                                view.PivotY = constraint.transform.transformPivotY;
                            }
                        }
                        view.TranslationX = constraint.transform.translationX;
                        view.TranslationY = constraint.transform.translationY;
                        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
                        {
                            view.TranslationZ = constraint.transform.translationZ;
                            if (constraint.transform.applyElevation)
                            {
                                view.Elevation = constraint.transform.elevation;
                            }
                        }
                    }*/
                }
                else
                {
                    Debug.WriteLine(TAG, "WARNING NO CONSTRAINTS for view " + id);
                }
            }

            foreach (int id in used)//剩下的约束不在ConstraintLayout的Children中,找出特殊的,如Barrier,Guideline,但是这里我们不弄,因为添加新的View有新的Id,那么就需要更新全部的约束,费时
            {
                if (id == ConstraintSet.PARENT_ID || id == constraintLayout.GetHashCode())//还要处理下layout
                    mConstraints[id].applyTo(constraintLayout.ConstraintSet.Constraints[id].layout);
                else
                    throw new NotImplementedException("如果还有约束身下而且是重要的,那么需要新建View插入原来的布局中,那么但是id怎么办,新建的有新的哈希,而约束是相对旧的哈希.");
                /*Constraint constraint = mConstraints[id];
                if (constraint == null)
                {
                    continue;
                }
                if (constraint.layout.mHelperType == BARRIER_TYPE)
                {
                    Barrier barrier = new Barrier();
                    barrier.Id = id;
                    if (constraint.layout.mReferenceIds != null)
                    {
                        barrier.ReferencedIds = constraint.layout.mReferenceIds;
                    }
                    else if (!string.ReferenceEquals(constraint.layout.mReferenceIdString, null))
                    {
                        constraint.layout.mReferenceIds = convertReferenceString(barrier, constraint.layout.mReferenceIdString);
                        barrier.ReferencedIds = constraint.layout.mReferenceIds;
                    }
                    barrier.Type = constraint.layout.mBarrierDirection;
                    barrier.Margin = constraint.layout.mBarrierMargin;
                    LayoutParams param = constraintLayout.generateDefaultLayoutParams();
                    barrier.validateParams();
                    constraint.applyTo(param);
                    constraintLayout.addView(barrier, param);
                }
                if (constraint.layout.mIsGuideline)
                {
                    Guideline g = new Guideline(constraintLayout.Context);
                    g.Id = id;
                    ConstraintLayout.LayoutParams param = constraintLayout.generateDefaultLayoutParams();
                    constraint.applyTo(param);
                    constraintLayout.addView(g, param);
                }*/
            }
            for (int i = 0; i < count; i++)
            {
#if WINDOWS
                View view = constraintLayout.Children[i];
#elif __IOS__
                View view = constraintLayout.Subviews[i];
#endif
                if (view is ConstraintHelper)
                {
                    ConstraintHelper constraintHelper = (ConstraintHelper)view;
                    constraintHelper.applyLayoutFeaturesInConstraintSet(constraintLayout);
                }
            }

            constraintLayout.ConstraintSet.IsChanged = true;
#if WINDOWS
            constraintLayout.InvalidateMeasure();
            constraintLayout.UpdateLayout();
#elif __IOS__
            constraintLayout.LayoutIfNeeded();
#endif
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
        /// Adjust the visibility of a view.<see cref="GONE"/> or <see cref="INVISIBLE"/> or <see cref="VISIBLE"/>
        /// </summary>
        /// <param name="viewId">     ID of view to adjust the vertical </param>
        /// <param name="visibility"> the visibility </param>
        public virtual void setVisibility(int viewId, int visibility)
        {
            get(viewId).propertySet.visibility = visibility;
        }

        /// <summary>
        /// ConstraintSet will not setVisibility. <seealso cref="VISIBILITY_MODE_IGNORE"/> or <see cref="VISIBILITY_MODE_NORMAL"/>
        /// </summary>
        /// <param name="viewId">         ID of view </param>
        /// <param name="visibilityMode"> </param>
        public virtual void setVisibilityMode(int viewId, int visibilityMode)
        {
            get(viewId).propertySet.mVisibilityMode = visibilityMode;
        }

        /// <summary>
        /// ConstraintSet will not setVisibility. <seealso cref="VISIBILITY_MODE_IGNORE"/> or <see cref="VISIBILITY_MODE_NORMAL"/>
        /// </summary>
        /// <param name="viewId"> ID of view </param>
        public virtual int getVisibilityMode(int viewId)
        {
            return get(viewId).propertySet.mVisibilityMode;
        }

        /// <summary>
        /// Get the visibility flag set in this view.<see cref="GONE"/> or <see cref="INVISIBLE"/> or <see cref="VISIBLE"/>
        /// </summary>
        /// <param name="viewId"> the id of the view </param>
        /// <returns> the visibility constraint for the view </returns>
        public virtual int getVisibility(int viewId)
        {
            return get(viewId).propertySet.visibility;
        }

        /// <summary>
        /// Get the height set in the view.
        /// It can be number, or <see cref="MATCH_CONSTRAINT"/>,or <see cref="MATCH_PARENT"/>,or <see cref="WRAP_CONTENT"/>
        /// </summary>
        /// <param name="viewId"> the id of the view </param>
        /// <returns> return the height constraint of the view </returns>
        public virtual int getHeight(int viewId)
        {
            return get(viewId).layout.mHeight;
        }

        /// <summary>
        /// Get the width set in the view.
        /// It can be number, or <see cref="MATCH_CONSTRAINT"/>,or <see cref="MATCH_PARENT"/>,or <see cref="WRAP_CONTENT"/>
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
            if (behavior >= 0 && behavior <= ConstraintWidget.WRAP_BEHAVIOR_SKIPPED)
            {
                get(viewId).layout.wrapBehaviorInParent = behavior;
            }
        }

        /// <summary>
        /// Sets the height of the view. It can be a dimension, <seealso cref="WRAP_CONTENT"/> or <see cref="MATCH_CONSTRAINT"/>
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its height </param>
        /// <param name="height"> the height of the view
        /// @since 1.1 </param>
        public virtual void ConstrainHeight(int viewId, int height)
        {
            get(viewId).layout.mHeight = height;
        }

        /// <summary>
        /// Sets the width of the view. It can be a dimension, <seealso cref="#WRAP_CONTENT"/> or <seealso cref="WRAP_CONTENT"/> or <see cref="MATCH_CONSTRAINT"/>
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its width </param>
        /// <param name="width">  the width of the view
        /// @since 1.1 </param>
        public virtual void ConstrainWidth(int viewId, int width)
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
        /// <see cref="MATCH_CONSTRAINT"/>.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust it height </param>
        /// <param name="height"> the maximum height of the constraint
        /// @since 1.1 </param>
        public virtual void constrainMaxHeight(int viewId, int height)
        {
            get(viewId).layout.matchConstraintMaxHeight = height;
        }

        /// <summary>
        /// Sets the maximum width of the view. It is a dimension, It is only applicable if width is
        /// <see cref="MATCH_CONSTRAINT"/>.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its max height </param>
        /// <param name="width">  the maximum width of the view
        /// @since 1.1 </param>
        public virtual void constrainMaxWidth(int viewId, int width)
        {
            get(viewId).layout.matchConstraintMaxWidth = width;
        }

        /// <summary>
        /// Sets the height of the view. It is a dimension, It is only applicable if height is
        /// <see cref="MATCH_CONSTRAINT"/>.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its min height </param>
        /// <param name="height"> the minimum height of the view
        /// @since 1.1 </param>
        public virtual void constrainMinHeight(int viewId, int height)
        {
            get(viewId).layout.matchConstraintMinHeight = height;
        }

        /// <summary>
        /// Sets the width of the view.  It is a dimension, It is only applicable if width is
        /// <see cref="MATCH_CONSTRAINT"/>.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its min height </param>
        /// <param name="width">  the minimum width of the view
        /// @since 1.1 </param>
        public virtual void constrainMinWidth(int viewId, int width)
        {
            get(viewId).layout.matchConstraintMinWidth = width;
        }

        /// <summary>
        /// Sets the width of the view as a percentage of the parent.
        /// </summary>
        /// <param name="viewId"> </param>
        /// <param name="percent">
        /// @since 1.1 </param>
        public virtual void constrainPercentWidth(int viewId, float percent)
        {
            get(viewId).layout.matchConstraintPercentWidth = percent;
        }

        /// <summary>
        /// Sets the height of the view as a percentage of the parent.
        /// </summary>
        /// <param name="viewId"> </param>
        /// <param name="percent">
        /// @since 1.1 </param>
        public virtual void constrainPercentHeight(int viewId, float percent)
        {
            get(viewId).layout.matchConstraintPercentHeight = percent;
        }

        /// <summary>
        /// Sets how the height is calculated ether <see cref="MATCH_CONSTRAINT_WRAP"/> or <see cref="MATCH_CONSTRAINT_SPREAD"/>.
        /// Default is spread.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its matchConstraintDefaultHeight </param>
        /// <param name="height"> MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD
        /// @since 1.1 </param>
        public virtual void constrainDefaultHeight(int viewId, int height)
        {
            get(viewId).layout.matchConstraintDefaultHeight = height;
        }

        /// <summary>
        /// Sets how the width is calculated ether <see cref="MATCH_CONSTRAINT_WRAP"/> or <see cref="MATCH_CONSTRAINT_SPREAD"/>.
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
        /// Sets how the height is calculated ether <see cref="MATCH_CONSTRAINT_WRAP"/> or <see cref="MATCH_CONSTRAINT_SPREAD"/>.
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
        /// Sets how the width is calculated ether <see cref="MATCH_CONSTRAINT_WRAP"/> or <see cref="MATCH_CONSTRAINT_SPREAD"/>.
        /// Default is spread.
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its matchConstraintDefaultWidth </param>
        /// <param name="width">  SPREAD or WRAP
        /// @since 1.1 </param>
        public virtual void constrainDefaultWidth(int viewId, int width)
        {
            get(viewId).layout.matchConstraintDefaultWidth = width;
        }

        /// <summary>
        /// The child's weight that we can use to distribute the available horizontal space
        /// in a chain, if the dimension behaviour is set to <see cref="MATCH_CONSTRAINT"/>
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its HorizontalWeight </param>
        /// <param name="weight"> the weight that we can use to distribute the horizontal space </param>
        public virtual void setHorizontalWeight(int viewId, float weight)
        {
            get(viewId).layout.horizontalWeight = weight;
        }

        /// <summary>
        /// The child's weight that we can use to distribute the available vertical space
        /// in a chain, if the dimension behaviour is set to <see cref="MATCH_CONSTRAINT"/>T
        /// </summary>
        /// <param name="viewId"> ID of view to adjust its VerticalWeight </param>
        /// <param name="weight"> the weight that we can use to distribute the vertical space </param>
        public virtual void setVerticalWeight(int viewId, float weight)
        {
            get(viewId).layout.verticalWeight = weight;
        }

        /// <summary>
        /// How the elements of the horizontal chain will be positioned. if the dimension
        /// behaviour is set to <see cref="MATCH_CONSTRAINT"/>. The possible values are:
        /// <br/>
        /// <seealso cref="CHAIN_SPREAD"/> -- the elements will be spread out<br/>
        /// <seealso cref="CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
        /// be spread out <br/>
        /// <seealso cref="CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
        /// horizontal bias attribute of the child will then affect the positioning of the packed
        /// elements<br/>
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
        /// <br/>
        /// <seealso cref="CHAIN_SPREAD"/> -- the elements will be spread out<br/>
        /// <seealso cref="CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
        /// be spread out<br/>
        /// <seealso cref="CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
        /// vertical bias attribute of the child will then affect the positioning of the packed
        /// elements<br/>
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
        /// public virtual void create(int guidelineID, int orientation)
        public virtual void SetGuidelineOrientation(int guidelineID, int orientation)
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
        public virtual void SetGuidelinePercent(int guidelineID, float ratio)
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
                    c.layout.matchConstraintPercentWidth = value;
                    break;
                case HEIGHT_PERCENT:
                    c.layout.matchConstraintPercentHeight = value;
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
                    c.layout.wrapBehaviorInParent = value;
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
                    c.layout.matchConstraintDefaultWidth = value;
                    break;
                case HEIGHT_DEFAULT:
                    c.layout.matchConstraintDefaultHeight = value;
                    break;
                case HEIGHT_MAX:
                    c.layout.matchConstraintMaxHeight = value;
                    break;
                case WIDTH_MAX:
                    c.layout.matchConstraintMaxWidth = value;
                    break;
                case HEIGHT_MIN:
                    c.layout.matchConstraintMinHeight = value;
                    break;
                case WIDTH_MIN:
                    c.layout.matchConstraintMinWidth = value;
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
                    c.layout.constraintTag = value;
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
