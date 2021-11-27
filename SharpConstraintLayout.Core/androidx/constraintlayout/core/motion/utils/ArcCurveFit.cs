using System;

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

    /// <summary>
    /// This provides provides a curve fit system that stitches the x,y path together with
    /// quarter ellipses
    /// </summary>

    public class ArcCurveFit : CurveFit
    {
        public const int ARC_START_VERTICAL = 1;
        public const int ARC_START_HORIZONTAL = 2;
        public const int ARC_START_FLIP = 3;
        public const int ARC_START_LINEAR = 0;

        private const int START_VERTICAL = 1;
        private const int START_HORIZONTAL = 2;
        private const int START_LINEAR = 3;
        private readonly double[] mTime;
        private Arc[] mArcs;
        private bool mExtrapolate = true;

        public override void getPos(double t, double[] v)
        {
            if (mExtrapolate)
            {
                if (t < mArcs[0].mTime1)
                {
                    double t0 = mArcs[0].mTime1;
                    double dt = t - mArcs[0].mTime1;
                    int p = 0;
                    if (mArcs[p].linear)
                    {
                        v[0] = (mArcs[p].getLinearX(t0) + dt * mArcs[p].getLinearDX(t0));
                        v[1] = (mArcs[p].getLinearY(t0) + dt * mArcs[p].getLinearDY(t0));
                    }
                    else
                    {
                        mArcs[p].Point = t0;
                        v[0] = mArcs[p].X + dt * mArcs[p].DX;
                        v[1] = mArcs[p].Y + dt * mArcs[p].DY;
                    }
                    return;
                }
                if (t > mArcs[mArcs.Length - 1].mTime2)
                {
                    double t0 = mArcs[mArcs.Length - 1].mTime2;
                    double dt = t - t0;
                    int p = mArcs.Length - 1;
                    if (mArcs[p].linear)
                    {
                        v[0] = (mArcs[p].getLinearX(t0) + dt * mArcs[p].getLinearDX(t0));
                        v[1] = (mArcs[p].getLinearY(t0) + dt * mArcs[p].getLinearDY(t0));
                    }
                    else
                    {
                        mArcs[p].Point = t;
                        v[0] = mArcs[p].X + dt * mArcs[p].DX;
                        v[1] = mArcs[p].Y + dt * mArcs[p].DY;
                    }
                    return;
                }
            }
            else
            {
                if (t < mArcs[0].mTime1)
                {
                    t = mArcs[0].mTime1;
                }
                if (t > mArcs[mArcs.Length - 1].mTime2)
                {
                    t = mArcs[mArcs.Length - 1].mTime2;
                }
            }

            for (int i = 0; i < mArcs.Length; i++)
            {
                if (t <= mArcs[i].mTime2)
                {
                    if (mArcs[i].linear)
                    {
                        v[0] = mArcs[i].getLinearX(t);
                        v[1] = mArcs[i].getLinearY(t);
                        return;
                    }
                    mArcs[i].Point = t;
                    v[0] = mArcs[i].X;
                    v[1] = mArcs[i].Y;
                    return;
                }
            }
        }

        public override void getPos(double t, float[] v)
        {
            if (mExtrapolate)
            {
                if (t < mArcs[0].mTime1)
                {
                    double t0 = mArcs[0].mTime1;
                    double dt = t - mArcs[0].mTime1;
                    int p = 0;
                    if (mArcs[p].linear)
                    {
                        v[0] = (float)(mArcs[p].getLinearX(t0) + dt * mArcs[p].getLinearDX(t0));
                        v[1] = (float)(mArcs[p].getLinearY(t0) + dt * mArcs[p].getLinearDY(t0));
                    }
                    else
                    {
                        mArcs[p].Point = t0;
                        v[0] = (float)(mArcs[p].X + dt * mArcs[p].DX);
                        v[1] = (float)(mArcs[p].Y + dt * mArcs[p].DY);
                    }
                    return;
                }
                if (t > mArcs[mArcs.Length - 1].mTime2)
                {
                    double t0 = mArcs[mArcs.Length - 1].mTime2;
                    double dt = t - t0;
                    int p = mArcs.Length - 1;
                    if (mArcs[p].linear)
                    {
                        v[0] = (float)(mArcs[p].getLinearX(t0) + dt * mArcs[p].getLinearDX(t0));
                        v[1] = (float)(mArcs[p].getLinearY(t0) + dt * mArcs[p].getLinearDY(t0));
                    }
                    else
                    {
                        mArcs[p].Point = t;
                        v[0] = (float) mArcs[p].X;
                        v[1] = (float) mArcs[p].Y;
                    }
                    return;
                }
            }
            else
            {
                if (t < mArcs[0].mTime1)
                {
                    t = mArcs[0].mTime1;
                }
                else if (t > mArcs[mArcs.Length - 1].mTime2)
                {
                    t = mArcs[mArcs.Length - 1].mTime2;
                }
            }
            for (int i = 0; i < mArcs.Length; i++)
            {
                if (t <= mArcs[i].mTime2)
                {
                    if (mArcs[i].linear)
                    {
                        v[0] = (float) mArcs[i].getLinearX(t);
                        v[1] = (float) mArcs[i].getLinearY(t);
                        return;
                    }
                    mArcs[i].Point = t;
                    v[0] = (float) mArcs[i].X;
                    v[1] = (float) mArcs[i].Y;
                    return;
                }
            }
        }

        public override void getSlope(double t, double[] v)
        {
            if (t < mArcs[0].mTime1)
            {
                t = mArcs[0].mTime1;
            }
            else if (t > mArcs[mArcs.Length - 1].mTime2)
            {
                t = mArcs[mArcs.Length - 1].mTime2;
            }

            for (int i = 0; i < mArcs.Length; i++)
            {
                if (t <= mArcs[i].mTime2)
                {
                    if (mArcs[i].linear)
                    {
                        v[0] = mArcs[i].getLinearDX(t);
                        v[1] = mArcs[i].getLinearDY(t);
                        return;
                    }
                    mArcs[i].Point = t;
                    v[0] = mArcs[i].DX;
                    v[1] = mArcs[i].DY;
                    return;
                }
            }
        }

        public override double getPos(double t, int j)
        {
            if (mExtrapolate)
            {
                if (t < mArcs[0].mTime1)
                {
                    double t0 = mArcs[0].mTime1;
                    double dt = t - mArcs[0].mTime1;
                    int p = 0;
                    if (mArcs[p].linear)
                    {
                        if (j == 0)
                        {
                            return mArcs[p].getLinearX(t0) + dt * mArcs[p].getLinearDX(t0);
                        }
                        return mArcs[p].getLinearY(t0) + dt * mArcs[p].getLinearDY(t0);
                    }
                    else
                    {
                        mArcs[p].Point = t0;
                        if (j == 0)
                        {
                            return mArcs[p].X + dt * mArcs[p].DX;
                        }
                        return mArcs[p].Y + dt * mArcs[p].DY;
                    }
                }
                if (t > mArcs[mArcs.Length - 1].mTime2)
                {
                    double t0 = mArcs[mArcs.Length - 1].mTime2;
                    double dt = t - t0;
                    int p = mArcs.Length - 1;
                    if (j == 0)
                    {
                        return mArcs[p].getLinearX(t0) + dt * mArcs[p].getLinearDX(t0);
                    }
                    return mArcs[p].getLinearY(t0) + dt * mArcs[p].getLinearDY(t0);
                }
            }
            else
            {
                if (t < mArcs[0].mTime1)
                {
                    t = mArcs[0].mTime1;
                }
                else if (t > mArcs[mArcs.Length - 1].mTime2)
                {
                    t = mArcs[mArcs.Length - 1].mTime2;
                }
            }

            for (int i = 0; i < mArcs.Length; i++)
            {
                if (t <= mArcs[i].mTime2)
                {

                    if (mArcs[i].linear)
                    {
                        if (j == 0)
                        {
                            return mArcs[i].getLinearX(t);
                        }
                        return mArcs[i].getLinearY(t);
                    }
                    mArcs[i].Point = t;

                    if (j == 0)
                    {
                        return mArcs[i].X;
                    }
                    return mArcs[i].Y;
                }
            }
            return Double.NaN;
        }

        public override double getSlope(double t, int j)
        {
            if (t < mArcs[0].mTime1)
            {
                t = mArcs[0].mTime1;
            }
            if (t > mArcs[mArcs.Length - 1].mTime2)
            {
                t = mArcs[mArcs.Length - 1].mTime2;
            }

            for (int i = 0; i < mArcs.Length; i++)
            {
                if (t <= mArcs[i].mTime2)
                {
                    if (mArcs[i].linear)
                    {
                        if (j == 0)
                        {
                            return mArcs[i].getLinearDX(t);
                        }
                        return mArcs[i].getLinearDY(t);
                    }
                    mArcs[i].Point = t;
                    if (j == 0)
                    {
                        return mArcs[i].DX;
                    }
                    return mArcs[i].DY;
                }
            }
            return Double.NaN;
        }

        public override double[] TimePoints
        {
            get
            {
                return mTime;
            }
        }

        public ArcCurveFit(int[] arcModes, double[] time, double[][] y)
        {
            mTime = time;
            mArcs = new Arc[time.Length - 1];
            int mode = START_VERTICAL;
            int last = START_VERTICAL;
            for (int i = 0; i < mArcs.Length; i++)
            {
                switch (arcModes[i])
                {
                    case ARC_START_VERTICAL:
                        last = mode = START_VERTICAL;
                        break;
                    case ARC_START_HORIZONTAL:
                        last = mode = START_HORIZONTAL;
                        break;
                    case ARC_START_FLIP:
                        mode = (last == START_VERTICAL) ? START_HORIZONTAL : START_VERTICAL;
                        last = mode;
                        break;
                    case ARC_START_LINEAR:
                        mode = START_LINEAR;
                    break;
                }
                mArcs[i] = new Arc(mode, time[i], time[i + 1], y[i][0], y[i][1], y[i + 1][0], y[i + 1][1]);
            }
        }

        private class Arc
        {
            internal const string TAG = "Arc";
            internal static double[] ourPercent = new double[91];
            internal double[] mLut;
            internal double mArcDistance;
            internal double mTime1;
            internal double mTime2;
            internal double mX1, mX2, mY1, mY2;
            internal double mOneOverDeltaTime;
            internal double mEllipseA;
            internal double mEllipseB;
            internal double mEllipseCenterX; // also used to cache the slope in the unused center
            internal double mEllipseCenterY; // also used to cache the slope in the unused center
            internal double mArcVelocity;
            internal double mTmpSinAngle;
            internal double mTmpCosAngle;
            internal bool mVertical;
            internal bool linear = false;
            internal const double EPSILON = 0.001;

            internal Arc(int mode, double t1, double t2, double x1, double y1, double x2, double y2)
            {
                mVertical = mode == START_VERTICAL;
                mTime1 = t1;
                mTime2 = t2;
                mOneOverDeltaTime = 1 / (mTime2 - mTime1);
                if (START_LINEAR == mode)
                {
                    linear = true;
                }
                double dx = x2 - x1;
                double dy = y2 - y1;
                if (linear || Math.Abs(dx) < EPSILON || Math.Abs(dy) < EPSILON)
                {
                    linear = true;
                    mX1 = x1;
                    mX2 = x2;
                    mY1 = y1;
                    mY2 = y2;
                    mArcDistance = MathExtension.hypot(dy, dx);
                    mArcVelocity = mArcDistance * mOneOverDeltaTime;
                    mEllipseCenterX = dx / (mTime2 - mTime1); // cache the slope in the unused center
                    mEllipseCenterY = dy / (mTime2 - mTime1); // cache the slope in the unused center
                    return;
                }
                mLut = new double[101];
                mEllipseA = (dx) * ((mVertical) ? -1 : 1);
                mEllipseB = (dy) * ((mVertical) ? 1 : -1);
                mEllipseCenterX = (mVertical) ? x2 : x1;
                mEllipseCenterY = (mVertical) ? y1 : y2;
                buildTable(x1, y1, x2, y2);
                mArcVelocity = mArcDistance * mOneOverDeltaTime;
            }

            internal virtual double Point
            {
                set
                {
                    double percent = (mVertical ? (mTime2 - value) : (value - mTime1)) * mOneOverDeltaTime;
                    double angle = Math.PI * 0.5 * lookup(percent);
    
                    mTmpSinAngle = Math.Sin(angle);
                    mTmpCosAngle = Math.Cos(angle);
                }
            }

            internal virtual double X
            {
                get
                {
                    return mEllipseCenterX + mEllipseA * mTmpSinAngle;
                }
            }

            internal virtual double Y
            {
                get
                {
                    return mEllipseCenterY + mEllipseB * mTmpCosAngle;
                }
            }

            internal virtual double DX
            {
                get
                {
                    double vx = mEllipseA * mTmpCosAngle;
                    double vy = -mEllipseB * mTmpSinAngle;
                    double norm = mArcVelocity / MathExtension.hypot(vx, vy);
                    return mVertical ? -vx * norm : vx * norm;
                }
            }

            internal virtual double DY
            {
                get
                {
                    double vx = mEllipseA * mTmpCosAngle;
                    double vy = -mEllipseB * mTmpSinAngle;
                    double norm = mArcVelocity / MathExtension.hypot(vx, vy);
                    return mVertical ? -vy * norm : vy * norm;
                }
            }

            public virtual double getLinearX(double t)
            {
                t = (t - mTime1) * mOneOverDeltaTime;
                return mX1 + t * (mX2 - mX1);
            }

            public virtual double getLinearY(double t)
            {
                t = (t - mTime1) * mOneOverDeltaTime;
                return mY1 + t * (mY2 - mY1);
            }

            public virtual double getLinearDX(double t)
            {
                return mEllipseCenterX;
            }

            public virtual double getLinearDY(double t)
            {
                return mEllipseCenterY;
            }

            internal virtual double lookup(double v)
            {
                if (v <= 0)
                {
                    return 0;
                }
                if (v >= 1)
                {
                    return 1;
                }
                double pos = v * (mLut.Length - 1);
                int iv = (int)(pos);
                double off = pos - (int)(pos);

                return mLut[iv] + (off * (mLut[iv + 1] - mLut[iv]));
            }

            internal virtual void buildTable(double x1, double y1, double x2, double y2)
            {
                double a = x2 - x1;
                double b = y1 - y2;
                double lx = 0, ly = 0;
                double dist = 0;
                for (int i = 0; i < ourPercent.Length; i++)
                {
                    double angle = MathExtension.toRadians(90.0 * i / (ourPercent.Length - 1));
                    double s = Math.Sin(angle);
                    double c = Math.Cos(angle);
                    double px = a * s;
                    double py = b * c;
                    if (i > 0)
                    {
                        dist += MathExtension.hypot(px - lx, py - ly);
                        ourPercent[i] = dist;
                    }
                    lx = px;
                    ly = py;
                }

                mArcDistance = dist;

                for (int i = 0; i < ourPercent.Length; i++)
                {
                    ourPercent[i] /= dist;
                }
                for (int i = 0; i < mLut.Length; i++)
                {
                    double pos = i / (double)(mLut.Length - 1);
                    //int index = Arrays.binarySearch(ourPercent, pos);
                    int index = Array.BinarySearch(ourPercent, pos);
                    if (index >= 0)
                    {
                        mLut[i] = index / (double)(ourPercent.Length - 1);
                    }
                    else if (index == -1)
                    {
                        mLut[i] = 0;
                    }
                    else
                    {
                        int p1 = -index - 2;
                        int p2 = -index - 1;

                        double ans = (p1 + (pos - ourPercent[p1]) / (ourPercent[p2] - ourPercent[p1])) / (ourPercent.Length - 1);
                        mLut[i] = ans;
                    }
                }
            }
        }
    }

}