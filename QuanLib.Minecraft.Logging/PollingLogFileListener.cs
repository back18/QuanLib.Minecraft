using QuanLib.Core;
using QuanLib.Core.Events;
using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging
{
    public class PollingLogFileListener : PollingTextFileListener, ILogListener
    {
        public PollingLogFileListener(string path, Encoding? encoding = null, int delayMilliseconds = 500, ILoggerGetter? loggerGetter = null)
            : base(path, encoding ?? Encoding.UTF8, delayMilliseconds, loggerGetter)
        {
            _logCache = new();
            _pollingCount = 0;
            WriteLog += OnWriteLog;
        }

        private readonly StringBuilder _logCache;

        private int _pollingCount;

        public event ValueEventHandler<ILogListener, ValueEventArgs<MinecraftLog>> WriteLog;

        protected override void OnPolling(PollingFileListener sender, ValueChangedEventArgs<FileInfo> e)
        {
            base.OnPolling(sender, e);

            if (_logCache.Length == 0)
                return;

            _pollingCount++;
            if (_pollingCount >= 2)
            {
                string log = _logCache.ToString();
                HandleWriteLog(log);
                _logCache.Clear();
                _pollingCount = 0;
            }
        }

        protected override void OnWriteLineText(ITextListener sender, ValueEventArgs<string> e)
        {
            base.OnWriteLineText(sender, e);

            if (e.Argument.StartsWith('[') && _logCache.Length > 0)
            {
                string log = _logCache.ToString();
                HandleWriteLog(log);
                _logCache.Clear();
                _pollingCount = 0;
            }

            if (_logCache.Length > 0)
                _logCache.AppendLine();
            _logCache.Append(e.Argument);
        }

        protected virtual void OnWriteLog(ILogListener sender, ValueEventArgs<MinecraftLog> e) { }

        private void HandleWriteLog(string log)
        {
            if (MinecraftLog.TryParse(log, out var result))
                WriteLog.Invoke(this, new(result));
        }
    }
}
