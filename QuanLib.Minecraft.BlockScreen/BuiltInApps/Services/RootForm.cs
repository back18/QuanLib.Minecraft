using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.UI.Controls;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Services
{
    public class RootForm : Form
    {
        public RootForm()
        {
            TaskBar = new(this);
            FormsPanel = new(this);
            ShowTaskBar_Button = new();
        }

        public readonly TaskBar TaskBar;

        public readonly FormsPanel FormsPanel;

        public readonly Button ShowTaskBar_Button;

        public bool ShowTitleBar
        {
            get => SubControls.Contains(TaskBar);
            set
            {
                if (value)
                {
                    if (!ShowTitleBar)
                    {
                        SubControls.TryAdd(TaskBar);
                        SubControls.Remove(ShowTaskBar_Button);
                        FormsPanel?.LayoutSyncer?.Sync();
                    }
                }
                else
                {
                    if (ShowTitleBar)
                    {
                        SubControls.Remove(TaskBar);
                        SubControls.TryAdd(ShowTaskBar_Button);
                        FormsPanel?.LayoutSyncer?.Sync();
                    }
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            MCOS os = GetMCOS();

            AllowResize = false;
            DisplayPriority = int.MinValue;
            MaxDisplayPriority = int.MinValue + 1;
            BorderWidth = 0;
            ClientSize = new(os.Screen.Width, os.Screen.Height);
            Location = new(0, 0);
            Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.LightBlue));

            SubControls.Add(TaskBar);
            TaskBar.ClientLocation = new(0, ClientSize.Height - 16);

            SubControls.Add(FormsPanel);
            FormsPanel.LayoutSyncer?.Sync();

            ShowTaskBar_Button.Visible = false;
            ShowTaskBar_Button.InvokeExternalCursorMove = true;
            ShowTaskBar_Button.Text = "↑";
            ShowTaskBar_Button.ClientSize = new(16, 16);
            ShowTaskBar_Button.LayoutSyncer = new(this, (oldPosition, newPosition) => { }, (oldSize, newSize) => ShowTaskBar_Button.ClientLocation = this.LifeLayout(null, ShowTaskBar_Button, 0, newSize.Height - ShowTaskBar_Button.Height));
            ShowTaskBar_Button.Anchor = PlaneFacing.Top | PlaneFacing.Right;
            ShowTaskBar_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            ShowTaskBar_Button.CursorEnter += ShowTaskBar_Button_CursorEnter;
            ShowTaskBar_Button.CursorLeave += ShowTaskBar_Button_CursorLeave;
            ShowTaskBar_Button.RightClick += ShowTaskBar_Button_RightClick;
        }

        private void ShowTaskBar_Button_CursorEnter(Point position, CursorMode mode)
        {
            ShowTaskBar_Button.Visible = true;
        }

        private void ShowTaskBar_Button_CursorLeave(Point position, CursorMode mode)
        {
            ShowTaskBar_Button.Visible = false;
        }

        private void ShowTaskBar_Button_RightClick(Point position)
        {
            ShowTitleBar = true;
        }

        public List<Form> GetFormList()
        {
            List<Form> result = new();
            foreach (var control in FormsPanel.SubControls)
            {
                if (control is Form form)
                    result.Add(form);
            }
            return result;
        }

        public void SelectedMaxDisplayPriority()
        {
            if (FormsPanel.SubControls.Count > 0)
            {
                for (int i = FormsPanel.SubControls.Count - 1; i >= 0; i--)
                {
                    if (FormsPanel.SubControls[i] is Form form && form.AllowSelected)
                    {
                        form.IsSelected = true;
                        break;
                    }
                }
            }
        }

        public bool TrySwitchForm(Form form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            if (!FormsPanel.SubControls.Contains(form))
                return false;
            if (!form.AllowSelected)
                return false;

            var controls = FormsPanel.SubControls.GetSelecteds();
            foreach (var control in controls)
            {
                if (!control.AllowDeselected)
                    return false;
            }

            form.IsSelected = true;
            foreach (var control in controls)
            {
                control.IsSelected = false;
            }

            return true;
        }
    }
}
