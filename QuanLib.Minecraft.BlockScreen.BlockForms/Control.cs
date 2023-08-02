using Newtonsoft.Json.Linq;
using QuanLib;
using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.Data;
using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Screens;
using QuanLib.Event;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    /// <summary>
    /// 控件
    /// </summary>
    public abstract class Control : IControl
    {
        protected Control()
        {
            InvokeExternalCursorMove = false;
            _LayoutSyncer = null;
            ParentContainer = null;
            LastRightClickTime = DateTime.MinValue;
            LastLeftClickTime = DateTime.MinValue;
            _DisplayPriority = 0;
            _MaxDisplayPriority = int.MaxValue;
            _Text = string.Empty;
            _Visible = true;
            _ClientLocation = new(0, 0);
            _ClientSize = new(SystemResourcesManager.DefaultFont.HalfWidth * 4, SystemResourcesManager.DefaultFont.Height);
            _AutoSize = false;
            _BorderWidth = 1;
            Skin = new(this);
            Anchor = Direction.Top | Direction.Left;
            Stretch = Direction.None;
            _ContentAnchor = AnchorPosition.UpperLeft;
            _ControlState = ControlState.None;

            Frame_Update = true;
            Frame_Old = null;
            Text_Update = false;
            Text_Old = Text;
            ClientLocation_Update = false;
            ClientLocation_Old = ClientLocation;
            ClientSize_Update = false;
            ClientSize_Old = ClientSize;

            CursorMove += OnCursorMove;
            CursorEnter += OnCursorEnter;
            CursorLeave += OnCursorLeave;
            RightClick += OnRightClick;
            LeftClick += OnLeftClick;
            DoubleRightClick += OnDoubleRightClick;
            DoubleLeftClick += OnDoubleLeftClick;
            CursorSlotChanged += OnCursorSlotChanged;
            CursorItemChanged += OnCursorItemChanged;
            TextEditorChanged += OnTextEditorChanged;
            BeforeFrame += OnBeforeFrame;
            AfterFrame += OnAfterFrame;
            InitializeCallback += OnInitializeCallback;
            ControlSelected += OnControlSelected;
            ControlDeselected += OnControlDeselected;
            TextChanged += OnTextChanged;
            Move += OnMove;
            Resize += OnResize;
            TextChangedNow += OnTextChangedNow;
            MoveNow += OnMoveNow;
            ResizeNow += OnResizeNow;
            Layout += OnLayout;
        }

        private bool Frame_Update;

        private ArrayFrame? Frame_Old;

        private bool Text_Update;

        private string Text_Old;

        private bool ClientLocation_Update;

        private Point ClientLocation_Old;

        private bool ClientSize_Update;

        private Size ClientSize_Old;

        public IContainerControl? GenericParentContainer { get; private set; }

        public ContainerControl? ParentContainer { get; private set; }

        public int ParentBorderWidth => ParentContainer?.BorderWidth ?? 0;

        public int Index => ParentContainer?.GetSubControls().IndexOf(this) ?? -1;

        public DateTime LastRightClickTime { get; private set; }

        public DateTime LastLeftClickTime { get; private set; }

        public bool InvokeExternalCursorMove { get; set; }

        public bool InitializeCompleted { get; private set; }

        public string Text
        {
            get => _Text;
            set
            {
                if (_Text != value)
                {
                    string temp = _Text;
                    _Text = value;
                    Text_Update = true;
                    TextChangedNow.Invoke(this, new(temp, _Text));
                    RequestUpdateFrame();
                }
            }
        }
        private string _Text;

        #region 位置与尺寸

        public Point ClientLocation
        {
            get => _ClientLocation;
            set
            {
                if (_ClientLocation != value)
                {
                    Point temp = _ClientLocation;
                    _ClientLocation = value;
                    ClientLocation_Update = true;
                    MoveNow.Invoke(this, new(temp, _ClientLocation));
                    RequestUpdateFrame();
                }
            }
        }
        private Point _ClientLocation;

        public Size ClientSize
        {
            get => _ClientSize;
            set
            {
                if (_ClientSize != value)
                {
                    Size temp = _ClientSize;
                    _ClientSize = value;
                    ClientSize_Update = true;
                    ResizeNow.Invoke(this, new(temp, _ClientSize));
                    RequestUpdateFrame();
                }
            }
        }
        private Size _ClientSize;

        public Point Location
        {
            get => new(ClientLocation.X + ParentBorderWidth, ClientLocation.Y + ParentBorderWidth);
            set
            {
                ClientLocation = new(value.X - ParentBorderWidth, value.Y - ParentBorderWidth);
            }
        }

        public Size Size
        {
            get => new(ClientSize.Width + BorderWidth * 2, ClientSize.Height * BorderWidth * 2);
            set
            {
                ClientSize = new(value.Width - BorderWidth * 2, value.Height - BorderWidth * 2);
            }
        }

        public int X
        {
            get => ClientLocation.X + ParentBorderWidth;
            set
            {
                ClientLocation = new(value -  ParentBorderWidth, ClientLocation.Y);
            }
        }

        public int Y
        {
            get => ClientLocation.Y + ParentBorderWidth;
            set
            {
                ClientLocation = new(ClientLocation.X, value - ParentBorderWidth);
            }
        }

        public int Width
        {
            get => ClientSize.Width + BorderWidth * 2;
            set
            {
                ClientSize = new(value - BorderWidth * 2, ClientSize.Height);
            }
        }

        public int Height
        {
            get => ClientSize.Height + BorderWidth * 2;
            set
            {
                ClientSize = new(ClientSize.Width, value - BorderWidth * 2);
            }
        }

        public int BorderWidth
        {
            get => _BorderWidth;
            set
            {
                if (_BorderWidth != value)
                {
                    _BorderWidth = value;
                    RequestUpdateFrame();
                }
            }
        }
        private int _BorderWidth;

        public int TopLocation
        {
            get => ClientLocation.Y;
            set
            {
                int offset = TopLocation - value;
                ClientSize = new(ClientSize.Width, ClientSize.Height + offset);
                Location = new(Location.X, Location.Y - offset);
            }
        }

        public int BottomLocation
        {
            get => ClientLocation.Y + Height - 1;
            set
            {
                int offset = value - BottomLocation;
                ClientSize = new(ClientSize.Width, ClientSize.Height + offset);
            }
        }

        public int LeftLocation
        {
            get => ClientLocation.X;
            set
            {
                int offset = LeftLocation - value;
                ClientSize = new(ClientSize.Width + offset, ClientSize.Height);
                Location = new(Location.X - offset, Location.Y);
            }
        }

        public int RightLocation
        {
            get => ClientLocation.X + Width - 1;
            set
            {
                int offset = value - RightLocation;
                ClientSize = new(ClientSize.Width + offset, ClientSize.Height);
            }
        }

        public int TopToBorder
        {
            get => Location.Y - ParentBorderWidth;
            set
            {
                int offset = TopToBorder - value;
                ClientSize = new(ClientSize.Width, ClientSize.Height + offset);
                Location = new(Location.X, Location.Y - offset);
            }
        }

        public int BottomToBorder
        {
            get => (ParentContainer?.Height - ParentBorderWidth ?? GetScreenPlaneSize().Height) - (Location.Y + Height);
            set
            {
                int offset = BottomToBorder - value;
                ClientSize = new(ClientSize.Width, ClientSize.Height + offset);
            }
        }

        public int LeftToBorder
        {
            get => Location.X - ParentBorderWidth;
            set
            {
                int offset = LeftToBorder - value;
                ClientSize = new(ClientSize.Width + offset, ClientSize.Height);
                Location = new(Location.X - offset, Location.Y);
            }
        }

        public int RightToBorder
        {
            get => (ParentContainer?.Width - ParentBorderWidth ?? GetScreenPlaneSize().Width) - (Location.X + Width);
            set
            {
                int offset = RightToBorder - value;
                ClientSize = new(ClientSize.Width + offset, ClientSize.Height);
            }
        }

        public bool AutoSize
        {
            get => _AutoSize;
            set
            {
                if (_AutoSize != value)
                {
                    if (value)
                        AutoSetSize();
                    _AutoSize = value;
                    RequestUpdateFrame();
                }
            }
        }
        private bool _AutoSize;

        #endregion

        #region 外观与布局

        public bool Visible
        {
            get => _Visible;
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    RequestUpdateFrame();
                }
            }
        }
        private bool _Visible;

        public int DisplayPriority
        {
            get
            {
                if (IsSelected)
                    return MaxDisplayPriority;
                else
                    return _DisplayPriority;
            }
            set
            {
                _DisplayPriority = value;
                if (!IsSelected)
                    GenericParentContainer?.GetSubControls().Sort();
            }
        }
        private int _DisplayPriority;

        public int MaxDisplayPriority
        {
            get => _MaxDisplayPriority;
            set
            {
                _MaxDisplayPriority = value;
                if (IsSelected)
                    GenericParentContainer?.GetSubControls().Sort();
            }
        }
        private int _MaxDisplayPriority;

        public ControlSkin Skin { get; }

        /// <summary>
        /// 锚定，大小不变，位置自适应父控件
        /// </summary>
        public Direction Anchor { get; set; }

        /// <summary>
        /// 拉伸，位置不变，大小自适应父控件
        /// </summary>
        public Direction Stretch { get; set; }

        public LayoutMode LayoutMode => LayoutSyncer is null ? LayoutMode.Auto : LayoutMode.Sync;

        public AnchorPosition ContentAnchor
        {
            get => _ContentAnchor;
            set
            {
                if (_ContentAnchor != value)
                {
                    _ContentAnchor = value;
                    RequestUpdateFrame();
                }
            }
        }
        private AnchorPosition _ContentAnchor;

        public ControlContent ControlContent
        {
            get
            {
                ControlContent result = ControlContent.None;
                if (!string.IsNullOrEmpty(Text))
                    result |= ControlContent.Text;
                if (Skin.GetBackgroundImage() is not null)
                    result |= ControlContent.Image;
                return result;
            }
        }

        public ControlState ControlState
        {
            get => _ControlState;
            set
            {
                if (_ControlState != value)
                {
                    _ControlState = value;
                    RequestUpdateFrame();
                }
            }
        }
        private ControlState _ControlState;

        public bool IsHover
        {
            get => ControlState.HasFlag(ControlState.Hover);
            private set
            {
                if (IsHover != value)
                {
                    if (value)
                    {
                        ControlState |= ControlState.Hover;
                    }
                    else
                    {
                        ControlState ^= ControlState.Hover;
                    }
                }
            }
        }

        public bool IsSelected
        {
            get => ControlState.HasFlag(ControlState.Selected);
            set
            {
                if (IsSelected != value)
                {
                    if (value)
                    {
                        ControlState |= ControlState.Selected;
                        ControlSelected.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        ControlState ^= ControlState.Selected;
                        ControlDeselected.Invoke(this, EventArgs.Empty);
                    }
                    GenericParentContainer?.GetSubControls().Sort();
                }
            }
        }

        public LayoutSyncer? LayoutSyncer
        {
            get => _LayoutSyncer;
            set
            {
                _LayoutSyncer?.Unbinding();
                _LayoutSyncer = value;
                _LayoutSyncer?.Binding();
                _LayoutSyncer?.Sync();
            }
        }
        private LayoutSyncer? _LayoutSyncer;

        ISkin IControlRendering.Skin => Skin;

        #endregion

        #region 事件发布

        public event EventHandler<Control, CursorEventArgs> CursorMove;

        public event EventHandler<Control, CursorEventArgs> CursorEnter;

        public event EventHandler<Control, CursorEventArgs> CursorLeave;

        public event EventHandler<Control, CursorEventArgs> RightClick;

        public event EventHandler<Control, CursorEventArgs> LeftClick;

        public event EventHandler<Control, CursorEventArgs> DoubleRightClick;

        public event EventHandler<Control, CursorEventArgs> DoubleLeftClick;

        public event EventHandler<Control, CursorSlotEventArgs> CursorSlotChanged;

        public event EventHandler<Control, CursorItemEventArgs> CursorItemChanged;

        public event EventHandler<Control, CursorTextEventArgs> TextEditorChanged;

        public event EventHandler<Control, EventArgs> BeforeFrame;

        public event EventHandler<Control, EventArgs> AfterFrame;

        public event EventHandler<Control, EventArgs> InitializeCallback;

        public event EventHandler<Control, EventArgs> ControlSelected;

        public event EventHandler<Control, EventArgs> ControlDeselected;

        public event EventHandler<Control, TextChangedEventArgs> TextChanged;

        public event EventHandler<Control, PositionChangedEventArgs> Move;

        public event EventHandler<Control, SizeChangedEventArgs> Resize;

        public event EventHandler<Control, TextChangedEventArgs> TextChangedNow;

        public event EventHandler<Control, PositionChangedEventArgs> MoveNow;

        public event EventHandler<Control, SizeChangedEventArgs> ResizeNow;

        public event EventHandler<Control, SizeChangedEventArgs> Layout;

        #endregion

        #region 事件订阅

        protected virtual void OnCursorMove(Control sender, CursorEventArgs e) { }

        protected virtual void OnCursorEnter(Control sender, CursorEventArgs e) { }

        protected virtual void OnCursorLeave(Control sender, CursorEventArgs e) { }

        protected virtual void OnRightClick(Control sender, CursorEventArgs e) { }

        protected virtual void OnLeftClick(Control sender, CursorEventArgs e) { }

        protected virtual void OnDoubleRightClick(Control sender, CursorEventArgs e) { }

        protected virtual void OnDoubleLeftClick(Control sender, CursorEventArgs e) { }

        protected virtual void OnCursorSlotChanged(Control sender, CursorSlotEventArgs e) { }

        protected virtual void OnCursorItemChanged(Control sender, CursorItemEventArgs e) { }

        protected virtual void OnTextEditorChanged(Control sender, CursorTextEventArgs e) { }

        protected virtual void OnBeforeFrame(Control sender, EventArgs e)
        {
            if (Text_Update)
            {
                if (Text != Text_Old)
                    TextChanged.Invoke(this, new(Text_Old, Text));
                Text_Update = false;
                Text_Old = Text;
            }

            if (ClientLocation_Update)
            {
                if (ClientLocation != ClientLocation_Old)
                    Move.Invoke(this, new(ClientLocation_Old, ClientLocation));
                ClientLocation_Update = false;
                ClientLocation_Old = ClientLocation;
            }

            if (ClientSize_Update)
            {
                if (ClientSize != ClientSize_Old)
                {
                    Resize.Invoke(this, new(ClientSize_Old, ClientSize));
                }
                ClientSize_Update = false;
                ClientSize_Old = ClientSize;
            }
        }

        protected virtual void OnAfterFrame(Control sender, EventArgs e) { }

        protected virtual void OnInitializeCallback(Control sender, EventArgs e) { }

        protected virtual void OnControlSelected(Control sender, EventArgs e) { }

        protected virtual void OnControlDeselected(Control sender, EventArgs e) { }

        protected virtual void OnTextChanged(Control sender, TextChangedEventArgs e) { }

        protected virtual void OnMove(Control sender, PositionChangedEventArgs e) { }

        protected virtual void OnResize(Control sender, SizeChangedEventArgs e) { }

        protected virtual void OnTextChangedNow(Control sender, TextChangedEventArgs e) { }

        protected virtual void OnMoveNow(Control sender, PositionChangedEventArgs e) { }

        protected virtual void OnResizeNow(Control sender, SizeChangedEventArgs e) { }

        public virtual void OnLayout(Control sender, SizeChangedEventArgs e)
        {
            Size offset = e.NewSize - e.OldSize;
            if (offset.Height != 0)
            {
                if (!Anchor.HasFlag(Direction.Top) && !Anchor.HasFlag(Direction.Bottom))
                {
                    double proportion = (ClientLocation.Y + Height / 2.0) / e.OldSize.Height;
                    ClientLocation = new(ClientLocation.X, (int)Math.Round(e.NewSize.Height * proportion - Height / 2.0));
                }
                if (Anchor.HasFlag(Direction.Bottom))
                    ClientLocation = new(ClientLocation.X, ClientLocation.Y + offset.Height);

                if (Stretch.HasFlag(Direction.Top) || Stretch.HasFlag(Direction.Bottom))
                    BottomToBorder -= offset.Height;
            }

            if (offset.Width != 0)
            {
                if (!Anchor.HasFlag(Direction.Left) && !Anchor.HasFlag(Direction.Right))
                {
                    double proportion = (ClientLocation.X + Width / 2.0) / e.OldSize.Width;
                    ClientLocation = new((int)Math.Round(e.NewSize.Width * proportion - Width / 2.0), ClientLocation.Y);
                }
                if (Anchor.HasFlag(Direction.Right))
                    ClientLocation = new(ClientLocation.X + offset.Width, ClientLocation.Y);

                if (Stretch.HasFlag(Direction.Left) || Stretch.HasFlag(Direction.Right))
                    RightToBorder -= offset.Width;
            }
        }

        #endregion

        #region 事件处理

        public virtual void HandleCursorMove(CursorEventArgs e)
        {
            UpdateHoverState(e);

            if (IncludedOnControl(e.Position) || InvokeExternalCursorMove)
            {
                CursorMove.Invoke(this, e);
            }
        }

        public virtual bool HandleRightClick(CursorEventArgs e)
        {
            if (Visible)
            {
                if (IsHover)
                {
                    RightClick.Invoke(this, e);
                    DateTime now = DateTime.Now;
                    if (LastRightClickTime == DateTime.MinValue || (DateTime.Now - LastRightClickTime).TotalMilliseconds > 500)
                    {
                        LastRightClickTime = now;
                    }
                    else
                    {
                        DoubleRightClick.Invoke(this, e);
                        LastRightClickTime = DateTime.MinValue;
                    }
                    return true;
                }
            }
            return false;
        }

        public virtual bool HandleLeftClick(CursorEventArgs e)
        {
            if (Visible)
            {
                if (IsHover)
                {
                    LeftClick.Invoke(this, e);
                    DateTime now = DateTime.Now;
                    if (LastLeftClickTime == DateTime.MinValue || (DateTime.Now - LastLeftClickTime).TotalMilliseconds > 500)
                    {
                        LastLeftClickTime = now;
                    }
                    else
                    {
                        DoubleLeftClick.Invoke(this, e);
                        LastLeftClickTime = DateTime.MinValue;
                    }
                    return true;
                }
            }
            return false;
        }

        public virtual void HandleCursorSlotChanged(CursorSlotEventArgs e)
        {
            if (Visible)
            {
                if (IncludedOnControl(e.Position))
                {
                    CursorSlotChanged.Invoke(this, e);
                }
            }
        }

        public virtual void HandleCursorItemChanged(CursorItemEventArgs e)
        {
            if (Visible)
            {
                if (IncludedOnControl(e.Position))
                    CursorItemChanged.Invoke(this, e);
            }
        }

        public virtual void HandleTextEditorChanged(CursorTextEventArgs e)
        {
            if (Visible)
            {
                if (IncludedOnControl(e.Position))
                    TextEditorChanged.Invoke(this, e);
            }
        }

        public virtual void HandleBeforeFrame(EventArgs e)
        {
            BeforeFrame.Invoke(this, e);
        }

        public virtual void HandleAfterFrame(EventArgs e)
        {
            AfterFrame.Invoke(this, e);
        }

        public virtual void HandleLayout(SizeChangedEventArgs e)
        {
            Layout.Invoke(this, e);
        }

        public virtual void UpdateAllHoverState(CursorEventArgs e)
        {
            UpdateHoverState(e);
        }

        public void UpdateHoverState(CursorEventArgs e)
        {
            bool included = IncludedOnControl(e.Position);
            if (IsHover)
            {
                if (!included)
                {
                    IsHover = false;
                    CursorLeave.Invoke(this, e);
                }
            }
            else
            {
                if (included)
                {
                    Control? control = ParentContainer?.GetSubControls().FirstHover;
                    if (control is not null)
                    {
                        if (control.Index < Index)
                        {
                            control.IsHover = false;
                            control.CursorLeave.Invoke(this, e);
                        }
                        else
                            return;
                    }
                    IsHover = true;
                    CursorEnter.Invoke(this, e);
                }
            }
        }

        #endregion

        #region 初始化

        public virtual void Initialize()
        {
            if (AutoSize)
                AutoSetSize();
        }

        public virtual void OnInitCompleted1()
        {

        }

        public virtual void OnInitCompleted2()
        {

        }

        public virtual void OnInitCompleted3()
        {
            Text_Update = false;
            Text_Old = Text;
            ClientLocation_Update = false;
            ClientLocation_Old = ClientLocation;
            ClientSize_Update = false;
            ClientSize_Old = ClientSize;

            InitializeCompleted = true;
        }

        public virtual void HandleInitialize()
        {
            Initialize();
            InitializeCallback.Invoke(this, EventArgs.Empty);
        }

        public virtual void HandleInitCompleted1()
        {
            OnInitCompleted1();
        }

        public virtual void HandleInitCompleted2()
        {
            OnInitCompleted2();
        }

        public virtual void HandleInitCompleted3()
        {
            OnInitCompleted3();
        }

        #endregion

        #region 位置移动

        public void ToTopMove(int offset)
        {
            Location = new(Location.X, Location.Y - offset);
        }

        public void ToBottomMove(int offset)
        {
            Location = new(Location.X, Location.Y + offset);
        }

        public void ToLeftMove(int offset)
        {
            Location = new(Location.X - offset, Location.Y);
        }

        public void ToRightMove(int offset)
        {
            Location = new(Location.X + offset, Location.Y);
        }

        public void MoveToTop(int distance)
        {
            int offset = TopToBorder - distance;
            ToTopMove(offset);
        }

        public void MoveToBottom(int distance)
        {
            int offset = BottomToBorder - distance;
            ToBottomMove(offset);
        }

        public void MoveToLeft(int distance)
        {
            int offset = LeftToBorder - distance;
            ToLeftMove(offset);
        }

        public void MoveToRight(int distance)
        {
            int offset = RightToBorder - distance;
            ToRightMove(offset);
        }

        #endregion

        #region 帧渲染处理

        protected void RequestUpdateFrame()
        {
            Frame_Update = true;
            ParentContainer?.RequestUpdateFrame();
        }

        public virtual IFrame RenderingFrame()
        {
            return ArrayFrame.BuildFrame(ClientSize.Width, ClientSize.Height, Skin.GetBackgroundBlockID());
        }

        bool IControlRendering.NeedRendering()
        {
            return Frame_Update;
        }

        ArrayFrame? IControlRendering.GetFrameCache()
        {
            return Frame_Old;
        }

        void IControlRendering.OnRenderingCompleted(ArrayFrame frame)
        {
            Frame_Update = false;
            Frame_Old = frame;
        }

        #endregion

        #region 父级相关处理

        /// <summary>
        /// 顺序从根控件到子控件，不包括当前控件
        /// </summary>
        /// <returns></returns>
        public Control[] GetParentControls()
        {
            List<Control> result = new();
            Control? parent = ParentContainer;
            while (parent is not null)
            {
                result.Add(parent);
                parent = parent.ParentContainer;
            }
            result.Reverse();
            return result.ToArray();
        }

        public Control GetRootControl()
        {
            Control result = this;
            while (true)
            {
                Control? parent = result.ParentContainer;
                if (parent is null)
                    return result;
                else
                    result = parent;
            }
        }

        public RootForm? GetRootForm()
        {
            IControl? result = this;
            while (true)
            {
                if (result is null)
                    return null;
                else if (result is RootForm form)
                    return form;
                else
                    result = result.GenericParentContainer;
            }
        }

        public Form? GetForm()
        {
            IControl? result = this;
            while (true)
            {
                if (result is null)
                    return null;
                else if (result is Form form)
                    return form;
                else
                    result = result.GenericParentContainer;
            }
        }

        public Process? GetProcess()
        {
            Form? form = GetForm();
            if (form is null)
                return null;

            return MCOS.GetMCOS().ProcessOf(form);
        }

        public ScreenContext? GetScreenContext()
        {
            Form? form = GetForm();
            if (form is null)
                return null;

            return MCOS.GetMCOS().ScreenContextOf(form);
        }

        public IPlaneSize GetScreenPlaneSize()
        {
            Form? form = GetForm();
            if (form is not null)
            {
                Screen? screen = MCOS.GetMCOS().ScreenContextOf(form)?.Screen;
                if (screen is not null)
                    return screen;

                IForm? initiator = MCOS.GetMCOS().ProcessOf(form)?.Initiator;
                if (initiator is not null)
                {
                    screen = MCOS.GetMCOS().ScreenContextOf(initiator)?.Screen;
                    if (screen is not null)
                        return screen;
                }
            }
           
            return new PlaneSize(128, 72, Facing.Zp);
        }

        public Size GetFormContainerSize()
        {
            IRootForm? rootForm1 = GetRootForm();
            if (rootForm1 is not null)
                return rootForm1.FormContainerSize;

            Form? form = GetForm();
            if (form is not null)
            {
                IForm? initiator = MCOS.GetMCOS().ProcessOf(form)?.Initiator;
                if (initiator is not null)
                {
                    if (initiator is IRootForm rootForm2)
                        return rootForm2.FormContainerSize;

                    IRootForm? rootForm3 = initiator.GetRootForm();
                    if (rootForm3 is not null)
                        return rootForm3.FormContainerSize;
                }
            }
           
            return new Size(128, 56);
        }

        #endregion

        protected void SetTextEditorInitialText()
        {
            Screen? screen = GetScreenContext()?.Screen;
            if (screen is not null)
                screen.InputHandler.InitialText = Text;
        }

        protected void ResetTextEditor()
        {
            GetScreenContext()?.Screen.InputHandler.ResetText();
        }

        public virtual void ClearAllLayoutSyncer()
        {
            LayoutSyncer = null;
        }

        public virtual void AutoSetSize()
        {

        }

        public virtual bool IncludedOnControl(Point position)
        {
            return position.X >= 0 && position.Y >= 0 && position.X < Width && position.Y < Height;
        }

        public Point ScreenPos2ControlPos(Point position)
        {
            Control[] parents = GetParentControls();
            foreach (var parent in parents)
                position = parent.ParentPos2SubPos(position);
            position = ParentPos2SubPos(position);
            return position;
        }

        public Point ParentPos2SubPos(Point position)
        {
            return new(position.X - Location.X, position.Y - Location.Y);
        }

        public Point SubPos2ParentPos(Point position)
        {
            return new(position.X + Location.X, position.Y + Location.Y);
        }

        public override string ToString()
        {
            return $"Type:{GetType().Name}|Text:{Text}|Pos:{ClientLocation.X},{ClientLocation.Y}|Size:{ClientSize.Width},{ClientSize.Height}";
        }

        public int CompareTo(IControl? other)
        {
            return DisplayPriority.CompareTo(other?.DisplayPriority);
        }

        void IControl.SetGenericContainerControl(IContainerControl? container)
        {
            GenericParentContainer = container;
            if (container is null)
                ParentContainer = null;
            else if (container is ContainerControl containerControl)
                ParentContainer = containerControl;
        }

        public class ControlSkin : ISkin
        {
            public ControlSkin(Control owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));

                string black = ConcretePixel.ToBlockID(MinecraftColor.Black);
                string white = ConcretePixel.ToBlockID(MinecraftColor.White);
                string gray = ConcretePixel.ToBlockID(MinecraftColor.Gray);

                _ForegroundBlockID = black;
                _BackgroundBlockID = white;
                _BorderBlockID = gray;
                _ForegroundBlockID_Selected = black;
                _BackgroundBlockID_Selected = white;
                _BorderBlockID_Selected = gray;
                _ForegroundBlockID_Hover = black;
                _BackgroundBlockID_Hover = white;
                _BorderBlockID__Hover = gray;
                _ForegroundBlockID_Hover_Selected = black;
                _BackgroundBlockID_Hover_Selected = white;
                _BorderBlockID_Hover_Selected = gray;
                _BackgroundImage = null;
                _BackgroundImage_Hover = null;
                _BackgroundImage_Selected = null;
                _BackgroundImage_Hover_Selected = null;
            }

            private readonly Control _owner;

            public string ForegroundBlockID
            {
                get => _ForegroundBlockID;
                set
                {
                    _ForegroundBlockID = value;
                    if (_owner.ControlState == ControlState.None)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _ForegroundBlockID;

            public string BackgroundBlockID
            {
                get => _BackgroundBlockID;
                set
                {
                    _BackgroundBlockID = value;
                    if (_owner.ControlState == ControlState.None)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _BackgroundBlockID;

            public string BorderBlockID
            {
                get => _BorderBlockID;
                set
                {
                    _BorderBlockID = value;
                    if (_owner.ControlState == ControlState.None)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _BorderBlockID;

            public string ForegroundBlockID_Hover
            {
                get => _ForegroundBlockID_Hover;
                set
                {
                    _ForegroundBlockID_Hover = value;
                    if (_owner.ControlState == ControlState.Hover)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _ForegroundBlockID_Hover;

            public string BackgroundBlockID_Hover
            {
                get => _BackgroundBlockID_Hover;
                set
                {
                    _BackgroundBlockID_Hover = value;
                    if (_owner.ControlState == ControlState.Hover)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _BackgroundBlockID_Hover;

            public string BorderBlockID__Hover
            {
                get => _BorderBlockID__Hover;
                set
                {
                    _BorderBlockID__Hover = value;
                    if (_owner.ControlState == ControlState.Hover)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _BorderBlockID__Hover;

            public string ForegroundBlockID_Selected
            {
                get => _ForegroundBlockID_Selected;
                set
                {
                    _ForegroundBlockID_Selected = value;
                    if (_owner.ControlState == ControlState.Selected)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _ForegroundBlockID_Selected;

            public string BackgroundBlockID_Selected
            {
                get => _BackgroundBlockID_Selected;
                set
                {
                    _BackgroundBlockID_Selected = value;
                    if (_owner.ControlState == ControlState.Selected)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _BackgroundBlockID_Selected;

            public string BorderBlockID_Selected
            {
                get => _BorderBlockID_Selected;
                set
                {
                    _BorderBlockID_Selected = value;
                    if (_owner.ControlState == ControlState.Selected)
                        _owner.RequestUpdateFrame();
                }
            }
            private string _BorderBlockID_Selected;

            public string ForegroundBlockID_Hover_Selected
            {
                get => _ForegroundBlockID_Hover_Selected;
                set
                {
                    _ForegroundBlockID_Hover_Selected = value;
                    if (_owner.ControlState == (ControlState.Hover | ControlState.Selected))
                        _owner.RequestUpdateFrame();
                }
            }
            private string _ForegroundBlockID_Hover_Selected;

            public string BackgroundBlockID_Hover_Selected
            {
                get => _BackgroundBlockID_Hover_Selected;
                set
                {
                    _BackgroundBlockID_Hover_Selected = value;
                    if (_owner.ControlState == (ControlState.Hover | ControlState.Selected))
                        _owner.RequestUpdateFrame();
                }
            }
            private string _BackgroundBlockID_Hover_Selected;

            public string BorderBlockID_Hover_Selected
            {
                get => _BorderBlockID_Hover_Selected;
                set
                {
                    _BorderBlockID_Hover_Selected = value;
                    if (_owner.ControlState == (ControlState.Hover | ControlState.Selected))
                        _owner.RequestUpdateFrame();
                }
            }
            private string _BorderBlockID_Hover_Selected;

            public ImageFrame? BackgroundImage
            {
                get => _BackgroundImage;
                set
                {
                    _BackgroundImage = value;
                    if (_owner.ControlState == ControlState.None)
                        _owner.RequestUpdateFrame();
                }
            }
            private ImageFrame? _BackgroundImage;

            public ImageFrame? BackgroundImage_Hover
            {
                get => _BackgroundImage_Hover;
                set
                {
                    _BackgroundImage_Hover = value;
                    if (_owner.ControlState == ControlState.Hover)
                        _owner.RequestUpdateFrame();
                }
            }
            private ImageFrame? _BackgroundImage_Hover;

            public ImageFrame? BackgroundImage_Selected
            {
                get => _BackgroundImage_Selected;
                set
                {
                    _BackgroundImage_Selected = value;
                    if (_owner.ControlState == ControlState.Selected)
                        _owner.RequestUpdateFrame();
                }
            }
            public ImageFrame? _BackgroundImage_Selected;

            public ImageFrame? BackgroundImage_Hover_Selected
            {
                get => _BackgroundImage_Hover_Selected;
                set
                {
                    _BackgroundImage_Hover_Selected = value;
                    if (_owner.ControlState == (ControlState.Hover | ControlState.Selected))
                        _owner.RequestUpdateFrame();
                }
            }
            private ImageFrame? _BackgroundImage_Hover_Selected;

            public string GetForegroundBlockID()
            {
                return _owner.ControlState switch
                {
                    ControlState.None => ForegroundBlockID,
                    ControlState.Hover => ForegroundBlockID_Hover,
                    ControlState.Selected => ForegroundBlockID_Selected,
                    ControlState.Hover | ControlState.Selected => ForegroundBlockID_Hover_Selected,
                    _ => throw new InvalidOperationException(),
                };
            }

            public string GetBackgroundBlockID()
            {
                return _owner.ControlState switch
                {
                    ControlState.None => BackgroundBlockID,
                    ControlState.Hover => BackgroundBlockID_Hover,
                    ControlState.Selected => BackgroundBlockID_Selected,
                    ControlState.Hover | ControlState.Selected => BackgroundBlockID_Hover_Selected,
                    _ => throw new InvalidOperationException(),
                };
            }

            public string GetBorderBlockID()
            {
                return _owner.ControlState switch
                {
                    ControlState.None => BorderBlockID,
                    ControlState.Hover => BorderBlockID__Hover,
                    ControlState.Selected => BorderBlockID_Selected,
                    ControlState.Hover | ControlState.Selected => BorderBlockID_Hover_Selected,
                    _ => throw new InvalidOperationException(),
                };
            }

            public ImageFrame? GetBackgroundImage()
            {
                return _owner.ControlState switch
                {
                    ControlState.None => BackgroundImage,
                    ControlState.Hover => BackgroundImage_Hover,
                    ControlState.Selected => BackgroundImage_Selected,
                    ControlState.Hover | ControlState.Selected => BackgroundImage_Hover_Selected,
                    _ => throw new InvalidOperationException(),
                };
            }

            public void SetAllForegroundBlockID(string blockID)
            {
                ForegroundBlockID = blockID;
                ForegroundBlockID_Hover = blockID;
                ForegroundBlockID_Selected = blockID;
                ForegroundBlockID_Hover_Selected = blockID;
            }

            public void SetAllBackgroundBlockID(string blockID)
            {
                BackgroundBlockID = blockID;
                BackgroundBlockID_Hover = blockID;
                BackgroundBlockID_Selected = blockID;
                BackgroundBlockID_Hover_Selected = blockID;
            }

            public void SetAllBorderBlockID(string blockID)
            {
                BorderBlockID = blockID;
                BorderBlockID__Hover = blockID;
                BorderBlockID_Selected = blockID;
                BorderBlockID_Hover_Selected = blockID;
            }

            public void SetAllBackgroundImage(ImageFrame? frame)
            {
                BackgroundImage = frame;
                BackgroundImage_Hover = frame;
                BackgroundImage_Selected = frame;
                BackgroundImage_Hover_Selected = frame;
            }

            public void SetForegroundBlockID(ControlState state, string blockID)
            {
                switch (state)
                {
                    case ControlState.None:
                        ForegroundBlockID = blockID;
                        break;
                    case ControlState.Hover:
                        ForegroundBlockID_Hover = blockID;
                        break;
                    case ControlState.Selected:
                        ForegroundBlockID_Selected = blockID;
                        break;
                    case ControlState.Hover | ControlState.Selected:
                        ForegroundBlockID_Hover_Selected = blockID;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            public void SetBackgroundBlockID(ControlState state, string blockID)
            {
                switch (state)
                {
                    case ControlState.None:
                        BackgroundBlockID = blockID;
                        break;
                    case ControlState.Hover:
                        BackgroundBlockID_Hover = blockID;
                        break;
                    case ControlState.Selected:
                        BackgroundBlockID_Selected = blockID;
                        break;
                    case ControlState.Hover | ControlState.Selected:
                        BackgroundBlockID_Hover_Selected = blockID;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            public void SetBorderBlockID(ControlState state, string blockID)
            {
                switch (state)
                {
                    case ControlState.None:
                        BorderBlockID = blockID;
                        break;
                    case ControlState.Hover:
                        BorderBlockID__Hover = blockID;
                        break;
                    case ControlState.Selected:
                        BorderBlockID_Selected = blockID;
                        break;
                    case ControlState.Hover | ControlState.Selected:
                        BorderBlockID_Hover_Selected = blockID;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            public void SetBackgroundImage(ControlState state, ImageFrame? frame)
            {
                switch (state)
                {
                    case ControlState.None:
                        BackgroundImage = frame;
                        break;
                    case ControlState.Hover:
                        BackgroundImage_Hover = frame;
                        break;
                    case ControlState.Selected:
                        BackgroundImage_Selected = frame;
                        break;
                    case ControlState.Hover | ControlState.Selected:
                        BackgroundImage_Hover_Selected = frame;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
