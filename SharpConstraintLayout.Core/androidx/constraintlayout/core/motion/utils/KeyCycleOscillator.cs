using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2020 The Android Open Source Project
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
namespace androidx.constraintlayout.core.motion.utils
{
    using WidgetFrame = androidx.constraintlayout.core.state.WidgetFrame;


    /// <summary>
    /// Provide the engine for executing cycles.
    /// KeyCycleOscillator
    /// 
    /// @suppress
    /// </summary>
    public abstract class KeyCycleOscillator
    {
        private const string TAG = "KeyCycleOscillator";
        private CurveFit mCurveFit;
        private CycleOscillator mCycleOscillator;
        private string mType;
        private int mWaveShape = 0;
        private string mWaveString = null;

        public int mVariesBy = 0; // 0 = position, 2=path
        internal List<WavePoint> mWavePoints = new List<WavePoint>();

        public static KeyCycleOscillator makeWidgetCycle(string attribute)
        {
            if (attribute.Equals(TypedValues.TypedValues_Attributes.S_PATH_ROTATE))
            {
                return new PathRotateSet(attribute);
            }
            return new CoreSpline(attribute);
        }

        private class CoreSpline : KeyCycleOscillator
        {
            internal string type;
            internal int typeId;

            public CoreSpline(string str)
            {
                type = str;
                typeId = TypedValues.TypedValues_Cycle.getId(type);
            }

            public override void setProperty(MotionWidget widget, float t)
            {
                widget.setValue(typeId, get(t));
            }
        }

        public class PathRotateSet : KeyCycleOscillator
        {
            internal string type;
            internal int typeId;

            public PathRotateSet(string str)
            {
                type = str;
                typeId = TypedValues.TypedValues_Cycle.getId(type);
            }

            public override void setProperty(MotionWidget widget, float t)
            {
                widget.setValue(typeId, get(t));
            }

            public virtual void setPathRotate(MotionWidget view, float t, double dx, double dy)
            {
                view.RotationZ = get(t) + (float) MathExtension.toDegrees(Math.Atan2(dy, dx));
            }
        }

        public virtual bool variesByPath()
        {
            return mVariesBy == 1;
        }

        internal class WavePoint
        {
            internal int mPosition;
            internal float mValue;
            internal float mOffset;
            internal float mPeriod;
            internal float mPhase;

            public WavePoint(int position, float period, float offset, float phase, float value)
            {
                mPosition = position;
                mValue = value;
                mOffset = offset;
                mPeriod = period;
                mPhase = phase;
            }
        }

        public override string ToString()
        {
            string str = mType;
            DecimalFormat df = new DecimalFormat("##.##");//TODO:验证DecimalFormat具体实现
            foreach (WavePoint wp in mWavePoints)
            {
                str += "[" + wp.mPosition + " , " + df.format(wp.mValue) + "] ";
            }
            return str;
        }

        public virtual string Type
        {
            set
            {
                mType = value;
            }
        }

        public virtual float get(float t)
        {
            return (float) mCycleOscillator.getValues(t);
        }

        public virtual float getSlope(float position)
        {
            return (float) mCycleOscillator.getSlope(position);
        }

        public virtual CurveFit CurveFit
        {
            get
            {
                return mCurveFit;
            }
        }

        protected internal virtual object Custom
        {
            set
            {
    
            }
        }

        /// <summary>
        /// sets a oscillator wave point
        /// </summary>
        /// <param name="framePosition"> the position </param>
        /// <param name="variesBy">      only varies by path supported for now </param>
        /// <param name="period">        the period of the wave </param>
        /// <param name="offset">        the offset value </param>
        /// <param name="value">         the adder </param>
        /// <param name="custom">        The ConstraintAttribute used to set the value </param>
        public virtual void setPoint(int framePosition, int shape, string waveString, int variesBy, float period, float offset, float phase, float value, object custom)
        {
            mWavePoints.Add(new WavePoint(framePosition, period, offset, phase, value));
            if (variesBy != -1)
            {
                mVariesBy = variesBy;
            }
            mWaveShape = shape;
            Custom = custom;
            mWaveString = waveString;
        }

        /// <summary>
        /// sets a oscillator wave point
        /// </summary>
        /// <param name="framePosition"> the position </param>
        /// <param name="variesBy">      only varies by path supported for now </param>
        /// <param name="period">        the period of the wave </param>
        /// <param name="offset">        the offset value </param>
        /// <param name="value">         the adder </param>
        public virtual void setPoint(int framePosition, int shape, string waveString, int variesBy, float period, float offset, float phase, float value)
        {
            mWavePoints.Add(new WavePoint(framePosition, period, offset, phase, value));
            if (variesBy != -1)
            {
                mVariesBy = variesBy;
            }
            mWaveShape = shape;
            mWaveString = waveString;
        }

