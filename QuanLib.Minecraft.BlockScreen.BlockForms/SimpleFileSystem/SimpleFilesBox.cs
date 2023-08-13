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
        public SimpleFilesBox()
        {
            _backwards = new();
            _forward = new();

            OpenFile += OnOpenFile;
            OpeningItemException += OnOpeningItemException;
        }

        private readonly Stack<string> _backwards;

        private readonly Stack<string> _forward;

        public event EventHandler<SimpleFilesBox, ExceptionEventArgs> OpeningItemException;

        public event EventHandler<SimpleFilesBox, FIleInfoEventArgs> OpenFile;

        protected virtual void OnOpenFile(SimpleFilesBox sender, FIleInfoEventArgs e) { }

        protected virtual void OnOpeningItemException(SimpleFilesBox sender, ExceptionEventArgs e) { }

        public override void Initialize()
        {
            base.Initialize();

            ActiveLayoutAll();
        }

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
            List<FileSystemItem> paths = new();
            if (string.IsNullOrEmpty(Text))
            {
                foreach (var drive in DriveInfo.GetDrives())
                    paths.Add(new DriveItem(drive));
            }
            else
            {
                try
                {
                    switch (FileUtil.GetPathType(Text))
                    {
                        case PathType.Unknown:
                            throw new DirectoryNotFoundException();
                        case PathType.Drive:
                            DirectoryInfo driveRoot = new DriveInfo(Text).RootDirectory;
                            foreach (var dir in driveRoot.GetDirectories())
                                paths.Add(new DirectoryItem(dir));
                            foreach (var file in driveRoot.GetFiles())
                                paths.Add(new FileItem(file));
                            break;
                        case PathType.Directory:
                            DirectoryInfo directory = new(Text);
                            foreach (var dir in directory.GetDirectories())
                                paths.Add(new DirectoryItem(dir));
                            foreach (var file in directory.GetFiles())
                                paths.Add(new FileItem(file));
                            break;
                        case PathType.File:
                            FileInfo fileInfo = new(Text);
                            OpenFile.Invoke(this, new(fileInfo));
                            return;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    OpeningItemException.Invoke(this, new(ex));
                    Backward();
                    _forward.Clear();
                    return;
                }
            }

            SubControls.Clear();

            if (paths.Count > 0)
            {
                foreach (var path in paths)
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
                this.ForceFillDownLayout(1, paths);
                PageSize = new(ClientSize.Width, paths[^1].BottomLocation + 2);
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
        }

        public void Forward()
        {
            if (_forward.Count > 0)
            {
                _backwards.Push(Text);
                Text = _forward.Pop();
            }
        }
    }
}
