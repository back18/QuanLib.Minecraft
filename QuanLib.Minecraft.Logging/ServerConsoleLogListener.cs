using QuanLib.Core;
using QuanLib.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging
{
    public class ServerConsoleLogListener : ILogListener
    {
        public ServerConsoleLogListener(ServerConsole owner)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));

            _owner = owner;
            _logCache = new();

            WriteLog += OnWriteLog;

            _owner.ReadLine += ServerConsole_ReadLine;
        }

        private readonly ServerConsole _owner;

        private readonly StringBuilder _logCache;

        public event ValueEventHandler<ILogListener, ValueEventArgs<MinecraftLog>> WriteLog;

        protected virtual void OnWriteLog(ILogListener sender, ValueEventArgs<MinecraftLog> e) { }

        private void ServerConsole_ReadLine(ServerConsole sender, ValueEventArgs<string> e)
        {
            if (e.Argument.StartsWith('[') && _logCache.Length > 0)
            {
                string log = _logCache.ToString();
                HandleWriteLog(log);
                _logCache.Clear();
            }

            if (_logCache.Length > 0)
                _logCache.AppendLine();
            _logCache.Append(e.Argument);

            if (!_owner.ReadAvailable)
            {
                string log = _logCache.ToString();
                HandleWriteLog(log);
                _logCache.Clear();
            }
        }

        private void HandleWriteLog(string log)
        {
            if (MinecraftLog.TryParse(log, out var result))
                WriteLog.Invoke(this, new(result));
        }
    }
}
