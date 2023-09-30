using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Blocks
{
    public static class BlockManager
    {
        static BlockManager()
        {
            Concrete = new();
            ConcretePowder = new();
            Terracotta = new();
            GlazedTerracotta = new();
            Wool = new();
            StainedGlass = new();
        }

        public static ConcreteBlock Concrete { get; }

        public static ConcretePowderBlock ConcretePowder { get; }

        public static TerracottaBlock Terracotta { get; }

        public static GlazedTerracottaBlock GlazedTerracotta { get; }

        public static WoolBlock Wool { get; }

        public static StainedGlassBlock StainedGlass { get; }
    }
}
