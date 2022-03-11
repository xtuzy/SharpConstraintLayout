using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2015 The Android Open Source Project
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

namespace SharpConstraintLayout.Maui.Platforms.Androids
{
    using SuppressLint = Android.Annotation.SuppressLint;
    using TargetApi = Android.Annotation.TargetApi;
    using Context = Android.Content.Context;
    using ApplicationInfo = Android.Content.PM.ApplicationInfo;
    using Resources = Android.Content.Res.Resources;
    using TypedArray = Android.Content.Res.TypedArray;
    using Canvas = Android.Graphics.Canvas;
    using Color = Android.Graphics.Color;
    using Paint = Android.Graphics.Paint;
    using Build = Android.OS.Build;
    //using NonNull = AndroidX.Annotations.NonNull;
    using NonNull = AndroidX.Annotations.INonNull;
    //using Nullable = AndroidX.Annotations.Nullable;
    using Nullable = AndroidX.Annotations.NullableAttribute;
    using LinearSystem = androidx.constraintlayout.core.LinearSystem;
    using Metrics = androidx.constraintlayout.core.Metrics;
    using ConstraintAnchor = androidx.constraintlayout.core.widgets.ConstraintAnchor;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Guideline = androidx.constraintlayout.core.widgets.Guideline;
    using Optimizer = androidx.constraintlayout.core.widgets.Optimizer;
    using BasicMeasure = androidx.constraintlayout.core.widgets.analyzer.BasicMeasure;
    using ViewCompat = AndroidX.Core.View.ViewCompat;

    using AttributeSet = Android.Util.IAttributeSet;
    using Log = Android.Util.Log;
    using SparseArray = Android.Util.SparseArray;
    using SparseIntArray = Android.Util.SparseIntArray;
    using View = Android.Views.View;
    using ViewGroup = Android.Views.ViewGroup;


    using static androidx.constraintlayout.widget.ConstraintLayout.LayoutParams;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static android.view.ViewGroup.LayoutParams.MATCH_PARENT;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static android.view.ViewGroup.LayoutParams.WRAP_CONTENT;

    /// <summary>
    /// A {@code ConstraintLayout} is a <seealso cref="android.view.ViewGroup"/> which allows you
    /// to position and size widgets in a flexible way.
    /// <para>
    /// <b>Note:</b> {@code ConstraintLayout} is available as a support library that you can use
    /// on Android systems starting with API level 9 (Gingerbread).
    /// As such, we are planning on enriching its API and capabilities over time.
    /// This documentation will reflect those changes.
    /// </para>
    /// <para>
    /// There are currently various types of constraints that you can use:
    /// <ul>
    /// <li>
    /// <a href="#RelativePositioning">Relative positioning</a>
    /// </li>
    /// <li>
    /// <a href="#Margins">Margins</a>
    /// </li>
    /// <li>
    /// <a href="#CenteringPositioning">Centering positioning</a>
    /// </li>
    /// <li>
    /// <a href="#CircularPositioning">Circular positioning</a>
    /// </li>
    /// <li>
    /// <a href="#VisibilityBehavior">Visibility behavior</a>
    /// </li>
    /// <li>
    /// <a href="#DimensionConstraints">Dimension constraints</a>
    /// </li>
    /// <li>
    /// <a href="#Chains">Chains</a>
    /// </li>
    /// <li>
    /// <a href="#VirtualHelpers">Virtual Helpers objects</a>
    /// </li>
    /// <li>
    /// <a href="#Optimizer">Optimizer</a>
    /// </li>
    /// </ul>
    /// </para>
    /// 
    /// <para>
    /// Note that you cannot have a circular dependency in constraints.
    /// </para>
    /// <para>
    /// Also see {@link ConstraintLayout.LayoutParams
    /// ConstraintLayout.LayoutParams} for layout attributes
    /// </para>
    /// 
    /// <div class="special reference">
    /// <h3>Developer Guide</h3>
    /// 
    /// <h4 id="RelativePositioning"> Relative positioning </h4>
    /// <para>
    /// Relative positioning is one of the basic building blocks of creating layouts in ConstraintLayout.
    /// Those constraints allow you to position a given widget relative to another one. You can constrain
    /// a widget on the horizontal and vertical axis:
    /// <ul>
    /// <li>Horizontal Axis: left, right, start and end sides</li>
    /// <li>Vertical Axis: top, bottom sides and text baseline</li>
    /// </ul>
    /// </para>
    /// <para>
    /// The general concept is to constrain a given side of a widget to another side of any other widget.
    /// </para>
    /// <para>
    /// For example, in order to position button B to the right of button A (Fig. 1):
    /// <br><div align="center">
    /// <img width="300px" src="resources/images/relative-positioning.png">
    /// <br><b><i>Fig. 1 - Relative Positioning Example</i></b>
    /// </div>
    /// </para>
    /// <para>
    /// you would need to do:
    /// </para>
    /// <pre>{@code
    ///         <Button android:id="@+id/buttonA" ... />
    ///         <Button android:id="@+id/buttonB" ...
    ///                 app:layout_constraintLeft_toRightOf="@+id/buttonA" />
    ///         }
    ///     </pre>
    /// This tells the system that we want the left side of button B to be constrained to the right side of button A.
    /// Such a position constraint means that the system will try to have both sides share the same location.
    /// <br><div align="center" >
    /// <img width="350px" src="resources/images/relative-positioning-constraints.png">
    /// <br><b><i>Fig. 2 - Relative Positioning Constraints</i></b>
    /// </div>
    /// 
    /// <para>Here is the list of available constraints (Fig. 2):</para>
    /// <ul>
    /// <li>{@code layout_constraintLeft_toLeftOf}</li>
    /// <li>{@code layout_constraintLeft_toRightOf}</li>
    /// <li>{@code layout_constraintRight_toLeftOf}</li>
    /// <li>{@code layout_constraintRight_toRightOf}</li>
    /// <li>{@code layout_constraintTop_toTopOf}</li>
    /// <li>{@code layout_constraintTop_toBottomOf}</li>
    /// <li>{@code layout_constraintBottom_toTopOf}</li>
    /// <li>{@code layout_constraintBottom_toBottomOf}</li>
    /// <li>{@code layout_constraintBaseline_toBaselineOf}</li>
    /// <li>{@code layout_constraintStart_toEndOf}</li>
    /// <li>{@code layout_constraintStart_toStartOf}</li>
    /// <li>{@code layout_constraintEnd_toStartOf}</li>
    /// <li>{@code layout_constraintEnd_toEndOf}</li>
    /// </ul>
    /// <para>
    /// They all take a reference {@code id} to another widget, or the {@code parent} (which will reference the parent container, i.e. the ConstraintLayout):
    /// <pre>{@code
    ///         <Button android:id="@+id/buttonB" ...
    ///                 app:layout_constraintLeft_toLeftOf="parent" />
    ///         }
    ///     </pre>
    /// 
    /// </para>
    /// 
    /// <h4 id="Margins"> Margins </h4>
    /// <para>
    /// <div align="center" >
    /// <img width="325px" src="resources/images/relative-positioning-margin.png">
    /// <br><b><i>Fig. 3 - Relative Positioning Margins</i></b>
    /// </div>
    /// </para>
    /// <para>If side margins are set, they will be applied to the corresponding constraints (if they exist) (Fig. 3), enforcing
    /// the margin as a space between the target and the source side. The usual layout margin attributes can be used to this effect:
    /// <ul>
    /// <li>{@code android:layout_marginStart}</li>
    /// <li>{@code android:layout_marginEnd}</li>
    /// <li>{@code android:layout_marginLeft}</li>
    /// <li>{@code android:layout_marginTop}</li>
    /// <li>{@code android:layout_marginRight}</li>
    /// <li>{@code android:layout_marginBottom}</li>
    /// <li>{@code layout_marginBaseline}</li>
    /// </ul>
    /// </para>
    /// <para>Note that a margin can only be positive or equal to zero, and takes a {@code Dimension}.</para>
    /// <h4 id="GoneMargin"> Margins when connected to a GONE widget</h4>
    /// <para>When a position constraint target's visibility is {@code View.GONE}, you can also indicate a different
    /// margin value to be used using the following attributes:</para>
    /// <ul>
    /// <li>{@code layout_goneMarginStart}</li>
    /// <li>{@code layout_goneMarginEnd}</li>
    /// <li>{@code layout_goneMarginLeft}</li>
    /// <li>{@code layout_goneMarginTop}</li>
    /// <li>{@code layout_goneMarginRight}</li>
    /// <li>{@code layout_goneMarginBottom}</li>
    /// <li>{@code layout_goneMarginBaseline}</li>
    /// </ul>
    /// </p>
    /// 
    /// </p>
    /// <h4 id="CenteringPositioning"> Centering positioning and bias</h4>
    /// <para>
    /// A useful aspect of {@code ConstraintLayout} is in how it deals with "impossible" constraints. For example, if
    /// we have something like:
    /// <pre>{@code
    ///         <androidx.constraintlayout.widget.ConstraintLayout ...>
    ///             <Button android:id="@+id/button" ...
    ///                 app:layout_constraintLeft_toLeftOf="parent"
    ///                 app:layout_constraintRight_toRightOf="parent"/>
    ///         </>
    ///         }
    ///     </pre>
    /// </para>
    /// <para>
    /// Unless the {@code ConstraintLayout} happens to have the exact same size as the {@code Button}, both constraints
    /// cannot be satisfied at the same time (both sides cannot be where we want them to be).
    /// </para>
    /// <para><div align="center" >
    /// <img width="325px" src="resources/images/centering-positioning.png">
    /// <br><b><i>Fig. 4 - Centering Positioning</i></b>
    /// </div>
    /// </para>
    /// <para>
    /// What happens in this case is that the constraints act like opposite forces
    /// pulling the widget apart equally (Fig. 4); such that the widget will end up being centered in the parent container.
    /// This will apply similarly for vertical constraints.
    /// </para>
    /// <h5 id="Bias">Bias</h5>
    /// <para>
    /// The default when encountering such opposite constraints is to center the widget; but you can tweak
    /// the positioning to favor one side over another using the bias attributes:
    /// <ul>
    /// <li>{@code layout_constraintHorizontal_bias}</li>
    /// <li>{@code layout_constraintVertical_bias}</li>
    /// </ul>
    /// </para>
    /// <para><div align="center" >
    /// <img width="325px" src="resources/images/centering-positioning-bias.png">
    /// <br><b><i>Fig. 5 - Centering Positioning with Bias</i></b>
    /// </div>
    /// </para>
    /// <para>
    /// For example the following will make the left side with a 30% bias instead of the default 50%, such that the left side will be
    /// shorter, with the widget leaning more toward the left side (Fig. 5):
    /// </para>
    /// <pre>{@code
    ///         <androidx.constraintlayout.widget.ConstraintLayout ...>
    ///             <Button android:id="@+id/button" ...
    ///                 app:layout_constraintHorizontal_bias="0.3"
    ///                 app:layout_constraintLeft_toLeftOf="parent"
    ///                 app:layout_constraintRight_toRightOf="parent"/>
    ///         </>
    ///         }
    ///     </pre>
    /// Using bias, you can craft User Interfaces that will better adapt to screen sizes changes.
    /// </p>
    /// </p>
    /// 
    /// <h4 id="CircularPositioning"> Circular positioning (<b>Added in 1.1</b>)</h4>
    /// <para>
    /// You can constrain a widget center relative to another widget center, at an angle and a distance. This allows
    /// you to position a widget on a circle (see Fig. 6). The following attributes can be used:
    /// <ul>
    /// <li>{@code layout_constraintCircle} : references another widget id</li>
    /// <li>{@code layout_constraintCircleRadius} : the distance to the other widget center</li>
    /// <li>{@code layout_constraintCircleAngle} : which angle the widget should be at (in degrees, from 0 to 360)</li>
    /// </ul>
    /// </para>
    /// <para><div align="center" >
    /// <img width="325px" src="resources/images/circle1.png">
    /// <img width="325px" src="resources/images/circle2.png">
    /// <br><b><i>Fig. 6 - Circular Positioning</i></b>
    /// </div>
    /// <br><br>
    /// <pre>{@code
    ///  <Button android:id="@+id/buttonA" ... />
    ///  <Button android:id="@+id/buttonB" ...
    ///      app:layout_constraintCircle="@+id/buttonA"
    ///      app:layout_constraintCircleRadius="100dp"
    ///      app:layout_constraintCircleAngle="45" />
    ///         }
    ///     </pre>
    /// </para>
    /// <h4 id="VisibilityBehavior"> Visibility behavior </h4>
    /// <para>
    /// {@code ConstraintLayout} has a specific handling of widgets being marked as {@code View.GONE}.
    /// </para>
    /// <para>{@code GONE} widgets, as usual, are not going to be displayed and are not part of the layout itself (i.e. their actual dimensions
    /// will not be changed if marked as {@code GONE}).
    /// 
    /// </para>
    /// <para>But in terms of the layout computations, {@code GONE} widgets are still part of it, with an important distinction:
    /// <ul>
    /// <li> For the layout pass, their dimension will be considered as zero (basically, they will be resolved to a point)</li>
    /// <li> If they have constraints to other widgets they will still be respected, but any margins will be as if equals to zero</li>
    /// </ul>
    /// 
    /// </para>
    /// <para><div align="center" >
    /// <img width="350px" src="resources/images/visibility-behavior.png">
    /// <br><b><i>Fig. 7 - Visibility Behavior</i></b>
    /// </div>
    /// </para>
    /// <para>This specific behavior allows to build layouts where you can temporarily mark widgets as being {@code GONE},
    /// without breaking the layout (Fig. 7), which can be particularly useful when doing simple layout animations.
    /// </para>
    /// <para><b>Note: </b>The margin used will be the margin that B had defined when connecting to A (see Fig. 7 for an example).
    /// In some cases, this might not be the margin you want (e.g. A had a 100dp margin to the side of its container,
    /// B only a 16dp to A, marking
    /// A as gone, B will have a margin of 16dp to the container).
    /// For this reason, you can specify an alternate
    /// margin value to be used when the connection is to a widget being marked as gone (see <a href="#GoneMargin">the section above about the gone margin attributes</a>).
    /// </para>
    /// 
    /// <h4 id="DimensionConstraints"> Dimensions constraints </h4>
    /// <h5>Minimum dimensions on ConstraintLayout</h5>
    /// <para>
    /// You can define minimum and maximum sizes for the {@code ConstraintLayout} itself:
    /// <ul>
    /// <li>{@code android:minWidth} set the minimum width for the layout</li>
    /// <li>{@code android:minHeight} set the minimum height for the layout</li>
    /// <li>{@code android:maxWidth} set the maximum width for the layout</li>
    /// <li>{@code android:maxHeight} set the maximum height for the layout</li>
    /// </ul>
    /// Those minimum and maximum dimensions will be used by {@code ConstraintLayout} when its dimensions are set to {@code WRAP_CONTENT}.
    /// </para>
    /// <h5>Widgets dimension constraints</h5>
    /// <para>
    /// The dimension of the widgets can be specified by setting the {@code android:layout_width} and
    /// {@code android:layout_height} attributes in 3 different ways:
    /// <ul>
    /// <li>Using a specific dimension (either a literal value such as {@code 123dp} or a {@code Dimension} reference)</li>
    /// <li>Using {@code WRAP_CONTENT}, which will ask the widget to compute its own size</li>
    /// <li>Using {@code 0dp}, which is the equivalent of "{@code MATCH_CONSTRAINT}"</li>
    /// </ul>
    /// </para>
    /// <para><div align="center" >
    /// <img width="325px" src="resources/images/dimension-match-constraints.png">
    /// <br><b><i>Fig. 8 - Dimension Constraints</i></b>
    /// </div>
    /// The first two works in a similar fashion as other layouts. The last one will resize the widget in such a way as
    /// matching the constraints that are set (see Fig. 8, (a) is wrap_content, (b) is 0dp). If margins are set, they will be taken in account
    /// in the computation (Fig. 8, (c) with 0dp).
    /// </para>
    /// <para>
    /// <b>Important: </b> {@code MATCH_PARENT} is not recommended for widgets contained in a {@code ConstraintLayout}. Similar behavior can
    /// be defined by using {@code MATCH_CONSTRAINT} with the corresponding left/right or top/bottom constraints being set to {@code "parent"}.
    /// </para>
    /// </p>
    /// <h5>WRAP_CONTENT : enforcing constraints (<i><b>Added in 1.1</b></i>)</h5>
    /// <para>
    /// If a dimension is set to {@code WRAP_CONTENT}, in versions before 1.1 they will be treated as a literal dimension -- meaning, constraints will
    /// not limit the resulting dimension. While in general this is enough (and faster), in some situations, you might want to use {@code WRAP_CONTENT},
    /// yet keep enforcing constraints to limit the resulting dimension. In that case, you can add one of the corresponding attribute:
    /// <ul>
    /// <li>{@code app:layout_constrainedWidth="true|false"}</li>
    /// <li>{@code app:layout_constrainedHeight="true|false"}</li>
    /// </ul>
    /// </para>
    /// <h5>MATCH_CONSTRAINT dimensions (<i><b>Added in 1.1</b></i>)</h5>
    /// <para>
    /// When a dimension is set to {@code MATCH_CONSTRAINT}, the default behavior is to have the resulting size take all the available space.
    /// Several additional modifiers are available:
    /// <ul>
    /// <li>{@code layout_constraintWidth_min} and {@code layout_constraintHeight_min} : will set the minimum size for this dimension</li>
    /// <li>{@code layout_constraintWidth_max} and {@code layout_constraintHeight_max} : will set the maximum size for this dimension</li>
    /// <li>{@code layout_constraintWidth_percent} and {@code layout_constraintHeight_percent} : will set the size of this dimension as a percentage of the parent</li>
    /// </ul>
    /// <h6>Min and Max</h6>
    /// The value indicated for min and max can be either a dimension in Dp, or "wrap", which will use the same value as what {@code WRAP_CONTENT} would do.
    /// <h6>Percent dimension</h6>
    /// To use percent, you need to set the following:
    /// <ul>
    /// <li>The dimension should be set to {@code MATCH_CONSTRAINT} (0dp)</li>
    /// <li>The default should be set to percent {@code app:layout_constraintWidth_default="percent"}
    /// or {@code app:layout_constraintHeight_default="percent"}</li>
    /// <li>Then set the {@code layout_constraintWidth_percent}
    /// or {@code layout_constraintHeight_percent} attributes to a value between 0 and 1</li>
    /// </ul>
    /// </para>
    /// <h5>Ratio</h5>
    /// <para>
    /// You can also define one dimension of a widget as a ratio of the other one. In order to do that, you
    /// need to have at least one constrained dimension be set to {@code 0dp} (i.e., {@code MATCH_CONSTRAINT}), and set the
    /// attribute {@code layout_constraintDimensionRatio} to a given ratio.
    /// For example:
    /// <pre>
    ///         {@code
    ///           <Button android:layout_width="wrap_content"
    ///                   android:layout_height="0dp"
    ///                   app:layout_constraintDimensionRatio="1:1" />
    ///         }
    ///     </pre>
    /// will set the height of the button to be the same as its width.
    /// </para>
    /// <para> The ratio can be expressed either as:
    /// <ul>
    /// <li>a float value, representing a ratio between width and height</li>
    /// <li>a ratio in the form "width:height"</li>
    /// </ul>
    /// </para>
    /// <para>
    /// You can also use ratio if both dimensions are set to {@code MATCH_CONSTRAINT} (0dp). In this case the system sets the
    /// largest dimensions that satisfies all constraints and maintains the aspect ratio specified. To constrain one specific side
    /// based on the dimensions of another, you can pre append {@code W,}" or {@code H,} to constrain the width or height
    /// respectively.
    /// For example,
    /// If one dimension is constrained by two targets (e.g. width is 0dp and centered on parent) you can indicate which
    /// side should be constrained, by adding the letter {@code W} (for constraining the width) or {@code H}
    /// (for constraining the height) in front of the ratio, separated
    /// by a comma:
    /// <pre>
    ///         {@code
    ///           <Button android:layout_width="0dp"
    ///                   android:layout_height="0dp"
    ///                   app:layout_constraintDimensionRatio="H,16:9"
    ///                   app:layout_constraintBottom_toBottomOf="parent"
    ///                   app:layout_constraintTop_toTopOf="parent"/>
    ///         }
    ///     </pre>
    /// will set the height of the button following a 16:9 ratio, while the width of the button will match the constraints
    /// to its parent.
    /// 
    /// </para>
    /// 
    /// <h4 id="Chains">Chains</h4>
    /// <para>Chains provide group-like behavior in a single axis (horizontally or vertically). The other axis can be constrained independently.</para>
    /// <h5>Creating a chain</h5>
    /// <para>
    /// A set of widgets are considered a chain if they are linked together via a bi-directional connection (see Fig. 9, showing a minimal chain, with two widgets).
    /// </para>
    /// <para><div align="center" >
    /// <img width="325px" src="resources/images/chains.png">
    /// <br><b><i>Fig. 9 - Chain</i></b>
    /// </div>
    /// </para>
    /// <para>
    /// <h5>Chain heads</h5>
    /// </para>
    /// <para>
    /// Chains are controlled by attributes set on the first element of the chain (the "head" of the chain):
    /// </para>
    /// <para><div align="center" >
    /// <img width="400px" src="resources/images/chains-head.png">
    /// <br><b><i>Fig. 10 - Chain Head</i></b>
    /// </div>
    /// </para>
    /// <para>The head is the left-most widget for horizontal chains, and the top-most widget for vertical chains.</para>
    /// <h5>Margins in chains</h5>
    /// <para>If margins are specified on connections, they will be taken into account. In the case of spread chains, margins will be deducted from the allocated space.</para>
    /// <h5>Chain Style</h5>
    /// <para>When setting the attribute {@code layout_constraintHorizontal_chainStyle} or {@code layout_constraintVertical_chainStyle} on the first element of a chain,
    /// the behavior of the chain will change according to the specified style (default is {@code CHAIN_SPREAD}).
    /// <ul>
    /// <li>{@code CHAIN_SPREAD} -- the elements will be spread out (default style)</li>
    /// <li>Weighted chain -- in {@code CHAIN_SPREAD} mode, if some widgets are set to {@code MATCH_CONSTRAINT}, they will split the available space</li>
    /// <li>{@code CHAIN_SPREAD_INSIDE} -- similar, but the endpoints of the chain will not be spread out</li>
    /// <li>{@code CHAIN_PACKED} -- the elements of the chain will be packed together. The horizontal or vertical
    /// bias attribute of the child will then affect the positioning of the packed elements</li>
    /// </ul>
    /// </para>
    /// <para><div align="center" >
    /// <img width="600px" src="resources/images/chains-styles.png">
    /// <br><b><i>Fig. 11 - Chains Styles</i></b>
    /// </div>
    /// </para>
    /// <h5>Weighted chains</h5>
    /// <para>The default behavior of a chain is to spread the elements equally in the available space. If one or more elements are using {@code MATCH_CONSTRAINT}, they
    /// will use the available empty space (equally divided among themselves). The attribute {@code layout_constraintHorizontal_weight} and {@code layout_constraintVertical_weight}
    /// will control how the space will be distributed among the elements using {@code MATCH_CONSTRAINT}. For example, on a chain containing two elements using {@code MATCH_CONSTRAINT},
    /// with the first element using a weight of 2 and the second a weight of 1, the space occupied by the first element will be twice that of the second element.</para>
    /// 
    /// <h5>Margins and chains (<i><b>in 1.1</b></i>)</h5>
    /// <para>When using margins on elements in a chain, the margins are additive.</para>
    /// <para>For example, on a horizontal chain, if one element defines a right margin of 10dp and the next element
    /// defines a left margin of 5dp, the resulting margin between those two elements is 15dp.</para>
    /// <para>An item plus its margins are considered together when calculating leftover space used by chains
    /// to position items. The leftover space does not contain the margins.</para>
    /// 
    /// <h4 id="VirtualHelpers"> Virtual Helper objects </h4>
    /// <para>In addition to the intrinsic capabilities detailed previously, you can also use special helper objects
    /// </para>
    /// </summary>
    /// in {@code ConstraintLayout} to help you with your layout. Currently, the {@code Guideline}{<seealso cref= Guideline} object allows you to create
    /// Horizontal and Vertical guidelines which are positioned relative to the {@code ConstraintLayout} container. Widgets can
    /// then be positioned by constraining them to such guidelines. In <b>1.1</b>, {@code Barrier} and {@code Group} were added too.</p>
    /// 
    /// <h4 id="Optimizer">Optimizer (<i><b>in 1.1</b></i>)</h4>
    /// <para>
    /// In 1.1 we exposed the constraints optimizer. You can decide which optimizations are applied by adding the tag <i>app:layout_optimizationLevel</i> to the ConstraintLayout element.
    /// <ul>
    /// <li><b>none</b> : no optimizations are applied</li>
    /// <li><b>standard</b> : Default. Optimize direct and barrier constraints only</li>
    /// <li><b>direct</b> : optimize direct constraints</li>
    /// <li><b>barrier</b> : optimize barrier constraints</li>
    /// <li><b>chain</b> : optimize chain constraints (experimental)</li>
    /// <li><b>dimensions</b> : optimize dimensions measures (experimental), reducing the number of measures of match constraints elements</li>
    /// </ul>
    /// </para>
    /// <para>This attribute is a mask, so you can decide to turn on or off specific optimizations by listing the ones you want.
    /// For example: <i>app:layout_optimizationLevel="direct|barrier|chain"</i> </para>
    /// </div> </seealso>
    public class ConstraintLayout : ViewGroup
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            mMeasurer = new Measurer(this, this);
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public const string VERSION = "ConstraintLayout-2.1.1";
        private const string TAG = "ConstraintLayout";

