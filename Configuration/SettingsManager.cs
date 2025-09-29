using System.Text.Json;

namespace ConsoleTranscriptionApp.Configuration
{
    internal class SettingsManager
    {
        private const string ConfigFile = "config.json";

        public static AppSettings? Load()
        {
            if (File.Exists(ConfigFile))
            {
                string json = File.ReadAllText(ConfigFile);
                return JsonSerializer.Deserialize<AppSettings>(json);
            }
            return null;
        }

        public static void Save(AppSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFile, json);
        }

        public static AppSettings? Create(string? key, string? region)
        {
            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(region)) return null;

            return new AppSettings
            {
                AzureKey = key,
                AzureRegion = region
            };
        }
    }
}
