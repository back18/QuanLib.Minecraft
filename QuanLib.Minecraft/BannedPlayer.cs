using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft
{
    public class BannedPlayer : BannedInfo, IPlayerInfo
    {
        public BannedPlayer(string name, Guid uuid, string source, string reason, DateTimeOffset created, DateTimeOffset expires) : base(source, reason, created, expires)
        {
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

            Name = name;
            Uuid = uuid;
        }

        public BannedPlayer(Model model) : base(model)
        {
            Name = model.name;
            Uuid = Guid.Parse(model.uuid);
        }

        public string Name { get; }

        public Guid Uuid { get; }

        public new class Model : BannedInfo.Model
        {
            public required string name { get; set; }

            public required string uuid { get; set; }
        }
    }
}
