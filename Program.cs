using System.Runtime;
using ConsoleTranscriptionApp.Configuration;
using ConsoleTranscriptionApp.Services;

namespace ConsoleTranscriptionApp;

enum AppState { Recording, Paused }

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        AppSettings settings = SettingsManager.Load();
        if (settings == null)
        {
            Console.WriteLine("> Settings not detected");
            Console.WriteLine("Type Azure Speech key:");
            string key = Console.ReadLine();

            Console.WriteLine("\nType Azure Speech region:");
            string region = Console.ReadLine();

            settings = new AppSettings
            {
                AzureKey = key,
                AzureRegion = region
            };

            SettingsManager.Save(settings);
        }
        else
        {
            Console.WriteLine("> Settings loaded from config.json");
        }

        string speechLang = string.Empty;
        while (String.IsNullOrEmpty(speechLang))
        {
            Console.WriteLine("\n\t----- AUDIO TRANSCRIBER -----\n");

            Console.WriteLine("\t1. English (US)");
            Console.WriteLine("\t2. German (DE)");
            Console.WriteLine("\t3. Other language");

            Console.WriteLine("\t4. Change key");
            Console.WriteLine("\n\t0. Exit\n");
            Console.WriteLine("> Type option:\n");

            switch (Console.ReadKey().KeyChar)
            {
                case '1':
                    speechLang = "en-US";
                    break;
                case '2':
                    speechLang = "de-DE";
                    break;
                case '3':
                    Console.WriteLine("\n> Type language code:");
                    speechLang = Console.ReadLine();
                    break;
                case '4':
                    Console.WriteLine("\n> Changing keys. Enter \"0\" to cancel");
                    Console.WriteLine("Type new Azure Speech key: ");
                    string key = Console.ReadLine();
                    if (key == "0") break;

                    Console.WriteLine("\nType new Azure Speech region: ");
                    string region = Console.ReadLine();
                    if (region == "0") break;

                    settings = new AppSettings
                    {
                        AzureKey = key,
                        AzureRegion = region
                    };

                    SettingsManager.Save(settings);
                    Console.WriteLine("\n> Settings saved");
                    break;

                case '0':
                    return 0;
            }
        }

        using SpeechTranscriber transcriber = new SpeechTranscriber(speechLang, settings.AzureKey, settings.AzureRegion);
        using AudioRecorder recorder = new AudioRecorder();

        recorder.AudioOutput += (s, a) =>
        {
             transcriber.PushAudio(a.Buffer, a.BytesRecorded);
        };

        AppState state = AppState.Paused;

        Console.WriteLine($"\n > {speechLang} selected. Press [SPACE] to begin and [ENTER] to quit app . . .\n");
        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey pressedKey = Console.ReadKey(true).Key;

                if (state == AppState.Recording && pressedKey == ConsoleKey.Spacebar)
                {
                    state = AppState.Paused;

                    recorder.Stop();
                    await transcriber.StopAsync();

                    Console.WriteLine("\n> Recording stopped. Press [SPACE] to resume . . .\n");
                }
                else if (state == AppState.Paused && pressedKey == ConsoleKey.Spacebar)
                {
                    state = AppState.Recording;

                    recorder.Start();
                    await transcriber.StartAsync();

                    Console.WriteLine("\n> Recording started. Press [SPACE] to stop . . .\n");
                }
                else if (pressedKey == ConsoleKey.Enter)
                {
                    if (state == AppState.Recording)
                    {
                        recorder.Stop();
                        await transcriber.StopAsync();
                    }
                    Console.WriteLine("\n> Exiting app . . .\n");
                    return 0;
                }
            }
            Thread.Sleep(100);
        }
    }
}