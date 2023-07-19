using QuanLib.Minecraft.BlockScreen.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps
{
    public class ProcessItem : Control
    {
        public ProcessItem(Process process)
        {
            _process = process ?? throw new ArgumentNullException(nameof(process));

            Icon_PictureBox = new();
            Name_Label = new();
        }

        private readonly Process _process;

        private readonly PictureBox Icon_PictureBox;

        private readonly Label Name_Label;

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(Icon_PictureBox);
            Icon_PictureBox.BorderWidth = 0;
            Icon_PictureBox.ClientSize = new(16, 16);
            Icon_PictureBox.ResizeOptions.Size = Icon_PictureBox.ClientSize;
            Icon_PictureBox.SetImage(_process.ApplicationInfo.Icon);
            Icon_PictureBox.ClientLocation = new(0, 0);

            SubControls.Add(Name_Label);
            Name_Label.Text = _process.ApplicationInfo.Name;
            Name_Label.ClientLocation = this.RightLayout(Icon_PictureBox, 0);
            RightToBorder = Name_Label.RightLocation;
        }
    }
}
