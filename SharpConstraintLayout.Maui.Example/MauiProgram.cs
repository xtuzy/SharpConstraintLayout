using MauiGestures;
using SharpConstraintLayout.Maui.Widget;

namespace SharpConstraintLayout.Maui.Example;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(handler =>
            {
                //handler.AddHandler(typeof(ConstraintLayout), typeof(Microsoft.Maui.Handlers.LayoutHandler));
            })
            .AddAdvancedGestures();

        return builder.Build();
    }
}
