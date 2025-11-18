using System.Diagnostics;
using SpeechTranscriptionApp.Services.AudioRecorder;
using SpeechTranscriptionApp.Services.SpeechTranscriber;
using SpeechTranscriptionApp.ViewModels;

namespace SpeechTranscriptionApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(ISpeechTranscriberFactory speechTranscriberFactory, IAudioRecorder audioRecorder)
        {
            InitializeComponent();

            MainPageViewModel viewModel = new MainPageViewModel(speechTranscriberFactory, audioRecorder);
            BindingContext = viewModel;
        }

        private void PhraseHistoryLoaded(object s, EventArgs e)
        {
            Trace.WriteLine("RELOADED");
            if (BindingContext is MainPageViewModel viewModel)
            {
                var collectionView = s as CollectionView;

                if (collectionView != null)
                {
                    collectionView.ItemsSource = null;
                    collectionView.ItemsSource = viewModel.RecognizedPhrases;
                }
            }
        }
    }
}
