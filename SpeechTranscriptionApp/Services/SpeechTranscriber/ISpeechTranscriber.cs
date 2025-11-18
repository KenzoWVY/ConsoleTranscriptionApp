namespace SpeechTranscriptionApp.Services.SpeechTranscriber
{
    public interface ISpeechTranscriber : IDisposable
    {
        event EventHandler<string>? Recognizing;
        event EventHandler<string>? PhraseRecognized;

        Task StartAsync(); 
        Task StopAsync();
    }
}
