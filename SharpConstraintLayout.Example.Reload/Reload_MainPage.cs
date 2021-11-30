using androidx.constraintlayout.core.widgets;
using ReloadPreview;
using SharpConstraintLayout.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpConstraintLayout.Example.Reload
{
    public class Reload_MainPage : IReload
    {
        ConstraintLayout MainPage;
        Window MainWindow;
        public void Reload(object controller, object view)
        {
            MainWindow = (Window)controller;
            MainPage = view as ConstraintLayout;
            MainPage.Children.Clear();
            BuildMainPage();
        }

        private void BuildMainPage()
        {
            Button pressureTestButton = new Button() { 
                Content = "PressureTest" ,
                Height =100,
                BorderBrush = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush(Colors.White),
                
            };
            
            pressureTestButton.Click += (sender, e) =>
            {
                new PressureTestWindow().Show();
            };

            Button complexLayoutRemoveAndAddTestButton = new Button() { 
                Content = "ComplexLayoutTest - Remove and Add",
                Height =100,
                BorderBrush = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush(Colors.White),
            };

            complexLayoutRemoveAndAddTestButton.Click += (sendere, e) =>
            {
                new ComplexLayoutTestWindow().Show();
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