        private const bool USE_CONSTRAINTS_HELPER = true;
        private const bool DEBUG = LinearSystem.FULL_DEBUG;
        private const bool DEBUG_DRAW_CONSTRAINTS = false;
        private const bool MEASURE = false;
        private const bool OPTIMIZE_HEIGHT_CHANGE = false;

        internal SparseArray<View> mChildrenByIds = new SparseArray<View>();

        // This array keep a list of helper objects if they are present
        private List<ConstraintHelper> mConstraintHelpers = new List<ConstraintHelper>(4);

        protected internal ConstraintWidgetContainer mLayoutWidget = new ConstraintWidgetContainer();

        private int mMinWidth = 0;
        private int mMinHeight = 0;
        private int mMaxWidth = int.MaxValue;
        private int mMaxHeight = int.MaxValue;

        protected internal bool mDirtyHierarchy = true;
        private int mOptimizationLevel = Optimizer.OPTIMIZATION_STANDARD;
        private ConstraintSet mConstraintSet = null;
        protected internal ConstraintLayoutStates mConstraintLayoutSpec = null;

        private int mConstraintSetId = -1;

        private Dictionary<string, int?> mDesignIds = new Dictionary<string, int?>();

        // Cache last measure
        private int mLastMeasureWidth = -1;
        private int mLastMeasureHeight = -1;
        internal int mLastMeasureWidthSize = -1;
        internal int mLastMeasureHeightSize = -1;
        internal int mLastMeasureWidthMode = MeasureSpec.UNSPECIFIED;
        internal int mLastMeasureHeightMode = MeasureSpec.UNSPECIFIED;
        private SparseArray<ConstraintWidget> mTempMapIdToWidget = new SparseArray<ConstraintWidget>();

        /// <summary>
        /// @suppress
        /// </summary>
        public const int DESIGN_INFO_ID = 0;
        private ConstraintsChangedListener mConstraintsChangedListener;
        private Metrics mMetrics;

        private static SharedValues sSharedValues = null;

