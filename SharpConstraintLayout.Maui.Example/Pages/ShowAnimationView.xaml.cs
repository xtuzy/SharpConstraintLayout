using SharpConstraintLayout.Maui.Widget;

namespace SharpConstraintLayout.Maui.Example.Pages
{

    public partial class ShowAnimationView : ContentView
    {
        private Button button1;
        private ConstraintLayout layout;

        public ShowAnimationView()
        {
            InitializeComponent();

            layout = new ConstraintLayout();
            this.Content = layout;
            button1 = new Button() { Text = "Button1" };
            layout.Add(button1);
            button1.Clicked += Button_Clicked;
            using (var startset = new FluentConstraintSet())
            {
                startset.Clone(layout);
                startset.Select(button1).CenterYTo().LeftToLeft();
                startset.ApplyTo(layout);
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //button.ScaleTo(1.5, 1000, Easing.CubicIn);
            //var animation = new Animation(v => button1.Scale = v, 1, 2);
            //animation.Commit(this, "SimpleAnimation", 16, 2000, Easing.Linear, (v, c) => button1.Scale = 1, () => false);
            var animation2 = new Animation(v =>
            {
                using (var set = new FluentConstraintSet())
                {
                    set.Clone(layout);
                    set.Select(button1).CenterTo().XBias((float)v).Rotation((float)v * 360).Scale((float)v * 2).Alpha((float)v)
                        ;
                    set.ApplyTo(layout);
                }
            }, 0, 1);
            animation2.Commit(this, "SimpleAnimation", 16, 1000, Easing.CubicInOut, (v, c) =>
            {
                using (var finalSet = new FluentConstraintSet())
                {
                    finalSet.Clone(layout);
                    finalSet.Select(button1).CenterYTo().RightToRight();
                    finalSet.ApplyTo(layout);
                }
            }, () => false);

        }
    }
}