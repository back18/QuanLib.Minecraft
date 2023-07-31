using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IControl : IControlInitializeHandling, IControlEventHandling, IControlRendering, IComparable<IControl>
    {
        public IContainerControl? GenericParentContainer { get; }

        public bool IsHover { get; }

        public bool IsSelected { get; set; }

        public int DisplayPriority { get; set; }

        public Point ParentPos2SubPos(Point position);

        public Point SubPos2ParentPos(Point position);

        public void SetGenericContainerControl(IContainerControl? container);
    }
}
