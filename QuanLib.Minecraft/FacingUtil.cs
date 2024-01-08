using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public static class FacingUtil
    {
        public static string ToChineseString(this Facing facing)
        {
            return facing switch
            {
                Facing.Xp => "东",
                Facing.Xm => "西",
                Facing.Yp => "上",
                Facing.Ym => "下",
                Facing.Zp => "南",
                Facing.Zm => "北",
                _ => throw new InvalidOperationException(),
            };
        }

        public static string ToEnglishString(this Facing facing)
        {
            return facing switch
            {
                Facing.Xp => "east",
                Facing.Xm => "west",
                Facing.Yp => "up",
                Facing.Ym => "down",
                Facing.Zp => "south",
                Facing.Zm => "north",
                _ => throw new InvalidOperationException(),
            };
        }

        public static Facing Reverse(this Facing facing)
        {
            return facing switch
            {
                Facing.Xp => Facing.Xm,
                Facing.Xm => Facing.Xp,
                Facing.Yp => Facing.Ym,
                Facing.Ym => Facing.Yp,
                Facing.Zp => Facing.Zm,
                Facing.Zm => Facing.Zp,
                _ => throw new InvalidOperationException(),
            };
        }

        public static Facing LeftRotate(this Facing facing, Facing axis)
        {
            return axis switch
            {
                Facing.Xp => XpLeftRotate(facing),
                Facing.Xm => XmLeftRotate(facing),
                Facing.Yp => YpLeftRotate(facing),
                Facing.Ym => YmLeftRotate(facing),
                Facing.Zp => ZpLeftRotate(facing),
                Facing.Zm => ZmLeftRotate(facing),
                _ => throw new InvalidOperationException(),
            };
        }

        public static Facing RightRotate(this Facing facing, Facing axis)
        {
            return axis switch
            {
                Facing.Xp => XpRightRotate(facing),
                Facing.Xm => XmRightRotate(facing),
                Facing.Yp => YpRightRotate(facing),
                Facing.Ym => YmRightRotate(facing),
                Facing.Zp => ZpRightRotate(facing),
                Facing.Zm => ZmRightRotate(facing),
                _ => throw new InvalidOperationException(),
            };
        }

        private static Facing XpLeftRotate(Facing facing)
        {
            return facing switch
            {
                Facing.Yp => Facing.Zm,
                Facing.Zm => Facing.Ym,
                Facing.Ym => Facing.Zp,
                Facing.Zp => Facing.Yp,
                _ => throw new InvalidOperationException(),
            };
        }

        private static Facing XpRightRotate(Facing facing)
        {
            return facing switch
            {
                Facing.Yp => Facing.Zp,
                Facing.Zp => Facing.Ym,
                Facing.Ym => Facing.Zm,
                Facing.Zm => Facing.Yp,
                _ => throw new InvalidOperationException(),
            };
        }

        private static Facing YpLeftRotate(Facing facing)
        {
            return facing switch
            {
                Facing.Xp => Facing.Zp,
                Facing.Zp => Facing.Xm,
                Facing.Xm => Facing.Zm,
                Facing.Zm => Facing.Xp,
                _ => throw new InvalidOperationException(),
            };
        }

        private static Facing YpRightRotate(Facing facing)
        {
            return facing switch
            {
                Facing.Xp => Facing.Zm,
                Facing.Zm => Facing.Xm,
                Facing.Xm => Facing.Zp,
                Facing.Zp => Facing.Xp,
                _ => throw new InvalidOperationException(),
            };
        }

        private static Facing ZpLeftRotate(Facing facing)
        {
            return facing switch
            {
                Facing.Yp => Facing.Xp,
                Facing.Xp => Facing.Ym,
                Facing.Ym => Facing.Xm,
                Facing.Xm => Facing.Yp,
                _ => throw new InvalidOperationException(),
            };
        }

        private static Facing ZpRightRotate(Facing facing)
        {
            return facing switch
            {
                Facing.Yp => Facing.Xm,
                Facing.Xm => Facing.Ym,
                Facing.Ym => Facing.Xp,
                Facing.Xp => Facing.Yp,
                _ => throw new InvalidOperationException(),
            };
        }

        private static Facing XmLeftRotate(Facing facing) => XpRightRotate(facing);

        private static Facing XmRightRotate(Facing facing) => XpLeftRotate(facing);

        private static Facing YmLeftRotate(Facing facing) => YpRightRotate(facing);

        private static Facing YmRightRotate(Facing facing) => YpLeftRotate(facing);

        private static Facing ZmLeftRotate(Facing facing) => ZpRightRotate(facing);

        private static Facing ZmRightRotate(Facing facing) => ZpLeftRotate(facing);
    }
}
