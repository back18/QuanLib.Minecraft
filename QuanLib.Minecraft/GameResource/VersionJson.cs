using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public class VersionJson
    {
        public VersionJson(JObject jobj)
        {
            _jobj = jobj ?? throw new ArgumentNullException(nameof(jobj));
        }

        private readonly JObject _jobj;

        public static async Task<VersionJson> DownloadAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException($"“{nameof(url)}”不能为 null 或空。", nameof(url));

            byte[] bytes = await DownloadUtil.DownloadBytesAsync(url);
            string text = Encoding.UTF8.GetString(bytes);
            JObject jobj = JObject.Parse(text);
            return new(jobj);
        }

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
