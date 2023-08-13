using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.Block;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class WindowForm : Form
    {
        protected WindowForm()
        {
            TitleBar = new(this);
            ClientPanel = new();
            ShowTitleBar_Button = new();
        }

        public readonly WindowFormTitleBar TitleBar;

        public readonly ClientPanel ClientPanel;

        public readonly Button ShowTitleBar_Button;

        public bool ShowTitleBar
        {
            get => SubControls.Contains(TitleBar);
            set
            {
                if (value)
                {
                    if (!ShowTitleBar)
                    {
                        SubControls.TryAdd(TitleBar);
                        SubControls.Remove(ShowTitleBar_Button);
                        ClientPanel?.LayoutSyncer?.Sync();
                    }
                }
                else
                {
                    if (ShowTitleBar)
                    {
                        SubControls.Remove(TitleBar);
                        SubControls.TryAdd(ShowTitleBar_Button);
                        ClientPanel?.LayoutSyncer?.Sync();
                    }
                }
            }
        }

        public override string Text
        {
            get => TitleBar?.Text ?? string.Empty;
            set
            {
                if (TitleBar is null)
                    return;

                if (TitleBar.Text != value)
                {
                    string temp = TitleBar.Text;
                    TitleBar.Text = value;
                    HandleTextChanged(new(temp, TitleBar.Text));
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(TitleBar);

            SubControls.Add(ClientPanel);
            ClientPanel.BorderWidth = 0;
            ClientPanel.LayoutSyncer = new(this,
            (sender, e) => { },
            (sender, e) =>
            {
                if (ShowTitleBar)
                {
                    ClientPanel.ClientSize = new(ClientSize.Width, ClientSize.Height - TitleBar.Height);
                    ClientPanel.ClientLocation = new(0, TitleBar.Height);
                }
                else
                {
                    ClientPanel.ClientLocation = new(0, 0);
                    ClientPanel.ClientSize = ClientSize;
                }
            });

            ShowTitleBar_Button.Visible = false;
            ShowTitleBar_Button.InvokeExternalCursorMove = true;
            ShowTitleBar_Button.ClientSize = new(16, 16);
            ShowTitleBar_Button.LayoutSyncer = new(this, (sender, e) => { }, (sender, e) =>
            ShowTitleBar_Button.ClientLocation = this.LeftLayout(null, ShowTitleBar_Button, 0, 0));
            ShowTitleBar_Button.Anchor = Direction.Top | Direction.Right;
            ShowTitleBar_Button.Skin.SetAllBackgroundImage(TextureManager.GetTexture("Shrink"));
            ShowTitleBar_Button.CursorEnter += ShowTitleBar_Button_CursorEnter;
            ShowTitleBar_Button.CursorLeave += ShowTitleBar_Button_CursorLeave;
            ShowTitleBar_Button.RightClick += ShowTitleBar_Button_RightClick;
        }

        protected override void OnInitializeCompleted(Control sender, EventArgs e)
        {
            if (ClientPanel.PageSize != new Size(0, 0))
            {
                RestoreSize = new(ClientPanel.PageSize.Width, ClientPanel.PageSize.Height + TitleBar.Height);
                RestoreLocation = new(Width / 2 - RestoreSize.Width / 2, Height / 2 - RestoreSize.Height / 2);
            }
            else
            {
                base.OnInitializeCompleted(sender, e);
            }
        }

        public override void OnInitCompleted1()
        {
            base.OnInitCompleted1();

            ShowTitleBar_Button.ClientLocation = this.LeftLayout(null, ShowTitleBar_Button, 0, 0);
        }

        protected override void OnMove(Control sender, PositionChangedEventArgs e)
        {
            base.OnMove(sender, e);

            TitleBar.UpdateMaximizeOrRestore();
        }

        protected override void OnResize(Control sender, SizeChangedEventArgs e)
        {
            base.OnResize(sender, e);

            TitleBar.UpdateMaximizeOrRestore();
        }

        protected override void OnControlSelected(Control sender, EventArgs e)
        {
            base.OnControlSelected(sender, e);

            TitleBar.Skin.SetAllForegroundBlockID(BlockManager.Concrete.Black);
        }

        protected override void OnControlDeselected(Control sender, EventArgs e)
        {
            base.OnControlDeselected(sender, e);

            TitleBar.Skin.SetAllForegroundBlockID(BlockManager.Concrete.LightGray);
        }

        private void ShowTitleBar_Button_CursorEnter(Control sender, CursorEventArgs e)
        {
            ShowTitleBar_Button.Visible = true;
        }

        private void ShowTitleBar_Button_CursorLeave(Control sender, CursorEventArgs e)
        {
            ShowTitleBar_Button.Visible = false;
        }

        private void ShowTitleBar_Button_RightClick(Control sender, CursorEventArgs e)
        {
            ShowTitleBar = true;
        }

        public class WindowFormTitleBar : ContainerControl<Control>
        {
            public WindowFormTitleBar(WindowForm owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));

                base.Text = _owner.Text;
                LayoutSyncer = new(_owner, (sender, e) => { }, (sender, e) => Width = e.NewSize.Width);
                _owner.TextChanged += (sender, e) => base.Text = _owner.Text;
                _owner.InitializeCompleted += Owner_InitializeCallback;

                MoveAnchorPoint = new(0, 0);
                _ButtonsToShow = FormButtons.Close | FormButtons.MaximizeOrRestore | FormButtons.Minimize | FormButtons.FullScreen;

                Title_IconTextBox = new();
                Close_Button = new();
                MaximizeOrRestore_Switch = new();
                Minimize_Button = new();
                FullScreen_Button = new();

                BorderWidth = 0;
                InvokeExternalCursorMove = true;
            }

            private readonly WindowForm _owner;

            private readonly IconTextBox Title_IconTextBox;

            private readonly Button Close_Button;

            private readonly Switch MaximizeOrRestore_Switch;

            private readonly Button Minimize_Button;

            private readonly Button FullScreen_Button;

            public override string Text
            {
                get => Title_IconTextBox.Text_Label.Text;
                set
                {
                    if (Title_IconTextBox.Text_Label.Text != value)
                    {
                        string temp = Title_IconTextBox.Text_Label.Text;
                        Title_IconTextBox.Text_Label.Text = value;
                        HandleTextChanged(new(temp, Title_IconTextBox.Text_Label.Text));
                    }
                }
            }

            public FormButtons ButtonsToShow
            {
                get => _ButtonsToShow;
                set
                {
                    _ButtonsToShow = value;
                    ActiveLayoutAll();
                }
            }
            private FormButtons _ButtonsToShow;

            public Point MoveAnchorPoint { get; set; }

            public override void Initialize()
            {
                base.Initialize();

                if (_owner != ParentContainer)
                    throw new InvalidOperationException();

                SubControls.Add(Title_IconTextBox);
                Title_IconTextBox.KeepWhenClear = true;
                Title_IconTextBox.AutoSize = true;
                Title_IconTextBox.Icon_PictureBox.SetImage(_owner.Icon);
                Title_IconTextBox.Text_Label.Text = _owner.Text;
                Title_IconTextBox.BorderWidth = 0;

                Close_Button.BorderWidth = 0;
                Close_Button.ClientSize = new(16, 16);
                Close_Button.Anchor = Direction.Top | Direction.Right;
                Close_Button.Skin.IsRenderedImageBackground = true;
                Close_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
                Close_Button.Skin.BackgroundBlockID_Hover = BlockManager.Concrete.Red;
                Close_Button.Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.Red;
                Close_Button.Skin.SetAllBackgroundImage(TextureManager.GetTexture("Close"));
                Close_Button.RightClick += Exit_Button_RightClick;

                MaximizeOrRestore_Switch.BorderWidth = 0;
                MaximizeOrRestore_Switch.ClientSize = new(16, 16);
                MaximizeOrRestore_Switch.Anchor = Direction.Top | Direction.Right;
                MaximizeOrRestore_Switch.Skin.IsRenderedImageBackground = true;
                MaximizeOrRestore_Switch.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
                MaximizeOrRestore_Switch.Skin.BackgroundBlockID_Selected = Skin.BackgroundBlockID;
                MaximizeOrRestore_Switch.Skin.BackgroundBlockID_Hover = BlockManager.Concrete.LightGray;
                MaximizeOrRestore_Switch.Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.LightGray;
                MaximizeOrRestore_Switch.Skin.SetBackgroundImage(ControlState.None, TextureManager.GetTexture("Maximize"));
                MaximizeOrRestore_Switch.Skin.SetBackgroundImage(ControlState.Hover, TextureManager.GetTexture("Maximize"));
                MaximizeOrRestore_Switch.Skin.SetBackgroundImage(ControlState.Selected, TextureManager.GetTexture("Restore"));
                MaximizeOrRestore_Switch.Skin.SetBackgroundImage(ControlState.Hover | ControlState.Selected, TextureManager.GetTexture("Restore"));

                Minimize_Button.BorderWidth = 0;
                Minimize_Button.ClientSize = new(16, 16);
                Minimize_Button.Anchor = Direction.Top | Direction.Right;
                Minimize_Button.Skin.IsRenderedImageBackground = true;
                Minimize_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
                Minimize_Button.Skin.BackgroundBlockID_Hover = BlockManager.Concrete.LightGray;
                Minimize_Button.Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.LightGray;
                Minimize_Button.Skin.SetAllBackgroundImage(TextureManager.GetTexture("Minimize"));
                Minimize_Button.RightClick += Minimize_Button_RightClick;

                FullScreen_Button.BorderWidth = 0;
                FullScreen_Button.ClientSize = new(16, 16);
                FullScreen_Button.Anchor = Direction.Top | Direction.Right;
                FullScreen_Button.Skin.IsRenderedImageBackground = true;
                FullScreen_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
                FullScreen_Button.Skin.BackgroundBlockID_Hover = BlockManager.Concrete.LightGray;
                FullScreen_Button.Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.LightGray;
                FullScreen_Button.Skin.SetAllBackgroundImage(TextureManager.GetTexture("Expand"));
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
                else if (_owner.IsSelected && _owner.AllowMove && (GetSubControls().FirstHover is null or IconTextBox))
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
                if (!_owner._onresize)
                {
                    if (_owner.IsMaximize)
                        MaximizeOrRestore_Switch.IsSelected = true;
                    else
                        MaximizeOrRestore_Switch.IsSelected = false;
                }
            }

            public override void ActiveLayoutAll()
            {
                SubControls.Clear();
                if (ButtonsToShow.HasFlag(FormButtons.Close))
                {
                    Close_Button.ClientLocation = this.LeftLayout(SubControls.RecentlyAddedControl, Close_Button, 0, 0);
                    SubControls.Add(Close_Button);
                }
                if (ButtonsToShow.HasFlag(FormButtons.MaximizeOrRestore))
                {
                    MaximizeOrRestore_Switch.ClientLocation = this.LeftLayout(SubControls.RecentlyAddedControl, MaximizeOrRestore_Switch, 0, 0);
                    SubControls.Add(MaximizeOrRestore_Switch);
                }
                if (ButtonsToShow.HasFlag(FormButtons.Minimize))
                {
                    Minimize_Button.ClientLocation = this.LeftLayout(SubControls.RecentlyAddedControl, Minimize_Button, 0, 0);
                    SubControls.Add(Minimize_Button);
                }
                if (ButtonsToShow.HasFlag(FormButtons.FullScreen))
                {
                    FullScreen_Button.ClientLocation = this.LeftLayout(SubControls.RecentlyAddedControl, FullScreen_Button, 0, 0);
                    SubControls.Add(FullScreen_Button);
                }
            }
        }
    }
}
