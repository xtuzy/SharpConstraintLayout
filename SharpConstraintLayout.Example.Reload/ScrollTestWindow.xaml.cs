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
        List<MessageModel> Data = new List<MessageModel> ();
        public ScrollTestWindow()
        {
            InitializeComponent();
            //listView.DataContext = Data;
            this.DataContext = Data;
            for (var index = 0; index < 5000; index++)
            {
                Data.Add(new MessageModel {Message=index+"message",Time = DateTime.Now.ToString() });
            }
        } 

        private void DefaultListViewButton_Click(object sender, RoutedEventArgs e)
        {
            
            listViewLeft.ItemsSource = Data;
        }

        private void ConstraintListViewItemButton_Click(object sender, RoutedEventArgs e)
        {
            listViewRight.ItemsSource = Data;
        }
    }

    public class MessageModel
    {
        public string Message { get; set; }
        public string Time { get; set; }
    }
}
