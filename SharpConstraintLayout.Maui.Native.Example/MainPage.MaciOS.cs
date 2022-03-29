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
            //ConstraintLayout.DEBUG = true;
            //ConstraintLayout.MEASURE = true;

            Page = new ConstraintLayout()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                BackgroundColor = UIColor.SystemPink
            };

            createControls();

            baseAlignTest(Page);
            //baselineTest(Page);
            //guidelineTest(Page);//bug meause and arrange infinity loop
            //barrierTest(Page);
            //visibilityTest(Page);
            flowTest(Page);
            // nestedLayoutTest(Page);
        }

        private void createControls()
        {
            FirstButton = new UIButton()
            {
                BackgroundColor = UIColor.Red
            };
            FirstButton.SetTitle("FirstButton", UIControlState.Normal);
            FirstButton.TouchUpInside += FirstButton_Click;

            SecondButton = new UIButton()
            {
                BackgroundColor = UIColor.Black
            };
            SecondButton.SetTitle("SecondButton", UIControlState.Normal);

            ThirdCanvas = new UIView()
            {
                BackgroundColor = UIColor.Green
            };

            FouthTextBlock = new UITextView()
            {
                ScrollEnabled = false,
                Text = "FouthTextBlock"
            };
            FouthTextBlock.Changed += (sender, e) =>
            {
                FouthTextBlock.InvalidateIntrinsicContentSize();
                FouthTextBlock.Superview.SetNeedsLayout();
            };

            FifthTextBox = new UITextField()
            {
                Enabled = true,
                EnablesReturnKeyAutomatically = true,
                ClearButtonMode = UITextFieldViewMode.Always,
                Text = "FifthTextBox",
            };
            FifthTextBox.EditingChanged += (sender, e) =>
            {
                FifthTextBox.InvalidateIntrinsicContentSize();
                FifthTextBox.Superview.SetNeedsLayout();
            };

            //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
            SixthRichTextBlock = new UITextView()
            {
                ScrollEnabled = false,
                AttributedText = new NSAttributedString("RichTextBlock", UIFont.SystemFontOfSize(14))
            };
            SixthRichTextBlock.Changed += (sender, e) =>
            {
                SixthRichTextBlock.InvalidateIntrinsicContentSize();
                SixthRichTextBlock.Superview.SetNeedsLayout();
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
