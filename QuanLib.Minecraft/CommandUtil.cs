using QuanLib.Minecraft.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public static class CommandUtil
    {
        public static string ToCommandArgument(this Gamemode gamemode)
        {
            return gamemode switch
            {
                Gamemode.Survival => "survival",
                Gamemode.Creative => "creative",
                Gamemode.Spectator => "spectator",
                Gamemode.Adventure => "adventure",
                _ => throw new InvalidOperationException(),
            };
        }

        public static string ToCommandArgument(this Sort sort)
        {
            return sort switch
            {
                Sort.Nearest => "nearest",
                Sort.Furthest => "furthest",
                Sort.Arbitrary => "arbitrary",
                Sort.Random => "random",
                _ => throw new InvalidOperationException(),
            };
        }

        public static string ToCommandArgument(this Target target)
        {
            return target switch
            {
                Target.AllPlayers => "@a",
                Target.AllEntities => "@e",
                Target.RandomPlayer => "@r",
                Target.NearestPlayer => "@p",
                Target.CommandExecutor => "@s",
                _ => throw new InvalidOperationException(),
            };
        }

        public static string ToCommandArgument(this TextColor color)
        {
            return color switch
            {
                TextColor.Black => "black",
                TextColor.DarkBlue => "dark_blue",
                TextColor.DarkGreen => "dark_green",
                TextColor.DarkAqua => "dark_aqua",
                TextColor.DarkRed => "dark_red",
                TextColor.Purple => "dark_purple",
                TextColor.Gold => "gold",
                TextColor.Gray => "gray",
                TextColor.DarkGray => "dark_gray",
                TextColor.Blue => "blue",
                TextColor.Green => "green",
                TextColor.Aqua => "aqua",
                TextColor.Red => "red",
                TextColor.LightPurple => "light_purple",
                TextColor.Yellow => "yellow",
                TextColor.White => "white",
                _ => throw new InvalidOperationException(),
            };
        }

        public static string ToCommandArgument(this MinecraftColor color)
        {
            switch (color)
            {
                case MinecraftColor.White:
                    return "white";
                case MinecraftColor.Orange:
                    return "orange";
                case MinecraftColor.Magenta:
                    return "magenta";
                case MinecraftColor.LightBlue:
                    return "light_blue";
                case MinecraftColor.Yellow:
                    return "yellow";
                case MinecraftColor.Lime:
                    return "lime";
                case MinecraftColor.Pink:
                    return "pink";
                case MinecraftColor.Gray:
                    return "gray";
                case MinecraftColor.LightGray:
                    return "light_gray";
                case MinecraftColor.Cyan:
                    return "cyan";
                case MinecraftColor.Purple:
                    return "purple";
                case MinecraftColor.Blue:
                    return "blue";
                case MinecraftColor.Brown:
                    return "brown";
                case MinecraftColor.Green:
                    return "green";
                case MinecraftColor.Red:
                    return "red";
                case MinecraftColor.Black:
                    return "black";
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string ToJson(string text, TextColor color, bool bold)
        {
            string boldStr = bold ? "true" : "false";
            return $"{{\"text\":\"{text}\",\"color\":\"{color.ToCommandArgument()}\",\"bold\":\"{boldStr}\"}}";
        }
    }
}
