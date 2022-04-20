using Microsoft.Maui.Storage;
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
    public partial class MainPage : StackPanel
    {

        private Button TestCsharpConstraintLayoutButton;
        private Button TestJavaConstraintLayoutButton;
        private Button TestSleepButton;
        TextBlock TestCSharpSummary;
        TextBlock TestJavaSummary;
        TextBlock TestSleepSummary;
        public MainPage()
        {
            this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.White);
            Orientation = Orientation.Vertical;
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center;

            var ButtonContainer = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Height = 60,
                Width = 200,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Gray),
            };

            TestCsharpConstraintLayoutButton = new Button()
            {
                Content = "Start Csharp",
            };
            TestCsharpConstraintLayoutButton.Click += (sender, e) =>
                        {
                            TestCsharpConstraintLayoutButton_Click(sender, null);
                        };
            TestSleepButton = new Button()
            {
                Content = "Start Sleep",
            };
            TestSleepButton.Click += (sender, e) =>
                            {
                                TestSleepButton_Clicked(sender, null);
                            };
            ButtonContainer.Children.Add(TestCsharpConstraintLayoutButton);
            ButtonContainer.Children.Add(TestSleepButton);

            TestCSharpSummary = new TextBlock()
            {
            };
            TestSleepSummary = new TextBlock();
            this.Children.Add(ButtonContainer);
            this.Children.Add(TestCSharpSummary);
            this.Children.Add(TestSleepSummary);
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
