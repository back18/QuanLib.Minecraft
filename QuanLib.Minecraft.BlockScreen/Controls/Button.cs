using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class Button : TextControl, IButton
    {
        public Button()
        {
            ReboundTime = 10;
            ReboundCountdown = 0;

            Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Yellow);
            Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.Lime);
            Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Lime);
            ContentLayout = ContentLayout.Centered;

            RightClick += Button_RightClick;
            BeforeFrame += Button_BeforeFrame;
        }

        public int ReboundTime { get; set; }

        public int ReboundCountdown { get; private set; }

        private void Button_RightClick(Point position)
        {
            if (!IsSelected)
            {
                IsSelected = true;
                ReboundCountdown = ReboundTime;
            }
        }
        
        private void Button_BeforeFrame()
        {
            if (IsSelected)
            {
                if (ReboundCountdown <= 0)
                    IsSelected = false;
                ReboundCountdown--;
            }
        }
    }
}
