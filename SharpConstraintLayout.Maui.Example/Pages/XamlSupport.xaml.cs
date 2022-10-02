using SharpConstraintLayout.Maui.Widget;

namespace SharpConstraintLayout.Maui.Example.Pages;

public partial class XamlSupport : ContentView
{
	public XamlSupport()
	{
		InitializeComponent();
	}

	private void layout_SizeChanged(object sender, EventArgs e)
	{
		using(var set = new FluentConstraintSet())
		{
			set.Clone(layout);
			set.Select(first).CenterTo();
			set.ApplyTo(layout);
		}
    }
}