using SpeechTranscriptionApp.Services.AudioRecorder;

namespace SpeechTranscriptionApp.Services.SpeechTranscriber
{
    public class SpeechTranscriberFactory : ISpeechTranscriberFactory
    { 
        public ISpeechTranscriber Create(string language, string azureKey, string azureRegion, IAudioRecorder audioRecorder)
        {
            return new SpeechTranscriber(language, azureKey, azureRegion, audioRecorder);
        }
    }
}
