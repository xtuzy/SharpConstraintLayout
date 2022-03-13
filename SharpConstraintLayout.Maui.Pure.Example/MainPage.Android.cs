using Android.Content;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Pure.Example
{
    public class MainPage : RelativeLayout
    {
        private Button MainButton;

        public MainPage(Context? context) : base(context)
        {
            this.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            MainButton = new Button(context)
            {
                Text = "MainButton321",
                LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            };
            ((RelativeLayout.LayoutParams)(MainButton.LayoutParameters)).SetMargins(50, 50, 0, 0);
            this.AddView(MainButton);
            //this.AddView(new TextView(context) { Text = "Text" });
            SetBackgroundColor(Android.Graphics.Color.Pink);
            MainButton.Click += MainButton_Click;
            LoadMauiAsset();
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
