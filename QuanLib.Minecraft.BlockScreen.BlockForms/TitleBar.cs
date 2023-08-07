using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.Block;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class TitleBar : ContainerControl<Control>
    {
        public TitleBar(WindowForm owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            Text = _owner.Text;
            LayoutSyncer = new(_owner, (sender, e) => { }, (sender, e) => Width = e.NewSize.Width);
            _owner.TextChanged += (sender, e) => Text = _owner.Text;
            _owner.InitializeCallback += Owner_InitializeCallback;

            MoveAnchorPoint = new(0, 0);
            _ButtonsToShow = FormButton.Close | FormButton.MaximizeOrRestore | FormButton.Minimize | FormButton.FullScreen;

            Close_Button = new();
            MaximizeOrRestore_Switch = new();
            Minimize_Button = new();
            FullScreen_Button = new();

            BorderWidth = 0;
            InvokeExternalCursorMove = true;
        }

        private readonly WindowForm _owner;

        private readonly Button Close_Button;

        private readonly Switch MaximizeOrRestore_Switch;

        private readonly Button Minimize_Button;

        private readonly Button FullScreen_Button;

        public FormButton ButtonsToShow
        {
            get => _ButtonsToShow;
            set
            {
                _ButtonsToShow = value;
                ActiveLayoutAll();
            }
        }
        private FormButton _ButtonsToShow;

        public Point MoveAnchorPoint { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();

            MCOS os = MCOS.GetMCOS();
            string dir = PathManager.SystemResources_Textures_Control_Dir;
            string lghtGray = BlockManager.Concrete.LightGray;
            string red = BlockManager.Concrete.Red;

            Close_Button.BorderWidth = 0;
            Close_Button.Text = "X";
            Close_Button.ClientSize = new(16, 16);
            Close_Button.Anchor = Direction.Top | Direction.Right;
            Close_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            Close_Button.Skin.SetBackgroundBlockID(ControlState.Hover, red);
            Close_Button.Skin.SetBackgroundBlockID(ControlState.Hover | ControlState.Selected, red);
            Close_Button.RightClick += Exit_Button_RightClick;

            MaximizeOrRestore_Switch.BorderWidth = 0;
            MaximizeOrRestore_Switch.ClientSize = new(16, 16);
            MaximizeOrRestore_Switch.Anchor = Direction.Top | Direction.Right;
            MaximizeOrRestore_Switch.Skin.BackgroundImage = new(Path.Combine(dir, "最大化.png"), GetScreenPlaneSize().NormalFacing, MaximizeOrRestore_Switch.ClientSize);
            MaximizeOrRestore_Switch.Skin.BackgroundImage_Selected = new(Path.Combine(dir, "还原.png"), GetScreenPlaneSize().NormalFacing, MaximizeOrRestore_Switch.ClientSize);
            MaximizeOrRestore_Switch.Skin.BackgroundImage_Hover = new(Path.Combine(dir, "最大化过度.png"), GetScreenPlaneSize().NormalFacing, MaximizeOrRestore_Switch.ClientSize);
            MaximizeOrRestore_Switch.Skin.BackgroundImage_Hover_Selected = new(Path.Combine(dir, "还原过度.png"), GetScreenPlaneSize().NormalFacing, MaximizeOrRestore_Switch.ClientSize);

            Minimize_Button.BorderWidth = 0;
            Minimize_Button.Text = "一";
            Minimize_Button.ClientSize = new(16, 16);
            Minimize_Button.Anchor = Direction.Top | Direction.Right;
            Minimize_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            Minimize_Button.Skin.BackgroundBlockID_Hover = lghtGray;
            Minimize_Button.Skin.BackgroundBlockID_Hover_Selected = lghtGray;
            Minimize_Button.RightClick += Minimize_Button_RightClick;

            FullScreen_Button.BorderWidth = 0;
            FullScreen_Button.Text = "↑";
            FullScreen_Button.ClientSize = new(16, 16);
            FullScreen_Button.Anchor = Direction.Top | Direction.Right;
            FullScreen_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            FullScreen_Button.Skin.BackgroundBlockID_Hover = lghtGray;
            FullScreen_Button.Skin.BackgroundBlockID_Hover_Selected = lghtGray;
            FullScreen_Button.RightClick += HideTitleBar_Button_RightClick;
        }

        public override void OnInitCompleted3()
        {
            base.OnInitCompleted3();

            ActiveLayoutAll();
        }

        private void Owner_InitializeCallback(Control sender, EventArgs e)
        {
            UpdateMaximizeOrRestore();
            MaximizeOrRestore_Switch.ControlSelected += MaximizeOrRestore_Switch_ControlSelected;
            MaximizeOrRestore_Switch.ControlDeselected += MaximizeOrRestore_Switch_ControlDeselected;
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            if (_owner.Moveing)
            {
                Point offset = new(e.Position.X - MoveAnchorPoint.X, e.Position.Y - MoveAnchorPoint.Y);
                _owner.ClientLocation = new(_owner.ClientLocation.X + offset.X, _owner.ClientLocation.Y + offset.Y);
            }
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            if (_owner.Moveing)
                _owner.Moveing = false;
            else if (_owner.IsSelected && _owner.AllowMove && !GetSubControls().HaveHover)
            {
                _owner.Moveing = true;
                MoveAnchorPoint = e.Position;
            }
        }

        private void Exit_Button_RightClick(Control sender, CursorEventArgs e)
        {
            _owner.CloseForm();
        }

        private void MaximizeOrRestore_Switch_ControlSelected(Control sender, EventArgs e)
        {
            _owner.MaximizeForm();
        }

        private void MaximizeOrRestore_Switch_ControlDeselected(Control sender, EventArgs e)
        {
            _owner.RestoreForm();
        }

        private void Minimize_Button_RightClick(Control sender, CursorEventArgs e)
        {
            _owner.MinimizeForm();
        }

        private void HideTitleBar_Button_RightClick(Control sender, CursorEventArgs e)
        {
            _owner.ShowTitleBar = false;
        }

        public void UpdateMaximizeOrRestore()
        {
            if (_owner.IsMaximize)
                MaximizeOrRestore_Switch.IsSelected = true;
            else
                MaximizeOrRestore_Switch.IsSelected = false;
        }

        public override void ActiveLayoutAll()
        {
            SubControls.Clear();
            if (ButtonsToShow.HasFlag(FormButton.Close))
            {
                Close_Button.ClientLocation = this.LeftLayout(SubControls.RecentlyAddedControl, Close_Button, 0, 0);
                SubControls.Add(Close_Button);
            }
            if (ButtonsToShow.HasFlag(FormButton.MaximizeOrRestore))
            {
                MaximizeOrRestore_Switch.ClientLocation = this.LeftLayout(SubControls.RecentlyAddedControl, MaximizeOrRestore_Switch, 0, 0);
                SubControls.Add(MaximizeOrRestore_Switch);
            }
            if (ButtonsToShow.HasFlag(FormButton.Minimize))
            {
                Minimize_Button.ClientLocation = this.LeftLayout(SubControls.RecentlyAddedControl, Minimize_Button, 0, 0);
                SubControls.Add(Minimize_Button);
            }
            if (ButtonsToShow.HasFlag(FormButton.FullScreen))
            {
                FullScreen_Button.ClientLocation = this.LeftLayout(SubControls.RecentlyAddedControl, FullScreen_Button, 0, 0);
                SubControls.Add(FullScreen_Button);
            }
        }
    }
}
