using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Downloading
{
    public class LocalAssetIndex(KeyValuePair<string, AssetIndex.Model> keyValuePair) : AssetIndex(keyValuePair.Value)
    {
        public string Path { get; } = keyValuePair.Key;
    }
}
