using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Desktop
{
    public class DesktopIcon : ContainerControl<Control>
    {
        public DesktopIcon(ApplicationInfo appInfo)
        {
            _appInfo = appInfo;

            Icon_PictureBox = new();
            Name_Label = new();

            BorderWidth = 0;
            ClientSize = new(24, 24);
            Skin.BackgroundBlockID = string.Empty;
            Skin.BackgroundBlockID_Hover = BlockManager.Concrete.White;
            Skin.BackgroundBlockID_Selected = BlockManager.Concrete.LightBlue;
            Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.Blue;
        }

        private readonly PictureBox Icon_PictureBox;

        private readonly Label Name_Label;

        private readonly ApplicationInfo _appInfo;

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(Icon_PictureBox);
            Icon_PictureBox.ClientLocation = new(3, 3);
            Icon_PictureBox.ClientSize = new(16, 16);
            Icon_PictureBox.DefaultResizeOptions.Size = Icon_PictureBox.ClientSize;
            Icon_PictureBox.SetImage(_appInfo.Icon);

            Name_Label.BorderWidth = 1;
            Name_Label.Text = _appInfo.Name;
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            if (ParentContainer?.AsControlCollection<Control>()?.Contains(Name_Label) ?? false)
            {
                Point parent = this.SubPos2ParentPos(e.Position);
                parent.Y += 5;
                Name_Label.ClientLocation = parent;
                if (Name_Label.BottomToBorder < 0)
                {
                    parent = Name_Label.ClientLocation;
                    parent.Y -= Name_Label.Height;
                    parent.Y -= 9;
                    Name_Label.ClientLocation = parent;
                }
                if (Name_Label.RightToBorder < 0)
                {
                    parent = Name_Label.ClientLocation;
                    parent.X += Name_Label.RightToBorder;
                    Name_Label.ClientLocation = parent;
                }
            }
        }

        protected override void OnCursorEnter(Control sender, CursorEventArgs e)
        {
            base.OnCursorEnter(sender, e);

            ParentContainer?.AsControlCollection<Control>()?.TryAdd(Name_Label);
        }

        protected override void OnCursorLeave(Control sender, CursorEventArgs e)
        {
            base.OnCursorLeave(sender, e);

            ParentContainer?.AsControlCollection<Control>()?.Remove(Name_Label);
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            IsSelected = !IsSelected;
        }

        protected override void OnDoubleRightClick(Control sender, CursorEventArgs e)
        {
            base.OnDoubleRightClick(sender, e);

            MCOS.GetMCOS().ProcessManager.ProcessList.Add(_appInfo, GetForm());
            ParentContainer?.AsControlCollection<Control>()?.ClearSelecteds();
        }
    }
}
