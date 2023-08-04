using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.DataScreen
{
    public class DataScreenApp : Application
    {
        public const string ID = "DataScreen";

        public const string Name = "数据大屏";

        public override object? Main(string[] args)
        {
            RunForm(new DataScreenForm());
            return null;
        }
    }
}
