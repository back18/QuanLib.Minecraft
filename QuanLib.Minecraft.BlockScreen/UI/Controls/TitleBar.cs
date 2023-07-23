using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI.Controls
{
    public class TitleBar : ContainerControl<Control>
    {
        public TitleBar(WindowForm owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            Text = _owner.Text;
            LayoutSyncer = new(_owner, (oldPosition, newPosition) => { }, (oldSize, newSize) => Width = newSize.Width);
            _owner.OnTextUpdateNow += (oldText, newText) => Text = _owner.Text;
            _owner.InitializeCallback += Owner_InitializeCallback;

            MoveAnchorPoint = new(0, 0);
            _ButtonsToShow = FormButton.Close | FormButton.MaximizeOrRestore | FormButton.Minimize | FormButton.FullScreen;

            Close_Button = new();
            MaximizeOrRestore_Switch = new();
            Minimize_Button = new();
            FullScreen_Button = new();

            BorderWidth = 0;
            InvokeExternalCursorMove = true;
            RightClick += FormTitleBar_RightClick;
            CursorMove += FormTitleBar_CursorMove;
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

            MCOS os = GetMCOS();
            string dir = PathManager.SystemResources_Textures_Control_Dir;
            string lghtGray = ConcretePixel.ToBlockID(MinecraftColor.LightGray);
            string red = ConcretePixel.ToBlockID(MinecraftColor.Red);

            Close_Button.BorderWidth = 0;
            Close_Button.Text = "X";
            Close_Button.ClientSize = new(16, 16);
            Close_Button.Anchor = PlaneFacing.Top | PlaneFacing.Right;
            Close_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            Close_Button.Skin.SetBackgroundBlockID(ControlState.Hover, red);
            Close_Button.Skin.SetBackgroundBlockID(ControlState.Hover | ControlState.Selected, red);
            Close_Button.RightClick += Exit_Button_RightClick;

            MaximizeOrRestore_Switch.BorderWidth = 0;
            MaximizeOrRestore_Switch.ClientSize = new(16, 16);
            MaximizeOrRestore_Switch.Anchor = PlaneFacing.Top | PlaneFacing.Right;
            MaximizeOrRestore_Switch.Skin.BackgroundImage = new(Path.Combine(dir, "最大化.png"), os.Screen.NormalFacing, MaximizeOrRestore_Switch.ClientSize);
            MaximizeOrRestore_Switch.Skin.BackgroundImage_Selected = new(Path.Combine(dir, "还原.png"), os.Screen.NormalFacing, MaximizeOrRestore_Switch.ClientSize);
            MaximizeOrRestore_Switch.Skin.BackgroundImage_Hover = new(Path.Combine(dir, "最大化过度.png"), os.Screen.NormalFacing, MaximizeOrRestore_Switch.ClientSize);
            MaximizeOrRestore_Switch.Skin.BackgroundImage_Hover_Selected = new(Path.Combine(dir, "还原过度.png"), os.Screen.NormalFacing, MaximizeOrRestore_Switch.ClientSize);

            Minimize_Button.BorderWidth = 0;
            Minimize_Button.Text = "一";
            Minimize_Button.ClientSize = new(16, 16);
            Minimize_Button.Anchor = PlaneFacing.Top | PlaneFacing.Right;
            Minimize_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            Minimize_Button.Skin.BackgroundBlockID_Hover = lghtGray;
            Minimize_Button.Skin.BackgroundBlockID_Hover_Selected = lghtGray;
            Minimize_Button.RightClick += Minimize_Button_RightClick;

            FullScreen_Button.BorderWidth = 0;
            FullScreen_Button.Text = "↑";
            FullScreen_Button.ClientSize = new(16, 16);
            FullScreen_Button.Anchor = PlaneFacing.Top | PlaneFacing.Right;
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

        private void Owner_InitializeCallback()
        {
            UpdateMaximizeOrRestore();
            MaximizeOrRestore_Switch.OnSelected += MaximizeOrRestore_Switch_OnSelected;
            MaximizeOrRestore_Switch.OnDeselected += MaximizeOrRestore_Switch_OnDeselected;
        }

        private void FormTitleBar_RightClick(Point position)
        {
            if (IsSelected)
                IsSelected = false;
            else if (_owner.IsSelected && !GetSubControls().HaveHover)
            {
                IsSelected = true;
                MoveAnchorPoint = position;
            }
        }

        private void FormTitleBar_CursorMove(Point position, CursorMode mode)
        {
            if (IsSelected)
            {
                Point offset = new(position.X - MoveAnchorPoint.X, position.Y - MoveAnchorPoint.Y);
                _owner.ClientLocation = new(_owner.ClientLocation.X + offset.X, _owner.ClientLocation.Y + offset.Y);
            }
        }

        private void Exit_Button_RightClick(Point position)
        {
            _owner.CloseForm();
        }

        private void MaximizeOrRestore_Switch_OnSelected()
        {
            _owner.MaximizeForm();
        }

        private void MaximizeOrRestore_Switch_OnDeselected()
        {
            _owner.RestoreForm();
        }

        private void Minimize_Button_RightClick(Point position)
        {
            GetProcess().Pending();
        }

        private void HideTitleBar_Button_RightClick(Point position)
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
            if (!AllowGetApplication())
                return;

            SubControls.Clear();
            if (ButtonsToShow.HasFlag(FormButton.Close))
            {
                Close_Button.ClientLocation = this.LifeLayout(SubControls.RecentlyAddedControl, Close_Button, 0, 0);
                SubControls.Add(Close_Button);
            }
            if (ButtonsToShow.HasFlag(FormButton.MaximizeOrRestore))
            {
                MaximizeOrRestore_Switch.ClientLocation = this.LifeLayout(SubControls.RecentlyAddedControl, MaximizeOrRestore_Switch, 0, 0);
                SubControls.Add(MaximizeOrRestore_Switch);
            }
            if (ButtonsToShow.HasFlag(FormButton.Minimize))
            {
                Minimize_Button.ClientLocation = this.LifeLayout(SubControls.RecentlyAddedControl, Minimize_Button, 0, 0);
                SubControls.Add(Minimize_Button);
            }
            if (ButtonsToShow.HasFlag(FormButton.FullScreen))
            {
                FullScreen_Button.ClientLocation = this.LifeLayout(SubControls.RecentlyAddedControl, FullScreen_Button, 0, 0);
                SubControls.Add(FullScreen_Button);
            }
        }
    }
}
