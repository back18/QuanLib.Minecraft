using QuanLib.Minecraft.BlockScreen.Controls;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Services
{
    public class TaskBar : ItemCollectionControl<Process>
    {
        public TaskBar(RootForm owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            BorderWidth = 0;
            ControlSyncer = new(_owner, (oldPosition, newPosition) => { }, (oldSize, newSize) => Width = newSize.Width);
            Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Yellow));

            StartMenu_Switch = new();
            Time_Label = new();
            TeskItems_Panel = new();
            FullScreen_Button = new();
        }

        private readonly RootForm _owner;

        private readonly Switch StartMenu_Switch;

        private readonly Panel TeskItems_Panel;

        private readonly Label Time_Label;

        private readonly Button FullScreen_Button;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentControl)
                throw new InvalidOperationException();

            string dir = PathManager.SystemResources_Textures_Control_Dir;
            string lghtGray = ConcretePixel.ToBlockID(MinecraftColor.LightGray);

            SubControls.Add(FullScreen_Button);
            FullScreen_Button.BorderWidth = 0;
            FullScreen_Button.Text = "↓";
            FullScreen_Button.ClientSize = new(16, 16);
            FullScreen_Button.ClientLocation = this.LifeLayout(null, FullScreen_Button, 0, 0);
            FullScreen_Button.Anchor = PlaneFacing.Bottom | PlaneFacing.Right;
            FullScreen_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            FullScreen_Button.Skin.SetBackgroundBlockID(ControlState.Hover, lghtGray);
            FullScreen_Button.Skin.SetBackgroundBlockID(ControlState.Hover | ControlState.Selected, lghtGray);
            FullScreen_Button.RightClick += HideTitleBar_Button_RightClick;
        }

        private void HideTitleBar_Button_RightClick(Point position)
        {
            _owner.ShowTitleBar = false;
        }
    }
}
