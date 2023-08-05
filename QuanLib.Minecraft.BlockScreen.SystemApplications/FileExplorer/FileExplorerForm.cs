using QuanLib.IO;
using QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Event;
using QuanLib.Minecraft.Block;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer
{
    public class FileExplorerForm : WindowForm
    {
        public FileExplorerForm()
        {
            Backward_Button = new();
            Forward_Button = new();
            PreviousPage_Button = new();
            NextPage_Button = new();
            OK_Button = new();
            Cancel_Button = new();
            Path_TextBox = new();
            PathList_Panel = new();

            _pages = new();
            _stack = new();
        }

        private readonly Button Backward_Button;

        private readonly Button Forward_Button;

        private readonly Button PreviousPage_Button;

        private readonly Button NextPage_Button;

        private readonly Button OK_Button;

        private readonly Button Cancel_Button;

        private readonly TextBox Path_TextBox;

        private readonly ScrollablePanel PathList_Panel;

        private readonly Dictionary<int, PathIcon[]> _pages;

        private readonly Stack<string> _stack;

        public override void Initialize()
        {
            base.Initialize();

            ClientPanel.PageSize = new(178, 85);
            Size size1 = ClientPanel.ClientSize;
            Size size2 = ClientPanel.ClientSize;
            if (size1.Width < ClientPanel.PageSize.Width)
                size1.Width = ClientPanel.PageSize.Width;
            if (size1.Height < ClientPanel.PageSize.Height)
                size1.Height = ClientPanel.PageSize.Height;
            ClientPanel.ClientSize = size1;

            int spacing = 2;
            int start1 = 2;
            int start2 = ClientPanel.ClientSize.Height - PreviousPage_Button.Height - 2;

            ClientPanel.SubControls.Add(Backward_Button);
            Backward_Button.Text = "←";
            Backward_Button.ClientSize = new(16, 16);
            Backward_Button.ClientLocation = ClientPanel.RightLayout(null, spacing, start1);
            Backward_Button.RightClick += Backward_Button_RightClick;

            ClientPanel.SubControls.Add(Forward_Button);
            Forward_Button.Text = "→";
            Forward_Button.ClientSize = new(16, 16);
            Forward_Button.ClientLocation = ClientPanel.RightLayout(Backward_Button, spacing);

            ClientPanel.SubControls.Add(Path_TextBox);
            Path_TextBox.ClientLocation = ClientPanel.RightLayout(Forward_Button, spacing);
            Path_TextBox.Width = ClientPanel.ClientSize.Width - Backward_Button.Width - Forward_Button.Width - 8;
            Path_TextBox.Stretch = Direction.Right;
            Path_TextBox.TextChanged += Path_TextBox_TextChanged;

            ClientPanel.SubControls.Add(PreviousPage_Button);
            PreviousPage_Button.Text = "上一页";
            PreviousPage_Button.ClientSize = new(48, 16);
            PreviousPage_Button.ClientLocation = ClientPanel.RightLayout(null, spacing, start2);
            PreviousPage_Button.Anchor = Direction.Bottom | Direction.Left;
            PreviousPage_Button.RightClick += PreviousPage_Button_RightClick;

            ClientPanel.SubControls.Add(NextPage_Button);
            NextPage_Button.Text = "下一页";
            NextPage_Button.ClientSize = new(48, 16);
            NextPage_Button.ClientLocation = ClientPanel.RightLayout(PreviousPage_Button, spacing);
            NextPage_Button.Anchor = Direction.Bottom | Direction.Left;
            NextPage_Button.RightClick += NextPage_Button_RightClick;

            ClientPanel.SubControls.Add(Cancel_Button);
            Cancel_Button.Text = "取消";
            Cancel_Button.ClientSize = new(32, 16);
            Cancel_Button.ClientLocation = ClientPanel.LifeLayout(null, Cancel_Button, spacing, start2);
            Cancel_Button.Anchor = Direction.Bottom | Direction.Right;
            Cancel_Button.RightClick += Cancel_Button_RightClick;

            ClientPanel.SubControls.Add(OK_Button);
            OK_Button.Text = "确定";
            OK_Button.ClientSize = new(32, 16);
            OK_Button.ClientLocation = ClientPanel.LifeLayout(Cancel_Button, OK_Button, spacing);
            OK_Button.Anchor = Direction.Bottom | Direction.Right;
            OK_Button.RightClick += OK_Button_RightClick;

            ClientPanel.SubControls.Add(PathList_Panel);
            PathList_Panel.Width = ClientPanel.ClientSize.Width - 4;
            PathList_Panel.Height = ClientPanel.ClientSize.Height - Backward_Button.Height - PreviousPage_Button.Height - 8;
            PathList_Panel.ClientLocation = ClientPanel.BottomLayout(Backward_Button, spacing);
            PathList_Panel.Stretch = Direction.Bottom | Direction.Right;
            PathList_Panel.Skin.SetAllBackgroundBlockID(BlockManager.Concrete.Lime);

            ClientPanel.ClientSize = size2;

            ActiveLayoutAll();
        }

        protected override void OnLayoutAll(AbstractContainer<Control> sender, SizeChangedEventArgs e)
        {
            base.OnLayoutAll(sender, e);

            ActiveLayoutAll();
        }

        private void OK_Button_RightClick(Control sender, CursorEventArgs e)
        {
            List<string> paths = new();
            foreach (var page in _pages.Values)
                foreach (var path in page)
                    if (path.IsSelected)
                    {
                        paths.Add(path.Path);
                        path.IsSelected = false;
                    }
        }

        private void Cancel_Button_RightClick(Control sender, CursorEventArgs e)
        {

        }

        private void Backward_Button_RightClick(Control sender, CursorEventArgs e)
        {
            Path_TextBox.Text = Path.GetDirectoryName(Path_TextBox.Text) ?? string.Empty;
        }

        private void Path_TextBox_TextChanged(Control sender, TextChangedEventArgs e)
        {
            ActiveLayoutAll();

            if (SystemResourcesManager.DefaultFont.GetTotalSize(e.NewText).Width > Path_TextBox.ClientSize.Width)
                Path_TextBox.ContentAnchor = AnchorPosition.UpperRight;
            else
                Path_TextBox.ContentAnchor = AnchorPosition.UpperLeft;
        }

        private void PreviousPage_Button_RightClick(Control sender, CursorEventArgs e)
        {

        }

        private void NextPage_Button_RightClick(Control sender, CursorEventArgs e)
        {

        }

        public override void ActiveLayoutAll()
        {
            List<PathIcon> paths = new();
            if (string.IsNullOrEmpty(Path_TextBox.Text))
            {
                foreach (var drive in DriveInfo.GetDrives())
                    paths.Add(new DriveIcon(drive));
            }
            else
            {
                switch (FileUtil.GetPathType(Path_TextBox.Text))
                {
                    case PathType.Unknown:
                        return;
                    case PathType.Drive:
                        DirectoryInfo driveRoot = new DriveInfo(Path_TextBox.Text).RootDirectory;
                        foreach (var dir in driveRoot.GetDirectories())
                            paths.Add(new DirectoryIcon(dir));
                        foreach (var file in driveRoot.GetFiles())
                            paths.Add(new FileIcon(file));
                        break;
                    case PathType.Directory:
                        DirectoryInfo directory = new DirectoryInfo(Path_TextBox.Text);
                        foreach (var dir in directory.GetDirectories())
                            paths.Add(new DirectoryIcon(dir));
                        foreach (var file in directory.GetFiles())
                            paths.Add(new FileIcon(file));
                        break;
                    case PathType.File:
                        paths.Add(new FileIcon(new(Path_TextBox.Text)));
                        break;
                    default:
                        break;
                }
            }

            PathList_Panel.SubControls.Clear();
            foreach (var icon in paths)
            {
                icon.AutoSetSize();
                icon.DoubleRightClick += (sender, e) =>
                {
                    Path_TextBox.Text = Path.Combine(Path_TextBox.Text, icon.Text);
                };
                PathList_Panel.SubControls.Add(icon);
            }

            PathList_Panel.ForceFillDownLayout(1, paths);
            PathList_Panel.PageSize = new(PathList_Panel.ClientSize.Width, paths[^1].BottomLocation + 2);
            PathList_Panel.OffsetPosition = new(0, 0);
            PathList_Panel.RefreshVerticalScrollBar();
        }
    }
}
