using QuanLib.Minecraft.Snbt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class LeftRightKeys
    {
        public LeftRightKeys(InteractionData leftClick, InteractionData rightClick)
        {
            LeftClick = leftClick;
            RightClick = rightClick;
        }

        public InteractionData LeftClick { get; }

        public InteractionData RightClick { get; }
    }
}
