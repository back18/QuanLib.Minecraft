using QuanLib.Core;
using QuanLib.Minecraft.Command.Sender;
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
        protected MinecraftInstance(string minecraftPath)
        {
            if (string.IsNullOrEmpty(minecraftPath))
                throw new ArgumentException($"“{nameof(minecraftPath)}”不能为 null 或空。", nameof(minecraftPath));

            MinecraftPath = minecraftPath;
            MinecraftDirectory = new(MinecraftPath);
            LogFileListener = new(MinecraftDirectory.Logs.Latest);
            LogParser = new(LogFileListener);
        }

        public string MinecraftPath { get; }

        public abstract string InstanceKey { get; }

        public abstract InstanceType InstanceType { get; }

        public virtual MinecraftDirectory MinecraftDirectory { get; }

        public virtual MinecraftLogParser LogParser { get; }

        public virtual PollingLogFileListener LogFileListener { get; }

        public abstract CommandSender CommandSender { get; }

        public abstract bool TestConnection();

        public virtual void WaitForConnection()
        {
            while (!TestConnection())
            {
                Thread.Sleep(1000);
            }
        }
    }
}
