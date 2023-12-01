using QuanLib.Core;
using QuanLib.Minecraft.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Senders
{
    public class CommandSender
    {
        public CommandSender(ITwowayCommandSender twowaySender, IOnewayCommandSender onewaySender)
        {
            ArgumentNullException.ThrowIfNull(twowaySender, nameof(twowaySender));
            ArgumentNullException.ThrowIfNull(onewaySender, nameof(onewaySender));

            TwowaySender = twowaySender;
            OnewaySender = onewaySender;

            CommandSent += OnCommandSent;
        }

        public ITwowayCommandSender TwowaySender { get; }

        public IOnewayCommandSender OnewaySender { get; }

        public event EventHandler<CommandSender, CommandInfoEventArgs> CommandSent;

        protected virtual void OnCommandSent(CommandSender sender, CommandInfoEventArgs e) { }

        public string SendCommand(string command)
        {
            DateTime sendingTime = DateTime.Now;
            string output = TwowaySender.SendCommand(command);
            DateTime receivingTime = DateTime.Now;
            CommandInfo commandInfo = new(sendingTime, receivingTime, command, output);
            CommandSent.Invoke(this, new(commandInfo));
            return output;
        }

        public async Task<string> SendCommandAsync(string command)
        {
            DateTime sendingTime = DateTime.Now;
            string output = await TwowaySender.SendCommandAsync(command);
            DateTime receivingTime = DateTime.Now;
            CommandInfo commandInfo = new(sendingTime, receivingTime, command, output);
            CommandSent.Invoke(this, new(commandInfo));
            return output;
        }
    }
}
