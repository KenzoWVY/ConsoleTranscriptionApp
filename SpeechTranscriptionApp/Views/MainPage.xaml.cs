using SpeechTranscriptionApp.Services.AudioRecorder;
using SpeechTranscriptionApp.Services.SpeechTranscriber;
using SpeechTranscriptionApp.ViewModels;

namespace SpeechTranscriptionApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(ISpeechTranscriberFactory speechTranscriberFactory, IAudioRecorder audioRecorder)
        {
            MainPageViewModel viewModel = new MainPageViewModel(speechTranscriberFactory, audioRecorder);
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}
