using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace ConsoleTranscriptionApp.Services;

public class SpeechTranscriber : IDisposable
{
    public PushAudioInputStream pushStream;

    private SpeechRecognizer recognizer;

    public SpeechTranscriber(string language, string azureKey, string azureRegion)
    {
        pushStream = new PushAudioInputStream();

        SpeechConfig speechConfigs = SpeechConfig.FromSubscription(azureKey, azureRegion);

        speechConfigs.SpeechRecognitionLanguage = language;
        speechConfigs.EnableDictation();

        recognizer = new SpeechRecognizer(speechConfigs, AudioConfig.FromStreamInput(pushStream));

        // Events
        recognizer.Recognizing += (s, e) =>
        {
            Console.WriteLine($"{e.Result.Text}");
        };

        recognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                Console.WriteLine($"\nRecognized: {e.Result.Text}");
            }
        };

        recognizer.Canceled += (s, e) =>
        {
            Console.WriteLine($"\n--- AZURE SPEECH CONNECTION CANCELED ---");

            Console.WriteLine($"Error Code: {e.ErrorCode}");
            Console.WriteLine($"Error Details: {e.ErrorDetails}");
            Console.WriteLine("\n> Please check your Azure Speech Key and Region in the config.json file. Press [ENTER] to quit");
        };

    }

    public async Task StartAsync()
    {
        await recognizer.StartContinuousRecognitionAsync();
    }

    public async Task StopAsync()
    {
        await recognizer.StopContinuousRecognitionAsync();
    }

    public void Dispose()
    {
        recognizer.Dispose();
        pushStream.Close();
    }
}
