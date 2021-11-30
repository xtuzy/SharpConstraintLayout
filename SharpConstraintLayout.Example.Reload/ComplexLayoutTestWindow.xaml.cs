using androidx.constraintlayout.core.widgets;
using SharpConstraintLayout.Wpf;
using System;
using System.Collections.Generic;
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
                .AddConnect(toolBar, ConstraintAnchor.Type.TOP, page, ConstraintAnchor.Type.TOP)
                .SetWidth(toolBar, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                .SetHeight(toolBar, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                .AddConnect(testPanel, ConstraintAnchor.Type.TOP, toolBar, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(testPanel, ConstraintAnchor.Type.BOTTOM, page, ConstraintAnchor.Type.BOTTOM)
                .SetWidth(testPanel, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                .SetHeight(testPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
            //工具栏详细设置
            var RemoveConstraintTestButton = new Button() { Content = "RemoveConstraintTest", Margin = new Thickness(10, 5, 0, 5) };
            var RemoveViewTestButton = new Button() { Content = "RemoveViewTest", Margin = new Thickness(10, 5, 0, 5) };
            var VisibilityTestButton = new Button() { Content = "VisibilityTest", Margin = new Thickness(10, 5, 0, 5) };
            var NestedConstraintLayoutTestButton = new Button() { Content = "NestedConstraintLayoutTest", Margin = new Thickness(10, 5, 0, 5) };
            var NestedGridTestButton = new Button() { Content = "NestedGridTest", Margin = new Thickness(10, 5, 0, 5) };
            var GridNestedConstaintLayoutTestButton = new Button() { Content = "GridNestedConstaintLayoutTest", Margin = new Thickness(10, 5, 0, 5) };
            var NestListViewTestButton = new Button() { Content = "NestListViewTest", Margin = new Thickness(10, 5, 0, 5) };
            var ListViewNestConstraintTestButton = new Button() { Content = "ListViewNestConstraintTest", Margin = new Thickness(10, 5, 0, 5) };
            var GridCompareConstraintNestListViewTestButton = new Button() { Content = "GridCompareConstraintNestListViewTest", Margin = new Thickness(10, 5, 0, 5) };
            var WarpPanelNestContraintLayoutTestButton = new Button() { Content = "WarpPanelNestContraintLayoutTest", Margin = new Thickness(10, 5, 0, 5) };
            var NewApiTestButton = new Button() { Content = "NewApiTest", Margin = new Thickness(10, 5, 0, 5) };
            var BarrierTestButton = new Button() { Content = "BarrierTest", Margin = new Thickness(10, 5, 0, 5) };
            var ChainTestButton = new Button() { Content = "ChainTest", Margin = new Thickness(10, 5, 0, 5) };
            var WidthHeightRatioTestButton = new Button() { Content = "WidthHeightRatioTest", Margin = new Thickness(10, 5, 0, 5) };
            var CircleTestButton = new Button() { Content = "CircleTest", Margin = new Thickness(10, 5, 0, 5) };
            


            toolBar.Children.Add(RemoveConstraintTestButton);
            toolBar.Children.Add(RemoveViewTestButton);
            toolBar.Children.Add(VisibilityTestButton);
            toolBar.Children.Add(NestedConstraintLayoutTestButton);

            toolBar.Children.Add(NestedGridTestButton);
            toolBar.Children.Add(GridNestedConstaintLayoutTestButton);

            toolBar.Children.Add(NestListViewTestButton);
            toolBar.Children.Add(ListViewNestConstraintTestButton);

            toolBar.Children.Add(GridCompareConstraintNestListViewTestButton);

            toolBar.Children.Add(WarpPanelNestContraintLayoutTestButton);

            toolBar.Children.Add(NewApiTestButton);
            toolBar.Children.Add(BarrierTestButton);
            toolBar.Children.Add(ChainTestButton);
            toolBar.Children.Add(WidthHeightRatioTestButton);
            toolBar.Children.Add(CircleTestButton);

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
            GridCompareConstraintNestListViewTestButton.Click += (sender, e) =>
            {
                new ScrollTestWindow().Show();
            };

            WarpPanelNestContraintLayoutTestButton.Click += (sender, e) =>
            {
                WarpPanelNestContraintLayoutTest(testPanel);
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

        }

        private void CircleTest(ConstraintLayout testPanel)
        {
            throw new NotImplementedException();
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

            set.AddConnect(firstButton, ConstraintAnchor.Type.CENTER, Page, ConstraintAnchor.Type.CENTER)
                .AddConnect(secondButton, ConstraintAnchor.Type.LEFT, firstButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(secondButton, ConstraintAnchor.Type.TOP, firstButton, ConstraintAnchor.Type.TOP)
                .AddConnect(thirdButton, ConstraintAnchor.Type.LEFT, secondButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(thirdButton, ConstraintAnchor.Type.TOP, secondButton, ConstraintAnchor.Type.TOP)
                .AddConnect(forthButton, ConstraintAnchor.Type.LEFT, thirdButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(forthButton, ConstraintAnchor.Type.TOP, firstButton, ConstraintAnchor.Type.TOP);

            set.AddConnect(fifthButton, ConstraintAnchor.Type.LEFT, firstButton, ConstraintAnchor.Type.LEFT)
                .AddConnect(fifthButton, ConstraintAnchor.Type.TOP, firstButton, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(sixthButton, ConstraintAnchor.Type.LEFT, fifthButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(sixthButton, ConstraintAnchor.Type.TOP, fifthButton, ConstraintAnchor.Type.TOP)
                .AddConnect(seventhButton, ConstraintAnchor.Type.LEFT, sixthButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(seventhButton, ConstraintAnchor.Type.TOP, fifthButton, ConstraintAnchor.Type.TOP)
                .AddConnect(eighthButton, ConstraintAnchor.Type.LEFT, seventhButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(eighthButton, ConstraintAnchor.Type.TOP, fifthButton, ConstraintAnchor.Type.TOP);

            Page.InvalidateVisual();

            //gone
            firstButton.Click += (sender, e) =>
            {
                thirdButton.SetVisibility(ConstraintSet.Visibility.Gone);
                Page.InvalidateMeasure();

            };
            //visible
            secondButton.Click += (sender, e) =>
            {
                thirdButton.SetVisibility(ConstraintSet.Visibility.Visible);
                Page.InvalidateMeasure();
            };
            //invisible
            fifthButton.Click += (sender, e) =>
            {
                seventhButton.SetVisibility(ConstraintSet.Visibility.Invisible);
                Page.InvalidateMeasure();

            };
            //visible
            sixthButton.Click += (sender, e) =>
            {
                seventhButton.SetVisibility(ConstraintSet.Visibility.Visible);
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
            firstButton.LeftToLeft(Page).RightToLeft(secondButton).CenterY(Page)
                .SetWidthBaseOnHeight(2);
            secondButton.LeftToRight(firstButton).RightToRight(Page).CenterY(Page)
                .SetHeightBaseOnWidth(2);
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
                .SetWidth(ConstraintSet.SizeType.Match_Constraint, 1f);
            secondButton.LeftToRight(firstButton).RightToRight(Page).TopToTop(Page)
                .SetWidth(ConstraintSet.SizeType.Match_Constraint,0.5f);
            //set chain spread,LeftToX and RightToX must all exist.
            thirdButton.LeftToLeft(Page).RightToLeft(forthButton).CenterY(Page)
                .SetHorizontalLayoutStyle(ConstraintSet.LayoutStyle.Chain_Spread);//it is default,you can not set
            forthButton.LeftToRight(thirdButton).RightToLeft(fifthButton). CenterY(Page);
            fifthButton.LeftToRight(forthButton).RightToRight(Page).CenterY(Page);

            //set chain spread inside,LeftToX and RightToX must all exist.
            sixthButton.LeftToLeft(Page).RightToLeft(seventhButton).TopToBottom(fifthButton,20)
                .SetHorizontalLayoutStyle(ConstraintSet.LayoutStyle.Chain_Spread_Inside);
            seventhButton.LeftToRight(sixthButton).RightToLeft(eighthButton).TopToTop(sixthButton);
            eighthButton.LeftToRight(seventhButton).RightToRight(Page).TopToTop(sixthButton);

            //set chain packed + bias
            ninthButton.LeftToLeft(Page).RightToLeft(tenthButton).TopToBottom(eighthButton, 20)
                .SetHorizontalLayoutStyle(ConstraintSet.LayoutStyle.Chain_Packed, 0.25f);
            tenthButton.LeftToRight(ninthButton).RightToLeft(eleventhButton).TopToTop(ninthButton);
            eleventhButton.LeftToRight(tenthButton).RightToRight(Page).TopToTop(ninthButton);

        }

        private void BarrierTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            TextBox firstTextBox = new TextBox() { Text = "Line1" + Environment.NewLine + "Line2", Tag = "first",TextWrapping=TextWrapping.Wrap,MaxWidth=100, AcceptsReturn = true };
            TextBox secondTextBox = new TextBox() { Text = "Second ", Tag = "second", AcceptsReturn = true };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third" };

            Page.Children.Add(firstTextBox);
            Page.Children.Add(secondTextBox);
            Page.Children.Add(thirdButton);

            BarrierLine barrier = new BarrierLine() { BarrierSide = BarrierLine.Side.Bottom };
            Page.Children.Add(barrier);
            barrier.AddView(firstTextBox);
            barrier.AddView(secondTextBox);
            firstTextBox.Center(Page);
            secondTextBox.LeftToRight(firstTextBox,20).TopToTop(firstTextBox);
            thirdButton.LeftToLeft(firstTextBox,20).TopToBottom(barrier);
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
            

            firstButton.Center(Page);

            //LeftToX, At toView Left is negative
            secondButton.LeftToLeft(firstButton,-20).BottomToTop(firstButton,60);
            thirdButton.LeftToLeft(firstButton,20).BottomToTop(firstButton,5);

            forthButton.LeftToRight(firstButton,-20).BottomToTop(firstButton,60);
            fifthButton.LeftToRight(firstButton,20).BottomToTop(firstButton,5);

            //RightToX, At toView Right is negative
            var HorizontalCenterGuidline = new GuideLine() { Percent = 0.5f,Orientation=GuideLine.Orientations.HORIZONTAL };
            Page.Children.Add(HorizontalCenterGuidline);
            sixthButton.RightToLeft(firstButton, -20).BottomToTop(HorizontalCenterGuidline);
            seventhButton.RightToLeft(firstButton,20).TopToBottom(HorizontalCenterGuidline);
        
            eighthButton.RightToRight(firstButton,-20).BottomToTop(HorizontalCenterGuidline);
            ninthButton.RightToRight(firstButton,20).TopToBottom(HorizontalCenterGuidline);

            var VerticalCenterGuideline = new GuideLine() { Percent = 0.5f,Orientation=GuideLine.Orientations.VERTICAL };
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

        private void WarpPanelNestContraintLayoutTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            var wrapPanel = new WrapPanel()
            {
                Background = new SolidColorBrush(Colors.Pink),
            };
            Page.Children.Add(wrapPanel);
            new ConstraintSet(Page)
                .AddConnect(wrapPanel, ConstraintAnchor.Type.LEFT, Page, ConstraintAnchor.Type.LEFT)
                .AddConnect(wrapPanel, ConstraintAnchor.Type.TOP, Page, ConstraintAnchor.Type.TOP)
                .SetWidth(wrapPanel, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                .SetHeight(wrapPanel, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
            var constraintlayout = new ConstraintLayout()
            {
                Tag = "constraintlayout",
                Background = new SolidColorBrush(Colors.RosyBrown),
                //DEBUG = true,
            };
            constraintlayout.Root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            constraintlayout.Root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

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
                .AddConnect(thirdButton, ConstraintAnchor.Type.CENTER, constraintlayout, ConstraintAnchor.Type.CENTER)
                //.AddConnect(thirdButton, ConstraintAnchor.Type.BOTTOM, forthButton, ConstraintAnchor.Type.TOP)
                .AddConnect(forthButton,ConstraintAnchor.Type.LEFT,thirdButton,ConstraintAnchor.Type.RIGHT)
                .AddConnect(forthButton, ConstraintAnchor.Type.TOP,thirdButton,ConstraintAnchor.Type.BOTTOM)
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
                .AddConnect(listView, ConstraintAnchor.Type.CENTER, Page, ConstraintAnchor.Type.CENTER)
                .SetWidth(listView, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                .SetHeight(listView, ConstraintWidget.DimensionBehaviour.MATCH_PARENT);
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
                    Background = new SolidColorBrush(Colors.Brown)
                };
                listView.Items.Add(item);
                item.Root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                item.Root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                Button firstButton = new Button() { Tag = "firstButton", Content = $"{index} Button" };
                TextBlock secondTextBlock = new TextBlock() { Tag = "secondTestBlock", Text = $"{index} Second TextBlock" };
                TextBlock thirdTextBlock = new TextBlock() { Tag = "thirdTextBlock", Text = $"{index} Third TextBlock" };
                item.Children.Add(firstButton);
                item.Children.Add(secondTextBlock);
                item.Children.Add(thirdTextBlock);
                new ConstraintSet(item)
                    //.AddConnect(firstButton, ConstraintAnchor.Type.CENTER_Y, item, ConstraintAnchor.Type.CENTER_Y)
                    .AddConnect(firstButton, ConstraintAnchor.Type.LEFT, item, ConstraintAnchor.Type.LEFT, 20)
                    .AddConnect(firstButton, ConstraintAnchor.Type.TOP, item, ConstraintAnchor.Type.TOP, 20)
                    .AddConnect(firstButton, ConstraintAnchor.Type.BOTTOM, item, ConstraintAnchor.Type.BOTTOM, 20)
                    .SetHeight(firstButton, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                    .AddConnect(secondTextBlock, ConstraintAnchor.Type.LEFT, firstButton, ConstraintAnchor.Type.RIGHT, 20)
                    .AddConnect(secondTextBlock, ConstraintAnchor.Type.TOP, firstButton, ConstraintAnchor.Type.TOP)
                    .AddConnect(thirdTextBlock, ConstraintAnchor.Type.LEFT, secondTextBlock, ConstraintAnchor.Type.LEFT)
                    .AddConnect(thirdTextBlock, ConstraintAnchor.Type.TOP, secondTextBlock, ConstraintAnchor.Type.BOTTOM, 20)
                    .AddConnect(thirdTextBlock, ConstraintAnchor.Type.BOTTOM, item, ConstraintAnchor.Type.BOTTOM, 20)
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
                .AddConnect(listView, ConstraintAnchor.Type.CENTER, Page, ConstraintAnchor.Type.CENTER)
                .SetWidth(listView, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                .SetHeight(listView, ConstraintWidget.DimensionBehaviour.MATCH_PARENT);
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
                .AddConnect(grid, ConstraintAnchor.Type.CENTER, Page, ConstraintAnchor.Type.CENTER)
                .SetWidth(grid, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                .SetHeight(grid, ConstraintWidget.DimensionBehaviour.MATCH_PARENT);
            var constraintlayout = new ConstraintLayout()
            {
                Tag = "constraintlayout",
                Background = new SolidColorBrush(Colors.RosyBrown),
                DEBUG = true,
            };
            //constraintlayout.Root.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
            //constraintlayout.Root.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;

            Button firstButton = new Button() { Content = "First", Tag = "first", Width = 100, Height = 100 };
            Button secondButton = new Button() { Content = "Second ", Tag = "second", Width = 100, Height = 100 };
            Button thirdButton = new Button() { Content = "Third ", Tag = "third", Width = 100, Height = 100 };
            Button forthButton = new Button() { Content = "Forth ", Tag = "forth", Width = 100, Height = 100 };

            grid.Children.Add(constraintlayout);
            Grid.SetRow(constraintlayout, 1);
            
            constraintlayout.Children.Add(thirdButton);
            constraintlayout.Children.Add(forthButton);

            new ConstraintSet(constraintlayout)
                .AddConnect(thirdButton, ConstraintAnchor.Type.CENTER, constraintlayout, ConstraintAnchor.Type.CENTER)
                //.AddConnect(thirdButton, ConstraintAnchor.Type.BOTTOM, forthButton, ConstraintAnchor.Type.TOP)
                .AddConnect(forthButton, ConstraintAnchor.Type.LEFT, thirdButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(forthButton, ConstraintAnchor.Type.TOP, thirdButton, ConstraintAnchor.Type.BOTTOM)
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

            set.AddConnect(firstButton, ConstraintAnchor.Type.CENTER, Page, ConstraintAnchor.Type.CENTER)
                .AddConnect(secondButton, ConstraintAnchor.Type.LEFT, firstButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(secondButton, ConstraintAnchor.Type.TOP, firstButton, ConstraintAnchor.Type.TOP)
                .AddConnect(thirdButton, ConstraintAnchor.Type.LEFT, secondButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(thirdButton, ConstraintAnchor.Type.TOP, secondButton, ConstraintAnchor.Type.TOP)
                .AddConnect(forthButton, ConstraintAnchor.Type.LEFT, thirdButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(forthButton, ConstraintAnchor.Type.TOP, firstButton, ConstraintAnchor.Type.TOP);

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
                set.AddConnect(thirdButton, ConstraintAnchor.Type.LEFT, secondButton, ConstraintAnchor.Type.RIGHT, 50)
                    .AddConnect(thirdButton, ConstraintAnchor.Type.TOP, secondButton, ConstraintAnchor.Type.TOP)
                    .AddConnect(forthButton, ConstraintAnchor.Type.LEFT, thirdButton, ConstraintAnchor.Type.RIGHT);
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

            set.AddConnect(firstButton, ConstraintAnchor.Type.CENTER, Page, ConstraintAnchor.Type.CENTER)
                .AddConnect(secondButton, ConstraintAnchor.Type.LEFT, firstButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(secondButton, ConstraintAnchor.Type.TOP, firstButton, ConstraintAnchor.Type.TOP)
                .AddConnect(thirdButton, ConstraintAnchor.Type.LEFT, secondButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(thirdButton, ConstraintAnchor.Type.TOP, secondButton, ConstraintAnchor.Type.TOP)
                .AddConnect(forthButton, ConstraintAnchor.Type.LEFT, thirdButton, ConstraintAnchor.Type.RIGHT)
                .AddConnect(forthButton, ConstraintAnchor.Type.TOP, thirdButton, ConstraintAnchor.Type.TOP);

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
                set.AddConnect(thirdButton, ConstraintAnchor.Type.LEFT, secondButton, ConstraintAnchor.Type.RIGHT, 50)
            .AddConnect(thirdButton, ConstraintAnchor.Type.TOP, secondButton, ConstraintAnchor.Type.TOP);
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
                .AddConnect(leftPanel, ConstraintAnchor.Type.LEFT, Page, ConstraintAnchor.Type.LEFT)
                .AddConnect(leftPanel, ConstraintAnchor.Type.RIGHT, centerPanel, ConstraintAnchor.Type.LEFT)
                .AddConnect(leftPanel, ConstraintAnchor.Type.TOP, topPanel, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(leftPanel, ConstraintAnchor.Type.BOTTOM, bottomPanel, ConstraintAnchor.Type.TOP)
                .SetHeight(leftPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)

                .AddConnect(topPanel, ConstraintAnchor.Type.LEFT, Page, ConstraintAnchor.Type.LEFT)
                .AddConnect(topPanel, ConstraintAnchor.Type.TOP, Page, ConstraintAnchor.Type.TOP)
                .SetWidth(topPanel, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)

                .AddConnect(rightPanel, ConstraintAnchor.Type.RIGHT, Page, ConstraintAnchor.Type.RIGHT)
                .AddConnect(rightPanel, ConstraintAnchor.Type.LEFT, centerPanel, ConstraintAnchor.Type.RIGHT)
                .AddConnect(rightPanel, ConstraintAnchor.Type.TOP, topPanel, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(rightPanel, ConstraintAnchor.Type.BOTTOM, bottomPanel, ConstraintAnchor.Type.TOP)
                .SetHeight(rightPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)

                .AddConnect(bottomPanel, ConstraintAnchor.Type.BOTTOM, Page, ConstraintAnchor.Type.BOTTOM)
                .SetWidth(bottomPanel, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)

                .AddConnect(centerPanel, ConstraintAnchor.Type.TOP, topPanel, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(centerPanel, ConstraintAnchor.Type.BOTTOM, bottomPanel, ConstraintAnchor.Type.TOP)
                .AddConnect(centerPanel, ConstraintAnchor.Type.LEFT, leftPanel, ConstraintAnchor.Type.RIGHT)
                .AddConnect(centerPanel, ConstraintAnchor.Type.RIGHT, rightPanel, ConstraintAnchor.Type.LEFT)
                .SetWidth(centerPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                .SetHeight(centerPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)

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
                .AddConnect(leftPanel, ConstraintAnchor.Type.LEFT, Page, ConstraintAnchor.Type.LEFT)
                .AddConnect(leftPanel, ConstraintAnchor.Type.RIGHT, centerPanel, ConstraintAnchor.Type.LEFT)
                .AddConnect(leftPanel, ConstraintAnchor.Type.TOP, topPanel, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(leftPanel, ConstraintAnchor.Type.BOTTOM, bottomPanel, ConstraintAnchor.Type.TOP)
                .SetHeight(leftPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)

                .AddConnect(topPanel, ConstraintAnchor.Type.LEFT, Page, ConstraintAnchor.Type.LEFT)
                .AddConnect(topPanel, ConstraintAnchor.Type.TOP, Page, ConstraintAnchor.Type.TOP)
                .SetWidth(topPanel, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)

                .AddConnect(rightPanel, ConstraintAnchor.Type.RIGHT, Page, ConstraintAnchor.Type.RIGHT)
                .AddConnect(rightPanel, ConstraintAnchor.Type.LEFT, centerPanel, ConstraintAnchor.Type.RIGHT)
                .AddConnect(rightPanel, ConstraintAnchor.Type.TOP, topPanel, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(rightPanel, ConstraintAnchor.Type.BOTTOM, bottomPanel, ConstraintAnchor.Type.TOP)
                .SetHeight(rightPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)

                .AddConnect(bottomPanel, ConstraintAnchor.Type.BOTTOM, Page, ConstraintAnchor.Type.BOTTOM)
                .SetWidth(bottomPanel, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)

                .AddConnect(centerPanel, ConstraintAnchor.Type.TOP, topPanel, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(centerPanel, ConstraintAnchor.Type.BOTTOM, bottomPanel, ConstraintAnchor.Type.TOP)
                .AddConnect(centerPanel, ConstraintAnchor.Type.LEFT, leftPanel, ConstraintAnchor.Type.RIGHT)
                .AddConnect(centerPanel, ConstraintAnchor.Type.RIGHT, rightPanel, ConstraintAnchor.Type.LEFT)
                .SetWidth(centerPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                .SetHeight(centerPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)

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
                .AddConnect(firstButton, ConstraintAnchor.Type.RIGHT, centerPanel, ConstraintAnchor.Type.RIGHT, 20)
                .AddConnect(firstButton, ConstraintAnchor.Type.TOP, centerPanel, ConstraintAnchor.Type.TOP, 20)
                //在centerPanel左下角
                .AddConnect(grid, ConstraintAnchor.Type.LEFT, centerPanel, ConstraintAnchor.Type.LEFT, 20)
                .AddConnect(grid, ConstraintAnchor.Type.TOP, firstButton, ConstraintAnchor.Type.BOTTOM, 20)
                .AddConnect(grid, ConstraintAnchor.Type.RIGHT, firstButton, ConstraintAnchor.Type.LEFT, 20)
                .AddConnect(grid, ConstraintAnchor.Type.BOTTOM, centerPanel, ConstraintAnchor.Type.BOTTOM, 20)
                .SetWidth(grid, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                .SetHeight(grid, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                //在Grid右下角
                .AddConnect(secondButton, ConstraintAnchor.Type.RIGHT, grid, ConstraintAnchor.Type.RIGHT, 20)
                .AddConnect(secondButton, ConstraintAnchor.Type.BOTTOM, grid, ConstraintAnchor.Type.BOTTOM, 20)
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
