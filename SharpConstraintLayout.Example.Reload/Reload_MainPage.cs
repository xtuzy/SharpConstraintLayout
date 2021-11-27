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

            GuideLine firsttLineGuideline = new GuideLine() {
                Orientation = GuideLine.Orientations.HORIZONTAL,
                Percent = 0.5f
            };

            Button ZLayoutTestButton = new Button()
            {
                Content = "ComplexLayoutTest - ZLayout",
                Height = 100,
                BorderBrush = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush(Colors.White),
            };
            ZLayoutTestButton.Click += (sender, e) =>
            {
                new ZWindow().Show();
            };


            MainPage.Children.Add(pressureTestButton);
            MainPage.Children.Add(complexLayoutRemoveAndAddTestButton);
            MainPage.Children.Add(firsttLineGuideline);
            MainPage.Children.Add(ZLayoutTestButton);

            ConstraintSet set = new ConstraintSet(MainPage);
            set.AddConnect(pressureTestButton, ConstraintAnchor.Type.TOP, MainPage, ConstraintAnchor.Type.TOP)
                .AddConnect(pressureTestButton, ConstraintAnchor.Type.BOTTOM, firsttLineGuideline, ConstraintAnchor.Type.TOP)
                .AddConnect(pressureTestButton, ConstraintAnchor.Type.LEFT, MainPage, ConstraintAnchor.Type.LEFT,20)
                .SetWidth(pressureTestButton, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                .SetHeight(pressureTestButton, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
            set.AddConnect(complexLayoutRemoveAndAddTestButton, ConstraintAnchor.Type.BOTTOM, firsttLineGuideline, ConstraintAnchor.Type.TOP)
                .AddConnect(complexLayoutRemoveAndAddTestButton,ConstraintAnchor.Type.TOP, MainPage, ConstraintAnchor.Type.TOP)
                .AddConnect(complexLayoutRemoveAndAddTestButton, ConstraintAnchor.Type.LEFT, pressureTestButton, ConstraintAnchor.Type.RIGHT, 20)
                .SetHeight(complexLayoutRemoveAndAddTestButton, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                .SetWidth(complexLayoutRemoveAndAddTestButton, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
            set.AddConnect(ZLayoutTestButton, ConstraintAnchor.Type.TOP, firsttLineGuideline, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(ZLayoutTestButton, ConstraintAnchor.Type.BOTTOM, MainPage, ConstraintAnchor.Type.BOTTOM)
                .AddConnect(ZLayoutTestButton, ConstraintAnchor.Type.LEFT, MainPage, ConstraintAnchor.Type.LEFT, 20)
                .SetHeight(ZLayoutTestButton, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT)
                .SetWidth(ZLayoutTestButton, ConstraintWidget.DimensionBehaviour.WRAP_CONTENT);
        }


    }
}
