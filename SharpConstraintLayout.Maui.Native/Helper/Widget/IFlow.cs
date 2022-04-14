
namespace SharpConstraintLayout.Maui.Helper.Widget
{
    public interface IFlow
    {
        //JniPeerMembers JniPeerMembers { get; }

        void SetFirstHorizontalBias(float bias);
        void SetFirstHorizontalStyle(int style);
        void SetFirstVerticalBias(float bias);
        void SetFirstVerticalStyle(int style);
        void SetHorizontalAlign(int align);
        void SetHorizontalBias(float bias);
        void SetHorizontalGap(int gap);
        void SetHorizontalStyle(int style);
        void SetLastHorizontalBias(float bias);
        void SetLastHorizontalStyle(int style);
        void SetLastVerticalBias(float bias);
        void SetLastVerticalStyle(int style);
        void SetMaxElementsWrap(int max);
        void SetOrientation(int orientation);
        void SetPadding(int padding);
        void SetPaddingBottom(int paddingBottom);
        void SetPaddingLeft(int paddingLeft);
        void SetPaddingRight(int paddingRight);
        void SetPaddingTop(int paddingTop);
        void SetVerticalAlign(int align);
        void SetVerticalBias(float bias);
        void SetVerticalGap(int gap);
        void SetVerticalStyle(int style);
        void SetWrapMode(int mode);
    }
}