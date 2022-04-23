using System;

namespace SharpConstraintLayout.Maui.Widget.Interface
{
    public interface IBarrier
    {
        bool AllowsGoneWidget { get; set; }
        //JniPeerMembers JniPeerMembers { get; }
        int Margin { get; set; }
        int Type { get; set; }
        void SetDpMargin(int margin);
    }
}