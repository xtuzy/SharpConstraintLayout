using Microsoft.Maui.Accessibility;
using Microsoft.Maui.Controls;
using System;
using System.Threading;

namespace SharpConstraintLayout.Core.Benchmark
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void CSharpBasisConstraintTestButton_Clicked(object sender, EventArgs e)
        {

            CSharpBasisConstraintTestSummary.Text = nameof(CSharpBasisConstraintTestSummary) + ":\n" + SimpleClock.BenchmarkTime(() =>
            {
                new CSharpConstraintLayoutTest().CSharpBasisConstraintTest(1000);
            }, 1);
        }

        private void JavaBasisConstraintTestButton_Clicked(object sender, EventArgs e)
        {
#if __ANDROID__
            JavaBasisConstraintTestSummary.Text = nameof(JavaBasisConstraintTestSummary) + ":\n" + SimpleClock.BenchmarkTime(() =>
            {
                new JavaConstraintLayoutTest().JavaBasisConstraintTest(1000);
            }, 1);
#endif
        }

        private void CSharpFlowConstraintTestButton_Clicked(object sender, EventArgs e)
        {
            CSharpFlowConstraintTestSummary.Text = nameof(CSharpFlowConstraintTestSummary) + ":\n" + SimpleClock.BenchmarkTime(() =>
            {
                new CSharpConstraintLayoutTest().CSharpFlowConstraintTest(1000);
            }, 1);
        }

        private void SleepTestButton_Clicked(object sender, EventArgs e)
        {
            SleepTestSummary.Text = nameof(SleepTestSummary) + ":\n" + SimpleClock.BenchmarkTime(() =>
            {
                SleepTest();
            }, 10);
        }

        void SleepTest()
        {
            Thread.Sleep(10);
        }
    }
}