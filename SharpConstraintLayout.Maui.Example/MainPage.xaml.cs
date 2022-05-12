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

        //ConstraintLayout.DEBUG = true;
        ConstraintLayout.MEASURE_MEASURELAYOUT = true;
        var content = new ConstraintLayout()
        {
            ConstrainPaddingTop = 10,
            ConstrainPaddingBottom = 10,
            ConstrainPaddingLeft = 10,
            ConstrainPaddingRight = 10,
            Background = new SolidColorBrush(Colors.HotPink),
        };
        Page.Content = content;
        createControls();

        //BaseAlignTest(content);
        //BaselineTest(content);
        //GuidelineTest(content);
        //BarrierTest(content);
        //VisibilityTest(content);
        //FlowTest(content);
        NestedConstraintLayoutTest(content);
        //CircleConstraintTest(content);
        //PlatformLayoutInConstraintLayoutTest(content);
        //FlowPerformanceTest(content);

    }

    private void createControls()
    {
        FirstButton = new Button()
        {
            Text = "FirstButton",
            //Background = new SolidColorBrush(Colors.Red)
        };

        SecondButton = new Button()
        {
            Text = "SecondBotton",
            Background = new SolidColorBrush(Colors.Black)
        };

        ThirdCanvas = new ContentView()
        {
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

