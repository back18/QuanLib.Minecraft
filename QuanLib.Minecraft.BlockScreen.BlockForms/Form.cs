using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class Form : ContainerControl<Control>, IForm
    {
        protected Form()
        {
            AllowSelected = true;
            AllowDeselected = true;
            AllowMove = true;
            AllowResize = true;
            Moveing = false;
            Resizeing = false;
            ResizeBorder = Direction.None;

            OnFormClose += (obj) => { };
        }

        public bool AllowSelected { get; set; }

        public bool AllowDeselected { get; set; }

        public bool AllowMove { get; set; }

        public bool AllowResize { get; set; }

        public bool Moveing { get; set; }

        public bool Resizeing { get; set; }

        public Direction ResizeBorder { get; private set; }

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
            private set => _Application = value;
        }
        private Application? _Application;

        public bool ApplicationIsNotNull => _Application is not null;

        public event Action<IForm> OnFormClose;

        public override void Initialize()
        {
            base.Initialize();

            MCOS os = GetMCOS();
            Text = GetProcess().ApplicationInfo.Name;
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
            if (AllowResize && IsSelected && !IsMaximize)
            {
                if (Resizeing)
                {
                    if (ResizeBorder.HasFlag(Direction.Top))
                        TopLocation = parent.Y;
                    if (ResizeBorder.HasFlag(Direction.Bottom))
                        BottomLocation = parent.Y;
                    if (ResizeBorder.HasFlag(Direction.Left))
                        LeftLocation = parent.X;
                    if (ResizeBorder.HasFlag(Direction.Right))
                        RightLocation = parent.X;
                }
                else
                {
                    ResizeBorder = Direction.None;
                    if (parent.Y >= TopLocation - 2 &&
                        parent.X >= LeftLocation - 2 &&
                        parent.Y <= BottomLocation + 2 &&
                        parent.X <= RightLocation + 2)
                    {
                        if (parent.Y <= TopLocation + 2)
                            ResizeBorder |= Direction.Top;
                        if (parent.X <= LeftLocation + 2)
                            ResizeBorder |= Direction.Left;
                        if (parent.Y >= BottomLocation - 2)
                            ResizeBorder |= Direction.Bottom;
                        if (parent.X >= RightLocation - 2)
                            ResizeBorder |= Direction.Right;
                    }

                    GetMCOS().CursorType = ResizeBorder switch
                    {
                        Direction.Top or Direction.Bottom => CursorType.VerticalResize,
                        Direction.Left or Direction.Right => CursorType.HorizontalResize,
                        Direction.Left | Direction.Top or Direction.Right | Direction.Bottom => CursorType.LeftObliqueResize,
                        Direction.Right | Direction.Top or Direction.Left | Direction.Bottom => CursorType.RightObliqueResize,
                        _ => CursorType.Default,
                    };
                }
            }
        }

        protected virtual void Form_OnMove(Point oldPosition, Point newPosition)
        {
            if (!IsMaximize)
            {
                RestoreLocation = ClientLocation;
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
            ClearAllLayoutSyncer();
            OnFormClose.Invoke(this);
        }

        void IApplicationComponent.SetApplication(Application application)
        {
            Application = application;
        }
    }
}
