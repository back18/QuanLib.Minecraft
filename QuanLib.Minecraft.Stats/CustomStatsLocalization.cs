using Newtonsoft.Json;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace QuanLib.Minecraft.Stats
{
    public class CustomStatsLocalization : ISingleton<CustomStatsLocalization>, ISingletonFactory<CustomStatsLocalization>
    {
        private CustomStatsLocalization(
            ReadOnlyDictionary<string, LocalizationInfo> english,
            ReadOnlyDictionary<string, LocalizationInfo> chinese)
        {
            ArgumentNullException.ThrowIfNull(english, nameof(english));
            ArgumentNullException.ThrowIfNull(chinese, nameof(chinese));

            English = english;
            Chinese = chinese;
        }

        private static readonly Lock _slock = new();

        public static bool IsLoaded { get; private set; }

        public static CustomStatsLocalization Instance
        {
            get => field ?? throw new InvalidOperationException("Instance not loaded, please call LoadInstance() method to load the instance first");
            private set => field = value;
        }

        public ReadOnlyDictionary<string, LocalizationInfo> English { get; }

        public ReadOnlyDictionary<string, LocalizationInfo> Chinese { get; }

        public static CustomStatsLocalization LoadInstance()
        {
            lock (_slock)
            {
                if (IsLoaded)
                    throw new InvalidOperationException("Instance already loaded, please do not call LoadInstance() method again");

                ReadOnlyDictionary<string, LocalizationInfo> english = LoadLanguageFile("en_us");
                ReadOnlyDictionary<string, LocalizationInfo> chinese = LoadLanguageFile("zh_cn");

                IsLoaded = true;
                Instance = new CustomStatsLocalization(english, chinese);

                return Instance;
            }
        }

        private static ReadOnlyDictionary<string, LocalizationInfo> LoadLanguageFile(string language)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.SystemResource.custom_stats_{language}.json") ?? throw new InvalidOperationException();
            using StreamReader streamReader = new(stream, Encoding.UTF8);
            string json = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<Dictionary<string, LocalizationInfo>>(json)?.AsReadOnly()
                ?? ReadOnlyDictionary<string, LocalizationInfo>.Empty;
        }
    }
}
