using Microsoft.Maui.Controls.Shapes;
using SharpConstraintLayout.Maui.DebugTool;
using SharpConstraintLayout.Maui.Example.Models;
using SharpConstraintLayout.Maui.Example.ViewModels;
using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace SharpConstraintLayout.Maui.Example;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        //ConstraintLayout.DEBUG = true;
        ConstraintLayout.MEASURE_MEASURELAYOUT = true;

        TempButton_Clicked(null, null);
    }

    private (Button FirstButton, Button SecondButton, ContentView ThirdCanvas, Label FouthTextBlock, Entry FifthTextBox, Editor SixthRichTextBlock) CreateControls()
    {
        var FirstButton = new Button()
        {
            Text = "FirstButton",
            //Background = new SolidColorBrush(Colors.Red)
        };

        var SecondButton = new Button()
        {
            Text = "SecondBotton",
            Background = new SolidColorBrush(Colors.Black)
        };

        var ThirdCanvas = new ContentView()
        {
            Background = new SolidColorBrush(Colors.LightGreen)
        };

        var FouthTextBlock = new Label()
        {
            Text = "FourthTextBlock",
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            BackgroundColor = Colors.Black
        };

        var FifthTextBox = new Entry()
        {
            Text = "FifthTextBox",
        };
        //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
        var SixthRichTextBlock = new Editor()
        {
            Text = "SixthRichTextBlock",
        };

        return (FirstButton, SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock);
    }

    ConstraintLayout CreateConstraintLayout()
    {
        var content = new ConstraintLayout()
        {
            ConstrainPaddingTop = 10,
            ConstrainPaddingBottom = 10,
            ConstrainPaddingLeft = 10,
            ConstrainPaddingRight = 10,
            Background = new SolidColorBrush(Colors.HotPink),
        };
        gridLayout.RemoveAt(gridLayout.Count - 1);
        gridLayout.Add(content);
        return content;
    }

    private void BaseAlign_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        BaseAlignTest(content);
    }

    private void Baseline_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        var label = new Label()
        {
            Text = "Labelfj",
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            BackgroundColor = Colors.Black
        };
        var entry = new Entry()
        {
            Text = "Entryfj",
            FontSize = 30,
            TextColor = Colors.White,
            BackgroundColor = Colors.Black
        };
        var editor = new Editor()
        {
            Text = "Editorfj",
            TextColor = Colors.White,
            BackgroundColor = Colors.Black
        };
        var searchBar = new SearchBar()
        {
            Text = "SearchBar",
            TextColor = Colors.White,
            BackgroundColor = Colors.Black
        };

        content.AddElement(label, entry, editor, searchBar);
        var set = new FluentConstraintSet();
        set.Clone(content);
        set.Select(label).CenterTo()
            .Select(entry).BaselineToBaseline(label).LeftToRight(label, 5)
            .Select(editor).BaselineToBaseline(entry).LeftToRight(entry, 5)
            .Select(searchBar).BaselineToBaseline(editor).LeftToRight(editor, 5);
        set.ApplyTo(content);
    }

    private void Guideline_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        GuidelineTest(content);
    }

    private void Barrier_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        BarrierTest(content);
    }

    private void Visibility_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        VisibilityTest(content);
    }

    private void Flow_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        FlowTest(content);
    }

    private void CircleConstraint_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        CircleConstraintTest(content);
    }

    private void NestedConstraintLayout_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        NestedConstraintLayoutTest(content);
    }

    private void PlatformLayoutInConstraintLayout_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        PlatformLayoutInConstraintLayoutTest(content);

    }

    private void FlowPerformance_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        FlowPerformanceTest(content);
    }

    private void ConstraintLayoutInListView_Clicked(object sender, EventArgs e)
    {
        gridLayout.RemoveAt(gridLayout.Count - 1);
        var listView = new ListView();
        gridLayout.Add(listView);
        ConstraintLayoutInListViewTest(listView);
    }

    private void TempButton_Clicked(object sender, EventArgs e)
    {
        using (var set = new FluentConstraintSet())
        {
            set.Clone(TempConstraintLayout);
            set.Select(TempButton).CenterXTo();
            set.ApplyTo(TempConstraintLayout);
        }
    }
}

