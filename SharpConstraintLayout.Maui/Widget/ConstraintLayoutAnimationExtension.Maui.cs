#if __MAUI__

namespace SharpConstraintLayout.Maui.Widget
{
    /// <summary>
    /// If you need more complex animation, you can learn form here.
    /// </summary>
    public static class ConstraintLayoutAnimationExtension
    {
        /// <summary>
        /// Animation from current layout state to endSet state
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="endSet"></param>
        /// <param name="animName"></param>
        /// <param name="rate"></param>
        /// <param name="length"></param>
        /// <param name="easing"></param>
        /// <param name="finished"></param>
        /// <param name="repeat"></param>
        public static void LayoutToWithAnim(this ConstraintLayout layout, ConstraintSet endSet, string animName, uint rate = 16, uint length = 250, Easing easing = null, Action<double, bool> finished = null, Func<bool> repeat = null)
        {
            var animation = CreateAnimation(layout, endSet, easing);
            if (finished == null)
                animation.Commit(layout, animName, rate, length, Easing.Linear, (v, b) => { endSet.ApplyTo(layout); }, repeat);
            else
                animation.Commit(layout, animName, rate, length, Easing.Linear, finished, repeat);
        }

        /// <summary>
        /// Animation from current layout startSet state to endSet state
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="startSet"></param>
        /// <param name="endSet"></param>
        /// <param name="animName"></param>
        /// <param name="rate"></param>
        /// <param name="length"></param>
        /// <param name="easing"></param>
        /// <param name="finished"></param>
        /// <param name="repeat"></param>
        public static void LayoutToWithAnim(this ConstraintLayout layout, ConstraintSet startSet, ConstraintSet endSet, string animName, uint rate = 16, uint length = 250, Easing easing = null, Action<double, bool> finished = null, Func<bool> repeat = null)
        {
            var animation = CreateAnimation(layout, startSet, endSet, easing);
            if (finished == null)
                animation.Commit(layout, animName, rate, length, Easing.Linear, (v, b) => { endSet.ApplyTo(layout); }, repeat);
            else
                animation.Commit(layout, animName, rate, length, Easing.Linear, finished, repeat);
        }

        public static Animation CreateAnimation(this ConstraintLayout layout, ConstraintSet end, Easing easing, List<View> onlyViews = null)
        {
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(onlyViews, true);
            end.ApplyToForAnim(layout);
            var endLayoutTreeInfo = layout.CaptureLayoutTreeInfo(onlyViews, true);
            var anim = CreateAnimation(layout, startLayoutTreeInfo, endLayoutTreeInfo, easing);
            return anim;
        }

        public static Animation CreateAnimation(this ConstraintLayout layout, ConstraintSet start, ConstraintSet end, Easing easing, List<View> onlyViews = null, params ViewInfoType[] ignoreInfoType)
        {
            start.ApplyToForAnim(layout);
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(onlyViews, true);
            end.ApplyToForAnim(layout);
            var endLayoutTreeInfo = layout.CaptureLayoutTreeInfo(onlyViews, true);
            var anim = CreateAnimation(layout, startLayoutTreeInfo, endLayoutTreeInfo, easing, ignoreInfoType);
            return anim;
        }

        /// <summary>
        /// Create anim for all layout property that have changed.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="startLayoutTreeInfo"></param>
        /// <param name="endLayoutTreeInfo"></param>
        /// <param name="easing"></param>
        /// <returns></returns>
        public static Animation CreateAnimation(this ConstraintLayout layout, Dictionary<int, ViewInfo> startLayoutTreeInfo, Dictionary<int, ViewInfo> endLayoutTreeInfo, Easing easing = null, params ViewInfoType[] ignoreInfoType)
        {
            var animation = new Animation();
            foreach (var item in startLayoutTreeInfo)
            {
                var view = layout.FindElementById(item.Key);
                var startInfo = item.Value;
                var finishInfo = endLayoutTreeInfo[item.Key];
                if (startInfo.Equals(finishInfo)) continue;
                var diffInfo = startInfo.Diff(finishInfo, ignoreInfoType);
                animation.Add(0, 1, new Animation((v) =>
                {
                    if (diffInfo.X != 0 || diffInfo.Y != 0 || diffInfo.Width != 0 || diffInfo.Height != 0)
                    {
                        var rect = new Rect((startInfo.X + diffInfo.X * v),
                        (startInfo.Y + diffInfo.Y * v),
                         (startInfo.Width + diffInfo.Width * v),
                         (startInfo.Height + diffInfo.Height * v));
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

                    //if (diffInfo.Scale != 0)
                    //    view.Scale = startInfo.Scale + diffInfo.Scale * v;
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

        /// <summary>
        /// Create anim for single view, you can control all detail of this view.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="view"></param>
        /// <param name="startLayoutTreeInfo"></param>
        /// <param name="endLayoutTreeInfo"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="easing"></param>
        /// <param name="finished"></param>
        /// <param name="ignoreInfoType"></param>
        /// <returns></returns>
        public static Animation CreateAnimation(this ConstraintLayout layout, View view, Dictionary<int, ViewInfo> startLayoutTreeInfo, Dictionary<int, ViewInfo> endLayoutTreeInfo, double start = 0, double end = 1, Easing easing = null, Action finished = null, params ViewInfoType[] ignoreInfoType)
        {
            var id = view.GetId();
            var startInfo = startLayoutTreeInfo[id];
            var finishInfo = endLayoutTreeInfo[id];
            if (startInfo.Equals(finishInfo)) return null;
            var diffInfo = startInfo.Diff(finishInfo, ignoreInfoType);
            return new Animation((v) =>
            {
                if (diffInfo.X != 0 || diffInfo.Y != 0 || diffInfo.Width != 0 || diffInfo.Height != 0)
                {
                    var rect = new Rect((startInfo.X + diffInfo.X * v),
                    (startInfo.Y + diffInfo.Y * v),
                     (startInfo.Width + diffInfo.Width * v),
                     (startInfo.Height + diffInfo.Height * v));
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

                //if (diffInfo.Scale != 0)
                //    view.Scale = startInfo.Scale + diffInfo.Scale * v;
                if (diffInfo.ScaleX != 0)
                    view.ScaleX = startInfo.ScaleX + diffInfo.ScaleX * v;
                if (diffInfo.ScaleY != 0)
                    view.ScaleY = startInfo.ScaleY + diffInfo.ScaleY * v;

                if (diffInfo.Alpha != 0)
                    view.Opacity = startInfo.Alpha + diffInfo.Alpha * v;
            }, start, end, easing, finished);
        }
   
        public static void FinishedAnimation(this ConstraintLayout layout,ConstraintSet set = null)
        {
            if(set != null)
            {
                set.ApplyTo(layout);
            }
            else
            {
                using(set = new ConstraintSet())
                {
                    set.Clone(layout);
                    set.ApplyTo(layout);
                }
            }
        }
    }
}
#endif