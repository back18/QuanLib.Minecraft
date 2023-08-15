using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class ProgressBar : Control
    {
        public double Progress
        {
            get => _Progress;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;

                if (_Progress != value)
                {
                    _Progress = value;
                    RequestUpdateFrame();
                }
            }
        }
        private double _Progress;
    }
}
