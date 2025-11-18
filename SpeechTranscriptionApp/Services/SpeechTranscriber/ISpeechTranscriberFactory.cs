using SpeechTranscriptionApp.Services.AudioRecorder;

namespace SpeechTranscriptionApp.Services.SpeechTranscriber
{
    public interface ISpeechTranscriberFactory
    {
        ISpeechTranscriber Create(string language, string azureKey, string azureRegion, IAudioRecorder audioRecorder);
    }
}
