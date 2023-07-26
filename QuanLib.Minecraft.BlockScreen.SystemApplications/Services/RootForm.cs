using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Services
{
    public class RootForm : Form, IRootForm
    {
        public RootForm()
        {
            TaskBar = new(this);
            FormsPanel = new(this);
            ShowTaskBar_Button = new();
        }

        private readonly TaskBar TaskBar;

        private readonly FormsPanel FormsPanel;

        private readonly Button ShowTaskBar_Button;

        public Size FormsPanelClientSize => FormsPanel.ClientSize;

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
            ShowTaskBar_Button.Anchor = Direction.Top | Direction.Right;
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

        public void AddForm(IForm form)
        {
            FormsPanel.SubControls.Add(form);
            TrySwitchSelectedForm(form);
        }

        public bool RemoveForm(IForm form)
        {
            if (!FormsPanel.SubControls.Remove(form))
                return false;

            form.IsSelected = false;
            SelectedMaxDisplayPriority();
            return true;
        }

        public bool ContainsForm(IForm form)
        {
            return FormsPanel.SubControls.Contains(form);
        }

        public bool TrySwitchSelectedForm(IForm form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            if (!FormsPanel.SubControls.Contains(form))
                return false;
            if (!form.AllowSelected)
                return false;

            var selecteds = FormsPanel.SubControls.GetSelecteds();
            foreach (var selected in selecteds)
            {
                if (!selected.AllowDeselected)
                    return false;
            }

            form.IsSelected = true;
            foreach (var electeds in selecteds)
            {
                electeds.IsSelected = false;
            }

            return true;
        }

        public void SelectedMaxDisplayPriority()
        {
            if (FormsPanel.SubControls.Count > 0)
            {
                for (int i = FormsPanel.SubControls.Count - 1; i >= 0; i--)
                {
                    if (FormsPanel.SubControls[i].AllowSelected)
                    {
                        FormsPanel.SubControls[i].IsSelected = true;
                        break;
                    }
                }
            }
        }
    }
}
