using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft
{
    public interface IPlayerInfo
    {
        public string Name { get; }

        public Guid Uuid { get; }
    }
}
