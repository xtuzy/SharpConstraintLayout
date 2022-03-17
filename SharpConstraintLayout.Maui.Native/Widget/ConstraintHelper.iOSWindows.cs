
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

namespace SharpConstraintLayout.Maui.Widget
{
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
    using Helper = androidx.constraintlayout.core.widgets.Helper;
    using HelperWidget = androidx.constraintlayout.core.widgets.HelperWidget;

#if WINDOWS
    using View = Microsoft.UI.Xaml.FrameworkElement;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Windows.Foundation;
#elif __IOS__
    using View = UIKit.UIView;
#elif __ANDROID__
    using Android.Content;
    using View = Android.Views.View;
#endif
    /// <summary>
    /// @suppress
    /// <b>Added in 1.1</b>
    /// <para>
    ///     This class manages a set of referenced widgets. HelperWidget objects can be created to act upon the set
    ///     of referenced widgets. The difference between {@code ConstraintHelper} and {@code ViewGroup} is that
    ///     multiple {@code ConstraintHelper} can reference the same widgets.
    /// </para>
    /// <para>
    ///     Widgets are referenced by being added to a comma separated list of ids, e.g:
    ///     <pre>
    ///     {@code
    ///         <androidx.constraintlayout.widget.Barrier
    ///              android:id="@+id/barrier"
    ///              android:layout_width="wrap_content"
    ///              android:layout_height="wrap_content"
    ///              app:barrierDirection="start"
    ///              app:constraint_referenced_ids="button1,button2" />
    ///     }
    ///     </pre>
    /// </para>
    /// </summary>
    public abstract partial class ConstraintHelper : View
    {
        bool InEditMode = false;

        /// <summary>
        /// @suppress
        /// </summary>
        protected internal int[] mIds = new int[32];
        /// <summary>
        /// @suppress
        /// </summary>
        protected internal int mCount;

        /// <summary>
        /// @suppress
        /// </summary>
        //protected internal Context myContext;
        /// <summary>
        /// @suppress
        /// </summary>
        protected internal Helper mHelperWidget;
        /// <summary>
        /// @suppress
        /// </summary>
        protected internal bool mUseViewMeasure = false;
        /// <summary>
        /// @suppress
        /// </summary>
        protected internal string mReferenceIds;
        /// <summary>
        /// @suppress
        /// </summary>
        protected internal string mReferenceTags;

        /// <summary>
        /// @suppress
        /// </summary>
        private View[] mViews = null;

        protected internal Dictionary<int?, string> mMap = new Dictionary<int?, string>();

#if __ANDROID__
        public ConstraintHelper(Context context) : base(context)
#else
        public ConstraintHelper() : base()
#endif

        {
            init();
        }


        /// <summary>
        /// @suppress
        /// </summary> 
        protected internal virtual void init()
        {
#if WINDOWS
            this.Loaded += MovedToWindow;
#elif __IOS__
#endif
        }
#if WINDOWS
        public virtual void MovedToWindow(object sender, RoutedEventArgs e)
        {
            //base.onAttachedToWindow();
            /*if (!string.ReferenceEquals(mReferenceIds, null))
            {
                Ids = mReferenceIds;
            }
            if (!string.ReferenceEquals(mReferenceTags, null))
            {
                ReferenceTags = mReferenceTags;
            }*/
            OnAttachedToWindow();
        }
#elif __IOS__
        bool isOnAttachedToWindow = true;
        public override void MovedToWindow()
        {
            base.MovedToWindow();
            if (isOnAttachedToWindow)
            {
                OnAttachedToWindow();
                isOnAttachedToWindow = false;
            }
        }
#elif __ANDROID__
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
        }
#endif
        public virtual void OnAttachedToWindow()
        {

        }

        /// <summary>
        /// Add a view to the helper. The referenced view need to be a child of the helper's parent.
        /// The view also need to have its id set in order to be added.
        /// </summary>
        /// <param name="view"> </param>
        public virtual void addView(View view)
        {

            if (view == this)
            {
                return;
            }
#if WINDOWS||__ANDROID__
            if (view.Parent == null)
#elif __IOS__
            if (view.Superview == null)
#endif
            {
                Debug.WriteLine("ConstraintHelper", "Views added to a ConstraintHelper need to have a parent");
                return;
            }
            mReferenceIds = null;
            addRscID(view.GetHashCode());
#if WINDOWS
            UpdateLayout();
#elif __IOS__
            SetNeedsLayout();
#elif __ANDROID__
            RequestLayout();
#endif
        }

        /// <summary>
        /// Remove a given view from the helper.
        /// </summary>
        /// <param name="view"> </param>
        /// <returns> index of view removed </returns>
        public virtual int removeView(View view)
        {
            int index = -1;
            //int id = view.Id;
            int id = view.GetHashCode();
            if (id == -1)
            {
                return index;
            }
            mReferenceIds = null;
            for (int i = 0; i < mCount; i++)
            {
                if (mIds[i] == id)
                {
                    index = i;
                    for (int j = i; j < mCount - 1; j++)
                    {
                        mIds[j] = mIds[j + 1];
                    }
                    mIds[mCount - 1] = 0;
                    mCount--;
                    break;
                }
            }
#if WINDOWS
            UpdateLayout();
#elif __IOS__
            SetNeedsLayout();
#elif __ANDROID__
            RequestLayout();
#endif
            return index;
        }

