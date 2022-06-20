#if __MAUI__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// If you need more complex animation, you can learn form here.
    /// </summary>
    public static class ConstraintLayoutAnimationExtension
    {
        public static void LayoutToWithAnim(this ConstraintLayout layout, ConstraintSet finishSet, string animName, uint rate = 16, uint length = 250, Easing easing = null, Action<double, bool> finished = null, Func<bool> repeat = null)
        {
            var animation = CreateAnimation(layout, finishSet, easing);
            if (finished == null)
                animation.Commit(layout, animName, rate, length, Easing.Linear, (v, b) => { finishSet.ApplyTo(layout); }, repeat);
            else
                animation.Commit(layout, animName, rate, length, Easing.Linear, finished, repeat);
        }

        public static Animation CreateAnimation(this ConstraintLayout layout, ConstraintSet finish, Easing easing)
        {
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(true);
            finish.ApplyToForAnim(layout);
            var finfishLayoutTreeInfo = layout.CaptureLayoutTreeInfo(true);
            var anim = GenerateAnimation(layout, startLayoutTreeInfo, finfishLayoutTreeInfo, easing);
            return anim;
        }

        public static Animation CreateAnimation(this ConstraintLayout layout, ConstraintSet start, ConstraintSet finish, Easing easing)
        {
            start.ApplyToForAnim(layout);
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(true);
            finish.ApplyToForAnim(layout);
            var finfishLayoutTreeInfo = layout.CaptureLayoutTreeInfo(true);
            var anim = GenerateAnimation(layout, startLayoutTreeInfo, finfishLayoutTreeInfo, easing);
            //start.ApplyToForAnim(layout);//restore start state
            return anim;
        }

        static Animation GenerateAnimation(ConstraintLayout layout, Dictionary<int, ViewInfo> startLayoutTreeInfo, Dictionary<int, ViewInfo> finfishLayoutTreeInfo, Easing easing = null)
        {
            var animation = new Animation();
            foreach (var item in startLayoutTreeInfo)
            {
                var view = layout.FindElementById(item.Key);
                var startInfo = item.Value;
                var finishInfo = finfishLayoutTreeInfo[item.Key];
                if (startInfo.Equals(finishInfo)) continue;
                var diffInfo = startInfo.Diff(finishInfo);
                animation.Add(0, 1, new Animation((v) =>
                {
                    if (diffInfo.X != 0 || diffInfo.Y != 0 || diffInfo.Size.Width != 0 || diffInfo.Size.Height != 0)
                    {
                        var rect = new Rect((startInfo.X + diffInfo.X * v),
                        (startInfo.Y + diffInfo.Y * v),
                         (startInfo.Size.Width + diffInfo.Size.Width * v),
                         (startInfo.Size.Height + diffInfo.Size.Height * v));
                        layout.LayoutChild(view, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                    }

                    if (diffInfo.TranlateX != 0)
                        view.TranslationX = diffInfo.TranlateX * v;
                    if (diffInfo.TranlateY != 0)
                        view.TranslationY = diffInfo.TranlateY * v;
                    if (diffInfo.Rotation != 0)
                        view.Rotation = startInfo.Rotation + diffInfo.Rotation * v;
                    if (diffInfo.RotationX != 0)
                        view.RotationX = startInfo.RotationX + diffInfo.RotationX * v;
                    if (diffInfo.RotationY != 0)
                        view.RotationY = startInfo.RotationY + diffInfo.RotationY * v;
                    //view.RotationZ = (finishInfo.RotationZ - startInfo.RotationZ) * v;

                    if (diffInfo.Scale != 0)
                        view.Scale = startInfo.Scale + diffInfo.Scale * v;
                    if (diffInfo.ScaleX != 0)
                        view.ScaleX = startInfo.ScaleX + diffInfo.ScaleX * v;
                    if (diffInfo.ScaleY != 0)
                        view.ScaleY = startInfo.ScaleY + diffInfo.ScaleY * v;

                    if (diffInfo.Alpha != 0)
                        view.Opacity = startInfo.Alpha + diffInfo.Alpha * v;
                }, 0, 1, easing));
            }
            return animation;
        }
    }
}
#endif