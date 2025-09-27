using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;

namespace ConsoleTranscriptionApp.Services;

public class SpeechTranscriber : IDisposable
{
    public PushAudioInputStream pushStream;

    private SpeechRecognizer recognizer;

    private bool _sessionEnded = false;

    private Queue<string> lastPhrases = new Queue<string>();

    public SpeechTranscriber(string language, string azureKey, string azureRegion)
    {
        pushStream = new PushAudioInputStream();

        SpeechConfig speechConfigs = SpeechConfig.FromSubscription(azureKey, azureRegion);
        speechConfigs.SpeechRecognitionLanguage = language;
        speechConfigs.EnableDictation();


        recognizer = new SpeechRecognizer(speechConfigs, AudioConfig.FromStreamInput(pushStream));

        // Event subscriptions

        // Event: Print real-time transcription
        recognizer.Recognizing += (s, e) =>
        {
            Console.SetCursorPosition(1, 6);
            Console.Write($"{e.Result.Text}");
        };

        // Event: Print phrase after fully recognized
        recognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                if (lastPhrases.Count > 5) lastPhrases.Dequeue();

                lastPhrases.Enqueue(e.Result.Text);

                Console.Clear();

                Console.SetCursorPosition(1, 20);
                Console.WriteLine($"[Recognized]:");
                foreach (string phrase in lastPhrases)
                {
                    Console.WriteLine(phrase + "\n");
                }
            }
        };

        // Event: Send error message in case connection with API fails
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

        // Event: Set flag up when connection with API ends due to silence
        recognizer.SessionStopped += (s, e) =>
        {
            _sessionEnded = true;
        };

    }

    public void PushAudio(byte[] buffer, int bytes)
    {
        pushStream.Write(buffer, bytes);
        if (_sessionEnded && bytes > 0) StartAsync();
    }

    public async Task StartAsync()
    {
        _sessionEnded = false;
        await recognizer.StartContinuousRecognitionAsync();
    }

    public async Task StopAsync()
    {
        _sessionEnded = false;
        await recognizer.StopContinuousRecognitionAsync();
    }

    public void Dispose()
    {
        recognizer.Dispose();
        pushStream.Close();
    }
}
