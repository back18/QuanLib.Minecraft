using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QuanLib.Minecraft
{
    public class PlayerCache : PlayerInfo
    {
        public PlayerCache(string name, Guid uuid, DateTimeOffset expiresOn) : base(name, uuid)
        {
            ExpiresOn = expiresOn;
        }

        public PlayerCache(Model model) : base(model)
        {
            ExpiresOn = DateTimeOffset.ParseExact(model.expiresOn, "yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
        }

        public DateTimeOffset ExpiresOn { get; }

        public new class Model : PlayerInfo.Model
        {
            public required string expiresOn { get; set; }
        }
    }
}
