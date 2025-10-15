using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class RunSyntax : CommandSyntax
    {
        public RunSyntax(ICommandSyntax? previous) : base(previous)
        {
            if (previous is not null)
                _endNode = previous;
            else
                _endNode = this;
        }

        private ICommandSyntax _endNode;

        public RunSyntax Command(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            SetSyntax(command);
            _endNode = this;
            return this;
        }

        public string Build()
        {
            List<string> parts = [];
            ICommandSyntax? current = _endNode;
            while (current is not null)
            {
                parts.Add(current.GetSyntax());
                current = current.Previous;
            }
            parts.Reverse();
            return string.Join(' ', parts);
        }

        public TCommand Push<TCommand>() where TCommand : Models.CommandBase, Models.ICreatible<TCommand>
        {
            TCommand command = CommandManager.CreateCommand<TCommand>();
            command.Execute = Build();
            return command;
        }
    }
}
