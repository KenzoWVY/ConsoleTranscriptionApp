using SpeechTranscriptionApp.Views;

namespace SpeechTranscriptionApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ConfigsPage), typeof(ConfigsPage));
        }
    }
}
