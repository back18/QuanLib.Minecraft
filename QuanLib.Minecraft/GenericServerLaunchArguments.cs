using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class GenericServerLaunchArguments : ServerLaunchArguments
    {
        public GenericServerLaunchArguments(string javaPath, string arguments) : base(javaPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(arguments, nameof(arguments));

            Arguments = arguments;
        }

        public string Arguments { get; }

        public override string GetArguments()
        {
            return Arguments;
        }
    }
}
