using QuanLib.Minecraft.BlockScreen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public interface IApplicationComponent
    {
        public Application Application { get; }

        public void SetApplication(Application application);
    }
}
