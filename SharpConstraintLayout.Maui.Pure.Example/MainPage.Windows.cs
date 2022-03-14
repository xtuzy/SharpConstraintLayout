
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
    public class MainPage : Grid
    {
        private Button FirstButton;

        public Button SecondButton;
        private Canvas ThirdCanvas;
        private TextBlock FouthTextBlock;
        private TextBox FifthTextBox;
        private RichTextBlock SixthRichTextBlock;

        public MainPage()
        {
            this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.HotPink);
            //Orientation = Orientation.Vertical;
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center;
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center;


            var layout = new ConstraintLayout()
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
            };
            this.Children.Add(layout);

            FirstButton = new Button()
            {
                Tag = nameof(FirstButton),
                ClickMode = ClickMode.Press,
                Content = "Click it.",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Red)
            };
            FirstButton.Click += MainButton_Click;

            SecondButton = new Button()
            {
                //Width = 100,Height = 100,
                Tag = nameof(SecondButton),
                Content = "Second.",
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
            var bseline = FouthTextBlock.BaselineOffset;

            FifthTextBox = new TextBox()
            {
                Tag = nameof(FifthTextBox),
                Text = "TextBox",
            };
            //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
            SixthRichTextBlock = new RichTextBlock()
            {
                Tag = nameof(SixthRichTextBlock),
            };
            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            SixthRichTextBlock.IsTextSelectionEnabled = true;
            SixthRichTextBlock.TextWrapping = TextWrapping.Wrap;
            run.Text = "This is some sample text to show the wrapping behavior.";
            paragraph.Inlines.Add(run);
            SixthRichTextBlock.Blocks.Add(paragraph);

            layout.AddView(FirstButton);
            layout.AddView(SecondButton);
            layout.AddView(ThirdCanvas);
            layout.AddView(FouthTextBlock);
            layout.AddView(FifthTextBox);
            layout.AddView(SixthRichTextBlock);

            var set = new ConstraintSet();
            set.Clone_Android(layout);

            set.Connect(FirstButton.GetId(), ConstraintSet.LEFT, layout.GetId(), ConstraintSet.LEFT, 50);
            set.Connect(FirstButton.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT, 50);
            set.CenterVertically(FirstButton.GetId(), ConstraintSet.PARENT_ID, ConstraintSet.TOP, 0, ConstraintSet.PARENT_ID, ConstraintSet.BOTTOM, 0, 0.5f);
            set.setVisibility(FirstButton.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);
            set.constrainHeight(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(SecondButton.GetId(), ConstraintSet.LEFT, layout.GetId(), ConstraintSet.LEFT, 50);
            set.Connect(SecondButton.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(SecondButton.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(SecondButton.GetId(), ConstraintSet.MATCH_PARENT);
            set.constrainHeight(SecondButton.GetId(), 200);

            set.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.RIGHT, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.BOTTOM, layout.GetId(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(ThirdCanvas.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(ThirdCanvas.GetId(), 200);
            set.constrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_PARENT);

            set.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.RIGHT, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.BOTTOM, layout.GetId(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(ThirdCanvas.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(ThirdCanvas.GetId(), 200);
            set.constrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_PARENT);

            set.ApplyTo_Android(layout);
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
