using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class SimpleServerLaunchArguments : ServerLaunchArguments
    {
        public SimpleServerLaunchArguments(string javaPath, string launchTarget, int xms = 0, int xmx = 0, bool enableGui = false, IList<string>? addonArgs = null)
            : base(javaPath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(launchTarget, nameof(launchTarget));

            LaunchTarget = launchTarget;
            Xms = xms;
            Xmx = xmx;
            EnableGui = enableGui;
            AddonArgs = addonArgs is null ? null : new(addonArgs);
        }

        public string LaunchTarget { get; }

        public int Xms { get; }

        public int Xmx { get; }

        public bool EnableGui { get; }

        public ReadOnlyCollection<string>? AddonArgs { get; }

        public override string GetArguments()
        {
            List<string> arguments = new();
            if (Xms > 0)
                arguments.Add($"-Xms{Xms}M");
            if (Xmx > 0)
                arguments.Add($"-Xmx{Xmx}M");
            if (Path.GetExtension(LaunchTarget) == ".jar")
                arguments.Add("-jar");
            arguments.Add(LaunchTarget);
            if (!EnableGui)
                arguments.Add("-nogui");
            if (AddonArgs is not null)
                arguments.AddRange(AddonArgs);

            return string.Join(' ', arguments);
        }
    }
}
