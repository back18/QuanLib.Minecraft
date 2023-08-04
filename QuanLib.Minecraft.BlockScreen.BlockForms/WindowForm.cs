using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;

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

        public readonly TitleBar TitleBar;

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
                    ClientPanel.ClientSize = new(ClientSize.Width, ClientSize.Height - 16);
                    ClientPanel.ClientLocation = new(0, 16);
                }
                else
                {
                    ClientPanel.ClientLocation = new(0, 0);
                    ClientPanel.ClientSize = ClientSize;
                }
            });

            ShowTitleBar_Button.Visible = false;
            ShowTitleBar_Button.InvokeExternalCursorMove = true;
            ShowTitleBar_Button.Text = "↓";
            ShowTitleBar_Button.ClientSize = new(16, 16);
            ShowTitleBar_Button.LayoutSyncer = new(this, (sender, e) => { }, (sender, e) =>
            ShowTitleBar_Button.ClientLocation = this.LifeLayout(null, ShowTitleBar_Button, 0, 0));
            ShowTitleBar_Button.Anchor = Direction.Top | Direction.Right;
            ShowTitleBar_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            ShowTitleBar_Button.CursorEnter += ShowTitleBar_Button_CursorEnter;
            ShowTitleBar_Button.CursorLeave += ShowTitleBar_Button_CursorLeave;
            ShowTitleBar_Button.RightClick += ShowTitleBar_Button_RightClick;
        }

        protected override void OnInitializeCallback(Control sender, EventArgs e)
        {
            if (ClientPanel.PageSize != new Size(0, 0))
            {
                RestoreSize = new(ClientPanel.PageSize.Width, ClientPanel.PageSize.Height + TitleBar.Height);
                RestoreLocation = new(Width / 2 - RestoreSize.Width / 2, Height / 2 - RestoreSize.Height / 2);
            }
            else
            {
                base.OnInitializeCallback(sender, e);
            }
        }

        public override void OnInitCompleted1()
        {
            base.OnInitCompleted1();

            ShowTitleBar_Button.ClientLocation = this.LifeLayout(null, ShowTitleBar_Button, 0, 0);
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

            TitleBar.Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Black));
        }

        protected override void OnControlDeselected(Control sender, EventArgs e)
        {
            base.OnControlDeselected(sender, e);

            TitleBar.Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.LightGray));
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
    }
}
