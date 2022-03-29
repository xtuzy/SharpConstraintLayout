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
using SharpConstraintLayout.Maui.Widget;
#if WINDOWS
using View = Microsoft.UI.Xaml.FrameworkElement;
using UIElement = Microsoft.UI.Xaml.UIElement;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
#elif __IOS__
using View = UIKit.UIView;
using UIElement = UIKit.UIView;
#endif
namespace SharpConstraintLayout.Maui.Helper.Widget
{
    /// 
    /// <summary>
    /// CircularFlow virtual layout.
    /// 
    /// Allows positioning of referenced widgets circular.
    /// 
    /// The elements referenced are indicated via constraint_referenced_ids, as with other ContraintHelper implementations.
    /// 
    /// XML attributes that are needed:
    /// <ul>
    ///     <li>constraint_referenced_ids = "view2, view3, view4,view5,view6". It receives id's of the views that will add the references.</li>
    ///     <li>circularflow_viewCenter = "view1". It receives the id of the view of the center where the views received in constraint_referenced_ids will be referenced.</li>
    ///     <li>circularflow_angles = "45,90,135,180,225". Receive the angles that you will assign to each view.</li>
    ///     <li>circularflow_radiusInDP = "90,100,110,120,130". Receive the radios in DP that you will assign to each view.</li>
    /// </ul>
    /// 
    /// Example in XML:
    /// <androidx.constraintlayout.helper.widget.CircularFlow
    ///         android:id="@+id/circularFlow"
    ///         android:layout_width="match_parent"
    ///         android:layout_height="match_parent"
    ///         app:circularflow_angles="0,40,80,120"
    ///         app:circularflow_radiusInDP="90,100,110,120"
    ///         app:circularflow_viewCenter="@+id/view1"
    ///         app:constraint_referenced_ids="view2,view3,view4,view5" />
    /// 
    /// DEFAULT radius - If you add a view and don't set its radius, the default value will be 0.
    /// DEFAULT angles - If you add a view and don't set its angle, the default value will be 0.
    /// 
    /// Recommendation - always set radius and angle for all views in <i>constraint_referenced_ids</i>
    /// 
    /// 
    /// </summary>

    public class CircularFlow : VirtualLayout, ICircularFlow
    {
        private const string TAG = "CircularFlow";
        internal ConstraintLayout mContainer;
        internal int mViewCenter;
        private static int DEFAULT_RADIUS = 0;
        private static float DEFAULT_ANGLE = 0F;
        /// <summary>
        /// @suppress
        /// </summary>
        private float[] mAngles;

        /// <summary>
        /// @suppress
        /// </summary>
        private int[] mRadius;

        /// <summary>
        /// @suppress
        /// </summary>
        private int mCountRadius;

        /// <summary>
        /// @suppress
        /// </summary>
        private int mCountAngle;

        /// <summary>
        /// @suppress
        /// </summary>
        private string mReferenceAngles;

        /// <summary>
        /// @suppress
        /// </summary>
        private string mReferenceRadius;

        /// <summary>
        /// @suppress
        /// </summary>
        private float? mReferenceDefaultAngle;

        /// <summary>
        /// @suppress
        /// </summary>
        private int? mReferenceDefaultRadius;

        public CircularFlow() : base()
        {
        }

        public virtual int[] GetRadius()
        {
            return ArrayHelperClass.Copy(mRadius, mCountRadius);
        }

        public virtual float[] GetAngles()
        {
            return ArrayHelperClass.Copy(mAngles, mCountAngle);
        }

        protected internal override void init()
        {
            base.init();
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            if (!string.ReferenceEquals(mReferenceAngles, null))
            {
                mAngles = new float[1];
                setAngles(mReferenceAngles);
            }
            if (!string.ReferenceEquals(mReferenceRadius, null))
            {
                mRadius = new int[1];
                setRadius(mReferenceRadius);
            }
            if (mReferenceDefaultAngle != null)
            {
                SetDefaultAngle(mReferenceDefaultAngle.Value);
            }
            if (mReferenceDefaultRadius != null)
            {
                SetDefaultRadius(mReferenceDefaultRadius.Value);
            }
            anchorReferences();
        }

