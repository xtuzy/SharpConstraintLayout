﻿using System;

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

namespace androidx.constraintlayout.core.motion.utils
{
	/// <summary>
	/// This contains the class to provide the logic for an animation to come to a stop using a spring
	/// model.
	/// 
	/// @suppress
	/// </summary>
	public class SpringStopEngine : StopEngine
	{
		internal double mDamping = 0.5f;
		private static readonly double UNSET = double.MaxValue;
		private bool mInitialized = false;
		private double mStiffness;
		private double mTargetPos;
		private double mLastVelocity;
		private float mLastTime;
		private float mPos;
		private float mV;
		private float mMass;
		private float mStopThreshold;
		private int mBoundaryMode = 0;

		public virtual string debug(string desc, float time)
		{
			return null;
		}

		internal virtual void log(string str)
		{
			//StackTraceElement s = (new Exception()).StackTrace[1];
			System.Diagnostics.StackFrame s = new System.Diagnostics.StackTrace(new Exception(), true).GetFrame(0);//需要pdb文件,https://stackoverflow.com/questions/3328990/how-can-i-get-the-line-number-which-threw-exception
			string line = ".(" + s.GetFileName() + ":" + s.GetFileLineNumber() + ") " + s.GetMethod().Name + "() ";
			Console.WriteLine(line + str);
		}

		public virtual void springConfig(float currentPos, float target, float currentVelocity, float mass, float stiffness, float damping, float stopThreshold, int boundaryMode)
		{
			mTargetPos = target;
			mDamping = damping;
			mInitialized = false;
			mPos = currentPos;
			mLastVelocity = currentVelocity;
			mStiffness = stiffness;
			mMass = mass;
			mStopThreshold = stopThreshold;
			mBoundaryMode = boundaryMode;
			mLastTime = 0;
		}

		public virtual float getVelocity(float t)
		{
			return (float) mV;
		}

		public virtual float getInterpolation(float time)
		{
			compute(time - mLastTime);
			mLastTime = time;
			return (float)(mPos);
		}

		public virtual float Acceleration
		{
			get
			{
				double k = mStiffness;
				double c = mDamping;
				double x = (mPos - mTargetPos);
				return (float)(-k * x - c * mV) / mMass;
			}
		}

		public virtual float Velocity
		{
			get
			{
				return 0;
			}
		}

		public virtual bool Stopped
		{
			get
			{
				double x = (mPos - mTargetPos);
				double k = mStiffness;
				double v = mV;
				double m = mMass;
				double energy = v * v * m + k * x * x;
				double max_def = Math.Sqrt(energy / k);
				return max_def <= mStopThreshold;
			}
		}

		private void compute(double dt)
		{

			double k = mStiffness;
			double c = mDamping;
			// Estimate how many time we should over sample based on the frequency and current sampling
			int overSample = (int)(1 + 9 / (Math.Sqrt(mStiffness / mMass) * dt * 4));
			dt /= overSample;

			for (int i = 0; i < overSample; i++)
			{
				double x = (mPos - mTargetPos);
				double a = (-k * x - c * mV) / mMass;
				// This refinement of a simple coding of the acceleration increases accuracy
				double avgV = mV + a * dt / 2; // pass 1 calculate the average velocity
				double avgX = mPos + dt * (avgV) / 2 - mTargetPos; // pass 1 calculate the average pos
				a = (-avgX * k - avgV * c) / mMass; //  calculate acceleration over that average pos

				double dv = a * dt; //  calculate change in velocity
				avgV = mV + dv / 2; //  average  velocity is current + half change
				mV += (float)dv;
				mPos += (float)(avgV * dt);
				if (mBoundaryMode > 0)
				{
					if (mPos < 0 && ((mBoundaryMode & 1) == 1))
					{
						mPos = -mPos;
						mV = -mV;
					}
					if (mPos > 1 && ((mBoundaryMode & 2) == 2))
					{
						mPos = 2 - mPos;
						mV = -mV;
					}
				}
			}
		}

	}

}