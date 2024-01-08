/// By Henderson Bare & Brett Baxter
/// Created: 3/22/2023
/// Course: CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Henderson Bare - This work may not be copied for use in academic course work
/// 
/// I, Henderson Bare and Brett Baxter, certify that I wrote this code from scratch and did not copy it in part or in whole from another source.
/// All references used in the completion of the assignment are cited in my README files and commented in the code.

using Microsoft.Extensions.Logging;

namespace ChatServer
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
                })
                .Services.AddLogging(configure =>
                {
                    configure.AddDebug();
                    configure.SetMinimumLevel(LogLevel.Debug);
                })
                .AddTransient<MainPage>();

            return builder.Build();
        }

    }
}