        private void anchorReferences()
        {
            mContainer = (ConstraintLayout)this.GetParent();
            for (int i = 0; i < mCount; i++)
            {
                View view = mContainer.FindViewById(mIds[i]) as View;
                if (view == null)
                {
                    continue;
                }
                int radius = DEFAULT_RADIUS;
                float angle = DEFAULT_ANGLE;

                if (mRadius != null && i < mRadius.Length)
                {
                    radius = mRadius[i];
                }
                else if (mReferenceDefaultRadius != null && mReferenceDefaultRadius != -1)
                {
                    mCountRadius++;
                    if (mRadius == null)
                    {
                        mRadius = new int[1];
                    }
                    mRadius = GetRadius();
                    mRadius[mCountRadius - 1] = radius;
                }
                else
                {
                    Debug.WriteLine("CircularFlow", "Added radius to view with id: " + mMap[view.GetId()]);
                }

                if (mAngles != null && i < mAngles.Length)
                {
                    angle = mAngles[i];
                }
                else if (mReferenceDefaultAngle != null && mReferenceDefaultAngle != -1)
                {
                    mCountAngle++;
                    if (mAngles == null)
                    {
                        mAngles = new float[1];
                    }
                    mAngles = GetAngles();
                    mAngles[mCountAngle - 1] = angle;
                }
                else
                {
                    Debug.WriteLine("CircularFlow", "Added angle to view with id: " + mMap[view.GetId()]);
                }
                //ConstraintLayout.LayoutParams @params = (ConstraintLayout.LayoutParams)view.LayoutParams;
                ConstraintSet.Layout @params = mContainer.mConstraintSet.GetConstraint(view.GetId()).layout;
                @params.circleAngle = angle;
                @params.circleConstraint = mViewCenter;
                @params.circleRadius = radius;
                //view.LayoutParams = @params;
            }
            applyLayoutFeatures();
        }

        /// <summary>
        /// Add a view to the CircularFlow. The referenced view need to be a child of the container parent.
        /// The view also need to have its id set in order to be added.
        /// The views previous need to have its radius and angle set in order to be added correctly a new view. </summary>
        /// <param name="view"> </param>
        /// <param name="radius"> </param>
        /// <param name="angle">
        /// @return </param>
        public virtual void AddViewToCircularFlow(View view, int radius, float angle)
        {
            if (ContainsId(view.GetId()))
            {
                return;
            }
            AddView(view);
            mCountAngle++;
            mAngles = GetAngles();
            mAngles[mCountAngle - 1] = angle;
            mCountRadius++;
            mRadius = GetRadius();
            //mRadius[mCountRadius - 1] = (int)(radius * myContext.Resources.DisplayMetrics.density);
            mRadius[mCountRadius - 1] = radius;//iOS and Windows use DP, Android use PX
            anchorReferences();
        }

        /// <summary>
        /// Update radius from a view in CircularFlow. The referenced view need to be a child of the container parent.
        /// The view also need to have its id set in order to be added. </summary>
        /// <param name="view"> </param>
        /// <param name="radius">
        /// @return </param>
        public virtual void UpdateRadius(View view, int radius)
        {
            if (!IsUpdatable(view))
            {
                Debug.WriteLine("CircularFlow", "It was not possible to update radius to view with id: " + view.GetId());
                return;
            }
            int indexView = IndexFromId(view.GetId());
            if (indexView > mRadius.Length)
            {
                return;
            }
            mRadius = GetRadius();
            //mRadius[indexView] = (int)(radius * myContext.Resources.DisplayMetrics.density);
            mRadius[indexView] = radius;//iOS and Windows use DP, Android use PX
            anchorReferences();
        }

        /// <summary>
        /// Update angle from a view in CircularFlow. The referenced view need to be a child of the container parent.
        /// The view also need to have its id set in order to be added. </summary>
        /// <param name="view"> </param>
        /// <param name="angle">
        /// @return </param>
        public virtual void UpdateAngle(View view, float angle)
        {
            if (!IsUpdatable(view))
            {
                Debug.WriteLine("CircularFlow", "It was not possible to update angle to view with id: " + view.GetId());
                return;
            }
            int indexView = IndexFromId(view.GetId());
            if (indexView > mAngles.Length)
            {
                return;
            }
            mAngles = GetAngles();
            mAngles[indexView] = angle;
            anchorReferences();
        }

        /// <summary>
        /// Update angle and radius from a view in CircularFlow. The referenced view need to be a child of the container parent.
        /// The view also need to have its id set in order to be added. </summary>
        /// <param name="view"> </param>
        /// <param name="radius"> </param>
        /// <param name="angle">
        /// @return </param>
        public virtual void UpdateReference(View view, int radius, float angle)
        {
            if (!IsUpdatable(view))
            {
                Debug.WriteLine("CircularFlow", "It was not possible to update radius and angle to view with id: " + view.GetId());
                return;
            }
            int indexView = IndexFromId(view.GetId());
            if (GetAngles().Length > indexView)
            {
                mAngles = GetAngles();
                mAngles[indexView] = angle;
            }
            if (GetRadius().Length > indexView)
            {
                mRadius = GetRadius();
                //mRadius[indexView] = (int)(radius * myContext.Resources.DisplayMetrics.density);
                mRadius[indexView] = radius;//iOS and Windows use DP, Android use PX
            }
            anchorReferences();
        }

