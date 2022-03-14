using CoreGraphics;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using SharpConstraintLayout.Maui.Pure.Core;
using Foundation;

namespace SharpConstraintLayout.Maui.Pure.Example
{
    public class MainPage
    {
        private ConstraintLayout layout;
        public UIView Page;
        private UIButton FirstButton;
        private UIButton SecondButton;
        private UIView ThirdCanvas;
        private UITextView FouthTextBlock;
        private UITextField FifthTextBox;
        private UITextView SixthRichTextBlock;

        public MainPage(CGRect frame)
        {
            
            Page = new UIStackView(frame)
            {
                BackgroundColor = UIColor.SystemYellowColor,
                Axis = UILayoutConstraintAxis.Vertical
            };
            layout = new ConstraintLayout()
            {
                Frame = frame,
                BackgroundColor = UIColor.White
            };
            Page.AddSubview(layout);

            FirstButton = new UIButton()
            {
                //Tag = nameof(FirstButton),
                //ClickMode = ClickMode.Press,
                BackgroundColor = UIColor.Red
            };
            FirstButton.SetTitle("FirstButton At Center",UIControlState.Normal);
            //FirstButton.Click += MainButton_Click;

            SecondButton = new UIButton()
            {
                //Width = 100,Height = 100,
                //Tag = nameof(SecondButton),
                BackgroundColor = UIColor.Black
            };
            SecondButton.SetTitle("Second At Bottom", UIControlState.Normal);

            ThirdCanvas = new UIView()
            {
                //Tag = nameof(ThirdCanvas),
                //Width = 100,
                //Height = 100,
                BackgroundColor = UIColor.Green
            };

            FouthTextBlock = new UITextView()
            {
                //Tag = nameof(FouthTextBlock),
                //Width = 100,
                //Height = 100,
                Text = "TextBlock"
            };

            FifthTextBox = new UITextField()
            {
                //Tag = nameof(FifthTextBox),
                Text = "TextBox",
            };

            //https://stackoverflow.com/questions/35710355/uwpc-adding-text-to-richtextblock
            SixthRichTextBlock = new UITextView()
            {
                //Tag = nameof(SixthRichTextBlock),
                AttributedText = new NSAttributedString("RichTextBlock", UIFont.SystemFontOfSize(40))
            };

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

            set.Connect(SecondButton.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            set.Connect(SecondButton.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(SecondButton.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(SecondButton.GetId(), ConstraintSet.WRAP_CONTENT);
            set.constrainHeight(SecondButton.GetId(), 200);

            set.Connect(ThirdCanvas.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.RIGHT, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.Connect(ThirdCanvas.GetId(), ConstraintSet.BOTTOM, layout.GetId(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(ThirdCanvas.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(ThirdCanvas.GetId(), 200);
            set.constrainHeight(ThirdCanvas.GetId(), ConstraintSet.MATCH_PARENT);

            set.Connect(FouthTextBlock.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.LEFT, 50);
            set.Connect(FouthTextBlock.GetId(), ConstraintSet.TOP, FirstButton.GetId(), ConstraintSet.BOTTOM, 50);
            set.setVisibility(FouthTextBlock.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(FouthTextBlock.GetId(), 200);

            set.constrainHeight(FouthTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(FifthTextBox.GetId(), ConstraintSet.BOTTOM, FirstButton.GetId(), ConstraintSet.TOP, 50);
            set.Connect(FifthTextBox.GetId(), ConstraintSet.LEFT, FirstButton.GetId(), ConstraintSet.LEFT);
            set.Connect(FifthTextBox.GetId(), ConstraintSet.RIGHT, FirstButton.GetId(), ConstraintSet.RIGHT);
            set.setVisibility(FifthTextBox.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(FifthTextBox.GetId(), 200);
            set.constrainHeight(FifthTextBox.GetId(), ConstraintSet.WRAP_CONTENT);

            set.Connect(SixthRichTextBlock.GetId(), ConstraintSet.RIGHT, FouthTextBlock.GetId(), ConstraintSet.LEFT, 50);
            set.Connect(SixthRichTextBlock.GetId(), ConstraintSet.BASELINE, FouthTextBlock.GetId(), ConstraintSet.BASELINE, 50);
            set.setVisibility(SixthRichTextBlock.GetId(), ConstraintSet.VISIBLE);
            set.constrainWidth(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);
            set.constrainHeight(SixthRichTextBlock.GetId(), ConstraintSet.WRAP_CONTENT);


            set.ApplyTo_Android(layout);
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
