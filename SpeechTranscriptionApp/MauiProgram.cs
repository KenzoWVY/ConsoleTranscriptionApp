using Microsoft.Extensions.Logging;
using SpeechTranscriptionApp.Services.AudioRecorder;
using SpeechTranscriptionApp.Services.SpeechTranscriber;

namespace SpeechTranscriptionApp
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

            builder.Services.AddSingleton<IAudioRecorder, AudioRecorder>();
            builder.Services.AddSingleton<ISpeechTranscriberFactory, SpeechTranscriberFactory>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
