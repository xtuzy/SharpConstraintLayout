using androidx.constraintlayout.core.widgets;
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

namespace SharpConstraintLayout.Example
{
    /// <summary>
    /// PressureTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PressureTestWindow : Window
    {
        private TextBox viewCountTextBox;

        public PressureTestWindow()
        {
            InitializeComponent();
            //页面分区
            var page = new ConstraintLayout()
            {
                Background = new SolidColorBrush(Colors.Cyan),
                Tag = "Page",
                TEST = true,
            };

            this.Content = page;

            var toolBar = new ConstraintLayout()
            {
                Height = 50,
                Background = new SolidColorBrush(Colors.Wheat),
                Tag = "ToolBar",
                //DEBUG = true,
            };

            var testPanel = new ConstraintLayout()
            {
                Tag = "TestPanel",
                //DEBUG = true,
            };
            page.Children.Add(toolBar);
            page.Children.Add(testPanel);
            new ConstraintSet(page)
                .AddConnect(toolBar, ConstraintAnchor.Type.TOP, page, ConstraintAnchor.Type.TOP)
                .SetWidth(toolBar, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                .AddConnect(testPanel, ConstraintAnchor.Type.TOP, toolBar, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(testPanel, ConstraintAnchor.Type.BOTTOM, page, ConstraintAnchor.Type.BOTTOM)
                .SetWidth(testPanel, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                .SetHeight(testPanel, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
            //工具栏详细设置
            viewCountTextBox = new TextBox() { Text = "500"};
            Button LayoutALotOfViewsToDrawTestButton = new Button() { Content = "LayoutALotOfViewsToDrawTest" };
            Button StartPureLayoutButton = new Button() { Content = "PureCalculateLayoutTime"};
            Button MultipleLevelNestTestButton = new Button() { Content = "MultipleLevelNestTest" };

            
            toolBar.Children.Add(StartPureLayoutButton);
            toolBar.Children.Add(LayoutALotOfViewsToDrawTestButton);
            toolBar.Children.Add(viewCountTextBox);
            toolBar.Children.Add(MultipleLevelNestTestButton);

            new ConstraintSet(toolBar)
                .AddConnect(viewCountTextBox, ConstraintAnchor.Type.CENTER_Y, toolBar, ConstraintAnchor.Type.CENTER_Y)
                .AddConnect(viewCountTextBox, ConstraintAnchor.Type.LEFT, toolBar, ConstraintAnchor.Type.LEFT,20)
                .AddConnect(LayoutALotOfViewsToDrawTestButton, ConstraintAnchor.Type.CENTER_Y, toolBar, ConstraintAnchor.Type.CENTER_Y)
                .AddConnect(LayoutALotOfViewsToDrawTestButton, ConstraintAnchor.Type.LEFT, viewCountTextBox, ConstraintAnchor.Type.RIGHT,20)
                .AddConnect(StartPureLayoutButton, ConstraintAnchor.Type.CENTER_Y, toolBar, ConstraintAnchor.Type.CENTER_Y)
                .AddConnect(StartPureLayoutButton, ConstraintAnchor.Type.LEFT, LayoutALotOfViewsToDrawTestButton, ConstraintAnchor.Type.RIGHT, 20)
                .AddConnect(MultipleLevelNestTestButton, ConstraintAnchor.Type.CENTER_Y,toolBar,ConstraintAnchor.Type.CENTER_Y)
                .AddConnect(MultipleLevelNestTestButton,ConstraintAnchor.Type.LEFT,StartPureLayoutButton,ConstraintAnchor.Type.RIGHT,20)
                ;

            LayoutALotOfViewsToDrawTestButton.Click += (sender, e) =>
            {
                LayoutALotOfViewsToDrawTest(testPanel);
            };
            StartPureLayoutButton.Click += (sender, e) =>
            {
                PureCalculateLayoutTime();
            };
            MultipleLevelNestTestButton.Click += (sender, e) =>
            {
                MultipleLevelNestTest(testPanel);
            };
        }

        private void MultipleLevelNestTest(ConstraintLayout Page)
        {
            Page.Children.Clear();
            ConstraintLayout preview = new ConstraintLayout()
            {
                Background = new SolidColorBrush(RandomColor()),
            };
            Page.Children.Add(preview);
            new ConstraintSet(Page)
                .AddConnect(preview, ConstraintAnchor.Type.CENTER, Page, ConstraintAnchor.Type.CENTER)
                .SetWidth(preview, ConstraintWidget.DimensionBehaviour.MATCH_PARENT)
                .SetHeight(preview, ConstraintWidget.DimensionBehaviour.MATCH_PARENT);
            for(var index = 0; index < viewCount; index++)
            {
                var view = new ConstraintLayout()
                {
                    Background = new SolidColorBrush(RandomColor()),
                };
                preview.Children.Add(view);
                new ConstraintSet(preview)
                    .AddConnect(view, ConstraintAnchor.Type.RIGHT, preview, ConstraintAnchor.Type.RIGHT, 1)
                    .AddConnect(view, ConstraintAnchor.Type.BOTTOM, preview, ConstraintAnchor.Type.BOTTOM, 1)
                    .AddConnect(view, ConstraintAnchor.Type.LEFT, preview, ConstraintAnchor.Type.LEFT,2)
                    .AddConnect(view, ConstraintAnchor.Type.TOP, preview, ConstraintAnchor.Type.TOP,2)
                    .SetWidth(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                    .SetHeight(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT)
                    ;
                preview = view;
            }

        }

        int viewCount => int.Parse( viewCountTextBox.Text);

        /// <summary>
        /// 比较规律的排列
        /// </summary>
        /// <param name="DrawPanel"></param>
        void LayoutALotOfViewsToDrawTest(ConstraintLayout DrawPanel)
        {
            DrawPanel.Children.Clear();
            
            Canvas previousView = new Canvas() { Width = 20, Height = 30 };
            previousView.Width = 30;
            previousView.Height = 50;
            previousView.Background = new SolidColorBrush(Colors.Cyan);
            DrawPanel.Children.Add(previousView);
            var set = new ConstraintSet(DrawPanel);
            set.AddConnect(previousView, ConstraintAnchor.Type.BOTTOM, DrawPanel, ConstraintAnchor.Type.BOTTOM);
            set.AddConnect(previousView, ConstraintAnchor.Type.LEFT, DrawPanel, ConstraintAnchor.Type.LEFT, 10);
            set.SetWidth(previousView, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
            set.SetHeight(previousView, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);

            for (var i = 0; i < viewCount; i++)
            {
                Canvas view = new Canvas();
                DrawPanel.Children.Add(view);
                set.AddConnect(view, ConstraintAnchor.Type.TOP, previousView, ConstraintAnchor.Type.TOP, -1);
                set.AddConnect(view, ConstraintAnchor.Type.BOTTOM, previousView, ConstraintAnchor.Type.BOTTOM,1);
                set.AddConnect(view, ConstraintAnchor.Type.LEFT, previousView, ConstraintAnchor.Type.LEFT, 1);
                set.AddConnect(view, ConstraintAnchor.Type.RIGHT, previousView, ConstraintAnchor.Type.RIGHT, -1);
                set.SetWidth(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                set.SetHeight(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                view.Background = new SolidColorBrush(RandomColor());
                previousView = view;
            }
            DrawPanel.InvalidateVisual();
        }


        Color RandomColor()
        {
            Random ran = new Random();
            int r = ran.Next(0, 255);
            int g = ran.Next(0, 255);
            int b = ran.Next(0, 255);
            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }
        
        void PureCalculateLayoutTime()
        {
            List<long> data = new List<long>();
            for (var index = 0; index < 10; index++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                ConstraintWidgetContainer root = new ConstraintWidgetContainer(0, 0, 1000, 600);
                ConstraintWidget preview = new ConstraintWidget(100, 40);
                root.add(preview);
                preview.connect(ConstraintAnchor.Type.LEFT, root, ConstraintAnchor.Type.LEFT);
                preview.connect(ConstraintAnchor.Type.BOTTOM, root, ConstraintAnchor.Type.BOTTOM);
                preview.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                preview.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.WRAP_CONTENT;
                for (var i = 0; i < viewCount; i++)
                {
                    var view = new ConstraintWidget(100, 40);
                    root.add(view);
                    view.connect(ConstraintAnchor.Type.LEFT, preview, ConstraintAnchor.Type.LEFT, 1);
                    view.connect(ConstraintAnchor.Type.RIGHT, preview, ConstraintAnchor.Type.RIGHT, 1);
                    view.connect(ConstraintAnchor.Type.TOP, preview, ConstraintAnchor.Type.TOP, 1);
                    view.connect(ConstraintAnchor.Type.BOTTOM, preview, ConstraintAnchor.Type.BOTTOM, 1);
                    view.HorizontalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                    view.VerticalDimensionBehaviour = ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT;
                    preview = view;
                }

                root.layout();
                sw.Stop();
                data.Add(sw.ElapsedMilliseconds);
            }
            string message = "";
            long sum = 0;
            foreach (var num in data)
            {
                sum += num;
                message+=num.ToString()+" ";
            }
                
            long result = sum/ data.Count;
            // Configure the message box to be displayed
            string caption = "Test Result";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBox.Show("All Data:"+message+"Average:" + result+" ms", caption, button, icon);
        }
    }
}
