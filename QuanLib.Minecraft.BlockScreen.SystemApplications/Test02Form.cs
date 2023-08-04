using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications
{
    public class Test02Form : WindowForm
    {
        public Test02Form()
        {
            button1 = new();
            comboButton1 = new();
            textBox1 = new();
            label1 = new();
            switch1 = new();
            switch2 = new();
            iconTextBox1 = new();
        }

        private readonly Button button1;

        private readonly ComboButton<Direction> comboButton1;

        private readonly TextBox textBox1;

        private readonly Label label1;

        private readonly Switch switch1;

        private readonly Switch switch2;

        private readonly IconTextBox iconTextBox1;

        public override void Initialize()
        {
            base.Initialize();

            string dir = PathManager.SystemResources_Textures_Control_Dir;

            ClientLocation = new(3, 3);
            BorderWidth = 8;
            Width = GetFormContainerSize().Width - 7;
            Height = GetFormContainerSize().Height - 10;

            ClientPanel.SubControls.Add(button1);
            button1.ClientLocation = new(5, 5);
            button1.Text = "Open";
            button1.RightClick += Button1_RightClick;
            button1.CursorMove += Button1_CursorMove;

            ClientPanel.SubControls.Add(textBox1);
            textBox1.ClientLocation = new(5, 25);
            textBox1.Text = "Text";

            ClientPanel.SubControls.Add(label1);
            label1.ClientLocation = new(5, 45);
            label1.Text = "lab";

            ClientPanel.SubControls.Add(switch1);
            switch1.ClientLocation = new(5, 65);

            ClientPanel.SubControls.Add(switch2);
            switch2.ClientLocation = new(5, 85);
            switch2.OnText = string.Empty;
            switch2.OffText = string.Empty;
            switch2.Skin.BackgroundImage = new(Path.Combine(dir, "OFF.png"), GetScreenPlaneSize().NormalFacing, switch2.ClientSize);
            switch2.Skin.BackgroundImage_Selected = new(Path.Combine(dir, "ON.png"), GetScreenPlaneSize().NormalFacing, switch2.ClientSize);
            switch2.Skin.BackgroundImage_Hover = new(Path.Combine(dir, "OFF过度.png"), GetScreenPlaneSize().NormalFacing, switch2.ClientSize);
            switch2.Skin.BackgroundImage_Hover_Selected = new(Path.Combine(dir, "ON过度.png"), GetScreenPlaneSize().NormalFacing, switch2.ClientSize);

            ClientPanel.SubControls.Add(comboButton1);
            comboButton1.ClientSize = new(110, 16);
            comboButton1.ClientLocation = ClientPanel.RightLayout(button1, 2, 3);
            comboButton1.Title = "方向";
            comboButton1.Items.Add(Direction.None);
            comboButton1.Items.Add(Direction.Top);
            comboButton1.Items.Add(Direction.Bottom);
            comboButton1.Items.Add(Direction.Right);
            comboButton1.Items.Add(Direction.Left);

            ClientPanel.SubControls.Add(iconTextBox1);
            iconTextBox1.Icon = new(Path.Combine(dir, "Start_ON.png"), GetScreenPlaneSize().NormalFacing, new Size(16, 16));
            iconTextBox1.Text = "hello";
            iconTextBox1.ClientLocation = ClientPanel.RightLayout(textBox1, 2);
        }

        private void Button1_CursorMove(Control sender, Event.CursorEventArgs e)
        {
            Console.WriteLine(e.Position);
        }

        private void Button1_RightClick(Control sender, Event.CursorEventArgs e)
        {
            Console.WriteLine("点击");
        }
    }
}
