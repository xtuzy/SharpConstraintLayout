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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpConstraintLayout.Example.Reload
{
    /// <summary>
    /// ListViewConstraintItem.xaml 的交互逻辑
    /// </summary>
    public partial class ListViewConstraintItem : UserControl
    {
        //static int index=0;
        Button DeleteButton;
        TextBlock MessageTextBlock;
        TextBlock TimeTextBlock;
        public ListViewConstraintItem()
        {
            InitializeComponent();
            //var content = new Grid();
            var item = new ConstraintLayout()
            {
                Tag = "ConstraintLayout Item",
                HeightType = ConstraintSet.SizeType.WrapContent,
                WidthType = ConstraintSet.SizeType.MatchParent,
                Background = new SolidColorBrush(Colors.Brown),
                //DEBUG = true,
            };
            this.Content = item;
            //content.Children.Add(item);
            MessageTextBlock = new TextBlock() { Text = "Message", Tag = "MessageTextBlock" };
            TimeTextBlock = new TextBlock() { Text = "Time", Tag = "TimeTextBlock" };
            DeleteButton = new Button() { Content = "Delete", Height = 50, Tag = "DeleteButton" };
            item.Children.Add(MessageTextBlock);
            item.Children.Add(TimeTextBlock);
            item.Children.Add(DeleteButton);
            MessageTextBlock.LeftToLeft(item, 20).CenterYTo(item);
            TimeTextBlock.CenterTo(item);
            DeleteButton.RightToRight(item, 20).CenterYTo(item);
            //Debug.WriteLine(index++);
        }



        public string MessageText
        {
            get { return (string)GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(string), typeof(ListViewConstraintItem),
                new PropertyMetadata("default", null, MessageTextCoerceValueCallback));

        private static object MessageTextCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue != null)
            {
                
                var control = d as ListViewConstraintItem;
                var value = baseValue.ToString();
                if (value.Contains("500"))
                {
                    //System.Diagnostics.Debug.WriteLine(value);
                }
                control.MessageTextBlock.Text = value;
                return value;
            }
            return baseValue;
        }

        public string TimeText
        {
            get { return (string)GetValue(TimeTextProperty); }
            set { SetValue(TimeTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeTextProperty =
            DependencyProperty.Register("TimeText", typeof(string), typeof(ListViewConstraintItem),
                new PropertyMetadata("default", null, TimeTextCoerceValueCallback));

        private static object TimeTextCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue != null)
            {
                var control = d as ListViewConstraintItem;
                var value = baseValue.ToString();
                control.TimeTextBlock.Text = value;
                return value;
            }
            return baseValue;
        }

        public event EventHandler Click;


    }
}
