using NAudio.Wave;

namespace SpeechTranscriptionApp.Services.AudioRecorder
{
    public class AudioRecorder : IAudioRecorder
    {
        private WasapiLoopbackCapture _capture;

        public event EventHandler<WaveInEventArgs>? AudioOutput;

        public AudioRecorder()
        {
            _capture = new WasapiLoopbackCapture();
            _capture.WaveFormat = new WaveFormat(16000, 16, 1);

            _capture.DataAvailable += (s, a) =>
            {
                AudioOutput?.Invoke(this, a);
            };
        }

        public void Start()
        {
            _capture.StartRecording();
        }

        public void Stop()
        {
            _capture.StopRecording();
        }

        public void Dispose()
        {
            _capture.Dispose();
        }
    }
}