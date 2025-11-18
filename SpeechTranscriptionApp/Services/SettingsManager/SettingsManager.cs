using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SpeechTranscriptionApp.Services.SettingsManager
{
    public static class SettingsManagertingsManager
    {
        private const string ConfigFile = "config.json";

        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("SALT_VALUE");

        public static AppSettings? Load()
        {
            if (File.Exists(ConfigFile))
            {
                string json = File.ReadAllText(ConfigFile);

                AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(json);

                if (settings != null)
                {
                    settings.AzureKey = Decrypt(settings.AzureKey);
                    return settings;
                }
            }
            return null;
        }

        public static void Save(AppSettings settings)
        {
            AppSettings updatedSettings = new AppSettings
            {
                AzureKey = Encrypt(settings.AzureKey),
                AzureRegion = settings.AzureRegion
            };
            string json = JsonSerializer.Serialize(updatedSettings, new JsonSerializerOptions { WriteIndented = true });
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

        private static string Encrypt(string inputData)
        {
            if (String.IsNullOrEmpty(inputData)) return String.Empty;

            byte[] encryptedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(inputData), Salt, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData)) return String.Empty;

            try
            {
                byte[] data = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), Salt, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(data);
            }
            catch (CryptographicException)
            {
                return string.Empty;
            }
        }
    }
}
