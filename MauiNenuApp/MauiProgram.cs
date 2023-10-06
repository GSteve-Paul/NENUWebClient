using Microsoft.Extensions.Logging;

namespace MauiNenuApp
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
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
//dotnet publish "D:\codes\Nenu\MauiNenuApp\MauiNenuApp.csproj" -c Release -f net7.0-windows10.0.19041.0
//https://learn.microsoft.com/en-us/dotnet/maui/windows/deployment/publish-cli