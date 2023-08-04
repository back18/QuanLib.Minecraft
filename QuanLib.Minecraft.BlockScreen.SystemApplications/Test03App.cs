using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications
{
    public class Test03App : Application
    {
        public const string ID = "Test03";

        public const string Name = "测试03";

        public override object? Main(string[] args)
        {
            RunForm(new Test03Form());
            return null;
        }
    }
}
