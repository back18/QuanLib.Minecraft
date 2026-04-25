using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft
{
    public class PlayerInfo : IPlayerInfo
    {
        public PlayerInfo(string name, Guid uuid)
        {
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

            Name = name;
            Uuid = uuid;
        }

        public PlayerInfo(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            Name = model.name;
            Uuid = Guid.Parse(model.uuid);
        }

        public string Name { get; }

        public Guid Uuid { get; }

        public override string ToString()
        {
            return $"{Name} ({Uuid})";
        }

        public class Model
        {
            public required string name { get; set; }

            public required string uuid { get; set; }
        }
    }
}
