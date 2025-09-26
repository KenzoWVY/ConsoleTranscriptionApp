using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace ConsoleTranscriptionApp.Services;

public class SpeechTranscriber : IDisposable
{
    public PushAudioInputStream pushStream;

    private SpeechRecognizer recognizer;

    private bool _manualStopFlag = false;

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
                Console.WriteLine($"\nRecognized: {e.Result.Text}\n");
            }
        };

        recognizer.Canceled += (s, e) =>
        {
            if (e.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"\n--- AZURE SPEECH CONNECTION CANCELED ---");

                Console.WriteLine($"Error Code: {e.ErrorCode}");
                Console.WriteLine($"Error Details: {e.ErrorDetails}");
                Console.WriteLine("\n> Check Azure Speech Key and Region in the config.json file. Press [ENTER] to quit");
            }
        };

        recognizer.SessionStopped += (s, e) =>
        {
            if (!_manualStopFlag)
            {
                recognizer.StartContinuousRecognitionAsync();
            }
        };

    }

    public async Task StartAsync()
    {
        await recognizer.StartContinuousRecognitionAsync();
    }

    public async Task StopAsync()
    {
        _manualStopFlag = true;
        await recognizer.StopContinuousRecognitionAsync();
    }

    public void Dispose()
    {
        recognizer.Dispose();
        pushStream.Close();
    }
}
