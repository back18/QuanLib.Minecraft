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
            if (string.IsNullOrWhiteSpace(arguments))
                throw new ArgumentException($"“{nameof(arguments)}”不能为 null 或空白。", nameof(arguments));

            Arguments = arguments;
        }

        public string Arguments { get; }

        public override string GetArguments()
        {
            return Arguments;
        }
    }
}
