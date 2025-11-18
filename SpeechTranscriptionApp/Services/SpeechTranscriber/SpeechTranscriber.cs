using System.Diagnostics;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SpeechTranscriptionApp.Services.AudioRecorder;

namespace SpeechTranscriptionApp.Services.SpeechTranscriber
{

    public class SpeechTranscriber : ISpeechTranscriber
    {
        private PushAudioInputStream _pushStream;

        private SpeechRecognizer _recognizer;

        private bool _sessionEnded = false;

        private IAudioRecorder _audioRecorder;

        public event EventHandler<string>? Recognizing;

        public event EventHandler<string>? PhraseRecognized;

        public SpeechTranscriber(string language, string azureKey, string azureRegion, IAudioRecorder audioRecorder)
        {
            _pushStream = new PushAudioInputStream();

            SpeechConfig speechConfigs = SpeechConfig.FromSubscription(azureKey, azureRegion);
            speechConfigs.SpeechRecognitionLanguage = language;
            speechConfigs.EnableDictation();


            _recognizer = new SpeechRecognizer(speechConfigs, AudioConfig.FromStreamInput(_pushStream));

            _audioRecorder = audioRecorder;

            // Event: Push audio data from recorder to recognizer
            _audioRecorder.AudioOutput += (s, a) =>
            {
                this.PushAudio(a.Buffer, a.BytesRecorded);
            };

            // Event: Send partial phrase to subscribers while recognizing
            _recognizer.Recognizing += (s, e) =>
            {
                Trace.WriteLine("Recognizing: ");
                Recognizing?.Invoke(this, e.Result.Text);
            };

            // Event: Send phrase to subscribers after fully recognized
            _recognizer.Recognized += (s, e) =>
            {
                Trace.WriteLine("Recognized: " + e.Result.Text);
                PhraseRecognized?.Invoke(this, e.Result.Text);
            };

            // Event: Send error message in case connection with API fails
            _recognizer.Canceled += (s, e) =>
            {
                Trace.WriteLine("\n\nCONNECTION ERROR\n");
            };

            // Event: Set flag up in case connection with API ends abruptly
            _recognizer.SessionStopped += (s, e) =>
            {
                Trace.WriteLine("\n\nSESSION INTERRUPTED\n\n");
                _sessionEnded = true;
            };

        }

        private void PushAudio(byte[] buffer, int bytes)
        {
            if (bytes > 0)
            {
                Trace.WriteLine(bytes);
                _pushStream.Write(buffer, bytes);

                if (_sessionEnded)
                {
                    Task.Run(StartAsync);
                }
            }
        }

        public async Task StartAsync()
        {
            _sessionEnded = false;
            await _recognizer.StartContinuousRecognitionAsync();
            _audioRecorder.Start();
        }

        public async Task StopAsync()
        {
            _sessionEnded = false;
            await _recognizer.StopContinuousRecognitionAsync();
            _audioRecorder.Stop();
        }

        public void Dispose()
        {
            _recognizer.Dispose();
            _pushStream.Close();
            _audioRecorder.Dispose();
        }
    }
}