        public virtual void setup(float pathLength)
        {
            int count = mWavePoints.Count;
            if (count == 0)
            {
                return;
            }
            mWavePoints.Sort(new ComparatorAnonymousInnerClass(this));
            double[] time = new double[count];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] values = new double[count][3];
            double[][] values = RectangularArrays.ReturnRectangularDoubleArray(count, 3);
            mCycleOscillator = new CycleOscillator(mWaveShape, mWaveString, mVariesBy, count);
            int i = 0;
            foreach (WavePoint wp in mWavePoints)
            {
                time[i] = wp.mPeriod * 1E-2;
                values[i][0] = wp.mValue;
                values[i][1] = wp.mOffset;
                values[i][2] = wp.mPhase;
                mCycleOscillator.setPoint(i, wp.mPosition, wp.mPeriod, wp.mOffset, wp.mPhase, wp.mValue);
                i++;
            }
            mCycleOscillator.setup(pathLength);
            mCurveFit = CurveFit.get(CurveFit.SPLINE, time, values);
        }

        private class ComparatorAnonymousInnerClass : IComparer<WavePoint>
        {
            private readonly KeyCycleOscillator outerInstance;

            public ComparatorAnonymousInnerClass(KeyCycleOscillator outerInstance)
            {
                this.outerInstance = outerInstance;
            }


            public int Compare(WavePoint lhs, WavePoint rhs)
            {
                //https://docs.microsoft.com/en-us/dotnet/api/system.collections.icomparer.compare?view=net-6.0#System_Collections_IComparer_Compare_System_Object_System_Object_
                return lhs.mPosition - rhs.mPosition;
            }
        }

        private class IntDoubleSort
        {
            internal static void sort(int[] key, float[] value, int low, int hi)
            {
                int[] stack = new int[key.Length + 10];
                int count = 0;
                stack[count++] = hi;
                stack[count++] = low;
                while (count > 0)
                {
                    low = stack[--count];
                    hi = stack[--count];
                    if (low < hi)
                    {
                        int p = partition(key, value, low, hi);
                        stack[count++] = p - 1;
                        stack[count++] = low;
                        stack[count++] = hi;
                        stack[count++] = p + 1;
                    }
                }
            }

            internal static int partition(int[] array, float[] value, int low, int hi)
            {
                int pivot = array[hi];
                int i = low;
                for (int j = low; j < hi; j++)
                {
                    if (array[j] <= pivot)
                    {
                        swap(array, value, i, j);
                        i++;
                    }
                }
                swap(array, value, i, hi);
                return i;
            }

            internal static void swap(int[] array, float[] value, int a, int b)
            {
                int tmp = array[a];
                array[a] = array[b];
                array[b] = tmp;
                float tmpv = value[a];
                value[a] = value[b];
                value[b] = tmpv;
            }
        }

        private class IntFloatFloatSort
        {
            internal static void sort(int[] key, float[] value1, float[] value2, int low, int hi)
            {
                int[] stack = new int[key.Length + 10];
                int count = 0;
                stack[count++] = hi;
                stack[count++] = low;
                while (count > 0)
                {
                    low = stack[--count];
                    hi = stack[--count];
                    if (low < hi)
                    {
                        int p = partition(key, value1, value2, low, hi);
                        stack[count++] = p - 1;
                        stack[count++] = low;
                        stack[count++] = hi;
                        stack[count++] = p + 1;
                    }
                }
            }

            internal static int partition(int[] array, float[] value1, float[] value2, int low, int hi)
            {
                int pivot = array[hi];
                int i = low;
                for (int j = low; j < hi; j++)
                {
                    if (array[j] <= pivot)
                    {
                        swap(array, value1, value2, i, j);
                        i++;
                    }
                }
                swap(array, value1, value2, i, hi);
                return i;
            }

            internal static void swap(int[] array, float[] value1, float[] value2, int a, int b)
            {
                int tmp = array[a];
                array[a] = array[b];
                array[b] = tmp;
                float tmpFloat = value1[a];
                value1[a] = value1[b];
                value1[b] = tmpFloat;
                tmpFloat = value2[a];
                value2[a] = value2[b];
                value2[b] = tmpFloat;
            }
        }

