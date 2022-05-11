using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Storage;
using SharpConstraintLayout.Maui.DebugTool;
using SharpConstraintLayout.Maui.Native.Example.Tool;
using SharpConstraintLayout.Maui.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SharpConstraintLayout.Maui.Native.Example
{
    public partial class MainPage : ConstraintLayout
    {
        private Button FirstButton;
        public Button SecondButton;
        private View ThirdCanvas;
        private TextView FouthTextBlock;
        private EditText FifthTextBox;
        private TextView SixthRichTextBlock;
        public ConstraintLayout layout;

        public MainPage(Context? context) : base(context)
        {
            ConstraintLayout.DEBUG = false;
            ConstraintLayout.MEASURE_MEASURELAYOUT = true;
            Id = View.GenerateViewId();
            this.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            createControls();
            this.SetBackgroundColor(Color.HotPink);

            //BaseAlignTest(this);
            //BaselineTest(this);//how to calculate baseline of TextBox,Label
            //GuidelineTest(this);//OK
            //BarrierTest(this);//OK
            //VisibilityTest(this);//OK
            //FlowTest(this);//bad performance
            //NestedConstraintLayoutTest(this);
            //PlatformLayoutInConstraintLayoutTest(this);//OK
            //AnimationTest(this);
            //CircleConstraintTest(this);
            //FlowPerformanceTest(this);//bad performance,need 20~40ms
            //WrapPanelPerformanceTest(this);//fast 3~7ms
            //GroupTest(this);
            //PlaceholderTest(this);
            //SizeTest(this);
            ConstraintLayoutInScrollViewTest(this);
        }

        void ConstraintLayoutInScrollViewTest(ConstraintLayout page)
        {
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

        private void createControls()
        {
            FirstButton = new Button(this.Context)
            {
                Id = View.GenerateViewId(),
                Text = "FirstButton At Center",
            };
            FirstButton.SetBackgroundColor(Color.Red);
            FirstButton.SetTextColor(Color.White);

            SecondButton = new Button(this.Context)
            {
                Id = View.GenerateViewId(),
                Text = "Second Button",
            };
            SecondButton.SetBackgroundColor(Color.Black);
            SecondButton.SetTextColor(Color.White);

            ThirdCanvas = new CustomView(this.Context)
            {
                Id = View.GenerateViewId(),
            };
            ThirdCanvas.SetBackgroundColor(Color.LightGreen);

            FouthTextBlock = new TextView(this.Context)
            {
                Id = View.GenerateViewId(),
                Tag = nameof(FouthTextBlock),
                Text = "TextBlock"
            };

            FifthTextBox = new EditText(this.Context)
            {
                Id = View.GenerateViewId(),
                Tag = nameof(FifthTextBox),
                Text = "TextBox",
            };

            SixthRichTextBlock = new TextView(this.Context)
            {
                Id = View.GenerateViewId(),
                Text = "RichTextBlock",
                TextSize = 18,
            };
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
