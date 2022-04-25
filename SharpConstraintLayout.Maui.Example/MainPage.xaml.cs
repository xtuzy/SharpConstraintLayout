using Microsoft.Maui.Controls.Shapes;
using SharpConstraintLayout.Maui.Widget;

namespace SharpConstraintLayout.Maui.Example;

public partial class MainPage : ContentPage
{
    private Button FirstButton;

    public Button SecondButton;
    private View ThirdCanvas;
    private Label FouthTextBlock;
    private Entry FifthTextBox;
    private Editor SixthRichTextBlock;
    public ConstraintLayout layout;

    public MainPage()
    {
        InitializeComponent();

        ConstraintLayout.DEBUG = true;
        ConstraintLayout.MEASURE = true;

        var content = new ConstraintLayout();
        Page.Content = content;
        createControls();
        this.Background = new SolidColorBrush(Colors.HotPink);

        content.AddElement(FirstButton);
        FirstButton.WidthRequest = 100;
        FirstButton.HeightRequest = 100;
        using (var set = new FluentConstraintSet())
        {
            set.Clone(content);
            set.Select(FirstButton).Width(100).Height(100).CenterTo();
            set.ApplyTo(content);
        }

        //BaseAlignTest(content);
        //baselineTest(content); 
        //GuidelineTest(content);
        //BarrierTest(content);
        //VisibilityTest(content);
        //flowTest(content);
        //nestedLayoutTest(content);
        //CircleConstraintTest(content);
        //PlatformLayoutInConstraintLayoutTest(content);
        //FlowPerformanceTest(content);
        //WrapPanelPerformanceTest(content);

    }

    private void createControls()
    {
        FirstButton = new Button()
        {
            Text = "FirstButton",
            Background = new SolidColorBrush(Colors.Red)
        };

        SecondButton = new Button()
        {
            Text = "SecondBotton",
            Background = new SolidColorBrush(Colors.Black)
        };

        ThirdCanvas = new Rectangle()
        {
            //Width = 100,
            //Height = 100,
            Background = new SolidColorBrush(Colors.LightGreen)
        };

        FouthTextBlock = new Label()
        {
            Text = "FourthTextBlock",
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
        };

        FifthTextBox = new Entry()
        {
            Text = "FifthTextBox",
        };

        //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
        SixthRichTextBlock = new Editor()
        {
            Text = "SixthRichTextBlock",
        };
    }
}

