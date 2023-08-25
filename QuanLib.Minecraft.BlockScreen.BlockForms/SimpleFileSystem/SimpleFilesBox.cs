using Newtonsoft.Json.Linq;
using QuanLib.Event;
using QuanLib.IO;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.SimpleFileSystem
{
    public class SimpleFilesBox : ScrollablePanel
    {
        public SimpleFilesBox(string rootDirectory)
        {
            _RootDirectory = string.Empty;  //这里赋值仅为了抑制 CS8618 - Non-nullable variable must contain a non-null value when exiting constructor. Consider declaring it as
            RootDirectory = rootDirectory;
            Text = rootDirectory;
            _backwards = new();
            _forward = new();
            _SearchText = string.Empty;

            OpenFile += OnOpenFile;
            OpeningItemException += OnOpeningItemException;
        }

        private readonly Stack<string> _backwards;

        private readonly Stack<string> _forward;

        public override string Text
        {
            get => base.Text;
            set
            {
                value = GetFullPath(value);
                base.Text = value;
            }
        }

        public string RootDirectory
        {
            get => _RootDirectory;
            set
            {
                value = GetFullPath(value);
                _RootDirectory = value;
            }
        }

        private string _RootDirectory;

        public string SearchText
        {
            get => _SearchText;
            set
            {
                if (_SearchText != value)
                {
                    _SearchText = value;
                    ActiveLayoutAll();
                }
            }
        }
        private string _SearchText;

        public event EventHandler<SimpleFilesBox, ExceptionEventArgs> OpeningItemException;

        public event EventHandler<SimpleFilesBox, FIleInfoEventArgs> OpenFile;

        protected virtual void OnOpenFile(SimpleFilesBox sender, FIleInfoEventArgs e) { }

        protected virtual void OnOpeningItemException(SimpleFilesBox sender, ExceptionEventArgs e) { }

        protected override void OnTextChanged(Control sender, TextChangedEventArgs e)
        {
            base.OnTextChanged(sender, e);

            ActiveLayoutAll();
        }

        protected override void OnLayoutAll(AbstractContainer<Control> sender, SizeChangedEventArgs e)
        {
            ActiveLayoutAll();
        }

        public override void ActiveLayoutAll()
        {
            List<FileSystemItem> items = new();

            try
            {
                if (!Text.StartsWith(RootDirectory))
                {
                    throw new UnauthorizedAccessException("未经授权的访问");
                }

                if (string.IsNullOrEmpty(Text))
                {
                    foreach (var drive in DriveInfo.GetDrives())
                        items.Add(new DriveItem(drive));
                }
                else
                {
                    switch (FileUtil.GetPathType(Text))
                    {
                        case PathType.Unknown:
                            throw new DirectoryNotFoundException("目录不存在");
                        case PathType.Drive:
                            DirectoryInfo driveRoot = new DriveInfo(Text).RootDirectory;
                            foreach (var dir in driveRoot.GetDirectories())
                                items.Add(new DirectoryItem(dir));
                            foreach (var file in driveRoot.GetFiles())
                                items.Add(new FileItem(file));
                            break;
                        case PathType.Directory:
                            DirectoryInfo directory = new(Text);
                            foreach (var dir in directory.GetDirectories())
                                items.Add(new DirectoryItem(dir));
                            foreach (var file in directory.GetFiles())
                                items.Add(new FileItem(file));
                            break;
                        case PathType.File:
                            FileInfo fileInfo = new(Text);
                            OpenFile.Invoke(this, new(fileInfo));
                            return;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                OpeningItemException.Invoke(this, new(ex));
                Backward();
                _forward.Clear();
                return;
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                List<FileSystemItem> result = new();
                foreach (var item in items)
                {
                    if (item.FileSystemInfo.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        result.Add(item);
                }
                items = result;
            }

            SubControls.Clear();

            if (items.Count > 0)
            {
                foreach (var path in items)
                {
                    path.AutoSetSize();
                    path.DoubleRightClick += (sender, e) =>
                    {
                        if (path is FileItem file)
                        {
                            OpenFile.Invoke(this, new(file.FileInfo));
                        }
                        else
                        {
                            _backwards.Push(Text);
                            _forward.Clear();
                            Text = Path.Combine(Text, path.Text);
                        }
                    };
                    SubControls.Add(path);
                }
                this.ForceFillDownLayout(1, items);
                PageSize = new(ClientSize.Width, items[^1].BottomLocation + 2);
                OffsetPosition = new(0, 0);
            }
            else
            {
                PageSize = ClientSize;
                OffsetPosition = new(0, 0);
            }
        }

        public void Backward()
        {
            if (_backwards.Count > 0)
            {
                _forward.Push(Text);
                Text = _backwards.Pop();
            }
            else
            {
                string? dir = Path.GetDirectoryName(Text);
                if (dir is not null && dir.StartsWith(RootDirectory))
                {
                    _forward.Push(Text);
                    Text = dir;
                }
                else
                {
                    Text = RootDirectory;
                }
            }
        }

        public void Forward()
        {
            if (_forward.Count > 0)
            {
                _backwards.Push(Text);
                Text = _forward.Pop();
            }
        }

        public string[] GetSelecteds()
        {
            List<string> result = new();
            foreach (var selected in SubControls.GetSelecteds())
            {
                if (selected is FileSystemItem item)
                    result.Add(item.FileSystemInfo.FullName);
            }
            return result.ToArray();
        }

        public static string GetFullPath(string value)
        {
            if (string.IsNullOrEmpty(value) && Environment.OSVersion.Platform != PlatformID.Win32NT)
                value = "/";
            value = string.IsNullOrEmpty(value) ? string.Empty : Path.GetFullPath(value);
            return value;
        }
    }
}
