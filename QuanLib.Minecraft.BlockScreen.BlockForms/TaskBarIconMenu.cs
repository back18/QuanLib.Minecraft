using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class TaskBarIconMenu : TiledMenuBox<TaskBarIcon>
    {
        public TaskBarIcon? TaskBarIconOf(IForm form)
        {
            foreach (var item in _items)
                if (item.Form == form)
                    return item;
            return null;
        }

        public bool ContainsForm(IForm form)
        {
            return TaskBarIconOf(form) is not null;
        }

        public void SwitchSelectedForm(IForm form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            foreach (var item in _items)
                item.IsSelected = false;

            var icon = TaskBarIconOf(form);
            if (icon is not null)
                icon.IsSelected = true;
        }
    }
}
