using Android.Content;
using Android.Views;
using Android.Widget;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Core.Benchmark
{
    public class MainPage : LinearLayout
    {
        private Button MainButton;
        TextView Summary;

        public MainPage(Context? context) : base(context)
        {
            this.Orientation = Orientation.Vertical;
            this.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            SetBackgroundColor(Android.Graphics.Color.White);

            MainButton = new Button(context)
            {
                Text = "Start",
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            };

            this.AddView(MainButton);
            Summary = new TextView(context)
            {
                Text = "Text",
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            this.AddView(Summary);

            //MainButton.Click += Button_Clicked;
            MainButton.Click += SimpleClock_Button_Clicked;
        }

        async void SimpleClock_Button_Clicked(object sender, EventArgs e)
        {
            SimpleClock.BenchmarkTime(() =>
            {
                void Sleep() => Thread.Sleep(10);
            }, 20);
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
            Summary.Text = text;
            //var size = Summary.Measure(double.MaxValue, double.MaxValue).Request;
            //Summary.WidthRequest = size.Width;
            //Summary.HeightRequest = size.Height;
        }

    }
}
