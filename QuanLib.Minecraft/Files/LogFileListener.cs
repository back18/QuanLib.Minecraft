using QuanLib.FileListeners;
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
            OnWriteLine += MinecraftLogListener_OnWriteLine;
        }

        private readonly StringBuilder _temp;

        public event Action<MinecraftLog> OnWriteLog = (obj) => { };

        private void MinecraftLogListener_OnWriteLine(string line)
        {
            if (line.StartsWith('['))
            {
                if (_temp.Length > 0)
                {
                    _temp.Remove(_temp.Length - 1, 1);
                    if (MinecraftLog.TryParse(_temp.ToString(), out var result1))
                        OnWriteLog.Invoke(result1);
                    _temp.Clear();
                }

                if (MinecraftLog.TryParse(line, out var result2))
                {
                    if (result2.Level != Level.ERROR)
                    {
                        OnWriteLog.Invoke(result2);
                    }
                    else
                    {
                        _temp.Append(line);
                        _temp.Append('\n');
                    }
                }
            }
            else if (_temp.Length > 0 && _temp[0] == '[')
            {
                _temp.Append(line);
                _temp.Append('\n');
            }
            else
            {

            }
        }
    }
}
