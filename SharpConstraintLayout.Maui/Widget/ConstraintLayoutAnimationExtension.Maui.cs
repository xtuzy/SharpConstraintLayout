#if __MAUI__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
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
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo();
            finish.ApplyToForAnim(layout);
            var finfishLayoutTreeInfo = layout.CaptureLayoutTreeInfo(true);
            return GenerateAnimation(layout, startLayoutTreeInfo, finfishLayoutTreeInfo, easing);
        }

        public static Animation CreateAnimation(this ConstraintLayout layout, ConstraintSet start, ConstraintSet finish)
        {
            start.ApplyToForAnim(layout);
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(true);
            finish.ApplyToForAnim(layout);
            var finfishLayoutTreeInfo = layout.CaptureLayoutTreeInfo(true);
            return GenerateAnimation(layout, startLayoutTreeInfo, finfishLayoutTreeInfo);
        }

        static Animation GenerateAnimation(ConstraintLayout layout, Dictionary<int, ViewInfo> startLayoutTreeInfo, Dictionary<int, ViewInfo> finfishLayoutTreeInfo, Easing easing = null)
        {
            var animation = new Animation();
            foreach (var item in startLayoutTreeInfo)
            {
                var view = layout.FindElementById(item.Key);
                var startInfo = item.Value;
                var finishInfo = finfishLayoutTreeInfo[item.Key];
                animation.Add(0, 1, new Animation((v) =>
                {
                    var rect = new Rect((startInfo.X + (finishInfo.X - startInfo.X) * v),
                        (startInfo.Y + (finishInfo.Y - startInfo.Y) * v),
                         (startInfo.Size.Width + (finishInfo.Size.Width - startInfo.Size.Width) * v),
                         (startInfo.Size.Height + (finishInfo.Size.Height - startInfo.Size.Height) * v));
                    layout.LayoutChild(view, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                    //view.Layout(rect);
                    view.TranslationX = (finishInfo.TranlateX - startInfo.TranlateX) * v;
                    view.TranslationY = (finishInfo.TranlateY - startInfo.TranlateY) * v;
                    /*Func<double, Rect> computeBounds = progress =>
                    {
                        double x = startInfo.X + (finishInfo.X - startInfo.X) * progress;
                        double y = startInfo.Y + (finishInfo.Y - startInfo.Y) * progress;
                        double w = startInfo.Size.Width + (finishInfo.Size.Width - finishInfo.Size.Width) * progress;
                        double h = startInfo.Size.Height + (finishInfo.Size.Height - finishInfo.Size.Height) * progress;

                        return new Rect(x, y, w, h);
                    };
                    view.Layout(computeBounds(v));*/
                    view.RotationX = startInfo.RotationX + (finishInfo.RotationX - startInfo.RotationX) * v;
                    view.RotationY = startInfo.RotationY + (finishInfo.RotationY - startInfo.RotationY) * v;
                    //view.RotationZ = (finishInfo.RotationZ - startInfo.RotationZ) * v;
                    view.ScaleX = startInfo.ScaleX + (finishInfo.ScaleX - startInfo.ScaleX) * v;
                    view.ScaleY = startInfo.ScaleY + (finishInfo.ScaleY - startInfo.ScaleY) * v;
                    view.Opacity = startInfo.Alpha + (finishInfo.Alpha - startInfo.Alpha) * v;
                }, 0, 1, easing));
            }
            return animation;
        }
    }
}
#endif