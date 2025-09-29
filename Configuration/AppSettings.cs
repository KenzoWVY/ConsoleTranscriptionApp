using System.Text.Json;

namespace ConsoleTranscriptionApp.Configuration;

public class AppSettings
{
    public required string AzureKey { get; set; }
    public required string AzureRegion { get; set; }

}
