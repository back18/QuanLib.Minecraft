using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class VersionJson
    {
        public VersionJson(JObject jobj)
        {
            ArgumentNullException.ThrowIfNull(jobj, nameof(jobj));

            _jobj = jobj;
        }

        private readonly JObject _jobj;

        public NetworkAssetIndex? GetIndexFile()
        {
            if (_jobj["assetIndex"] is not JObject jobj)
                return null;

            var model = jobj.ToObject<NetworkAssetIndex.Model>();
            if (model is null)
                return null;

            return new(model);
        }

        public NetworkAssetIndex? GetClientCore()
        {
            if (_jobj["downloads"]?["client"] is not JObject jobj)
                return null;

            var model = jobj.ToObject<NetworkAssetIndex.Model>();
            if (model is null)
                return null;

            return new(model);
        }

        public NetworkAssetIndex? GetServerCore()
        {
            if (_jobj["downloads"]?["server"] is not JObject jobj)
                return null;

            var model = jobj.ToObject<NetworkAssetIndex.Model>();
            if (model is null)
                return null;

            return new(model);
        }
    }
}
