using System.Collections.Generic;
using System.Reflection;

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
namespace androidx.constraintlayout.core.motion.key
{
	using floatRect = androidx.constraintlayout.core.motion.utils.FloatRect;
	using SplineSet = androidx.constraintlayout.core.motion.utils.SplineSet;
	using TypedValues = androidx.constraintlayout.core.motion.utils.TypedValues;


	public class MotionKeyTrigger : MotionKey
	{
		private bool InstanceFieldsInitialized = false;

		public MotionKeyTrigger()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			mType = KEY_TYPE;
			mCustom = new Dictionary<string,CustomVariable>();
		}

		private const string TAG = "KeyTrigger";
		public const string VIEW_TRANSITION_ON_CROSS = "viewTransitionOnCross";
		public const string VIEW_TRANSITION_ON_POSITIVE_CROSS = "viewTransitionOnPositiveCross";
		public const string VIEW_TRANSITION_ON_NEGATIVE_CROSS = "viewTransitionOnNegativeCross";
		public const string POST_LAYOUT = "postLayout";
		public const string TRIGGER_SLACK = "triggerSlack";
		public const string TRIGGER_COLLISION_VIEW = "triggerCollisionView";
		public const string TRIGGER_COLLISION_ID = "triggerCollisionId";
		public const string TRIGGER_ID = "triggerID";
		public const string POSITIVE_CROSS = "positiveCross";
		public const string NEGATIVE_CROSS = "negativeCross";
		public const string TRIGGER_RECEIVER = "triggerReceiver";
		public const string CROSS = "CROSS";

		private int mCurveFit = -1;
		private string mCross = null;
		private int mTriggerReceiver = UNSET;
		private string mNegativeCross = null;
		private string mPositiveCross = null;
		private int mTriggerID = UNSET;
		private int mTriggerCollisionId = UNSET;
	//   TODO private MotionWidget mTriggerCollisionView = null;
		internal float mTriggerSlack = .1f;
		private bool mFireCrossReset = true;
		private bool mFireNegativeReset = true;
		private bool mFirePositiveReset = true;
		private float mFireThreshold = float.NaN;
		private float mFireLastPos;
		private bool mPostLayout = false;
		internal int mViewTransitionOnNegativeCross = UNSET;
		internal int mViewTransitionOnPositiveCross = UNSET;
		internal int mViewTransitionOnCross = UNSET;

		public const int TYPE_VIEW_TRANSITION_ON_CROSS = 301;
		public const int TYPE_VIEW_TRANSITION_ON_POSITIVE_CROSS = 302;
		public const int TYPE_VIEW_TRANSITION_ON_NEGATIVE_CROSS = 303;
		public const int TYPE_POST_LAYOUT = 304;
		public const int TYPE_TRIGGER_SLACK = 305;
		public const int TYPE_TRIGGER_COLLISION_VIEW = 306;
		public const int TYPE_TRIGGER_COLLISION_ID = 307;
		public const int TYPE_TRIGGER_ID = 308;
		public const int TYPE_POSITIVE_CROSS = 309;
		public const int TYPE_NEGATIVE_CROSS = 310;
		public const int TYPE_TRIGGER_RECEIVER = 311;
		public const int TYPE_CROSS = 312;

		internal floatRect mCollisionRect = new floatRect();
		internal floatRect mTargetRect = new floatRect();
		internal Dictionary<string, MethodInfo> mMethodHashMap = new Dictionary<string, MethodInfo>();
		public const int KEY_TYPE = 5;

		public override void getAttributeNames(HashSet<string> attributes)
		{

		}

		public override void addValues(Dictionary<string, SplineSet> splines)
		{

		}

		public override int getId(string name)
		{
			switch (name)
			{
				case VIEW_TRANSITION_ON_CROSS:
					return TYPE_VIEW_TRANSITION_ON_CROSS;
				case VIEW_TRANSITION_ON_POSITIVE_CROSS:
					return TYPE_VIEW_TRANSITION_ON_POSITIVE_CROSS;
				case VIEW_TRANSITION_ON_NEGATIVE_CROSS:
					return TYPE_VIEW_TRANSITION_ON_NEGATIVE_CROSS;
				case POST_LAYOUT:
					return TYPE_POST_LAYOUT;
				case TRIGGER_SLACK:
					return TYPE_TRIGGER_SLACK;
				case TRIGGER_COLLISION_VIEW:
					return TYPE_TRIGGER_COLLISION_VIEW;
				case TRIGGER_COLLISION_ID:
					return TYPE_TRIGGER_COLLISION_ID;
				case TRIGGER_ID:
					return TYPE_TRIGGER_ID;
				case POSITIVE_CROSS:
					return TYPE_POSITIVE_CROSS;
				case NEGATIVE_CROSS:
					return TYPE_NEGATIVE_CROSS;
				case TRIGGER_RECEIVER:
					return TYPE_TRIGGER_RECEIVER;
			}
			return -1;
		}

