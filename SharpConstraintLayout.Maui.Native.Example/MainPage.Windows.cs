using Microsoft.Maui.Storage;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SharpConstraintLayout.Maui.Widget;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
namespace SharpConstraintLayout.Maui.Native.Example
{
    public partial class MainPage : ConstraintLayout
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
            //ConstraintLayout.DEBUG = true;
            ConstraintLayout.MEASURE = true;

            createControls();
            this.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.HotPink);

            //baseAlignTest(this);
            //baselineTest(this); 
            //guidelineTest(this);
            //barrierTest(this);//bug:text box have false length
            //visibilityTest(this);
            //flowTest(this);
            //nestedLayoutTest(this);
            //circleConstraintTest(this);
            nativeLayoutInConstraintLayoutTest(this);//OK
            //performanceTest(this);
            //performanceTest_WrapPanel(this);

        }

        private void createControls()
        {
            FirstButton = new Button()
            {
                Tag = nameof(FirstButton),
                ClickMode = ClickMode.Press,
                Content = "FirstButton",
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Red)
            };

            SecondButton = new Button()
            {
                //Width = 100,Height = 100,
                Tag = nameof(SecondButton),
                Content = "SecondBotton",
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
                Text = "FourthTextBlock",
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            FifthTextBox = new TextBox()
            {
                Tag = nameof(FifthTextBox),
                Text = "FifthTextBox",
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