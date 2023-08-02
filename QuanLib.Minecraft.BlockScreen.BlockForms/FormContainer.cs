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
    public class FormContainer : GenericPanel<IForm>
    {
        public FormContainer(RootForm owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            BorderWidth = 0;
            LayoutSyncer = new(_owner,
            (sender, e) => { },
            (sender, e) =>
            {
                if (_owner.ShowTitleBar)
                {
                    ClientSize = new(e.NewSize.Width, e.NewSize.Height - 16);
                    foreach (var form in SubControls)
                        form.ClientSize = new(form.ClientSize.Width, form.ClientSize.Height - 16);
                }
                else
                {
                    ClientSize = new(e.NewSize.Width, e.NewSize.Height);
                    foreach (var form in SubControls)
                        form.ClientSize = new(form.ClientSize.Width, form.ClientSize.Height + 16);
                }
            });
        }

        private readonly RootForm _owner;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();
        }

        public override void HandleCursorMove(CursorEventArgs e)
        {
            foreach (var control in SubControls.Reverse())
            {
                if (control.IsSelected)
                {
                    control.HandleCursorMove(new(control.ParentPos2SubPos(e.Position)));
                }
            }

            UpdateHoverState(e);
        }

        public override bool HandleRightClick(CursorEventArgs e)
        {
            bool result = false;
            foreach (var control in SubControls.Reverse())
            {
                Point sub = control.ParentPos2SubPos(e.Position);
                if (control is Form form)
                {
                    if (form.ResizeBorder != Direction.None)
                    {
                        form.Resizeing = !form.Resizeing;
                        result = true;
                        break;
                    }
                    else if (form.IncludedOnControl(sub))
                    {
                        if (form.IsSelected)
                            form.HandleRightClick(new(sub));
                        else
                            _owner.TrySwitchSelectedForm(form);
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
                return SubControls.FirstHover.HandleRightClick(new(SubControls.FirstHover.ParentPos2SubPos(e.Position)));
        }

        public override bool HandleLeftClick(CursorEventArgs e)
        {
            bool result = false;
            foreach (var control in SubControls.Reverse())
            {
                if (control.IsSelected)
                {
                    result = control.HandleLeftClick(new(control.ParentPos2SubPos(e.Position)));
                }
            }

            return result;
        }

        public override void HandleCursorSlotChanged(CursorSlotEventArgs e)
        {
            foreach (var control in SubControls.Reverse())
            {
                if (control.IsSelected)
                {
                    control.HandleCursorSlotChanged(new(control.ParentPos2SubPos(e.Position), e.OldSlot, e.NewSlot));
                }
            }
        }

        public override void HandleCursorItemChanged(CursorItemEventArgs e)
        {
            foreach (var control in SubControls.Reverse())
            {
                if (control.IsSelected)
                {
                    control.HandleCursorItemChanged(new(control.ParentPos2SubPos(e.Position), e.Item));
                }
            }
        }

        public override void HandleTextEditorChanged(CursorTextEventArgs e)
        {
            foreach (var control in SubControls.Reverse())
            {
                if (control.IsSelected)
                {
                    control.HandleTextEditorChanged(new(control.ParentPos2SubPos(e.Position), e.Text));
                }
            }
        }
    }
}
