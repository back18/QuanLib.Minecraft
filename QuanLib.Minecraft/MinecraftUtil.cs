using QuanLib.Minecraft.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public static class MinecraftUtil
    {
        public static TimeSpan GameTicksToTimeSpan(int ticks)
        {
            return TimeSpan.FromMilliseconds(ticks * 50);
        }

        public static TimeSpan DayTimeToTimeSpan(int time)
        {
            int hour = (time / 1000 + 6) % 24; // 计算小时数
            int minute = (int)(((time % 1000) / 1000.0) * 60); // 计算分钟数
            return new TimeSpan(hour, minute, 0);
        }

        public static MinecraftColor ToReverseColor(MinecraftColor color)
        {
            int colorID = (int)color;
            if (colorID < 8)
                colorID += 8;
            else
                colorID -= 8;
            return (MinecraftColor)colorID;
        }

        public static bool TryEntityPositionSbnt(string s, out Vector3Double result)
        {
            if (string.IsNullOrEmpty(s))
                goto err;

            int index = s.IndexOf('[');
            if (index < 0)
                goto err;

            string[] items = s[index..].Trim('[', ']').Split(", "); ;
            if (items.Length != 3)
                goto err;

            if (!double.TryParse(items[0].TrimEnd('d'), out var x) ||
                !double.TryParse(items[1].TrimEnd('d'), out var y) ||
                !double.TryParse(items[2].TrimEnd('d'), out var z))
                goto err;

            result = new(x, y, z);
            return true;

            err:
            result = default;
            return false;
        }

        public static SurfacePos BlockPos2ChunkPos(SurfacePos blockPos)
        {
            return new SurfacePos
                    ((int)Math.Round(blockPos.X / 16.0, MidpointRounding.ToNegativeInfinity),
                    (int)Math.Round(blockPos.Z / 16.0, MidpointRounding.ToNegativeInfinity));
        }

        public static SurfacePos ChunkPos2BlockPos(SurfacePos chunkPos)
        {
            return new(chunkPos.X * 16, chunkPos.Z * 16);
        }
    }
}
