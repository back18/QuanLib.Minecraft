using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command
{
    public class CommandInfo
    {
        public CommandInfo(DateTime sendingTime, DateTime receivingTime, string input, string output)
        {
            SendingTime = sendingTime;
            ReceivingTime = receivingTime;
            Input = input;
            Output = output;
        }

        public DateTime SendingTime { get; }

        public DateTime ReceivingTime { get; }

        public string Input { get; }

        public string Output { get; }
    }
}
