using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging.LogEvent
{
    public interface ILogEventPublisher
    {
        public bool Match(MinecraftLog log);

        public bool TryParse(string message, [MaybeNullWhen(false)] out EventArgs result);

        public void TriggerEvent(EventArgs e);
    }
}
