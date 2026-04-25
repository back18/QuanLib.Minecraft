using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft
{
    public class OpPlayer : PlayerInfo
    {
        public OpPlayer(string name, Guid uuid, int level, bool bypassesPlayerLimit) : base(name, uuid)
        {
            Level = level;
            BypassesPlayerLimit = bypassesPlayerLimit;
        }

        public OpPlayer(Model model) : base(model)
        {
            Level = model.level;
            BypassesPlayerLimit = model.bypassesPlayerLimit;
        }

        public int Level { get; }

        public bool BypassesPlayerLimit { get; }

        public new class Model : PlayerInfo.Model
        {
            public required int level { get; set; }

            public required bool bypassesPlayerLimit { get; set; }
        }
    }
}
