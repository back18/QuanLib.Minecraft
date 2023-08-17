using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox
{
    public class ApplicationListBoxForm : DialogBoxForm<ApplicationInfo?>
    {
        public ApplicationListBoxForm(IForm initiator, string title) : base(initiator, title)
        {
            ApplicationList_ListMenuBox = new();
            OK_Button = new();
            Cancel_Button = new();

            DefaultResult = null;
            DialogResult = DefaultResult;
        }

        private readonly ListMenuBox<ApplicationItem> ApplicationList_ListMenuBox;

        private readonly Button OK_Button;

        private readonly Button Cancel_Button;

        public override ApplicationInfo? DefaultResult { get; }

        public override ApplicationInfo? DialogResult { get; protected set; }

        public override void Initialize()
        {
            base.Initialize();

            ClientSize = new(113, 86 + TitleBar.Height);
            CenterOnInitiatorForm();

            ClientPanel.SubControls.Add(ApplicationList_ListMenuBox);
            ApplicationList_ListMenuBox.ClientLocation = new(2, 2);
            ApplicationList_ListMenuBox.ClientSize = new(107, 60);
            ApplicationList_ListMenuBox.RightClick += ApplicationList_ListMenuBox_RightClick;

            foreach (var appInfo in MCOS.Instance.ApplicationManager.ApplicationList.Values)
            {
                if (appInfo.AppendToDesktop)
                {
                    ApplicationItem item = new(appInfo);
                    item.ClientSize = new(96, 16);
                    ApplicationList_ListMenuBox.AddedSubControlAndLayout(item);
                }
            }

            ClientPanel.SubControls.Add(Cancel_Button);
            Cancel_Button.Text = "取消";
            Cancel_Button.ClientSize = new(32, 16);
            Cancel_Button.ClientLocation = ClientPanel.LeftLayout(null, Cancel_Button, 2, ApplicationList_ListMenuBox.BottomLocation + 3);
            Cancel_Button.RightClick += Cancel_Button_RightClick;

            ClientPanel.SubControls.Add(OK_Button);
            OK_Button.Text = "确认";
            OK_Button.ClientSize = new(32, 16);
            OK_Button.ClientLocation = ClientPanel.LeftLayout(Cancel_Button, OK_Button, 2);
            OK_Button.RightClick += OK_Button_RightClick;
        }

        private void ApplicationList_ListMenuBox_RightClick(Control sender, CursorEventArgs e)
        {
            var selecteds = ApplicationList_ListMenuBox.SubControls.GetSelecteds();
            Control? selected = ApplicationList_ListMenuBox.SubControls.FirstSelected;
            if (selecteds.Count > 1 && selected is not null)
            {
                selected.IsSelected = false;
            }
        }

        private void OK_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            DialogResult =(ApplicationList_ListMenuBox.SubControls.FirstSelected as ApplicationItem)?.ApplicationInfo;
            CloseForm();
        }

        private void Cancel_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            CloseForm();
        }
    }
}
