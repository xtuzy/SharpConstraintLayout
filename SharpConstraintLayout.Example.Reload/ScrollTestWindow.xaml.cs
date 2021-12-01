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

namespace SharpConstraintLayout.Example
{
    /// <summary>
    /// ScrollTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ScrollTestWindow : Window
    {
        public ScrollTestWindow()
        {
            InitializeComponent();
        }

        private void GridNestListViewButton_Click(object sender, RoutedEventArgs e)
        {
            testPanel.Children.Clear();
            var listView = new ListView()
            {
                Background = new SolidColorBrush(Colors.Pink),
            };
            testPanel.Children.Add(listView);
            for (var index = 0; index < 5000; index++)
            {
                var item1 = new ListViewItem { Content = $"This is {index} item added programmatically." };
                listView.Items.Add(item1);
            }
        }

        private void ConstraintNestListViewButton_Click(object sender, RoutedEventArgs e)
        {
            testPanel.Children.Clear();
            var Page = new ConstraintLayout() { Tag= "testPanel" };
            testPanel.Children.Add(Page);
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
    }
}
