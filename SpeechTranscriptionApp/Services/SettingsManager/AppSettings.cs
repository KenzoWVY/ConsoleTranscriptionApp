using System.Text.Json.Serialization;

namespace SpeechTranscriptionApp.Services.SettingsManager
{
    public class AppSettings
    {
        [JsonPropertyName("AzureKey")]
        public required string AzureKey { get; set; }

        [JsonPropertyName("AzureRegion")]
        public required string AzureRegion { get; set; }
    }
}
