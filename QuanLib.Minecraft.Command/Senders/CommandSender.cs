using QuanLib.Core;
using QuanLib.Minecraft.Command.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Senders
{
    public class CommandSender
    {
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;

        public CommandSender(ITwowayCommandSender twowaySender, IOnewayCommandSender onewaySender)
        {
            ArgumentNullException.ThrowIfNull(twowaySender, nameof(twowaySender));
            ArgumentNullException.ThrowIfNull(onewaySender, nameof(onewaySender));

            TwowaySender = twowaySender;
            OnewaySender = onewaySender;

            CommandSent += OnCommandSent;
        }

        private static readonly double s_tickFrequency = (double)TicksPerSecond / Stopwatch.Frequency;

        public ITwowayCommandSender TwowaySender { get; }

        public IOnewayCommandSender OnewaySender { get; }

        public event ValueEventHandler<CommandSender, CommandInfoEventArgs> CommandSent;

        protected virtual void OnCommandSent(CommandSender sender, CommandInfoEventArgs e) { }

        public string SendCommand(string command)
        {
            long startTimeStamp = Stopwatch.GetTimestamp();
            string output = TwowaySender.SendCommand(command);
            long endTimeStamp = Stopwatch.GetTimestamp();

            if (Stopwatch.Frequency != TicksPerSecond)
            {
                startTimeStamp = (long)(startTimeStamp * s_tickFrequency);
                endTimeStamp = (long)(endTimeStamp * s_tickFrequency);
            }

            CommandSent.Invoke(this, new(new(command, output, startTimeStamp, endTimeStamp)));
            return output;
        }

        public async Task<string> SendCommandAsync(string command)
        {
            long startTimeStamp = Stopwatch.GetTimestamp();
            string output = await TwowaySender.SendCommandAsync(command);
            long endTimeStamp = Stopwatch.GetTimestamp();

            if (Stopwatch.Frequency != TicksPerSecond)
            {
                startTimeStamp = (long)(startTimeStamp * s_tickFrequency);
                endTimeStamp = (long)(endTimeStamp * s_tickFrequency);
            }

            CommandSent.Invoke(this, new(new(command, output, startTimeStamp, endTimeStamp)));
            return output;
        }
    }
}