        internal class CycleOscillator
        {
            internal const int UNSET = -1; // -1 is typically used through out android to the UNSET value
            internal const string TAG = "CycleOscillator";
            internal readonly int mVariesBy;
            internal Oscillator mOscillator = new Oscillator();
            internal readonly int OFFST = 0;
            internal readonly int PHASE = 1;
            internal readonly int VALUE = 2;

            internal float[] mValues;
            internal double[] mPosition;
            internal float[] mPeriod;
            internal float[] mOffset; // offsets will be spline interpolated
            internal float[] mPhase; // phase will be spline interpolated
            internal float[] mScale; // scales will be spline interpolated
            internal int mWaveShape;
            internal CurveFit mCurveFit;
            internal double[] mSplineValueCache; // for the return value of the curve fit
            internal double[] mSplineSlopeCache; // for the return value of the curve fit
            internal float mPathLength;

            internal CycleOscillator(int waveShape, string customShape, int variesBy, int steps)
            {
                mWaveShape = waveShape;
                mVariesBy = variesBy;
                mOscillator.setType(waveShape, customShape);
                mValues = new float[steps];
                mPosition = new double[steps];
                mPeriod = new float[steps];
                mOffset = new float[steps];
                mPhase = new float[steps];
                mScale = new float[steps];
            }

            public virtual double getValues(float time)
            {
                if (mCurveFit != null)
                {
                    mCurveFit.getPos(time, mSplineValueCache);
                }
                else
                { // only one value no need to interpolate
                    mSplineValueCache[OFFST] = mOffset[0];
                    mSplineValueCache[PHASE] = mPhase[0];
                    mSplineValueCache[VALUE] = mValues[0];

                }
                double offset = mSplineValueCache[OFFST];
                double phase = mSplineValueCache[PHASE];
                double waveValue = mOscillator.getValue(time, phase);
                return offset + waveValue * mSplineValueCache[VALUE];
            }

            public virtual double LastPhase
            {
                get
                {
                    return mSplineValueCache[1];
                }
            }

            public virtual double getSlope(float time)
            {
                if (mCurveFit != null)
                {
                    mCurveFit.getSlope(time, mSplineSlopeCache);
                    mCurveFit.getPos(time, mSplineValueCache);
                }
                else
                { // only one value no need to interpolate
                    mSplineSlopeCache[OFFST] = 0;
                    mSplineSlopeCache[PHASE] = 0;
                    mSplineSlopeCache[VALUE] = 0;
                }
                double waveValue = mOscillator.getValue(time, mSplineValueCache[PHASE]);
                double waveSlope = mOscillator.getSlope(time, mSplineValueCache[PHASE], mSplineSlopeCache[PHASE]);
                return mSplineSlopeCache[OFFST] + waveValue * mSplineSlopeCache[VALUE] + waveSlope * mSplineValueCache[VALUE];
            }

            /// <param name="index"> </param>
            /// <param name="framePosition"> </param>
            /// <param name="wavePeriod"> </param>
            /// <param name="offset"> </param>
            /// <param name="values"> </param>
            public virtual void setPoint(int index, int framePosition, float wavePeriod, float offset, float phase, float values)
            {
                mPosition[index] = framePosition / 100.0;
                mPeriod[index] = wavePeriod;
                mOffset[index] = offset;
                mPhase[index] = phase;
                mValues[index] = values;
            }

            public virtual void setup(float pathLength)
            {
                mPathLength = pathLength;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] splineValues = new double[mPosition.Length][3];
                double[][] splineValues = RectangularArrays.ReturnRectangularDoubleArray(mPosition.Length, 3);
                mSplineValueCache = new double[2 + mValues.Length];
                mSplineSlopeCache = new double[2 + mValues.Length];
                if (mPosition[0] > 0)
                {
                    mOscillator.addPoint(0, mPeriod[0]);
                }
                int last = mPosition.Length - 1;
                if (mPosition[last] < 1.0f)
                {
                    mOscillator.addPoint(1, mPeriod[last]);
                }

                for (int i = 0; i < splineValues.Length; i++)
                {
                    splineValues[i][OFFST] = mOffset[i];
                    splineValues[i][PHASE] = mPhase[i];
                    splineValues[i][VALUE] = mValues[i];
                    mOscillator.addPoint(mPosition[i], mPeriod[i]);
                }

                // TODO: add mVariesBy and get total time and path length
                mOscillator.normalize();
                if (mPosition.Length > 1)
                {
                    mCurveFit = CurveFit.get(CurveFit.SPLINE, mPosition, splineValues);
                }
                else
                {
                    mCurveFit = null;
                }
            }
        }

        public virtual void setProperty(MotionWidget widget, float t)
        {

        }

    }

}