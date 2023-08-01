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
        public LogFileListener(string path, Encoding encoding) : base(path, encoding)
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
                if (_temp.Length > 0)
                {
                    _temp.Remove(_temp.Length - 1, 1);
                    if (MinecraftLog.TryParse(_temp.ToString(), out var result1))
                        WriteLog.Invoke(this, new(result1));
                    _temp.Clear();
                }

                if (MinecraftLog.TryParse(e.Text, out var result2))
                {
                    if (result2.Level != Level.ERROR)
                    {
                        WriteLog.Invoke(this, new(result2));
                    }
                    else
                    {
                        _temp.Append(e.Text);
                        _temp.Append('\n');
                    }
                }
            }
            else if (_temp.Length > 0 && _temp[0] == '[')
            {
                _temp.Append(e.Text);
                _temp.Append('\n');
            }
            else
            {

            }
        }
    }
}
