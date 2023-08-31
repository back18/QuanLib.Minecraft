using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox;

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
            numberBox1 = new();
        }

        private readonly Button button1;

        private readonly ComboButton<Direction> comboButton1;

        private readonly TextBox textBox1;

        private readonly Label label1;

        private readonly Switch switch1;

        private readonly Switch switch2;

        private readonly IconTextBox iconTextBox1;

        private readonly NumberBox numberBox1;

        public override void Initialize()
        {
            base.Initialize();

            ClientLocation = new(3, 3);
            BorderWidth = 8;
            Width = GetFormContainerSize().Width - 7;
            Height = GetFormContainerSize().Height - 10;

            ClientPanel.SubControls.Add(button1);
            button1.ClientLocation = new(5, 5);
            button1.Text = "Test";
            button1.RightClick += Button1_RightClick;

            ClientPanel.SubControls.Add(textBox1);
            textBox1.ClientLocation = new(5, 25);
            textBox1.Text = "Text";

            ClientPanel.SubControls.Add(label1);
            label1.ClientLocation = new(5, 45);
            label1.Text = "lab";

            ClientPanel.SubControls.Add(switch1);
            switch1.ClientLocation = new(5, 65);

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
            iconTextBox1.Icon_PictureBox.SetImage(TextureManager.GetTexture("Start"));
            iconTextBox1.Text_Label.Text = "Minedows";
            iconTextBox1.AutoSetSize();
            iconTextBox1.ClientLocation = ClientPanel.RightLayout(textBox1, 2);

            ClientPanel.SubControls.Add(numberBox1);
            numberBox1.ClientSize = new(48, 16);
            numberBox1.ClientLocation = ClientPanel.RightLayout(label1, 2);
        }

        private void Button1_RightClick(Control sender, Event.CursorEventArgs e)
        {
            _ = DialogBoxHelper.OpenMessageBoxAsync(this, "崩溃测试", "点击“是”系统将立刻崩溃", MessageBoxButtons.Yes | MessageBoxButtons.No | MessageBoxButtons.Cancel, (result) =>
            {
                if (result == MessageBoxButtons.Yes)
                    MCOS.Instance.AddTask(() => throw new Exception("手动触发崩溃"));
            });
        }

        protected override void OnLeftClick(Control sender, CursorEventArgs e)
        {
            base.OnLeftClick(sender, e);

            Console.WriteLine("左键测试");
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            Console.WriteLine("右键测试");
        }
    }
}
