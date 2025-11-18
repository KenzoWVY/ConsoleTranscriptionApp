using NAudio.Wave;

namespace SpeechTranscriptionApp.Services.AudioRecorder
{
    public interface IAudioRecorder : IDisposable
    {
        event EventHandler<WaveInEventArgs>? AudioOutput;

        void Stop();
        void Start();
    }
}
