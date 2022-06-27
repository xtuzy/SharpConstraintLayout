using SharpConstraintLayout.Maui.Widget;
using System.Diagnostics;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace SharpConstraintLayout.Maui.Example.Pages
{

    public partial class ShowAnimationView : ContentView
    {
        private Button button1;
        private Button button2;
        private ContentView backgroundView;
        private Button button3;
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
                layout.AddElement(backgroundView, button1, button2, button3);

                startset = new FluentConstraintSet();
                startset.Clone(layout);
                startset.Select(button1).CenterYTo().LeftToLeft().Width(200)
                    .Select(backgroundView).TopToTop().EdgesXTo().Width(SizeBehavier.MatchConstraint).PercentHeight(0.5f).Height(SizeBehavier.MatchConstraint)
                    .Select(button2).BottomToBottom(backgroundView, 20).RightToRight(backgroundView, 20)
                    .Select(button3).BottomToTop(button2, 20).CenterXTo(button2).Rotation(0)
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
            if (isExpaned)//ÐèÒªÊÕËõ
            {
                var beginState = new FluentConstraintSet();
                beginState.Clone(startset);

                var button1FinishState = new FluentConstraintSet();
                button1FinishState.Clone(startset);
                button1FinishState.Select(button1).Clear().CenterYTo().RightToRight();

                var button23FinishState = new FluentConstraintSet();
                button23FinishState.Clone(startset);
                button23FinishState.Select(backgroundView).Clear().TopToTop().LeftToLeft().Height(50).MinHeight(20).Width(SizeBehavier.MatchParent)
                    .Select(button2).Clear().BottomToBottom(null, 20).RightToRight(null, 20)
                    .Select(button3).Clear().CenterYTo(button2).RightToLeft(button2, 50).Rotation(-90).Scale(2).Alpha(0.3f)
                    ;

                var button1Anim = layout.CreateAnimation(beginState, button1FinishState, Easing.Linear);

                var button2Anim = layout.CreateAnimation(beginState, button23FinishState, Easing.Linear);

                var allAnim = new Animation()
                {
                    { 0, 1, button2Anim },
                    { 0.5, 1, button1Anim }
                };
                allAnim.Commit(layout, "ConstrainTo", 16, 3000, Easing.Linear, (v, b) =>
                {
                    //When finish,we need combine all button state
                    button23FinishState.Clone(new KeyValuePair<int, ConstraintSet.Constraint>(button1.GetId(), button1FinishState.GetConstraint(button1.GetId())));
                    button23FinishState.ApplyTo(layout);
                });
            }
            else
            {
                var button123FinishState = new FluentConstraintSet();
                button123FinishState.Clone(layout);
                button123FinishState.Select(button1).Clear().CenterYTo().LeftToLeft().Width(200)
                    .Select(backgroundView).Clear().TopToTop().EdgesXTo().Width(SizeBehavier.MatchConstraint).PercentHeight(0.5f).Height(SizeBehavier.MatchConstraint)
                    .Select(button2).Clear().BottomToBottom(backgroundView, 20).RightToRight(backgroundView, 20)
                    .Select(button3).Clear().BottomToTop(button2, 20).CenterXTo(button2).Rotation(0)
                    ;
                layout.LayoutToWithAnim(button123FinishState, "ConstrainTo", 16, 3000, Easing.SpringOut);
            }
            isExpaned = !isExpaned;
        }
    }
}