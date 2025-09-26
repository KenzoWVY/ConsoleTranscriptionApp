using Microsoft.CognitiveServices.Speech.Audio;
using NAudio.Wave;

namespace ConsoleTranscriptionApp.Services;

public class AudioRecorder : IDisposable
{
    private WasapiLoopbackCapture capture;

    public AudioRecorder(SpeechTranscriber transcriber)
    {
        var pushStream = transcriber.pushStream;
        capture = new WasapiLoopbackCapture();
        capture.WaveFormat = new WaveFormat(16000, 16, 1);

        capture.DataAvailable += async (s, a) =>
        {
            pushStream.Write(a.Buffer, a.BytesRecorded);
            if (transcriber.sessionEnded && a.BytesRecorded > 0) transcriber.StartAsync();
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
