using CoreGraphics;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using SharpConstraintLayout.Maui.Widget;
using Foundation;

namespace SharpConstraintLayout.Maui.Native.Example
{
    public partial class MainPage
    {
        private ConstraintLayout layout;
        public ConstraintLayout Page;
        private UIButton FirstButton;
        private UIButton SecondButton;
        private UIView ThirdCanvas;
        private UITextView FouthTextBlock;
        private UITextField FifthTextBox;
        private UITextView SixthRichTextBlock;

        public MainPage(CGRect frame)
        {
            Page = new ConstraintLayout()
            {
                //Frame = frame,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                BackgroundColor = UIColor.SystemPink
            };

            createControls();

            //controlsTest(Page);
            //baselineTest(Page);
            //guidelineTest(Page);
            //barrierTest(Page);
            //visibilityTest(Page);
            flowTest(Page);
        }

        private void createControls()
        {
            FirstButton = new UIButton()
            {
                //Tag = nameof(FirstButton),
                //ClickMode = ClickMode.Press,
                BackgroundColor = UIColor.Red
            };
            FirstButton.SetTitle("FirstButton", UIControlState.Normal);
            FirstButton.TouchUpInside += FirstButton_Click;

            SecondButton = new UIButton()
            {
                //Width = 100,Height = 100,
                //Tag = nameof(SecondButton),
                BackgroundColor = UIColor.Black
            };
            SecondButton.SetTitle("SecondButton", UIControlState.Normal);

            ThirdCanvas = new UIView()
            {
                //Tag = nameof(ThirdCanvas),
                //Width = 100,
                //Height = 100,
                BackgroundColor = UIColor.Green
            };

            FouthTextBlock = new UITextView()
            {
                ScrollEnabled = false,
                //Tag = nameof(FouthTextBlock),
                //Width = 100,
                //Height = 100,
                Text = "FouthTextBlock"
            };

            FifthTextBox = new UITextField()
            {
                //Tag = nameof(FifthTextBox),
                Text = "FifthTextBox",
            };

            //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
            SixthRichTextBlock = new UITextView()
            {
                ScrollEnabled = false,
                //Tag = nameof(SixthRichTextBlock),
                AttributedText = new NSAttributedString("RichTextBlock", UIFont.SystemFontOfSize(14))
            };
        }

        private void FirstButton_Click(object sender, EventArgs e)
        {
            LoadMauiAsset();
        }

        async Task LoadMauiAsset()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("favorite_black_24dp.svg");
            Debug.WriteLine($"svg size:{stream.Length}");
        }

        public UIView GetPage()
        {
            return Page;
        }
    }
}
