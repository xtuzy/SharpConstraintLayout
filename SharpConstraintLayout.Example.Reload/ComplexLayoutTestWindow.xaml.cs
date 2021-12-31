using SharpConstraintLayout.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SharpConstraintLayout.Example.Reload
{
    /// <summary>
    /// ComplexLayoutTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ComplexLayoutTestWindow : Window
    {
        public ComplexLayoutTestWindow()
        {
            InitializeComponent();
            //页面分区
            var page = new ConstraintLayout()
            {
                Background = new SolidColorBrush(Colors.Cyan),
                Tag = "Page",
                //TEST = true,
            };
            this.RootWindow.Children.Add(page);

            var toolBar = new WrapPanel()
            {
                Background = new SolidColorBrush(Colors.Wheat),
                Tag = "ToolBar",
            };

            var testPanel = new ConstraintLayout()
            {
                Tag = "TestPanel",
                //TEST = true,
                //DEBUG = true,
            };

            page.Children.Add(toolBar);
            page.Children.Add(testPanel);
            new ConstraintSet(page)
                .AddConnect(toolBar, ConstraintSet.Side.Top, page, ConstraintSet.Side.Top)
                .SetWidth(toolBar, ConstraintSet.SizeType.MatchParent)
                .SetHeight(toolBar, ConstraintSet.SizeType.WrapContent)
                .AddConnect(testPanel, ConstraintSet.Side.Top, toolBar, ConstraintSet.Side.Bottom)
                .AddConnect(testPanel, ConstraintSet.Side.Bottom, page, ConstraintSet.Side.Bottom)
                .SetWidth(testPanel, ConstraintSet.SizeType.MatchParent)
                .SetHeight(testPanel, ConstraintSet.SizeType.MatchConstraint);
            //工具栏详细设置
            var RemoveConstraintTestButton = new Button() { Content = "RemoveConstraintTest", Margin = new Thickness(10, 5, 0, 5) };
            var RemoveViewTestButton = new Button() { Content = "RemoveViewTest", Margin = new Thickness(10, 5, 0, 5) };
            var VisibilityTestButton = new Button() { Content = "VisibilityTest", Margin = new Thickness(10, 5, 0, 5) };
            var NestedConstraintLayoutTestButton = new Button() { Content = "NestedConstraintLayoutTest", Margin = new Thickness(10, 5, 0, 5) };
            var NestedGridTestButton = new Button() { Content = "NestedGridTest", Margin = new Thickness(10, 5, 0, 5) };
            var GridNestedConstaintLayoutTestButton = new Button() { Content = "GridNestedConstaintLayoutTest", Margin = new Thickness(10, 5, 0, 5) };
            var NestListViewTestButton = new Button() { Content = "NestListViewTest", Margin = new Thickness(10, 5, 0, 5) };
            var ListViewNestConstraintTestButton = new Button() { Content = "ListViewNestConstraintTest", Margin = new Thickness(10, 5, 0, 5) };
            var ListViewItemUseConstraintTestButton = new Button() { Content = "ListViewItemUseConstraintTest", Margin = new Thickness(10, 5, 0, 5) };
            var WrapPanelNestContraintLayoutTestButton = new Button() { Content = "WrapPanelNestContraintLayoutTest", Margin = new Thickness(10, 5, 0, 5) };
            var NewApiTestButton = new Button() { Content = "NewApiTest", Margin = new Thickness(10, 5, 0, 5) };
            var BarrierTestButton = new Button() { Content = "BarrierTest", Margin = new Thickness(10, 5, 0, 5) };
            var ChainTestButton = new Button() { Content = "ChainTest", Margin = new Thickness(10, 5, 0, 5) };
            var WidthHeightRatioTestButton = new Button() { Content = "WidthHeightRatioTest", Margin = new Thickness(10, 5, 0, 5) };
            var CircleTestButton = new Button() { Content = "CircleTest", Margin = new Thickness(10, 5, 0, 5) };
            var FlowBoxTestButton = new Button() { Content = "FlowBoxTest", Margin = new Thickness(10, 5, 0, 5) };
            var BaselineTestButton = new Button() { Content = "BaselineTest", Margin = new Thickness(10, 5, 0, 5) };
            

            toolBar.Children.Add(RemoveConstraintTestButton);
            toolBar.Children.Add(RemoveViewTestButton);
            toolBar.Children.Add(VisibilityTestButton);
            toolBar.Children.Add(NestedConstraintLayoutTestButton);

            toolBar.Children.Add(NestedGridTestButton);
            toolBar.Children.Add(GridNestedConstaintLayoutTestButton);

            toolBar.Children.Add(NestListViewTestButton);
            toolBar.Children.Add(ListViewNestConstraintTestButton);

            toolBar.Children.Add(ListViewItemUseConstraintTestButton);

            toolBar.Children.Add(WrapPanelNestContraintLayoutTestButton);

            toolBar.Children.Add(NewApiTestButton);
            toolBar.Children.Add(BarrierTestButton);
            toolBar.Children.Add(ChainTestButton);
            toolBar.Children.Add(WidthHeightRatioTestButton);
            toolBar.Children.Add(CircleTestButton);
            toolBar.Children.Add(FlowBoxTestButton);
            toolBar.Children.Add(BaselineTestButton);



            RemoveConstraintTestButton.Click += (sender, e) =>
            {
                RemoveConstraintTest(testPanel);
            };
            
            RemoveViewTestButton.Click += (sender, e) =>
            {
                RemoveViewTest(testPanel);
            };

            VisibilityTestButton.Click += (sender, e) =>
            {
                VisibilityTest(testPanel);
            };

            NestedConstraintLayoutTestButton.Click += (sender, e) =>
            {
                NestedConstraintLayoutTest(testPanel);
            };

            NestedGridTestButton.Click += (sender, e) =>
            {
                NestedGridTest(testPanel);
            };

            GridNestedConstaintLayoutTestButton.Click += (sender, e) =>
            {
                GridNestedConstaintLayoutTest(testPanel);
            };
            NestListViewTestButton.Click += (sender, e) =>
            {
                NestListViewTest(testPanel);
            };
            ListViewNestConstraintTestButton.Click += (sender, e) =>
            {
                ListViewNestConstraintTest(testPanel);
            };
            ListViewItemUseConstraintTestButton.Click += (sender, e) =>
            {
                new ScrollTestWindow().Show();
            };

            WrapPanelNestContraintLayoutTestButton.Click += (sender, e) =>
            {
                WrapPanelNestContraintLayoutTest(testPanel);
            };

            NewApiTestButton.Click += (sender, e) =>
            {
                NewApiTest(testPanel);
            };

            BarrierTestButton.Click += (sender, e) =>
            {
                BarrierTest(testPanel);
            };
            ChainTestButton.Click += (sender, e) =>
            {
                ChainTest(testPanel);
            };
            WidthHeightRatioTestButton.Click += (sender, e) =>
            {
                WidthHeightRatioTest(testPanel);
            };

            CircleTestButton.Click += (sender, e) =>
            {
                CircleTest(testPanel);
            };
            FlowBoxTestButton.Click += (sender, e) =>
            {
                FlowBoxTest(testPanel);
            };
            BaselineTestButton.Click += (sender, e) =>
            {
                BaselineTest(testPanel);
            };
                
        }

        private void BaselineTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
           /* TextBlock textBlock = new TextBlock() { Text = "牛啊牛啊",FontSize=30, Background = new SolidColorBrush(Colors.Pink),Margin=new Thickness(0,0,0,5),Padding =new Thickness(0,0,0,0),TextAlignment=TextAlignment.Center};
            TextBlock textBlock1 = new TextBlock() { Text = "牛啊牛啊1",FontSize=60, Background = new SolidColorBrush(Colors.Pink),Margin=new Thickness(0,0,0,5),Padding =new Thickness(0,0,0,0),TextAlignment=TextAlignment.Center};
            TextBox textBox = new TextBox() { Text = "马啊马啊",FontSize=30};
            RichTextBox richTextBox = new RichTextBox() { };
            Button button = new Button() { Content = "狗啊狗啊" ,FontSize=30,BorderThickness= new Thickness(0,0,0,0),Padding = new Thickness(0,0,0,0)};
            Page.Children.Add(textBlock);
            Page.Children.Add(textBox);
            Page.Children.Add(richTextBox);
            Page.Children.Add(button);
            Page.Children.Add(textBlock1);

            textBlock.TopToTop(Page);
            textBox.LeftToRight(textBlock);
            richTextBox.BottomToBottom(Page);
            button.LeftToRight(textBox);
            textBlock1.LeftToRight(button);

            //Page.UpdateLayout();
            //we can calculate baseline position to top.
            //TextBlock:ActualHeight-Padding.Bottom
            //TextBox:ActualHeight-BorderThickness.Bottom-Padding.Bottom
            //Button:ActualHeight-BorderThickness.Bottom-Padding.Bottom
            Debug.WriteLine($"{textBlock} Desierd {textBlock.DesiredSize.Height} ActualHeight {textBlock.ActualHeight},BaselineOffset {textBlock.BaselineOffset},LineHeight {textBlock.LineHeight},Padding {textBlock.Padding.Bottom},Margin {textBlock.Margin.Bottom}");
            Debug.WriteLine($"{textBlock1} ActualHeight {textBlock1.ActualHeight},BaselineOffset {textBlock1.BaselineOffset},LineHeight {textBlock1.LineHeight},Padding {textBlock1.Padding.Bottom},Margin {textBlock1.Margin.Bottom}");
            Debug.WriteLine($"{textBox} ActualHeight {textBox.ActualHeight},ExtentHeight {textBox.ExtentHeight},BorderThickness {textBox.BorderThickness.Bottom},Padding {textBox.Padding.Bottom},Margin {textBox.Margin.Bottom}");
            Debug.WriteLine($"{button} ActualHeight {button.ActualHeight},BorderThickness {button.BorderThickness.Bottom},Padding {button.Padding.Bottom},Margin {button.Margin.Bottom}");
            */
            //so,i according to it create api for baseline.
            TextBlock firstTextBlock = new TextBlock() {
                Height=300,Width=150,
                TextAlignment=TextAlignment.Center,
                VerticalAlignment=VerticalAlignment.Center, 
                Text = "first", 
                Tag= "first", FontSize = 60, Background = new SolidColorBrush(Colors.Pink)};
            TextBox secondTextBox = new TextBox() { 
                Height = 300, Width = 150,
                AcceptsReturn = true,
                VerticalContentAlignment =VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center, 
                Text = "second马",
                Tag="second", FontSize = 18 
            };
            Button thirdButton = new Button() {
                Height=100, Width = 300, 
                VerticalContentAlignment =VerticalAlignment.Bottom, 
                Content = "third狗",
                Tag="third", FontSize = 30};
            Label forthLable = new Label() {
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = "forthLable",
            };
            Page.Children.Add(firstTextBlock);
            Page.Children.Add(secondTextBox);
            Page.Children.Add(thirdButton);
            Page.Children.Add(forthLable);
            firstTextBlock.LeftToLeft(Page).CenterYTo(Page);
            secondTextBox.LeftToRight(firstTextBlock).BaselineToBaseline(firstTextBlock);
            thirdButton.LeftToRight(secondTextBox).BaselineToBaseline(secondTextBox);
            forthLable.LeftToRight(thirdButton).BaselineToBaseline(thirdButton);
            Page.UpdateLayout();
            Debug.WriteLine($"{firstTextBlock} Baseline {Page.GetWidget(firstTextBlock).BaselineDistance}");
            Debug.WriteLine($"{secondTextBox} Baseline {Page.GetWidget(secondTextBox).BaselineDistance}");
            Debug.WriteLine($"{thirdButton} Baseline {Page.GetWidget(thirdButton).BaselineDistance}");
        }

        private void FlowBoxTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            //Horizontal
            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 100, Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100, Height = 100 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 100, Height = 100 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 100, Height = 100 };
            FlowBox horizontalFlow = new FlowBox() { WrapMode = FlowBox.Mode.Chain};
            Canvas canvas = new Canvas() { Background = new SolidColorBrush(Colors.Coral)};

            Page.Children.Add(canvas);
            Page.Children.Add(firstButton);
            Page.Children.Add(secondButton);
            Page.Children.Add(thirdButton);
            Page.Children.Add(forthButton);
            Page.Children.Add(horizontalFlow);
            

            horizontalFlow.AddView(firstButton);
            horizontalFlow.AddView(secondButton);
            horizontalFlow.AddView(thirdButton);
            horizontalFlow.AddView(forthButton);

            horizontalFlow
                .LeftToLeft(Page)
                .RightToRight(Page)
                .WidthEqualTo(ConstraintSet.SizeType.MatchConstraint)
                .HeightEqualTo(ConstraintSet.SizeType.WrapContent);
            canvas
                .LeftToLeft(horizontalFlow)
                .RightToRight(horizontalFlow)
                .TopToTop(horizontalFlow)
                .BottomToBottom(horizontalFlow)
                .WidthEqualTo(ConstraintSet.SizeType.MatchConstraint)
                .HeightEqualTo(ConstraintSet.SizeType.MatchConstraint);

            //Vertical
        }

        private void CircleTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 100, Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100, Height = 100 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 100, Height = 100 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 100, Height = 100 };

            Page.Children.Add(firstButton);
            Page.Children.Add(secondButton);
            Page.Children.Add(thirdButton);
            Page.Children.Add(forthButton);

            firstButton.CenterTo(Page);
            //0 at Top
            secondButton.CircleToCenter(firstButton, 0, 200);
            thirdButton.CircleToCenter(firstButton, 120, 200);
            forthButton.CircleToCenter(firstButton,240, 200);
        }

        private void VisibilityTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 100, Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100, Height = 100 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 100, Height = 100 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 100, Height = 100 };

            Button fifthButton = new Button() { Content = "Fifth ", Width = 100, Height = 100 };
            Button sixthButton = new Button() { Content = "Sixth ", Width = 100, Height = 100 };
            Button seventhButton = new Button() { Content = "Seventh ", Width = 100, Height = 100 };
            Button eighthButton = new Button() { Content = "Eighth", Width = 100, Height = 100 };

            Page.Children.Add(firstButton);
            Page.Children.Add(secondButton);
            Page.Children.Add(thirdButton);
            Page.Children.Add(forthButton);

            Page.Children.Add(fifthButton);
            Page.Children.Add(sixthButton);
            Page.Children.Add(seventhButton);
            Page.Children.Add(eighthButton);

            ConstraintSet set = new ConstraintSet(Page);

            set.AddConnect(firstButton, ConstraintSet.Side.Center, Page, ConstraintSet.Side.Center)
                .AddConnect(secondButton, ConstraintSet.Side.Left, firstButton, ConstraintSet.Side.Right)
                .AddConnect(secondButton, ConstraintSet.Side.Top, firstButton, ConstraintSet.Side.Top)
                .AddConnect(thirdButton, ConstraintSet.Side.Left, secondButton, ConstraintSet.Side.Right)
                .AddConnect(thirdButton, ConstraintSet.Side.Top, secondButton, ConstraintSet.Side.Top)
                .AddConnect(forthButton, ConstraintSet.Side.Left, thirdButton, ConstraintSet.Side.Right)
                .AddConnect(forthButton, ConstraintSet.Side.Top, firstButton, ConstraintSet.Side.Top);

            set.AddConnect(fifthButton, ConstraintSet.Side.Left, firstButton, ConstraintSet.Side.Left)
                .AddConnect(fifthButton, ConstraintSet.Side.Top, firstButton, ConstraintSet.Side.Bottom)
                .AddConnect(sixthButton, ConstraintSet.Side.Left, fifthButton, ConstraintSet.Side.Right)
                .AddConnect(sixthButton, ConstraintSet.Side.Top, fifthButton, ConstraintSet.Side.Top)
                .AddConnect(seventhButton, ConstraintSet.Side.Left, sixthButton, ConstraintSet.Side.Right)
                .AddConnect(seventhButton, ConstraintSet.Side.Top, fifthButton, ConstraintSet.Side.Top)
                .AddConnect(eighthButton, ConstraintSet.Side.Left, seventhButton, ConstraintSet.Side.Right)
                .AddConnect(eighthButton, ConstraintSet.Side.Top, fifthButton, ConstraintSet.Side.Top);

            Page.InvalidateVisual();

            //gone
            firstButton.Click += (sender, e) =>
            {
                thirdButton.VisibilityEqualTo(ConstraintSet.Visibility.Gone);
                Page.InvalidateMeasure();

            };
            //visible
            secondButton.Click += (sender, e) =>
            {
                thirdButton.VisibilityEqualTo(ConstraintSet.Visibility.Visible);
                Page.InvalidateMeasure();
            };
            //invisible
            fifthButton.Click += (sender, e) =>
            {
                seventhButton.VisibilityEqualTo(ConstraintSet.Visibility.Invisible);
                Page.InvalidateMeasure();

            };
            //visible
            sixthButton.Click += (sender, e) =>
            {
                seventhButton.VisibilityEqualTo(ConstraintSet.Visibility.Visible);
                Page.InvalidateMeasure();

            };
        }

        private void WidthHeightRatioTest(ConstraintLayout Page)
        {
            Page.Children.Clear();

            Button firstButton = new Button() { Content = "First", Tag = "first", Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100 };
            Page.Children.Add(firstButton);
            Page.Children.Add(secondButton);
            firstButton.LeftToLeft(Page).RightToLeft(secondButton).CenterYTo(Page)
                .WidthEqualToHeightX(2);
            secondButton.LeftToRight(firstButton).RightToRight(Page).CenterYTo(Page)
                .HeightEqualToWidthX(2);
        }

        private void ChainTest(ConstraintLayout Page)
        {
            Page.Children.Clear();

            Button firstButton = new Button() { Content = "First", Tag = "first", Height = 50 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second",  Height = 50 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Height = 50 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth",  Height = 50 };
            Button fifthButton = new Button() { Content = "Fifth ", Tag = "fifth",  Height = 50 };
            Button sixthButton = new Button() { Content = "Sixth ",  Height = 50 };
            Button seventhButton = new Button() { Content = "Seventh ",  Height = 50 };
            Button eighthButton = new Button() { Content = "Eighth", Height = 50 };
            Button ninthButton = new Button() { Content = "Ninth",  Height = 50 };
            Button tenthButton = new Button() { Content = "tenth",Height = 50 };
            Button eleventhButton = new Button() { Content = "eleventh",  Height = 50 };

            Page.Children.Add(firstButton);
            Page.Children.Add(secondButton);
            Page.Children.Add(thirdButton);
            Page.Children.Add(forthButton);
            Page.Children.Add(fifthButton);
            Page.Children.Add(sixthButton);
            Page.Children.Add(seventhButton);
            Page.Children.Add(eighthButton);
            Page.Children.Add(ninthButton);
            Page.Children.Add(tenthButton);
            Page.Children.Add(eleventhButton);

            //set weight,LeftToX and RightToX must all exist.
            firstButton.LeftToLeft(Page).RightToLeft(secondButton).TopToTop(Page)
                .WidthEqualTo(ConstraintSet.SizeType.MatchConstraint, 1f);
            secondButton.LeftToRight(firstButton).RightToRight(Page).TopToTop(Page)
                .WidthEqualTo(ConstraintSet.SizeType.MatchConstraint,0.5f);
            //set chain spread,LeftToX and RightToX must all exist.
            thirdButton.LeftToLeft(Page).RightToLeft(forthButton).CenterYTo(Page)
                .SetHorizontalChainStyle(ConstraintSet.ChainStyle.ChainSpread);//it is default,you can not set
            forthButton.LeftToRight(thirdButton).RightToLeft(fifthButton). CenterYTo(Page);
            fifthButton.LeftToRight(forthButton).RightToRight(Page).CenterYTo(Page);

            //set chain spread inside,LeftToX and RightToX must all exist.
            sixthButton.LeftToLeft(Page).RightToLeft(seventhButton).TopToBottom(fifthButton,20)
                .SetHorizontalChainStyle(ConstraintSet.ChainStyle.ChainSpreadInside);
            seventhButton.LeftToRight(sixthButton).RightToLeft(eighthButton).TopToTop(sixthButton);
            eighthButton.LeftToRight(seventhButton).RightToRight(Page).TopToTop(sixthButton);

            //set chain packed + bias
            ninthButton.LeftToLeft(Page).RightToLeft(tenthButton).TopToBottom(eighthButton, 20)
                .SetHorizontalChainStyle(ConstraintSet.ChainStyle.ChainPacked, 0.25f);
            tenthButton.LeftToRight(ninthButton).RightToLeft(eleventhButton).TopToTop(ninthButton);
            eleventhButton.LeftToRight(tenthButton).RightToRight(Page).TopToTop(ninthButton);

        }

        private void BarrierTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            //Horizontal
            TextBox firstTextBox = new TextBox() { Text = "First" + Environment.NewLine + "Line2", Tag = "first",TextWrapping=TextWrapping.Wrap,MaxWidth=100, AcceptsReturn = true };
            TextBox secondTextBox = new TextBox() { Text = "Second ", Tag = "second", AcceptsReturn = true };
            Button firstButton = new Button() { Content = "First", Tag = "first" };
            BarrierLine horizontalBarrier = new BarrierLine() { BarrierSide = BarrierLine.Side.Bottom };
            Page.Children.Add(firstTextBox);
            Page.Children.Add(secondTextBox);
            Page.Children.Add(firstButton);
            Page.Children.Add(horizontalBarrier);
            horizontalBarrier.AddView(firstTextBox);
            horizontalBarrier.AddView(secondTextBox);
            firstTextBox.CenterTo(Page);
            secondTextBox.LeftToRight(firstTextBox,20).TopToTop(firstTextBox);
            firstButton.LeftToLeft(firstTextBox,20).TopToBottom(horizontalBarrier);
            //Vertical
            TextBox thirdTextBox = new TextBox() { Text = "Third ", Tag = "third", AcceptsReturn = true };
            Button secondButton = new Button() { Content = "Second ", Tag = "second" };
            BarrierLine verticalBarrier = new BarrierLine() { BarrierSide = BarrierLine.Side.Right };
            Page.Children.Add(thirdTextBox);
            Page.Children.Add(secondButton);
            Page.Children.Add(verticalBarrier);
            verticalBarrier.AddView(secondTextBox);
            verticalBarrier.AddView(thirdTextBox);
            thirdTextBox.LeftToLeft(secondTextBox).BottomToTop(secondTextBox);
            secondButton.LeftToRight(verticalBarrier).TopToTop(thirdTextBox);
        }

        private void NewApiTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 600, Height = 300};
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 60, Height = 50 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 60, Height = 50 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 60, Height = 50 };
            Button fifthButton = new Button() { Content = "Fifth ",  Width = 60, Height = 50 };
            Button sixthButton = new Button() { Content = "Sixth ",  Width = 60, Height = 50 };
            Button seventhButton = new Button() { Content = "Seventh ", Width = 60, Height = 50 };
            Button eighthButton = new Button() { Content = "Eighth", Width = 60, Height = 50 };
            Button ninthButton = new Button() { Content = "Ninth", Width = 60, Height = 50 };
            Button tenthButton = new Button() { Content = "tenth", Width = 60, Height = 50 };
            Button eleventhButton = new Button() { Content = "eleventh", Width = 60, Height = 50 };
            Button twelfthButton = new Button() { Content = "twelfth", Width = 60, Height = 50 };
            Button thirteenthButton = new Button() { Content = "thirteenth", Width = 60, Height = 50 };
            Button fourteenthButton = new Button() { Content = "fourteenth", Width = 60, Height = 50 };
            Button fifteenthButton = new Button() { Content = "fifteenth", Width = 60, Height = 50 };
            Button sixteenthButton = new Button() { Content = "sixteenth", Width = 60, Height = 50 };
            Button seventeenthButton = new Button() { Content = "seventeenth", Width = 60, Height = 50 };

            Page.Children.Add(firstButton);
            Page.Children.Add(secondButton);
            Page.Children.Add(thirdButton);
            Page.Children.Add(forthButton);
            Page.Children.Add(fifthButton);
            Page.Children.Add(sixthButton);
            Page.Children.Add(seventhButton);
            Page.Children.Add(eighthButton);
            Page.Children.Add(ninthButton);
            Page.Children.Add(tenthButton);
            Page.Children.Add(eleventhButton);
            Page.Children.Add(twelfthButton);
            Page.Children.Add(thirteenthButton);
            Page.Children.Add(fourteenthButton);
            Page.Children.Add(fifteenthButton);
            Page.Children.Add(sixteenthButton);
            Page.Children.Add(seventeenthButton);
            

            firstButton.CenterTo(Page);

            //LeftToX, At toView Left is negative
            secondButton.LeftToLeft(firstButton,-20).BottomToTop(firstButton,60);
            thirdButton.LeftToLeft(firstButton,20).BottomToTop(firstButton,5);

            forthButton.LeftToRight(firstButton,-20).BottomToTop(firstButton,60);
            fifthButton.LeftToRight(firstButton,20).BottomToTop(firstButton,5);

            //RightToX, At toView Right is negative
            var HorizontalCenterGuidline = new GuideLine() { Percent = 0.5f,Orientation=GuideLine.OrientationType.Horizontal };
            Page.Children.Add(HorizontalCenterGuidline);
            sixthButton.RightToLeft(firstButton, -20).BottomToTop(HorizontalCenterGuidline);
            seventhButton.RightToLeft(firstButton,20).TopToBottom(HorizontalCenterGuidline);
        
            eighthButton.RightToRight(firstButton,-20).BottomToTop(HorizontalCenterGuidline);
            ninthButton.RightToRight(firstButton,20).TopToBottom(HorizontalCenterGuidline);

            var VerticalCenterGuideline = new GuideLine() { Percent = 0.5f,Orientation=GuideLine.OrientationType.Vertical };
            Page.Children.Add(VerticalCenterGuideline);
            //TopToX, At toView Top is negative
            tenthButton.RightToLeft(eleventhButton).TopToTop(firstButton, -20);
            eleventhButton.RightToLeft(VerticalCenterGuideline,20).TopToTop(firstButton, 20);

            twelfthButton.RightToLeft(thirteenthButton).TopToBottom(firstButton, -20);
            thirteenthButton.RightToLeft(VerticalCenterGuideline,20).TopToBottom(firstButton, 20);
            //BottomToX, At toView Bottom is negative
            fourteenthButton.LeftToRight(VerticalCenterGuideline, 20).BottomToTop(firstButton, -20);
            fifteenthButton.LeftToRight(fourteenthButton).BottomToTop(firstButton, 20);

            sixteenthButton.LeftToRight(VerticalCenterGuideline, 20).BottomToBottom(firstButton, -20);
            seventeenthButton.LeftToRight(sixteenthButton).BottomToBottom(firstButton, 20);
        }

        private void WrapPanelNestContraintLayoutTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            var wrapPanel = new WrapPanel()
            {
                Background = new SolidColorBrush(Colors.Pink),
            };
            Page.Children.Add(wrapPanel);
            new ConstraintSet(Page)
                .AddConnect(wrapPanel, ConstraintSet.Side.Left, Page, ConstraintSet.Side.Left)
                .AddConnect(wrapPanel, ConstraintSet.Side.Top, Page, ConstraintSet.Side.Top)
                .SetWidth(wrapPanel, ConstraintSet.SizeType.WrapContent)
                .SetHeight(wrapPanel, ConstraintSet.SizeType.WrapContent);
            var constraintlayout = new ConstraintLayout()
            {
                Tag = "constraintlayout",
                Background = new SolidColorBrush(Colors.RosyBrown),
                //DEBUG = true,
                WidthType = ConstraintSet.SizeType.WrapContent,
                HeightType = ConstraintSet.SizeType.WrapContent,
            };

            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 100, Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100, Height = 100 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 100, Height = 100 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 100, Height = 100 };

            wrapPanel.Children.Add(constraintlayout);
            wrapPanel.Children.Add(firstButton);
            wrapPanel.Children.Add(secondButton);

            constraintlayout.Children.Add(thirdButton);
            constraintlayout.Children.Add(forthButton);

            new ConstraintSet(constraintlayout)
                .AddConnect(thirdButton, ConstraintSet.Side.Center, constraintlayout, ConstraintSet.Side.Center)
                //.AddConnect(thirdButton, ConstraintSet.Side.Bottom, forthButton, ConstraintSet.Side.Top)
                .AddConnect(forthButton,ConstraintSet.Side.Left,thirdButton,ConstraintSet.Side.Right)
                .AddConnect(forthButton, ConstraintSet.Side.Top,thirdButton,ConstraintSet.Side.Bottom)
                ;

        }

        private void ListViewNestConstraintTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            var listView = new ListView()
            {
                Background = new SolidColorBrush(Colors.Pink),
                /*ItemTemplate = new DataTemplate()
                {
                    DataType = typeof(Contact),
                    VisualTree = new FrameworkElementFactory
                },*/
                Tag = "listview",
                //HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            listView.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            Page.Children.Add(listView);
            new ConstraintSet(Page)
                .AddConnect(listView, ConstraintSet.Side.Center, Page, ConstraintSet.Side.Center)
                .SetWidth(listView, ConstraintSet.SizeType.MatchParent)
                .SetHeight(listView, ConstraintSet.SizeType.MatchParent);
            for (var index = 0; index < 5000; index++)
            {
                //var item1 = new ListViewItem {
                //    Content = $"This is {index} item added programmatically." 
                //};
                //listView.Items.Add(item1);
                var item = new ConstraintLayout()
                {
                    //DEBUG=true,
                    Tag = "listview_item",
                    Background = new SolidColorBrush(Colors.Brown),
                    HeightType = ConstraintSet.SizeType.WrapContent,
                    WidthType = ConstraintSet.SizeType.WrapContent,
                };
                listView.Items.Add(item);
                Button firstButton = new Button() { Tag = "firstButton", Content = $"{index} Button" };
                TextBlock secondTextBlock = new TextBlock() { Tag = "secondTestBlock", Text = $"{index} Second TextBlock" };
                TextBlock thirdTextBlock = new TextBlock() { Tag = "thirdTextBlock", Text = $"{index} Third TextBlock" };
                item.Children.Add(firstButton);
                item.Children.Add(secondTextBlock);
                item.Children.Add(thirdTextBlock);
                new ConstraintSet(item)
                    //.AddConnect(firstButton, ConstraintSet.Side.Center_Y, item, ConstraintSet.Side.Center_Y)
                    .AddConnect(firstButton, ConstraintSet.Side.Left, item, ConstraintSet.Side.Left, 20)
                    .AddConnect(firstButton, ConstraintSet.Side.Top, item, ConstraintSet.Side.Top, 20)
                    .AddConnect(firstButton, ConstraintSet.Side.Bottom, item, ConstraintSet.Side.Bottom, 20)
                    .SetHeight(firstButton, ConstraintSet.SizeType.MatchConstraint)
                    .AddConnect(secondTextBlock, ConstraintSet.Side.Left, firstButton, ConstraintSet.Side.Right, 20)
                    .AddConnect(secondTextBlock, ConstraintSet.Side.Top, firstButton, ConstraintSet.Side.Top)
                    .AddConnect(thirdTextBlock, ConstraintSet.Side.Left, secondTextBlock, ConstraintSet.Side.Left)
                    .AddConnect(thirdTextBlock, ConstraintSet.Side.Top, secondTextBlock, ConstraintSet.Side.Bottom, 20)
                    .AddConnect(thirdTextBlock, ConstraintSet.Side.Bottom, item, ConstraintSet.Side.Bottom, 20)
                    ;
            }
        }


        private void NestListViewTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            var listView = new ListView()
            {
                Background = new SolidColorBrush(Colors.Pink),
            };
            Page.Children.Add(listView);
            new ConstraintSet(Page)
                .AddConnect(listView, ConstraintSet.Side.Center, Page, ConstraintSet.Side.Center)
                .SetWidth(listView, ConstraintSet.SizeType.MatchParent)
                .SetHeight(listView, ConstraintSet.SizeType.MatchParent);
            for (var index = 0; index < 5000; index++)
            {
                var item1 = new ListViewItem { Content = $"This is {index} item added programmatically." };
                listView.Items.Add(item1);
            }
        }

        private void GridNestedConstaintLayoutTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            Grid grid = new Grid()
            {
                Background = new SolidColorBrush(Colors.Pink),
            };
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            Page.Children.Add(grid);
            new ConstraintSet(Page)
                .AddConnect(grid, ConstraintSet.Side.Center, Page, ConstraintSet.Side.Center)
                .SetWidth(grid, ConstraintSet.SizeType.MatchParent)
                .SetHeight(grid, ConstraintSet.SizeType.MatchParent);
            var constraintlayout = new ConstraintLayout()
            {
                Tag = "constraintlayout",
                Background = new SolidColorBrush(Colors.RosyBrown),
                DEBUG = true,
            };
            //constraintlayout.Root.VerticalDimensionBehaviour = ConstraintSet.SizeType.Wrap_Content;
            //constraintlayout.Root.HorizontalDimensionBehaviour = ConstraintSet.SizeType.Wrap_Content;

            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 100, Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100, Height = 100 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 100, Height = 100 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 100, Height = 100 };

            grid.Children.Add(constraintlayout);
            Grid.SetRow(constraintlayout, 1);
            
            constraintlayout.Children.Add(thirdButton);
            constraintlayout.Children.Add(forthButton);

            new ConstraintSet(constraintlayout)
                .AddConnect(thirdButton, ConstraintSet.Side.Center, constraintlayout, ConstraintSet.Side.Center)
                //.AddConnect(thirdButton, ConstraintSet.Side.Bottom, forthButton, ConstraintSet.Side.Top)
                .AddConnect(forthButton, ConstraintSet.Side.Left, thirdButton, ConstraintSet.Side.Right)
                .AddConnect(forthButton, ConstraintSet.Side.Top, thirdButton, ConstraintSet.Side.Bottom)
                ;
        }

        private void RemoveViewTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 100, Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100, Height = 100 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 100, Height = 100 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 100, Height = 100 };

            Page.Children.Add(firstButton);
            Page.Children.Add(secondButton);
            Page.Children.Add(thirdButton);
            Page.Children.Add(forthButton);

            ConstraintSet set = new ConstraintSet(Page);

            set.AddConnect(firstButton, ConstraintSet.Side.Center, Page, ConstraintSet.Side.Center)
                .AddConnect(secondButton, ConstraintSet.Side.Left, firstButton, ConstraintSet.Side.Right)
                .AddConnect(secondButton, ConstraintSet.Side.Top, firstButton, ConstraintSet.Side.Top)
                .AddConnect(thirdButton, ConstraintSet.Side.Left, secondButton, ConstraintSet.Side.Right)
                .AddConnect(thirdButton, ConstraintSet.Side.Top, secondButton, ConstraintSet.Side.Top)
                .AddConnect(forthButton, ConstraintSet.Side.Left, thirdButton, ConstraintSet.Side.Right)
                .AddConnect(forthButton, ConstraintSet.Side.Top, firstButton, ConstraintSet.Side.Top);

            Page.InvalidateVisual();

            //remove view,this will remove view at visualtree and remove all constraint related to the view.
            firstButton.Click += (sender, e) =>
            {
                Page.Children.Remove(thirdButton);
                Page.InvalidateMeasure();

            };
            //add view, we need add it's constraint,and other view's constraint rely on it.
            secondButton.Click += (sender, e) =>
            {
                Page.Children.Add(thirdButton);
                set.AddConnect(thirdButton, ConstraintSet.Side.Left, secondButton, ConstraintSet.Side.Right, 50)
                    .AddConnect(thirdButton, ConstraintSet.Side.Top, secondButton, ConstraintSet.Side.Top)
                    .AddConnect(forthButton, ConstraintSet.Side.Left, thirdButton, ConstraintSet.Side.Right);
                Page.InvalidateMeasure();
            };
        }

        /// <summary>
        /// Remove a view's Constraint then add.
        /// </summary>
        /// <param name="Page"></param>
        private void RemoveConstraintTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 100, Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100, Height = 100 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 100, Height = 100 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 100, Height = 100 };

            Page.Children.Add(firstButton);
            Page.Children.Add(secondButton);
            Page.Children.Add(thirdButton);
            Page.Children.Add(forthButton);

            ConstraintSet set = new ConstraintSet(Page);

            set.AddConnect(firstButton, ConstraintSet.Side.Center, Page, ConstraintSet.Side.Center)
                .AddConnect(secondButton, ConstraintSet.Side.Left, firstButton, ConstraintSet.Side.Right)
                .AddConnect(secondButton, ConstraintSet.Side.Top, firstButton, ConstraintSet.Side.Top)
                .AddConnect(thirdButton, ConstraintSet.Side.Left, secondButton, ConstraintSet.Side.Right)
                .AddConnect(thirdButton, ConstraintSet.Side.Top, secondButton, ConstraintSet.Side.Top)
                .AddConnect(forthButton, ConstraintSet.Side.Left, thirdButton, ConstraintSet.Side.Right)
                .AddConnect(forthButton, ConstraintSet.Side.Top, thirdButton, ConstraintSet.Side.Top);

            Page.InvalidateVisual();

            //remove constraint
            firstButton.Click += (sender, e) =>
            {
                set.Clear(thirdButton);
                Page.InvalidateMeasure();
            };
            //add constraint
            secondButton.Click += (sender, e) =>
            {
                set.AddConnect(thirdButton, ConstraintSet.Side.Left, secondButton, ConstraintSet.Side.Right, 50)
            .AddConnect(thirdButton, ConstraintSet.Side.Top, secondButton, ConstraintSet.Side.Top);
                Page.InvalidateMeasure();
            };
        }

        /// <summary>
        /// ----------
        /// |        |
        /// ----------
        /// | |    | |
        /// | |    | |
        /// | |    | |
        /// ----------
        /// |        |
        /// ----------
        /// </summary>
        /// <param name="Page"></param>
        private void NestedConstraintLayoutTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            var leftPanel = new ConstraintLayout() { Width = 100, Background = new SolidColorBrush(Colors.Green), Tag = "leftPanel" };
            var topPanel = new ConstraintLayout() { Height = 50, Background = new SolidColorBrush(Colors.Red), Tag = "topPanel" };
            var centerPanel = new ConstraintLayout() { Background = new SolidColorBrush(Colors.White), Tag = "centerPanel" };
            var rightPanel = new ConstraintLayout() { Width = 100, Background = new SolidColorBrush(Colors.Blue), Tag = "rightPanel" };
            var bottomPanel = new ConstraintLayout() { Height = 50, Background = new SolidColorBrush(Colors.LightPink), Tag = "bottomPanel" };

            Page.Children.Add(leftPanel);
            Page.Children.Add(topPanel);
            Page.Children.Add(centerPanel);
            Page.Children.Add(rightPanel);
            Page.Children.Add(bottomPanel);

            new ConstraintSet(Page)
                .AddConnect(leftPanel, ConstraintSet.Side.Left, Page, ConstraintSet.Side.Left)
                .AddConnect(leftPanel, ConstraintSet.Side.Right, centerPanel, ConstraintSet.Side.Left)
                .AddConnect(leftPanel, ConstraintSet.Side.Top, topPanel, ConstraintSet.Side.Bottom)
                .AddConnect(leftPanel, ConstraintSet.Side.Bottom, bottomPanel, ConstraintSet.Side.Top)
                .SetHeight(leftPanel, ConstraintSet.SizeType.MatchConstraint)

                .AddConnect(topPanel, ConstraintSet.Side.Left, Page, ConstraintSet.Side.Left)
                .AddConnect(topPanel, ConstraintSet.Side.Top, Page, ConstraintSet.Side.Top)
                .SetWidth(topPanel, ConstraintSet.SizeType.MatchParent)

                .AddConnect(rightPanel, ConstraintSet.Side.Right, Page, ConstraintSet.Side.Right)
                .AddConnect(rightPanel, ConstraintSet.Side.Left, centerPanel, ConstraintSet.Side.Right)
                .AddConnect(rightPanel, ConstraintSet.Side.Top, topPanel, ConstraintSet.Side.Bottom)
                .AddConnect(rightPanel, ConstraintSet.Side.Bottom, bottomPanel, ConstraintSet.Side.Top)
                .SetHeight(rightPanel, ConstraintSet.SizeType.MatchConstraint)

                .AddConnect(bottomPanel, ConstraintSet.Side.Bottom, Page, ConstraintSet.Side.Bottom)
                .SetWidth(bottomPanel, ConstraintSet.SizeType.MatchParent)

                .AddConnect(centerPanel, ConstraintSet.Side.Top, topPanel, ConstraintSet.Side.Bottom)
                .AddConnect(centerPanel, ConstraintSet.Side.Bottom, bottomPanel, ConstraintSet.Side.Top)
                .AddConnect(centerPanel, ConstraintSet.Side.Left, leftPanel, ConstraintSet.Side.Right)
                .AddConnect(centerPanel, ConstraintSet.Side.Right, rightPanel, ConstraintSet.Side.Left)
                .SetWidth(centerPanel, ConstraintSet.SizeType.MatchConstraint)
                .SetHeight(centerPanel, ConstraintSet.SizeType.MatchConstraint)

                ;

            topPanel.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    //相对于WIndow位置
                    Window window = Window.GetWindow(topPanel);
                    Point point = topPanel.TransformToAncestor(window).Transform(new Point(0, 0));
                    // Configure the message box to be displayed
                    string caption = "Test Result";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show($"topPanel:Actual Size ({point.X},{point.Y},{topPanel.ActualWidth},{topPanel.ActualHeight}),DesiredSize ({topPanel.DesiredSize.Width},{topPanel.DesiredSize.Height})", caption, button, icon);
                }
            };

            centerPanel.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    //相对于WIndow位置
                    Window window = Window.GetWindow(rightPanel);
                    Point point = rightPanel.TransformToAncestor(window).Transform(new Point(0, 0));
                    // Configure the message box to be displayed
                    string caption = "Test Result";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show($"rightPanel:Actual Size ({point.X},{point.Y},{rightPanel.ActualWidth},{rightPanel.ActualHeight}),DesiredSize ({rightPanel.DesiredSize.Width},{rightPanel.DesiredSize.Height})", caption, button, icon);
                }
            };

            leftPanel.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    //相对于WIndow位置
                    Window window = Window.GetWindow(bottomPanel);
                    Point point = bottomPanel.TransformToAncestor(window).Transform(new Point(0, 0));
                    // Configure the message box to be displayed
                    string caption = "Test Result";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show($"bottomPanel:Actual Size ({point.X},{point.Y},{bottomPanel.ActualWidth},{bottomPanel.ActualHeight}),DesiredSize ({bottomPanel.DesiredSize.Width},{bottomPanel.DesiredSize.Height})", caption, button, icon);
                }
            };
        }

        /// <summary>
        /// 嵌套Grid
        /// </summary>
        /// <param name="Page"></param>
        private void NestedGridTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            var leftPanel = new ConstraintLayout() { Width = 100, Background = new SolidColorBrush(Colors.Green), Tag = "leftPanel" };
            var topPanel = new ConstraintLayout() { Height = 50, Background = new SolidColorBrush(Colors.Red), Tag = "topPanel" };
            var centerPanel = new ConstraintLayout() { Background = new SolidColorBrush(Colors.White), Tag = "centerPanel" };
            var rightPanel = new ConstraintLayout() { Width = 100, Background = new SolidColorBrush(Colors.Blue), Tag = "rightPanel" };
            var bottomPanel = new ConstraintLayout() { Height = 50, Background = new SolidColorBrush(Colors.LightPink), Tag = "bottomPanel" };

            Page.Children.Add(leftPanel);
            Page.Children.Add(topPanel);
            Page.Children.Add(centerPanel);
            Page.Children.Add(rightPanel);
            Page.Children.Add(bottomPanel);

            new ConstraintSet(Page)
                .AddConnect(leftPanel, ConstraintSet.Side.Left, Page, ConstraintSet.Side.Left)
                .AddConnect(leftPanel, ConstraintSet.Side.Right, centerPanel, ConstraintSet.Side.Left)
                .AddConnect(leftPanel, ConstraintSet.Side.Top, topPanel, ConstraintSet.Side.Bottom)
                .AddConnect(leftPanel, ConstraintSet.Side.Bottom, bottomPanel, ConstraintSet.Side.Top)
                .SetHeight(leftPanel, ConstraintSet.SizeType.MatchConstraint)

                .AddConnect(topPanel, ConstraintSet.Side.Left, Page, ConstraintSet.Side.Left)
                .AddConnect(topPanel, ConstraintSet.Side.Top, Page, ConstraintSet.Side.Top)
                .SetWidth(topPanel, ConstraintSet.SizeType.MatchParent)

                .AddConnect(rightPanel, ConstraintSet.Side.Right, Page, ConstraintSet.Side.Right)
                .AddConnect(rightPanel, ConstraintSet.Side.Left, centerPanel, ConstraintSet.Side.Right)
                .AddConnect(rightPanel, ConstraintSet.Side.Top, topPanel, ConstraintSet.Side.Bottom)
                .AddConnect(rightPanel, ConstraintSet.Side.Bottom, bottomPanel, ConstraintSet.Side.Top)
                .SetHeight(rightPanel, ConstraintSet.SizeType.MatchConstraint)

                .AddConnect(bottomPanel, ConstraintSet.Side.Bottom, Page, ConstraintSet.Side.Bottom)
                .SetWidth(bottomPanel, ConstraintSet.SizeType.MatchParent)

                .AddConnect(centerPanel, ConstraintSet.Side.Top, topPanel, ConstraintSet.Side.Bottom)
                .AddConnect(centerPanel, ConstraintSet.Side.Bottom, bottomPanel, ConstraintSet.Side.Top)
                .AddConnect(centerPanel, ConstraintSet.Side.Left, leftPanel, ConstraintSet.Side.Right)
                .AddConnect(centerPanel, ConstraintSet.Side.Right, rightPanel, ConstraintSet.Side.Left)
                .SetWidth(centerPanel, ConstraintSet.SizeType.MatchConstraint)
                .SetHeight(centerPanel, ConstraintSet.SizeType.MatchConstraint)

                ;

            topPanel.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    //topPanel相对于WIndow位置
                    Window window = Window.GetWindow(topPanel);
                    Point point = topPanel.TransformToAncestor(window).Transform(new Point(0, 0));
                    // Configure the message box to be displayed
                    string caption = "Test Result";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show($"topPanel:Actual Size ({point.X},{point.Y},{topPanel.ActualWidth},{topPanel.ActualHeight}),DesiredSize ({topPanel.DesiredSize.Width},{topPanel.DesiredSize.Height})", caption, button, icon);
                }
            };

            centerPanel.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    //centerPanel相对于WIndow位置
                    Window window = Window.GetWindow(centerPanel);
                    Point point = centerPanel.TransformToAncestor(window).Transform(new Point(0, 0));
                    // Configure the message box to be displayed
                    string caption = "Test Result";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show($"centerPanel:Actual Size ({point.X},{point.Y},{centerPanel.ActualWidth},{centerPanel.ActualHeight}),DesiredSize ({centerPanel.DesiredSize.Width},{centerPanel.DesiredSize.Height})", caption, button, icon);
                }
            };

            leftPanel.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount >= 2)
                {
                    //bottomPanel相对于WIndow位置
                    Window window = Window.GetWindow(leftPanel);
                    Point point = leftPanel.TransformToAncestor(window).Transform(new Point(0, 0));
                    // Configure the message box to be displayed
                    string caption = "Test Result";
                    MessageBoxButton button = MessageBoxButton.YesNoCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;

                    MessageBox.Show($"leftPanel:Actual Size ({point.X},{point.Y},{leftPanel.ActualWidth},{leftPanel.ActualHeight}),DesiredSize ({leftPanel.DesiredSize.Width},{leftPanel.DesiredSize.Height})", caption, button, icon);
                }
            };

            var firstButton = new Button() { Content = "first", Tag = "first" };
            var secondButton = new Button() { Content = "second", Tag = "second" };
            var thirdButton = new Button() { Content = "third", Tag = "third" };
            var forthButton = new Button() { Content = "forth", Tag = "forth" };

            var grid = new Grid() { Tag = "grid" };
            grid.Background = new SolidColorBrush(Colors.Brown);

            centerPanel.Children.Add(grid);
            centerPanel.Children.Add(firstButton);
            centerPanel.Children.Add(secondButton);



            new ConstraintSet(centerPanel)
                //在centerPanel右上角
                .AddConnect(firstButton, ConstraintSet.Side.Right, centerPanel, ConstraintSet.Side.Right, 20)
                .AddConnect(firstButton, ConstraintSet.Side.Top, centerPanel, ConstraintSet.Side.Top, 20)
                //在centerPanel左下角
                .AddConnect(grid, ConstraintSet.Side.Left, centerPanel, ConstraintSet.Side.Left, 20)
                .AddConnect(grid, ConstraintSet.Side.Top, firstButton, ConstraintSet.Side.Bottom, 20)
                .AddConnect(grid, ConstraintSet.Side.Right, firstButton, ConstraintSet.Side.Left, 20)
                .AddConnect(grid, ConstraintSet.Side.Bottom, centerPanel, ConstraintSet.Side.Bottom, 20)
                .SetWidth(grid, ConstraintSet.SizeType.MatchConstraint)
                .SetHeight(grid, ConstraintSet.SizeType.MatchConstraint)
                //在Grid右下角
                .AddConnect(secondButton, ConstraintSet.Side.Right, grid, ConstraintSet.Side.Right, 20)
                .AddConnect(secondButton, ConstraintSet.Side.Bottom, grid, ConstraintSet.Side.Bottom, 20)
                ;

            grid.Children.Add(thirdButton);
            grid.Children.Add(forthButton);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetRow(thirdButton, 1);
            Grid.SetColumn(thirdButton, 1);

            firstButton.Click += (sender, e) =>
            {
                //相对于WIndow位置
                Window window = Window.GetWindow(firstButton);
                Point point = firstButton.TransformToAncestor(window).Transform(new Point(0, 0));
                // Configure the message box to be displayed
                string caption = "Test Result";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;

                MessageBox.Show($"firstButton:Actual Size ({point.X},{point.Y},{firstButton.ActualWidth},{firstButton.ActualHeight}),DesiredSize ({firstButton.DesiredSize.Width},{firstButton.DesiredSize.Height})", caption, button, icon);
            };

        }
    }
}
