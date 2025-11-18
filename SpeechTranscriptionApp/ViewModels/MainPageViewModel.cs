using System.Collections.ObjectModel;
using SpeechTranscriptionApp.Services.SpeechTranscriber;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeechTranscriptionApp.Services.AudioRecorder;

namespace SpeechTranscriptionApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly ISpeechTranscriberFactory _speechTranscriberFactory;

        private readonly IAudioRecorder _audioRecorder;

        private readonly ISpeechTranscriber _speechTranscriber;

        private bool _isRecognizing = false;

        [ObservableProperty] private ObservableCollection<string> _recognizedPhrases;

        [ObservableProperty] private string _currentPhrase = "";

        [ObservableProperty] private string _startStopButtonText = "Start";

        public MainPageViewModel(ISpeechTranscriberFactory speechTranscriberFactory, IAudioRecorder audioRecorder)
        {
            RecognizedPhrases = new ObservableCollection<string>();

            _speechTranscriberFactory = speechTranscriberFactory;
            _audioRecorder = audioRecorder;

            _speechTranscriber = speechTranscriberFactory.Create("en-us", "XD", "XD", _audioRecorder);

            _speechTranscriber.PhraseRecognized += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    RecognizedPhrases.Add(e);
                });
            };

            _speechTranscriber.Recognizing += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CurrentPhrase = e;
                });
            };
        }

        private void _speechTranscriber_Recognizing(object? sender, string e)
        {
            throw new NotImplementedException();
        }

        [RelayCommand]
        public async Task ToggleTranscription()
        {
            if (_isRecognizing)
            {
                await _speechTranscriber.StopAsync();
                _isRecognizing = false;
                StartStopButtonText = "Start";
            }
            else
            {
                await _speechTranscriber.StartAsync();
                _isRecognizing = true;
                StartStopButtonText = "Stop";
            }
        }
    }
}
