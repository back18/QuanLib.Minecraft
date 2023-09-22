using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public interface IDownloadAssetIndex : INetworkAssetIndex
    {
        public string Path { get; }
    }
}
