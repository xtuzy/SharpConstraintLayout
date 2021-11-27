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
namespace androidx.constraintlayout.core.widgets
{
	using Grouping = androidx.constraintlayout.core.widgets.analyzer.Grouping;
	using WidgetGroup = androidx.constraintlayout.core.widgets.analyzer.WidgetGroup;


	/// <summary>
	/// Model a constraint relation. Widgets contains anchors, and a constraint relation between
	/// two widgets is made by connecting one anchor to another. The anchor will contains a pointer
	/// to the target anchor if it is connected.
	/// </summary>
	public class ConstraintAnchor
	{

		private const bool ALLOW_BINARY = false;

		private HashSet<ConstraintAnchor> mDependents = null;
		private int mFinalValue;
		private bool mHasFinalValue;

		public virtual void findDependents(int orientation, List<WidgetGroup> list, WidgetGroup group)
		{
			if (mDependents != null)
			{
				foreach (ConstraintAnchor anchor in mDependents)
				{
					Grouping.findDependents(anchor.mOwner, orientation, list, group);
				}
			}
		}

		public virtual HashSet<ConstraintAnchor> Dependents
		{
			get
			{
				return mDependents;
			}
		}
		public virtual bool hasDependents()
		{
			if (mDependents == null)
			{
				return false;
			}
			return mDependents.Count > 0;
		}

		public virtual bool hasCenteredDependents()
		{
			if (mDependents == null)
			{
				return false;
			}
			foreach (ConstraintAnchor anchor in mDependents)
			{
				ConstraintAnchor opposite = anchor.Opposite;
				if (opposite.Connected)
				{
					return true;
				}
			}
			return false;
		}

		public virtual int FinalValue
		{
			set
			{
				this.mFinalValue = value;
				this.mHasFinalValue = true;
			}
			get
			{
				if (!mHasFinalValue)
				{
					return 0;
				}
				return mFinalValue;
			}
		}


		public virtual void resetFinalResolution()
		{
			mHasFinalValue = false;
			mFinalValue = 0;
		}

		public virtual bool hasFinalValue()
		{
			return mHasFinalValue;
		}

		/// <summary>
		/// Define the type of anchor
		/// </summary>
		public enum Type
		{
			NONE,
			LEFT,
			TOP,
			RIGHT,
			BOTTOM,
			BASELINE,
			CENTER,
			CENTER_X,
			CENTER_Y
		}

		private static readonly int UNSET_GONE_MARGIN = int.MinValue;

		public readonly ConstraintWidget mOwner;
		public readonly Type mType;
		public ConstraintAnchor mTarget;
		public int mMargin = 0;
		internal int mGoneMargin = UNSET_GONE_MARGIN;

		internal SolverVariable mSolverVariable;

		public virtual void copyFrom(ConstraintAnchor source, Dictionary<ConstraintWidget, ConstraintWidget> map)
		{
			if (mTarget != null)
			{
				if (mTarget.mDependents != null)
				{
					mTarget.mDependents.Remove(this);
				}
			}
			if (source.mTarget != null)
			{
				Type type = source.mTarget.getType();
				ConstraintWidget owner = map[source.mTarget.mOwner];
				mTarget = owner.getAnchor(type);
			}
			else
			{
				mTarget = null;
			}
			if (mTarget != null)
			{
				if (mTarget.mDependents == null)
				{
					mTarget.mDependents = new HashSet<ConstraintAnchor>();
				}
				mTarget.mDependents.Add(this);
			}
			mMargin = source.mMargin;
			mGoneMargin = source.mGoneMargin;
		}

		/// <summary>
		/// Constructor </summary>
		/// <param name="owner"> the widget owner of this anchor. </param>
		/// <param name="type"> the anchor type. </param>
		public ConstraintAnchor(ConstraintWidget owner, Type type)
		{
			mOwner = owner;
			mType = type;
		}

		/// <summary>
		/// Return the solver variable for this anchor
		/// @return
		/// </summary>
		public virtual SolverVariable SolverVariable
		{
			get
			{
				return mSolverVariable;
			}
		}

		/// <summary>
		/// Reset the solver variable
		/// </summary>
		public virtual void resetSolverVariable(Cache cache)
		{
			if (mSolverVariable == null)
			{
				mSolverVariable = new SolverVariable(SolverVariable.Type.UNRESTRICTED, null);
			}
			else
			{
				mSolverVariable.reset();
			}
		}

		/// <summary>
		/// Return the anchor's owner </summary>
		/// <returns> the Widget owning the anchor </returns>
		public virtual ConstraintWidget Owner
		{
			get
			{
				return mOwner;
			}
		}

		/// <summary>
		/// Return the type of the anchor </summary>
		/// <returns> type of the anchor. </returns>
		public virtual Type getType()
		{
			return mType;
		}

