using System;
using System.Collections.Generic;

//using System.Diagnostics;

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

using static SharpConstraintLayout.Maui.Widget.ConstraintSet;

namespace SharpConstraintLayout.Maui.Widget
{
    using androidx.constraintlayout.core.widgets;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using GuidelineWidget = androidx.constraintlayout.core.widgets.Guideline;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    using VirtualLayoutWidget = androidx.constraintlayout.core.widgets.VirtualLayout;

#if WINDOWS
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Foundation;
#elif __IOS__

    using Panel = UIKit.UIView;

    using UIElement = UIKit.UIView;

    using CoreGraphics;
    using Size = CoreGraphics.CGSize;
    using Microsoft.Maui.Graphics;

#elif __ANDROID__
#endif

    /// <summary>
    /// ConstraintLayout is a AndroidX layout for <see
    /// href="https://developer.android.com/reference/androidx/constraintlayout/widget/ConstraintLayout">google</see>.
    /// Now you can use it at iOS and WinUI, that means you can reuse some android layout code when
    /// use UIKit and WinUI. <br/>
    ///
    /// Notice the size of ConstraintLayout need be set by its parent. Such as add a
    /// ConstraintLayout to UIView and constrainted size is <see cref="ConstraintSet.MatchParent"/>,
    /// if you not set
    /// <code>AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight</code>
    /// ,when you rotate screen, maybe constraintlayout's width and height is not correct. <br/>
    /// Another, not set <see cref="ConstraintSet.MatchParent"/> when Parent can have infinity size,
    /// such as StackPanel can have infinity height at WinUI, if you set ConstraintLayout is <see
    /// cref="ConstraintSet.MatchParent"/>, it will throw exception or get false size.
    /// </summary>
    public class ConstraintLayout : Panel, IConstraintLayout
    {
        public const string VERSION = "ConstraintLayout-2.1.1";
        public const bool ISSUPPORTRTLPLATFORM = false;//现在完全不支持,需要对照原来的代码看删除了那些关于Rtl的

        /// <summary>
        /// if is true,will print some layout info.
        /// </summary>
        public static bool DEBUG = false;
        /// <summary>
        /// if is true,will print time of measure+layout spend.
        /// </summary>
        public static bool MEASURE = false;

        private readonly ConstraintWidgetContainer mLayout = new ConstraintWidgetContainer();//default widget
        private readonly Dictionary<int, UIElement> idToViews = new Dictionary<int, UIElement>();
        public readonly Dictionary<int, ConstraintWidget> idsToConstraintWidgets = new Dictionary<int, ConstraintWidget>();
        private Dictionary<int, ConstraintWidget> mTempMapIdToWidget = new Dictionary<int, ConstraintWidget>();

        // This array keep a list of helper objects if they are present
        private List<ConstraintHelper> mConstraintHelpers = new List<ConstraintHelper>(4);

        /// <summary>
        /// 使用其中的Constraints保存布局信息,其用来替代View的LayoutParams,所以每个View对应一个LayoutParams
        /// </summary>
        public ConstraintSet mConstraintSet { get; set; }

        private int mOptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;

        private MeasurerAnonymousInnerClass mMeasurer;
        private androidx.constraintlayout.core.Metrics mMetrics;

        public ConstraintLayout()
        {
            mLayout.CompanionWidget = this;
            mConstraintSet = new ConstraintSet();//默认创建一个保存最开始的Constraints,之后新建的clone其中的信息
            //constraintLayout have default Widget,need add it to dictionary,we can use it like other child later.
            //ConstraintSet.Constraints.Add(ConstraintSet.PARENT_ID, new ConstraintSet.Constraint());//对于Layout,都用ParentID代替GetHashCode,这是因为Layout可以在ApplyTo时替换
            //这里换种思路,不管是ParentId还是HashCode对应的应该都是同一个约束,修改也是同一个
            var rootConstraint = new ConstraintSet.Constraint();
            mConstraintSet.Constraints.Add(ConstraintSet.ParentId, rootConstraint);
            mConstraintSet.Constraints.Add(this.GetId(), rootConstraint);

            mLayout.Measurer = mMeasurer = new MeasurerAnonymousInnerClass(this);

            //ClipToBounds = true;//view in ConstraintLayout always easy out of bounds
        }

        #region Add And Remove

#if WINDOWS
        public void AddView(UIElement element)
        {
            Children?.Add(element);
            OnAddedView(element);
        }

        public void RemoveView(UIElement element)
        {
            Children?.Remove(element);
            OnRemovedView(element);
        }

        public void RemoveAllViews()
        {
            foreach (var element in this.Children)
            {
                Children?.Remove(element);
                OnRemovedView(element);
            }
        }

        public int ChildCount { get { return Children != null ? Children.Count : 0; } }
#elif __IOS__

        private UIElement[] Children => Subviews;

        public override void AddSubview(UIElement view)
        {
            base.AddSubview(view);
            OnAddedView(view);
        }

        public void AddView(UIElement element)
        {
            base.AddSubview(element);
            OnAddedView(element);
        }

        public void RemoveView(UIElement element)
        {
            element.RemoveFromSuperview();
            OnRemovedView(element);
        }

        public void RemoveAllViews()
        {
            foreach (var element in this.Subviews)
            {
                element.RemoveFromSuperview();
                OnRemovedView(element);
            }
        }

        public int ChildCount => Subviews.Length;

#endif

        protected void OnAddedView(UIElement element)
        {
            if (element == null)
            {
                return;
            }

            var id = element.GetId();

            //Add to three dictionary(Wighet,View,Constraints)
            ConstraintWidget widget = CreateOrGetWidgetAndAddToLayout(id);
            mConstraintSet.Constraints.Add(id, new ConstraintSet.Constraint());
            idToViews.Add(id, element as UIElement);
            // Do stuff with the added object
            if (element is Guideline)//Guideline have default widget
            {
                Guideline guideLine = (Guideline)element;
                idsToConstraintWidgets[id] = guideLine.mGuideline;//替换widget类型
                mConstraintSet.Constraints[id].layout.mIsGuideline = true;
            }

            if (element is ConstraintHelper)//ConstraintHelper also have default widget
            {
                ConstraintHelper helper = (ConstraintHelper)element;
                helper.ValidateParams(idsToConstraintWidgets);//其中会替换widget类型
                if (!mConstraintHelpers.Contains(helper))
                {
                    mConstraintHelpers.Add(helper);
                }
                mConstraintSet.Constraints[id].layout.isHelper = true;
            }
        }

        protected void OnRemovedView(UIElement element)
        {
            if (element == null)
            {
                return;
            }

            var id = element.GetId();
            idToViews.Remove(id);
            ConstraintWidget widget = CreateOrGetWidgetAndAddToLayout(id);
            idsToConstraintWidgets.Remove(id);
            if (element is ConstraintHelper)
                mConstraintHelpers.Remove(element as ConstraintHelper);
        }

