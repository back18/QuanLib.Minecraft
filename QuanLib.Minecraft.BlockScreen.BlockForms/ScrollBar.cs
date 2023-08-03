using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class ScrollBar : Control
    {
        protected ScrollBar()
        {
            _SliderSize = 1;
            _SliderPosition = 0;

            Skin.ForegroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.LightGray);
            Skin.ForegroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.LightGray);
            Skin.ForegroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Skin.ForegroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
        }

        public double SliderSize
        {
            get => _SliderSize;
            set
            {
                if (_SliderSize != value)
                {
                    _SliderSize = value;
                    RequestUpdateFrame();
                }
            }
        }
        private double _SliderSize;

        public double SliderPosition
        {
            get => _SliderPosition;
            set
            {
                if ( _SliderPosition != value)
                {
                    _SliderPosition = value;
                    RequestUpdateFrame();
                }
            }
        }
        private double _SliderPosition;
    }
}
