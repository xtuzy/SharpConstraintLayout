
namespace SharpConstraintLayout.Maui.Widget
{
    public interface IGuideline
    {
        //JniPeerMembers JniPeerMembers { get; }

        void SetGuidelineBegin(int margin);
        void SetGuidelineEnd(int margin);
        void SetGuidelinePercent(float ratio);
    }
}