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
        public PollingLogFileListener(string path, ILoggerGetter? loggerGetter = null) : base(path, GetEncoding(), loggerGetter)
        {
            _temp = new();
            _count = 0;
            WriteLog += OnWriteLog;
        }

        private readonly StringBuilder _temp;

        private int _count;

        public event EventHandler<ILogListener, EventArgs<MinecraftLog>> WriteLog;

        protected override void OnPolling(PollingFileListener sender, ValueChangedEventArgs<FileInfo> e)
        {
            base.OnPolling(sender, e);

            if (_temp.Length > 0)
            {
                _count++;
                if (_count >= 2)
                {
                    WriteLog.Invoke(this, new(new(_temp.ToString())));
                    _temp.Clear();
                    _count = 0;
                }
            }
        }

        protected override void OnWriteLineText(ITextListener sender, EventArgs<string> e)
        {
            base.OnWriteLineText(sender, e);

            if (e.Argument.StartsWith('[') && _temp.Length > 0)
            {
                WriteLog.Invoke(this, new(new(_temp.ToString())));
                _temp.Clear();
                _count = 0;
            }

            _temp.AppendLine(e.Argument);
        }

        protected virtual void OnWriteLog(ILogListener sender, EventArgs<MinecraftLog> e) { }

        private static Encoding GetEncoding()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return Encoding.GetEncoding("GBK");
            }
            else
            {
                return Encoding.UTF8;
            }
        }
    }
}
