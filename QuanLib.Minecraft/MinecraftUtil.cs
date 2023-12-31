﻿using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.Snbt;
using QuanLib.Minecraft.Snbt.Models;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public static class MinecraftUtil
    {
        public static MinecraftColor Reverse(this MinecraftColor color)
        {
            int colorID = (int)color;
            if (colorID < 8)
                colorID += 8;
            else
                colorID -= 8;
            return (MinecraftColor)colorID;
        }

        public static string ToColorCode(this TextColor color)
        {
            return color switch
            {
                TextColor.Black => "§0",
                TextColor.DarkBlue => "§1",
                TextColor.DarkGreen => "§2",
                TextColor.DarkAqua => "§3",
                TextColor.DarkRed => "§4",
                TextColor.Purple => "§5",
                TextColor.Gold => "§6",
                TextColor.Gray => "§7",
                TextColor.DarkGray => "§8",
                TextColor.Blue => "§9",
                TextColor.Green => "§a",
                TextColor.Aqua => "§b",
                TextColor.Red => "§c",
                TextColor.LightPurple => "§d",
                TextColor.Yellow => "§e",
                TextColor.White => "§f",
                _ => throw new InvalidOperationException(),
            };
        }

        public static ChunkPos BlockPos2ChunkPos(IVector3<int> blockPos)
        {
            return new ChunkPos
                    ((int)Math.Round(blockPos.X / 16.0, MidpointRounding.ToNegativeInfinity),
                    (int)Math.Round(blockPos.Z / 16.0, MidpointRounding.ToNegativeInfinity));
        }

        public static BlockPos ChunkPos2BlockPos(ChunkPos chunkPos)
        {
            return new(chunkPos.X * 16, 0, chunkPos.Z * 16);
        }

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

        public static bool TryParseEntityPosSbnt(string s, out EntityPos result)
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

        public static bool TryParseRotationSbnt(string s, out Rotation result)
        {
            if (string.IsNullOrEmpty(s))
                goto err;

            int index = s.IndexOf('[');
            if (index < 0)
                goto err;

            string[] items = s[index..].Trim('[', ']').Split(", "); ;
            if (items.Length != 2)
                goto err;

            if (!float.TryParse(items[0].TrimEnd('f'), out var yaw) ||
                !float.TryParse(items[1].TrimEnd('f'), out var pitch))
                goto err;

            result = new(yaw, pitch);
            return true;

            err:
            result = default;
            return false;
        }

        public static bool TryParseUuidSbnt(string s, out Guid result)
        {
            MatchCollection matches = Regex.Matches(s, @"-?\d+");

            List<int> items = new();
            foreach (Match match in matches.Cast<Match>())
            {
                if (!int.TryParse(match.Value, out var item))
                    goto err;

                items.Add(item);
            }

            if (items.Count != 4)
                goto err;

            result = SnbtSerializer.ToGuid(items.ToArray());
            return true;

            err:
            result = default;
            return false;
        }

        public static bool TryParseBlockState(string s, [MaybeNullWhen(false)] out Dictionary<string, string> result)
        {
            if (string.IsNullOrEmpty(s))
                goto err;

            Dictionary<string, string> items = new();
            string[] states = s.Split(',');
            foreach (string state in states)
            {
                string[] kv = state.Split('=');
                if (kv.Length != 2)
                    goto err;

                items.Add(kv[0], kv[1]);
            }

            result = items;
            return true;

            err:
            result = null;
            return false;
        }

        public static string GetItemName(Item item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            if (item.Tag is not null &&
                item.Tag.TryGetValue("display", out var display) &&
                display is Dictionary<string, object> displayTag &&
                displayTag.TryGetValue("Name", out var name) &&
                name is string nameString)
            {
                try
                {
                    JObject nameJson = JObject.Parse(nameString);
                    if (nameJson.TryGetValue("text", out var text) && text is JValue textValue && textValue.Value is string textString)
                    {
                        return textString;
                    }
                }
                catch
                {

                }
            }

            return item.ID;
        }
    }
}
