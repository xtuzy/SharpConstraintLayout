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

#if __MAUI__
    using Panel = Microsoft.Maui.Controls.Layout;
    using UIElement = Microsoft.Maui.Controls.View;
    using Microsoft.Maui.Graphics;
#elif WINDOWS
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
    using Panel = Android.Views.ViewGroup;
    using UIElement = Android.Views.View;
    using Microsoft.Maui.Graphics;
#endif
    using AndroidMeasureSpec = SharpConstraintLayout.Maui.Widget.MeasureSpec;
    using SharpConstraintLayout.Maui.Widget.Interface;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// ConstraintLayout is a AndroidX layout for <see
    /// href="https://developer.android.com/reference/androidx/constraintlayout/widget/ConstraintLayout">androidx.constraintlayout.widget.ConstraintLayout</see>.
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
    public partial class ConstraintLayout : Panel, IConstraintLayout
    {
        public const string VERSION = "ConstraintLayout-2.1.1";
        public const bool ISSUPPORTRTLPLATFORM = false;//现在完全不支持,需要对照原来的代码看删除了那些关于Rtl的

        /// <summary>
        /// if is true,will print some layout info.
        /// </summary>
        public static bool DEBUG = false;
        public string DebugName;
        /// <summary>
        /// 从Maui引入, Debug用它
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// if is true,will print time of measure spend.
        /// </summary>
        public static bool MEASURE_MEASURELAYOUT = false;
        public static bool MEASUREEVERYWIDGET = false;
        public static bool MEASUREEVERYCHILD = false;

        public static bool LOG = true;

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

        void init()
        {
            mLayout.CompanionWidget = this;
            mConstraintSet = new ConstraintSet();//默认创建一个保存最开始的Constraints,之后新建的clone其中的信息
            //constraintLayout have default Widget,need add it to dictionary,we can use it like other child later.
            //ConstraintSet.Constraints.Add(ConstraintSet.PARENT_ID, new ConstraintSet.Constraint());//对于Layout,都用ParentID代替GetHashCode,这是因为Layout可以在ApplyTo时替换
            //这里换种思路,不管是ParentId还是HashCode对应的应该都是同一个约束,修改也是同一个
            var rootConstraint = new ConstraintSet.Constraint();
            rootConstraint.layout.mWidth = ConstraintSet.MatchParent;//@zhouyang Add:Default set WrapContent,it is more useful
            rootConstraint.layout.mHeight = ConstraintSet.MatchParent;

            mConstraintSet.Constraints.Add(ConstraintSet.ParentId, rootConstraint);
            //mConstraintSet.Constraints.Add(this.GetId(), rootConstraint); //@zhouyang 2022/6/27 为了Maui中使用StyleId, 需要推迟GetId,将其移动到了OnAddedView中

            mLayout.Measurer = mMeasurer = new MeasurerAnonymousInnerClass(this);

            //ClipToBounds = true;//view in ConstraintLayout always easy out of bounds
        }

        #region Add And Remove

        protected void OnAddedView(UIElement element)
        {            
            if (element == null)
            {
                return;
            }

            if (!mConstraintSet.Constraints.ContainsKey(this.GetId()))
                mConstraintSet.Constraints.Add(this.GetId(), mConstraintSet.Constraints[ConstraintSet.ParentId]);

            //Add to flow, not change set dirrectly, so it not change, we need it change to recreate constrain
            if (mConstraintSet.IsChanged == false)
                mConstraintSet.IsChanged = true;

            var id = element.GetId();

            //Add to three dictionary(Wighet,View,Constraints)
            ConstraintWidget widget = GetOrAddWidgetById(id);//加入存储Widget的字典

            var childConstraint = new ConstraintSet.Constraint();
            mConstraintSet.Constraints.Add(id, childConstraint);//加入存储约束的字典
            childConstraint.layout.mWidth = ConstraintSet.WrapContent;//@zhouyang Add:Default set WrapContent,it is more useful
            childConstraint.layout.mHeight = ConstraintSet.WrapContent;
            idToViews.Add(id, element as UIElement);//加入存储View的字典
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
            //Remove form flow, not change set dirrectly, so it not change, we need it change to recreate constrain
            if (mConstraintSet.IsChanged == false)
                mConstraintSet.IsChanged = true;

            var id = element.GetId();
            idToViews.Remove(id);//从存储View的字典中移除
            ConstraintWidget widget = GetOrAddWidgetById(id);
            idsToConstraintWidgets.Remove(id);//从存储Widget的字典移除
            MLayoutWidget.remove(widget);
            if (element is ConstraintHelper)
                mConstraintHelpers.Remove(element as ConstraintHelper);
            mConstraintSet.Constraints.Remove(id);//从存储约束的字典移除

        }

        private ConstraintWidget GetOrAddWidgetById(int id)
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
                    //MLayoutWidget.add(widget);//@zhouyang Add 2022/3/29:Android like if not use ConstraintSet to set, widget not add to Container, i need a default for other layout just as child, ConstraintHelper we don't set,because use they need use CosntraintSet
                    //widget.CompanionWidget = FindViewById(id);//@zhouyang Add 2022/3/29
                    return widget;
                }
            }
        }

        #endregion Add And Remove

        #region Get

        /// <summary>
        /// default widget of ConstraintLayout. you can control more action by it.
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
        /// <returns>widget is a virtual control for calculate constraint</returns>
        public ConstraintWidget GetWidgetByElement(UIElement view)
        {
            if (view == this)
                return MLayoutWidget;
            var id = view.GetId();
            if (this.idsToConstraintWidgets.ContainsKey(id))
                return this.idsToConstraintWidgets[id];
            else
                return null;
        }

        public UIElement FindElementById(int id)
        {
            if (idToViews.ContainsKey(id))
                return idToViews[id];
            else
                return null;
        }

        #endregion Get

        #region LayoutProperty
        /// <summary>
        /// ConstraintLayout的内边距
        /// </summary>
        public int ConstrainPaddingTop { set; get; }
        public int ConstrainPaddingBottom { set; get; }
        public int ConstrainPaddingLeft { set; get; }
        public int ConstrainPaddingRight { set; get; }
        public int ConstrainPaddingStart { set; get; }
        public int ConstrainPaddingEnd { set; get; }

        /// <summary>
        /// Compute the padding width, taking in account RTL start/end padding if available and present.
        /// <see cref="getPaddingWidth"/>
        /// </summary>
        /// <returns></returns>
        public int ConstrainPaddingWidth
        {
            get
            {

                int widthPadding = Math.Max(0, ConstrainPaddingRight) + Math.Max(0, ConstrainPaddingLeft);
                int rtlPadding = 0;

                if (ISSUPPORTRTLPLATFORM)
                {
                    rtlPadding = Math.Max(0, ConstrainPaddingStart) + Math.Max(0, ConstrainPaddingEnd);
                }
                if (rtlPadding > 0)
                {
                    widthPadding = rtlPadding;
                }

                return widthPadding;
            }
        }

        private int mMinWidth = 0;
        private int mMinHeight = 0;
        private int mMaxWidth = int.MaxValue;
        private int mMaxHeight = int.MaxValue;

        public int ConstrainMinWidth
        {
            get => mMinWidth;
            set
            {
                if (value == mMinWidth)
                    return;
                mMinWidth = value;
                RequestReLayout();
            }
        }

        public int ConstrainMinHeight
        {
            get => mMinHeight;
            set
            {
                if (value == mMinHeight)
                    return;
                mMinHeight = value;
                RequestReLayout();
            }
        }

        public int ConstrainMaxWidth
        {
            get => mMaxWidth;
            set
            {
                if (value == mMaxWidth)
                    return;
                mMaxWidth = value;
                RequestReLayout();
            }
        }

        public int ConstrainMaxHeight
        {
            get => mMaxHeight;
            set
            {
                if (value == mMaxHeight)
                    return;
                mMaxHeight = value;
                RequestReLayout();
            }
        }

        /// <summary>
        /// 该属性约束自身的高度,其值可以是固定数值,或者<see cref="ConstraintSet.WrapContent"/>,<see cref="ConstraintSet.MatchConstraint"/>,<see cref="ConstraintSet.MatchParent"/>,
        /// 当依赖Parent给与的高度时,使用<see cref="ConstraintSet.MatchParent"/>,其为默认;当依赖自身Child的大小时,使用其它.
        /// (因为父布局的行为不能确定,因此可能会与父布局产生冲突,请多加尝试);在Xaml中可通过ConstrainHeight="{x:Static constrain:ConstraintSet.WrapContent}"来设置
        /// </summary>
        public int ConstrainHeight
        {
            get
            {
                return this.mConstraintSet.GetConstraint(ConstraintSet.ParentId).layout.mHeight;
            }
            set
            {
                if (value > 0)
                {
#if __MAUI__
                    this.SetHeight(value);
#else
                    this.SetSizeAndMargin(0, value, 0, 0, 0, 0, 0, 0, 0, 0);
#endif
                }
                this.mConstraintSet.GetConstraint(ConstraintSet.ParentId).layout.mHeight = value;
                this.mConstraintSet.IsChanged = true;
            }
        }

        /// <summary>
        /// 该属性约束自身的宽度,其值可以是固定数值,或者<see cref="ConstraintSet.WrapContent"/>,<see cref="ConstraintSet.MatchConstraint"/>,<see cref="ConstraintSet.MatchParent"/>,
        /// 当依赖Parent给与的宽度时,使用<see cref="ConstraintSet.MatchParent"/>,其为默认;当依赖自身Child的大小时,使用其它.
        /// (因为父布局的行为不能确定,因此可能会与父布局产生冲突,请多加尝试)；在Xaml中可通过ConstrainWidth="{x:Static constrain:ConstraintSet.WrapContent}"来设置
        /// </summary>
        public int ConstrainWidth
        {
            get
            {
                return this.mConstraintSet.GetConstraint(ConstraintSet.ParentId).layout.mWidth;
            }
            set
            {
                if (value > 0)
                {
#if __MAUI__
                    this.SetWidth(value);
#else
                    this.SetSizeAndMargin(value, 0, 0, 0, 0, 0, 0, 0, 0, 0);
#endif

                }
                this.mConstraintSet.GetConstraint(ConstraintSet.ParentId).layout.mWidth = value;
                this.mConstraintSet.IsChanged = true;
            }
        }

        #endregion LayoutProperty

        #region Measure

        /*@zhouyang 2022/6/13 注释掉.
        /// <summary>
        /// 存储MeasureSpec给子ConstraintLayout使用,因为iOS和Windows布局体系里本身不传递
        /// </summary>
        public int HorizontalSpec { get; set; } = 0;
        public int VerticalSpec { get; set; } = 0;
        */

        /// <summary>
        /// Android中Spec由parent制作,其它平台需自己制作
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public (int horizontalSpec, int verticalSpec) MakeSpec(ConstraintLayout layout, Size availableSize)
        {
            var constrainWidth = this.mConstraintSet.GetConstraint(ConstraintSet.ParentId).layout.mWidth;
            var constrainHeight = this.mConstraintSet.GetConstraint(ConstraintSet.ParentId).layout.mHeight;
            int horizontalSpec = 0;
            int verticalSpec = 0;
            (bool isInfinityAvailabelWidth, bool isInfinityAvailabelHeight) = IsInfinitable(this, constrainWidth, constrainHeight, availableSize);
            //If a direction available value and we need MATCH_PARENT, that always generate mistake result, so we not accept it. please modify constraint.
            if ((isInfinityAvailabelWidth && constrainWidth == ConstraintSet.MatchParent) || (isInfinityAvailabelHeight && constrainHeight == ConstraintSet.MatchParent))
            {
#if __MAUI__
                var errorStr = $"ConstraintLayout's parent is {this.Parent.GetType().Name},it give ConstraintLayout a infinity size, you set ConstraintLayout have MATCH_PARENT size, ConstraintLayout can't generate correct result.";
#else
                var errorStr = $"ConstraintLayout's parent is {this.GetParent().GetType().Name},it give ConstraintLayout a infinity size, you set ConstraintLayout have MATCH_PARENT size, ConstraintLayout can't generate correct result.";
#endif
                System.Diagnostics.Debug.WriteLine(errorStr);
                System.Diagnostics.Trace.WriteLine(errorStr);
                throw new InvalidOperationException(errorStr);
            }

            int availableWidth = (int)availableSize.Width;
            int availableHeight = (int)availableSize.Height;

            if (isInfinityAvailabelWidth)
            {
                availableWidth = int.MaxValue;
            }

            if (isInfinityAvailabelHeight)
            {
                availableHeight = int.MaxValue;
            }

            int horizontalDimension = availableWidth;
            int verticalDimension = availableHeight;
            /*
             * Android中Spec的制作:ScrollView传入的高是UNSPECIFIED,高度有具体数值.
             */
            if (constrainWidth == ConstraintSet.WrapContent)
            {
                //WrapContent时,如果Window中Parent传入无限值,代表对此Layout大小没有限制,如果是确定数值,则代表有限制
                if (isInfinityAvailabelWidth)
                    horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(horizontalDimension, AndroidMeasureSpec.UNSPECIFIED);//ScrollView的子View是UNSPECIFIED,Windouws中对应无限值
                else
                    horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(horizontalDimension, AndroidMeasureSpec.AT_MOST);
                horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(horizontalDimension, AndroidMeasureSpec.AT_MOST);
            }
            else if (constrainWidth == ConstraintSet.MatchParent || constrainWidth == ConstraintSet.MatchConstraint)
            { //MatchParent时,Parent最终都有固定大小,此Layout大小也就确定,所以此处使用EXACTLY

                if (isInfinityAvailabelWidth)
                    horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(horizontalDimension, AndroidMeasureSpec.UNSPECIFIED);
                else
                    horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(horizontalDimension, AndroidMeasureSpec.EXACTLY);
            }
            else
            {
                //指定了具体大小
                horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(constrainWidth, AndroidMeasureSpec.EXACTLY);
            }

            if (constrainHeight == ConstraintSet.WrapContent)
            {

                if (isInfinityAvailabelHeight)
                    verticalSpec = AndroidMeasureSpec.MakeMeasureSpec(verticalDimension, AndroidMeasureSpec.UNSPECIFIED);//ScrollView的子View是UNSPECIFIED,Windouws中对应无限值
                else
                {
                    verticalSpec = AndroidMeasureSpec.MakeMeasureSpec(verticalDimension, AndroidMeasureSpec.AT_MOST);
                }
            }
            else if (constrainHeight == ConstraintSet.MatchParent || constrainHeight == ConstraintSet.MatchConstraint)
            {
                if (isInfinityAvailabelHeight)
                    verticalSpec = AndroidMeasureSpec.MakeMeasureSpec(verticalDimension, AndroidMeasureSpec.UNSPECIFIED);//ScrollView的子View是UNSPECIFIED,Windouws中对应无限值
                else
                    verticalSpec = AndroidMeasureSpec.MakeMeasureSpec(verticalDimension, AndroidMeasureSpec.EXACTLY);
            }
            else
            {
                verticalSpec = AndroidMeasureSpec.MakeMeasureSpec(constrainHeight, AndroidMeasureSpec.EXACTLY);
            }

            /*
             * @zhouyang 2022/6/13 注释掉以下与HorizontalSpec相关代码.
             * 在处理嵌套ConstraintLayout时,如果指定了子ConstraintLayout的大小,那么Measure时传入ConsraintLayout的值应该是确定的,
             * 如果再使用Parent的度量值去计算,那么大小会不正确.
             */
            /*if (!isInfinityAvailabelWidth && constrainWidth <= 0)
            {
                if (this.GetParent() is ConstraintLayout)
                {
                    var parent = this.GetParent() as ConstraintLayout;
                    if (parent.HorizontalSpec != 0)
                    {
                        horizontalSpec = AndroidMeasureSpec.getChildMeasureSpec(parent.HorizontalSpec, parent.ConstrainPaddingWidth, constrainWidth);
                    }
                    else
                        if (DEBUG) SimpleDebug.WriteLine($"{parent} verticalSpec is 0");
                }

            }
            if (!isInfinityAvailabelHeight && constrainHeight <= 0)
            {
                if (this.GetParent() is ConstraintLayout)
                {
                    var parent = this.GetParent() as ConstraintLayout;
                    if (parent.VerticalSpec != 0)
                    {
                        verticalSpec = AndroidMeasureSpec.getChildMeasureSpec(parent.VerticalSpec, parent.ConstrainPaddingTop + parent.ConstrainPaddingBottom, constrainHeight);
                    }
                    else
                        if (DEBUG) SimpleDebug.WriteLine($"{parent} verticalSpec is 0");
                }
            }*/

            isInfinityAvailabelWidth = false;
            isInfinityAvailabelHeight = false;

            /*//存储Spec给Child使用
            if (horizontalSpec != HorizontalSpec)
            {
                HorizontalSpec = horizontalSpec;
            }
            if (verticalSpec != VerticalSpec)
            {
                VerticalSpec = verticalSpec;
            }*/

            return (horizontalSpec, verticalSpec);
        }

        /// <summary>
        /// iOS:iOS的布局流程中,父布局通过指定Frame来给Child布局,也就是说父布局要先知道Child测量的大小,这与Android不同,Android的布局流程是先给Child测量,然后再给父布局测量.
        /// 这就导致Child需要依据父布局大小布局时,不知道父布局大小.当父布局是ConstraintLayout时,我们可以处理,当父布局是平台布局时,就需要按照平台布局特征来得到大小.
        /// 如果父布局是UIScrollView时,可以有无限值,那么在Android布局体系中对应着Child是UNSPECIFIED
        /// Windows:
        /// </summary>
        /// <param name="availableSize"></param>
        /// <param name="horizontalSpec"></param>
        /// <param name="verticalSpec"></param>
        /// <returns></returns>
        public Size MeasureLayout(Size availableSize, int horizontalSpec = 0, int verticalSpec = 0)
        {
            Stopwatch updateHierarchySW = null;

            if (MEASURE_MEASURELAYOUT)
            {
                updateHierarchySW = new Stopwatch();
                updateHierarchySW.Start();
            }

            //Update Constraints to wigets
            if (mConstraintSet.IsChanged)
            {
                //update widget of child
                if (updateHierarchy())
                {
                    MLayoutWidget.updateHierarchy();
                }

                //update widget of Container layout
                var constraint = mConstraintSet.Constraints[ConstraintSet.ParentId];
                applyConstraintsFromLayoutParams(false, this, MLayoutWidget, constraint, idsToConstraintWidgets);

                mConstraintSet.IsChanged = false;
            }

            if (MEASURE_MEASURELAYOUT)
            {
                updateHierarchySW.Stop();
                Logger?.LogInformation($"{this} updateHierarchy time: {updateHierarchySW.Elapsed.TotalMilliseconds.ToString("0.000")} ms");
            }

            if (DEBUG) Logger?.LogDebug($"{nameof(MeasureLayout)} {this.GetType().FullName} {availableSize} Spec=({AndroidMeasureSpec.ToString(horizontalSpec)} x {AndroidMeasureSpec.ToString(verticalSpec)})");

            MLayoutWidget.Rtl = isRtl();

            Stopwatch resolveSystemSW = null;

            if (MEASURE_MEASURELAYOUT)
            {
                resolveSystemSW = new Stopwatch();
                resolveSystemSW.Start();
            }

            resolveSystem(MLayoutWidget, mOptimizationLevel, horizontalSpec, verticalSpec);

            if (MEASURE_MEASURELAYOUT)
            {
                resolveSystemSW.Stop();
                Logger?.LogInformation($"{this} resolveSystem time: {resolveSystemSW.Elapsed.TotalMilliseconds.ToString("0.000")} ms");
            }

            resolveMeasuredDimension(horizontalSpec, verticalSpec, MLayoutWidget.Width, MLayoutWidget.Height,
                    MLayoutWidget.WidthMeasuredTooSmall, MLayoutWidget.HeightMeasuredTooSmall);

            //windows和iOS将返回值作为该Layout的大小
            return new Size(mLastMeasureWidth, mLastMeasureHeight);
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

            int resolvedWidthSize = AndroidMeasureSpec.resolveSizeAndState(androidLayoutWidth, widthMeasureSpec, childState);
            int resolvedHeightSize = AndroidMeasureSpec.resolveSizeAndState(androidLayoutHeight, heightMeasureSpec,
                    childState << AndroidMeasureSpec.MEASURED_HEIGHT_STATE_SHIFT);
            resolvedWidthSize &= AndroidMeasureSpec.MEASURED_SIZE_MASK;
            resolvedHeightSize &= AndroidMeasureSpec.MEASURED_SIZE_MASK;
            resolvedWidthSize = Math.Min(mMaxWidth, resolvedWidthSize);
            resolvedHeightSize = Math.Min(mMaxHeight, resolvedHeightSize);
            if (isWidthMeasuredTooSmall)
            {
                resolvedWidthSize |= AndroidMeasureSpec.MEASURED_STATE_TOO_SMALL;
            }
            if (isHeightMeasuredTooSmall)
            {
                resolvedHeightSize |= AndroidMeasureSpec.MEASURED_STATE_TOO_SMALL;
            }
#if __ANDROID__ && !__MAUI__
            SetMeasuredDimension(resolvedWidthSize, resolvedHeightSize);//存储大小到布局树,iOS通过设置Bounds,Windows通过返回大小
#endif
            mLastMeasureWidth = resolvedWidthSize;
            mLastMeasureHeight = resolvedHeightSize;
        }

        // Cache last measure
        private int mLastMeasureWidth = -1;

        private int mLastMeasureHeight = -1;
        private int mLastMeasureWidthSize = -1;
        private int mLastMeasureHeightSize = -1;
        private int mLastMeasureWidthMode = AndroidMeasureSpec.UNSPECIFIED;
        private int mLastMeasureHeightMode = AndroidMeasureSpec.UNSPECIFIED;

        /// <summary>
        /// Handles measuring a layout
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="optimizationLevel"></param>
        /// <param name="widthMeasureSpec"></param>
        /// <param name="heightMeasureSpec"></param>
        protected void resolveSystem(ConstraintWidgetContainer layout, int optimizationLevel, int widthMeasureSpec, int heightMeasureSpec)
        {
            int widthMode = AndroidMeasureSpec.GetMode(widthMeasureSpec);
            int widthSize = AndroidMeasureSpec.GetSize(widthMeasureSpec);
            int heightMode = AndroidMeasureSpec.GetMode(heightMeasureSpec);
            int heightSize = AndroidMeasureSpec.GetSize(heightMeasureSpec);

            int paddingY = Math.Max(0, ConstrainPaddingTop);
            int paddingBottom = Math.Max(0, ConstrainPaddingBottom);
            int paddingHeight = paddingY + paddingBottom;
            int paddingWidth = ConstrainPaddingWidth;
            int paddingX;
            //传入布局的测量数据,用于测量child时
            mMeasurer.captureLayoutInfo(widthMeasureSpec, heightMeasureSpec, paddingY, paddingBottom, paddingWidth, paddingHeight);

            int paddingStart = Math.Max(0, ConstrainPaddingStart);
            int paddingEnd = Math.Max(0, ConstrainPaddingEnd);
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
                paddingX = Math.Max(0, ConstrainPaddingLeft);
            }

            widthSize -= paddingWidth;
            heightSize -= paddingHeight;

            setSelfDimensionBehaviour(layout, widthMode, widthSize, heightMode, heightSize);
            layout.measure(optimizationLevel, widthMode, widthSize, heightMode, heightSize, mLastMeasureWidth, mLastMeasureHeight, paddingX, paddingY);
        }

        protected void setSelfDimensionBehaviour(ConstraintWidgetContainer layout, int widthMode, int widthSize, int heightMode, int heightSize)
        {
            int heightPadding = mMeasurer.paddingHeight;
            int widthPadding = mMeasurer.paddingWidth;

            ConstraintWidget.DimensionBehaviour widthBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            ConstraintWidget.DimensionBehaviour heightBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

            int desiredWidth = 0;
            int desiredHeight = 0;
            int childCount = ChildCount;

            if (widthMode == AndroidMeasureSpec.AT_MOST)
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
            else if (widthMode == AndroidMeasureSpec.UNSPECIFIED)
            {
                {
                    widthBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    if (childCount == 0)
                    {
                        desiredWidth = Math.Max(0, mMinWidth);
                    }
                }
            }
            else if (widthMode == AndroidMeasureSpec.EXACTLY)
            {
                {
                    desiredWidth = Math.Min(mMaxWidth - widthPadding, widthSize);
                }
            }

            if (heightMode == AndroidMeasureSpec.AT_MOST)
            {
                heightBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                desiredHeight = heightSize;
                if (childCount == 0)
                {
                    desiredHeight = Math.Max(0, mMinHeight);
                }
            }
            else if (heightMode == AndroidMeasureSpec.UNSPECIFIED)
            {
                heightBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                if (childCount == 0)
                {
                    desiredHeight = Math.Max(0, mMinHeight);
                }
            }
            else if (heightMode == AndroidMeasureSpec.EXACTLY)
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

        public void ArrangeLayout()
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
                    LayoutChild(component, child.X, child.Y, child.Width, child.Height);
                    if (DEBUG && ChildCount < 10) Logger?.LogDebug($"{nameof(ArrangeLayout)} {component.GetViewLayoutInfo()} Widget={new Rect(child.X, child.Y, child.Width, child.Height)}");
                }

                if (component is Placeholder)
                {
                    Placeholder holder = (Placeholder)component;
                    UIElement content = holder.Content;
                    if (content != null)
                    {
                        UIElementExtension.SetViewVisibility(content, ConstraintSet.Visible);
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
                Stopwatch sw = null;

                if (MEASUREEVERYWIDGET)
                {
                    sw = new Stopwatch();
                    sw.Start();
                }

                AndroidSourceCodeMeasureUseSpecForWindows(widget, measure);

                if (MEASUREEVERYWIDGET)
                {
                    sw.Stop();
                    outerInstance.Logger?.LogInformation($"{widget.CompanionWidget} widget measure time: {sw.Elapsed.TotalMilliseconds.ToString("0.000")} ms");
                }
            }

            public virtual void didMeasures()
            {
                int widgetsCount = outerInstance.ChildCount;
                for (int i = 0; i < widgetsCount; i++)
                {
                    UIElement child = (UIElement)outerInstance.GetChildAt(i);
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

                ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.horizontalBehavior;
                ConstraintWidget.DimensionBehaviour verticalBehavior = measure.verticalBehavior;

                int horizontalDimension = measure.horizontalDimension;
                int verticalDimension = measure.verticalDimension;

                int horizontalSpec = 0;
                int verticalSpec = 0;

                int heightPadding = paddingTop + paddingBottom;
                int widthPadding = paddingWidth;

                var child = (UIElement)widget.CompanionWidget;

                Stopwatch sw = null;
                if (MEASUREEVERYCHILD)
                {
                    sw = new Stopwatch();
                    sw.Start();
                }

#if __MAUI__
                var childCurrentPlatformMeasuredSize = child.GetWrapContentSize();
#else
                var childCurrentPlatformMeasuredSize = child.GetWrapContentSize();//Windows上DesireSize会随TextBox变化,iOS的ContentSize也会变,安卓的好像指挥先标记需要测量,之后MeasuredSize才变.这里放在最前面,是因为获取iOS的ContentSize可能需要计算,避免重复计算就放在最前.MAUI同Android
                var childCurrentPlatformBaseline = child.GetBaseline(childCurrentPlatformMeasuredSize.Height);//缓存Baseline为iOS;MAUI因为不进行Pass判断,因此不提前计算
#endif
                if (MEASUREEVERYCHILD)
                {
                    sw.Stop();
                    outerInstance.Logger?.LogInformation($"{widget.CompanionWidget} measure time: {sw.Elapsed.TotalMilliseconds.ToString("0.000")} ms");
                }

                switch (horizontalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        {
                            horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(horizontalDimension, AndroidMeasureSpec.EXACTLY);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        {
                            horizontalSpec = AndroidMeasureSpec.getChildMeasureSpec(layoutWidthSpec, widthPadding, ConstraintSet.WrapContent);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        {
                            // Horizontal spec must account for margin as well as padding here.
                            horizontalSpec = AndroidMeasureSpec.getChildMeasureSpec(layoutWidthSpec, widthPadding + widget.HorizontalMargin, ConstraintSet.MatchParent);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        {
                            horizontalSpec = AndroidMeasureSpec.getChildMeasureSpec(layoutWidthSpec, widthPadding, ConstraintSet.WrapContent);
                            bool shouldDoWrap = widget.mMatchConstraintDefaultWidth == ConstraintSet.MatchConstraintWrap;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
                                // bool otherDimensionStable = child.MeasuredHeight == widget.Height;//Windows中当前还未Measure,先使用Defalut DesiredSize尝试
                                bool otherDimensionStable = childCurrentPlatformMeasuredSize.Height == widget.Height;
                                bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedHorizontally);
                                if (useCurrent)
                                {
                                    horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(widget.Width, AndroidMeasureSpec.EXACTLY);
                                }
                            }
                        }
                        break;
                }

                switch (verticalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                        {
                            verticalSpec = AndroidMeasureSpec.MakeMeasureSpec(verticalDimension, AndroidMeasureSpec.EXACTLY);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                        {
                            verticalSpec = AndroidMeasureSpec.getChildMeasureSpec(layoutHeightSpec, heightPadding, ConstraintSet.WrapContent);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                        {
                            // Vertical spec must account for margin as well as padding here.
                            verticalSpec = AndroidMeasureSpec.getChildMeasureSpec(layoutHeightSpec, heightPadding + widget.VerticalMargin, ConstraintSet.MatchParent);
                        }
                        break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                        {
                            verticalSpec = AndroidMeasureSpec.getChildMeasureSpec(layoutHeightSpec, heightPadding, ConstraintSet.WrapContent);
                            bool shouldDoWrap = widget.mMatchConstraintDefaultHeight == ConstraintSet.MatchConstraintWrap;
                            if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                            {
                                // the solver gives us our new dimension, but if we previously had it measured with
                                // a wrap, it can be incorrect if the other side was also variable.
                                // So in that case, we have to double-check the other side is stable (else we can't
                                // just assume the wrap value will be correct).
                                //bool otherDimensionStable = child.MeasuredWidth == widget.Width;//Windows中Measure后的高度应该是DesiredSize
                                bool otherDimensionStable = childCurrentPlatformMeasuredSize.Width == widget.Width;
                                bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedVertically);
                                if (useCurrent)
                                {
                                    verticalSpec = AndroidMeasureSpec.MakeMeasureSpec(widget.Height, AndroidMeasureSpec.EXACTLY);
                                }
                            }
                        }
                        break;
                }

                /*
                 * @zhouyang add at 2022/4/15: i found EditText can't measure correctly, because at here will let it skip measure.
                 * That because Android only get real size after measure, Windows can get real size before load measure. so we let android must remeasure.
                 * AndroidX.ConstraintLayout use mDirtyHierarchy to mark need measure, but it like also measure all.
                 * At Windows, WrapPanel also remeasure all, i feel it not good, so Windows i use these code still.
                 * 
                 * MAUI需要Measure后才有正确的大小,否则和之前的一样,那么会一直Pass,因此让其一直需要重新测量
                */
#if !__MAUI__
#if __ANDROID__ && !__MAUI__
                //before measure,we don't know new size, we just know it is dirty,
                //so all dirty need remeasure,other view that wrap content maybe not remeasure.
                if (!child.IsLayoutRequested && widget.HorizontalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT && widget.VerticalDimensionBehaviour == ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
#endif
                {
                    ConstraintWidgetContainer container = (ConstraintWidgetContainer)widget.Parent;
                    if (container != null && Optimizer.enabled(outerInstance.mOptimizationLevel, Optimizer.OPTIMIZATION_CACHE_MEASURES))
                    {

                        var currentSize = childCurrentPlatformMeasuredSize;
                        if (currentSize.Width > 0 && //为0的强制测量,避免出错
                            currentSize.Width == widget.Width && currentSize.Width < container.Width && currentSize.Height == widget.Height && currentSize.Height < container.Height &&
                            childCurrentPlatformBaseline == widget.BaselineDistance
                            && !widget.MeasureRequested)
                        // note: the container check replicates legacy behavior, but we might want
                        // to not enforce that in 3.0
                        {
                            if (!(child is VirtualLayout))//when VirtualLayout is similar,we still need measure it's child, so we need remeasure it
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
                                        outerInstance.Logger?.LogDebug("SKIPPED " + child.GetType().FullName + widget);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
#endif
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
                var @params = outerInstance.mConstraintSet.GetConstraint(child.GetId()).layout;

                int width = 0;
                int height = 0;
                int baseline = 0;

                int w = 0;
                int h = 0;
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
                        if (DEBUG) outerInstance.Logger?.LogDebug($"{child.GetType().FullName} before onMeasure: widget={widget},spec=({MeasureSpec.GetSize(horizontalSpec)} x {MeasureSpec.GetSize(verticalSpec)})");
                        ((VirtualLayout)child).onMeasure(layout, horizontalSpec, verticalSpec);
                        if (DEBUG) outerInstance.Logger?.LogDebug($"{child.GetType().FullName}  after onMeasure: widget={widget},control={child.GetViewLayoutInfo()}");
#if __MAUI__
                        //(w, h) = ((int)child.WidthRequest, (int)child.HeightRequest);
                        var size = (child as VirtualLayout).MeasuredSize;
                        (w, h) = ((int)size.Width, (int)size.Height);
#elif __IOS__ && !__MAUI__
                        (w, h) = ((int)child.Bounds.Width, (int)child.Bounds.Height);//我在iOS的Flow中,将测量值存储到了Bounds
#elif WINDOWS && !__MAUI__
                        (w, h) = ((int)(child as FrameworkElement).Width, (int)(child as FrameworkElement).Height);
#else
                        (w, h) = child.GetWrapContentSize();
#endif
                    }
                    else
                    {
                        if (DEBUG) outerInstance.Logger?.LogDebug($"{child.GetType().FullName}  before onMeasure: widget={widget},control={child.GetWrapContentSize()},spec=({AndroidMeasureSpec.GetSize(horizontalSpec)} x {AndroidMeasureSpec.GetSize(verticalSpec)})");
#if __IOS__ && !__MAUI__
                        (w, h) = (UIElementExtension.GetDefaultSize(childCurrentPlatformMeasuredSize.Width, horizontalSpec), UIElementExtension.GetDefaultSize(childCurrentPlatformMeasuredSize.Height, verticalSpec));//iOS没有Measure函数,只需要使用当前的测量值即可
#else
                        (w, h) = child.MeasureSelf(horizontalSpec, verticalSpec);
#endif
                        if (DEBUG) outerInstance.Logger?.LogDebug($"{child.GetType().FullName}  after onMeasure: widget={widget},control={child.GetWrapContentSize()},measured=({w} x {h})");
                    }
                    widget.setLastMeasureSpec(horizontalSpec, verticalSpec);

#if __ANDROID__ || __MAUI__
                    baseline = child.GetBaseline(h);//Android重新测量后可能有新的Baseline,iOS和Windows在MeasureSelf前就确定了,MAUI同Android
#else
                    baseline = childCurrentPlatformBaseline;
#endif

                    width = w;
                    height = h;

                    if (DEBUG)
                    {
                        string measurement = $"spec ({AndroidMeasureSpec.ToString(horizontalSpec)} x {AndroidMeasureSpec.ToString(verticalSpec)}) => ({width} {height})";
                        outerInstance.Logger?.LogDebug($"{child.GetType().FullName} platform measure result: {measurement}");
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
                    /*
                     * 这个步骤不理解:这里的意思是拿测量过后的值和最大最小的值对比得到的大小再次进行测量,取这次测量大小为最终大小.
                     * 但问题是再次测量得到的值不能作为最终值(Windows),因为WrapContent测量得出来的还是第一次测量的值
                     */
#if __ANDROID__ || __MAUI__
                    if (w != width || h != height)
                    {
                        if (w != width)
                        {
                            horizontalSpec = AndroidMeasureSpec.MakeMeasureSpec(width, AndroidMeasureSpec.EXACTLY);
                        }
                        if (h != height)
                        {
                            verticalSpec = AndroidMeasureSpec.MakeMeasureSpec(height, AndroidMeasureSpec.EXACTLY);
                        }
                        //child.measure(horizontalSpec, verticalSpec);
                        (width, height) = child.MeasureSelf(horizontalSpec, verticalSpec);

                        widget.setLastMeasureSpec(horizontalSpec, verticalSpec);
                        baseline = child.GetBaseline(height);
                        if (DEBUG)
                        {
                            string measurement2 = AndroidMeasureSpec.ToString(horizontalSpec) + " x " + AndroidMeasureSpec.ToString(verticalSpec) + " => " + width + " x " + height;
                            outerInstance.Logger?.LogDebug("measure (b) " + widget.DebugName + " : " + measurement2);
                        }
                    }
#endif
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
                int lastMode = AndroidMeasureSpec.GetMode(lastMeasureSpec);
                int lastSize = AndroidMeasureSpec.GetSize(lastMeasureSpec);
                int mode = AndroidMeasureSpec.GetMode(spec);
                int size = AndroidMeasureSpec.GetSize(spec);
                if (mode == AndroidMeasureSpec.EXACTLY
                        && (lastMode == AndroidMeasureSpec.AT_MOST || lastMode == AndroidMeasureSpec.UNSPECIFIED)
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
                UIElement child = GetChildAt(i);
                ConstraintWidget widget = GetWidgetByElement(child);
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
                UIElement child = GetChildAt(i);
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
                UIElement child = GetChildAt(i);
                ConstraintWidget widget = GetWidgetByElement(child);
                mTempMapIdToWidget.Add(child.GetId(), widget);
            }

            for (int i = 0; i < count; i++)//apply constraint到每个widget
            {
                UIElement child = GetChildAt(i);
                ConstraintWidget widget = GetWidgetByElement(child);
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

        /// <summary>
        /// Call this when something has changed which has invalidated the layout of this view. This will schedule a layout pass of the view tree. This should not be called while the view hierarchy is currently in a layout pass (isInLayout(). If layout is happening, the request may be honored at the end of the current layout pass (and then layout will run again) or after the current frame is drawn and the next layout occurs.
        /// Subclasses which override this method should call the superclass method to handle possible request-during-layout errors correctly.
        /// </summary>
        public void RequestReLayout()
        {
            //According to https://stackoverflow.com/questions/13856180/usage-of-forcelayout-requestlayout-and-invalidate
            //At Android,this will let remeasure layout
#if __MAUI__
            this.InvalidateMeasure();
#elif WINDOWS
            this.InvalidateMeasure();
#elif __IOS__
            this.SetNeedsLayout();
#elif __ANDROID__
            this.RequestLayout();
#endif
        }
    }
}