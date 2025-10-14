using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ConsoleTranscriptionApp.Configuration
{
    internal class SettingsManager
    {
        private const string ConfigFile = "config.json";

        private static readonly byte[] salt = Encoding.UTF8.GetBytes("SALT_VALUE");

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

#pragma warning disable CA1416 // TODO: Add cross-platform support
            byte[] encryptedData = ProtectedData.Protect(Encoding.UTF8.GetBytes(inputData), salt, DataProtectionScope.CurrentUser);
#pragma warning restore CA1416 //

            return Convert.ToBase64String(encryptedData);
        }
        public static string Decrypt(string encryptedData)
        {
            if (string.IsNullOrEmpty(encryptedData)) return String.Empty;

            try
            {
#pragma warning disable CA1416 // TODO: Add cross-platform support
                byte[] data = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), salt, DataProtectionScope.CurrentUser);
#pragma warning restore CA1416
                return Encoding.UTF8.GetString(data);
            }
            catch (CryptographicException)
            {
                return string.Empty;
            }
        }
    }
}
