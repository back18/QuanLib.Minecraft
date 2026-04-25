using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QuanLib.Minecraft
{
    public class BannedIpAddress : BannedInfo
    {
        public BannedIpAddress(IPAddress ip, string source, string reason, DateTimeOffset created, DateTimeOffset expires) : base(source, reason, created, expires)
        {
            ArgumentNullException.ThrowIfNull(ip, nameof(ip));

            IP = ip;
        }

        public BannedIpAddress(Model model) : base(model)
        {
            IP = IPAddress.Parse(model.ip);
        }

        public IPAddress IP { get; }

        public new class Model : BannedInfo.Model
        {
            public required string ip { get; set; }
        }
    }
}
