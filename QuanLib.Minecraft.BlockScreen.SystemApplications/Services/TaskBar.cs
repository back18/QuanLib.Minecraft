using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Services
{
    public class TaskBar : ContainerControl<Control>
    {
        public TaskBar(RootForm owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            BorderWidth = 0;
            LayoutSyncer = new(_owner, (oldPosition, newPosition) => { }, (oldSize, newSize) => Width = newSize.Width);
            Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Lime));

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

            MCOS os = GetMCOS();
            string dir = PathManager.SystemResources_Textures_Control_Dir;
            string green = ConcretePixel.ToBlockID(MinecraftColor.Green);

            SubControls.Add(FullScreen_Button);
            FullScreen_Button.BorderWidth = 0;
            FullScreen_Button.Text = "↓";
            FullScreen_Button.ClientSize = new(16, 16);
            FullScreen_Button.ClientLocation = this.LifeLayout(null, FullScreen_Button, 0, 0);
            FullScreen_Button.Anchor = Direction.Bottom | Direction.Right;
            FullScreen_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            FullScreen_Button.Skin.BackgroundBlockID_Hover = green;
            FullScreen_Button.Skin.BackgroundBlockID_Hover_Selected = green;
            FullScreen_Button.RightClick += HideTitleBar_Button_RightClick;

            SubControls.Add(StartMenu_Switch);
            StartMenu_Switch.BorderWidth = 0;
            StartMenu_Switch.ClientSize = new(16, 16);
            ImageFrame startOFF = new(Path.Combine(dir, "Start_OFF.png"), os.Screen.NormalFacing, StartMenu_Switch.ClientSize);
            ImageFrame startON = new(Path.Combine(dir, "Start_ON.png"), os.Screen.NormalFacing, StartMenu_Switch.ClientSize);
            StartMenu_Switch.Skin.BackgroundImage = startOFF;
            StartMenu_Switch.Skin.BackgroundImage_Hover = startOFF;
            StartMenu_Switch.Skin.BackgroundImage_Selected = startON;
            StartMenu_Switch.Skin.BackgroundImage_Hover_Selected = startON;

            //SubControls.Add(TeskItems_Panel);

        }

        private void HideTitleBar_Button_RightClick(Point position)
        {
            _owner.ShowTitleBar = false;
        }
    }
}
