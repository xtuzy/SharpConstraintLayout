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
    public class MainPage
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
                BackgroundColor = UIColor.SystemPinkColor
            }; 
            layout = new ConstraintLayout()
            {
                //Frame = frame,
                //AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                BackgroundColor = UIColor.White
            };
            var guide = new Guideline()
            {
                GuidelinePercent = 0.3f,
            };
            Page.AddView(layout);
            Page.AddView(guide);

            var pageSet = new ConstraintSet();
            pageSet.Clone(Page);
            pageSet.SetGuidelineOrientation(guide.GetId(), ConstraintSet.HORIZONTAL);
            //pageSet.SetGuidelinePercent(guide.GetId(), 0.5f);
            pageSet.ConstrainWidth(guide.GetId(), ConstraintSet.WRAP_CONTENT);
            pageSet.ConstrainHeight(guide.GetId(), ConstraintSet.WRAP_CONTENT);

            pageSet.Connect(layout.GetId(), ConstraintSet.LEFT, Page.GetId(), ConstraintSet.LEFT);
            pageSet.Connect(layout.GetId(), ConstraintSet.RIGHT, Page.GetId(), ConstraintSet.RIGHT);
            pageSet.Connect(layout.GetId(), ConstraintSet.TOP, guide.GetId(), ConstraintSet.TOP);
            pageSet.Connect(layout.GetId(), ConstraintSet.BOTTOM, Page.GetId(), ConstraintSet.BOTTOM);
            pageSet.ConstrainWidth(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            pageSet.ConstrainHeight(layout.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            pageSet.ApplyTo(Page);

            FirstButton = new UIButton() 
            {
                //Tag = nameof(FirstButton),
                //ClickMode = ClickMode.Press,
                BackgroundColor = UIColor.Red
            };
            FirstButton.SetTitle("FirstButton",UIControlState.Normal);
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

            layout.AddView(FirstButton);
            layout.AddView(SecondButton);
            layout.AddView(ThirdCanvas);
            layout.AddView(FouthTextBlock);
            layout.AddView(FifthTextBox);
            layout.AddView(SixthRichTextBlock);

            var set = new ConstraintSet();
            set.Clone(layout);

            set.Connect(FirstButton.GetId(), ConstraintSet.LEFT, layout.GetId(), ConstraintSet.LEFT);
            set.Connect(FirstButton.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT);
            set.CenterVertically(FirstButton.GetId(), ConstraintSet.PARENT_ID, ConstraintSet.TOP, 0, ConstraintSet.PARENT_ID, ConstraintSet.BOTTOM, 0, 0.5f);
            set.ConstrainWidth(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);
            set.ConstrainHeight(FirstButton.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(SecondButton.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            set.Connect(SecondButton.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.ConstrainWidth(SecondButton.GetId(), 150);
            set.ConstrainHeight(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.RIGHT, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.RIGHT, layout.GetId(), ConstraintSet.RIGHT, 10);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.BOTTOM, layout.GetId(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(ThirdCanvas.GetId(), ConstraintSet.GONE);//BUG:设置Visibility无效
            set.ConstrainWidth(ThirdCanvas.GetId(), ConstraintSet.MATCH_CONSTRAINT);
            set.ConstrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_PARENT);

            set.Connect(FouthTextBlock.GetId(), ConstraintSet.RIGHT, SecondButton.GetId(), ConstraintSet.RIGHT);
            set.Connect(FouthTextBlock.GetId(), ConstraintSet.TOP, SecondButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.ConstrainWidth(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            set.ConstrainHeight(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(FifthTextBox.GetId(), ConstraintSet.BOTTOM, FirstButton.GetId(), ConstraintSet.TOP, 50);
            set.Connect(FifthTextBox.GetId(), ConstraintSet.LEFT, SecondButton.GetId(), ConstraintSet.LEFT);
            set.Connect(FifthTextBox.GetId(), ConstraintSet.RIGHT, SecondButton.GetId(), ConstraintSet.RIGHT);
            set.ConstrainWidth(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);//BUG:设置宽高具体数值,位置错误
            set.ConstrainHeight(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(SixthRichTextBlock.GetId(), ConstraintSet.RIGHT, FouthTextBlock.GetId(), ConstraintSet.LEFT, 10);
            set.Connect(SixthRichTextBlock.GetId(), ConstraintSet.BASELINE, FouthTextBlock.GetId(), ConstraintSet.BASELINE);//BUG:Baseline无效
            set.ConstrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            set.ConstrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);

            set.ApplyTo(layout);
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
