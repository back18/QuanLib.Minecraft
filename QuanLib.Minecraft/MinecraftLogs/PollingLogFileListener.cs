using log4net.Core;
using QuanLib.Core;
using QuanLib.Core.Events;
using QuanLib.Core.FileListeners;
using QuanLib.Minecraft.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.MinecraftLogs
{
    public class PollingLogFileListener : PollingTextFileListener, ILogListener
    {
        public PollingLogFileListener(string path, Func<Type, LogImpl> logger) : base(path, GetEncoding(), logger)
        {
            _temp = new();
            _count = 0;
            WriteLog += OnWriteLog;
        }

        private readonly StringBuilder _temp;

        private int _count;

        public event EventHandler<ILogListener, MinecraftLogEventArgs> WriteLog;

        protected override void OnPolling(PollingFileListener sender, FileInfoChangedEventArge e)
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

        protected override void OnWriteLineText(ITextListener sender, TextEventArgs e)
        {
            base.OnWriteLineText(sender, e);

            if (e.Text.StartsWith('[') && _temp.Length > 0)
            {
                WriteLog.Invoke(this, new(new(_temp.ToString())));
                _temp.Clear();
                _count = 0;
            }

            _temp.AppendLine(e.Text);
        }

        protected virtual void OnWriteLog(ILogListener sender, MinecraftLogEventArgs e) { }

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
