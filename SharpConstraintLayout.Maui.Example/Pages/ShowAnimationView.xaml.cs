using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace SharpConstraintLayout.Maui.Example.Pages
{

    public partial class ShowAnimationView : ContentView
    {
        private Button button1;
        private Button button2;
        private ContentView backgroundView;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private ConstraintLayout layout;

        public ShowAnimationView()
        {
            InitializeComponent();

            BasisAnim.Clicked += (sender, e) =>
            {
                animContent.RemoveAt(0);
                layout = new ConstraintLayout();
                layout.AbortAnimation("ShakeMove");
                animContent.Children.Add(layout);
                button1 = new Button() { Text = "Resize" };
                button2 = new Button() { Text = "Click Me" };
                button2.Clicked += Button2_BasisAnim_Clicked;
                backgroundView = new ContentView() { BackgroundColor = Colors.Green };
                button3 = new Button() { Text = "Scale & Alpha & Rotate" };
                button4 = new Button() { Text = "Invisible<->Visiable" };
                button5 = new Button() { Text = "Go<->Visiable" };
                button6 = new Button() { Text = "Button6" };
                layout.AddElement(backgroundView, button1, button2, button3, button4, button5, button6);

                startSet = new FluentConstraintSet();
                startSet.Clone(layout);
                startSet.Select(button1).CenterYTo().LeftToLeft().Width(200)
                    .Select(backgroundView).TopToTop().EdgesXTo().Width(SizeBehavier.MatchConstraint).PercentHeight(0.5f).Height(SizeBehavier.MatchConstraint)
                    .Select(button2).BottomToBottom(backgroundView, 20).RightToRight(backgroundView, 20)
                    .Select(button3).BottomToTop(button2, 20).CenterXTo(button2).Rotation(0)
                    .Select(button4).CenterXTo().TopToBottom(button1)
                    .Select(button5).TopToBottom(button4, 10).CenterXTo(button4)
                    .Select(button6).TopToBottom(button5, 10).CenterXTo(button5)
                    ;
                startSet.ApplyTo(layout);
            };

            AnimPro.Clicked += (sender, e) =>
            {
                animContent.RemoveAt(0);
                layout = new ConstraintLayout();
                animContent.Children.Add(layout);
                button1 = new Button() { Text = "CenterX" };
                button2 = new Button() { Text = "Click Me" };
                button2.Clicked += Button2_AnimPro_Clicked;
                backgroundView = new ContentView() { BackgroundColor = Colors.Green };
                button3 = new Button() { Text = "CenterY" };
                button4 = new Button() { Text = "Button4" };
                button5 = new Button() { Text = "Button5" };
                button6 = new Button() { Text = "Button6" };
                layout.AddElement(backgroundView, button1, button2, button3, button4, button5, button6);

                startSet = new FluentConstraintSet();
                startSet.Clone(layout);
                startSet.Select(button1).CenterXTo().BottomToBottom(null, 10)
                    .Select(backgroundView).TopToTop().EdgesXTo().Width(SizeBehavier.MatchConstraint).PercentHeight(0.5f).Height(SizeBehavier.MatchConstraint)
                    .Select(button2).BottomToBottom(backgroundView, 20).RightToRight(backgroundView, 20)
                    .Select(button3).BottomToBottom(backgroundView, 20).LeftToLeft(null, 40).Rotation(0)
                    .Select(button4, button5, button6).CircleTo(button3, new int[] { 50, 50, 50 }, new float[] { 120, 240, 360 })
                    .Select(button4).Rotation(120)
                    .Select(button5).Rotation(240)
                    .Select(button6).Rotation(360)
                    ;
                startSet.ApplyTo(layout);
            };
        }

        private void Button2_AnimPro_Clicked(object sender, EventArgs e)
        {
            var startState = new FluentConstraintSet();
            startState.Clone(startSet);
            var dRotation = 360;
            var dR = 100;
            var endState = new FluentConstraintSet();
            endState.Clone(startState);
            endState.Select(backgroundView).Clear().TopToTop().LeftToLeft().Height(50).MinHeight(20).Width(SizeBehavier.MatchParent)
                .Select(button1).Clear(ConstrainedEdge.Bottom).TopToTop(null, 10)
                .Select(button3).Clear().BottomToBottom(backgroundView, 20).RightToRight(null, 40).Rotation(dRotation)
                    .Select(button4, button5, button6).Clear().CircleTo(button3, new int[] { 50 + dR, 50 + dR, 50 + dR }, new float[] { 120 + dRotation, 240 + dRotation, 360 + dRotation })
                    .Select(button4).Rotation(120 + dRotation)
                    .Select(button5).Rotation(240 + dRotation).Height(200)
                    .Select(button6).Rotation(360 + dRotation)
                    ;

            var moveAnim = layout.CreateAnimation(startState, endState, new List<View> { backgroundView, button1, button2 }, Easing.Linear);

            var shakeAnim = new Animation((v) =>
            {
                button1.TranslationX = Math.Sin(4 * 2 * Math.PI * v) * 50;
            }, 0, 1, Easing.Linear);

            var rollAnim = CreateCircleAnimation(layout, button3, new View[] { button4, button5, button6 }, startState, endState, Easing.Linear);
            var allAnim = new Animation()
            {
                {0,1,moveAnim },
                {0,1,shakeAnim },
                {0,1,rollAnim }
            };
            allAnim.Commit(layout, "ShakeMove", 12, 2000, Easing.Linear, (v, b) =>
                {
                    startSet.ApplyTo(layout);
                });
        }

        bool isExpaned = true;
        private FluentConstraintSet startSet;

        private void Button2_BasisAnim_Clicked(object sender, EventArgs e)
        {
            layout.AbortAnimation("ConstrainTo");
            if (isExpaned)//–Ë“™ ’Àı
            {
                var beginState = new FluentConstraintSet();
                beginState.Clone(startSet);

                var button1FinishState = new FluentConstraintSet();
                button1FinishState.Clone(startSet);
                button1FinishState.Select(button1).Clear().CenterYTo().RightToRight();

                var buttonFinishState = new FluentConstraintSet();
                buttonFinishState.Clone(startSet);
                buttonFinishState.Select(backgroundView).Clear().TopToTop().LeftToLeft().Height(50).MinHeight(20).Width(SizeBehavier.MatchParent)
                    .Select(button2).Clear().BottomToBottom(null, 20).RightToRight(null, 20)
                    .Select(button3).Clear().CenterYTo(button2).RightToLeft(button2, 50).Rotation(-90).Scale(2).Alpha(0.3f)
                    .Select(button4).Visibility(FluentConstraintSet.Visibility.Invisible)
                    .Select(button5).Visibility(FluentConstraintSet.Visibility.Gone);

                var button1Anim = layout.CreateAnimation(beginState, button1FinishState, default, Easing.Linear);

                var button236Anim = layout.CreateAnimation(beginState, buttonFinishState,
                    new List<View> { backgroundView, button2, button3, button4, button6 }, Easing.Linear);

                var button4Anim = CreateVisibilityAnimation(layout, button4, beginState, buttonFinishState, Easing.Linear);
                var button5Anim = CreateVisibilityAnimation(layout, button5, beginState, buttonFinishState, Easing.Linear);
                var allAnim = new Animation()
                {
                    { 0, 1, button236Anim },
                    { 0.5, 1, button1Anim },
                    { 0, 1, button4Anim },
                    { 0, 1, button5Anim }
                };
                allAnim.Commit(layout, "ConstrainTo", 16, 3000, Easing.Linear, (v, b) =>
                {
                    //When finish,we need combine all button state
                    buttonFinishState.Clone(
                        (button1, button1FinishState)
                        );
                    buttonFinishState.ApplyTo(layout);
                });
            }
            else
            {
                var startState = new FluentConstraintSet();
                startState.Clone(layout);

                var button123FinishState = new FluentConstraintSet();
                button123FinishState.Clone(layout);
                button123FinishState.Select(button1).Clear().CenterYTo().LeftToLeft().Width(200)
                    .Select(backgroundView).Clear().TopToTop().EdgesXTo().Width(SizeBehavier.MatchConstraint).PercentHeight(0.5f).Height(SizeBehavier.MatchConstraint)
                    .Select(button2).Clear().BottomToBottom(backgroundView, 20).RightToRight(backgroundView, 20)
                    .Select(button3).Clear().BottomToTop(button2, 20).CenterXTo(button2).Rotation(0)
                    .Select(button4).Visibility(FluentConstraintSet.Visibility.Visible)
                    .Select(button5).Visibility(FluentConstraintSet.Visibility.Visible)
                    ;
                var restoreAnim = layout.CreateAnimation(button123FinishState, new List<View>() { button1, backgroundView, button2, button3, button6 }, Easing.Linear);
                var visibility4Anim = CreateVisibilityAnimation(layout, button4, startState, button123FinishState, Easing.Linear);
                var visibility5Anim = CreateVisibilityAnimation(layout, button5, startState, button123FinishState, Easing.Linear);
                var allAnim = new Animation()
                {
                    {0,1,restoreAnim },
                    {0,1,visibility4Anim },
                    {0,1,visibility5Anim }
                };
                allAnim.Commit(layout, "ConstrainTo", 16, 3000, Easing.SpringOut, (v, b) =>
                {
                    button123FinishState.ApplyTo(layout);
                });
            }
            isExpaned = !isExpaned;
        }

        /// <summary>
        /// This method show how to create anim for Visibility.
        /// We change Alpha and Size to show Visibility change.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="easing"></param>
        /// <param name="view"></param>
        /// <param name="ignoreInfoType"></param>
        /// <returns></returns>
        public Animation CreateVisibilityAnimation(ConstraintLayout layout, View view, ConstraintSet start, ConstraintSet end, Easing easing, params ViewInfoType[] ignoreInfoType)
        {
            start.ApplyToForAnim(layout);
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(new List<View>() { view }, true);
            end.ApplyToForAnim(layout);
            var endLayoutTreeInfo = layout.CaptureLayoutTreeInfo(new List<View>() { view }, true);

            return layout.CreateAnimation(startLayoutTreeInfo, endLayoutTreeInfo, view, (startInfo, endInfo, diffInfo, curentRect, v) =>
            {
                //Change Alpha
                if (diffInfo.Alpha != 0)
                {
                    view.IsVisible = true;
                    view.Opacity = startInfo.Alpha + diffInfo.Alpha * v;
                }

                //Decrease height
                var bigWidth = diffInfo.Width > 0 ? endInfo.Width : startInfo.Width;
                var x = diffInfo.Width > 0 ? endInfo.X : startInfo.X;
                var rect = new Rect(x, (startInfo.Y + diffInfo.Y * v), bigWidth, (startInfo.Height + diffInfo.Height * v));
                return rect;
            }, 0, 1);
        }

        /// <summary>
        /// Default ConstraintLayout use start and end location to make anim, so anim path is straight line.
        /// Why not let ConstraintLayout calculate all keyframe? because ConstraintLayout not very fast when change ConstraintSet, and it will waste many cpu to calcute.
        /// So if you want path is curve, you need add translate anim to move the view location of keyframe, ConstraintLayout only provide real size and real position change.
        /// So if you want cicle path, you need use math calculate next position of view at circle.
        /// </summary>
        /// <param name="layout"></param>
        /// <param name="centerView"></param>
        /// <param name="circleViews"></param>
        /// <param name="startSet"></param>
        /// <param name="endSet"></param>
        /// <param name="easing"></param>
        /// <param name="ignoreInfoType"></param>
        /// <returns></returns>
        public Animation CreateCircleAnimation(ConstraintLayout layout, View centerView, View[] circleViews, ConstraintSet startSet, ConstraintSet endSet, Easing easing, params ViewInfoType[] ignoreInfoType)
        {
            startSet.ApplyToForAnim(layout);
            var allViews = new List<View>();
            allViews.Add(centerView);
            foreach (var view in circleViews)
            {
                allViews.Add(view);
            }
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(allViews, true);
            endSet.ApplyToForAnim(layout);
            var endLayoutTreeInfo = layout.CaptureLayoutTreeInfo(allViews, true);
            return layout.CreateAnimation(startLayoutTreeInfo, endLayoutTreeInfo, centerView, (centerViewStartInfo, centerViewEndInfo, centerViewDiffInfo, centerViewnNextRect, v) =>
            {
                foreach (var circleView in circleViews)
                {
                    var id = circleView.GetId();
                    //
                    var circleViewStartInfo = startLayoutTreeInfo[id];
                    var circleViewEndInfo = endLayoutTreeInfo[id];
                    var circleViewDiffInfo = circleViewStartInfo.Diff(circleViewEndInfo);

                    if (circleViewDiffInfo.Rotation != 0)
                        circleView.Rotation = circleViewStartInfo.Rotation + circleViewDiffInfo.Rotation * v;

                    Rect circleViewNextRect = new Rect()
                    {
                        Width = circleViewStartInfo.Width + circleViewDiffInfo.Width * v,
                        Height = circleViewStartInfo.Height + circleViewDiffInfo.Height * v
                    };

                    var startContrint = startSet.GetConstraint(id);
                    var endContrint = endSet.GetConstraint(id);

                    var startR = startContrint.layout.circleRadius;
                    var endR = endContrint.layout.circleRadius;
                    var r = startR + (endR - startR) * v;

                    var startAngle = startContrint.layout.circleAngle;
                    var endAngle = endContrint.layout.circleAngle;
                    var angle = startAngle + (endAngle - startAngle) * v;

                    var circleViewCenterX = centerViewnNextRect.Center.X + Math.Sin(angle * Math.PI / 180) * r;
                    var circleViewCenterY = centerViewnNextRect.Center.Y + -Math.Cos(angle * Math.PI / 180) * r;

                    circleViewNextRect.X = circleViewCenterX - circleViewNextRect.Width / 2;
                    circleViewNextRect.Y = circleViewCenterY - circleViewNextRect.Height / 2;
                    layout.LayoutChild(circleView, (int)circleViewNextRect.X, (int)circleViewNextRect.Y, (int)circleViewNextRect.Width, (int)circleViewNextRect.Height);
                }

                return centerViewnNextRect;
            }, 0, 1);
        }
    }
}