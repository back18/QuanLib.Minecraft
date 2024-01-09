using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public interface IDownloadAssetIndex : INetworkAssetIndex
    {
        public string Path { get; }
    }
}
