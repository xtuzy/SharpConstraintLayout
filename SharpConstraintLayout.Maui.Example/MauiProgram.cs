namespace SharpConstraintLayout.Maui.Example
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                   // fonts.AddFont("OpenSans_Regular.ttf", "OpenSansRegular");
                });

            return builder.Build();
        }
    }
}