using androidx.constraintlayout.core.widgets;
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
    /// ZWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ZWindow : Window
    {
        public ZWindow()
        {
            InitializeComponent();
            var page = new ZLayout();
            this.Content = page;
            var Page = page;
            Page.Background = new SolidColorBrush(Colors.Cyan);
            //先添加控件
            Canvas previousView = new Canvas() { Width = 20, Height = 30 };
            previousView.Width = 30;
            previousView.Height = 50;
            previousView.Background = new SolidColorBrush(Colors.Yellow);
            Page.Children.Add(previousView);
            for (var i = 0; i < viewCount; i++)
            {
                Canvas view = new Canvas();
                Page.Children.Add(view);
                view.Background = new SolidColorBrush(RandomColor());
                if (i == viewCount - 1)
                    view.Background = new SolidColorBrush(Colors.Blue);
            }
            //布局时再调用自定义布局
            page.CustomLayout = CustomLayout;
        }

        int viewCount = 1800;


        bool isFirstTimeLayout = true;
        /// <summary>
        /// N形状布局..................
        /// .
        /// </summary>
        /// <param name="Page"></param>
        public void CustomLayout(ZLayout Page)
        {
            var set = new ZConstraintSet(Page);
            if (isFirstTimeLayout)//第一次就先直接全部布局完
            {
                var previousView = Page.Children[0] as Canvas;
                set.Clear(previousView);
                set.AddConnect(previousView, ConstraintAnchor.Type.BOTTOM, Page, ConstraintAnchor.Type.BOTTOM);
                set.AddConnect(previousView, ConstraintAnchor.Type.LEFT, Page, ConstraintAnchor.Type.LEFT, 10);
                set.SetWidth(previousView, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
                set.SetHeight(previousView, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);

                for (var i = 1; i < Page.Children.Count; i++)
                {
                    previousView = Page.Children[i - 1] as Canvas;
                    var view = Page.Children[i] as Canvas;
                    set.Clear(view);
                    set.AddConnect(view, ConstraintAnchor.Type.TOP, previousView, ConstraintAnchor.Type.TOP, -1);
                    set.AddConnect(view, ConstraintAnchor.Type.BOTTOM, previousView, ConstraintAnchor.Type.BOTTOM, 1);
                    set.AddConnect(view, ConstraintAnchor.Type.LEFT, previousView, ConstraintAnchor.Type.LEFT, 1);
                    set.AddConnect(view, ConstraintAnchor.Type.RIGHT, previousView, ConstraintAnchor.Type.RIGHT, -1);
                    set.SetWidth(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                    set.SetHeight(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                }
                //isFirstTimeLayout = false;
                Page.UpdateCustomLayoutInfo();//全部布局完就计算
            }

            

            //然后看哪里开始不对
            bool isStartDown = false;
            bool isStartUp = false;

            for (var i = 1; i < Page.Children.Count; i++)
            {
                var previousView = Page.Children[i - 1] as Canvas;
                var view = Page.Children[i] as Canvas;

                var info = Page.GetCustomLayoutInfo(view);
                var pageInfo = Page.GetCustomLayoutInfo(Page);

                if (pageInfo.Top > info.Top)
                {
                    isStartDown = true;
                    isStartUp = false;
                }

                if (pageInfo.Bottom < info.Bottom)
                {
                    isStartDown = false;
                    isStartUp = true;
                }

                //从开始不对的地方全部改变方向
                if (isStartUp || isStartDown)
                {
                    for (var index = i; index < Page.Children.Count; index++)
                    {
                        previousView = Page.Children[index - 1] as Canvas;
                        view = Page.Children[index] as Canvas;
                        if (isStartUp)
                        {
                            set.Clear(view);
                            set.AddConnect(view, ConstraintAnchor.Type.TOP, previousView, ConstraintAnchor.Type.TOP, -1);
                            set.AddConnect(view, ConstraintAnchor.Type.BOTTOM, previousView, ConstraintAnchor.Type.BOTTOM, 1);
                            set.AddConnect(view, ConstraintAnchor.Type.LEFT, previousView, ConstraintAnchor.Type.LEFT, 1);
                            set.AddConnect(view, ConstraintAnchor.Type.RIGHT, previousView, ConstraintAnchor.Type.RIGHT, -1);
                            set.SetWidth(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                            set.SetHeight(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                        }

                        if (isStartDown)
                        {
                            set.Clear(view);
                            set.AddConnect(view, ConstraintAnchor.Type.TOP, previousView, ConstraintAnchor.Type.TOP, 1);
                            set.AddConnect(view, ConstraintAnchor.Type.BOTTOM, previousView, ConstraintAnchor.Type.BOTTOM, -1);
                            set.AddConnect(view, ConstraintAnchor.Type.LEFT, previousView, ConstraintAnchor.Type.LEFT, 1);
                            set.AddConnect(view, ConstraintAnchor.Type.RIGHT, previousView, ConstraintAnchor.Type.RIGHT, -1);
                            set.SetWidth(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                            set.SetHeight(view, ConstraintWidget.DimensionBehaviour.MATCH_CONSTRAINT);
                        }
                    }
                    //更新布局信息,等待下次判断
                    Page.UpdateCustomLayoutInfo();
                    //之后是单调的,需要复原再次判断
                    isStartDown = false;
                    isStartUp = false;
                }
            }

            Page.InvalidateVisual();
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
