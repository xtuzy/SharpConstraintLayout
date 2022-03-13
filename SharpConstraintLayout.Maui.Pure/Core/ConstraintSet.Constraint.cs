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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Pure.Core
{
	public partial class ConstraintSet
	{
		public class Constraint
		{
			internal int mViewId;
			internal string mTargetString;
			public readonly PropertySet propertySet = new PropertySet();
			//public readonly Motion motion = new Motion();
			public readonly Layout layout = new Layout();
			public readonly Transform transform = new Transform();
			//public Dictionary<string, ConstraintAttribute> mCustomConstraints = new Dictionary<string, ConstraintAttribute>();
			internal Delta mDelta;

			internal class Delta
			{
				internal const int INITIAL_BOOLEAN = 4;
				internal const int INITIAL_INT = 10;
				internal const int INITIAL_FLOAT = 10;
				internal const int INITIAL_STRING = 5;
				internal int[] mTypeInt = new int[INITIAL_INT];
				internal int[] mValueInt = new int[INITIAL_INT];
				internal int mCountInt = 0;

				internal virtual void add(int type, int value)
				{
					if (mCountInt >= mTypeInt.Length)
					{
						//mTypeInt = Arrays.copyOf(mTypeInt, mTypeInt.Length * 2);
						mTypeInt = ArrayHelperClass.Copy(mTypeInt, mTypeInt.Length * 2);
						//mValueInt = Arrays.copyOf(mValueInt, mValueInt.Length * 2);
						mValueInt = ArrayHelperClass.Copy(mValueInt, mValueInt.Length * 2);
					}
					mTypeInt[mCountInt] = type;
					mValueInt[mCountInt++] = value;
				}

				internal int[] mTypeFloat = new int[INITIAL_FLOAT];
				internal float[] mValueFloat = new float[INITIAL_FLOAT];
				internal int mCountFloat = 0;

				internal virtual void add(int type, float value)
				{
					if (mCountFloat >= mTypeFloat.Length)
					{
						//mTypeFloat = Arrays.copyOf(mTypeFloat, mTypeFloat.Length * 2);
						mTypeFloat = ArrayHelperClass.Copy(mTypeFloat, mTypeFloat.Length * 2);
						//mValueFloat = Arrays.copyOf(mValueFloat, mValueFloat.Length * 2);
						mValueFloat = ArrayHelperClass.Copy(mValueFloat, mValueFloat.Length * 2);
					}
					mTypeFloat[mCountFloat] = type;
					mValueFloat[mCountFloat++] = value;
				}

				internal int[] mTypeString = new int[INITIAL_STRING];
				internal string[] mValueString = new string[INITIAL_STRING];
				internal int mCountString = 0;

				internal virtual void add(int type, string value)
				{
					if (mCountString >= mTypeString.Length)
					{
						//mTypeString = Arrays.copyOf(mTypeString, mTypeString.Length * 2);
						mTypeString = ArrayHelperClass.Copy(mTypeString, mTypeString.Length * 2);
						//mValueString = Arrays.copyOf(mValueString, mValueString.Length * 2);
						mValueString = ArrayHelperClass.Copy(mValueString, mValueString.Length * 2);
					}
					mTypeString[mCountString] = type;
					mValueString[mCountString++] = value;
				}

				internal int[] mTypeBoolean = new int[INITIAL_BOOLEAN];
				internal bool[] mValueBoolean = new bool[INITIAL_BOOLEAN];
				internal int mCountBoolean = 0;

				internal virtual void add(int type, bool value)
				{
					if (mCountBoolean >= mTypeBoolean.Length)
					{
						//mTypeBoolean = Arrays.copyOf(mTypeBoolean, mTypeBoolean.Length * 2);
						mTypeBoolean = ArrayHelperClass.Copy(mTypeBoolean, mTypeBoolean.Length * 2);
						//mValueBoolean = Arrays.copyOf(mValueBoolean, mValueBoolean.Length * 2);
						mValueBoolean = ArrayHelperClass.Copy(mValueBoolean, mValueBoolean.Length * 2);
					}
					mTypeBoolean[mCountBoolean] = type;
					mValueBoolean[mCountBoolean++] = value;
				}

				internal virtual void applyDelta(Constraint c)
				{
					for (int i = 0; i < mCountInt; i++)
					{
						setDeltaValue(c, mTypeInt[i], mValueInt[i]);
					}
					for (int i = 0; i < mCountFloat; i++)
					{
						setDeltaValue(c, mTypeFloat[i], mValueFloat[i]);
					}
					for (int i = 0; i < mCountString; i++)
					{
						setDeltaValue(c, mTypeString[i], mValueString[i]);
					}
					for (int i = 0; i < mCountBoolean; i++)
					{
						setDeltaValue(c, mTypeBoolean[i], mValueBoolean[i]);
					}
				}

				//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
				//ORIGINAL LINE: @SuppressLint("LogConditional") void printDelta(String tag)
				internal virtual void printDelta(string tag)
				{
					Debug.WriteLine(tag, "int");

					for (int i = 0; i < mCountInt; i++)
					{
						Debug.WriteLine(tag, mTypeInt[i] + " = " + mValueInt[i]);
					}
					Debug.WriteLine(tag, "float");

					for (int i = 0; i < mCountFloat; i++)
					{
						Debug.WriteLine(tag, mTypeFloat[i] + " = " + mValueFloat[i]);
					}
					Debug.WriteLine(tag, "strings");

					for (int i = 0; i < mCountString; i++)
					{
						Debug.WriteLine(tag, mTypeString[i] + " = " + mValueString[i]);
					}
					Debug.WriteLine(tag, "boolean");
					for (int i = 0; i < mCountBoolean; i++)
					{
						Debug.WriteLine(tag, mTypeBoolean[i] + " = " + mValueBoolean[i]);
					}
				}
			}

			public virtual void applyDelta(Constraint c)
			{
				if (mDelta != null)
				{
					mDelta.applyDelta(c);
				}
			}

			public virtual void printDelta(string tag)
			{
				if (mDelta != null)
				{
					mDelta.printDelta(tag);
				}
				else
				{
					//Debug.WriteLine(tag, "DELTA IS NULL");
					Debug.WriteLine(tag, "DELTA IS NULL");
				}
			}

			/*internal virtual ConstraintAttribute get(string attributeName, AttributeType attributeType)
			{
				ConstraintAttribute ret;
				if (mCustomConstraints.ContainsKey(attributeName))
				{
					ret = mCustomConstraints[attributeName];
					if (ret.Type != attributeType)
					{
						throw new System.ArgumentException("ConstraintAttribute is already a " + ret.Type.name());
					}
				}
				else
				{
					ret = new ConstraintAttribute(attributeName, attributeType);
					mCustomConstraints[attributeName] = ret;
				}
				return ret;
			}

			internal virtual void setStringValue(string attributeName, string value)
			{
				get(attributeName, AttributeType.STRING_TYPE).StringValue = value;
			}

			internal virtual void setFloatValue(string attributeName, float value)
			{
				get(attributeName, AttributeType.FLOAT_TYPE).FloatValue = value;
			}

			internal virtual void setIntValue(string attributeName, int value)
			{
				get(attributeName, AttributeType.INT_TYPE).IntValue = value;
			}

			internal virtual void setColorValue(string attributeName, int value)
			{
				get(attributeName, AttributeType.COLOR_TYPE).ColorValue = value;
			}*/

			public virtual Constraint clone()
			{
				Constraint clone = new Constraint();
				clone.layout.copyFrom(layout);
				//clone.motion.copyFrom(motion);
				clone.propertySet.copyFrom(propertySet);
				clone.transform.copyFrom(transform);
				clone.mViewId = mViewId;
				clone.mDelta = mDelta;
				return clone;
			}

			/*internal virtual void fillFromConstraints(ConstraintHelper helper, int viewId, Constraints.LayoutParams param)
			{
				fillFromConstraints(viewId, param);
				if (helper is Barrier)
				{
					layout.mHelperType = BARRIER_TYPE;
					Barrier barrier = (Barrier)helper;
					layout.mBarrierDirection = barrier.Type;
					layout.mReferenceIds = barrier.ReferencedIds;
					layout.mBarrierMargin = barrier.Margin;
				}
			}*/

			/*internal virtual void fillFromConstraints(int viewId, Constraints.LayoutParams param)
			{
				fillFrom(viewId, param);
				propertySet.alpha = param.alpha;
				transform.rotation = param.rotation;
				transform.rotationX = param.rotationX;
				transform.rotationY = param.rotationY;
				transform.scaleX = param.scaleX;
				transform.scaleY = param.scaleY;
				transform.transformPivotX = param.transformPivotX;
				transform.transformPivotY = param.transformPivotY;
				transform.translationX = param.translationX;
				transform.translationY = param.translationY;
				transform.translationZ = param.translationZ;
				transform.elevation = param.elevation;
				transform.applyElevation = param.applyElevation;
			}*/

			internal virtual void fillFrom(int viewId, ConstraintSet.Layout param)
			{
				mViewId = viewId;
				layout.leftToLeft = param.leftToLeft;
				layout.leftToRight = param.leftToRight;
				layout.rightToLeft = param.rightToLeft;
				layout.rightToRight = param.rightToRight;
				layout.topToTop = param.topToTop;
				layout.topToBottom = param.topToBottom;
				layout.bottomToTop = param.bottomToTop;
				layout.bottomToBottom = param.bottomToBottom;
				layout.baselineToBaseline = param.baselineToBaseline;
				layout.baselineToTop = param.baselineToTop;
				layout.baselineToBottom = param.baselineToBottom;
				layout.startToEnd = param.startToEnd;
				layout.startToStart = param.startToStart;
				layout.endToStart = param.endToStart;
				layout.endToEnd = param.endToEnd;

				layout.horizontalBias = param.horizontalBias;
				layout.verticalBias = param.verticalBias;
				layout.dimensionRatio = param.dimensionRatio;

				layout.circleConstraint = param.circleConstraint;
				layout.circleRadius = param.circleRadius;
				layout.circleAngle = param.circleAngle;

				layout.editorAbsoluteX = param.editorAbsoluteX;
				layout.editorAbsoluteY = param.editorAbsoluteY;
				layout.orientation = param.orientation;
				layout.guidePercent = param.guidePercent;
				layout.guideBegin = param.guideBegin;
				layout.guideEnd = param.guideEnd;
				layout.mWidth = param.mWidth;
				layout.mHeight = param.mHeight;
				layout.leftMargin = param.leftMargin;
				layout.rightMargin = param.rightMargin;
				layout.topMargin = param.topMargin;
				layout.bottomMargin = param.bottomMargin;
				layout.baselineMargin = param.baselineMargin;
				layout.verticalWeight = param.verticalWeight;
				layout.horizontalWeight = param.horizontalWeight;
				layout.verticalChainStyle = param.verticalChainStyle;
				layout.horizontalChainStyle = param.horizontalChainStyle;
				layout.constrainedWidth = param.constrainedWidth;
				layout.constrainedHeight = param.constrainedHeight;
				layout.matchConstraintDefaultWidth = param.matchConstraintDefaultWidth;
				layout.matchConstraintDefaultHeight = param.matchConstraintDefaultHeight;
				layout.matchConstraintMaxWidth = param.matchConstraintMaxWidth;
				layout.matchConstraintMaxHeight = param.matchConstraintMaxHeight;
				layout.matchConstraintMinWidth = param.matchConstraintMinWidth;
				layout.matchConstraintMinHeight = param.matchConstraintMinHeight;
				layout.matchConstraintPercentWidth = param.matchConstraintPercentWidth;
				layout.matchConstraintPercentHeight = param.matchConstraintPercentHeight;
				layout.constraintTag = param.constraintTag;
				layout.goneTopMargin = param.goneTopMargin;
				layout.goneBottomMargin = param.goneBottomMargin;
				layout.goneLeftMargin = param.goneLeftMargin;
				layout.goneRightMargin = param.goneRightMargin;
				layout.goneStartMargin = param.goneStartMargin;
				layout.goneEndMargin = param.goneEndMargin;
				layout.goneBaselineMargin = param.goneBaselineMargin;
				layout.wrapBehaviorInParent = param.wrapBehaviorInParent;

				//int currentApiVersion = Build.VERSION.SDK_INT;
				//if (currentApiVersion >= Build.VERSION_CODES.JELLY_BEAN_MR1)
				//{
					layout.endMargin = param.endMargin;
					layout.startMargin = param.startMargin;
				//}
			}

			public virtual void applyTo(ConstraintSet.Layout param)
			{
				param.leftToLeft = layout.leftToLeft;
				param.leftToRight = layout.leftToRight;
				param.rightToLeft = layout.rightToLeft;
				param.rightToRight = layout.rightToRight;

				param.topToTop = layout.topToTop;
				param.topToBottom = layout.topToBottom;
				param.bottomToTop = layout.bottomToTop;
				param.bottomToBottom = layout.bottomToBottom;

				param.baselineToBaseline = layout.baselineToBaseline;
				param.baselineToTop = layout.baselineToTop;
				param.baselineToBottom = layout.baselineToBottom;

				param.startToEnd = layout.startToEnd;
				param.startToStart = layout.startToStart;
				param.endToStart = layout.endToStart;
				param.endToEnd = layout.endToEnd;

				param.leftMargin = layout.leftMargin;
				param.rightMargin = layout.rightMargin;
				param.topMargin = layout.topMargin;
				param.bottomMargin = layout.bottomMargin;
				param.goneStartMargin = layout.goneStartMargin;
				param.goneEndMargin = layout.goneEndMargin;
				param.goneTopMargin = layout.goneTopMargin;
				param.goneBottomMargin = layout.goneBottomMargin;

				param.horizontalBias = layout.horizontalBias;
				param.verticalBias = layout.verticalBias;

				param.circleConstraint = layout.circleConstraint;
				param.circleRadius = layout.circleRadius;
				param.circleAngle = layout.circleAngle;

				param.dimensionRatio = layout.dimensionRatio;
				param.editorAbsoluteX = layout.editorAbsoluteX;
				param.editorAbsoluteY = layout.editorAbsoluteY;
				param.verticalWeight = layout.verticalWeight;
				param.horizontalWeight = layout.horizontalWeight;
				param.verticalChainStyle = layout.verticalChainStyle;
				param.horizontalChainStyle = layout.horizontalChainStyle;
				param.constrainedWidth = layout.constrainedWidth;
				param.constrainedHeight = layout.constrainedHeight;
				param.matchConstraintDefaultWidth = layout.matchConstraintDefaultWidth;
				param.matchConstraintDefaultHeight = layout.matchConstraintDefaultHeight;
				param.matchConstraintMaxWidth = layout.matchConstraintMaxWidth;
				param.matchConstraintMaxHeight = layout.matchConstraintMaxHeight;
				param.matchConstraintMinWidth = layout.matchConstraintMinWidth;
				param.matchConstraintMinHeight = layout.matchConstraintMinHeight;
				param.matchConstraintPercentWidth = layout.matchConstraintPercentWidth;
				param.matchConstraintPercentHeight = layout.matchConstraintPercentHeight;
				param.orientation = layout.orientation;
				param.guidePercent = layout.guidePercent;
				param.guideBegin = layout.guideBegin;
				param.guideEnd = layout.guideEnd;
				param.mWidth = layout.mWidth;
				param.mHeight = layout.mHeight;
				if (!string.ReferenceEquals(layout.constraintTag, null))
				{
					param.constraintTag = layout.constraintTag;
				}
				param.wrapBehaviorInParent = layout.wrapBehaviorInParent;

				//if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
				//{
					param.startMargin = layout.startMargin;
					param.endMargin = layout.endMargin;
				//}

				//param.validate();
			}
		}
	}
}
