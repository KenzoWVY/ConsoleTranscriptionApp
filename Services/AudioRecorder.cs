using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;

namespace ConsoleTranscriptionApp.Services;

public class AudioRecorder : IDisposable
{
    private readonly WaveFormat captureFormat = new WaveFormat(16000, 16, 1);

    private WasapiLoopbackCapture capture;

    public AudioRecorder(PushAudioInputStream pushStream)
    {
        capture = new WasapiLoopbackCapture();
        capture.WaveFormat = captureFormat;

        capture.DataAvailable += (s, a) =>
        {
            pushStream.Write(a.Buffer, a.BytesRecorded);
        };
    }

    public void Start()
    {
        capture.StartRecording();
    }

    public void Stop()
    {
        capture.StopRecording();
    }

    public void Dispose()
    {
        capture.Dispose();
    }
}