		/// <summary>
		/// Return the connection's margin from this anchor to its target. </summary>
		/// <returns> the margin value. 0 if not connected. </returns>
		public virtual int Margin
		{
			get
			{
				if (mOwner.Visibility == ConstraintWidget.GONE)
				{
					return 0;
				}
				if (mGoneMargin != UNSET_GONE_MARGIN && mTarget != null && mTarget.mOwner.Visibility == ConstraintWidget.GONE)
				{
					return mGoneMargin;
				}
				return mMargin;
			}
			set
			{
				if (Connected)
				{
					mMargin = value;
				}
			}
		}

		/// <summary>
		/// Return the connection's target (null if not connected) </summary>
		/// <returns> the ConstraintAnchor target </returns>
		public virtual ConstraintAnchor Target
		{
			get
			{
				return mTarget;
			}
		}

		/// <summary>
		/// Resets the anchor's connection.
		/// </summary>
		public virtual void reset()
		{
			if (mTarget != null && mTarget.mDependents != null)
			{
				mTarget.mDependents.Remove(this);
				if (mTarget.mDependents.Count == 0)
				{
					mTarget.mDependents = null;
				}
			}
			mDependents = null;
			mTarget = null;
			mMargin = 0;
			mGoneMargin = UNSET_GONE_MARGIN;
			mHasFinalValue = false;
			mFinalValue = 0;
		}

		/// <summary>
		/// Connects this anchor to another one.
		/// </summary>
		/// <param name="toAnchor"> </param>
		/// <param name="margin"> </param>
		/// <param name="goneMargin"> </param>
		/// <param name="forceConnection"> </param>
		/// <returns> true if the connection succeeds. </returns>
		public virtual bool connect(ConstraintAnchor toAnchor, int margin, int goneMargin, bool forceConnection)
		{
			if (toAnchor == null)
			{
				reset();
				return true;
			}
			if (!forceConnection && !isValidConnection(toAnchor))
			{
				return false;
			}
			mTarget = toAnchor;
			if (mTarget.mDependents == null)
			{
				mTarget.mDependents = new HashSet<ConstraintAnchor>();
			}
			if (mTarget.mDependents != null)
			{
				mTarget.mDependents.Add(this);
			}
			mMargin = margin;
			mGoneMargin = goneMargin;
			return true;
		}


		/// <summary>
		/// Connects this anchor to another one. </summary>
		/// <param name="toAnchor"> </param>
		/// <param name="margin"> </param>
		/// <returns> true if the connection succeeds. </returns>
		public virtual bool connect(ConstraintAnchor toAnchor, int margin)
		{
			return connect(toAnchor, margin, UNSET_GONE_MARGIN, false);
		}

		/// <summary>
		/// Returns the connection status of this anchor </summary>
		/// <returns> true if the anchor is connected to another one. </returns>
		public virtual bool Connected
		{
			get
			{
				return mTarget != null;
			}
		}

		/// <summary>
		/// Checks if the connection to a given anchor is valid. </summary>
		/// <param name="anchor"> the anchor we want to connect to </param>
		/// <returns> true if it's a compatible anchor </returns>
		public virtual bool isValidConnection(ConstraintAnchor anchor)
		{
			if (anchor == null)
			{
				return false;
			}
			Type target = anchor.getType();
			if (target == mType)
			{
				if (mType == Type.BASELINE && (!anchor.Owner.hasBaseline() || !Owner.hasBaseline()))
				{
					return false;
				}
				return true;
			}
			switch (mType)
			{
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER:
				{
					// allow everything but baseline and center_x/center_y
					return target != Type.BASELINE && target != Type.CENTER_X && target != Type.CENTER_Y;
				}
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.LEFT:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.RIGHT:
				{
					bool isCompatible = target == Type.LEFT || target == Type.RIGHT;
					if (anchor.Owner is Guideline)
					{
						isCompatible = isCompatible || target == Type.CENTER_X;
					}
					return isCompatible;
				}
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.TOP:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BOTTOM:
				{
					bool isCompatible = target == Type.TOP || target == Type.BOTTOM;
					if (anchor.Owner is Guideline)
					{
						isCompatible = isCompatible || target == Type.CENTER_Y;
					}
					return isCompatible;
				}
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BASELINE:
				{
					if (target == Type.LEFT || target == Type.RIGHT)
					{
						return false;
					}
					return true;
				}
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_X:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_Y:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.NONE:
					return false;
			}
			throw new AssertionError(mType.ToString());
		}

		/// <summary>
		/// Return true if this anchor is a side anchor
		/// </summary>
		/// <returns> true if side anchor </returns>
		public virtual bool SideAnchor
		{
			get
			{
				switch (mType)
				{
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.LEFT:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.RIGHT:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.TOP:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BOTTOM:
						return true;
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BASELINE:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_X:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_Y:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.NONE:
						return false;
				}
				throw new AssertionError(mType.ToString());
			}
		}

