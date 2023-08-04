using QuanLib.Minecraft.BlockScreen.BlockForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Drawing
{
    public class DrawingForm : WindowForm
    {
        public DrawingForm()
        {
            DrawingBox = new();
        }

        private readonly DrawingBox DrawingBox;

        public override void Initialize()
        {
            base.Initialize();

            ClientPanel.SubControls.Add(DrawingBox);
            DrawingBox.ClientLocation = new(16, 16);
        }
    }
}
