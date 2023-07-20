using QuanLib.Minecraft.BlockScreen;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public abstract class Form : ControlContainer<Control>, IApplicationComponent
    {
        protected Form()
        {
            AllowSelected = true;
            AllowDeselected = true;
            AllowResize = true;
            IsOnResize = false;
            ResizeBorder = PlaneFacing.None;
        }

        public bool AllowSelected { get; set; }

        public bool AllowDeselected { get; set; }

        public bool AllowResize { get; set; }

        public bool IsOnResize { get; internal set; }

        public PlaneFacing ResizeBorder { get; private set; }

        public bool IsMaximize => Location == MaximizeLocation && Width == MaximizeSize.Width && Height == MaximizeSize.Height;

        public Point MaximizeLocation => new(0, 0);

        public Size MaximizeSize => GetMCOS().FormsPanelSize;

        public Point RestoreLocation { get; private set; }

        public Size RestoreSize { get; private set; }

        public Application Application
        {
            get
            {
                if (_Application is null)
                    throw new InvalidOperationException();
                return _Application;
            }
            internal set => _Application = value;
        }
        private Application? _Application;

        public bool IsInitialize => _Application is not null;

        public override void Initialize()
        {
            base.Initialize();

            MCOS os = GetMCOS();
            Text = Application.AppName;
            Width = os.FormsPanelSize.Width;
            Height = os.FormsPanelSize.Height;
            InvokeExternalCursorMove = true;
            CursorMove += Form_CursorMove;
            OnMove += Form_OnMove;
            OnResize += Form_OnResize;
            InitializeCallback += Form_InitializeCallback;
        }

        private void Form_InitializeCallback()
        {
            if (IsMaximize)
            {
                RestoreSize = ClientSize * 2 / 3;
                RestoreLocation = new(Width / 2 - RestoreSize.Width / 2, Height / 2 - RestoreSize.Height / 2);
            }
            else
            {
                RestoreSize = ClientSize;
                RestoreLocation = ClientLocation;
            }
        }

        private void Form_CursorMove(Point position, CursorMode mode)
        {
            Point parent = SubPos2ParentPos(position);
            if (AllowResize && IsSelected)
            {
                if (IsOnResize)
                {
                    if (ResizeBorder.HasFlag(PlaneFacing.Top))
                        TopLocation = parent.Y;
                    if (ResizeBorder.HasFlag(PlaneFacing.Bottom))
                        BottomLocation = parent.Y;
                    if (ResizeBorder.HasFlag(PlaneFacing.Left))
                        LeftLocation = parent.X;
                    if (ResizeBorder.HasFlag(PlaneFacing.Right))
                        RightLocation = parent.X;
                }
                else
                {
                    ResizeBorder = PlaneFacing.None;
                    if (parent.Y >= TopLocation - 2 &&
                        parent.X >= LeftLocation - 2 &&
                        parent.Y <= BottomLocation + 2 &&
                        parent.X <= RightLocation + 2)
                    {
                        if (parent.Y <= TopLocation + 2)
                            ResizeBorder |= PlaneFacing.Top;
                        if (parent.X <= LeftLocation + 2)
                            ResizeBorder |= PlaneFacing.Left;
                        if (parent.Y >= BottomLocation - 2)
                            ResizeBorder |= PlaneFacing.Bottom;
                        if (parent.X >= RightLocation - 2)
                            ResizeBorder |= PlaneFacing.Right;
                    }

                    GetMCOS().CursorType = ResizeBorder switch
                    {
                        PlaneFacing.Top or PlaneFacing.Bottom => CursorType.VerticalResize,
                        PlaneFacing.Left or PlaneFacing.Right => CursorType.HorizontalResize,
                        PlaneFacing.Left | PlaneFacing.Top or PlaneFacing.Right | PlaneFacing.Bottom => CursorType.LeftObliqueResize,
                        PlaneFacing.Right | PlaneFacing.Top or PlaneFacing.Left | PlaneFacing.Bottom => CursorType.RightObliqueResize,
                        _ => CursorType.Default,
                    };
                }
            }
        }

        protected virtual void Form_OnMove(Point oldPosition, Point newPosition)
        {
            if (!IsMaximize)
            {
                RestoreLocation = ClientLocation ;
                RestoreSize = ClientSize;
            }
        }

        protected virtual void Form_OnResize(Size oldSize, Size newSize)
        {
            if (!IsMaximize)
            {
                RestoreLocation = ClientLocation;
                RestoreSize = ClientSize;
            }
        }

        public void MaximizeForm()
        {
            Size maximize = MaximizeSize;
            Width = maximize.Width;
            Height = maximize.Height;
            Location = new(0, 0);
        }

        public void RestoreForm()
        {
            ClientLocation = RestoreLocation;
            ClientSize = RestoreSize;
        }

        public virtual void CloseForm()
        {
            ClearAllControlSyncer();
            Application.Exit();
        }
    }
}
