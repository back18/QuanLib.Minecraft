using QuanLib.Minecraft.BlockScreen.UI.Controls;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Desktop
{
    public class DesktopIcon : ContainerControl<Control>
    {
        public DesktopIcon(ApplicationInfo appInfo)
        {
            _appInfo = appInfo;

            Icon_PictureBox = new();
            Name_Label = new();

            BorderWidth = 0;
            ClientSize = new(26, 26);
            Skin.BackgroundBlockID = string.Empty;
            Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.White);
            Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Blue);
            CursorEnter += DesktopIcon_CursorEnter;
            CursorLeave += DesktopIcon_CursorLeave;
            CursorMove += DesktopIcon_CursorMove;
            RightClick += DesktopIcon_RightClick;
            DoubleRightClick += DesktopIcon_DoubleRightClick;
        }

        private readonly PictureBox Icon_PictureBox;

        private readonly Label Name_Label;

        private readonly ApplicationInfo _appInfo;

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(Icon_PictureBox);
            Icon_PictureBox.ClientLocation = new(4, 4);
            Icon_PictureBox.ClientSize = new(16, 16);
            Icon_PictureBox.ResizeOptions.Size = Icon_PictureBox.ClientSize;
            Icon_PictureBox.SetImage(_appInfo.Icon);

            Name_Label.BorderWidth = 1;
            Name_Label.Text = _appInfo.Name;
        }

        private void DesktopIcon_CursorEnter(Point position, CursorMode mode)
        {
            ParentContainer?.AsControlCollection<Control>()?.TryAdd(Name_Label);
        }

        private void DesktopIcon_CursorLeave(Point position, CursorMode mode)
        {
            ParentContainer? .AsControlCollection<Control>()?.Remove(Name_Label);
        }

        private void DesktopIcon_CursorMove(Point position, CursorMode mode)
        {
            if (ParentContainer?.AsControlCollection<Control>()?.Contains(Name_Label) ?? false)
            {
                Point parent = SubPos2ParentPos(position);
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

        private void DesktopIcon_RightClick(Point position)
        {
            IsSelected = !IsSelected;
        }

        private void DesktopIcon_DoubleRightClick(Point position)
        {
            GetMCOS().RunApp(_appInfo.ID, Array.Empty<string>(), GetProcess());
            ParentContainer?.AsControlCollection<Control>()?.ClearSelected();
        }
    }
}
