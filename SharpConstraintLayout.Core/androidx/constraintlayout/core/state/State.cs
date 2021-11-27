using System;
using System.Collections.Generic;

/*
 * Copyright (C) 2019 The Android Open Source Project
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

namespace androidx.constraintlayout.core.state
{
	using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
	using ConstraintWidgetContainer = androidx.constraintlayout.core.widgets.ConstraintWidgetContainer;
	using HelperWidget = androidx.constraintlayout.core.widgets.HelperWidget;
	using AlignHorizontallyReference = androidx.constraintlayout.core.state.helpers.AlignHorizontallyReference;
	using AlignVerticallyReference = androidx.constraintlayout.core.state.helpers.AlignVerticallyReference;
	using BarrierReference = androidx.constraintlayout.core.state.helpers.BarrierReference;
	using GuidelineReference = androidx.constraintlayout.core.state.helpers.GuidelineReference;
	using VerticalChainReference = androidx.constraintlayout.core.state.helpers.VerticalChainReference;
	using HorizontalChainReference = androidx.constraintlayout.core.state.helpers.HorizontalChainReference;


	/// <summary>
	/// Represents a full state of a ConstraintLayout
	/// </summary>
	public class State
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			mParent = new ConstraintReference(this);
		}


		protected internal Dictionary<object, Reference> mReferences = new Dictionary<object, Reference>();
		protected internal Dictionary<object, HelperReference> mHelperReferences = new Dictionary<object, HelperReference>();
		internal Dictionary<string, List<string>> mTags = new Dictionary<string, List<string>>();

		internal const int UNKNOWN = -1;
		internal const int CONSTRAINT_SPREAD = 0;
		internal const int CONSTRAINT_WRAP = 1;
		internal const int CONSTRAINT_RATIO = 2;

		public const int PARENT = 0;

		public ConstraintReference mParent;

		public enum Constraint
		{
			LEFT_TO_LEFT,
			LEFT_TO_RIGHT,
			RIGHT_TO_LEFT,
			RIGHT_TO_RIGHT,
			START_TO_START,
			START_TO_END,
			END_TO_START,
			END_TO_END,
			TOP_TO_TOP,
			TOP_TO_BOTTOM,
			BOTTOM_TO_TOP,
			BOTTOM_TO_BOTTOM,
			BASELINE_TO_BASELINE,
			CENTER_HORIZONTALLY,
			CENTER_VERTICALLY,
			CIRCULAR_CONSTRAINT,
		}

		public enum Direction
		{
			LEFT,
			RIGHT,
			START,
			END,
			TOP,
			BOTTOM
		}

		public enum Helper
		{
			HORIZONTAL_CHAIN,
			VERTICAL_CHAIN,
			ALIGN_HORIZONTALLY,
			ALIGN_VERTICALLY,
			BARRIER,
			LAYER,
			FLOW
		}

		public enum Chain
		{
			SPREAD,
			SPREAD_INSIDE,
			PACKED
		}

		public State()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			mReferences[PARENT] = mParent;
		}

		public virtual void reset()
		{
			mHelperReferences.Clear();
			mTags.Clear();
		}

		/// <summary>
		/// Implements a conversion function for values, returning int.
		/// This can be used in case values (e.g. margins) are represented
		/// via an object, not directly an int.
		/// </summary>
		/// <param name="value"> the object to convert from
		/// @return </param>
		internal virtual int convertDimension(object value)
		{
			if (value is float?)
			{
				//return ((Float) value).Value;
				return (int)((float?)value).Value;
			}
			if (value is int?)
			{
				return ((int?)value).Value;
			}
			return 0;
		}

		/// <summary>
		/// Create a new reference given a key.
		/// </summary>
		/// <param name="key">
		/// @return </param>
		public virtual ConstraintReference createConstraintReference(object key)
		{
			return new ConstraintReference(this);
		}

		public virtual bool sameFixedWidth(int width)
		{
			return mParent.Width.equalsFixedValue(width);
		}

		public virtual bool sameFixedHeight(int height)
		{
			return mParent.Height.equalsFixedValue(height);
		}

		public virtual State width(Dimension dimension)
		{
			return setWidth(dimension);
		}

		public virtual State height(Dimension dimension)
		{
			return setHeight(dimension);
		}

		public virtual State setWidth(Dimension dimension)
		{
			mParent.setWidth(dimension);
			return this;
		}

		public virtual State setHeight(Dimension dimension)
		{
			mParent.setHeight(dimension);
			return this;
		}

		internal virtual Reference reference(object key)
		{
			return mReferences[key];
		}

		public virtual ConstraintReference constraints(object key)
		{
			Reference reference = mReferences.ContainsKey(key)? mReferences[key]:null;
			if (reference == null)
			{
				reference = createConstraintReference(key);
				mReferences[key] = reference;
				reference.Key = key;
			}
			if (reference is ConstraintReference)
			{
				return (ConstraintReference) reference;
			}
			return null;
		}

		private int numHelpers = 0;
		private string createHelperKey()
		{
			return "__HELPER_KEY_" + numHelpers++ + "__";
		}

		public virtual HelperReference helper(object key, State.Helper type)
		{
			if (key == null)
			{
				key = createHelperKey();
			}
			HelperReference reference = mHelperReferences[key];
			if (reference == null)
			{
				switch (type)
				{
					case androidx.constraintlayout.core.state.State.Helper.HORIZONTAL_CHAIN:
					{
						reference = new HorizontalChainReference(this);
					}
					break;
					case androidx.constraintlayout.core.state.State.Helper.VERTICAL_CHAIN:
					{
						reference = new VerticalChainReference(this);
					}
					break;
					case androidx.constraintlayout.core.state.State.Helper.ALIGN_HORIZONTALLY:
					{
						reference = new AlignHorizontallyReference(this);
					}
					break;
					case androidx.constraintlayout.core.state.State.Helper.ALIGN_VERTICALLY:
					{
						reference = new AlignVerticallyReference(this);
					}
					break;
					case androidx.constraintlayout.core.state.State.Helper.BARRIER:
					{
						reference = new BarrierReference(this);
					}
					break;
					default:
					{
						reference = new HelperReference(this, type);
					}
				break;
				}
				mHelperReferences[key] = reference;
			}
			return reference;
		}

		public virtual GuidelineReference horizontalGuideline(object key)
		{
			return guideline(key, ConstraintWidget.HORIZONTAL);
		}

		public virtual GuidelineReference verticalGuideline(object key)
		{
			return guideline(key, ConstraintWidget.VERTICAL);
		}

		public virtual GuidelineReference guideline(object key, int orientation)
		{
			ConstraintReference reference = constraints(key);
			if (reference.Facade == null || !(reference.Facade is GuidelineReference))
			{
				GuidelineReference guidelineReference = new GuidelineReference(this);
				guidelineReference.Orientation = orientation;
				guidelineReference.Key = key;
				reference.Facade = guidelineReference;
			}
			return (GuidelineReference) reference.Facade;
		}

		public virtual BarrierReference barrier(object key, Direction direction)
		{
			ConstraintReference reference = constraints(key);
			if (reference.Facade == null || !(reference.Facade is BarrierReference))
			{
				BarrierReference barrierReference = new BarrierReference(this);
				barrierReference.BarrierDirection = direction;
				reference.Facade = barrierReference;
			}
			return (BarrierReference) reference.Facade;
		}

		public virtual VerticalChainReference verticalChain()
		{
			return (VerticalChainReference) helper(null, Helper.VERTICAL_CHAIN);
		}

		public virtual VerticalChainReference verticalChain(params object[] references)
		{
			VerticalChainReference reference = (VerticalChainReference) helper(null, State.Helper.VERTICAL_CHAIN);
			reference.add(references);
			return reference;
		}

		public virtual HorizontalChainReference horizontalChain()
		{
			return (HorizontalChainReference) helper(null, Helper.HORIZONTAL_CHAIN);
		}

		public virtual HorizontalChainReference horizontalChain(params object[] references)
		{
			HorizontalChainReference reference = (HorizontalChainReference) helper(null, Helper.HORIZONTAL_CHAIN);
			reference.add(references);
			return reference;
		}

		public virtual AlignHorizontallyReference centerHorizontally(params object[] references)
		{
			AlignHorizontallyReference reference = (AlignHorizontallyReference) helper(null, Helper.ALIGN_HORIZONTALLY);
			reference.add(references);
			return reference;
		}

		public virtual AlignVerticallyReference centerVertically(params object[] references)
		{
			AlignVerticallyReference reference = (AlignVerticallyReference) helper(null, Helper.ALIGN_VERTICALLY);
			reference.add(references);
			return reference;
		}

		public virtual void directMapping()
		{
			foreach (object key in mReferences.Keys)
			{
				Reference @ref = constraints(key);
				if (!(@ref is ConstraintReference))
				{
					continue;
				}
				ConstraintReference reference = (ConstraintReference) @ref;
				reference.View = key;
			}
		}

		public virtual void map(object key, object view)
		{
			Reference @ref = constraints(key);
			if (@ref is ConstraintReference)
			{
				ConstraintReference reference = (ConstraintReference) @ref;
				reference.View = view;
			}
		}

		public virtual void setTag(string key, string tag)
		{
			Reference @ref = constraints(key);
			if (@ref is ConstraintReference)
			{
				ConstraintReference reference = (ConstraintReference) @ref;
				reference.Tag = tag;
				List<string> list = null;
				if (!mTags.ContainsKey(tag))
				{
					list = new List<string>();
					mTags[tag] = list;
				}
				else
				{
					list = mTags[tag];
				}
				list.Add(key);
			}
		}

		public virtual List<string> getIdsForTag(string tag)
		{
			if (mTags.ContainsKey(tag))
			{
				return mTags[tag];
			}
			return null;
		}

		public virtual void apply(ConstraintWidgetContainer container)
		{
			container.removeAllChildren();
			mParent.Width.apply(this, container, ConstraintWidget.HORIZONTAL);
			mParent.Height.apply(this, container, ConstraintWidget.VERTICAL);
			foreach (object key in mHelperReferences.Keys)
			{
				HelperReference reference = mHelperReferences[key];
				HelperWidget helperWidget = reference.HelperWidget;
				if (helperWidget != null)
				{
					Reference constraintReference = mReferences[key];
					if (constraintReference == null)
					{
						constraintReference = constraints(key);
					}
					constraintReference.ConstraintWidget = helperWidget;
				}
			}
			foreach (object key in mReferences.Keys)
			{
				Reference reference = mReferences[key];
				if (reference != mParent && reference.Facade is HelperReference)
				{
					HelperWidget helperWidget = ((HelperReference) reference.Facade).HelperWidget;
					if (helperWidget != null)
					{
						Reference constraintReference = mReferences[key];
						if (constraintReference == null)
						{
							constraintReference = constraints(key);
						}
						constraintReference.ConstraintWidget = helperWidget;
					}
				}
			}
			foreach (object key in mReferences.Keys)
			{
				Reference reference = mReferences[key];
				if (reference != mParent)
				{
					ConstraintWidget widget = reference.ConstraintWidget;
					widget.DebugName = reference.Key.ToString();
					widget.Parent = null;
					if (reference.Facade is GuidelineReference)
					{
						// we apply Guidelines first to correctly setup their ConstraintWidget.
						reference.apply();
					}
					container.add(widget);
				}
				else
				{
					reference.ConstraintWidget = container;
				}
			}
			foreach (object key in mHelperReferences.Keys)
			{
				HelperReference reference = mHelperReferences[key];
				HelperWidget helperWidget = reference.HelperWidget;
				if (helperWidget != null)
				{
					foreach (object keyRef in reference.mReferences)
					{
						Reference constraintReference = mReferences[keyRef];
						reference.HelperWidget.add(constraintReference.ConstraintWidget);
					}
					reference.apply();
				}
				else
				{
					reference.apply();
				}
			}
			foreach (object key in mReferences.Keys)
			{
				Reference reference = mReferences[key];
				if (reference != mParent && reference.Facade is HelperReference)
				{
					HelperReference helperReference = (HelperReference) reference.Facade;
					HelperWidget helperWidget = helperReference.HelperWidget;
					if (helperWidget != null)
					{
						foreach (object keyRef in helperReference.mReferences)
						{
							Reference constraintReference = mReferences[keyRef];
							if (constraintReference != null)
							{
								helperWidget.add(constraintReference.ConstraintWidget);
							}
							else if (keyRef is Reference)
							{
								helperWidget.add(((Reference) keyRef).ConstraintWidget);
							}
							else
							{
								Console.WriteLine("couldn't find reference for " + keyRef);
							}
						}
						reference.apply();
					}
				}
			}
			foreach (object key in mReferences.Keys)
			{
				Reference reference = mReferences[key];
				reference.apply();
				ConstraintWidget widget = reference.ConstraintWidget;
				if (widget != null && key is string)
				{
					widget.stringId = (string) key;
				}
			}
		}
	}

}