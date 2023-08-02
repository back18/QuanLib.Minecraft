using CoreRCON;
using QuanLib.Event;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.Data;
using QuanLib.Minecraft.Vectors;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    /// <summary>
    /// 屏幕输入处理
    /// </summary>
    public class ScreenInputHandler : ICursorReader, ITextEditor
    {
        public ScreenInputHandler(Screen owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _objective = ConfigManager.ScreenConfig.RightClickObjective;
            _batuuid = "f78b8570-3f3f-43be-8ade-5f11e77a4563";
            IsInitialState = true;
            CurrentPlayer = string.Empty;
            CurrenMode = CursorMode.Cursor;
            CurrentPosition = new(0, 0);
            CurrentSlot = 0;
            CurrentItem = null;
            InitialText = string.Empty;
            CurrentText = string.Empty;

            CursorMove += OnCursorMove;
            RightClick += OnRightClick;
            LeftClick += OnLeftClick;
            CursorSlotChanged += OnCursorSlotChanged;
            CursorItemChanged += OnCursorItemChanged;
            TextEditorChanged += OnTextEditorChanged;
        }

        private const string MOUSE_ITEM = "minecraft:snowball";

        private const string TEXTEDITOR_ITEM = "minecraft:writable_book";

        private readonly Screen _owner;

        private readonly string _objective;

        private readonly string _batuuid;

        public bool IsInitialState { get; private set; }

        public CursorMode CurrenMode { get; private set; }

        public string CurrentPlayer { get; private set; }

        public Point CurrentPosition { get; private set; }

        public int CurrentSlot { get; private set; }

        public Item? CurrentItem { get; private set; }

        public string InitialText { get; set; }

        public string CurrentText { get; private set; }

        public event EventHandler<ICursorReader, CursorEventArgs> CursorMove;

        public event EventHandler<ICursorReader, CursorEventArgs> RightClick;

        public event EventHandler<ICursorReader, CursorEventArgs> LeftClick;

        public event EventHandler<ICursorReader, CursorSlotEventArgs> CursorSlotChanged;

        public event EventHandler<ICursorReader, CursorItemEventArgs> CursorItemChanged;

        public event EventHandler<ITextEditor, CursorTextEventArgs> TextEditorChanged;

        protected virtual void OnCursorMove(ICursorReader sender, CursorEventArgs e) { }

        protected virtual void OnRightClick(ICursorReader sender, CursorEventArgs e) { }

        protected virtual void OnLeftClick(ICursorReader sender, CursorEventArgs e) { }

        protected virtual void OnCursorSlotChanged(ICursorReader sender, CursorSlotEventArgs e) { }

        protected virtual void OnCursorItemChanged(ICursorReader sender, CursorItemEventArgs e) { }

        protected virtual void OnTextEditorChanged(ITextEditor sender, CursorTextEventArgs e) { }

        public void ResetText()
        {
            CurrentText = string.Empty;
            IsInitialState = true;
        }

        public void HandleInput()
        {
            Screen screen = _owner;
            ServerCommandHelper command = MCOS.GetMCOS().MinecraftServer.CommandHelper;

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
            Screen screen = _owner;
            ServerCommandHelper command = MCOS.GetMCOS().MinecraftServer.CommandHelper;

            if (!command.TryGetPlayerSelectedItemSlot(player, out var slot))
                return false;

            command.TryGetPlayerItem(player, slot, out Item? selectedItem);
            command.TryGetPlayerDualWieldItem(player, out Item? dualWieldItem);

            bool swap = false;
            if (selectedItem is not null && (selectedItem.ID == MOUSE_ITEM || selectedItem.ID == TEXTEDITOR_ITEM))
            {

            }
            else if (dualWieldItem is not null && (dualWieldItem.ID == MOUSE_ITEM || dualWieldItem.ID == TEXTEDITOR_ITEM))
            {
                Item? temp = selectedItem;
                selectedItem = dualWieldItem;
                dualWieldItem = temp;
                swap = true;
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

            List<Action> actions = new();

            CurrentPlayer = player;

            switch (selectedItem.ID)
            {
                case MOUSE_ITEM:
                    CurrenMode = CursorMode.Cursor;
                    break;
                case TEXTEDITOR_ITEM:
                    CurrenMode = CursorMode.TextEditor;
                    break;
            }

            if (targetPosition != CurrentPosition)
            {
                CurrentPosition = targetPosition;
                actions.Add(() => CursorMove.Invoke(this, new(CurrentPosition)));
            }

            if (swap && slot != CurrentSlot)
            {
                int temp = CurrentSlot;
                CurrentSlot = slot;
                actions.Add(() => CursorSlotChanged.Invoke(this, new(CurrentPosition, temp, CurrentSlot)));
            }

            if (Item.EqualsID(dualWieldItem, CurrentItem))
            {
                CurrentItem = dualWieldItem;
                actions.Add(() => CursorItemChanged.Invoke(this, new(CurrentPosition, CurrentItem)));
            }

            switch (selectedItem.ID)
            {
                case MOUSE_ITEM:
                    if (command.TryGetPlayerScoreboard(player, _objective, out var score) && score > 0)
                    {
                        RightClick.Invoke(this, new(CurrentPosition));
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
                            actions.Add(() => TextEditorChanged.Invoke(this, new(CurrentPosition, CurrentText)));
                        }
                    }
                    else if (!string.IsNullOrEmpty(CurrentText))
                    {
                        CurrentText = string.Empty;
                        actions.Add(() => TextEditorChanged.Invoke(this, new(CurrentPosition, CurrentText)));
                    }
                    break;
            }

            //position.Y -= 0.5;
            //command.TelePortAsync(_batuuid, position + direction * 0.625).Wait();

            foreach (var action in actions)
                action.Invoke();

            return true;
        }
    }
}
