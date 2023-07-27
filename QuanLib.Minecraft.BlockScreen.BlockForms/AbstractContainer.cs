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
            OnAddedSubControl += ContainerControl_OnAddSubControl;
            OnLayoutAll += LayoutAll;

            OnResize += ControlContainer_OnResize;
        }

        public Type SubControlType => typeof(TControl);

        public bool IsSubControlType<T>() => typeof(T) == SubControlType;

        public abstract event Action<TControl> OnAddedSubControl;

        public abstract event Action<TControl> OnRemovedSubControl;

        public event Action<Size, Size> OnLayoutAll;

        public abstract IReadOnlyControlCollection<TControl> GetSubControls();

        IReadOnlyControlCollection<IControl> IContainerControl.GetSubControls()
        {
            return GetSubControls();
        }

        private void ContainerControl_OnAddSubControl(IControl control)
        {
            IControlInitializeHandling handling = control;
            if (InitializeCompleted && !handling.InitializeCompleted)
                handling.HandleAllInitialize();
        }

        private void ControlContainer_OnResize(Size oldSize, Size newSize)
        {
            OnLayoutAll.Invoke(oldSize, newSize);
        }

        public virtual void LayoutAll(Size oldSize, Size newSize)
        {
            foreach (var control in GetSubControls())
            {
                control.Layout(oldSize, newSize);
            }
        }

        public override void HandleInitialize()
        {
            base.HandleInitialize();

            foreach (var control in GetSubControls())
            {
                control.HandleInitialize();
            }
        }

        public override void HandleOnInitCompleted1()
        {
            base.HandleOnInitCompleted1();

            foreach (var control in GetSubControls())
            {
                control.HandleOnInitCompleted1();
            }
        }

        public override void HandleOnInitCompleted2()
        {
            base.HandleOnInitCompleted2();

            foreach (var control in GetSubControls())
            {
                control.HandleOnInitCompleted2();
            }
        }

        public override void HandleOnInitCompleted3()
        {
            base.HandleOnInitCompleted3();

            foreach (var control in GetSubControls())
            {
                control.HandleOnInitCompleted3();
            }
        }

        public override void HandleCursorMove(Point position)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleCursorMove(control.ParentPos2SubPos(position));
            }

            base.HandleCursorMove(position);
        }

        public override bool HandleRightClick(Point position)
        {
            TControl? control = GetSubControls().FirstHover;
            control?.HandleRightClick(control.ParentPos2SubPos(position));

            return base.HandleRightClick(position);
        }

        public override bool HandleLeftClick(Point position)
        {
            TControl? control = GetSubControls().FirstHover;
            control?.HandleLeftClick(control.ParentPos2SubPos(position));

            return base.HandleLeftClick(position);
        }

        public override void HandleTextEditorUpdate(Point position, string text)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleTextEditorUpdate(control.ParentPos2SubPos(position), text);
            }

            base.HandleTextEditorUpdate(position, text);
        }

        public override void HandleBeforeFrame()
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleBeforeFrame();
            }

            base.HandleBeforeFrame();
        }

        public override void HandleAfterFrame()
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleAfterFrame();
            }

            base.HandleAfterFrame();
        }
    }
}
