using System;

namespace SharpConstraintLayout.Maui.Widget.Interface
{
    public interface IBarrier
    {
        bool AllowsGoneWidget { get; set; }
        //JniPeerMembers JniPeerMembers { get; }
        //int ConstrainMargin { get; set; }
        int ConstrainType { get; set; }
        //void SetMarginDp(int margin);
    }
}