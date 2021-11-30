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
                .AddConnect(toolBar, ConstraintSet.Side.Top, page, ConstraintSet.Side.Top)
                .SetWidth(toolBar, ConstraintSet.SizeType.MatchParent)
                .AddConnect(testPanel, ConstraintSet.Side.Top, toolBar, ConstraintSet.Side.Bottom)
                .AddConnect(testPanel, ConstraintSet.Side.Bottom, page, ConstraintSet.Side.Bottom)
                .SetWidth(testPanel, ConstraintSet.SizeType.MatchParent)
                .SetHeight(testPanel, ConstraintSet.SizeType.MatchConstraint);
            //工具栏详细设置
            viewCountTextBox = new TextBox() { Text = "500"};
            Button LayoutALotOfViewsToDrawTestButton = new Button() { Content = "LayoutALotOfViewsToDrawTest" };
            Button MultipleLevelNestTestButton = new Button() { Content = "MultipleLevelNestTest" };

            
            toolBar.Children.Add(LayoutALotOfViewsToDrawTestButton);
            toolBar.Children.Add(viewCountTextBox);
            toolBar.Children.Add(MultipleLevelNestTestButton);

            new ConstraintSet(toolBar)
                .AddConnect(viewCountTextBox, ConstraintSet.Side.CenterY, toolBar, ConstraintSet.Side.CenterY)
                .AddConnect(viewCountTextBox, ConstraintSet.Side.Left, toolBar, ConstraintSet.Side.Left,20)
                .AddConnect(LayoutALotOfViewsToDrawTestButton, ConstraintSet.Side.CenterY, toolBar, ConstraintSet.Side.CenterY)
                .AddConnect(LayoutALotOfViewsToDrawTestButton, ConstraintSet.Side.Left, viewCountTextBox, ConstraintSet.Side.Right,20)
               
                .AddConnect(MultipleLevelNestTestButton, ConstraintSet.Side.CenterY,toolBar,ConstraintSet.Side.CenterY)
                .AddConnect(MultipleLevelNestTestButton,ConstraintSet.Side.Left,LayoutALotOfViewsToDrawTestButton,ConstraintSet.Side.Right,20)
                ;

            LayoutALotOfViewsToDrawTestButton.Click += (sender, e) =>
            {
                LayoutALotOfViewsToDrawTest(testPanel);
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
                .AddConnect(preview, ConstraintSet.Side.Center, Page, ConstraintSet.Side.Center)
                .SetWidth(preview, ConstraintSet.SizeType.MatchParent)
                .SetHeight(preview, ConstraintSet.SizeType.MatchParent);
            for(var index = 0; index < viewCount; index++)
            {
                var view = new ConstraintLayout()
                {
                    Background = new SolidColorBrush(RandomColor()),
                };
                preview.Children.Add(view);
                new ConstraintSet(preview)
                    .AddConnect(view, ConstraintSet.Side.Right, preview, ConstraintSet.Side.Right, 1)
                    .AddConnect(view, ConstraintSet.Side.Bottom, preview, ConstraintSet.Side.Bottom, 1)
                    .AddConnect(view, ConstraintSet.Side.Left, preview, ConstraintSet.Side.Left,2)
                    .AddConnect(view, ConstraintSet.Side.Top, preview, ConstraintSet.Side.Top,2)
                    .SetWidth(view, ConstraintSet.SizeType.MatchConstraint)
                    .SetHeight(view, ConstraintSet.SizeType.MatchConstraint)
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
            set.AddConnect(previousView, ConstraintSet.Side.Bottom, DrawPanel, ConstraintSet.Side.Bottom);
            set.AddConnect(previousView, ConstraintSet.Side.Left, DrawPanel, ConstraintSet.Side.Left, 10);
            set.SetWidth(previousView, ConstraintSet.SizeType.WrapContent);
            set.SetHeight(previousView, ConstraintSet.SizeType.WrapContent);

            for (var i = 0; i < viewCount; i++)
            {
                Canvas view = new Canvas();
                DrawPanel.Children.Add(view);
                set.AddConnect(view, ConstraintSet.Side.Top, previousView, ConstraintSet.Side.Top, -1);
                set.AddConnect(view, ConstraintSet.Side.Bottom, previousView, ConstraintSet.Side.Bottom,1);
                set.AddConnect(view, ConstraintSet.Side.Left, previousView, ConstraintSet.Side.Left, 1);
                set.AddConnect(view, ConstraintSet.Side.Right, previousView, ConstraintSet.Side.Right, -1);
                set.SetWidth(view, ConstraintSet.SizeType.MatchConstraint);
                set.SetHeight(view, ConstraintSet.SizeType.MatchConstraint);
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
    }
}
