using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Downloading
{
    public abstract class VersionJsonBase
    {
        protected VersionJsonBase(JObject jobj)
        {
            ArgumentNullException.ThrowIfNull(jobj, nameof(jobj));

            JObject = jobj;
        }

        protected readonly JObject JObject;

        public string Id => JObject["id"]?.Value<string>() ?? string.Empty;

        public string Type => JObject["type"]?.Value<string>() ?? string.Empty;

        public string Time => JObject["time"]?.Value<string>() ?? string.Empty;

        public string ReleaseTime => JObject["releaseTime"]?.Value<string>() ?? string.Empty;

        public string MainClass => JObject["mainClass"]?.Value<string>() ?? string.Empty;

        public int MinimumLauncherVersion => JObject["minimumLauncherVersion"]?.Value<int>() ?? 0;

        public bool Root => JObject["root"]?.Value<bool>() ?? false;

        public virtual NetworkAssetIndex? GetIndexFile()
        {
            if (JObject["assetIndex"] is not JObject jobj)
                return null;

            var model = jobj.ToObject<NetworkAssetIndex.Model>();
            if (model is null)
                return null;

            return new(model);
        }
    }
}
