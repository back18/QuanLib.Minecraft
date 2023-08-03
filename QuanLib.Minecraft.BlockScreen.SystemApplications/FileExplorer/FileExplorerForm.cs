﻿using QuanLib.IO;
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

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer
{
    public class FileExplorerForm : WindowForm
    {
        public FileExplorerForm()
        {
            _PageNumber = 1;

            Backward_Button = new();
            Forward_Button = new();
            PreviousPage_Button = new();
            NextPage_Button = new();
            PageNumber_Label = new();
            OK_Button = new();
            Cancel_Button = new();
            Path_TextBox = new();
            PathList_Panel = new();

            _pages = new();
            _pathStack = new();
        }

        private readonly Button Backward_Button;

        private readonly Button Forward_Button;

        private readonly Button PreviousPage_Button;

        private readonly Button NextPage_Button;

        private readonly Label PageNumber_Label;

        private readonly Button OK_Button;

        private readonly Button Cancel_Button;

        private readonly TextBox Path_TextBox;

        private readonly ScrollablePanel PathList_Panel;

        private readonly Dictionary<int, PathIcon[]> _pages;

        private readonly Stack<string> _pathStack;

        public int PageNumber
        {
            get => _PageNumber;
            set
            {
                if (_PageNumber != value)
                {
                    if (value < 1)
                        value = 1;
                    else if (value > _pages.Count)
                        value = _pages.Count;
                    if (value == _PageNumber)
                        return;
                    _PageNumber = value;
                    RequestUpdateFrame();
                }
            }
        }
        private int _PageNumber;

        public override void Initialize()
        {
            base.Initialize();

            Client_Panel.PageSize = new(128, 72);

            int spacing = 2;
            int start1 = 2;
            int start2 = Client_Panel.ClientSize.Height - PreviousPage_Button.Height - 2;

            Client_Panel.SubControls.Add(Backward_Button);
            Backward_Button.Text = "←";
            Backward_Button.ClientSize = new(16, 16);
            Backward_Button.ClientLocation = Client_Panel.RightLayout(null, spacing, start1);
            Backward_Button.RightClick += Backward_Button_RightClick;

            Client_Panel.SubControls.Add(Forward_Button);
            Forward_Button.Text = "→";
            Forward_Button.ClientSize = new(16, 16);
            Forward_Button.ClientLocation = Client_Panel.RightLayout(Backward_Button, spacing);

            Client_Panel.SubControls.Add(Path_TextBox);
            Path_TextBox.ClientLocation = Client_Panel.RightLayout(Forward_Button, spacing);
            Path_TextBox.Width = Client_Panel.ClientSize.Width - Backward_Button.Width - Forward_Button.Width - 8;
            Path_TextBox.Stretch = Direction.Right;
            Path_TextBox.TextChanged += Path_TextBox_TextChanged;

            Client_Panel.SubControls.Add(PreviousPage_Button);
            PreviousPage_Button.Text = "上一页";
            PreviousPage_Button.ClientSize = new(48, 16);
            PreviousPage_Button.ClientLocation = Client_Panel.RightLayout(null, spacing, start2);
            PreviousPage_Button.Anchor = Direction.Bottom | Direction.Left;
            PreviousPage_Button.RightClick += PreviousPage_Button_RightClick;

            Client_Panel.SubControls.Add(NextPage_Button);
            NextPage_Button.Text = "下一页";
            NextPage_Button.ClientSize = new(48, 16);
            NextPage_Button.ClientLocation = Client_Panel.RightLayout(PreviousPage_Button, spacing);
            NextPage_Button.Anchor = Direction.Bottom | Direction.Left;
            NextPage_Button.RightClick += NextPage_Button_RightClick;

            Client_Panel.SubControls.Add(PageNumber_Label);
            PageNumber_Label.Text = "1/1";
            PageNumber_Label.ClientLocation = Client_Panel.RightLayout(NextPage_Button, spacing);
            PageNumber_Label.Anchor = Direction.Bottom | Direction.Left;

            Client_Panel.SubControls.Add(Cancel_Button);
            Cancel_Button.Text = "取消";
            Cancel_Button.ClientSize = new(32, 16);
            Cancel_Button.ClientLocation = Client_Panel.LifeLayout(null, Cancel_Button, spacing, start2);
            Cancel_Button.Anchor = Direction.Bottom | Direction.Right;
            Cancel_Button.RightClick += Cancel_Button_RightClick;

            Client_Panel.SubControls.Add(OK_Button);
            OK_Button.Text = "确定";
            OK_Button.ClientSize = new(32, 16);
            OK_Button.ClientLocation = Client_Panel.LifeLayout(Cancel_Button, OK_Button, spacing);
            OK_Button.Anchor = Direction.Bottom | Direction.Right;
            OK_Button.RightClick += OK_Button_RightClick;

            Client_Panel.SubControls.Add(PathList_Panel);
            PathList_Panel.Width = Client_Panel.ClientSize.Width - 4;
            PathList_Panel.Height = Client_Panel.ClientSize.Height - Backward_Button.Height - PreviousPage_Button.Height - 8;
            PathList_Panel.ClientLocation = Client_Panel.BottomLayout(Backward_Button, spacing);
            PathList_Panel.Stretch = Direction.Bottom | Direction.Right;
            PathList_Panel.Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Lime));

            UpdatePathList();
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
            UpdatePathList();

            if (SystemResourcesManager.DefaultFont.GetTotalSize(e.NewText).Width > Path_TextBox.ClientSize.Width)
                Path_TextBox.ContentAnchor = AnchorPosition.UpperRight;
            else
                Path_TextBox.ContentAnchor = AnchorPosition.UpperLeft;
        }

        private void PreviousPage_Button_RightClick(Control sender, CursorEventArgs e)
        {
            PageNumber--;
        }

        private void NextPage_Button_RightClick(Control sender, CursorEventArgs e)
        {
            PageNumber++;
        }

        private void UpdatePathList()
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

            PathList_Panel.ForceFillLayout(1, paths);
            PathList_Panel.PageSize = new(PathList_Panel.ClientSize.Width, paths[^1].BottomLocation + 2);
            PathList_Panel.OffsetPosition = new(0, 0);
            PathList_Panel.RefreshVerticalScrollBar();

            //_pages.Clear();
            //int pageNumber = 1;
            //var pages = PathList_Panel.FillLayouPagingt(1, paths);
            //if (pages.Length == 0)
            //{
            //    _pages.Add(pageNumber, Array.Empty<PathIcon>());
            //}
            //else
            //{
            //    foreach (var page in pages)
            //        _pages.Add(pageNumber++, page);
            //}
            PageNumber = 1;
        }

        private void PageTurning(int pageNumber)
        {
            PathList_Panel.SubControls.Clear();
            foreach (var icon in _pages[pageNumber])
                PathList_Panel.SubControls.Add(icon);
            PageNumber_Label.Text = $"{pageNumber}/{_pages.Count}";
        }
    }
}
