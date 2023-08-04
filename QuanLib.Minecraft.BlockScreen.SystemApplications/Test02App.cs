using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications
{
    public class Test02App : Application
    {
        public const string ID = "Test02";

        public const string Name = "测试02";

        public override object? Main(string[] args)
        {
            RunForm(new Test02Form());
            return null;
        }
    }
}
