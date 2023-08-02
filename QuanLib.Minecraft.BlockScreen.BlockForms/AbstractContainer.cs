using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class AbstractContainer<TControl> : Control, IContainerControl where TControl : class, IControl
    {
        protected AbstractContainer()
        {
            AddedSubControl += OnAddedSubControl;
            RemovedSubControl += OnRemovedSubControl;
        }

        public Type SubControlType => typeof(TControl);

        public bool IsSubControlType<T>() => typeof(T) == SubControlType;

        public abstract event EventHandler<AbstractContainer<TControl>, ControlEventArgs<TControl>> AddedSubControl;

        public abstract event EventHandler<AbstractContainer<TControl>, ControlEventArgs<TControl>> RemovedSubControl;

        protected virtual void OnAddedSubControl(AbstractContainer<TControl> sender, ControlEventArgs<TControl> e)
        {
            IControlInitializeHandling handling = e.Control;
            if (InitializeCompleted && !handling.InitializeCompleted)
                handling.HandleAllInitialize();
        }

        protected virtual void OnRemovedSubControl(AbstractContainer<TControl> sender, ControlEventArgs<TControl> e)
        {

        }

        public abstract IReadOnlyControlCollection<TControl> GetSubControls();

        IReadOnlyControlCollection<IControl> IContainerControl.GetSubControls()
        {
            return GetSubControls();
        }

        public override void HandleInitialize()
        {
            base.HandleInitialize();

            foreach (var control in GetSubControls())
            {
                control.HandleInitialize();
            }
        }

        public override void HandleInitCompleted1()
        {
            base.HandleInitCompleted1();

            foreach (var control in GetSubControls())
            {
                control.HandleInitCompleted1();
            }
        }

        public override void HandleInitCompleted2()
        {
            base.HandleInitCompleted2();

            foreach (var control in GetSubControls())
            {
                control.HandleInitCompleted2();
            }
        }

        public override void HandleInitCompleted3()
        {
            base.HandleInitCompleted3();

            foreach (var control in GetSubControls())
            {
                control.HandleInitCompleted3();
            }
        }

        public override void HandleCursorMove(CursorEventArgs e)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleCursorMove(new(control.ParentPos2SubPos(e.Position)));
            }

            base.HandleCursorMove(e);
        }

        public override bool HandleRightClick(CursorEventArgs e)
        {
            TControl? control = GetSubControls().FirstHover;
            control?.HandleRightClick(new(control.ParentPos2SubPos(e.Position)));

            return base.HandleRightClick(e);
        }

        public override bool HandleLeftClick(CursorEventArgs e)
        {
            TControl? control = GetSubControls().FirstHover;
            control?.HandleLeftClick(new(control.ParentPos2SubPos(e.Position)));

            return base.HandleLeftClick(e);
        }

        public override void HandleCursorSlotChanged(CursorSlotEventArgs e)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleCursorSlotChanged(new(control.ParentPos2SubPos(e.Position), e.OldSlot, e.NewSlot));
            }

            base.HandleCursorSlotChanged(e);
        }

        public override void HandleCursorItemChanged(CursorItemEventArgs e)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleCursorItemChanged(new(control.ParentPos2SubPos(e.Position), e.Item));
            }

            base.HandleCursorItemChanged(e);
        }

        public override void HandleTextEditorChanged(CursorTextEventArgs e)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleTextEditorChanged(new(control.ParentPos2SubPos(e.Position), e.Text));
            }

            base.HandleTextEditorChanged(e);
        }

        public override void HandleBeforeFrame(EventArgs e)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleBeforeFrame(e);
            }

            base.HandleBeforeFrame(e);
        }

        public override void HandleAfterFrame(EventArgs e)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleAfterFrame(e);
            }

            base.HandleAfterFrame(e);
        }
    }
}
