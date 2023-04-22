﻿using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Shapes;
using SharpConstraintLayout.Maui.Example.Models;
using SharpConstraintLayout.Maui.Example.Pages;
using SharpConstraintLayout.Maui.Example.Tool;
using SharpConstraintLayout.Maui.Example.ViewModels;
using SharpConstraintLayout.Maui.Native.Example.Tool;
using SharpConstraintLayout.Maui.Widget;
using static SharpConstraintLayout.Maui.Widget.FluentConstraintSet;

namespace SharpConstraintLayout.Maui.Example;

public partial class MainPage : ContentPage
{

#if WINDOWS || __ANDROID__ || __IOS__
    BlogFrameRate.FrameRateCalculator fr;
#endif
    public MainPage()
    {
        InitializeComponent();

        ConstraintLayout.DEBUGCONSTRAINTLAYOUTPROCESS = true;
        //ConstraintLayout.MEASURE_MEASURELAYOUT = true;
        this.SizeChanged += (sender, e) =>
        {
            System.Diagnostics.Debug.WriteLine("MainPage: Width=" + (sender as Page).Bounds.Width);
            App.WindowWidth = (int)(sender as Page).Bounds.Width;
        };

        FitGridTest();

#if WINDOWS || __ANDROID__ || __IOS__
        if (fr == null)
        {
            fr = new BlogFrameRate.FrameRateCalculator();
            fr.FrameRateUpdated += (value) =>
            {
                this.Dispatcher.Dispatch(() => fpsLabel.Text = value.Frames.ToString());
            };
            fr.Start();
        }
#endif
    }

    /// <summary>
    /// ConstraintLayout use int in internal, so need double to int, here we test it if will get error.
    /// </summary>
    void FitGridTest()
    {
        var myHorizontalStackLayout = new MyHorizontalStackLayout();
        var myConstraintLayout = new ConstraintLayout(new Log());
        compareWithGridLayout.Add(myHorizontalStackLayout);
        compareWithGridLayout.Add(myConstraintLayout);
        Grid.SetRow(myHorizontalStackLayout, 0);
        Grid.SetRow(myConstraintLayout, 1);
        myHorizontalStackLayout.Add(new Entry() { Text = "In MyHorizontalStackLayout", VerticalOptions = LayoutOptions.Center });
        var entry = new Entry() { Text = "In ConstraintLayout" };
        myConstraintLayout.Add(entry);
        using (var set = new FluentConstraintSet())
        {
            set.Clone(myConstraintLayout);
            set.Select(entry).CenterYTo();
            set.ApplyTo(myConstraintLayout);
        }

        var testMessage = "HorizontalStackLayout should have same height with ConstraintLayout";
        var addRowTask = new Task(() =>
        {
            UIThread.Invoke(() =>
            {
                compareWithGridLayout.AddRowDefinition(new RowDefinition() { Height = GridLength.Star });
            }, myConstraintLayout);
            Task.Run(async () =>
            {
                await Task.Delay(3000);//wait UI change or show
                UIThread.Invoke(() =>
                {
                    SimpleTest.AreEqual(myHorizontalStackLayout.GetSize().Height, myConstraintLayout.GetSize().Height, nameof(FitGridTest));
                }, myConstraintLayout);
            });
        });
        Task.Run(async () =>
        {
            await Task.Delay(3000);//wait UI change or show
            UIThread.Invoke(() =>
            {
                SimpleTest.AreEqual(myHorizontalStackLayout.GetSize().Height, myConstraintLayout.GetSize().Height, nameof(FitGridTest));
            }, myConstraintLayout);
        }).ContinueWith((t) =>
        {
            addRowTask.Start();
        });
    }

    /// <summary>
    /// For know official layout measure times
    /// </summary>
    public class MyHorizontalStackLayout : HorizontalStackLayout
    {
        protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            System.Diagnostics.Debug.WriteLine("MyHorizontalStackLayout MeasureOverride");
            return base.MeasureOverride(widthConstraint, heightConstraint);
        }

        protected override Size ArrangeOverride(Rect bounds)
        {
            System.Diagnostics.Debug.WriteLine("MyHorizontalStackLayout ArrangeOverride");
            return base.ArrangeOverride(bounds);
        }
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
            Text = "FourthLabel",
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            BackgroundColor = Colors.Black
        };

        var FifthTextBox = new Entry()
        {
            Text = "FifthEntry",
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };
        FifthTextBox.TextChanged += (sender, e) =>
        {
            (FifthTextBox.Parent as ConstraintLayout)?.RequestReLayout();
        };

        //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
        var SixthRichTextBlock = new Editor()
        {
            Text = "SixthEditor",
            AutoSize = EditorAutoSizeOption.TextChanges
        };

        return (FirstButton, SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock);
    }

    ConstraintLayout CreateConstraintLayout(ILogger log = null)
    {
        var content = new ConstraintLayout(log)
        {
            ConstrainPaddingTop = 10,
            ConstrainPaddingBottom = 10,
            ConstrainPaddingLeft = 10,
            ConstrainPaddingRight = 10,
            DebugName = "Root",
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
            BackgroundColor = Colors.Black,
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };
        var editor = new Editor()
        {
            Text = "Editorfj",
            TextColor = Colors.White,
            BackgroundColor = Colors.Black,
            AutoSize = EditorAutoSizeOption.TextChanges
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
        set.Select(label).CenterYTo().LeftToLeft()
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

    private void StackLayoutInConstraintLayout_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout(new Log());
        StackLayoutInConstraintLayoutTest(content);
    }

    private void ScrollViewInConstraintLayout_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        ScrollViewInConstraintLayoutTest(content);
    }

    private void FlowPerformance_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        FlowPerformanceTest(content);
    }

    ListView listView;
    private void ConstraintLayoutInListView_Clicked(object sender, EventArgs e)
    {
        gridLayout.RemoveAt(gridLayout.Count - 1);
        listView = new ListView();
        gridLayout.Add(listView);
        ConstraintLayoutInListViewTest(listView);
    }

    private void ConstraintLayoutInContentView_Clicked(object sender, EventArgs e)
    {
        gridLayout.RemoveAt(gridLayout.Count - 1);
        var contentView = new TestContentView();
        gridLayout.Add(contentView);
    }

    private void Animation_Clicked(object sender, EventArgs e)
    {
        gridLayout.RemoveAt(gridLayout.Count - 1);
        var contentView = new ShowAnimationView();
        gridLayout.Add(contentView);
    }

    private void ZIndex_Clicked(object sender, EventArgs e)
    {
        gridLayout.RemoveAt(gridLayout.Count - 1);
        var contentView = new ShowZIndexView();
        gridLayout.Add(contentView);
    }

    private void Group_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        GroupTest(content);
    }

    private void ConstraintLayoutInScrollView_Clicked(object sender, EventArgs e)
    {
        gridLayout.RemoveAt(gridLayout.Count - 1);
        var scrollView = new ScrollView();
        gridLayout.Add(scrollView);
        ConstraintLayoutInScrollViewTest(scrollView);
    }

    private void RemoveAndAdd_Clicked(object sender, EventArgs e)
    {
        var content = CreateConstraintLayout();
        RemoveAndAddTest(content);
    }

    private void XamlSupport_Clicked(object sender, EventArgs e)
    {
        gridLayout.RemoveAt(gridLayout.Count - 1);
        gridLayout.Add(new XamlSupport());
    }
}

