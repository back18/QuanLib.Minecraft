using CoreRCON;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.Datas;
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
    /// 屏幕输入读取器
    /// </summary>
    public class ScreenInputReader : ICursorReader, ITextReader, IMCOSComponent
    {
        public ScreenInputReader(string objective)
        {
            if (string.IsNullOrEmpty(objective))
                throw new ArgumentException($"“{nameof(objective)}”不能为 null 或空。", nameof(objective));

            _objective = objective;
            _batuuid = "f78b8570-3f3f-43be-8ade-5f11e77a4563";
            IsInitialState = true;
            CurrentPlayer = string.Empty;
            CurrenMode = CursorMode.Mouse;
            CurrentPosition = new(0, 0);
            CurrentItem = null;
            InitialText = string.Empty;
            CurrentText = string.Empty;

            OnCursorMove += (obj) => { };
            OnRightClick += (obj) => { };
            OnLeftClick += (obj) => { };
            OnTextUpdate += (arg1, arg2) => { };
        }

        private const string MOUSE_ITEM = "minecraft:snowball";

        private const string TEXTEDITOR_ITEM = "minecraft:writable_book";

        private readonly string _objective;

        private readonly string _batuuid;

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

        public bool IsInitialState { get; private set; }

        public string CurrentPlayer { get; private set; }

        public CursorMode CurrenMode { get; private set; }

        public Point CurrentPosition { get; private set; }

        public Item? CurrentItem { get; private set; }

        public string InitialText { get; set; }

        public string CurrentText { get; private set; }

        public event Action<Point> OnCursorMove;

        public event Action<Point> OnRightClick;

        public event Action<Point> OnLeftClick;

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

            Dictionary<string, IVector3<double>> players = command.GetRangePlayerPosition(start, range);
            if (players.Count == 0)
                return;

            Func<IVector3<double>, double> GetDistance = screen.NormalFacing switch
            {
                Facing.Xp or Facing.Xm => (vector3) => Math.Abs(vector3.X - screen.PlaneCoordinate),
                Facing.Yp or Facing.Ym => (vector3) => Math.Abs(vector3.Y - screen.PlaneCoordinate),
                Facing.Zp or Facing.Zm => (vector3) => Math.Abs(vector3.Z - screen.PlaneCoordinate),
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

            if (!command.TryGetPlayerSelectedItem(player, out var selectedItem) & !command.TryGetPlayerDualWieldItem(player, out var dualWieldItem))
                return false;

            if (selectedItem is not null && (selectedItem.ID == MOUSE_ITEM || selectedItem.ID == TEXTEDITOR_ITEM))
            {

            }
            else if (dualWieldItem is not null && (dualWieldItem.ID == MOUSE_ITEM || dualWieldItem.ID == TEXTEDITOR_ITEM))
            {
                Item? temp = selectedItem;
                selectedItem = dualWieldItem;
                dualWieldItem = temp;
            }
            else
            {
                return false;
            }

            if (!command.TryGetEntityPosition(player, out var position) || !command.TryGetEntityRotation(player, out var rotation))
                return false;

            if (!Vector3Double.CheckPlaneReachability(position, rotation, screen.NormalFacing, screen.PlaneCoordinate))
                return false;

            position.Y += 1.625;
            Vector3<int> targetBlock = Vector3Double.GetToPlaneIntersection(position, rotation.ToDirection(), screen.NormalFacing, screen.PlaneCoordinate).ToVector3Int();
            Point targetPosition = screen.ToScreenPosition(targetBlock);
            if (!screen.IncludedOnScreen(targetPosition))
                return false;

            CurrentPlayer = player;
            CurrentItem = dualWieldItem;

            switch (selectedItem.ID)
            {
                case MOUSE_ITEM:
                    CurrenMode = CursorMode.Mouse;
                    break;
                case TEXTEDITOR_ITEM:
                    CurrenMode = CursorMode.TextEditor;
                    break;
            }

            if (targetPosition != CurrentPosition)
            {
                CurrentPosition = targetPosition;
                OnCursorMove.Invoke(CurrentPosition);
            }

            switch (selectedItem.ID)
            {
                case MOUSE_ITEM:
                    if (command.TryGetPlayerScoreboard(player, _objective, out var score) && score > 0)
                    {
                        OnRightClick.Invoke(CurrentPosition);
                        command.SetPlayerScoreboard(player, _objective, 0);
                    }
                    //if (command.TryGetEntityHealth(_batuuid, out var health) && health < 6)
                    //{
                    //    OnLeftClick.Invoke(CurrentPosition);
                    //    Console.WriteLine("左键");
                    //    command.SetEntityHealthAsync(_batuuid, 6).Wait();
                    //}
                    break;
                case TEXTEDITOR_ITEM:
                    if (IsInitialState)
                    {
                        if (string.IsNullOrEmpty(InitialText))
                            command.SetPlayerHotbarItem(player, selectedItem.Slot, $"minecraft:writable_book{{pages:[]}}");
                        else
                            command.SetPlayerHotbarItem(player, selectedItem.Slot, $"minecraft:writable_book{{pages:[\"{InitialText}\"]}}");
                        CurrentText = InitialText;
                        IsInitialState = false;
                    }
                    else if (
                        selectedItem.Tag is not null &&
                        selectedItem.Tag.TryGetValue("pages", out var pagesTag) &&
                        pagesTag is string[] texts && texts.Length > 0)
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
                    break;
            }

            //position.Y -= 0.5;
            //command.TelePortAsync(_batuuid, position + direction * 0.625).Wait();

            return true;
        }
    }
}
