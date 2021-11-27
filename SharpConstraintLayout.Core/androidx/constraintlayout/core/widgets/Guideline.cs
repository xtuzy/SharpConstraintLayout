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

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
    using static androidx.constraintlayout.core.widgets.ConstraintWidget.DimensionBehaviour;

    /// <summary>
    /// Guideline
    /// </summary>
    public class Guideline : ConstraintWidget
    {
        private bool InstanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {
            mAnchor = mTop;
        }

        public new const int HORIZONTAL = 0;
        public new const int VERTICAL = 1;

        public const int RELATIVE_PERCENT = 0;
        public const int RELATIVE_BEGIN = 1;
        public const int RELATIVE_END = 2;
        public const int RELATIVE_UNKNOWN = -1;

        protected internal float mRelativePercent = -1;
        protected internal int mRelativeBegin = -1;
        protected internal int mRelativeEnd = -1;

        private ConstraintAnchor mAnchor;
        private int mOrientation = HORIZONTAL;
        private int mMinimumPosition = 0;
        private bool resolved;

        public Guideline()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
            mAnchors.Clear();
            mAnchors.Add(mAnchor);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int count = mListAnchors.length;
            int count = mListAnchors.Length;
            for (int i = 0; i < count; i++)
            {
                mListAnchors[i] = mAnchor;
            }
        }

        public override void copy(ConstraintWidget src, Dictionary<ConstraintWidget, ConstraintWidget> map)
        {
            base.copy(src, map);
            Guideline srcGuideline = (Guideline)src;
            mRelativePercent = srcGuideline.mRelativePercent;
            mRelativeBegin = srcGuideline.mRelativeBegin;
            mRelativeEnd = srcGuideline.mRelativeEnd;
            Orientation = srcGuideline.mOrientation;
        }

        public override bool allowedInBarrier()
        {
            return true;
        }

        public virtual int RelativeBehaviour
        {
            get
            {
                if (mRelativePercent != -1)
                {
                    return RELATIVE_PERCENT;
                }
                if (mRelativeBegin != -1)
                {
                    return RELATIVE_BEGIN;
                }
                if (mRelativeEnd != -1)
                {
                    return RELATIVE_END;
                }
                return RELATIVE_UNKNOWN;
            }
        }

        public virtual int Orientation
        {
            set
            {
                if (mOrientation == value)
                {
                    return;
                }
                mOrientation = value;
                mAnchors.Clear();
                if (mOrientation == VERTICAL)
                {
                    mAnchor = mLeft;
                }
                else
                {
                    mAnchor = mTop;
                }
                mAnchors.Add(mAnchor);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int count = mListAnchors.length;
                int count = mListAnchors.Length;
                for (int i = 0; i < count; i++)
                {
                    mListAnchors[i] = mAnchor;
                }
            }
            get
            {
                return mOrientation;
            }
        }

        public virtual ConstraintAnchor Anchor
        {
            get
            {
                return mAnchor;
            }
        }

        /// <summary>
        /// Specify the xml type for the container
        /// 
        /// @return
        /// </summary>
        public override string Type
        {
            get
            {
                return "Guideline";
            }
        }


        public virtual int MinimumPosition
        {
            set
            {
                mMinimumPosition = value;
            }
        }

        public override ConstraintAnchor getAnchor(ConstraintAnchor.Type anchorType)
        {
            switch (anchorType)
            {
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.LEFT:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.RIGHT:
                    {
                        if (mOrientation == VERTICAL)
                        {
                            return mAnchor;
                        }
                    }
                    break;
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.TOP:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BOTTOM:
                    {
                        if (mOrientation == HORIZONTAL)
                        {
                            return mAnchor;
                        }
                    }
                    break;
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.BASELINE:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_X:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.CENTER_Y:
                case androidx.constraintlayout.core.widgets.ConstraintAnchor.Type.NONE:
                    return null;
            }
            return null;
        }

        public virtual void setGuidePercent(int value)
        {

            setGuidePercent(value / 100f);

        }

        public virtual void setGuidePercent(float value)
        {
            if (value > -1)
            {
                mRelativePercent = value;
                mRelativeBegin = -1;
                mRelativeEnd = -1;
            }
        }

        public virtual int GuideBegin
        {
            set
            {
                if (value > -1)
                {
                    mRelativePercent = -1;
                    mRelativeBegin = value;
                    mRelativeEnd = -1;
                }
            }
        }

        public virtual int GuideEnd
        {
            set
            {
                if (value > -1)
                {
                    mRelativePercent = -1;
                    mRelativeBegin = -1;
                    mRelativeEnd = value;
                }
            }
        }

        public virtual float RelativePercent
        {
            get
            {
                return mRelativePercent;
            }
        }

        public virtual int RelativeBegin
        {
            get
            {
                return mRelativeBegin;
            }
        }

        public virtual int RelativeEnd
        {
            get
            {
                return mRelativeEnd;
            }
        }

        public virtual int FinalValue
        {
            set
            {
                if (LinearSystem.FULL_DEBUG)
                {
                    Console.WriteLine("*** SET FINAL GUIDELINE VALUE " + value + " FOR " + DebugName);
                }
                mAnchor.FinalValue = value;
                resolved = true;
            }
        }

        public override bool ResolvedHorizontally
        {
            get
            {
                return resolved;
            }
        }

        public override bool ResolvedVertically
        {
            get
            {
                return resolved;
            }
        }


        public override void addToSolver(LinearSystem system, bool optimize)
        {
            if (LinearSystem.FULL_DEBUG)
            {
                Console.WriteLine("\n----------------------------------------------");
                Console.WriteLine("-- adding " + DebugName + " to the solver");
                Console.WriteLine("----------------------------------------------\n");
            }

            ConstraintWidgetContainer parent = (ConstraintWidgetContainer)Parent;
            if (parent == null)
            {
                return;
            }
            ConstraintAnchor begin = parent.getAnchor(ConstraintAnchor.Type.LEFT);
            ConstraintAnchor end = parent.getAnchor(ConstraintAnchor.Type.RIGHT);
            bool parentWrapContent = mParent != null ? mParent.mListDimensionBehaviors[DIMENSION_HORIZONTAL] == WRAP_CONTENT : false;
            if (mOrientation == HORIZONTAL)
            {
                begin = parent.getAnchor(ConstraintAnchor.Type.TOP);
                end = parent.getAnchor(ConstraintAnchor.Type.BOTTOM);
                parentWrapContent = mParent != null ? mParent.mListDimensionBehaviors[DIMENSION_VERTICAL] == WRAP_CONTENT : false;
            }
            if (resolved && mAnchor.hasFinalValue())
            {
                SolverVariable guide = system.createObjectVariable(mAnchor);
                if (LinearSystem.FULL_DEBUG)
                {
                    Console.WriteLine("*** SET FINAL POSITION FOR GUIDELINE " + DebugName + " TO " + mAnchor.FinalValue);
                }
                system.addEquality(guide, mAnchor.FinalValue);
                if (mRelativeBegin != -1)
                {
                    if (parentWrapContent)
                    {
                        system.addGreaterThan(system.createObjectVariable(end), guide, 0, SolverVariable.STRENGTH_EQUALITY);
                    }
                }
                else if (mRelativeEnd != -1)
                {
                    if (parentWrapContent)
                    {
                        SolverVariable parentRight = system.createObjectVariable(end);
                        system.addGreaterThan(guide, system.createObjectVariable(begin), 0, SolverVariable.STRENGTH_EQUALITY);
                        system.addGreaterThan(parentRight, guide, 0, SolverVariable.STRENGTH_EQUALITY);
                    }
                }
                resolved = false;
                return;
            }
            if (mRelativeBegin != -1)
            {
                SolverVariable guide = system.createObjectVariable(mAnchor);
                SolverVariable parentLeft = system.createObjectVariable(begin);
                system.addEquality(guide, parentLeft, mRelativeBegin, SolverVariable.STRENGTH_FIXED);
                if (parentWrapContent)
                {
                    system.addGreaterThan(system.createObjectVariable(end), guide, 0, SolverVariable.STRENGTH_EQUALITY);
                }
            }
            else if (mRelativeEnd != -1)
            {
                SolverVariable guide = system.createObjectVariable(mAnchor);
                SolverVariable parentRight = system.createObjectVariable(end);
                system.addEquality(guide, parentRight, -mRelativeEnd, SolverVariable.STRENGTH_FIXED);
                if (parentWrapContent)
                {
                    system.addGreaterThan(guide, system.createObjectVariable(begin), 0, SolverVariable.STRENGTH_EQUALITY);
                    system.addGreaterThan(parentRight, guide, 0, SolverVariable.STRENGTH_EQUALITY);
                }
            }
            else if (mRelativePercent != -1)
            {
                SolverVariable guide = system.createObjectVariable(mAnchor);
                SolverVariable parentRight = system.createObjectVariable(end);
                system.addConstraint(LinearSystem.createRowDimensionPercent(system, guide, parentRight, mRelativePercent));
            }
        }

        public override void updateFromSolver(LinearSystem system, bool optimize)
        {
            if (Parent == null)
            {
                return;
            }
            int value = system.getObjectVariableValue(mAnchor);
            if (mOrientation == VERTICAL)
            {
                X = value;
                Y = 0;
                Height = Parent.Height;
                Width = 0;
            }
            else
            {
                X = 0;
                Y = value;
                Width = Parent.Width;
                Height = 0;
            }
        }

        internal virtual void inferRelativePercentPosition()
        {
            float percent = (X / (float)Parent.Width);
            if (mOrientation == HORIZONTAL)
            {
                percent = (Y / (float)Parent.Height);
            }
            setGuidePercent(percent);
        }

        internal virtual void inferRelativeBeginPosition()
        {
            int position = X;
            if (mOrientation == HORIZONTAL)
            {
                position = Y;
            }
            GuideBegin = position;
        }

        internal virtual void inferRelativeEndPosition()
        {
            int position = Parent.Width - X;
            if (mOrientation == HORIZONTAL)
            {
                position = Parent.Height - Y;
            }
            GuideEnd = position;
        }

        public virtual void cyclePosition()
        {
            if (mRelativeBegin != -1)
            {
                // cycle to percent-based position
                inferRelativePercentPosition();
            }
            else if (mRelativePercent != -1)
            {
                // cycle to end-based position
                inferRelativeEndPosition();
            }
            else if (mRelativeEnd != -1)
            {
                // cycle to begin-based position
                inferRelativeBeginPosition();
            }
        }

        public virtual bool Percent
        {
            get
            {
                return mRelativePercent != -1 && mRelativeBegin == -1 && mRelativeEnd == -1;
            }
        }
    }

}