using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Notepad
{
    public class NotepadApp : Application
    {
        public const string ID = "Notepad";

        public const string Name = "记事本";

        public override object? Main(string[] args)
        {
            string? path = null;
            if (args.Length > 0)
                path = args[0];

            RunForm(new NotepadForm(path));
            return null;
        }
    }
}
