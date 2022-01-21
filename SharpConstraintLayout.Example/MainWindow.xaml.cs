using androidx.constraintlayout.core.widgets;
using ReloadPreview;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpConstraintLayout.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConstraintLayout page;

        public MainWindow()
        {
            InitializeComponent();
            page = new ConstraintLayout();
            page.Background = new SolidColorBrush(Colors.Cyan);
            this.RootWindow.Children.Add(page);
            SetContent(page);
        }



        private void SetContent(ConstraintLayout MainPage)
        {
            Button pressureTestButton = new Button()
            {
                Content = "PressureTest",
                Height = 100,
                BorderBrush = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush(Colors.White),

            };

            pressureTestButton.Click += (sender, e) =>
            {
                new PressureTestWindow().Show();
            };

            Button complexLayoutRemoveAndAddTestButton = new Button()
            {
                Content = "ComplexLayoutTest - Remove and Add",
                Height = 100,
                BorderBrush = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush(Colors.White),
            };

            complexLayoutRemoveAndAddTestButton.Click += (sendere, e) =>
            {
                var window = new ComplexLayoutTestWindow();
                ReloadClient.GlobalInstance.Reload += window.ReloadClient_Reload;
                window.Show();
            };

            MainPage.Children.Add(pressureTestButton);
            MainPage.Children.Add(complexLayoutRemoveAndAddTestButton);


            ConstraintSet set = new ConstraintSet(MainPage);
            set.AddConnect(pressureTestButton, ConstraintSet.Side.CenterY, MainPage, ConstraintSet.Side.CenterY)
                .AddConnect(pressureTestButton, ConstraintSet.Side.Left, MainPage, ConstraintSet.Side.Left)
                .AddConnect(pressureTestButton, ConstraintSet.Side.Right, complexLayoutRemoveAndAddTestButton, ConstraintSet.Side.Left);

            set.AddConnect(complexLayoutRemoveAndAddTestButton, ConstraintSet.Side.CenterY, MainPage, ConstraintSet.Side.CenterY)
                .AddConnect(complexLayoutRemoveAndAddTestButton, ConstraintSet.Side.Left, pressureTestButton, ConstraintSet.Side.Right, 20)
                .AddConnect(complexLayoutRemoveAndAddTestButton, ConstraintSet.Side.Right, MainPage, ConstraintSet.Side.Right);
        }
    }
}
