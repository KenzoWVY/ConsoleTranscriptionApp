using System.Collections.ObjectModel;
using System.Diagnostics;
using SpeechTranscriptionApp.Services.SpeechTranscriber;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeechTranscriptionApp.Services.AudioRecorder;
using SpeechTranscriptionApp.Views;

namespace SpeechTranscriptionApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly ISpeechTranscriberFactory _speechTranscriberFactory;

        private readonly IAudioRecorder _audioRecorder;

        private readonly ISpeechTranscriber _speechTranscriber;

        private bool _isRecognizing = false;

        [ObservableProperty] private ObservableCollection<string> recognizedPhrases = new ObservableCollection<string>();

        [ObservableProperty] private string _currentPhrase = "";

        [ObservableProperty] private string _startStopButtonText = "Start";

        public MainPageViewModel(ISpeechTranscriberFactory speechTranscriberFactory, IAudioRecorder audioRecorder)
        {
            _speechTranscriberFactory = speechTranscriberFactory;
            _audioRecorder = audioRecorder;

            _speechTranscriber = speechTranscriberFactory.Create("en-us", "no", "no", _audioRecorder);

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

                foreach (string phrase in RecognizedPhrases)
                {
                    Trace.WriteLine(phrase);
                }
            }
        }

        [RelayCommand]
        public async Task ChangeConfigs()
        {
            if (_isRecognizing)
            { 
                await _speechTranscriber.StopAsync();
            }

            // await Shell.Current.GoToAsync(nameof(ConfigsPage));
        }
    }
}