        /// <summary>
        /// Helpers typically reference a collection of ids </summary>
        /// <returns> ids referenced </returns>
        public virtual int[] ReferencedIds
        {
            get
            {
                //return Arrays.copyOf(mIds, mCount);
                return ArrayHelperClass.Copy(mIds, mCount);
            }
            set
            {
                mReferenceIds = null;
                mCount = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    addRscID(value[i]);
                }
            }
        }


        /// <summary>
        /// @suppress
        /// </summary>
        private void addRscID(int id)
        {
            //if (id == Id)
            if (id == this.GetId())
            {
                return;
            }
            if (mCount + 1 > mIds.Length)
            {
                //mIds = Arrays.copyOf(mIds, mIds.Length * 2);
                mIds = ArrayHelperClass.Copy(mIds, mIds.Length * 2);
            }
            mIds[mCount] = id;
            mCount++;
        }

        /// <summary>
        /// @suppress
        /// </summary>
        /*public override void onDraw(Canvas canvas)
        {
            // Nothing
        }*/

        /*/// <summary>
        /// @suppress
        /// </summary>
        protected internal override void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (mUseViewMeasure)
            {
                base.onMeasure(widthMeasureSpec, heightMeasureSpec);
            }
            else
            {
                setMeasuredDimension(0, 0);
            }
        }*/

        /// <summary>
        /// @suppress
        /// Allows a helper to replace the default ConstraintWidget in LayoutParams by its own subclass
        /// </summary>
        public virtual void validateParams(Dictionary<int,ConstraintWidget> idsToConstraintWidgets=null)
        {
            if (mHelperWidget == null)
            {
                return;
            }

            if (idsToConstraintWidgets != null && idsToConstraintWidgets.ContainsKey(this.GetId()))
                idsToConstraintWidgets[this.GetId()] = (ConstraintWidget)mHelperWidget;
        }

        /// <summary>
        /// @suppress
        /// </summary>
        /*private void addTag(string tagString)
        {
            if (string.ReferenceEquals(tagString, null) || tagString.Length == 0)
            {
                return;
            }
            if (myContext == null)
            {
                return;
            }

            tagString = tagString.Trim();

            ConstraintLayout parent = null;
            if (Parent is ConstraintLayout)
            {
                parent = (ConstraintLayout)Parent;
            }
            if (parent == null)
            {
                Debug.WriteLine("ConstraintHelper", "Parent not a ConstraintLayout");
                return;
            }
            int count = parent.ChildCount;
            for (int i = 0; i < count; i++)
            {
                View v = parent.getViewAt(i);
                ViewGroup.LayoutParams @params = v.LayoutParams;
                if (@params is ConstraintLayout.LayoutParams)
                {
                    ConstraintLayout.LayoutParams lp = (ConstraintLayout.LayoutParams)@params;
                    if (tagString.Equals(lp.constraintTag))
                    {
                        if (v.Id == AndroidView.NO_ID)
                        {
                            Log.w("ConstraintHelper", "to use ConstraintTag view " + v.GetType().Name + " must have an ID");
                        }
                        else
                        {
                            //addRscID(v.Id);
                            addRscID(v.GetHashCode());
                        }
                    }
                }

            }
        }*/

        /// <summary>
        /// @suppress
        /// </summary>
        /*protected internal virtual string ReferenceTags
        {
            set
            {
                mReferenceTags = value;
                if (string.ReferenceEquals(value, null))
                {
                    return;
                }
                int begin = 0;
                mCount = 0;
                while (true)
                {
                    int end = value.IndexOf(',', begin);
                    if (end == -1)
                    {
                        addTag(value.Substring(begin));
                        break;
                    }
                    addTag(value.Substring(begin, end - begin));
                    begin = end + 1;
                }
            }
        }*/

        /// <summary>
        /// @suppress </summary>
        /// <param name="container"> </param>
        protected internal virtual void applyLayoutFeatures(ConstraintLayout container)
        {
#if WINDOWS || __ANDROID__
            var visibility = this.Visibility;
#elif __IOS__
            var visbility = this.Hidden;
#endif
            float elevation = 0;
#if __ANDROID__
            if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP)
            {
                elevation = Elevation;
            }