        private ConstraintWidget CreateOrGetWidgetAndAddToLayout(int id)
        {
            if (id == this.GetId() || id == ConstraintSet.ParentId)
            {
                return MLayoutWidget;
            }
            else
            {
                if (idsToConstraintWidgets.ContainsKey(id))
                    return idsToConstraintWidgets[id];
                else
                {
                    ConstraintWidget widget = new ConstraintWidget();
                    idsToConstraintWidgets.Add(id, widget);
                    return widget;
                }
            }
        }

        #endregion Add And Remove

        #region Get

        /// <summary>
        /// default widget of ConstraintLayout. you can constrol more action by it.
        /// </summary>
        public virtual ConstraintWidgetContainer MLayoutWidget
        {
            get => mLayout;
        }

        public int OptimizationLevel
        {
            get => mOptimizationLevel;
            set { mOptimizationLevel = value; MLayoutWidget.OptimizationLevel = value; }
        }

        /// <summary>
        /// get widget of this view.
        /// </summary>
        /// <param name="view"></param>
        /// <returns>widget is a virtual control for caculate constraint</returns>
        public ConstraintWidget GetViewWidget(UIElement view)
        {
            if (view == this)
                return MLayoutWidget;
            return this.idsToConstraintWidgets[view.GetId()];
        }

        public UIElement FindViewById(int id)
        {
            return idToViews[id];
        }

        #endregion Get

        #region Measure

        long measureTime = 0;
#if WINDOWS
        protected override Size MeasureOverride(Size availableSize)
#elif __IOS__

        protected Size MeasureOverride(Size availableSize)
#endif
        {
            if (MEASURE)
            {
                measureTime = DateTimeHelperClass.CurrentUnixTimeMillis();
            }

            if (DEBUG) Debug.WriteLine($"{nameof(MeasureOverride)} {this} {availableSize}");

            /*
                * Update Constraints to  wigets
                */
            if (mConstraintSet.IsChanged)
            {
                //update widget of child
                if (updateHierarchy())
                {
                    MLayoutWidget.updateHierarchy();
                }

                //update widget of Container layout
                var constraint = mConstraintSet.Constraints[this.GetId()];
                applyConstraintsFromLayoutParams(false, this, MLayoutWidget, constraint, idsToConstraintWidgets);

                mConstraintSet.IsChanged = false;
            }
#if WINDOWS
            /*
             * Analysis available size
             */
            int availableWidth = 0;
            int availableHeight = 0;
            bool isInfinityAvailabelSize = false;
            //sometimes no fixsize
            if (double.IsPositiveInfinity(availableSize.Width))
            {
                availableWidth = int.MaxValue; isInfinityAvailabelSize = true;
            }
            else
                availableWidth = (int)availableSize.Width;

            if (double.IsPositiveInfinity(availableSize.Height))
            {
                availableHeight = int.MaxValue;
                isInfinityAvailabelSize = true;
            }
            else
            {
                availableHeight = (int)availableSize.Height;
            }

            //If a direction available value and we need MATCH_PARENT, that always generate mistake result, so we not accept it. please modify constraint.
            if (isInfinityAvailabelSize && (MLayoutWidget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT || MLayoutWidget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.MATCH_PARENT))
            {
                var errorStr = $"ConstraintLayout's parent {this.Parent} gived ConstraintLayout a infinity size, you set ConstraintLayout have MATCH_PARENT size, ConstraintLayout can't generate correct result.";
                throw new InvalidOperationException(errorStr);
            }
#elif __IOS__
            //iOS中,如果指定了Frame,那么能获得自身的值,如果没有,那么可以取Superview的Frame,因为布局child必须要layout的大小去参照,iOS开始会Measure两次,能拿到Superview的大小
            int availableWidth = (int)availableSize.Width;
            int availableHeight = (int)availableSize.Height;
#endif
            int horizontalDimension = availableWidth;
            int verticalDimension = availableHeight;

            int horizontalSpec = 0;
            int verticalSpec = 0;

            switch (MLayoutWidget.HorizontalDimensionBehaviour)
            {
                case ConstraintWidget.DimensionBehaviour.FIXED:
                    {
                        //已知是Fixed时说明默认没有赋值或者赋值了,赋值了的话原始的约束绝对有值,那么Widget.Width不用变,没有赋值的直接按available大小处理,因为
                        //父布局会指定值,而我们不指定则代表遵从父布局的指定
                        if (mConstraintSet.Constraints[this.GetId()].layout.mWidth > 0)//用原始的约束来判断是否为固定值,如果是WrapContent,MatchParent,MatchContraint,则<=0
                            horizontalDimension = MLayoutWidget.Width;
                        horizontalSpec = MeasureSpec.MakeMeasureSpec(horizontalDimension, MeasureSpec.EXACTLY);
                    }
                    break;
                case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                    {
                        horizontalSpec = MeasureSpec.MakeMeasureSpec(horizontalDimension, MeasureSpec.AT_MOST);
                    }
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    {
                        horizontalSpec = MeasureSpec.MakeMeasureSpec(horizontalDimension, MeasureSpec.EXACTLY);
                    }
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                    {
                        horizontalSpec = MeasureSpec.MakeMeasureSpec(horizontalDimension, MeasureSpec.EXACTLY);
                    }
                    break;
            }

            switch (MLayoutWidget.VerticalDimensionBehaviour)
            {
                case ConstraintWidget.DimensionBehaviour.FIXED:
                    {
                        // 已知是Fixed时说明默认没有赋值或者赋值了,赋值了的话原始的约束绝对有值,那么Widget.Width不用变,没有赋值的直接按available大小处理,因为
                        //父布局会指定值,而我们不指定则代表遵从父布局的指定
                        if (mConstraintSet.Constraints[this.GetId()].layout.mHeight > 0)//用原始的约束来判断是否为固定值,如果是WrapContent,MatchParent,MatchContraint,则<=0
                            verticalDimension = MLayoutWidget.Height;
                        verticalSpec = MeasureSpec.MakeMeasureSpec(verticalDimension, MeasureSpec.EXACTLY);
                    }
                    break;
                case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                    {
                        verticalSpec = MeasureSpec.MakeMeasureSpec(verticalDimension, MeasureSpec.AT_MOST);
                    }
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    {
                        verticalSpec = MeasureSpec.MakeMeasureSpec(verticalDimension, MeasureSpec.EXACTLY);
                    }
                    break;
                case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                    {
                        verticalSpec = MeasureSpec.MakeMeasureSpec(verticalDimension, MeasureSpec.EXACTLY);
                    }
                    break;
            }

            OnMeasure(horizontalSpec, verticalSpec);

            /*#if WINDOWS
                        *//*
                         * First measure all WRAP_CONTENT child size,we need know some default size.
                         *//*
                        var layoutAvailableSize = new Size(MLayoutWidget.Width, MLayoutWidget.Height);
                        foreach (UIElement child in Children)
                        {
                            var widget = GetViewWidget(child);
                            //依赖自己大小的先测量
                            if (widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT
                                && widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                                child.Measure(availableSize);
                        }
            #endif*/

            /*
             * Other MATCH_PARENT and MATCH_CONSTRAINT we let ConstraintLayout.Core analysis.
             */

            //windows需要返回值
            return new Size(MLayoutWidget.Width, MLayoutWidget.Height);
        }

