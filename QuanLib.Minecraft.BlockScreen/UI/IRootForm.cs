using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IRootForm : IForm
    {
        public Size FormContainerSize { get; }

        public void AddForm(IForm form);

        public bool RemoveForm(IForm form);

        public bool ContainsForm(IForm form);

        public IEnumerable<IForm> GetAllForm();

        public bool TrySwitchSelectedForm(IForm form);

        public void SelectedMaxDisplayPriority();
    }
}
