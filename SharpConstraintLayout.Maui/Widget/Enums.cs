using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// Set size how to match constraint when set SizeBehavior is <see cref="SizeBehavier.MatchConstraint"/>
    /// </summary>
    public enum ConstrainedSizeBehavier
    {
        MatchConstraintSpread = ConstraintSet.MatchConstraintSpread,
        MatchConstraintWrap = ConstraintSet.MatchConstraintWrap,
        MatchConstraintPercent = ConstraintSet.MatchConstraintPercent,
    }

    public enum SizeBehavier
    {
        WrapContent = ConstraintSet.WrapContent,
        MatchParent = ConstraintSet.MatchParent,
        MatchConstraint = ConstraintSet.MatchConstraint,
    }

    public enum Edge
    {
        Left = ConstraintSet.Left,
        Right = ConstraintSet.Right,
        Top = ConstraintSet.Top,
        Bottom = ConstraintSet.Bottom,
        Baseline = ConstraintSet.Baseline,
        Start = ConstraintSet.Start,
        End = ConstraintSet.End,
    }

    public enum ConstrainedEdge
    {
        /// <summary>
        /// such as <see cref="Element.LeftToLeft(UIElement, int)"/> and <see cref="Element.LeftToRight(UIElement, int)"/>
        /// </summary>
        Left = ConstraintSet.Left,
        Right = ConstraintSet.Right,
        Top = ConstraintSet.Top,
        Bottom = ConstraintSet.Bottom,
        Baseline = ConstraintSet.Baseline,
        Start = ConstraintSet.Start,
        End = ConstraintSet.End,
        Clrcle = ConstraintSet.CircleReference,
        Center = 9,
        CenterX = 10,
        CenterY = 11,
    }

    /// <summary>
    /// Guideline Orientation of line.
    /// </summary>
    public enum Orientation
    {
        X = ConstraintSet.Horizontal,
        Y = ConstraintSet.Vertical,
    }

    /// <summary>
    /// Barrier
    /// </summary>
    public enum Direction
    {
        Left = Barrier.LEFT,
        Right = Barrier.RIGHT,
        Top = Barrier.TOP,
        Bottom = Barrier.BOTTOM,
    }

    public enum Visibility
    {
        Visible = ConstraintSet.Visible,
        Invisible = ConstraintSet.Invisible,
        Gone = ConstraintSet.Gone,
    }

    public enum ChainStyle
    {
        Spread = ConstraintSet.ChainSpread,
        SpreadInside = ConstraintSet.ChainSpreadInside,
        Packed = ConstraintSet.ChainPacked,
    }
}