		/// <summary>
		/// Return true if the connection to the given anchor is in the
		/// same dimension (horizontal or vertical)
		/// </summary>
		/// <param name="anchor"> the anchor we want to connect to </param>
		/// <returns> true if it's an anchor on the same dimension </returns>
		public virtual bool isSimilarDimensionConnection(ConstraintAnchor anchor)
		{
			Type target = anchor.getType();
			if (target == mType)
			{
				return true;
			}
			switch (mType)
			{
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER:
				{
					return target != Type.BASELINE;
				}
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.LEFT:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.RIGHT:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_X:
				{
					return target == Type.LEFT || target == Type.RIGHT || target == Type.CENTER_X;
				}
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.TOP:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BOTTOM:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_Y:
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BASELINE:
				{
					return target == Type.TOP || target == Type.BOTTOM || target == Type.CENTER_Y || target == Type.BASELINE;
				}
				case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.NONE:
					return false;
			}
			throw new AssertionError(mType.ToString());
		}


		/// <summary>
		/// Set the gone margin of the connection (if there's one) </summary>
		/// <param name="margin"> the new margin of the connection </param>
		public virtual int GoneMargin
		{
			set
			{
				if (Connected)
				{
					mGoneMargin = value;
				}
			}
		}

		/// <summary>
		/// Utility function returning true if this anchor is a vertical one.
		/// </summary>
		/// <returns> true if vertical anchor, false otherwise </returns>
		public virtual bool VerticalAnchor
		{
			get
			{
				switch (mType)
				{
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.LEFT:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.RIGHT:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_X:
						return false;
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_Y:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.TOP:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BOTTOM:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BASELINE:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.NONE:
						return true;
				}
				throw new AssertionError(mType.ToString());
			}
		}

		/// <summary>
		/// Return a string representation of this anchor
		/// </summary>
		/// <returns> string representation of the anchor </returns>
		public override string ToString()
		{
			return mOwner.DebugName + ":" + mType.ToString();
		}

		/// 
		/// <summary>
		///  Return true if we can connect this anchor to this target.
		/// We recursively follow connections in order to detect eventual cycles; if we
		/// do we disallow the connection.
		/// We also only allow connections to direct parent, siblings, and descendants.
		/// </summary>
		/// <param name="target"> the ConstraintWidget we are trying to connect to </param>
		/// <param name="anchor"> Allow anchor if it loops back to me directly </param>
		/// <returns> if the connection is allowed, false otherwise </returns>
		public virtual bool isConnectionAllowed(ConstraintWidget target, ConstraintAnchor anchor)
		{
			if (ALLOW_BINARY)
			{
				if (anchor != null && anchor.Target == this)
				{
					return true;
				}
			}
			return isConnectionAllowed(target);
		}

		/// <summary>
		/// Return true if we can connect this anchor to this target.
		/// We recursively follow connections in order to detect eventual cycles; if we
		/// do we disallow the connection.
		/// We also only allow connections to direct parent, siblings, and descendants.
		/// </summary>
		/// <param name="target"> the ConstraintWidget we are trying to connect to </param>
		/// <returns> true if the connection is allowed, false otherwise </returns>
		public virtual bool isConnectionAllowed(ConstraintWidget target)
		{
			HashSet<ConstraintWidget> @checked = new HashSet<ConstraintWidget>();
			if (isConnectionToMe(target, @checked))
			{
				return false;
			}
			ConstraintWidget parent = Owner.Parent;
			if (parent == target)
			{ // allow connections to parent
				return true;
			}
			if (target.Parent == parent)
			{ // allow if we share the same parent
				return true;
			}
			return false;
		}

		/// <summary>
		/// Recursive with check for loop
		/// </summary>
		/// <param name="target"> </param>
		/// <param name="checked"> set of things already checked </param>
		/// <returns> true if it is connected to me </returns>
		private bool isConnectionToMe(ConstraintWidget target, HashSet<ConstraintWidget> @checked)
		{
			if (@checked.Contains(target))
			{
				return false;
			}
			@checked.Add(target);

			if (target == Owner)
			{
				return true;
			}
			List<ConstraintAnchor> targetAnchors = target.Anchors;
			for (int i = 0, targetAnchorsSize = targetAnchors.Count; i < targetAnchorsSize; i++)
			{
				ConstraintAnchor anchor = targetAnchors[i];
				if (anchor.isSimilarDimensionConnection(this) && anchor.Connected)
				{
					if (isConnectionToMe(anchor.Target.Owner, @checked))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Returns the opposite anchor to this one </summary>
		/// <returns> opposite anchor </returns>
		public ConstraintAnchor Opposite
		{
			get
			{
				switch (mType)
				{
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.LEFT:
					{
						return mOwner.mRight;
					}
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.RIGHT:
					{
						return mOwner.mLeft;
					}
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.TOP:
					{
						return mOwner.mBottom;
					}
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BOTTOM:
					{
						return mOwner.mTop;
					}
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BASELINE:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_X:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_Y:
					case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.NONE:
						return null;
				}
				throw new AssertionError(mType.ToString());
			}
		}
	}

}