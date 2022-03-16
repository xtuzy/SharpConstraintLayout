
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
    public class MainPage : ConstraintLayout
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

            layout = new ConstraintLayout()
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Black)
            };
            var guide = new Guideline()
            {
                //GuidelinePercent = 0.3f,
            };
            this.AddView(layout);
            this.AddView(guide);

            var pageSet = new ConstraintSet();
            pageSet.Clone(this);
            pageSet.SetGuidelineOrientation(guide.GetId(), ConstraintSet.VERTICAL);
            pageSet.SetGuidelinePercent(guide.GetId(),0.5f);
            pageSet.ConstrainWidth(guide.GetId(), ConstraintSet.WRAP_CONTENT);
            pageSet.ConstrainHeight(guide.GetId(), ConstraintSet.WRAP_CONTENT);

            pageSet.Connect(layout.GetId(),ConstraintSet.LEFT,guide.GetId(),ConstraintSet.RIGHT);
            pageSet.Connect(layout.GetId(), ConstraintSet.RIGHT, this.GetId(), ConstraintSet.RIGHT);
            pageSet.Connect(layout.GetId(), ConstraintSet.TOP, this.GetId(), ConstraintSet.TOP);
            pageSet.Connect(layout.GetId(), ConstraintSet.BOTTOM, this.GetId(), ConstraintSet.BOTTOM);
            pageSet.ConstrainWidth(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            pageSet.ConstrainHeight(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            pageSet.ApplyTo(this);
            
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

            var layoutSet = new ConstraintSet();
            layoutSet.Clone(layout);

            layoutSet.ConstrainWidth(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);

            layoutSet.Connect(FirstButton.GetId(), ConstraintSet.LEFT, layout.GetId(), ConstraintSet.LEFT);
            layoutSet.Connect(FirstButton.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT);
            layoutSet.CenterVertically(FirstButton.GetId(), ConstraintSet.PARENT_ID, ConstraintSet.TOP, 0, ConstraintSet.PARENT_ID, ConstraintSet.BOTTOM, 0, 0.5f);
            layoutSet.ConstrainWidth(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            layoutSet.Connect(SecondButton.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 10);
            layoutSet.ConstrainWidth(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);


            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.RIGHT, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.TOP, layout.GetId(), ConstraintSet.TOP, 50);
            layoutSet.Connect(ThirdCanvas.GetId(), ConstraintSet.BOTTOM, layout.GetId(), ConstraintSet.BOTTOM, 50);
            layoutSet.setVisibility(ThirdCanvas.GetHashCode(), ConstraintSet.GONE);
            layoutSet.ConstrainWidth(ThirdCanvas.GetId(), 100);
            layoutSet.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_CONSTRAINT);

            layoutSet.Connect(FouthTextBlock.GetId(), ConstraintSet.RIGHT, SecondButton.GetId(), ConstraintSet.RIGHT);
            layoutSet.Connect(FouthTextBlock.GetId(), ConstraintSet.TOP, SecondButton.GetId(), ConstraintSet.BOTTOM, 10);
            layoutSet.ConstrainWidth(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.BOTTOM, FirstButton.GetId(), ConstraintSet.TOP, 50);
            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.LEFT);
            layoutSet.Connect(FifthTextBox.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            layoutSet.ConstrainWidth(FifthTextBox.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            layoutSet.ConstrainHeight(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);

            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.RIGHT, FouthTextBlock.GetId(), ConstraintSet.LEFT, 50);
            layoutSet.Connect(SixthRichTextBlock.GetId(), ConstraintSet.BASELINE, FouthTextBlock.GetId(), ConstraintSet.BASELINE, 50);
            layoutSet.ConstrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            layoutSet.ConstrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);


            layoutSet.ApplyTo(layout);
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
