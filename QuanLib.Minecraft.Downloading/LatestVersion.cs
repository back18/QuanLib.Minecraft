using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class LatestVersion
    {
        public LatestVersion(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            Release = model.release;
            Snapshot = model.snapshot;
        }

        public string Release { get; }

        public string Snapshot { get; }

        public class Model
        {
            public required string release { get; set; }

            public required string snapshot { get; set; }
        }
    }
}
