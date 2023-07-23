using Newtonsoft.Json.Linq;
using QuanLib.BDF;
using QuanLib.Minecraft.Datas;
using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI.Controls
{
    /// <summary>
    /// 控件
    /// </summary>
    public abstract class Control : IControlRendering, IComparer<Control>, IComparable<Control>
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
            _ClientSize = new(MCOS.DefaultFont.HalfWidth * 4, MCOS.DefaultFont.Height);
            _AutoSize = false;
            _BorderWidth = 1;
            Skin = new(this);
            Anchor = PlaneFacing.Top | PlaneFacing.Left;
            Stretch = PlaneFacing.None;
            _ContentAnchor = ContentAnchor.UpperLeft;
            _ControlState = ControlState.None;

            Frame_Update = true;
            Frame_Old = null;
            Text_Update = false;
            Text_Old = Text;
            ClientLocation_Update = false;
            ClientLocation_Old = ClientLocation;
            ClientSize_Update = false;
            ClientSize_Old = ClientSize;

            CursorMove += (arg1, arg2) => { };
            CursorEnter += (arg1, arg2) => { };
            CursorLeave += (arg1, arg2) => { };
            RightClick += (obj) => { };
            LeftClick += (obj) => { };
            DoubleRightClick += (obj) => { };
            DoubleLeftClick += (obj) => { };
            TextEditorUpdate += (arg1, arg2) => { };
            BeforeFrame += Control_BeforeFrame;
            AfterFrame += Control_AfterFrame;
            InitializeCallback += () => { };
            OnSelected += () => { };
            OnDeselected += () => { };
            OnTextUpdate += (arg1, arg2) => { };
            OnMove += Control_OnMove;
            OnResize += Control_OnResize;
            OnTextUpdateNow += (arg1, arg2) => { };
            OnMoveNow += (arg1, arg2) => { };
            OnResizeNow += (arg1, arg2) => { };
        }

        private bool Frame_Update;

        private ArrayFrame? Frame_Old;

        private bool Text_Update;

        private string Text_Old;

        private bool ClientLocation_Update;

        private Point ClientLocation_Old;

        private bool ClientSize_Update;

        private Size ClientSize_Old;

        public ContainerControl? ParentContainer { get; internal protected set; }

        public int ParentBorderWidth => ParentContainer?.BorderWidth ?? 0;

        public int Index => ParentContainer?.GetSubControls().IndexOf(this) ?? -1;

        public DateTime LastRightClickTime { get; private set; }

        public DateTime LastLeftClickTime { get; private set; }

        public bool InvokeExternalCursorMove { get; set; }

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
                    OnTextUpdateNow.Invoke(temp, _Text);
                    RequestUpdateFrame();
                }
            }
        }
        private string _Text;

        #region 位置与尺寸

        public Point Location
        {
            get => new(ClientLocation.X + ParentBorderWidth, ClientLocation.Y + ParentBorderWidth);
            set
            {
                ClientLocation = new(value.X - ParentBorderWidth, value.Y - ParentBorderWidth);
            }
        }

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
                    OnMoveNow.Invoke(temp, _ClientLocation);
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
                    OnResizeNow.Invoke(temp, _ClientSize);
                    RequestUpdateFrame();
                }
            }
        }
        private Size _ClientSize;

        Point IControlRendering.RenderingLocation => new(ClientLocation.X + BorderWidth, ClientLocation.Y + BorderWidth);

        Size IControlRendering.RenderingSize => ClientSize;

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
            get => (ParentContainer?.Height - ParentBorderWidth ?? GetMCOS().Screen.Height) - (Location.Y + Height);
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
            get => (ParentContainer?.Width - ParentBorderWidth ?? GetMCOS().Screen.Width) - (Location.X + Width);
            set
            {
                int offset = RightToBorder - value;
                ClientSize = new(ClientSize.Width + offset, ClientSize.Height);
            }
        }

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

        public ControlSkin Skin { get; }

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
                    ParentContainer?.GetSubControls().Sort();
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
                    ParentContainer?.GetSubControls().Sort();
            }
        }
        private int _MaxDisplayPriority;

        /// <summary>
        /// 锚定，大小不变，位置自适应父控件
        /// </summary>
        public PlaneFacing Anchor { get; set; }

        /// <summary>
        /// 拉伸，位置不变，大小自适应父控件
        /// </summary>
        public PlaneFacing Stretch { get; set; }

        public LayoutMode LayoutMode => LayoutSyncer is null ? LayoutMode.Auto : LayoutMode.Sync;

        public ContentAnchor ContentAnchor
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
        private ContentAnchor _ContentAnchor;

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
                        OnSelected.Invoke();
                    }
                    else
                    {
                        ControlState ^= ControlState.Selected;
                        OnDeselected.Invoke();
                    }
                    ParentContainer?.GetSubControls().Sort();
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

        #region 事件声明

        public event Action<Point, CursorMode> CursorMove;

        public event Action<Point, CursorMode> CursorEnter;

        public event Action<Point, CursorMode> CursorLeave;

        public event Action<Point> RightClick;

        public event Action<Point> LeftClick;

        public event Action<Point> DoubleRightClick;

        public event Action<Point> DoubleLeftClick;

        public event Action<Point, string> TextEditorUpdate;

        public event Action BeforeFrame;

        public event Action AfterFrame;

        public event Action InitializeCallback;

        public event Action OnSelected;

        public event Action OnDeselected;

        public event Action<string, string> OnTextUpdate;

        public event Action<Point, Point> OnMove;

        public event Action<Size, Size> OnResize;

        public event Action<string, string> OnTextUpdateNow;

        public event Action<Point, Point> OnMoveNow;

        public event Action<Size, Size> OnResizeNow;

        #endregion

        #region 事件订阅

        private void Control_BeforeFrame()
        {
            if (Text_Update)
            {
                if (Text != Text_Old)
                    OnTextUpdate.Invoke(Text_Old, Text);
                Text_Update = false;
                Text_Old = Text;
            }

            if (ClientLocation_Update)
            {
                if (ClientLocation != ClientLocation_Old)
                    OnMove.Invoke(ClientLocation_Old, ClientLocation);
                ClientLocation_Update = false;
                ClientLocation_Old = ClientLocation;
            }

            if (ClientSize_Update)
            {
                if (ClientSize != ClientSize_Old)
                {
                    OnResize.Invoke(ClientSize_Old, ClientSize);
                }
                ClientSize_Update = false;
                ClientSize_Old = ClientSize;
            }
        }

        private void Control_AfterFrame()
        {

        }

        private void Control_OnMove(Point oldPosition, Point newPosition)
        {
            if (!FormIsInitialize())
                return;
            MCOS os = GetMCOS();
            UpdateAllHoverState(ScreenPos2ControlPos(os.PlayerCursorReader.CurrentPosition), os.PlayerCursorReader.CursorMode);
        }

        private void Control_OnResize(Size oldSize, Size newSize)
        {
            if (!FormIsInitialize())
                return;
            MCOS os = GetMCOS();
            UpdateAllHoverState(ScreenPos2ControlPos(os.PlayerCursorReader.CurrentPosition), os.PlayerCursorReader.CursorMode);
        }

        #endregion

        #region 事件处理

        internal virtual void HandleCursorMove(Point position, CursorMode mode)
        {
            UpdateHoverState(position, mode);

            if (IncludedOnControl(position) || InvokeExternalCursorMove)
            {
                CursorMove.Invoke(position, mode);
            }
        }

        internal virtual bool HandleRightClick(Point position)
        {
            if (Visible)
            {
                if (IsHover)
                {
                    RightClick.Invoke(position);
                    DateTime now = DateTime.Now;
                    if (LastRightClickTime == DateTime.MinValue || (DateTime.Now - LastRightClickTime).TotalMilliseconds > 500)
                    {
                        LastRightClickTime = now;
                    }
                    else
                    {
                        DoubleRightClick.Invoke(position);
                        LastRightClickTime = DateTime.MinValue;
                    }
                    return true;
                }
            }
            return false;
        }

        internal virtual bool HandleLeftClick(Point position)
        {
            if (Visible)
            {
                if (IsHover)
                {
                    LeftClick.Invoke(position);
                    DateTime now = DateTime.Now;
                    if (LastLeftClickTime == DateTime.MinValue || (DateTime.Now - LastLeftClickTime).TotalMilliseconds > 500)
                    {
                        LastLeftClickTime = now;
                    }
                    else
                    {
                        DoubleLeftClick.Invoke(position);
                        LastLeftClickTime = DateTime.MinValue;
                    }
                    return true;
                }
            }
            return false;
        }

        internal virtual void HandleTextEditorUpdate(Point position, string text)
        {
            if (Visible)
            {
                if (IncludedOnControl(position))
                    TextEditorUpdate.Invoke(position, text);
            }
        }

        internal virtual void HandleBeforeFrame()
        {
            BeforeFrame.Invoke();
        }

        internal virtual void HandleAfterFrame()
        {
            AfterFrame.Invoke();
        }

        protected void UpdateAllHoverState(Point position, CursorMode mode)
        {
            if (this is ContainerControl container)
            {
                foreach (var control in container.GetSubControls().ToArray())
                {
                    control.UpdateAllHoverState(control.ParentPos2SubPos(position), mode);
                }
            }

            UpdateHoverState(position, mode);
        }

        protected void UpdateHoverState(Point position, CursorMode mode)
        {
            bool included = IncludedOnControl(position);
            if (IsHover)
            {
                if (!included)
                {
                    IsHover = false;
                    CursorLeave.Invoke(position, mode);
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
                            control.CursorLeave.Invoke(position, mode);
                        }
                        else
                            return;
                    }
                    IsHover = true;
                    CursorEnter.Invoke(position, mode);
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

        public virtual void OnInitComplete1()
        {

        }

        public virtual void OnInitComplete2()
        {

        }

        public virtual void OnInitComplete3()
        {
            Text_Update = false;
            Text_Old = Text;
            ClientLocation_Update = false;
            ClientLocation_Old = ClientLocation;
            ClientSize_Update = false;
            ClientSize_Old = ClientSize;
        }

        internal void HandleInitialize()
        {
            Initialize();
            InitializeCallback.Invoke();
            if (this is ContainerControl container)
            {
                foreach (var control in container.GetSubControls())
                {
                    control.HandleInitialize();
                }
            }
        }

        internal void HandleOnInitComplete1()
        {
            OnInitComplete1();
            if (this is ContainerControl container)
            {
                foreach (var control in container.GetSubControls())
                {
                    control.HandleOnInitComplete1();
                }
            }
        }

        internal void HandleOnInitComplete2()
        {
            OnInitComplete2();
            if (this is ContainerControl container)
            {
                foreach (var control in container.GetSubControls())
                {
                    control.HandleOnInitComplete2();
                }
            }
        }

        internal void HandleOnInitComplete3()
        {
            OnInitComplete3();
            if (this is ContainerControl container)
            {
                foreach (var control in container.GetSubControls())
                {
                    control.HandleOnInitComplete3();
                }
            }
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

        public virtual AbstractFrame RenderingFrame()
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

        IEnumerable<IControlRendering> IControlRendering.GetSubRenderings()
        {
            return Array.Empty<IControlRendering>();
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

        public Form? GetForm()
        {
            Control? result = this;
            while (true)
            {
                if (result is null)
                    return null;
                else if (result is Form form)
                    return form;
                else
                    result = result.ParentContainer;
            }
        }

        public Application GetApplication() => GetForm()?.Application ?? throw new InvalidOperationException();

        public MCOS GetMCOS() => GetApplication().MCOS;

        public Process GetProcess() => GetApplication().Process;

        public bool FormIsInitialize() => GetForm()?.IsInitialize ?? false;

        #endregion

        protected void SetTextEditorInitialText()
        {
            GetMCOS().PlayerCursorReader.InitialText = Text;
        }

        protected void ResetTextEditor()
        {
            GetMCOS().PlayerCursorReader.ResetText();
        }

        public void ClearAllControlSyncer()
        {
            if (this is ContainerControl container)
                container.GetSubControls().ClearSyncers();

            LayoutSyncer = null;
        }

        public virtual void Layout(Size oldSize, Size newSize)
        {
            Size offset = newSize - oldSize;
            if (offset.Height != 0)
            {
                if (!Anchor.HasFlag(PlaneFacing.Top) && !Anchor.HasFlag(PlaneFacing.Bottom))
                {
                    double proportion = (ClientLocation.Y + Height / 2.0) / oldSize.Height;
                    ClientLocation = new(ClientLocation.X, (int)Math.Round(newSize.Height * proportion - Height / 2.0));
                }
                if (Anchor.HasFlag(PlaneFacing.Bottom))
                    ClientLocation = new(ClientLocation.X, ClientLocation.Y + offset.Height);

                if (Stretch.HasFlag(PlaneFacing.Top) || Stretch.HasFlag(PlaneFacing.Bottom))
                    BottomToBorder -= offset.Height;
            }

            if (offset.Width != 0)
            {
                if (!Anchor.HasFlag(PlaneFacing.Left) && !Anchor.HasFlag(PlaneFacing.Right))
                {
                    double proportion = (ClientLocation.X + Width / 2.0) / oldSize.Width;
                    ClientLocation = new((int)Math.Round(newSize.Width * proportion - Width / 2.0), ClientLocation.Y);
                }
                if (Anchor.HasFlag(PlaneFacing.Right))
                    ClientLocation = new(ClientLocation.X + offset.Width, ClientLocation.Y);

                if (Stretch.HasFlag(PlaneFacing.Left) || Stretch.HasFlag(PlaneFacing.Right))
                    RightToBorder -= offset.Width;
            }
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

        public int Compare(Control? x, Control? y)
        {
            if (x?.DisplayPriority < y?.DisplayPriority)
                return -1;
            else if (x?.DisplayPriority > y?.DisplayPriority)
                return 1;
            else
                return 0;
        }

        public int CompareTo(Control? other)
        {
            return DisplayPriority.CompareTo(other?.DisplayPriority);
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
