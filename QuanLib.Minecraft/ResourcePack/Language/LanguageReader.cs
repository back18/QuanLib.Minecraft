using Nett;
using Newtonsoft.Json;
using QuanLib.Core.Extension;
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

        public static LanguageManager Load(ResourceEntryManager resources, string language)     //TODO 原版语言文件加载
        {
            if (resources is null)
                throw new ArgumentNullException(nameof(resources));
            if (string.IsNullOrEmpty(language))
                throw new ArgumentException($"“{nameof(language)}”不能为 null 或空。", nameof(language));

            ConcurrentDictionary<string, TemplateText> result = new();
            int total = 0;
            int count = 0;
            foreach (var resource in resources.Values)
            {
                if (!resource.Languages.TryGetValue(DEFAULT_LANGUAGE + ".json", out var defaultEntry))
                    continue;

                Dictionary<string, string>? defaultDictionary = TryParseJson(defaultEntry);
                if (defaultDictionary is null)
                    continue;

                if (language != DEFAULT_LANGUAGE && resource.Languages.TryGetValue(language + ".json", out var languageEntry))
                {
                    Dictionary<string, string>? languageDictionary = TryParseJson(languageEntry);
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
                    if (TemplateText.TryParseLanguage(item.Key, item.Value, out var text))
                        result.TryAdd(text.Key, text);
                    Interlocked.Increment(ref count);
                });
            }

            while (count < total)
                Thread.Sleep(10);

            return new(result.ToDictionary(kv => kv.Key, kv => kv.Value), language);
        }

        private static Dictionary<string, string>? TryParseJson(ZipArchiveEntry entry)
        {
            if (entry is null)
                throw new ArgumentNullException(nameof(entry));

            using Stream stream = entry.Open();
            string text = stream.ToUtf8Text();
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
            }
            catch
            {
                return null;
            }
        }
    }
}
