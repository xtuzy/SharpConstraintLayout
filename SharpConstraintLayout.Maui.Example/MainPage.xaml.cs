using System.Diagnostics;

namespace SharpConstraintLayout.Maui.Example
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private Button button1;
        private Label label1;

        public MainPage()
        {
            InitializeComponent();
            
            Content = new Grid() { BackgroundColor = Colors.Pink };

            var Page = new ConstraintLayout() { BackgroundColor = Colors.Yellow };
            (Content as Grid).Add(Page);
            LayoutPage(Page);
        }

        private void LayoutPage(ConstraintLayout Page)
        {
            // Content = new ConstraintLayout() { };
            // var Page = Content as ConstraintLayout;
            Page.HeightType = ConstraintSet.SizeType.MatchParent;
            Page.WidthType = ConstraintSet.SizeType.MatchParent;

            button1 = new Button() { Text = "Center" };
            Button button2 = new Button() { Text = "CenterY&Right" };
            Button button3 = new Button() { Text = "CenterX&Top" };
            Button button4 = new Button() { Text = "Bottom&Left" };
            Button button5 = new Button() { Text = "button4'Left&Top" };
            label1 = new Label() { Text = "label1", FontSize = 30, HeightRequest = 100, BackgroundColor = Colors.Cyan, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
            var editor1 = new Editor() { Text = "editor1", FontSize = 30, HeightRequest = 100, BackgroundColor = Colors.Cyan, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };
            var entry1 = new Entry() { Text = "entry1", FontSize = 30, HeightRequest = 100, BackgroundColor = Colors.Cyan, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center };

            Page.Children.Add(button1);
            Page.Children.Add(button2);
            Page.Children.Add(button3);
            Page.Children.Add(button4);
            Page.Children.Add(button5);
            Page.Children.Add(label1);
            Page.Children.Add(editor1);
            Page.Children.Add(entry1);


            button1.CenterTo(Page);
            button2.CenterYTo(Page).RightToRight(Page, 20);
            button3.CenterXTo(Page).TopToTop(Page, 20);
            button4.BottomToBottom(Page, 20).RightToRight(Page, 20);
            button5.RightToLeft(button4, 20).BottomToTop(button4, 20);
            label1.BaselineToBaseline(button1).RightToLeft(button1, 20);
            editor1.BaselineToBaseline(button1).LeftToRight(button1, 20);
            entry1.BaselineToBaseline(button1).RightToLeft(button1, 20);
            button1.Clicked += OnCounterClicked;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            Debug.WriteLine($"{nameof(button1.HeightRequest)}:{button1.HeightRequest},{nameof(button1.Height)}:{button1.Height},{nameof(button1.DesiredSize)}:{button1.DesiredSize}}}");
            var button = label1;
            Debug.WriteLine($"{nameof(button.HeightRequest)}:{button.HeightRequest},{nameof(button.Height)}:{button.Height},{nameof(button.DesiredSize)}:{button.DesiredSize}}}");
        }
    }
}