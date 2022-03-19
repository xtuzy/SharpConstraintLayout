using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using Microsoft.Maui.Essentials;
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
            Id = View.GenerateViewId();
            this.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            createControls();
            this.SetBackgroundColor(Color.HotPink);

            //controlsTest(this);
            //baselineTest(this);
            //guidelineTest(this);
            //barrierTest(this);
            //visibilityTest(this);
            flowTest(this);
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

            ThirdCanvas = new View(this.Context)
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
    }
}
