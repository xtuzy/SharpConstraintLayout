using SharpConstraintLayout.Maui.Widget;

namespace SharpConstraintLayout.Maui.Example.Pages;

public partial class TestContentView : ContentView
{
    public TestContentView()
    {
        InitializeComponent();
        var layout = new ConstraintLayout();
        this.Content = layout;
        var button1 = new Button() { Text = "Button" };
        layout.Add(button1);
        using (var startset = new FluentConstraintSet())
        {
            startset.Clone(layout);
            startset.Select(button1).CenterTo()
                ;
            startset.ApplyTo(layout);
        }
    }
}