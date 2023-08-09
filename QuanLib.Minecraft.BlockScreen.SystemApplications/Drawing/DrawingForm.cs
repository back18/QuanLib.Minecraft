using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
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
            Draw_Switch = new();
            Zoom_Switch = new();
            Drag_Switch = new();
            PenWidth_NumberBox = new();
            MoreMenu_Switch = new();
            More_MenuBox = new();
            Undo_Button = new();
            Redo_Button = new();
            FillButton = new();
            Create_Button = new();
            Open_Button = new();
            Save_Button = new();
            DrawingBox = new();
        }

        private readonly Switch Draw_Switch;

        private readonly Switch Zoom_Switch;

        private readonly Switch Drag_Switch;

        private readonly NumberBox PenWidth_NumberBox;

        private readonly Switch MoreMenu_Switch;

        private readonly ListMenuBox<Button> More_MenuBox;

        private readonly Button Undo_Button;

        private readonly Button Redo_Button;

        private readonly Button FillButton;

        private readonly Button Create_Button;

        private readonly Button Open_Button;

        private readonly Button Save_Button;

        private readonly DrawingBox DrawingBox;

        public override void Initialize()
        {
            base.Initialize();

            ClientPanel.SubControls.Add(Draw_Switch);
            Draw_Switch.Text = "绘制";
            Draw_Switch.ClientLocation = ClientPanel.LeftLayout(null, Draw_Switch, 1, 1);
            Draw_Switch.Anchor = Direction.Top | Direction.Right;
            Draw_Switch.ControlSelected += Draw_Switch_ControlSelected;
            Draw_Switch.ControlDeselected += Draw_Switch_ControlDeselected;
            Draw_Switch.IsSelected = true;

            ClientPanel.SubControls.Add(Zoom_Switch);
            Zoom_Switch.Text = "缩放";
            Zoom_Switch.ClientLocation = ClientPanel.BottomLayout(Draw_Switch, 1);
            Zoom_Switch.Anchor = Direction.Top | Direction.Right;
            Zoom_Switch.ControlSelected += Zoom_Switch_ControlSelected;
            Zoom_Switch.ControlDeselected += Zoom_Switch_ControlDeselected;

            ClientPanel.SubControls.Add(Drag_Switch);
            Drag_Switch.Text = "拖拽";
            Drag_Switch.ClientLocation = ClientPanel.BottomLayout(Zoom_Switch, 1);
            Drag_Switch.Anchor = Direction.Top | Direction.Right;
            Drag_Switch.ControlSelected += Drag_Switch_ControlSelected;
            Drag_Switch.ControlDeselected += Drag_Switch_ControlDeselected;

            ClientPanel.SubControls.Add(PenWidth_NumberBox);
            PenWidth_NumberBox.Skin.SetAllBackgroundBlockID(BlockManager.Concrete.Pink);
            PenWidth_NumberBox.MinNumberValue = 1;
            PenWidth_NumberBox.ClientLocation = ClientPanel.BottomLayout(Drag_Switch, 1);
            PenWidth_NumberBox.Anchor = Direction.Top | Direction.Right;
            PenWidth_NumberBox.NumberValueChanged += PenWidth_NumberBox_NumberValueChanged;
            PenWidth_NumberBox.NumberValue = 5;

            ClientPanel.SubControls.Add(MoreMenu_Switch);
            MoreMenu_Switch.Skin.BackgroundBlockID = MoreMenu_Switch.Skin.BackgroundBlockID_Hover = BlockManager.Concrete.Yellow;
            MoreMenu_Switch.Skin.BackgroundBlockID_Selected = MoreMenu_Switch.Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.Orange;
            MoreMenu_Switch.OffText = "更多";
            MoreMenu_Switch.OnText = "隐藏";
            MoreMenu_Switch.ClientLocation = ClientPanel.BottomLayout(PenWidth_NumberBox, 1);
            MoreMenu_Switch.Anchor = Direction.Top | Direction.Right;
            MoreMenu_Switch.ControlSelected += MoreMenu_Switch_ControlSelected;
            MoreMenu_Switch.ControlDeselected += MoreMenu_Switch_ControlDeselected; ;

            More_MenuBox.Size = new(45, MoreMenu_Switch.BottomLocation);
            More_MenuBox.Skin.SetAllBackgroundBlockID(string.Empty);
            More_MenuBox.Anchor = Direction.Top | Direction.Right;

            More_MenuBox.AddedSubControlAndLayout(Undo_Button);
            Undo_Button.Text = "撤销";
            Undo_Button.Skin.BackgroundBlockID = string.Empty;
            Undo_Button.RightClick += Undo_Button_RightClick;

            More_MenuBox.AddedSubControlAndLayout(Redo_Button);
            Redo_Button.Text = "重做";
            Redo_Button.Skin.BackgroundBlockID = string.Empty;
            Redo_Button.RightClick += Redo_Button_RightClick;

            More_MenuBox.AddedSubControlAndLayout(FillButton);
            FillButton.Text = "填充";
            FillButton.Skin.BackgroundBlockID = string.Empty;
            FillButton.RightClick += Fill_Button_RightClick;

            More_MenuBox.AddedSubControlAndLayout(Create_Button);
            Create_Button.Text = "新建";
            Create_Button.Skin.BackgroundBlockID = string.Empty;
            Create_Button.RightClick += Create_Button_RightClick;

            More_MenuBox.AddedSubControlAndLayout(Open_Button);
            Open_Button.Text = "打开";
            Open_Button.Skin.BackgroundBlockID = string.Empty;
            Open_Button.RightClick += Open_Button_RightClick;

            More_MenuBox.AddedSubControlAndLayout(Save_Button);
            Save_Button.Text = "保存";
            Save_Button.Skin.BackgroundBlockID = string.Empty;
            Save_Button.RightClick += Save_Button_RightClick;

            ClientPanel.SubControls.Add(DrawingBox);
            DrawingBox.ClientLocation = new(1, 1);
            DrawingBox.Size = new(ClientPanel.ClientSize.Width - Draw_Switch.Width - 3, ClientPanel.ClientSize.Height - 2);
            DrawingBox.Stretch = Direction.Bottom | Direction.Right;
            DrawingBox.SetImage(DrawingBox.CreateImage(DrawingBox.ClientSize, BlockManager.Concrete.White));
        }

        private void Draw_Switch_ControlSelected(Control sender, EventArgs e)
        {
            DrawingBox.EnableDraw = true;
        }

        private void Draw_Switch_ControlDeselected(Control sender, EventArgs e)
        {
            DrawingBox.EnableDraw = false;
        }

        private void Zoom_Switch_ControlSelected(Control sender, EventArgs e)
        {
            DrawingBox.EnableZoom = true;
        }

        private void Zoom_Switch_ControlDeselected(Control sender, EventArgs e)
        {
            DrawingBox.EnableZoom = false;
        }

        private void Drag_Switch_ControlSelected(Control sender, EventArgs e)
        {
            DrawingBox.EnableDrag = true;
        }

        private void Drag_Switch_ControlDeselected(Control sender, EventArgs e)
        {
            DrawingBox.EnableDrag = false;
        }

        private void PenWidth_NumberBox_NumberValueChanged(NumberBox sender, QuanLib.Event.ValueChangedEventArgs<int> e)
        {
            DrawingBox.PenWidth = e.NewValue;
        }

        private void MoreMenu_Switch_ControlSelected(Control sender, EventArgs e)
        {
            ClientPanel.SubControls.TryAdd(More_MenuBox);
            More_MenuBox.ClientLocation = new(sender.LeftLocation - More_MenuBox.Width - 1, sender.BottomLocation - More_MenuBox.Height + 1);
        }

        private void MoreMenu_Switch_ControlDeselected(Control sender, EventArgs e)
        {
            ClientPanel.SubControls.Remove(More_MenuBox);
        }

        private void Undo_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            DrawingBox.Undo();
        }

        private void Redo_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            DrawingBox.Redo();
        }

        private void Fill_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            DrawingBox.Fill();
        }

        private void Create_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            DrawingBox.SetImage(DrawingBox.CreateImage(16, 16, string.Empty));
        }

        private void Open_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            DrawingBox.TryReadImageFile("new.png");
        }

        private void Save_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            DrawingBox.ImageFrame.Image.Save("new.png");
        }
    }
}
