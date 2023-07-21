using QuanLib.Minecraft.BlockScreen.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.TaskManager
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
            int start = Client_Panel.ClientSize.Height - PreviousPage_Button.Height - 2;

            Client_Panel.SubControls.Add(PreviousPage_Button);
            PreviousPage_Button.Text = "上一页";
            PreviousPage_Button.ClientSize = new(48, 16);
            PreviousPage_Button.ClientLocation = Client_Panel.RightLayout(null, spacing, start);
            PreviousPage_Button.Anchor = PlaneFacing.Bottom | PlaneFacing.Left;

            Client_Panel.SubControls.Add(NextPage_Button);
            NextPage_Button.Text = "下一页";
            NextPage_Button.ClientSize = new(48, 16);
            NextPage_Button.ClientLocation = Client_Panel.RightLayout(PreviousPage_Button, spacing);
            NextPage_Button.Anchor = PlaneFacing.Bottom | PlaneFacing.Left;

            Client_Panel.SubControls.Add(PageNumber_Label);
            PageNumber_Label.Text = "1/1";
            PageNumber_Label.ClientLocation = Client_Panel.RightLayout(NextPage_Button, spacing);
            PageNumber_Label.Anchor = PlaneFacing.Bottom | PlaneFacing.Left;

            Client_Panel.SubControls.Add(Close_Button);
            Close_Button.Text = "关闭";
            Close_Button.ClientSize = new(32, 16);
            Close_Button.ClientLocation = Client_Panel.LifeLayout(null, Close_Button, spacing, start);
            Close_Button.Anchor = PlaneFacing.Bottom | PlaneFacing.Right;

            Client_Panel.SubControls.Add(Open_Button);
            Open_Button.Text = "打开";
            Open_Button.ClientSize = new(32, 16);
            Open_Button.ClientLocation = Client_Panel.LifeLayout(Close_Button, Open_Button, spacing);
            Open_Button.Anchor = PlaneFacing.Bottom | PlaneFacing.Right;

            Client_Panel.SubControls.Add(TaskList_Panel);
            TaskList_Panel.Width = Client_Panel.ClientSize.Width - 4;
            TaskList_Panel.Height = Client_Panel.ClientSize.Height - PreviousPage_Button.Height - 6;
            TaskList_Panel.ClientLocation = new(2, 2);
            TaskList_Panel.Stretch = PlaneFacing.Bottom | PlaneFacing.Right;
        }
    }
}
