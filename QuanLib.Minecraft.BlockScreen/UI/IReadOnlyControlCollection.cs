using QuanLib.Minecraft.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IReadOnlyControlCollection<out T> : IReadOnlyList<T> where T : class, IControl
    {
        public T? FirstHover { get; }

        public T? FirstSelected { get; }

        public T? RecentlyAddedControl { get; }

        public T? RecentlyRemovedControl { get; }

        public bool HaveHover { get; }

        public bool HaveSelected { get; }

        public IReadOnlyList<T> GetHovers();

        public IReadOnlyList<T> GetSelecteds();

        public void Sort();

        public T[] ToArray();
    }
}
