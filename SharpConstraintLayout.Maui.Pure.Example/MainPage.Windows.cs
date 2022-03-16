
using Microsoft.Maui.Essentials;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
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
        private Button FirstButton;

        public Button SecondButton;
        private Canvas ThirdCanvas;
        private TextBlock FouthTextBlock;
        private TextBox FifthTextBox;
        private RichTextBlock SixthRichTextBlock;
        public ConstraintLayout layout;

        public MainPage()
        {
            this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.HotPink);
            //Orientation = Orientation.Vertical;
            //HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;
            //VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center;


            layout = new ConstraintLayout()
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
            };
            this.Children.Add(layout);

            FirstButton = new Button()
            {
                Tag = nameof(FirstButton),
                ClickMode = ClickMode.Press,
                Content = "FirstButton At Center",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Red)
            };
            FirstButton.Click += MainButton_Click;

            SecondButton = new Button()
            {
                //Width = 100,Height = 100,
                Tag = nameof(SecondButton),
                Content = "Second At Bottom",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
            };

            ThirdCanvas = new Canvas()
            {
                Tag = nameof(ThirdCanvas),
                //Width = 100,
                //Height = 100,
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.LightGreen)
            };

            FouthTextBlock = new TextBlock()
            {
                Tag = nameof(FouthTextBlock),
                //Width = 100,
                //Height = 100,
                Text = "TextBlock"
            };

            FifthTextBox = new TextBox()
            {
                Tag = nameof(FifthTextBox),
                Text = "TextBox",
                AcceptsReturn = true
            };

            //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
            SixthRichTextBlock = new RichTextBlock()
            {
                Tag = nameof(SixthRichTextBlock),
                IsTextSelectionEnabled = true,
                TextWrapping = TextWrapping.Wrap
            };
            Paragraph paragraph = new Paragraph()
            {
                FontSize = 18
            };
            Run run = new Run()
            {
                Text = "RichTextBlock"
            };
            paragraph.Inlines.Add(run);
            SixthRichTextBlock.Blocks.Add(paragraph);

            layout.AddView(FirstButton);
            layout.AddView(SecondButton);
            layout.AddView(ThirdCanvas);
            layout.AddView(FouthTextBlock);
            layout.AddView(FifthTextBox);
            layout.AddView(SixthRichTextBlock);

            var set = new ConstraintSet();
            set.Clone(layout);

            set.constrainWidth(layout.GetId(), ConstraintSet.WRAP_CONTENT);
            set.constrainHeight(layout.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(FirstButton.GetId(), ConstraintSet.LEFT, layout.GetId(), ConstraintSet.LEFT);
            set.Connect(FirstButton.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT);
            set.CenterVertically(FirstButton.GetId(), ConstraintSet.PARENT_ID, ConstraintSet.TOP, 0, ConstraintSet.PARENT_ID, ConstraintSet.BOTTOM, 0, 0.5f);
            set.constrainWidth(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);
            set.constrainHeight(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(SecondButton.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            set.Connect(SecondButton.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 10);
            set.constrainWidth(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);
            set.constrainHeight(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);

            
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.RIGHT, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.TOP, layout.GetId(), ConstraintSet.TOP, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.BOTTOM, layout.GetId(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(ThirdCanvas.GetHashCode(),ConstraintSet.GONE);
            set.constrainWidth(ThirdCanvas.GetId(), 100);
            set.constrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            
            set.Connect(FouthTextBlock.GetId(), ConstraintSet.RIGHT, SecondButton.GetId(), ConstraintSet.RIGHT);
            set.Connect(FouthTextBlock.GetId(), ConstraintSet.TOP, SecondButton.GetId(), ConstraintSet.BOTTOM, 10);
            set.constrainWidth(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            set.constrainHeight(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            
            set.Connect(FifthTextBox.GetId(), ConstraintSet.BOTTOM, FirstButton.GetId(), ConstraintSet.TOP, 50);
            set.Connect(FifthTextBox.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.LEFT);
            set.Connect(FifthTextBox.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            set.constrainWidth(FifthTextBox.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            set.constrainHeight(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);
            
            set.Connect(SixthRichTextBlock.GetId(), ConstraintSet.RIGHT, FouthTextBlock.GetId(), ConstraintSet.LEFT, 50);
            set.Connect(SixthRichTextBlock.GetId(), ConstraintSet.BASELINE, FouthTextBlock.GetId(), ConstraintSet.BASELINE, 50);
            set.constrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            set.constrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);


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
