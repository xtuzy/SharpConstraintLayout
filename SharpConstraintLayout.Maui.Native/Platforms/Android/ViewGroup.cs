using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (C) 2006 The Android Open Source Project
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

namespace android.view
{
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static android.os.Build.VERSION_CODES.JELLY_BEAN_MR1;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static android.view.WindowInsetsAnimation.Callback.DISPATCH_MODE_CONTINUE_ON_SUBTREE;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static android.view.WindowInsetsAnimation.Callback.DISPATCH_MODE_STOP;

    using LayoutTransition = android.animation.LayoutTransition;
    using CallSuper = android.annotation.CallSuper;
    using IdRes = android.annotation.IdRes;
    using NonNull = android.annotation.NonNull;
    using Nullable = android.annotation.Nullable;
    using TestApi = android.annotation.TestApi;
    using UiThread = android.annotation.UiThread;
    using UnsupportedAppUsage = android.compat.annotation.UnsupportedAppUsage;
    using ClipData = android.content.ClipData;
    using Context = android.content.Context;
    using Intent = android.content.Intent;
    using PackageManager = android.content.pm.PackageManager;
    using Configuration = android.content.res.Configuration;
    using TypedArray = android.content.res.TypedArray;
    using Bitmap = android.graphics.Bitmap;
    using Canvas = android.graphics.Canvas;
    using Color = android.graphics.Color;
    using Insets = android.graphics.Insets;
    using Matrix = android.graphics.Matrix;
    using Paint = android.graphics.Paint;
    using Point = android.graphics.Point;
    using PointF = android.graphics.PointF;
    using Rect = android.graphics.Rect;
    using RectF = android.graphics.RectF;
    using Region = android.graphics.Region;
    using Build = android.os.Build;
    using Bundle = android.os.Bundle;
    using Parcelable = android.os.Parcelable;
    using SystemClock = android.os.SystemClock;
    using AttributeSet = android.util.AttributeSet;
    using Log = android.util.Log;
    using Pools = android.util.Pools;
    using SynchronizedPool = android.util.Pools.SynchronizedPool;
    using SparseArray = android.util.SparseArray;
    using SparseBooleanArray = android.util.SparseBooleanArray;
    using Bounds = android.view.WindowInsetsAnimation.Bounds;
    using DispatchMode = android.view.WindowInsetsAnimation.Callback.DispatchMode;
    using AccessibilityEvent = android.view.accessibility.AccessibilityEvent;
    using AccessibilityManager = android.view.accessibility.AccessibilityManager;
    using AccessibilityNodeInfo = android.view.accessibility.AccessibilityNodeInfo;
    using Animation = android.view.animation.Animation;
    using AnimationUtils = android.view.animation.AnimationUtils;
    using LayoutAnimationController = android.view.animation.LayoutAnimationController;
    using Transformation = android.view.animation.Transformation;
    using Helper = android.view.autofill.Helper;
    using InspectableProperty = android.view.inspector.InspectableProperty;
    using EnumEntry = android.view.inspector.InspectableProperty.EnumEntry;

    using R = com.android.@internal.R;


    /// <summary>
    /// <para>
    /// A <code>ViewGroup</code> is a special view that can contain other views
    /// (called children.) The view group is the base class for layouts and views
    /// containers. This class also defines the
    /// <seealso cref="android.view.ViewGroup.LayoutParams"/> class which serves as the base
    /// class for layouts parameters.
    /// </para>
    /// 
    /// <para>
    /// Also see <seealso cref="LayoutParams"/> for layout attributes.
    /// </para>
    /// 
    /// <div class="special reference">
    /// <h3>Developer Guides</h3>
    /// <para>For more information about creating user interface layouts, read the
    /// <a href="{@docRoot}guide/topics/ui/declaring-layout.html">XML Layouts</a> developer
    /// guide.</para></div>
    /// 
    /// <para>Here is a complete implementation of a custom ViewGroup that implements
    /// a simple <seealso cref="android.widget.FrameLayout"/> along with the ability to stack
    /// children in left and right gutters.</para>
    /// 
    /// {@sample development/samples/ApiDemos/src/com/example/android/apis/view/CustomLayout.java
    ///      Complete}
    /// 
    /// <para>If you are implementing XML layout attributes as shown in the example, this is the
    /// corresponding definition for them that would go in <code>res/values/attrs.xml</code>:</para>
    /// 
    /// {@sample development/samples/ApiDemos/res/values/attrs.xml CustomLayout}
    /// 
    /// <para>Finally the layout manager can be used in an XML layout like so:</para>
    /// 
    /// {@sample development/samples/ApiDemos/res/layout/custom_layout.xml Complete}
    /// 
    /// @attr ref android.R.styleable#ViewGroup_clipChildren
    /// @attr ref android.R.styleable#ViewGroup_clipToPadding
    /// @attr ref android.R.styleable#ViewGroup_layoutAnimation
    /// @attr ref android.R.styleable#ViewGroup_animationCache
    /// @attr ref android.R.styleable#ViewGroup_persistentDrawingCache
    /// @attr ref android.R.styleable#ViewGroup_alwaysDrawnWithCache
    /// @attr ref android.R.styleable#ViewGroup_addStatesFromChildren
    /// @attr ref android.R.styleable#ViewGroup_descendantFocusability
    /// @attr ref android.R.styleable#ViewGroup_animateLayoutChanges
    /// @attr ref android.R.styleable#ViewGroup_splitMotionEvents
    /// @attr ref android.R.styleable#ViewGroup_layoutMode
    /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UiThread public abstract class ViewGroup extends View implements ViewParent, ViewManager
    public abstract class ViewGroup : View, ViewParent, ViewManager
    {
        private const string TAG = "ViewGroup";

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage private static final boolean DBG = false;
        private const bool DBG = false;

        /// <summary>
        /// Views which have been hidden or removed which need to be animated on
        /// their way out.
        /// This field should be made private, so it is hidden from the SDK.
        /// {@hide}
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage protected java.util.ArrayList<View> mDisappearingChildren;
        protected internal List<View> mDisappearingChildren;

        /// <summary>
        /// Listener used to propagate events indicating when children are added
        /// and/or removed from a view group.
        /// This field should be made private, so it is hidden from the SDK.
        /// {@hide}
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage(maxTargetSdk = android.os.Build.VERSION_CODES.P, trackingBug = 123768704) protected OnHierarchyChangeListener mOnHierarchyChangeListener;
        protected internal OnHierarchyChangeListener mOnHierarchyChangeListener;

        // The view contained within this ViewGroup that has or contains focus.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage(maxTargetSdk = android.os.Build.VERSION_CODES.P, trackingBug = 115609023) private View mFocused;
        private View mFocused;
        // The view contained within this ViewGroup (excluding nested keyboard navigation clusters)
        // that is or contains a default-focus view.
        private View mDefaultFocus;
        // The last child of this ViewGroup which held focus within the current cluster
        internal View mFocusedInCluster;

        /// <summary>
        /// A Transformation used when drawing children, to
        /// apply on the child being drawn.
        /// </summary>
        private Transformation mChildTransformation;

        /// <summary>
        /// Used to track the current invalidation region.
        /// </summary>
        internal RectF mInvalidateRegion;

        /// <summary>
        /// A Transformation used to calculate a correct
        /// invalidation area when the application is autoscaled.
        /// </summary>
        internal Transformation mInvalidationTransformation;

        // Current frontmost child that can accept drag and lies under the drag location.
        // Used only to generate ENTER/EXIT events for pre-Nougat aps.
        private View mCurrentDragChild;

        // Metadata about the ongoing drag
        private DragEvent mCurrentDragStartEvent;
        private bool mIsInterestedInDrag;
        private HashSet<View> mChildrenInterestedInDrag;

        // Used during drag dispatch
        private PointF mLocalPoint;

        // Lazily-created holder for point computations.
        private float[] mTempPosition;

        // Lazily-created holder for point computations.
        private Point mTempPoint;

        // Lazily created Rect for dispatch to children
        private Rect mTempRect;

        // Lazily created int[2] for dispatch to children
        private int[] mTempLocation;

        // Layout animation
        private LayoutAnimationController mLayoutAnimationController;
        private Animation.AnimationListener mAnimationListener;

        // First touch target in the linked list of touch targets.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage private TouchTarget mFirstTouchTarget;
        private TouchTarget mFirstTouchTarget;

        // For debugging only.  You can see these in hierarchyviewer.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"FieldCanBeLocal", "UnusedDeclaration"}) @ViewDebug.ExportedProperty(category = "events") private long mLastTouchDownTime;
        private long mLastTouchDownTime;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "events") private int mLastTouchDownIndex = -1;
        private int mLastTouchDownIndex = -1;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"FieldCanBeLocal", "UnusedDeclaration"}) @ViewDebug.ExportedProperty(category = "events") private float mLastTouchDownX;
        private float mLastTouchDownX;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"FieldCanBeLocal", "UnusedDeclaration"}) @ViewDebug.ExportedProperty(category = "events") private float mLastTouchDownY;
        private float mLastTouchDownY;

        // First hover target in the linked list of hover targets.
        // The hover targets are children which have received ACTION_HOVER_ENTER.
        // They might not have actually handled the hover event, but we will
        // continue sending hover events to them as long as the pointer remains over
        // their bounds and the view group does not intercept hover.
        private HoverTarget mFirstHoverTarget;

        // True if the view group itself received a hover event.
        // It might not have actually handled the hover event.
        private bool mHoveredSelf;

        // The child capable of showing a tooltip and currently under the pointer.
        private View mTooltipHoverTarget;

        // True if the view group is capable of showing a tooltip and the pointer is directly
        // over the view group but not one of its child views.
        private bool mTooltipHoveredSelf;

        /// <summary>
        /// Internal flags.
        /// 
        /// This field should be made private, so it is hidden from the SDK.
        /// {@hide}
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(flagMapping = { @ViewDebug.FlagToString(mask = FLAG_CLIP_CHILDREN, equals = FLAG_CLIP_CHILDREN, name = "CLIP_CHILDREN"), @ViewDebug.FlagToString(mask = FLAG_CLIP_TO_PADDING, equals = FLAG_CLIP_TO_PADDING, name = "CLIP_TO_PADDING"), @ViewDebug.FlagToString(mask = FLAG_PADDING_NOT_NULL, equals = FLAG_PADDING_NOT_NULL, name = "PADDING_NOT_NULL") }, formatToHexString = true) @UnsupportedAppUsage(maxTargetSdk = android.os.Build.VERSION_CODES.P, trackingBug = 123769411) protected int mGroupFlags;
        protected internal int mGroupFlags;

        /// <summary>
        /// Either <seealso cref="#LAYOUT_MODE_CLIP_BOUNDS"/> or <seealso cref="#LAYOUT_MODE_OPTICAL_BOUNDS"/>.
        /// </summary>
        private int mLayoutMode = LAYOUT_MODE_UNDEFINED;

        /// <summary>
        /// NOTE: If you change the flags below make sure to reflect the changes
        ///       the DisplayList class
        /// </summary>

        // When set, ViewGroup invalidates only the child's rectangle
        // Set by default
        internal const int FLAG_CLIP_CHILDREN = 0x1;

        // When set, ViewGroup excludes the padding area from the invalidate rectangle
        // Set by default
        private const int FLAG_CLIP_TO_PADDING = 0x2;

        // When set, dispatchDraw() will invoke invalidate(); this is set by drawChild() when
        // a child needs to be invalidated and FLAG_OPTIMIZE_INVALIDATE is set
        internal const int FLAG_INVALIDATE_REQUIRED = 0x4;

        // When set, dispatchDraw() will run the layout animation and unset the flag
        private const int FLAG_RUN_ANIMATION = 0x8;

        // When set, there is either no layout animation on the ViewGroup or the layout
        // animation is over
        // Set by default
        internal const int FLAG_ANIMATION_DONE = 0x10;

        // If set, this ViewGroup has padding; if unset there is no padding and we don't need
        // to clip it, even if FLAG_CLIP_TO_PADDING is set
        private const int FLAG_PADDING_NOT_NULL = 0x20;

        /// @deprecated - functionality removed 
        [Obsolete("- functionality removed")]
        private const int FLAG_ANIMATION_CACHE = 0x40;

        // When set, this ViewGroup converts calls to invalidate(Rect) to invalidate() during a
        // layout animation; this avoid clobbering the hierarchy
        // Automatically set when the layout animation starts, depending on the animation's
        // characteristics
        internal const int FLAG_OPTIMIZE_INVALIDATE = 0x80;

        // When set, the next call to drawChild() will clear mChildTransformation's matrix
        internal const int FLAG_CLEAR_TRANSFORMATION = 0x100;

        // When set, this ViewGroup invokes mAnimationListener.onAnimationEnd() and removes
        // the children's Bitmap caches if necessary
        // This flag is set when the layout animation is over (after FLAG_ANIMATION_DONE is set)
        private const int FLAG_NOTIFY_ANIMATION_LISTENER = 0x200;

        /// <summary>
        /// When set, the drawing method will call <seealso cref="#getChildDrawingOrder(int, int)"/>
        /// to get the index of the child to draw for that iteration.
        /// 
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage(maxTargetSdk = android.os.Build.VERSION_CODES.P, trackingBug = 123769377) protected static final int FLAG_USE_CHILD_DRAWING_ORDER = 0x400;
        protected internal const int FLAG_USE_CHILD_DRAWING_ORDER = 0x400;

        /// <summary>
        /// When set, this ViewGroup supports static transformations on children; this causes
        /// <seealso cref="#getChildStaticTransformation(View, android.view.animation.Transformation)"/> to be
        /// invoked when a child is drawn.
        /// 
        /// Any subclass overriding
        /// <seealso cref="#getChildStaticTransformation(View, android.view.animation.Transformation)"/> should
        /// set this flags in <seealso cref="#mGroupFlags"/>.
        /// 
        /// {@hide}
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage(maxTargetSdk = android.os.Build.VERSION_CODES.P, trackingBug = 123769647) protected static final int FLAG_SUPPORT_STATIC_TRANSFORMATIONS = 0x800;
        protected internal const int FLAG_SUPPORT_STATIC_TRANSFORMATIONS = 0x800;

        // UNUSED FLAG VALUE: 0x1000;

        /// <summary>
        /// When set, this ViewGroup's drawable states also include those
        /// of its children.
        /// </summary>
        private const int FLAG_ADD_STATES_FROM_CHILDREN = 0x2000;

        /// @deprecated functionality removed 
        [Obsolete("functionality removed")]
        private const int FLAG_ALWAYS_DRAWN_WITH_CACHE = 0x4000;

        /// @deprecated functionality removed 
        [Obsolete("functionality removed")]
        private const int FLAG_CHILDREN_DRAWN_WITH_CACHE = 0x8000;

        /// <summary>
        /// When set, this group will go through its list of children to notify them of
        /// any drawable state change.
        /// </summary>
        private const int FLAG_NOTIFY_CHILDREN_ON_DRAWABLE_STATE_CHANGE = 0x10000;

        private const int FLAG_MASK_FOCUSABILITY = 0x60000;

        /// <summary>
        /// This view will get focus before any of its descendants.
        /// </summary>
        public const int FOCUS_BEFORE_DESCENDANTS = 0x20000;

        /// <summary>
        /// This view will get focus only if none of its descendants want it.
        /// </summary>
        public const int FOCUS_AFTER_DESCENDANTS = 0x40000;

        /// <summary>
        /// This view will block any of its descendants from getting focus, even
        /// if they are focusable.
        /// </summary>
        public const int FOCUS_BLOCK_DESCENDANTS = 0x60000;

        /// <summary>
        /// Used to map between enum in attrubutes and flag values.
        /// </summary>
        private static readonly int[] DESCENDANT_FOCUSABILITY_FLAGS = new int[] {FOCUS_BEFORE_DESCENDANTS, FOCUS_AFTER_DESCENDANTS, FOCUS_BLOCK_DESCENDANTS};

        /// <summary>
        /// When set, this ViewGroup should not intercept touch events.
        /// {@hide}
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage(maxTargetSdk = android.os.Build.VERSION_CODES.P, trackingBug = 123983692) protected static final int FLAG_DISALLOW_INTERCEPT = 0x80000;
        protected internal const int FLAG_DISALLOW_INTERCEPT = 0x80000;

        /// <summary>
        /// When set, this ViewGroup will split MotionEvents to multiple child Views when appropriate.
        /// </summary>
        private const int FLAG_SPLIT_MOTION_EVENTS = 0x200000;

        /// <summary>
        /// When set, this ViewGroup will not dispatch onAttachedToWindow calls
        /// to children when adding new views. This is used to prevent multiple
        /// onAttached calls when a ViewGroup adds children in its own onAttached method.
        /// </summary>
        private const int FLAG_PREVENT_DISPATCH_ATTACHED_TO_WINDOW = 0x400000;

        /// <summary>
        /// When true, indicates that a layoutMode has been explicitly set, either with
        /// an explicit call to <seealso cref="#setLayoutMode(int)"/> in code or from an XML resource.
        /// This distinguishes the situation in which a layout mode was inherited from
        /// one of the ViewGroup's ancestors and cached locally.
        /// </summary>
        private const int FLAG_LAYOUT_MODE_WAS_EXPLICITLY_SET = 0x800000;

        internal const int FLAG_IS_TRANSITION_GROUP = 0x1000000;

        internal const int FLAG_IS_TRANSITION_GROUP_SET = 0x2000000;

        /// <summary>
        /// When set, focus will not be permitted to enter this group if a touchscreen is present.
        /// </summary>
        internal const int FLAG_TOUCHSCREEN_BLOCKS_FOCUS = 0x4000000;

        /// <summary>
        /// When true, indicates that a call to startActionModeForChild was made with the type parameter
        /// and should not be ignored. This helps in backwards compatibility with the existing method
        /// without a type.
        /// </summary>
        /// <seealso cref= #startActionModeForChild(View, android.view.ActionMode.Callback) </seealso>
        /// <seealso cref= #startActionModeForChild(View, android.view.ActionMode.Callback, int) </seealso>
        private const int FLAG_START_ACTION_MODE_FOR_CHILD_IS_TYPED = 0x8000000;

        /// <summary>
        /// When true, indicates that a call to startActionModeForChild was made without the type
        /// parameter. This helps in backwards compatibility with the existing method
        /// without a type.
        /// </summary>
        /// <seealso cref= #startActionModeForChild(View, android.view.ActionMode.Callback) </seealso>
        /// <seealso cref= #startActionModeForChild(View, android.view.ActionMode.Callback, int) </seealso>
        private const int FLAG_START_ACTION_MODE_FOR_CHILD_IS_NOT_TYPED = 0x10000000;

        /// <summary>
        /// When set, indicates that a call to showContextMenuForChild was made with explicit
        /// coordinates within the initiating child view.
        /// </summary>
        private const int FLAG_SHOW_CONTEXT_MENU_WITH_COORDS = 0x20000000;

        /// <summary>
        /// Indicates which types of drawing caches are to be kept in memory.
        /// This field should be made private, so it is hidden from the SDK.
        /// {@hide}
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage protected int mPersistentDrawingCache;
        protected internal int mPersistentDrawingCache;

        /// <summary>
        /// Used to indicate that no drawing cache should be kept in memory.
        /// </summary>
        /// @deprecated The view drawing cache was largely made obsolete with the introduction of
        /// hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
        /// layers are largely unnecessary and can easily result in a net loss in performance due to the
        /// cost of creating and updating the layer. In the rare cases where caching layers are useful,
        /// such as for alpha animations, <seealso cref="#setLayerType(int, Paint)"/> handles this with hardware
        /// rendering. For software-rendered snapshots of a small part of the View hierarchy or
        /// individual Views it is recommended to create a <seealso cref="Canvas"/> from either a <seealso cref="Bitmap"/> or
        /// <seealso cref="android.graphics.Picture"/> and call <seealso cref="#draw(Canvas)"/> on the View. However these
        /// software-rendered usages are discouraged and have compatibility issues with hardware-only
        /// rendering features such as <seealso cref="android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE"/>
        /// bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
        /// reports or unit testing the <seealso cref="PixelCopy"/> API is recommended. 
        [Obsolete("The view drawing cache was largely made obsolete with the introduction of")]
        public const int PERSISTENT_NO_CACHE = 0x0;

        /// <summary>
        /// Used to indicate that the animation drawing cache should be kept in memory.
        /// </summary>
        /// @deprecated The view drawing cache was largely made obsolete with the introduction of
        /// hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
        /// layers are largely unnecessary and can easily result in a net loss in performance due to the
        /// cost of creating and updating the layer. In the rare cases where caching layers are useful,
        /// such as for alpha animations, <seealso cref="#setLayerType(int, Paint)"/> handles this with hardware
        /// rendering. For software-rendered snapshots of a small part of the View hierarchy or
        /// individual Views it is recommended to create a <seealso cref="Canvas"/> from either a <seealso cref="Bitmap"/> or
        /// <seealso cref="android.graphics.Picture"/> and call <seealso cref="#draw(Canvas)"/> on the View. However these
        /// software-rendered usages are discouraged and have compatibility issues with hardware-only
        /// rendering features such as <seealso cref="android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE"/>
        /// bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
        /// reports or unit testing the <seealso cref="PixelCopy"/> API is recommended. 
        [Obsolete("The view drawing cache was largely made obsolete with the introduction of")]
        public const int PERSISTENT_ANIMATION_CACHE = 0x1;

        /// <summary>
        /// Used to indicate that the scrolling drawing cache should be kept in memory.
        /// </summary>
        /// @deprecated The view drawing cache was largely made obsolete with the introduction of
        /// hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
        /// layers are largely unnecessary and can easily result in a net loss in performance due to the
        /// cost of creating and updating the layer. In the rare cases where caching layers are useful,
        /// such as for alpha animations, <seealso cref="#setLayerType(int, Paint)"/> handles this with hardware
        /// rendering. For software-rendered snapshots of a small part of the View hierarchy or
        /// individual Views it is recommended to create a <seealso cref="Canvas"/> from either a <seealso cref="Bitmap"/> or
        /// <seealso cref="android.graphics.Picture"/> and call <seealso cref="#draw(Canvas)"/> on the View. However these
        /// software-rendered usages are discouraged and have compatibility issues with hardware-only
        /// rendering features such as <seealso cref="android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE"/>
        /// bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
        /// reports or unit testing the <seealso cref="PixelCopy"/> API is recommended. 
        [Obsolete("The view drawing cache was largely made obsolete with the introduction of")]
        public const int PERSISTENT_SCROLLING_CACHE = 0x2;

        /// <summary>
        /// Used to indicate that all drawing caches should be kept in memory.
        /// </summary>
        /// @deprecated The view drawing cache was largely made obsolete with the introduction of
        /// hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
        /// layers are largely unnecessary and can easily result in a net loss in performance due to the
        /// cost of creating and updating the layer. In the rare cases where caching layers are useful,
        /// such as for alpha animations, <seealso cref="#setLayerType(int, Paint)"/> handles this with hardware
        /// rendering. For software-rendered snapshots of a small part of the View hierarchy or
        /// individual Views it is recommended to create a <seealso cref="Canvas"/> from either a <seealso cref="Bitmap"/> or
        /// <seealso cref="android.graphics.Picture"/> and call <seealso cref="#draw(Canvas)"/> on the View. However these
        /// software-rendered usages are discouraged and have compatibility issues with hardware-only
        /// rendering features such as <seealso cref="android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE"/>
        /// bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
        /// reports or unit testing the <seealso cref="PixelCopy"/> API is recommended. 
        [Obsolete("The view drawing cache was largely made obsolete with the introduction of")]
        public const int PERSISTENT_ALL_CACHES = 0x3;

        // Layout Modes

        private const int LAYOUT_MODE_UNDEFINED = -1;

        /// <summary>
        /// This constant is a <seealso cref="#setLayoutMode(int) layoutMode"/>.
        /// Clip bounds are the raw values of <seealso cref="#getLeft() left"/>, <seealso cref="#getTop() top"/>,
        /// <seealso cref="#getRight() right"/> and <seealso cref="#getBottom() bottom"/>.
        /// </summary>
        public const int LAYOUT_MODE_CLIP_BOUNDS = 0;

        /// <summary>
        /// This constant is a <seealso cref="#setLayoutMode(int) layoutMode"/>.
        /// Optical bounds describe where a widget appears to be. They sit inside the clip
        /// bounds which need to cover a larger area to allow other effects,
        /// such as shadows and glows, to be drawn.
        /// </summary>
        public const int LAYOUT_MODE_OPTICAL_BOUNDS = 1;

        /// <summary>
        /// @hide </summary>
        public static int LAYOUT_MODE_DEFAULT = LAYOUT_MODE_CLIP_BOUNDS;

        /// <summary>
        /// We clip to padding when FLAG_CLIP_TO_PADDING and FLAG_PADDING_NOT_NULL
        /// are set at the same time.
        /// </summary>
        protected internal const int CLIP_TO_PADDING_MASK = FLAG_CLIP_TO_PADDING | FLAG_PADDING_NOT_NULL;

        // Index of the child's left position in the mLocation array
        private const int CHILD_LEFT_INDEX = 0;
        // Index of the child's top position in the mLocation array
        private const int CHILD_TOP_INDEX = 1;

        // Child views of this ViewGroup
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage(maxTargetSdk = android.os.Build.VERSION_CODES.P) private View[] mChildren;
        private View[] mChildren;
        // Number of valid children in the mChildren array, the rest should be null or not
        // considered as children
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage(maxTargetSdk = android.os.Build.VERSION_CODES.P) private int mChildrenCount;
        private int mChildrenCount;

        // Whether layout calls are currently being suppressed, controlled by calls to
        // suppressLayout()
        internal bool mSuppressLayout = false;

        // Whether any layout calls have actually been suppressed while mSuppressLayout
        // has been true. This tracks whether we need to issue a requestLayout() when
        // layout is later re-enabled.
        private bool mLayoutCalledWhileSuppressed = false;

        private const int ARRAY_INITIAL_CAPACITY = 12;
        private const int ARRAY_CAPACITY_INCREMENT = 12;

        private static float[] sDebugLines;

        // Used to draw cached views
        internal Paint mCachePaint;

        // Used to animate add/remove changes in layout
        private LayoutTransition mTransition;

        // The set of views that are currently being transitioned. This list is used to track views
        // being removed that should not actually be removed from the parent yet because they are
        // being animated.
        private List<View> mTransitioningViews;

        // List of children changing visibility. This is used to potentially keep rendering
        // views during a transition when they otherwise would have become gone/invisible
        private List<View> mVisibilityChangingChildren;

        // Temporary holder of presorted children, only used for
        // input/software draw dispatch for correctly Z ordering.
        private List<View> mPreSortedChildren;

        // Indicates how many of this container's child subtrees contain transient state
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") private int mChildCountWithTransientState = 0;
        private int mChildCountWithTransientState = 0;

        /// <summary>
        /// Currently registered axes for nested scrolling. Flag set consisting of
        /// <seealso cref="#SCROLL_AXIS_HORIZONTAL"/> <seealso cref="#SCROLL_AXIS_VERTICAL"/> or <seealso cref="#SCROLL_AXIS_NONE"/>
        /// for null.
        /// </summary>
        private int mNestedScrollAxes;

        // Used to manage the list of transient views, added by addTransientView()
        private IList<int?> mTransientIndices = null;
        private IList<View> mTransientViews = null;

        /// <summary>
        /// Keeps track of how many child views have UnhandledKeyEventListeners. This should only be
        /// updated on the UI thread so shouldn't require explicit synchronization.
        /// </summary>
        internal int mChildUnhandledKeyListeners = 0;

        /// <summary>
        /// Current dispatch mode of animation events
        /// </summary>
        /// <seealso cref= WindowInsetsAnimation.Callback#getDispatchMode() </seealso>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: private @DispatchMode int mInsetsAnimationDispatchMode = DISPATCH_MODE_CONTINUE_ON_SUBTREE;
        private int mInsetsAnimationDispatchMode = DISPATCH_MODE_CONTINUE_ON_SUBTREE;

        /// <summary>
        /// Empty ActionMode used as a sentinel in recursive entries to startActionModeForChild.
        /// </summary>
        /// <seealso cref= #startActionModeForChild(View, android.view.ActionMode.Callback) </seealso>
        /// <seealso cref= #startActionModeForChild(View, android.view.ActionMode.Callback, int) </seealso>
        private static readonly ActionMode SENTINEL_ACTION_MODE = new ActionModeAnonymousInnerClass();

        private class ActionModeAnonymousInnerClass : ActionMode
        {
            public ActionModeAnonymousInnerClass()
            {
            }

            public override CharSequence Title
            {
                set
                {
                }
                get
                {
                    return null;
                }
            }
            public override int Title
            {
                set
                {
                }
            }
            public override CharSequence Subtitle
            {
                set
                {
                }
                get
                {
                    return null;
                }
            }
            public override int Subtitle
            {
                set
                {
                }
            }
            public override View CustomView
            {
                set
                {
                }
                get
                {
                    return null;
                }
            }
            public override void invalidate()
            {
            }
            public override void finish()
            {
            }
            public override Menu Menu
            {
                get
                {
                    return null;
                }
            }




            public override MenuInflater MenuInflater
            {
                get
                {
                    return null;
                }
            }
        }

        public ViewGroup(Context context) : this(context, null)
        {
        }

        public ViewGroup(Context context, AttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public ViewGroup(Context context, AttributeSet attrs, int defStyleAttr) : this(context, attrs, defStyleAttr, 0)
        {
        }

        public ViewGroup(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {

            initViewGroup();
            initFromAttributes(context, attrs, defStyleAttr, defStyleRes);
        }

        private void initViewGroup()
        {
            // ViewGroup doesn't draw by default
            if (!ShowingLayoutBounds)
            {
                setFlags(WILL_NOT_DRAW, DRAW_MASK);
            }
            mGroupFlags |= FLAG_CLIP_CHILDREN;
            mGroupFlags |= FLAG_CLIP_TO_PADDING;
            mGroupFlags |= FLAG_ANIMATION_DONE;
            mGroupFlags |= FLAG_ANIMATION_CACHE;
            mGroupFlags |= FLAG_ALWAYS_DRAWN_WITH_CACHE;

            if (mContext.ApplicationInfo.targetSdkVersion >= Build.VERSION_CODES.HONEYCOMB)
            {
                mGroupFlags |= FLAG_SPLIT_MOTION_EVENTS;
            }

            DescendantFocusability = FOCUS_BEFORE_DESCENDANTS;

            mChildren = new View[ARRAY_INITIAL_CAPACITY];
            mChildrenCount = 0;

            mPersistentDrawingCache = PERSISTENT_SCROLLING_CACHE;
        }

        private void initFromAttributes(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.content.res.TypedArray a = context.obtainStyledAttributes(attrs, com.android.internal.R.styleable.ViewGroup, defStyleAttr, defStyleRes);
            TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.ViewGroup, defStyleAttr, defStyleRes);
            saveAttributeDataForStyleable(context, R.styleable.ViewGroup, attrs, a, defStyleAttr, defStyleRes);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
            int N = a.IndexCount;
            for (int i = 0; i < N; i++)
            {
                int attr = a.getIndex(i);
                switch (attr)
                {
                    case R.styleable.ViewGroup_clipChildren:
                        ClipChildren = a.getBoolean(attr, true);
                        break;
                    case R.styleable.ViewGroup_clipToPadding:
                        ClipToPadding = a.getBoolean(attr, true);
                        break;
                    case R.styleable.ViewGroup_animationCache:
                        AnimationCacheEnabled = a.getBoolean(attr, true);
                        break;
                    case R.styleable.ViewGroup_persistentDrawingCache:
                        PersistentDrawingCache = a.getInt(attr, PERSISTENT_SCROLLING_CACHE);
                        break;
                    case R.styleable.ViewGroup_addStatesFromChildren:
                        AddStatesFromChildren = a.getBoolean(attr, false);
                        break;
                    case R.styleable.ViewGroup_alwaysDrawnWithCache:
                        AlwaysDrawnWithCacheEnabled = a.getBoolean(attr, true);
                        break;
                    case R.styleable.ViewGroup_layoutAnimation:
                        int id = a.getResourceId(attr, -1);
                        if (id > 0)
                        {
                            LayoutAnimation = AnimationUtils.loadLayoutAnimation(mContext, id);
                        }
                        break;
                    case R.styleable.ViewGroup_descendantFocusability:
                        DescendantFocusability = DESCENDANT_FOCUSABILITY_FLAGS[a.getInt(attr, 0)];
                        break;
                    case R.styleable.ViewGroup_splitMotionEvents:
                        MotionEventSplittingEnabled = a.getBoolean(attr, false);
                        break;
                    case R.styleable.ViewGroup_animateLayoutChanges:
                        bool animateLayoutChanges = a.getBoolean(attr, false);
                        if (animateLayoutChanges)
                        {
                            LayoutTransition = new LayoutTransition();
                        }
                        break;
                    case R.styleable.ViewGroup_layoutMode:
                        LayoutMode = a.getInt(attr, LAYOUT_MODE_UNDEFINED);
                        break;
                    case R.styleable.ViewGroup_transitionGroup:
                        TransitionGroup = a.getBoolean(attr, false);
                        break;
                    case R.styleable.ViewGroup_touchscreenBlocksFocus:
                        TouchscreenBlocksFocus = a.getBoolean(attr, false);
                        break;
                }
            }

            a.recycle();
        }

        /// <summary>
        /// Gets the descendant focusability of this view group.  The descendant
        /// focusability defines the relationship between this view group and its
        /// descendants when looking for a view to take focus in
        /// <seealso cref="#requestFocus(int, android.graphics.Rect)"/>.
        /// </summary>
        /// <returns> one of <seealso cref="#FOCUS_BEFORE_DESCENDANTS"/>, <seealso cref="#FOCUS_AFTER_DESCENDANTS"/>,
        ///   <seealso cref="#FOCUS_BLOCK_DESCENDANTS"/>. </returns>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "focus", mapping = { @ViewDebug.IntToString(from = FOCUS_BEFORE_DESCENDANTS, to = "FOCUS_BEFORE_DESCENDANTS"), @ViewDebug.IntToString(from = FOCUS_AFTER_DESCENDANTS, to = "FOCUS_AFTER_DESCENDANTS"), @ViewDebug.IntToString(from = FOCUS_BLOCK_DESCENDANTS, to = "FOCUS_BLOCK_DESCENDANTS") }) @InspectableProperty(enumMapping = { @EnumEntry(value = FOCUS_BEFORE_DESCENDANTS, name = "beforeDescendants"), @EnumEntry(value = FOCUS_AFTER_DESCENDANTS, name = "afterDescendants"), @EnumEntry(value = FOCUS_BLOCK_DESCENDANTS, name = "blocksDescendants") }) public int getDescendantFocusability()
        public virtual int DescendantFocusability
        {
            get
            {
                return mGroupFlags & FLAG_MASK_FOCUSABILITY;
            }
            set
            {
                switch (value)
                {
                    case FOCUS_BEFORE_DESCENDANTS:
                    case FOCUS_AFTER_DESCENDANTS:
                    case FOCUS_BLOCK_DESCENDANTS:
                        break;
                    default:
                        throw new System.ArgumentException("must be one of FOCUS_BEFORE_DESCENDANTS, " + "FOCUS_AFTER_DESCENDANTS, FOCUS_BLOCK_DESCENDANTS");
                }
                mGroupFlags &= ~FLAG_MASK_FOCUSABILITY;
                mGroupFlags |= (value & FLAG_MASK_FOCUSABILITY);
            }
        }


        internal override void handleFocusGainInternal(int direction, Rect previouslyFocusedRect)
        {
            if (mFocused != null)
            {
                mFocused.unFocus(this);
                mFocused = null;
                mFocusedInCluster = null;
            }
            base.handleFocusGainInternal(direction, previouslyFocusedRect);
        }

        public override void requestChildFocus(View child, View focused)
        {
            if (DBG)
            {
                Console.WriteLine(this + " requestChildFocus()");
            }
            if (DescendantFocusability == FOCUS_BLOCK_DESCENDANTS)
            {
                return;
            }

            // Unfocus us, if necessary
            base.unFocus(focused);

            // We had a previous notion of who had focus. Clear it.
            if (mFocused != child)
            {
                if (mFocused != null)
                {
                    mFocused.unFocus(focused);
                }

                mFocused = child;
            }
            if (mParent != null)
            {
                mParent.requestChildFocus(this, focused);
            }
        }

        internal virtual View DefaultFocus
        {
            set
            {
                // Stop at any higher view which is explicitly focused-by-default
                if (mDefaultFocus != null && mDefaultFocus.FocusedByDefault)
                {
                    return;
                }
    
                mDefaultFocus = value;
    
                if (mParent is ViewGroup)
                {
                    ((ViewGroup) mParent).DefaultFocus = this;
                }
            }
        }

        /// Clears the default-focus chain from {<param name="child">} up to the first parent which has another
        /// default-focusable branch below it or until there is no default-focus chain.
        /// </param>
        /// <param name="child"> </param>
        internal virtual void clearDefaultFocus(View child)
        {
            // Stop at any higher view which is explicitly focused-by-default
            if (mDefaultFocus != child && mDefaultFocus != null && mDefaultFocus.FocusedByDefault)
            {
                return;
            }

            mDefaultFocus = null;

            // Search child siblings for default focusables.
            for (int i = 0; i < mChildrenCount; ++i)
            {
                View sibling = mChildren[i];
                if (sibling.FocusedByDefault)
                {
                    mDefaultFocus = sibling;
                    return;
                }
                else if (mDefaultFocus == null && sibling.hasDefaultFocus())
                {
                    mDefaultFocus = sibling;
                }
            }

            if (mParent is ViewGroup)
            {
                ((ViewGroup) mParent).clearDefaultFocus(this);
            }
        }

        internal override bool hasDefaultFocus()
        {
            return mDefaultFocus != null || base.hasDefaultFocus();
        }

        /// <summary>
        /// Removes {@code child} (and associated focusedInCluster chain) from the cluster containing
        /// it.
        /// <br>
        /// This is intended to be run on {@code child}'s immediate parent. This is necessary because
        /// the chain is sometimes cleared after {@code child} has been detached.
        /// </summary>
        internal virtual void clearFocusedInCluster(View child)
        {
            if (mFocusedInCluster != child)
            {
                return;
            }
            clearFocusedInCluster();
        }

        /// <summary>
        /// Removes the focusedInCluster chain from this up to the cluster containing it.
        /// </summary>
        internal virtual void clearFocusedInCluster()
        {
            View top = findKeyboardNavigationCluster();
            ViewParent parent = this;
            do
            {
                ((ViewGroup) parent).mFocusedInCluster = null;
                if (parent == top)
                {
                    break;
                }
                parent = parent.Parent;
            } while (parent is ViewGroup);
        }

        public override void focusableViewAvailable(View v)
        {
            if (mParent != null && (DescendantFocusability != FOCUS_BLOCK_DESCENDANTS) && ((mViewFlags & VISIBILITY_MASK) == VISIBLE) && (FocusableInTouchMode || !shouldBlockFocusForTouchscreen()) && !(Focused && DescendantFocusability != FOCUS_AFTER_DESCENDANTS))
                    // shortcut: don't report a new focusable view if we block our descendants from
                    // getting focus or if we're not visible
                    // shortcut: don't report a new focusable view if we already are focused
                    // (and we don't prefer our descendants)
                    //
                    // note: knowing that mFocused is non-null is not a good enough reason
                    // to break the traversal since in that case we'd actually have to find
                    // the focused view and make sure it wasn't FOCUS_AFTER_DESCENDANTS and
                    // an ancestor of v; this will get checked for at ViewAncestor
            {
                mParent.focusableViewAvailable(v);
            }
        }

        public override bool showContextMenuForChild(View originalView)
        {
            if (ShowingContextMenuWithCoords)
            {
                // We're being called for compatibility. Return false and let the version
                // with coordinates recurse up.
                return false;
            }
            return mParent != null && mParent.showContextMenuForChild(originalView);
        }

        /// <summary>
        /// @hide used internally for compatibility with existing app code only
        /// </summary>
        public bool ShowingContextMenuWithCoords
        {
            get
            {
                return (mGroupFlags & FLAG_SHOW_CONTEXT_MENU_WITH_COORDS) != 0;
            }
        }

        public override bool showContextMenuForChild(View originalView, float x, float y)
        {
            try
            {
                mGroupFlags |= FLAG_SHOW_CONTEXT_MENU_WITH_COORDS;
                if (showContextMenuForChild(originalView))
                {
                    return true;
                }
            }
            finally
            {
                mGroupFlags &= ~FLAG_SHOW_CONTEXT_MENU_WITH_COORDS;
            }
            return mParent != null && mParent.showContextMenuForChild(originalView, x, y);
        }

        public override ActionMode startActionModeForChild(View originalView, ActionMode.Callback callback)
        {
            if ((mGroupFlags & FLAG_START_ACTION_MODE_FOR_CHILD_IS_TYPED) == 0)
            {
                // This is the original call.
                try
                {
                    mGroupFlags |= FLAG_START_ACTION_MODE_FOR_CHILD_IS_NOT_TYPED;
                    return startActionModeForChild(originalView, callback, ActionMode.TYPE_PRIMARY);
                }
                finally
                {
                    mGroupFlags &= ~FLAG_START_ACTION_MODE_FOR_CHILD_IS_NOT_TYPED;
                }
            }
            else
            {
                // We are being called from the new method with type.
                return SENTINEL_ACTION_MODE;
            }
        }

        public override ActionMode startActionModeForChild(View originalView, ActionMode.Callback callback, int type)
        {
            if ((mGroupFlags & FLAG_START_ACTION_MODE_FOR_CHILD_IS_NOT_TYPED) == 0 && type == ActionMode.TYPE_PRIMARY)
            {
                ActionMode mode;
                try
                {
                    mGroupFlags |= FLAG_START_ACTION_MODE_FOR_CHILD_IS_TYPED;
                    mode = startActionModeForChild(originalView, callback);
                }
                finally
                {
                    mGroupFlags &= ~FLAG_START_ACTION_MODE_FOR_CHILD_IS_TYPED;
                }
                if (mode != SENTINEL_ACTION_MODE)
                {
                    return mode;
                }
            }
            if (mParent != null)
            {
                try
                {
                    return mParent.startActionModeForChild(originalView, callback, type);
                }
                catch (AbstractMethodError)
                {
                    // Custom view parents might not implement this method.
                    return mParent.startActionModeForChild(originalView, callback);
                }
            }
            return null;
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override bool dispatchActivityResult(string who, int requestCode, int resultCode, Intent data)
        {
            if (base.dispatchActivityResult(who, requestCode, resultCode, data))
            {
                return true;
            }
            int childCount = ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View child = getChildAt(i);
                if (child.dispatchActivityResult(who, requestCode, resultCode, data))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find the nearest view in the specified direction that wants to take
        /// focus.
        /// </summary>
        /// <param name="focused"> The view that currently has focus </param>
        /// <param name="direction"> One of FOCUS_UP, FOCUS_DOWN, FOCUS_LEFT, and
        ///        FOCUS_RIGHT, or 0 for not applicable. </param>
        public override View focusSearch(View focused, int direction)
        {
            if (RootNamespace)
            {
                // root namespace means we should consider ourselves the top of the
                // tree for focus searching; otherwise we could be focus searching
                // into other tabs.  see LocalActivityManager and TabHost for more info.
                return FocusFinder.Instance.findNextFocus(this, focused, direction);
            }
            else if (mParent != null)
            {
                return mParent.focusSearch(focused, direction);
            }
            return null;
        }

        public override bool requestChildRectangleOnScreen(View child, Rect rectangle, bool immediate)
        {
            return false;
        }

        public override bool requestSendAccessibilityEvent(View child, AccessibilityEvent @event)
        {
            ViewParent parent = mParent;
            if (parent == null)
            {
                return false;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean propagate = onRequestSendAccessibilityEvent(child, event);
            bool propagate = onRequestSendAccessibilityEvent(child, @event);
            if (!propagate)
            {
                return false;
            }
            return parent.requestSendAccessibilityEvent(this, @event);
        }

        /// <summary>
        /// Called when a child has requested sending an <seealso cref="AccessibilityEvent"/> and
        /// gives an opportunity to its parent to augment the event.
        /// <para>
        /// If an <seealso cref="android.view.View.AccessibilityDelegate"/> has been specified via calling
        /// <seealso cref="android.view.View#setAccessibilityDelegate(android.view.View.AccessibilityDelegate)"/> its
        /// <seealso cref="android.view.View.AccessibilityDelegate#onRequestSendAccessibilityEvent(ViewGroup, View, AccessibilityEvent)"/>
        /// is responsible for handling this call.
        /// </para>
        /// </summary>
        /// <param name="child"> The child which requests sending the event. </param>
        /// <param name="event"> The event to be sent. </param>
        /// <returns> True if the event should be sent.
        /// </returns>
        /// <seealso cref= #requestSendAccessibilityEvent(View, AccessibilityEvent) </seealso>
        public virtual bool onRequestSendAccessibilityEvent(View child, AccessibilityEvent @event)
        {
            if (mAccessibilityDelegate != null)
            {
                return mAccessibilityDelegate.onRequestSendAccessibilityEvent(this, child, @event);
            }
            else
            {
                return onRequestSendAccessibilityEventInternal(child, @event);
            }
        }

        /// <seealso cref= #onRequestSendAccessibilityEvent(View, AccessibilityEvent)
        /// 
        /// Note: Called from the default <seealso cref="View.AccessibilityDelegate"/>.
        /// 
        /// @hide </seealso>
        public virtual bool onRequestSendAccessibilityEventInternal(View child, AccessibilityEvent @event)
        {
            return true;
        }

        /// <summary>
        /// Called when a child view has changed whether or not it is tracking transient state.
        /// </summary>
        public override void childHasTransientStateChanged(View child, bool childHasTransientState)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean oldHasTransientState = hasTransientState();
            bool oldHasTransientState = hasTransientState();
            if (childHasTransientState)
            {
                mChildCountWithTransientState++;
            }
            else
            {
                mChildCountWithTransientState--;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean newHasTransientState = hasTransientState();
            bool newHasTransientState = hasTransientState();
            if (mParent != null && oldHasTransientState != newHasTransientState)
            {
                try
                {
                    mParent.childHasTransientStateChanged(this, newHasTransientState);
                }
                catch (AbstractMethodError e)
                {
                    Log.e(TAG, mParent.GetType().Name + " does not fully implement ViewParent", e);
                }
            }
        }

        public override bool hasTransientState()
        {
            return mChildCountWithTransientState > 0 || base.hasTransientState();
        }

        public override bool dispatchUnhandledMove(View focused, int direction)
        {
            return mFocused != null && mFocused.dispatchUnhandledMove(focused, direction);
        }

        public override void clearChildFocus(View child)
        {
            if (DBG)
            {
                Console.WriteLine(this + " clearChildFocus()");
            }

            mFocused = null;
            if (mParent != null)
            {
                mParent.clearChildFocus(this);
            }
        }

        public override void clearFocus()
        {
            if (DBG)
            {
                Console.WriteLine(this + " clearFocus()");
            }
            if (mFocused == null)
            {
                base.clearFocus();
            }
            else
            {
                View focused = mFocused;
                mFocused = null;
                focused.clearFocus();
            }
        }

        internal override void unFocus(View focused)
        {
            if (DBG)
            {
                Console.WriteLine(this + " unFocus()");
            }
            if (mFocused == null)
            {
                base.unFocus(focused);
            }
            else
            {
                mFocused.unFocus(focused);
                mFocused = null;
            }
        }

        /// <summary>
        /// Returns the focused child of this view, if any. The child may have focus
        /// or contain focus.
        /// </summary>
        /// <returns> the focused child or null. </returns>
        public virtual View FocusedChild
        {
            get
            {
                return mFocused;
            }
        }

        internal virtual View DeepestFocusedChild
        {
            get
            {
                View v = this;
                while (v != null)
                {
                    if (v.Focused)
                    {
                        return v;
                    }
                    v = v is ViewGroup ? ((ViewGroup) v).FocusedChild : null;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns true if this view has or contains focus
        /// </summary>
        /// <returns> true if this view has or contains focus </returns>
        public override bool hasFocus()
        {
            return (mPrivateFlags & PFLAG_FOCUSED) != 0 || mFocused != null;
        }

        /*
         * (non-Javadoc)
         *
         * @see android.view.View#findFocus()
         */
        public override View findFocus()
        {
            if (DBG)
            {
                Console.WriteLine("Find focus in " + this + ": flags=" + Focused + ", child=" + mFocused);
            }

            if (Focused)
            {
                return this;
            }

            if (mFocused != null)
            {
                return mFocused.findFocus();
            }
            return null;
        }

        internal override bool hasFocusable(bool allowAutoFocus, bool dispatchExplicit)
        {
            // This should probably be super.hasFocusable, but that would change
            // behavior. Historically, we have not checked the ancestor views for
            // shouldBlockFocusForTouchscreen() in ViewGroup.hasFocusable.

            // Invisible and gone views are never focusable.
            if ((mViewFlags & VISIBILITY_MASK) != VISIBLE)
            {
                return false;
            }

            // Only use effective focusable value when allowed.
            if ((allowAutoFocus || Focusable != FOCUSABLE_AUTO) && Focusable)
            {
                return true;
            }

            // Determine whether we have a focused descendant.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int descendantFocusability = getDescendantFocusability();
            int descendantFocusability = DescendantFocusability;
            if (descendantFocusability != FOCUS_BLOCK_DESCENDANTS)
            {
                return hasFocusableChild(dispatchExplicit);
            }

            return false;
        }

        internal virtual bool hasFocusableChild(bool dispatchExplicit)
        {
            // Determine whether we have a focusable descendant.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;

            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];

                // In case the subclass has overridden has[Explicit]Focusable, dispatch
                // to the expected one for each child even though we share logic here.
                if ((dispatchExplicit && child.hasExplicitFocusable()) || (!dispatchExplicit && child.hasFocusable()))
                {
                    return true;
                }
            }

            return false;
        }

        public override void addFocusables(List<View> views, int direction, int focusableMode)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int focusableCount = views.size();
            int focusableCount = views.Count;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int descendantFocusability = getDescendantFocusability();
            int descendantFocusability = DescendantFocusability;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean blockFocusForTouchscreen = shouldBlockFocusForTouchscreen();
            bool blockFocusForTouchscreen = shouldBlockFocusForTouchscreen();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean focusSelf = (isFocusableInTouchMode() || !blockFocusForTouchscreen);
            bool focusSelf = (FocusableInTouchMode || !blockFocusForTouchscreen);

            if (descendantFocusability == FOCUS_BLOCK_DESCENDANTS)
            {
                if (focusSelf)
                {
                    base.addFocusables(views, direction, focusableMode);
                }
                return;
            }

            if (blockFocusForTouchscreen)
            {
                focusableMode |= FOCUSABLES_TOUCH_MODE;
            }

            if ((descendantFocusability == FOCUS_BEFORE_DESCENDANTS) && focusSelf)
            {
                base.addFocusables(views, direction, focusableMode);
            }

            int count = 0;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = new View[mChildrenCount];
            View[] children = new View[mChildrenCount];
            for (int i = 0; i < mChildrenCount; ++i)
            {
                View child = mChildren[i];
                if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE)
                {
                    children[count++] = child;
                }
            }
            FocusFinder.sort(children, 0, count, this, LayoutRtl);
            for (int i = 0; i < count; ++i)
            {
                children[i].addFocusables(views, direction, focusableMode);
            }

            // When set to FOCUS_AFTER_DESCENDANTS, we only add ourselves if
            // there aren't any focusable descendants.  this is
            // to avoid the focus search finding layouts when a more precise search
            // among the focusable children would be more interesting.
            if ((descendantFocusability == FOCUS_AFTER_DESCENDANTS) && focusSelf && focusableCount == views.Count)
            {
                base.addFocusables(views, direction, focusableMode);
            }
        }

        public override void addKeyboardNavigationClusters(ICollection<View> views, int direction)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int focusableCount = views.size();
            int focusableCount = views.Count;

            if (KeyboardNavigationCluster)
            {
                // Cluster-navigation can enter a touchscreenBlocksFocus cluster, so temporarily
                // disable touchscreenBlocksFocus to evaluate whether it contains focusables.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean blockedFocus = getTouchscreenBlocksFocus();
                bool blockedFocus = TouchscreenBlocksFocus;
                try
                {
                    TouchscreenBlocksFocusNoRefocus = false;
                    base.addKeyboardNavigationClusters(views, direction);
                }
                finally
                {
                    TouchscreenBlocksFocusNoRefocus = blockedFocus;
                }
            }
            else
            {
                base.addKeyboardNavigationClusters(views, direction);
            }

            if (focusableCount != views.Count)
            {
                // No need to look for groups inside a group.
                return;
            }

            if (DescendantFocusability == FOCUS_BLOCK_DESCENDANTS)
            {
                return;
            }

            int count = 0;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] visibleChildren = new View[mChildrenCount];
            View[] visibleChildren = new View[mChildrenCount];
            for (int i = 0; i < mChildrenCount; ++i)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = mChildren[i];
                View child = mChildren[i];
                if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE)
                {
                    visibleChildren[count++] = child;
                }
            }
            FocusFinder.sort(visibleChildren, 0, count, this, LayoutRtl);
            for (int i = 0; i < count; ++i)
            {
                visibleChildren[i].addKeyboardNavigationClusters(views, direction);
            }
        }

        /// <summary>
        /// Set whether this ViewGroup should ignore focus requests for itself and its children.
        /// If this option is enabled and the ViewGroup or a descendant currently has focus, focus
        /// will proceed forward.
        /// </summary>
        /// <param name="touchscreenBlocksFocus"> true to enable blocking focus in the presence of a touchscreen </param>
        public virtual bool TouchscreenBlocksFocus
        {
            set
            {
                if (value)
                {
                    mGroupFlags |= FLAG_TOUCHSCREEN_BLOCKS_FOCUS;
                    if (hasFocus() && !KeyboardNavigationCluster)
                    {
    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final View focusedChild = getDeepestFocusedChild();
                        View focusedChild = DeepestFocusedChild;
                        if (!focusedChild.FocusableInTouchMode)
                        {
    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final View newFocus = focusSearch(FOCUS_FORWARD);
                            View newFocus = focusSearch(FOCUS_FORWARD);
                            if (newFocus != null)
                            {
                                newFocus.requestFocus();
                            }
                        }
                    }
                }
                else
                {
                    mGroupFlags &= ~FLAG_TOUCHSCREEN_BLOCKS_FOCUS;
                }
            }
            get
            {
                return (mGroupFlags & FLAG_TOUCHSCREEN_BLOCKS_FOCUS) != 0;
            }
        }

        private bool TouchscreenBlocksFocusNoRefocus
        {
            set
            {
                if (value)
                {
                    mGroupFlags |= FLAG_TOUCHSCREEN_BLOCKS_FOCUS;
                }
                else
                {
                    mGroupFlags &= ~FLAG_TOUCHSCREEN_BLOCKS_FOCUS;
                }
            }
        }


        internal virtual bool shouldBlockFocusForTouchscreen()
        {
            // There is a special case for keyboard-navigation clusters. We allow cluster navigation
            // to jump into blockFocusForTouchscreen ViewGroups which are clusters. Once in the
            // cluster, focus is free to move around within it.
            return TouchscreenBlocksFocus && mContext.PackageManager.hasSystemFeature(PackageManager.FEATURE_TOUCHSCREEN) && !(KeyboardNavigationCluster && (hasFocus() || (findKeyboardNavigationCluster() != this)));
        }

        public override void findViewsWithText(List<View> outViews, CharSequence text, int flags)
        {
            base.findViewsWithText(outViews, text, flags);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < childrenCount; i++)
            {
                View child = children[i];
                if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE && (child.mPrivateFlags & PFLAG_IS_ROOT_NAMESPACE) == 0)
                {
                    child.findViewsWithText(outViews, text, flags);
                }
            }
        }

        /// <summary>
        /// @hide </summary>
        public override View findViewByAccessibilityIdTraversal(int accessibilityId)
        {
            View foundView = base.findViewByAccessibilityIdTraversal(accessibilityId);
            if (foundView != null)
            {
                return foundView;
            }

            if (AccessibilityNodeProvider != null)
            {
                return null;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < childrenCount; i++)
            {
                View child = children[i];
                foundView = child.findViewByAccessibilityIdTraversal(accessibilityId);
                if (foundView != null)
                {
                    return foundView;
                }
            }

            return null;
        }

        /// <summary>
        /// @hide </summary>
        public override View findViewByAutofillIdTraversal(int autofillId)
        {
            View foundView = base.findViewByAutofillIdTraversal(autofillId);
            if (foundView != null)
            {
                return foundView;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < childrenCount; i++)
            {
                View child = children[i];
                foundView = child.findViewByAutofillIdTraversal(autofillId);
                if (foundView != null)
                {
                    return foundView;
                }
            }

            return null;
        }

        public override void dispatchWindowFocusChanged(bool hasFocus)
        {
            base.dispatchWindowFocusChanged(hasFocus);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchWindowFocusChanged(hasFocus);
            }
        }

        public override void addTouchables(List<View> views)
        {
            base.addTouchables(views);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;

            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE)
                {
                    child.addTouchables(views);
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @UnsupportedAppUsage public void makeOptionalFitsSystemWindows()
        public override void makeOptionalFitsSystemWindows()
        {
            base.makeOptionalFitsSystemWindows();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].makeOptionalFitsSystemWindows();
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override void makeFrameworkOptionalFitsSystemWindows()
        {
            base.makeFrameworkOptionalFitsSystemWindows();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].makeFrameworkOptionalFitsSystemWindows();
            }
        }

        public override void dispatchDisplayHint(int hint)
        {
            base.dispatchDisplayHint(hint);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchDisplayHint(hint);
            }
        }

        /// <summary>
        /// Called when a view's visibility has changed. Notify the parent to take any appropriate
        /// action.
        /// </summary>
        /// <param name="child"> The view whose visibility has changed </param>
        /// <param name="oldVisibility"> The previous visibility value (GONE, INVISIBLE, or VISIBLE). </param>
        /// <param name="newVisibility"> The new visibility value (GONE, INVISIBLE, or VISIBLE).
        /// @hide </param>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage protected void onChildVisibilityChanged(View child, int oldVisibility, int newVisibility)
        protected internal virtual void onChildVisibilityChanged(View child, int oldVisibility, int newVisibility)
        {
            if (mTransition != null)
            {
                if (newVisibility == VISIBLE)
                {
                    mTransition.showChild(this, child, oldVisibility);
                }
                else
                {
                    mTransition.hideChild(this, child, newVisibility);
                    if (mTransitioningViews != null && mTransitioningViews.Contains(child))
                    {
                        // Only track this on disappearing views - appearing views are already visible
                        // and don't need special handling during drawChild()
                        if (mVisibilityChangingChildren == null)
                        {
                            mVisibilityChangingChildren = new List<View>();
                        }
                        mVisibilityChangingChildren.Add(child);
                        addDisappearingView(child);
                    }
                }
            }

            // in all cases, for drags
            if (newVisibility == VISIBLE && mCurrentDragStartEvent != null)
            {
                if (!mChildrenInterestedInDrag.Contains(child))
                {
                    notifyChildOfDragStart(child);
                }
            }
        }

        protected internal override void dispatchVisibilityChanged(View changedView, int visibility)
        {
            base.dispatchVisibilityChanged(changedView, visibility);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchVisibilityChanged(changedView, visibility);
            }
        }

        public override void dispatchWindowVisibilityChanged(int visibility)
        {
            base.dispatchWindowVisibilityChanged(visibility);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchWindowVisibilityChanged(visibility);
            }
        }

        internal override bool dispatchVisibilityAggregated(bool isVisible)
        {
            isVisible = base.dispatchVisibilityAggregated(isVisible);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                // Only dispatch to visible children. Not visible children and their subtrees already
                // know that they aren't visible and that's not going to change as a result of
                // whatever triggered this dispatch.
                if (children[i].Visibility == VISIBLE)
                {
                    children[i].dispatchVisibilityAggregated(isVisible);
                }
            }
            return isVisible;
        }

        public override void dispatchConfigurationChanged(Configuration newConfig)
        {
            base.dispatchConfigurationChanged(newConfig);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchConfigurationChanged(newConfig);
            }
        }

        public override void recomputeViewAttributes(View child)
        {
            if (mAttachInfo != null && !mAttachInfo.mRecomputeGlobalAttributes)
            {
                ViewParent parent = mParent;
                if (parent != null)
                {
                    parent.recomputeViewAttributes(this);
                }
            }
        }

        internal override void dispatchCollectViewAttributes(AttachInfo attachInfo, int visibility)
        {
            if ((visibility & VISIBILITY_MASK) == VISIBLE)
            {
                base.dispatchCollectViewAttributes(attachInfo, visibility);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
                int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                View[] children = mChildren;
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                    View child = children[i];
                    child.dispatchCollectViewAttributes(attachInfo, visibility | (child.mViewFlags & VISIBILITY_MASK));
                }
            }
        }

        public override void bringChildToFront(View child)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = indexOfChild(child);
            int index = indexOfChild(child);
            if (index >= 0)
            {
                removeFromArray(index);
                addInArray(child, mChildrenCount);
                child.mParent = this;
                requestLayout();
                invalidate();
            }
        }

        private PointF LocalPoint
        {
            get
            {
                if (mLocalPoint == null)
                {
                    mLocalPoint = new PointF();
                }
                return mLocalPoint;
            }
        }

        internal override bool dispatchDragEnterExitInPreN(DragEvent @event)
        {
            if (@event.mAction == DragEvent.ACTION_DRAG_EXITED && mCurrentDragChild != null)
            {
                // The drag exited a sub-tree of views; notify of the exit all descendants that are in
                // entered state.
                // We don't need this recursive delivery for ENTERED events because they get generated
                // from the recursive delivery of LOCATION/DROP events, and hence, don't need their own
                // recursion.
                mCurrentDragChild.dispatchDragEnterExitInPreN(@event);
                mCurrentDragChild = null;
            }
            return mIsInterestedInDrag && base.dispatchDragEnterExitInPreN(@event);
        }

        // TODO: Write real docs
        public override bool dispatchDragEvent(DragEvent @event)
        {
            bool retval = false;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float tx = event.mX;
            float tx = @event.mX;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float ty = event.mY;
            float ty = @event.mY;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.content.ClipData td = event.mClipData;
            ClipData td = @event.mClipData;

            // Dispatch down the view hierarchy
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.graphics.PointF localPoint = getLocalPoint();
            PointF localPoint = LocalPoint;

            switch (@event.mAction)
            {
            case DragEvent.ACTION_DRAG_STARTED:
            {
                // Clear the state to recalculate which views we drag over.
                mCurrentDragChild = null;

                // Set up our tracking of drag-started notifications
                mCurrentDragStartEvent = DragEvent.obtain(@event);
                if (mChildrenInterestedInDrag == null)
                {
                    mChildrenInterestedInDrag = new HashSet<View>();
                }
                else
                {
                    mChildrenInterestedInDrag.Clear();
                }

                // Now dispatch down to our children, caching the responses
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
                int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                View[] children = mChildren;
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                    View child = children[i];
                    child.mPrivateFlags2 &= ~View.DRAG_MASK;
                    if (child.Visibility == VISIBLE)
                    {
                        if (notifyChildOfDragStart(children[i]))
                        {
                            retval = true;
                        }
                    }
                }

                // Notify itself of the drag start.
                mIsInterestedInDrag = base.dispatchDragEvent(@event);
                if (mIsInterestedInDrag)
                {
                    retval = true;
                }

                if (!retval)
                {
                    // Neither us nor any of our children are interested in this drag, so stop tracking
                    // the current drag event.
                    mCurrentDragStartEvent.recycle();
                    mCurrentDragStartEvent = null;
                }
            }
            break;

            case DragEvent.ACTION_DRAG_ENDED:
            {
                // Release the bookkeeping now that the drag lifecycle has ended
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.HashSet<View> childrenInterestedInDrag = mChildrenInterestedInDrag;
                HashSet<View> childrenInterestedInDrag = mChildrenInterestedInDrag;
                if (childrenInterestedInDrag != null)
                {
                    foreach (View child in childrenInterestedInDrag)
                    {
                        // If a child was interested in the ongoing drag, it's told that it's over
                        if (child.dispatchDragEvent(@event))
                        {
                            retval = true;
                        }
                    }
                    childrenInterestedInDrag.Clear();
                }
                if (mCurrentDragStartEvent != null)
                {
                    mCurrentDragStartEvent.recycle();
                    mCurrentDragStartEvent = null;
                }

                if (mIsInterestedInDrag)
                {
                    if (base.dispatchDragEvent(@event))
                    {
                        retval = true;
                    }
                    mIsInterestedInDrag = false;
                }
            }
            break;

            case DragEvent.ACTION_DRAG_LOCATION:
            case DragEvent.ACTION_DROP:
            {
                // Find the [possibly new] drag target
                View target = findFrontmostDroppableChildAt(@event.mX, @event.mY, localPoint);

                if (target != mCurrentDragChild)
                {
                    if (sCascadedDragDrop)
                    {
                        // For pre-Nougat apps, make sure that the whole hierarchy of views that contain
                        // the drag location is kept in the state between ENTERED and EXITED events.
                        // (Starting with N, only the innermost view will be in that state).

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int action = event.mAction;
                        int action = @event.mAction;
                        // Position should not be available for ACTION_DRAG_ENTERED and
                        // ACTION_DRAG_EXITED.
                        @event.mX = 0;
                        @event.mY = 0;
                        @event.mClipData = null;

                        if (mCurrentDragChild != null)
                        {
                            @event.mAction = DragEvent.ACTION_DRAG_EXITED;
                            mCurrentDragChild.dispatchDragEnterExitInPreN(@event);
                        }

                        if (target != null)
                        {
                            @event.mAction = DragEvent.ACTION_DRAG_ENTERED;
                            target.dispatchDragEnterExitInPreN(@event);
                        }

                        @event.mAction = action;
                        @event.mX = tx;
                        @event.mY = ty;
                        @event.mClipData = td;
                    }
                    mCurrentDragChild = target;
                }

                if (target == null && mIsInterestedInDrag)
                {
                    target = this;
                }

                // Dispatch the actual drag notice, localized into the target coordinates.
                if (target != null)
                {
                    if (target != this)
                    {
                        @event.mX = localPoint.x;
                        @event.mY = localPoint.y;

                        retval = target.dispatchDragEvent(@event);

                        @event.mX = tx;
                        @event.mY = ty;

                        if (mIsInterestedInDrag)
                        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean eventWasConsumed;
                            bool eventWasConsumed;
                            if (sCascadedDragDrop)
                            {
                                eventWasConsumed = retval;
                            }
                            else
                            {
                                eventWasConsumed = @event.mEventHandlerWasCalled;
                            }

                            if (!eventWasConsumed)
                            {
                                retval = base.dispatchDragEvent(@event);
                            }
                        }
                    }
                    else
                    {
                        retval = base.dispatchDragEvent(@event);
                    }
                }
            }
            break;
            }

            return retval;
        }

        // Find the frontmost child view that lies under the given point, and calculate
        // the position within its own local coordinate system.
        internal virtual View findFrontmostDroppableChildAt(float x, float y, PointF outLocalPoint)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = count - 1; i >= 0; i--)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                if (!child.canAcceptDrag())
                {
                    continue;
                }

                if (isTransformedTouchPointInView(x, y, child, outLocalPoint))
                {
                    return child;
                }
            }
            return null;
        }

        internal virtual bool notifyChildOfDragStart(View child)
        {
            // The caller guarantees that the child is not in mChildrenInterestedInDrag yet.

            if (ViewDebug.DEBUG_DRAG)
            {
                Log.d(View.VIEW_LOG_TAG, "Sending drag-started to view: " + child);
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float tx = mCurrentDragStartEvent.mX;
            float tx = mCurrentDragStartEvent.mX;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float ty = mCurrentDragStartEvent.mY;
            float ty = mCurrentDragStartEvent.mY;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float[] point = getTempLocationF();
            float[] point = TempLocationF;
            point[0] = tx;
            point[1] = ty;
            transformPointToViewLocal(point, child);

            mCurrentDragStartEvent.mX = point[0];
            mCurrentDragStartEvent.mY = point[1];
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean canAccept = child.dispatchDragEvent(mCurrentDragStartEvent);
            bool canAccept = child.dispatchDragEvent(mCurrentDragStartEvent);
            mCurrentDragStartEvent.mX = tx;
            mCurrentDragStartEvent.mY = ty;
            mCurrentDragStartEvent.mEventHandlerWasCalled = false;
            if (canAccept)
            {
                mChildrenInterestedInDrag.Add(child);
                if (!child.canAcceptDrag())
                {
                    child.mPrivateFlags2 |= View.PFLAG2_DRAG_CAN_ACCEPT;
                    child.refreshDrawableState();
                }
            }
            return canAccept;
        }

        [Obsolete]
        public override void dispatchWindowSystemUiVisiblityChanged(int visible)
        {
            base.dispatchWindowSystemUiVisiblityChanged(visible);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                child.dispatchWindowSystemUiVisiblityChanged(visible);
            }
        }

        [Obsolete]
        public override void dispatchSystemUiVisibilityChanged(int visible)
        {
            base.dispatchSystemUiVisibilityChanged(visible);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                child.dispatchSystemUiVisibilityChanged(visible);
            }
        }

        internal override bool updateLocalSystemUiVisibility(int localValue, int localChanges)
        {
            bool changed = base.updateLocalSystemUiVisibility(localValue, localChanges);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                changed |= child.updateLocalSystemUiVisibility(localValue, localChanges);
            }
            return changed;
        }

        public override bool dispatchKeyEventPreIme(KeyEvent @event)
        {
            if ((mPrivateFlags & (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS)) == (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS))
            {
                return base.dispatchKeyEventPreIme(@event);
            }
            else if (mFocused != null && (mFocused.mPrivateFlags & PFLAG_HAS_BOUNDS) == PFLAG_HAS_BOUNDS)
            {
                return mFocused.dispatchKeyEventPreIme(@event);
            }
            return false;
        }

        public override bool dispatchKeyEvent(KeyEvent @event)
        {
            if (mInputEventConsistencyVerifier != null)
            {
                mInputEventConsistencyVerifier.onKeyEvent(@event, 1);
            }

            if ((mPrivateFlags & (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS)) == (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS))
            {
                if (base.dispatchKeyEvent(@event))
                {
                    return true;
                }
            }
            else if (mFocused != null && (mFocused.mPrivateFlags & PFLAG_HAS_BOUNDS) == PFLAG_HAS_BOUNDS)
            {
                if (mFocused.dispatchKeyEvent(@event))
                {
                    return true;
                }
            }

            if (mInputEventConsistencyVerifier != null)
            {
                mInputEventConsistencyVerifier.onUnhandledEvent(@event, 1);
            }
            return false;
        }

        public override bool dispatchKeyShortcutEvent(KeyEvent @event)
        {
            if ((mPrivateFlags & (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS)) == (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS))
            {
                return base.dispatchKeyShortcutEvent(@event);
            }
            else if (mFocused != null && (mFocused.mPrivateFlags & PFLAG_HAS_BOUNDS) == PFLAG_HAS_BOUNDS)
            {
                return mFocused.dispatchKeyShortcutEvent(@event);
            }
            return false;
        }

        public override bool dispatchTrackballEvent(MotionEvent @event)
        {
            if (mInputEventConsistencyVerifier != null)
            {
                mInputEventConsistencyVerifier.onTrackballEvent(@event, 1);
            }

            if ((mPrivateFlags & (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS)) == (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS))
            {
                if (base.dispatchTrackballEvent(@event))
                {
                    return true;
                }
            }
            else if (mFocused != null && (mFocused.mPrivateFlags & PFLAG_HAS_BOUNDS) == PFLAG_HAS_BOUNDS)
            {
                if (mFocused.dispatchTrackballEvent(@event))
                {
                    return true;
                }
            }

            if (mInputEventConsistencyVerifier != null)
            {
                mInputEventConsistencyVerifier.onUnhandledEvent(@event, 1);
            }
            return false;
        }

        public override bool dispatchCapturedPointerEvent(MotionEvent @event)
        {
            if ((mPrivateFlags & (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS)) == (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS))
            {
                if (base.dispatchCapturedPointerEvent(@event))
                {
                    return true;
                }
            }
            else if (mFocused != null && (mFocused.mPrivateFlags & PFLAG_HAS_BOUNDS) == PFLAG_HAS_BOUNDS)
            {
                if (mFocused.dispatchCapturedPointerEvent(@event))
                {
                    return true;
                }
            }
            return false;
        }

        public override void dispatchPointerCaptureChanged(bool hasCapture)
        {
            exitHoverTargets();

            base.dispatchPointerCaptureChanged(hasCapture);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchPointerCaptureChanged(hasCapture);
            }
        }

        public override PointerIcon onResolvePointerIcon(MotionEvent @event, int pointerIndex)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float x = event.getX(pointerIndex);
            float x = @event.getX(pointerIndex);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float y = event.getY(pointerIndex);
            float y = @event.getY(pointerIndex);
            if (isOnScrollbarThumb(x, y) || DraggingScrollBar)
            {
                return PointerIcon.getSystemIcon(mContext, PointerIcon.TYPE_ARROW);
            }
            // Check what the child under the pointer says about the pointer.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
            if (childrenCount != 0)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildOrderedChildList();
                List<View> preorderedList = buildOrderedChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
                bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                View[] children = mChildren;
                for (int i = childrenCount - 1; i >= 0; i--)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                    int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                    View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                    View childWithAccessibilityFocus = @event.TargetAccessibilityFocus ? findChildWithAccessibilityFocus() : null;

                    if (!child.canReceivePointerEvents() || !isTransformedTouchPointInView(x, y, child, null))
                    {

                        // If there is a view that has accessibility focus we want it
                        // to get the event first and if not handled we will perform a
                        // normal dispatch. We may do a double iteration but this is
                        // safer given the timeframe.
                        if (childWithAccessibilityFocus != null)
                        {
                            if (childWithAccessibilityFocus != child)
                            {
                                continue;
                            }
                            childWithAccessibilityFocus = null;
                            i = childrenCount - 1;
                        }
                        @event.TargetAccessibilityFocus = false;
                        continue;
                    }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PointerIcon pointerIcon = dispatchResolvePointerIcon(event, pointerIndex, child);
                    PointerIcon pointerIcon = dispatchResolvePointerIcon(@event, pointerIndex, child);
                    if (pointerIcon != null)
                    {
                        if (preorderedList != null)
                        {
                            preorderedList.Clear();
                        }
                        return pointerIcon;
                    }
                }
                if (preorderedList != null)
                {
                    preorderedList.Clear();
                }
            }

            // The pointer is not a child or the child has no preferences, returning the default
            // implementation.
            return base.onResolvePointerIcon(@event, pointerIndex);
        }

        private PointerIcon dispatchResolvePointerIcon(MotionEvent @event, int pointerIndex, View child)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final PointerIcon pointerIcon;
            PointerIcon pointerIcon;
            if (!child.hasIdentityMatrix())
            {
                MotionEvent transformedEvent = getTransformedMotionEvent(@event, child);
                pointerIcon = child.onResolvePointerIcon(transformedEvent, pointerIndex);
                transformedEvent.recycle();
            }
            else
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetX = mScrollX - child.mLeft;
                float offsetX = mScrollX - child.mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetY = mScrollY - child.mTop;
                float offsetY = mScrollY - child.mTop;
                @event.offsetLocation(offsetX, offsetY);
                pointerIcon = child.onResolvePointerIcon(@event, pointerIndex);
                @event.offsetLocation(-offsetX, -offsetY);
            }
            return pointerIcon;
        }

        private int getAndVerifyPreorderedIndex(int childrenCount, int i, bool customOrder)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex;
            int childIndex;
            if (customOrder)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex1 = getChildDrawingOrder(childrenCount, i);
                int childIndex1 = getChildDrawingOrder(childrenCount, i);
                if (childIndex1 >= childrenCount)
                {
                    throw new System.IndexOutOfRangeException("getChildDrawingOrder() " + "returned invalid index " + childIndex1 + " (child count is " + childrenCount + ")");
                }
                childIndex = childIndex1;
            }
            else
            {
                childIndex = i;
            }
            return childIndex;
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"ConstantConditions"}) @Override protected boolean dispatchHoverEvent(MotionEvent event)
        protected internal override bool dispatchHoverEvent(MotionEvent @event)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int action = event.getAction();
            int action = @event.Action;

            // First check whether the view group wants to intercept the hover event.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean interceptHover = onInterceptHoverEvent(event);
            bool interceptHover = onInterceptHoverEvent(@event);
            @event.Action = action; // restore action in case it was changed

            MotionEvent eventNoHistory = @event;
            bool handled = false;

            // Send events to the hovered children and build a new list of hover targets until
            // one is found that handles the event.
            HoverTarget firstOldHoverTarget = mFirstHoverTarget;
            mFirstHoverTarget = null;
            if (!interceptHover && action != MotionEvent.ACTION_HOVER_EXIT)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float x = event.getX();
                float x = @event.X;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float y = event.getY();
                float y = @event.Y;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
                int childrenCount = mChildrenCount;
                if (childrenCount != 0)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildOrderedChildList();
                    List<View> preorderedList = buildOrderedChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
                    bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                    View[] children = mChildren;
                    HoverTarget lastHoverTarget = null;
                    for (int i = childrenCount - 1; i >= 0; i--)
                    {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                        int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                        View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                        if (!child.canReceivePointerEvents() || !isTransformedTouchPointInView(x, y, child, null))
                        {
                            continue;
                        }

                        // Obtain a hover target for this child.  Dequeue it from the
                        // old hover target list if the child was previously hovered.
                        HoverTarget hoverTarget = firstOldHoverTarget;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean wasHovered;
                        bool wasHovered;
                        for (HoverTarget predecessor = null; ;)
                        {
                            if (hoverTarget == null)
                            {
                                hoverTarget = HoverTarget.obtain(child);
                                wasHovered = false;
                                break;
                            }

                            if (hoverTarget.child == child)
                            {
                                if (predecessor != null)
                                {
                                    predecessor.next = hoverTarget.next;
                                }
                                else
                                {
                                    firstOldHoverTarget = hoverTarget.next;
                                }
                                hoverTarget.next = null;
                                wasHovered = true;
                                break;
                            }

                            predecessor = hoverTarget;
                            hoverTarget = hoverTarget.next;
                        }

                        // Enqueue the hover target onto the new hover target list.
                        if (lastHoverTarget != null)
                        {
                            lastHoverTarget.next = hoverTarget;
                        }
                        else
                        {
                            mFirstHoverTarget = hoverTarget;
                        }
                        lastHoverTarget = hoverTarget;

                        // Dispatch the event to the child.
                        if (action == MotionEvent.ACTION_HOVER_ENTER)
                        {
                            if (!wasHovered)
                            {
                                // Send the enter as is.
                                handled |= dispatchTransformedGenericPointerEvent(@event, child); // enter
                            }
                        }
                        else if (action == MotionEvent.ACTION_HOVER_MOVE)
                        {
                            if (!wasHovered)
                            {
                                // Synthesize an enter from a move.
                                eventNoHistory = obtainMotionEventNoHistoryOrSelf(eventNoHistory);
                                eventNoHistory.Action = MotionEvent.ACTION_HOVER_ENTER;
                                handled |= dispatchTransformedGenericPointerEvent(eventNoHistory, child); // enter
                                eventNoHistory.Action = action;

                                handled |= dispatchTransformedGenericPointerEvent(eventNoHistory, child); // move
                            }
                            else
                            {
                                // Send the move as is.
                                handled |= dispatchTransformedGenericPointerEvent(@event, child);
                            }
                        }
                        if (handled)
                        {
                            break;
                        }
                    }
                    if (preorderedList != null)
                    {
                        preorderedList.Clear();
                    }
                }
            }

            // Send exit events to all previously hovered children that are no longer hovered.
            while (firstOldHoverTarget != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = firstOldHoverTarget.child;
                View child = firstOldHoverTarget.child;

                // Exit the old hovered child.
                if (action == MotionEvent.ACTION_HOVER_EXIT)
                {
                    // Send the exit as is.
                    handled |= dispatchTransformedGenericPointerEvent(@event, child); // exit
                }
                else
                {
                    // Synthesize an exit from a move or enter.
                    // Ignore the result because hover focus has moved to a different view.
                    if (action == MotionEvent.ACTION_HOVER_MOVE)
                    {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hoverExitPending = event.isHoverExitPending();
                        bool hoverExitPending = @event.HoverExitPending;
                        @event.HoverExitPending = true;
                        dispatchTransformedGenericPointerEvent(@event, child); // move
                        @event.HoverExitPending = hoverExitPending;
                    }
                    eventNoHistory = obtainMotionEventNoHistoryOrSelf(eventNoHistory);
                    eventNoHistory.Action = MotionEvent.ACTION_HOVER_EXIT;
                    dispatchTransformedGenericPointerEvent(eventNoHistory, child); // exit
                    eventNoHistory.Action = action;
                }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HoverTarget nextOldHoverTarget = firstOldHoverTarget.next;
                HoverTarget nextOldHoverTarget = firstOldHoverTarget.next;
                firstOldHoverTarget.recycle();
                firstOldHoverTarget = nextOldHoverTarget;
            }

            // Send events to the view group itself if no children have handled it and the view group
            // itself is not currently being hover-exited.
            bool newHoveredSelf = !handled && (action != MotionEvent.ACTION_HOVER_EXIT) && !@event.HoverExitPending;
            if (newHoveredSelf == mHoveredSelf)
            {
                if (newHoveredSelf)
                {
                    // Send event to the view group as before.
                    handled |= base.dispatchHoverEvent(@event);
                }
            }
            else
            {
                if (mHoveredSelf)
                {
                    // Exit the view group.
                    if (action == MotionEvent.ACTION_HOVER_EXIT)
                    {
                        // Send the exit as is.
                        handled |= base.dispatchHoverEvent(@event); // exit
                    }
                    else
                    {
                        // Synthesize an exit from a move or enter.
                        // Ignore the result because hover focus is moving to a different view.
                        if (action == MotionEvent.ACTION_HOVER_MOVE)
                        {
                            base.dispatchHoverEvent(@event); // move
                        }
                        eventNoHistory = obtainMotionEventNoHistoryOrSelf(eventNoHistory);
                        eventNoHistory.Action = MotionEvent.ACTION_HOVER_EXIT;
                        base.dispatchHoverEvent(eventNoHistory); // exit
                        eventNoHistory.Action = action;
                    }
                    mHoveredSelf = false;
                }

                if (newHoveredSelf)
                {
                    // Enter the view group.
                    if (action == MotionEvent.ACTION_HOVER_ENTER)
                    {
                        // Send the enter as is.
                        handled |= base.dispatchHoverEvent(@event); // enter
                        mHoveredSelf = true;
                    }
                    else if (action == MotionEvent.ACTION_HOVER_MOVE)
                    {
                        // Synthesize an enter from a move.
                        eventNoHistory = obtainMotionEventNoHistoryOrSelf(eventNoHistory);
                        eventNoHistory.Action = MotionEvent.ACTION_HOVER_ENTER;
                        handled |= base.dispatchHoverEvent(eventNoHistory); // enter
                        eventNoHistory.Action = action;

                        handled |= base.dispatchHoverEvent(eventNoHistory); // move
                        mHoveredSelf = true;
                    }
                }
            }

            // Recycle the copy of the event that we made.
            if (eventNoHistory != @event)
            {
                eventNoHistory.recycle();
            }

            // Done.
            return handled;
        }

        private void exitHoverTargets()
        {
            if (mHoveredSelf || mFirstHoverTarget != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long now = android.os.SystemClock.uptimeMillis();
                long now = SystemClock.uptimeMillis();
                MotionEvent @event = MotionEvent.obtain(now, now, MotionEvent.ACTION_HOVER_EXIT, 0.0f, 0.0f, 0);
                @event.Source = InputDevice.SOURCE_TOUCHSCREEN;
                dispatchHoverEvent(@event);
                @event.recycle();
            }
        }

        private void cancelHoverTarget(View view)
        {
            HoverTarget predecessor = null;
            HoverTarget target = mFirstHoverTarget;
            while (target != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HoverTarget next = target.next;
                HoverTarget next = target.next;
                if (target.child == view)
                {
                    if (predecessor == null)
                    {
                        mFirstHoverTarget = next;
                    }
                    else
                    {
                        predecessor.next = next;
                    }
                    target.recycle();

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long now = android.os.SystemClock.uptimeMillis();
                    long now = SystemClock.uptimeMillis();
                    MotionEvent @event = MotionEvent.obtain(now, now, MotionEvent.ACTION_HOVER_EXIT, 0.0f, 0.0f, 0);
                    @event.Source = InputDevice.SOURCE_TOUCHSCREEN;
                    view.dispatchHoverEvent(@event);
                    @event.recycle();
                    return;
                }
                predecessor = target;
                target = next;
            }
        }

        internal override bool dispatchTooltipHoverEvent(MotionEvent @event)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int action = event.getAction();
            int action = @event.Action;
            switch (action)
            {
                case MotionEvent.ACTION_HOVER_ENTER:
                    break;

                case MotionEvent.ACTION_HOVER_MOVE:
                    View newTarget = null;

                    // Check what the child under the pointer says about the tooltip.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
                    int childrenCount = mChildrenCount;
                    if (childrenCount != 0)
                    {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float x = event.getX();
                        float x = @event.X;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float y = event.getY();
                        float y = @event.Y;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildOrderedChildList();
                        List<View> preorderedList = buildOrderedChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
                        bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                        View[] children = mChildren;
                        for (int i = childrenCount - 1; i >= 0; i--)
                        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                            int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                            View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                            if (!child.canReceivePointerEvents() || !isTransformedTouchPointInView(x, y, child, null))
                            {
                                continue;
                            }
                            if (dispatchTooltipHoverEvent(@event, child))
                            {
                                newTarget = child;
                                break;
                            }
                        }
                        if (preorderedList != null)
                        {
                            preorderedList.Clear();
                        }
                    }

                    if (mTooltipHoverTarget != newTarget)
                    {
                        if (mTooltipHoverTarget != null)
                        {
                            @event.Action = MotionEvent.ACTION_HOVER_EXIT;
                            mTooltipHoverTarget.dispatchTooltipHoverEvent(@event);
                            @event.Action = action;
                        }
                        mTooltipHoverTarget = newTarget;
                    }

                    if (mTooltipHoverTarget != null)
                    {
                        if (mTooltipHoveredSelf)
                        {
                            mTooltipHoveredSelf = false;
                            @event.Action = MotionEvent.ACTION_HOVER_EXIT;
                            base.dispatchTooltipHoverEvent(@event);
                            @event.Action = action;
                        }
                        return true;
                    }

                    mTooltipHoveredSelf = base.dispatchTooltipHoverEvent(@event);
                    return mTooltipHoveredSelf;

                case MotionEvent.ACTION_HOVER_EXIT:
                    if (mTooltipHoverTarget != null)
                    {
                        mTooltipHoverTarget.dispatchTooltipHoverEvent(@event);
                        mTooltipHoverTarget = null;
                    }
                    else if (mTooltipHoveredSelf)
                    {
                        base.dispatchTooltipHoverEvent(@event);
                        mTooltipHoveredSelf = false;
                    }
                    break;
            }
            return false;
        }

        private bool dispatchTooltipHoverEvent(MotionEvent @event, View child)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean result;
            bool result;
            if (!child.hasIdentityMatrix())
            {
                MotionEvent transformedEvent = getTransformedMotionEvent(@event, child);
                result = child.dispatchTooltipHoverEvent(transformedEvent);
                transformedEvent.recycle();
            }
            else
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetX = mScrollX - child.mLeft;
                float offsetX = mScrollX - child.mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetY = mScrollY - child.mTop;
                float offsetY = mScrollY - child.mTop;
                @event.offsetLocation(offsetX, offsetY);
                result = child.dispatchTooltipHoverEvent(@event);
                @event.offsetLocation(-offsetX, -offsetY);
            }
            return result;
        }

        private void exitTooltipHoverTargets()
        {
            if (mTooltipHoveredSelf || mTooltipHoverTarget != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long now = android.os.SystemClock.uptimeMillis();
                long now = SystemClock.uptimeMillis();
                MotionEvent @event = MotionEvent.obtain(now, now, MotionEvent.ACTION_HOVER_EXIT, 0.0f, 0.0f, 0);
                @event.Source = InputDevice.SOURCE_TOUCHSCREEN;
                dispatchTooltipHoverEvent(@event);
                @event.recycle();
            }
        }

        /// <summary>
        /// @hide </summary>
        protected internal override bool hasHoveredChild()
        {
            return mFirstHoverTarget != null;
        }

        /// <summary>
        /// @hide </summary>
        protected internal override bool pointInHoveredChild(MotionEvent @event)
        {
            if (mFirstHoverTarget != null)
            {
                return isTransformedTouchPointInView(@event.X, @event.Y, mFirstHoverTarget.child, null);
            }
            return false;
        }

        public override void addChildrenForAccessibility(List<View> outChildren)
        {
            if (AccessibilityNodeProvider != null)
            {
                return;
            }
            ChildListForAccessibility children = ChildListForAccessibility.obtain(this, true);
            try
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = children.getChildCount();
                int childrenCount = children.ChildCount;
                for (int i = 0; i < childrenCount; i++)
                {
                    View child = children.getChildAt(i);
                    if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE)
                    {
                        if (child.includeForAccessibility())
                        {
                            outChildren.Add(child);
                        }
                        else
                        {
                            child.addChildrenForAccessibility(outChildren);
                        }
                    }
                }
            }
            finally
            {
                children.recycle();
            }
        }

        /// <summary>
        /// Implement this method to intercept hover events before they are handled
        /// by child views.
        /// <para>
        /// This method is called before dispatching a hover event to a child of
        /// the view group or to the view group's own <seealso cref="#onHoverEvent"/> to allow
        /// the view group a chance to intercept the hover event.
        /// This method can also be used to watch all pointer motions that occur within
        /// the bounds of the view group even when the pointer is hovering over
        /// a child of the view group rather than over the view group itself.
        /// </para>
        /// </para><para>
        /// The view group can prevent its children from receiving hover events by
        /// implementing this method and returning <code>true</code> to indicate
        /// that it would like to intercept hover events.  The view group must
        /// continuously return <code>true</code> from <seealso cref="#onInterceptHoverEvent"/>
        /// for as long as it wishes to continue intercepting hover events from
        /// its children.
        /// </para><para>
        /// Interception preserves the invariant that at most one view can be
        /// hovered at a time by transferring hover focus from the currently hovered
        /// child to the view group or vice-versa as needed.
        /// </para><para>
        /// If this method returns <code>true</code> and a child is already hovered, then the
        /// child view will first receive a hover exit event and then the view group
        /// itself will receive a hover enter event in <seealso cref="#onHoverEvent"/>.
        /// Likewise, if this method had previously returned <code>true</code> to intercept hover
        /// events and instead returns <code>false</code> while the pointer is hovering
        /// within the bounds of one of a child, then the view group will first receive a
        /// hover exit event in <seealso cref="#onHoverEvent"/> and then the hovered child will
        /// receive a hover enter event.
        /// </para><para>
        /// The default implementation handles mouse hover on the scroll bars.
        /// </p>
        /// </summary>
        /// <param name="event"> The motion event that describes the hover. </param>
        /// <returns> True if the view group would like to intercept the hover event
        /// and prevent its children from receiving it. </returns>
        public virtual bool onInterceptHoverEvent(MotionEvent @event)
        {
            if (@event.isFromSource(InputDevice.SOURCE_MOUSE))
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int action = event.getAction();
                int action = @event.Action;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float x = event.getX();
                float x = @event.X;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float y = event.getY();
                float y = @event.Y;
                if ((action == MotionEvent.ACTION_HOVER_MOVE || action == MotionEvent.ACTION_HOVER_ENTER) && isOnScrollbar(x, y))
                {
                    return true;
                }
            }
            return false;
        }

        private static MotionEvent obtainMotionEventNoHistoryOrSelf(MotionEvent @event)
        {
            if (@event.HistorySize == 0)
            {
                return @event;
            }
            return MotionEvent.obtainNoHistory(@event);
        }

        protected internal override bool dispatchGenericPointerEvent(MotionEvent @event)
        {
            // Send the event to the child under the pointer.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
            if (childrenCount != 0)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float x = event.getX();
                float x = @event.X;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float y = event.getY();
                float y = @event.Y;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildOrderedChildList();
                List<View> preorderedList = buildOrderedChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
                bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                View[] children = mChildren;
                for (int i = childrenCount - 1; i >= 0; i--)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                    int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                    View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                    if (!child.canReceivePointerEvents() || !isTransformedTouchPointInView(x, y, child, null))
                    {
                        continue;
                    }

                    if (dispatchTransformedGenericPointerEvent(@event, child))
                    {
                        if (preorderedList != null)
                        {
                            preorderedList.Clear();
                        }
                        return true;
                    }
                }
                if (preorderedList != null)
                {
                    preorderedList.Clear();
                }
            }

            // No child handled the event.  Send it to this view group.
            return base.dispatchGenericPointerEvent(@event);
        }

        protected internal override bool dispatchGenericFocusedEvent(MotionEvent @event)
        {
            // Send the event to the focused child or to this view group if it has focus.
            if ((mPrivateFlags & (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS)) == (PFLAG_FOCUSED | PFLAG_HAS_BOUNDS))
            {
                return base.dispatchGenericFocusedEvent(@event);
            }
            else if (mFocused != null && (mFocused.mPrivateFlags & PFLAG_HAS_BOUNDS) == PFLAG_HAS_BOUNDS)
            {
                return mFocused.dispatchGenericMotionEvent(@event);
            }
            return false;
        }

        /// <summary>
        /// Dispatches a generic pointer event to a child, taking into account
        /// transformations that apply to the child.
        /// </summary>
        /// <param name="event"> The event to send. </param>
        /// <param name="child"> The view to send the event to. </param>
        /// <returns> {@code true} if the child handled the event. </returns>
        private bool dispatchTransformedGenericPointerEvent(MotionEvent @event, View child)
        {
            bool handled;
            if (!child.hasIdentityMatrix())
            {
                MotionEvent transformedEvent = getTransformedMotionEvent(@event, child);
                handled = child.dispatchGenericMotionEvent(transformedEvent);
                transformedEvent.recycle();
            }
            else
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetX = mScrollX - child.mLeft;
                float offsetX = mScrollX - child.mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetY = mScrollY - child.mTop;
                float offsetY = mScrollY - child.mTop;
                @event.offsetLocation(offsetX, offsetY);
                handled = child.dispatchGenericMotionEvent(@event);
                @event.offsetLocation(-offsetX, -offsetY);
            }
            return handled;
        }

        /// <summary>
        /// Returns a MotionEvent that's been transformed into the child's local coordinates.
        /// 
        /// It's the responsibility of the caller to recycle it once they're finished with it. </summary>
        /// <param name="event"> The event to transform. </param>
        /// <param name="child"> The view whose coordinate space is to be used. </param>
        /// <returns> A copy of the the given MotionEvent, transformed into the given View's coordinate
        ///         space. </returns>
        private MotionEvent getTransformedMotionEvent(MotionEvent @event, View child)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetX = mScrollX - child.mLeft;
            float offsetX = mScrollX - child.mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetY = mScrollY - child.mTop;
            float offsetY = mScrollY - child.mTop;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MotionEvent transformedEvent = MotionEvent.obtain(event);
            MotionEvent transformedEvent = MotionEvent.obtain(@event);
            transformedEvent.offsetLocation(offsetX, offsetY);
            if (!child.hasIdentityMatrix())
            {
                transformedEvent.transform(child.InverseMatrix);
            }
            return transformedEvent;
        }

        public override bool dispatchTouchEvent(MotionEvent ev)
        {
            if (mInputEventConsistencyVerifier != null)
            {
                mInputEventConsistencyVerifier.onTouchEvent(ev, 1);
            }

            // If the event targets the accessibility focused view and this is it, start
            // normal event dispatch. Maybe a descendant is what will handle the click.
            if (ev.TargetAccessibilityFocus && AccessibilityFocusedViewOrHost)
            {
                ev.TargetAccessibilityFocus = false;
            }

            bool handled = false;
            if (onFilterTouchEventForSecurity(ev))
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int action = ev.getAction();
                int action = ev.Action;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int actionMasked = action & MotionEvent.ACTION_MASK;
                int actionMasked = action & MotionEvent.ACTION_MASK;

                // Handle an initial down.
                if (actionMasked == MotionEvent.ACTION_DOWN)
                {
                    // Throw away all previous state when starting a new touch gesture.
                    // The framework may have dropped the up or cancel event for the previous gesture
                    // due to an app switch, ANR, or some other state change.
                    cancelAndClearTouchTargets(ev);
                    resetTouchState();
                }

                // Check for interception.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean intercepted;
                bool intercepted;
                if (actionMasked == MotionEvent.ACTION_DOWN || mFirstTouchTarget != null)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean disallowIntercept = (mGroupFlags & FLAG_DISALLOW_INTERCEPT) != 0;
                    bool disallowIntercept = (mGroupFlags & FLAG_DISALLOW_INTERCEPT) != 0;
                    if (!disallowIntercept)
                    {
                        intercepted = onInterceptTouchEvent(ev);
                        ev.Action = action; // restore action in case it was changed
                    }
                    else
                    {
                        intercepted = false;
                    }
                }
                else
                {
                    // There are no touch targets and this action is not an initial down
                    // so this view group continues to intercept touches.
                    intercepted = true;
                }

                // If intercepted, start normal event dispatch. Also if there is already
                // a view that is handling the gesture, do normal event dispatch.
                if (intercepted || mFirstTouchTarget != null)
                {
                    ev.TargetAccessibilityFocus = false;
                }

                // Check for cancelation.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean canceled = resetCancelNextUpFlag(this) || actionMasked == MotionEvent.ACTION_CANCEL;
                bool canceled = resetCancelNextUpFlag(this) || actionMasked == MotionEvent.ACTION_CANCEL;

                // Update list of touch targets for pointer down, if needed.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isMouseEvent = ev.getSource() == InputDevice.SOURCE_MOUSE;
                bool isMouseEvent = ev.Source == InputDevice.SOURCE_MOUSE;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean split = (mGroupFlags & FLAG_SPLIT_MOTION_EVENTS) != 0 && !isMouseEvent;
                bool split = (mGroupFlags & FLAG_SPLIT_MOTION_EVENTS) != 0 && !isMouseEvent;
                TouchTarget newTouchTarget = null;
                bool alreadyDispatchedToNewTouchTarget = false;
                if (!canceled && !intercepted)
                {
                    // If the event is targeting accessibility focus we give it to the
                    // view that has accessibility focus and if it does not handle it
                    // we clear the flag and dispatch the event to all children as usual.
                    // We are looking up the accessibility focused host to avoid keeping
                    // state since these events are very rare.
                    View childWithAccessibilityFocus = ev.TargetAccessibilityFocus ? findChildWithAccessibilityFocus() : null;

                    if (actionMasked == MotionEvent.ACTION_DOWN || (split && actionMasked == MotionEvent.ACTION_POINTER_DOWN) || actionMasked == MotionEvent.ACTION_HOVER_MOVE)
                    {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int actionIndex = ev.getActionIndex();
                        int actionIndex = ev.ActionIndex; // always 0 for down
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int idBitsToAssign = split ? 1 << ev.getPointerId(actionIndex) : TouchTarget.ALL_POINTER_IDS;
                        int idBitsToAssign = split ? 1 << ev.getPointerId(actionIndex) : TouchTarget.ALL_POINTER_IDS;

                        // Clean up earlier touch targets for this pointer id in case they
                        // have become out of sync.
                        removePointersFromTouchTargets(idBitsToAssign);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
                        int childrenCount = mChildrenCount;
                        if (newTouchTarget == null && childrenCount != 0)
                        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float x = isMouseEvent ? ev.getXCursorPosition() : ev.getX(actionIndex);
                            float x = isMouseEvent ? ev.XCursorPosition : ev.getX(actionIndex);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float y = isMouseEvent ? ev.getYCursorPosition() : ev.getY(actionIndex);
                            float y = isMouseEvent ? ev.YCursorPosition : ev.getY(actionIndex);
                            // Find a child that can receive the event.
                            // Scan children from front to back.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildTouchDispatchChildList();
                            List<View> preorderedList = buildTouchDispatchChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
                            bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                            View[] children = mChildren;
                            for (int i = childrenCount - 1; i >= 0; i--)
                            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                                int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                                View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                                if (!child.canReceivePointerEvents() || !isTransformedTouchPointInView(x, y, child, null))
                                {
                                    continue;
                                }

                                newTouchTarget = getTouchTarget(child);
                                if (newTouchTarget != null)
                                {
                                    // Child is already receiving touch within its bounds.
                                    // Give it the new pointer in addition to the ones it is handling.
                                    newTouchTarget.pointerIdBits |= idBitsToAssign;
                                    break;
                                }

                                resetCancelNextUpFlag(child);
                                if (dispatchTransformedTouchEvent(ev, false, child, idBitsToAssign))
                                {
                                    // Child wants to receive touch within its bounds.
                                    mLastTouchDownTime = ev.DownTime;
                                    if (preorderedList != null)
                                    {
                                        // childIndex points into presorted list, find original index
                                        for (int j = 0; j < childrenCount; j++)
                                        {
                                            if (children[childIndex] == mChildren[j])
                                            {
                                                mLastTouchDownIndex = j;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        mLastTouchDownIndex = childIndex;
                                    }
                                    mLastTouchDownX = ev.X;
                                    mLastTouchDownY = ev.Y;
                                    newTouchTarget = addTouchTarget(child, idBitsToAssign);
                                    alreadyDispatchedToNewTouchTarget = true;
                                    break;
                                }

                                // The accessibility focus didn't handle the event, so clear
                                // the flag and do a normal dispatch to all children.
                                ev.TargetAccessibilityFocus = false;
                            }
                            if (preorderedList != null)
                            {
                                preorderedList.Clear();
                            }
                        }

                        if (newTouchTarget == null && mFirstTouchTarget != null)
                        {
                            // Did not find a child to receive the event.
                            // Assign the pointer to the least recently added target.
                            newTouchTarget = mFirstTouchTarget;
                            while (newTouchTarget.next != null)
                            {
                                newTouchTarget = newTouchTarget.next;
                            }
                            newTouchTarget.pointerIdBits |= idBitsToAssign;
                        }
                    }
                }

                // Dispatch to touch targets.
                if (mFirstTouchTarget == null)
                {
                    // No touch targets so treat this as an ordinary view.
                    handled = dispatchTransformedTouchEvent(ev, canceled, null, TouchTarget.ALL_POINTER_IDS);
                }
                else
                {
                    // Dispatch to touch targets, excluding the new touch target if we already
                    // dispatched to it.  Cancel touch targets if necessary.
                    TouchTarget predecessor = null;
                    TouchTarget target = mFirstTouchTarget;
                    while (target != null)
                    {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TouchTarget next = target.next;
                        TouchTarget next = target.next;
                        if (alreadyDispatchedToNewTouchTarget && target == newTouchTarget)
                        {
                            handled = true;
                        }
                        else
                        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean cancelChild = resetCancelNextUpFlag(target.child) || intercepted;
                            bool cancelChild = resetCancelNextUpFlag(target.child) || intercepted;
                            if (dispatchTransformedTouchEvent(ev, cancelChild, target.child, target.pointerIdBits))
                            {
                                handled = true;
                            }
                            if (cancelChild)
                            {
                                if (predecessor == null)
                                {
                                    mFirstTouchTarget = next;
                                }
                                else
                                {
                                    predecessor.next = next;
                                }
                                target.recycle();
                                target = next;
                                continue;
                            }
                        }
                        predecessor = target;
                        target = next;
                    }
                }

                // Update list of touch targets for pointer up or cancel, if needed.
                if (canceled || actionMasked == MotionEvent.ACTION_UP || actionMasked == MotionEvent.ACTION_HOVER_MOVE)
                {
                    resetTouchState();
                }
                else if (split && actionMasked == MotionEvent.ACTION_POINTER_UP)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int actionIndex = ev.getActionIndex();
                    int actionIndex = ev.ActionIndex;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int idBitsToRemove = 1 << ev.getPointerId(actionIndex);
                    int idBitsToRemove = 1 << ev.getPointerId(actionIndex);
                    removePointersFromTouchTargets(idBitsToRemove);
                }
            }

            if (!handled && mInputEventConsistencyVerifier != null)
            {
                mInputEventConsistencyVerifier.onUnhandledEvent(ev, 1);
            }
            return handled;
        }

        /// <summary>
        /// Provide custom ordering of views in which the touch will be dispatched.
        /// 
        /// This is called within a tight loop, so you are not allowed to allocate objects, including
        /// the return array. Instead, you should return a pre-allocated list that will be cleared
        /// after the dispatch is finished.
        /// @hide
        /// </summary>
        public virtual List<View> buildTouchDispatchChildList()
        {
            return buildOrderedChildList();
        }

         /// <summary>
         /// Finds the child which has accessibility focus.
         /// </summary>
         /// <returns> The child that has focus. </returns>
        private View findChildWithAccessibilityFocus()
        {
            ViewRootImpl viewRoot = ViewRootImpl;
            if (viewRoot == null)
            {
                return null;
            }

            View current = viewRoot.AccessibilityFocusedHost;
            if (current == null)
            {
                return null;
            }

            ViewParent parent = current.Parent;
            while (parent is View)
            {
                if (parent == this)
                {
                    return current;
                }
                current = (View) parent;
                parent = current.Parent;
            }

            return null;
        }

        /// <summary>
        /// Resets all touch state in preparation for a new cycle.
        /// </summary>
        private void resetTouchState()
        {
            clearTouchTargets();
            resetCancelNextUpFlag(this);
            mGroupFlags &= ~FLAG_DISALLOW_INTERCEPT;
            mNestedScrollAxes = SCROLL_AXIS_NONE;
        }

        /// <summary>
        /// Resets the cancel next up flag.
        /// Returns true if the flag was previously set.
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: private static boolean resetCancelNextUpFlag(@NonNull View view)
        private static bool resetCancelNextUpFlag(View view)
        {
            if ((view.mPrivateFlags & PFLAG_CANCEL_NEXT_UP_EVENT) != 0)
            {
                view.mPrivateFlags &= ~PFLAG_CANCEL_NEXT_UP_EVENT;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all touch targets.
        /// </summary>
        private void clearTouchTargets()
        {
            TouchTarget target = mFirstTouchTarget;
            if (target != null)
            {
                do
                {
                    TouchTarget next = target.next;
                    target.recycle();
                    target = next;
                } while (target != null);
                mFirstTouchTarget = null;
            }
        }

        /// <summary>
        /// Cancels and clears all touch targets.
        /// </summary>
        private void cancelAndClearTouchTargets(MotionEvent @event)
        {
            if (mFirstTouchTarget != null)
            {
                bool syntheticEvent = false;
                if (@event == null)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long now = android.os.SystemClock.uptimeMillis();
                    long now = SystemClock.uptimeMillis();
                    @event = MotionEvent.obtain(now, now, MotionEvent.ACTION_CANCEL, 0.0f, 0.0f, 0);
                    @event.Source = InputDevice.SOURCE_TOUCHSCREEN;
                    syntheticEvent = true;
                }

                for (TouchTarget target = mFirstTouchTarget; target != null; target = target.next)
                {
                    resetCancelNextUpFlag(target.child);
                    dispatchTransformedTouchEvent(@event, true, target.child, target.pointerIdBits);
                }
                clearTouchTargets();

                if (syntheticEvent)
                {
                    @event.recycle();
                }
            }
        }

        /// <summary>
        /// Gets the touch target for specified child view.
        /// Returns null if not found.
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: private TouchTarget getTouchTarget(@NonNull View child)
        private TouchTarget getTouchTarget(View child)
        {
            for (TouchTarget target = mFirstTouchTarget; target != null; target = target.next)
            {
                if (target.child == child)
                {
                    return target;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a touch target for specified child to the beginning of the list.
        /// Assumes the target child is not already present.
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: private TouchTarget addTouchTarget(@NonNull View child, int pointerIdBits)
        private TouchTarget addTouchTarget(View child, int pointerIdBits)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TouchTarget target = TouchTarget.obtain(child, pointerIdBits);
            TouchTarget target = TouchTarget.obtain(child, pointerIdBits);
            target.next = mFirstTouchTarget;
            mFirstTouchTarget = target;
            return target;
        }

        /// <summary>
        /// Removes the pointer ids from consideration.
        /// </summary>
        private void removePointersFromTouchTargets(int pointerIdBits)
        {
            TouchTarget predecessor = null;
            TouchTarget target = mFirstTouchTarget;
            while (target != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TouchTarget next = target.next;
                TouchTarget next = target.next;
                if ((target.pointerIdBits & pointerIdBits) != 0)
                {
                    target.pointerIdBits &= ~pointerIdBits;
                    if (target.pointerIdBits == 0)
                    {
                        if (predecessor == null)
                        {
                            mFirstTouchTarget = next;
                        }
                        else
                        {
                            predecessor.next = next;
                        }
                        target.recycle();
                        target = next;
                        continue;
                    }
                }
                predecessor = target;
                target = next;
            }
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage private void cancelTouchTarget(View view)
        private void cancelTouchTarget(View view)
        {
            TouchTarget predecessor = null;
            TouchTarget target = mFirstTouchTarget;
            while (target != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TouchTarget next = target.next;
                TouchTarget next = target.next;
                if (target.child == view)
                {
                    if (predecessor == null)
                    {
                        mFirstTouchTarget = next;
                    }
                    else
                    {
                        predecessor.next = next;
                    }
                    target.recycle();

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long now = android.os.SystemClock.uptimeMillis();
                    long now = SystemClock.uptimeMillis();
                    MotionEvent @event = MotionEvent.obtain(now, now, MotionEvent.ACTION_CANCEL, 0.0f, 0.0f, 0);
                    @event.Source = InputDevice.SOURCE_TOUCHSCREEN;
                    view.dispatchTouchEvent(@event);
                    @event.recycle();
                    return;
                }
                predecessor = target;
                target = next;
            }
        }

        private Rect TempRect
        {
            get
            {
                if (mTempRect == null)
                {
                    mTempRect = new Rect();
                }
                return mTempRect;
            }
        }

        private float[] TempLocationF
        {
            get
            {
                if (mTempPosition == null)
                {
                    mTempPosition = new float[2];
                }
                return mTempPosition;
            }
        }

        private Point TempPoint
        {
            get
            {
                if (mTempPoint == null)
                {
                    mTempPoint = new Point();
                }
                return mTempPoint;
            }
        }

        /// <summary>
        /// Returns true if a child view contains the specified point when transformed
        /// into its coordinate space.
        /// Child must not be null.
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage protected boolean isTransformedTouchPointInView(float x, float y, View child, android.graphics.PointF outLocalPoint)
        protected internal virtual bool isTransformedTouchPointInView(float x, float y, View child, PointF outLocalPoint)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float[] point = getTempLocationF();
            float[] point = TempLocationF;
            point[0] = x;
            point[1] = y;
            transformPointToViewLocal(point, child);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean isInView = child.pointInView(point[0], point[1]);
            bool isInView = child.pointInView(point[0], point[1]);
            if (isInView && outLocalPoint != null)
            {
                outLocalPoint.set(point[0], point[1]);
            }
            return isInView;
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage public void transformPointToViewLocal(float[] point, View child)
        public virtual void transformPointToViewLocal(float[] point, View child)
        {
            point[0] += mScrollX - child.mLeft;
            point[1] += mScrollY - child.mTop;

            if (!child.hasIdentityMatrix())
            {
                child.InverseMatrix.mapPoints(point);
            }
        }

        /// <summary>
        /// Transforms a motion event into the coordinate space of a particular child view,
        /// filters out irrelevant pointer ids, and overrides its action if necessary.
        /// If child is null, assumes the MotionEvent will be sent to this ViewGroup instead.
        /// </summary>
        private bool dispatchTransformedTouchEvent(MotionEvent @event, bool cancel, View child, int desiredPointerIdBits)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean handled;
            bool handled;

            // Canceling motions is a special case.  We don't need to perform any transformations
            // or filtering.  The important part is the action, not the contents.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldAction = event.getAction();
            int oldAction = @event.Action;
            if (cancel || oldAction == MotionEvent.ACTION_CANCEL)
            {
                @event.Action = MotionEvent.ACTION_CANCEL;
                if (child == null)
                {
                    handled = base.dispatchTouchEvent(@event);
                }
                else
                {
                    handled = child.dispatchTouchEvent(@event);
                }
                @event.Action = oldAction;
                return handled;
            }

            // Calculate the number of pointers to deliver.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldPointerIdBits = event.getPointerIdBits();
            int oldPointerIdBits = @event.PointerIdBits;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int newPointerIdBits = oldPointerIdBits & desiredPointerIdBits;
            int newPointerIdBits = oldPointerIdBits & desiredPointerIdBits;

            // If for some reason we ended up in an inconsistent state where it looks like we
            // might produce a motion event with no pointers in it, then drop the event.
            if (newPointerIdBits == 0)
            {
                return false;
            }

            // If the number of pointers is the same and we don't need to perform any fancy
            // irreversible transformations, then we can reuse the motion event for this
            // dispatch as long as we are careful to revert any changes we make.
            // Otherwise we need to make a copy.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MotionEvent transformedEvent;
            MotionEvent transformedEvent;
            if (newPointerIdBits == oldPointerIdBits)
            {
                if (child == null || child.hasIdentityMatrix())
                {
                    if (child == null)
                    {
                        handled = base.dispatchTouchEvent(@event);
                    }
                    else
                    {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetX = mScrollX - child.mLeft;
                        float offsetX = mScrollX - child.mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetY = mScrollY - child.mTop;
                        float offsetY = mScrollY - child.mTop;
                        @event.offsetLocation(offsetX, offsetY);

                        handled = child.dispatchTouchEvent(@event);

                        @event.offsetLocation(-offsetX, -offsetY);
                    }
                    return handled;
                }
                transformedEvent = MotionEvent.obtain(@event);
            }
            else
            {
                transformedEvent = @event.split(newPointerIdBits);
            }

            // Perform any necessary transformations and dispatch.
            if (child == null)
            {
                handled = base.dispatchTouchEvent(transformedEvent);
            }
            else
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetX = mScrollX - child.mLeft;
                float offsetX = mScrollX - child.mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float offsetY = mScrollY - child.mTop;
                float offsetY = mScrollY - child.mTop;
                transformedEvent.offsetLocation(offsetX, offsetY);
                if (!child.hasIdentityMatrix())
                {
                    transformedEvent.transform(child.InverseMatrix);
                }

                handled = child.dispatchTouchEvent(transformedEvent);
            }

            // Done.
            transformedEvent.recycle();
            return handled;
        }

        /// <summary>
        /// Enable or disable the splitting of MotionEvents to multiple children during touch event
        /// dispatch. This behavior is enabled by default for applications that target an
        /// SDK version of <seealso cref="Build.VERSION_CODES#HONEYCOMB"/> or newer.
        /// 
        /// <para>When this option is enabled MotionEvents may be split and dispatched to different child
        /// views depending on where each pointer initially went down. This allows for user interactions
        /// such as scrolling two panes of content independently, chording of buttons, and performing
        /// independent gestures on different pieces of content.
        /// 
        /// </para>
        /// </summary>
        /// <param name="split"> <code>true</code> to allow MotionEvents to be split and dispatched to multiple
        ///              child views. <code>false</code> to only allow one child view to be the target of
        ///              any MotionEvent received by this ViewGroup.
        /// @attr ref android.R.styleable#ViewGroup_splitMotionEvents </param>
        public virtual bool MotionEventSplittingEnabled
        {
            set
            {
                // TODO Applications really shouldn't change this setting mid-touch event,
                // but perhaps this should handle that case and send ACTION_CANCELs to any child views
                // with gestures in progress when this is changed.
                if (value)
                {
                    mGroupFlags |= FLAG_SPLIT_MOTION_EVENTS;
                }
                else
                {
                    mGroupFlags &= ~FLAG_SPLIT_MOTION_EVENTS;
                }
            }
            get
            {
                return (mGroupFlags & FLAG_SPLIT_MOTION_EVENTS) == FLAG_SPLIT_MOTION_EVENTS;
            }
        }


        /// <summary>
        /// Returns true if this ViewGroup should be considered as a single entity for removal
        /// when executing an Activity transition. If this is false, child elements will move
        /// individually during the transition.
        /// </summary>
        /// <returns> True if the ViewGroup should be acted on together during an Activity transition.
        /// The default value is true when there is a non-null background or if
        /// <seealso cref="#getTransitionName()"/> is not null or if a
        /// non-null <seealso cref="android.view.ViewOutlineProvider"/> other than
        /// <seealso cref="android.view.ViewOutlineProvider#BACKGROUND"/> was given to
        /// <seealso cref="#setOutlineProvider(ViewOutlineProvider)"/> and false otherwise. </returns>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @InspectableProperty public boolean isTransitionGroup()
        public virtual bool TransitionGroup
        {
            get
            {
                if ((mGroupFlags & FLAG_IS_TRANSITION_GROUP_SET) != 0)
                {
                    return ((mGroupFlags & FLAG_IS_TRANSITION_GROUP) != 0);
                }
                else
                {
    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final ViewOutlineProvider outlineProvider = getOutlineProvider();
                    ViewOutlineProvider outlineProvider = OutlineProvider;
                    return Background != null || TransitionName != null || (outlineProvider != null && outlineProvider != ViewOutlineProvider.BACKGROUND);
                }
            }
            set
            {
                mGroupFlags |= FLAG_IS_TRANSITION_GROUP_SET;
                if (value)
                {
                    mGroupFlags |= FLAG_IS_TRANSITION_GROUP;
                }
                else
                {
                    mGroupFlags &= ~FLAG_IS_TRANSITION_GROUP;
                }
            }
        }


        public override void requestDisallowInterceptTouchEvent(bool disallowIntercept)
        {

            if (disallowIntercept == ((mGroupFlags & FLAG_DISALLOW_INTERCEPT) != 0))
            {
                // We're already in this state, assume our ancestors are too
                return;
            }

            if (disallowIntercept)
            {
                mGroupFlags |= FLAG_DISALLOW_INTERCEPT;
            }
            else
            {
                mGroupFlags &= ~FLAG_DISALLOW_INTERCEPT;
            }

            // Pass it up to our parent
            if (mParent != null)
            {
                mParent.requestDisallowInterceptTouchEvent(disallowIntercept);
            }
        }

        /// <summary>
        /// Implement this method to intercept all touch screen motion events.  This
        /// allows you to watch events as they are dispatched to your children, and
        /// take ownership of the current gesture at any point.
        /// 
        /// <para>Using this function takes some care, as it has a fairly complicated
        /// interaction with {@link View#onTouchEvent(MotionEvent)
        /// View.onTouchEvent(MotionEvent)}, and using it requires implementing
        /// that method as well as this one in the correct way.  Events will be
        /// received in the following order:
        /// 
        /// <ol>
        /// <li> You will receive the down event here.
        /// <li> The down event will be handled either by a child of this view
        /// group, or given to your own onTouchEvent() method to handle; this means
        /// you should implement onTouchEvent() to return true, so you will
        /// continue to see the rest of the gesture (instead of looking for
        /// a parent view to handle it).  Also, by returning true from
        /// onTouchEvent(), you will not receive any following
        /// events in onInterceptTouchEvent() and all touch processing must
        /// happen in onTouchEvent() like normal.
        /// <li> For as long as you return false from this function, each following
        /// event (up to and including the final up) will be delivered first here
        /// and then to the target's onTouchEvent().
        /// <li> If you return true from here, you will not receive any
        /// following events: the target view will receive the same event but
        /// with the action <seealso cref="MotionEvent#ACTION_CANCEL"/>, and all further
        /// events will be delivered to your onTouchEvent() method and no longer
        /// appear here.
        /// </ol>
        /// 
        /// </para>
        /// </summary>
        /// <param name="ev"> The motion event being dispatched down the hierarchy. </param>
        /// <returns> Return true to steal motion events from the children and have
        /// them dispatched to this ViewGroup through onTouchEvent().
        /// The current target will receive an ACTION_CANCEL event, and no further
        /// messages will be delivered here. </returns>
        public virtual bool onInterceptTouchEvent(MotionEvent ev)
        {
            if (ev.isFromSource(InputDevice.SOURCE_MOUSE) && ev.Action == MotionEvent.ACTION_DOWN && ev.isButtonPressed(MotionEvent.BUTTON_PRIMARY) && isOnScrollbarThumb(ev.X, ev.Y))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// Looks for a view to give focus to respecting the setting specified by
        /// <seealso cref="#getDescendantFocusability()"/>.
        /// 
        /// Uses <seealso cref="#onRequestFocusInDescendants(int, android.graphics.Rect)"/> to
        /// find focus within the children of this group when appropriate.
        /// </summary>
        /// <seealso cref= #FOCUS_BEFORE_DESCENDANTS </seealso>
        /// <seealso cref= #FOCUS_AFTER_DESCENDANTS </seealso>
        /// <seealso cref= #FOCUS_BLOCK_DESCENDANTS </seealso>
        /// <seealso cref= #onRequestFocusInDescendants(int, android.graphics.Rect) </seealso>
        public override bool requestFocus(int direction, Rect previouslyFocusedRect)
        {
            if (DBG)
            {
                Console.WriteLine(this + " ViewGroup.requestFocus direction=" + direction);
            }
            int descendantFocusability = DescendantFocusability;

            bool result;
            switch (descendantFocusability)
            {
                case FOCUS_BLOCK_DESCENDANTS:
                    result = base.requestFocus(direction, previouslyFocusedRect);
                    break;
                case FOCUS_BEFORE_DESCENDANTS:
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean took = super.requestFocus(direction, previouslyFocusedRect);
                    bool took = base.requestFocus(direction, previouslyFocusedRect);
                    result = took ? took : onRequestFocusInDescendants(direction, previouslyFocusedRect);
                    break;
                }
                case FOCUS_AFTER_DESCENDANTS:
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean took = onRequestFocusInDescendants(direction, previouslyFocusedRect);
                    bool took = onRequestFocusInDescendants(direction, previouslyFocusedRect);
                    result = took ? took : base.requestFocus(direction, previouslyFocusedRect);
                    break;
                }
                default:
                    throw new System.InvalidOperationException("descendant focusability must be one of FOCUS_BEFORE_DESCENDANTS," + " FOCUS_AFTER_DESCENDANTS, FOCUS_BLOCK_DESCENDANTS but is " + descendantFocusability);
            }
            if (result && !LayoutValid && ((mPrivateFlags & PFLAG_WANTS_FOCUS) == 0))
            {
                mPrivateFlags |= PFLAG_WANTS_FOCUS;
            }
            return result;
        }

        /// <summary>
        /// Look for a descendant to call <seealso cref="View#requestFocus"/> on.
        /// Called by <seealso cref="ViewGroup#requestFocus(int, android.graphics.Rect)"/>
        /// when it wants to request focus within its children.  Override this to
        /// customize how your <seealso cref="ViewGroup"/> requests focus within its children. </summary>
        /// <param name="direction"> One of FOCUS_UP, FOCUS_DOWN, FOCUS_LEFT, and FOCUS_RIGHT </param>
        /// <param name="previouslyFocusedRect"> The rectangle (in this View's coordinate system)
        ///        to give a finer grained hint about where focus is coming from.  May be null
        ///        if there is no hint. </param>
        /// <returns> Whether focus was taken. </returns>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"ConstantConditions"}) protected boolean onRequestFocusInDescendants(int direction, android.graphics.Rect previouslyFocusedRect)
        protected internal virtual bool onRequestFocusInDescendants(int direction, Rect previouslyFocusedRect)
        {
            int index;
            int increment;
            int end;
            int count = mChildrenCount;
            if ((direction & FOCUS_FORWARD) != 0)
            {
                index = 0;
                increment = 1;
                end = count;
            }
            else
            {
                index = count - 1;
                increment = -1;
                end = -1;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = index; i != end; i += increment)
            {
                View child = children[i];
                if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE)
                {
                    if (child.requestFocus(direction, previouslyFocusedRect))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool restoreDefaultFocus()
        {
            if (mDefaultFocus != null && DescendantFocusability != FOCUS_BLOCK_DESCENDANTS && (mDefaultFocus.mViewFlags & VISIBILITY_MASK) == VISIBLE && mDefaultFocus.restoreDefaultFocus())
            {
                return true;
            }
            return base.restoreDefaultFocus();
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TestApi @Override public boolean restoreFocusInCluster(@FocusRealDirection int direction)
        public override bool restoreFocusInCluster(int direction)
        {
            // Allow cluster-navigation to enter touchscreenBlocksFocus ViewGroups.
            if (KeyboardNavigationCluster)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean blockedFocus = getTouchscreenBlocksFocus();
                bool blockedFocus = TouchscreenBlocksFocus;
                try
                {
                    TouchscreenBlocksFocusNoRefocus = false;
                    return restoreFocusInClusterInternal(direction);
                }
                finally
                {
                    TouchscreenBlocksFocusNoRefocus = blockedFocus;
                }
            }
            else
            {
                return restoreFocusInClusterInternal(direction);
            }
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: private boolean restoreFocusInClusterInternal(@FocusRealDirection int direction)
        private bool restoreFocusInClusterInternal(int direction)
        {
            if (mFocusedInCluster != null && DescendantFocusability != FOCUS_BLOCK_DESCENDANTS && (mFocusedInCluster.mViewFlags & VISIBILITY_MASK) == VISIBLE && mFocusedInCluster.restoreFocusInCluster(direction))
            {
                return true;
            }
            return base.restoreFocusInCluster(direction);
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override bool restoreFocusNotInCluster()
        {
            if (mFocusedInCluster != null)
            {
                // since clusters don't nest; we can assume that a non-null mFocusedInCluster
                // will refer to a view not-in a cluster.
                return restoreFocusInCluster(View.FOCUS_DOWN);
            }
            if (KeyboardNavigationCluster || (mViewFlags & VISIBILITY_MASK) != VISIBLE)
            {
                return false;
            }
            int descendentFocusability = DescendantFocusability;
            if (descendentFocusability == FOCUS_BLOCK_DESCENDANTS)
            {
                return base.requestFocus(FOCUS_DOWN, null);
            }
            if (descendentFocusability == FOCUS_BEFORE_DESCENDANTS && base.requestFocus(FOCUS_DOWN, null))
            {
                return true;
            }
            for (int i = 0; i < mChildrenCount; ++i)
            {
                View child = mChildren[i];
                if (!child.KeyboardNavigationCluster && child.restoreFocusNotInCluster())
                {
                    return true;
                }
            }
            if (descendentFocusability == FOCUS_AFTER_DESCENDANTS && !hasFocusableChild(false))
            {
                return base.requestFocus(FOCUS_DOWN, null);
            }
            return false;
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// @hide
        /// </summary>
        public override void dispatchStartTemporaryDetach()
        {
            base.dispatchStartTemporaryDetach();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchStartTemporaryDetach();
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// @hide
        /// </summary>
        public override void dispatchFinishTemporaryDetach()
        {
            base.dispatchFinishTemporaryDetach();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchFinishTemporaryDetach();
            }
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @UnsupportedAppUsage void dispatchAttachedToWindow(AttachInfo info, int visibility)
        internal override void dispatchAttachedToWindow(AttachInfo info, int visibility)
        {
            mGroupFlags |= FLAG_PREVENT_DISPATCH_ATTACHED_TO_WINDOW;
            base.dispatchAttachedToWindow(info, visibility);
            mGroupFlags &= ~FLAG_PREVENT_DISPATCH_ATTACHED_TO_WINDOW;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                child.dispatchAttachedToWindow(info, combineVisibility(visibility, child.Visibility));
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int transientCount = mTransientIndices == null ? 0 : mTransientIndices.size();
            int transientCount = mTransientIndices == null ? 0 : mTransientIndices.Count;
            for (int i = 0; i < transientCount; ++i)
            {
                View view = mTransientViews[i];
                view.dispatchAttachedToWindow(info, combineVisibility(visibility, view.Visibility));
            }
        }

        internal override void dispatchScreenStateChanged(int screenState)
        {
            base.dispatchScreenStateChanged(screenState);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchScreenStateChanged(screenState);
            }
        }

        internal override void dispatchMovedToDisplay(Display display, Configuration config)
        {
            base.dispatchMovedToDisplay(display, config);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchMovedToDisplay(display, config);
            }
        }

        /// <summary>
        /// @hide </summary>
        public override bool dispatchPopulateAccessibilityEventInternal(AccessibilityEvent @event)
        {
            bool handled = false;
            if (includeForAccessibility())
            {
                handled = base.dispatchPopulateAccessibilityEventInternal(@event);
                if (handled)
                {
                    return handled;
                }
            }
            // Let our children have a shot in populating the event.
            ChildListForAccessibility children = ChildListForAccessibility.obtain(this, true);
            try
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childCount = children.getChildCount();
                int childCount = children.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    View child = children.getChildAt(i);
                    if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE)
                    {
                        handled = child.dispatchPopulateAccessibilityEvent(@event);
                        if (handled)
                        {
                            return handled;
                        }
                    }
                }
            }
            finally
            {
                children.recycle();
            }
            return false;
        }

        /// <summary>
        /// Dispatch creation of <seealso cref="ViewStructure"/> down the hierarchy.  This implementation
        /// adds in all child views of the view group, in addition to calling the default View
        /// implementation.
        /// </summary>
        public override void dispatchProvideStructure(ViewStructure structure)
        {
            base.dispatchProvideStructure(structure);
            if (AssistBlocked || structure.ChildCount != 0)
            {
                return;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
            if (childrenCount <= 0)
            {
                return;
            }

            if (!LaidOut)
            {
                if (Helper.sVerbose)
                {
                    Log.v(VIEW_LOG_TAG, "dispatchProvideStructure(): not laid out, ignoring " + childrenCount + " children of " + AccessibilityViewId);
                }
                return;
            }

            structure.ChildCount = childrenCount;
            List<View> preorderedList = buildOrderedChildList();
            bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
            for (int i = 0; i < childrenCount; i++)
            {
                int childIndex;
                try
                {
                    childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                }
                catch (System.IndexOutOfRangeException e)
                {
                    childIndex = i;
                    if (mContext.ApplicationInfo.targetSdkVersion < Build.VERSION_CODES.M)
                    {
                        Log.w(TAG, "Bad getChildDrawingOrder while collecting assist @ " + i + " of " + childrenCount, e);
                        // At least one app is failing when we call getChildDrawingOrder
                        // at this point, so deal semi-gracefully with it by falling back
                        // on the basic order.
                        customOrder = false;
                        if (i > 0)
                        {
                            // If we failed at the first index, there really isn't
                            // anything to do -- we will just proceed with the simple
                            // sequence order.
                            // Otherwise, we failed in the middle, so need to come up
                            // with an order for the remaining indices and use that.
                            // Failed at the first one, easy peasy.
                            int[] permutation = new int[childrenCount];
                            SparseBooleanArray usedIndices = new SparseBooleanArray();
                            // Go back and collected the indices we have done so far.
                            for (int j = 0; j < i; j++)
                            {
                                permutation[j] = getChildDrawingOrder(childrenCount, j);
                                usedIndices.put(permutation[j], true);
                            }
                            // Fill in the remaining indices with indices that have not
                            // yet been used.
                            int nextIndex = 0;
                            for (int j = i; j < childrenCount; j++)
                            {
                                while (usedIndices.get(nextIndex, false))
                                {
                                    nextIndex++;
                                }
                                permutation[j] = nextIndex;
                                nextIndex++;
                            }
                            // Build the final view list.
                            preorderedList = new List<>(childrenCount);
                            for (int j = 0; j < childrenCount; j++)
                            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = permutation[j];
                                int index = permutation[j];
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = mChildren[index];
                                View child = mChildren[index];
                                preorderedList.Add(child);
                            }
                        }
                    }
                    else
                    {
                        throw e;
                    }
                }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, mChildren, childIndex);
                View child = getAndVerifyPreorderedView(preorderedList, mChildren, childIndex);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ViewStructure cstructure = structure.newChild(i);
                ViewStructure cstructure = structure.newChild(i);
                child.dispatchProvideStructure(cstructure);
            }
            if (preorderedList != null)
            {
                preorderedList.Clear();
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// <para>This implementation adds in all child views of the view group, in addition to calling the
        /// default <seealso cref="View"/> implementation.
        /// </para>
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override public void dispatchProvideAutofillStructure(ViewStructure structure, @AutofillFlags int flags)
        public override void dispatchProvideAutofillStructure(ViewStructure structure, int flags)
        {
            base.dispatchProvideAutofillStructure(structure, flags);

            if (structure.ChildCount != 0)
            {
                return;
            }

            if (!LaidOut)
            {
                if (Helper.sVerbose)
                {
                    Log.v(VIEW_LOG_TAG, "dispatchProvideAutofillStructure(): not laid out, ignoring " + mChildrenCount + " children of " + AutofillId);
                }
                return;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ChildListForAutoFillOrContentCapture children = getChildrenForAutofill(flags);
            ChildListForAutoFillOrContentCapture children = getChildrenForAutofill(flags);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = children.size();
            int childrenCount = children.Count;
            structure.ChildCount = childrenCount;
            for (int i = 0; i < childrenCount; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children.get(i);
                View child = children[i];
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ViewStructure cstructure = structure.newChild(i);
                ViewStructure cstructure = structure.newChild(i);
                child.dispatchProvideAutofillStructure(cstructure, flags);
            }
            children.recycle();
        }

        /// <summary>
        /// @hide </summary>
        public override void dispatchProvideContentCaptureStructure()
        {
            base.dispatchProvideContentCaptureStructure();

            if (!LaidOut)
            {
                return;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ChildListForAutoFillOrContentCapture children = getChildrenForContentCapture();
            ChildListForAutoFillOrContentCapture children = ChildrenForContentCapture;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = children.size();
            int childrenCount = children.Count;
            for (int i = 0; i < childrenCount; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children.get(i);
                View child = children[i];
                child.dispatchProvideContentCaptureStructure();
            }
            children.recycle();
        }

        /// <summary>
        /// Gets the children for autofill. Children for autofill are the first
        /// level descendants that are important for autofill. The returned
        /// child list object is pooled and the caller must recycle it once done.
        /// @hide 
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: private @NonNull ChildListForAutoFillOrContentCapture getChildrenForAutofill(@AutofillFlags int flags)
        private ChildListForAutoFillOrContentCapture getChildrenForAutofill(int flags)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ChildListForAutoFillOrContentCapture children = ChildListForAutoFillOrContentCapture.obtain();
            ChildListForAutoFillOrContentCapture children = ChildListForAutoFillOrContentCapture.obtain();
            populateChildrenForAutofill(children, flags);
            return children;
        }

        /// <summary>
        /// @hide </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: private void populateChildrenForAutofill(java.util.ArrayList<View> list, @AutofillFlags int flags)
        private void populateChildrenForAutofill(List<View> list, int flags)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
            if (childrenCount <= 0)
            {
                return;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildOrderedChildList();
            List<View> preorderedList = buildOrderedChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
            bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
            for (int i = 0; i < childrenCount; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = (preorderedList == null) ? mChildren[childIndex] : preorderedList.get(childIndex);
                View child = (preorderedList == null) ? mChildren[childIndex] : preorderedList[childIndex];
                if ((flags & AUTOFILL_FLAG_INCLUDE_NOT_IMPORTANT_VIEWS) != 0 || child.ImportantForAutofill)
                {
                    list.Add(child);
                }
                else if (child is ViewGroup)
                {
                    ((ViewGroup) child).populateChildrenForAutofill(list, flags);
                }
            }
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: private @NonNull ChildListForAutoFillOrContentCapture getChildrenForContentCapture()
        private ChildListForAutoFillOrContentCapture ChildrenForContentCapture
        {
            get
            {
    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final ChildListForAutoFillOrContentCapture children = ChildListForAutoFillOrContentCapture.obtain();
                ChildListForAutoFillOrContentCapture children = ChildListForAutoFillOrContentCapture.obtain();
                populateChildrenForContentCapture(children);
                return children;
            }
        }

        /// <summary>
        /// @hide </summary>
        private void populateChildrenForContentCapture(List<View> list)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
            if (childrenCount <= 0)
            {
                return;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildOrderedChildList();
            List<View> preorderedList = buildOrderedChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
            bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
            for (int i = 0; i < childrenCount; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = (preorderedList == null) ? mChildren[childIndex] : preorderedList.get(childIndex);
                View child = (preorderedList == null) ? mChildren[childIndex] : preorderedList[childIndex];
                if (child.ImportantForContentCapture)
                {
                    list.Add(child);
                }
                else if (child is ViewGroup)
                {
                    ((ViewGroup) child).populateChildrenForContentCapture(list);
                }
            }
        }

        private static View getAndVerifyPreorderedView(List<View> preorderedList, View[] children, int childIndex)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child;
            View child;
            if (preorderedList != null)
            {
                child = preorderedList[childIndex];
                if (child == null)
                {
                    throw new Exception("Invalid preorderedList contained null child at index " + childIndex);
                }
            }
            else
            {
                child = children[childIndex];
            }
            return child;
        }

        /// <summary>
        /// @hide </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @UnsupportedAppUsage public void onInitializeAccessibilityNodeInfoInternal(android.view.accessibility.AccessibilityNodeInfo info)
        public override void onInitializeAccessibilityNodeInfoInternal(AccessibilityNodeInfo info)
        {
            base.onInitializeAccessibilityNodeInfoInternal(info);
            if (AccessibilityNodeProvider != null)
            {
                return;
            }
            if (mAttachInfo != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> childrenForAccessibility = mAttachInfo.mTempArrayList;
                List<View> childrenForAccessibility = mAttachInfo.mTempArrayList;
                childrenForAccessibility.Clear();
                addChildrenForAccessibility(childrenForAccessibility);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenForAccessibilityCount = childrenForAccessibility.size();
                int childrenForAccessibilityCount = childrenForAccessibility.Count;
                for (int i = 0; i < childrenForAccessibilityCount; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = childrenForAccessibility.get(i);
                    View child = childrenForAccessibility[i];
                    info.addChildUnchecked(child);
                }
                childrenForAccessibility.Clear();
            }
            info.AvailableExtraData = Collections.singletonList(AccessibilityNodeInfo.EXTRA_DATA_RENDERING_INFO_KEY);
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// <param name="info"> The info to which to add the extra data. Never {@code null}. </param>
        /// <param name="extraDataKey"> A key specifying the type of extra data to add to the info. The
        ///                     extra data should be added to the <seealso cref="Bundle"/> returned by
        ///                     the info's <seealso cref="AccessibilityNodeInfo#getExtras"/> method. Never
        ///                     {@code null}. </param>
        /// <param name="arguments"> A <seealso cref="Bundle"/> holding any arguments relevant for this request. May be
        ///                  {@code null} if the service provided no arguments.
        ///  </param>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override public void addExtraDataToAccessibilityNodeInfo(@NonNull android.view.accessibility.AccessibilityNodeInfo info, @NonNull String extraDataKey, @Nullable android.os.Bundle arguments)
        public override void addExtraDataToAccessibilityNodeInfo(AccessibilityNodeInfo info, string extraDataKey, Bundle arguments)
        {
            if (extraDataKey.Equals(AccessibilityNodeInfo.EXTRA_DATA_RENDERING_INFO_KEY))
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.accessibility.AccessibilityNodeInfo.ExtraRenderingInfo extraRenderingInfo = android.view.accessibility.AccessibilityNodeInfo.ExtraRenderingInfo.obtain();
                AccessibilityNodeInfo.ExtraRenderingInfo extraRenderingInfo = AccessibilityNodeInfo.ExtraRenderingInfo.obtain();
                extraRenderingInfo.setLayoutSize(LayoutParams.width, LayoutParams.height);
                info.ExtraRenderingInfo = extraRenderingInfo;
            }
        }

        public override CharSequence AccessibilityClassName
        {
            get
            {
    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                return typeof(ViewGroup).FullName;
            }
        }

        public override void notifySubtreeAccessibilityStateChanged(View child, View source, int changeType)
        {
            // If this is a live region, we should send a subtree change event
            // from this view. Otherwise, we can let it propagate up.
            if (AccessibilityLiveRegion != ACCESSIBILITY_LIVE_REGION_NONE)
            {
                notifyViewAccessibilityStateChangedIfNeeded(AccessibilityEvent.CONTENT_CHANGE_TYPE_SUBTREE);
            }
            else if (mParent != null)
            {
                try
                {
                    mParent.notifySubtreeAccessibilityStateChanged(this, source, changeType);
                }
                catch (AbstractMethodError e)
                {
                    Log.e(VIEW_LOG_TAG, mParent.GetType().Name + " does not fully implement ViewParent", e);
                }
            }
        }

        /// <summary>
        /// @hide </summary>
        public override void notifySubtreeAccessibilityStateChangedIfNeeded()
        {
            if (!AccessibilityManager.getInstance(mContext).Enabled || mAttachInfo == null)
            {
                return;
            }
            // If something important for a11y is happening in this subtree, make sure it's dispatched
            // from a view that is important for a11y so it doesn't get lost.
            if ((ImportantForAccessibility != IMPORTANT_FOR_ACCESSIBILITY_NO_HIDE_DESCENDANTS) && !ImportantForAccessibility && (ChildCount > 0))
            {
                ViewParent a11yParent = ParentForAccessibility;
                if (a11yParent is View)
                {
                    ((View) a11yParent).notifySubtreeAccessibilityStateChangedIfNeeded();
                    return;
                }
            }
            base.notifySubtreeAccessibilityStateChangedIfNeeded();
        }

        internal override void resetSubtreeAccessibilityStateChanged()
        {
            base.resetSubtreeAccessibilityStateChanged();
            View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childCount = mChildrenCount;
            int childCount = mChildrenCount;
            for (int i = 0; i < childCount; i++)
            {
                children[i].resetSubtreeAccessibilityStateChanged();
            }
        }

        /// <summary>
        /// Counts the number of children of this View that will be sent to an accessibility service.
        /// </summary>
        /// <returns> The number of children an {@code AccessibilityNodeInfo} rooted at this View
        /// would have. </returns>
        internal virtual int NumChildrenForAccessibility
        {
            get
            {
                int numChildrenForAccessibility = 0;
                for (int i = 0; i < ChildCount; i++)
                {
                    View child = getChildAt(i);
                    if (child.includeForAccessibility())
                    {
                        numChildrenForAccessibility++;
                    }
                    else if (child is ViewGroup)
                    {
                        numChildrenForAccessibility += ((ViewGroup) child).NumChildrenForAccessibility;
                    }
                }
                return numChildrenForAccessibility;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// <para>Subclasses should always call <code>super.onNestedPrePerformAccessibilityAction</code></para>
        /// </summary>
        /// <param name="target"> The target view dispatching this action </param>
        /// <param name="action"> Action being performed; see
        ///               <seealso cref="android.view.accessibility.AccessibilityNodeInfo"/> </param>
        /// <param name="args"> Optional action arguments </param>
        /// <returns> false by default. Subclasses should return true if they handle the event. </returns>
        public override bool onNestedPrePerformAccessibilityAction(View target, int action, Bundle args)
        {
            return false;
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @UnsupportedAppUsage void dispatchDetachedFromWindow()
        internal override void dispatchDetachedFromWindow()
        {
            // If we still have a touch target, we are still in the process of
            // dispatching motion events to a child; we need to get rid of that
            // child to avoid dispatching events to it after the window is torn
            // down. To make sure we keep the child in a consistent state, we
            // first send it an ACTION_CANCEL motion event.
            cancelAndClearTouchTargets(null);

            // Similarly, set ACTION_EXIT to all hover targets and clear them.
            exitHoverTargets();
            exitTooltipHoverTargets();

            // In case view is detached while transition is running
            mLayoutCalledWhileSuppressed = false;

            // Tear down our drag tracking
            mChildrenInterestedInDrag = null;
            mIsInterestedInDrag = false;
            if (mCurrentDragStartEvent != null)
            {
                mCurrentDragStartEvent.recycle();
                mCurrentDragStartEvent = null;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchDetachedFromWindow();
            }
            clearDisappearingChildren();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int transientCount = mTransientViews == null ? 0 : mTransientIndices.size();
            int transientCount = mTransientViews == null ? 0 : mTransientIndices.Count;
            for (int i = 0; i < transientCount; ++i)
            {
                View view = mTransientViews[i];
                view.dispatchDetachedFromWindow();
            }
            base.dispatchDetachedFromWindow();
        }

        /// <summary>
        /// @hide
        /// </summary>
        protected internal override void internalSetPadding(int left, int top, int right, int bottom)
        {
            base.internalSetPadding(left, top, right, bottom);

            if ((mPaddingLeft | mPaddingTop | mPaddingRight | mPaddingBottom) != 0)
            {
                mGroupFlags |= FLAG_PADDING_NOT_NULL;
            }
            else
            {
                mGroupFlags &= ~FLAG_PADDING_NOT_NULL;
            }
        }

        protected internal override void dispatchSaveInstanceState(SparseArray<Parcelable> container)
        {
            base.dispatchSaveInstanceState(container);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                View c = children[i];
                if ((c.mViewFlags & PARENT_SAVE_DISABLED_MASK) != PARENT_SAVE_DISABLED)
                {
                    c.dispatchSaveInstanceState(container);
                }
            }
        }

        /// <summary>
        /// Perform dispatching of a <seealso cref="#saveHierarchyState(android.util.SparseArray)"/>  freeze()}
        /// to only this view, not to its children.  For use when overriding
        /// <seealso cref="#dispatchSaveInstanceState(android.util.SparseArray)"/>  dispatchFreeze()} to allow
        /// subclasses to freeze their own state but not the state of their children.
        /// </summary>
        /// <param name="container"> the container </param>
        protected internal virtual void dispatchFreezeSelfOnly(SparseArray<Parcelable> container)
        {
            base.dispatchSaveInstanceState(container);
        }

        protected internal override void dispatchRestoreInstanceState(SparseArray<Parcelable> container)
        {
            base.dispatchRestoreInstanceState(container);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                View c = children[i];
                if ((c.mViewFlags & PARENT_SAVE_DISABLED_MASK) != PARENT_SAVE_DISABLED)
                {
                    c.dispatchRestoreInstanceState(container);
                }
            }
        }

        /// <summary>
        /// Perform dispatching of a <seealso cref="#restoreHierarchyState(android.util.SparseArray)"/>
        /// to only this view, not to its children.  For use when overriding
        /// <seealso cref="#dispatchRestoreInstanceState(android.util.SparseArray)"/> to allow
        /// subclasses to thaw their own state but not the state of their children.
        /// </summary>
        /// <param name="container"> the container </param>
        protected internal virtual void dispatchThawSelfOnly(SparseArray<Parcelable> container)
        {
            base.dispatchRestoreInstanceState(container);
        }

        /// <summary>
        /// Enables or disables the drawing cache for each child of this view group.
        /// </summary>
        /// <param name="enabled"> true to enable the cache, false to dispose of it
        /// </param>
        /// @deprecated The view drawing cache was largely made obsolete with the introduction of
        /// hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
        /// layers are largely unnecessary and can easily result in a net loss in performance due to the
        /// cost of creating and updating the layer. In the rare cases where caching layers are useful,
        /// such as for alpha animations, <seealso cref="#setLayerType(int, Paint)"/> handles this with hardware
        /// rendering. For software-rendered snapshots of a small part of the View hierarchy or
        /// individual Views it is recommended to create a <seealso cref="Canvas"/> from either a <seealso cref="Bitmap"/> or
        /// <seealso cref="android.graphics.Picture"/> and call <seealso cref="#draw(Canvas)"/> on the View. However these
        /// software-rendered usages are discouraged and have compatibility issues with hardware-only
        /// rendering features such as <seealso cref="android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE"/>
        /// bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
        /// reports or unit testing the <seealso cref="PixelCopy"/> API is recommended. 
        [Obsolete("The view drawing cache was largely made obsolete with the introduction of")]
        protected internal virtual bool ChildrenDrawingCacheEnabled
        {
            set
            {
                if (value || (mPersistentDrawingCache & PERSISTENT_ALL_CACHES) != PERSISTENT_ALL_CACHES)
                {
    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final View[] children = mChildren;
                    View[] children = mChildren;
    //JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
    //ORIGINAL LINE: final int count = mChildrenCount;
                    int count = mChildrenCount;
                    for (int i = 0; i < count; i++)
                    {
                        children[i].DrawingCacheEnabled = value;
                    }
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override Bitmap createSnapshot(ViewDebug.CanvasProvider canvasProvider, bool skipChildren)
        {
            int count = mChildrenCount;
            int[] visibilities = null;

            if (skipChildren)
            {
                visibilities = new int[count];
                for (int i = 0; i < count; i++)
                {
                    View child = getChildAt(i);
                    visibilities[i] = child.Visibility;
                    if (visibilities[i] == View.VISIBLE)
                    {
                        child.mViewFlags = (child.mViewFlags & ~View.VISIBILITY_MASK) | (View.INVISIBLE & View.VISIBILITY_MASK);
                    }
                }
            }

            try
            {
                return base.createSnapshot(canvasProvider, skipChildren);
            }
            finally
            {
                if (skipChildren)
                {
                    for (int i = 0; i < count; i++)
                    {
                        View child = getChildAt(i);
                        child.mViewFlags = (child.mViewFlags & ~View.VISIBILITY_MASK) | (visibilities[i] & View.VISIBILITY_MASK);
                    }
                }
            }
        }

        /// <summary>
        /// Return true if this ViewGroup is laying out using optical bounds. </summary>
        internal virtual bool LayoutModeOptical
        {
            get
            {
                return mLayoutMode == LAYOUT_MODE_OPTICAL_BOUNDS;
            }
        }

        internal override Insets computeOpticalInsets()
        {
            if (LayoutModeOptical)
            {
                int left = 0;
                int top = 0;
                int right = 0;
                int bottom = 0;
                for (int i = 0; i < mChildrenCount; i++)
                {
                    View child = getChildAt(i);
                    if (child.Visibility == VISIBLE)
                    {
                        Insets insets = child.OpticalInsets;
                        left = Math.Max(left, insets.left);
                        top = Math.Max(top, insets.top);
                        right = Math.Max(right, insets.right);
                        bottom = Math.Max(bottom, insets.bottom);
                    }
                }
                return Insets.of(left, top, right, bottom);
            }
            else
            {
                return Insets.NONE;
            }
        }

        private static void fillRect(Canvas canvas, Paint paint, int x1, int y1, int x2, int y2)
        {
            if (x1 != x2 && y1 != y2)
            {
                if (x1 > x2)
                {
                    int tmp = x1;
                    x1 = x2;
                    x2 = tmp;
                }
                if (y1 > y2)
                {
                    int tmp = y1;
                    y1 = y2;
                    y2 = tmp;
                }
                canvas.drawRect(x1, y1, x2, y2, paint);
            }
        }

        private static int sign(int x)
        {
            return (x >= 0) ? 1 : -1;
        }

        private static void drawCorner(Canvas c, Paint paint, int x1, int y1, int dx, int dy, int lw)
        {
            fillRect(c, paint, x1, y1, x1 + dx, y1 + lw * sign(dy));
            fillRect(c, paint, x1, y1, x1 + lw * sign(dx), y1 + dy);
        }

        private static void drawRectCorners(Canvas canvas, int x1, int y1, int x2, int y2, Paint paint, int lineLength, int lineWidth)
        {
            drawCorner(canvas, paint, x1, y1, lineLength, lineLength, lineWidth);
            drawCorner(canvas, paint, x1, y2, lineLength, -lineLength, lineWidth);
            drawCorner(canvas, paint, x2, y1, -lineLength, lineLength, lineWidth);
            drawCorner(canvas, paint, x2, y2, -lineLength, -lineLength, lineWidth);
        }

        private static void fillDifference(Canvas canvas, int x2, int y2, int x3, int y3, int dx1, int dy1, int dx2, int dy2, Paint paint)
        {
            int x1 = x2 - dx1;
            int y1 = y2 - dy1;

            int x4 = x3 + dx2;
            int y4 = y3 + dy2;

            fillRect(canvas, paint, x1, y1, x4, y2);
            fillRect(canvas, paint, x1, y2, x2, y3);
            fillRect(canvas, paint, x3, y2, x4, y3);
            fillRect(canvas, paint, x1, y3, x4, y4);
        }

        /// <summary>
        /// @hide
        /// </summary>
        protected internal virtual void onDebugDrawMargins(Canvas canvas, Paint paint)
        {
            for (int i = 0; i < ChildCount; i++)
            {
                View c = getChildAt(i);
                c.LayoutParams.onDebugDraw(c, canvas, paint);
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
        protected internal virtual void onDebugDraw(Canvas canvas)
        {
            Paint paint = DebugPaint;

            {
            // Draw optical bounds
                paint.Color = Color.RED;
                paint.Style = Paint.Style.STROKE;

                for (int i = 0; i < ChildCount; i++)
                {
                    View c = getChildAt(i);
                    if (c.Visibility != View.GONE)
                    {
                        Insets insets = c.OpticalInsets;

                        drawRect(canvas, paint, c.Left + insets.left, c.Top + insets.top, c.Right - insets.right - 1, c.Bottom - insets.bottom - 1);
                    }
                }
            }

            {
            // Draw margins
                paint.Color = Color.argb(63, 255, 0, 255);
                paint.Style = Paint.Style.FILL;

                onDebugDrawMargins(canvas, paint);
            }

            {
            // Draw clip bounds
                paint.Color = DEBUG_CORNERS_COLOR;
                paint.Style = Paint.Style.FILL;

                int lineLength = dipsToPixels(DEBUG_CORNERS_SIZE_DIP);
                int lineWidth = dipsToPixels(1);
                for (int i = 0; i < ChildCount; i++)
                {
                    View c = getChildAt(i);
                    if (c.Visibility != View.GONE)
                    {
                        drawRectCorners(canvas, c.Left, c.Top, c.Right, c.Bottom, paint, lineLength, lineWidth);
                    }
                }
            }
        }

        protected internal override void dispatchDraw(Canvas canvas)
        {
            bool usingRenderNodeProperties = canvas.isRecordingFor(mRenderNode);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            int flags = mGroupFlags;

            if ((flags & FLAG_RUN_ANIMATION) != 0 && canAnimate())
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean buildCache = !isHardwareAccelerated();
                bool buildCache = !HardwareAccelerated;
                for (int i = 0; i < childrenCount; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                    View child = children[i];
                    if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE)
                    {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LayoutParams params = child.getLayoutParams();
                        LayoutParams @params = child.LayoutParams;
                        attachLayoutAnimationParameters(child, @params, i, childrenCount);
                        bindLayoutAnimation(child);
                    }
                }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.view.animation.LayoutAnimationController controller = mLayoutAnimationController;
                LayoutAnimationController controller = mLayoutAnimationController;
                if (controller.willOverlap())
                {
                    mGroupFlags |= FLAG_OPTIMIZE_INVALIDATE;
                }

                controller.start();

                mGroupFlags &= ~FLAG_RUN_ANIMATION;
                mGroupFlags &= ~FLAG_ANIMATION_DONE;

                if (mAnimationListener != null)
                {
                    mAnimationListener.onAnimationStart(controller.Animation);
                }
            }

            int clipSaveCount = 0;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean clipToPadding = (flags & CLIP_TO_PADDING_MASK) == CLIP_TO_PADDING_MASK;
            bool clipToPadding = (flags & CLIP_TO_PADDING_MASK) == CLIP_TO_PADDING_MASK;
            if (clipToPadding)
            {
                clipSaveCount = canvas.save(Canvas.CLIP_SAVE_FLAG);
                canvas.clipRect(mScrollX + mPaddingLeft, mScrollY + mPaddingTop, mScrollX + mRight - mLeft - mPaddingRight, mScrollY + mBottom - mTop - mPaddingBottom);
            }

            // We will draw our child's animation, let's reset the flag
            mPrivateFlags &= ~PFLAG_DRAW_ANIMATION;
            mGroupFlags &= ~FLAG_INVALIDATE_REQUIRED;

            bool more = false;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long drawingTime = getDrawingTime();
            long drawingTime = DrawingTime;

            if (usingRenderNodeProperties)
            {
                canvas.insertReorderBarrier();
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int transientCount = mTransientIndices == null ? 0 : mTransientIndices.size();
            int transientCount = mTransientIndices == null ? 0 : mTransientIndices.Count;
            int transientIndex = transientCount != 0 ? 0 : -1;
            // Only use the preordered list if not HW accelerated, since the HW pipeline will do the
            // draw reordering internally
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = usingRenderNodeProperties ? null : buildOrderedChildList();
            List<View> preorderedList = usingRenderNodeProperties ? null : buildOrderedChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
            bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
            for (int i = 0; i < childrenCount; i++)
            {
                while (transientIndex >= 0 && mTransientIndices[transientIndex] == i)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View transientChild = mTransientViews.get(transientIndex);
                    View transientChild = mTransientViews[transientIndex];
                    if ((transientChild.mViewFlags & VISIBILITY_MASK) == VISIBLE || transientChild.Animation != null)
                    {
                        more |= drawChild(canvas, transientChild, drawingTime);
                    }
                    transientIndex++;
                    if (transientIndex >= transientCount)
                    {
                        transientIndex = -1;
                    }
                }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE || child.Animation != null)
                {
                    more |= drawChild(canvas, child, drawingTime);
                }
            }
            while (transientIndex >= 0)
            {
                // there may be additional transient views after the normal views
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View transientChild = mTransientViews.get(transientIndex);
                View transientChild = mTransientViews[transientIndex];
                if ((transientChild.mViewFlags & VISIBILITY_MASK) == VISIBLE || transientChild.Animation != null)
                {
                    more |= drawChild(canvas, transientChild, drawingTime);
                }
                transientIndex++;
                if (transientIndex >= transientCount)
                {
                    break;
                }
            }
            if (preorderedList != null)
            {
                preorderedList.Clear();
            }

            // Draw any disappearing views that have animations
            if (mDisappearingChildren != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> disappearingChildren = mDisappearingChildren;
                List<View> disappearingChildren = mDisappearingChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int disappearingCount = disappearingChildren.size() - 1;
                int disappearingCount = disappearingChildren.Count - 1;
                // Go backwards -- we may delete as animations finish
                for (int i = disappearingCount; i >= 0; i--)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = disappearingChildren.get(i);
                    View child = disappearingChildren[i];
                    more |= drawChild(canvas, child, drawingTime);
                }
            }
            if (usingRenderNodeProperties)
            {
                canvas.insertInorderBarrier();
            }

            if (ShowingLayoutBounds)
            {
                onDebugDraw(canvas);
            }

            if (clipToPadding)
            {
                canvas.restoreToCount(clipSaveCount);
            }

            // mGroupFlags might have been updated by drawChild()
            flags = mGroupFlags;

            if ((flags & FLAG_INVALIDATE_REQUIRED) == FLAG_INVALIDATE_REQUIRED)
            {
                invalidate(true);
            }

            if ((flags & FLAG_ANIMATION_DONE) == 0 && (flags & FLAG_NOTIFY_ANIMATION_LISTENER) == 0 && mLayoutAnimationController.Done && !more)
            {
                // We want to erase the drawing cache and notify the listener after the
                // next frame is drawn because one extra invalidate() is caused by
                // drawChild() after the animation is over
                mGroupFlags |= FLAG_NOTIFY_ANIMATION_LISTENER;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Runnable end = new Runnable()
                ThreadStart end = () =>
            {
               notifyAnimationListener();
            };
                post(end);
            }
        }

        /// <summary>
        /// Returns the ViewGroupOverlay for this view group, creating it if it does
        /// not yet exist. In addition to <seealso cref="ViewOverlay"/>'s support for drawables,
        /// <seealso cref="ViewGroupOverlay"/> allows views to be added to the overlay. These
        /// views, like overlay drawables, are visual-only; they do not receive input
        /// events and should not be used as anything other than a temporary
        /// representation of a view in a parent container, such as might be used
        /// by an animation effect.
        /// 
        /// <para>Note: Overlays do not currently work correctly with {@link
        /// SurfaceView} or <seealso cref="TextureView"/>; contents in overlays for these
        /// types of views may not display correctly.</para>
        /// </summary>
        /// <returns> The ViewGroupOverlay object for this view. </returns>
        /// <seealso cref= ViewGroupOverlay </seealso>
        public override ViewGroupOverlay Overlay
        {
            get
            {
                if (mOverlay == null)
                {
                    mOverlay = new ViewGroupOverlay(mContext, this);
                }
                return (ViewGroupOverlay) mOverlay;
            }
        }

        /// <summary>
        /// Converts drawing order position to container position. Override this
        /// if you want to change the drawing order of children. By default, it
        /// returns drawingPosition.
        /// <para>
        /// NOTE: In order for this method to be called, you must enable child ordering
        /// first by calling <seealso cref="#setChildrenDrawingOrderEnabled(boolean)"/>.
        /// 
        /// </para>
        /// </summary>
        /// <param name="drawingPosition"> the drawing order position. </param>
        /// <returns> the container position of a child for this drawing order position.
        /// </returns>
        /// <seealso cref= #setChildrenDrawingOrderEnabled(boolean) </seealso>
        /// <seealso cref= #isChildrenDrawingOrderEnabled() </seealso>
        protected internal virtual int getChildDrawingOrder(int childCount, int drawingPosition)
        {
            return drawingPosition;
        }

        /// <summary>
        /// Converts drawing order position to container position.
        /// <para>
        /// Children are not necessarily drawn in the order in which they appear in the container.
        /// ViewGroups can enable a custom ordering via <seealso cref="#setChildrenDrawingOrderEnabled(boolean)"/>.
        /// This method returns the container position of a child that appears in the given position
        /// in the current drawing order.
        /// 
        /// </para>
        /// </summary>
        /// <param name="drawingPosition"> the drawing order position. </param>
        /// <returns> the container position of a child for this drawing order position.
        /// </returns>
        /// <seealso cref= #getChildDrawingOrder(int, int)} </seealso>
        public int getChildDrawingOrder(int drawingPosition)
        {
            return getChildDrawingOrder(ChildCount, drawingPosition);
        }

        private bool hasChildWithZ()
        {
            for (int i = 0; i < mChildrenCount; i++)
            {
                if (mChildren[i].Z != 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Populates (and returns) mPreSortedChildren with a pre-ordered list of the View's children,
        /// sorted first by Z, then by child drawing order (if applicable). This list must be cleared
        /// after use to avoid leaking child Views.
        /// 
        /// Uses a stable, insertion sort which is commonly O(n) for ViewGroups with very few elevated
        /// children.
        /// </summary>
        internal virtual List<View> buildOrderedChildList()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
            if (childrenCount <= 1 || !hasChildWithZ())
            {
                return null;
            }

            if (mPreSortedChildren == null)
            {
                mPreSortedChildren = new List<>(childrenCount);
            }
            else
            {
                // callers should clear, so clear shouldn't be necessary, but for safety...
                mPreSortedChildren.Clear();
                mPreSortedChildren.Capacity = childrenCount;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = isChildrenDrawingOrderEnabled();
            bool customOrder = ChildrenDrawingOrderEnabled;
            for (int i = 0; i < childrenCount; i++)
            {
                // add next child (in child order) to end of list
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View nextChild = mChildren[childIndex];
                View nextChild = mChildren[childIndex];
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float currentZ = nextChild.getZ();
                float currentZ = nextChild.Z;

                // insert ahead of any Views with greater Z
                int insertIndex = i;
                while (insertIndex > 0 && mPreSortedChildren[insertIndex - 1].Z > currentZ)
                {
                    insertIndex--;
                }
                mPreSortedChildren.Insert(insertIndex, nextChild);
            }
            return mPreSortedChildren;
        }

        private void notifyAnimationListener()
        {
            mGroupFlags &= ~FLAG_NOTIFY_ANIMATION_LISTENER;
            mGroupFlags |= FLAG_ANIMATION_DONE;

            if (mAnimationListener != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Runnable end = new Runnable()
               ThreadStart end = () =>
           {
               mAnimationListener.onAnimationEnd(mLayoutAnimationController.Animation);
           };
               post(end);
            }

            invalidate(true);
        }

        /// <summary>
        /// This method is used to cause children of this ViewGroup to restore or recreate their
        /// display lists. It is called by getDisplayList() when the parent ViewGroup does not need
        /// to recreate its own display list, which would happen if it went through the normal
        /// draw/dispatchDraw mechanisms.
        /// 
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @UnsupportedAppUsage protected void dispatchGetDisplayList()
        protected internal override void dispatchGetDisplayList()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                if (((child.mViewFlags & VISIBILITY_MASK) == VISIBLE || child.Animation != null))
                {
                    recreateChildDisplayList(child);
                }
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int transientCount = mTransientViews == null ? 0 : mTransientIndices.size();
            int transientCount = mTransientViews == null ? 0 : mTransientIndices.Count;
            for (int i = 0; i < transientCount; ++i)
            {
                View child = mTransientViews[i];
                if (((child.mViewFlags & VISIBILITY_MASK) == VISIBLE || child.Animation != null))
                {
                    recreateChildDisplayList(child);
                }
            }
            if (mOverlay != null)
            {
                View overlayView = mOverlay.OverlayView;
                recreateChildDisplayList(overlayView);
            }
            if (mDisappearingChildren != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> disappearingChildren = mDisappearingChildren;
                List<View> disappearingChildren = mDisappearingChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int disappearingCount = disappearingChildren.size();
                int disappearingCount = disappearingChildren.Count;
                for (int i = 0; i < disappearingCount; ++i)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = disappearingChildren.get(i);
                    View child = disappearingChildren[i];
                    recreateChildDisplayList(child);
                }
            }
        }

        private void recreateChildDisplayList(View child)
        {
            child.mRecreateDisplayList = (child.mPrivateFlags & PFLAG_INVALIDATED) != 0;
            child.mPrivateFlags &= ~PFLAG_INVALIDATED;
            child.updateDisplayListIfDirty();
            child.mRecreateDisplayList = false;
        }

        /// <summary>
        /// Draw one child of this View Group. This method is responsible for getting
        /// the canvas in the right state. This includes clipping, translating so
        /// that the child's scrolled origin is at 0, 0, and applying any animation
        /// transformations.
        /// </summary>
        /// <param name="canvas"> The canvas on which to draw the child </param>
        /// <param name="child"> Who to draw </param>
        /// <param name="drawingTime"> The time at which draw is occurring </param>
        /// <returns> True if an invalidate() was issued </returns>
        protected internal virtual bool drawChild(Canvas canvas, View child, long drawingTime)
        {
            return child.draw(canvas, this, drawingTime);
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override void getScrollIndicatorBounds(@NonNull android.graphics.Rect out)
        internal override void getScrollIndicatorBounds(Rect @out)
        {
            base.getScrollIndicatorBounds(@out);

            // If we have padding and we're supposed to clip children to that
            // padding, offset the scroll indicators to match our clip bounds.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean clipToPadding = (mGroupFlags & CLIP_TO_PADDING_MASK) == CLIP_TO_PADDING_MASK;
            bool clipToPadding = (mGroupFlags & CLIP_TO_PADDING_MASK) == CLIP_TO_PADDING_MASK;
            if (clipToPadding)
            {
                @out.left += mPaddingLeft;
                @out.right -= mPaddingRight;
                @out.top += mPaddingTop;
                @out.bottom -= mPaddingBottom;
            }
        }

        /// <summary>
        /// Returns whether this group's children are clipped to their bounds before drawing.
        /// The default value is true. </summary>
        /// <seealso cref= #setClipChildren(boolean)
        /// </seealso>
        /// <returns> True if the group's children will be clipped to their bounds,
        /// false otherwise. </returns>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "drawing") @InspectableProperty public boolean getClipChildren()
        public virtual bool ClipChildren
        {
            get
            {
                return ((mGroupFlags & FLAG_CLIP_CHILDREN) != 0);
            }
            set
            {
                bool previousValue = (mGroupFlags & FLAG_CLIP_CHILDREN) == FLAG_CLIP_CHILDREN;
                if (value != previousValue)
                {
                    setBooleanFlag(FLAG_CLIP_CHILDREN, value);
                    for (int i = 0; i < mChildrenCount; ++i)
                    {
                        View child = getChildAt(i);
                        if (child.mRenderNode != null)
                        {
                            child.mRenderNode.ClipToBounds = value;
                        }
                    }
                    invalidate(true);
                }
            }
        }


        /// <summary>
        /// Sets whether this ViewGroup will clip its children to its padding and resize (but not
        /// clip) any EdgeEffect to the padded region, if padding is present.
        /// <para>
        /// By default, children are clipped to the padding of their parent
        /// ViewGroup. This clipping behavior is only enabled if padding is non-zero.
        /// 
        /// </para>
        /// </summary>
        /// <param name="clipToPadding"> true to clip children to the padding of the group, and resize (but
        ///        not clip) any EdgeEffect to the padded region. False otherwise.
        /// @attr ref android.R.styleable#ViewGroup_clipToPadding </param>
        public virtual bool ClipToPadding
        {
            set
            {
                if (hasBooleanFlag(FLAG_CLIP_TO_PADDING) != value)
                {
                    setBooleanFlag(FLAG_CLIP_TO_PADDING, value);
                    invalidate(true);
                }
            }
            get
            {
                return hasBooleanFlag(FLAG_CLIP_TO_PADDING);
            }
        }


        public override void dispatchSetSelected(bool selected)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            for (int i = 0; i < count; i++)
            {
                children[i].Selected = selected;
            }
        }

        public override void dispatchSetActivated(bool activated)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            for (int i = 0; i < count; i++)
            {
                children[i].Activated = activated;
            }
        }

        protected internal override void dispatchSetPressed(bool pressed)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                // Children that are clickable on their own should not
                // show a pressed state when their parent view does.
                // Clearing a pressed state always propagates.
                if (!pressed || (!child.Clickable && !child.LongClickable))
                {
                    child.Pressed = pressed;
                }
            }
        }

        /// <summary>
        /// Dispatches drawable hotspot changes to child views that meet at least
        /// one of the following criteria:
        /// <ul>
        ///     <li>Returns {@code false} from both <seealso cref="View#isClickable()"/> and
        ///     <seealso cref="View#isLongClickable()"/></li>
        ///     <li>Requests duplication of parent state via
        ///     <seealso cref="View#setDuplicateParentStateEnabled(boolean)"/></li>
        /// </ul>
        /// </summary>
        /// <param name="x"> hotspot x coordinate </param>
        /// <param name="y"> hotspot y coordinate </param>
        /// <seealso cref= #drawableHotspotChanged(float, float) </seealso>
        public override void dispatchDrawableHotspotChanged(float x, float y)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            if (count == 0)
            {
                return;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                // Children that are clickable on their own should not
                // receive hotspots when their parent view does.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean nonActionable = !child.isClickable() && !child.isLongClickable();
                bool nonActionable = !child.Clickable && !child.LongClickable;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean duplicatesState = (child.mViewFlags & DUPLICATE_PARENT_STATE) != 0;
                bool duplicatesState = (child.mViewFlags & DUPLICATE_PARENT_STATE) != 0;
                if (nonActionable || duplicatesState)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final float[] point = getTempLocationF();
                    float[] point = TempLocationF;
                    point[0] = x;
                    point[1] = y;
                    transformPointToViewLocal(point, child);
                    child.drawableHotspotChanged(point[0], point[1]);
                }
            }
        }

        internal override void dispatchCancelPendingInputEvents()
        {
            base.dispatchCancelPendingInputEvents();

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchCancelPendingInputEvents();
            }
        }

        /// <summary>
        /// When this property is set to true, this ViewGroup supports static transformations on
        /// children; this causes
        /// <seealso cref="#getChildStaticTransformation(View, android.view.animation.Transformation)"/> to be
        /// invoked when a child is drawn.
        /// 
        /// Any subclass overriding
        /// <seealso cref="#getChildStaticTransformation(View, android.view.animation.Transformation)"/> should
        /// set this property to true.
        /// </summary>
        /// <param name="enabled"> True to enable static transformations on children, false otherwise.
        /// </param>
        /// <seealso cref= #getChildStaticTransformation(View, android.view.animation.Transformation) </seealso>
        protected internal virtual bool StaticTransformationsEnabled
        {
            set
            {
                setBooleanFlag(FLAG_SUPPORT_STATIC_TRANSFORMATIONS, value);
            }
        }

        /// <summary>
        /// Sets  <code>t</code> to be the static transformation of the child, if set, returning a
        /// boolean to indicate whether a static transform was set. The default implementation
        /// simply returns <code>false</code>; subclasses may override this method for different
        /// behavior. <seealso cref="#setStaticTransformationsEnabled(boolean)"/> must be set to true
        /// for this method to be called.
        /// </summary>
        /// <param name="child"> The child view whose static transform is being requested </param>
        /// <param name="t"> The Transformation which will hold the result </param>
        /// <returns> true if the transformation was set, false otherwise </returns>
        /// <seealso cref= #setStaticTransformationsEnabled(boolean) </seealso>
        protected internal virtual bool getChildStaticTransformation(View child, Transformation t)
        {
            return false;
        }

        internal virtual Transformation ChildTransformation
        {
            get
            {
                if (mChildTransformation == null)
                {
                    mChildTransformation = new Transformation();
                }
                return mChildTransformation;
            }
        }

        /// <summary>
        /// {@hide}
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override protected <T extends View> T findViewTraversal(@IdRes int id)
        protected internal override T findViewTraversal<T>(int id) where T : View
        {
            if (id == mID)
            {
                return (T) this;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] where = mChildren;
            View[] where = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = mChildrenCount;
            int len = mChildrenCount;

            for (int i = 0; i < len; i++)
            {
                View v = where[i];

                if ((v.mPrivateFlags & PFLAG_IS_ROOT_NAMESPACE) == 0)
                {
                    v = v.findViewById(id);

                    if (v != null)
                    {
                        return (T) v;
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// {@hide}
        /// </summary>
        protected internal override T findViewWithTagTraversal<T>(object tag) where T : View
        {
            if (tag != null && tag.Equals(mTag))
            {
                return (T) this;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] where = mChildren;
            View[] where = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = mChildrenCount;
            int len = mChildrenCount;

            for (int i = 0; i < len; i++)
            {
                View v = where[i];

                if ((v.mPrivateFlags & PFLAG_IS_ROOT_NAMESPACE) == 0)
                {
                    v = v.findViewWithTag(tag);

                    if (v != null)
                    {
                        return (T) v;
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// {@hide}
        /// </summary>
        protected internal override T findViewByPredicateTraversal<T>(System.Predicate<View> predicate, View childToSkip) where T : View
        {
            if (predicate(this))
            {
                return (T) this;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] where = mChildren;
            View[] where = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = mChildrenCount;
            int len = mChildrenCount;

            for (int i = 0; i < len; i++)
            {
                View v = where[i];

                if (v != childToSkip && (v.mPrivateFlags & PFLAG_IS_ROOT_NAMESPACE) == 0)
                {
                    v = v.findViewByPredicate(predicate);

                    if (v != null)
                    {
                        return (T) v;
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// This method adds a view to this container at the specified index purely for the
        /// purposes of allowing that view to draw even though it is not a normal child of
        /// the container. That is, the view does not participate in layout, focus, accessibility,
        /// input, or other normal view operations; it is purely an item to be drawn during the normal
        /// rendering operation of this container. The index that it is added at is the order
        /// in which it will be drawn, with respect to the other views in the container.
        /// For example, a transient view added at index 0 will be drawn before all other views
        /// in the container because it will be drawn first (including before any real view
        /// at index 0). There can be more than one transient view at any particular index;
        /// these views will be drawn in the order in which they were added to the list of
        /// transient views. The index of transient views can also be greater than the number
        /// of normal views in the container; that just means that they will be drawn after all
        /// other views are drawn.
        /// 
        /// <para>Note that since transient views do not participate in layout, they must be sized
        /// manually or, more typically, they should just use the size that they had before they
        /// were removed from their container.</para>
        /// 
        /// <para>Transient views are useful for handling animations of views that have been removed
        /// from the container, but which should be animated out after the removal. Adding these
        /// views as transient views allows them to participate in drawing without side-effecting
        /// the layout of the container.</para>
        /// 
        /// <para>Transient views must always be explicitly <seealso cref="#removeTransientView(View) removed"/>
        /// from the container when they are no longer needed. For example, a transient view
        /// which is added in order to fade it out in its old location should be removed
        /// once the animation is complete.</para>
        /// </summary>
        /// <param name="view"> The view to be added. The view must not have a parent. </param>
        /// <param name="index"> The index at which this view should be drawn, must be >= 0.
        /// This value is relative to the <seealso cref="#getChildAt(int) index"/> values in the normal
        /// child list of this container, where any transient view at a particular index will
        /// be drawn before any normal child at that same index.
        /// 
        /// @hide </param>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage public void addTransientView(View view, int index)
        public virtual void addTransientView(View view, int index)
        {
            if (index < 0 || view == null)
            {
                return;
            }
            if (view.mParent != null)
            {
                throw new System.InvalidOperationException("The specified view already has a parent " + view.mParent);
            }

            if (mTransientIndices == null)
            {
                mTransientIndices = new List<int?>();
                mTransientViews = new List<View>();
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldSize = mTransientIndices.size();
            int oldSize = mTransientIndices.Count;
            if (oldSize > 0)
            {
                int insertionIndex;
                for (insertionIndex = 0; insertionIndex < oldSize; ++insertionIndex)
                {
                    if (index < mTransientIndices[insertionIndex])
                    {
                        break;
                    }
                }
                mTransientIndices.Insert(insertionIndex, index);
                mTransientViews.Insert(insertionIndex, view);
            }
            else
            {
                mTransientIndices.Add(index);
                mTransientViews.Add(view);
            }
            view.mParent = this;
            if (mAttachInfo != null)
            {
                view.dispatchAttachedToWindow(mAttachInfo, (mViewFlags & VISIBILITY_MASK));
            }
            invalidate(true);
        }

        /// <summary>
        /// Removes a view from the list of transient views in this container. If there is no
        /// such transient view, this method does nothing.
        /// </summary>
        /// <param name="view"> The transient view to be removed
        /// 
        /// @hide </param>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage public void removeTransientView(View view)
        public virtual void removeTransientView(View view)
        {
            if (mTransientViews == null)
            {
                return;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = mTransientViews.size();
            int size = mTransientViews.Count;
            for (int i = 0; i < size; ++i)
            {
                if (view == mTransientViews[i])
                {
                    mTransientViews.RemoveAt(i);
                    mTransientIndices.RemoveAt(i);
                    view.mParent = null;
                    if (view.mAttachInfo != null)
                    {
                        view.dispatchDetachedFromWindow();
                    }
                    invalidate(true);
                    return;
                }
            }
        }

        /// <summary>
        /// Returns the number of transient views in this container. Specific transient
        /// views and the index at which they were added can be retrieved via
        /// <seealso cref="#getTransientView(int)"/> and <seealso cref="#getTransientViewIndex(int)"/>.
        /// </summary>
        /// <seealso cref= #addTransientView(View, int) </seealso>
        /// <returns> The number of transient views in this container
        /// 
        /// @hide </returns>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage public int getTransientViewCount()
        public virtual int TransientViewCount
        {
            get
            {
                return mTransientIndices == null ? 0 : mTransientIndices.Count;
            }
        }

        /// <summary>
        /// Given a valid position within the list of transient views, returns the index of
        /// the transient view at that position.
        /// </summary>
        /// <param name="position"> The position of the index being queried. Must be at least 0
        /// and less than the value returned by <seealso cref="#getTransientViewCount()"/>. </param>
        /// <returns> The index of the transient view stored in the given position if the
        /// position is valid, otherwise -1
        /// 
        /// @hide </returns>
        public virtual int getTransientViewIndex(int position)
        {
            if (position < 0 || mTransientIndices == null || position >= mTransientIndices.Count)
            {
                return -1;
            }
            return mTransientIndices[position];
        }

        /// <summary>
        /// Given a valid position within the list of transient views, returns the
        /// transient view at that position.
        /// </summary>
        /// <param name="position"> The position of the view being queried. Must be at least 0
        /// and less than the value returned by <seealso cref="#getTransientViewCount()"/>. </param>
        /// <returns> The transient view stored in the given position if the
        /// position is valid, otherwise null
        /// 
        /// @hide </returns>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage public View getTransientView(int position)
        public virtual View getTransientView(int position)
        {
            if (mTransientViews == null || position >= mTransientViews.Count)
            {
                return null;
            }
            return mTransientViews[position];
        }

        /// <summary>
        /// <para>Adds a child view. If no layout parameters are already set on the child, the
        /// default parameters for this ViewGroup are set on the child.</para>
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="child"> the child view to add
        /// </param>
        /// <seealso cref= #generateDefaultLayoutParams() </seealso>
        public virtual void addView(View child)
        {
            addView(child, -1);
        }

        /// <summary>
        /// Adds a child view. If no layout parameters are already set on the child, the
        /// default parameters for this ViewGroup are set on the child.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="child"> the child view to add </param>
        /// <param name="index"> the position at which to add the child
        /// </param>
        /// <seealso cref= #generateDefaultLayoutParams() </seealso>
        public virtual void addView(View child, int index)
        {
            if (child == null)
            {
                throw new System.ArgumentException("Cannot add a null child view to a ViewGroup");
            }
            LayoutParams @params = child.LayoutParams;
            if (@params == null)
            {
                @params = generateDefaultLayoutParams();
                if (@params == null)
                {
                    throw new System.ArgumentException("generateDefaultLayoutParams() cannot return null");
                }
            }
            addView(child, index, @params);
        }

        /// <summary>
        /// Adds a child view with this ViewGroup's default layout parameters and the
        /// specified width and height.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="child"> the child view to add </param>
        public virtual void addView(View child, int width, int height)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LayoutParams params = generateDefaultLayoutParams();
            LayoutParams @params = generateDefaultLayoutParams();
            @params.width = width;
            @params.height = height;
            addView(child, -1, @params);
        }

        /// <summary>
        /// Adds a child view with the specified layout parameters.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="child"> the child view to add </param>
        /// <param name="params"> the layout parameters to set on the child </param>
        public override void addView(View child, LayoutParams @params)
        {
            addView(child, -1, @params);
        }

        /// <summary>
        /// Adds a child view with the specified layout parameters.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="child"> the child view to add </param>
        /// <param name="index"> the position at which to add the child or -1 to add last </param>
        /// <param name="params"> the layout parameters to set on the child </param>
        public virtual void addView(View child, int index, LayoutParams @params)
        {
            if (DBG)
            {
                Console.WriteLine(this + " addView");
            }

            if (child == null)
            {
                throw new System.ArgumentException("Cannot add a null child view to a ViewGroup");
            }

            // addViewInner() will call child.requestLayout() when setting the new LayoutParams
            // therefore, we call requestLayout() on ourselves before, so that the child's request
            // will be blocked at our level
            requestLayout();
            invalidate(true);
            addViewInner(child, index, @params, false);
        }

        public override void updateViewLayout(View view, ViewGroup.LayoutParams @params)
        {
            if (!checkLayoutParams(@params))
            {
                throw new System.ArgumentException("Invalid LayoutParams supplied to " + this);
            }
            if (view.mParent != this)
            {
                throw new System.ArgumentException("Given view not a child of " + this);
            }
            view.LayoutParams = @params;
        }

        protected internal virtual bool checkLayoutParams(ViewGroup.LayoutParams p)
        {
            return p != null;
        }

        /// <summary>
        /// Interface definition for a callback to be invoked when the hierarchy
        /// within this view changed. The hierarchy changes whenever a child is added
        /// to or removed from this view.
        /// </summary>
        public interface OnHierarchyChangeListener
        {
            /// <summary>
            /// Called when a new child is added to a parent view.
            /// </summary>
            /// <param name="parent"> the view in which a child was added </param>
            /// <param name="child"> the new child view added in the hierarchy </param>
            void onChildViewAdded(View parent, View child);

            /// <summary>
            /// Called when a child is removed from a parent view.
            /// </summary>
            /// <param name="parent"> the view from which the child was removed </param>
            /// <param name="child"> the child removed from the hierarchy </param>
            void onChildViewRemoved(View parent, View child);
        }

        /// <summary>
        /// Register a callback to be invoked when a child is added to or removed
        /// from this view.
        /// </summary>
        /// <param name="listener"> the callback to invoke on hierarchy change </param>
        public virtual void setOnHierarchyChangeListener(OnHierarchyChangeListener listener)
        {
            mOnHierarchyChangeListener = listener;
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage void dispatchViewAdded(View child)
        internal virtual void dispatchViewAdded(View child)
        {
            onViewAdded(child);
            if (mOnHierarchyChangeListener != null)
            {
                mOnHierarchyChangeListener.onChildViewAdded(this, child);
            }
        }

        /// <summary>
        /// Called when a new child is added to this ViewGroup. Overrides should always
        /// call super.onViewAdded.
        /// </summary>
        /// <param name="child"> the added child view </param>
        public virtual void onViewAdded(View child)
        {
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage void dispatchViewRemoved(View child)
        internal virtual void dispatchViewRemoved(View child)
        {
            onViewRemoved(child);
            if (mOnHierarchyChangeListener != null)
            {
                mOnHierarchyChangeListener.onChildViewRemoved(this, child);
            }
        }

        /// <summary>
        /// Called when a child view is removed from this ViewGroup. Overrides should always
        /// call super.onViewRemoved.
        /// </summary>
        /// <param name="child"> the removed child view </param>
        public virtual void onViewRemoved(View child)
        {
        }

        private void clearCachedLayoutMode()
        {
            if (!hasBooleanFlag(FLAG_LAYOUT_MODE_WAS_EXPLICITLY_SET))
            {
               mLayoutMode = LAYOUT_MODE_UNDEFINED;
            }
        }

        protected internal override void onAttachedToWindow()
        {
            base.onAttachedToWindow();
            clearCachedLayoutMode();
        }

        protected internal override void onDetachedFromWindow()
        {
            base.onDetachedFromWindow();
            clearCachedLayoutMode();
        }

        /// <summary>
        /// @hide </summary>
        protected internal override void destroyHardwareResources()
        {
            base.destroyHardwareResources();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                getChildAt(i).destroyHardwareResources();
            }
        }

        /// <summary>
        /// Adds a view during layout. This is useful if in your onLayout() method,
        /// you need to add more views (as does the list view for example).
        /// 
        /// If index is negative, it means put it at the end of the list.
        /// </summary>
        /// <param name="child"> the view to add to the group </param>
        /// <param name="index"> the index at which the child must be added or -1 to add last </param>
        /// <param name="params"> the layout parameters to associate with the child </param>
        /// <returns> true if the child was added, false otherwise </returns>
        protected internal virtual bool addViewInLayout(View child, int index, LayoutParams @params)
        {
            return addViewInLayout(child, index, @params, false);
        }

        /// <summary>
        /// Adds a view during layout. This is useful if in your onLayout() method,
        /// you need to add more views (as does the list view for example).
        /// 
        /// If index is negative, it means put it at the end of the list.
        /// </summary>
        /// <param name="child"> the view to add to the group </param>
        /// <param name="index"> the index at which the child must be added or -1 to add last </param>
        /// <param name="params"> the layout parameters to associate with the child </param>
        /// <param name="preventRequestLayout"> if true, calling this method will not trigger a
        ///        layout request on child </param>
        /// <returns> true if the child was added, false otherwise </returns>
        protected internal virtual bool addViewInLayout(View child, int index, LayoutParams @params, bool preventRequestLayout)
        {
            if (child == null)
            {
                throw new System.ArgumentException("Cannot add a null child view to a ViewGroup");
            }
            child.mParent = null;
            addViewInner(child, index, @params, preventRequestLayout);
            child.mPrivateFlags = (child.mPrivateFlags & ~PFLAG_DIRTY_MASK) | PFLAG_DRAWN;
            return true;
        }

        /// <summary>
        /// Prevents the specified child to be laid out during the next layout pass.
        /// </summary>
        /// <param name="child"> the child on which to perform the cleanup </param>
        protected internal virtual void cleanupLayoutState(View child)
        {
            child.mPrivateFlags &= ~View.PFLAG_FORCE_LAYOUT;
        }

        private void addViewInner(View child, int index, LayoutParams @params, bool preventRequestLayout)
        {

            if (mTransition != null)
            {
                // Don't prevent other add transitions from completing, but cancel remove
                // transitions to let them complete the process before we add to the container
                mTransition.cancel(LayoutTransition.DISAPPEARING);
            }

            if (child.Parent != null)
            {
                throw new System.InvalidOperationException("The specified child already has a parent. " + "You must call removeView() on the child's parent first.");
            }

            if (mTransition != null)
            {
                mTransition.addChild(this, child);
            }

            if (!checkLayoutParams(@params))
            {
                @params = generateLayoutParams(@params);
            }

            if (preventRequestLayout)
            {
                child.mLayoutParams = @params;
            }
            else
            {
                child.LayoutParams = @params;
            }

            if (index < 0)
            {
                index = mChildrenCount;
            }

            addInArray(child, index);

            // tell our children
            if (preventRequestLayout)
            {
                child.assignParent(this);
            }
            else
            {
                child.mParent = this;
            }
            if (child.hasUnhandledKeyListener())
            {
                incrementChildUnhandledKeyListeners();
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean childHasFocus = child.hasFocus();
            bool childHasFocus = child.hasFocus();
            if (childHasFocus)
            {
                requestChildFocus(child, child.findFocus());
            }

            AttachInfo ai = mAttachInfo;
            if (ai != null && (mGroupFlags & FLAG_PREVENT_DISPATCH_ATTACHED_TO_WINDOW) == 0)
            {
                bool lastKeepOn = ai.mKeepScreenOn;
                ai.mKeepScreenOn = false;
                child.dispatchAttachedToWindow(mAttachInfo, (mViewFlags & VISIBILITY_MASK));
                if (ai.mKeepScreenOn)
                {
                    needGlobalAttributesUpdate(true);
                }
                ai.mKeepScreenOn = lastKeepOn;
            }

            if (child.LayoutDirectionInherited)
            {
                child.resetRtlProperties();
            }

            dispatchViewAdded(child);

            if ((child.mViewFlags & DUPLICATE_PARENT_STATE) == DUPLICATE_PARENT_STATE)
            {
                mGroupFlags |= FLAG_NOTIFY_CHILDREN_ON_DRAWABLE_STATE_CHANGE;
            }

            if (child.hasTransientState())
            {
                childHasTransientStateChanged(child, true);
            }

            if (child.Visibility != View.GONE)
            {
                notifySubtreeAccessibilityStateChangedIfNeeded();
            }

            if (mTransientIndices != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int transientCount = mTransientIndices.size();
                int transientCount = mTransientIndices.Count;
                for (int i = 0; i < transientCount; ++i)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldIndex = mTransientIndices.get(i);
                    int oldIndex = mTransientIndices[i];
                    if (index <= oldIndex)
                    {
                        mTransientIndices[i] = oldIndex + 1;
                    }
                }
            }

            if (mCurrentDragStartEvent != null && child.Visibility == VISIBLE)
            {
                notifyChildOfDragStart(child);
            }

            if (child.hasDefaultFocus())
            {
                // When adding a child that contains default focus, either during inflation or while
                // manually assembling the hierarchy, update the ancestor default-focus chain.
                DefaultFocus = child;
            }

            touchAccessibilityNodeProviderIfNeeded(child);
        }

        /// <summary>
        /// We may need to touch the provider to bring up the a11y layer. In a11y mode
        /// clients inspect the screen or the user touches it which triggers bringing up
        /// of the a11y infrastructure while in autofill mode we want the infra up and
        /// running from the beginning since we watch for a11y events to drive autofill.
        /// </summary>
        private void touchAccessibilityNodeProviderIfNeeded(View child)
        {
            if (mContext.AutofillCompatibilityEnabled)
            {
                child.AccessibilityNodeProvider;
            }
        }

        private void addInArray(View child, int index)
        {
            View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = children.length;
            int size = children.Length;
            if (index == count)
            {
                if (size == count)
                {
                    mChildren = new View[size + ARRAY_CAPACITY_INCREMENT];
                    Array.Copy(children, 0, mChildren, 0, size);
                    children = mChildren;
                }
                children[mChildrenCount++] = child;
            }
            else if (index < count)
            {
                if (size == count)
                {
                    mChildren = new View[size + ARRAY_CAPACITY_INCREMENT];
                    Array.Copy(children, 0, mChildren, 0, index);
                    Array.Copy(children, index, mChildren, index + 1, count - index);
                    children = mChildren;
                }
                else
                {
                    Array.Copy(children, index, children, index + 1, count - index);
                }
                children[index] = child;
                mChildrenCount++;
                if (mLastTouchDownIndex >= index)
                {
                    mLastTouchDownIndex++;
                }
            }
            else
            {
                throw new System.IndexOutOfRangeException("index=" + index + " count=" + count);
            }
        }

        // This method also sets the child's mParent to null
        private void removeFromArray(int index)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            if (!(mTransitioningViews != null && mTransitioningViews.Contains(children[index])))
            {
                children[index].mParent = null;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            if (index == count - 1)
            {
                children[--mChildrenCount] = null;
            }
            else if (index >= 0 && index < count)
            {
                Array.Copy(children, index + 1, children, index, count - index - 1);
                children[--mChildrenCount] = null;
            }
            else
            {
                throw new System.IndexOutOfRangeException();
            }
            if (mLastTouchDownIndex == index)
            {
                mLastTouchDownTime = 0;
                mLastTouchDownIndex = -1;
            }
            else if (mLastTouchDownIndex > index)
            {
                mLastTouchDownIndex--;
            }
        }

        // This method also sets the children's mParent to null
        private void removeFromArray(int start, int count)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;

            start = Math.Max(0, start);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int end = Math.min(childrenCount, start + count);
            int end = Math.Min(childrenCount, start + count);

            if (start == end)
            {
                return;
            }

            if (end == childrenCount)
            {
                for (int i = start; i < end; i++)
                {
                    children[i].mParent = null;
                    children[i] = null;
                }
            }
            else
            {
                for (int i = start; i < end; i++)
                {
                    children[i].mParent = null;
                }

                // Since we're looping above, we might as well do the copy, but is arraycopy()
                // faster than the extra 2 bounds checks we would do in the loop?
                Array.Copy(children, end, children, start, childrenCount - end);

                for (int i = childrenCount - (end - start); i < childrenCount; i++)
                {
                    children[i] = null;
                }
            }

            mChildrenCount -= (end - start);
        }

        private void bindLayoutAnimation(View child)
        {
            Animation a = mLayoutAnimationController.getAnimationForView(child);
            child.Animation = a;
        }

        /// <summary>
        /// Subclasses should override this method to set layout animation
        /// parameters on the supplied child.
        /// </summary>
        /// <param name="child"> the child to associate with animation parameters </param>
        /// <param name="params"> the child's layout parameters which hold the animation
        ///        parameters </param>
        /// <param name="index"> the index of the child in the view group </param>
        /// <param name="count"> the number of children in the view group </param>
        protected internal virtual void attachLayoutAnimationParameters(View child, LayoutParams @params, int index, int count)
        {
            LayoutAnimationController.AnimationParameters animationParams = @params.layoutAnimationParameters;
            if (animationParams == null)
            {
                animationParams = new LayoutAnimationController.AnimationParameters();
                @params.layoutAnimationParameters = animationParams;
            }

            animationParams.count = count;
            animationParams.index = index;
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        public override void removeView(View view)
        {
            if (removeViewInternal(view))
            {
                requestLayout();
                invalidate(true);
            }
        }

        /// <summary>
        /// Removes a view during layout. This is useful if in your onLayout() method,
        /// you need to remove more views.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="view"> the view to remove from the group </param>
        public virtual void removeViewInLayout(View view)
        {
            removeViewInternal(view);
        }

        /// <summary>
        /// Removes a range of views during layout. This is useful if in your onLayout() method,
        /// you need to remove more views.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="start"> the index of the first view to remove from the group </param>
        /// <param name="count"> the number of views to remove from the group </param>
        public virtual void removeViewsInLayout(int start, int count)
        {
            removeViewsInternal(start, count);
        }

        /// <summary>
        /// Removes the view at the specified position in the group.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="index"> the position in the group of the view to remove </param>
        public virtual void removeViewAt(int index)
        {
            removeViewInternal(index, getChildAt(index));
            requestLayout();
            invalidate(true);
        }

        /// <summary>
        /// Removes the specified range of views from the group.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        /// <param name="start"> the first position in the group of the range of views to remove </param>
        /// <param name="count"> the number of views to remove </param>
        public virtual void removeViews(int start, int count)
        {
            removeViewsInternal(start, count);
            requestLayout();
            invalidate(true);
        }

        private bool removeViewInternal(View view)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = indexOfChild(view);
            int index = indexOfChild(view);
            if (index >= 0)
            {
                removeViewInternal(index, view);
                return true;
            }
            return false;
        }

        private void removeViewInternal(int index, View view)
        {
            if (mTransition != null)
            {
                mTransition.removeChild(this, view);
            }

            bool clearChildFocus = false;
            if (view == mFocused)
            {
                view.unFocus(null);
                clearChildFocus = true;
            }
            if (view == mFocusedInCluster)
            {
                clearFocusedInCluster(view);
            }

            view.clearAccessibilityFocus();

            cancelTouchTarget(view);
            cancelHoverTarget(view);

            if (view.Animation != null || (mTransitioningViews != null && mTransitioningViews.Contains(view)))
            {
                addDisappearingView(view);
            }
            else if (view.mAttachInfo != null)
            {
               view.dispatchDetachedFromWindow();
            }

            if (view.hasTransientState())
            {
                childHasTransientStateChanged(view, false);
            }

            needGlobalAttributesUpdate(false);

            removeFromArray(index);

            if (view.hasUnhandledKeyListener())
            {
                decrementChildUnhandledKeyListeners();
            }

            if (view == mDefaultFocus)
            {
                clearDefaultFocus(view);
            }
            if (clearChildFocus)
            {
                clearChildFocus(view);
                if (!rootViewRequestFocus())
                {
                    notifyGlobalFocusCleared(this);
                }
            }

            dispatchViewRemoved(view);

            if (view.Visibility != View.GONE)
            {
                notifySubtreeAccessibilityStateChangedIfNeeded();
            }

            int transientCount = mTransientIndices == null ? 0 : mTransientIndices.Count;
            for (int i = 0; i < transientCount; ++i)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int oldIndex = mTransientIndices.get(i);
                int oldIndex = mTransientIndices[i];
                if (index < oldIndex)
                {
                    mTransientIndices[i] = oldIndex - 1;
                }
            }

            if (mCurrentDragStartEvent != null)
            {
                mChildrenInterestedInDrag.remove(view);
            }
        }

        /// <summary>
        /// Sets the LayoutTransition object for this ViewGroup. If the LayoutTransition object is
        /// not null, changes in layout which occur because of children being added to or removed from
        /// the ViewGroup will be animated according to the animations defined in that LayoutTransition
        /// object. By default, the transition object is null (so layout changes are not animated).
        /// 
        /// <para>Replacing a non-null transition will cause that previous transition to be
        /// canceled, if it is currently running, to restore this container to
        /// its correct post-transition state.</para>
        /// </summary>
        /// <param name="transition"> The LayoutTransition object that will animated changes in layout. A value
        /// of <code>null</code> means no transition will run on layout changes.
        /// @attr ref android.R.styleable#ViewGroup_animateLayoutChanges </param>
        public virtual LayoutTransition LayoutTransition
        {
            set
            {
                if (mTransition != null)
                {
                    LayoutTransition previousTransition = mTransition;
                    previousTransition.cancel();
                    previousTransition.removeTransitionListener(mLayoutTransitionListener);
                }
                mTransition = value;
                if (mTransition != null)
                {
                    mTransition.addTransitionListener(mLayoutTransitionListener);
                }
            }
            get
            {
                return mTransition;
            }
        }


        private void removeViewsInternal(int start, int count)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int end = start + count;
            int end = start + count;

            if (start < 0 || count < 0 || end > mChildrenCount)
            {
                throw new System.IndexOutOfRangeException();
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View focused = mFocused;
            View focused = mFocused;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean detach = mAttachInfo != null;
            bool detach = mAttachInfo != null;
            bool clearChildFocus = false;
            View clearDefaultFocus = null;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;

            for (int i = start; i < end; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View view = children[i];
                View view = children[i];

                if (mTransition != null)
                {
                    mTransition.removeChild(this, view);
                }

                if (view == focused)
                {
                    view.unFocus(null);
                    clearChildFocus = true;
                }
                if (view == mDefaultFocus)
                {
                    clearDefaultFocus = view;
                }
                if (view == mFocusedInCluster)
                {
                    clearFocusedInCluster(view);
                }

                view.clearAccessibilityFocus();

                cancelTouchTarget(view);
                cancelHoverTarget(view);

                if (view.Animation != null || (mTransitioningViews != null && mTransitioningViews.Contains(view)))
                {
                    addDisappearingView(view);
                }
                else if (detach)
                {
                   view.dispatchDetachedFromWindow();
                }

                if (view.hasTransientState())
                {
                    childHasTransientStateChanged(view, false);
                }

                needGlobalAttributesUpdate(false);

                dispatchViewRemoved(view);
            }

            removeFromArray(start, count);

            if (clearDefaultFocus != null)
            {
                clearDefaultFocus(clearDefaultFocus);
            }
            if (clearChildFocus)
            {
                clearChildFocus(focused);
                if (!rootViewRequestFocus())
                {
                    notifyGlobalFocusCleared(focused);
                }
            }
        }

        /// <summary>
        /// Call this method to remove all child views from the
        /// ViewGroup.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        public virtual void removeAllViews()
        {
            removeAllViewsInLayout();
            requestLayout();
            invalidate(true);
        }

        /// <summary>
        /// Called by a ViewGroup subclass to remove child views from itself,
        /// when it must first know its size on screen before it can calculate how many
        /// child views it will render. An example is a Gallery or a ListView, which
        /// may "have" 50 children, but actually only render the number of children
        /// that can currently fit inside the object on screen. Do not call
        /// this method unless you are extending ViewGroup and understand the
        /// view measuring and layout pipeline.
        /// 
        /// <para><strong>Note:</strong> do not invoke this method from
        /// <seealso cref="#draw(android.graphics.Canvas)"/>, <seealso cref="#onDraw(android.graphics.Canvas)"/>,
        /// <seealso cref="#dispatchDraw(android.graphics.Canvas)"/> or any related method.</para>
        /// </summary>
        public virtual void removeAllViewsInLayout()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            if (count <= 0)
            {
                return;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            mChildrenCount = 0;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View focused = mFocused;
            View focused = mFocused;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean detach = mAttachInfo != null;
            bool detach = mAttachInfo != null;
            bool clearChildFocus = false;

            needGlobalAttributesUpdate(false);

            for (int i = count - 1; i >= 0; i--)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View view = children[i];
                View view = children[i];

                if (mTransition != null)
                {
                    mTransition.removeChild(this, view);
                }

                if (view == focused)
                {
                    view.unFocus(null);
                    clearChildFocus = true;
                }

                view.clearAccessibilityFocus();

                cancelTouchTarget(view);
                cancelHoverTarget(view);

                if (view.Animation != null || (mTransitioningViews != null && mTransitioningViews.Contains(view)))
                {
                    addDisappearingView(view);
                }
                else if (detach)
                {
                   view.dispatchDetachedFromWindow();
                }

                if (view.hasTransientState())
                {
                    childHasTransientStateChanged(view, false);
                }

                dispatchViewRemoved(view);

                view.mParent = null;
                children[i] = null;
            }

            if (mDefaultFocus != null)
            {
                clearDefaultFocus(mDefaultFocus);
            }
            if (mFocusedInCluster != null)
            {
                clearFocusedInCluster(mFocusedInCluster);
            }
            if (clearChildFocus)
            {
                clearChildFocus(focused);
                if (!rootViewRequestFocus())
                {
                    notifyGlobalFocusCleared(focused);
                }
            }
        }

        /// <summary>
        /// Finishes the removal of a detached view. This method will dispatch the detached from
        /// window event and notify the hierarchy change listener.
        /// <para>
        /// This method is intended to be lightweight and makes no assumptions about whether the
        /// parent or child should be redrawn. Proper use of this method will include also making
        /// any appropriate <seealso cref="#requestLayout()"/> or <seealso cref="#invalidate()"/> calls.
        /// For example, callers can <seealso cref="#post(Runnable) post"/> a <seealso cref="Runnable"/>
        /// which performs a <seealso cref="#requestLayout()"/> on the next frame, after all detach/remove
        /// calls are finished, causing layout to be run prior to redrawing the view hierarchy.
        /// 
        /// </para>
        /// </summary>
        /// <param name="child"> the child to be definitely removed from the view hierarchy </param>
        /// <param name="animate"> if true and the view has an animation, the view is placed in the
        ///                disappearing views list, otherwise, it is detached from the window
        /// </param>
        /// <seealso cref= #attachViewToParent(View, int, android.view.ViewGroup.LayoutParams) </seealso>
        /// <seealso cref= #detachAllViewsFromParent() </seealso>
        /// <seealso cref= #detachViewFromParent(View) </seealso>
        /// <seealso cref= #detachViewFromParent(int) </seealso>
        protected internal virtual void removeDetachedView(View child, bool animate)
        {
            if (mTransition != null)
            {
                mTransition.removeChild(this, child);
            }

            if (child == mFocused)
            {
                child.clearFocus();
            }
            if (child == mDefaultFocus)
            {
                clearDefaultFocus(child);
            }
            if (child == mFocusedInCluster)
            {
                clearFocusedInCluster(child);
            }

            child.clearAccessibilityFocus();

            cancelTouchTarget(child);
            cancelHoverTarget(child);

            if ((animate && child.Animation != null) || (mTransitioningViews != null && mTransitioningViews.Contains(child)))
            {
                addDisappearingView(child);
            }
            else if (child.mAttachInfo != null)
            {
                child.dispatchDetachedFromWindow();
            }

            if (child.hasTransientState())
            {
                childHasTransientStateChanged(child, false);
            }

            dispatchViewRemoved(child);
        }

        /// <summary>
        /// Attaches a view to this view group. Attaching a view assigns this group as the parent,
        /// sets the layout parameters and puts the view in the list of children so that
        /// it can be retrieved by calling <seealso cref="#getChildAt(int)"/>.
        /// <para>
        /// This method is intended to be lightweight and makes no assumptions about whether the
        /// parent or child should be redrawn. Proper use of this method will include also making
        /// any appropriate <seealso cref="#requestLayout()"/> or <seealso cref="#invalidate()"/> calls.
        /// For example, callers can <seealso cref="#post(Runnable) post"/> a <seealso cref="Runnable"/>
        /// which performs a <seealso cref="#requestLayout()"/> on the next frame, after all detach/attach
        /// calls are finished, causing layout to be run prior to redrawing the view hierarchy.
        /// </para>
        /// <para>
        /// This method should be called only for views which were detached from their parent.
        /// 
        /// </para>
        /// </summary>
        /// <param name="child"> the child to attach </param>
        /// <param name="index"> the index at which the child should be attached </param>
        /// <param name="params"> the layout parameters of the child
        /// </param>
        /// <seealso cref= #removeDetachedView(View, boolean) </seealso>
        /// <seealso cref= #detachAllViewsFromParent() </seealso>
        /// <seealso cref= #detachViewFromParent(View) </seealso>
        /// <seealso cref= #detachViewFromParent(int) </seealso>
        protected internal virtual void attachViewToParent(View child, int index, LayoutParams @params)
        {
            child.mLayoutParams = @params;

            if (index < 0)
            {
                index = mChildrenCount;
            }

            addInArray(child, index);

            child.mParent = this;
            child.mPrivateFlags = (child.mPrivateFlags & ~PFLAG_DIRTY_MASK & ~PFLAG_DRAWING_CACHE_VALID) | PFLAG_DRAWN | PFLAG_INVALIDATED;
            this.mPrivateFlags |= PFLAG_INVALIDATED;

            if (child.hasFocus())
            {
                requestChildFocus(child, child.findFocus());
            }
            dispatchVisibilityAggregated(AttachedToWindow && WindowVisibility == VISIBLE && Shown);
            notifySubtreeAccessibilityStateChangedIfNeeded();
        }

        /// <summary>
        /// Detaches a view from its parent. Detaching a view should be followed
        /// either by a call to
        /// <seealso cref="#attachViewToParent(View, int, android.view.ViewGroup.LayoutParams)"/>
        /// or a call to <seealso cref="#removeDetachedView(View, boolean)"/>. Detachment should only be
        /// temporary; reattachment or removal should happen within the same drawing cycle as
        /// detachment. When a view is detached, its parent is null and cannot be retrieved by a
        /// call to <seealso cref="#getChildAt(int)"/>.
        /// </summary>
        /// <param name="child"> the child to detach
        /// </param>
        /// <seealso cref= #detachViewFromParent(int) </seealso>
        /// <seealso cref= #detachViewsFromParent(int, int) </seealso>
        /// <seealso cref= #detachAllViewsFromParent() </seealso>
        /// <seealso cref= #attachViewToParent(View, int, android.view.ViewGroup.LayoutParams) </seealso>
        /// <seealso cref= #removeDetachedView(View, boolean) </seealso>
        protected internal virtual void detachViewFromParent(View child)
        {
            removeFromArray(indexOfChild(child));
        }

        /// <summary>
        /// Detaches a view from its parent. Detaching a view should be followed
        /// either by a call to
        /// <seealso cref="#attachViewToParent(View, int, android.view.ViewGroup.LayoutParams)"/>
        /// or a call to <seealso cref="#removeDetachedView(View, boolean)"/>. Detachment should only be
        /// temporary; reattachment or removal should happen within the same drawing cycle as
        /// detachment. When a view is detached, its parent is null and cannot be retrieved by a
        /// call to <seealso cref="#getChildAt(int)"/>.
        /// </summary>
        /// <param name="index"> the index of the child to detach
        /// </param>
        /// <seealso cref= #detachViewFromParent(View) </seealso>
        /// <seealso cref= #detachAllViewsFromParent() </seealso>
        /// <seealso cref= #detachViewsFromParent(int, int) </seealso>
        /// <seealso cref= #attachViewToParent(View, int, android.view.ViewGroup.LayoutParams) </seealso>
        /// <seealso cref= #removeDetachedView(View, boolean) </seealso>
        protected internal virtual void detachViewFromParent(int index)
        {
            removeFromArray(index);
        }

        /// <summary>
        /// Detaches a range of views from their parents. Detaching a view should be followed
        /// either by a call to
        /// <seealso cref="#attachViewToParent(View, int, android.view.ViewGroup.LayoutParams)"/>
        /// or a call to <seealso cref="#removeDetachedView(View, boolean)"/>. Detachment should only be
        /// temporary; reattachment or removal should happen within the same drawing cycle as
        /// detachment. When a view is detached, its parent is null and cannot be retrieved by a
        /// call to <seealso cref="#getChildAt(int)"/>.
        /// </summary>
        /// <param name="start"> the first index of the childrend range to detach </param>
        /// <param name="count"> the number of children to detach
        /// </param>
        /// <seealso cref= #detachViewFromParent(View) </seealso>
        /// <seealso cref= #detachViewFromParent(int) </seealso>
        /// <seealso cref= #detachAllViewsFromParent() </seealso>
        /// <seealso cref= #attachViewToParent(View, int, android.view.ViewGroup.LayoutParams) </seealso>
        /// <seealso cref= #removeDetachedView(View, boolean) </seealso>
        protected internal virtual void detachViewsFromParent(int start, int count)
        {
            removeFromArray(start, count);
        }

        /// <summary>
        /// Detaches all views from the parent. Detaching a view should be followed
        /// either by a call to
        /// <seealso cref="#attachViewToParent(View, int, android.view.ViewGroup.LayoutParams)"/>
        /// or a call to <seealso cref="#removeDetachedView(View, boolean)"/>. Detachment should only be
        /// temporary; reattachment or removal should happen within the same drawing cycle as
        /// detachment. When a view is detached, its parent is null and cannot be retrieved by a
        /// call to <seealso cref="#getChildAt(int)"/>.
        /// </summary>
        /// <seealso cref= #detachViewFromParent(View) </seealso>
        /// <seealso cref= #detachViewFromParent(int) </seealso>
        /// <seealso cref= #detachViewsFromParent(int, int) </seealso>
        /// <seealso cref= #attachViewToParent(View, int, android.view.ViewGroup.LayoutParams) </seealso>
        /// <seealso cref= #removeDetachedView(View, boolean) </seealso>
        protected internal virtual void detachAllViewsFromParent()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            if (count <= 0)
            {
                return;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            mChildrenCount = 0;

            for (int i = count - 1; i >= 0; i--)
            {
                children[i].mParent = null;
                children[i] = null;
            }
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @CallSuper public void onDescendantInvalidated(@NonNull View child, @NonNull View target)
        public override void onDescendantInvalidated(View child, View target)
        {
            /*
             * HW-only, Rect-ignoring damage codepath
             *
             * We don't deal with rectangles here, since RenderThread native code computes damage for
             * everything drawn by HWUI (and SW layer / drawing cache doesn't keep track of damage area)
             */

            // if set, combine the animation flag into the parent
            mPrivateFlags |= (target.mPrivateFlags & PFLAG_DRAW_ANIMATION);

            if ((target.mPrivateFlags & ~PFLAG_DIRTY_MASK) != 0)
            {
                // We lazily use PFLAG_DIRTY, since computing opaque isn't worth the potential
                // optimization in provides in a DisplayList world.
                mPrivateFlags = (mPrivateFlags & ~PFLAG_DIRTY_MASK) | PFLAG_DIRTY;

                // simplified invalidateChildInParent behavior: clear cache validity to be safe...
                mPrivateFlags &= ~PFLAG_DRAWING_CACHE_VALID;
            }

            // ... and mark inval if in software layer that needs to repaint (hw handled in native)
            if (mLayerType == LAYER_TYPE_SOFTWARE)
            {
                // Layered parents should be invalidated. Escalate to a full invalidate (and note that
                // we do this after consuming any relevant flags from the originating descendant)
                mPrivateFlags |= PFLAG_INVALIDATED | PFLAG_DIRTY;
                target = this;
            }

            if (mParent != null)
            {
                mParent.onDescendantInvalidated(this, target);
            }
        }


        /// <summary>
        /// Don't call or override this method. It is used for the implementation of
        /// the view hierarchy.
        /// </summary>
        /// @deprecated Use <seealso cref="#onDescendantInvalidated(View, View)"/> instead to observe updates to
        /// draw state in descendants. 
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Deprecated("Use <seealso cref="#onDescendantInvalidated(View, View)"/> instead to observe updates to") @Override public final void invalidateChild(View child, final android.graphics.Rect dirty)
        [Obsolete("Use <seealso cref="#onDescendantInvalidated(View, View)"/> instead to observe updates to")]
        public override void invalidateChild(View child, Rect dirty)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AttachInfo attachInfo = mAttachInfo;
            AttachInfo attachInfo = mAttachInfo;
            if (attachInfo != null && attachInfo.mHardwareAccelerated)
            {
                // HW accelerated fast path
                onDescendantInvalidated(child, child);
                return;
            }

            ViewParent parent = this;
            if (attachInfo != null)
            {
                // If the child is drawing an animation, we want to copy this flag onto
                // ourselves and the parent to make sure the invalidate request goes
                // through
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean drawAnimation = (child.mPrivateFlags & PFLAG_DRAW_ANIMATION) != 0;
                bool drawAnimation = (child.mPrivateFlags & PFLAG_DRAW_ANIMATION) != 0;

                // Check whether the child that requests the invalidate is fully opaque
                // Views being animated or transformed are not considered opaque because we may
                // be invalidating their old position and need the parent to paint behind them.
                Matrix childMatrix = child.Matrix;
                // Mark the child as dirty, using the appropriate flag
                // Make sure we do not set both flags at the same time

                if (child.mLayerType != LAYER_TYPE_NONE)
                {
                    mPrivateFlags |= PFLAG_INVALIDATED;
                    mPrivateFlags &= ~PFLAG_DRAWING_CACHE_VALID;
                }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] location = attachInfo.mInvalidateChildLocation;
                int[] location = attachInfo.mInvalidateChildLocation;
                location[CHILD_LEFT_INDEX] = child.mLeft;
                location[CHILD_TOP_INDEX] = child.mTop;
                if (!childMatrix.Identity || (mGroupFlags & ViewGroup.FLAG_SUPPORT_STATIC_TRANSFORMATIONS) != 0)
                {
                    RectF boundingRect = attachInfo.mTmpTransformRect;
                    boundingRect.set(dirty);
                    Matrix transformMatrix;
                    if ((mGroupFlags & ViewGroup.FLAG_SUPPORT_STATIC_TRANSFORMATIONS) != 0)
                    {
                        Transformation t = attachInfo.mTmpTransformation;
                        bool transformed = getChildStaticTransformation(child, t);
                        if (transformed)
                        {
                            transformMatrix = attachInfo.mTmpMatrix;
                            transformMatrix.set(t.Matrix);
                            if (!childMatrix.Identity)
                            {
                                transformMatrix.preConcat(childMatrix);
                            }
                        }
                        else
                        {
                            transformMatrix = childMatrix;
                        }
                    }
                    else
                    {
                        transformMatrix = childMatrix;
                    }
                    transformMatrix.mapRect(boundingRect);
                    dirty.set((int) Math.Floor(boundingRect.left), (int) Math.Floor(boundingRect.top), (int) Math.Ceiling(boundingRect.right), (int) Math.Ceiling(boundingRect.bottom));
                }

                do
                {
                    View view = null;
                    if (parent is View)
                    {
                        view = (View) parent;
                    }

                    if (drawAnimation)
                    {
                        if (view != null)
                        {
                            view.mPrivateFlags |= PFLAG_DRAW_ANIMATION;
                        }
                        else if (parent is ViewRootImpl)
                        {
                            ((ViewRootImpl) parent).mIsAnimating = true;
                        }
                    }

                    // If the parent is dirty opaque or not dirty, mark it dirty with the opaque
                    // flag coming from the child that initiated the invalidate
                    if (view != null)
                    {
                        if ((view.mPrivateFlags & PFLAG_DIRTY_MASK) != PFLAG_DIRTY)
                        {
                            view.mPrivateFlags = (view.mPrivateFlags & ~PFLAG_DIRTY_MASK) | PFLAG_DIRTY;
                        }
                    }

                    parent = parent.invalidateChildInParent(location, dirty);
                    if (view != null)
                    {
                        // Account for transform on current parent
                        Matrix m = view.Matrix;
                        if (!m.Identity)
                        {
                            RectF boundingRect = attachInfo.mTmpTransformRect;
                            boundingRect.set(dirty);
                            m.mapRect(boundingRect);
                            dirty.set((int) Math.Floor(boundingRect.left), (int) Math.Floor(boundingRect.top), (int) Math.Ceiling(boundingRect.right), (int) Math.Ceiling(boundingRect.bottom));
                        }
                    }
                } while (parent != null);
            }
        }

        /// <summary>
        /// Don't call or override this method. It is used for the implementation of
        /// the view hierarchy.
        /// 
        /// This implementation returns null if this ViewGroup does not have a parent,
        /// if this ViewGroup is already fully invalidated or if the dirty rectangle
        /// does not intersect with this ViewGroup's bounds.
        /// </summary>
        /// @deprecated Use <seealso cref="#onDescendantInvalidated(View, View)"/> instead to observe updates to
        /// draw state in descendants. 
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Deprecated("Use <seealso cref="#onDescendantInvalidated(View, View)"/> instead to observe updates to") @Override public ViewParent invalidateChildInParent(final int[] location, final android.graphics.Rect dirty)
        [Obsolete("Use <seealso cref="#onDescendantInvalidated(View, View)"/> instead to observe updates to")]
        public override ViewParent invalidateChildInParent(int[] location, Rect dirty)
        {
            if ((mPrivateFlags & (PFLAG_DRAWN | PFLAG_DRAWING_CACHE_VALID)) != 0)
            {
                // either DRAWN, or DRAWING_CACHE_VALID
                if ((mGroupFlags & (FLAG_OPTIMIZE_INVALIDATE | FLAG_ANIMATION_DONE)) != FLAG_OPTIMIZE_INVALIDATE)
                {
                    dirty.offset(location[CHILD_LEFT_INDEX] - mScrollX, location[CHILD_TOP_INDEX] - mScrollY);
                    if ((mGroupFlags & FLAG_CLIP_CHILDREN) == 0)
                    {
                        dirty.union(0, 0, mRight - mLeft, mBottom - mTop);
                    }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int left = mLeft;
                    int left = mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int top = mTop;
                    int top = mTop;

                    if ((mGroupFlags & FLAG_CLIP_CHILDREN) == FLAG_CLIP_CHILDREN)
                    {
                        if (!dirty.intersect(0, 0, mRight - left, mBottom - top))
                        {
                            dirty.setEmpty();
                        }
                    }

                    location[CHILD_LEFT_INDEX] = left;
                    location[CHILD_TOP_INDEX] = top;
                }
                else
                {

                    if ((mGroupFlags & FLAG_CLIP_CHILDREN) == FLAG_CLIP_CHILDREN)
                    {
                        dirty.set(0, 0, mRight - mLeft, mBottom - mTop);
                    }
                    else
                    {
                        // in case the dirty rect extends outside the bounds of this container
                        dirty.union(0, 0, mRight - mLeft, mBottom - mTop);
                    }
                    location[CHILD_LEFT_INDEX] = mLeft;
                    location[CHILD_TOP_INDEX] = mTop;

                    mPrivateFlags &= ~PFLAG_DRAWN;
                }
                mPrivateFlags &= ~PFLAG_DRAWING_CACHE_VALID;
                if (mLayerType != LAYER_TYPE_NONE)
                {
                    mPrivateFlags |= PFLAG_INVALIDATED;
                }

                return mParent;
            }

            return null;
        }

        /// <summary>
        /// Offset a rectangle that is in a descendant's coordinate
        /// space into our coordinate space. </summary>
        /// <param name="descendant"> A descendant of this view </param>
        /// <param name="rect"> A rectangle defined in descendant's coordinate space. </param>
        public void offsetDescendantRectToMyCoords(View descendant, Rect rect)
        {
            offsetRectBetweenParentAndChild(descendant, rect, true, false);
        }

        /// <summary>
        /// Offset a rectangle that is in our coordinate space into an ancestor's
        /// coordinate space. </summary>
        /// <param name="descendant"> A descendant of this view </param>
        /// <param name="rect"> A rectangle defined in descendant's coordinate space. </param>
        public void offsetRectIntoDescendantCoords(View descendant, Rect rect)
        {
            offsetRectBetweenParentAndChild(descendant, rect, false, false);
        }

        /// <summary>
        /// Helper method that offsets a rect either from parent to descendant or
        /// descendant to parent.
        /// </summary>
        internal virtual void offsetRectBetweenParentAndChild(View descendant, Rect rect, bool offsetFromChildToParent, bool clipToBounds)
        {

            // already in the same coord system :)
            if (descendant == this)
            {
                return;
            }

            ViewParent theParent = descendant.mParent;

            // search and offset up to the parent
            while ((theParent != null) && (theParent is View) && (theParent != this))
            {

                if (offsetFromChildToParent)
                {
                    rect.offset(descendant.mLeft - descendant.mScrollX, descendant.mTop - descendant.mScrollY);
                    if (clipToBounds)
                    {
                        View p = (View) theParent;
                        bool intersected = rect.intersect(0, 0, p.mRight - p.mLeft, p.mBottom - p.mTop);
                        if (!intersected)
                        {
                            rect.setEmpty();
                        }
                    }
                }
                else
                {
                    if (clipToBounds)
                    {
                        View p = (View) theParent;
                        bool intersected = rect.intersect(0, 0, p.mRight - p.mLeft, p.mBottom - p.mTop);
                        if (!intersected)
                        {
                            rect.setEmpty();
                        }
                    }
                    rect.offset(descendant.mScrollX - descendant.mLeft, descendant.mScrollY - descendant.mTop);
                }

                descendant = (View) theParent;
                theParent = descendant.mParent;
            }

            // now that we are up to this view, need to offset one more time
            // to get into our coordinate space
            if (theParent == this)
            {
                if (offsetFromChildToParent)
                {
                    rect.offset(descendant.mLeft - descendant.mScrollX, descendant.mTop - descendant.mScrollY);
                }
                else
                {
                    rect.offset(descendant.mScrollX - descendant.mLeft, descendant.mScrollY - descendant.mTop);
                }
            }
            else
            {
                throw new System.ArgumentException("parameter must be a descendant of this view");
            }
        }

        /// <summary>
        /// Offset the vertical location of all children of this view by the specified number of pixels.
        /// </summary>
        /// <param name="offset"> the number of pixels to offset
        /// 
        /// @hide </param>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage public void offsetChildrenTopAndBottom(int offset)
        public virtual void offsetChildrenTopAndBottom(int offset)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            bool invalidate = false;

            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View v = children[i];
                View v = children[i];
                v.mTop += offset;
                v.mBottom += offset;
                if (v.mRenderNode != null)
                {
                    invalidate = true;
                    v.mRenderNode.offsetTopAndBottom(offset);
                }
            }

            if (invalidate)
            {
                invalidateViewProperty(false, false);
            }
            notifySubtreeAccessibilityStateChangedIfNeeded();
        }

        public override bool getChildVisibleRect(View child, Rect r, Point offset)
        {
            return getChildVisibleRect(child, r, offset, false);
        }

        /// <param name="forceParentCheck"> true to guarantee that this call will propagate to all ancestors,
        ///      false otherwise
        /// 
        /// @hide </param>
        public virtual bool getChildVisibleRect(View child, Rect r, Point offset, bool forceParentCheck)
        {
            // It doesn't make a whole lot of sense to call this on a view that isn't attached,
            // but for some simple tests it can be useful. If we don't have attach info this
            // will allocate memory.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.graphics.RectF rect = mAttachInfo != null ? mAttachInfo.mTmpTransformRect : new android.graphics.RectF();
            RectF rect = mAttachInfo != null ? mAttachInfo.mTmpTransformRect : new RectF();
            rect.set(r);

            if (!child.hasIdentityMatrix())
            {
                child.Matrix.mapRect(rect);
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dx = child.mLeft - mScrollX;
            int dx = child.mLeft - mScrollX;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dy = child.mTop - mScrollY;
            int dy = child.mTop - mScrollY;

            rect.offset(dx, dy);

            if (offset != null)
            {
                if (!child.hasIdentityMatrix())
                {
                    float[] position = mAttachInfo != null ? mAttachInfo.mTmpTransformLocation : new float[2];
                    position[0] = offset.x;
                    position[1] = offset.y;
                    child.Matrix.mapPoints(position);
                    offset.x = Math.Round(position[0]);
                    offset.y = Math.Round(position[1]);
                }
                offset.x += dx;
                offset.y += dy;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int width = mRight - mLeft;
            int width = mRight - mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int height = mBottom - mTop;
            int height = mBottom - mTop;

            bool rectIsVisible = true;
            if (mParent == null || (mParent is ViewGroup && ((ViewGroup) mParent).ClipChildren))
            {
                // Clip to bounds.
                rectIsVisible = rect.intersect(0, 0, width, height);
            }

            if ((forceParentCheck || rectIsVisible) && (mGroupFlags & CLIP_TO_PADDING_MASK) == CLIP_TO_PADDING_MASK)
            {
                // Clip to padding.
                rectIsVisible = rect.intersect(mPaddingLeft, mPaddingTop, width - mPaddingRight, height - mPaddingBottom);
            }

            if ((forceParentCheck || rectIsVisible) && mClipBounds != null)
            {
                // Clip to clipBounds.
                rectIsVisible = rect.intersect(mClipBounds.left, mClipBounds.top, mClipBounds.right, mClipBounds.bottom);
            }
            r.set((int) Math.Floor(rect.left), (int) Math.Floor(rect.top), (int) Math.Ceiling(rect.right), (int) Math.Ceiling(rect.bottom));

            if ((forceParentCheck || rectIsVisible) && mParent != null)
            {
                if (mParent is ViewGroup)
                {
                    rectIsVisible = ((ViewGroup) mParent).getChildVisibleRect(this, r, offset, forceParentCheck);
                }
                else
                {
                    rectIsVisible = mParent.getChildVisibleRect(this, r, offset);
                }
            }
            return rectIsVisible;
        }

        public override void layout(int l, int t, int r, int b)
        {
            if (!mSuppressLayout && (mTransition == null || !mTransition.ChangingLayout))
            {
                if (mTransition != null)
                {
                    mTransition.layoutChange(this);
                }
                base.layout(l, t, r, b);
            }
            else
            {
                // record the fact that we noop'd it; request layout when transition finishes
                mLayoutCalledWhileSuppressed = true;
            }
        }

        protected internal override abstract void onLayout(bool changed, int l, int t, int r, int b);

        /// <summary>
        /// Indicates whether the view group has the ability to animate its children
        /// after the first layout.
        /// </summary>
        /// <returns> true if the children can be animated, false otherwise </returns>
        protected internal virtual bool canAnimate()
        {
            return mLayoutAnimationController != null;
        }

        /// <summary>
        /// Runs the layout animation. Calling this method triggers a relayout of
        /// this view group.
        /// </summary>
        public virtual void startLayoutAnimation()
        {
            if (mLayoutAnimationController != null)
            {
                mGroupFlags |= FLAG_RUN_ANIMATION;
                requestLayout();
            }
        }

        /// <summary>
        /// Schedules the layout animation to be played after the next layout pass
        /// of this view group. This can be used to restart the layout animation
        /// when the content of the view group changes or when the activity is
        /// paused and resumed.
        /// </summary>
        public virtual void scheduleLayoutAnimation()
        {
            mGroupFlags |= FLAG_RUN_ANIMATION;
        }

        /// <summary>
        /// Sets the layout animation controller used to animate the group's
        /// children after the first layout.
        /// </summary>
        /// <param name="controller"> the animation controller </param>
        public virtual LayoutAnimationController LayoutAnimation
        {
            set
            {
                mLayoutAnimationController = value;
                if (mLayoutAnimationController != null)
                {
                    mGroupFlags |= FLAG_RUN_ANIMATION;
                }
            }
            get
            {
                return mLayoutAnimationController;
            }
        }


        /// <summary>
        /// Indicates whether the children's drawing cache is used during a layout
        /// animation. By default, the drawing cache is enabled but this will prevent
        /// nested layout animations from working. To nest animations, you must disable
        /// the cache.
        /// </summary>
        /// <returns> true if the animation cache is enabled, false otherwise
        /// </returns>
        /// <seealso cref= #setAnimationCacheEnabled(boolean) </seealso>
        /// <seealso cref= View#setDrawingCacheEnabled(boolean)
        /// </seealso>
        /// @deprecated As of <seealso cref="android.os.Build.VERSION_CODES#M"/>, this property is ignored.
        /// Caching behavior of children may be controlled through <seealso cref="View#setLayerType(int, Paint)"/>. 
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deprecated("As of <seealso cref="android.os.Build.VERSION_CODES#M"/>, this property is ignored.") @InspectableProperty(name = "animationCache") public boolean isAnimationCacheEnabled()
        [Obsolete("As of <seealso cref="android.os.Build.VERSION_CODES#M"/>, this property is ignored.")]
        public virtual bool AnimationCacheEnabled
        {
            get
            {
                return (mGroupFlags & FLAG_ANIMATION_CACHE) == FLAG_ANIMATION_CACHE;
            }
            set
            {
                setBooleanFlag(FLAG_ANIMATION_CACHE, value);
            }
        }


        /// <summary>
        /// Indicates whether this ViewGroup will always try to draw its children using their
        /// drawing cache. By default this property is enabled.
        /// </summary>
        /// <returns> true if the animation cache is enabled, false otherwise
        /// </returns>
        /// <seealso cref= #setAlwaysDrawnWithCacheEnabled(boolean) </seealso>
        /// <seealso cref= #setChildrenDrawnWithCacheEnabled(boolean) </seealso>
        /// <seealso cref= View#setDrawingCacheEnabled(boolean)
        /// </seealso>
        /// @deprecated As of <seealso cref="android.os.Build.VERSION_CODES#M"/>, this property is ignored.
        /// Child views may no longer have their caching behavior disabled by parents. 
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deprecated("As of <seealso cref="android.os.Build.VERSION_CODES#M"/>, this property is ignored.") @InspectableProperty(name = "alwaysDrawnWithCache") public boolean isAlwaysDrawnWithCacheEnabled()
        [Obsolete("As of <seealso cref="android.os.Build.VERSION_CODES#M"/>, this property is ignored.")]
        public virtual bool AlwaysDrawnWithCacheEnabled
        {
            get
            {
                return (mGroupFlags & FLAG_ALWAYS_DRAWN_WITH_CACHE) == FLAG_ALWAYS_DRAWN_WITH_CACHE;
            }
            set
            {
                setBooleanFlag(FLAG_ALWAYS_DRAWN_WITH_CACHE, value);
            }
        }


        /// <summary>
        /// Indicates whether the ViewGroup is currently drawing its children using
        /// their drawing cache.
        /// </summary>
        /// <returns> true if children should be drawn with their cache, false otherwise
        /// </returns>
        /// <seealso cref= #setAlwaysDrawnWithCacheEnabled(boolean) </seealso>
        /// <seealso cref= #setChildrenDrawnWithCacheEnabled(boolean)
        /// </seealso>
        /// @deprecated As of <seealso cref="android.os.Build.VERSION_CODES#M"/>, this property is ignored.
        /// Child views may no longer be forced to cache their rendering state by their parents.
        /// Use <seealso cref="View#setLayerType(int, Paint)"/> on individual Views instead. 
        [Obsolete("As of <seealso cref="android.os.Build.VERSION_CODES#M"/>, this property is ignored.")]
        protected internal virtual bool ChildrenDrawnWithCacheEnabled
        {
            get
            {
                return (mGroupFlags & FLAG_CHILDREN_DRAWN_WITH_CACHE) == FLAG_CHILDREN_DRAWN_WITH_CACHE;
            }
            set
            {
                setBooleanFlag(FLAG_CHILDREN_DRAWN_WITH_CACHE, value);
            }
        }


        /// <summary>
        /// Indicates whether the ViewGroup is drawing its children in the order defined by
        /// <seealso cref="#getChildDrawingOrder(int, int)"/>.
        /// </summary>
        /// <returns> true if children drawing order is defined by <seealso cref="#getChildDrawingOrder(int, int)"/>,
        ///         false otherwise
        /// </returns>
        /// <seealso cref= #setChildrenDrawingOrderEnabled(boolean) </seealso>
        /// <seealso cref= #getChildDrawingOrder(int, int) </seealso>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "drawing") protected boolean isChildrenDrawingOrderEnabled()
        protected internal virtual bool ChildrenDrawingOrderEnabled
        {
            get
            {
                return (mGroupFlags & FLAG_USE_CHILD_DRAWING_ORDER) == FLAG_USE_CHILD_DRAWING_ORDER;
            }
            set
            {
                setBooleanFlag(FLAG_USE_CHILD_DRAWING_ORDER, value);
            }
        }


        private bool hasBooleanFlag(int flag)
        {
            return (mGroupFlags & flag) == flag;
        }

        private void setBooleanFlag(int flag, bool value)
        {
            if (value)
            {
                mGroupFlags |= flag;
            }
            else
            {
                mGroupFlags &= ~flag;
            }
        }

        /// <summary>
        /// Returns an integer indicating what types of drawing caches are kept in memory.
        /// </summary>
        /// <seealso cref= #setPersistentDrawingCache(int) </seealso>
        /// <seealso cref= #setAnimationCacheEnabled(boolean)
        /// </seealso>
        /// <returns> one or a combination of <seealso cref="#PERSISTENT_NO_CACHE"/>,
        ///         <seealso cref="#PERSISTENT_ANIMATION_CACHE"/>, <seealso cref="#PERSISTENT_SCROLLING_CACHE"/>
        ///         and <seealso cref="#PERSISTENT_ALL_CACHES"/>
        /// </returns>
        /// @deprecated The view drawing cache was largely made obsolete with the introduction of
        /// hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
        /// layers are largely unnecessary and can easily result in a net loss in performance due to the
        /// cost of creating and updating the layer. In the rare cases where caching layers are useful,
        /// such as for alpha animations, <seealso cref="#setLayerType(int, Paint)"/> handles this with hardware
        /// rendering. For software-rendered snapshots of a small part of the View hierarchy or
        /// individual Views it is recommended to create a <seealso cref="Canvas"/> from either a <seealso cref="Bitmap"/> or
        /// <seealso cref="android.graphics.Picture"/> and call <seealso cref="#draw(Canvas)"/> on the View. However these
        /// software-rendered usages are discouraged and have compatibility issues with hardware-only
        /// rendering features such as <seealso cref="android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE"/>
        /// bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
        /// reports or unit testing the <seealso cref="PixelCopy"/> API is recommended. 
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deprecated("The view drawing cache was largely made obsolete with the introduction of") @ViewDebug.ExportedProperty(category = "drawing", mapping = { @ViewDebug.IntToString(from = PERSISTENT_NO_CACHE, to = "NONE"), @ViewDebug.IntToString(from = PERSISTENT_ANIMATION_CACHE, to = "ANIMATION"), @ViewDebug.IntToString(from = PERSISTENT_SCROLLING_CACHE, to = "SCROLLING"), @ViewDebug.IntToString(from = PERSISTENT_ALL_CACHES, to = "ALL") }) @InspectableProperty(enumMapping = { @EnumEntry(value = PERSISTENT_NO_CACHE, name = "none"), @EnumEntry(value = PERSISTENT_ANIMATION_CACHE, name = "animation"), @EnumEntry(value = PERSISTENT_SCROLLING_CACHE, name = "scrolling"), @EnumEntry(value = PERSISTENT_ALL_CACHES, name = "all")}) public int getPersistentDrawingCache()
        [Obsolete("The view drawing cache was largely made obsolete with the introduction of")]
        public virtual int PersistentDrawingCache
        {
            get
            {
                return mPersistentDrawingCache;
            }
            set
            {
                mPersistentDrawingCache = value & PERSISTENT_ALL_CACHES;
            }
        }


        private void setLayoutMode(int layoutMode, bool explicitly)
        {
            mLayoutMode = layoutMode;
            setBooleanFlag(FLAG_LAYOUT_MODE_WAS_EXPLICITLY_SET, explicitly);
        }

        /// <summary>
        /// Recursively traverse the view hierarchy, resetting the layoutMode of any
        /// descendants that had inherited a different layoutMode from a previous parent.
        /// Recursion terminates when a descendant's mode is:
        /// <ul>
        ///     <li>Undefined</li>
        ///     <li>The same as the root node's</li>
        ///     <li>A mode that had been explicitly set</li>
        /// <ul/>
        /// The first two clauses are optimizations. </summary>
        /// <param name="layoutModeOfRoot"> </param>
        internal override void invalidateInheritedLayoutMode(int layoutModeOfRoot)
        {
            if (mLayoutMode == LAYOUT_MODE_UNDEFINED || mLayoutMode == layoutModeOfRoot || hasBooleanFlag(FLAG_LAYOUT_MODE_WAS_EXPLICITLY_SET))
            {
                return;
            }
            setLayoutMode(LAYOUT_MODE_UNDEFINED, false);

            // apply recursively
            for (int i = 0, N = ChildCount; i < N; i++)
            {
                getChildAt(i).invalidateInheritedLayoutMode(layoutModeOfRoot);
            }
        }

        /// <summary>
        /// Returns the basis of alignment during layout operations on this ViewGroup:
        /// either <seealso cref="#LAYOUT_MODE_CLIP_BOUNDS"/> or <seealso cref="#LAYOUT_MODE_OPTICAL_BOUNDS"/>.
        /// <para>
        /// If no layoutMode was explicitly set, either programmatically or in an XML resource,
        /// the method returns the layoutMode of the view's parent ViewGroup if such a parent exists,
        /// otherwise the method returns a default value of <seealso cref="#LAYOUT_MODE_CLIP_BOUNDS"/>.
        /// 
        /// </para>
        /// </summary>
        /// <returns> the layout mode to use during layout operations
        /// </returns>
        /// <seealso cref= #setLayoutMode(int) </seealso>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @InspectableProperty(enumMapping = { @EnumEntry(value = LAYOUT_MODE_CLIP_BOUNDS, name = "clipBounds"), @EnumEntry(value = LAYOUT_MODE_OPTICAL_BOUNDS, name = "opticalBounds") }) public int getLayoutMode()
        public virtual int LayoutMode
        {
            get
            {
                if (mLayoutMode == LAYOUT_MODE_UNDEFINED)
                {
                    int inheritedLayoutMode = (mParent is ViewGroup) ? ((ViewGroup) mParent).LayoutMode : LAYOUT_MODE_DEFAULT;
                    setLayoutMode(inheritedLayoutMode, false);
                }
                return mLayoutMode;
            }
            set
            {
                if (mLayoutMode != value)
                {
                    invalidateInheritedLayoutMode(value);
                    setLayoutMode(value, value != LAYOUT_MODE_UNDEFINED);
                    requestLayout();
                }
            }
        }


        /// <summary>
        /// Returns a new set of layout parameters based on the supplied attributes set.
        /// </summary>
        /// <param name="attrs"> the attributes to build the layout parameters from
        /// </param>
        /// <returns> an instance of <seealso cref="android.view.ViewGroup.LayoutParams"/> or one
        ///         of its descendants </returns>
        public virtual LayoutParams generateLayoutParams(AttributeSet attrs)
        {
            return new LayoutParams(Context, attrs);
        }

        /// <summary>
        /// Returns a safe set of layout parameters based on the supplied layout params.
        /// When a ViewGroup is passed a View whose layout params do not pass the test of
        /// <seealso cref="#checkLayoutParams(android.view.ViewGroup.LayoutParams)"/>, this method
        /// is invoked. This method should return a new set of layout params suitable for
        /// this ViewGroup, possibly by copying the appropriate attributes from the
        /// specified set of layout params.
        /// </summary>
        /// <param name="p"> The layout parameters to convert into a suitable set of layout parameters
        ///          for this ViewGroup.
        /// </param>
        /// <returns> an instance of <seealso cref="android.view.ViewGroup.LayoutParams"/> or one
        ///         of its descendants </returns>
        protected internal virtual LayoutParams generateLayoutParams(ViewGroup.LayoutParams p)
        {
            return p;
        }

        /// <summary>
        /// Returns a set of default layout parameters. These parameters are requested
        /// when the View passed to <seealso cref="#addView(View)"/> has no layout parameters
        /// already set. If null is returned, an exception is thrown from addView.
        /// </summary>
        /// <returns> a set of default layout parameters or null </returns>
        protected internal virtual LayoutParams generateDefaultLayoutParams()
        {
            return new LayoutParams(LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
        }

        protected internal override void debug(int depth)
        {
            base.debug(depth);
            string output;

            if (mFocused != null)
            {
                output = debugIndent(depth);
                output += "mFocused";
                Log.d(VIEW_LOG_TAG, output);
                mFocused.debug(depth + 1);
            }
            if (mDefaultFocus != null)
            {
                output = debugIndent(depth);
                output += "mDefaultFocus";
                Log.d(VIEW_LOG_TAG, output);
                mDefaultFocus.debug(depth + 1);
            }
            if (mFocusedInCluster != null)
            {
                output = debugIndent(depth);
                output += "mFocusedInCluster";
                Log.d(VIEW_LOG_TAG, output);
                mFocusedInCluster.debug(depth + 1);
            }
            if (mChildrenCount != 0)
            {
                output = debugIndent(depth);
                output += "{";
                Log.d(VIEW_LOG_TAG, output);
            }
            int count = mChildrenCount;
            for (int i = 0; i < count; i++)
            {
                View child = mChildren[i];
                child.debug(depth + 1);
            }

            if (mChildrenCount != 0)
            {
                output = debugIndent(depth);
                output += "}";
                Log.d(VIEW_LOG_TAG, output);
            }
        }

        /// <summary>
        /// Returns the position in the group of the specified child view.
        /// </summary>
        /// <param name="child"> the view for which to get the position </param>
        /// <returns> a positive integer representing the position of the view in the
        ///         group, or -1 if the view does not exist in the group </returns>
        public virtual int indexOfChild(View child)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                if (children[i] == child)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the number of children in the group.
        /// </summary>
        /// <returns> a positive integer representing the number of children in
        ///         the group </returns>
        public virtual int ChildCount
        {
            get
            {
                return mChildrenCount;
            }
        }

        /// <summary>
        /// Returns the view at the specified position in the group.
        /// </summary>
        /// <param name="index"> the position at which to get the view from </param>
        /// <returns> the view at the specified position or null if the position
        ///         does not exist within the group </returns>
        public virtual View getChildAt(int index)
        {
            if (index < 0 || index >= mChildrenCount)
            {
                return null;
            }
            return mChildren[index];
        }

        /// <summary>
        /// Ask all of the children of this view to measure themselves, taking into
        /// account both the MeasureSpec requirements for this view and its padding.
        /// We skip children that are in the GONE state The heavy lifting is done in
        /// getChildMeasureSpec.
        /// </summary>
        /// <param name="widthMeasureSpec"> The width requirements for this view </param>
        /// <param name="heightMeasureSpec"> The height requirements for this view </param>
        protected internal virtual void measureChildren(int widthMeasureSpec, int heightMeasureSpec)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = mChildrenCount;
            int size = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = 0; i < size; ++i)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                View child = children[i];
                if ((child.mViewFlags & VISIBILITY_MASK) != GONE)
                {
                    measureChild(child, widthMeasureSpec, heightMeasureSpec);
                }
            }
        }

        /// <summary>
        /// Ask one of the children of this view to measure itself, taking into
        /// account both the MeasureSpec requirements for this view and its padding.
        /// The heavy lifting is done in getChildMeasureSpec.
        /// </summary>
        /// <param name="child"> The child to measure </param>
        /// <param name="parentWidthMeasureSpec"> The width requirements for this view </param>
        /// <param name="parentHeightMeasureSpec"> The height requirements for this view </param>
        protected internal virtual void measureChild(View child, int parentWidthMeasureSpec, int parentHeightMeasureSpec)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final LayoutParams lp = child.getLayoutParams();
            LayoutParams lp = child.LayoutParams;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childWidthMeasureSpec = getChildMeasureSpec(parentWidthMeasureSpec, mPaddingLeft + mPaddingRight, lp.width);
            int childWidthMeasureSpec = getChildMeasureSpec(parentWidthMeasureSpec, mPaddingLeft + mPaddingRight, lp.width);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childHeightMeasureSpec = getChildMeasureSpec(parentHeightMeasureSpec, mPaddingTop + mPaddingBottom, lp.height);
            int childHeightMeasureSpec = getChildMeasureSpec(parentHeightMeasureSpec, mPaddingTop + mPaddingBottom, lp.height);

            child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
        }

        /// <summary>
        /// Ask one of the children of this view to measure itself, taking into
        /// account both the MeasureSpec requirements for this view and its padding
        /// and margins. The child must have MarginLayoutParams The heavy lifting is
        /// done in getChildMeasureSpec.
        /// </summary>
        /// <param name="child"> The child to measure </param>
        /// <param name="parentWidthMeasureSpec"> The width requirements for this view </param>
        /// <param name="widthUsed"> Extra space that has been used up by the parent
        ///        horizontally (possibly by other children of the parent) </param>
        /// <param name="parentHeightMeasureSpec"> The height requirements for this view </param>
        /// <param name="heightUsed"> Extra space that has been used up by the parent
        ///        vertically (possibly by other children of the parent) </param>
        protected internal virtual void measureChildWithMargins(View child, int parentWidthMeasureSpec, int widthUsed, int parentHeightMeasureSpec, int heightUsed)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final MarginLayoutParams lp = (MarginLayoutParams) child.getLayoutParams();
            MarginLayoutParams lp = (MarginLayoutParams) child.LayoutParams;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childWidthMeasureSpec = getChildMeasureSpec(parentWidthMeasureSpec, mPaddingLeft + mPaddingRight + lp.leftMargin + lp.rightMargin + widthUsed, lp.width);
            int childWidthMeasureSpec = getChildMeasureSpec(parentWidthMeasureSpec, mPaddingLeft + mPaddingRight + lp.leftMargin + lp.rightMargin + widthUsed, lp.width);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childHeightMeasureSpec = getChildMeasureSpec(parentHeightMeasureSpec, mPaddingTop + mPaddingBottom + lp.topMargin + lp.bottomMargin + heightUsed, lp.height);
            int childHeightMeasureSpec = getChildMeasureSpec(parentHeightMeasureSpec, mPaddingTop + mPaddingBottom + lp.topMargin + lp.bottomMargin + heightUsed, lp.height);

            child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
        }

        /// <summary>
        /// Does the hard part of measureChildren: figuring out the MeasureSpec to
        /// pass to a particular child. This method figures out the right MeasureSpec
        /// for one dimension (height or width) of one child view.
        /// 
        /// The goal is to combine information from our MeasureSpec with the
        /// LayoutParams of the child to get the best possible results. For example,
        /// if the this view knows its size (because its MeasureSpec has a mode of
        /// EXACTLY), and the child has indicated in its LayoutParams that it wants
        /// to be the same size as the parent, the parent should ask the child to
        /// layout given an exact size.
        /// </summary>
        /// <param name="spec"> The requirements for this view </param>
        /// <param name="padding"> The padding of this view for the current dimension and
        ///        margins, if applicable </param>
        /// <param name="childDimension"> How big the child wants to be in the current
        ///        dimension </param>
        /// <returns> a MeasureSpec integer for the child </returns>
        public static int getChildMeasureSpec(int spec, int padding, int childDimension)
        {
            int specMode = MeasureSpec.getMode(spec);
            int specSize = MeasureSpec.getSize(spec);

            int size = Math.Max(0, specSize - padding);

            int resultSize = 0;
            int resultMode = 0;

            switch (specMode)
            {
            // Parent has imposed an exact size on us
            case MeasureSpec.EXACTLY:
                if (childDimension >= 0)
                {
                    resultSize = childDimension;
                    resultMode = MeasureSpec.EXACTLY;
                }
                else if (childDimension == LayoutParams.MATCH_PARENT)
                {
                    // Child wants to be our size. So be it.
                    resultSize = size;
                    resultMode = MeasureSpec.EXACTLY;
                }
                else if (childDimension == LayoutParams.WRAP_CONTENT)
                {
                    // Child wants to determine its own size. It can't be
                    // bigger than us.
                    resultSize = size;
                    resultMode = MeasureSpec.AT_MOST;
                }
                break;

            // Parent has imposed a maximum size on us
            case MeasureSpec.AT_MOST:
                if (childDimension >= 0)
                {
                    // Child wants a specific size... so be it
                    resultSize = childDimension;
                    resultMode = MeasureSpec.EXACTLY;
                }
                else if (childDimension == LayoutParams.MATCH_PARENT)
                {
                    // Child wants to be our size, but our size is not fixed.
                    // Constrain child to not be bigger than us.
                    resultSize = size;
                    resultMode = MeasureSpec.AT_MOST;
                }
                else if (childDimension == LayoutParams.WRAP_CONTENT)
                {
                    // Child wants to determine its own size. It can't be
                    // bigger than us.
                    resultSize = size;
                    resultMode = MeasureSpec.AT_MOST;
                }
                break;

            // Parent asked to see how big we want to be
            case MeasureSpec.UNSPECIFIED:
                if (childDimension >= 0)
                {
                    // Child wants a specific size... let him have it
                    resultSize = childDimension;
                    resultMode = MeasureSpec.EXACTLY;
                }
                else if (childDimension == LayoutParams.MATCH_PARENT)
                {
                    // Child wants to be our size... find out how big it should
                    // be
                    resultSize = View.sUseZeroUnspecifiedMeasureSpec ? 0 : size;
                    resultMode = MeasureSpec.UNSPECIFIED;
                }
                else if (childDimension == LayoutParams.WRAP_CONTENT)
                {
                    // Child wants to determine its own size.... find out how
                    // big it should be
                    resultSize = View.sUseZeroUnspecifiedMeasureSpec ? 0 : size;
                    resultMode = MeasureSpec.UNSPECIFIED;
                }
                break;
            }
            //noinspection ResourceType
            return MeasureSpec.makeMeasureSpec(resultSize, resultMode);
        }


        /// <summary>
        /// Removes any pending animations for views that have been removed. Call
        /// this if you don't want animations for exiting views to stack up.
        /// </summary>
        public virtual void clearDisappearingChildren()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> disappearingChildren = mDisappearingChildren;
            List<View> disappearingChildren = mDisappearingChildren;
            if (disappearingChildren != null)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = disappearingChildren.size();
                int count = disappearingChildren.Count;
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View view = disappearingChildren.get(i);
                    View view = disappearingChildren[i];
                    if (view.mAttachInfo != null)
                    {
                        view.dispatchDetachedFromWindow();
                    }
                    view.clearAnimation();
                }
                disappearingChildren.Clear();
                invalidate();
            }
        }

        /// <summary>
        /// Add a view which is removed from mChildren but still needs animation
        /// </summary>
        /// <param name="v"> View to add </param>
        private void addDisappearingView(View v)
        {
            List<View> disappearingChildren = mDisappearingChildren;

            if (disappearingChildren == null)
            {
                disappearingChildren = mDisappearingChildren = new List<View>();
            }

            disappearingChildren.Add(v);
        }

        /// <summary>
        /// Cleanup a view when its animation is done. This may mean removing it from
        /// the list of disappearing views.
        /// </summary>
        /// <param name="view"> The view whose animation has finished </param>
        /// <param name="animation"> The animation, cannot be null </param>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: void finishAnimatingView(final View view, android.view.animation.Animation animation)
        internal virtual void finishAnimatingView(View view, Animation animation)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> disappearingChildren = mDisappearingChildren;
            List<View> disappearingChildren = mDisappearingChildren;
            if (disappearingChildren != null)
            {
                if (disappearingChildren.Contains(view))
                {
                    disappearingChildren.Remove(view);

                    if (view.mAttachInfo != null)
                    {
                        view.dispatchDetachedFromWindow();
                    }

                    view.clearAnimation();
                    mGroupFlags |= FLAG_INVALIDATE_REQUIRED;
                }
            }

            if (animation != null && !animation.FillAfter)
            {
                view.clearAnimation();
            }

            if ((view.mPrivateFlags & PFLAG_ANIMATION_STARTED) == PFLAG_ANIMATION_STARTED)
            {
                view.onAnimationEnd();
                // Should be performed by onAnimationEnd() but this avoid an infinite loop,
                // so we'd rather be safe than sorry
                view.mPrivateFlags &= ~PFLAG_ANIMATION_STARTED;
                // Draw one more frame after the animation is done
                mGroupFlags |= FLAG_INVALIDATE_REQUIRED;
            }
        }

        /// <summary>
        /// Utility function called by View during invalidation to determine whether a view that
        /// is invisible or gone should still be invalidated because it is being transitioned (and
        /// therefore still needs to be drawn).
        /// </summary>
        internal virtual bool isViewTransitioning(View view)
        {
            return (mTransitioningViews != null && mTransitioningViews.Contains(view));
        }

        /// <summary>
        /// This method tells the ViewGroup that the given View object, which should have this
        /// ViewGroup as its parent,
        /// should be kept around  (re-displayed when the ViewGroup draws its children) even if it
        /// is removed from its parent. This allows animations, such as those used by
        /// <seealso cref="android.app.Fragment"/> and <seealso cref="android.animation.LayoutTransition"/> to animate
        /// the removal of views. A call to this method should always be accompanied by a later call
        /// to <seealso cref="#endViewTransition(View)"/>, such as after an animation on the View has finished,
        /// so that the View finally gets removed.
        /// </summary>
        /// <param name="view"> The View object to be kept visible even if it gets removed from its parent. </param>
        public virtual void startViewTransition(View view)
        {
            if (view.mParent == this)
            {
                if (mTransitioningViews == null)
                {
                    mTransitioningViews = new List<View>();
                }
                mTransitioningViews.Add(view);
            }
        }

        /// <summary>
        /// This method should always be called following an earlier call to
        /// <seealso cref="#startViewTransition(View)"/>. The given View is finally removed from its parent
        /// and will no longer be displayed. Note that this method does not perform the functionality
        /// of removing a view from its parent; it just discontinues the display of a View that
        /// has previously been removed.
        /// </summary>
        /// <returns> view The View object that has been removed but is being kept around in the visible
        /// hierarchy by an earlier call to <seealso cref="#startViewTransition(View)"/>. </returns>
        public virtual void endViewTransition(View view)
        {
            if (mTransitioningViews != null)
            {
                mTransitioningViews.Remove(view);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> disappearingChildren = mDisappearingChildren;
                List<View> disappearingChildren = mDisappearingChildren;
                if (disappearingChildren != null && disappearingChildren.Contains(view))
                {
                    disappearingChildren.Remove(view);
                    if (mVisibilityChangingChildren != null && mVisibilityChangingChildren.Contains(view))
                    {
                        mVisibilityChangingChildren.Remove(view);
                    }
                    else
                    {
                        if (view.mAttachInfo != null)
                        {
                            view.dispatchDetachedFromWindow();
                        }
                        if (view.mParent != null)
                        {
                            view.mParent = null;
                        }
                    }
                    invalidate();
                }
            }
        }

        private LayoutTransition.TransitionListener mLayoutTransitionListener = new TransitionListenerAnonymousInnerClass();

        private class TransitionListenerAnonymousInnerClass : LayoutTransition.TransitionListener
        {
            public TransitionListenerAnonymousInnerClass()
            {
            }

            public override void startTransition(LayoutTransition transition, ViewGroup container, View view, int transitionType)
            {
                // We only care about disappearing items, since we need special logic to keep
                // those items visible after they've been 'removed'
                if (transitionType == LayoutTransition.DISAPPEARING)
                {
                    outerInstance.startViewTransition(view);
                }
            }

            public override void endTransition(LayoutTransition transition, ViewGroup container, View view, int transitionType)
            {
                if (outerInstance.mLayoutCalledWhileSuppressed && !transition.ChangingLayout)
                {
                    requestLayout();
                    outerInstance.mLayoutCalledWhileSuppressed = false;
                }
                if (transitionType == LayoutTransition.DISAPPEARING && outerInstance.mTransitioningViews != null)
                {
                    outerInstance.endViewTransition(view);
                }
            }
        }

        /// <summary>
        /// Tells this ViewGroup to suppress all layout() calls until layout
        /// suppression is disabled with a later call to suppressLayout(false).
        /// When layout suppression is disabled, a requestLayout() call is sent
        /// if layout() was attempted while layout was being suppressed.
        /// </summary>
        public virtual void suppressLayout(bool suppress)
        {
            mSuppressLayout = suppress;
            if (!suppress)
            {
                if (mLayoutCalledWhileSuppressed)
                {
                    requestLayout();
                    mLayoutCalledWhileSuppressed = false;
                }
            }
        }

        /// <summary>
        /// Returns whether layout calls on this container are currently being
        /// suppressed, due to an earlier call to <seealso cref="#suppressLayout(boolean)"/>.
        /// </summary>
        /// <returns> true if layout calls are currently suppressed, false otherwise. </returns>
        public virtual bool LayoutSuppressed
        {
            get
            {
                return mSuppressLayout;
            }
        }

        public override bool gatherTransparentRegion(Region region)
        {
            // If no transparent regions requested, we are always opaque.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean meOpaque = (mPrivateFlags & View.PFLAG_REQUEST_TRANSPARENT_REGIONS) == 0;
            bool meOpaque = (mPrivateFlags & View.PFLAG_REQUEST_TRANSPARENT_REGIONS) == 0;
            if (meOpaque && region == null)
            {
                // The caller doesn't care about the region, so stop now.
                return true;
            }
            base.gatherTransparentRegion(region);
            // Instead of naively traversing the view tree, we have to traverse according to the Z
            // order here. We need to go with the same order as dispatchDraw().
            // One example is that after surfaceView punch a hole, we will still allow other views drawn
            // on top of that hole. In this case, those other views should be able to cut the
            // transparent region into smaller area.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
            bool noneOfTheChildrenAreTransparent = true;
            if (childrenCount > 0)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildOrderedChildList();
                List<View> preorderedList = buildOrderedChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
                bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                View[] children = mChildren;
                for (int i = 0; i < childrenCount; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                    int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                    View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                    if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE || child.Animation != null)
                    {
                        if (!child.gatherTransparentRegion(region))
                        {
                            noneOfTheChildrenAreTransparent = false;
                        }
                    }
                }
                if (preorderedList != null)
                {
                    preorderedList.Clear();
                }
            }
            return meOpaque || noneOfTheChildrenAreTransparent;
        }

        public override void requestTransparentRegion(View child)
        {
            if (child != null)
            {
                child.mPrivateFlags |= View.PFLAG_REQUEST_TRANSPARENT_REGIONS;
                if (mParent != null)
                {
                    mParent.requestTransparentRegion(this);
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override void subtractObscuredTouchableRegion(Region touchableRegion, View view)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childrenCount = mChildrenCount;
            int childrenCount = mChildrenCount;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.ArrayList<View> preorderedList = buildTouchDispatchChildList();
            List<View> preorderedList = buildTouchDispatchChildList();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean customOrder = preorderedList == null && isChildrenDrawingOrderEnabled();
            bool customOrder = preorderedList == null && ChildrenDrawingOrderEnabled;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
            for (int i = childrenCount - 1; i >= 0; i--)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                if (child == view)
                {
                    // We've reached the target view.
                    break;
                }
                if (!child.canReceivePointerEvents())
                {
                    // This child cannot be touched. Skip it.
                    continue;
                }
                applyOpToRegionByBounds(touchableRegion, child, Region.Op.DIFFERENCE);
            }

            // The touchable region should not exceed the bounds of its container.
            applyOpToRegionByBounds(touchableRegion, this, Region.Op.INTERSECT);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ViewParent parent = getParent();
            ViewParent parent = Parent;
            if (parent != null)
            {
                parent.subtractObscuredTouchableRegion(touchableRegion, this);
            }
        }

        private static void applyOpToRegionByBounds(Region region, View view, Region.Op op)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] locationInWindow = new int[2];
            int[] locationInWindow = new int[2];
            view.getLocationInWindow(locationInWindow);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int x = locationInWindow[0];
            int x = locationInWindow[0];
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int y = locationInWindow[1];
            int y = locationInWindow[1];
            region.op(x, y, x + view.Width, y + view.Height, op);
        }

        public override WindowInsets dispatchApplyWindowInsets(WindowInsets insets)
        {
            insets = base.dispatchApplyWindowInsets(insets);
            if (insets.Consumed)
            {
                return insets;
            }
            if (View.sBrokenInsetsDispatch)
            {
                return brokenDispatchApplyWindowInsets(insets);
            }
            else
            {
                return newDispatchApplyWindowInsets(insets);
            }
        }

        private WindowInsets brokenDispatchApplyWindowInsets(WindowInsets insets)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                insets = getChildAt(i).dispatchApplyWindowInsets(insets);
                if (insets.Consumed)
                {
                    break;
                }
            }
            return insets;
        }

        private WindowInsets newDispatchApplyWindowInsets(WindowInsets insets)
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                getChildAt(i).dispatchApplyWindowInsets(insets);
            }
            return insets;
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override public void setWindowInsetsAnimationCallback(@Nullable WindowInsetsAnimation.Callback callback)
        public override WindowInsetsAnimation.Callback WindowInsetsAnimationCallback
        {
            set
            {
                base.WindowInsetsAnimationCallback = value;
                mInsetsAnimationDispatchMode = value != null ? value.DispatchMode : DISPATCH_MODE_CONTINUE_ON_SUBTREE;
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override bool hasWindowInsetsAnimationCallback()
        {
            if (base.hasWindowInsetsAnimationCallback())
            {
                return true;
            }

            // If we are root-level content view that fits insets, we imitate consuming behavior, so
            // no child will retrieve window insets animation callback.
            // See dispatchWindowInsetsAnimationPrepare.
            bool isOptionalFitSystemWindows = (mViewFlags & OPTIONAL_FITS_SYSTEM_WINDOWS) != 0 || FrameworkOptionalFitsSystemWindows;
            if (isOptionalFitSystemWindows && mAttachInfo != null && mAttachInfo.mContentOnApplyWindowInsetsListener != null && (WindowSystemUiVisibility & SYSTEM_UI_LAYOUT_FLAGS) == 0)
            {
                return false;
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                if (getChildAt(i).hasWindowInsetsAnimationCallback())
                {
                    return true;
                }
            }
            return false;
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override public void dispatchWindowInsetsAnimationPrepare(@NonNull WindowInsetsAnimation animation)
        public override void dispatchWindowInsetsAnimationPrepare(WindowInsetsAnimation animation)
        {
            base.dispatchWindowInsetsAnimationPrepare(animation);

            // If we are root-level content view that fits insets, set dispatch mode to stop to imitate
            // consume behavior.
            bool isOptionalFitSystemWindows = (mViewFlags & OPTIONAL_FITS_SYSTEM_WINDOWS) != 0 || FrameworkOptionalFitsSystemWindows;
            if (isOptionalFitSystemWindows && mAttachInfo != null && ListenerInfo.mWindowInsetsAnimationCallback == null && mAttachInfo.mContentOnApplyWindowInsetsListener != null && (WindowSystemUiVisibility & SYSTEM_UI_LAYOUT_FLAGS) == 0)
            {
                mInsetsAnimationDispatchMode = DISPATCH_MODE_STOP;
                return;
            }

            if (mInsetsAnimationDispatchMode == DISPATCH_MODE_STOP)
            {
                return;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                getChildAt(i).dispatchWindowInsetsAnimationPrepare(animation);
            }
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @NonNull public android.view.WindowInsetsAnimation.Bounds dispatchWindowInsetsAnimationStart(@NonNull WindowInsetsAnimation animation, @NonNull android.view.WindowInsetsAnimation.Bounds bounds)
        public override Bounds dispatchWindowInsetsAnimationStart(WindowInsetsAnimation animation, Bounds bounds)
        {
            bounds = base.dispatchWindowInsetsAnimationStart(animation, bounds);
            if (mInsetsAnimationDispatchMode == DISPATCH_MODE_STOP)
            {
                return bounds;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                getChildAt(i).dispatchWindowInsetsAnimationStart(animation, bounds);
            }
            return bounds;
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @NonNull public WindowInsets dispatchWindowInsetsAnimationProgress(@NonNull WindowInsets insets, @NonNull java.util.List<WindowInsetsAnimation> runningAnimations)
        public override WindowInsets dispatchWindowInsetsAnimationProgress(WindowInsets insets, IList<WindowInsetsAnimation> runningAnimations)
        {
            insets = base.dispatchWindowInsetsAnimationProgress(insets, runningAnimations);
            if (mInsetsAnimationDispatchMode == DISPATCH_MODE_STOP)
            {
                return insets;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                getChildAt(i).dispatchWindowInsetsAnimationProgress(insets, runningAnimations);
            }
            return insets;
        }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override public void dispatchWindowInsetsAnimationEnd(@NonNull WindowInsetsAnimation animation)
        public override void dispatchWindowInsetsAnimationEnd(WindowInsetsAnimation animation)
        {
            base.dispatchWindowInsetsAnimationEnd(animation);
            if (mInsetsAnimationDispatchMode == DISPATCH_MODE_STOP)
            {
                return;
            }
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = getChildCount();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                getChildAt(i).dispatchWindowInsetsAnimationEnd(animation);
            }
        }

        /// <summary>
        /// Offsets the given rectangle in parent's local coordinates into child's coordinate space
        /// and clips the result to the child View's bounds, padding and clipRect if appropriate. If the
        /// resulting rectangle is not empty, the request is forwarded to the child.
        /// <para>
        /// Note: This method does not account for any static View transformations which may be
        /// applied to the child view.
        /// 
        /// </para>
        /// </summary>
        /// <param name="child">            the child to dispatch to </param>
        /// <param name="localVisibleRect"> the visible (clipped) area of this ViewGroup, in local coordinates </param>
        /// <param name="windowOffset">     the offset of localVisibleRect within the window </param>
        /// <param name="targets">          a queue to collect located targets </param>
        private void dispatchTransformedScrollCaptureSearch(View child, Rect localVisibleRect, Point windowOffset, LinkedList<ScrollCaptureTarget> targets)
        {

            // copy local visible rect for modification and dispatch
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.graphics.Rect childVisibleRect = getTempRect();
            Rect childVisibleRect = TempRect;
            childVisibleRect.set(localVisibleRect);

            // transform to child coords
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.graphics.Point childWindowOffset = getTempPoint();
            Point childWindowOffset = TempPoint;
            childWindowOffset.set(windowOffset.x, windowOffset.y);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dx = child.mLeft - mScrollX;
            int dx = child.mLeft - mScrollX;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dy = child.mTop - mScrollY;
            int dy = child.mTop - mScrollY;

            childVisibleRect.offset(-dx, -dy);
            childWindowOffset.offset(dx, dy);

            bool rectIsVisible = true;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int width = mRight - mLeft;
            int width = mRight - mLeft;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int height = mBottom - mTop;
            int height = mBottom - mTop;

            // Clip to child bounds
            if (ClipChildren)
            {
                rectIsVisible = childVisibleRect.intersect(0, 0, child.Width, child.Height);
            }

            // Clip to child padding.
            if (rectIsVisible && (child is ViewGroup) && ((ViewGroup) child).ClipToPadding)
            {
                rectIsVisible = childVisibleRect.intersect(child.mPaddingLeft, child.mPaddingTop, child.Width - child.mPaddingRight, child.Height - child.mPaddingBottom);
            }
            // Clip to child clipBounds.
            if (rectIsVisible && child.mClipBounds != null)
            {
                rectIsVisible = childVisibleRect.intersect(child.mClipBounds);
            }
            if (rectIsVisible)
            {
                child.dispatchScrollCaptureSearch(childVisibleRect, childWindowOffset, targets);
            }
        }

        /// <summary>
        /// Handle the scroll capture search request by checking this view if applicable, then to each
        /// child view.
        /// </summary>
        /// <param name="localVisibleRect"> the visible area of this ViewGroup in local coordinates, according to
        ///                         the parent </param>
        /// <param name="windowOffset">     the offset of this view within the window </param>
        /// <param name="targets">          the collected list of scroll capture targets
        /// 
        /// @hide </param>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override public void dispatchScrollCaptureSearch(@NonNull android.graphics.Rect localVisibleRect, @NonNull android.graphics.Point windowOffset, @NonNull java.util.Queue<ScrollCaptureTarget> targets)
        public override void dispatchScrollCaptureSearch(Rect localVisibleRect, Point windowOffset, LinkedList<ScrollCaptureTarget> targets)
        {

            // Dispatch to self first.
            base.dispatchScrollCaptureSearch(localVisibleRect, windowOffset, targets);

            // Then dispatch to children, if not excluding descendants.
            if ((ScrollCaptureHint & SCROLL_CAPTURE_HINT_EXCLUDE_DESCENDANTS) == 0)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childCount = getChildCount();
                int childCount = ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    View child = getChildAt(i);
                    // Only visible views can be captured.
                    if (child.Visibility != View.VISIBLE)
                    {
                        continue;
                    }
                    // Transform to child coords and dispatch
                    dispatchTransformedScrollCaptureSearch(child, localVisibleRect, windowOffset, targets);
                }
            }
        }

        /// <summary>
        /// Returns the animation listener to which layout animation events are
        /// sent.
        /// </summary>
        /// <returns> an <seealso cref="android.view.animation.Animation.AnimationListener"/> </returns>
        public virtual Animation.AnimationListener LayoutAnimationListener
        {
            get
            {
                return mAnimationListener;
            }
            set
            {
                mAnimationListener = value;
            }
        }

        protected internal override void drawableStateChanged()
        {
            base.drawableStateChanged();

            if ((mGroupFlags & FLAG_NOTIFY_CHILDREN_ON_DRAWABLE_STATE_CHANGE) != 0)
            {
                if ((mGroupFlags & FLAG_ADD_STATES_FROM_CHILDREN) != 0)
                {
                    throw new System.InvalidOperationException("addStateFromChildren cannot be enabled if a" + " child has duplicateParentState set to true");
                }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
                View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
                int count = mChildrenCount;

                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = children[i];
                    View child = children[i];
                    if ((child.mViewFlags & DUPLICATE_PARENT_STATE) != 0)
                    {
                        child.refreshDrawableState();
                    }
                }
            }
        }

        public override void jumpDrawablesToCurrentState()
        {
            base.jumpDrawablesToCurrentState();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View[] children = mChildren;
            View[] children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int count = mChildrenCount;
            int count = mChildrenCount;
            for (int i = 0; i < count; i++)
            {
                children[i].jumpDrawablesToCurrentState();
            }
        }

        protected internal override int[] onCreateDrawableState(int extraSpace)
        {
            if ((mGroupFlags & FLAG_ADD_STATES_FROM_CHILDREN) == 0)
            {
                return base.onCreateDrawableState(extraSpace);
            }

            int need = 0;
            int n = ChildCount;
            for (int i = 0; i < n; i++)
            {
                int[] childState = getChildAt(i).DrawableState;

                if (childState != null)
                {
                    need += childState.Length;
                }
            }

            int[] state = base.onCreateDrawableState(extraSpace + need);

            for (int i = 0; i < n; i++)
            {
                int[] childState = getChildAt(i).DrawableState;

                if (childState != null)
                {
                    state = mergeDrawableStates(state, childState);
                }
            }

            return state;
        }

        /// <summary>
        /// Sets whether this ViewGroup's drawable states also include
        /// its children's drawable states.  This is used, for example, to
        /// make a group appear to be focused when its child EditText or button
        /// is focused.
        /// </summary>
        public virtual bool AddStatesFromChildren
        {
            set
            {
                if (value)
                {
                    mGroupFlags |= FLAG_ADD_STATES_FROM_CHILDREN;
                }
                else
                {
                    mGroupFlags &= ~FLAG_ADD_STATES_FROM_CHILDREN;
                }
    
                refreshDrawableState();
            }
        }

        /// <summary>
        /// Returns whether this ViewGroup's drawable states also include
        /// its children's drawable states.  This is used, for example, to
        /// make a group appear to be focused when its child EditText or button
        /// is focused.
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @InspectableProperty public boolean addStatesFromChildren()
        public virtual bool addStatesFromChildren()
        {
            return (mGroupFlags & FLAG_ADD_STATES_FROM_CHILDREN) != 0;
        }

        /// <summary>
        /// If <seealso cref="#addStatesFromChildren"/> is true, refreshes this group's
        /// drawable state (to include the states from its children).
        /// </summary>
        public override void childDrawableStateChanged(View child)
        {
            if ((mGroupFlags & FLAG_ADD_STATES_FROM_CHILDREN) != 0)
            {
                refreshDrawableState();
            }
        }


        /// <summary>
        /// This method is called by LayoutTransition when there are 'changing' animations that need
        /// to start after the layout/setup phase. The request is forwarded to the ViewAncestor, who
        /// starts all pending transitions prior to the drawing phase in the current traversal.
        /// </summary>
        /// <param name="transition"> The LayoutTransition to be started on the next traversal.
        /// 
        /// @hide </param>
        public virtual void requestTransitionStart(LayoutTransition transition)
        {
            ViewRootImpl viewAncestor = ViewRootImpl;
            if (viewAncestor != null)
            {
                viewAncestor.requestTransitionStart(transition);
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override bool resolveRtlPropertiesIfNeeded()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean result = super.resolveRtlPropertiesIfNeeded();
            bool result = base.resolveRtlPropertiesIfNeeded();
            // We dont need to resolve the children RTL properties if nothing has changed for the parent
            if (result)
            {
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                    View child = getChildAt(i);
                    if (child.LayoutDirectionInherited)
                    {
                        child.resolveRtlPropertiesIfNeeded();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override bool resolveLayoutDirection()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean result = super.resolveLayoutDirection();
            bool result = base.resolveLayoutDirection();
            if (result)
            {
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                    View child = getChildAt(i);
                    if (child.LayoutDirectionInherited)
                    {
                        child.resolveLayoutDirection();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override bool resolveTextDirection()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean result = super.resolveTextDirection();
            bool result = base.resolveTextDirection();
            if (result)
            {
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                    View child = getChildAt(i);
                    if (child.TextDirectionInherited)
                    {
                        child.resolveTextDirection();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override bool resolveTextAlignment()
        {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean result = super.resolveTextAlignment();
            bool result = base.resolveTextAlignment();
            if (result)
            {
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                    View child = getChildAt(i);
                    if (child.TextAlignmentInherited)
                    {
                        child.resolveTextAlignment();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @UnsupportedAppUsage public void resolvePadding()
        public override void resolvePadding()
        {
            base.resolvePadding();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                View child = getChildAt(i);
                if (child.LayoutDirectionInherited && !child.PaddingResolved)
                {
                    child.resolvePadding();
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
        protected internal override void resolveDrawables()
        {
            base.resolveDrawables();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                View child = getChildAt(i);
                if (child.LayoutDirectionInherited && !child.areDrawablesResolved())
                {
                    child.resolveDrawables();
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
        public override void resolveLayoutParams()
        {
            base.resolveLayoutParams();
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                View child = getChildAt(i);
                child.resolveLayoutParams();
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TestApi @Override public void resetResolvedLayoutDirection()
        public override void resetResolvedLayoutDirection()
        {
            base.resetResolvedLayoutDirection();

            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                View child = getChildAt(i);
                if (child.LayoutDirectionInherited)
                {
                    child.resetResolvedLayoutDirection();
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TestApi @Override public void resetResolvedTextDirection()
        public override void resetResolvedTextDirection()
        {
            base.resetResolvedTextDirection();

            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                View child = getChildAt(i);
                if (child.TextDirectionInherited)
                {
                    child.resetResolvedTextDirection();
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TestApi @Override public void resetResolvedTextAlignment()
        public override void resetResolvedTextAlignment()
        {
            base.resetResolvedTextAlignment();

            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                View child = getChildAt(i);
                if (child.TextAlignmentInherited)
                {
                    child.resetResolvedTextAlignment();
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TestApi @Override public void resetResolvedPadding()
        public override void resetResolvedPadding()
        {
            base.resetResolvedPadding();

            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                View child = getChildAt(i);
                if (child.LayoutDirectionInherited)
                {
                    child.resetResolvedPadding();
                }
            }
        }

        /// <summary>
        /// @hide
        /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @TestApi @Override protected void resetResolvedDrawables()
        protected internal override void resetResolvedDrawables()
        {
            base.resetResolvedDrawables();

            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = getChildAt(i);
                View child = getChildAt(i);
                if (child.LayoutDirectionInherited)
                {
                    child.resetResolvedDrawables();
                }
            }
        }

        /// <summary>
        /// Return true if the pressed state should be delayed for children or descendants of this
        /// ViewGroup. Generally, this should be done for containers that can scroll, such as a List.
        /// This prevents the pressed state from appearing when the user is actually trying to scroll
        /// the content.
        /// 
        /// The default implementation returns true for compatibility reasons. Subclasses that do
        /// not scroll should generally override this method and return false.
        /// </summary>
        public virtual bool shouldDelayChildPressedState()
        {
            return true;
        }

        /// <summary>
        /// @inheritDoc
        /// </summary>
        public override bool onStartNestedScroll(View child, View target, int nestedScrollAxes)
        {
            return false;
        }

        /// <summary>
        /// @inheritDoc
        /// </summary>
        public override void onNestedScrollAccepted(View child, View target, int axes)
        {
            mNestedScrollAxes = axes;
        }

        /// <summary>
        /// @inheritDoc
        /// 
        /// <para>The default implementation of onStopNestedScroll calls
        /// <seealso cref="#stopNestedScroll()"/> to halt any recursive nested scrolling in progress.</para>
        /// </summary>
        public override void onStopNestedScroll(View child)
        {
            // Stop any recursive nested scrolling.
            stopNestedScroll();
            mNestedScrollAxes = 0;
        }

        /// <summary>
        /// @inheritDoc
        /// </summary>
        public override void onNestedScroll(View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
        {
            // Re-dispatch up the tree by default
            dispatchNestedScroll(dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed, null);
        }

        /// <summary>
        /// @inheritDoc
        /// </summary>
        public override void onNestedPreScroll(View target, int dx, int dy, int[] consumed)
        {
            // Re-dispatch up the tree by default
            dispatchNestedPreScroll(dx, dy, consumed, null);
        }

        /// <summary>
        /// @inheritDoc
        /// </summary>
        public override bool onNestedFling(View target, float velocityX, float velocityY, bool consumed)
        {
            // Re-dispatch up the tree by default
            return dispatchNestedFling(velocityX, velocityY, consumed);
        }

        /// <summary>
        /// @inheritDoc
        /// </summary>
        public override bool onNestedPreFling(View target, float velocityX, float velocityY)
        {
            // Re-dispatch up the tree by default
            return dispatchNestedPreFling(velocityX, velocityY);
        }

        /// <summary>
        /// Return the current axes of nested scrolling for this ViewGroup.
        /// 
        /// <para>A ViewGroup returning something other than <seealso cref="#SCROLL_AXIS_NONE"/> is currently
        /// acting as a nested scrolling parent for one or more descendant views in the hierarchy.</para>
        /// </summary>
        /// <returns> Flags indicating the current axes of nested scrolling </returns>
        /// <seealso cref= #SCROLL_AXIS_HORIZONTAL </seealso>
        /// <seealso cref= #SCROLL_AXIS_VERTICAL </seealso>
        /// <seealso cref= #SCROLL_AXIS_NONE </seealso>
        public virtual int NestedScrollAxes
        {
            get
            {
                return mNestedScrollAxes;
            }
        }

        /// <summary>
        /// @hide </summary>
        protected internal virtual void onSetLayoutParams(View child, LayoutParams layoutParams)
        {
            requestLayout();
        }

        /// <summary>
        /// @hide </summary>
        public override void captureTransitioningViews(IList<View> transitioningViews)
        {
            if (Visibility != View.VISIBLE)
            {
                return;
            }
            if (TransitionGroup)
            {
                transitioningViews.Add(this);
            }
            else
            {
                int count = ChildCount;
                for (int i = 0; i < count; i++)
                {
                    View child = getChildAt(i);
                    child.captureTransitioningViews(transitioningViews);
                }
            }
        }

        /// <summary>
        /// @hide </summary>
        public override void findNamedViews(IDictionary<string, View> namedElements)
        {
            if (Visibility != VISIBLE && mGhostView == null)
            {
                return;
            }
            base.findNamedViews(namedElements);
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                View child = getChildAt(i);
                child.findNamedViews(namedElements);
            }
        }

        internal override bool hasUnhandledKeyListener()
        {
            return (mChildUnhandledKeyListeners > 0) || base.hasUnhandledKeyListener();
        }

        internal virtual void incrementChildUnhandledKeyListeners()
        {
            mChildUnhandledKeyListeners += 1;
            if (mChildUnhandledKeyListeners == 1)
            {
                if (mParent is ViewGroup)
                {
                    ((ViewGroup) mParent).incrementChildUnhandledKeyListeners();
                }
            }
        }

        internal virtual void decrementChildUnhandledKeyListeners()
        {
            mChildUnhandledKeyListeners -= 1;
            if (mChildUnhandledKeyListeners == 0)
            {
                if (mParent is ViewGroup)
                {
                    ((ViewGroup) mParent).decrementChildUnhandledKeyListeners();
                }
            }
        }

        internal override View dispatchUnhandledKeyEvent(KeyEvent evt)
        {
            if (!hasUnhandledKeyListener())
            {
                return null;
            }
            List<View> orderedViews = buildOrderedChildList();
            if (orderedViews != null)
            {
                try
                {
                    for (int i = orderedViews.Count - 1; i >= 0; --i)
                    {
                        View v = orderedViews[i];
                        View consumer = v.dispatchUnhandledKeyEvent(evt);
                        if (consumer != null)
                        {
                            return consumer;
                        }
                    }
                }
                finally
                {
                    orderedViews.Clear();
                }
            }
            else
            {
                for (int i = ChildCount - 1; i >= 0; --i)
                {
                    View v = getChildAt(i);
                    View consumer = v.dispatchUnhandledKeyEvent(evt);
                    if (consumer != null)
                    {
                        return consumer;
                    }
                }
            }
            if (onUnhandledKeyEvent(evt))
            {
                return this;
            }
            return null;
        }

        /// <summary>
        /// LayoutParams are used by views to tell their parents how they want to be
        /// laid out. See
        /// <seealso cref="android.R.styleable#ViewGroup_Layout ViewGroup Layout Attributes"/>
        /// for a list of all child view attributes that this class supports.
        /// 
        /// <para>
        /// The base LayoutParams class just describes how big the view wants to be
        /// for both width and height. For each dimension, it can specify one of:
        /// <ul>
        /// <li>FILL_PARENT (renamed MATCH_PARENT in API Level 8 and higher), which
        /// means that the view wants to be as big as its parent (minus padding)
        /// <li> WRAP_CONTENT, which means that the view wants to be just big enough
        /// to enclose its content (plus padding)
        /// <li> an exact number
        /// </ul>
        /// There are subclasses of LayoutParams for different subclasses of
        /// ViewGroup. For example, AbsoluteLayout has its own subclass of
        /// LayoutParams which adds an X and Y value.</para>
        /// 
        /// <div class="special reference">
        /// <h3>Developer Guides</h3>
        /// <para>For more information about creating user interface layouts, read the
        /// <a href="{@docRoot}guide/topics/ui/declaring-layout.html">XML Layouts</a> developer
        /// guide.</para></div>
        /// 
        /// @attr ref android.R.styleable#ViewGroup_Layout_layout_height
        /// @attr ref android.R.styleable#ViewGroup_Layout_layout_width
        /// </summary>
        public class LayoutParams
        {
            /// <summary>
            /// Special value for the height or width requested by a View.
            /// FILL_PARENT means that the view wants to be as big as its parent,
            /// minus the parent's padding, if any. This value is deprecated
            /// starting in API Level 8 and replaced by <seealso cref="#MATCH_PARENT"/>.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({"UnusedDeclaration"}) @Deprecated public static final int FILL_PARENT = -1;
            [Obsolete]
            public const int FILL_PARENT = -1;

            /// <summary>
            /// Special value for the height or width requested by a View.
            /// MATCH_PARENT means that the view wants to be as big as its parent,
            /// minus the parent's padding, if any. Introduced in API Level 8.
            /// </summary>
            public const int MATCH_PARENT = -1;

            /// <summary>
            /// Special value for the height or width requested by a View.
            /// WRAP_CONTENT means that the view wants to be just large enough to fit
            /// its own internal content, taking its own padding into account.
            /// </summary>
            public const int WRAP_CONTENT = -2;

            /// <summary>
            /// Information about how wide the view wants to be. Can be one of the
            /// constants FILL_PARENT (replaced by MATCH_PARENT
            /// in API Level 8) or WRAP_CONTENT, or an exact size.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout", mapping = { @ViewDebug.IntToString(from = MATCH_PARENT, to = "MATCH_PARENT"), @ViewDebug.IntToString(from = WRAP_CONTENT, to = "WRAP_CONTENT") }) @InspectableProperty(name = "layout_width", enumMapping = { @EnumEntry(name = "match_parent", value = MATCH_PARENT), @EnumEntry(name = "wrap_content", value = WRAP_CONTENT) }) public int width;
            public int width;

            /// <summary>
            /// Information about how tall the view wants to be. Can be one of the
            /// constants FILL_PARENT (replaced by MATCH_PARENT
            /// in API Level 8) or WRAP_CONTENT, or an exact size.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout", mapping = { @ViewDebug.IntToString(from = MATCH_PARENT, to = "MATCH_PARENT"), @ViewDebug.IntToString(from = WRAP_CONTENT, to = "WRAP_CONTENT") }) @InspectableProperty(name = "layout_height", enumMapping = { @EnumEntry(name = "match_parent", value = MATCH_PARENT), @EnumEntry(name = "wrap_content", value = WRAP_CONTENT) }) public int height;
            public int height;

            /// <summary>
            /// Used to animate layouts.
            /// </summary>
            public LayoutAnimationController.AnimationParameters layoutAnimationParameters;

            /// <summary>
            /// Creates a new set of layout parameters. The values are extracted from
            /// the supplied attributes set and context. The XML attributes mapped
            /// to this set of layout parameters are:
            /// 
            /// <ul>
            ///   <li><code>layout_width</code>: the width, either an exact value,
            ///   <seealso cref="#WRAP_CONTENT"/>, or <seealso cref="#FILL_PARENT"/> (replaced by
            ///   <seealso cref="#MATCH_PARENT"/> in API Level 8)</li>
            ///   <li><code>layout_height</code>: the height, either an exact value,
            ///   <seealso cref="#WRAP_CONTENT"/>, or <seealso cref="#FILL_PARENT"/> (replaced by
            ///   <seealso cref="#MATCH_PARENT"/> in API Level 8)</li>
            /// </ul>
            /// </summary>
            /// <param name="c"> the application environment </param>
            /// <param name="attrs"> the set of attributes from which to extract the layout
            ///              parameters' values </param>
            public LayoutParams(Context c, AttributeSet attrs)
            {
                TypedArray a = c.obtainStyledAttributes(attrs, R.styleable.ViewGroup_Layout);
                setBaseAttributes(a, R.styleable.ViewGroup_Layout_layout_width, R.styleable.ViewGroup_Layout_layout_height);
                a.recycle();
            }

            /// <summary>
            /// Creates a new set of layout parameters with the specified width
            /// and height.
            /// </summary>
            /// <param name="width"> the width, either <seealso cref="#WRAP_CONTENT"/>,
            ///        <seealso cref="#FILL_PARENT"/> (replaced by <seealso cref="#MATCH_PARENT"/> in
            ///        API Level 8), or a fixed size in pixels </param>
            /// <param name="height"> the height, either <seealso cref="#WRAP_CONTENT"/>,
            ///        <seealso cref="#FILL_PARENT"/> (replaced by <seealso cref="#MATCH_PARENT"/> in
            ///        API Level 8), or a fixed size in pixels </param>
            public LayoutParams(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            /// <summary>
            /// Copy constructor. Clones the width and height values of the source.
            /// </summary>
            /// <param name="source"> The layout params to copy from. </param>
            public LayoutParams(LayoutParams source)
            {
                this.width = source.width;
                this.height = source.height;
            }

            /// <summary>
            /// Used internally by MarginLayoutParams.
            /// @hide
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage LayoutParams()
            internal LayoutParams()
            {
            }

            /// <summary>
            /// Extracts the layout parameters from the supplied attributes.
            /// </summary>
            /// <param name="a"> the style attributes to extract the parameters from </param>
            /// <param name="widthAttr"> the identifier of the width attribute </param>
            /// <param name="heightAttr"> the identifier of the height attribute </param>
            protected internal virtual void setBaseAttributes(TypedArray a, int widthAttr, int heightAttr)
            {
                width = a.getLayoutDimension(widthAttr, "layout_width");
                height = a.getLayoutDimension(heightAttr, "layout_height");
            }

            /// <summary>
            /// Resolve layout parameters depending on the layout direction. Subclasses that care about
            /// layoutDirection changes should override this method. The default implementation does
            /// nothing.
            /// </summary>
            /// <param name="layoutDirection"> the direction of the layout
            /// 
            /// <seealso cref="View#LAYOUT_DIRECTION_LTR"/>
            /// <seealso cref="View#LAYOUT_DIRECTION_RTL"/> </param>
            public virtual void resolveLayoutDirection(int layoutDirection)
            {
            }

            /// <summary>
            /// Returns a String representation of this set of layout parameters.
            /// </summary>
            /// <param name="output"> the String to prepend to the internal representation </param>
            /// <returns> a String with the following format: output +
            ///         "ViewGroup.LayoutParams={ width=WIDTH, height=HEIGHT }"
            /// 
            /// @hide </returns>
            public virtual string debug(string output)
            {
                return output + "ViewGroup.LayoutParams={ width=" + sizeToString(width) + ", height=" + sizeToString(height) + " }";
            }

            /// <summary>
            /// Use {@code canvas} to draw suitable debugging annotations for these LayoutParameters.
            /// </summary>
            /// <param name="view"> the view that contains these layout parameters </param>
            /// <param name="canvas"> the canvas on which to draw
            /// 
            /// @hide </param>
            public virtual void onDebugDraw(View view, Canvas canvas, Paint paint)
            {
            }

            /// <summary>
            /// Converts the specified size to a readable String.
            /// </summary>
            /// <param name="size"> the size to convert </param>
            /// <returns> a String instance representing the supplied size
            /// 
            /// @hide </returns>
            protected internal static string sizeToString(int size)
            {
                if (size == WRAP_CONTENT)
                {
                    return "wrap-content";
                }
                if (size == MATCH_PARENT)
                {
                    return "match-parent";
                }
                return size.ToString();
            }

            /// <summary>
            /// @hide </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: void encode(@NonNull ViewHierarchyEncoder encoder)
            internal virtual void encode(ViewHierarchyEncoder encoder)
            {
                encoder.beginObject(this);
                encodeProperties(encoder);
                encoder.endObject();
            }

            /// <summary>
            /// @hide </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: protected void encodeProperties(@NonNull ViewHierarchyEncoder encoder)
            protected internal virtual void encodeProperties(ViewHierarchyEncoder encoder)
            {
                encoder.addProperty("width", width);
                encoder.addProperty("height", height);
            }
        }

        /// <summary>
        /// Per-child layout information for layouts that support margins.
        /// See
        /// <seealso cref="android.R.styleable#ViewGroup_MarginLayout ViewGroup Margin Layout Attributes"/>
        /// for a list of all child view attributes that this class supports.
        /// 
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_margin
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginHorizontal
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginVertical
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginLeft
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginTop
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginRight
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginBottom
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginStart
        /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginEnd
        /// </summary>
        public class MarginLayoutParams : ViewGroup.LayoutParams
        {
            /// <summary>
            /// The left margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @InspectableProperty(name = "layout_marginLeft") public int leftMargin;
            public int leftMargin;

            /// <summary>
            /// The top margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @InspectableProperty(name = "layout_marginTop") public int topMargin;
            public int topMargin;

            /// <summary>
            /// The right margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @InspectableProperty(name = "layout_marginRight") public int rightMargin;
            public int rightMargin;

            /// <summary>
            /// The bottom margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @InspectableProperty(name = "layout_marginBottom") public int bottomMargin;
            public int bottomMargin;

            /// <summary>
            /// The start margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @UnsupportedAppUsage private int startMargin = DEFAULT_MARGIN_RELATIVE;
            internal int startMargin = DEFAULT_MARGIN_RELATIVE;

            /// <summary>
            /// The end margin in pixels of the child. Margin values should be positive.
            /// Call <seealso cref="ViewGroup#setLayoutParams(LayoutParams)"/> after reassigning a new value
            /// to this field.
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout") @UnsupportedAppUsage private int endMargin = DEFAULT_MARGIN_RELATIVE;
            internal int endMargin = DEFAULT_MARGIN_RELATIVE;

            /// <summary>
            /// The default start and end margin.
            /// @hide
            /// </summary>
            public static readonly int DEFAULT_MARGIN_RELATIVE = int.MinValue;

            /// <summary>
            /// Bit  0: layout direction
            /// Bit  1: layout direction
            /// Bit  2: left margin undefined
            /// Bit  3: right margin undefined
            /// Bit  4: is RTL compatibility mode
            /// Bit  5: need resolution
            /// 
            /// Bit 6 to 7 not used
            /// 
            /// @hide
            /// </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ViewDebug.ExportedProperty(category = "layout", flagMapping = { @ViewDebug.FlagToString(mask = LAYOUT_DIRECTION_MASK, equals = LAYOUT_DIRECTION_MASK, name = "LAYOUT_DIRECTION"), @ViewDebug.FlagToString(mask = LEFT_MARGIN_UNDEFINED_MASK, equals = LEFT_MARGIN_UNDEFINED_MASK, name = "LEFT_MARGIN_UNDEFINED_MASK"), @ViewDebug.FlagToString(mask = RIGHT_MARGIN_UNDEFINED_MASK, equals = RIGHT_MARGIN_UNDEFINED_MASK, name = "RIGHT_MARGIN_UNDEFINED_MASK"), @ViewDebug.FlagToString(mask = RTL_COMPATIBILITY_MODE_MASK, equals = RTL_COMPATIBILITY_MODE_MASK, name = "RTL_COMPATIBILITY_MODE_MASK"), @ViewDebug.FlagToString(mask = NEED_RESOLUTION_MASK, equals = NEED_RESOLUTION_MASK, name = "NEED_RESOLUTION_MASK") }, formatToHexString = true) byte mMarginFlags;
            internal sbyte mMarginFlags;

            internal const int LAYOUT_DIRECTION_MASK = 0x00000003;
            internal const int LEFT_MARGIN_UNDEFINED_MASK = 0x00000004;
            internal const int RIGHT_MARGIN_UNDEFINED_MASK = 0x00000008;
            internal const int RTL_COMPATIBILITY_MODE_MASK = 0x00000010;
            internal const int NEED_RESOLUTION_MASK = 0x00000020;

            internal const int DEFAULT_MARGIN_RESOLVED = 0;
            internal static readonly int UNDEFINED_MARGIN = DEFAULT_MARGIN_RELATIVE;

            /// <summary>
            /// Creates a new set of layout parameters. The values are extracted from
            /// the supplied attributes set and context.
            /// </summary>
            /// <param name="c"> the application environment </param>
            /// <param name="attrs"> the set of attributes from which to extract the layout
            ///              parameters' values </param>
            public MarginLayoutParams(Context c, AttributeSet attrs) : base()
            {

                TypedArray a = c.obtainStyledAttributes(attrs, R.styleable.ViewGroup_MarginLayout);
                setBaseAttributes(a, R.styleable.ViewGroup_MarginLayout_layout_width, R.styleable.ViewGroup_MarginLayout_layout_height);

                int margin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_margin, -1);
                if (margin >= 0)
                {
                    leftMargin = margin;
                    topMargin = margin;
                    rightMargin = margin;
                    bottomMargin = margin;
                }
                else
                {
                    int horizontalMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginHorizontal, -1);
                    int verticalMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginVertical, -1);

                    if (horizontalMargin >= 0)
                    {
                        leftMargin = horizontalMargin;
                        rightMargin = horizontalMargin;
                    }
                    else
                    {
                        leftMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginLeft, UNDEFINED_MARGIN);
                        if (leftMargin == UNDEFINED_MARGIN)
                        {
                            mMarginFlags |= (sbyte)LEFT_MARGIN_UNDEFINED_MASK;
                            leftMargin = DEFAULT_MARGIN_RESOLVED;
                        }
                        rightMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginRight, UNDEFINED_MARGIN);
                        if (rightMargin == UNDEFINED_MARGIN)
                        {
                            mMarginFlags |= (sbyte)RIGHT_MARGIN_UNDEFINED_MASK;
                            rightMargin = DEFAULT_MARGIN_RESOLVED;
                        }
                    }

                    startMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginStart, DEFAULT_MARGIN_RELATIVE);
                    endMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginEnd, DEFAULT_MARGIN_RELATIVE);

                    if (verticalMargin >= 0)
                    {
                        topMargin = verticalMargin;
                        bottomMargin = verticalMargin;
                    }
                    else
                    {
                        topMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginTop, DEFAULT_MARGIN_RESOLVED);
                        bottomMargin = a.getDimensionPixelSize(R.styleable.ViewGroup_MarginLayout_layout_marginBottom, DEFAULT_MARGIN_RESOLVED);
                    }

                    if (MarginRelative)
                    {
                       mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                    }
                }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hasRtlSupport = c.getApplicationInfo().hasRtlSupport();
                bool hasRtlSupport = c.ApplicationInfo.hasRtlSupport();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int targetSdkVersion = c.getApplicationInfo().targetSdkVersion;
                int targetSdkVersion = c.ApplicationInfo.targetSdkVersion;
                if (targetSdkVersion < JELLY_BEAN_MR1 || !hasRtlSupport)
                {
                    mMarginFlags |= (sbyte)RTL_COMPATIBILITY_MODE_MASK;
                }

                // Layout direction is LTR by default
                mMarginFlags |= LAYOUT_DIRECTION_LTR;

                a.recycle();
            }

            public MarginLayoutParams(int width, int height) : base(width, height)
            {

                mMarginFlags |= (sbyte)LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags |= (sbyte)RIGHT_MARGIN_UNDEFINED_MASK;

                mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
                mMarginFlags &= (sbyte)(~RTL_COMPATIBILITY_MODE_MASK);
            }

            /// <summary>
            /// Copy constructor. Clones the width, height and margin values of the source.
            /// </summary>
            /// <param name="source"> The layout params to copy from. </param>
            public MarginLayoutParams(MarginLayoutParams source)
            {
                this.width = source.width;
                this.height = source.height;

                this.leftMargin = source.leftMargin;
                this.topMargin = source.topMargin;
                this.rightMargin = source.rightMargin;
                this.bottomMargin = source.bottomMargin;
                this.startMargin = source.startMargin;
                this.endMargin = source.endMargin;

                this.mMarginFlags = source.mMarginFlags;
            }

            public MarginLayoutParams(LayoutParams source) : base(source)
            {

                mMarginFlags |= (sbyte)LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags |= (sbyte)RIGHT_MARGIN_UNDEFINED_MASK;

                mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
                mMarginFlags &= (sbyte)(~RTL_COMPATIBILITY_MODE_MASK);
            }

            /// <summary>
            /// @hide Used internally.
            /// </summary>
            public void copyMarginsFrom(MarginLayoutParams source)
            {
                this.leftMargin = source.leftMargin;
                this.topMargin = source.topMargin;
                this.rightMargin = source.rightMargin;
                this.bottomMargin = source.bottomMargin;
                this.startMargin = source.startMargin;
                this.endMargin = source.endMargin;

                this.mMarginFlags = source.mMarginFlags;
            }

            /// <summary>
            /// Sets the margins, in pixels. A call to <seealso cref="android.view.View#requestLayout()"/> needs
            /// to be done so that the new margins are taken into account. Left and right margins may be
            /// overridden by <seealso cref="android.view.View#requestLayout()"/> depending on layout direction.
            /// Margin values should be positive.
            /// </summary>
            /// <param name="left"> the left margin size </param>
            /// <param name="top"> the top margin size </param>
            /// <param name="right"> the right margin size </param>
            /// <param name="bottom"> the bottom margin size
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginLeft
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginTop
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginRight
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginBottom </param>
            public virtual void setMargins(int left, int top, int right, int bottom)
            {
                leftMargin = left;
                topMargin = top;
                rightMargin = right;
                bottomMargin = bottom;
                mMarginFlags &= (sbyte)(~LEFT_MARGIN_UNDEFINED_MASK);
                mMarginFlags &= (sbyte)(~RIGHT_MARGIN_UNDEFINED_MASK);
                if (MarginRelative)
                {
                    mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                }
                else
                {
                    mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
                }
            }

            /// <summary>
            /// Sets the relative margins, in pixels. A call to <seealso cref="android.view.View#requestLayout()"/>
            /// needs to be done so that the new relative margins are taken into account. Left and right
            /// margins may be overridden by <seealso cref="android.view.View#requestLayout()"/> depending on
            /// layout direction. Margin values should be positive.
            /// </summary>
            /// <param name="start"> the start margin size </param>
            /// <param name="top"> the top margin size </param>
            /// <param name="end"> the right margin size </param>
            /// <param name="bottom"> the bottom margin size
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginStart
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginTop
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginEnd
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginBottom
            /// 
            /// @hide </param>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage public void setMarginsRelative(int start, int top, int end, int bottom)
            public virtual void setMarginsRelative(int start, int top, int end, int bottom)
            {
                startMargin = start;
                topMargin = top;
                endMargin = end;
                bottomMargin = bottom;
                mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
            }

            /// <summary>
            /// Sets the relative start margin. Margin values should be positive.
            /// </summary>
            /// <param name="start"> the start margin size
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginStart </param>
            public virtual int MarginStart
            {
                set
                {
                    startMargin = value;
                    mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                }
                get
                {
                    if (startMargin != DEFAULT_MARGIN_RELATIVE)
                    {
                        return startMargin;
                    }
                    if ((mMarginFlags & NEED_RESOLUTION_MASK) == NEED_RESOLUTION_MASK)
                    {
                        doResolveMargins();
                    }
                    switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                    {
                        case View.LAYOUT_DIRECTION_RTL:
                            return rightMargin;
                        case View.LAYOUT_DIRECTION_LTR:
                        default:
                            return leftMargin;
                    }
                }
            }


            /// <summary>
            /// Sets the relative end margin. Margin values should be positive.
            /// </summary>
            /// <param name="end"> the end margin size
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginEnd </param>
            public virtual int MarginEnd
            {
                set
                {
                    endMargin = value;
                    mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                }
                get
                {
                    if (endMargin != DEFAULT_MARGIN_RELATIVE)
                    {
                        return endMargin;
                    }
                    if ((mMarginFlags & NEED_RESOLUTION_MASK) == NEED_RESOLUTION_MASK)
                    {
                        doResolveMargins();
                    }
                    switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                    {
                        case View.LAYOUT_DIRECTION_RTL:
                            return leftMargin;
                        case View.LAYOUT_DIRECTION_LTR:
                        default:
                            return rightMargin;
                    }
                }
            }


            /// <summary>
            /// Check if margins are relative.
            /// 
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginStart
            /// @attr ref android.R.styleable#ViewGroup_MarginLayout_layout_marginEnd
            /// </summary>
            /// <returns> true if either marginStart or marginEnd has been set. </returns>
            public virtual bool MarginRelative
            {
                get
                {
                    return (startMargin != DEFAULT_MARGIN_RELATIVE || endMargin != DEFAULT_MARGIN_RELATIVE);
                }
            }

            /// <summary>
            /// Set the layout direction </summary>
            /// <param name="layoutDirection"> the layout direction.
            ///        Should be either <seealso cref="View#LAYOUT_DIRECTION_LTR"/>
            ///                     or <seealso cref="View#LAYOUT_DIRECTION_RTL"/>. </param>
            public virtual int LayoutDirection
            {
                set
                {
                    if (value != View.LAYOUT_DIRECTION_LTR && value != View.LAYOUT_DIRECTION_RTL)
                    {
                        return;
                    }
                    if (value != (mMarginFlags & LAYOUT_DIRECTION_MASK))
                    {
                        mMarginFlags &= (sbyte)(~LAYOUT_DIRECTION_MASK);
                        mMarginFlags |= (sbyte)(value & LAYOUT_DIRECTION_MASK);
                        if (MarginRelative)
                        {
                            mMarginFlags |= (sbyte)NEED_RESOLUTION_MASK;
                        }
                        else
                        {
                            mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
                        }
                    }
                }
                get
                {
                    return (mMarginFlags & LAYOUT_DIRECTION_MASK);
                }
            }


            /// <summary>
            /// This will be called by <seealso cref="android.view.View#requestLayout()"/>. Left and Right margins
            /// may be overridden depending on layout direction.
            /// </summary>
            public override void resolveLayoutDirection(int layoutDirection)
            {
                LayoutDirection = layoutDirection;

                // No relative margin or pre JB-MR1 case or no need to resolve, just dont do anything
                // Will use the left and right margins if no relative margin is defined.
                if (!MarginRelative || (mMarginFlags & NEED_RESOLUTION_MASK) != NEED_RESOLUTION_MASK)
                {
                    return;
                }

                // Proceed with resolution
                doResolveMargins();
            }

            internal virtual void doResolveMargins()
            {
                if ((mMarginFlags & RTL_COMPATIBILITY_MODE_MASK) == RTL_COMPATIBILITY_MODE_MASK)
                {
                    // if left or right margins are not defined and if we have some start or end margin
                    // defined then use those start and end margins.
                    if ((mMarginFlags & LEFT_MARGIN_UNDEFINED_MASK) == LEFT_MARGIN_UNDEFINED_MASK && startMargin > DEFAULT_MARGIN_RELATIVE)
                    {
                        leftMargin = startMargin;
                    }
                    if ((mMarginFlags & RIGHT_MARGIN_UNDEFINED_MASK) == RIGHT_MARGIN_UNDEFINED_MASK && endMargin > DEFAULT_MARGIN_RELATIVE)
                    {
                        rightMargin = endMargin;
                    }
                }
                else
                {
                    // We have some relative margins (either the start one or the end one or both). So use
                    // them and override what has been defined for left and right margins. If either start
                    // or end margin is not defined, just set it to default "0".
                    switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                    {
                        case View.LAYOUT_DIRECTION_RTL:
                            leftMargin = (endMargin > DEFAULT_MARGIN_RELATIVE) ? endMargin : DEFAULT_MARGIN_RESOLVED;
                            rightMargin = (startMargin > DEFAULT_MARGIN_RELATIVE) ? startMargin : DEFAULT_MARGIN_RESOLVED;
                            break;
                        case View.LAYOUT_DIRECTION_LTR:
                        default:
                            leftMargin = (startMargin > DEFAULT_MARGIN_RELATIVE) ? startMargin : DEFAULT_MARGIN_RESOLVED;
                            rightMargin = (endMargin > DEFAULT_MARGIN_RELATIVE) ? endMargin : DEFAULT_MARGIN_RESOLVED;
                            break;
                    }
                }
                mMarginFlags &= (sbyte)(~NEED_RESOLUTION_MASK);
            }

            /// <summary>
            /// @hide
            /// </summary>
            public virtual bool LayoutRtl
            {
                get
                {
                    return ((mMarginFlags & LAYOUT_DIRECTION_MASK) == View.LAYOUT_DIRECTION_RTL);
                }
            }

            /// <summary>
            /// @hide
            /// </summary>
            public override void onDebugDraw(View view, Canvas canvas, Paint paint)
            {
                Insets oi = isLayoutModeOptical(view.mParent) ? view.OpticalInsets : Insets.NONE;

                fillDifference(canvas, view.Left + oi.left, view.Top + oi.top, view.Right - oi.right, view.Bottom - oi.bottom, leftMargin, topMargin, rightMargin, bottomMargin, paint);
            }

            /// <summary>
            /// @hide </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override protected void encodeProperties(@NonNull ViewHierarchyEncoder encoder)
            protected internal override void encodeProperties(ViewHierarchyEncoder encoder)
            {
                base.encodeProperties(encoder);
                encoder.addProperty("leftMargin", leftMargin);
                encoder.addProperty("topMargin", topMargin);
                encoder.addProperty("rightMargin", rightMargin);
                encoder.addProperty("bottomMargin", bottomMargin);
                encoder.addProperty("startMargin", startMargin);
                encoder.addProperty("endMargin", endMargin);
            }
        }

        /* Describes a touched view and the ids of the pointers that it has captured.
         *
         * This code assumes that pointer ids are always in the range 0..31 such that
         * it can use a bitfield to track which pointer ids are present.
         * As it happens, the lower layers of the input dispatch pipeline also use the
         * same trick so the assumption should be safe here...
         */
        private sealed class TouchTarget
        {
            internal const int MAX_RECYCLED = 32;
            internal static readonly object sRecycleLock = new object[0];
            internal static TouchTarget sRecycleBin;
            internal static int sRecycledCount;

            public const int ALL_POINTER_IDS = -1; // all ones

            // The touched child view.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage public View child;
            public View child;

            // The combined bit mask of pointer ids for all pointers captured by the target.
            public int pointerIdBits;

            // The next target in the target list.
            public TouchTarget next;

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @UnsupportedAppUsage private TouchTarget()
            internal TouchTarget()
            {
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public static TouchTarget obtain(@NonNull View child, int pointerIdBits)
            public static TouchTarget obtain(View child, int pointerIdBits)
            {
                if (child == null)
                {
                    throw new System.ArgumentException("child must be non-null");
                }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TouchTarget target;
                TouchTarget target;
                lock (sRecycleLock)
                {
                    if (sRecycleBin == null)
                    {
                        target = new TouchTarget();
                    }
                    else
                    {
                        target = sRecycleBin;
                        sRecycleBin = target.next;
                         sRecycledCount--;
                        target.next = null;
                    }
                }
                target.child = child;
                target.pointerIdBits = pointerIdBits;
                return target;
            }

            public void recycle()
            {
                if (child == null)
                {
                    throw new System.InvalidOperationException("already recycled once");
                }

                lock (sRecycleLock)
                {
                    if (sRecycledCount < MAX_RECYCLED)
                    {
                        next = sRecycleBin;
                        sRecycleBin = this;
                        sRecycledCount += 1;
                    }
                    else
                    {
                        next = null;
                    }
                    child = null;
                }
            }
        }

        /* Describes a hovered view. */
        private sealed class HoverTarget
        {
            internal const int MAX_RECYCLED = 32;
            internal static readonly object sRecycleLock = new object[0];
            internal static HoverTarget sRecycleBin;
            internal static int sRecycledCount;

            // The hovered child view.
            public View child;

            // The next target in the target list.
            public HoverTarget next;

            internal HoverTarget()
            {
            }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: public static HoverTarget obtain(@NonNull View child)
            public static HoverTarget obtain(View child)
            {
                if (child == null)
                {
                    throw new System.ArgumentException("child must be non-null");
                }

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final HoverTarget target;
                HoverTarget target;
                lock (sRecycleLock)
                {
                    if (sRecycleBin == null)
                    {
                        target = new HoverTarget();
                    }
                    else
                    {
                        target = sRecycleBin;
                        sRecycleBin = target.next;
                        sRecycledCount--;
                        target.next = null;
                    }
                }
                target.child = child;
                return target;
            }

            public void recycle()
            {
                if (child == null)
                {
                    throw new System.InvalidOperationException("already recycled once");
                }

                lock (sRecycleLock)
                {
                    if (sRecycledCount < MAX_RECYCLED)
                    {
                        next = sRecycleBin;
                        sRecycleBin = this;
                        sRecycledCount += 1;
                    }
                    else
                    {
                        next = null;
                    }
                    child = null;
                }
            }
        }

        /// <summary>
        /// Pooled class that to hold the children for autifill.
        /// </summary>
        private class ChildListForAutoFillOrContentCapture : List<View>
        {
            internal const int MAX_POOL_SIZE = 32;

            internal static readonly Pools.SimplePool<ChildListForAutoFillOrContentCapture> sPool = new Pools.SimplePool<ChildListForAutoFillOrContentCapture>(MAX_POOL_SIZE);

            public static ChildListForAutoFillOrContentCapture obtain()
            {
                ChildListForAutoFillOrContentCapture list = sPool.acquire();
                if (list == null)
                {
                    list = new ChildListForAutoFillOrContentCapture();
                }
                return list;
            }

            public virtual void recycle()
            {
                this.Clear();
                sPool.release(this);
            }
        }

        /// <summary>
        /// Pooled class that orderes the children of a ViewGroup from start
        /// to end based on how they are laid out and the layout direction.
        /// </summary>
        internal class ChildListForAccessibility
        {

            internal const int MAX_POOL_SIZE = 32;

            internal static readonly Pools.SynchronizedPool<ChildListForAccessibility> sPool = new Pools.SynchronizedPool<ChildListForAccessibility>(MAX_POOL_SIZE);

            internal readonly List<View> mChildren = new List<View>();

            internal readonly List<ViewLocationHolder> mHolders = new List<ViewLocationHolder>();

            public static ChildListForAccessibility obtain(ViewGroup parent, bool sort)
            {
                ChildListForAccessibility list = sPool.acquire();
                if (list == null)
                {
                    list = new ChildListForAccessibility();
                }
                list.init(parent, sort);
                return list;
            }

            public virtual void recycle()
            {
                clear();
                sPool.release(this);
            }

            public virtual int ChildCount
            {
                get
                {
                    return mChildren.Count;
                }
            }

            public virtual View getChildAt(int index)
            {
                return mChildren[index];
            }

            internal virtual void init(ViewGroup parent, bool sort)
            {
                List<View> children = mChildren;
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int childCount = parent.getChildCount();
                int childCount = parent.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    View child = parent.getChildAt(i);
                    children.Add(child);
                }
                if (sort)
                {
                    List<ViewLocationHolder> holders = mHolders;
                    for (int i = 0; i < childCount; i++)
                    {
                        View child = children[i];
                        ViewLocationHolder holder = ViewLocationHolder.obtain(parent, child);
                        holders.Add(holder);
                    }
                    sort(holders);
                    for (int i = 0; i < childCount; i++)
                    {
                        ViewLocationHolder holder = holders[i];
                        children[i] = holder.mView;
                        holder.recycle();
                    }
                    holders.Clear();
                }
            }

            internal virtual void sort(List<ViewLocationHolder> holders)
            {
                // This is gross but the least risky solution. The current comparison
                // strategy breaks transitivity but produces very good results. Coming
                // up with a new strategy requires time which we do not have, so ...
                try
                {
                    ViewLocationHolder.ComparisonStrategy = ViewLocationHolder.COMPARISON_STRATEGY_STRIPE;
                    holders.Sort();
                }
                catch (System.ArgumentException)
                {
                    // Note that in practice this occurs extremely rarely in a couple
                    // of pathological cases.
                    ViewLocationHolder.ComparisonStrategy = ViewLocationHolder.COMPARISON_STRATEGY_LOCATION;
                    holders.Sort();
                }
            }

            internal virtual void clear()
            {
                mChildren.Clear();
            }
        }

        /// <summary>
        /// Pooled class that holds a View and its location with respect to
        /// a specified root. This enables sorting of views based on their
        /// coordinates without recomputing the position relative to the root
        /// on every comparison.
        /// </summary>
        internal class ViewLocationHolder : IComparable<ViewLocationHolder>
        {

            internal const int MAX_POOL_SIZE = 32;

            internal static readonly Pools.SynchronizedPool<ViewLocationHolder> sPool = new Pools.SynchronizedPool<ViewLocationHolder>(MAX_POOL_SIZE);

            public const int COMPARISON_STRATEGY_STRIPE = 1;

            public const int COMPARISON_STRATEGY_LOCATION = 2;

            internal static int sComparisonStrategy = COMPARISON_STRATEGY_STRIPE;

            internal readonly Rect mLocation = new Rect();

            internal ViewGroup mRoot;

            public View mView;

            internal int mLayoutDirection;

            public static ViewLocationHolder obtain(ViewGroup root, View view)
            {
                ViewLocationHolder holder = sPool.acquire();
                if (holder == null)
                {
                    holder = new ViewLocationHolder();
                }
                holder.init(root, view);
                return holder;
            }

            public static int ComparisonStrategy
            {
                set
                {
                    sComparisonStrategy = value;
                }
            }

            public virtual void recycle()
            {
                clear();
                sPool.release(this);
            }

            public virtual int CompareTo(ViewLocationHolder another)
            {
                // This instance is greater than an invalid argument.
                if (another == null)
                {
                    return 1;
                }

                int boundsResult = compareBoundsOfTree(this, another);
                if (boundsResult != 0)
                {
                    return boundsResult;
                }

                // Just break the tie somehow. The accessibility ids are unique
                // and stable, hence this is deterministic tie breaking.
                return mView.AccessibilityViewId - another.mView.AccessibilityViewId;
            }

            /// <summary>
            /// Compare two views based on their bounds. Use the bounds of their children to break ties.
            /// </summary>
            /// <param name="holder1"> Holder of first view to compare </param>
            /// <param name="holder2"> Holder of second view to compare. Must have the same root as holder1. </param>
            /// <returns> The compare result, with equality if no good comparison was found. </returns>
            internal static int compareBoundsOfTree(ViewLocationHolder holder1, ViewLocationHolder holder2)
            {
                if (sComparisonStrategy == COMPARISON_STRATEGY_STRIPE)
                {
                    // First is above second.
                    if (holder1.mLocation.bottom - holder2.mLocation.top <= 0)
                    {
                        return -1;
                    }
                    // First is below second.
                    if (holder1.mLocation.top - holder2.mLocation.bottom >= 0)
                    {
                        return 1;
                    }
                }

                // We are ordering left-to-right, top-to-bottom.
                if (holder1.mLayoutDirection == LAYOUT_DIRECTION_LTR)
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int leftDifference = holder1.mLocation.left - holder2.mLocation.left;
                    int leftDifference = holder1.mLocation.left - holder2.mLocation.left;
                    if (leftDifference != 0)
                    {
                        return leftDifference;
                    }
                }
                else
                { // RTL
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rightDifference = holder1.mLocation.right - holder2.mLocation.right;
                    int rightDifference = holder1.mLocation.right - holder2.mLocation.right;
                    if (rightDifference != 0)
                    {
                        return -rightDifference;
                    }
                }
                // We are ordering left-to-right, top-to-bottom.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int topDifference = holder1.mLocation.top - holder2.mLocation.top;
                int topDifference = holder1.mLocation.top - holder2.mLocation.top;
                if (topDifference != 0)
                {
                    return topDifference;
                }
                // Break tie by height.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int heightDiference = holder1.mLocation.height() - holder2.mLocation.height();
                int heightDiference = holder1.mLocation.height() - holder2.mLocation.height();
                if (heightDiference != 0)
                {
                    return -heightDiference;
                }
                // Break tie by width.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int widthDifference = holder1.mLocation.width() - holder2.mLocation.width();
                int widthDifference = holder1.mLocation.width() - holder2.mLocation.width();
                if (widthDifference != 0)
                {
                    return -widthDifference;
                }

                // Find a child of each view with different screen bounds.
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.graphics.Rect view1Bounds = new android.graphics.Rect();
                Rect view1Bounds = new Rect();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.graphics.Rect view2Bounds = new android.graphics.Rect();
                Rect view2Bounds = new Rect();
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.graphics.Rect tempRect = new android.graphics.Rect();
                Rect tempRect = new Rect();
                holder1.mView.getBoundsOnScreen(view1Bounds, true);
                holder2.mView.getBoundsOnScreen(view2Bounds, true);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child1 = holder1.mView.findViewByPredicateTraversal((view) ->
                View child1 = holder1.mView.findViewByPredicateTraversal((view) =>
            {
                view.getBoundsOnScreen(tempRect, true);
                return !tempRect.Equals(view1Bounds);
            }, null);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child2 = holder2.mView.findViewByPredicateTraversal((view) ->
                View child2 = holder2.mView.findViewByPredicateTraversal((view) =>
            {
                view.getBoundsOnScreen(tempRect, true);
                return !tempRect.Equals(view2Bounds);
            }, null);


                // Compare the children recursively
                if ((child1 != null) && (child2 != null))
                {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ViewLocationHolder childHolder1 = ViewLocationHolder.obtain(holder1.mRoot, child1);
                    ViewLocationHolder childHolder1 = ViewLocationHolder.obtain(holder1.mRoot, child1);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ViewLocationHolder childHolder2 = ViewLocationHolder.obtain(holder1.mRoot, child2);
                    ViewLocationHolder childHolder2 = ViewLocationHolder.obtain(holder1.mRoot, child2);
                    return compareBoundsOfTree(childHolder1, childHolder2);
                }

                // If only one has a child, use that one
                if (child1 != null)
                {
                    return 1;
                }

                if (child2 != null)
                {
                    return -1;
                }

                // Give up
                return 0;
            }

            internal virtual void init(ViewGroup root, View view)
            {
                Rect viewLocation = mLocation;
                view.getDrawingRect(viewLocation);
                root.offsetDescendantRectToMyCoords(view, viewLocation);
                mView = view;
                mRoot = root;
                mLayoutDirection = root.LayoutDirection;
            }

            internal virtual void clear()
            {
                mView = null;
                mRoot = null;
                mLocation.set(0, 0, 0, 0);
            }
        }

        private static void drawRect(Canvas canvas, Paint paint, int x1, int y1, int x2, int y2)
        {
            if (sDebugLines == null)
            {
                // TODO: This won't work with multiple UI threads in a single process
                sDebugLines = new float[16];
            }

            sDebugLines[0] = x1;
            sDebugLines[1] = y1;
            sDebugLines[2] = x2;
            sDebugLines[3] = y1;

            sDebugLines[4] = x2;
            sDebugLines[5] = y1;
            sDebugLines[6] = x2;
            sDebugLines[7] = y2;

            sDebugLines[8] = x2;
            sDebugLines[9] = y2;
            sDebugLines[10] = x1;
            sDebugLines[11] = y2;

            sDebugLines[12] = x1;
            sDebugLines[13] = y2;
            sDebugLines[14] = x1;
            sDebugLines[15] = y1;

            canvas.drawLines(sDebugLines, paint);
        }

        /// <summary>
        /// @hide </summary>
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Override @UnsupportedAppUsage protected void encodeProperties(@NonNull ViewHierarchyEncoder encoder)
        protected internal override void encodeProperties(ViewHierarchyEncoder encoder)
        {
            base.encodeProperties(encoder);

            encoder.addProperty("focus:descendantFocusability", DescendantFocusability);
            encoder.addProperty("drawing:clipChildren", ClipChildren);
            encoder.addProperty("drawing:clipToPadding", ClipToPadding);
            encoder.addProperty("drawing:childrenDrawingOrderEnabled", ChildrenDrawingOrderEnabled);
            encoder.addProperty("drawing:persistentDrawingCache", PersistentDrawingCache);

            int n = ChildCount;
            encoder.addProperty("meta:__childCount__", (short)n);
            for (int i = 0; i < n; i++)
            {
                encoder.addPropertyKey("meta:__child__" + i);
                getChildAt(i).encode(encoder);
            }
        }

        /// <summary>
        /// @hide </summary>
        public override void onDescendantUnbufferedRequested()
        {
            // First look at the focused child for focused events
            int focusedChildNonPointerSource = InputDevice.SOURCE_CLASS_NONE;
            if (mFocused != null)
            {
                focusedChildNonPointerSource = mFocused.mUnbufferedInputSource & (~InputDevice.SOURCE_CLASS_POINTER);
            }
            mUnbufferedInputSource = focusedChildNonPointerSource;

            // Request unbuffered dispatch for pointer events for this view if any child requested
            // unbuffered dispatch for pointer events. This is because we can't expect that the pointer
            // source would dispatch to the focused view.
            for (int i = 0; i < mChildrenCount; i++)
            {
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final View child = mChildren[i];
                View child = mChildren[i];
                if ((child.mUnbufferedInputSource & InputDevice.SOURCE_CLASS_POINTER) != 0)
                {
                    mUnbufferedInputSource |= InputDevice.SOURCE_CLASS_POINTER;
                    break;
                }
            }
            if (mParent != null)
            {
                mParent.onDescendantUnbufferedRequested();
            }
        }
    }

}