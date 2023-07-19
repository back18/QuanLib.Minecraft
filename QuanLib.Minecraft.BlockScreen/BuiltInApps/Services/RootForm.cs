using QuanLib.Minecraft.BlockScreen.Controls;
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
            Client_Panel = new();
            ShowTaskBar_Button = new();
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
        }

        public readonly TaskBar TaskBar;

        public readonly Panel Client_Panel;

        public readonly Button ShowTaskBar_Button;

        public override Frame RenderingFrame()
        {
            return Frame.BuildFrame(Width, Height, Skin.GetBackgroundBlockID());
        }

        public List<Form> GetFormList()
        {
            List<Form> result = new();
            foreach (var control in SubControls)
            {
                if (control is Form form)
                    result.Add(form);
            }
            return result;
        }

        public void SelectedMaxDisplayPriority()
        {
            if (SubControls.Count > 0)
            {
                for (int i = SubControls.Count - 1; i >= 0; i--)
                {
                    if (SubControls[i] is Form form && form.AllowSelected)
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

            if (!SubControls.Contains(form))
                return false;
            if (!form.AllowSelected)
                return false;

            Control[] controls = SubControls.GetSelecteds();
            foreach (var control in controls)
            {
                if (control is Form selected)
                {
                    if (!selected.AllowDeselected)
                        return false;
                }
            }

            form.IsSelected = true;
            foreach (var control in controls)
            {
                if (control is Form selected)
                    selected.IsSelected = false;
            }

            return true;
        }
    }
}
