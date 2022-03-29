namespace SharpConstraintLayout.Maui.Widget
{
    public interface IVirtualLayout
    {
        //JniPeerMembers JniPeerMembers { get; }
        void OnMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec);
    }
}