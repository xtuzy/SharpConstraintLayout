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
using SharpConstraintLayout.Maui.Widget.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if __MAUI__
using View = Microsoft.Maui.Controls.View;
#elif WINDOWS
using View = Microsoft.UI.Xaml.UIElement;
#elif __IOS__
using View = UIKit.UIView;
#elif __ANDROID__
using View = Android.Views.View;
#endif

namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintSet : IConstraintSet
    {
        public virtual Constraint GetParameters(int mId)
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
                View view = constraintLayout.GetChildAt(i);

                //ConstraintLayout.LayoutParams param = (ConstraintLayout.LayoutParams)view.LayoutParams;
                int id = view.GetId();
                var param = constraintLayout.mConstraintSet.Constraints[id];//获取旧的constraint

                if (mForceId && id == -1)
                {
                    throw new Exception("All children of ConstraintLayout must have ids to use ConstraintSet");
                }
                if (!mConstraints.ContainsKey(id))
                {
                    //mConstraints[id] = new Constraint();
                    mConstraints.Add(id, param.Clone());
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
                    constraint.layout.mBarrierDirection = barrier.ConstrainType;
                    constraint.layout.mBarrierMargin = barrier.ConstrainMargin;
                }
            }
        }

        /// <summary>
        /// Used to set constraints when used by constraint layout.
        /// 再Clone后新的ConstraintSet有新的约束字典,把这些约束更新到旧字典上去. 注意:这里还用到了统一替换PARENT_ID
        /// </summary>
        public virtual void ApplyTo(ConstraintLayout constraintLayout)
        {
            int parentID = constraintLayout.GetId();

            int count = constraintLayout.ChildCount;
            List<int> used = mConstraints.Keys.ToList();//已经设置了约束的id
            if (count != used.Count) Debug.WriteLine("The count of ConstraintLayout children is not equal to temprary constraints list, maybe you not use clone.", TAG);
            for (int i = 0; i < count; i++)//查看layout的child
            {
                View view = constraintLayout.GetChildAt(i);

                int id = view.GetId();
                if (!mConstraints.ContainsKey(id))
                {
                    Debug.WriteLine($"id unknown {view}", TAG);
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
                        barrier.ConstrainType = constraint.layout.mBarrierDirection;
                        barrier.ConstrainMargin = constraint.layout.mBarrierMargin;

                        barrier.AllowsGoneWidget = constraint.layout.mBarrierAllowsGoneWidgets;
                        if (constraint.layout.mReferenceIds != null)
                        {
                            barrier.ReferencedIds = constraint.layout.mReferenceIds;
                        }
                    }

                    //将新的约束更新到ConstraintLayout的ConstraintSet中
                    var param = constraintLayout.mConstraintSet.Constraints[id];
                    param.layout.Validate();
                    constraint.ApplyTo(param.layout);//Android源码ApplyTo是应用到Params,Params中具有负责View布局的属性,其中Margin,Width,Height是ViewGroup自带的,其他是ConstraintLayout中新增的,也就是说,这里使用ConstraintSet替代Params,需要添加ViewGroup的属性

                    //设置原本在ViewGroup.Params中的属性,这些属性影响Android中View的Measure值,因此我们需要在其他平台单独设置
#if __MAUI__
                    view.SetWidth(constraint.layout.mWidth);
                    view.SetHeight(constraint.layout.mHeight);
                    view.SetMinWidth(constraint.layout.matchConstraintMinWidth);
                    view.SetMinHeight(constraint.layout.matchConstraintMinHeight);
                    view.SetMaxWidth(constraint.layout.matchConstraintMaxWidth);
                    view.SetMaxHeight(constraint.layout.matchConstraintMaxHeight);
#else
                    view.SetSizeAndMargin(constraint.layout.mWidth, constraint.layout.mHeight, constraint.layout.matchConstraintMinWidth, constraint.layout.matchConstraintMinHeight, constraint.layout.matchConstraintMaxWidth, constraint.layout.matchConstraintMaxHeight, constraint.layout.leftMargin, constraint.layout.topMargin, constraint.layout.rightMargin, constraint.layout.bottomMargin);
#endif

                    if (view is Guideline)//不像Android一样用户提供指定约束是isGuideline就创建Guideline,必须通过Guideline控件实现
                    {
                        if (constraint.layout.orientation != Unset)//如果用户设置了新的orientation
                        {
                            //Orientation在这里设置的原因是在Measure中当约束update到widget时,其他的widget需要对齐Guideline的widget,如果guideline没有设置orientation,那么widget找不到对应的边的Anchor,因为Guideline需要方向才有正确的Anchor
                            (view as Guideline).mGuideline.Orientation = constraint.layout.orientation;
                        }
                    }

                    /*if (applyPostLayout)
                    {
                        ConstraintAttribute.setAttributes(view, constraint.mCustomConstraints);
                    }*/

                    if (constraint.propertySet.mVisibilityMode == VisibilityModeNormal)
                    {
                        //view.Visibility = constraint.propertySet.visibility;
                        param.propertySet.visibility = constraint.propertySet.visibility;//这里我变成设置constraint
                        view.SetViewVisibility(constraint.propertySet.visibility);
                    }
                    if (constraint.propertySet.visibility != ConstraintSet.Invisible)//在可见性为Invisible时,设置可见性时会将Alpha设置为0,再设置Alpha会造成冲突
                        view.SetAlphaProperty(constraint.propertySet);
                    view.SetTransform(constraint.transform);
                }
                else
                {
                    Debug.WriteLine(TAG, "WARNING NO CONSTRAINTS for view " + id);
                }
            }

            foreach (int id in used)//剩下的约束不在ConstraintLayout的Children中,找出特殊的,如Barrier,Guideline,但是这里我们不弄,因为添加新的View有新的Id,那么就需要更新全部的约束,费时
            {
                if (id == ConstraintSet.ParentId || id == constraintLayout.GetId())//还要处理下layout
                    mConstraints[id].ApplyTo(constraintLayout.mConstraintSet.Constraints[id].layout);
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
                View view = constraintLayout.GetChildAt(i);

                if (view is ConstraintHelper)
                {
                    ConstraintHelper constraintHelper = (ConstraintHelper)view;
                    constraintHelper.applyLayoutFeaturesInConstraintSet(constraintLayout);
                }
            }

            constraintLayout.mConstraintSet.IsChanged = true;
            constraintLayout.mConstraintSet.IsForAnimation = false;

            UIThread.Invoke(() =>
            {
                constraintLayout.RequestReLayout();
            }, constraintLayout);
        }

        /// <summary>
        /// 为动画获得坐标,不造成重新Layout
        /// </summary>
        /// <param name="constraintLayout"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void ApplyToForAnim(ConstraintLayout constraintLayout)
        {
            int parentID = constraintLayout.GetId();

            int count = constraintLayout.ChildCount;
            List<int> used = mConstraints.Keys.ToList();//已经设置了约束的id
            if (count != used.Count) Debug.WriteLine("The count of ConstraintLayout children is not equal to temprary constraints list, maybe you not use clone.", TAG);
            for (int i = 0; i < count; i++)//查看layout的child
            {
                View view = constraintLayout.GetChildAt(i);

                int id = view.GetId();
                if (!mConstraints.ContainsKey(id))
                {
                    Debug.WriteLine($"id unknown {view}", TAG);
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
                        barrier.ConstrainType = constraint.layout.mBarrierDirection;
                        barrier.ConstrainMargin = constraint.layout.mBarrierMargin;

                        barrier.AllowsGoneWidget = constraint.layout.mBarrierAllowsGoneWidgets;
                        if (constraint.layout.mReferenceIds != null)
                        {
                            barrier.ReferencedIds = constraint.layout.mReferenceIds;
                        }
                    }

                    //将新的约束更新到ConstraintLayout的ConstraintSet中
                    var param = constraintLayout.mConstraintSet.Constraints[id];
                    param.layout.Validate();
                    constraint.ApplyTo(param.layout);//Android源码ApplyTo是应用到Params,Params中具有负责View布局的属性,其中Margin,Width,Height是ViewGroup自带的,其他是ConstraintLayout中新增的,也就是说,这里使用ConstraintSet替代Params,需要添加ViewGroup的属性

                    //设置原本在ViewGroup.Params中的属性,这些属性影响Android中View的Measure值,因此我们需要在其他平台单独设置
#if __MAUI__
                    //view.SetWidth(constraint.layout.mWidth);
                    view.SetWidth(ConstraintSet.Unset);
                    //view.SetHeight(constraint.layout.mHeight);
                    view.SetHeight(ConstraintSet.Unset);
                    //view.SetMinWidth(ConstraintSet.Unset);
                    //view.SetMinHeight(constraint.layout.matchConstraintMinHeight);
                    //view.SetMaxWidth(constraint.layout.matchConstraintMaxWidth);
                    //view.SetMaxHeight(constraint.layout.matchConstraintMaxHeight);
#else
                    //view.SetSizeAndMargin(constraint.layout.mWidth, constraint.layout.mHeight, constraint.layout.matchConstraintMinWidth, constraint.layout.matchConstraintMinHeight, constraint.layout.matchConstraintMaxWidth, constraint.layout.matchConstraintMaxHeight, constraint.layout.leftMargin, constraint.layout.topMargin, constraint.layout.rightMargin, constraint.layout.bottomMargin);
#endif

                    if (view is Guideline)//不像Android一样用户提供指定约束是isGuideline就创建Guideline,必须通过Guideline控件实现
                    {
                        if (constraint.layout.orientation != Unset)//如果用户设置了新的orientation
                        {
                            //Orientation在这里设置的原因是在Measure中当约束update到widget时,其他的widget需要对齐Guideline的widget,如果guideline没有设置orientation,那么widget找不到对应的边的Anchor,因为Guideline需要方向才有正确的Anchor
                            (view as Guideline).mGuideline.Orientation = constraint.layout.orientation;
                        }
                    }

                    /*if (applyPostLayout)
                    {
                        ConstraintAttribute.setAttributes(view, constraint.mCustomConstraints);
                    }*/

                    if (constraint.propertySet.mVisibilityMode == VisibilityModeNormal)
                    {
                        //view.Visibility = constraint.propertySet.visibility;
                        param.propertySet.visibility = constraint.propertySet.visibility;//这里我变成设置constraint
                        view.SetViewVisibility(constraint.propertySet.visibility);
                    }
                    if (constraint.propertySet.visibility != ConstraintSet.Invisible)//在可见性为Invisible时,设置可见性时会将Alpha设置为0,再设置Alpha会造成冲突
                        view.SetAlphaProperty(constraint.propertySet);
                    view.SetTransform(constraint.transform);
                }
                else
                {
                    Debug.WriteLine(TAG, "WARNING NO CONSTRAINTS for view " + id);
                }
            }

            foreach (int id in used)//剩下的约束不在ConstraintLayout的Children中,找出特殊的,如Barrier,Guideline,但是这里我们不弄,因为添加新的View有新的Id,那么就需要更新全部的约束,费时
            {
                if (id == ConstraintSet.ParentId || id == constraintLayout.GetId())//还要处理下layout
                    mConstraints[id].ApplyTo(constraintLayout.mConstraintSet.Constraints[id].layout);
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
                View view = constraintLayout.GetChildAt(i);

                if (view is ConstraintHelper)
                {
                    ConstraintHelper constraintHelper = (ConstraintHelper)view;
                    constraintHelper.applyLayoutFeaturesInConstraintSet(constraintLayout);
                }
            }

            constraintLayout.mConstraintSet.IsChanged = true;
            constraintLayout.mConstraintSet.IsForAnimation = true;

            //UIThread.Invoke(() =>
            //{
            //    constraintLayout.RequestReLayout();
            //}, constraintLayout);
        }

        /// <summary>
        /// Center widget between the other two widgets. (for sides see: {@link #TOP, <seealso
        /// cref="#BOTTOM"/>, {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso
        /// cref="#RIGHT"/>) Note, sides must be all vertical or horizontal sides.
        /// </summary>
        /// <param name="centerID">ID of the widget to be centered</param>
        /// <param name="firstID">
        /// ID of the first widget to connect the left or top of the widget to
        /// </param>
        /// <param name="firstSide">the side of the widget to connect to</param>
        /// <param name="firstMargin">the connection margin</param>
        /// <param name="secondId">
        /// the ID of the second widget to connect to right or top of the widget to
        /// </param>
        /// <param name="secondSide">the side of the widget to connect to</param>
        /// <param name="secondMargin">the connection margin</param>
        /// <param name="bias">the ratio between two connections</param>
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

            if (firstSide == Left || firstSide == Right)
            {
                Connect(centerID, Left, firstID, firstSide, firstMargin);
                Connect(centerID, Right, secondId, secondSide, secondMargin);
                Constraint constraint = mConstraints[centerID];
                if (constraint != null)
                {
                    constraint.layout.horizontalBias = bias;
                }
            }
            else if (firstSide == Start || firstSide == End)
            {
                Connect(centerID, Start, firstID, firstSide, firstMargin);
                Connect(centerID, End, secondId, secondSide, secondMargin);
                Constraint constraint = mConstraints[centerID];
                if (constraint != null)
                {
                    constraint.layout.horizontalBias = bias;
                }
            }
            else
            {
                Connect(centerID, Top, firstID, firstSide, firstMargin);
                Connect(centerID, Bottom, secondId, secondSide, secondMargin);
                Constraint constraint = mConstraints[centerID];
                if (constraint != null)
                {
                    constraint.layout.verticalBias = bias;
                }
            }
        }

        /// <summary>
        /// Centers the widget horizontally to the left and right side on another widgets sides.
        /// (for sides see: {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso
        /// cref="#RIGHT"/>)
        /// </summary>
        /// <param name="centerID">ID of widget to be centered</param>
        /// <param name="leftId">The Id of the widget on the left side</param>
        /// <param name="leftSide">The side of the leftId widget to connect to</param>
        /// <param name="leftMargin">The margin on the left side</param>
        /// <param name="rightId">The Id of the widget on the right side</param>
        /// <param name="rightSide">The side of the rightId widget to connect to</param>
        /// <param name="rightMargin">The margin on the right side</param>
        /// <param name="bias">
        /// The ratio of the space on the left vs. right sides 0.5 is centered (default)
        /// </param>
        public virtual void CenterHorizontally(int centerID, int leftId, int leftSide, int leftMargin, int rightId, int rightSide, int rightMargin, float bias)
        {
            Connect(centerID, Left, leftId, leftSide, leftMargin);
            Connect(centerID, Right, rightId, rightSide, rightMargin);
            Constraint constraint = mConstraints[centerID];
            if (constraint != null)
            {
                constraint.layout.horizontalBias = bias;
            }
        }

        /// <summary>
        /// Centers the widgets horizontally to the left and right side on another widgets sides.
        /// (for sides see: <seealso cref="#START"/>, <seealso cref="#END"/>, <seealso
        /// cref="#LEFT"/>, <seealso cref="#RIGHT"/>)
        /// </summary>
        /// <param name="centerID">ID of widget to be centered</param>
        /// <param name="startId">
        /// The Id of the widget on the start side (left in non rtl languages)
        /// </param>
        /// <param name="startSide">The side of the startId widget to connect to</param>
        /// <param name="startMargin">The margin on the start side</param>
        /// <param name="endId">
        /// The Id of the widget on the start side (left in non rtl languages)
        /// </param>
        /// <param name="endSide">The side of the endId widget to connect to</param>
        /// <param name="endMargin">The margin on the end side</param>
        /// <param name="bias">
        /// The ratio of the space on the start vs end side 0.5 is centered (default)
        /// </param>
        public virtual void CenterHorizontallyRtl(int centerID, int startId, int startSide, int startMargin, int endId, int endSide, int endMargin, float bias)
        {
            Connect(centerID, Start, startId, startSide, startMargin);
            Connect(centerID, End, endId, endSide, endMargin);
            Constraint constraint = mConstraints[centerID];
            if (constraint != null)
            {
                constraint.layout.horizontalBias = bias;
            }
        }

        /// <summary>
        /// Centers the widgets vertically to the top and bottom side on another widgets sides. (for
        /// sides see: {@link #TOP, <seealso cref="#BOTTOM"/>)
        /// </summary>
        /// <param name="centerID">ID of widget to be centered</param>
        /// <param name="topId">The Id of the widget on the top side</param>
        /// <param name="topSide">The side of the leftId widget to connect to</param>
        /// <param name="topMargin">The margin on the top side</param>
        /// <param name="bottomId">The Id of the widget on the bottom side</param>
        /// <param name="bottomSide">The side of the bottomId widget to connect to</param>
        /// <param name="bottomMargin">The margin on the bottom side</param>
        /// <param name="bias">
        /// The ratio of the space on the top vs. bottom sides 0.5 is centered (default)
        /// </param>
        public virtual void CenterVertically(int centerID, int topId, int topSide, int topMargin, int bottomId, int bottomSide, int bottomMargin, float bias)
        {
            Connect(centerID, Top, topId, topSide, topMargin);
            Connect(centerID, Bottom, bottomId, bottomSide, bottomMargin);
            Constraint constraint = mConstraints[centerID];
            if (constraint != null)
            {
                constraint.layout.verticalBias = bias;
            }
        }

        /// <summary>
        /// Spaces a set of widgets vertically between the view topId and bottomId. Widgets can be
        /// spaced with weights. This operation sets all the related margins to 0.
        /// <para>(for sides see: {@link #TOP, <seealso cref="#BOTTOM"/>)</para>
        /// </summary>
        /// <param name="topId">The id of the widget to connect to or PARENT_ID</param>
        /// <param name="topSide">the side of the start to connect to</param>
        /// <param name="bottomId">The id of the widget to connect to or PARENT_ID</param>
        /// <param name="bottomSide">the side of the right to connect to</param>
        /// <param name="chainIds">widgets to use as a chain</param>
        /// <param name="weights">can be null</param>
        /// <param name="style">set the style of the chain</param>
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

            Connect(chainIds[0], Top, topId, topSide, 0);
            for (int i = 1; i < chainIds.Length; i++)
            {
                int chainId = chainIds[i];
                Connect(chainIds[i], Top, chainIds[i - 1], Bottom, 0);
                Connect(chainIds[i - 1], Bottom, chainIds[i], Top, 0);
                if (weights != null)
                {
                    get(chainIds[i]).layout.verticalWeight = weights[i];
                }
            }
            Connect(chainIds[chainIds.Length - 1], Bottom, bottomId, bottomSide, 0);
        }

        /// <summary>
        /// Spaces a set of widgets horizontally between the view startID and endId. Widgets can be
        /// spaced with weights. This operation sets all the related margins to 0.
        /// <para>
        /// (for sides see: {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso
        /// cref="#RIGHT"/>
        /// </para>
        /// </summary>
        /// <param name="leftId">The id of the widget to connect to or PARENT_ID</param>
        /// <param name="leftSide">the side of the start to connect to</param>
        /// <param name="rightId">The id of the widget to connect to or PARENT_ID</param>
        /// <param name="rightSide">the side of the right to connect to</param>
        /// <param name="chainIds">The widgets in the chain</param>
        /// <param name="weights">The weight to assign to each element in the chain or null</param>
        /// <param name="style">The type of chain</param>
        public virtual void CreateHorizontalChain(int leftId, int leftSide, int rightId, int rightSide, int[] chainIds, float[] weights, int style)
        {
            createHorizontalChain(leftId, leftSide, rightId, rightSide, chainIds, weights, style, Left, Right);
        }

        /// <summary>
        /// Spaces a set of widgets horizontal between the view startID and endId. Widgets can be
        /// spaced with weights. (for sides see: {@link #START, <seealso cref="#END"/>, {@link
        /// #LEFT, <seealso cref="#RIGHT"/>)
        /// </summary>
        /// <param name="startId">The id of the widget to connect to or PARENT_ID</param>
        /// <param name="startSide">the side of the start to connect to</param>
        /// <param name="endId">The id of the widget to connect to or PARENT_ID</param>
        /// <param name="endSide">the side of the end to connect to</param>
        /// <param name="chainIds">The widgets in the chain</param>
        /// <param name="weights">The weight to assign to each element in the chain or null</param>
        /// <param name="style">The type of chain</param>
        public virtual void CreateHorizontalChainRtl(int startId, int startSide, int endId, int endSide, int[] chainIds, float[] weights, int style)
        {
            createHorizontalChain(startId, startSide, endId, endSide, chainIds, weights, style, Start, End);
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
            Connect(chainIds[0], left, leftId, leftSide, Unset);
            for (int i = 1; i < chainIds.Length; i++)
            {
                int chainId = chainIds[i];
                Connect(chainIds[i], left, chainIds[i - 1], right, Unset);
                Connect(chainIds[i - 1], right, chainIds[i], left, Unset);
                if (weights != null)
                {
                    get(chainIds[i]).layout.horizontalWeight = weights[i];
                }
            }

            Connect(chainIds[chainIds.Length - 1], right, rightId, rightSide, Unset);
        }

        /// <summary>
        /// Create a constraint between two widgets. (for sides see: {@link #TOP, <seealso
        /// cref="Bottom"/>, {@link #START, <seealso cref="End"/>, {@link #LEFT, <seealso
        /// cref="Right"/>, <seealso cref="Baseline"/>)
        /// </summary>
        /// <param name="startID">the ID of the widget to be constrained</param>
        /// <param name="startSide">the side of the widget to constrain</param>
        /// <param name="endID">the id of the widget to constrain to</param>
        /// <param name="endSide">the side of widget to constrain to</param>
        /// <param name="margin">the margin to constrain (margin must be positive)</param>
        public virtual void Connect(int startID, int startSide, int endID, int endSide, int margin)
        {
            Constraint constraint = get(startID);
            if (constraint == null)
            {
                return;
            }
            switch (startSide)
            {
                case Left:
                    if (endSide == Left)
                    {
                        constraint.layout.leftToLeft = endID;
                        constraint.layout.leftToRight = Layout.UNSET;
                    }
                    else if (endSide == Right)
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

                case Right:
                    if (endSide == Left)
                    {
                        constraint.layout.rightToLeft = endID;
                        constraint.layout.rightToRight = Layout.UNSET;
                    }
                    else if (endSide == Right)
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

                case Top:
                    if (endSide == Top)
                    {
                        constraint.layout.topToTop = endID;
                        constraint.layout.topToBottom = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else if (endSide == Bottom)
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

                case Bottom:
                    if (endSide == Bottom)
                    {
                        constraint.layout.bottomToBottom = endID;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else if (endSide == Top)
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

                case Baseline:
                    if (endSide == Baseline)
                    {
                        constraint.layout.baselineToBaseline = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else if (endSide == Top)
                    {
                        constraint.layout.baselineToTop = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else if (endSide == Bottom)
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

                case Start:
                    if (endSide == Start)
                    {
                        constraint.layout.startToStart = endID;
                        constraint.layout.startToEnd = Layout.UNSET;
                    }
                    else if (endSide == End)
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

                case End:
                    if (endSide == End)
                    {
                        constraint.layout.endToEnd = endID;
                        constraint.layout.endToStart = Layout.UNSET;
                    }
                    else if (endSide == Start)
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
        /// Create a constraint between two widgets. (for sides see: {@link #TOP, <seealso
        /// cref="#BOTTOM"/>, {@link #START, <seealso cref="#END"/>, {@link #LEFT, <seealso
        /// cref="#RIGHT"/>, <seealso cref="#BASELINE"/>)
        /// </summary>
        /// <param name="startID">the ID of the widget to be constrained</param>
        /// <param name="startSide">the side of the widget to constrain</param>
        /// <param name="endID">the id of the widget to constrain to</param>
        /// <param name="endSide">the side of widget to constrain to</param>
        public virtual void Connect(int startID, int startSide, int endID, int endSide)
        {
            Constraint constraint = get(startID);
            if (constraint == null)
            {
                return;
            }
            switch (startSide)
            {
                case Left:
                    if (endSide == Left)
                    {
                        constraint.layout.leftToLeft = endID;
                        constraint.layout.leftToRight = Layout.UNSET;
                    }
                    else if (endSide == Right)
                    {
                        constraint.layout.leftToRight = endID;
                        constraint.layout.leftToLeft = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("left to " + sideToString(endSide) + " undefined");
                    }
                    break;

                case Right:
                    if (endSide == Left)
                    {
                        constraint.layout.rightToLeft = endID;
                        constraint.layout.rightToRight = Layout.UNSET;
                    }
                    else if (endSide == Right)
                    {
                        constraint.layout.rightToRight = endID;
                        constraint.layout.rightToLeft = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;

                case Top:
                    if (endSide == Top)
                    {
                        constraint.layout.topToTop = endID;
                        constraint.layout.topToBottom = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else if (endSide == Bottom)
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

                case Bottom:
                    if (endSide == Bottom)
                    {
                        constraint.layout.bottomToBottom = endID;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                    }
                    else if (endSide == Top)
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

                case Baseline:
                    if (endSide == Baseline)
                    {
                        constraint.layout.baselineToBaseline = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else if (endSide == Top)
                    {
                        constraint.layout.baselineToTop = endID;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topToBottom = Layout.UNSET;
                    }
                    else if (endSide == Bottom)
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

                case Start:
                    if (endSide == Start)
                    {
                        constraint.layout.startToStart = endID;
                        constraint.layout.startToEnd = Layout.UNSET;
                    }
                    else if (endSide == End)
                    {
                        constraint.layout.startToEnd = endID;
                        constraint.layout.startToStart = Layout.UNSET;
                    }
                    else
                    {
                        throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
                    }
                    break;

                case End:
                    if (endSide == End)
                    {
                        constraint.layout.endToEnd = endID;
                        constraint.layout.endToStart = Layout.UNSET;
                    }
                    else if (endSide == Start)
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
        /// <param name="viewId">ID of view to center Horizontally</param>
        /// <param name="toView">ID of view to center on (or in)</param>
        public virtual void CenterHorizontally(int viewId, int toView)
        {
            if (toView == ParentId)
            {
                Center(viewId, ParentId, ConstraintSet.Left, 0, ParentId, ConstraintSet.Right, 0, 0.5f);
            }
            else
            {
                Center(viewId, toView, ConstraintSet.Right, 0, toView, ConstraintSet.Left, 0, 0.5f);
            }
        }

        /// <summary>
        /// Centers the view horizontally relative to toView's position.
        /// </summary>
        /// <param name="viewId">ID of view to center Horizontally</param>
        /// <param name="toView">ID of view to center on (or in)</param>
        public virtual void CenterHorizontallyRtl(int viewId, int toView)
        {
            if (toView == ParentId)
            {
                Center(viewId, ParentId, ConstraintSet.Start, 0, ParentId, ConstraintSet.End, 0, 0.5f);
            }
            else
            {
                Center(viewId, toView, ConstraintSet.End, 0, toView, ConstraintSet.Start, 0, 0.5f);
            }
        }

        /// <summary>
        /// Centers the view vertically relative to toView's position.
        /// </summary>
        /// <param name="viewId">ID of view to center Horizontally</param>
        /// <param name="toView">ID of view to center on (or in)</param>
        public virtual void CenterVertically(int viewId, int toView)
        {
            if (toView == ParentId)
            {
                Center(viewId, ParentId, ConstraintSet.Top, 0, ParentId, ConstraintSet.Bottom, 0, 0.5f);
            }
            else
            {
                Center(viewId, toView, ConstraintSet.Bottom, 0, toView, ConstraintSet.Top, 0, 0.5f);
            }
        }

        /// <summary>
        /// 清除该View的所有约束
        /// </summary>
        /// <param name="viewId"></param>
        public void Clear(int viewId)
        {
            mConstraints.Remove(viewId);
        }

        /// <summary>
        /// 清楚某一边的约束
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="anchor"></param>
        /// <exception cref="System.ArgumentException"></exception>
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
                    case Left:
                        constraint.layout.leftToRight = Layout.UNSET;
                        constraint.layout.leftToLeft = Layout.UNSET;
                        constraint.layout.leftMargin = Layout.UNSET;
                        constraint.layout.goneLeftMargin = Layout.UNSET_GONE_MARGIN;
                        break;

                    case Right:
                        constraint.layout.rightToRight = Layout.UNSET;
                        constraint.layout.rightToLeft = Layout.UNSET;
                        constraint.layout.rightMargin = Layout.UNSET;
                        constraint.layout.goneRightMargin = Layout.UNSET_GONE_MARGIN;
                        break;

                    case Top:
                        constraint.layout.topToBottom = Layout.UNSET;
                        constraint.layout.topToTop = Layout.UNSET;
                        constraint.layout.topMargin = 0;
                        constraint.layout.goneTopMargin = Layout.UNSET_GONE_MARGIN;
                        break;

                    case Bottom:
                        constraint.layout.bottomToTop = Layout.UNSET;
                        constraint.layout.bottomToBottom = Layout.UNSET;
                        constraint.layout.bottomMargin = 0;
                        constraint.layout.goneBottomMargin = Layout.UNSET_GONE_MARGIN;
                        break;

                    case Baseline:
                        constraint.layout.baselineToBaseline = Layout.UNSET;
                        constraint.layout.baselineToTop = Layout.UNSET;
                        constraint.layout.baselineToBottom = Layout.UNSET;
                        constraint.layout.baselineMargin = 0;
                        constraint.layout.goneBaselineMargin = Layout.UNSET_GONE_MARGIN;
                        break;

                    case Start:
                        constraint.layout.startToEnd = Layout.UNSET;
                        constraint.layout.startToStart = Layout.UNSET;
                        constraint.layout.startMargin = 0;
                        constraint.layout.goneStartMargin = Layout.UNSET_GONE_MARGIN;
                        break;

                    case End:
                        constraint.layout.endToStart = Layout.UNSET;
                        constraint.layout.endToEnd = Layout.UNSET;
                        constraint.layout.endMargin = 0;
                        constraint.layout.goneEndMargin = Layout.UNSET_GONE_MARGIN;
                        break;

                    case CircleReference:
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
        /// <param name="viewId">ID of view to adjust the margin on</param>
        /// <param name="anchor">The side to adjust the margin on</param>
        /// <param name="value">The new value for the margin</param>
        public virtual void SetMargin(int viewId, int anchor, int value)
        {
            Constraint constraint = get(viewId);
            switch (anchor)
            {
                case Left:
                    constraint.layout.leftMargin = value;
                    break;

                case Right:
                    constraint.layout.rightMargin = value;
                    break;

                case Top:
                    constraint.layout.topMargin = value;
                    break;

                case Bottom:
                    constraint.layout.bottomMargin = value;
                    break;

                case Baseline:
                    constraint.layout.baselineMargin = value;
                    break;

                case Start:
                    constraint.layout.startMargin = value;
                    break;

                case End:
                    constraint.layout.endMargin = value;
                    break;

                default:
                    throw new System.ArgumentException("unknown constraint");
            }
        }

        /// <summary>
        /// Sets the gone margin.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the margin on</param>
        /// <param name="anchor">The side to adjust the margin on</param>
        /// <param name="value">The new value for the margin</param>
        public virtual void SetGoneMargin(int viewId, int anchor, int value)
        {
            Constraint constraint = get(viewId);
            switch (anchor)
            {
                case Left:
                    constraint.layout.goneLeftMargin = value;
                    break;

                case Right:
                    constraint.layout.goneRightMargin = value;
                    break;

                case Top:
                    constraint.layout.goneTopMargin = value;
                    break;

                case Bottom:
                    constraint.layout.goneBottomMargin = value;
                    break;

                case Baseline:
                    constraint.layout.goneBaselineMargin = value;
                    break;

                case Start:
                    constraint.layout.goneStartMargin = value;
                    break;

                case End:
                    constraint.layout.goneEndMargin = value;
                    break;

                default:
                    throw new System.ArgumentException("unknown constraint");
            }
        }

        /// <summary>
        /// Adjust the horizontal bias of the view (used with views constrained on left and right).
        /// </summary>
        /// <param name="viewId">ID of view to adjust the horizontal</param>
        /// <param name="bias">the new bias 0.5 is in the middle</param>
        public virtual void SetHorizontalBias(int viewId, float bias)
        {
            get(viewId).layout.horizontalBias = bias;
        }

        /// <summary>
        /// Adjust the vertical bias of the view (used with views constrained on left and right).
        /// </summary>
        /// <param name="viewId">ID of view to adjust the vertical</param>
        /// <param name="bias">the new bias 0.5 is in the middle</param>
        public virtual void SetVerticalBias(int viewId, float bias)
        {
            get(viewId).layout.verticalBias = bias;
        }

        /// <summary>
        /// Constrains the views aspect ratio. For Example a HD screen is 16 by 9 = 16/(float)9 =
        /// 1.777f.
        /// </summary>
        /// <param name="viewId">ID of view to constrain</param>
        /// <param name="ratio">The ratio of the width to height (width / height)</param>
        public virtual void SetDimensionRatio(int viewId, string ratio)
        {
            get(viewId).layout.dimensionRatio = ratio;
        }

        /// <summary>
        /// Adjust the visibility of a view. <see cref="Gone"/> or <see cref="Invisible"/> or <see
        /// cref="Visible"/>
        /// </summary>
        /// <param name="viewId">ID of view to adjust the vertical</param>
        /// <param name="visibility">the visibility</param>
        public virtual void SetVisibility(int viewId, int visibility)
        {
            get(viewId).propertySet.visibility = visibility;
        }

        /// <summary>
        /// ConstraintSet will not setVisibility. <seealso cref="VisibilityModeIgnore"/> or <see
        /// cref="VisibilityModeNormal"/>
        /// </summary>
        /// <param name="viewId">ID of view</param>
        /// <param name="visibilityMode"></param>
        public virtual void SetVisibilityMode(int viewId, int visibilityMode)
        {
            get(viewId).propertySet.mVisibilityMode = visibilityMode;
        }

        /// <summary>
        /// ConstraintSet will not setVisibility. <seealso cref="VisibilityModeIgnore"/> or <see
        /// cref="VisibilityModeNormal"/>
        /// </summary>
        /// <param name="viewId">ID of view</param>
        public virtual int GetVisibilityMode(int viewId)
        {
            return get(viewId).propertySet.mVisibilityMode;
        }

        /// <summary>
        /// Get the visibility flag set in this view. <see cref="Gone"/> or <see cref="Invisible"/>
        /// or <see cref="Visible"/>
        /// </summary>
        /// <param name="viewId">the id of the view</param>
        /// <returns>the visibility constraint for the view</returns>
        public virtual int GetVisibility(int viewId)
        {
            return get(viewId).propertySet.visibility;
        }

        /// <summary>
        /// Get the height set in the view. It can be number, or <see cref="MatchConstraint"/>,or
        /// <see cref="MatchParent"/>,or <see cref="WrapContent"/>
        /// </summary>
        /// <param name="viewId">the id of the view</param>
        /// <returns>return the height constraint of the view</returns>
        public virtual int GetHeight(int viewId)
        {
            return get(viewId).layout.mHeight;
        }

        /// <summary>
        /// Get the width set in the view. It can be number, or <see cref="MatchConstraint"/>,or
        /// <see cref="MatchParent"/>,or <see cref="WrapContent"/>
        /// </summary>
        /// <param name="viewId">the id of the view</param>
        /// <returns>return the width constraint of the view</returns>
        public virtual int GetWidth(int viewId)
        {
            return get(viewId).layout.mWidth;
        }

        /// <summary>
        /// Adjust the alpha of a view.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the vertical</param>
        /// <param name="alpha">the alpha</param>
        public virtual void SetAlpha(int viewId, float alpha)
        {
            get(viewId).propertySet.alpha = alpha;
        }

        /// <summary>
        /// return with the constraint set will apply elevation for the specified view.
        /// </summary>
        /// <returns>true if the elevation will be set on this view (default is false)</returns>
        public virtual bool GetApplyElevation(int viewId)
        {
            return get(viewId).transform.applyElevation;
        }

        /// <summary>
        /// set if elevation will be applied to the view. Elevation logic is based on style and
        /// animation. By default it is not used because it would lead to unexpected results.
        /// </summary>
        /// <param name="apply">true if this constraint set applies elevation to this view</param>
        public virtual void SetApplyElevation(int viewId, bool apply)
        {
            //if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
            //{
            get(viewId).transform.applyElevation = apply;
            //}
        }

        /// <summary>
        /// Adjust the elevation of a view.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the elevation</param>
        /// <param name="elevation">the elevation</param>
        public virtual void SetElevation(int viewId, float elevation)
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
        /// <param name="viewId">ID of view to adjust th Z rotation</param>
        /// <param name="rotation">the rotation about the X axis</param>
        public virtual void SetRotation(int viewId, float rotation)
        {
            get(viewId).transform.rotation = rotation;
        }

        /// <summary>
        /// Adjust the post-layout rotation about the X axis of a view.
        /// </summary>
        /// <param name="viewId">ID of view to adjust th X rotation</param>
        /// <param name="rotationX">the rotation about the X axis</param>
        public virtual void SetRotationX(int viewId, float rotationX)
        {
            get(viewId).transform.rotationX = rotationX;
        }

        /// <summary>
        /// Adjust the post-layout rotation about the Y axis of a view.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the Y rotation</param>
        /// <param name="rotationY">the rotationY</param>
        public virtual void SetRotationY(int viewId, float rotationY)
        {
            get(viewId).transform.rotationY = rotationY;
        }

        /// <summary>
        /// Adjust the post-layout scale in X of a view.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the scale in X</param>
        /// <param name="scaleX">the scale in X</param>
        public virtual void SetScaleX(int viewId, float scaleX)
        {
            get(viewId).transform.scaleX = scaleX;
        }

        /// <summary>
        /// Adjust the post-layout scale in Y of a view.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the scale in Y</param>
        /// <param name="scaleY">the scale in Y</param>
        public virtual void SetScaleY(int viewId, float scaleY)
        {
            get(viewId).transform.scaleY = scaleY;
        }

        /// <summary>
        /// Set X location of the pivot point around which the view will rotate and scale. use
        /// Float.NaN to clear the pivot value.
        /// Note: once an actual View has had its pivot set it cannot be cleared.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the transforms pivot point about X</param>
        /// <param name="transformPivotX">X location of the pivot point.</param>
        public virtual void SetTransformPivotX(int viewId, float transformPivotX)
        {
            get(viewId).transform.transformPivotX = transformPivotX;
        }

        /// <summary>
        /// Set Y location of the pivot point around which the view will rotate and scale. use
        /// Float.NaN to clear the pivot value.
        /// Note: once an actual View has had its pivot set it cannot be cleared.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the transforms pivot point about Y</param>
        /// <param name="transformPivotY">Y location of the pivot point.</param>
        public virtual void SetTransformPivotY(int viewId, float transformPivotY)
        {
            get(viewId).transform.transformPivotY = transformPivotY;
        }

        /// <summary>
        /// Set X,Y location of the pivot point around which the view will rotate and scale. use
        /// Float.NaN to clear the pivot value.
        /// Note: once an actual View has had its pivot set it cannot be cleared.
        /// </summary>
        /// <param name="viewId">ID of view to adjust the transforms pivot point</param>
        /// <param name="transformPivotX">X location of the pivot point.</param>
        /// <param name="transformPivotY">Y location of the pivot point.</param>
        public virtual void SetTransformPivot(int viewId, float transformPivotX, float transformPivotY)
        {
            Constraint constraint = get(viewId);
            constraint.transform.transformPivotY = transformPivotY;
            constraint.transform.transformPivotX = transformPivotX;
        }

        /// <summary>
        /// Adjust the post-layout X translation of a view.
        /// </summary>
        /// <param name="viewId">ID of view to translate in X</param>
        /// <param name="translationX">the translation in X</param>
        public virtual void SetTranslationX(int viewId, float translationX)
        {
            get(viewId).transform.translationX = translationX;
        }

        /// <summary>
        /// Adjust the post-layout Y translation of a view.
        /// </summary>
        /// <param name="viewId">ID of view to to translate in Y</param>
        /// <param name="translationY">the translation in Y</param>
        public virtual void SetTranslationY(int viewId, float translationY)
        {
            get(viewId).transform.translationY = translationY;
        }

        /// <summary> Adjust the post-layout translation of a view. </summary> <param name="viewId">
        /// ID of view to adjust its translation in X & Y </param> <param name="translationX"> the
        /// translation in X </param> <param name="translationY"> the translation in Y </param>
        public virtual void SetTranslation(int viewId, float translationX, float translationY)
        {
            Constraint constraint = get(viewId);
            constraint.transform.translationX = translationX;
            constraint.transform.translationY = translationY;
        }

        /// <summary>
        /// Adjust the translation in Z of a view.
        /// </summary>
        /// <param name="viewId">ID of view to adjust</param>
        /// <param name="translationZ">the translationZ</param>
        public virtual void SetTranslationZ(int viewId, float translationZ)
        {
            //if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
            //{
            get(viewId).transform.translationZ = translationZ;
            //}
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public virtual void SetEditorAbsoluteX(int viewId, int position)
        {
            get(viewId).layout.editorAbsoluteX = position;
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public virtual void SetEditorAbsoluteY(int viewId, int position)
        {
            get(viewId).layout.editorAbsoluteY = position;
        }

        /// <summary>
        /// Sets the wrap behavior of the widget in the parent's wrap computation
        /// </summary>
        public virtual void SetLayoutWrapBehavior(int viewId, int behavior)
        {
            if (behavior >= 0 && behavior <= ConstraintWidget.WRAP_BEHAVIOR_SKIPPED)
            {
                get(viewId).layout.wrapBehaviorInParent = behavior;
            }
        }

        /// <summary>
        /// Sets the height of the view. It can be a dimension, <seealso cref="WrapContent"/> or
        /// <see cref="MatchConstraint"/>
        /// </summary>
        /// <param name="viewId">ID of view to adjust its height</param>
        /// <param name="height">the height of the view @since 1.1</param>
        public virtual void ConstrainHeight(int viewId, int height)
        {
            get(viewId).layout.mHeight = height;
        }

        /// <summary>
        /// Sets the width of the view. It can be a dimension, <seealso cref="MatchParent"/> or
        /// <seealso cref="WrapContent"/> or <see cref="MatchConstraint"/>
        /// </summary>
        /// <param name="viewId">ID of view to adjust its width</param>
        /// <param name="width">the width of the view @since 1.1</param>
        public virtual void ConstrainWidth(int viewId, int width)
        {
            get(viewId).layout.mWidth = width;
        }

        /// <summary>
        /// Constrain the view on a circle constraint.
        /// Notice the angle <see href="https://developer.android.com/reference/androidx/constraintlayout/widget/ConstraintLayout#CircularPositioning">ConstraintLayout.CircularPositioning</see>
        /// </summary>
        /// <param name="viewId">ID of the view we constrain</param>
        /// <param name="id">ID of the view we constrain relative to</param>
        /// <param name="radius">the radius of the circle in degrees</param>
        /// <param name="angle">the angle @since 1.1</param>
        public virtual void ConstrainCircle(int viewId, int id, int radius, float angle)
        {
            Constraint constraint = get(viewId);
            constraint.layout.circleConstraint = id;
            constraint.layout.circleRadius = radius;
            constraint.layout.circleAngle = angle;
        }

        /// <summary>
        /// Sets the maximum height of the view. It is a dimension, It is only applicable if height
        /// is <see cref="MatchConstraint"/>.
        /// </summary>
        /// <param name="viewId">ID of view to adjust it height</param>
        /// <param name="height">the maximum height of the constraint @since 1.1</param>
        public virtual void ConstrainMaxHeight(int viewId, int height)
        {
            get(viewId).layout.matchConstraintMaxHeight = height;
        }

        /// <summary>
        /// Sets the maximum width of the view. It is a dimension, It is only applicable if width is
        /// <see cref="MatchConstraint"/>.
        /// </summary>
        /// <param name="viewId">ID of view to adjust its max height</param>
        /// <param name="width">the maximum width of the view @since 1.1</param>
        public virtual void ConstrainMaxWidth(int viewId, int width)
        {
            get(viewId).layout.matchConstraintMaxWidth = width;
        }

        /// <summary>
        /// Sets the height of the view. It is a dimension, It is only applicable if height is <see
        /// cref="MatchConstraint"/>.
        /// </summary>
        /// <param name="viewId">ID of view to adjust its min height</param>
        /// <param name="height">the minimum height of the view @since 1.1</param>
        public virtual void ConstrainMinHeight(int viewId, int height)
        {
            get(viewId).layout.matchConstraintMinHeight = height;
        }

        /// <summary>
        /// Sets the width of the view. It is a dimension, It is only applicable if width is <see
        /// cref="MatchConstraint"/>.
        /// </summary>
        /// <param name="viewId">ID of view to adjust its min height</param>
        /// <param name="width">the minimum width of the view @since 1.1</param>
        public virtual void ConstrainMinWidth(int viewId, int width)
        {
            get(viewId).layout.matchConstraintMinWidth = width;
        }

        /// <summary>
        /// Sets the width of the view as a percentage of the parent.
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="percent">@since 1.1</param>
        public virtual void ConstrainPercentWidth(int viewId, float percent)
        {
            get(viewId).layout.matchConstraintPercentWidth = percent;
        }

        /// <summary>
        /// Sets the height of the view as a percentage of the parent.
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="percent">@since 1.1</param>
        public virtual void ConstrainPercentHeight(int viewId, float percent)
        {
            get(viewId).layout.matchConstraintPercentHeight = percent;
        }

        /// <summary>
        /// Sets how the height is calculated ether <see cref="MatchConstraintWrap"/> or <see
        /// cref="MatchConstraintSpread"/>. Default is spread.
        /// </summary>
        /// <param name="viewId">ID of view to adjust its matchConstraintDefaultHeight</param>
        /// <param name="height">MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD @since 1.1</param>
        public virtual void ConstrainDefaultHeight(int viewId, int height)
        {
            get(viewId).layout.matchConstraintDefaultHeight = height;
        }

        /// <summary>
        /// Sets how the width is calculated ether <see cref="MatchConstraintWrap"/> or <see
        /// cref="MatchConstraintSpread"/>. Default is spread.
        /// </summary>
        /// <param name="viewId">ID of view to adjust its matchConstraintDefaultWidth</param>
        /// <param name="constrained">if true with will be constrained @since 1.1</param>
        public virtual void ConstrainedWidth(int viewId, bool constrained)
        {
            get(viewId).layout.constrainedWidth = constrained;
        }

        /// <summary>
        /// Sets how the height is calculated ether <see cref="MatchConstraintWrap"/> or <see
        /// cref="MatchConstraintSpread"/>. Default is spread.
        /// </summary>
        /// <param name="viewId">ID of view to adjust its matchConstraintDefaultHeight</param>
        /// <param name="constrained">if true height will be constrained @since 1.1</param>
        public virtual void ConstrainedHeight(int viewId, bool constrained)
        {
            get(viewId).layout.constrainedHeight = constrained;
        }

        /// <summary>
        /// Sets how the width is calculated ether <see cref="MatchConstraintWrap"/> or <see
        /// cref="MatchConstraintSpread"/>. Default is spread.
        /// </summary>
        /// <param name="viewId">ID of view to adjust its matchConstraintDefaultWidth</param>
        /// <param name="width">SPREAD or WRAP @since 1.1</param>
        public virtual void ConstrainDefaultWidth(int viewId, int width)
        {
            get(viewId).layout.matchConstraintDefaultWidth = width;
        }

        /// <summary>
        /// The child's weight that we can use to distribute the available horizontal space in a
        /// chain, if the dimension behaviour is set to <see cref="MatchConstraint"/>
        /// </summary>
        /// <param name="viewId">ID of view to adjust its HorizontalWeight</param>
        /// <param name="weight">
        /// the weight that we can use to distribute the horizontal space
        /// </param>
        public virtual void SetHorizontalWeight(int viewId, float weight)
        {
            get(viewId).layout.horizontalWeight = weight;
        }

        /// <summary>
        /// The child's weight that we can use to distribute the available vertical space in a
        /// chain, if the dimension behaviour is set to <see cref="MatchConstraint"/> T
        /// </summary>
        /// <param name="viewId">ID of view to adjust its VerticalWeight</param>
        /// <param name="weight">the weight that we can use to distribute the vertical space</param>
        public virtual void SetVerticalWeight(int viewId, float weight)
        {
            get(viewId).layout.verticalWeight = weight;
        }

        /// <summary>
        /// How the elements of the horizontal chain will be positioned. if the dimension behaviour
        /// is set to <see cref="MatchConstraint"/>. The possible values are: <br/><seealso
        /// cref="ChainSpread"/> -- the elements will be spread out <br/><seealso
        /// cref="ChainSpreadInside"/> -- similar, but the endpoints of the chain will not be spread
        /// out <br/><seealso cref="ChainPacked"/> -- the elements of the chain will be packed
        /// together. The horizontal bias attribute of the child will then affect the positioning of
        /// the packed elements <br/>
        /// </summary>
        /// <param name="viewId">ID of view to adjust its HorizontalChainStyle</param>
        /// <param name="chainStyle">
        /// the weight that we can use to distribute the horizontal space
        /// </param>
        public virtual void SetHorizontalChainStyle(int viewId, int chainStyle)
        {
            get(viewId).layout.horizontalChainStyle = chainStyle;
        }

        /// <summary>
        /// How the elements of the vertical chain will be positioned. in a chain, if the dimension
        /// behaviour is set to MATCH_CONSTRAINT <br/><seealso cref="ChainSpread"/> -- the elements
        /// will be spread out <br/><seealso cref="ChainSpreadInside"/> -- similar, but the
        /// endpoints of the chain will not be spread out <br/><seealso cref="ChainPacked"/> -- the
        /// elements of the chain will be packed together. The vertical bias attribute of the child
        /// will then affect the positioning of the packed elements <br/>
        /// </summary>
        /// <param name="viewId">ID of view to adjust its VerticalChainStyle</param>
        /// <param name="chainStyle">
        /// the weight that we can use to distribute the horizontal space
        /// </param>
        public virtual void SetVerticalChainStyle(int viewId, int chainStyle)
        {
            get(viewId).layout.verticalChainStyle = chainStyle;
        }

        /// <summary>
        /// Adds a view to a horizontal chain.
        /// </summary>
        /// <param name="viewId">view to add</param>
        /// <param name="leftId">view in chain to the left</param>
        /// <param name="rightId">view in chain to the right</param>
        public virtual void AddToHorizontalChain(int viewId, int leftId, int rightId)
        {
            Connect(viewId, Left, leftId, (leftId == ParentId) ? Left : Right, 0);
            Connect(viewId, Right, rightId, (rightId == ParentId) ? Right : Left, 0);
            if (leftId != ParentId)
            {
                Connect(leftId, Right, viewId, Left, 0);
            }
            if (rightId != ParentId)
            {
                Connect(rightId, Left, viewId, Right, 0);
            }
        }

        /// <summary>
        /// Adds a view to a horizontal chain.
        /// </summary>
        /// <param name="viewId">view to add</param>
        /// <param name="leftId">view to the start side</param>
        /// <param name="rightId">view to the end side</param>
        public virtual void AddToHorizontalChainRTL(int viewId, int leftId, int rightId)
        {
            Connect(viewId, Start, leftId, (leftId == ParentId) ? Start : End, 0);
            Connect(viewId, End, rightId, (rightId == ParentId) ? End : Start, 0);
            if (leftId != ParentId)
            {
                Connect(leftId, End, viewId, Start, 0);
            }
            if (rightId != ParentId)
            {
                Connect(rightId, Start, viewId, End, 0);
            }
        }

        /// <summary>
        /// Adds a view to a vertical chain.
        /// </summary>
        /// <param name="viewId">view to add to a vertical chain</param>
        /// <param name="topId">view above.</param>
        /// <param name="bottomId">view below</param>
        public virtual void AddToVerticalChain(int viewId, int topId, int bottomId)
        {
            Connect(viewId, Top, topId, (topId == ParentId) ? Top : Bottom, 0);
            Connect(viewId, Bottom, bottomId, (bottomId == ParentId) ? Bottom : Top, 0);
            if (topId != ParentId)
            {
                Connect(topId, Bottom, viewId, Top, 0);
            }
            if (bottomId != ParentId)
            {
                Connect(bottomId, Top, viewId, Bottom, 0);
            }
        }

        /// <summary>
        /// Removes a view from a vertical chain. This assumes the view is connected to a vertical
        /// chain. Its behaviour is undefined if not part of a vertical chain.
        /// </summary>
        /// <param name="viewId">the view to be removed</param>
        public virtual void RemoveFromVerticalChain(int viewId)
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
                        Connect(topId, Bottom, bottomId, Top, 0);
                        Connect(bottomId, Top, topId, Bottom, 0);
                    }
                    else if (constraint.layout.bottomToBottom != Layout.UNSET)
                    {
                        // top connected to view. Bottom connected to parent
                        Connect(topId, Bottom, constraint.layout.bottomToBottom, Bottom, 0);
                    }
                    else if (constraint.layout.topToTop != Layout.UNSET)
                    {
                        // bottom connected to view. Top connected to parent
                        Connect(bottomId, Top, constraint.layout.topToTop, Top, 0);
                    }
                }
            }
            Clear(viewId, Top);
            Clear(viewId, Bottom);
        }

        /// <summary>
        /// Removes a view from a horizontal chain. This assumes the view is connected to a
        /// horizontal chain. Its behaviour is undefined if not part of a horizontal chain.
        /// </summary>
        /// <param name="viewId">the view to be removed</param>
        public virtual void RemoveFromHorizontalChain(int viewId)
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
                        Connect(leftId, Right, rightId, Left, 0);
                        Connect(rightId, Left, leftId, Right, 0);
                    }
                    else if (constraint.layout.rightToRight != Layout.UNSET)
                    {
                        // left connected to view. right connected to parent
                        Connect(leftId, Right, constraint.layout.rightToRight, Right, 0);
                    }
                    else if (constraint.layout.leftToLeft != Layout.UNSET)
                    {
                        // right connected to view. left connected to parent
                        Connect(rightId, Left, constraint.layout.leftToLeft, Left, 0);
                    }
                    Clear(viewId, Left);
                    Clear(viewId, Right);
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
                            Connect(startId, End, endId, Start, 0);
                            Connect(endId, Start, leftId, End, 0);
                        }
                        else if (endId != Layout.UNSET)
                        {
                            if (constraint.layout.rightToRight != Layout.UNSET)
                            {
                                // left connected to view. right connected to parent
                                Connect(leftId, End, constraint.layout.rightToRight, End, 0);
                            }
                            else if (constraint.layout.leftToLeft != Layout.UNSET)
                            {
                                // right connected to view. left connected to parent
                                Connect(endId, Start, constraint.layout.leftToLeft, Start, 0);
                            }
                        }
                    }
                    Clear(viewId, Start);
                    Clear(viewId, End);
                }
            }
        }

        /// <summary>
        /// Creates a ConstraintLayout virtual object. Currently only horizontal or vertical
        /// GuideLines.
        /// </summary>
        /// <param name="guidelineID">ID of guideline to create</param>
        /// <param name="orientation">the Orientation of the guideline</param>
        /// public virtual void create(int guidelineID, int orientation)
        public virtual void Create(int guidelineID, int orientation)
        {
            Constraint constraint = get(guidelineID);
            constraint.layout.mIsGuideline = true;
            constraint.layout.orientation = orientation;
        }

        /// <summary>
        /// Creates a ConstraintLayout Barrier object.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="direction">Barrier.{LEFT,RIGHT,TOP,BOTTOM,START,END}</param>
        /// <param name="referenced">@since 1.1</param>
        public virtual void CreateBarrier(int id, int direction, int margin, params int[] referenced)
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
        /// <param name="guidelineID">ID of the guideline</param>
        /// <param name="margin">the distance to the top or left edge</param>
        public virtual void SetGuidelineBegin(int guidelineID, int margin)
        {
            get(guidelineID).layout.guideBegin = margin;
            get(guidelineID).layout.guideEnd = Layout.UNSET;
            get(guidelineID).layout.guidePercent = Layout.UNSET;
        }

        /// <summary>
        /// Set a guideline's distance to end.
        /// </summary>
        /// <param name="guidelineID">ID of the guideline</param>
        /// <param name="margin">the margin to the right or bottom side of container</param>
        public virtual void SetGuidelineEnd(int guidelineID, int margin)
        {
            get(guidelineID).layout.guideEnd = margin;
            get(guidelineID).layout.guideBegin = Layout.UNSET;
            get(guidelineID).layout.guidePercent = Layout.UNSET;
        }

        /// <summary>
        /// Set a Guideline's percent.
        /// </summary>
        /// <param name="guidelineID">ID of the guideline</param>
        /// <param name="ratio">
        /// the ratio between the gap on the left and right 0.0 is top/left 0.5 is middle
        /// </param>
        public virtual void SetGuidelinePercent(int guidelineID, float ratio)
        {
            get(guidelineID).layout.guidePercent = ratio;
            get(guidelineID).layout.guideEnd = Layout.UNSET;
            get(guidelineID).layout.guideBegin = Layout.UNSET;
        }

        /// <summary>
        /// get the reference id's of a helper.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>array of id's</returns>
        public virtual int[] GetReferencedIds(int id)
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
        /// <param name="id"></param>
        /// <param name="referenced">@since 2.0</param>
        public virtual void SetReferencedIds(int id, params int[] referenced)
        {
            Constraint constraint = get(id);
            constraint.layout.mReferenceIds = referenced;
        }

        /// <summary>
        /// set the barrier type (Barrier.LEFT, Barrier.TOP, Barrier.RIGHT, Barrier.BOTTOM,
        /// Barrier.END, Barrier.START)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public virtual void SetBarrierType(int id, int type)
        {
            Constraint constraint = get(id);
            constraint.layout.mHelperType = type;
        }

        private Constraint get(int id)
        {
            if (!mConstraints.ContainsKey(id))
            {
                var newConstraint = new Constraint();
                mConstraints[id] = newConstraint;
                newConstraint.layout.mWidth = ConstraintSet.WrapContent;//@zhouyang Add:Default set WrapContent,it is more useful
                newConstraint.layout.mHeight = ConstraintSet.WrapContent;
            }
            return mConstraints[id];
        }

        private string sideToString(int side)
        {
            switch (side)
            {
                case Left:
                    return "left";

                case Right:
                    return "right";

                case Top:
                    return "top";

                case Bottom:
                    return "bottom";

                case Baseline:
                    return "baseline";

                case Start:
                    return "start";

                case End:
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
    }
}