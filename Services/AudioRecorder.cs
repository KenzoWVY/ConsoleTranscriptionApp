using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;

namespace ConsoleTranscriptionApp.Services;

public class AudioRecorder : IDisposable
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
