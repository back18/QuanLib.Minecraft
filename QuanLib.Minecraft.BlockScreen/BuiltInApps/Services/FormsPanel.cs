using QuanLib.Minecraft.BlockScreen.Controls;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Services
{
    public class FormsPanel : Panel
    {
        public FormsPanel(RootForm owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            BorderWidth = 0;
            ControlSyncer = new(_owner,
            (oldPosition, newPosition) => { },
            (oldSize, newSize) =>
            {
                if (_owner.ShowTitleBar)
                    ClientSize = new(newSize.Width, newSize.Height - 16);
                else
                    ClientSize = new(newSize.Width, newSize.Height);
            });
            OnAddSubControl += FormsPanel_OnAddSubControl;
            OnRemoveSubControl += FormsPanel_OnRemoveSubControl;
        }

        private readonly RootForm _owner;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentControl)
                throw new InvalidOperationException();
        }

        private void FormsPanel_OnAddSubControl(Control control)
        {
            if (control is not Form)
                return;

            control.Stretch = PlaneFacing.Bottom;
            control.ControlSyncer = new(this,
            (oldPosition, newPosition) => { },
            (oldSize, newSize) =>
            {
                control.OnOnLayout(oldSize, newSize);
            });
        }

        private void FormsPanel_OnRemoveSubControl(Control control)
        {
            if (control is not Form)
                return;

            control.ControlSyncer = null;
        }

        internal override void HandleCursorMove(Point position, CursorMode mode)
        {
            foreach (var control in SubControls.Reverse())
            {
                if (control.IsSelected)
                {
                    control.HandleCursorMove(control.ParentPos2SubPos(position), mode);
                }
            }

            UpdateHoverState(position, mode);
        }

        internal override bool HandleRightClick(Point position)
        {
            bool result = false;
            foreach (var control in SubControls.Reverse())
            {
                Point sub = control.ParentPos2SubPos(position);
                if (control is Form form)
                {
                    if (form.ResizeBorder != PlaneFacing.None)
                    {
                        form.IsOnResize = !form.IsOnResize;
                        result = true;
                        break;
                    }
                    else if (form.IncludedOnControl(sub))
                    {
                        if (form.IsSelected)
                            form.HandleRightClick(sub);
                        else
                            _owner.TrySwitchForm(form);
                        result = true;
                        break;
                    }
                }
            }

            if (result)
                return true;
            else if (SubControls.FirstHover is null)
                return false;
            else
                return SubControls.FirstHover.HandleRightClick(SubControls.FirstHover.ParentPos2SubPos(position));
        }

        internal override bool HandleLeftClick(Point position)
        {
            bool result = false;
            foreach (var control in SubControls.Reverse())
            {
                if (control.IsSelected)
                {
                    result = control.HandleLeftClick(control.ParentPos2SubPos(position));
                }
            }

            return result;
        }

        internal override void HandleTextEditorUpdate(Point position, string text)
        {
            foreach (var control in SubControls.Reverse())
            {
                if (control.IsSelected)
                {
                    control.HandleTextEditorUpdate(control.ParentPos2SubPos(position), text);
                }
            }
        }
    }
}
