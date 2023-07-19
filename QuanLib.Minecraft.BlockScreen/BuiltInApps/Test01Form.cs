using Microsoft.VisualBasic;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.Controls;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class Test01Form : WindowForm
    {
        public Test01Form()
        {
            TextBox1 = new();
            TextBox2 = new();
            TextBox3 = new();
            Button1 = new();
            Button2 = new();
            Button3 = new();
        }

        private readonly TextBox TextBox1;

        private readonly TextBox TextBox2;

        private readonly TextBox TextBox3;

        private readonly Button Button1;

        private readonly Button Button2;

        private readonly Button Button3;

        public override void Initialize()
        {
            base.Initialize();

            ClientLocation = new(3, 3);
            Width = Application.MCOS.FormsPanelSize.Width - 7;
            Height = Application.MCOS.FormsPanelSize.Height - 10;

            Client_Panel.SubControls.Add(TextBox1);
            Client_Panel.SubControls.Add(TextBox2);
            Client_Panel.SubControls.Add(TextBox3);
            Client_Panel.SubControls.Add(Button1);
            Client_Panel.SubControls.Add(Button2);
            Client_Panel.SubControls.Add(Button3);

            TextBox1.ClientLocation = new(2, 2);
            TextBox1.ClientSize = new(88, 16);
            TextBox1.Text = "TextBox1";

            TextBox2.ClientLocation = new(2, 25);
            TextBox2.ClientSize = new(88, 16);
            TextBox2.Text = "TextBox2";

            TextBox3.ClientLocation = new(2, 48);
            TextBox3.ClientSize = new(88, 16);
            TextBox3.Text = "TextBox3";

            Button1.ClientLocation = new(93, 2);
            Button1.ClientSize = new(16, 16);
            Button1.Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.Pink);
            Button1.Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Yellow);
            Button1.Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.Red);
            Button1.Text = "X";
            Button1.RightClick += Button1_RightClick;

            Button2.ClientLocation = new(93, 25);
            Button2.ClientSize = new(16, 16);
            Button2.Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.Pink);
            Button2.Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Yellow);
            Button2.Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.Red);
            Button2.Text = "X";
            Button2.RightClick += Button2_RightClick;

            Button3.ClientLocation = new(93, 48);
            Button3.ClientSize = new(16, 16);
            Button3.Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.Pink);
            Button3.Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Yellow);
            Button3.Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.Red);
            Button3.Text = "X";
            Button3.RightClick += Button3_RightClick;
        }

        private void Button3_RightClick(Point obj)
        {
            TextBox3.Text = string.Empty;
        }

        private void Button2_RightClick(Point obj)
        {
            TextBox2.Text = string.Empty;
        }

        private void Button1_RightClick(Point obj)
        {
            TextBox1.Text = string.Empty;
        }

        public override Frame RenderingFrame()
        {
            FrameBuilder fb = new(Frame.BuildFrame(Width, Height, ConcretePixel.ToBlockID(MinecraftColor.Lime)));
            CorrectSize(fb);
            return fb.ToFrame();
        }
    }
}
