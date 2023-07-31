﻿using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer
{
    public class VideoPlayerForm : WindowForm
    {
        public VideoPlayerForm()
        {
            Read_Button = new();
            Path_TextBox = new();
            Video_VideoPlayer = new();
        }

        private readonly BlockForms.VideoPlayer Video_VideoPlayer;

        private readonly Button Read_Button;

        private readonly TextBox Path_TextBox;

        public override void Initialize()
        {
            base.Initialize();

            Client_Panel.Resize += Client_Panel_OnResize;

            Client_Panel.SubControls.Add(Video_VideoPlayer);
            Video_VideoPlayer.Visible = false;
            Video_VideoPlayer.VideoBox.ResizeOptions.Size = Client_Panel.ClientSize;
            Video_VideoPlayer.Stretch = Direction.Bottom | Direction.Right;

            Client_Panel.SubControls.Add(Read_Button);
            Read_Button.Text = "读取";
            Read_Button.ClientLocation = Client_Panel.LifeLayout(null, Read_Button, 2, 2);
            Read_Button.Anchor = Direction.Top | Direction.Right;
            Read_Button.RightClick += Read_Button_RightClick;

            Client_Panel.SubControls.Add(Path_TextBox);
            Path_TextBox.ClientLocation = new(2, 2);
            Path_TextBox.Width = Client_Panel.ClientSize.Width - Read_Button.Width - 6;
            Path_TextBox.Stretch = Direction.Right;
            Path_TextBox.TextEditorChanged += Path_TextBox_TextEditorChanged;
        }

        private void Read_Button_RightClick(Control sender, CursorEventArgs e)
        {
            if (Video_VideoPlayer.VideoBox.TryReadMediaFile(Path_TextBox.Text))
            {
                Read_Button.Visible = false;
                Path_TextBox.Visible = false;
                Video_VideoPlayer.Visible = true;
                Video_VideoPlayer.VideoBox.Play();
            }
            else
            {
                Path_TextBox.Text = "无法读取";
            }
        }

        private void Path_TextBox_TextEditorChanged(Control sender, CursorTextEventArgs e)
        {
            if (MCOS.DefaultFont.GetTotalSize(e.Text).Width > Path_TextBox.ClientSize.Width)
                Path_TextBox.ContentAnchor = AnchorPosition.UpperRight;
            else
                Path_TextBox.ContentAnchor = AnchorPosition.UpperLeft;
        }

        private void Client_Panel_OnResize(Control sender, SizeChangedEventArgs e)
        {
            Video_VideoPlayer.VideoBox.ResizeOptions.Size = e.NewSize;
        }

        public override void CloseForm()
        {
            Video_VideoPlayer.VideoBox.Dispose();
            base.CloseForm();
        }
    }
}
