using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public static class PacketKey
    {
        public const string Empty = nameof(Empty);

        public const string Login = nameof(Login);

        public const string Command = nameof(Command);

        public const string BatchCommand = nameof(BatchCommand);

        public const string BatchSetBlock = nameof(BatchSetBlock);

        public const string Event = nameof(Event);
    }
}
