/*
 * Copyright (C) 2017 The Android Open Source Project
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
    using SuppressLint = android.annotation.SuppressLint;
    using Context = android.content.Context;
    using TypedArray = android.content.res.TypedArray;
    using Canvas = android.graphics.Canvas;
    using Paint = android.graphics.Paint;
    using Rect = android.graphics.Rect;
    using Typeface = android.graphics.Typeface;
    using ConstraintWidget = androidx.constraintlayout.core.widgets.ConstraintWidget;
    using AttributeSet = android.util.AttributeSet;
    using View = android.view.View;

    /// <summary>
    /// <b>Added in 1.1</b>
    /// <para>
    /// A {@code Placeholder} provides a virtual object which can position an existing object.
    /// </para>
    /// <para>
    /// When the id of another view is set on a placeholder (using {@code setContent()}),
    /// the placeholder effectively becomes the content view. If the content view exist on the
    /// screen it is treated as gone from its original location.
    /// </para>
    /// <para>
    /// The content view is positioned using the layout of the parameters of the {@code Placeholder}  (the {@code Placeholder}
    /// is simply constrained in the layout like any other view).
    /// </para>
    /// 
    /// </summary>
    public class Placeholder : View
    {

      private int mContentId = -1;
      private View mContent = null;
      private int mEmptyVisibility = View.INVISIBLE;

      public Placeholder(Context context) : base(context)
      {
        init(null);
      }

      public Placeholder(Context context, AttributeSet attrs) : base(context, attrs)
      {
        init(attrs);
      }

      public Placeholder(Context context, AttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
      {
        init(attrs);
      }

      public Placeholder(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr)
      {
        init(attrs);
      }

      private void init(AttributeSet attrs)
      {
        base.Visibility = mEmptyVisibility;
        mContentId = -1;
        if (attrs != null)
        {
          TypedArray a = Context.obtainStyledAttributes(attrs, R.styleable.ConstraintLayout_placeholder);
//JAVA TO C# CONVERTER CRACKED BY X-CRACKER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int N = a.getIndexCount();
          int N = a.IndexCount;
          for (int i = 0; i < N; i++)
          {
            int attr = a.getIndex(i);
            if (attr == R.styleable.ConstraintLayout_placeholder_content)
            {
              mContentId = a.getResourceId(attr, mContentId);
            }
            else
            {
              if (attr == R.styleable.ConstraintLayout_placeholder_placeholder_emptyVisibility)
              {
                mEmptyVisibility = a.getInt(attr, mEmptyVisibility);
              }
            }
          }
          a.recycle();
        }
      }

      /// <summary>
      /// Sets the visibility of placeholder when not containing objects typically gone or invisible.
      /// This can be important as it affects behaviour of surrounding components.
      /// </summary>
      /// <param name="visibility"> Either View.VISIBLE, View.INVISIBLE, View.GONE </param>
      public virtual int EmptyVisibility
      {
          set
          {
            mEmptyVisibility = value;
          }
          get
          {
            return mEmptyVisibility;
          }
      }


      /// <summary>
      /// Returns the content view </summary>
      /// <returns> {@code null} if no content is set, otherwise the content view </returns>
      public virtual View Content
      {
          get
          {
            return mContent;
          }
      }

      /// <summary>
      /// Placeholder does not draw anything itself - therefore Paint and Rect allocations
      /// are fine to suppress and ignore.
      /// 
      /// @suppress </summary>
      /// <param name="canvas"> </param>
      public virtual void onDraw(Canvas canvas)
      {
        if (InEditMode)
        {
          canvas.drawRGB(223, 223, 223);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressLint("DrawAllocation") android.graphics.Paint paint = new android.graphics.Paint();
          Paint paint = new Paint();
          paint.setARGB(255, 210, 210, 210);
          paint.TextAlign = Paint.Align.CENTER;
          paint.Typeface = Typeface.create(Typeface.DEFAULT, Typeface.NORMAL);

//JAVA TO C# CONVERTER CRACKED BY X-CRACKER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressLint("DrawAllocation") android.graphics.Rect r = new android.graphics.Rect();
          Rect r = new Rect();
          canvas.getClipBounds(r);
          paint.TextSize = r.height();
          int cHeight = r.height();
          int cWidth = r.width();
          paint.TextAlign = Paint.Align.LEFT;
          string text = "?";
          paint.getTextBounds(text, 0, text.Length, r);
          float x = cWidth / 2f - r.width() / 2f - r.left;
          float y = cHeight / 2f + r.height() / 2f - r.bottom;
          canvas.drawText(text, x, y, paint);
        }
      }

      /// <summary>
      /// @suppress </summary>
      /// <param name="container"> </param>
      public virtual void updatePreLayout(ConstraintLayout container)
      {
        if (mContentId == -1)
        {
          if (!InEditMode)
          {
            Visibility = mEmptyVisibility;
          }
        }

        mContent = container.findViewById(mContentId);
        if (mContent != null)
        {
          ConstraintLayout.LayoutParams layoutParamsContent = (ConstraintLayout.LayoutParams) mContent.LayoutParams;
          layoutParamsContent.isInPlaceholder = true;
          mContent.Visibility = View.VISIBLE;
          Visibility = View.VISIBLE;
        }
      }

      /// <summary>
      /// Sets the content view id
      /// </summary>
      /// <param name="id"> the id of the content view we want to place in the Placeholder </param>
      public virtual int ContentId
      {
          set
          {
            if (mContentId == value)
            {
              return;
            }
            if (mContent != null)
            {
              mContent.Visibility = VISIBLE; // ???
              ConstraintLayout.LayoutParams layoutParamsContent = (ConstraintLayout.LayoutParams) mContent.LayoutParams;
              layoutParamsContent.isInPlaceholder = false;
              mContent = null;
            }
    
            mContentId = value;
            if (value != ConstraintLayout.LayoutParams.UNSET)
            {
              View v = ((View) Parent).findViewById(value);
              if (v != null)
              {
                v.Visibility = GONE;
              }
            }
          }
      }

      /// <summary>
      /// @suppress </summary>
      /// <param name="container"> </param>
      public virtual void updatePostMeasure(ConstraintLayout container)
      {
        if (mContent == null)
        {
          return;
        }
        ConstraintLayout.LayoutParams layoutParams = (ConstraintLayout.LayoutParams) LayoutParams;
        ConstraintLayout.LayoutParams layoutParamsContent = (ConstraintLayout.LayoutParams) mContent.LayoutParams;
        layoutParamsContent.widget.Visibility = View.VISIBLE;
        if (layoutParams.widget.HorizontalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.FIXED)
        {
          layoutParams.widget.Width = layoutParamsContent.widget.Width;
        }
        if (layoutParams.widget.VerticalDimensionBehaviour != ConstraintWidget.DimensionBehaviour.FIXED)
        {
          layoutParams.widget.Height = layoutParamsContent.widget.Height;
        }
        layoutParamsContent.widget.Visibility = View.GONE;
      }

    }

}