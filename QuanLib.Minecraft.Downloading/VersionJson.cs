using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class VersionJson(JObject jobj) : VersionJsonBase(jobj)
    {
        public string Jar => JObject["jar"]?.Value<string>() ?? string.Empty;

        public NetworkAssetIndex? GetClientCore()
        {
            if (JObject["downloads"]?["client"] is not JObject jobj)
                return null;

            var model = jobj.ToObject<NetworkAssetIndex.Model>();
            if (model is null)
                return null;

            return new(model);
        }

        public NetworkAssetIndex? GetServerCore()
        {
            if (JObject["downloads"]?["server"] is not JObject jobj)
                return null;

            var model = jobj.ToObject<NetworkAssetIndex.Model>();
            if (model is null)
                return null;

            return new(model);
        }

        public VersionJsonPatch[] GetPatches()
        {
            if (JObject["patches"] is not JArray jarr)
                return Array.Empty<VersionJsonPatch>();

            List<VersionJsonPatch> patches = [];
            foreach (JToken item in jarr)
            {
                if (item is not JObject jobj)
                    continue;

                VersionJsonPatch patch = new(jobj);
                patches.Add(patch);
            }

            return patches.ToArray();
        }

        public VersionJsonPatch? GetGamePatch()
        {
            if (JObject["patches"] is not JArray jarr)
                return null;

            foreach (JToken item in jarr)
            {
                if (item is not JObject jobj)
                    continue;

                if (jobj["id"]?.Value<string>() != "game")
                    continue;

                return new VersionJsonPatch(jobj);
            }

            return null;
        }
    }
}
