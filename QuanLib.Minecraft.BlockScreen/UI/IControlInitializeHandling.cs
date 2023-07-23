using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IControlInitializeHandling
    {
        public bool InitializeCompleted { get; }

        public void HandleInitialize();

        public void HandleOnInitCompleted1();

        public void HandleOnInitCompleted2();

        public void HandleOnInitCompleted3();

        public void HandleAllInitialize()
        {
            HandleInitialize();
            HandleOnInitCompleted1();
            HandleOnInitCompleted2();
            HandleOnInitCompleted3();
        }
    }
}