        /// <summary>
        /// Handles calling setMeasuredDimension()
        /// </summary>
        /// <param name="widthMeasureSpec"></param>
        /// <param name="heightMeasureSpec"></param>
        /// <param name="measuredWidth"></param>
        /// <param name="measuredHeight"></param>
        /// <param name="isWidthMeasuredTooSmall"></param>
        /// <param name="isHeightMeasuredTooSmall"></param>
        protected void resolveMeasuredDimension(int widthMeasureSpec, int heightMeasureSpec,
                                                int measuredWidth, int measuredHeight,
                                                bool isWidthMeasuredTooSmall, bool isHeightMeasuredTooSmall)
        {
            int childState = 0;
            int heightPadding = mMeasurer.paddingHeight;
            int widthPadding = mMeasurer.paddingWidth;

            int androidLayoutWidth = measuredWidth + widthPadding;
            int androidLayoutHeight = measuredHeight + heightPadding;

            int resolvedWidthSize = MeasureSpec.resolveSizeAndState(androidLayoutWidth, widthMeasureSpec, childState);
            int resolvedHeightSize = MeasureSpec.resolveSizeAndState(androidLayoutHeight, heightMeasureSpec,
                    childState << MeasureSpec.MEASURED_HEIGHT_STATE_SHIFT);
            resolvedWidthSize &= MeasureSpec.MEASURED_SIZE_MASK;
            resolvedHeightSize &= MeasureSpec.MEASURED_SIZE_MASK;
            resolvedWidthSize = Math.Min(mMaxWidth, resolvedWidthSize);
            resolvedHeightSize = Math.Min(mMaxHeight, resolvedHeightSize);
            if (isWidthMeasuredTooSmall)
            {
                resolvedWidthSize |= MeasureSpec.MEASURED_STATE_TOO_SMALL;
            }
            if (isHeightMeasuredTooSmall)
            {
                resolvedHeightSize |= MeasureSpec.MEASURED_STATE_TOO_SMALL;
            }
            //setMeasuredDimension(resolvedWidthSize, resolvedHeightSize);
            mLastMeasureWidth = resolvedWidthSize;
            mLastMeasureHeight = resolvedHeightSize;
        }

        private void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var temp = this;
            MLayoutWidget.Rtl = isRtl();

            resolveSystem(MLayoutWidget, mOptimizationLevel, widthMeasureSpec, heightMeasureSpec);
            resolveMeasuredDimension(widthMeasureSpec, heightMeasureSpec, MLayoutWidget.Width, MLayoutWidget.Height,
                    MLayoutWidget.WidthMeasuredTooSmall, MLayoutWidget.HeightMeasuredTooSmall);

            if (MEASURE)
            {
                measureTime = DateTimeHelperClass.CurrentUnixTimeMillis() - measureTime;
                Debug.WriteLine(MLayoutWidget.DebugName + " (" + ChildCount + ") DONE onMeasure width: " + MeasureSpec.ToString(widthMeasureSpec)
                        + " height: " + MeasureSpec.ToString(heightMeasureSpec) + " => " + mLastMeasureWidth + " x " + mLastMeasureHeight
                        + " lasted " + measureTime
                );
            }
        }

        private int PaddingTop = 0;
        private int PaddingBottom = 0;
        private int PaddingLeft = 0;
        private int PaddingRight = 0;
        private int PaddingStart = 0;
        private int PaddingEnd = 0;

        private int PaddingWidth() => PaddingRight - PaddingLeft;

        // Cache last measure
        private int mLastMeasureWidth = -1;

        private int mLastMeasureHeight = -1;
        private int mLastMeasureWidthSize = -1;
        private int mLastMeasureHeightSize = -1;
        private int mLastMeasureWidthMode = MeasureSpec.UNSPECIFIED;
        private int mLastMeasureHeightMode = MeasureSpec.UNSPECIFIED;

        /// <summary>
        /// Handles measuring a layout
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="optimizationLevel"></param>
        /// <param name="widthMeasureSpec"></param>
        /// <param name="heightMeasureSpec"></param>
        protected void resolveSystem(ConstraintWidgetContainer layout, int optimizationLevel, int widthMeasureSpec, int heightMeasureSpec)
        {
            int widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            int heightMode = MeasureSpec.GetMode(heightMeasureSpec);
            int heightSize = MeasureSpec.GetSize(heightMeasureSpec);

            int paddingY = Math.Max(0, PaddingTop);
            int paddingBottom = Math.Max(0, PaddingBottom);
            int paddingHeight = paddingY + paddingBottom;
            int paddingWidth = PaddingWidth();
            int paddingX;
            //传入布局的测量数据,用于测量child时
            mMeasurer.captureLayoutInfo(widthMeasureSpec, heightMeasureSpec, paddingY, paddingBottom, paddingWidth, paddingHeight);

            int paddingStart = Math.Max(0, PaddingStart);
            int paddingEnd = Math.Max(0, PaddingEnd);
            if (paddingStart > 0 || paddingEnd > 0)
            {
                if (isRtl())
                {
                    paddingX = paddingEnd;
                }
                else
                {
                    paddingX = paddingStart;
                }
            }
            else
            {
                paddingX = Math.Max(0, PaddingLeft);
            }

            widthSize -= paddingWidth;
            heightSize -= paddingHeight;

            setSelfDimensionBehaviour(layout, widthMode, widthSize, heightMode, heightSize);
            layout.measure(optimizationLevel, widthMode, widthSize, heightMode, heightSize, mLastMeasureWidth, mLastMeasureHeight, paddingX, paddingY);
        }

        private int mMinWidth = 0;
        private int mMinHeight = 0;
        private int mMaxWidth = int.MaxValue;
        private int mMaxHeight = int.MaxValue;

