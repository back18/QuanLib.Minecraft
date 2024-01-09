using QuanLib.Core;
using QuanLib.Minecraft.Command.Senders;
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
    }
}