        /// <summary>
        /// Returns the SharedValues instance, creating it if it doesn't exist.
        /// </summary>
        /// <returns> the SharedValues instance </returns>
        public static SharedValues SharedValues
        {
            get
            {
                if (sSharedValues == null)
                {
                    sSharedValues = new SharedValues();
                }
                return sSharedValues;
            }
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public virtual void setDesignInformation(int type, object value1, object value2)
        {
            if (type == DESIGN_INFO_ID && value1 is string && value2 is int?)
            {
                if (mDesignIds == null)
                {
                    mDesignIds = new Dictionary<>();
                }
                string name = (string) value1;
                int index = name.IndexOf("/", StringComparison.Ordinal);
                if (index != -1)
                {
                    name = name.Substring(index + 1);
                }
                int id = (int?) value2;
                mDesignIds[name] = id;
            }
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public virtual object getDesignInformation(int type, object value)
        {
            if (type == DESIGN_INFO_ID && value is string)
            {
                string name = (string) value;
                if (mDesignIds != null && mDesignIds.ContainsKey(name))
                {
                    return mDesignIds[name];
                }
            }
            return null;
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public ConstraintLayout(@NonNull android.content.Context context)
        public ConstraintLayout(Context context) : base(context)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            init(null, 0, 0);
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public ConstraintLayout(@NonNull android.content.Context context, @Nullable android.util.AttributeSet attrs)
        public ConstraintLayout(Context context, AttributeSet attrs) : base(context, attrs)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            init(attrs, 0, 0);
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public ConstraintLayout(@NonNull android.content.Context context, @Nullable android.util.AttributeSet attrs, int defStyleAttr)
        public ConstraintLayout(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            init(attrs, defStyleAttr, 0);
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TargetApi(android.os.Build.VERSION_CODES.LOLLIPOP) public ConstraintLayout(@NonNull android.content.Context context, @Nullable android.util.AttributeSet attrs, int defStyleAttr, int defStyleRes)
        public ConstraintLayout(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            init(attrs, defStyleAttr, defStyleRes);
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public override int Id
        {
            set
            {
                mChildrenByIds.remove(Id);
                base.Id = value;
                mChildrenByIds.put(Id, this);
            }
        }

        // -------------------------------------------------------------------------------------------
        // Measure widgets callbacks
        // -------------------------------------------------------------------------------------------


        // -------------------------------------------------------------------------------------------

        internal class Measurer : BasicMeasure.Measurer
        {
            private readonly ConstraintLayout outerInstance;

            internal ConstraintLayout layout;
            internal int paddingTop;
            internal int paddingBottom;
            internal int paddingWidth;
            internal int paddingHeight;
            internal int layoutWidthSpec;
            internal int layoutHeightSpec;

            public virtual void captureLayoutInfo(int widthSpec, int heightSpec, int top, int bottom, int width, int height)
            {
                paddingTop = top;
                paddingBottom = bottom;
                paddingWidth = width;
                paddingHeight = height;
                layoutWidthSpec = widthSpec;
                layoutHeightSpec = heightSpec;
            }

            public Measurer(ConstraintLayout outerInstance, ConstraintLayout l)
            {
                this.outerInstance = outerInstance;
                layout = l;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressLint("WrongCall") @Override public final void measure(androidx.constraintlayout.core.widgets.ConstraintWidget widget, androidx.constraintlayout.core.widgets.analyzer.BasicMeasure.Measure measure)
            public void measure(ConstraintWidget widget, BasicMeasure.Measure measure)
            {
                if (widget == null)
                {
                    return;
                }
                if (widget.Visibility == GONE && !widget.InPlaceholder)
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

                long startMeasure;
                long endMeasure;

                if (MEASURE)
                {
                    startMeasure = System.nanoTime();
                }

                ConstraintWidget.DimensionBehaviour horizontalBehavior = measure.horizontalBehavior;
                ConstraintWidget.DimensionBehaviour verticalBehavior = measure.verticalBehavior;

                int horizontalDimension = measure.horizontalDimension;
                int verticalDimension = measure.verticalDimension;

                int horizontalSpec = 0;
                int verticalSpec = 0;

                int heightPadding = paddingTop + paddingBottom;
                int widthPadding = paddingWidth;

                View child = (View) widget.CompanionWidget;

                switch (horizontalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                    {
                        horizontalSpec = MeasureSpec.makeMeasureSpec(horizontalDimension, MeasureSpec.EXACTLY);
                    }
                    break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                    {
                        horizontalSpec = getChildMeasureSpec(layoutWidthSpec, widthPadding, WRAP_CONTENT);
                    }
                    break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    {
                        // Horizontal spec must account for margin as well as padding here.
                        horizontalSpec = getChildMeasureSpec(layoutWidthSpec, widthPadding + widget.HorizontalMargin, LayoutParams.MATCH_PARENT);
                    }
                    break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                    {
                        horizontalSpec = getChildMeasureSpec(layoutWidthSpec, widthPadding, WRAP_CONTENT);
                        bool shouldDoWrap = widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP;
                        if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                        {
                            // the solver gives us our new dimension, but if we previously had it measured with
                            // a wrap, it can be incorrect if the other side was also variable.
                            // So in that case, we have to double-check the other side is stable (else we can't
                            // just assume the wrap value will be correct).
                            bool otherDimensionStable = child.MeasuredHeight == widget.Height;
                            bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedHorizontally);
                            if (useCurrent)
                            {
                                horizontalSpec = MeasureSpec.makeMeasureSpec(widget.Width, MeasureSpec.EXACTLY);
                            }
                        }
                    }
                    break;
                }

                switch (verticalBehavior)
                {
                    case ConstraintWidget.DimensionBehaviour.FIXED:
                    {
                        verticalSpec = MeasureSpec.makeMeasureSpec(verticalDimension, MeasureSpec.EXACTLY);
                    }
                    break;
                    case ConstraintWidget.DimensionBehaviour.WRAP_CONTENT:
                    {
                        verticalSpec = getChildMeasureSpec(layoutHeightSpec, heightPadding, WRAP_CONTENT);
                    }
                    break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_PARENT:
                    {
                        // Vertical spec must account for margin as well as padding here.
                        verticalSpec = getChildMeasureSpec(layoutHeightSpec, heightPadding + widget.VerticalMargin, LayoutParams.MATCH_PARENT);
                    }
                    break;
                    case ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT:
                    {
                        verticalSpec = getChildMeasureSpec(layoutHeightSpec, heightPadding, WRAP_CONTENT);
                        bool shouldDoWrap = widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP;
                        if (measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS)
                        {
                            // the solver gives us our new dimension, but if we previously had it measured with
                            // a wrap, it can be incorrect if the other side was also variable.
                            // So in that case, we have to double-check the other side is stable (else we can't
                            // just assume the wrap value will be correct).
                            bool otherDimensionStable = child.MeasuredWidth == widget.Width;
                            bool useCurrent = measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS || !shouldDoWrap || (shouldDoWrap && otherDimensionStable) || (child is Placeholder) || (widget.ResolvedVertically);
                            if (useCurrent)
                            {
                                verticalSpec = MeasureSpec.makeMeasureSpec(widget.Height, MeasureSpec.EXACTLY);
                            }
                        }
                    }
                    break;
                }

                ConstraintWidgetContainer container = (ConstraintWidgetContainer) widget.Parent;
                if (container != null && Optimizer.enabled(outerInstance.mOptimizationLevel, Optimizer.OPTIMIZATION_CACHE_MEASURES))
                {
                    if (child.MeasuredWidth == widget.Width && child.MeasuredWidth < container.Width && child.MeasuredHeight == widget.Height && child.MeasuredHeight < container.Height && child.Baseline == widget.BaselineDistance && !widget.MeasureRequested)
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
                                Console.WriteLine("SKIPPED " + widget);
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
                LayoutParams @params = (LayoutParams) child.LayoutParams;

                int width = 0;
                int height = 0;
                int baseline = 0;

                if ((measure.measureStrategy == BasicMeasure.Measure.TRY_GIVEN_DIMENSIONS || measure.measureStrategy == BasicMeasure.Measure.USE_GIVEN_DIMENSIONS) || !(horizontalMatchConstraints && widget.mMatchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD && verticalMatchConstraints && widget.mMatchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD))
                {

                    if (child is VirtualLayout && widget is androidx.constraintlayout.core.widgets.VirtualLayout)
                    {
                        androidx.constraintlayout.core.widgets.VirtualLayout layout = (androidx.constraintlayout.core.widgets.VirtualLayout) widget;
                        ((VirtualLayout) child).onMeasure(layout, horizontalSpec, verticalSpec);
                    }
                    else
                    {
                        child.measure(horizontalSpec, verticalSpec);
                    }
                    widget.setLastMeasureSpec(horizontalSpec, verticalSpec);

                    int w = child.MeasuredWidth;
                    int h = child.MeasuredHeight;
                    baseline = child.Baseline;

                    width = w;
                    height = h;

                    if (DEBUG)
                    {
                        string measurement = MeasureSpec.ToString(horizontalSpec) + " x " + MeasureSpec.ToString(verticalSpec) + " => " + width + " x " + height;
                        Console.WriteLine("    (M) measure " + " (" + widget.DebugName + ") : " + measurement);
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
                            horizontalSpec = MeasureSpec.makeMeasureSpec(width, MeasureSpec.EXACTLY);
                        }
                        if (h != height)
                        {
                            verticalSpec = MeasureSpec.makeMeasureSpec(height, MeasureSpec.EXACTLY);
                        }
                        child.measure(horizontalSpec, verticalSpec);

                        widget.setLastMeasureSpec(horizontalSpec, verticalSpec);
                        width = child.MeasuredWidth;
                        height = child.MeasuredHeight;
                        baseline = child.Baseline;
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
                    endMeasure = System.nanoTime();
                    if (outerInstance.mMetrics != null)
                    {
                        outerInstance.mMetrics.measuresWidgetsDuration += (endMeasure - startMeasure);
                    }
                }
            }

            /// <summary>
            /// Returns true if the previous measure spec is equivalent to the new one.
            /// - if it's the same...
            /// - if it's not, but the previous was AT_MOST or UNSPECIFIED and the new one
            ///   is EXACTLY with the same size.
            /// </summary>
            /// <param name="lastMeasureSpec"> </param>
            /// <param name="spec"> </param>
            /// <param name="widgetSize">
            /// @return </param>
            internal virtual bool isSimilarSpec(int lastMeasureSpec, int spec, int widgetSize)
            {
                if (lastMeasureSpec == spec)
                {
                    return true;
                }
                int lastMode = MeasureSpec.getMode(lastMeasureSpec);
                int lastSize = MeasureSpec.getSize(lastMeasureSpec);
                int mode = MeasureSpec.getMode(spec);
                int size = MeasureSpec.getSize(spec);
                if (mode == MeasureSpec.EXACTLY && (lastMode == MeasureSpec.AT_MOST || lastMode == MeasureSpec.UNSPECIFIED) && widgetSize == size)
                {
                    return true;
                }
                return false;
            }

            public void didMeasures()
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int widgetsCount = layout.getChildCount();
                int widgetsCount = layout.ChildCount;
                for (int i = 0; i < widgetsCount; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View child = layout.getChildAt(i);
                    View child = layout.getChildAt(i);
                    if (child is Placeholder)
                    {
                        ((Placeholder) child).updatePostMeasure(layout);
                    }
                }
                // TODO refactor into an updatePostMeasure interface
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int helperCount = layout.mConstraintHelpers.size();
                int helperCount = layout.mConstraintHelpers.Count;
                if (helperCount > 0)
                {
                    for (int i = 0; i < helperCount; i++)
                    {
                        ConstraintHelper helper = layout.mConstraintHelpers[i];
                        helper.updatePostMeasure(layout);
                    }
                }
            }
        }

        internal Measurer mMeasurer;

        private void init(AttributeSet attrs, int defStyleAttr, int defStyleRes)
        {
            mLayoutWidget.CompanionWidget = this;
            mLayoutWidget.Measurer = mMeasurer;
            mChildrenByIds.put(Id, this);
            mConstraintSet = null;
            if (attrs != null)
            {
                TypedArray a = Context.obtainStyledAttributes(attrs, R.styleable.ConstraintLayout_Layout, defStyleAttr, defStyleRes);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
                int N = a.IndexCount;
                for (int i = 0; i < N; i++)
                {
                    int attr = a.getIndex(i);
                    if (attr == R.styleable.ConstraintLayout_Layout_android_minWidth)
                    {
                        mMinWidth = a.getDimensionPixelOffset(attr, mMinWidth);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_android_minHeight)
                    {
                        mMinHeight = a.getDimensionPixelOffset(attr, mMinHeight);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_android_maxWidth)
                    {
                        mMaxWidth = a.getDimensionPixelOffset(attr, mMaxWidth);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_android_maxHeight)
                    {
                        mMaxHeight = a.getDimensionPixelOffset(attr, mMaxHeight);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_layout_optimizationLevel)
                    {
                        mOptimizationLevel = a.getInt(attr, mOptimizationLevel);
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_layoutDescription)
                    {
                        int id = a.getResourceId(attr, 0);
                        if (id != 0)
                        {
                            try
                            {
                                parseLayoutDescription(id);
                            }
                            catch (Resources.NotFoundException)
                            {
                                mConstraintLayoutSpec = null;
                            }
                        }
                    }
                    else if (attr == R.styleable.ConstraintLayout_Layout_constraintSet)
                    {
                        int id = a.getResourceId(attr, 0);
                        try
                        {
                            mConstraintSet = new ConstraintSet();
                            mConstraintSet.load(Context, id);
                        }
                        catch (Resources.NotFoundException)
                        {
                            mConstraintSet = null;
                        }
                        mConstraintSetId = id;
                    }
                }
                a.recycle();
            }
            mLayoutWidget.OptimizationLevel = mOptimizationLevel;
        }

        /// <summary>
        /// Subclasses can override the handling of layoutDescription
        /// </summary>
        /// <param name="id"> </param>
        protected internal virtual void parseLayoutDescription(int id)
        {
            mConstraintLayoutSpec = new ConstraintLayoutStates(Context, this, id);
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public override void onViewAdded(View view)
        {
            base.onViewAdded(view);
            ConstraintWidget widget = getViewWidget(view);
            if (view is androidx.constraintlayout.widget.Guideline)
            {
                if (!(widget is Guideline))
                {
                    LayoutParams layoutParams = (LayoutParams) view.LayoutParams;
                    layoutParams.widget = new Guideline();
                    layoutParams.isGuideline = true;
                    ((Guideline) layoutParams.widget).Orientation = layoutParams.orientation;
                }
            }
            if (view is ConstraintHelper)
            {
                ConstraintHelper helper = (ConstraintHelper) view;
                helper.validateParams();
                LayoutParams layoutParams = (LayoutParams) view.LayoutParams;
                layoutParams.isHelper = true;
                if (!mConstraintHelpers.Contains(helper))
                {
                    mConstraintHelpers.Add(helper);
                }
            }
            mChildrenByIds.put(view.Id, view);
            mDirtyHierarchy = true;
        }

        /// <summary>
        /// @suppress
        /// </summary>
        public override void onViewRemoved(View view)
        {
            base.onViewRemoved(view);
            mChildrenByIds.remove(view.Id);
            ConstraintWidget widget = getViewWidget(view);
            mLayoutWidget.remove(widget);
            mConstraintHelpers.Remove(view);
            mDirtyHierarchy = true;
        }

        /// <summary>
        /// Set the min width for this view
        /// </summary>
        /// <param name="value"> </param>
        public virtual int MinWidth
        {
            set
            {
                if (value == mMinWidth)
                {
                    return;
                }
                mMinWidth = value;
                requestLayout();
            }
            get
            {
                return mMinWidth;
            }
        }

        /// <summary>
        /// Set the min height for this view
        /// </summary>
        /// <param name="value"> </param>
        public virtual int MinHeight
        {
            set
            {
                if (value == mMinHeight)
                {
                    return;
                }
                mMinHeight = value;
                requestLayout();
            }
            get
            {
                return mMinHeight;
            }
        }



        /// <summary>
        /// Set the max width for this view
        /// </summary>
        /// <param name="value"> </param>
        public virtual int MaxWidth
        {
            set
            {
                if (value == mMaxWidth)
                {
                    return;
                }
                mMaxWidth = value;
                requestLayout();
            }
            get
            {
                return mMaxWidth;
            }
        }

        /// <summary>
        /// Set the max height for this view
        /// </summary>
        /// <param name="value"> </param>
        public virtual int MaxHeight
        {
            set
            {
                if (value == mMaxHeight)
                {
                    return;
                }
                mMaxHeight = value;
                requestLayout();
            }
            get
            {
                return mMaxHeight;
            }
        }



        private bool updateHierarchy()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;

            bool recompute = false;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View child = getChildAt(i);
                View child = getChildAt(i);
                if (child.LayoutRequested)
                {
                    recompute = true;
                    break;
                }
            }
            if (recompute)
            {
                setChildrenConstraints();
            }
            return recompute;
        }

        private void setChildrenConstraints()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isInEditMode = DEBUG || isInEditMode();
            bool isInEditMode = DEBUG || InEditMode;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;

            // Make sure everything is fully reset before anything else
            for (int i = 0; i < count; i++)
            {
                View child = getChildAt(i);
                ConstraintWidget widget = getViewWidget(child);
                if (widget == null)
                {
                    continue;
                }
                widget.reset();
            }

            if (isInEditMode)
            {
                // In design mode, let's make sure we keep track of the ids; in Studio, a build step
                // might not have been done yet, so asking the system for ids can break. So to be safe,
                // we save the current ids, which helpers can ask for.
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View view = getChildAt(i);
                    View view = getChildAt(i);
                    try
                    {
                        string IdAsString = Resources.getResourceName(view.Id);
                        setDesignInformation(DESIGN_INFO_ID, IdAsString, view.Id);
                        int slashIndex = IdAsString.IndexOf('/');
                        if (slashIndex != -1)
                        {
                            IdAsString = IdAsString.Substring(slashIndex + 1);
                        }
                        getTargetWidget(view.Id).DebugName = IdAsString;
                    }
                    catch (Resources.NotFoundException)
                    {
                        // nothing
                    }
                }
            }
            else if (DEBUG)
            {
                mLayoutWidget.DebugName = "root";
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View view = getChildAt(i);
                    View view = getChildAt(i);
                    try
                    {
                        string IdAsString = Resources.getResourceName(view.Id);
                        setDesignInformation(DESIGN_INFO_ID, IdAsString, view.Id);
                        int slashIndex = IdAsString.IndexOf('/');
                        if (slashIndex != -1)
                        {
                            IdAsString = IdAsString.Substring(slashIndex + 1);
                        }
                        getTargetWidget(view.Id).DebugName = IdAsString;
                    }
                    catch (Resources.NotFoundException)
                    {
                        // nothing
                    }
                }
            }

            if (USE_CONSTRAINTS_HELPER && mConstraintSetId != -1)
            {
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View child = getChildAt(i);
                    View child = getChildAt(i);
                    if (child.Id == mConstraintSetId && child is Constraints)
                    {
                        mConstraintSet = ((Constraints) child).ConstraintSet;
                    }
                }
            }

            if (mConstraintSet != null)
            {
                mConstraintSet.applyToInternal(this, true);
            }

            mLayoutWidget.removeAllChildren();

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int helperCount = mConstraintHelpers.size();
            int helperCount = mConstraintHelpers.Count;
            if (helperCount > 0)
            {
                for (int i = 0; i < helperCount; i++)
                {
                    ConstraintHelper helper = mConstraintHelpers[i];
                    helper.updatePreLayout(this);
                }
            }

            // TODO refactor into an updatePreLayout interface
            for (int i = 0; i < count; i++)
            {
                View child = getChildAt(i);
                if (child is Placeholder)
                {
                    ((Placeholder) child).updatePreLayout(this);
                }
            }

            mTempMapIdToWidget.clear();
            mTempMapIdToWidget.put(PARENT_ID, mLayoutWidget);
            mTempMapIdToWidget.put(Id, mLayoutWidget);
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View child = getChildAt(i);
                View child = getChildAt(i);
                ConstraintWidget widget = getViewWidget(child);
                mTempMapIdToWidget.put(child.Id, widget);
            }

            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View child = getChildAt(i);
                View child = getChildAt(i);
                ConstraintWidget widget = getViewWidget(child);
                if (widget == null)
                {
                    continue;
                }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LayoutParams layoutParams = (LayoutParams) child.getLayoutParams();
                LayoutParams layoutParams = (LayoutParams) child.LayoutParams;
                mLayoutWidget.add(widget);
                applyConstraintsFromLayoutParams(isInEditMode, child, widget, layoutParams, mTempMapIdToWidget);
            }
        }


        protected internal virtual void applyConstraintsFromLayoutParams(bool isInEditMode, View child, ConstraintWidget widget, LayoutParams layoutParams, SparseArray<ConstraintWidget> idToWidget)
        {

            layoutParams.validate();
            layoutParams.helped = false;

            widget.Visibility = child.Visibility;
            if (layoutParams.isInPlaceholder)
            {
                widget.InPlaceholder = true;
                widget.Visibility = View.GONE;
            }
            widget.CompanionWidget = child;

            if (child is ConstraintHelper)
            {
                ConstraintHelper helper = (ConstraintHelper) child;
                helper.resolveRtl(widget, mLayoutWidget.Rtl);
            }
            if (layoutParams.isGuideline)
            {
                Guideline guideline = (Guideline) widget;
                int resolvedGuideBegin = layoutParams.resolvedGuideBegin;
                int resolvedGuideEnd = layoutParams.resolvedGuideEnd;
                float resolvedGuidePercent = layoutParams.resolvedGuidePercent;
                if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1)
                {
                    resolvedGuideBegin = layoutParams.guideBegin;
                    resolvedGuideEnd = layoutParams.guideEnd;
                    resolvedGuidePercent = layoutParams.guidePercent;
                }
                if (resolvedGuidePercent != UNSET)
                {
                    guideline.GuidePercent = resolvedGuidePercent;
                }
                else if (resolvedGuideBegin != UNSET)
                {
                    guideline.GuideBegin = resolvedGuideBegin;
                }
                else if (resolvedGuideEnd != UNSET)
                {
                    guideline.GuideEnd = resolvedGuideEnd;
                }
            }
            else
            {
                // Get the left/right constraints resolved for RTL
                int resolvedLeftToLeft = layoutParams.resolvedLeftToLeft;
                int resolvedLeftToRight = layoutParams.resolvedLeftToRight;
                int resolvedRightToLeft = layoutParams.resolvedRightToLeft;
                int resolvedRightToRight = layoutParams.resolvedRightToRight;
                int resolveGoneLeftMargin = layoutParams.resolveGoneLeftMargin;
                int resolveGoneRightMargin = layoutParams.resolveGoneRightMargin;
                float resolvedHorizontalBias = layoutParams.resolvedHorizontalBias;

                if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1)
                {
                    // Pre JB MR1, left/right should take precedence, unless they are
                    // not defined and somehow a corresponding start/end constraint exists
                    resolvedLeftToLeft = layoutParams.leftToLeft;
                    resolvedLeftToRight = layoutParams.leftToRight;
                    resolvedRightToLeft = layoutParams.rightToLeft;
                    resolvedRightToRight = layoutParams.rightToRight;
                    resolveGoneLeftMargin = layoutParams.goneLeftMargin;
                    resolveGoneRightMargin = layoutParams.goneRightMargin;
                    resolvedHorizontalBias = layoutParams.horizontalBias;

                    if (resolvedLeftToLeft == UNSET && resolvedLeftToRight == UNSET)
                    {
                        if (layoutParams.startToStart != UNSET)
                        {
                            resolvedLeftToLeft = layoutParams.startToStart;
                        }
                        else if (layoutParams.startToEnd != UNSET)
                        {
                            resolvedLeftToRight = layoutParams.startToEnd;
                        }
                    }
                    if (resolvedRightToLeft == UNSET && resolvedRightToRight == UNSET)
                    {
                        if (layoutParams.endToStart != UNSET)
                        {
                            resolvedRightToLeft = layoutParams.endToStart;
                        }
                        else if (layoutParams.endToEnd != UNSET)
                        {
                            resolvedRightToRight = layoutParams.endToEnd;
                        }
                    }
                }

                // Circular constraint
                if (layoutParams.circleConstraint != UNSET)
                {
                    ConstraintWidget target = idToWidget.get(layoutParams.circleConstraint);
                    if (target != null)
                    {
                        widget.connectCircularConstraint(target, layoutParams.circleAngle, layoutParams.circleRadius);
                    }
                }
                else
                {
                    // Left constraint
                    if (resolvedLeftToLeft != UNSET)
                    {
                        ConstraintWidget target = idToWidget.get(resolvedLeftToLeft);
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.LEFT, layoutParams.leftMargin, resolveGoneLeftMargin);
                        }
                    }
                    else if (resolvedLeftToRight != UNSET)
                    {
                        ConstraintWidget target = idToWidget.get(resolvedLeftToRight);
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.LEFT, target, ConstraintAnchor.Type.RIGHT, layoutParams.leftMargin, resolveGoneLeftMargin);
                        }
                    }

                    // Right constraint
                    if (resolvedRightToLeft != UNSET)
                    {
                        ConstraintWidget target = idToWidget.get(resolvedRightToLeft);
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.LEFT, layoutParams.rightMargin, resolveGoneRightMargin);
                        }
                    }
                    else if (resolvedRightToRight != UNSET)
                    {
                        ConstraintWidget target = idToWidget.get(resolvedRightToRight);
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.RIGHT, target, ConstraintAnchor.Type.RIGHT, layoutParams.rightMargin, resolveGoneRightMargin);
                        }
                    }

                    // Top constraint
                    if (layoutParams.topToTop != UNSET)
                    {
                        ConstraintWidget target = idToWidget.get(layoutParams.topToTop);
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.TOP, layoutParams.topMargin, layoutParams.goneTopMargin);
                        }
                    }
                    else if (layoutParams.topToBottom != UNSET)
                    {
                        ConstraintWidget target = idToWidget.get(layoutParams.topToBottom);
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.TOP, target, ConstraintAnchor.Type.BOTTOM, layoutParams.topMargin, layoutParams.goneTopMargin);
                        }
                    }

                    // Bottom constraint
                    if (layoutParams.bottomToTop != UNSET)
                    {
                        ConstraintWidget target = idToWidget.get(layoutParams.bottomToTop);
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.TOP, layoutParams.bottomMargin, layoutParams.goneBottomMargin);
                        }
                    }
                    else if (layoutParams.bottomToBottom != UNSET)
                    {
                        ConstraintWidget target = idToWidget.get(layoutParams.bottomToBottom);
                        if (target != null)
                        {
                            widget.immediateConnect(ConstraintAnchor.Type.BOTTOM, target, ConstraintAnchor.Type.BOTTOM, layoutParams.bottomMargin, layoutParams.goneBottomMargin);
                        }
                    }

                    // Baseline constraint
                    if (layoutParams.baselineToBaseline != UNSET)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.baselineToBaseline, ConstraintAnchor.Type.BASELINE);
                    }
                    else if (layoutParams.baselineToTop != UNSET)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.baselineToTop, ConstraintAnchor.Type.TOP);
                    }
                    else if (layoutParams.baselineToBottom != UNSET)
                    {
                        setWidgetBaseline(widget, layoutParams, idToWidget, layoutParams.baselineToBottom, ConstraintAnchor.Type.BOTTOM);
                    }

                    if (resolvedHorizontalBias >= 0)
                    {
                        widget.HorizontalBiasPercent = resolvedHorizontalBias;
                    }
                    if (layoutParams.verticalBias >= 0)
                    {
                        widget.VerticalBiasPercent = layoutParams.verticalBias;
                    }
                }

                if (isInEditMode && ((layoutParams.editorAbsoluteX != UNSET) || (layoutParams.editorAbsoluteY != UNSET)))
                {
                    widget.setOrigin(layoutParams.editorAbsoluteX, layoutParams.editorAbsoluteY);
                }

                // FIXME: need to agree on the correct magic value for this rather than simply using zero.
                if (!layoutParams.horizontalDimensionFixed)
                {
                    if (layoutParams.width == MATCH_PARENT)
                    {
                        if (layoutParams.constrainedWidth)
                        {
                            widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        }
                        else
                        {
                            widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
                        }
                        widget.getAnchor(ConstraintAnchor.Type.LEFT).mMargin = layoutParams.leftMargin;
                        widget.getAnchor(ConstraintAnchor.Type.RIGHT).mMargin = layoutParams.rightMargin;
                    }
                    else
                    {
                        widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        widget.Width = 0;
                    }
                }
                else
                {
                    widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
                    widget.Width = layoutParams.width;
                    if (layoutParams.width == WRAP_CONTENT)
                    {
                        widget.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }
                if (!layoutParams.verticalDimensionFixed)
                {
                    if (layoutParams.height == MATCH_PARENT)
                    {
                        if (layoutParams.constrainedHeight)
                        {
                            widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                        }
                        else
                        {
                            widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_PARENT;
                        }
                        widget.getAnchor(ConstraintAnchor.Type.TOP).mMargin = layoutParams.topMargin;
                        widget.getAnchor(ConstraintAnchor.Type.BOTTOM).mMargin = layoutParams.bottomMargin;
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
                    widget.Height = layoutParams.height;
                    if (layoutParams.height == WRAP_CONTENT)
                    {
                        widget.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    }
                }

                widget.setDimensionRatio(layoutParams.dimensionRatio);
                widget.HorizontalWeight = layoutParams.horizontalWeight;
                widget.VerticalWeight = layoutParams.verticalWeight;
                widget.HorizontalChainStyle = layoutParams.horizontalChainStyle;
                widget.VerticalChainStyle = layoutParams.verticalChainStyle;
                widget.WrapBehaviorInParent = layoutParams.wrapBehaviorInParent;
                widget.setHorizontalMatchStyle(layoutParams.matchConstraintDefaultWidth, layoutParams.matchConstraintMinWidth, layoutParams.matchConstraintMaxWidth, layoutParams.matchConstraintPercentWidth);
                widget.setVerticalMatchStyle(layoutParams.matchConstraintDefaultHeight, layoutParams.matchConstraintMinHeight, layoutParams.matchConstraintMaxHeight, layoutParams.matchConstraintPercentHeight);
            }
        }

        private void setWidgetBaseline(ConstraintWidget widget, LayoutParams layoutParams, SparseArray<ConstraintWidget> idToWidget, int baselineTarget, ConstraintAnchor.Type type)
        {
            View view = mChildrenByIds.get(baselineTarget);
            ConstraintWidget target = idToWidget.get(baselineTarget);
            if (target != null && view != null && view.LayoutParams is LayoutParams)
            {
                layoutParams.needsBaseline = true;
                if (type == ConstraintAnchor.Type.BASELINE)
                { // baseline to baseline
                    LayoutParams targetParams = (LayoutParams) view.LayoutParams;
                    targetParams.needsBaseline = true;
                    targetParams.widget.HasBaseline = true;
                }
                ConstraintAnchor baseline = widget.getAnchor(ConstraintAnchor.Type.BASELINE);
                ConstraintAnchor targetAnchor = target.getAnchor(type);
                baseline.connect(targetAnchor, layoutParams.baselineMargin, layoutParams.goneBaselineMargin, true);
                widget.HasBaseline = true;
                widget.getAnchor(ConstraintAnchor.Type.TOP).reset();
                widget.getAnchor(ConstraintAnchor.Type.BOTTOM).reset();
            }
        }

        private ConstraintWidget getTargetWidget(int id)
        {
            if (id == LayoutParams.PARENT_ID)
            {
                return mLayoutWidget;
            }
            else
            {
                View view = mChildrenByIds.get(id);
                if (view == null)
                {
                    view = findViewById(id);
                    if (view != null && view != this && view.Parent == this)
                    {
                        onViewAdded(view);
                    }
                }
                if (view == this)
                {
                    return mLayoutWidget;
                }
                return view == null ? null : ((LayoutParams) view.LayoutParams).widget;
            }
        }

        /// <param name="view">
        /// @return
        /// @suppress </param>
        public ConstraintWidget getViewWidget(View view)
        {
            if (view == this)
            {
                return mLayoutWidget;
            }
            if (view != null)
            {
                if (view.LayoutParams is LayoutParams)
                {
                    return ((LayoutParams) view.LayoutParams).widget;
                }
                view.LayoutParams = generateLayoutParams(view.LayoutParams);
                if (view.LayoutParams is LayoutParams)
                {
                    return ((LayoutParams) view.LayoutParams).widget;
                }
            }
            return null;
        }

        /// <param name="metrics">
        /// @suppress Fills metrics object </param>
        public virtual void fillMetrics(Metrics metrics)
        {
            mMetrics = metrics;
            mLayoutWidget.fillMetrics(metrics);
        }

        private int mOnMeasureWidthMeasureSpec = 0;
        private int mOnMeasureHeightMeasureSpec = 0;

        /// <summary>
        /// Handles measuring a layout
        /// </summary>
        /// <param name="layout"> </param>
        /// <param name="optimizationLevel"> </param>
        /// <param name="widthMeasureSpec"> </param>
        /// <param name="heightMeasureSpec"> </param>
        protected internal virtual void resolveSystem(ConstraintWidgetContainer layout, int optimizationLevel, int widthMeasureSpec, int heightMeasureSpec)
        {
            int widthMode = MeasureSpec.getMode(widthMeasureSpec);
            int widthSize = MeasureSpec.getSize(widthMeasureSpec);
            int heightMode = MeasureSpec.getMode(heightMeasureSpec);
            int heightSize = MeasureSpec.getSize(heightMeasureSpec);

            int paddingY = Math.Max(0, PaddingTop);
            int paddingBottom = Math.Max(0, PaddingBottom);
            int paddingHeight = paddingY + paddingBottom;
            int paddingWidth = PaddingWidth;
            int paddingX;
            mMeasurer.captureLayoutInfo(widthMeasureSpec, heightMeasureSpec, paddingY, paddingBottom, paddingWidth, paddingHeight);

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
            {
                int paddingStart = Math.Max(0, PaddingStart);
                int paddingEnd = Math.Max(0, PaddingEnd);
                if (paddingStart > 0 || paddingEnd > 0)
                {
                    if (Rtl)
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

        /// <summary>
        /// Handles calling setMeasuredDimension()
        /// </summary>
        /// <param name="widthMeasureSpec"> </param>
        /// <param name="heightMeasureSpec"> </param>
        /// <param name="measuredWidth"> </param>
        /// <param name="measuredHeight"> </param>
        /// <param name="isWidthMeasuredTooSmall"> </param>
        /// <param name="isHeightMeasuredTooSmall"> </param>
        protected internal virtual void resolveMeasuredDimension(int widthMeasureSpec, int heightMeasureSpec, int measuredWidth, int measuredHeight, bool isWidthMeasuredTooSmall, bool isHeightMeasuredTooSmall)
        {
            int childState = 0;
            int heightPadding = mMeasurer.paddingHeight;
            int widthPadding = mMeasurer.paddingWidth;

            int androidLayoutWidth = measuredWidth + widthPadding;
            int androidLayoutHeight = measuredHeight + heightPadding;

            int resolvedWidthSize = resolveSizeAndState(androidLayoutWidth, widthMeasureSpec, childState);
            int resolvedHeightSize = resolveSizeAndState(androidLayoutHeight, heightMeasureSpec, childState << MEASURED_HEIGHT_STATE_SHIFT);
            resolvedWidthSize &= MEASURED_SIZE_MASK;
            resolvedHeightSize &= MEASURED_SIZE_MASK;
            resolvedWidthSize = Math.Min(mMaxWidth, resolvedWidthSize);
            resolvedHeightSize = Math.Min(mMaxHeight, resolvedHeightSize);
            if (isWidthMeasuredTooSmall)
            {
                resolvedWidthSize |= MEASURED_STATE_TOO_SMALL;
            }
            if (isHeightMeasuredTooSmall)
            {
                resolvedHeightSize |= MEASURED_STATE_TOO_SMALL;
            }
            setMeasuredDimension(resolvedWidthSize, resolvedHeightSize);
            mLastMeasureWidth = resolvedWidthSize;
            mLastMeasureHeight = resolvedHeightSize;
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            long time = 0;
            if (DEBUG)
            {
                time = DateTimeHelperClass.CurrentUnixTimeMillis();
            }

            bool sameSpecsAsPreviousMeasure = (mOnMeasureWidthMeasureSpec == widthMeasureSpec && mOnMeasureHeightMeasureSpec == heightMeasureSpec);
            sameSpecsAsPreviousMeasure = false; //TODO re-enable
            if (!mDirtyHierarchy && !sameSpecsAsPreviousMeasure)
            {
                // it's possible that, if we are already marked for a relayout, a view would not call to request a layout;
                // in that case we'd miss updating the hierarchy correctly (window insets change may do that -- we receive
                // a second onMeasure before onLayout).
                // We have to iterate on our children to verify that none set a request layout flag...
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View child = getChildAt(i);
                    View child = getChildAt(i);
                    if (child.LayoutRequested)
                    {
                        if (DEBUG)
                        {
                            Console.WriteLine("### CHILD " + child + " REQUESTED LAYOUT, FORCE DIRTY HIERARCHY");
                        }
                        mDirtyHierarchy = true;
                        break;
                    }
                }
            }

            if (!mDirtyHierarchy)
            {
                if (sameSpecsAsPreviousMeasure)
                {
                    resolveMeasuredDimension(widthMeasureSpec, heightMeasureSpec, mLayoutWidget.Width, mLayoutWidget.Height, mLayoutWidget.WidthMeasuredTooSmall, mLayoutWidget.HeightMeasuredTooSmall);
                    return;
                }
                if (OPTIMIZE_HEIGHT_CHANGE && mOnMeasureWidthMeasureSpec == widthMeasureSpec && MeasureSpec.getMode(widthMeasureSpec) == MeasureSpec.EXACTLY && MeasureSpec.getMode(heightMeasureSpec) == MeasureSpec.AT_MOST && MeasureSpec.getMode(mOnMeasureHeightMeasureSpec) == MeasureSpec.AT_MOST)
                {
                    int newSize = MeasureSpec.getSize(heightMeasureSpec);
                    if (DEBUG)
                    {
                        Console.WriteLine("### COMPATIBLE REQ " + newSize + " >= ? " + mLayoutWidget.Height);
                    }
                    if (newSize >= mLayoutWidget.Height && !mLayoutWidget.HeightMeasuredTooSmall)
                    {
                        mOnMeasureWidthMeasureSpec = widthMeasureSpec;
                        mOnMeasureHeightMeasureSpec = heightMeasureSpec;
                        resolveMeasuredDimension(widthMeasureSpec, heightMeasureSpec, mLayoutWidget.Width, mLayoutWidget.Height, mLayoutWidget.WidthMeasuredTooSmall, mLayoutWidget.HeightMeasuredTooSmall);
                        return;
                    }
                }
            }
            mOnMeasureWidthMeasureSpec = widthMeasureSpec;
            mOnMeasureHeightMeasureSpec = heightMeasureSpec;

            if (DEBUG)
            {
                Console.WriteLine("### ON MEASURE " + mDirtyHierarchy + " of " + mLayoutWidget.DebugName + " onMeasure width: " + MeasureSpec.ToString(widthMeasureSpec) + " height: " + MeasureSpec.ToString(heightMeasureSpec) + this);
            }

            mLayoutWidget.Rtl = Rtl;

            if (mDirtyHierarchy)
            {
                mDirtyHierarchy = false;
                if (updateHierarchy())
                {
                    mLayoutWidget.updateHierarchy();
                }
            }

            resolveSystem(mLayoutWidget, mOptimizationLevel, widthMeasureSpec, heightMeasureSpec);
            resolveMeasuredDimension(widthMeasureSpec, heightMeasureSpec, mLayoutWidget.Width, mLayoutWidget.Height, mLayoutWidget.WidthMeasuredTooSmall, mLayoutWidget.HeightMeasuredTooSmall);

            if (DEBUG)
            {
                time = DateTimeHelperClass.CurrentUnixTimeMillis() - time;
                Console.WriteLine(mLayoutWidget.DebugName + " (" + ChildCount + ") DONE onMeasure width: " + MeasureSpec.ToString(widthMeasureSpec) + " height: " + MeasureSpec.ToString(heightMeasureSpec) + " => " + mLastMeasureWidth + " x " + mLastMeasureHeight + " lasted " + time);
            }
        }

        protected internal virtual bool Rtl
        {
            get
            {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                {
                    bool isRtlSupported = (Context.ApplicationInfo.flags & ApplicationInfo.FLAG_SUPPORTS_RTL) != 0;
                    return isRtlSupported && (View.LAYOUT_DIRECTION_RTL == LayoutDirection);
                }
                return false;
            }
        }

        /// <summary>
        /// Compute the padding width, taking in account RTL start/end padding if available and present. </summary>
        /// <returns> padding width </returns>
        private int PaddingWidth
        {
            get
            {
                int widthPadding = Math.Max(0, PaddingLeft) + Math.Max(0, PaddingRight);
                int rtlPadding = 0;
    
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                {
                    rtlPadding = Math.Max(0, PaddingStart) + Math.Max(0, PaddingEnd);
                }
                if (rtlPadding > 0)
                {
                    widthPadding = rtlPadding;
                }
                return widthPadding;
            }
        }

        protected internal virtual void setSelfDimensionBehaviour(ConstraintWidgetContainer layout, int widthMode, int widthSize, int heightMode, int heightSize)
        {

            int heightPadding = mMeasurer.paddingHeight;
            int widthPadding = mMeasurer.paddingWidth;

            ConstraintWidget.DimensionBehaviour widthBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;
            ConstraintWidget.DimensionBehaviour heightBehaviour = ConstraintWidget.DimensionBehaviour.FIXED;

            int desiredWidth = 0;
            int desiredHeight = 0;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childCount = getChildCount();
            int childCount = ChildCount;

            switch (widthMode)
            {
                case MeasureSpec.AT_MOST:
                {
                    widthBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    desiredWidth = widthSize;
                    if (childCount == 0)
                    {
                        desiredWidth = Math.Max(0, mMinWidth);
                    }
                }
                break;
                case MeasureSpec.UNSPECIFIED:
                {
                    widthBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    if (childCount == 0)
                    {
                        desiredWidth = Math.Max(0, mMinWidth);
                    }
                }
                break;
                case MeasureSpec.EXACTLY:
                {
                    desiredWidth = Math.Min(mMaxWidth - widthPadding, widthSize);
                }
            break;
            }
            switch (heightMode)
            {
                case MeasureSpec.AT_MOST:
                {
                    heightBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    desiredHeight = heightSize;
                    if (childCount == 0)
                    {
                        desiredHeight = Math.Max(0, mMinHeight);
                    }
                }
                break;
                case MeasureSpec.UNSPECIFIED:
                {
                    heightBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                    if (childCount == 0)
                    {
                        desiredHeight = Math.Max(0, mMinHeight);
                    }
                }
                break;
                case MeasureSpec.EXACTLY:
                {
                    desiredHeight = Math.Min(mMaxHeight - heightPadding, heightSize);
                }
            break;
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

        /// <summary>
        /// Set the State of the ConstraintLayout, causing it to load a particular ConstraintSet.
        /// For states with variants the variant with matching width and height constraintSet will be chosen
        /// </summary>
        /// <param name="id">           the constraint set state </param>
        /// <param name="screenWidth">  the width of the screen in pixels </param>
        /// <param name="screenHeight"> the height of the screen in pixels </param>
        public virtual void setState(int id, int screenWidth, int screenHeight)
        {
            if (mConstraintLayoutSpec != null)
            {
                mConstraintLayoutSpec.updateConstraints(id, screenWidth, screenHeight);
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override void onLayout(bool changed, int left, int top, int right, int bottom)
        {
            if (DEBUG)
            {
                Console.WriteLine(mLayoutWidget.DebugName + " onLayout changed: " + changed + " left: " + left + " top: " + top + " right: " + right + " bottom: " + bottom + " (" + (right - left) + " x " + (bottom - top) + ")");
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int widgetsCount = getChildCount();
            int widgetsCount = ChildCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isInEditMode = isInEditMode();
            bool isInEditMode = InEditMode;
            for (int i = 0; i < widgetsCount; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.View child = getChildAt(i);
                View child = getChildAt(i);
                LayoutParams @params = (LayoutParams) child.LayoutParams;
                ConstraintWidget widget = @params.widget;

                if (child.Visibility == GONE && !@params.isGuideline && !@params.isHelper && !@params.isVirtualGroup && !isInEditMode)
                {
                    // If we are in edit mode, let's layout the widget so that they are at "the right place"
                    // visually in the editor (as we get our positions from layoutlib)
                    continue;
                }
                if (@params.isInPlaceholder)
                {
                    continue;
                }
                int l = widget.X;
                int t = widget.Y;
                int r = l + widget.Width;
                int b = t + widget.Height;

                if (DEBUG)
                {
                    if (child.Visibility != View.GONE && (child.MeasuredWidth != widget.Width || child.MeasuredHeight != widget.Height))
                    {
                        int deltaX = Math.Abs(child.MeasuredWidth - widget.Width);
                        int deltaY = Math.Abs(child.MeasuredHeight - widget.Height);
                        if (deltaX > 1 || deltaY > 1)
                        {
                            Console.WriteLine("child " + child + " measuredWidth " + child.MeasuredWidth + " vs " + widget.Width + " x measureHeight " + child.MeasuredHeight + " vs " + widget.Height);
                        }
                    }
                }

                child.layout(l, t, r, b);
                if (child is Placeholder)
                {
                    Placeholder holder = (Placeholder) child;
                    View content = holder.Content;
                    if (content != null)
                    {
                        content.Visibility = VISIBLE;
                        content.layout(l, t, r, b);
                    }
                }
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int helperCount = mConstraintHelpers.size();
            int helperCount = mConstraintHelpers.Count;
            if (helperCount > 0)
            {
                for (int i = 0; i < helperCount; i++)
                {
                    ConstraintHelper helper = mConstraintHelpers[i];
                    helper.updatePostLayout(this);
                }
            }
        }

        /// <summary>
        /// Set the optimization for the layout resolution.
        /// <para>
        /// The optimization can be any of the following:
        /// <ul>
        /// <li>Optimizer.OPTIMIZATION_NONE</li>
        /// <li>Optimizer.OPTIMIZATION_STANDARD</li>
        /// <li>a mask composed of specific optimizations</li>
        /// </ul>
        /// The mask can be composed of any combination of the following:
        /// <ul>
        /// <li>Optimizer.OPTIMIZATION_DIRECT  </li>
        /// <li>Optimizer.OPTIMIZATION_BARRIER  </li>
        /// <li>Optimizer.OPTIMIZATION_CHAIN  (experimental) </li>
        /// <li>Optimizer.OPTIMIZATION_DIMENSIONS  (experimental) </li>
        /// </ul>
        /// Note that the current implementation of Optimizer.OPTIMIZATION_STANDARD is as a mask of DIRECT and BARRIER.
        /// </para>
        /// </summary>
        /// <param name="level"> optimization level
        /// @since 1.1 </param>
        public virtual int OptimizationLevel
        {
            set
            {
                mOptimizationLevel = value;
                mLayoutWidget.OptimizationLevel = value;
            }
            get
            {
                return mLayoutWidget.OptimizationLevel;
            }
        }


        /// <summary>
        /// @suppress
        /// </summary>
        public override LayoutParams generateLayoutParams(AttributeSet attrs)
        {
            return new LayoutParams(Context, attrs);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override LayoutParams generateDefaultLayoutParams()
        {
            return new LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override ViewGroup.LayoutParams generateLayoutParams(ViewGroup.LayoutParams p)
        {
            return new LayoutParams(p);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        protected internal override bool checkLayoutParams(ViewGroup.LayoutParams p)
        {
            return p is LayoutParams;
        }

        /// <summary>
        /// Sets a ConstraintSet object to manage constraints. The ConstraintSet overrides LayoutParams of child views.
        /// </summary>
        /// <param name="set"> Layout children using ConstraintSet </param>
        public virtual ConstraintSet ConstraintSet
        {
            set
            {
                mConstraintSet = value;
            }
        }

        /// <param name="id"> the view id </param>
        /// <returns> the child view, can return null
        /// @suppress Return a direct child view by its id if it exists </returns>
        public virtual View getViewById(int id)
        {
            return mChildrenByIds.get(id);
        }

        /// <summary>
        /// @suppress
        /// </summary>
        protected internal override void dispatchDraw(Canvas canvas)
        {
            if (mConstraintHelpers != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int helperCount = mConstraintHelpers.size();
                int helperCount = mConstraintHelpers.Count;
                if (helperCount > 0)
                {
                    for (int i = 0; i < helperCount; i++)
                    {
                        ConstraintHelper helper = mConstraintHelpers[i];
                        helper.updatePreDraw(this);
                    }
                }
            }

            base.dispatchDraw(canvas);

            if (DEBUG || InEditMode)
            {
                float cw = Width;
                float ch = Height;
                float ow = 1080;
                float oh = 1920;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
                    View child = getChildAt(i);
                    if (child.Visibility == GONE)
                    {
                        continue;
                    }
                    object tag = child.Tag;
                    if (tag != null && tag is string)
                    {
                        string coordinates = (string) tag;
                        string[] split = coordinates.Split(",", true);
                        if (split.Length == 4)
                        {
                            int x = int.Parse(split[0]);
                            int y = int.Parse(split[1]);
                            int w = int.Parse(split[2]);
                            int h = int.Parse(split[3]);
                            x = (int)((x / ow) * cw);
                            y = (int)((y / oh) * ch);
                            w = (int)((w / ow) * cw);
                            h = (int)((h / oh) * ch);
                            Paint paint = new Paint();
                            paint.Color = Color.RED;
                            canvas.drawLine(x, y, x + w, y, paint);
                            canvas.drawLine(x + w, y, x + w, y + h, paint);
                            canvas.drawLine(x + w, y + h, x, y + h, paint);
                            canvas.drawLine(x, y + h, x, y, paint);
                            paint.Color = Color.GREEN;
                            canvas.drawLine(x, y, x + w, y + h, paint);
                            canvas.drawLine(x, y + h, x + w, y, paint);
                        }
                    }
                }
            }
            if (DEBUG_DRAW_CONSTRAINTS)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
                    View child = getChildAt(i);
                    if (child.Visibility == GONE)
                    {
                        continue;
                    }
                    ConstraintWidget widget = getViewWidget(child);
                    if (widget.mTop.Connected)
                    {
                        ConstraintWidget target = widget.mTop.mTarget.mOwner;
                        int x1 = widget.X + widget.Width / 2;
                        int y1 = widget.Y;
                        int x2 = target.X + target.Width / 2;
                        int y2 = 0;
                        if (widget.mTop.mTarget.mType == ConstraintAnchor.Type.TOP)
                        {
                            y2 = target.Y;
                        }
                        else
                        {
                            y2 = target.Y + target.Height;
                        }
                        Paint paint = new Paint();
                        paint.Color = Color.RED;
                        paint.StrokeWidth = 4;
                        canvas.drawLine(x1, y1, x2, y2, paint);
                    }
                    if (widget.mBottom.Connected)
                    {
                        ConstraintWidget target = widget.mBottom.mTarget.mOwner;
                        int x1 = widget.X + widget.Width / 2;
                        int y1 = widget.Y + widget.Height;
                        int x2 = target.X + target.Width / 2;
                        int y2 = 0;
                        if (widget.mBottom.mTarget.mType == ConstraintAnchor.Type.TOP)
                        {
                            y2 = target.Y;
                        }
                        else
                        {
                            y2 = target.Y + target.Height;
                        }
                        Paint paint = new Paint();
                        paint.StrokeWidth = 4;
                        paint.Color = Color.RED;
                        canvas.drawLine(x1, y1, x2, y2, paint);
                    }
                }
            }
        }

        public virtual ConstraintsChangedListener OnConstraintsChanged
        {
            set
            {
                this.mConstraintsChangedListener = value;
                if (mConstraintLayoutSpec != null)
                {
                    mConstraintLayoutSpec.OnConstraintsChanged = value;
                }
            }
        }

        /// <summary>
        /// Load a layout description file from the resources.
        /// </summary>
        /// <param name="layoutDescription"> The resource id, or 0 to reset the layout description. </param>
        public virtual void loadLayoutDescription(int layoutDescription)
        {
            if (layoutDescription != 0)
            {
                try
                {
                    mConstraintLayoutSpec = new ConstraintLayoutStates(Context, this, layoutDescription);
                }
                catch (Resources.NotFoundException)
                {
                    mConstraintLayoutSpec = null;
                }
            }
            else
            {
                mConstraintLayoutSpec = null;
            }
        }

        /// <summary>
        /// This class contains the different attributes specifying how a view want to be laid out inside
        /// a <seealso cref="ConstraintLayout"/>. For building up constraints at run time, using <seealso cref="ConstraintSet"/> is recommended.
        /// </summary>
        public class LayoutParams : ViewGroup.MarginLayoutParams
        {
            /// <summary>
            /// Dimension will be controlled by constraints.
            /// </summary>
            public const int MATCH_CONSTRAINT = 0;

            /// <summary>
            /// References the id of the parent.
            /// </summary>
            public const int PARENT_ID = 0;

            /// <summary>
            /// Defines an id that is not set.
            /// </summary>
            public const int UNSET = -1;


            /// <summary>
            /// Defines an id that is not set.
            /// </summary>
            public static readonly int GONE_UNSET = int.MinValue;


            /// <summary>
            /// The horizontal orientation.
            /// </summary>
            public const int HORIZONTAL = ConstraintWidget.HORIZONTAL;

            /// <summary>
            /// The vertical orientation.
            /// </summary>
            public const int VERTICAL = ConstraintWidget.VERTICAL;

            /// <summary>
            /// The left side of a view.
            /// </summary>
            public const int LEFT = 1;

            /// <summary>
            /// The right side of a view.
            /// </summary>
            public const int RIGHT = 2;

            /// <summary>
            /// The top of a view.
            /// </summary>
            public const int TOP = 3;

            /// <summary>
            /// The bottom side of a view.
            /// </summary>
            public const int BOTTOM = 4;

            /// <summary>
            /// The baseline of the text in a view.
            /// </summary>
            public const int BASELINE = 5;

            /// <summary>
            /// The left side of a view in left to right languages.
            /// In right to left languages it corresponds to the right side of the view
            /// </summary>
            public const int START = 6;

            /// <summary>
            /// The right side of a view in right to left languages.
            /// In right to left languages it corresponds to the left side of the view
            /// </summary>
            public const int END = 7;

            /// <summary>
            /// Circle reference from a view.
            /// </summary>
            public const int CIRCLE = 8;

            /// <summary>
            /// Set matchConstraintDefault* default to the wrap content size.
            /// Use to set the matchConstraintDefaultWidth and matchConstraintDefaultHeight
            /// </summary>
            public const int MATCH_CONSTRAINT_WRAP = ConstraintWidget.MATCH_CONSTRAINT_WRAP;

            /// <summary>
            /// Set matchConstraintDefault* spread as much as possible within its constraints.
            /// Use to set the matchConstraintDefaultWidth and matchConstraintDefaultHeight
            /// </summary>
            public const int MATCH_CONSTRAINT_SPREAD = ConstraintWidget.MATCH_CONSTRAINT_SPREAD;

            /// <summary>
            /// Set matchConstraintDefault* percent to be based on a percent of another dimension (by default, the parent)
            /// Use to set the matchConstraintDefaultWidth and matchConstraintDefaultHeight
            /// </summary>
            public const int MATCH_CONSTRAINT_PERCENT = ConstraintWidget.MATCH_CONSTRAINT_PERCENT;

            /// <summary>
            /// Chain spread style
            /// </summary>
            public const int CHAIN_SPREAD = ConstraintWidget.CHAIN_SPREAD;

            /// <summary>
            /// Chain spread inside style
            /// </summary>
            public const int CHAIN_SPREAD_INSIDE = ConstraintWidget.CHAIN_SPREAD_INSIDE;

            /// <summary>
            /// Chain packed style
            /// </summary>
            public const int CHAIN_PACKED = ConstraintWidget.CHAIN_PACKED;

            /// <summary>
            /// The distance of child (guideline) to the top or left edge of its parent.
            /// </summary>
            public int guideBegin = UNSET;

            /// <summary>
            /// The distance of child (guideline) to the bottom or right edge of its parent.
            /// </summary>
            public int guideEnd = UNSET;

            /// <summary>
            /// The ratio of the distance to the parent's sides
            /// </summary>
            public float guidePercent = UNSET;

            /// <summary>
            /// Constrains the left side of a child to the left side of a target child (contains the target child id).
            /// </summary>
            public int leftToLeft = UNSET;

            /// <summary>
            /// Constrains the left side of a child to the right side of a target child (contains the target child id).
            /// </summary>
            public int leftToRight = UNSET;

            /// <summary>
            /// Constrains the right side of a child to the left side of a target child (contains the target child id).
            /// </summary>
            public int rightToLeft = UNSET;

            /// <summary>
            /// Constrains the right side of a child to the right side of a target child (contains the target child id).
            /// </summary>
            public int rightToRight = UNSET;

            /// <summary>
            /// Constrains the top side of a child to the top side of a target child (contains the target child id).
            /// </summary>
            public int topToTop = UNSET;

            /// <summary>
            /// Constrains the top side of a child to the bottom side of a target child (contains the target child id).
            /// </summary>
            public int topToBottom = UNSET;

            /// <summary>
            /// Constrains the bottom side of a child to the top side of a target child (contains the target child id).
            /// </summary>
            public int bottomToTop = UNSET;

            /// <summary>
            /// Constrains the bottom side of a child to the bottom side of a target child (contains the target child id).
            /// </summary>
            public int bottomToBottom = UNSET;

            /// <summary>
            /// Constrains the baseline of a child to the baseline of a target child (contains the target child id).
            /// </summary>
            public int baselineToBaseline = UNSET;

            /// <summary>
            /// Constrains the baseline of a child to the top of a target child (contains the target child id).
            /// </summary>
            public int baselineToTop = UNSET;

            /// <summary>
            /// Constrains the baseline of a child to the bottom of a target child (contains the target child id).
            /// </summary>
            public int baselineToBottom = UNSET;

            /// <summary>
            /// Constrains the center of a child to the center of a target child (contains the target child id).
            /// </summary>
            public int circleConstraint = UNSET;

            /// <summary>
            /// The radius used for a circular constraint
            /// </summary>
            public int circleRadius = 0;

            /// <summary>
            /// The angle used for a circular constraint]
            /// </summary>
            public float circleAngle = 0;

            /// <summary>
            /// Constrains the start side of a child to the end side of a target child (contains the target child id).
            /// </summary>
            public int startToEnd = UNSET;

            /// <summary>
            /// Constrains the start side of a child to the start side of a target child (contains the target child id).
            /// </summary>
            public int startToStart = UNSET;

            /// <summary>
            /// Constrains the end side of a child to the start side of a target child (contains the target child id).
            /// </summary>
            public int endToStart = UNSET;

            /// <summary>
            /// Constrains the end side of a child to the end side of a target child (contains the target child id).
            /// </summary>
            public int endToEnd = UNSET;

            /// <summary>
            /// The left margin to use when the target is gone.
            /// </summary>
            public int goneLeftMargin = GONE_UNSET;

            /// <summary>
            /// The top margin to use when the target is gone.
            /// </summary>
            public int goneTopMargin = GONE_UNSET;

            /// <summary>
            /// The right margin to use when the target is gone
            /// </summary>
            public int goneRightMargin = GONE_UNSET;

            /// <summary>
            /// The bottom margin to use when the target is gone.
            /// </summary>
            public int goneBottomMargin = GONE_UNSET;

            /// <summary>
            /// The start margin to use when the target is gone.
            /// </summary>
            public int goneStartMargin = GONE_UNSET;

            /// <summary>
            /// The end margin to use when the target is gone.
            /// </summary>
            public int goneEndMargin = GONE_UNSET;

            /// <summary>
            /// The baseline margin to use when the target is gone.
            /// </summary>
            public int goneBaselineMargin = GONE_UNSET;

            /// <summary>
            /// The baseline margin.
            /// </summary>
            public int baselineMargin = 0;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Layout margins handling TODO: re-activate in 3.0
            ///////////////////////////////////////////////////////////////////////////////////////////

            /// <summary>
            /// The left margin.
            /// </summary>
            // public int leftMargin = 0;

            /// <summary>
            /// The right margin.
            /// </summary>
            // public int rightMargin = 0;

            // int originalLeftMargin = 0;
            // int originalRightMargin = 0;

            /// <summary>
            /// The top margin.
            /// </summary>
            // public int topMargin = 0;

            /// <summary>
            /// The bottom margin.
            /// </summary>
            // public int bottomMargin = 0;

            /// <summary>
            /// The start margin.
            /// </summary>
            // public int startMargin = UNSET;

            /// <summary>
            /// The end margin.
            /// </summary>
            // public int endMargin = UNSET;

            // boolean isRtl = false;
            // int layoutDirection = ViewCompat.LAYOUT_DIRECTION_LTR;

            internal bool widthSet = true; // need to be set to false when we reactivate this in 3.0
            internal bool heightSet = true; // need to be set to false when we reactivate this in 3.0

            ///////////////////////////////////////////////////////////////////////////////////////////

            /// <summary>
            /// The ratio between two connections when the left and right (or start and end) sides are constrained.
            /// </summary>
            public float horizontalBias = 0.5f;

            /// <summary>
            /// The ratio between two connections when the top and bottom sides are constrained.
            /// </summary>
            public float verticalBias = 0.5f;

            /// <summary>
            /// The ratio information.
            /// </summary>
            public string dimensionRatio = null;

            /// <summary>
            /// The ratio between the width and height of the child.
            /// </summary>
            internal float dimensionRatioValue = 0;

            /// <summary>
            /// The child's side to constrain using dimensRatio.
            /// </summary>
            internal int dimensionRatioSide = VERTICAL;

            /// <summary>
            /// The child's weight that we can use to distribute the available horizontal space
            /// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
            /// </summary>
            public float horizontalWeight = UNSET;

            /// <summary>
            /// The child's weight that we can use to distribute the available vertical space
            /// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
            /// </summary>
            public float verticalWeight = UNSET;

            /// <summary>
            /// If the child is the start of a horizontal chain, this attribute will drive how
            /// the elements of the chain will be positioned. The possible values are:
            /// <ul>
            /// <li><seealso cref="#CHAIN_SPREAD"/> -- the elements will be spread out</li>
            /// <li><seealso cref="#CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
            /// be spread out</li>
            /// <li><seealso cref="#CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
            /// horizontal bias attribute of the child will then affect the positioning of the packed
            /// elements</li>
            /// </ul>
            /// </summary>
            public int horizontalChainStyle = CHAIN_SPREAD;

            /// <summary>
            /// If the child is the start of a vertical chain, this attribute will drive how
            /// the elements of the chain will be positioned. The possible values are:
            /// <ul>
            /// <li><seealso cref="#CHAIN_SPREAD"/> -- the elements will be spread out</li>
            /// <li><seealso cref="#CHAIN_SPREAD_INSIDE"/> -- similar, but the endpoints of the chain will not
            /// be spread out</li>
            /// <li><seealso cref="#CHAIN_PACKED"/> -- the elements of the chain will be packed together. The
            /// vertical bias attribute of the child will then affect the positioning of the packed
            /// elements</li>
            /// </ul>
            /// </summary>
            public int verticalChainStyle = CHAIN_SPREAD;

            /// <summary>
            /// Define how the widget horizontal dimension is handled when set to MATCH_CONSTRAINT
            /// <ul>
            /// <li><seealso cref="#MATCH_CONSTRAINT_SPREAD"/> -- the default. The dimension will expand up to
            /// the constraints, minus margins</li>
            /// <li><seealso cref="#MATCH_CONSTRAINT_WRAP"/> -- DEPRECATED -- use instead WRAP_CONTENT and
            /// constrainedWidth=true<br>
            /// The dimension will be the same as WRAP_CONTENT, unless the size ends
            /// up too large for the constraints; in that case the dimension will expand up to the constraints, minus margins
            /// This attribute may not be applied if the widget is part of a chain in that dimension.</li>
            /// <li><seealso cref="#MATCH_CONSTRAINT_PERCENT"/> -- The dimension will be a percent of another
            /// widget (by default, the parent)</li>
            /// </ul>
            /// </summary>
            public int matchConstraintDefaultWidth = MATCH_CONSTRAINT_SPREAD;

            /// <summary>
            /// Define how the widget vertical dimension is handled when set to MATCH_CONSTRAINT
            /// <ul>
            /// <li><seealso cref="#MATCH_CONSTRAINT_SPREAD"/> -- the default. The dimension will expand up to
            /// the constraints, minus margins</li>
            /// <li><seealso cref="#MATCH_CONSTRAINT_WRAP"/> -- DEPRECATED -- use instead WRAP_CONTENT and
            /// constrainedWidth=true<br>
            /// The dimension will be the same as WRAP_CONTENT, unless the size ends
            /// up too large for the constraints; in that case the dimension will expand up to the constraints, minus margins
            /// This attribute may not be applied if the widget is part of a chain in that dimension.</li>
            /// <li><seealso cref="#MATCH_CONSTRAINT_PERCENT"/> -- The dimension will be a percent of another
            /// widget (by default, the parent)</li>
            /// </ul>
            /// </summary>
            public int matchConstraintDefaultHeight = MATCH_CONSTRAINT_SPREAD;

            /// <summary>
            /// Specify a minimum width size for the widget. It will only apply if the size of the widget
            /// is set to MATCH_CONSTRAINT. Don't apply if the widget is part of a horizontal chain.
            /// </summary>
            public int matchConstraintMinWidth = 0;

            /// <summary>
            /// Specify a minimum height size for the widget. It will only apply if the size of the widget
            /// is set to MATCH_CONSTRAINT. Don't apply if the widget is part of a vertical chain.
            /// </summary>
            public int matchConstraintMinHeight = 0;

            /// <summary>
            /// Specify a maximum width size for the widget. It will only apply if the size of the widget
            /// is set to MATCH_CONSTRAINT. Don't apply if the widget is part of a horizontal chain.
            /// </summary>
            public int matchConstraintMaxWidth = 0;

            /// <summary>
            /// Specify a maximum height size for the widget. It will only apply if the size of the widget
            /// is set to MATCH_CONSTRAINT. Don't apply if the widget is part of a vertical chain.
            /// </summary>
            public int matchConstraintMaxHeight = 0;

            /// <summary>
            /// Specify the percentage when using the match constraint percent mode. From 0 to 1.
            /// </summary>
            public float matchConstraintPercentWidth = 1;

            /// <summary>
            /// Specify the percentage when using the match constraint percent mode. From 0 to 1.
            /// </summary>
            public float matchConstraintPercentHeight = 1;

            /// <summary>
            /// The design time location of the left side of the child.
            /// Used at design time for a horizontally unconstrained child.
            /// </summary>
            public int editorAbsoluteX = UNSET;

            /// <summary>
            /// The design time location of the right side of the child.
            /// Used at design time for a vertically unconstrained child.
            /// </summary>
            public int editorAbsoluteY = UNSET;

            public int orientation = UNSET;

            /// <summary>
            /// Specify if the horizontal dimension is constrained in case both left & right constraints are set
            /// and the widget dimension is not a fixed dimension. By default, if a widget is set to WRAP_CONTENT,
            /// we will treat that dimension as a fixed dimension, meaning the dimension will not change regardless
            /// of constraints. Setting this attribute to true allows the dimension to change in order to respect
            /// constraints.
            /// </summary>
            public bool constrainedWidth = false;

            /// <summary>
            /// Specify if the vertical dimension is constrained in case both top & bottom constraints are set
            /// and the widget dimension is not a fixed dimension. By default, if a widget is set to WRAP_CONTENT,
            /// we will treat that dimension as a fixed dimension, meaning the dimension will not change regardless
            /// of constraints. Setting this attribute to true allows the dimension to change in order to respect
            /// constraints.
            /// </summary>
            public bool constrainedHeight = false;

            /// <summary>
            /// Define a category of view to be used by helpers and motionLayout
            /// </summary>
            public string constraintTag = null;

            public const int WRAP_BEHAVIOR_INCLUDED = ConstraintWidget.WRAP_BEHAVIOR_INCLUDED;
            public const int WRAP_BEHAVIOR_HORIZONTAL_ONLY = ConstraintWidget.WRAP_BEHAVIOR_HORIZONTAL_ONLY;
            public const int WRAP_BEHAVIOR_VERTICAL_ONLY = ConstraintWidget.WRAP_BEHAVIOR_VERTICAL_ONLY;
            public const int WRAP_BEHAVIOR_SKIPPED = ConstraintWidget.WRAP_BEHAVIOR_SKIPPED;

            /// <summary>
            /// Specify how this view is taken in account during the parent's wrap computation
            /// 
            /// Can be either of:
            /// WRAP_BEHAVIOR_INCLUDED the widget is taken in account for the wrap (default)
            /// WRAP_BEHAVIOR_HORIZONTAL_ONLY the widget will be included in the wrap only horizontally
            /// WRAP_BEHAVIOR_VERTICAL_ONLY the widget will be included in the wrap only vertically
            /// WRAP_BEHAVIOR_SKIPPED the widget is not part of the wrap computation
            /// </summary>
            public int wrapBehaviorInParent = WRAP_BEHAVIOR_INCLUDED;

            // Internal use only
            internal bool horizontalDimensionFixed = true;
            internal bool verticalDimensionFixed = true;

            internal bool needsBaseline = false;
            internal bool isGuideline = false;
            internal bool isHelper = false;
            internal bool isInPlaceholder = false;
            internal bool isVirtualGroup = false;

            internal int resolvedLeftToLeft = UNSET;
            internal int resolvedLeftToRight = UNSET;
            internal int resolvedRightToLeft = UNSET;
            internal int resolvedRightToRight = UNSET;
            internal int resolveGoneLeftMargin = GONE_UNSET;
            internal int resolveGoneRightMargin = GONE_UNSET;
            internal float resolvedHorizontalBias = 0.5f;

            internal int resolvedGuideBegin;
            internal int resolvedGuideEnd;
            internal float resolvedGuidePercent;

            internal ConstraintWidget widget = new ConstraintWidget();

            /// <summary>
            /// @suppress
            /// </summary>
            public virtual ConstraintWidget ConstraintWidget
            {
                get
                {
                    return widget;
                }
            }

            /// <param name="text">
            /// @suppress </param>
            public virtual string WidgetDebugName
            {
                set
                {
                    widget.DebugName = value;
                }
            }

            public virtual void reset()
            {
                if (widget != null)
                {
                    widget.reset();
                }
            }

            public bool helped = false;

            /// <summary>
            /// Create a LayoutParams base on an existing layout Params
            /// </summary>
            /// <param name="source"> the Layout Params to be copied </param>
            public LayoutParams(LayoutParams source) : base(source)
            {

                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                // this.layoutDirection = source.layoutDirection;
                // this.isRtl = source.isRtl;
                // this.originalLeftMargin = source.originalLeftMargin;
                // this.originalRightMargin = source.originalRightMargin;
                // this.startMargin = source.startMargin;
                // this.endMargin = source.endMargin;
                // this.leftMargin = source.leftMargin;
                // this.rightMargin = source.rightMargin;
                // this.topMargin = source.topMargin;
                // this.bottomMargin = source.bottomMargin;
                ///////////////////////////////////////////////////////////////////////////////////////////

                this.guideBegin = source.guideBegin;
                this.guideEnd = source.guideEnd;
                this.guidePercent = source.guidePercent;
                this.leftToLeft = source.leftToLeft;
                this.leftToRight = source.leftToRight;
                this.rightToLeft = source.rightToLeft;
                this.rightToRight = source.rightToRight;
                this.topToTop = source.topToTop;
                this.topToBottom = source.topToBottom;
                this.bottomToTop = source.bottomToTop;
                this.bottomToBottom = source.bottomToBottom;
                this.baselineToBaseline = source.baselineToBaseline;
                this.baselineToTop = source.baselineToTop;
                this.baselineToBottom = source.baselineToBottom;
                this.circleConstraint = source.circleConstraint;
                this.circleRadius = source.circleRadius;
                this.circleAngle = source.circleAngle;
                this.startToEnd = source.startToEnd;
                this.startToStart = source.startToStart;
                this.endToStart = source.endToStart;
                this.endToEnd = source.endToEnd;
                this.goneLeftMargin = source.goneLeftMargin;
                this.goneTopMargin = source.goneTopMargin;
                this.goneRightMargin = source.goneRightMargin;
                this.goneBottomMargin = source.goneBottomMargin;
                this.goneStartMargin = source.goneStartMargin;
                this.goneEndMargin = source.goneEndMargin;
                this.goneBaselineMargin = source.goneBaselineMargin;
                this.baselineMargin = source.baselineMargin;
                this.horizontalBias = source.horizontalBias;
                this.verticalBias = source.verticalBias;
                this.dimensionRatio = source.dimensionRatio;
                this.dimensionRatioValue = source.dimensionRatioValue;
                this.dimensionRatioSide = source.dimensionRatioSide;
                this.horizontalWeight = source.horizontalWeight;
                this.verticalWeight = source.verticalWeight;
                this.horizontalChainStyle = source.horizontalChainStyle;
                this.verticalChainStyle = source.verticalChainStyle;
                this.constrainedWidth = source.constrainedWidth;
                this.constrainedHeight = source.constrainedHeight;
                this.matchConstraintDefaultWidth = source.matchConstraintDefaultWidth;
                this.matchConstraintDefaultHeight = source.matchConstraintDefaultHeight;
                this.matchConstraintMinWidth = source.matchConstraintMinWidth;
                this.matchConstraintMaxWidth = source.matchConstraintMaxWidth;
                this.matchConstraintMinHeight = source.matchConstraintMinHeight;
                this.matchConstraintMaxHeight = source.matchConstraintMaxHeight;
                this.matchConstraintPercentWidth = source.matchConstraintPercentWidth;
                this.matchConstraintPercentHeight = source.matchConstraintPercentHeight;
                this.editorAbsoluteX = source.editorAbsoluteX;
                this.editorAbsoluteY = source.editorAbsoluteY;
                this.orientation = source.orientation;
                this.horizontalDimensionFixed = source.horizontalDimensionFixed;
                this.verticalDimensionFixed = source.verticalDimensionFixed;
                this.needsBaseline = source.needsBaseline;
                this.isGuideline = source.isGuideline;
                this.resolvedLeftToLeft = source.resolvedLeftToLeft;
                this.resolvedLeftToRight = source.resolvedLeftToRight;
                this.resolvedRightToLeft = source.resolvedRightToLeft;
                this.resolvedRightToRight = source.resolvedRightToRight;
                this.resolveGoneLeftMargin = source.resolveGoneLeftMargin;
                this.resolveGoneRightMargin = source.resolveGoneRightMargin;
                this.resolvedHorizontalBias = source.resolvedHorizontalBias;
                this.constraintTag = source.constraintTag;
                this.wrapBehaviorInParent = source.wrapBehaviorInParent;
                this.widget = source.widget;
                this.widthSet = source.widthSet;
                this.heightSet = source.heightSet;
            }

            private class Table
            {
                public const int UNUSED = 0;
                public const int ANDROID_ORIENTATION = 1;
                public const int LAYOUT_CONSTRAINT_CIRCLE = 2;
                public const int LAYOUT_CONSTRAINT_CIRCLE_RADIUS = 3;
                public const int LAYOUT_CONSTRAINT_CIRCLE_ANGLE = 4;
                public const int LAYOUT_CONSTRAINT_GUIDE_BEGIN = 5;
                public const int LAYOUT_CONSTRAINT_GUIDE_END = 6;
                public const int LAYOUT_CONSTRAINT_GUIDE_PERCENT = 7;
                public const int LAYOUT_CONSTRAINT_LEFT_TO_LEFT_OF = 8;
                public const int LAYOUT_CONSTRAINT_LEFT_TO_RIGHT_OF = 9;
                public const int LAYOUT_CONSTRAINT_RIGHT_TO_LEFT_OF = 10;
                public const int LAYOUT_CONSTRAINT_RIGHT_TO_RIGHT_OF = 11;
                public const int LAYOUT_CONSTRAINT_TOP_TO_TOP_OF = 12;
                public const int LAYOUT_CONSTRAINT_TOP_TO_BOTTOM_OF = 13;
                public const int LAYOUT_CONSTRAINT_BOTTOM_TO_TOP_OF = 14;
                public const int LAYOUT_CONSTRAINT_BOTTOM_TO_BOTTOM_OF = 15;
                public const int LAYOUT_CONSTRAINT_BASELINE_TO_BASELINE_OF = 16;
                public const int LAYOUT_CONSTRAINT_START_TO_END_OF = 17;
                public const int LAYOUT_CONSTRAINT_START_TO_START_OF = 18;
                public const int LAYOUT_CONSTRAINT_END_TO_START_OF = 19;
                public const int LAYOUT_CONSTRAINT_END_TO_END_OF = 20;
                public const int LAYOUT_GONE_MARGIN_LEFT = 21;
                public const int LAYOUT_GONE_MARGIN_TOP = 22;
                public const int LAYOUT_GONE_MARGIN_RIGHT = 23;
                public const int LAYOUT_GONE_MARGIN_BOTTOM = 24;
                public const int LAYOUT_GONE_MARGIN_START = 25;
                public const int LAYOUT_GONE_MARGIN_END = 26;
                public const int LAYOUT_CONSTRAINED_WIDTH = 27;
                public const int LAYOUT_CONSTRAINED_HEIGHT = 28;
                public const int LAYOUT_CONSTRAINT_HORIZONTAL_BIAS = 29;
                public const int LAYOUT_CONSTRAINT_VERTICAL_BIAS = 30;
                public const int LAYOUT_CONSTRAINT_WIDTH_DEFAULT = 31;
                public const int LAYOUT_CONSTRAINT_HEIGHT_DEFAULT = 32;
                public const int LAYOUT_CONSTRAINT_WIDTH_MIN = 33;
                public const int LAYOUT_CONSTRAINT_WIDTH_MAX = 34;
                public const int LAYOUT_CONSTRAINT_WIDTH_PERCENT = 35;
                public const int LAYOUT_CONSTRAINT_HEIGHT_MIN = 36;
                public const int LAYOUT_CONSTRAINT_HEIGHT_MAX = 37;
                public const int LAYOUT_CONSTRAINT_HEIGHT_PERCENT = 38;
                public const int LAYOUT_CONSTRAINT_LEFT_CREATOR = 39;
                public const int LAYOUT_CONSTRAINT_TOP_CREATOR = 40;
                public const int LAYOUT_CONSTRAINT_RIGHT_CREATOR = 41;
                public const int LAYOUT_CONSTRAINT_BOTTOM_CREATOR = 42;
                public const int LAYOUT_CONSTRAINT_BASELINE_CREATOR = 43;
                public const int LAYOUT_CONSTRAINT_DIMENSION_RATIO = 44;
                public const int LAYOUT_CONSTRAINT_HORIZONTAL_WEIGHT = 45;
                public const int LAYOUT_CONSTRAINT_VERTICAL_WEIGHT = 46;
                public const int LAYOUT_CONSTRAINT_HORIZONTAL_CHAINSTYLE = 47;
                public const int LAYOUT_CONSTRAINT_VERTICAL_CHAINSTYLE = 48;
                public const int LAYOUT_EDITOR_ABSOLUTEX = 49;
                public const int LAYOUT_EDITOR_ABSOLUTEY = 50;
                public const int LAYOUT_CONSTRAINT_TAG = 51;
                public const int LAYOUT_CONSTRAINT_BASELINE_TO_TOP_OF = 52;
                public const int LAYOUT_CONSTRAINT_BASELINE_TO_BOTTOM_OF = 53;
                public const int LAYOUT_MARGIN_BASELINE = 54;
                public const int LAYOUT_GONE_MARGIN_BASELINE = 55;
                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                // public static final int LAYOUT_MARGIN_LEFT = 56;
                // public static final int LAYOUT_MARGIN_RIGHT = 57;
                // public static final int LAYOUT_MARGIN_TOP = 58;
                // public static final int LAYOUT_MARGIN_BOTTOM = 59;
                // public static final int LAYOUT_MARGIN_START = 60;
                // public static final int LAYOUT_MARGIN_END = 61;
                // public static final int LAYOUT_WIDTH = 62;
                // public static final int LAYOUT_HEIGHT = 63;
                ///////////////////////////////////////////////////////////////////////////////////////////
                public const int LAYOUT_CONSTRAINT_WIDTH = 64;
                public const int LAYOUT_CONSTRAINT_HEIGHT = 65;
                public const int LAYOUT_WRAP_BEHAVIOR_IN_PARENT = 66;

                public static readonly SparseIntArray map = new SparseIntArray();

                static Table()
                {
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    // Layout margins handling TODO: re-activate in 3.0
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_width, LAYOUT_WIDTH);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_height, LAYOUT_HEIGHT);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginLeft, LAYOUT_MARGIN_LEFT);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginRight, LAYOUT_MARGIN_RIGHT);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginTop, LAYOUT_MARGIN_TOP);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginBottom, LAYOUT_MARGIN_BOTTOM);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginStart, LAYOUT_MARGIN_START);
                    // map.append(R.styleable.ConstraintLayout_Layout_android_layout_marginEnd, LAYOUT_MARGIN_END);
                    ///////////////////////////////////////////////////////////////////////////////////////////
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth, LAYOUT_CONSTRAINT_WIDTH);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight, LAYOUT_CONSTRAINT_HEIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintLeft_toLeftOf, LAYOUT_CONSTRAINT_LEFT_TO_LEFT_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintLeft_toRightOf, LAYOUT_CONSTRAINT_LEFT_TO_RIGHT_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintRight_toLeftOf, LAYOUT_CONSTRAINT_RIGHT_TO_LEFT_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintRight_toRightOf, LAYOUT_CONSTRAINT_RIGHT_TO_RIGHT_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintTop_toTopOf, LAYOUT_CONSTRAINT_TOP_TO_TOP_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintTop_toBottomOf, LAYOUT_CONSTRAINT_TOP_TO_BOTTOM_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBottom_toTopOf, LAYOUT_CONSTRAINT_BOTTOM_TO_TOP_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBottom_toBottomOf, LAYOUT_CONSTRAINT_BOTTOM_TO_BOTTOM_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBaseline_toBaselineOf, LAYOUT_CONSTRAINT_BASELINE_TO_BASELINE_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBaseline_toTopOf, LAYOUT_CONSTRAINT_BASELINE_TO_TOP_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBaseline_toBottomOf, LAYOUT_CONSTRAINT_BASELINE_TO_BOTTOM_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintCircle, LAYOUT_CONSTRAINT_CIRCLE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintCircleRadius, LAYOUT_CONSTRAINT_CIRCLE_RADIUS);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintCircleAngle, LAYOUT_CONSTRAINT_CIRCLE_ANGLE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_editor_absoluteX, LAYOUT_EDITOR_ABSOLUTEX);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_editor_absoluteY, LAYOUT_EDITOR_ABSOLUTEY);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintGuide_begin, LAYOUT_CONSTRAINT_GUIDE_BEGIN);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintGuide_end, LAYOUT_CONSTRAINT_GUIDE_END);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintGuide_percent, LAYOUT_CONSTRAINT_GUIDE_PERCENT);
                    map.append(R.styleable.ConstraintLayout_Layout_android_orientation, ANDROID_ORIENTATION);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintStart_toEndOf, LAYOUT_CONSTRAINT_START_TO_END_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintStart_toStartOf, LAYOUT_CONSTRAINT_START_TO_START_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintEnd_toStartOf, LAYOUT_CONSTRAINT_END_TO_START_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintEnd_toEndOf, LAYOUT_CONSTRAINT_END_TO_END_OF);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginLeft, LAYOUT_GONE_MARGIN_LEFT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginTop, LAYOUT_GONE_MARGIN_TOP);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginRight, LAYOUT_GONE_MARGIN_RIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginBottom, LAYOUT_GONE_MARGIN_BOTTOM);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginStart, LAYOUT_GONE_MARGIN_START);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginEnd, LAYOUT_GONE_MARGIN_END);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_goneMarginBaseline, LAYOUT_GONE_MARGIN_BASELINE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_marginBaseline, LAYOUT_MARGIN_BASELINE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHorizontal_bias, LAYOUT_CONSTRAINT_HORIZONTAL_BIAS);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintVertical_bias, LAYOUT_CONSTRAINT_VERTICAL_BIAS);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintDimensionRatio, LAYOUT_CONSTRAINT_DIMENSION_RATIO);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHorizontal_weight, LAYOUT_CONSTRAINT_HORIZONTAL_WEIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintVertical_weight, LAYOUT_CONSTRAINT_VERTICAL_WEIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHorizontal_chainStyle, LAYOUT_CONSTRAINT_HORIZONTAL_CHAINSTYLE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintVertical_chainStyle, LAYOUT_CONSTRAINT_VERTICAL_CHAINSTYLE);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constrainedWidth, LAYOUT_CONSTRAINED_WIDTH);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constrainedHeight, LAYOUT_CONSTRAINED_HEIGHT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth_default, LAYOUT_CONSTRAINT_WIDTH_DEFAULT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight_default, LAYOUT_CONSTRAINT_HEIGHT_DEFAULT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth_min, LAYOUT_CONSTRAINT_WIDTH_MIN);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth_max, LAYOUT_CONSTRAINT_WIDTH_MAX);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintWidth_percent, LAYOUT_CONSTRAINT_WIDTH_PERCENT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight_min, LAYOUT_CONSTRAINT_HEIGHT_MIN);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight_max, LAYOUT_CONSTRAINT_HEIGHT_MAX);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintHeight_percent, LAYOUT_CONSTRAINT_HEIGHT_PERCENT);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintLeft_creator, LAYOUT_CONSTRAINT_LEFT_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintTop_creator, LAYOUT_CONSTRAINT_TOP_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintRight_creator, LAYOUT_CONSTRAINT_RIGHT_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBottom_creator, LAYOUT_CONSTRAINT_BOTTOM_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintBaseline_creator, LAYOUT_CONSTRAINT_BASELINE_CREATOR);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_constraintTag, LAYOUT_CONSTRAINT_TAG);
                    map.append(R.styleable.ConstraintLayout_Layout_layout_wrapBehaviorInParent, LAYOUT_WRAP_BEHAVIOR_IN_PARENT);
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Layout margins handling TODO: re-activate in 3.0
            ///////////////////////////////////////////////////////////////////////////////////////////
            /*
            public void setMarginStart(int start) {
                startMargin = start;
            }
    
            public void setMarginEnd(int end) {
                endMargin = end;
            }
    
            public int getMarginStart() {
                return startMargin;
            }
    
            public int getMarginEnd() {
                return endMargin;
            }
    
            public int getLayoutDirection() {
                return layoutDirection;
            }
            */
            ///////////////////////////////////////////////////////////////////////////////////////////

            public LayoutParams(Context c, AttributeSet attrs) : base(c, attrs)
            {

                TypedArray a = c.obtainStyledAttributes(attrs, R.styleable.ConstraintLayout_Layout);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
                int N = a.IndexCount;

                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                // super(WRAP_CONTENT, WRAP_CONTENT);
                /*
                if (N == 0) {
                   // check if it's an include
                   throw new IllegalArgumentException("Invalid LayoutParams supplied to " + this);
                }
    
                // let's first apply full margins if they are present.
                int margin = a.getDimensionPixelSize(R.styleable.ConstraintLayout_Layout_android_layout_margin, -1);
                int horizontalMargin = -1;
                int verticalMargin = -1;
                if (margin >= 0) {
                    originalLeftMargin = margin;
                    originalRightMargin = margin;
                    topMargin = margin;
                    bottomMargin = margin;
                } else {
                    horizontalMargin = a.getDimensionPixelSize(R.styleable.ConstraintLayout_Layout_android_layout_marginHorizontal, -1);
                    verticalMargin = a.getDimensionPixelSize(R.styleable.ConstraintLayout_Layout_android_layout_marginVertical, -1);
                    if (horizontalMargin >= 0) {
                        originalLeftMargin = horizontalMargin;
                        originalRightMargin = horizontalMargin;
                    }
                    if (verticalMargin >= 0) {
                        topMargin = verticalMargin;
                        bottomMargin = verticalMargin;
                    }
                }
                */
                ///////////////////////////////////////////////////////////////////////////////////////////

                for (int i = 0; i < N; i++)
                {
                    int attr = a.getIndex(i);
                    int look = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.map.get(attr);
                    switch (look)
                    {
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.UNUSED:
                        {
                            // Skip
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH:
                        {
                            ConstraintSet.parseDimensionConstraints(this, a, attr, HORIZONTAL);
                            widthSet = true;
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT:
                        {
                            ConstraintSet.parseDimensionConstraints(this, a, attr, VERTICAL);
                            heightSet = true;
                            break;
                        }
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        // Layout margins handling TODO: re-activate in 3.0
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        /*
                        case Table.LAYOUT_WIDTH: {
                            width = a.getLayoutDimension(R.styleable.ConstraintLayout_Layout_android_layout_width, "layout_width");
                            widthSet = true;
                            break;
                        }
                        case Table.LAYOUT_HEIGHT: {
                            height = a.getLayoutDimension(R.styleable.ConstraintLayout_Layout_android_layout_height, "layout_height");
                            heightSet = true;
                            break;
                        }
                        */
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_WRAP_BEHAVIOR_IN_PARENT:
                        {
                            wrapBehaviorInParent = a.getInt(attr, wrapBehaviorInParent);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_LEFT_TO_LEFT_OF:
                        {
                            leftToLeft = a.getResourceId(attr, leftToLeft);
                            if (leftToLeft == UNSET)
                            {
                                leftToLeft = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_LEFT_TO_RIGHT_OF:
                        {
                            leftToRight = a.getResourceId(attr, leftToRight);
                            if (leftToRight == UNSET)
                            {
                                leftToRight = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_RIGHT_TO_LEFT_OF:
                        {
                            rightToLeft = a.getResourceId(attr, rightToLeft);
                            if (rightToLeft == UNSET)
                            {
                                rightToLeft = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_RIGHT_TO_RIGHT_OF:
                        {
                            rightToRight = a.getResourceId(attr, rightToRight);
                            if (rightToRight == UNSET)
                            {
                                rightToRight = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_TOP_TO_TOP_OF:
                        {
                            topToTop = a.getResourceId(attr, topToTop);
                            if (topToTop == UNSET)
                            {
                                topToTop = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_TOP_TO_BOTTOM_OF:
                        {
                            topToBottom = a.getResourceId(attr, topToBottom);
                            if (topToBottom == UNSET)
                            {
                                topToBottom = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BOTTOM_TO_TOP_OF:
                        {
                            bottomToTop = a.getResourceId(attr, bottomToTop);
                            if (bottomToTop == UNSET)
                            {
                                bottomToTop = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BOTTOM_TO_BOTTOM_OF:
                        {
                            bottomToBottom = a.getResourceId(attr, bottomToBottom);
                            if (bottomToBottom == UNSET)
                            {
                                bottomToBottom = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BASELINE_TO_BASELINE_OF:
                        {
                            baselineToBaseline = a.getResourceId(attr, baselineToBaseline);
                            if (baselineToBaseline == UNSET)
                            {
                                baselineToBaseline = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BASELINE_TO_TOP_OF:
                        {
                            baselineToTop = a.getResourceId(attr, baselineToTop);
                            if (baselineToTop == UNSET)
                            {
                                baselineToTop = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BASELINE_TO_BOTTOM_OF:
                        {
                            baselineToBottom = a.getResourceId(attr, baselineToBottom);
                            if (baselineToBottom == UNSET)
                            {
                                baselineToBottom = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_CIRCLE:
                        {
                            circleConstraint = a.getResourceId(attr, circleConstraint);
                            if (circleConstraint == UNSET)
                            {
                                circleConstraint = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_CIRCLE_RADIUS:
                        {
                            circleRadius = a.getDimensionPixelSize(attr, circleRadius);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_CIRCLE_ANGLE:
                        {
                            circleAngle = a.getFloat(attr, circleAngle) % 360;
                            if (circleAngle < 0)
                            {
                                circleAngle = (360 - circleAngle) % 360;
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_EDITOR_ABSOLUTEX:
                        {
                            editorAbsoluteX = a.getDimensionPixelOffset(attr, editorAbsoluteX);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_EDITOR_ABSOLUTEY:
                        {
                            editorAbsoluteY = a.getDimensionPixelOffset(attr, editorAbsoluteY);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_GUIDE_BEGIN:
                        {
                            guideBegin = a.getDimensionPixelOffset(attr, guideBegin);
                            break;
                        }

                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_GUIDE_END:
                        {
                            guideEnd = a.getDimensionPixelOffset(attr, guideEnd);
                            break;
                        }

                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_GUIDE_PERCENT:
                        {
                            guidePercent = a.getFloat(attr, guidePercent);
                            break;
                        }

                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.ANDROID_ORIENTATION:
                        {
                            orientation = a.getInt(attr, orientation);
                            break;
                        }

                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_START_TO_END_OF:
                        {
                            startToEnd = a.getResourceId(attr, startToEnd);
                            if (startToEnd == UNSET)
                            {
                                startToEnd = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_START_TO_START_OF:
                        {
                            startToStart = a.getResourceId(attr, startToStart);
                            if (startToStart == UNSET)
                            {
                                startToStart = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_END_TO_START_OF:
                        {
                            endToStart = a.getResourceId(attr, endToStart);
                            if (endToStart == UNSET)
                            {
                                endToStart = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_END_TO_END_OF:
                        {
                            endToEnd = a.getResourceId(attr, endToEnd);
                            if (endToEnd == UNSET)
                            {
                                endToEnd = a.getInt(attr, UNSET);
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_LEFT:
                        {
                            goneLeftMargin = a.getDimensionPixelSize(attr, goneLeftMargin);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_TOP:
                        {
                            goneTopMargin = a.getDimensionPixelSize(attr, goneTopMargin);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_RIGHT:
                        {
                            goneRightMargin = a.getDimensionPixelSize(attr, goneRightMargin);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_BOTTOM:
                        {
                            goneBottomMargin = a.getDimensionPixelSize(attr, goneBottomMargin);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_START:
                        {
                            goneStartMargin = a.getDimensionPixelSize(attr, goneStartMargin);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_END:
                        {
                            goneEndMargin = a.getDimensionPixelSize(attr, goneEndMargin);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_GONE_MARGIN_BASELINE:
                        {
                            goneBaselineMargin = a.getDimensionPixelSize(attr, goneBaselineMargin);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_MARGIN_BASELINE:
                        {
                            baselineMargin = a.getDimensionPixelSize(attr, baselineMargin);
                            break;
                        }
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        // Layout margins handling TODO: re-activate in 3.0
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        /*
                        case Table.LAYOUT_MARGIN_LEFT: {
                            if (margin == -1 && horizontalMargin == -1) {
                                originalLeftMargin = a.getDimensionPixelSize(attr, originalLeftMargin);
                            }
                            break;
                        }
                        case Table.LAYOUT_MARGIN_RIGHT: {
                            if (margin == -1 && horizontalMargin == -1) {
                                originalRightMargin = a.getDimensionPixelSize(attr, originalRightMargin);
                            }
                            break;
                        }
                        case Table.LAYOUT_MARGIN_TOP: {
                            if (margin == -1 && verticalMargin == -1) {
                                topMargin = a.getDimensionPixelSize(attr, topMargin);
                            }
                            break;
                        }
                        case Table.LAYOUT_MARGIN_BOTTOM: {
                            if (margin == -1 && verticalMargin == -1) {
                                bottomMargin = a.getDimensionPixelSize(attr, bottomMargin);
                            }
                            break;
                        }
                        case Table.LAYOUT_MARGIN_START: {
                            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
                                if (margin == -1 && horizontalMargin == -1) {
                                    startMargin = a.getDimensionPixelSize(attr, startMargin);
                                }
                            }
                            break;
                        }
                        case Table.LAYOUT_MARGIN_END: {
                            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
                                if (margin == -1 && horizontalMargin == -1) {
                                    endMargin = a.getDimensionPixelSize(attr, endMargin);
                                }
                            }
                            break;
                        }
                        */
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HORIZONTAL_BIAS:
                        {
                            horizontalBias = a.getFloat(attr, horizontalBias);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_VERTICAL_BIAS:
                        {
                            verticalBias = a.getFloat(attr, verticalBias);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_DIMENSION_RATIO:
                        {
                            ConstraintSet.parseDimensionRatioString(this, a.getString(attr));
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HORIZONTAL_WEIGHT:
                        {
                            horizontalWeight = a.getFloat(attr, horizontalWeight);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_VERTICAL_WEIGHT:
                        {
                            verticalWeight = a.getFloat(attr, verticalWeight);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HORIZONTAL_CHAINSTYLE:
                        {
                            horizontalChainStyle = a.getInt(attr, CHAIN_SPREAD);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_VERTICAL_CHAINSTYLE:
                        {
                            verticalChainStyle = a.getInt(attr, CHAIN_SPREAD);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINED_WIDTH:
                        {
                            constrainedWidth = a.getBoolean(attr, constrainedWidth);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINED_HEIGHT:
                        {
                            constrainedHeight = a.getBoolean(attr, constrainedHeight);
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH_DEFAULT:
                        {
                            matchConstraintDefaultWidth = a.getInt(attr, MATCH_CONSTRAINT_SPREAD);
                            if (matchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
                            {
                                Log.e(TAG, "layout_constraintWidth_default=\"wrap\" is deprecated." + "\nUse layout_width=\"WRAP_CONTENT\" and layout_constrainedWidth=\"true\" instead.");
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT_DEFAULT:
                        {
                            matchConstraintDefaultHeight = a.getInt(attr, MATCH_CONSTRAINT_SPREAD);
                            if (matchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
                            {
                                Log.e(TAG, "layout_constraintHeight_default=\"wrap\" is deprecated." + "\nUse layout_height=\"WRAP_CONTENT\" and layout_constrainedHeight=\"true\" instead.");
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH_MIN:
                        {
                            try
                            {
                                matchConstraintMinWidth = a.getDimensionPixelSize(attr, matchConstraintMinWidth);
                            }
                            catch (Exception)
                            {
                                int value = a.getInt(attr, matchConstraintMinWidth);
                                if (value == WRAP_CONTENT)
                                {
                                    matchConstraintMinWidth = WRAP_CONTENT;
                                }
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH_MAX:
                        {
                            try
                            {
                                matchConstraintMaxWidth = a.getDimensionPixelSize(attr, matchConstraintMaxWidth);
                            }
                            catch (Exception)
                            {
                                int value = a.getInt(attr, matchConstraintMaxWidth);
                                if (value == WRAP_CONTENT)
                                {
                                    matchConstraintMaxWidth = WRAP_CONTENT;
                                }
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_WIDTH_PERCENT:
                        {
                            matchConstraintPercentWidth = Math.Max(0, a.getFloat(attr, matchConstraintPercentWidth));
                            matchConstraintDefaultWidth = MATCH_CONSTRAINT_PERCENT;
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT_MIN:
                        {
                            try
                            {
                                matchConstraintMinHeight = a.getDimensionPixelSize(attr, matchConstraintMinHeight);
                            }
                            catch (Exception)
                            {
                                int value = a.getInt(attr, matchConstraintMinHeight);
                                if (value == WRAP_CONTENT)
                                {
                                    matchConstraintMinHeight = WRAP_CONTENT;
                                }
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT_MAX:
                        {
                            try
                            {
                                matchConstraintMaxHeight = a.getDimensionPixelSize(attr, matchConstraintMaxHeight);
                            }
                            catch (Exception)
                            {
                                int value = a.getInt(attr, matchConstraintMaxHeight);
                                if (value == WRAP_CONTENT)
                                {
                                    matchConstraintMaxHeight = WRAP_CONTENT;
                                }
                            }
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_HEIGHT_PERCENT:
                        {
                            matchConstraintPercentHeight = Math.Max(0, a.getFloat(attr, matchConstraintPercentHeight));
                            matchConstraintDefaultHeight = MATCH_CONSTRAINT_PERCENT;
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_TAG:
                            constraintTag = a.getString(attr);
                            break;
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_LEFT_CREATOR:
                        {
                            // Skip
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_TOP_CREATOR:
                        {
                            // Skip
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_RIGHT_CREATOR:
                        {
                            // Skip
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BOTTOM_CREATOR:
                        {
                            // Skip
                            break;
                        }
                        case androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.Table.LAYOUT_CONSTRAINT_BASELINE_CREATOR:
                        {
                            // Skip
                            break;
                        }
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                /*
                if (Build.VERSION.SDK_INT < Build.VERSION_CODES.JELLY_BEAN_MR1) {
                    leftMargin = originalLeftMargin;
                    rightMargin = originalRightMargin;
                }
                */
                ///////////////////////////////////////////////////////////////////////////////////////////

                a.recycle();
                validate();
            }

            public virtual void validate()
            {
                isGuideline = false;
                horizontalDimensionFixed = true;
                verticalDimensionFixed = true;
                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                /*
                if (dimensionRatio != null && !widthSet && !heightSet) {
                    width = MATCH_CONSTRAINT;
                    height = MATCH_CONSTRAINT;
                }
                */
                ///////////////////////////////////////////////////////////////////////////////////////////

                if (width == WRAP_CONTENT && constrainedWidth)
                {
                    horizontalDimensionFixed = false;
                    if (matchConstraintDefaultWidth == MATCH_CONSTRAINT_SPREAD)
                    {
                        matchConstraintDefaultWidth = MATCH_CONSTRAINT_WRAP;
                    }
                }
                if (height == WRAP_CONTENT && constrainedHeight)
                {
                    verticalDimensionFixed = false;
                    if (matchConstraintDefaultHeight == MATCH_CONSTRAINT_SPREAD)
                    {
                        matchConstraintDefaultHeight = MATCH_CONSTRAINT_WRAP;
                    }
                }
                if (width == MATCH_CONSTRAINT || width == MATCH_PARENT)
                {
                    horizontalDimensionFixed = false;
                    // We have to reset LayoutParams width/height to WRAP_CONTENT here, as some widgets like TextView
                    // will use the layout params directly as a hint to know if they need to request a layout
                    // when their content change (e.g. during setTextView)
                    if (width == MATCH_CONSTRAINT && matchConstraintDefaultWidth == MATCH_CONSTRAINT_WRAP)
                    {
                        width = WRAP_CONTENT;
                        constrainedWidth = true;
                    }
                }
                if (height == MATCH_CONSTRAINT || height == MATCH_PARENT)
                {
                    verticalDimensionFixed = false;
                    // We have to reset LayoutParams width/height to WRAP_CONTENT here, as some widgets like TextView
                    // will use the layout params directly as a hint to know if they need to request a layout
                    // when their content change (e.g. during setTextView)
                    if (height == MATCH_CONSTRAINT && matchConstraintDefaultHeight == MATCH_CONSTRAINT_WRAP)
                    {
                        height = WRAP_CONTENT;
                        constrainedHeight = true;
                    }
                }
                if (guidePercent != UNSET || guideBegin != UNSET || guideEnd != UNSET)
                {
                    isGuideline = true;
                    horizontalDimensionFixed = true;
                    verticalDimensionFixed = true;
                    if (!(widget is Guideline))
                    {
                        widget = new Guideline();
                    }
                    ((Guideline) widget).Orientation = orientation;
                }
            }

            public LayoutParams(int width, int height) : base(width, height)
            {
            }

            public LayoutParams(ViewGroup.LayoutParams source) : base(source)
            {
            }

            /// <summary>
            /// {@inheritDoc}
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @TargetApi(android.os.Build.VERSION_CODES.JELLY_BEAN_MR1) public void resolveLayoutDirection(int layoutDirection)
            public override void resolveLayoutDirection(int layoutDirection)
            {
                ///////////////////////////////////////////////////////////////////////////////////////////
                // Layout margins handling TODO: re-activate in 3.0
                ///////////////////////////////////////////////////////////////////////////////////////////
                /*
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1) {
                    this.layoutDirection = layoutDirection;
                    isRtl = (View.LAYOUT_DIRECTION_RTL == layoutDirection);
                }
    
                // First apply margins.
                leftMargin = originalLeftMargin;
                rightMargin = originalRightMargin;
    
                if (isRtl) {
                    leftMargin = originalRightMargin;
                    rightMargin = originalLeftMargin;
                    if (startMargin != UNSET) {
                        rightMargin = startMargin;
                    }
                    if (endMargin != UNSET) {
                        leftMargin = endMargin;
                    }
                } else {
                    if (startMargin != UNSET) {
                        leftMargin = startMargin;
                    }
                    if (endMargin != UNSET) {
                        rightMargin = endMargin;
                    }
                }
                */
                ///////////////////////////////////////////////////////////////////////////////////////////
                int originalLeftMargin = leftMargin;
                int originalRightMargin = rightMargin;

                bool isRtl = false;
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
                {
                    base.resolveLayoutDirection(layoutDirection);
                    isRtl = (View.LAYOUT_DIRECTION_RTL == LayoutDirection);
                }
                ///////////////////////////////////////////////////////////////////////////////////////////

                resolvedRightToLeft = UNSET;
                resolvedRightToRight = UNSET;
                resolvedLeftToLeft = UNSET;
                resolvedLeftToRight = UNSET;

                resolveGoneLeftMargin = UNSET;
                resolveGoneRightMargin = UNSET;
                resolveGoneLeftMargin = goneLeftMargin;
                resolveGoneRightMargin = goneRightMargin;
                resolvedHorizontalBias = horizontalBias;

                resolvedGuideBegin = guideBegin;
                resolvedGuideEnd = guideEnd;
                resolvedGuidePercent = guidePercent;

                // Post JB MR1, if start/end are defined, they take precedence over left/right
                if (isRtl)
                {
                    bool startEndDefined = false;
                    if (startToEnd != UNSET)
                    {
                        resolvedRightToLeft = startToEnd;
                        startEndDefined = true;
                    }
                    else if (startToStart != UNSET)
                    {
                        resolvedRightToRight = startToStart;
                        startEndDefined = true;
                    }
                    if (endToStart != UNSET)
                    {
                        resolvedLeftToRight = endToStart;
                        startEndDefined = true;
                    }
                    if (endToEnd != UNSET)
                    {
                        resolvedLeftToLeft = endToEnd;
                        startEndDefined = true;
                    }
                    if (goneStartMargin != GONE_UNSET)
                    {
                        resolveGoneRightMargin = goneStartMargin;
                    }
                    if (goneEndMargin != GONE_UNSET)
                    {
                        resolveGoneLeftMargin = goneEndMargin;
                    }
                    if (startEndDefined)
                    {
                        resolvedHorizontalBias = 1 - horizontalBias;
                    }

                    // Only apply to vertical guidelines
                    if (isGuideline && orientation == Guideline.VERTICAL)
                    {
                        if (guidePercent != UNSET)
                        {
                            resolvedGuidePercent = 1 - guidePercent;
                            resolvedGuideBegin = UNSET;
                            resolvedGuideEnd = UNSET;
                        }
                        else if (guideBegin != UNSET)
                        {
                            resolvedGuideEnd = guideBegin;
                            resolvedGuideBegin = UNSET;
                            resolvedGuidePercent = UNSET;
                        }
                        else if (guideEnd != UNSET)
                        {
                            resolvedGuideBegin = guideEnd;
                            resolvedGuideEnd = UNSET;
                            resolvedGuidePercent = UNSET;
                        }
                    }
                }
                else
                {
                    if (startToEnd != UNSET)
                    {
                        resolvedLeftToRight = startToEnd;
                    }
                    if (startToStart != UNSET)
                    {
                        resolvedLeftToLeft = startToStart;
                    }
                    if (endToStart != UNSET)
                    {
                        resolvedRightToLeft = endToStart;
                    }
                    if (endToEnd != UNSET)
                    {
                        resolvedRightToRight = endToEnd;
                    }
                    if (goneStartMargin != GONE_UNSET)
                    {
                        resolveGoneLeftMargin = goneStartMargin;
                    }
                    if (goneEndMargin != GONE_UNSET)
                    {
                        resolveGoneRightMargin = goneEndMargin;
                    }
                }
                // if no constraint is defined via RTL attributes, use left/right if present
                if (endToStart == UNSET && endToEnd == UNSET && startToStart == UNSET && startToEnd == UNSET)
                {
                    if (rightToLeft != UNSET)
                    {
                        resolvedRightToLeft = rightToLeft;
                        if (rightMargin <= 0 && originalRightMargin > 0)
                        {
                            rightMargin = originalRightMargin;
                        }
                    }
                    else if (rightToRight != UNSET)
                    {
                        resolvedRightToRight = rightToRight;
                        if (rightMargin <= 0 && originalRightMargin > 0)
                        {
                            rightMargin = originalRightMargin;
                        }
                    }
                    if (leftToLeft != UNSET)
                    {
                        resolvedLeftToLeft = leftToLeft;
                        if (leftMargin <= 0 && originalLeftMargin > 0)
                        {
                            leftMargin = originalLeftMargin;
                        }
                    }
                    else if (leftToRight != UNSET)
                    {
                        resolvedLeftToRight = leftToRight;
                        if (leftMargin <= 0 && originalLeftMargin > 0)
                        {
                            leftMargin = originalLeftMargin;
                        }
                    }
                }
            }

            /// <summary>
            /// Tag that can be used to identify a view as being a member of a group.
            /// Which can be used for Helpers or in MotionLayout
            /// </summary>
            /// <returns> tag string or null if not defined </returns>
            public virtual string ConstraintTag
            {
                get
                {
                    return constraintTag;
                }
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        public override void requestLayout()
        {
            markHierarchyDirty();
            base.requestLayout();
        }

        public override void forceLayout()
        {
            markHierarchyDirty();
            base.forceLayout();
        }

        private void markHierarchyDirty()
        {
            mDirtyHierarchy = true;
            // reset measured cache
            mLastMeasureWidth = -1;
            mLastMeasureHeight = -1;
            mLastMeasureWidthSize = -1;
            mLastMeasureHeightSize = -1;
            mLastMeasureWidthMode = MeasureSpec.UNSPECIFIED;
            mLastMeasureHeightMode = MeasureSpec.UNSPECIFIED;
        }

        /// <summary>
        /// @suppress
        /// 
        /// @return
        /// </summary>
        public override bool shouldDelayChildPressedState()
        {
            return false;
        }
    }

}