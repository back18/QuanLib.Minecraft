using static QuanLib.Minecraft.BlockScreen.Config.ConfigManager;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreRCON.Parsers.Standard;
using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.Selectors;
using System.Numerics;
using QuanLib.Minecraft.Data;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public class ScreenBuildContext
    {
        public ScreenBuildContext(string player)
        {
            if (string.IsNullOrEmpty(player))
                throw new ArgumentException($"“{nameof(player)}”不能为 null 或空。", nameof(player));

            Player = player;
            BuildState = ScreenBuildState.ReadStartPosition;
            Timeout = ScreenConfig.ScreenBuildTimeout;
            Error = false;
        }

        public string Player { get; }

        public Screen? Screen { get; private set; }

        public ScreenBuildState BuildState { get; private set; }

        public Facing NormalFacing { get; private set; }

        public int PlaneCoordinate { get; private set; }

        public Vector3<int> StartPosition { get; private set; }

        public Vector3<int> EndPosition { get; private set; }

        public int Timeout { get; private set; }

        public bool Error { get; private set; }

        public void Handle()
        {
            ServerCommandHelper command = MCOS.Instance.MinecraftServer.CommandHelper;

            if (BuildState == ScreenBuildState.Timedout || BuildState == ScreenBuildState.Canceled || BuildState == ScreenBuildState.Completed)
            {
                return;
            }

            if (ScreenConfig.ScreenBuildTimeout != -1 && Timeout <= 0)
            {
                BuildState = ScreenBuildState.Timedout;
                return;
            }

            if (!command.TryGetEntityPosition(Player, out var position) ||
                !command.TryGetEntityRotation(Player, out var rotation))
            {
                BuildState = ScreenBuildState.Canceled;
                return;
            }

            if (command.TryGetPlayerSelectedItem(Player, out var item) &&
                item.Tag is not null &&
                item.Tag.TryGetValue("display", out var display) &&
                display is Dictionary<string, object> displayTag &&
                displayTag.TryGetValue("Name", out var name) &&
                name is string nameString)
            {
                JObject nameJson;
                try
                {
                    nameJson = JObject.Parse(nameString);
                }
                catch
                {
                    TryCancel();
                    return;
                }

                string? text = nameJson["text"]?.Value<string>();
                if (text is null || text != ScreenConfig.ScreenBuildItemName)
                {
                    TryCancel();
                    return;
                }

                position.Y += 1.625;
                Vector3<int> targetPosition;
                if (BuildState == ScreenBuildState.ReadStartPosition)
                {
                    int distance = 1;
                    if (command.TryGetPlayerDualWieldItem(Player, out var dualWieldItem))
                        distance += dualWieldItem.Count * 4;

                    Facing playerFacing;
                    if (rotation.Pitch <= -60 || rotation.Pitch >= 60)
                        playerFacing = rotation.PitchFacing;
                    else
                        playerFacing = rotation.YawFacing;

                    var target = playerFacing switch
                    {
                        Facing.Yp => position.Y + distance,
                        Facing.Ym => position.Y - distance,
                        Facing.Xp => position.X + distance,
                        Facing.Xm => position.X - distance,
                        Facing.Zp => position.Z + distance,
                        Facing.Zm => position.Z - distance,
                        _ => throw new InvalidOperationException(),
                    };

                    NormalFacing = MinecraftUtil.ToReverseFacing(playerFacing);
                    PlaneCoordinate = (int)Math.Round(target, MidpointRounding.ToNegativeInfinity);
                    targetPosition = Vector3Double.GetToPlaneIntersection(position, rotation.ToDirection(), NormalFacing, PlaneCoordinate).ToVector3Int();

                    if (targetPosition.Y < ScreenConfig.MinY || targetPosition.Y > ScreenConfig.MaxY)
                    {
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] 错误：目标位置的Y轴需要在{ScreenConfig.MinY}至{ScreenConfig.MaxY}之间", TextColor.Red);
                        Error = true;
                        goto click;
                    }

                    command.SendTitle(new PlayerSelector(Player), 0, 10, 10, "正在创建屏幕");
                    command.SendSubTitle(new PlayerSelector(Player), 0, 10, 10, $"方向:{MinecraftUtil.FacingToChineseString(playerFacing)} 距离:{distance} 目标位置:{targetPosition}");
                }
                else if (BuildState == ScreenBuildState.ReadEndPosition)
                {
                    targetPosition = Vector3Double.GetToPlaneIntersection(position, rotation.ToDirection(), NormalFacing, PlaneCoordinate).ToVector3Int();

                    if (targetPosition.Y < ScreenConfig.MinY || targetPosition.Y > ScreenConfig.MaxY)
                    {
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] 错误：目标位置的Y轴需要在{ScreenConfig.MinY}至{ScreenConfig.MaxY}之间", TextColor.Red);
                        Error = true;
                        goto click;
                    }

                    Screen? newScreen = Screen;
                    if (EndPosition != targetPosition)
                    {
                        newScreen = Screen.CreateScreen(StartPosition, targetPosition, NormalFacing);
                    }

                    if (newScreen is null)
                        goto click;

                    if (Screen.Replace(Screen, newScreen, true))
                    {
                        if (EndPosition != targetPosition)
                        {
                            Screen = newScreen;
                            EndPosition = targetPosition;
                        }
                    }
                    else
                    {
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] 错误：所选范围内含有非空气方块", TextColor.Red);
                        Error = true;
                        goto click;
                    }

                    if (newScreen.Width > ScreenConfig.MaxLength || newScreen.Height > ScreenConfig.MaxLength)
                    {
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] 错误：屏幕最大长度为{ScreenConfig.MaxLength}", TextColor.Red);
                        Error = true;
                        goto click;
                    }
                    else if (newScreen.TotalPixels > ScreenConfig.MaxPixels)
                    {
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] 错误：屏幕最大像素数量为{ScreenConfig.MaxLength}", TextColor.Red);
                        Error = true;
                        goto click;
                    }
                    else if (newScreen.Width < ScreenConfig.MinLength || newScreen.Height < ScreenConfig.MinLength)
                    {
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] 错误：屏幕最小长度为{ScreenConfig.MinLength}", TextColor.Red);
                        Error = true;
                    }
                    else if (newScreen.TotalPixels < ScreenConfig.MinPixels)
                    {
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] 错误：屏幕最小像素数量为{ScreenConfig.MinPixels}", TextColor.Red);
                        Error = true;
                    }

                    command.SendTitle(new PlayerSelector(Player), 0, 10, 10, "正在确定屏幕尺寸");
                    command.SendSubTitle(new PlayerSelector(Player), 0, 10, 10, $"宽度:{Screen?.Width ?? 0} 高度:{Screen?.Height ?? 0} 像素数量: {Screen?.TotalPixels ?? 0}");
                }
                else
                {
                    throw new InvalidOperationException();
                }

                click:
                if (command.TryGetPlayerScoreboard(Player, ScreenConfig.RightClickObjective, out var score) && score > 0)
                {
                    command.SetPlayerScoreboard(Player, ScreenConfig.RightClickObjective, 0);

                    if (Error)
                    {
                        command.SendChatMessage(new PlayerSelector(Player), "[屏幕构建器] 出现一个或多个错误，无法创建屏幕", TextColor.Red);
                        return;
                    }

                    if (BuildState == ScreenBuildState.ReadStartPosition)
                    {
                        StartPosition = targetPosition;
                        BuildState = ScreenBuildState.ReadEndPosition;
                        command.SendChatMessage(new PlayerSelector(Player), $"[屏幕构建器] 屏幕左上角已确定，位于{StartPosition}");
                    }
                    else if (BuildState == ScreenBuildState.ReadEndPosition && Screen is not null)
                    {
                        command.SendChatMessage(new PlayerSelector(Player), $"[屏幕构建器] 屏幕右下角已确定，位于{EndPosition}");
                        BuildState = ScreenBuildState.Completed;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            else
            {
                TryCancel();
                return;
            }

            Timeout = ScreenConfig.ScreenBuildTimeout;
            Error = false;

            void TryCancel()
            {
                if (BuildState == ScreenBuildState.ReadEndPosition)
                {
                    if (ScreenConfig.ScreenBuildTimeout == -1)
                    {
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] 你有一个屏幕未完成创建， 位于{StartPosition}", TextColor.Red);
                    }
                    else
                    {
                        Timeout--;
                        command.SendActionbarTitle(new PlayerSelector(Player), $"[屏幕构建器] {Timeout / 20}秒后无操作将取消本次屏幕创建", TextColor.Red);
                    }
                }
                else
                {
                    BuildState = ScreenBuildState.Canceled;
                }
            }
        }
    }
}
