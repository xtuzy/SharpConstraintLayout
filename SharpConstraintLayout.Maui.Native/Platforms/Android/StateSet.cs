using System.Collections.Generic;

/*
 * Copyright (C) 2018 The Android Open Source Project
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

namespace SharpConstraintLayout.Maui.Platforms.Androids
{
	using Context = android.content.Context;
	using Resources = android.content.res.Resources;
	using TypedArray = android.content.res.TypedArray;
	using XmlResourceParser = android.content.res.XmlResourceParser;
	using AttributeSet = android.util.AttributeSet;
	using Log = android.util.Log;
	using SparseArray = android.util.SparseArray;
	using Xml = android.util.Xml;

	using XmlPullParser = org.xmlpull.v1.XmlPullParser;
	using XmlPullParserException = org.xmlpull.v1.XmlPullParserException;


	/// <summary>
	/// @suppress
	/// </summary>

	public class StateSet
	{
		public const string TAG = "ConstraintLayoutStates";
		private const bool DEBUG = false;
		internal int mDefaultState = -1;

		 internal ConstraintSet mDefaultConstraintSet;
		internal int mCurrentStateId = -1; // default
		internal int mCurrentConstraintNumber = -1; // default
		private SparseArray<State> mStateList = new SparseArray<State>();
		private SparseArray<ConstraintSet> mConstraintSetMap = new SparseArray<ConstraintSet>();
		private ConstraintsChangedListener mConstraintsChangedListener = null;

		/// <summary>
		/// Parse a StateSet </summary>
		/// <param name="context"> </param>
		/// <param name="parser"> </param>
		public StateSet(Context context, XmlPullParser parser)
		{
			load(context, parser);
		}

		/// <summary>
		/// Load a constraint set from a constraintSet.xml file
		/// </summary>
		/// <param name="context">    the context for the inflation </param>
		/// <param name="parser">  mId of xml file in res/xml/ </param>
		private void load(Context context, XmlPullParser parser)
		{
			if (DEBUG)
			{
				Log.v(TAG, "#########load stateSet###### ");
			}
			// Parse the stateSet attributes
			AttributeSet attrs = Xml.asAttributeSet(parser);
			TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.StateSet);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
			int N = a.IndexCount;

			for (int i = 0; i < N; i++)
			{
				int attr = a.getIndex(i);
				if (attr == R.styleable.StateSet_defaultState)
				{
					mDefaultState = a.getResourceId(attr, mDefaultState);
				}
			}
			a.recycle();

			string tagName = null;
			try
			{
				Variant match;
				string document = null;
				State state = null;
				for (int eventType = parser.EventType; eventType != XmlResourceParser.END_DOCUMENT; eventType = parser.next())
				{

					switch (eventType)
					{
						case XmlResourceParser.START_DOCUMENT:
							document = parser.Name;
							break;
						case XmlResourceParser.START_TAG:
							tagName = parser.Name;
							switch (tagName)
							{
								case "LayoutDescription":
									break;
								case "StateSet":
									break;
								case "State":
									state = new State(context, parser);
									mStateList.put(state.mId, state);
									break;
								case "Variant":
									match = new Variant(context, parser);
									if (state != null)
									{
										state.add(match);
									}
									break;

								default:
									if (DEBUG)
									{
										Log.v(TAG, "unknown tag " + tagName);
									}
								break;
							}

							break;
						case XmlResourceParser.END_TAG:
							if ("StateSet".Equals(parser.Name))
							{
								if (DEBUG)
								{
									Log.v(TAG, "############ finished parsing state set");
								}
									return;
							}

							tagName = null;
							break;
						case XmlResourceParser.TEXT:
							break;
					}
				}

			}
			catch (XmlPullParserException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		public virtual bool needsToChange(int id, float width, float height)
		{
			if (mCurrentStateId != id)
			{
				return true;
			}

			State state = (id == -1) ? mStateList.valueAt(0) : mStateList.get(mCurrentStateId);

			if (mCurrentConstraintNumber != -1)
			{
				if (state.mVariants[mCurrentConstraintNumber].match(width, height))
				{
					return false;
				}
			}

			if (mCurrentConstraintNumber == state.findMatch(width, height))
			{
				return false;
			}
			return true;
		}

		public virtual ConstraintsChangedListener OnConstraintsChanged
		{
			set
			{
				this.mConstraintsChangedListener = value;
			}
		}

		public virtual int stateGetConstraintID(int id, int width, int height)
		{
			return updateConstraints(-1, id, width, height);
		}

		/// <summary>
		/// converts a state to a constraintSet
		/// </summary>
		/// <param name="currentConstrainSettId"> </param>
		/// <param name="stateId"> </param>
		/// <param name="width"> </param>
		/// <param name="height">
		/// @return </param>
		public virtual int convertToConstraintSet(int currentConstrainSettId, int stateId, float width, float height)
		{
			State state = mStateList.get(stateId);
			if (state == null)
			{
				return stateId;
			}
			if (width == -1 || height == -1)
			{ // for the case without width/height matching
				if (state.mConstraintID == currentConstrainSettId)
				{
					return currentConstrainSettId;
				}
				foreach (Variant mVariant in state.mVariants)
				{
					if (currentConstrainSettId == mVariant.mConstraintID)
					{
						return currentConstrainSettId;
					}
				}
				return state.mConstraintID;
			}
			else
			{
				Variant match = null;
				foreach (Variant mVariant in state.mVariants)
				{
					if (mVariant.match(width,height))
					{
						if (currentConstrainSettId == mVariant.mConstraintID)
						{
							return currentConstrainSettId;
						}
						match = mVariant;
					}
				}
				if (match != null)
				{
					return match.mConstraintID;
				}

				return state.mConstraintID;
			}
		}

		public virtual int updateConstraints(int currentId, int id, float width, float height)
		{
			if (currentId == id)
			{
				State state;
				if (id == -1)
				{
					state = mStateList.valueAt(0); // id not being used take the first
				}
				else
				{
					state = mStateList.get(mCurrentStateId);

				}
				if (state == null)
				{
					return -1;
				}
				if (mCurrentConstraintNumber != -1)
				{
					if (state.mVariants[currentId].match(width, height))
					{
						return currentId;
					}
				}
				int match = state.findMatch(width, height);
				if (currentId == match)
				{
					return currentId;
				}

				return (match == -1) ? state.mConstraintID : state.mVariants[match].mConstraintID;

			}
			else
			{
				State state = mStateList.get(id);
				if (state == null)
				{
					return -1;
				}
				int match = state.findMatch(width, height);
				return (match == -1) ? state.mConstraintID : state.mVariants[match].mConstraintID;
			}

		}

		/////////////////////////////////////////////////////////////////////////
		//      This represents one state
		/////////////////////////////////////////////////////////////////////////
		internal class State
		{
			internal int mId;
			internal List<Variant> mVariants = new List<Variant>();
			internal int mConstraintID = -1;
			internal bool mIsLayout = false;
			public State(Context context, XmlPullParser parser)
			{
				AttributeSet attrs = Xml.asAttributeSet(parser);
				TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.State);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
				int N = a.IndexCount;
				for (int i = 0; i < N; i++)
				{
					int attr = a.getIndex(i);
					if (attr == R.styleable.State_android_id)
					{
						mId = a.getResourceId(attr, mId);
					}
					else if (attr == R.styleable.State_constraints)
					{
						mConstraintID = a.getResourceId(attr, mConstraintID);
						string type = context.Resources.getResourceTypeName(mConstraintID);
						string name = context.Resources.getResourceName(mConstraintID);

						if ("layout".Equals(type))
						{
							mIsLayout = true;
						}
					}
				}
				a.recycle();
			}

			internal virtual void add(Variant size)
			{
				mVariants.Add(size);
			}

			public virtual int findMatch(float width, float height)
			{
				for (int i = 0; i < mVariants.Count; i++)
				{
					if (mVariants[i].match(width, height))
					{
						return i;
					}
				}
				return -1;
			}
		}

		internal class Variant
		{
			internal int mId;
			internal float mMinWidth = Float.NaN;
			internal float mMinHeight = Float.NaN;
			internal float mMaxWidth = Float.NaN;
			internal float mMaxHeight = Float.NaN;
			internal int mConstraintID = -1;
			internal bool mIsLayout = false;

			public Variant(Context context, XmlPullParser parser)
			{
				AttributeSet attrs = Xml.asAttributeSet(parser);
				TypedArray a = context.obtainStyledAttributes(attrs, R.styleable.Variant);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
				int N = a.IndexCount;
				if (DEBUG)
				{
					Log.v(TAG, "############### Variant");
				}

				for (int i = 0; i < N; i++)
				{
					int attr = a.getIndex(i);
					if (attr == R.styleable.Variant_constraints)
					{
						mConstraintID = a.getResourceId(attr, mConstraintID);
						string type = context.Resources.getResourceTypeName(mConstraintID);
						string name = context.Resources.getResourceName(mConstraintID);

						if ("layout".Equals(type))
						{
							mIsLayout = true;
						}
					}
					else if (attr == R.styleable.Variant_region_heightLessThan)
					{
						mMaxHeight = a.getDimension(attr, mMaxHeight);
					}
					else if (attr == R.styleable.Variant_region_heightMoreThan)
					{
						mMinHeight = a.getDimension(attr, mMinHeight);
					}
					else if (attr == R.styleable.Variant_region_widthLessThan)
					{
						mMaxWidth = a.getDimension(attr, mMaxWidth);
					}
					else if (attr == R.styleable.Variant_region_widthMoreThan)
					{
						mMinWidth = a.getDimension(attr, mMinWidth);
					}
					else
					{
						Log.v(TAG, "Unknown tag");
					}
				}
				a.recycle();
				if (DEBUG)
				{
					Log.v(TAG, "############### Variant");
					if (!float.IsNaN(mMinWidth))
					{
						Log.v(TAG, "############### Variant mMinWidth " + mMinWidth);
					}
					if (!float.IsNaN(mMinHeight))
					{
						Log.v(TAG, "############### Variant mMinHeight " + mMinHeight);
					}
					if (!float.IsNaN(mMaxWidth))
					{
						Log.v(TAG, "############### Variant mMaxWidth " + mMaxWidth);
					}
					if (!float.IsNaN(mMaxHeight))
					{
						Log.v(TAG, "############### Variant mMinWidth " + mMaxHeight);
					}
				}
			}

			internal virtual bool match(float widthDp, float heightDp)
			{
				if (DEBUG)
				{
					Log.v(TAG, "width = " + (int) widthDp + " < " + mMinWidth + " && " + (int) widthDp + " > " + mMaxWidth + " height = " + (int) heightDp + " < " + mMinHeight + " && " + (int) heightDp + " > " + mMaxHeight);
				}
				if (!float.IsNaN(mMinWidth))
				{
					if (widthDp < mMinWidth)
					{
						return false;
					}
				}
				if (!float.IsNaN(mMinHeight))
				{
					if (heightDp < mMinHeight)
					{
						return false;
					}
				}
				if (!float.IsNaN(mMaxWidth))
				{
					if (widthDp > mMaxWidth)
					{
						return false;
					}
				}
				if (!float.IsNaN(mMaxHeight))
				{
					if (heightDp > mMaxHeight)
					{
						return false;
					}
				}
				return true;

			}
		}

	}

}