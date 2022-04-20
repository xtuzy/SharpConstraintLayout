using Android.Content;
using Android.Views;
using Android.Widget;

using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Core.Benchmark
{
    public partial class MainPage : LinearLayout
    {
        private Button TestCsharpConstraintLayoutButton;
        private Button TestJavaConstraintLayoutButton;
        private Button TestSleepButton;
        TextView TestCSharpSummary;
        TextView TestJavaSummary;
        TextView TestSleepSummary;

        public MainPage(Context? context) : base(context)
        {
            this.Orientation = Orientation.Vertical;

            this.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            SetBackgroundColor(Android.Graphics.Color.White);

            var ButtonContainer = new LinearLayout(context)
            {
                Orientation = Orientation.Horizontal,
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
            };

            TestCsharpConstraintLayoutButton = new Button(context)
            {
                Text = "Start C#",
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            };

            TestJavaConstraintLayoutButton = new Button(context)
            {
                Text = "Start Java",
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            };

            TestSleepButton = new Button(context)
            {
                Text = "Start Sleep",
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            };
            ButtonContainer.AddView(TestCsharpConstraintLayoutButton);
            ButtonContainer.AddView(TestJavaConstraintLayoutButton);
            ButtonContainer.AddView(TestSleepButton);

            TestCSharpSummary = new TextView(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            TestCSharpSummary.SetBackgroundColor(Android.Graphics.Color.DarkBlue);
            TestJavaSummary = new TextView(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            TestJavaSummary.SetBackgroundColor(Android.Graphics.Color.DodgerBlue);
            TestSleepSummary = new TextView(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            TestSleepSummary.SetBackgroundColor(Android.Graphics.Color.DeepSkyBlue);

            this.AddView(ButtonContainer);
            this.AddView(TestCSharpSummary);
            this.AddView(TestJavaSummary);
            this.AddView(TestSleepSummary);

            //MainButton.Click += Button_Clicked;
            TestCsharpConstraintLayoutButton.Click += TestCsharpConstraintLayoutButton_Click;
            TestJavaConstraintLayoutButton.Click += TestJavaConstraintLayoutButton_Click;
            TestSleepButton.Click += TestSleepButton_Clicked;
        }

        private void TestJavaConstraintLayoutButton_Click(object sender, EventArgs e)
        {
            TestJavaSummary.Text = nameof(TestJavaSummary) + ":\n" + SimpleClock.BenchmarkTime(() =>
            {
                Java();
            }, 50);
            TestCSharpSummary.Invalidate();
        }

        void Java()
        {
            int viewCount = 100;
            AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidgetContainer root = new AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidgetContainer(0, 0, 1000, 600);
            AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget preview = new AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget(100, 40);
            root.Add(preview);
            preview.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Left, root, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Left);
            preview.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Bottom, root, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Bottom);
            preview.HorizontalDimensionBehaviour = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget.DimensionBehaviour.MatchConstraint;
            preview.VerticalDimensionBehaviour = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget.DimensionBehaviour.MatchConstraint;
            for (var i = 0; i < viewCount; i++)
            {
                var view = new AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget(100, 40);
                root.Add(view);
                view.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Left, preview, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Left, 1);
                view.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Right, preview, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Right, -1);
                view.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Top, preview, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Top, -1);
                view.Connect(AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Bottom, preview, AndroidX.ConstraintLayout.Core.Widgets.ConstraintAnchor.Type.Bottom);
                view.HorizontalDimensionBehaviour = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget.DimensionBehaviour.MatchConstraint;
                view.VerticalDimensionBehaviour = AndroidX.ConstraintLayout.Core.Widgets.ConstraintWidget.DimensionBehaviour.MatchConstraint;
                preview = view;
            }

            root.Layout();
        }

        /// <summary>
        /// Can't Run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void Button_Clicked(object sender, EventArgs e)
        {
            SetIsRunning(true);
            try
            {
                var logger = new AccumulationLogger();
                await Task.Run(() =>
                {
                    var config = default(IConfig);
#if DEBUG
                    config = new DebugInProcessConfig();
#endif
                    var summary = BenchmarkRunner.Run<IntroBasic>(config);
                    MarkdownExporter.Console.ExportToLog(summary, logger);
                    ConclusionHelper.Print(logger,
                            summary.BenchmarksCases
                                   .SelectMany(benchmark => benchmark.Config.GetCompositeAnalyser().Analyse(summary))
                                   .Distinct()
                                   .ToList());
                });
                SetSummary(logger.GetLog());
            }
            catch (Exception exc)
            {
                Toast.MakeText(this.Context, exc.Message, ToastLength.Long).Show();
                //await DisplayAlert("Error", exc.Message, "Ok");
            }
            finally
            {
                SetIsRunning(false);
            }
        }

        void SetIsRunning(bool isRunning)
        {
            //Indicator.IsRunning = isRunning;
            //Run.IsVisible = Summary.IsVisible = !isRunning;
        }

        void SetSummary(string text)
        {
            TestCSharpSummary.Text = text;
            //var size = Summary.Measure(double.MaxValue, double.MaxValue).Request;
            //Summary.WidthRequest = size.Width;
            //Summary.HeightRequest = size.Height;
        }

    }
}
