using System;

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
	using Build = android.os.Build;
	using View = android.view.View;
	using ViewGroup = android.view.ViewGroup;

	/// <summary>
	///  <b>Added in 2.0</b>
	///  <para>
	///  ConstraintProperties provides an easy to use api to update the layout params
	///  of <seealso cref="ConstraintLayout"/> children
	///  </para>
	/// </summary>
	public class ConstraintProperties
	{
		internal ConstraintLayout.LayoutParams mParams;
		internal View mView;
		/// <summary>
		/// The left side of a view.
		/// </summary>
		public const int LEFT = ConstraintLayout.LayoutParams.LEFT;

		/// <summary>
		/// The right side of a view.
		/// </summary>
		public const int RIGHT = ConstraintLayout.LayoutParams.RIGHT;

		/// <summary>
		/// The top of a view.
		/// </summary>
		public const int TOP = ConstraintLayout.LayoutParams.TOP;

		/// <summary>
		/// The bottom side of a view.
		/// </summary>
		public const int BOTTOM = ConstraintLayout.LayoutParams.BOTTOM;

	/// <summary>
	/// The baseline of the text in a view.
	/// </summary>
		public const int BASELINE = ConstraintLayout.LayoutParams.BASELINE;

		/// <summary>
		/// The left side of a view in left to right languages.
		/// In right to left languages it corresponds to the right side of the view
		/// </summary>
		public const int START = ConstraintLayout.LayoutParams.START;

		/// <summary>
		/// The right side of a view in right to left languages.
		/// In right to left languages it corresponds to the left side of the view
		/// </summary>
		public const int END = ConstraintLayout.LayoutParams.END;
		/// <summary>
		/// Used to indicate a parameter is cleared or not set
		/// </summary>
		public const int UNSET = ConstraintLayout.LayoutParams.UNSET;
		/// <summary>
		/// References the id of the parent.
		/// </summary>
		public const int PARENT_ID = ConstraintLayout.LayoutParams.PARENT_ID;

		/// <summary>
		/// Dimension will be controlled by constraints
		/// </summary>
		public const int MATCH_CONSTRAINT = ConstraintLayout.LayoutParams.MATCH_CONSTRAINT;

		/// <summary>
		/// Dimension will set by the view's content
		/// </summary>
		public static readonly int WRAP_CONTENT = ConstraintLayout.LayoutParams.WRAP_CONTENT;

		/// <summary>
		/// How to calculate the size of a view in 0 dp by using its wrap_content size
		/// </summary>
		public static readonly int MATCH_CONSTRAINT_WRAP = ConstraintLayout.LayoutParams.MATCH_CONSTRAINT_WRAP;

		/// <summary>
		/// Calculate the size of a view in 0 dp by reducing the constrains gaps as much as possible
		/// </summary>
		public static readonly int MATCH_CONSTRAINT_SPREAD = ConstraintLayout.LayoutParams.MATCH_CONSTRAINT_SPREAD;

		/// <summary>
		/// Center view between the other two widgets.
		/// </summary>
		/// <param name="firstID">      ID of the first widget to connect the left or top of the widget to </param>
		/// <param name="firstSide">    the side of the widget to connect to </param>
		/// <param name="firstMargin">  the connection margin </param>
		/// <param name="secondId">     the ID of the second widget to connect to right or top of the widget to </param>
		/// <param name="secondSide">   the side of the widget to connect to </param>
		/// <param name="secondMargin"> the connection margin </param>
		/// <param name="bias">         the ratio between two connections </param>
		/// <returns> this </returns>

		public virtual ConstraintProperties center(int firstID, int firstSide, int firstMargin, int secondId, int secondSide, int secondMargin, float bias)
		{
			// Error checking

			if (firstMargin < 0)
			{
				throw new System.ArgumentException("margin must be > 0");
			}
			if (secondMargin < 0)
			{
				throw new System.ArgumentException("margin must be > 0");
			}
			if (bias <= 0 || bias > 1)
			{
				throw new System.ArgumentException("bias must be between 0 and 1 inclusive");
			}

			if (firstSide == LEFT || firstSide == RIGHT)
			{
				connect(LEFT, firstID, firstSide, firstMargin);
				connect(RIGHT, secondId, secondSide, secondMargin);

				mParams.horizontalBias = bias;
			}
			else if (firstSide == START || firstSide == END)
			{
				connect(START, firstID, firstSide, firstMargin);
				connect(END, secondId, secondSide, secondMargin);

				mParams.horizontalBias = bias;
			}
			else
			{
				connect(TOP, firstID, firstSide, firstMargin);
				connect(BOTTOM, secondId, secondSide, secondMargin);
				mParams.verticalBias = bias;
			}

			return this;
		}

		/// <summary>
		/// Centers the widget horizontally to the left and right side on another widgets sides.
		/// </summary>
		/// <param name="leftId">      The Id of the widget on the left side </param>
		/// <param name="leftSide">    The side of the leftId widget to connect to </param>
		/// <param name="leftMargin">  The margin on the left side </param>
		/// <param name="rightId">     The Id of the widget on the right side </param>
		/// <param name="rightSide">   The side  of the rightId widget to connect to </param>
		/// <param name="rightMargin"> The margin on the right side </param>
		/// <param name="bias">        The ratio of the space on the left vs. right sides 0.5 is centered (default) </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties centerHorizontally(int leftId, int leftSide, int leftMargin, int rightId, int rightSide, int rightMargin, float bias)
		{
			connect(LEFT, leftId, leftSide, leftMargin);
			connect(RIGHT, rightId, rightSide, rightMargin);
			mParams.horizontalBias = bias;
			return this;
		}

		/// <summary>
		/// Centers the widgets horizontally to the left and right side on another widgets sides.
		/// </summary>
		/// <param name="startId">     The Id of the widget on the start side (left in non rtl languages) </param>
		/// <param name="startSide">   The side of the startId widget to connect to </param>
		/// <param name="startMargin"> The margin on the start side </param>
		/// <param name="endId">       The Id of the widget on the start side (left in non rtl languages) </param>
		/// <param name="endSide">     The side of the endId widget to connect to </param>
		/// <param name="endMargin">   The margin on the end side </param>
		/// <param name="bias">        The ratio of the space on the start vs end side 0.5 is centered (default) </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties centerHorizontallyRtl(int startId, int startSide, int startMargin, int endId, int endSide, int endMargin, float bias)
		{
			connect(START, startId, startSide, startMargin);
			connect(END, endId, endSide, endMargin);
			mParams.horizontalBias = bias;
			return this;
		}

		/// <summary>
		/// Centers the widgets Vertically to the top and bottom side on another widgets sides.
		/// </summary>
		/// <param name="topId">        The Id of the widget on the top side </param>
		/// <param name="topSide">      The side of the leftId widget to connect to </param>
		/// <param name="topMargin">    The margin on the top side </param>
		/// <param name="bottomId">     The Id of the widget on the bottom side </param>
		/// <param name="bottomSide">   The side of the bottomId widget to connect to </param>
		/// <param name="bottomMargin"> The margin on the bottom side </param>
		/// <param name="bias">         The ratio of the space on the top vs. bottom sides 0.5 is centered (default) </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties centerVertically(int topId, int topSide, int topMargin, int bottomId, int bottomSide, int bottomMargin, float bias)
		{
			connect(TOP, topId, topSide, topMargin);
			connect(BOTTOM, bottomId, bottomSide, bottomMargin);
			mParams.verticalBias = bias;
			return this;
		}

		/// <summary>
		/// Centers the view horizontally relative to toView's position.
		/// </summary>
		/// <param name="toView"> ID of view to center on (or in) </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties centerHorizontally(int toView)
		{
			if (toView == PARENT_ID)
			{
				center(PARENT_ID, ConstraintSet.LEFT, 0, PARENT_ID, ConstraintSet.RIGHT, 0, 0.5f);
			}
			else
			{
				center(toView, ConstraintSet.RIGHT, 0, toView, ConstraintSet.LEFT, 0, 0.5f);
			}
			return this;
		}

		/// <summary>
		/// Centers the view horizontally relative to toView's position.
		/// </summary>
		/// <param name="toView"> ID of view to center on (or in) </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties centerHorizontallyRtl(int toView)
		{
			if (toView == PARENT_ID)
			{
				center(PARENT_ID, ConstraintSet.START, 0, PARENT_ID, ConstraintSet.END, 0, 0.5f);
			}
			else
			{
				center(toView, ConstraintSet.END, 0, toView, ConstraintSet.START, 0, 0.5f);
			}
			return this;
		}

		/// <summary>
		/// Centers the view vertically relative to toView's position.
		/// </summary>
		/// <param name="toView"> ID of view to center on (or in) </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties centerVertically(int toView)
		{
			if (toView == PARENT_ID)
			{
				center(PARENT_ID, ConstraintSet.TOP, 0, PARENT_ID, ConstraintSet.BOTTOM, 0, 0.5f);
			}
			else
			{
				center(toView, ConstraintSet.BOTTOM, 0, toView, ConstraintSet.TOP, 0, 0.5f);
			}
			return this;
		}

		/// <summary>
		/// Remove a constraint from this view.
		/// </summary>
		/// <param name="anchor"> the Anchor to remove constraint from </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties removeConstraints(int anchor)
		{
			switch (anchor)
			{
				case LEFT:
					mParams.leftToRight = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.leftToLeft = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.leftMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.goneLeftMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.GONE_UNSET;
					break;
				case RIGHT:
					mParams.rightToRight = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.rightToLeft = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.rightMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.goneRightMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.GONE_UNSET;
					break;
				case TOP:
					mParams.topToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.topToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.topMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.goneTopMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.GONE_UNSET;
					break;
				case BOTTOM:
					mParams.bottomToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.bottomToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.bottomMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.goneBottomMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.GONE_UNSET;
					break;
				case BASELINE:
					mParams.baselineToBaseline = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					break;
				case START:
					mParams.startToEnd = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.startToStart = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.MarginStart = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.goneStartMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.GONE_UNSET;
					break;
				case END:
					mParams.endToStart = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.endToEnd = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.MarginEnd = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					mParams.goneEndMargin = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.GONE_UNSET;
					break;
				default:
					throw new System.ArgumentException("unknown constraint");
			}
			return this;
		}

		/// <summary>
		/// Sets the margin.
		/// </summary>
		/// <param name="anchor"> The side to adjust the margin on </param>
		/// <param name="value">  The new value for the margin </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties margin(int anchor, int value)
		{
			switch (anchor)
			{
				case LEFT:
					mParams.leftMargin = value;
					break;
				case RIGHT:
					mParams.rightMargin = value;
					break;
				case TOP:
					mParams.topMargin = value;
					break;
				case BOTTOM:
					mParams.bottomMargin = value;
					break;
				case BASELINE:
					throw new System.ArgumentException("baseline does not support margins");
				case START:
					mParams.MarginStart = value;
					break;
				case END:
					mParams.MarginEnd = value;
					break;
				default:
					throw new System.ArgumentException("unknown constraint");
			}
			return this;
		}

		/// <summary>
		/// Sets the gone margin.
		/// </summary>
		/// <param name="anchor"> The side to adjust the margin on </param>
		/// <param name="value">  The new value for the margin </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties goneMargin(int anchor, int value)
		{
			switch (anchor)
			{
				case LEFT:
					mParams.goneLeftMargin = value;
					break;
				case RIGHT:
					mParams.goneRightMargin = value;
					break;
				case TOP:
					mParams.goneTopMargin = value;
					break;
				case BOTTOM:
					mParams.goneBottomMargin = value;
					break;
				case BASELINE:
					throw new System.ArgumentException("baseline does not support margins");
				case START:
					mParams.goneStartMargin = value;
					break;
				case END:
					mParams.goneEndMargin = value;
					break;
				default:
					throw new System.ArgumentException("unknown constraint");
			}
			return this;
		}

		/// <summary>
		/// Adjust the horizontal bias of the view (used with views constrained on left and right).
		/// </summary>
		/// <param name="bias"> the new bias 0.5 is in the middle </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties horizontalBias(float bias)
		{
			mParams.horizontalBias = bias;
			return this;
		}

		/// <summary>
		/// Adjust the vertical bias of the view (used with views constrained on left and right).
		/// </summary>
		/// <param name="bias"> the new bias 0.5 is in the middle </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties verticalBias(float bias)
		{
			mParams.verticalBias = bias;
			return this;
		}

		/// <summary>
		/// Constrains the views aspect ratio.
		/// For Example a HD screen is 16 by 9 = 16/(float)9 = 1.777f.
		/// </summary>
		/// <param name="ratio"> The ratio of the width to height (width / height) </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties dimensionRatio(string ratio)
		{
			mParams.dimensionRatio = ratio;
			return this;
		}

		/// <summary>
		/// Adjust the visibility of a view.
		/// </summary>
		/// <param name="visibility"> the visibility (View.VISIBLE, View.INVISIBLE, View.GONE) </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties visibility(int visibility)
		{
			mView.Visibility = visibility;
			return this;
		}

		/// <summary>
		/// Adjust the alpha of a view.
		/// </summary>
		/// <param name="alpha"> the alpha </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties alpha(float alpha)
		{
			mView.Alpha = alpha;
			return this;
		}

		/// <summary>
		/// Set the elevation of a view.
		/// </summary>
		/// <param name="elevation"> the elevation </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties elevation(float elevation)
		{
			if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
			{
				mView.Elevation = elevation;
			}
			return this;
		}

		/// <summary>
		/// Adjust the post-layout rotation about the Z axis of a view.
		/// </summary>
		/// <param name="rotation"> the rotation about the Z axis </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties rotation(float rotation)
		{
			mView.Rotation = rotation;
			return this;
		}

		/// <summary>
		/// Adjust the post-layout rotation about the X axis of a view.
		/// </summary>
		/// <param name="rotationX"> the rotation about the X axis </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties rotationX(float rotationX)
		{
			mView.RotationX = rotationX;
			return this;
		}

		/// <summary>
		/// Adjust the post-layout rotation about the Y axis of a view.
		/// </summary>
		/// <param name="rotationY"> the rotation about the Y axis </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties rotationY(float rotationY)
		{
			mView.RotationY = rotationY;
			return this;
		}

		/// <summary>
		/// Adjust the post-layout scale in X of a view.
		/// </summary>
		/// <param name="scaleX"> the scale in X </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties scaleX(float scaleX)
		{
			mView.ScaleY = scaleX;
			return this;
		}

		/// <summary>
		/// Adjust the post-layout scale in Y of a view.
		/// </summary>
		/// <param name="scaleY"> the scale in Y </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties scaleY(float scaleY)
		{
			return this;
		}

		/// <summary>
		/// Set X location of the pivot point around which the view will rotate and scale.
		/// </summary>
		/// <param name="transformPivotX"> X location of the pivot point. </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties transformPivotX(float transformPivotX)
		{
			mView.PivotX = transformPivotX;
			return this;
		}

		/// <summary>
		/// Set Y location of the pivot point around which the view will rotate and scale.
		/// </summary>
		/// <param name="transformPivotY"> Y location of the pivot point. </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties transformPivotY(float transformPivotY)
		{
			mView.PivotY = transformPivotY;
			return this;
		}

		/// <summary>
		/// Set X and Y location of the pivot point around which the view will rotate and scale.
		/// </summary>
		/// <param name="transformPivotX"> X location of the pivot point. </param>
		/// <param name="transformPivotY"> Y location of the pivot point. </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties transformPivot(float transformPivotX, float transformPivotY)
		{
			mView.PivotX = transformPivotX;
			mView.PivotY = transformPivotY;
			return this;
		}

		/// <summary>
		/// Adjust the post-layout X translation of a view.
		/// </summary>
		/// <param name="translationX"> the translation in X </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties translationX(float translationX)
		{
			mView.TranslationX = translationX;
			return this;
		}

		/// <summary>
		/// Adjust the  post-layout Y translation of a view.
		/// </summary>
		/// <param name="translationY"> the translation in Y </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties translationY(float translationY)
		{
			mView.TranslationY = translationY;
			return this;
		}

		/// <summary>
		/// Adjust the  post-layout X and Y translation of a view.
		/// </summary>
		/// <param name="translationX"> the translation in X </param>
		/// <param name="translationY"> the translation in Y </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties translation(float translationX, float translationY)
		{
			mView.TranslationX = translationX;
			mView.TranslationY = translationY;
			return this;
		}

		/// <summary>
		/// Adjust the post-layout translation in Z of a view. This is the preferred way to adjust the shadow.
		/// </summary>
		/// <param name="translationZ"> the translationZ </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties translationZ(float translationZ)
		{
			if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP)
			{
				mView.TranslationZ = translationZ;
			}
			return this;
		}

		/// <summary>
		/// Sets the height of the view.
		/// </summary>
		/// <param name="height"> the height of the view </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties constrainHeight(int height)
		{
			mParams.height = height;
			return this;
		}

		/// <summary>
		/// Sets the width of the view.
		/// </summary>
		/// <param name="width"> the width of the view </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties constrainWidth(int width)
		{
			mParams.width = width;
			return this;
		}

		/// <summary>
		/// Sets the maximum height of the view. It is a dimension, It is only applicable if height is
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="height"> the maximum height of the view </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties constrainMaxHeight(int height)
		{
			mParams.matchConstraintMaxHeight = height;
			return this;
		}

		/// <summary>
		/// Sets the maximum width of the view. It is a dimension, It is only applicable if height is
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="width"> the maximum width of the view </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties constrainMaxWidth(int width)
		{
			mParams.matchConstraintMaxWidth = width;
			return this;
		}

		/// <summary>
		/// Sets the minimum height of the view. It is a dimension, It is only applicable if height is
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="height"> the minimum height of the view </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties constrainMinHeight(int height)
		{
			mParams.matchConstraintMinHeight = height;
			return this;
		}

		/// <summary>
		/// Sets the minimum width of the view. It is a dimension, It is only applicable if height is
		/// #MATCH_CONSTRAINT}.
		/// </summary>
		/// <param name="width"> the minimum width of the view </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties constrainMinWidth(int width)
		{
			mParams.matchConstraintMinWidth = width;
			return this;
		}

		/// <summary>
		/// Sets how the height is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
		/// Default is spread.
		/// </summary>
		/// <param name="height"> MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties constrainDefaultHeight(int height)
		{
			mParams.matchConstraintDefaultHeight = height;
			return this;
		}

		/// <summary>
		/// Sets how the width is calculated ether MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD.
		/// Default is spread.
		/// </summary>
		/// <param name="width"> MATCH_CONSTRAINT_WRAP or MATCH_CONSTRAINT_SPREAD </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties constrainDefaultWidth(int width)
		{
			mParams.matchConstraintDefaultWidth = width;
			return this;
		}

		/// <summary>
		/// The child's weight that we can use to distribute the available horizontal space
		/// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
		/// </summary>
		/// <param name="weight"> the weight that we can use to distribute the horizontal space </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties horizontalWeight(float weight)
		{
			mParams.horizontalWeight = weight;
			return this;
		}

		/// <summary>
		/// The child's weight that we can use to distribute the available vertical space
		/// in a chain, if the dimension behaviour is set to MATCH_CONSTRAINT
		/// </summary>
		/// <param name="weight"> the weight that we can use to distribute the vertical space </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties verticalWeight(float weight)
		{
			mParams.verticalWeight = weight;
			return this;
		}

		/// <summary>
		/// How the elements of the horizontal chain will be positioned. If the dimension
		/// behaviour is set to MATCH_CONSTRAINT. The possible values are:
		/// <para>
		/// <ul>
		///   <li>CHAIN_SPREAD -- the elements will be spread out</li>
		///   <li>CHAIN_SPREAD_INSIDE -- similar, but the endpoints of the chain will not be spread out</li>
		///   <li>CHAIN_PACKED -- the elements of the chain will be packed together. The horizontal
		/// bias attribute of the child will then affect the positioning of the packed elements</li>
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="chainStyle"> the weight that we can use to distribute the horizontal space </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties horizontalChainStyle(int chainStyle)
		{
			mParams.horizontalChainStyle = chainStyle;
			return this;
		}

		/// <summary>
		/// How the elements of the vertical chain will be positioned. in a chain, if the dimension
		/// behaviour is set to MATCH_CONSTRAINT
		/// <para>
		/// <ul>
		///   <li>CHAIN_SPREAD -- the elements will be spread out</li>
		///   <li>CHAIN_SPREAD_INSIDE -- similar, but the endpoints of the chain will not be spread out</li>
		///   <li>CHAIN_PACKED -- the elements of the chain will be packed together. The horizontal
		/// bias attribute of the child will then affect the positioning of the packed elements</li>
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <param name="chainStyle"> the weight that we can use to distribute the horizontal space </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties verticalChainStyle(int chainStyle)
		{
			mParams.verticalChainStyle = chainStyle;
			return this;
		}

		/// <summary>
		/// Adds the view to a horizontal chain.
		/// </summary>
		/// <param name="leftId">  id of the view in chain to the left </param>
		/// <param name="rightId"> id of the view in chain to the right </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties addToHorizontalChain(int leftId, int rightId)
		{
			connect(LEFT, leftId, (leftId == PARENT_ID) ? LEFT : RIGHT, 0);
			connect(RIGHT, rightId, (rightId == PARENT_ID) ? RIGHT : LEFT, 0);
			if (leftId != PARENT_ID)
			{
				View leftView = ((ViewGroup)(mView.Parent)).findViewById(leftId);
				ConstraintProperties leftProp = new ConstraintProperties(leftView);
				leftProp.connect(RIGHT, mView.Id, LEFT, 0);
			}
			if (rightId != PARENT_ID)
			{
				View rightView = ((ViewGroup)(mView.Parent)).findViewById(rightId);
				ConstraintProperties rightProp = new ConstraintProperties(rightView);
				rightProp.connect(LEFT, mView.Id, RIGHT, 0);
			}
			return this;
		}

		/// <summary>
		/// Adds the view to a horizontal chain using RTL attributes.
		/// </summary>
		/// <param name="leftId">  id of the view in chain to the left </param>
		/// <param name="rightId"> id of the view in chain to the right </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties addToHorizontalChainRTL(int leftId, int rightId)
		{
			connect(START, leftId, (leftId == PARENT_ID) ? START : END, 0);
			connect(END, rightId, (rightId == PARENT_ID) ? END : START, 0);
			if (leftId != PARENT_ID)
			{
				View leftView = ((ViewGroup)(mView.Parent)).findViewById(leftId);
				ConstraintProperties leftProp = new ConstraintProperties(leftView);
				leftProp.connect(END, mView.Id, START, 0);
			}
			if (rightId != PARENT_ID)
			{
				View rightView = ((ViewGroup)(mView.Parent)).findViewById(rightId);
				ConstraintProperties rightProp = new ConstraintProperties(rightView);
				rightProp.connect(START, mView.Id, END, 0);
			}
			return this;
		}

		/// <summary>
		/// Adds a view to a vertical chain.
		/// </summary>
		/// <param name="topId">    view above. </param>
		/// <param name="bottomId"> view below </param>
		/// <returns> this </returns>
		public virtual ConstraintProperties addToVerticalChain(int topId, int bottomId)
		{
			connect(TOP, topId, (topId == PARENT_ID) ? TOP : BOTTOM, 0);
			connect(BOTTOM, bottomId, (bottomId == PARENT_ID) ? BOTTOM : TOP, 0);
			if (topId != PARENT_ID)
			{
				View topView = ((ViewGroup)(mView.Parent)).findViewById(topId);
				ConstraintProperties topProp = new ConstraintProperties(topView);
				topProp.connect(BOTTOM, mView.Id, TOP, 0);
			}
			if (bottomId != PARENT_ID)
			{
				View bottomView = ((ViewGroup)(mView.Parent)).findViewById(bottomId);
				ConstraintProperties bottomProp = new ConstraintProperties(bottomView);
				bottomProp.connect(TOP, mView.Id, BOTTOM, 0);
			}
			return this;
		}

		/// <summary>
		/// Removes a view from a vertical chain.
		/// This assumes the view is connected to a vertical chain.
		/// Its behaviour is undefined if not part of a vertical chain.
		/// </summary>
		/// <returns> this </returns>
		public virtual ConstraintProperties removeFromVerticalChain()
		{
			int topId = mParams.topToBottom;
			int bottomId = mParams.bottomToTop;
			if (topId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET || bottomId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
			{
				View topView = ((ViewGroup)(mView.Parent)).findViewById(topId);
				ConstraintProperties topProp = new ConstraintProperties(topView);
				View bottomView = ((ViewGroup)(mView.Parent)).findViewById(bottomId);
				ConstraintProperties bottomProp = new ConstraintProperties(bottomView);
				if (topId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET && bottomId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
				{
					// top and bottom connected to views
					topProp.connect(BOTTOM, bottomId, TOP, 0);
					bottomProp.connect(TOP, topId, BOTTOM, 0);
				}
				else if (topId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET || bottomId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
				{
					if (mParams.bottomToBottom != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
					{
						// top connected to view. Bottom connected to parent
						topProp.connect(BOTTOM, mParams.bottomToBottom, BOTTOM, 0);
					}
					else if (mParams.topToTop != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
					{
						// bottom connected to view. Top connected to parent
						bottomProp.connect(TOP, mParams.topToTop, TOP, 0);
					}
				}
			}

			removeConstraints(TOP);
			removeConstraints(BOTTOM);
			return this;
		}

		/// <summary>
		/// Removes a view from a vertical chain.
		/// This assumes the view is connected to a vertical chain.
		/// Its behaviour is undefined if not part of a vertical chain.
		/// </summary>
		/// <returns> this </returns>
		public virtual ConstraintProperties removeFromHorizontalChain()
		{
			int leftId = mParams.leftToRight;
			int rightId = mParams.rightToLeft;

			if (leftId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET || rightId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
			{
				View leftView = ((ViewGroup)(mView.Parent)).findViewById(leftId);
				ConstraintProperties leftProp = new ConstraintProperties(leftView);
				View rightView = ((ViewGroup)(mView.Parent)).findViewById(rightId);
				ConstraintProperties rightProp = new ConstraintProperties(rightView);
				if (leftId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET && rightId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
				{
					// left and right connected to views
					leftProp.connect(RIGHT, rightId, LEFT, 0);
					rightProp.connect(LEFT, leftId, RIGHT, 0);
				}
				else if (leftId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET || rightId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
				{
					if (mParams.rightToRight != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
					{
						// left connected to view. right connected to parent
						leftProp.connect(RIGHT, mParams.rightToRight, RIGHT, 0);
					}
					else if (mParams.leftToLeft != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
					{
						// right connected to view. left connected to parent
						rightProp.connect(LEFT, mParams.leftToLeft, LEFT, 0);
					}
				}
				removeConstraints(LEFT);
				removeConstraints(RIGHT);
			}
			else
			{

				int startId = mParams.startToEnd;
				int endId = mParams.endToStart;
				if (startId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET || endId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
				{
					View startView = ((ViewGroup)(mView.Parent)).findViewById(startId);
					ConstraintProperties startProp = new ConstraintProperties(startView);
					View endView = ((ViewGroup)(mView.Parent)).findViewById(endId);
					ConstraintProperties endProp = new ConstraintProperties(endView);

					if (startId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET && endId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
					{
						// start and end connected to views
						startProp.connect(END, endId, START, 0);
						endProp.connect(START, leftId, END, 0);
					}
					else if (leftId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET || endId != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
					{
						if (mParams.rightToRight != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
						{
							// left connected to view. right connected to parent
							startProp.connect(END, mParams.rightToRight, END, 0);
						}
						else if (mParams.leftToLeft != androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET)
						{
							// right connected to view. left connected to parent
							endProp.connect(START, mParams.leftToLeft, START, 0);
						}
					}
				}
				removeConstraints(START);
				removeConstraints(END);
			}
			return this;
		}

		/// <summary>
		/// Create a constraint between two widgets.
		/// </summary>
		/// <param name="startSide"> the side of the widget to constrain </param>
		/// <param name="endID">     the id of the widget to constrain to </param>
		/// <param name="endSide">   the side of widget to constrain to </param>
		/// <param name="margin">    the margin to constrain (margin must be positive) </param>
		public virtual ConstraintProperties connect(int startSide, int endID, int endSide, int margin)
		{

			switch (startSide)
			{
				case LEFT:
					if (endSide == LEFT)
					{
						mParams.leftToLeft = endID;
						mParams.leftToRight = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else if (endSide == RIGHT)
					{
						mParams.leftToRight = endID;
						mParams.leftToLeft = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;

					}
					else
					{
						throw new System.ArgumentException("Left to " + sideToString(endSide) + " undefined");
					}
					mParams.leftMargin = margin;
					break;
				case RIGHT:
					if (endSide == LEFT)
					{
						mParams.rightToLeft = endID;
						mParams.rightToRight = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;

					}
					else if (endSide == RIGHT)
					{
						mParams.rightToRight = endID;
						mParams.rightToLeft = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;

					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					mParams.rightMargin = margin;
					break;
				case TOP:
					if (endSide == TOP)
					{
						mParams.topToTop = endID;
						mParams.topToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToBaseline = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else if (endSide == BOTTOM)
					{
						mParams.topToBottom = endID;
						mParams.topToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToBaseline = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					mParams.topMargin = margin;
					break;
				case BOTTOM:
					if (endSide == BOTTOM)
					{
						mParams.bottomToBottom = endID;
						mParams.bottomToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToBaseline = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else if (endSide == TOP)
					{
						mParams.bottomToTop = endID;
						mParams.bottomToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToBaseline = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.baselineToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					mParams.bottomMargin = margin;
					break;
				case BASELINE:
					if (endSide == BASELINE)
					{
						mParams.baselineToBaseline = endID;
						mParams.bottomToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.bottomToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.topToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.topToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					if (endSide == TOP)
					{
						mParams.baselineToTop = endID;
						mParams.bottomToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.bottomToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.topToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.topToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else if (endSide == BOTTOM)
					{
						mParams.baselineToBottom = endID;
						mParams.bottomToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.bottomToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.topToTop = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
						mParams.topToBottom = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					mParams.baselineMargin = margin;
					break;
				case START:
					if (endSide == START)
					{
						mParams.startToStart = endID;
						mParams.startToEnd = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else if (endSide == END)
					{
						mParams.startToEnd = endID;
						mParams.startToStart = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
					{
						mParams.MarginStart = margin;
					}
					break;
				case END:
					if (endSide == END)
					{
						mParams.endToEnd = endID;
						mParams.endToStart = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else if (endSide == START)
					{
						mParams.endToStart = endID;
						mParams.endToEnd = androidx.constraintlayout.widget.ConstraintLayout.LayoutParams.UNSET;
					}
					else
					{
						throw new System.ArgumentException("right to " + sideToString(endSide) + " undefined");
					}
					if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1)
					{

						mParams.MarginEnd = margin;
					}

					break;
				default:
					throw new System.ArgumentException(sideToString(startSide) + " to " + sideToString(endSide) + " unknown");
			}
			return this;
		}

		private string sideToString(int side)
		{
			switch (side)
			{
				case LEFT:
					return "left";
				case RIGHT:
					return "right";
				case TOP:
					return "top";
				case BOTTOM:
					return "bottom";
				case BASELINE:
					return "baseline";
				case START:
					return "start";
				case END:
					return "end";
			}
			return "undefined";
		}

		public ConstraintProperties(View view)
		{
			ViewGroup.LayoutParams @params = view.LayoutParams;
			if (@params is ConstraintLayout.LayoutParams)
			{
				mParams = (ConstraintLayout.LayoutParams) @params;
				mView = view;
			}
			else
			{
				throw new Exception("Only children of ConstraintLayout.LayoutParams supported");
			}
		}

		public virtual void apply()
		{
		}
	}

}