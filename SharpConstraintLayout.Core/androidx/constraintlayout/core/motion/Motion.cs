using System;
using System.Collections.Generic;
using System.Linq;

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
namespace androidx.constraintlayout.core.motion
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static androidx.constraintlayout.core.motion.MotionWidget.UNSET;
using static androidx.constraintlayout.core.motion.MotionWidget;

    using MotionConstraintSet = androidx.constraintlayout.core.motion.key.MotionConstraintSet;
    using MotionKey = androidx.constraintlayout.core.motion.key.MotionKey;
    using MotionKeyAttributes = androidx.constraintlayout.core.motion.key.MotionKeyAttributes;
    using MotionKeyCycle = androidx.constraintlayout.core.motion.key.MotionKeyCycle;
    using MotionKeyPosition = androidx.constraintlayout.core.motion.key.MotionKeyPosition;
    using MotionKeyTimeCycle = androidx.constraintlayout.core.motion.key.MotionKeyTimeCycle;
    using MotionKeyTrigger = androidx.constraintlayout.core.motion.key.MotionKeyTrigger;
    using CurveFit = androidx.constraintlayout.core.motion.utils.CurveFit;
    using DifferentialInterpolator = androidx.constraintlayout.core.motion.utils.DifferentialInterpolator;
    using Easing = androidx.constraintlayout.core.motion.utils.Easing;
    using FloatRect = androidx.constraintlayout.core.motion.utils.FloatRect;
    using KeyCache = androidx.constraintlayout.core.motion.utils.KeyCache;
    using KeyCycleOscillator = androidx.constraintlayout.core.motion.utils.KeyCycleOscillator;
    using KeyFrameArray = androidx.constraintlayout.core.motion.utils.KeyFrameArray;
    using Rect = androidx.constraintlayout.core.motion.utils.Rect;
    using SplineSet = androidx.constraintlayout.core.motion.utils.SplineSet;
    using TimeCycleSplineSet = androidx.constraintlayout.core.motion.utils.TimeCycleSplineSet;
    using Utils = androidx.constraintlayout.core.motion.utils.Utils;
    using VelocityMatrix = androidx.constraintlayout.core.motion.utils.VelocityMatrix;
    using ViewState = androidx.constraintlayout.core.motion.utils.ViewState;
    using WidgetFrame = androidx.constraintlayout.core.state.WidgetFrame;


    /// <summary>
    /// This contains the picture of a view through the a transition and is used to interpolate it
    /// During an transition every view has a MotionController which drives its position.
    /// <para>
    /// All parameter which affect a views motion are added to MotionController and then setup()
    /// builds out the splines that control the view.
    /// 
    /// @suppress
    /// </para>
    /// </summary>
    public class Motion
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            mValuesBuff = new float[MAX_DIMENSION];
        }

        public const int PATH_PERCENT = 0;
        public const int PATH_PERPENDICULAR = 1;
        public const int HORIZONTAL_PATH_X = 2;
        public const int HORIZONTAL_PATH_Y = 3;
        public const int VERTICAL_PATH_X = 4;
        public const int VERTICAL_PATH_Y = 5;
        public const int DRAW_PATH_NONE = 0;
        public const int DRAW_PATH_BASIC = 1;
        public const int DRAW_PATH_RELATIVE = 2;
        public const int DRAW_PATH_CARTESIAN = 3;
        public const int DRAW_PATH_AS_CONFIGURED = 4;
        public const int DRAW_PATH_RECTANGLE = 5;
        public const int DRAW_PATH_SCREEN = 6;

        public const int ROTATION_RIGHT = 1;
        public const int ROTATION_LEFT = 2;
        internal Rect mTempRect = new Rect(); // for efficiency

        private const string TAG = "MotionController";
        private const bool DEBUG = false;
        private const bool FAVOR_FIXED_SIZE_VIEWS = false;
        internal MotionWidget mView;
        internal int mId;
        internal string mConstraintTag;
        private int mCurveFitType = UNSET;
        private MotionPaths mStartMotionPath = new MotionPaths();
        private MotionPaths mEndMotionPath = new MotionPaths();

        private MotionConstrainedPoint mStartPoint = new MotionConstrainedPoint();
        private MotionConstrainedPoint mEndPoint = new MotionConstrainedPoint();

        private CurveFit[] mSpline; // spline 0 is the generic one that process all the standard attributes
        private CurveFit mArcSpline;
        internal float mMotionStagger = float.NaN;
        internal float mStaggerOffset = 0;
        internal float mStaggerScale = 1.0f;
        internal float mCurrentCenterX, mCurrentCenterY;
        private int[] mInterpolateVariables;
        private double[] mInterpolateData; // scratch data created during setup
        private double[] mInterpolateVelocity; // scratch data created during setup

        private string[] mAttributeNames; // the names of the custom attributes
        private int[] mAttributeInterpolatorCount; // how many interpolators for each custom attribute
        private int MAX_DIMENSION = 4;
        private float[] mValuesBuff;
        private List<MotionPaths> mMotionPaths = new List<MotionPaths>();
        private float[] mVelocity = new float[1]; // used as a temp buffer to return values

        private List<MotionKey> mKeyList = new List<MotionKey>(); // List of key frame items
        private Dictionary<string, TimeCycleSplineSet> mTimeCycleAttributesMap; // splines to calculate for use TimeCycles
        private Dictionary<string, SplineSet> mAttributesMap; // splines to calculate values of attributes
        private Dictionary<string, KeyCycleOscillator> mCycleMap; // splines to calculate values of attributes
        private MotionKeyTrigger[] mKeyTriggers; // splines to calculate values of attributes
        private int mPathMotionArc = UNSET;
        private int mTransformPivotTarget = UNSET; // if set, pivot point is maintained as the other object
        private MotionWidget mTransformPivotView = null; // if set, pivot point is maintained as the other object
        private int mQuantizeMotionSteps = UNSET;
        private float mQuantizeMotionPhase = float.NaN;
        private DifferentialInterpolator mQuantizeMotionInterpolator = null;
        private bool mNoMovement = false;

        /// <summary>
        /// Get the view to pivot around
        /// </summary>
        /// <returns> id of view or UNSET if not set </returns>
        public virtual int TransformPivotTarget
        {
            get
            {
                return mTransformPivotTarget;
            }
            set
            {
                mTransformPivotTarget = value;
                mTransformPivotView = null;
            }
        }


        /// <summary>
        /// provides acces to MotionPath objects
        /// </summary>
        /// <param name="i">
        /// @return </param>
        public virtual MotionPaths getKeyFrame(int i)
        {
            return mMotionPaths[i];
        }

        public Motion(MotionWidget view)
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            View = view;
        }

        /// <summary>
        /// get the left most position of the widget at the start of the movement.
        /// </summary>
        /// <returns> the left most position </returns>
        public virtual float StartX
        {
            get
            {
                return mStartMotionPath.x;
            }
        }

        /// <summary>
        /// get the top most position of the widget at the start of the movement.
        /// Positive is down.
        /// </summary>
        /// <returns> the top most position </returns>
        public virtual float StartY
        {
            get
            {
                return mStartMotionPath.y;
            }
        }

        /// <summary>
        /// get the left most position of the widget at the end of the movement.
        /// </summary>
        /// <returns> the left most position </returns>
        public virtual float FinalX
        {
            get
            {
                return mEndMotionPath.x;
            }
        }

        /// <summary>
        /// get the top most position of the widget at the end of the movement.
        /// Positive is down.
        /// </summary>
        /// <returns> the top most position </returns>
        public virtual float FinalY
        {
            get
            {
                return mEndMotionPath.y;
            }
        }

        /// <summary>
        /// get the width of the widget at the start of the movement.
        /// </summary>
        /// <returns> the width at the start </returns>
        public virtual float StartWidth
        {
            get
            {
                return mStartMotionPath.width;
            }
        }

        /// <summary>
        /// get the width of the widget at the start of the movement.
        /// </summary>
        /// <returns> the height at the start </returns>
        public virtual float StartHeight
        {
            get
            {
                return mStartMotionPath.height;
            }
        }

        /// <summary>
        /// get the width of the widget at the end of the movement.
        /// </summary>
        /// <returns> the width at the end </returns>
        public virtual float FinalWidth
        {
            get
            {
                return mEndMotionPath.width;
            }
        }

        /// <summary>
        /// get the width of the widget at the end of the movement.
        /// </summary>
        /// <returns> the height at the end </returns>
        public virtual float FinalHeight
        {
            get
            {
                return mEndMotionPath.height;
            }
        }

        /// <summary>
        /// Will return the id of the view to move relative to
        /// The position at the start and then end will be viewed relative to this view
        /// -1 is the return value if NOT in polar mode
        /// </summary>
        /// <returns> the view id of the view this is in polar mode to or -1 if not in polar </returns>
        public virtual int AnimateRelativeTo
        {
            get
            {
                return mStartMotionPath.mAnimateRelativeTo;
            }
        }

        public virtual void setupRelative(Motion motionController)
        {
            mStartMotionPath.setupRelative(motionController, motionController.mStartMotionPath);
            mEndMotionPath.setupRelative(motionController, motionController.mEndMotionPath);
        }

        public virtual float CenterX
        {
            get
            {
                return mCurrentCenterX;
            }
        }

        public virtual float CenterY
        {
            get
            {
                return mCurrentCenterY;
            }
        }

        public virtual void getCenter(double p, float[] pos, float[] vel)
        {
            double[] position = new double[4];
            double[] velocity = new double[4];
            int[] temp = new int[4];
            mSpline[0].getPos(p, position);
            mSpline[0].getSlope(p, velocity);
            //Arrays.fill(vel, 0);
            vel.Fill(0);
            mStartMotionPath.getCenter(p, mInterpolateVariables, position, pos, velocity, vel);
        }

        /// <summary>
        /// fill the array point with the center coordinates point[0] is filled with the
        /// x coordinate of "time" 0.0 mPoints[point.length-1] is filled with the y coordinate of "time"
        /// 1.0
        /// </summary>
        /// <param name="points">     array to fill (should be 2x the number of mPoints </param>
        /// <param name="pointCount"> </param>
        /// <returns> number of key frames </returns>
        public virtual void buildPath(float[] points, int pointCount)
        {
            float mils = 1.0f / (pointCount - 1);
            SplineSet trans_x = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.TRANSLATION_X];
            SplineSet trans_y = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.TRANSLATION_Y];
            KeyCycleOscillator osc_x = (mCycleMap == null) ? null : mCycleMap[MotionKey.TRANSLATION_X];
            KeyCycleOscillator osc_y = (mCycleMap == null) ? null : mCycleMap[MotionKey.TRANSLATION_Y];

            for (int i = 0; i < pointCount; i++)
            {
                float position = (i) * mils;
                if (mStaggerScale != 1.0f)
                {
                    if (position < mStaggerOffset)
                    {
                        position = 0;
                    }
                    if (position > mStaggerOffset && position < 1.0)
                    {
                        position -= mStaggerOffset;
                        position *= mStaggerScale;
                        position = Math.Min(position, 1.0f);
                    }
                }
                double p = position;

                Easing easing = mStartMotionPath.mKeyFrameEasing;
                float start = 0;
                float end = float.NaN;
                foreach (MotionPaths frame in mMotionPaths)
                {
                    if (frame.mKeyFrameEasing != null)
                    { // this frame has an easing
                        if (frame.time < position)
                        { // frame with easing is before the current pos
                            easing = frame.mKeyFrameEasing; // this is the candidate
                            start = frame.time; // this is also the starting time
                        }
                        else
                        { // frame with easing is past the pos
                            if (float.IsNaN(end))
                            { // we never ended the time line
                                end = frame.time;
                            }
                        }
                    }
                }

                if (easing != null)
                {
                    if (float.IsNaN(end))
                    {
                        end = 1.0f;
                    }
                    float offset = (position - start) / (end - start);
                    offset = (float) easing.get(offset);
                    p = offset * (end - start) + start;

                }

                mSpline[0].getPos(p, mInterpolateData);
                if (mArcSpline != null)
                {
                    if (mInterpolateData.Length > 0)
                    {
                        mArcSpline.getPos(p, mInterpolateData);
                    }
                }
                mStartMotionPath.getCenter(p, mInterpolateVariables, mInterpolateData, points, i * 2);

                if (osc_x != null)
                {
                    points[i * 2] += osc_x.get(position);
                }
                else if (trans_x != null)
                {
                    points[i * 2] += trans_x.get(position);
                }
                if (osc_y != null)
                {
                    points[i * 2 + 1] += osc_y.get(position);
                }
                else if (trans_y != null)
                {
                    points[i * 2 + 1] += trans_y.get(position);
                }
            }
        }

        internal virtual double[] getPos(double position)
        {
            mSpline[0].getPos(position, mInterpolateData);
            if (mArcSpline != null)
            {
                if (mInterpolateData.Length > 0)
                {
                    mArcSpline.getPos(position, mInterpolateData);
                }
            }
            return mInterpolateData;
        }

        /// <summary>
        /// fill the array point with the center coordinates point[0] is filled with the
        /// x coordinate of "time" 0.0 mPoints[point.length-1] is filled with the y coordinate of "time"
        /// 1.0
        /// </summary>
        /// <param name="bounds">     array to fill (should be 2x the number of mPoints </param>
        /// <param name="pointCount"> </param>
        /// <returns> number of key frames </returns>
        internal virtual void buildBounds(float[] bounds, int pointCount)
        {
            float mils = 1.0f / (pointCount - 1);
            SplineSet trans_x = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.TRANSLATION_X];
            SplineSet trans_y = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.TRANSLATION_Y];
            KeyCycleOscillator osc_x = (mCycleMap == null) ? null : mCycleMap[MotionKey.TRANSLATION_X];
            KeyCycleOscillator osc_y = (mCycleMap == null) ? null : mCycleMap[MotionKey.TRANSLATION_Y];

            for (int i = 0; i < pointCount; i++)
            {
                float position = (i) * mils;
                if (mStaggerScale != 1.0f)
                {
                    if (position < mStaggerOffset)
                    {
                        position = 0;
                    }
                    if (position > mStaggerOffset && position < 1.0)
                    {
                        position -= mStaggerOffset;
                        position *= mStaggerScale;
                        position = Math.Min(position, 1.0f);
                    }
                }
                double p = position;

                Easing easing = mStartMotionPath.mKeyFrameEasing;
                float start = 0;
                float end = float.NaN;
                foreach (MotionPaths frame in mMotionPaths)
                {
                    if (frame.mKeyFrameEasing != null)
                    { // this frame has an easing
                        if (frame.time < position)
                        { // frame with easing is before the current pos
                            easing = frame.mKeyFrameEasing; // this is the candidate
                            start = frame.time; // this is also the starting time
                        }
                        else
                        { // frame with easing is past the pos
                            if (float.IsNaN(end))
                            { // we never ended the time line
                                end = frame.time;
                            }
                        }
                    }
                }

                if (easing != null)
                {
                    if (float.IsNaN(end))
                    {
                        end = 1.0f;
                    }
                    float offset = (position - start) / (end - start);
                    offset = (float) easing.get(offset);
                    p = offset * (end - start) + start;

                }

                mSpline[0].getPos(p, mInterpolateData);
                if (mArcSpline != null)
                {
                    if (mInterpolateData.Length > 0)
                    {
                        mArcSpline.getPos(p, mInterpolateData);
                    }
                }
                mStartMotionPath.getBounds(mInterpolateVariables, mInterpolateData, bounds, i * 2);
            }
        }

        private float PreCycleDistance
        {
            get
            {
                int pointCount = 100;
                float[] points = new float[2];
                float sum = 0;
                float mils = 1.0f / (pointCount - 1);
                double x = 0, y = 0;
                for (int i = 0; i < pointCount; i++)
                {
                    float position = (i) * mils;
    
                    double p = position;
    
                    Easing easing = mStartMotionPath.mKeyFrameEasing;
                    float start = 0;
                    float end = float.NaN;
                    foreach (MotionPaths frame in mMotionPaths)
                    {
                        if (frame.mKeyFrameEasing != null)
                        { // this frame has an easing
                            if (frame.time < position)
                            { // frame with easing is before the current pos
                                easing = frame.mKeyFrameEasing; // this is the candidate
                                start = frame.time; // this is also the starting time
                            }
                            else
                            { // frame with easing is past the pos
                                if (float.IsNaN(end))
                                { // we never ended the time line
                                    end = frame.time;
                                }
                            }
                        }
                    }
    
                    if (easing != null)
                    {
                        if (float.IsNaN(end))
                        {
                            end = 1.0f;
                        }
                        float offset = (position - start) / (end - start);
                        offset = (float) easing.get(offset);
                        p = offset * (end - start) + start;
    
                    }
    
                    mSpline[0].getPos(p, mInterpolateData);
                    mStartMotionPath.getCenter(p, mInterpolateVariables, mInterpolateData, points, 0);
                    if (i > 0)
                    {
                        sum += (float)MathExtension.hypot(y - points[1], x - points[0]);
                    }
                    x = points[0];
                    y = points[1];
                }
                return sum;
            }
        }

        internal virtual MotionKeyPosition getPositionKeyframe(int layoutWidth, int layoutHeight, float x, float y)
        {
            FloatRect start = new FloatRect();
            start.left = mStartMotionPath.x;
            start.top = mStartMotionPath.y;
            start.right = start.left + mStartMotionPath.width;
            start.bottom = start.top + mStartMotionPath.height;
            FloatRect end = new FloatRect();
            end.left = mEndMotionPath.x;
            end.top = mEndMotionPath.y;
            end.right = end.left + mEndMotionPath.width;
            end.bottom = end.top + mEndMotionPath.height;
            foreach (MotionKey key in mKeyList)
            {
                if (key is MotionKeyPosition)
                {
                    if (((MotionKeyPosition) key).intersects(layoutWidth, layoutHeight, start, end, x, y))
                    {
                        return (MotionKeyPosition) key;
                    }
                }
            }
            return null;
        }

        public virtual int buildKeyFrames(float[] keyFrames, int[] mode, int[] pos)
        {
            if (keyFrames != null)
            {
                int count = 0;
                double[] time = mSpline[0].TimePoints;
                if (mode != null)
                {
                    foreach (MotionPaths keyFrame in mMotionPaths)
                    {
                        mode[count++] = keyFrame.mMode;
                    }
                    count = 0;
                }
                if (pos != null)
                {
                    foreach (MotionPaths keyFrame in mMotionPaths)
                    {
                        pos[count++] = (int)(100 * keyFrame.position);
                    }
                    count = 0;
                }
                for (int i = 0; i < time.Length; i++)
                {
                    mSpline[0].getPos(time[i], mInterpolateData);
                    mStartMotionPath.getCenter(time[i], mInterpolateVariables, mInterpolateData, keyFrames, count);
                    count += 2;
                }
                return count / 2;
            }
            return 0;
        }

        internal virtual int buildKeyBounds(float[] keyBounds, int[] mode)
        {
            if (keyBounds != null)
            {
                int count = 0;
                double[] time = mSpline[0].TimePoints;
                if (mode != null)
                {
                    foreach (MotionPaths keyFrame in mMotionPaths)
                    {
                        mode[count++] = keyFrame.mMode;
                    }
                    count = 0;
                }

                for (int i = 0; i < time.Length; i++)
                {
                    mSpline[0].getPos(time[i], mInterpolateData);
                    mStartMotionPath.getBounds(mInterpolateVariables, mInterpolateData, keyBounds, count);
                    count += 2;
                }
                return count / 2;
            }
            return 0;
        }

        internal string[] attributeTable;

        internal virtual int getAttributeValues(string attributeType, float[] points, int pointCount)
        {
            float mils = 1.0f / (pointCount - 1);
            SplineSet spline = mAttributesMap[attributeType];
            if (spline == null)
            {
                return -1;
            }
            for (int j = 0; j < points.Length; j++)
            {
                points[j] = spline.get(j / (points.Length - 1));
            }
            return points.Length;
        }

        public virtual void buildRect(float p, float[] path, int offset)
        {
            p = getAdjustedPosition(p, null);
            mSpline[0].getPos(p, mInterpolateData);
            mStartMotionPath.getRect(mInterpolateVariables, mInterpolateData, path, offset);
        }

        internal virtual void buildRectangles(float[] path, int pointCount)
        {
            float mils = 1.0f / (pointCount - 1);
            for (int i = 0; i < pointCount; i++)
            {
                float position = (i) * mils;
                position = getAdjustedPosition(position, null);
                mSpline[0].getPos(position, mInterpolateData);
                mStartMotionPath.getRect(mInterpolateVariables, mInterpolateData, path, i * 8);
            }
        }

        internal virtual float getKeyFrameParameter(int type, float x, float y)
        {

            float dx = mEndMotionPath.x - mStartMotionPath.x;
            float dy = mEndMotionPath.y - mStartMotionPath.y;
            float startCenterX = mStartMotionPath.x + mStartMotionPath.width / 2;
            float startCenterY = mStartMotionPath.y + mStartMotionPath.height / 2;
            float hypotenuse = (float)MathExtension.hypot(dx, dy);
            if (hypotenuse < 0.0000001)
            {
                return float.NaN;
            }

            float vx = x - startCenterX;
            float vy = y - startCenterY;
            float distFromStart = (float)MathExtension.hypot(vx, vy);
            if (distFromStart == 0)
            {
                return 0;
            }
            float pathDistance = (vx * dx + vy * dy);

            switch (type)
            {
                case PATH_PERCENT:
                    return pathDistance / hypotenuse;
                case PATH_PERPENDICULAR:
                    return (float) Math.Sqrt(hypotenuse * hypotenuse - pathDistance * pathDistance);
                case HORIZONTAL_PATH_X:
                    return vx / dx;
                case HORIZONTAL_PATH_Y:
                    return vy / dx;
                case VERTICAL_PATH_X:
                    return vx / dy;
                case VERTICAL_PATH_Y:
                    return vy / dy;
            }
            return 0;
        }

        private void insertKey(MotionPaths point)
        {
            MotionPaths redundant = null;
            foreach (MotionPaths p in mMotionPaths)
            {
                if (point.position == p.position)
                {
                    redundant = p;
                }
            }
            if (redundant != null)
            {
                mMotionPaths.Remove(redundant);
            }
            //int pos = Collections.binarySearch(mMotionPaths, point);
            int pos = mMotionPaths.BinarySearch(point);
            if (pos == 0)
            {
                Utils.loge(TAG, " KeyPath position \"" + point.position + "\" outside of range");
            }
            mMotionPaths.Insert(-pos - 1, point);
        }

        internal virtual void addKeys(List<MotionKey> list)
        {
            mKeyList.AddRange(list);
            if (DEBUG)
            {
                foreach (MotionKey key in mKeyList)
                {
                    Utils.log(TAG, " ################ set = " + key.GetType().Name);
                }
            }
        }

        public virtual void addKey(MotionKey key)
        {
            mKeyList.Add(key);
            if (DEBUG)
            {
                Utils.log(TAG, " ################ addKey = " + key.GetType().Name);
            }
        }

        public virtual int PathMotionArc
        {
            set
            {
                mPathMotionArc = value;
            }
        }

        /// <summary>
        /// Called after all TimePoints & Cycles have been added;
        /// Spines are evaluated
        /// </summary>
        public virtual void setup(int parentWidth, int parentHeight, float transitionDuration, long currentTime)
        {
            HashSet<string> springAttributes = new HashSet<string>(); // attributes we need to interpolate
            HashSet<string> timeCycleAttributes = new HashSet<string>(); // attributes we need to interpolate
            HashSet<string> splineAttributes = new HashSet<string>(); // attributes we need to interpolate
            HashSet<string> cycleAttributes = new HashSet<string>(); // attributes we need to oscillate
            Dictionary<string, int?> interpolation = new Dictionary<string, int?>();
            List<MotionKeyTrigger> triggerList = null;
            if (DEBUG)
            {
                if (mKeyList == null)
                {
                    Utils.log(TAG, ">>>>>>>>>>>>>>> mKeyList==null");

                }
                else
                {
                    Utils.log(TAG, ">>>>>>>>>>>>>>> mKeyList for " + mView.Name);

                }
            }

            if (mPathMotionArc != UNSET)
            {
                mStartMotionPath.mPathMotionArc = mPathMotionArc;
            }

            mStartPoint.different(mEndPoint, splineAttributes);
            if (DEBUG)
            {
                HashSet<string> attr = new HashSet<string>();
                mStartPoint.different(mEndPoint, attr);
                //Utils.log(TAG, ">>>>>>>>>>>>>>> MotionConstrainedPoint found " + Arrays.ToString(attr.ToArray()));
                Utils.log(TAG, ">>>>>>>>>>>>>>> MotionConstrainedPoint found " + attr.ToArray().ToString());
            }
            if (mKeyList != null)
            {
                foreach (MotionKey key in mKeyList)
                {
                    if (key is MotionKeyPosition)
                    {
                        MotionKeyPosition keyPath = (MotionKeyPosition) key;
                        insertKey(new MotionPaths(parentWidth, parentHeight, keyPath, mStartMotionPath, mEndMotionPath));
                        if (keyPath.mCurveFit != UNSET)
                        {
                            mCurveFitType = keyPath.mCurveFit;
                        }
                    }
                    else if (key is MotionKeyCycle)
                    {
                        key.getAttributeNames(cycleAttributes);
                    }
                    else if (key is MotionKeyTimeCycle)
                    {
                        key.getAttributeNames(timeCycleAttributes);
                    }
                    else if (key is MotionKeyTrigger)
                    {
                        if (triggerList == null)
                        {
                            triggerList = new List<MotionKeyTrigger>();
                        }
                        triggerList.Add((MotionKeyTrigger) key);
                    }
                    else
                    {
                        key.Interpolation = interpolation;
                        key.getAttributeNames(splineAttributes);
                    }
                }
            }

            //--------------------------- trigger support --------------------

            if (triggerList != null)
            {
                mKeyTriggers = triggerList.ToArray();
            }

            //--------------------------- splines support --------------------
            if (splineAttributes.Count > 0)
            {
                mAttributesMap = new Dictionary<string,SplineSet>();
                foreach (string attribute in splineAttributes)
                {
                    SplineSet splineSets;
                    if (attribute.StartsWith("CUSTOM,", StringComparison.Ordinal))
                    {
                        KeyFrameArray.CustomVar attrList = new KeyFrameArray.CustomVar();
                        string customAttributeName = attribute.Split(",", true)[1];
                        foreach (MotionKey key in mKeyList)
                        {
                            if (key.mCustom == null)
                            {
                                continue;
                            }
                            CustomVariable customAttribute = key.mCustom[customAttributeName];
                            if (customAttribute != null)
                            {
                                attrList.append(key.mFramePosition, customAttribute);
                            }
                        }
                        splineSets = SplineSet.makeCustomSplineSet(attribute, attrList);
                    }
                    else
                    {
                        splineSets = SplineSet.makeSpline(attribute, currentTime);
                    }
                    if (splineSets == null)
                    {
                        continue;
                    }
                    splineSets.Type = attribute;
                    mAttributesMap[attribute] = splineSets;
                }
                if (mKeyList != null)
                {
                    foreach (MotionKey key in mKeyList)
                    {
                        if ((key is MotionKeyAttributes))
                        {
                            key.addValues(mAttributesMap);
                        }
                    }
                }
                mStartPoint.addValues(mAttributesMap, 0);
                mEndPoint.addValues(mAttributesMap, 100);

                foreach (string spline in mAttributesMap.Keys)
                {
                    int curve = CurveFit.SPLINE; // default is SPLINE
                    if (interpolation.ContainsKey(spline))
                    {
                        int? boxedCurve = interpolation[spline];
                        if (boxedCurve != null)
                        {
                            curve = boxedCurve.Value;
                        }
                    }
                    SplineSet splineSet = mAttributesMap[spline];
                    if (splineSet != null)
                    {
                        splineSet.setup(curve);
                    }
                }
            }

            //--------------------------- timeCycle support --------------------
            if (timeCycleAttributes.Count > 0)
            {
                if (mTimeCycleAttributesMap == null)
                {
                    mTimeCycleAttributesMap = new Dictionary<string,TimeCycleSplineSet>();
                }
                foreach (string attribute in timeCycleAttributes)
                {
                    if (mTimeCycleAttributesMap.ContainsKey(attribute))
                    {
                        continue;
                    }

                    SplineSet splineSets = null;
                    if (attribute.StartsWith("CUSTOM,", StringComparison.Ordinal))
                    {
                        KeyFrameArray.CustomVar attrList = new KeyFrameArray.CustomVar();
                        string customAttributeName = attribute.Split(",", true)[1];
                        foreach (MotionKey key in mKeyList)
                        {
                            if (key.mCustom == null)
                            {
                                continue;
                            }
                            CustomVariable customAttribute = key.mCustom[customAttributeName];
                            if (customAttribute != null)
                            {
                                attrList.append(key.mFramePosition, customAttribute);
                            }
                        }
                        splineSets = SplineSet.makeCustomSplineSet(attribute, attrList);
                    }
                    else
                    {
                        splineSets = SplineSet.makeSpline(attribute, currentTime);
                    }
                    if (splineSets == null)
                    {
                        continue;
                    }
                    splineSets.Type = attribute;
    //                mTimeCycleAttributesMap.put(attribute, splineSets);
                }

                if (mKeyList != null)
                {
                    foreach (MotionKey key in mKeyList)
                    {
                        if (key is MotionKeyTimeCycle)
                        {
                            ((MotionKeyTimeCycle) key).addTimeValues(mTimeCycleAttributesMap);
                        }
                    }
                }

                foreach (string spline in mTimeCycleAttributesMap.Keys)
                {
                    int curve = CurveFit.SPLINE; // default is SPLINE
                    if (interpolation.ContainsKey(spline))
                    {
                        curve = interpolation[spline].Value;
                    }
                    mTimeCycleAttributesMap[spline].setup(curve);
                }
            }

            //--------------------------------- end new key frame 2

            MotionPaths[] points = new MotionPaths[2 + mMotionPaths.Count];
            int count = 1;
            points[0] = mStartMotionPath;
            points[points.Length - 1] = mEndMotionPath;
            if (mMotionPaths.Count > 0 && mCurveFitType == MotionKey.UNSET)
            {
                mCurveFitType = CurveFit.SPLINE;
            }
            foreach (MotionPaths point in mMotionPaths)
            {
                points[count++] = point;
            }

            // -----  setup custom attributes which must be in the start and end constraint sets
            int variables = 18;
            HashSet<string> attributeNameSet = new HashSet<string>();
            foreach (string s in mEndMotionPath.customAttributes.Keys)
            {
                if (mStartMotionPath.customAttributes.ContainsKey(s))
                {
                    if (!splineAttributes.Contains("CUSTOM," + s))
                    {
                        attributeNameSet.Add(s);
                    }
                }
            }

            //mAttributeNames = attributeNameSet.ToArray(new string[0]);
            mAttributeNames = attributeNameSet.ToArray();//TODO:理解原来字符串参数是何意
            mAttributeInterpolatorCount = new int[mAttributeNames.Length];
            for (int i = 0; i < mAttributeNames.Length; i++)
            {
                string attributeName = mAttributeNames[i];
                mAttributeInterpolatorCount[i] = 0;
                for (int j = 0; j < points.Length; j++)
                {
                    if (points[j].customAttributes.ContainsKey(attributeName))
                    {
                        CustomVariable attribute = points[j].customAttributes[attributeName];
                        if (attribute != null)
                        {
                            mAttributeInterpolatorCount[i] += attribute.numberOfInterpolatedValues();
                            break;
                        }
                    }
                }
            }
            bool arcMode = points[0].mPathMotionArc != UNSET;
            bool[] mask = new bool[variables + mAttributeNames.Length]; // defaults to false
            for (int i = 1; i < points.Length; i++)
            {
                points[i].different(points[i - 1], mask, mAttributeNames, arcMode);
            }

            count = 0;
            for (int i = 1; i < mask.Length; i++)
            {
                if (mask[i])
                {
                    count++;
                }
            }

            mInterpolateVariables = new int[count];
            int varLen = Math.Max(2, count);
            mInterpolateData = new double[varLen];
            mInterpolateVelocity = new double[varLen];

            count = 0;
            for (int i = 1; i < mask.Length; i++)
            {
                if (mask[i])
                {
                    mInterpolateVariables[count++] = i;
                }
            }

//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] splineData = new double[points.Length][mInterpolateVariables.Length];
            double[][] splineData = RectangularArrays.ReturnRectangularDoubleArray(points.Length, mInterpolateVariables.Length);
            double[] timePoint = new double[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                points[i].fillStandard(splineData[i], mInterpolateVariables);
                timePoint[i] = points[i].time;
            }

            for (int j = 0; j < mInterpolateVariables.Length; j++)
            {
                int interpolateVariable = mInterpolateVariables[j];
                if (interpolateVariable < MotionPaths.names.Length)
                {
                    string s = MotionPaths.names[mInterpolateVariables[j]] + " [";
                    for (int i = 0; i < points.Length; i++)
                    {
                        s += splineData[i][j];
                    }
                }
            }
            mSpline = new CurveFit[1 + mAttributeNames.Length];

            for (int i = 0; i < mAttributeNames.Length; i++)
            {
                int pointCount = 0;
                double[][] splinePoints = null;
                double[] timePoints = null;
                string name = mAttributeNames[i];

                for (int j = 0; j < points.Length; j++)
                {
                    if (points[j].hasCustomData(name))
                    {
                        if (splinePoints == null)
                        {
                            timePoints = new double[points.Length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: splinePoints = new double[points.Length][points[j].getCustomDataCount(name)];
                            splinePoints = RectangularArrays.ReturnRectangularDoubleArray(points.Length, points[j].getCustomDataCount(name));
                        }
                        timePoints[pointCount] = points[j].time;
                        points[j].getCustomData(name, splinePoints[pointCount], 0);
                        pointCount++;
                    }
                }
                //timePoints = Arrays.copyOf(timePoints, pointCount);
                timePoints = timePoints.Copy<double>(pointCount);
                //splinePoints = Arrays.copyOf(splinePoints, pointCount);
                splinePoints = splinePoints.Copy<double[]>(pointCount);
                mSpline[i + 1] = CurveFit.get(mCurveFitType, timePoints, splinePoints);
            }

            mSpline[0] = CurveFit.get(mCurveFitType, timePoint, splineData);
            // --------------------------- SUPPORT ARC MODE --------------
            if (points[0].mPathMotionArc != UNSET)
            {
                int size = points.Length;
                int[] mode = new int[size];
                double[] time = new double[size];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] values = new double[size][2];
                double[][] values = RectangularArrays.ReturnRectangularDoubleArray(size, 2);
                for (int i = 0; i < size; i++)
                {
                    mode[i] = points[i].mPathMotionArc;
                    time[i] = points[i].time;
                    values[i][0] = points[i].x;
                    values[i][1] = points[i].y;
                }

                mArcSpline = CurveFit.getArc(mode, time, values);
            }

            //--------------------------- Cycle support --------------------
            float distance = float.NaN;
            mCycleMap = new Dictionary<string,KeyCycleOscillator>();
            if (mKeyList != null)
            {
                foreach (string attribute in cycleAttributes)
                {
                    KeyCycleOscillator cycle = KeyCycleOscillator.makeWidgetCycle(attribute);
                    if (cycle == null)
                    {
                        continue;
                    }

                    if (cycle.variesByPath())
                    {
                        if (float.IsNaN(distance))
                        {
                            distance = PreCycleDistance;
                        }
                    }
                    cycle.Type = attribute;
                    mCycleMap[attribute] = cycle;
                }
                foreach (MotionKey key in mKeyList)
                {
                    if (key is MotionKeyCycle)
                    {
                        ((MotionKeyCycle) key).addCycleValues(mCycleMap);
                    }
                }
                foreach (KeyCycleOscillator cycle in mCycleMap.Values)
                {
                    cycle.setup(distance);
                }
            }

            if (DEBUG)
            {
                Utils.log(TAG, "Animation of splineAttributes " + splineAttributes.ToArray().ToString());
                Utils.log(TAG, "Animation of cycle " +mCycleMap.Keys.ToArray().ToString());
                if (mAttributesMap != null)
                {
                    Utils.log(TAG, " splines = " + mAttributesMap.Keys.ToArray().ToString());
                    foreach (string s in mAttributesMap.Keys)
                    {
                        Utils.log(TAG, s + " = " + mAttributesMap[s]);
                    }
                }
                Utils.log(TAG, " ---------------------------------------- ");
            }

            //--------------------------- end cycle support ----------------
        }

        /// <summary>
        /// Debug string
        /// 
        /// @return
        /// </summary>
        public override string ToString()
        {
            return " start: x: " + mStartMotionPath.x + " y: " + mStartMotionPath.y + " end: x: " + mEndMotionPath.x + " y: " + mEndMotionPath.y;
        }

        private void readView(MotionPaths motionPaths)
        {
            motionPaths.setBounds((int) mView.X, (int) mView.Y, mView.Width, mView.Height);
        }

        public virtual MotionWidget View
        {
            set
            {
                mView = value;
            }
            get
            {
                return mView;
            }
        }


        public virtual MotionWidget Start
        {
            set
            {
                mStartMotionPath.time = 0;
                mStartMotionPath.position = 0;
                mStartMotionPath.setBounds(value.X, value.Y, value.Width, value.Height);
                mStartMotionPath.applyParameters(value);
                mStartPoint.State = value;
            }
        }

        public virtual MotionWidget End
        {
            set
            {
                mEndMotionPath.time = 1;
                mEndMotionPath.position = 1;
                readView(mEndMotionPath);
                mEndMotionPath.setBounds(value.Left, value.Top, value.Width, value.Height);
                mEndMotionPath.applyParameters(value);
                mEndPoint.State = value;
            }
        }

        public virtual void setStartState(ViewState rect, MotionWidget v, int rotation, int preWidth, int preHeight)
        {
            mStartMotionPath.time = 0;
            mStartMotionPath.position = 0;
            int cx, cy;
            Rect r = new Rect();
            switch (rotation)
            {
                case 2:
                    cx = rect.left + rect.right;
                    cy = rect.top + rect.bottom;
                    r.left = preHeight - (cy + rect.width()) / 2;
                    r.top = (cx - rect.height()) / 2;
                    r.right = r.left + rect.width();
                    r.bottom = r.top + rect.height();
                    break;
                case 1:
                    cx = rect.left + rect.right;
                    cy = rect.top + rect.bottom;
                    r.left = (cy - rect.width()) / 2;
                    r.top = preWidth - (cx + rect.height()) / 2;
                    r.right = r.left + rect.width();
                    r.bottom = r.top + rect.height();
                    break;
            }
            mStartMotionPath.setBounds(r.left, r.top, r.width(), r.height());
            mStartPoint.setState(r, v, rotation, rect.rotation);
        }

        internal virtual void rotate(Rect rect, Rect @out, int rotation, int preHeight, int preWidth)
        {
            int cx, cy;
            switch (rotation)
            {

                case MotionConstraintSet.ROTATE_PORTRATE_OF_LEFT:
                    cx = rect.left + rect.right;
                    cy = rect.top + rect.bottom;
                    @out.left = preHeight - (cy + rect.width()) / 2;
                    @out.top = (cx - rect.height()) / 2;
                    @out.right = @out.left + rect.width();
                    @out.bottom = @out.top + rect.height();
                    break;
                case MotionConstraintSet.ROTATE_PORTRATE_OF_RIGHT:
                    cx = rect.left + rect.right;
                    cy = rect.top + rect.bottom;
                    @out.left = (cy - rect.width()) / 2;
                    @out.top = preWidth - (cx + rect.height()) / 2;
                    @out.right = @out.left + rect.width();
                    @out.bottom = @out.top + rect.height();
                    break;
                case MotionConstraintSet.ROTATE_LEFT_OF_PORTRATE:
                    cx = rect.left + rect.right;
                    cy = rect.bottom + rect.top;
                    @out.left = preHeight - (cy + rect.width()) / 2;
                    @out.top = (cx - rect.height()) / 2;
                    @out.right = @out.left + rect.width();
                    @out.bottom = @out.top + rect.height();
                    break;
                case MotionConstraintSet.ROTATE_RIGHT_OF_PORTRATE:
                    cx = rect.left + rect.right;
                    cy = rect.top + rect.bottom;
                    @out.left = rect.height() / 2 + rect.top - cx / 2;
                    @out.top = preWidth - (cx + rect.height()) / 2;
                    @out.right = @out.left + rect.width();
                    @out.bottom = @out.top + rect.height();
                    break;
            }
        }

        // Todo : Implement  QuantizeMotion scene rotate
        //    void setStartState(Rect cw, ConstraintSet constraintSet, int parentWidth, int parentHeight) {
        //        int rotate = constraintSet.mRotate; // for rotated frames
        //        if (rotate != 0) {
        //            rotate(cw, mTempRect, rotate, parentWidth, parentHeight);
        //        }
        //        mStartMotionPath.time = 0;
        //        mStartMotionPath.position = 0;
        //        readView(mStartMotionPath);
        //        mStartMotionPath.setBounds(cw.left, cw.top, cw.width(), cw.height());
        //        ConstraintSet.Constraint constraint = constraintSet.getParameters(mId);
        //        mStartMotionPath.applyParameters(constraint);
        //        mMotionStagger = constraint.motion.mMotionStagger;
        //        mStartPoint.setState(cw, constraintSet, rotate, mId);
        //        mTransformPivotTarget = constraint.transform.transformPivotTarget;
        //        mQuantizeMotionSteps = constraint.motion.mQuantizeMotionSteps;
        //        mQuantizeMotionPhase = constraint.motion.mQuantizeMotionPhase;
        //        mQuantizeMotionInterpolator = getInterpolator(mView.getContext(),
        //                constraint.motion.mQuantizeInterpolatorType,
        //                constraint.motion.mQuantizeInterpolatorString,
        //                constraint.motion.mQuantizeInterpolatorID
        //        );
        //    }

        internal const int EASE_IN_OUT = 0;
        internal const int EASE_IN = 1;
        internal const int EASE_OUT = 2;
        internal const int LINEAR = 3;
        internal const int BOUNCE = 4;
        internal const int OVERSHOOT = 5;
        private const int SPLINE_STRING = -1;
        private const int INTERPOLATOR_REFERENCE_ID = -2;
        private const int INTERPOLATOR_UNDEFINED = -3;

        private static DifferentialInterpolator getInterpolator(int type, string interpolatorString, int id)
        {
            switch (type)
            {
                case SPLINE_STRING:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final androidx.constraintlayout.core.motion.utils.Easing easing = androidx.constraintlayout.core.motion.utils.Easing.getInterpolator(interpolatorString);
                    Easing easing = Easing.getInterpolator(interpolatorString);
                    return new DifferentialInterpolatorAnonymousInnerClass(easing);

            }
            return null;
        }

        private class DifferentialInterpolatorAnonymousInnerClass : DifferentialInterpolator
        {
            private Easing easing;

            public DifferentialInterpolatorAnonymousInnerClass(Easing easing)
            {
                this.easing = easing;
            }

            internal float mX;

            public virtual float getInterpolation(float x)
            {
                mX = x;
                return (float) easing.get(x);
            }

            public virtual float Velocity
            {
                get
                {
                    return (float) easing.getDiff(mX);
                }
            }
        }

    //    void setEndState(Rect cw, ConstraintSet constraintSet, int parentWidth, int parentHeight) {
    //        int rotate = constraintSet.mRotate; // for rotated frames
    //        if (rotate != 0) {
    //            rotate(cw, mTempRect, rotate, parentWidth, parentHeight);
    //            cw = mTempRect;
    //        }
    //        mEndMotionPath.time = 1;
    //        mEndMotionPath.position = 1;
    //        readView(mEndMotionPath);
    //        mEndMotionPath.setBounds(cw.left, cw.top, cw.width(), cw.height());
    //        mEndMotionPath.applyParameters(constraintSet.getParameters(mId));
    //        mEndPoint.setState(cw, constraintSet, rotate, mId);
    //    }

        internal virtual MotionWidget BothStates
        {
            set
            {
                mStartMotionPath.time = 0;
                mStartMotionPath.position = 0;
                mNoMovement = true;
                mStartMotionPath.setBounds(value.X, value.Y, value.Width, value.Height);
                mEndMotionPath.setBounds(value.X, value.Y, value.Width, value.Height);
                mStartPoint.State = value;
                mEndPoint.State = value;
            }
        }

        /// <summary>
        /// Calculates the adjusted (and optional velocity)
        /// Note if requesting velocity staggering is not considered
        /// </summary>
        /// <param name="position"> position pre stagger </param>
        /// <param name="velocity"> return velocity </param>
        /// <returns> actual position accounting for easing and staggering </returns>
        private float getAdjustedPosition(float position, float[] velocity)
        {
            if (velocity != null)
            {
                velocity[0] = 1;
            }
            else if (mStaggerScale != 1.0)
            {
                if (position < mStaggerOffset)
                {
                    position = 0;
                }
                if (position > mStaggerOffset && position < 1.0)
                {
                    position -= mStaggerOffset;
                    position *= mStaggerScale;
                    position = Math.Min(position, 1.0f);
                }
            }

            // adjust the position based on the easing curve
            float adjusted = position;
            Easing easing = mStartMotionPath.mKeyFrameEasing;
            float start = 0;
            float end = float.NaN;
            foreach (MotionPaths frame in mMotionPaths)
            {
                if (frame.mKeyFrameEasing != null)
                { // this frame has an easing
                    if (frame.time < position)
                    { // frame with easing is before the current pos
                        easing = frame.mKeyFrameEasing; // this is the candidate
                        start = frame.time; // this is also the starting time
                    }
                    else
                    { // frame with easing is past the pos
                        if (float.IsNaN(end))
                        { // we never ended the time line
                            end = frame.time;
                        }
                    }
                }
            }

            if (easing != null)
            {
                if (float.IsNaN(end))
                {
                    end = 1.0f;
                }
                float offset = (position - start) / (end - start);
                float new_offset = (float) easing.get(offset);
                adjusted = new_offset * (end - start) + start;
                if (velocity != null)
                {
                    velocity[0] = (float) easing.getDiff(offset);
                }
            }
            return adjusted;
        }

        internal virtual void endTrigger(bool start)
        {
    //        if ("button".equals(Debug.getName(mView)))
    //            if (mKeyTriggers != null) {
    //                for (int i = 0; i < mKeyTriggers.length; i++) {
    //                    mKeyTriggers[i].conditionallyFire(start ? -100 : 100, mView);
    //                }
    //            }
        }
        //##############################################################################################
        //$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%
        //$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%
        //$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%
        //$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%$%
        //##############################################################################################

        /// <summary>
        /// The main driver of interpolation
        /// </summary>
        /// <param name="child"> </param>
        /// <param name="global_position"> </param>
        /// <param name="time"> </param>
        /// <param name="keyCache"> </param>
        /// <returns> do you need to keep animating </returns>
        public virtual bool interpolate(MotionWidget child, float global_position, long time, KeyCache keyCache)
        {
            bool timeAnimation = false;
            float position = getAdjustedPosition(global_position, null);
            // This quantize the position into steps e.g 4 steps = 0-0.25,0.25-0.50 etc
            if (mQuantizeMotionSteps != UNSET)
            {
                float pin = position;
                float steps = 1.0f / mQuantizeMotionSteps; // the length of a step
                float jump = (float) Math.Floor(position / steps) * steps; // step jumps
                float section = (position % steps) / steps; // float from 0 to 1 in a step

                if (!float.IsNaN(mQuantizeMotionPhase))
                {
                    section = (section + mQuantizeMotionPhase) % 1;
                }
                if (mQuantizeMotionInterpolator != null)
                {
                    section = mQuantizeMotionInterpolator.getInterpolation(section);
                }
                else
                {
                    section = section > 0.5 ? 1 : 0;
                }
                position = section * steps + jump;
            }
            // MotionKeyTimeCycle.PathRotate timePathRotate = null;
            if (mAttributesMap != null)
            {
                foreach (SplineSet aSpline in mAttributesMap.Values)
                {
                    aSpline.setProperty(child, position);
                }
            }

            //       TODO add KeyTimeCycle
            //        if (mTimeCycleAttributesMap != null) {
            //            for (ViewTimeCycle aSpline : mTimeCycleAttributesMap.values()) {
            //                if (aSpline instanceof ViewTimeCycle.PathRotate) {
            //                    timePathRotate = (ViewTimeCycle.PathRotate) aSpline;
            //                    continue;
            //                }
            //                timeAnimation |= aSpline.setProperty(child, position, time, keyCache);
            //            }
            //        }

            if (mSpline != null)
            {
                mSpline[0].getPos(position, mInterpolateData);
                mSpline[0].getSlope(position, mInterpolateVelocity);
                if (mArcSpline != null)
                {
                    if (mInterpolateData.Length > 0)
                    {
                        mArcSpline.getPos(position, mInterpolateData);
                        mArcSpline.getSlope(position, mInterpolateVelocity);
                    }
                }

                if (!mNoMovement)
                {
                    mStartMotionPath.setView(position, child, mInterpolateVariables, mInterpolateData, mInterpolateVelocity, null);
                }
                if (mTransformPivotTarget != UNSET)
                {
                    if (mTransformPivotView == null)
                    {
                        MotionWidget layout = (MotionWidget) child.Parent;
                        mTransformPivotView = layout.findViewById(mTransformPivotTarget);
                    }
                    if (mTransformPivotView != null)
                    {
                        float cy = (mTransformPivotView.Top + mTransformPivotView.Bottom) / 2.0f;
                        float cx = (mTransformPivotView.Left + mTransformPivotView.Right) / 2.0f;
                        if (child.Right - child.Left > 0 && child.Bottom - child.Top > 0)
                        {
                            float px = (cx - child.Left);
                            float py = (cy - child.Top);
                            child.PivotX = px;
                            child.PivotY = py;
                        }
                    }
                }

                //       TODO add support for path rotate
                //            if (mAttributesMap != null) {
                //                for (SplineSet aSpline : mAttributesMap.values()) {
                //                    if (aSpline instanceof ViewSpline.PathRotate && mInterpolateVelocity.length > 1)
                //                        ((ViewSpline.PathRotate) aSpline).setPathRotate(child, position,
                //                                mInterpolateVelocity[0], mInterpolateVelocity[1]);
                //                }
                //
                //            }
                //            if (timePathRotate != null) {
                //                timeAnimation |= timePathRotate.setPathRotate(child, keyCache, position, time,
                //                        mInterpolateVelocity[0], mInterpolateVelocity[1]);
                //            }

                for (int i = 1; i < mSpline.Length; i++)
                {
                    CurveFit spline = mSpline[i];
                    spline.getPos(position, mValuesBuff);
                    //interpolated here
                    mStartMotionPath.customAttributes[mAttributeNames[i - 1]].setInterpolatedValue(child, mValuesBuff);
                }
                if (mStartPoint.mVisibilityMode == MotionWidget.VISIBILITY_MODE_NORMAL)
                {
                    if (position <= 0.0f)
                    {
                        child.Visibility = mStartPoint.visibility;
                    }
                    else if (position >= 1.0f)
                    {
                        child.Visibility = mEndPoint.visibility;
                    }
                    else if (mEndPoint.visibility != mStartPoint.visibility)
                    {
                        child.Visibility = MotionWidget.VISIBLE;
                    }
                }

                if (mKeyTriggers != null)
                {
                    for (int i = 0; i < mKeyTriggers.Length; i++)
                    {
                        mKeyTriggers[i].conditionallyFire(position, child);
                    }
                }
            }
            else
            {
                // do the interpolation

                float float_l = (mStartMotionPath.x + (mEndMotionPath.x - mStartMotionPath.x) * position);
                float float_t = (mStartMotionPath.y + (mEndMotionPath.y - mStartMotionPath.y) * position);
                float float_width = (mStartMotionPath.width + (mEndMotionPath.width - mStartMotionPath.width) * position);
                float float_height = (mStartMotionPath.height + (mEndMotionPath.height - mStartMotionPath.height) * position);
                int l = (int)(0.5f + float_l);
                int t = (int)(0.5f + float_t);
                int r = (int)(0.5f + float_l + float_width);
                int b = (int)(0.5f + float_t + float_height);
                int width = r - l;
                int height = b - t;

                if (FAVOR_FIXED_SIZE_VIEWS)
                {
                    l = (int)(mStartMotionPath.x + (mEndMotionPath.x - mStartMotionPath.x) * position);
                    t = (int)(mStartMotionPath.y + (mEndMotionPath.y - mStartMotionPath.y) * position);
                    width = (int)(mStartMotionPath.width + (mEndMotionPath.width - mStartMotionPath.width) * position);
                    height = (int)(mStartMotionPath.height + (mEndMotionPath.height - mStartMotionPath.height) * position);
                    r = l + width;
                    b = t + height;
                }
                // widget is responsible to call measure
                child.layout(l, t, r, b);
            }

            // TODO add pathRotate KeyCycles
            if (mCycleMap != null)
            {
                foreach (KeyCycleOscillator osc in mCycleMap.Values)
                {
                    if (osc is KeyCycleOscillator.PathRotateSet)
                    {
                        ((KeyCycleOscillator.PathRotateSet) osc).setPathRotate(child, position, mInterpolateVelocity[0], mInterpolateVelocity[1]);
                    }
                    else
                    {
                        osc.setProperty(child, position);
                    }
                }
            }
            //   When we support TimeCycle return true if repaint is needed
            //        return timeAnimation;
            return false;
        }

        /// <summary>
        /// This returns the differential with respect to the animation layout position (Progress)
        /// of a point on the view (post layout effects are not computed)
        /// </summary>
        /// <param name="position">    position in time </param>
        /// <param name="locationX">   the x location on the view (0 = left edge, 1 = right edge) </param>
        /// <param name="locationY">   the y location on the view (0 = top, 1 = bottom) </param>
        /// <param name="mAnchorDpDt"> returns the differential of the motion with respect to the position </param>
        internal virtual void getDpDt(float position, float locationX, float locationY, float[] mAnchorDpDt)
        {
            position = getAdjustedPosition(position, mVelocity);

            if (mSpline != null)
            {
                mSpline[0].getSlope(position, mInterpolateVelocity);
                mSpline[0].getPos(position, mInterpolateData);
                float v = mVelocity[0];
                for (int i = 0; i < mInterpolateVelocity.Length; i++)
                {
                    mInterpolateVelocity[i] *= v;
                }

                if (mArcSpline != null)
                {
                    if (mInterpolateData.Length > 0)
                    {
                        mArcSpline.getPos(position, mInterpolateData);
                        mArcSpline.getSlope(position, mInterpolateVelocity);
                        mStartMotionPath.setDpDt(locationX, locationY, mAnchorDpDt, mInterpolateVariables, mInterpolateVelocity, mInterpolateData);
                    }
                    return;
                }
                mStartMotionPath.setDpDt(locationX, locationY, mAnchorDpDt, mInterpolateVariables, mInterpolateVelocity, mInterpolateData);
                return;
            }
            // do the interpolation
            float dleft = (mEndMotionPath.x - mStartMotionPath.x);
            float dTop = (mEndMotionPath.y - mStartMotionPath.y);
            float dWidth = (mEndMotionPath.width - mStartMotionPath.width);
            float dHeight = (mEndMotionPath.height - mStartMotionPath.height);
            float dRight = dleft + dWidth;
            float dBottom = dTop + dHeight;
            mAnchorDpDt[0] = dleft * (1 - locationX) + dRight * (locationX);
            mAnchorDpDt[1] = dTop * (1 - locationY) + dBottom * (locationY);
        }

        /// <summary>
        /// This returns the differential with respect to the animation post layout transform
        /// of a point on the view
        /// </summary>
        /// <param name="position">    position in time </param>
        /// <param name="width">       width of the view </param>
        /// <param name="height">      height of the view </param>
        /// <param name="locationX">   the x location on the view (0 = left edge, 1 = right edge) </param>
        /// <param name="locationY">   the y location on the view (0 = top, 1 = bottom) </param>
        /// <param name="mAnchorDpDt"> returns the differential of the motion with respect to the position </param>
        internal virtual void getPostLayoutDvDp(float position, int width, int height, float locationX, float locationY, float[] mAnchorDpDt)
        {
            if (DEBUG)
            {
                Utils.log(TAG, " position= " + position + " location= " + locationX + " , " + locationY);
            }
            position = getAdjustedPosition(position, mVelocity);

            SplineSet trans_x = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.TRANSLATION_X];
            SplineSet trans_y = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.TRANSLATION_Y];
            SplineSet rotation = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.ROTATION];
            SplineSet scale_x = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.SCALE_X];
            SplineSet scale_y = (mAttributesMap == null) ? null : mAttributesMap[MotionKey.SCALE_Y];

            KeyCycleOscillator osc_x = (mCycleMap == null) ? null : mCycleMap[MotionKey.TRANSLATION_X];
            KeyCycleOscillator osc_y = (mCycleMap == null) ? null : mCycleMap[MotionKey.TRANSLATION_Y];
            KeyCycleOscillator osc_r = (mCycleMap == null) ? null : mCycleMap[MotionKey.ROTATION];
            KeyCycleOscillator osc_sx = (mCycleMap == null) ? null : mCycleMap[MotionKey.SCALE_X];
            KeyCycleOscillator osc_sy = (mCycleMap == null) ? null : mCycleMap[MotionKey.SCALE_Y];

            VelocityMatrix vmat = new VelocityMatrix();
            vmat.clear();
            vmat.setRotationVelocity(rotation, position);
            vmat.setTranslationVelocity(trans_x, trans_y, position);
            vmat.setScaleVelocity(scale_x, scale_y, position);
            vmat.setRotationVelocity(osc_r, position);
            vmat.setTranslationVelocity(osc_x, osc_y, position);
            vmat.setScaleVelocity(osc_sx, osc_sy, position);
            if (mArcSpline != null)
            {
                if (mInterpolateData.Length > 0)
                {
                    mArcSpline.getPos(position, mInterpolateData);
                    mArcSpline.getSlope(position, mInterpolateVelocity);
                    mStartMotionPath.setDpDt(locationX, locationY, mAnchorDpDt, mInterpolateVariables, mInterpolateVelocity, mInterpolateData);
                }
                vmat.applyTransform(locationX, locationY, width, height, mAnchorDpDt);
                return;
            }
            if (mSpline != null)
            {
                position = getAdjustedPosition(position, mVelocity);
                mSpline[0].getSlope(position, mInterpolateVelocity);
                mSpline[0].getPos(position, mInterpolateData);
                float v = mVelocity[0];
                for (int i = 0; i < mInterpolateVelocity.Length; i++)
                {
                    mInterpolateVelocity[i] *= v;
                }
                mStartMotionPath.setDpDt(locationX, locationY, mAnchorDpDt, mInterpolateVariables, mInterpolateVelocity, mInterpolateData);
                vmat.applyTransform(locationX, locationY, width, height, mAnchorDpDt);
                return;
            }

            // do the interpolation
            float dleft = (mEndMotionPath.x - mStartMotionPath.x);
            float dTop = (mEndMotionPath.y - mStartMotionPath.y);
            float dWidth = (mEndMotionPath.width - mStartMotionPath.width);
            float dHeight = (mEndMotionPath.height - mStartMotionPath.height);
            float dRight = dleft + dWidth;
            float dBottom = dTop + dHeight;
            mAnchorDpDt[0] = dleft * (1 - locationX) + dRight * (locationX);
            mAnchorDpDt[1] = dTop * (1 - locationY) + dBottom * (locationY);

            vmat.clear();
            vmat.setRotationVelocity(rotation, position);
            vmat.setTranslationVelocity(trans_x, trans_y, position);
            vmat.setScaleVelocity(scale_x, scale_y, position);
            vmat.setRotationVelocity(osc_r, position);
            vmat.setTranslationVelocity(osc_x, osc_y, position);
            vmat.setScaleVelocity(osc_sx, osc_sy, position);
            vmat.applyTransform(locationX, locationY, width, height, mAnchorDpDt);
            return;
        }

        public virtual int DrawPath
        {
            get
            {
                int mode = mStartMotionPath.mDrawPath;
                foreach (MotionPaths keyFrame in mMotionPaths)
                {
                    mode = Math.Max(mode, keyFrame.mDrawPath);
                }
                mode = Math.Max(mode, mEndMotionPath.mDrawPath);
                return mode;
            }
            set
            {
                mStartMotionPath.mDrawPath = value;
            }
        }


        internal virtual string name()
        {

            return mView.Name;
        }

        internal virtual void positionKeyframe(MotionWidget view, MotionKeyPosition key, float x, float y, string[] attribute, float[] value)
        {
            FloatRect start = new FloatRect();
            start.left = mStartMotionPath.x;
            start.top = mStartMotionPath.y;
            start.right = start.left + mStartMotionPath.width;
            start.bottom = start.top + mStartMotionPath.height;
            FloatRect end = new FloatRect();
            end.left = mEndMotionPath.x;
            end.top = mEndMotionPath.y;
            end.right = end.left + mEndMotionPath.width;
            end.bottom = end.top + mEndMotionPath.height;
            key.positionAttributes(view, start, end, x, y, attribute, value);
        }

        /// <summary>
        /// Get the keyFrames for the view controlled by this MotionController
        /// </summary>
        /// <param name="type"> is position(0-100) + 1000*mType(1=Attributes, 2=Position, 3=TimeCycle 4=Cycle 5=Trigger </param>
        /// <param name="pos">  the x&y position of the keyFrame along the path </param>
        /// <returns> Number of keyFrames found </returns>
        public virtual int getKeyFramePositions(int[] type, float[] pos)
        {
            int i = 0;
            int count = 0;
            foreach (MotionKey key in mKeyList)
            {
                type[i++] = key.mFramePosition + 1000 * key.mType;
                float time = key.mFramePosition / 100.0f;
                mSpline[0].getPos(time, mInterpolateData);
                mStartMotionPath.getCenter(time, mInterpolateVariables, mInterpolateData, pos, count);
                count += 2;
            }

            return i;
        }

        /// <summary>
        /// Get the keyFrames for the view controlled by this MotionController
        /// the info data structure is of the the form
        /// 0 length if your are at index i the [i+len+1] is the next entry
        /// 1 type  1=Attributes, 2=Position, 3=TimeCycle 4=Cycle 5=Trigger
        /// 2 position
        /// 3 x location
        /// 4 y location
        /// 5
        /// ...
        /// length
        /// </summary>
        /// <param name="info"> is a data structure array of int that holds info on each keyframe </param>
        /// <returns> Number of keyFrames found </returns>
        public virtual int getKeyFrameInfo(int type, int[] info)
        {
            int count = 0;
            int cursor = 0;
            float[] pos = new float[2];
            int len;
            foreach (MotionKey key in mKeyList)
            {
                if (key.mType != type && type == -1)
                {
                    continue;
                }
                len = cursor;
                info[cursor] = 0;

                info[++cursor] = key.mType;
                info[++cursor] = key.mFramePosition;

                float time = key.mFramePosition / 100.0f;
                mSpline[0].getPos(time, mInterpolateData);
                mStartMotionPath.getCenter(time, mInterpolateVariables, mInterpolateData, pos, 0);
                //info[++cursor] = float.floatToIntBits(pos[0]);
                info[++cursor] = pos[0].floatToIntBits();
                //info[++cursor] = float.floatToIntBits(pos[1]);
                info[++cursor] = pos[1].floatToIntBits();
                if (key is MotionKeyPosition)
                {
                    MotionKeyPosition kp = (MotionKeyPosition) key;
                    info[++cursor] = kp.mPositionType;

                    //info[++cursor] = Float.floatToIntBits(kp.mPercentX);
                    info[++cursor] = kp.mPercentX.floatToIntBits();
                    //info[++cursor] = Float.floatToIntBits(kp.mPercentY);
                    info[++cursor] = kp.mPercentY.floatToIntBits();
                }
                cursor++;
                info[len] = cursor - len;
                count++;
            }

            return count;
        }
    }

}