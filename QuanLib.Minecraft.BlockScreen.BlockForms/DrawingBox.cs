﻿using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.Event;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class DrawingBox : ScalablePictureBox
    {
        public DrawingBox()
        {
            EnableDrag = false;
            Drawing = false;
            _PenWidth = 1;

            _undos = new();
            _redos = new();

            LastCursorPosition = new(0, 0);
        }

        private static readonly Point InvalidPosition = new(-1, -1);

        private readonly Stack<Image<Rgba32>> _undos;

        private readonly Stack<Image<Rgba32>> _redos;

        private Point LastCursorPosition;

        public bool EnableDraw { get; set; }

        public bool Drawing { get; private set; }

        public int PenWidth
        {
            get => _PenWidth;
            set
            {
                if (value < 1)
                    value = 1;
                _PenWidth = value;
            }
        }
        private int _PenWidth;

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            if (EnableDraw)
            {
                if (Drawing)
                {
                    Drawing = false;
                }
                else
                {
                    Drawing = true;
                    _undos.Push(ImageFrame.Image.Clone());
                    ClearRedoStack();
                }
            }
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            if (!Drawing)
            {
                LastCursorPosition = InvalidPosition;
                return;
            }
            else if (LastCursorPosition == InvalidPosition)
            {
                LastCursorPosition = e.Position;
                return;
            }

            var item = GetScreenContext()?.Screen.InputHandler.CurrentItem;
            if (item is not null && MinecraftResourcesManager.BlockTextureManager.TryGetValue(item.ID, out var texture))
            {
                ImageFrame.Image.Mutate(ctx =>
                {
                    var pen = new Pen(texture.AverageColors[GetScreenPlaneSize().NormalFacing], PenWidth);
                    pen.JointStyle = JointStyle.Round;
                    pen.EndCapStyle = EndCapStyle.Round;
                    Point position1 = ClientPos2ImagePos(Rectangle, LastCursorPosition);
                    Point position2 = ClientPos2ImagePos(Rectangle, e.Position);
                    ctx.DrawLines(pen, new PointF[] { position1, position2 });
                });
                ImageFrame.Update(Rectangle);
                RequestUpdateFrame();
            }

            LastCursorPosition = e.Position;
        }

        protected override void OnCursorEnter(Control sender, CursorEventArgs e)
        {
            base.OnCursorEnter(sender, e);

            var context = GetScreenContext();
            if (context is not null)
                context.IsShowCursor = false;
        }

        protected override void OnCursorLeave(Control sender, CursorEventArgs e)
        {
            base.OnCursorLeave(sender, e);

            var context = GetScreenContext();
            if (context is not null)
                context.IsShowCursor = true;
        }

        protected override void OnImageFrameChanged(PictureBox sender, ImageFrameChangedEventArgs e)
        {
            base.OnImageFrameChanged(sender, e);

            ClearUndoStack();
            ClearRedoStack();
        }

        public void Clear()
        {
            _undos.Push(ImageFrame.Image.Clone());
            ClearRedoStack();

            Rgba32 color = GetBlockAverageColor(BlockManager.Concrete.White);
            ImageFrame.Image.Mutate(ctx => ctx.BackgroundColor(color).Fill(color));
            ImageFrame.Update(Rectangle);
            RequestUpdateFrame();
        }

        public void Undo()
        {
            if (_undos.Count > 0)
            {
                _redos.Push(ImageFrame.Image);
                ImageFrame.Image = _undos.Pop();
                ImageFrame.Update(Rectangle);
                RequestUpdateFrame();
            }
        }

        public void Redo()
        {
            if (_redos.Count > 0)
            {
                _undos.Push(ImageFrame.Image);
                ImageFrame.Image = _redos.Pop();
                ImageFrame.Update(Rectangle);
                RequestUpdateFrame();
            }
        }

        private void ClearUndoStack()
        {
            while (_undos.Count > 0)
            {
                _undos.Pop().Dispose();
            }
        }

        private void ClearRedoStack()
        {
            while (_redos.Count > 0)
            {
                _redos.Pop().Dispose();
            }
        }
    }
}