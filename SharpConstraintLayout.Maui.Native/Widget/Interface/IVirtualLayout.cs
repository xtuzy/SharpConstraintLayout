namespace SharpConstraintLayout.Maui.Widget.Interface
{
    public interface IVirtualLayout
    {
        //JniPeerMembers JniPeerMembers { get; }
        void onMeasure(androidx.constraintlayout.core.widgets.VirtualLayout layout, int widthMeasureSpec, int heightMeasureSpec);
    }
}