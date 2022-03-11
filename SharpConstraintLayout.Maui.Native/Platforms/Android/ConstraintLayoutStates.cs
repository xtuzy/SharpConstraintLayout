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
	public class ConstraintLayoutStates
	{
		public const string TAG = "ConstraintLayoutStates";
		private const bool DEBUG = false;
		private readonly ConstraintLayout mConstraintLayout;
		internal ConstraintSet mDefaultConstraintSet;
		internal int mCurrentStateId = -1; // default
		internal int mCurrentConstraintNumber = -1; // default
		private SparseArray<State> mStateList = new SparseArray<State>();
		private SparseArray<ConstraintSet> mConstraintSetMap = new SparseArray<ConstraintSet>();
		private ConstraintsChangedListener mConstraintsChangedListener = null;

		internal ConstraintLayoutStates(Context context, ConstraintLayout layout, int resourceID)
		{
			mConstraintLayout = layout;
			load(context, resourceID);
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

		public virtual void updateConstraints(int id, float width, float height)
		{
			if (mCurrentStateId == id)
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
				if (mCurrentConstraintNumber != -1)
				{
					if (state.mVariants[mCurrentConstraintNumber].match(width, height))
					{
						return;
					}
				}
				int match = state.findMatch(width, height);
				if (mCurrentConstraintNumber == match)
				{
					return;
				}

				ConstraintSet constraintSet = (match == -1) ? mDefaultConstraintSet : state.mVariants[match].mConstraintSet;
				int cid = (match == -1) ? state.mConstraintID : state.mVariants[match].mConstraintID;
				if (constraintSet == null)
				{
					return;
				}
				mCurrentConstraintNumber = match;
				if (mConstraintsChangedListener != null)
				{
					mConstraintsChangedListener.preLayoutChange(-1, cid);
				}
				constraintSet.applyTo(mConstraintLayout);
				if (mConstraintsChangedListener != null)
				{
					mConstraintsChangedListener.postLayoutChange(-1, cid);
				}

			}
			else
			{
				mCurrentStateId = id;
				State state = mStateList.get(mCurrentStateId);
				int match = state.findMatch(width, height);
				ConstraintSet constraintSet = (match == -1) ? state.mConstraintSet : state.mVariants[match].mConstraintSet;
				int cid = (match == -1) ? state.mConstraintID : state.mVariants[match].mConstraintID;

				if (constraintSet == null)
				{
					Log.v(TAG, "NO Constraint set found ! id=" + id + ", dim =" + width + ", " + height);
					return;
				}
				mCurrentConstraintNumber = match;
				if (mConstraintsChangedListener != null)
				{
					mConstraintsChangedListener.preLayoutChange(id, cid);
				}
				constraintSet.applyTo(mConstraintLayout);
				if (mConstraintsChangedListener != null)
				{
					mConstraintsChangedListener.postLayoutChange(id, cid);
				}
			}

		}

		public virtual ConstraintsChangedListener OnConstraintsChanged
		{
			set
			{
				this.mConstraintsChangedListener = value;
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
			internal ConstraintSet mConstraintSet;

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
							mConstraintSet = new ConstraintSet();
							mConstraintSet.clone(context, mConstraintID);
							if (DEBUG)
							{
								Log.v(TAG, "############### mConstraintSet.load(" + name + ")");
							}
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
			internal ConstraintSet mConstraintSet;

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
							mConstraintSet = new ConstraintSet();
							if (DEBUG)
							{
								Log.v(TAG, "############### mConstraintSet.load(" + name + ")");
							}
							mConstraintSet.clone(context, mConstraintID);
							if (DEBUG)
							{
								Log.v(TAG, "############### mConstraintSet.load(" + name + ")");
							}
						}
						else
						{
							if (DEBUG)
							{
								Log.v(TAG, "############### id -> ConstraintSet should be in this file");
							}

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

		/// <summary>
		/// Load a constraint set from a constraintSet.xml file
		/// </summary>
		/// <param name="context">    the context for the inflation </param>
		/// <param name="resourceId"> mId of xml file in res/xml/ </param>
		private void load(Context context, int resourceId)
		{
			if (DEBUG)
			{
				Log.v(TAG, "############### ");
			}
			Resources res = context.Resources;
			XmlPullParser parser = res.getXml(resourceId);
			string document = null;
			string tagName = null;
			try
			{
				Variant match;
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
								case "layoutDescription":
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
								case "ConstraintSet":
									parseConstraintSet(context, parser);
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
							tagName = null;
							break;
						case XmlResourceParser.TEXT:
							break;
					}
				}
	//            for (Variant sizeMatch : mSizeMatchList) {
	//                if (sizeMatch.mConstraintSet == null) {
	//                    continue;
	//                }
	//                if (sizeMatch.mConstraintID != -1) {
	//                    sizeMatch.mConstraintSet = mConstraintSetMap.get(sizeMatch.mConstraintID);
	//                }
	//            }
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

		private void parseConstraintSet(Context context, XmlPullParser parser)
		{
			ConstraintSet set = new ConstraintSet();
			int count = parser.AttributeCount;
			for (int i = 0; i < count; i++)
			{
				string name = parser.getAttributeName(i);
				string s = parser.getAttributeValue(i);
				if (string.ReferenceEquals(name, null) || string.ReferenceEquals(s, null))
				{
					continue;
				}
				if ("id".Equals(name))
				{
					int id = -1;
					if (s.Contains("/"))
					{
						string tmp = s.Substring(s.IndexOf('/') + 1);
						id = context.Resources.getIdentifier(tmp, "id", context.PackageName);

					}
					if (id == -1)
					{
						if (s.Length > 1)
						{
							id = int.Parse(s.Substring(1));
						}
						else
						{
							Log.e(TAG, "error in parsing id");
						}
					}
					set.load(context, parser);
					if (DEBUG)
					{
						Log.v(TAG, " id name " + context.Resources.getResourceName(id));
					}
					mConstraintSetMap.put(id, set);
					break;
				}
			}
		}

	}

}