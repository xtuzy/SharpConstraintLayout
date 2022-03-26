
using Microsoft.Maui.Essentials;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SharpConstraintLayout.Core.Benchmark
{
    public class MainPage : StackPanel
    {
        private Button MainButton;

        public MainPage()
        {
            this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.HotPink);
            Orientation = Orientation.Vertical;
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center;
            MainButton = new Button()
            {
                Content = "Click it.",
            };
            MainButton.Click += MainButton_Click;
            this.Children.Add(MainButton);
            LoadMauiAsset();
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
