using QuanLib.Minecraft.BlockScreen.Controls;
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

            ControlSyncer = new(_owner, (oldPosition, newPosition) => { }, (oldSize, newSize) => Width = newSize.Width);

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


        }
    }
}
