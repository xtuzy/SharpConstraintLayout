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
        public static void LayoutTo(this ConstraintLayout layout, ConstraintSet endSet, string animName, uint rate = 16, uint length = 250, Easing easing = default, Action<double, bool> finished = default, Func<bool> repeat = default)
        {
            var animation = CreateAnimation(layout, endSet, default, easing);
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
        public static void LayoutTo(this ConstraintLayout layout, ConstraintSet startSet, ConstraintSet endSet, string animName, uint rate = 16, uint length = 250, Easing easing = default, Action<double, bool> finished = default, Func<bool> repeat = default)
        {
            var animation = CreateAnimation(layout, startSet, endSet, default, easing);
            if (finished == null)
                animation.Commit(layout, animName, rate, length, Easing.Linear, (v, b) => { endSet.ApplyTo(layout); }, repeat);
            else
                animation.Commit(layout, animName, rate, length, Easing.Linear, finished, repeat);
        }

        public static Animation CreateAnimation(this ConstraintLayout layout, ConstraintSet end, List<View> onlyViews = default, Easing easing = default)
        {
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(onlyViews, true);
            end.ApplyToForAnim(layout);
            var endLayoutTreeInfo = layout.CaptureLayoutTreeInfo(onlyViews, true);
            var anim = CreateAnimation(layout, startLayoutTreeInfo, endLayoutTreeInfo, easing);
            return anim;
        }

        public static Animation CreateAnimation(this ConstraintLayout layout, ConstraintSet start, ConstraintSet end, List<View> onlyViews = default, Easing easing = default, params ViewInfoType[] ignoreInfoType)
        {
            start.ApplyToForAnim(layout);
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(onlyViews, true);
            end.ApplyToForAnim(layout);
            var endLayoutTreeInfo = layout.CaptureLayoutTreeInfo(onlyViews, true);
            var anim = CreateAnimation(layout, startLayoutTreeInfo, endLayoutTreeInfo, easing, ignoreInfoType);
            return anim;
        }

        /// <summary>
        /// Create anim for all view if property will changed.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="startLayoutTreeInfo"></param>
        /// <param name="endLayoutTreeInfo"></param>
        /// <param name="easing"></param>
        /// <param name="ignoreInfoType">Define which info you want ignore, we don't make anim for it even if it change between different ConstraintSet</param>
        /// <returns></returns>
        public static Animation CreateAnimation(this ConstraintLayout layout, Dictionary<int, ViewInfo> startLayoutTreeInfo, Dictionary<int, ViewInfo> endLayoutTreeInfo, Easing easing = default, params ViewInfoType[] ignoreInfoType)
        {
            var animation = new Animation();
            foreach (var item in startLayoutTreeInfo)
            {
                var view = layout.FindElementById(item.Key);
                var startInfo = item.Value;
                var endInfo = endLayoutTreeInfo[item.Key];
                if (startInfo.Equals(endInfo)) continue;
                var diffInfo = startInfo.Diff(endInfo, ignoreInfoType);
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
        /// Create anim for single view if property will changed, you can control all detail of this view.
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
        public static Animation CreateAnimation(this ConstraintLayout layout, Dictionary<int, ViewInfo> startLayoutTreeInfo, Dictionary<int, ViewInfo> endLayoutTreeInfo, View view, double start = 0, double end = 1, Easing easing = default, Action finished = default, params ViewInfoType[] ignoreInfoType)
        {
            var id = view.GetId();
            var startInfo = startLayoutTreeInfo[id];
            var endInfo = endLayoutTreeInfo[id];
            if (startInfo.Equals(endInfo)) return null;
            var diffInfo = startInfo.Diff(endInfo, ignoreInfoType);
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

                if (diffInfo.ScaleX != 0)
                    view.ScaleX = startInfo.ScaleX + diffInfo.ScaleX * v;
                if (diffInfo.ScaleY != 0)
                    view.ScaleY = startInfo.ScaleY + diffInfo.ScaleY * v;

                if (diffInfo.Alpha != 0)
                    view.Opacity = startInfo.Alpha + diffInfo.Alpha * v;
            }, start, end, easing, finished);
        }

        /// <summary>
        /// Create anim for single view, you can control all detail of this view. If other view rely on it, you can set other view on callback func.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="startLayoutTreeInfo"></param>
        /// <param name="endLayoutTreeInfo"></param>
        /// <param name="view">which view that you want make anim for it</param>
        /// <param name="callback">args are startInfo, endInfo, diffInfo, next rect, and current anim value.
        /// nextRect containt next position and next size of view by default calculated, if you return it, next time view will be layout to this rect.
        /// diffInfo equal to endInfo - startInfo, but ignore info that you want ignore, these ignored info is 0 in diffInfo.
        /// return value is rect that will be use to layout finally, view will move to this rect. you can return nextRect, or other according to you want, if you return Rect.Zero, view not move real position and size(if you changed translate or scale, it only change surface, not view).
        /// </param>
        /// <param name="start">start value of anim</param>
        /// <param name="end">end value of anim</param>
        /// <param name="easing"></param>
        /// <param name="finished"></param>
        /// <param name="needAutoLayout">maybe you don't want update layout when you only want update translate or other</param>
        /// <param name="ignoreInfoType">which layout infor you want ignore, diffInfo will set it is 0</param>
        /// <returns></returns>
        public static Animation CreateAnimation(this ConstraintLayout layout, Dictionary<int, ViewInfo> startLayoutTreeInfo, Dictionary<int, ViewInfo> endLayoutTreeInfo, View view, Func<ViewInfo, ViewInfo, ViewInfo, Rect, double, Rect> callback, double start = 0, double end = 1, Easing easing = default, Action finished = default, params ViewInfoType[] ignoreInfoType)
        {
            var id = view.GetId();
            var startInfo = startLayoutTreeInfo[id];
            var endInfo = endLayoutTreeInfo[id];
            if (startInfo.Equals(endInfo)) return null;
            var diffInfo = startInfo.Diff(endInfo, ignoreInfoType);
            return new Animation((v) =>
            {
                Rect nextRect = Rect.Zero;
                if (diffInfo.X != 0 || diffInfo.Y != 0 || diffInfo.Width != 0 || diffInfo.Height != 0)
                {
                    nextRect = new Rect((startInfo.X + diffInfo.X * v),
                    (startInfo.Y + diffInfo.Y * v),
                     (startInfo.Width + diffInfo.Width * v),
                     (startInfo.Height + diffInfo.Height * v));
                }
                var finalRect = callback.Invoke(startInfo, endInfo, diffInfo, nextRect, v);
                if (finalRect != Rect.Zero)
                    layout.LayoutChild(view, (int)finalRect.X, (int)finalRect.Y, (int)finalRect.Width, (int)finalRect.Height);
            }, start, end, easing, finished);
        }

        /// <summary>
        /// Apply end or other ConstraintSet to ConstraintLayout, let ConstraintLayout leave Anim mode.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="set"></param>
        public static void FinishedAnimation(this ConstraintLayout layout, ConstraintSet set = null)
        {
            if (set != null)
            {
                set.ApplyTo(layout);
            }
            else
            {
                using (set = new ConstraintSet())
                {
                    set.Clone(layout);
                    set.ApplyTo(layout);
                }
            }
        }
    }
}
#endif