        /// <summary>
        /// Set default Angle for CircularFlow.
        /// </summary>
        /// <param name="angle">
        /// @return </param>
        public virtual void SetDefaultAngle(float angle)
        {
            DEFAULT_ANGLE = angle;
        }

        /// <summary>
        /// Set default Radius for CircularFlow.
        /// </summary>
        /// <param name="radius">
        /// @return </param>
        public virtual void SetDefaultRadius(int radius)
        {
            DEFAULT_RADIUS = radius;
        }

        public override int RemoveView(View view)
        {
            int index = base.RemoveView(view);
            if (index == -1)
            {
                return index;
            }
            ConstraintSet c = new ConstraintSet();
            c.Clone(mContainer);
            c.Clear(view.GetId(), ConstraintSet.CircleReference);
            c.ApplyTo(mContainer);

            if (index < mAngles.Length)
            {
                mAngles = removeAngle(mAngles, index);
                mCountAngle--;
            }
            if (index < mRadius.Length)
            {
                mRadius = removeRadius(mRadius, index);
                mCountRadius--;
            }
            anchorReferences();
            return index;
        }

        /// <summary>
        /// @suppress
        /// </summary>
        private float[] removeAngle(float[] angles, int index)
        {
            if (angles == null || index < 0 || index >= mCountAngle)
            {
                return angles;
            }

            return removeElementFromArray(angles, index);
        }

        /// <summary>
        /// @suppress
        /// </summary>
        private int[] removeRadius(int[] radius, int index)
        {
            if (radius == null || index < 0 || index >= mCountRadius)
            {
                return radius;
            }

            return removeElementFromArray(radius, index);
        }

        /// <summary>
        /// @suppress
        /// </summary>
        private void setAngles(string idList)
        {
            if (string.ReferenceEquals(idList, null))
            {
                return;
            }
            int begin = 0;
            mCountAngle = 0;
            while (true)
            {
                int end = idList.IndexOf(',', begin);
                if (end == -1)
                {
                    addAngle(idList.Substring(begin).Trim());
                    break;
                }
                addAngle(idList.Substring(begin, end - begin).Trim());
                begin = end + 1;
            }
        }

        /// <summary>
        /// @suppress
        /// </summary>
        private void setRadius(string idList)
        {
            if (string.ReferenceEquals(idList, null))
            {
                return;
            }
            int begin = 0;
            mCountRadius = 0;
            while (true)
            {
                int end = idList.IndexOf(',', begin);
                if (end == -1)
                {
                    addRadius(idList.Substring(begin).Trim());
                    break;
                }
                addRadius(idList.Substring(begin, end - begin).Trim());
                begin = end + 1;
            }
        }

        /// <summary>
        /// @suppress
        /// </summary>
        private void addAngle(string angleString)
        {
            if (string.ReferenceEquals(angleString, null) || angleString.Length == 0)
            {
                return;
            }

            if (mAngles == null)
            {
                return;
            }

            if (mCountAngle + 1 > mAngles.Length)
            {
                mAngles = ArrayHelperClass.Copy(mAngles, mAngles.Length + 1);
            }
            mAngles[mCountAngle] = int.Parse(angleString);
            mCountAngle++;
        }

        /// <summary>
        /// @suppress
        /// </summary>
        private void addRadius(string radiusString)
        {
            if (string.ReferenceEquals(radiusString, null) || radiusString.Length == 0)
            {
                return;
            }

            if (mRadius == null)
            {
                return;
            }

            if (mCountRadius + 1 > mRadius.Length)
            {
                mRadius = ArrayHelperClass.Copy(mRadius, mRadius.Length + 1);
            }

            //mRadius[mCountRadius] = (int)(int.Parse(radiusString) * myContext.Resources.DisplayMetrics.density);
            mRadius[mCountRadius] = (int)(int.Parse(radiusString));//iOS and Windows use DP, Android use PX
            mCountRadius++;
        }

        public static int[] removeElementFromArray(int[] array, int index)
        {
            int[] newArray = new int[array.Length - 1];

            for (int i = 0, k = 0; i < array.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }
                newArray[k++] = array[i];
            }
            return newArray;
        }

        public static float[] removeElementFromArray(float[] array, int index)
        {
            float[] newArray = new float[array.Length - 1];

            for (int i = 0, k = 0; i < array.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }
                newArray[k++] = array[i];
            }
            return newArray;
        }

        public virtual bool IsUpdatable(View view)
        {
            if (!ContainsId(view.GetId()))
            {
                return false;
            }
            int indexView = IndexFromId(view.GetId());
            return indexView != -1;
        }
    }

}