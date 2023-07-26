﻿using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class VideoPlayer : ContainerControl<Control>
    {
        public VideoPlayer()
        {
            VideoBox = new();
            PauseOrResume_Switch = new();
            ProgressBar_VideoProgressBar = new(this);
            TimeText_VideoTimeTextBox = new(this);

            OverlayLayerShowTime = 20;
            OverlayLayerCountdown = 0;
        }

        public readonly VideoBox VideoBox;

        private readonly Switch PauseOrResume_Switch;

        private readonly VideoProgressBar ProgressBar_VideoProgressBar;

        private readonly VideoTimeTextBox TimeText_VideoTimeTextBox;

        public int OverlayLayerShowTime { get; set; }

        public int OverlayLayerCountdown { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            MCOS os = GetMCOS();
            string dir = PathManager.SystemResources_Textures_Control_Dir;

            CursorMove += VideoPlayer_CursorMove;
            RightClick += VideoPlayer_RightClick;
            BeforeFrame += VideoPlayer_BeforeFrame;
            OnSelected += VideoPlayer_OnSelected;
            OnDeselected += VideoPlayer_OnDeselected;

            SubControls.Add(VideoBox);
            VideoBox.BorderWidth = 0;
            VideoBox.DisplayPriority = -16;
            VideoBox.MaxDisplayPriority = -15;
            VideoBox.OnResume += VideoBox_OnResume;
            VideoBox.OnPause += VideoBox_OnPause;
            VideoBox.OnEndedPlay += VideoBox_OnEndedPlay;

            SubControls.Add(PauseOrResume_Switch);
            PauseOrResume_Switch.Visible = false;
            PauseOrResume_Switch.BorderWidth = 0;
            PauseOrResume_Switch.ClientSize = new(16, 16);
            PauseOrResume_Switch.ClientLocation = this.CenterLayout(PauseOrResume_Switch);
            PauseOrResume_Switch.LayoutSyncer = new(this, (oldPosition, newPosition) => { }, (oldSize, newSize) =>
            PauseOrResume_Switch.ClientLocation = this.CenterLayout(PauseOrResume_Switch));
            ImageFrame pause = new(Path.Combine(dir, "暂停.png"), os.Screen.NormalFacing, PauseOrResume_Switch.ClientSize);
            ImageFrame resume = new(Path.Combine(dir, "播放.png"), os.Screen.NormalFacing, PauseOrResume_Switch.ClientSize);
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.None, resume);
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Hover, resume);
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Selected, pause);
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Hover | ControlState.Selected, pause);
            PauseOrResume_Switch.IsSelected = VideoBox.PlayerState == VideoPlayerState.Playing;
            PauseOrResume_Switch.OnSelected += PauseOrResume_Switch_OnSelected;
            PauseOrResume_Switch.OnDeselected += PauseOrResume_Switch_OnDeselected;

            SubControls.Add(ProgressBar_VideoProgressBar);
            ProgressBar_VideoProgressBar.Visible = false;
            ProgressBar_VideoProgressBar.Height = 6;
            ProgressBar_VideoProgressBar.Width = ClientSize.Width - 4;
            ProgressBar_VideoProgressBar.ClientLocation = this.TopLayout(null, ProgressBar_VideoProgressBar, 2, 2);
            ProgressBar_VideoProgressBar.Anchor = Direction.Bottom | Direction.Left;
            ProgressBar_VideoProgressBar.Stretch = Direction.Left;

            SubControls.Add(TimeText_VideoTimeTextBox);
            TimeText_VideoTimeTextBox.Visible = false;
            TimeText_VideoTimeTextBox.ClientLocation = this.TopLayout(ProgressBar_VideoProgressBar, TimeText_VideoTimeTextBox, 2);
            TimeText_VideoTimeTextBox.Anchor = Direction.Bottom | Direction.Left;
        }

        private void VideoPlayer_RightClick(Point position)
        {
            if (IsSelected && !SubControls.HaveHover)
            {
                IsSelected = false;
                OverlayLayerCountdown = 0;
            }
            else
            {
                IsSelected = true;
                OverlayLayerCountdown = OverlayLayerShowTime;
            }
        }

        private void VideoPlayer_CursorMove(Point position, CursorMode mode)
        {
            IsSelected = true;
            OverlayLayerCountdown = OverlayLayerShowTime;
        }

        private void VideoPlayer_BeforeFrame()
        {
            if (IsSelected && !SubControls.HaveHover)
            {
                if (OverlayLayerCountdown <= 0)
                    IsSelected = false;
                OverlayLayerCountdown--;
            }
        }

        private void VideoPlayer_OnSelected()
        {
            PauseOrResume_Switch.Visible = true;
            ProgressBar_VideoProgressBar.Visible = true;
            TimeText_VideoTimeTextBox.Visible = true;
        }

        private void VideoPlayer_OnDeselected()
        {
            PauseOrResume_Switch.Visible = false;
            ProgressBar_VideoProgressBar.Visible = false;
            TimeText_VideoTimeTextBox.Visible = false;
        }

        private void VideoBox_OnResume()
        {
            PauseOrResume_Switch.IsSelected = true;
        }

        private void VideoBox_OnPause()
        {
            PauseOrResume_Switch.IsSelected = false;
        }

        private void VideoBox_OnEndedPlay()
        {
            PauseOrResume_Switch.IsSelected = false;
        }

        private void PauseOrResume_Switch_OnSelected()
        {
            VideoBox.Resume();
        }

        private void PauseOrResume_Switch_OnDeselected()
        {
            VideoBox.Pause();
        }

        public static string FromTimeSpan(TimeSpan time)
        {
            string result = string.Empty;
            if (time.Hours > 0)
                result += time.Hours.ToString().PadLeft(2, '0') + ':';
            result += $"{time.Minutes.ToString().PadLeft(2, '0')}:{time.Seconds.ToString().PadLeft(2, '0')}";
            return result;
        }
    }
}