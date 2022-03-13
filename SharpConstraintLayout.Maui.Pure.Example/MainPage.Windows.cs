
using Microsoft.Maui.Essentials;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharpConstraintLayout.Maui.Pure.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
//using SharpConstraintLayout.Maui.Pure.Core;
namespace SharpConstraintLayout.Maui.Pure.Example
{
    public class MainPage : StackPanel
    {
        private Button MainButton;

        public Button SecondButton;

        public MainPage()
        {
            this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.HotPink);
            Orientation = Orientation.Vertical;
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center;
            

            var layout = new ConstraintLayout()
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Yellow)
            };
            this.Children.Add(layout);

            MainButton = new Button()
            {
                Tag = nameof(MainButton),
                ClickMode = ClickMode.Press,
                Content = "Click it.",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Red)
            };
            SecondButton = new Button()
            {
                Tag = nameof(SecondButton),
                Content = "Second.",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
            };
            MainButton.Click += MainButton_Click;
            layout.AddView(MainButton);
            layout.AddView(SecondButton);

            var set = new ConstraintSet();
            set.Clone(layout);
            set.constrainHeight(layout.GetHashCode(), 500);
            set.constrainWidth(layout.GetHashCode(), 500);
            set.centerHorizontally(MainButton.GetHashCode(),ConstraintSet.PARENT_ID);
            set.centerVertically(MainButton.GetHashCode(),ConstraintSet.PARENT_ID);

            set.setVisibility(MainButton.GetHashCode(), ConstraintSet.VISIBLE);
            set.centerHorizontally(SecondButton.GetHashCode(), ConstraintSet.PARENT_ID);
            set.constrainHeight(layout.GetHashCode(), ConstraintSet.MATCH_PARENT);
            set.constrainWidth(layout.GetHashCode(), ConstraintSet.MATCH_PARENT);
            set.Connect(SecondButton.Id(), ConstraintSet.TOP, MainButton.Id(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(SecondButton.GetHashCode(), ConstraintSet.VISIBLE);
            set.constrainHeight(SecondButton.GetHashCode(), 100);
            set.constrainWidth(SecondButton.GetHashCode(), 200);
            set.ApplyTo(layout);
        }

        private void MainButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine("Hello"); 
        }

        async Task LoadMauiAsset()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("favorite_black_24dp.svg");
            System.Diagnostics.Debug.WriteLine($"svg size:{stream.Length}");
        }
    }
}
