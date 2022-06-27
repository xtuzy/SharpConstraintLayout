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
                animContent.Children.Add(layout);
                button1 = new Button() { Text = "Resize" };
                button2 = new Button() { Text = "Click Me" };
                button2.Clicked += Button2_Clicked;
                backgroundView = new ContentView() { BackgroundColor = Colors.Green };
                button3 = new Button() { Text = "Scale & Alpha & Rotate" };
                button4 = new Button() { Text = "Invisible<->Visiable" };
                button5 = new Button() { Text = "Go<->Visiable" };
                button6 = new Button() { Text = "Button6" };
                layout.AddElement(backgroundView, button1, button2, button3, button4, button5, button6);

                startset = new FluentConstraintSet();
                startset.Clone(layout);
                startset.Select(button1).CenterYTo().LeftToLeft().Width(200)
                    .Select(backgroundView).TopToTop().EdgesXTo().Width(SizeBehavier.MatchConstraint).PercentHeight(0.5f).Height(SizeBehavier.MatchConstraint)
                    .Select(button2).BottomToBottom(backgroundView, 20).RightToRight(backgroundView, 20)
                    .Select(button3).BottomToTop(button2, 20).CenterXTo(button2).Rotation(0)
                    .Select(button4).CenterXTo().TopToBottom(button1)
                    .Select(button5).TopToBottom(button4, 10).CenterXTo(button4)
                    .Select(button6).TopToBottom(button5, 10).CenterXTo(button5)
                    ;
                startset.ApplyTo(layout);
            };

            AnimPro.Clicked += (sender, e) =>
            {
                animContent.RemoveAt(0);
            };
        }

        bool isExpaned = true;
        private FluentConstraintSet startset;

        private void Button2_Clicked(object sender, EventArgs e)
        {
            layout.AbortAnimation("ConstrainTo");
            if (isExpaned)//–Ë“™ ’Àı
            {
                var beginState = new FluentConstraintSet();
                beginState.Clone(startset);

                var button1FinishState = new FluentConstraintSet();
                button1FinishState.Clone(startset);
                button1FinishState.Select(button1).Clear().CenterYTo().RightToRight();

                var buttonFinishState = new FluentConstraintSet();
                buttonFinishState.Clone(startset);
                buttonFinishState.Select(backgroundView).Clear().TopToTop().LeftToLeft().Height(50).MinHeight(20).Width(SizeBehavier.MatchParent)
                    .Select(button2).Clear().BottomToBottom(null, 20).RightToRight(null, 20)
                    .Select(button3).Clear().CenterYTo(button2).RightToLeft(button2, 50).Rotation(-90).Scale(2).Alpha(0.3f)
                    .Select(button4).Visibility(FluentConstraintSet.Visibility.Invisible)
                    .Select(button5).Visibility(FluentConstraintSet.Visibility.Gone);

                var button1Anim = layout.CreateAnimation(beginState, button1FinishState, Easing.Linear);

                var button236Anim = layout.CreateAnimation(beginState, buttonFinishState, Easing.Linear,
                    new List<View> { backgroundView, button2, button3, button4, button6 });

                var button4Anim = CreateVisibilityAnimation(layout,beginState, buttonFinishState, Easing.Linear, button4);
                var button5Anim = CreateVisibilityAnimation(layout, beginState, buttonFinishState, Easing.Linear, button5);
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
                var restoreAnim = layout.CreateAnimation(button123FinishState, Easing.Linear, new List<View>() { button1, backgroundView, button2, button3, button6 });
                var visibility4Anim = CreateVisibilityAnimation(layout, startState, button123FinishState, Easing.Linear, button4);
                var visibility5Anim = CreateVisibilityAnimation(layout, startState, button123FinishState, Easing.Linear, button5);
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
        public Animation CreateVisibilityAnimation(ConstraintLayout layout, ConstraintSet start, ConstraintSet end, Easing easing, View view, params ViewInfoType[] ignoreInfoType)
        {
            start.ApplyToForAnim(layout);
            var startLayoutTreeInfo = layout.CaptureLayoutTreeInfo(new List<View>() { view }, true);
            end.ApplyToForAnim(layout);
            var endLayoutTreeInfo = layout.CaptureLayoutTreeInfo(new List<View>() { view }, true);

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

                if (diffInfo.ScaleX != 0)
                    view.ScaleX = startInfo.ScaleX + diffInfo.ScaleX * v;
                if (diffInfo.ScaleY != 0)
                    view.ScaleY = startInfo.ScaleY + diffInfo.ScaleY * v;

                if (diffInfo.Alpha != 0)
                {
                    view.IsVisible = true;
                    view.Opacity = startInfo.Alpha + diffInfo.Alpha * v;
                }
            }, 0, 1, easing);
        }
    }
}