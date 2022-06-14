using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace SharpConstraintLayout.Maui.Example.Pages
{

    public partial class ShowAnimationView : ContentView
    {
        private Button button1;
        private Button button2;
        private ContentView backgroundView;
        private ConstraintLayout layout;

        public ShowAnimationView()
        {
            InitializeComponent();

            layout = new ConstraintLayout();
            this.Content = layout;
            button1 = new Button() { Text = "Button1" };
            button1.Clicked += Button1_Clicked;
            button2 = new Button() { Text = "Button2" };
            button2.Clicked += Button2_Clicked;
            backgroundView = new ContentView() { BackgroundColor = Colors.Green };
            layout.AddElement(backgroundView, button1, button2);

            var startset = new FluentConstraintSet();
            startset.Clone(layout);
            startset.Select(button1).CenterYTo().LeftToLeft()
                .Select(backgroundView).TopToTop().EdgesXTo().Width(SizeBehavier.MatchConstraint).PercentHeight(0.5f).Height(SizeBehavier.MatchConstraint)
                .Select(button2).BottomToBottom(backgroundView, 20).RightToRight(backgroundView, 20);
            startset.ApplyTo(layout);

        }

        bool isExpaned = true;
        private void Button2_Clicked(object sender, EventArgs e)
        {
            layout.AbortAnimation("ConstrainTo");
            if (isExpaned)//ÐèÒªÊÕËõ
            {
                var finish = new FluentConstraintSet();
                finish.Clone(layout);
                finish.Select(backgroundView).Clear().TopToTop().LeftToLeft().Height(20).Width(SizeBehavier.MatchParent)
                    .Select(button2).Clear().BottomToBottom(null, 20).RightToRight(null, 20);
                layout.LayoutToWithAnim(finish, "ConstrainTo", 16, 1200, Easing.SpringOut);
            }
            else
            {
                var start = new FluentConstraintSet();
                start.Clone(layout);
                start.Select(backgroundView).Clear().TopToTop().EdgesXTo().Width(SizeBehavier.MatchConstraint).PercentHeight(0.5f).Height(SizeBehavier.MatchConstraint)
                    .Select(button2).Clear().BottomToBottom(backgroundView, 20).RightToRight(backgroundView, 20);
                layout.LayoutToWithAnim(start, "ConstrainTo", 16, 3000, Easing.SpringOut);
            }
            isExpaned = !isExpaned;
        }

        private void Button1_Clicked(object sender, EventArgs e)
        {
            var start = new FluentConstraintSet();
            start.Clone(layout);
            start.Select(button1).Clear().CenterYTo().LeftToLeft();
            var finish = new FluentConstraintSet();
            finish.Clone(layout);
            finish.Select(button1).Clear().CenterYTo().RightToRight();
            layout.LayoutToWithAnim(finish, "ConstrainTo", 16, 1200, Easing.SpringOut, (v, b) => { start.ApplyTo(layout); });
        }
    }
}