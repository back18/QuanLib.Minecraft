using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class ScrollablePanel : Panel<Control>
    {
        public ScrollablePanel()
        {
            FirstHandleCursorSlotChanged = true;

            ScrollDelta = 16;
            _PageSize = new(0, 0);
            ScrollBarShowTime = 20;
            ScrollBarHideTime = 0;

            VerticalScrollBar = new();
            HorizontalScrollBar = new();

            PageSizeChanged += OnPageSizeChanged;
        }

        private readonly VerticalScrollBar VerticalScrollBar;

        private readonly HorizontalScrollBar HorizontalScrollBar;

        public bool ShowVerticalScrollBar => ClientSize.Height < PageSize.Height;

        public bool ShowHorizontalScrollBar => ClientSize.Width < PageSize.Width;

        public int ScrollBarShowTime { get; set; }

        public int ScrollBarHideTime { get; private set; }

        public int ScrollDelta { get; set; }

        public Size PageSize
        {
            get => _PageSize;
            set
            {
                if (_PageSize != value)
                {
                    Size temp = _PageSize;
                    _PageSize = value;
                    PageSizeChanged.Invoke(this, new(temp, _PageSize));
                    RequestUpdateFrame();
                }
            }
        }
        private Size _PageSize;

        public event EventHandler<ScrollablePanel, SizeChangedEventArgs> PageSizeChanged;

        protected virtual void OnPageSizeChanged(ScrollablePanel sender, SizeChangedEventArgs e)
        {
            if (e.OldSize.Height != e.NewSize.Height)
                RefreshVerticalScrollBar();
            if (e.OldSize.Width != e.NewSize.Width)
                RefreshHorizontalScrollBar();
        }

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(VerticalScrollBar);
            VerticalScrollBar.Visible = false;
            VerticalScrollBar.KeepWhenClear = true;
            VerticalScrollBar.DisplayPriority = 16;
            VerticalScrollBar.MaxDisplayPriority = 32;
            VerticalScrollBar.Width = 8;
            VerticalScrollBar.LayoutSyncer = new(this, (sender, e) => { }, (sender, e) => { });
            VerticalScrollBar.RightClick += VerticalScrollBar_RightClick;

            SubControls.Add(HorizontalScrollBar);
            HorizontalScrollBar.Visible = false;
            HorizontalScrollBar.KeepWhenClear = true;
            HorizontalScrollBar.DisplayPriority = 16;
            HorizontalScrollBar.MaxDisplayPriority = 32;
            HorizontalScrollBar.Height = 8;
            HorizontalScrollBar.LayoutSyncer = new(this, (sender, e) => { }, (sender, e) => { });
            HorizontalScrollBar.RightClick += HorizontalScrollBar_RightClick;
        }

        protected override void OnInitializeCompleted(Control sender, EventArgs e)
        {
            base.OnInitializeCompleted(sender, e);

            RefreshVerticalScrollBar();
            RefreshHorizontalScrollBar();
        }

        public override IFrame RenderingFrame()
        {
            return ArrayFrame.BuildFrame(GetRenderingSize(), Skin.GetBackgroundBlockID());
        }

        protected override void OnResize(Control sender, SizeChangedEventArgs e)
        {
            Size oldSize = e.OldSize;
            Size newSize = e.NewSize;

            if (oldSize.Height < PageSize.Height)
                oldSize.Height = PageSize.Height;
            if (oldSize.Width < PageSize.Width)
                oldSize.Width = PageSize.Width;

            if (newSize.Height < PageSize.Height)
            {
                newSize.Height = PageSize.Height;
                RefreshVerticalScrollBar();
            }
            else
            {
                HideScrollBar();
            }

            if (newSize.Width < PageSize.Width)
            {
                newSize.Width = PageSize.Width;
                RefreshHorizontalScrollBar();
            }
            else
            {
                HideScrollBar();
            }

            base.OnResize(sender, new(oldSize, newSize));
        }

        protected override void OnCursorSlotChanged(Control sender, CursorSlotEventArgs e)
        {
            base.OnCursorSlotChanged(sender, e);

            Point offset;
            int delte = e.Delta * ScrollDelta;
            if (ShowVerticalScrollBar)
            {
                offset = new(OffsetPosition.X, OffsetPosition.Y + delte);
                int max = Math.Max(ClientSize.Height, PageSize.Height) - ClientSize.Height;
                if (offset.Y < 0)
                    offset.Y = 0;
                else if (offset.Y > max)
                    offset.Y = max;
            }
            else if (ShowHorizontalScrollBar)
            {
                offset = new(OffsetPosition.X + delte, OffsetPosition.Y);
                int max = Math.Max(ClientSize.Width, PageSize.Width) - ClientSize.Width;
                if (offset.X < 0)
                    offset.X = 0;
                else if (offset.X > max)
                    offset.X = max;
            }
            else
            {
                return;
            }

            OffsetPosition = offset;
        }

        protected override void OnOffsetPositionChanged(Control sender, PositionChangedEventArgs e)
        {
            base.OnOffsetPositionChanged(sender, e);

            if (e.OldPosition.Y != e.NewPosition.Y)
                RefreshVerticalScrollBar();
            if (e.OldPosition.X != e.NewPosition.X)
                RefreshHorizontalScrollBar();
        }

        private void VerticalScrollBar_RightClick(Control sender, CursorEventArgs e)
        {
            if (ShowVerticalScrollBar)
            {
                double position = (double)e.Position.Y / VerticalScrollBar.ClientSize.Height - VerticalScrollBar.SliderSize / 2;
                int max = Math.Max(ClientSize.Height, PageSize.Height) - ClientSize.Height;
                Point offset = new(OffsetPosition.X, (int)Math.Round(PageSize.Height * position));
                if (offset.Y < 0)
                    offset.Y = 0;
                else if (offset.Y > max)
                    offset.Y = max;
                OffsetPosition = offset;
                RefreshVerticalScrollBar();
            }
        }

        private void HorizontalScrollBar_RightClick(Control sender, CursorEventArgs e)
        {
            if (ShowHorizontalScrollBar)
            {
                double position = (double)e.Position.X / HorizontalScrollBar.ClientSize.Width - HorizontalScrollBar.SliderSize / 2;
                int max = Math.Max(ClientSize.Width, PageSize.Width) - ClientSize.Width;
                Point offset = new((int)Math.Round(PageSize.Width * position), OffsetPosition.Y);
                if (offset.X < 0)
                    offset.X = 0;
                else if (offset.X > max)
                    offset.X = max;
                OffsetPosition = offset;
                RefreshHorizontalScrollBar();
            }
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            ShowScrollBar();
        }

        protected override void OnBeforeFrame(Control sender, EventArgs e)
        {
            base.OnBeforeFrame(sender, e);

            if (VerticalScrollBar.Visible == true ||  HorizontalScrollBar.Visible == true)
            {
                if (ScrollBarHideTime <= 0 && !VerticalScrollBar.IsHover && !HorizontalScrollBar.IsHover)
                {
                    HideScrollBar();
                }
                ScrollBarHideTime--;
            }
        }

        public void ShowScrollBar()
        {
            if (ShowVerticalScrollBar)
                VerticalScrollBar.Visible = true;
            if (ShowHorizontalScrollBar)
                HorizontalScrollBar.Visible = true;

            ScrollBarHideTime = ScrollBarShowTime;
        }

        public void HideScrollBar()
        {
            VerticalScrollBar.Visible = false;
            HorizontalScrollBar.Visible = false;
        }

        public void RefreshVerticalScrollBar()
        {
            if (ShowVerticalScrollBar)
            {
                VerticalScrollBar.SliderSize = (double)ClientSize.Height / PageSize.Height;
                VerticalScrollBar.SliderPosition = (double)OffsetPosition.Y / PageSize.Height;
                VerticalScrollBar.Height = ClientSize.Height;
                if (ShowHorizontalScrollBar)
                    VerticalScrollBar.Height -= HorizontalScrollBar.Height - 2;

                VerticalScrollBar.ClientLocation = new Point(ClientSize.Width - VerticalScrollBar.Width + OffsetPosition.X + 1, OffsetPosition.Y);
                HorizontalScrollBar.ClientLocation = new Point(OffsetPosition.X, ClientSize.Height - HorizontalScrollBar.Height + OffsetPosition.Y + 1);

                ShowScrollBar();
            }
        }

        public void RefreshHorizontalScrollBar()
        {
            if (ShowHorizontalScrollBar)
            {
                HorizontalScrollBar.SliderSize = (double)ClientSize.Width / PageSize.Width;
                HorizontalScrollBar.SliderPosition = (double)OffsetPosition.X / PageSize.Width;
                HorizontalScrollBar.Width = ClientSize.Width;
                if (ShowVerticalScrollBar)
                    HorizontalScrollBar.Width -= VerticalScrollBar.Width - 2;

                VerticalScrollBar.ClientLocation = new Point(ClientSize.Width - VerticalScrollBar.Width + OffsetPosition.X + 1, OffsetPosition.Y);
                HorizontalScrollBar.ClientLocation = new Point(OffsetPosition.X, ClientSize.Height - HorizontalScrollBar.Height + OffsetPosition.Y + 1);

                ShowScrollBar();
            }
        }

        public Size GetRenderingSize()
        {
            return new(Math.Max(ClientSize.Width, PageSize.Width), Math.Max(ClientSize.Height, PageSize.Height));
        }
    }
}
