using QuanLib.Minecraft.SNBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.SNBT
{
    public class LeftRightKeys
    {
        public LeftRightKeys(InteractionData leftClick, InteractionData rightClick)
        {
            ArgumentNullException.ThrowIfNull(leftClick, nameof(leftClick));
            ArgumentNullException.ThrowIfNull(rightClick, nameof(rightClick));

            LeftClick = leftClick;
            RightClick = rightClick;
        }

        public InteractionData LeftClick { get; }

        public InteractionData RightClick { get; }
    }
}
