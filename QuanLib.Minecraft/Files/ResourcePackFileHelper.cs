using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class ResourcePackFileHelper
    {
        public ResourcePackFileHelper(ResourcePackPathManager paths)
        {
            _paths = paths ?? throw new ArgumentNullException(nameof(paths));
        }

        private readonly ResourcePackPathManager _paths;
    }
}
