using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Services
{
    public class ServicesApp : ServicesApplication
    {
        public ServicesApp()
        {
            RootForm = new ServicesForm();
        }

        public const string ID = "Services";

        public const string Name = "系统服务";

        public override IRootForm RootForm { get; }

        public override object? Main(string[] args)
        {
            RunForm(RootForm);
            return null;
        }
    }
}
