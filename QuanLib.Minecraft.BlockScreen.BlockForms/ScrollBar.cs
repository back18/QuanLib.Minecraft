using QuanLib.Minecraft.Block;
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

            Skin.ForegroundBlockID = BlockManager.Concrete.LightGray;
            Skin.ForegroundBlockID_Selected = BlockManager.Concrete.LightGray;
            Skin.ForegroundBlockID_Hover = BlockManager.Concrete.LightBlue;
            Skin.ForegroundBlockID_Hover_Selected = BlockManager.Concrete.LightBlue;
        }

        public double SliderSize
        {
            get => _SliderSize;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;

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
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;

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
