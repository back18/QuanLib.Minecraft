using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.Datas;
using QuanLib.Minecraft.Vectors;
using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class ScreenConstructor : IMCOSComponent
    {
        public ScreenConstructor(string objective)
        {
            if (string.IsNullOrEmpty(objective))
                throw new ArgumentException($"“{nameof(objective)}”不能为 null 或空。", nameof(objective));

            State = ScreenConstructorState.ReadStartPosition;
            StartPosition = new(0, 0, 0);
            EndPosition = new(0, 0, 0);

            _objective = objective;
            _screen = null;
        }

        private const string MOUSE_ITEM = "minecraft:snowball";

        private readonly string _objective;

        private Screen? _screen;

        public MCOS MCOS
        {
            get
            {
                if (_MCOS is null)
                    throw new InvalidOperationException();
                return _MCOS;
            }
            internal set => _MCOS = value;
        }
        private MCOS? _MCOS;

        public Facing NormalFacing { get; private set; }

        public int PlaneCoordinate { get; private set; }

        public Vector3<int> StartPosition { get; private set; }

        public Vector3<int> EndPosition { get; private set; }

        public ScreenConstructorState State { get; private set; }

        public void Handle()
        {
            ServerCommandHelper command = MCOS.MinecraftServer.CommandHelper;

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

                ServerCommandHelper command = MCOS.MinecraftServer.CommandHelper;

                if (!command.TryGetEntityPosition(player, out var position) || !command.TryGetEntityRotation(player, out var rotation))
                    return;

                position.Y += 1.625;
                if (State == ScreenConstructorState.ReadStartPosition)
                {
                    int distance = 1;
                    if (command.TryGetPlayerDualWieldItem(player, out var dualWieldItem))
                        distance = distance += dualWieldItem.Count * 4;

                    Facing facing;
                    if (rotation.Pitch <= -60 || rotation.Pitch >= 60)
                        facing = rotation.PitchFacing;
                    else
                        facing = rotation.YawFacing;

                    var target = facing switch
                    {
                        Facing.Yp => position.Y + distance,
                        Facing.Ym => position.Y - distance,
                        Facing.Xp => position.X + distance,
                        Facing.Xm => position.X - distance,
                        Facing.Zp => position.Z + distance,
                        Facing.Zm => position.Z - distance,
                        _ => throw new InvalidOperationException(),
                    };

                    NormalFacing = MinecraftUtil.ToReverseFacing(facing);
                    PlaneCoordinate = (int)Math.Round(target, MidpointRounding.ToNegativeInfinity);
                }

                Vector3<int> targetBlock = Vector3Double.GetToPlaneIntersection(position, rotation.ToDirection(), NormalFacing, PlaneCoordinate).ToVector3Int();

                if (State == ScreenConstructorState.ReadEndPosition)
                {
                    if (targetBlock != EndPosition)
                    {
                        EndPosition = targetBlock;
                        _screen?.Clear();
                        _screen = Screen.CreateScreen(StartPosition, EndPosition);
                        _screen.MCOS = MCOS;
                        if (_screen.Width <= 256 && _screen.Height <= 256)
                        {
                            _screen.Fill();
                        }
                    }
                }

                if (command.TryGetPlayerScoreboard(player, _objective, out var score) && score > 0)
                {
                    switch (State)
                    {
                        case ScreenConstructorState.ReadStartPosition:
                            StartPosition = targetBlock;
                            State = ScreenConstructorState.ReadEndPosition;
                            break;
                        case ScreenConstructorState.ReadEndPosition:
                            State = ScreenConstructorState.ReadStartPosition;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    command.SetPlayerScoreboard(player, _objective, 0);
                }
            }
        }
    }
}