		public override MotionKey copy(MotionKey src)
		{
			base.copy(src);
			MotionKeyTrigger k = (MotionKeyTrigger) src;
			mCurveFit = k.mCurveFit;
			mCross = k.mCross;
			mTriggerReceiver = k.mTriggerReceiver;
			mNegativeCross = k.mNegativeCross;
			mPositiveCross = k.mPositiveCross;
			mTriggerID = k.mTriggerID;
			mTriggerCollisionId = k.mTriggerCollisionId;
			// TODO mTriggerCollisionView = k.mTriggerCollisionView;
			mTriggerSlack = k.mTriggerSlack;
			mFireCrossReset = k.mFireCrossReset;
			mFireNegativeReset = k.mFireNegativeReset;
			mFirePositiveReset = k.mFirePositiveReset;
			mFireThreshold = k.mFireThreshold;
			mFireLastPos = k.mFireLastPos;
			mPostLayout = k.mPostLayout;
			mCollisionRect = k.mCollisionRect;
			mTargetRect = k.mTargetRect;
			mMethodHashMap = k.mMethodHashMap;
			return this;
		}

		public override MotionKey clone()
		{
			return (new MotionKeyTrigger()).copy(this);
		}
		private void fireCustom(string str, MotionWidget widget)
		{
			bool callAll = str.Length == 1;
			if (!callAll)
			{
				str = str.Substring(1).ToLowerInvariant();
			}
			foreach (string name in mCustom.Keys)
			{
				string lowerCase = name.ToLowerInvariant();
				if (callAll || lowerCase.Matches(str))
				{
					CustomVariable custom = mCustom[name];
					if (custom != null)
					{
						custom.applyToWidget(widget);
					}
				}
			}
		}
		public virtual void conditionallyFire(float position, MotionWidget child)
		{
		}

		public override bool setValue(int type, int value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Trigger.TYPE_TRIGGER_RECEIVER:
					mTriggerReceiver = value;
					break;
				case TypedValues.TypedValues_Trigger.TYPE_TRIGGER_ID:
					mTriggerID = toInt(value);
					break;
				case TypedValues.TypedValues_Trigger.TYPE_TRIGGER_COLLISION_ID:
					mTriggerCollisionId = value;
					break;
				case TypedValues.TypedValues_Trigger.TYPE_VIEW_TRANSITION_ON_NEGATIVE_CROSS:
					mViewTransitionOnNegativeCross = value;
					break;
				case TypedValues.TypedValues_Trigger.TYPE_VIEW_TRANSITION_ON_POSITIVE_CROSS:
					mViewTransitionOnPositiveCross = value;
					break;

				case TypedValues.TypedValues_Trigger.TYPE_VIEW_TRANSITION_ON_CROSS:
					mViewTransitionOnCross = value;
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}

		public override bool setValue(int type, float value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Trigger.TYPE_TRIGGER_SLACK:
					mTriggerSlack = value;
					break;
				default:
				return base.setValue(type, value);
			}
			return true;
		}

		public override bool setValue(int type, string value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Trigger.TYPE_CROSS:
					mCross = value;
					break;
				case TypedValues.TypedValues_Trigger.TYPE_NEGATIVE_CROSS:
					mNegativeCross = value;
					break;
				case TypedValues.TypedValues_Trigger.TYPE_POSITIVE_CROSS:
					mPositiveCross = value;
					break;
	//                TODO
	//            case TRIGGER_COLLISION_VIEW:
	//                mTriggerCollisionView = (MotionWidget) value;
	//                break;

				default:

					return base.setValue(type, value);
			}
			return true;
		}

		public override bool setValue(int type, bool value)
		{
			switch (type)
			{
				case TypedValues.TypedValues_Trigger.TYPE_POST_LAYOUT:
					mPostLayout = value;
					break;
				default:
					return base.setValue(type, value);
			}
			return true;
		}


	}

}