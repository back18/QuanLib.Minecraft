using CoreRCON;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.Vectors;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    /// <summary>
    /// 光标
    /// </summary>
    public class PlayerCursorReader : IMCOSComponent
    {
        public PlayerCursorReader(string scoreboard)
        {
            _scoreboard = scoreboard ?? throw new ArgumentNullException(nameof(scoreboard));
            _batuuid = "f78b8570-3f3f-43be-8ade-5f11e77a4563";
            IsInitialState = true;
            CurrentPlayer = string.Empty;
            CursorMode = CursorMode.Mouse;
            InitialText = string.Empty;
            CurrentText = string.Empty;

            OnCursorMove += (arg1, arg2) => { };
            OnRightClick += (obj) => { };
            OnLeftClick += (obj) => { };
            OnTextUpdate += (arg1, arg2) => { };
        }

        private const string MOUSE_ITEM = "minecraft:snowball";

        private const string TEXTEDITOR_ITEM = "minecraft:writable_book";

        private readonly string _scoreboard;

        private readonly string _batuuid;

        public bool IsInitialState { get; private set; }

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

        /// <summary>
        /// 光标模式
        /// </summary>
        public CursorMode CursorMode { get; private set; }

        /// <summary>
        /// 初始文本
        /// </summary>
        public string InitialText { get; set; }

        /// <summary>
        /// 当前文本
        /// </summary>
        public string CurrentText { get; private set; }

        /// <summary>
        /// 光标当前的位置
        /// </summary>
        public Point CurrentPosition { get; private set; }

        /// <summary>
        /// 光标的操作员
        /// </summary>
        public string CurrentPlayer { get; private set; }

        /// <summary>
        /// 当光标移动时
        /// </summary>
        public event Action<Point, CursorMode> OnCursorMove;

        /// <summary>
        /// 当光标右键点击时
        /// </summary>
        public event Action<Point> OnRightClick;

        /// <summary>
        /// 当光标右键点击时
        /// </summary>
        public event Action<Point> OnLeftClick;

        /// <summary>
        /// 当文本编辑器更新时
        /// </summary>
        public event Action<Point, string> OnTextUpdate;

        public void ResetText()
        {
            CurrentText = string.Empty;
            IsInitialState = true;
        }

        public void Handle()
        {
            Screen screen = MCOS.Screen;
            ServerCommandHelper command = MCOS.MinecraftServer.CommandHelper;

            int length = screen.Width > screen.Height ? screen.Width : screen.Height;
            Vector3<int> center = screen.WorldCenterPosition;
            Vector3<double> start = new(center.X - length, center.Y - length, center.Z - length);
            Vector3<double> range = new(length * 2, length * 2, length * 2);

            Dictionary<string, IVector3<double>> players = command.GetRangePlayerPositionAsync(start, range).Result;
            if (players.Count == 0)
                return;

            Func<IVector3<double>, double> GetDistance = screen.NormalFacing switch
            {
                Facing.Xp or Facing.Xm => (vector3) => Math.Abs(vector3.X - screen.ScreenPlaneCoordinate),
                Facing.Yp or Facing.Ym => (vector3) => Math.Abs(vector3.Y - screen.ScreenPlaneCoordinate),
                Facing.Zp or Facing.Zm => (vector3) => Math.Abs(vector3.Z - screen.ScreenPlaneCoordinate),
                _ => throw new InvalidOperationException(),
            };
            List<(string name, double distance)> distances = new();
            foreach (var player in players)
                distances.Add((player.Key, GetDistance(player.Value)));
            var orderDistances = distances.OrderBy(item => item.distance);

            foreach (var distance in orderDistances)
            {
                if (HandlePlayer(distance.name))
                    return;
            }
        }

        private bool HandlePlayer(string player)
        {
            Screen screen = MCOS.Screen;
            ServerCommandHelper command = MCOS.MinecraftServer.CommandHelper;

            if (!command.TryGetPlayerSelectedItemSlot(player, out var solt) || !command.TryGetPlayerItem(player, solt, out var item))
                return false;

            if (item.ID != MOUSE_ITEM && item.ID != TEXTEDITOR_ITEM)
                return false;

            if (!command.TryGetEntityPosition(player, out var position) || !command.TryGetEntityRotation(player, out var rotation))
                return false;

            if (!Vector3Double.CheckPlaneReachability(position, rotation, screen.NormalFacing, screen.ScreenPlaneCoordinate))
                return false;

            position.Y += 1.625;
            Vector3<int> targetBlock = Vector3Double.GetToPlaneIntersection(position, rotation.ToDirection(), screen.NormalFacing, screen.ScreenPlaneCoordinate).ToVector3Int();
            Point targetPosition = screen.ToScreenPosition(targetBlock);
            if (!screen.IncludedOnScreen(targetPosition))
                return false;

            CurrentPlayer = player;

            if (targetPosition != CurrentPosition)
            {
                CurrentPosition = targetPosition;
                OnCursorMove.Invoke(CurrentPosition, CursorMode);
            }

            switch (item.ID)
            {
                case MOUSE_ITEM:
                    if (command.TryGetPlayerScoreboard(player, _scoreboard, out var score) && score > 0)
                    {
                        OnRightClick.Invoke(CurrentPosition);
                        command.SetPlayerScoreboardAsync(player, _scoreboard, 0).Wait();
                    }
                    //if (command.TryGetEntityHealth(_batuuid, out var health) && health < 6)
                    //{
                    //    OnLeftClick.Invoke(CurrentPosition);
                    //    Console.WriteLine("左键");
                    //    command.SetEntityHealthAsync(_batuuid, 6).Wait();
                    //}
                    CursorMode = CursorMode.Mouse;
                    break;
                case TEXTEDITOR_ITEM:
                    if (IsInitialState)
                    {
                        if (string.IsNullOrEmpty(InitialText))
                            command.SetPlayerHotbarItemAsync(player, solt, $"minecraft:writable_book{{pages:[]}}").Wait();
                        else
                            command.SetPlayerHotbarItemAsync(player, solt, $"minecraft:writable_book{{pages:[\"{InitialText}\"]}}").Wait();
                        CurrentText = InitialText;
                        IsInitialState = false;
                    }
                    else if (item.Tag.TryGetValue("pages", out var pagesTag) && pagesTag is string[] texts && texts.Length > 0)
                    {
                        if (texts[0] != CurrentText)
                        {
                            CurrentText = texts[0];
                            OnTextUpdate.Invoke(CurrentPosition, CurrentText);
                        }
                    }
                    else if (!string.IsNullOrEmpty(CurrentText))
                    {
                        CurrentText = string.Empty;
                        OnTextUpdate.Invoke(CurrentPosition, CurrentText);
                    }
                    CursorMode = CursorMode.TextEditor;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            //position.Y -= 0.5;
            //command.TelePortAsync(_batuuid, position + direction * 0.625).Wait();

            return true;
        }
    }
}
