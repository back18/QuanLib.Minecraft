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
    public class TaskBar : ContainerControl<Control>
    {
        public TaskBar(RootForm owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            BorderWidth = 0;
            LayoutSyncer = new(_owner, (sender, e) => { }, (sender, e) =>
            {
                Width = e.NewSize.Width;
                ClientLocation = new(0, e.NewSize.Height - 16);
            });
            Skin.SetAllBackgroundBlockID(BlockManager.Concrete.Lime);

            StartMenu_Switch = new();
            TeskItems_Panel = new();
            FullScreen_Button = new();
        }

        private readonly RootForm _owner;

        private readonly Switch StartMenu_Switch;

        private readonly Panel<Control> TeskItems_Panel;

        private readonly Button FullScreen_Button;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();

            MCOS os = MCOS.GetMCOS();
            string dir = PathManager.SystemResources_Textures_Control_Dir;
            string green = BlockManager.Concrete.Green;

            SubControls.Add(FullScreen_Button);
            FullScreen_Button.BorderWidth = 0;
            FullScreen_Button.Text = "↓";
            FullScreen_Button.ClientSize = new(16, 16);
            FullScreen_Button.ClientLocation = this.LeftLayout(null, FullScreen_Button, 0, 0);
            FullScreen_Button.Anchor = Direction.Bottom | Direction.Right;
            FullScreen_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            FullScreen_Button.Skin.BackgroundBlockID_Hover = green;
            FullScreen_Button.Skin.BackgroundBlockID_Hover_Selected = green;
            FullScreen_Button.RightClick += HideTitleBar_Button_RightClick;

            SubControls.Add(StartMenu_Switch);
            StartMenu_Switch.BorderWidth = 0;
            StartMenu_Switch.ClientSize = new(16, 16);
            ImageFrame startOFF = new(Path.Combine(dir, "Start_OFF.png"), GetScreenPlaneSize().NormalFacing, StartMenu_Switch.ClientSize);
            ImageFrame startON = new(Path.Combine(dir, "Start_ON.png"), GetScreenPlaneSize().NormalFacing, StartMenu_Switch.ClientSize);
            StartMenu_Switch.Skin.BackgroundImage = startOFF;
            StartMenu_Switch.Skin.BackgroundImage_Hover = startOFF;
            StartMenu_Switch.Skin.BackgroundImage_Selected = startON;
            StartMenu_Switch.Skin.BackgroundImage_Hover_Selected = startON;

            //SubControls.Add(TeskItems_Panel);

        }

        private void HideTitleBar_Button_RightClick(Control sender, CursorEventArgs e)
        {
            _owner.ShowTaskBar = false;
        }
    }
}
