using QuanLib.Event;
using QuanLib.FileListeners;
using QuanLib.Minecraft.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class LogFileListener : TextFileListener, ILogListener
    {
        public LogFileListener(string path) : base(path, GetEncoding())
        {
            _temp = new();
            WriteLog += OnWriteLog;
        }

        private readonly StringBuilder _temp;

        public event EventHandler<ILogListener, MinecraftLogEventArgs> WriteLog;

        protected virtual void OnWriteLog(ILogListener sender, MinecraftLogEventArgs e) { }

        protected override void OnWriteLineText(TextFileListener sender, TextEventArgs e)
        {
            base.OnWriteLineText(sender, e);

            if (e.Text.StartsWith('['))
            {
                WriteLog.Invoke(this, new(new(e.Text)));
            }
        }

        private static Encoding GetEncoding()
        {
            Encoding encoding = Encoding.UTF8;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                encoding = Encoding.GetEncoding("GBK");
            }
            return encoding;
        }
    }
}