        protected void setSelfDimensionBehaviour(ConstraintWidgetContainer layout, int widthMode, int widthSize, int heightMode, int heightSize)
        {
            int heightPadding = mMeasurer.paddingHeight;
            int widthPadding = mMeasurer.paddingWidth;

            ConstraintWidget.DimensionBehaviour widthBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            ConstraintWidget.DimensionBehaviour heightBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

            int desiredWidth = 0;
            int desiredHeight = 0;
            int childCount = ChildCount;

            if (widthMode == MeasureSpec.AT_MOST)
            {
                {
                    widthBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    desiredWidth = widthSize;
                    if (childCount == 0)
                    {
                        desiredWidth = Math.Max(0, mMinWidth);
                    }
                }
            }
            else if (widthMode == MeasureSpec.UNSPECIFIED)
            {
                {
                    widthBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    if (childCount == 0)
                    {
                        desiredWidth = Math.Max(0, mMinWidth);
                    }
                }
            }
            else if (widthMode == MeasureSpec.EXACTLY)
            {
                {
                    desiredWidth = Math.Min(mMaxWidth - widthPadding, widthSize);
                }
            }

            if (heightMode == MeasureSpec.AT_MOST)
            {
                heightBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                desiredHeight = heightSize;
                if (childCount == 0)
                {
                    desiredHeight = Math.Max(0, mMinHeight);
                }
            }
            else if (heightMode == MeasureSpec.UNSPECIFIED)
            {
                heightBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                if (childCount == 0)
                {
                    desiredHeight = Math.Max(0, mMinHeight);
                }
            }
            else if (heightMode == MeasureSpec.EXACTLY)
            {
                desiredHeight = Math.Min(mMaxHeight - heightPadding, heightSize);
            }

            if (desiredWidth != layout.Width || desiredHeight != layout.Height)
            {
                layout.invalidateMeasures();
            }
            layout.X = 0;
            layout.Y = 0;
            layout.MaxWidth = mMaxWidth - widthPadding;
            layout.MaxHeight = mMaxHeight - heightPadding;
            layout.MinWidth = 0;
            layout.MinHeight = 0;
            layout.HorizontalDimensionBehaviour = widthBehaviour;
            layout.Width = desiredWidth;
            layout.VerticalDimensionBehaviour = heightBehaviour;
            layout.Height = desiredHeight;
            layout.MinWidth = mMinWidth - widthPadding;
            layout.MinHeight = mMinHeight - heightPadding;
        }

        protected bool isRtl()
        {
            return false;
        }

        #endregion Measure

        #region Layout

#if WINDOWS
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DEBUG) Debug.WriteLine($"{nameof(ArrangeOverride)} {this} {finalSize}");

            OnLayout();

            return new Size(MLayoutWidget.Width, MLayoutWidget.Height);//这里必须返回Widget的大小,因为返回值决定了layout的绘制范围?
        }

        void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            element.Arrange(new Rect(x, y, w, h));
        }
#endif

#if __IOS__
        public override Size IntrinsicContentSize => this.Frame.Size;

        /// <summary>
        /// iOS don't have measure method, but at first show, it will load LayoutSubviews twice,
        /// that means we can get child size at second time layout.
        /// </summary>
        public override void LayoutSubviews()
        {
            //base.LayoutSubviews();
            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {this} {this.Frame}");
            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} {Superview} {this.Superview?.Frame}");

            //得到自身或Superview的大小作为availableSize
            //if (this.Frame.Size.Width > 0)
            //{
            //    MeasureOverride(this.Frame.Size);
            //}
            //else
            {
                if (Superview != null)
                    MeasureOverride(this.Superview.Frame.Size);
            }

            if (DEBUG) Debug.WriteLine($"{nameof(LayoutSubviews)} RootWidget {this.MLayoutWidget.ToString()}");

            //更新layout的大小
            if (this.Frame.Size.Width > 0)
            {
                Frame = new CGRect(Frame.X, Frame.Y, MLayoutWidget.Width, MLayoutWidget.Height);
            }
            else
            {
                if (Superview != null)
                    Frame = new CGRect(this.Superview.Frame.X, this.Superview.Frame.Y, MLayoutWidget.Width, MLayoutWidget.Height);
            }

            OnLayout();
        }

        private void LayoutChild(UIElement element, int x, int y, int w, int h)
        {
            element.Frame = new CoreGraphics.CGRect(x, y, w, h);
        }

