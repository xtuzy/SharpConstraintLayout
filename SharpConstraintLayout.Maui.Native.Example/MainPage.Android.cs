using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Storage;
using SharpConstraintLayout.Maui.DebugTool;
using SharpConstraintLayout.Maui.Native.Example.Tool;
using SharpConstraintLayout.Maui.Widget;
using SharpConstraintLayout.Maui.Helper.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SharpConstraintLayout.Maui.Native.Example
{
    public partial class MainPage : ConstraintLayout
    {
        public MainPage(Context? context) : base(context)
        {
            //ConstraintLayout.DEBUG = false;
            ConstraintLayout.MEASURE_MEASURELAYOUT = true;
            Id = View.GenerateViewId();
            this.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            this.SetBackgroundColor(Color.HotPink);
            var buttonList = new LinearLayout(context) { Orientation = Orientation.Horizontal, LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent) };
            var baseAlignBt = new Button(context) { Text = "BaseAlign" };
            var baselineBt = new Button(context) { Text = "Baseline" };
            var guidelineBt = new Button(context) { Text = "Guideline" };
            var barrierBt = new Button(context) { Text = "Barrier" };
            var visibilityBt = new Button(context) { Text = "Visibility" };
            var flowBt = new Button(context) { Text = "Flow" };
            var platformLayoutInConstraintLayoutBt = new Button(context) { Text = "PlatformLayoutInConstraintLayout" };
            var constraintLayoutInScrollViewBt = new Button(context) { Text = "ConstraintLayoutInScrollView" };
            var circleConstraintBt = new Button(context) { Text = "CircleConstraint" };
            var flowPerformanceBt = new Button(context) { Text = "FlowPerformance" };
            var wrapPanelPerformanceBt = new Button(context) { Text = "WrapPanelPerformance" };
            var groupBt = new Button(context) { Text = "Group" };
            var placeholderBt = new Button(context) { Text = "Placeholder" };
            var sizeBt = new Button(context) { Text = "Size" };
            var nestedConstraintLayoutBt = new Button(context) { Text = "NestedConstraintLayout" };
            buttonList.AddView(baseAlignBt);
            buttonList.AddView(baselineBt);
            buttonList.AddView(guidelineBt);
            buttonList.AddView(barrierBt);
            buttonList.AddView(visibilityBt);
            buttonList.AddView(flowBt);
            buttonList.AddView(platformLayoutInConstraintLayoutBt);
            buttonList.AddView(constraintLayoutInScrollViewBt);
            buttonList.AddView(circleConstraintBt);
            buttonList.AddView(flowPerformanceBt);
            buttonList.AddView(wrapPanelPerformanceBt);
            buttonList.AddView(groupBt);
            buttonList.AddView(placeholderBt);
            buttonList.AddView(sizeBt);
            buttonList.AddView(nestedConstraintLayoutBt);
            var scroll = new HorizontalScrollView(context) { };
            scroll.HorizontalScrollBarEnabled = true;
            scroll.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, 30) { };
            scroll.AddView(buttonList);
            var layout = new ConstraintLayout(context);
            layout.SetBackgroundColor(Color.Yellow);
            this.AddElement(scroll);
            this.AddElement(layout);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(this);
                set.Select(scroll).TopToTop().LeftToLeft().RightToRight().Width(FluentConstraintSet.SizeBehavier.MatchConstraint)
                    .Select(layout).TopToBottom(scroll).BottomToBottom().EdgesXTo().Height(FluentConstraintSet.SizeBehavier.MatchConstraint)
                    .Width(FluentConstraintSet.SizeBehavier.MatchConstraint)
                    ;
                set.ApplyTo(this);
            }

            baseAlignBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                BaseAlignTest(layout);
            };
            baselineBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                BaselineTest(layout);
            };
            guidelineBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                GuidelineTest(layout);
            };
            barrierBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                BarrierTest(layout);
            };
            visibilityBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                VisibilityTest(layout);
            };
            flowBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                FlowTest(layout);
            };
            platformLayoutInConstraintLayoutBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                PlatformLayoutInConstraintLayoutTest(layout);
            };
            constraintLayoutInScrollViewBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                ConstraintLayoutInScrollViewTest(layout);
            };
            circleConstraintBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                CircleConstraintTest(layout);
            };
            flowPerformanceBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                FlowPerformanceTest(layout);
            };
            wrapPanelPerformanceBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                WrapPanelPerformanceTest(layout);
            };
            groupBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                GroupTest(layout);
            };
            placeholderBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                PlaceholderTest(layout);
            };
            sizeBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                SizeTest(layout);
            };
            nestedConstraintLayoutBt.Click += (sender, e) =>
            {
                layout.RemoveAllElements();
                NestedConstraintLayoutTest(layout);
            };

            //ConstraintLayoutInScrollViewTest(this);
        }

        void ConstraintLayoutInScrollViewTest(ConstraintLayout page)
        {
            (Button FirstButton, Button SecondButton, View ThirdCanvas, TextView FouthTextBlock, EditText FifthTextBox, TextView SixthRichTextBlock) = CreateControls();
            var linerlayout = new LinearLayout(page.Context)
            {
                Orientation = Orientation.Vertical,
            };
            linerlayout.SetBackgroundColor(Color.Green);
            linerlayout.AddView(new Button(page.Context) { Text = "Button1" });
            var canvas = new CustomView(page.Context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(150, 150),
            };
            linerlayout.AddView(canvas);
            canvas.SetBackgroundColor(Color.Green);
            var scrollView = new ScrollView(page.Context)
            {
                //ContentSize = new CGSize(300, 600),
            };
            scrollView.SetBackgroundColor(Color.White);
            page.AddElement(linerlayout);
            page.AddElement(scrollView);

            using (var set = new FluentConstraintSet())
            {
                set.Clone(page);
                set.Select(linerlayout)
                    .TopToTop().LeftToLeft()
                    .Width(FluentConstraintSet.SizeBehavier.MatchParent)
                    .Height(200)
                    .Select(scrollView)
                    .LeftToLeft().TopToBottom(linerlayout)
                    .Width(FluentConstraintSet.SizeBehavier.MatchParent)
                    .Height(FluentConstraintSet.SizeBehavier.MatchParent);
                set.ApplyTo(page);
            }

            var firstConstraintLayoutPage = new ConstraintLayout(page.Context)
            {
                //Frame = new CGRect(0, 0, 200, 100),
                ConstrainWidth = ConstraintSet.MatchParent,
                ConstrainHeight = ConstraintSet.WrapContent,
                ConstrainPaddingLeft = 10,
                ConstrainPaddingTop = 10,
                ConstrainPaddingRight = 10,
                ConstrainPaddingBottom = 10,
            };
            firstConstraintLayoutPage.SetBackgroundColor(Color.Pink);
            //firstConstraintLayoutPage.LayoutParameters = new ScrollView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            scrollView.AddView(firstConstraintLayoutPage);

            firstConstraintLayoutPage.AddElement(FirstButton);
            firstConstraintLayoutPage.AddElement(SecondButton);
            firstConstraintLayoutPage.AddElement(ThirdCanvas);
            firstConstraintLayoutPage.AddElement(FouthTextBlock);
            firstConstraintLayoutPage.AddElement(FifthTextBox);
            firstConstraintLayoutPage.AddElement(SixthRichTextBlock);

            //ThirdCanvas.LayoutParameters = new ViewGroup.LayoutParams(150, 150);
            using (var set = new FluentConstraintSet())
            {
                set.Clone(firstConstraintLayoutPage);
                set.Select(FirstButton)
                    .TopToTop().CenterXTo()
                    .Select(SecondButton).TopToBottom(FirstButton).CenterXTo()
                    .Select(ThirdCanvas).TopToBottom(SecondButton).CenterXTo()
                    .Height(1300).Width(FluentConstraintSet.SizeBehavier.MatchConstraint)
                    .Select(FouthTextBlock).TopToBottom(ThirdCanvas).CenterXTo()
                    .Select(FifthTextBox).TopToBottom(FouthTextBlock).CenterXTo()
                    .Select(SixthRichTextBlock).TopToBottom(FifthTextBox).CenterXTo().BottomToBottom()
                    .Select(FirstButton, SecondButton, FouthTextBlock, FifthTextBox, SixthRichTextBlock)
                    .Width(FluentConstraintSet.SizeBehavier.WrapContent)
                    .Height(FluentConstraintSet.SizeBehavier.WrapContent);
                set.ApplyTo(firstConstraintLayoutPage);
            }

            Task.Run(async () =>
                  {
                      await Task.Delay(3000);//wait ui show
                      UIThread.Invoke(() =>
                         {
                             SimpleDebug.WriteLine("firstConstraintLayoutPage:" + firstConstraintLayoutPage.GetViewLayoutInfo());
                             //SimpleDebug.WriteLine("FirstButton:" + FirstButton.GetViewLayoutInfo());
                             //SimpleDebug.WriteLine("scrollView:" + scrollView.GetViewLayoutInfo());
                             //SimpleDebug.WriteLine("page:" + page.GetViewLayoutInfo());
                         }, page);
                  });
        }

        private (Button FirstButton, Button SecondButton, View ThirdCanvas, TextView FouthTextBlock, EditText FifthTextBox, TextView SixthRichTextBlock) CreateControls()
        {
            var FirstButton = new Button(this.Context)
            {
                Id = View.GenerateViewId(),
                Text = "FirstButton At Center",
            };
            FirstButton.SetBackgroundColor(Color.Red);
            FirstButton.SetTextColor(Color.White);

            var SecondButton = new Button(this.Context)
            {
                Id = View.GenerateViewId(),
                Text = "Second Button",
            };
            SecondButton.SetBackgroundColor(Color.Black);
            SecondButton.SetTextColor(Color.White);

            var ThirdCanvas = new CustomView(this.Context)
            {
                Id = View.GenerateViewId(),
            };
            ThirdCanvas.SetBackgroundColor(Color.LightGreen);

            var FouthTextBlock = new TextView(this.Context)
            {
                Id = View.GenerateViewId(),
                Tag = "FouthTextBlock",
                Text = "TextBlock"
            };

            var FifthTextBox = new EditText(this.Context)
            {
                Id = View.GenerateViewId(),
                Tag = "FifthTextBox",
                Text = "TextBox",
            };

            var SixthRichTextBlock = new TextView(this.Context)
            {
                Id = View.GenerateViewId(),
                Text = "RichTextBlock",
                TextSize = 18,
            };

            return (FirstButton, SecondButton, ThirdCanvas, FouthTextBlock, FifthTextBox, SixthRichTextBlock);
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            Toast.MakeText((sender as View).Context, "Clicked", ToastLength.Short).Show();
        }

        async Task LoadMauiAsset()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("favorite_black_24dp.svg");
            System.Diagnostics.Debug.WriteLine($"svg size:{stream.Length}");
        }

        public class CustomView : View
        {
            public CustomView(Context context) : base(context)
            {
            }

            protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
                SimpleDebug.WriteLine($"CustomView OnMeasure:{MeasureSpeToString(widthMeasureSpec)}:{MeasureSpeToString(heightMeasureSpec)} => ({MeasuredWidth},{this.MeasuredHeight}) Parent={this.Parent}");

                //参考Android开发艺术探索:自定义View需要自己设置WrapContent,不然大小和MatchParent一样
                int desiredWidth = 100;
                int desiredHeight = 100;

                MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
                int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
                MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);
                int heightSize = MeasureSpec.GetSize(heightMeasureSpec);

                if (widthMode == MeasureSpecMode.AtMost && heightMode == MeasureSpecMode.AtMost)
                {
                    SetMeasuredDimension(desiredWidth, desiredHeight);
                }
                else if (widthMode == MeasureSpecMode.AtMost)
                {
                    SetMeasuredDimension(desiredWidth, heightSize);
                }
                else if (heightMode == MeasureSpecMode.AtMost)
                {
                    SetMeasuredDimension(widthSize, desiredHeight);
                }
            }

            public string MeasureSpeToString(int measureSpec)
            {
                MeasureSpecMode mode = MeasureSpec.GetMode(measureSpec);
                int size = MeasureSpec.GetSize(measureSpec);

                StringBuilder sb = new StringBuilder("MeasureSpec: ");

                if (mode == MeasureSpecMode.Unspecified)
                    sb.Append("UNSPECIFIED ");
                else if (mode == MeasureSpecMode.Exactly)
                    sb.Append("EXACTLY ");
                else if (mode == MeasureSpecMode.AtMost)
                    sb.Append("AT_MOST ");
                else
                    sb.Append(mode).Append(" ");

                sb.Append(size);
                return sb.ToString();
            }
        }
    }
}
