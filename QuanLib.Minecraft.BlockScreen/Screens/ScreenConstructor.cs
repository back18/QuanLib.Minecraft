using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.Data;
using QuanLib.Minecraft.Selectors;
using QuanLib.Minecraft.Vector;
using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public class ScreenConstructor
    {
        public ScreenConstructor()
        {
            State = ScreenConstructorState.ReadStartPosition;
            StartPosition = new(0, 0, 0);
            EndPosition = new(0, 0, 0);

            _objective = ConfigManager.ScreenConfig.RightClickObjective;
            _screen = null;
        }

        private const string MOUSE_ITEM = "minecraft:snowball";

        private readonly string _objective;

        private Screen? _screen;

        public Facing NormalFacing { get; private set; }

        public int PlaneCoordinate { get; private set; }

        public Vector3<int> StartPosition { get; private set; }

        public Vector3<int> EndPosition { get; private set; }

        public ScreenConstructorState State { get; private set; }

        public void Handle()
        {
            ServerCommandHelper command = MCOS.Instance.MinecraftServer.CommandHelper;

            Dictionary<string, Item> players = command.GetAllPlayerSelectedItem();
            if (players.Count == 0)
                return;

            foreach (var player in players)
                HandlePlayer(player.Key, player.Value);
        }

        private void HandlePlayer(string player, Item item)
        {
            if (item.ID != MOUSE_ITEM)
                return;

            if (item.Tag is not null &&
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
                    return;
                }
                string? text = nameJson["text"]?.Value<string>();
                if (text is null || text != "创建屏幕")
                    return;

                ServerCommandHelper command = MCOS.Instance.MinecraftServer.CommandHelper;

                if (!command.TryGetEntityPosition(player, out var position) || !command.TryGetEntityRotation(player, out var rotation))
                    return;

                position.Y += 1.625;
                Vector3<int> targetPosition = new(0, 0, 0);
                if (State == ScreenConstructorState.ReadStartPosition)
                {
                    int distance = 1;
                    if (command.TryGetPlayerDualWieldItem(player, out var dualWieldItem))
                        distance = distance += dualWieldItem.Count * 4;

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

                    command.SendTitle(new PlayerSelector(player), 0, 10, 10, "你正在创建屏幕");
                    command.SendSubTitle(new PlayerSelector(player), 0, 10, 10, $"方向:{MinecraftUtil.FacingToChineseString(playerFacing)} 距离:{distance} 目标位置:{targetPosition}");
                }
                else if (State == ScreenConstructorState.ReadEndPosition)
                {
                    targetPosition = Vector3Double.GetToPlaneIntersection(position, rotation.ToDirection(), NormalFacing, PlaneCoordinate).ToVector3Int();
                    if (targetPosition.Y < -64 || targetPosition.Y > 319)
                    {
                        command.SendActionbarTitle(new PlayerSelector(player), "[屏幕创建器] 失败：目标位置的Y轴需要在-64至319之间");
                        return;
                    }

                    if (targetPosition != EndPosition)
                    {
                        EndPosition = targetPosition;
                        Screen? temp = _screen;
                        _screen = Screen.CreateScreen(StartPosition, EndPosition);
                        if (_screen.Width > 256 || _screen.Height > 256)
                        {
                            command.SendActionbarTitle(new PlayerSelector(player), "[屏幕创建器] 失败：超过屏幕尺寸上限256×256");
                            return;
                        }

                        Screen.Replace(temp, _screen);
                    }

                    command.SendTitle(new PlayerSelector(player), 0, 10, 10, "正在确定屏幕尺寸");
                    command.SendSubTitle(new PlayerSelector(player), 0, 10, 10, $"宽度:{_screen?.Width ?? 0} 高度:{_screen?.Height ?? 0}");
                }
                else
                {
                    throw new InvalidOperationException();
                }

                if (command.TryGetPlayerScoreboard(player, _objective, out var score) && score > 0)
                {
                    switch (State)
                    {
                        case ScreenConstructorState.ReadStartPosition:
                            if (targetPosition.Y < -64 || targetPosition.Y > 319)
                            {
                                command.SendActionbarTitle(new PlayerSelector(player), "[屏幕创建器] 失败：目标位置的Y轴需要在-64至319之间");
                                break;
                            }
                            StartPosition = targetPosition;
                            State = ScreenConstructorState.ReadEndPosition;
                            break;
                        case ScreenConstructorState.ReadEndPosition:
                            State = ScreenConstructorState.ReadStartPosition;
                            if (_screen is not null)
                            {
                                MCOS.Instance.ScreenManager.ScreenList.Add(_screen);
                                _screen = null;
                            }
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    command.SetPlayerScoreboard(player, _objective, 0);
                }
            }
        }

        public enum ScreenConstructorState
        {
            ReadStartPosition,

            ReadEndPosition,
        }
    }
}
