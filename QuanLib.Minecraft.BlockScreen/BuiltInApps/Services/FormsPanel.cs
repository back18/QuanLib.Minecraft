using QuanLib.Minecraft.BlockScreen.UI.Controls;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Services
{
    public class FormsPanel : Panel<Form>
    {
        public FormsPanel(RootForm owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            BorderWidth = 0;
            LayoutSyncer = new(_owner,
            (oldPosition, newPosition) => { },
            (oldSize, newSize) =>
            {
                if (_owner.ShowTitleBar)
                    ClientSize = new(newSize.Width, newSize.Height - 16);
                else
                    ClientSize = new(newSize.Width, newSize.Height);
            });
            OnAddedSubControl += FormsPanel_OnAddSubControl;
            OnRemovedSubControl += FormsPanel_OnRemoveSubControl;
        }

        private readonly RootForm _owner;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();
        }

        private void FormsPanel_OnAddSubControl(Control control)
        {
            if (control is not Form)
                return;

            control.Stretch = PlaneFacing.Bottom;
            control.LayoutSyncer = new(this,
            (oldPosition, newPosition) => { },
            (oldSize, newSize) =>
            {
                control.Layout(oldSize, newSize);
            });
        }

        private void FormsPanel_OnRemoveSubControl(Control control)
        {
            if (control is not Form)
                return;

            control.LayoutSyncer = null;
        }

        public override void HandleCursorMove(Point position, CursorMode mode)
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

        public override bool HandleRightClick(Point position)
        {
            bool result = false;
            foreach (var control in SubControls.Reverse())
            {
                Point sub = control.ParentPos2SubPos(position);
                if (control is Form form)
                {
                    if (form.ResizeBorder != PlaneFacing.None)
                    {
                        form.Resizeing = !form.Resizeing;
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

        public override bool HandleLeftClick(Point position)
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

        public override void HandleTextEditorUpdate(Point position, string text)
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