#endif
            for (int i = 0; i < mCount; i++)
            {
                int id = mIds[i];
                View view = (View)container.getViewById(id);
                if (view != null)
                {
#if WINDOWS || __ANDROID__
                    view.Visibility = visibility;
#elif __IOS__
                    view.Hidden = visbility;
#endif

#if __ANDROID__
                    if (elevation > 0 && android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.LOLLIPOP)
                    {
                        view.TranslationZ = view.TranslationZ + elevation;
                    }
#endif
                }
            }
        }

        /// <summary>
        /// @suppress
        /// </summary>
        protected internal virtual void applyLayoutFeatures()
        {
#if WINDOWS || __ANDROID__
            Object parent = this.Parent;
#elif __IOS__
            Object parent = this.Superview;
#endif
            if (parent != null && parent is ConstraintLayout)
            {
                applyLayoutFeatures((ConstraintLayout)parent);
            }
        }

        /// <summary>
        /// @suppress
        /// </summary>
        protected internal virtual void applyLayoutFeaturesInConstraintSet(ConstraintLayout container)
        {
        }

        /// <summary>
        /// @suppress
        /// Allows a helper a chance to update its internal object pre layout or set up connections for the pointed elements
        /// </summary>
        /// <param name="container"> </param>
        public virtual void updatePreLayout(ConstraintLayout container)
        {
            /*if (InEditMode)
            {
                Ids = mReferenceIds;
            }*/
            if (mHelperWidget == null)
            {
                return;
            }
            mHelperWidget.removeAllIds();
            for (int i = 0; i < mCount; i++)
            {
                int id = mIds[i];
                View view = (View)container.getViewById(id);
                /*if (view == null)
                {
                    // hm -- we couldn't find the view.
                    // It might still be there though, but with the wrong id (with dynamic modules)
                    string candidate = mMap[id];
                    int foundId = findId(container, candidate);
                    if (foundId != 0)
                    {
                        mIds[i] = foundId;
                        mMap[foundId] = candidate;
                        view = container.getViewById(foundId);
                    }
                }*/
                if (view != null)
                {
                    mHelperWidget.add(container.GetWidget(view));
                }
            }
            mHelperWidget.updateConstraints(container.RootWidget);
        }

        public virtual void updatePreLayout(ConstraintWidgetContainer container, Helper helper, Dictionary<int, ConstraintWidget> map)
        {
            helper.removeAllIds();
            for (int i = 0; i < mCount; i++)
            {
                int id = mIds[i];
                //helper.add(map.get(id));
                helper.add(map[id]);
            }
        }

        protected internal virtual View[] getViews(ConstraintLayout layout)
        {

            if (mViews == null || mViews.Length != mCount)
            {
                mViews = new View[mCount];
            }

            for (int i = 0; i < mCount; i++)
            {
                int id = mIds[i];
                mViews[i] = (View)layout.getViewById(id);
            }
            return mViews;
        }

        /// <summary>
        /// @suppress
        /// Allows a helper a chance to update its internal object post layout or set up connections for the pointed elements
        /// </summary>
        /// <param name="container"> </param>
        public virtual void updatePostLayout(ConstraintLayout container)
        {
            // Do nothing
        }

        /// <summary>
        /// @suppress </summary>
        /// <param name="container"> </param>
        public virtual void updatePostMeasure(ConstraintLayout container)
        {
            // Do nothing
        }

        public virtual void updatePostConstraints(ConstraintLayout container)
        {
            // Do nothing
        }

        public virtual void updatePreDraw(ConstraintLayout container)
        {
            // Do nothing
        }

        //public virtual void loadParameters(ConstraintSet.Constraint constraint, HelperWidget child, ConstraintLayout.LayoutParams layoutParams, SparseArray<ConstraintWidget> mapIdToWidget)
        public virtual void loadParameters(ConstraintSet.Constraint constraint, HelperWidget child, Dictionary<int, ConstraintWidget> mapIdToWidget)
        {
            // TODO: we need to rethink this -- the list of referenced views shouldn't be resolved at updatePreLayout stage,
            // as this makes changing referenced views tricky at runtime
            if (constraint.layout.mReferenceIds != null)
            {
                ReferencedIds = constraint.layout.mReferenceIds;
            }
            /*else if (!string.ReferenceEquals(constraint.layout.mReferenceIdString, null))
            {
                if (constraint.layout.mReferenceIdString.Length > 0)
                {
                    constraint.layout.mReferenceIds = convertReferenceString(this, constraint.layout.mReferenceIdString);
                }
                else
                {
                    constraint.layout.mReferenceIds = null;
                }
            }*/
            if (child != null)
            {
                child.removeAllIds();
                if (constraint.layout.mReferenceIds != null)
                {
                    for (int i = 0; i < constraint.layout.mReferenceIds.Length; i++)
                    {
                        int id = constraint.layout.mReferenceIds[i];
                        ConstraintWidget widget = mapIdToWidget[id];
                        if (widget != null)
                        {
                            child.add(widget);
                        }
                    }
                }
            }
        }
        

        public virtual void resolveRtl(ConstraintWidget widget, bool isRtl)
        {
            // nothing here
        }

        /*public override void setTag(int key, object tag)
        {
            base.setTag(key, tag);
            if (tag == null && string.ReferenceEquals(mReferenceIds, null))
            {
                addRscID(key);
            }
        }*/

        //ORIGINAL LINE: public boolean containsId(final int id)
        public virtual bool containsId(int id)
        {
            bool result = false;
            foreach (int i in mIds)
            {
                if (i == id)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        //ORIGINAL LINE: public int indexFromId(final int id)
        public virtual int indexFromId(int id)
        {
            int index = -1;
            foreach (int i in mIds)
            {
                index++;
                if (i == id)
                {
                    return index;
                }
            }
            return index;
        }
    }

}