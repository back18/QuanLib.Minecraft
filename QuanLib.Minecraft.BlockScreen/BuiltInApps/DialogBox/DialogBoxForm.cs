using QuanLib.Minecraft.BlockScreen.Controls;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.DialogBox
{
    public abstract class DialogBoxForm : WindowForm
    {
        protected DialogBoxForm()
        {
            AllowDeselected = false;
            AllowResize = false;
            TitleBar.ButtonsToShow = TitleBarButtonType.Close;

            _ButtonsToShow = DialogBoxReturnValue.Yes | DialogBoxReturnValue.No | DialogBoxReturnValue.Cancel;
            ButtonSize = new(32, 16);
        }

        public DialogBoxReturnValue ButtonsToShow
        {
            get => _ButtonsToShow;
            set
            {
                _ButtonsToShow = value;
                RefreshButtons();
            }
        }
        private DialogBoxReturnValue _ButtonsToShow;

        public Size ButtonSize { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            Client_Panel.Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Lime));
        }

        public override void OnInitComplete3()
        {
            base.OnInitComplete3();

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            if (!FormIsInitialize())
                return;

            Client_Panel.SubControls.Clear();
            if (ButtonsToShow.HasFlag(DialogBoxReturnValue.Yes))
            {
                Button button = new();
                button.Text = "是";
                button.ClientSize = ButtonSize;
                button.ClientLocation = Client_Panel.RightLayout(Client_Panel.SubControls.RecentlyAddedControl, 2, 18);
                button.RightClick += (position) => Complete(DialogBoxReturnValue.Yes);
                Client_Panel.SubControls.Add(button);
            }
            if (ButtonsToShow.HasFlag(DialogBoxReturnValue.No))
            {
                Button button = new();
                button.Text = "否";
                button.ClientSize = ButtonSize;
                button.ClientLocation = Client_Panel.RightLayout(Client_Panel.SubControls.RecentlyAddedControl, 2, 18);
                button.RightClick += (position) => Complete(DialogBoxReturnValue.No);
                Client_Panel.SubControls.Add(button);
            }
            if (ButtonsToShow.HasFlag(DialogBoxReturnValue.OK))
            {
                Button button = new();
                button.Text = "确认";
                button.ClientSize = ButtonSize;
                button.ClientLocation = Client_Panel.RightLayout(Client_Panel.SubControls.RecentlyAddedControl, 2, 18);
                button.RightClick += (position) => Complete(DialogBoxReturnValue.OK);
                Client_Panel.SubControls.Add(button);
            }
            if (ButtonsToShow.HasFlag(DialogBoxReturnValue.Cancel))
            {
                Button button = new();
                button.Text = "取消";
                button.ClientSize = ButtonSize;
                button.ClientLocation = Client_Panel.RightLayout(Client_Panel.SubControls.RecentlyAddedControl, 2, 18);
                button.RightClick += (position) => Complete(DialogBoxReturnValue.Cancel);
                Client_Panel.SubControls.Add(button);
            }

            int width = (Client_Panel.SubControls.RecentlyAddedControl?.RightLocation ?? 64) + 3;
            int height = 54;
            ClientSize = new(width, height);
        }

        private void Complete(DialogBoxReturnValue returnValue)
        {
            DialogBoxApp app = (DialogBoxApp)GetApplication();
            app.ReturnValue = returnValue;
            app.Exit();
        }
    }
}
