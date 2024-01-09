using QuanLib.Core;
using QuanLib.Minecraft.Command;
using QuanLib.Minecraft.Command.Models;
using QuanLib.Minecraft.CommandSenders;
using QuanLib.Minecraft.Directorys;
using QuanLib.Minecraft.MinecraftLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public abstract class MinecraftInstance : UnmanagedRunnable
    {
        protected MinecraftInstance(string minecraftPath, ILogbuilder? logbuilder = null) : base(logbuilder)
        {
            ArgumentException.ThrowIfNullOrEmpty(minecraftPath, nameof(minecraftPath));

            MinecraftPath = minecraftPath;
            MinecraftDirectory = new(MinecraftPath);
            LogFileListener = new(MinecraftDirectory.LogsDir.LatestFile, logbuilder);
            LogParser = new(LogFileListener);
        }

        public string MinecraftPath { get; }

        public abstract string InstanceKey { get; }

        public abstract InstanceType InstanceType { get; }

        public virtual MinecraftDirectory MinecraftDirectory { get; }

        public virtual MinecraftLogParser LogParser { get; }

        public virtual PollingLogFileListener LogFileListener { get; }

        public abstract CommandSender CommandSender { get; }

        public abstract bool TestConnectivity();

        public abstract Task<bool> TestConnectivityAsync();

        public virtual void WaitForConnection()
        {
            while (!TestConnectivity())
            {
                Thread.Sleep(1000);
            }
        }

        public virtual async Task WaitForConnectionAsync()
        {
            while (!await TestConnectivityAsync())
            {
                await Task.Delay(1000);
            }
        }

        public virtual int GetGameTick()
        {
            TimeQueryGametimeCommand command = CommandManager.TimeQueryGametimeCommand;

            if (!command.Input.TryFormat(Array.Empty<object>(), out var input))
                return 0;

            string output = CommandSender.SendCommand(input);

            if (!command.Output.TryMatch(output, out var outargs))
                return 0;

            if (outargs is null || outargs.Length != 1 || !int.TryParse(outargs[0], out var result))
                return 0;

            return result;
        }

        public virtual async Task<int> GetGameTickAsync()
        {
            TimeQueryGametimeCommand command = CommandManager.TimeQueryGametimeCommand;

            if (!command.Input.TryFormat(Array.Empty<object>(), out var input))
                return 0;

            string output = await CommandSender.SendCommandAsync(input);

            if (!command.Output.TryMatch(output, out var outargs))
                return 0;

            if (outargs is null || outargs.Length != 1 || !int.TryParse(outargs[0], out var result))
                return 0;

            return result;
        }
    }
}
