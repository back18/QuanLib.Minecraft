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
using QuanLib.Minecraft.BlockScreen.BlockForms.SimpleFileSystem;
using QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox;
using QuanLib.Minecraft.BlockScreen.Config;

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
            SimpleFilesBox = new();
        }

        private readonly Button Backward_Button;

        private readonly Button Forward_Button;

        private readonly Button PreviousPage_Button;

        private readonly Button NextPage_Button;

        private readonly Button OK_Button;

        private readonly Button Cancel_Button;

        private readonly TextBox Path_TextBox;

        private readonly SimpleFilesBox SimpleFilesBox;

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
            Forward_Button.RightClick += Forward_Button_RightClick;

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
            Cancel_Button.ClientLocation = ClientPanel.LeftLayout(null, Cancel_Button, spacing, start2);
            Cancel_Button.Anchor = Direction.Bottom | Direction.Right;
            Cancel_Button.RightClick += Cancel_Button_RightClick;

            ClientPanel.SubControls.Add(OK_Button);
            OK_Button.Text = "确定";
            OK_Button.ClientSize = new(32, 16);
            OK_Button.ClientLocation = ClientPanel.LeftLayout(Cancel_Button, OK_Button, spacing);
            OK_Button.Anchor = Direction.Bottom | Direction.Right;
            OK_Button.RightClick += OK_Button_RightClick;

            ClientPanel.SubControls.Add(SimpleFilesBox);
            SimpleFilesBox.Width = ClientPanel.ClientSize.Width - 4;
            SimpleFilesBox.Height = ClientPanel.ClientSize.Height - Backward_Button.Height - PreviousPage_Button.Height - 8;
            SimpleFilesBox.ClientLocation = ClientPanel.BottomLayout(Backward_Button, spacing);
            SimpleFilesBox.Stretch = Direction.Bottom | Direction.Right;
            SimpleFilesBox.Skin.SetAllBackgroundBlockID(BlockManager.Concrete.Lime);
            SimpleFilesBox.TextChanged += SimpleFilesBox_TextChanged;
            SimpleFilesBox.OpenFile += SimpleFilesBox_OpenFile;
            SimpleFilesBox.OpeningItemException += SimpleFilesBox_OpeningItemException;

            ClientPanel.ClientSize = size2;
        }

        private void OK_Button_RightClick(Control sender, CursorEventArgs e)
        {

        }

        private void Cancel_Button_RightClick(Control sender, CursorEventArgs e)
        {

        }

        private void Backward_Button_RightClick(Control sender, CursorEventArgs e)
        {
            SimpleFilesBox.Backward();
        }

        private void Forward_Button_RightClick(Control sender, CursorEventArgs e)
        {
            SimpleFilesBox.Forward();
        }

        private void Path_TextBox_TextChanged(Control sender, TextChangedEventArgs e)
        {
            SimpleFilesBox.Text = Path_TextBox.Text;

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

        private void SimpleFilesBox_TextChanged(Control sender, TextChangedEventArgs e)
        {
            Path_TextBox.Text = SimpleFilesBox.Text;
        }

        private void SimpleFilesBox_OpenFile(SimpleFilesBox sender, FIleInfoEventArgs e)
        {
            FileInfo fileInfo = e.FileInfo;
            string extension = Path.GetExtension(fileInfo.Name).TrimStart('.');
            if (ConfigManager.Registry.TryGetValue(extension, out var id) && MCOS.Instance.ApplicationManager.ApplicationList.TryGetValue(id, out var app))
            {
                MCOS.Instance.ProcessManager.ProcessList.Add(app, new string[] { fileInfo.FullName }, this);
            }
            else
            {
                _ = DialogBoxManager.OpenMessageBoxAsync(this, "提示", $"找不到合适的应用程序打开“{extension}”格式的文件", MessageBoxButtons.OK);
            }
        }

        private void SimpleFilesBox_OpeningItemException(SimpleFilesBox sender, ExceptionEventArgs e)
        {
            _ = DialogBoxManager.OpenMessageBoxAsync(this, "警告", $"无法打开文件或文件夹，因为：\n{e.Exception.GetType().Name}: {e.Exception.Message}", MessageBoxButtons.OK);
        }
    }
}
