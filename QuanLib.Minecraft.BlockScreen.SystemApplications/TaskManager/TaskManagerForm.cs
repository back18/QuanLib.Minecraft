using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.TaskManager
{
    public class TaskManagerForm : WindowForm
    {
        public TaskManagerForm()
        {
            PreviousPage_Button = new();
            NextPage_Button = new();
            PageNumber_Label = new();
            Open_Button = new();
            Close_Button = new();
            TaskList_Panel = new();
        }

        private readonly Button PreviousPage_Button;

        private readonly Button NextPage_Button;

        private readonly Label PageNumber_Label;

        private readonly Button Open_Button;

        private readonly Button Close_Button;

        private readonly Panel<TaskIcon> TaskList_Panel;

        public override void Initialize()
        {
            base.Initialize();

            int spacing = 2;
            int start = ClientPanel.ClientSize.Height - PreviousPage_Button.Height - 2;

            ClientPanel.SubControls.Add(PreviousPage_Button);
            PreviousPage_Button.Text = "上一页";
            PreviousPage_Button.ClientSize = new(48, 16);
            PreviousPage_Button.ClientLocation = ClientPanel.RightLayout(null, spacing, start);
            PreviousPage_Button.Anchor = Direction.Bottom | Direction.Left;

            ClientPanel.SubControls.Add(NextPage_Button);
            NextPage_Button.Text = "下一页";
            NextPage_Button.ClientSize = new(48, 16);
            NextPage_Button.ClientLocation = ClientPanel.RightLayout(PreviousPage_Button, spacing);
            NextPage_Button.Anchor = Direction.Bottom | Direction.Left;

            ClientPanel.SubControls.Add(PageNumber_Label);
            PageNumber_Label.Text = "1/1";
            PageNumber_Label.ClientLocation = ClientPanel.RightLayout(NextPage_Button, spacing);
            PageNumber_Label.Anchor = Direction.Bottom | Direction.Left;

            ClientPanel.SubControls.Add(Close_Button);
            Close_Button.Text = "关闭";
            Close_Button.ClientSize = new(32, 16);
            Close_Button.ClientLocation = ClientPanel.LeftLayout(null, Close_Button, spacing, start);
            Close_Button.Anchor = Direction.Bottom | Direction.Right;

            ClientPanel.SubControls.Add(Open_Button);
            Open_Button.Text = "打开";
            Open_Button.ClientSize = new(32, 16);
            Open_Button.ClientLocation = ClientPanel.LeftLayout(Close_Button, Open_Button, spacing);
            Open_Button.Anchor = Direction.Bottom | Direction.Right;

            ClientPanel.SubControls.Add(TaskList_Panel);
            TaskList_Panel.Width = ClientPanel.ClientSize.Width - 4;
            TaskList_Panel.Height = ClientPanel.ClientSize.Height - PreviousPage_Button.Height - 6;
            TaskList_Panel.ClientLocation = new(2, 2);
            TaskList_Panel.Stretch = Direction.Bottom | Direction.Right;
        }
    }
}
