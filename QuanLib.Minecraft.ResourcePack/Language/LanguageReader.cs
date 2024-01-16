using Newtonsoft.Json;
using QuanLib.Core.Extensions;
using QuanLib.IO.Zip;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Language
{
    public static class LanguageReader
    {
        public const string DEFAULT_LANGUAGE = "en_us";

        public static LanguageManager Load(ResourceEntryManager resources, string language)
        {
            ArgumentNullException.ThrowIfNull(resources, nameof(resources));
            ArgumentException.ThrowIfNullOrEmpty(language, nameof(language));

            ConcurrentDictionary<string, TextTemplate> result = new();
            int total = 0;
            int count = 0;
            foreach (var resource in resources.Values)
            {
                if (!resource.Languages.TryGetValue(DEFAULT_LANGUAGE + ".json", out var defaultItem))
                    continue;

                Dictionary<string, string>? defaultDictionary = TryParseJson(defaultItem);
                if (defaultDictionary is null)
                    continue;

                if (language != DEFAULT_LANGUAGE && resource.Languages.TryGetValue(language + ".json", out var languageItem))
                {
                    Dictionary<string, string>? languageDictionary = TryParseJson(languageItem);
                    if (languageDictionary is not null)
                    {
                        foreach (var item in languageDictionary)
                            defaultDictionary[item.Key] = item.Value;
                    }
                }
                else
                {
                    language = DEFAULT_LANGUAGE;
                }

                total += defaultDictionary.Count;
                Parallel.ForEach(defaultDictionary, item =>
                {
                    if (TextTemplate.TryParse(item.Value, out var template))
                        result.TryAdd(item.Key, template);
                    Interlocked.Increment(ref count);
                });
            }

            while (count < total)
                Thread.Sleep(10);

            return new(result.ToDictionary(kv => kv.Key, kv => kv.Value), language);
        }

        private static Dictionary<string, string>? TryParseJson(ZipItem zipItem)
        {
            ArgumentNullException.ThrowIfNull(zipItem, nameof(zipItem));

            try
            {
                string text = zipItem.ReadAllText();
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            }
            catch
            {
                return null;
            }
        }

        private static Dictionary<string, string>? TryParseJson(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
            }
            catch
            {
                return null;
            }
        }
    }
}
