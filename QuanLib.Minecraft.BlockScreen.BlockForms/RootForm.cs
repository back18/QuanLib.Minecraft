using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class RootForm : Form, IRootForm
    {
        public RootForm()
        {
            AllowMove = false;
            AllowResize = false;
            DisplayPriority = int.MinValue;
            MaxDisplayPriority = int.MinValue + 1;

            TaskBar = new(this);
            FormContainer = new(this);
            ShowTaskBar_Button = new();
        }

        private readonly TaskBar TaskBar;

        private readonly FormContainer FormContainer;

        private readonly Button ShowTaskBar_Button;

        public Size FormContainerSize => FormContainer.ClientSize;

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
                        FormContainer?.LayoutSyncer?.Sync();
                    }
                }
                else
                {
                    if (ShowTitleBar)
                    {
                        SubControls.Remove(TaskBar);
                        SubControls.TryAdd(ShowTaskBar_Button);
                        FormContainer?.LayoutSyncer?.Sync();
                    }
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            MCOS os = MCOS.GetMCOS();

            AllowResize = false;
            DisplayPriority = int.MinValue;
            MaxDisplayPriority = int.MinValue + 1;
            BorderWidth = 0;
            Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.LightBlue));

            SubControls.Add(TaskBar);

            SubControls.Add(FormContainer);
            FormContainer.LayoutSyncer?.Sync();

            ShowTaskBar_Button.Visible = false;
            ShowTaskBar_Button.InvokeExternalCursorMove = true;
            ShowTaskBar_Button.Text = "↑";
            ShowTaskBar_Button.ClientSize = new(16, 16);
            ShowTaskBar_Button.LayoutSyncer = new(this, (sender, e) => { }, (sender, e) =>
            ShowTaskBar_Button.ClientLocation = this.LifeLayout(null, ShowTaskBar_Button, 0, e.NewSize.Height - ShowTaskBar_Button.Height));
            ShowTaskBar_Button.Anchor = Direction.Top | Direction.Right;
            ShowTaskBar_Button.Skin.BackgroundBlockID = Skin.BackgroundBlockID;
            ShowTaskBar_Button.CursorEnter += ShowTaskBar_Button_CursorEnter;
            ShowTaskBar_Button.CursorLeave += ShowTaskBar_Button_CursorLeave;
            ShowTaskBar_Button.RightClick += ShowTaskBar_Button_RightClick;
        }

        private void ShowTaskBar_Button_CursorEnter(Control sender, CursorEventArgs e)
        {
            ShowTaskBar_Button.Visible = true;
        }

        private void ShowTaskBar_Button_CursorLeave(Control sender, CursorEventArgs e)
        {
            ShowTaskBar_Button.Visible = false;
        }

        private void ShowTaskBar_Button_RightClick(Control sender, CursorEventArgs e)
        {
            ShowTitleBar = true;
        }

        public void AddForm(IForm form)
        {
            if (form == this)
                return;
            FormContainer.SubControls.Add(form);
            TrySwitchSelectedForm(form);
        }

        public bool RemoveForm(IForm form)
        {
            if (!FormContainer.SubControls.Remove(form))
                return false;

            form.IsSelected = false;
            SelectedMaxDisplayPriority();
            return true;
        }

        public bool ContainsForm(IForm form)
        {
            return FormContainer.SubControls.Contains(form);
        }

        public IEnumerable<IForm> GetAllForm()
        {
            return FormContainer.SubControls;
        }

        public bool TrySwitchSelectedForm(IForm form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            if (!FormContainer.SubControls.Contains(form))
                return false;
            if (!form.AllowSelected)
                return false;

            var selecteds = FormContainer.SubControls.GetSelecteds();
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
            if (FormContainer.SubControls.Count > 0)
            {
                for (int i = FormContainer.SubControls.Count - 1; i >= 0; i--)
                {
                    if (FormContainer.SubControls[i].AllowSelected)
                    {
                        FormContainer.SubControls[i].IsSelected = true;
                        break;
                    }
                }
            }
        }
    }
}
