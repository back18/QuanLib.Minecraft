using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public abstract class WindowForm : Form
    {
        protected WindowForm()
        {
            TitleBar = new(this);
            Client_Panel = new();
            ShowTitleBar_Button = new();
        }

        public readonly TitleBar TitleBar;

        public readonly Panel<Control> Client_Panel;

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
                        Client_Panel?.LayoutSyncer?.Sync();
                    }
                }
                else
                {
                    if (ShowTitleBar)
                    {
                        SubControls.Remove(TitleBar);
                        SubControls.TryAdd(ShowTitleBar_Button);
                        Client_Panel?.LayoutSyncer?.Sync();
                    }
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            OnSelected += Form_OnSelected;
            OnDeselected += Form_OnDeselected;

            SubControls.Add(TitleBar);

            SubControls.Add(Client_Panel);
            Client_Panel.BorderWidth = 0;
            Client_Panel.LayoutSyncer = new(this,
            (oldPosition, newPosition) =>
            {
                if (ShowTitleBar)
                    Client_Panel.ClientLocation = new(0, 16);
                else
                    Client_Panel.ClientLocation = new(0, 0);
            },
            (oldSize, newSize) =>
            {
                if (ShowTitleBar)
                    Client_Panel.ClientSize = new(ClientSize.Width, ClientSize.Height - 16);
                else
                    Client_Panel.ClientSize = new(ClientSize.Width, ClientSize.Height);
            });

            Client_Panel?.LayoutSyncer?.Sync();

            ShowTitleBar_Button.Visible = false;
            ShowTitleBar_Button.InvokeExternalCursorMove = true;
            ShowTitleBar_Button.Text = "↓";
            ShowTitleBar_Button.ClientSize = new(16, 16);
            ShowTitleBar_Button.LayoutSyncer = new(this, (oldPosition, newPosition) => { }, (oldSize, newSize) => ShowTitleBar_Button.ClientLocation = this.LifeLayout(null, ShowTitleBar_Button, 0, 0));
            ShowTitleBar_Button.Anchor = PlaneFacing.Top | PlaneFacing.Right;
            ShowTitleBar_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            ShowTitleBar_Button.CursorEnter += ShowTitleBar_Button_CursorEnter;
            ShowTitleBar_Button.CursorLeave += ShowTitleBar_Button_CursorLeave;
            ShowTitleBar_Button.RightClick += ShowTitleBar_Button_RightClick;
        }

        public override void OnInitComplete1()
        {
            base.OnInitComplete1();

            ShowTitleBar_Button.ClientLocation = this.LifeLayout(null, ShowTitleBar_Button, 0, 0);
        }

        protected override void Form_OnMove(Point oldPosition, Point newPosition)
        {
            base.Form_OnMove(oldPosition, newPosition);

            TitleBar.UpdateMaximizeOrRestore();
        }

        protected override void Form_OnResize(Size oldSize, Size newSize)
        {
            base.Form_OnResize(oldSize, newSize);

            TitleBar.UpdateMaximizeOrRestore();
        }

        private void Form_OnSelected()
        {
            TitleBar.Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Black));
        }

        private void Form_OnDeselected()
        {
            TitleBar.Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.LightGray));
        }

        private void ShowTitleBar_Button_CursorEnter(Point position, CursorMode mode)
        {
            ShowTitleBar_Button.Visible = true;
        }

        private void ShowTitleBar_Button_CursorLeave(Point position, CursorMode mode)
        {
            ShowTitleBar_Button.Visible = false;
        }

        private void ShowTitleBar_Button_RightClick(Point position)
        {
            ShowTitleBar = true;
        }
    }
}
