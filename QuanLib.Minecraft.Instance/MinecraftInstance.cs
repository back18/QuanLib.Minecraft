using QuanLib.Core;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.Logging;
using QuanLib.Minecraft.PathManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public abstract class MinecraftInstance : UnmanagedRunnable
    {
        protected MinecraftInstance(string minecraftPath, ILoggerGetter? loggerGetter = null) : base(loggerGetter)
        {
            ArgumentException.ThrowIfNullOrEmpty(minecraftPath, nameof(minecraftPath));

            MinecraftPath = minecraftPath;
        }

        public string MinecraftPath { get; }

        public abstract string Identifier { get; }

        public abstract bool IsClient { get; }

        public abstract bool IsSubprocess { get; }

        public abstract MinecraftPathManager MinecraftPathManager { get; }

        public abstract CommandSender CommandSender { get; }

        public abstract ILogListener LogListener { get; }

        public abstract LogAnalyzer LogAnalyzer { get; }

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