#endif

        private void OnLayout()
        {
            //layout child
            foreach (ConstraintWidget child in mLayout.Children)
            {
                UIElement component = (UIElement)child.CompanionWidget;

                if (child.Visibility == Gone && !(component is Guideline) && !(component is ConstraintHelper) && !(component is VirtualLayout))
                {
                    // If we are in edit mode, let's layout the widget so that they are at "the
                    // right place" visually in the editor (as we get our positions from layoutlib)
                    continue;
                }

                if (child.InPlaceholder)
                {
                    continue;
                }

                if (component != null)
                {
#if WINDOWS
                    if (component is TextBox)
                    {
                        var temp = child.Width;
                    }
#endif
                    if (DEBUG) Debug.WriteLine($"{nameof(OnLayout)} {component} {new Rect(child.X, child.Y, child.Width, child.Height)}");
                    LayoutChild(component, child.X, child.Y, child.Width, child.Height);
                }

                if (component is Placeholder)
                {
                    Placeholder holder = (Placeholder)component;
                    UIElement content = holder.Content;
                    if (content != null)
                    {
                        ViewExtension.SetViewVisibility(content, ConstraintSet.Visible);
                        LayoutChild(content, child.X, child.Y, child.Width, child.Height);
                    }
                }
            }
        }

        #endregion Layout

        private class MeasurerAnonymousInnerClass : BasicMeasure.Measurer
        {
            private readonly ConstraintLayout outerInstance;

            internal int paddingTop = 0;
            internal int paddingBottom = 0;
            internal int paddingWidth = 0;
            internal int paddingHeight = 0;
            internal int layoutWidthSpec = 0;
            internal int layoutHeightSpec = 0;

            public virtual void captureLayoutInfo(int widthSpec, int heightSpec, int top, int bottom, int width, int height)
            {
                paddingTop = top;
                paddingBottom = bottom;
                paddingWidth = width;
                paddingHeight = height;
                layoutWidthSpec = widthSpec;
                layoutHeightSpec = heightSpec;
            }

            public MeasurerAnonymousInnerClass(ConstraintLayout outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
#if WPF
                if (widget is VirtualLayout)
                    outerInstance.measureFlow(widget, measure);
                else
                    outerInstance.innerMeasure(widget, measure);
#elif WINDOWS || __IOS__
                //AndroidSourceCodeMeasureNoSpec(widget, measure);
                //SimpleMeasure(widget, measure);
                AndroidSourceCodeMeasureUseSpecForWindows(widget, measure);
#endif
            }

            public virtual void didMeasures()
            {
                int widgetsCount = outerInstance.ChildCount;
                for (int i = 0; i < widgetsCount; i++)
                {
                    UIElement child = (UIElement)outerInstance.Children[i];
                    if (child is Placeholder)
                    {
                        ((Placeholder)child).UpdatePostMeasure(outerInstance);
                    }
                }
                // TODO refactor into an updatePostMeasure interface
                int helperCount = outerInstance.mConstraintHelpers.Count;
                if (helperCount > 0)
                {
                    for (int i = 0; i < helperCount; i++)
                    {
                        ConstraintHelper helper = outerInstance.mConstraintHelpers[i];
                        helper.UpdatePostMeasure(outerInstance);
                    }
                }
            }

            /// <summary>
            /// Spec是Android中根据parent和child的matchparent,wrapcontent的行为进行计算的一个中介.
            /// 我看到ConstraintWidget中也包含这个逻辑,考虑是否应该用Android中的判断逻辑.
            /// 
            /// For Windows是因为Windows也有Measure,而iOS没有Measure,iOS应该能精简更多代码,因为无需传入Parent的参数就能测量自身,那么只需要靠ConstraintWidget自己去测量然后布局就够了.
            /// </summary>
            /// <param name="widget"></param>
            /// <param name="measure"></param>
            public void AndroidSourceCodeMeasureUseSpecForWindows(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
                if (widget == null)
                {
                    return;
                }
                if (widget.Visibility == ConstraintWidget.GONE && !widget.InPlaceholder)
                {
                    measure.measuredWidth = 0;
                    measure.measuredHeight = 0;
                    measure.measuredBaseline = 0;
                    return;
                }
                if (widget.Parent == null)
                {
                    return;
                }

                long startMeasure = 0;
                long endMeasure = 0;

                if (MEASURE)
                {
                    startMeasure = DateTimeHelperClass.nanoTime();
                }

                ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.horizontalBehavior;
                ConstraintWidget.DimensionBehaviour verticalBehavior = measure.verticalBehavior;

                int horizontalDimension = measure.horizontalDimension;
                int verticalDimension = measure.verticalDimension;

                int horizontalSpec = 0;
                int verticalSpec = 0;

                int heightPadding = paddingTop + paddingBottom;
                int widthPadding = paddingWidth;

                var child = (UIElement)widget.CompanionWidget;

                switch (horizontalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        {
                            horizontalSpec = MeasureSpec.MakeMeasureSpec(horizontalDimension, MeasureSpec.EXACTLY);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        {
                            horizontalSpec = MeasureSpec.getChildMeasureSpec(layoutWidthSpec, widthPadding, ConstraintSet.WrapContent);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        {
                            // Horizontal spec must account for margin as well as padding here.
                            horizontalSpec = MeasureSpec.getChildMeasureSpec(layoutWidthSpec, widthPadding + widget.HorizontalMargin, ConstraintSet.MatchParent);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        {
                            horizontalSpec = MeasureSpec.getChildMeasureSpec(layoutWidthSpec, widthPadding, ConstraintSet.WrapContent);
                            bool shouldDoWrap = widget.mMatchConstraintDefaultWidth == ConstraintSet.MatchConstraintWrap;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
                                //bool otherDimensionStable = child.MeasuredHeight == widget.Height;//Windows中当前还未Measure,先使用Defalut DesiredSize尝试
                                bool otherDimensionStable = child.GetDefaultSize().Height == widget.Height;
                                bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedHorizontally);
                                if (useCurrent)
                                {
                                    horizontalSpec = MeasureSpec.MakeMeasureSpec(widget.Width, MeasureSpec.EXACTLY);
                                }
                            }
                        }
                        break;
                }

                switch (verticalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        {
                            verticalSpec = MeasureSpec.MakeMeasureSpec(verticalDimension, MeasureSpec.EXACTLY);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        {
                            verticalSpec = MeasureSpec.getChildMeasureSpec(layoutHeightSpec, heightPadding, ConstraintSet.WrapContent);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        {
                            // Vertical spec must account for margin as well as padding here.
                            verticalSpec = MeasureSpec.getChildMeasureSpec(layoutHeightSpec, heightPadding + widget.VerticalMargin, ConstraintSet.MatchParent);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        {
                            verticalSpec = MeasureSpec.getChildMeasureSpec(layoutHeightSpec, heightPadding, ConstraintSet.WrapContent);
                            bool shouldDoWrap = widget.mMatchConstraintDefaultHeight == ConstraintSet.MatchConstraintWrap;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
                                //bool otherDimensionStable = child.MeasuredWidth == widget.Width;//Windows中Measure后的高度应该是DesiredSize
                                bool otherDimensionStable = child.GetDefaultSize().Width == widget.Width;
                                bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedVertically);
                                if (useCurrent)
                                {
                                    verticalSpec = MeasureSpec.MakeMeasureSpec(widget.Height, MeasureSpec.EXACTLY);
                                }
                            }
                        }
                        break;
                }

                ConstraintWidgetContainer container = (ConstraintWidgetContainer)widget.Parent;
                if (container != null && Optimizer.enabled(outerInstance.mOptimizationLevel, Optimizer.OPTIMIZATION_CACHE_MEASURES))
                {
                    //if (child.MeasuredWidth == widget.Width && child.MeasuredWidth < container.Width && child.MeasuredHeight == widget.Height && child.MeasuredHeight < container.Height && child.Baseline == widget.BaselineDistance && !widget.MeasureRequested)//Windows中当前还未Measure,先使用Defalut DesiredSize尝试
                    if (child.GetDefaultSize().Width == widget.Width && child.GetDefaultSize().Width < container.Width && child.GetDefaultSize().Height == widget.Height && child.GetDefaultSize().Height < container.Height && child.GetBaseline() == widget.BaselineDistance && !widget.MeasureRequested)
                    // note: the container check replicates legacy behavior, but we might want
                    // to not enforce that in 3.0
                    {
                        bool similar = isSimilarSpec(widget.LastHorizontalMeasureSpec, horizontalSpec, widget.Width) && isSimilarSpec(widget.LastVerticalMeasureSpec, verticalSpec, widget.Height);
                        if (similar)
                        {
                            measure.measuredWidth = widget.Width;
                            measure.measuredHeight = widget.Height;
                            measure.measuredBaseline = widget.BaselineDistance;
                            // if the dimensions of the solver widget are already the same as the real view, no need to remeasure.
                            if (DEBUG)
                            {
                                Debug.WriteLine("SKIPPED " + widget);
                            }
                            return;
                        }
                    }
                }

                bool horizontalMatchConstraints = (horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                bool verticalMatchConstraints = (verticalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);

                bool verticalDimensionKnown = (verticalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT || verticalBehavior == ConstraintWidget.DimensionBehaviour.FIXED);
                bool horizontalDimensionKnown = (horizontalBehavior == ConstraintWidget.DimensionBehaviour.MATCH_PARENT || horizontalBehavior == ConstraintWidget.DimensionBehaviour.FIXED);
                bool horizontalUseRatio = horizontalMatchConstraints && widget.mDimensionRatio > 0;
                bool verticalUseRatio = verticalMatchConstraints && widget.mDimensionRatio > 0;

                if (child == null)
                {
                    return;
                }
                //LayoutParams @params = (LayoutParams)child.LayoutParams;//Windows中没有LayoutParams,大多数布局数据使用Constraint.Layout存储
                var @params = outerInstance.mConstraintSet.Constraints[child.GetId()].layout;

                int width = 0;
                int height = 0;
                int baseline = 0;

                if ((measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS
                    || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                    || !(horizontalMatchConstraints
                    && widget.mMatchConstraintDefaultWidth == ConstraintSet.MatchConstraintSpread
                    && verticalMatchConstraints
                    && widget.mMatchConstraintDefaultHeight == ConstraintSet.MatchConstraintSpread))
                {
                    if (child is VirtualLayout && widget is androidx.constraintlayout.core.widgets.VirtualLayout)
                    {
                        androidx.constraintlayout.core.widgets.VirtualLayout layout = (androidx.constraintlayout.core.widgets.VirtualLayout)widget;
                        ((VirtualLayout)child).OnMeasure(layout, horizontalSpec, verticalSpec);
                    }
                    else
                    {
                        //child.measure(horizontalSpec, verticalSpec);//Android中传入Spec给child去测量,windows中直接传大小即可
                        child.MeasureSelf(MeasureSpec.GetSize(horizontalSpec), MeasureSpec.GetSize(verticalSpec));
                    }
                    widget.setLastMeasureSpec(horizontalSpec, verticalSpec);

                    (int w, int h) = child.GetMeasuredSize(widget);//Windows中Measure后的高度应该是DesiredSize
                    baseline = (int)child.GetBaseline();

                    width = w;
                    height = h;

                    if (DEBUG)
                    {
                        string measurement = MeasureSpec.ToString(horizontalSpec) + " x " + MeasureSpec.ToString(verticalSpec) + " => " + width + " x " + height;
                        Debug.WriteLine("    (M) measure " + " (" + widget.DebugName + ") : " + measurement);
                    }

                    if (widget.mMatchConstraintMinWidth > 0)
                    {
                        width = Math.Max(widget.mMatchConstraintMinWidth, width);
                    }
                    if (widget.mMatchConstraintMaxWidth > 0)
                    {
                        width = Math.Min(widget.mMatchConstraintMaxWidth, width);
                    }
                    if (widget.mMatchConstraintMinHeight > 0)
                    {
                        height = Math.Max(widget.mMatchConstraintMinHeight, height);
                    }
                    if (widget.mMatchConstraintMaxHeight > 0)
                    {
                        height = Math.Min(widget.mMatchConstraintMaxHeight, height);
                    }

                    bool optimizeDirect = Optimizer.enabled(outerInstance.mOptimizationLevel, Optimizer.OPTIMIZATION_DIRECT);
                    if (!optimizeDirect)
                    {
                        if (horizontalUseRatio && verticalDimensionKnown)
                        {
                            float ratio = widget.mDimensionRatio;
                            width = (int)(0.5f + height * ratio);
                        }
                        else if (verticalUseRatio && horizontalDimensionKnown)
                        {
                            float ratio = widget.mDimensionRatio;
                            height = (int)(0.5f + width / ratio);
                        }
                    }

                    if (w != width || h != height)
                    {
                        if (w != width)
                        {
                            horizontalSpec = MeasureSpec.MakeMeasureSpec(width, MeasureSpec.EXACTLY);
                        }
                        if (h != height)
                        {
                            verticalSpec = MeasureSpec.MakeMeasureSpec(height, MeasureSpec.EXACTLY);
                        }
                        //child.measure(horizontalSpec, verticalSpec);//Android中传入Spec给child去测量,windows中直接传大小即可
                        child.MeasureSelf(MeasureSpec.GetSize(horizontalSpec), MeasureSpec.GetSize(verticalSpec));

                        widget.setLastMeasureSpec(horizontalSpec, verticalSpec);
                        (width, height) = child.GetMeasuredSize(widget);//Windows中Measure后的高度应该是DesiredSize
                        baseline = (int)child.GetBaseline();
                        if (DEBUG)
                        {
                            string measurement2 = MeasureSpec.ToString(horizontalSpec) + " x " + MeasureSpec.ToString(verticalSpec) + " => " + width + " x " + height;
                            Console.WriteLine("measure (b) " + widget.DebugName + " : " + measurement2);
                        }
                    }
                }

                bool hasBaseline = baseline != -1;

                measure.measuredNeedsSolverPass = (width != measure.horizontalDimension) || (height != measure.verticalDimension);
                if (@params.needsBaseline)
                {
                    hasBaseline = true;
                }
                if (hasBaseline && baseline != -1 && widget.BaselineDistance != baseline)
                {
                    measure.measuredNeedsSolverPass = true;
                }
                measure.measuredWidth = width;
                measure.measuredHeight = height;
                measure.measuredHasBaseline = hasBaseline;
                measure.measuredBaseline = baseline;
                if (MEASURE)
                {
                    endMeasure = DateTimeHelperClass.nanoTime();
                    if (outerInstance.mMetrics != null)
                    {
                        outerInstance.mMetrics.measuresWidgetsDuration += (endMeasure - startMeasure);
                    }
                }
            }

            ///<summary>
            ///  Returns true if the previous measure spec is equivalent to the new one.
            ///  - if it's the same...
            ///  - if it's not, but the previous was AT_MOST or UNSPECIFIED and the new one
            ///    is EXACTLY with the same size.
            /// </summary>
            private bool isSimilarSpec(int lastMeasureSpec, int spec, int widgetSize)
            {
                if (lastMeasureSpec == spec)
                {
                    return true;
                }
                int lastMode = MeasureSpec.GetMode(lastMeasureSpec);
                int lastSize = MeasureSpec.GetSize(lastMeasureSpec);
                int mode = MeasureSpec.GetMode(spec);
                int size = MeasureSpec.GetSize(spec);
                if (mode == MeasureSpec.EXACTLY
                        && (lastMode == MeasureSpec.AT_MOST || lastMode == MeasureSpec.UNSPECIFIED)
                        && widgetSize == size)
                {
                    return true;
                }
                return false;
            }

        }

        #region Apply Contrants to Widget

        private bool updateHierarchy()
        {
            //int count = ChildCount;

            //Android中的逻辑是只要有一个布局变了,就需要调用setChildrenConstraints,其也是更新全部的内容,所以这里我直接标记为重新计算
            //bool recompute = false;
            bool recompute = true;
            /*for (int i = 0; i < count; i++)
            {
                //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                UIElement child = Children[i];
                if (child.LayoutRequested)
                {
                    recompute = true;
                    break;
                }
            }*/
            if (recompute)
            {
                setChildrenConstraints();
            }
            return recompute;
        }

        private bool InEditMode = false;

        private void setChildrenConstraints()
        {
            bool isInEditMode = DEBUG || InEditMode;

            int count = ChildCount;

            // Make sure everything is fully reset before anything else.重置W全部idget，之后再设置需要的
            for (int i = 0; i < count; i++)
            {
                UIElement child = Children[i];
                ConstraintWidget widget = GetViewWidget(child);
                if (widget == null)
                {
                    continue;
                }
                widget.reset();//全部widget的数据重置
            }

            /*if (USE_CONSTRAINTS_HELPER && mConstraintSetId != -1)
            {
                for (int i = 0; i < count; i++)
                {
                    //ORIGINAL LINE: final android.view.View child = getChildAt(i);
                    View child = (View)Children[i];
                    if (child.GetHashCode() == mConstraintSetId && child is Constraints)
                    {
                        mConstraintSet = ((Constraints)child).ConstraintSet;
                    }
                }
            }*/

            /*if (ConstraintSet != null)
            {
                //这一步我不执行,因为这一步会清除ConstraintSet,我需要保留这个信息去替换LayoutParams的功能
                //ConstraintSet.applyToInternal(this, true);
            }*/

            MLayoutWidget.removeAllChildren();

            int helperCount = mConstraintHelpers.Count;
            if (helperCount > 0)
            {
                for (int i = 0; i < helperCount; i++)
                {
                    ConstraintHelper helper = mConstraintHelpers[i];
                    helper.UpdatePreLayout(this);//可以整理一下数据
                }
            }

            // TODO refactor into an updatePreLayout interface
            for (int i = 0; i < count; i++)
            {
                UIElement child = Children[i];
                if (child is Placeholder)
                {
                    ((Placeholder)child).UpdatePreLayout(this);
                }
            }

            mTempMapIdToWidget.Clear();
            mTempMapIdToWidget.Add(ConstraintSet.ParentId, MLayoutWidget);
            mTempMapIdToWidget.Add(this.GetId(), MLayoutWidget);
            for (int i = 0; i < count; i++)//添加widgets到临时字典
            {
                UIElement child = Children[i];
                ConstraintWidget widget = GetViewWidget(child);
                mTempMapIdToWidget.Add(child.GetId(), widget);
            }

            for (int i = 0; i < count; i++)//apply constraint到每个widget
            {
                UIElement child = Children[i];
                ConstraintWidget widget = GetViewWidget(child);
                if (widget == null)
                {
                    continue;
                }
                Constraint layoutConstraint = mConstraintSet.Constraints[child.GetId()];
                MLayoutWidget.add(widget);
                applyConstraintsFromLayoutParams(isInEditMode, child, widget, layoutConstraint, mTempMapIdToWidget);
            }
        }

        /// <summary>
        /// Apply constraint to widget
        /// </summary>
        /// <param name="isInEditMode"></param>
        /// <param name="child">view</param>
        /// <param name="widget">view's widget</param>
        /// <param name="layoutParams">constraint</param>
        /// <param name="idToWidget">a widget dict for find widget</param>
        protected internal virtual void applyConstraintsFromLayoutParams(bool isInEditMode, UIElement child, ConstraintWidget widget, Constraint layoutParams, Dictionary<int, ConstraintWidget> idToWidget)
        {
            layoutParams.layout.Validate();//根据已知值设置其他值
            layoutParams.layout.helped = false;

            widget.Visibility = layoutParams.propertySet.visibility;//这里我设置为从constraints中取,涉及布局的都交给constraints

            if (layoutParams.layout.isInPlaceholder)
            {
                widget.InPlaceholder = true;
                widget.Visibility = ConstraintSet.Gone;
            }
            widget.CompanionWidget = child;

            if (child is ConstraintHelper)
            {
                ConstraintHelper helper = (ConstraintHelper)child;
                helper.ResolveRtl(widget, MLayoutWidget.Rtl);
            }

            if (layoutParams.layout.mIsGuideline)
            {
                GuidelineWidget guideline = (GuidelineWidget)widget;
                /*int resolvedGuideBegin = layoutParams.resolvedGuideBegin;
                int resolvedGuideEnd = layoutParams.resolvedGuideEnd;
                float resolvedGuidePercent = layoutParams.resolvedGuidePercent;
                if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1)
                {*/
                var resolvedGuideBegin = layoutParams.layout.guideBegin;
                var resolvedGuideEnd = layoutParams.layout.guideEnd;
                var resolvedGuidePercent = layoutParams.layout.guidePercent;
                /*}*/
                if (resolvedGuidePercent != Unset)
                {
                    //guideline.GuidePercent = resolvedGuidePercent;
                    guideline.setGuidePercent(resolvedGuidePercent);
                }
                else if (resolvedGuideBegin != Unset)
                {
                    guideline.GuideBegin = resolvedGuideBegin;
                }
                else if (resolvedGuideEnd != Unset)
                {
                    guideline.GuideEnd = resolvedGuideEnd;
                }
            }
            else
            {
                // Get the left/right constraints resolved for RTL
                /*int resolvedLeftToLeft = layoutParams.resolvedLeftToLeft;
                int resolvedLeftToRight = layoutParams.resolvedLeftToRight;
                int resolvedRightToLeft = layoutParams.resolvedRightToLeft;
                int resolvedRightToRight = layoutParams.resolvedRightToRight;
                int resolveGoneLeftMargin = layoutParams.resolveGoneLeftMargin;
                int resolveGoneRightMargin = layoutParams.resolveGoneRightMargin;
                float resolvedHorizontalBias = layoutParams.resolvedHorizontalBias;
        */
                /*if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1)
                {*/
                // Pre JB MR1, left/right should take precedence, unless they are not defined and
                // somehow a corresponding start/end constraint exists
                var resolvedLeftToLeft = layoutParams.layout.leftToLeft;
                var resolvedLeftToRight = layoutParams.layout.leftToRight;
                var resolvedRightToLeft = layoutParams.layout.rightToLeft;
                var resolvedRightToRight = layoutParams.layout.rightToRight;
                var resolveGoneLeftMargin = layoutParams.layout.goneLeftMargin;
                var resolveGoneRightMargin = layoutParams.layout.goneRightMargin;
                var resolvedHorizontalBias = layoutParams.layout.horizontalBias;

                if (resolvedLeftToLeft == Unset && resolvedLeftToRight == Unset)
                {
                    if (layoutParams.layout.startToStart != Unset)
                    {
                        resolvedLeftToLeft = layoutParams.layout.startToStart;
                    }
                    else if (layoutParams.layout.startToEnd != Unset)
                    {
                        resolvedLeftToRight = layoutParams.layout.startToEnd;
                    }
                }
                if (resolvedRightToLeft == Unset && resolvedRightToRight == Unset)
                {
                    if (layoutParams.layout.endToStart != Unset)
                    {
                        resolvedRightToLeft = layoutParams.layout.endToStart;
                    }
                    else if (layoutParams.layout.endToEnd != Unset)
                    {
                        resolvedRightToRight = layoutParams.layout.endToEnd;
                    }
                }
                /*}*/

                // Circular constraint
                if (layoutParams.layout.circleConstraint != Unset)
                {
                    ConstraintWidget target = idToWidget[layoutParams.layout.circleConstraint];
                    if (target != null)
                    {
                        widget.connectCircularConstraint(target, layoutParams.layout.circleAngle, layoutParams.layout.circleRadius);
                    }
                }
                else
                {
                    // Left constraint
                    if (resolvedLeftToLeft != Unset)
                    {
                        ConstraintWidget target = idToWidget[resolvedLeftToLeft];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.LEFT, layoutParams.layout.leftMargin, resolveGoneLeftMargin);
                        }
                    }
                    else if (resolvedLeftToRight != Unset)
                    {
                        ConstraintWidget target = idToWidget[resolvedLeftToRight];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.RIGHT, layoutParams.layout.leftMargin, resolveGoneLeftMargin);
                        }
                    }

                    // Right constraint
                    if (resolvedRightToLeft != Unset)
                    {
                        ConstraintWidget target = idToWidget[resolvedRightToLeft];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.LEFT, layoutParams.layout.rightMargin, resolveGoneRightMargin);
                        }
                    }
                    else if (resolvedRightToRight != Unset)
                    {
                        ConstraintWidget target = idToWidget[resolvedRightToRight];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.RIGHT, layoutParams.layout.rightMargin, resolveGoneRightMargin);
                        }
                    }

                    // Top constraint
                    if (layoutParams.layout.topToTop != Unset)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.topToTop];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.TOP, layoutParams.layout.topMargin, layoutParams.layout.goneTopMargin);
                        }
                    }
                    else if (layoutParams.layout.topToBottom != Unset)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.topToBottom];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.BOTTOM, layoutParams.layout.topMargin, layoutParams.layout.goneTopMargin);
                        }
                    }

                    // Bottom constraint
                    if (layoutParams.layout.bottomToTop != Unset)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.bottomToTop];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.TOP, layoutParams.layout.bottomMargin, layoutParams.layout.goneBottomMargin);
                        }
                    }
                    else if (layoutParams.layout.bottomToBottom != Unset)
                    {
                        ConstraintWidget target = idToWidget[layoutParams.layout.bottomToBottom];
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.BOTTOM, layoutParams.layout.bottomMargin, layoutParams.layout.goneBottomMargin);
                        }
                    }

                    // Baseline constraint
                    if (layoutParams.layout.baselineToBaseline != Unset)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.layout.baselineToBaseline, ConstraintAnchor.Type.BASELINE);
                    }
                    else if (layoutParams.layout.baselineToTop != Unset)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.layout.baselineToTop, ConstraintAnchor.Type.TOP);
                    }
                    else if (layoutParams.layout.baselineToBottom != Unset)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.layout.baselineToBottom, ConstraintAnchor.Type.BOTTOM);
                    }

                    if (resolvedHorizontalBias >= 0)
                    {
                        widget.HorizontalBiasPercent = resolvedHorizontalBias;
                    }
                    if (layoutParams.layout.verticalBias >= 0)
                    {
                        widget.VerticalBiasPercent = layoutParams.layout.verticalBias;
                    }
                }

                if (isInEditMode && ((layoutParams.layout.editorAbsoluteX != Unset) || (layoutParams.layout.editorAbsoluteY != Unset)))
                {
                    widget.setOrigin(layoutParams.layout.editorAbsoluteX, layoutParams.layout.editorAbsoluteY);
                }

                // FIXME: need to agree on the correct magic value for this rather than simply using
                // zero.
                if (!layoutParams.layout.horizontalDimensionFixed)//Match_Parent和Match_Constraint是不固定
                {
                    if (layoutParams.layout.mWidth == ConstraintSet.MatchParent)
                    {
                        if (layoutParams.layout.constrainedWidth)
                        {
                            widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        }
                        else
                        {
                            widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
                        }
                        widget.getAnchor(ConstraintAnchor.Type.LEFT).mMargin = layoutParams.layout.leftMargin;
                        widget.getAnchor(ConstraintAnchor.Type.RIGHT).mMargin = layoutParams.layout.rightMargin;
                    }
                    else
                    {
                        widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        widget.Width = 0;
                    }
                }
                else//具体数值或Warp_Content是固定
                {
                    widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                    widget.Width = layoutParams.layout.mWidth;
                    if (layoutParams.layout.mWidth == WrapContent)
                    {
                        widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }
                if (!layoutParams.layout.verticalDimensionFixed)
                {
                    if (layoutParams.layout.mHeight == ConstraintSet.MatchParent)
                    {
                        if (layoutParams.layout.constrainedHeight)
                        {
                            widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        }
                        else
                        {
                            widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
                        }
                        widget.getAnchor(ConstraintAnchor.Type.TOP).mMargin = layoutParams.layout.topMargin;
                        widget.getAnchor(ConstraintAnchor.Type.BOTTOM).mMargin = layoutParams.layout.bottomMargin;
                    }
                    else
                    {
                        widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        widget.Height = 0;
                    }
                }
                else
                {
                    widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                    widget.Height = layoutParams.layout.mHeight;
                    if (layoutParams.layout.mHeight == WrapContent)
                    {
                        widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }

                widget.setDimensionRatio(layoutParams.layout.dimensionRatio);
                widget.HorizontalWeight = layoutParams.layout.horizontalWeight;
                widget.VerticalWeight = layoutParams.layout.verticalWeight;
                widget.HorizontalChainStyle = layoutParams.layout.horizontalChainStyle;
                widget.VerticalChainStyle = layoutParams.layout.verticalChainStyle;
                widget.WrapBehaviorInParent = layoutParams.layout.wrapBehaviorInParent;
                widget.setHorizontalMatchStyle(layoutParams.layout.matchConstraintDefaultWidth, layoutParams.layout.matchConstraintMinWidth, layoutParams.layout.matchConstraintMaxWidth, layoutParams.layout.matchConstraintPercentWidth);
                widget.setVerticalMatchStyle(layoutParams.layout.matchConstraintDefaultHeight, layoutParams.layout.matchConstraintMinHeight, layoutParams.layout.matchConstraintMaxHeight, layoutParams.layout.matchConstraintPercentHeight);
            }
        }

        private void setWidgetBaseline(ConstraintWidget widget, ConstraintSet.Constraint layoutParams, Dictionary<int, ConstraintWidget> idToWidget, int baselineTarget, ConstraintAnchor.Type type)
        {
            UIElement view = idToViews[baselineTarget];
            ConstraintWidget target = idToWidget[baselineTarget];
            if (target != null && view != null)
            {
                layoutParams.layout.needsBaseline = true;
                if (type == ConstraintAnchor.Type.BASELINE)
                {
                    // baseline to baseline
                    var targetParams = mConstraintSet.Constraints[baselineTarget];
                    targetParams.layout.needsBaseline = true;
                    target.HasBaseline = true;
                }
                ConstraintAnchor baseline = widget.getAnchor(ConstraintAnchor.Type.BASELINE);
                ConstraintAnchor targetAnchor = target.getAnchor(type);
                baseline.connect(targetAnchor, layoutParams.layout.baselineMargin, layoutParams.layout.goneBaselineMargin, true);
                widget.HasBaseline = true;
                widget.getAnchor(ConstraintAnchor.Type.TOP).reset();
                widget.getAnchor(ConstraintAnchor.Type.BOTTOM).reset();
            }
        }

        #endregion Apply Contrants to Widget
    }
}