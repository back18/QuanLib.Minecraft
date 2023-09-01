using CoreRCON;
using CoreRCON.Parsers.Standard;
using QuanLib.Core;
using QuanLib.Core.FileListeners;
using QuanLib.Minecraft.DirectoryManagers;
using QuanLib.Minecraft.MinecraftLogs;
using QuanLib.Minecraft.Selectors;
using QuanLib.Minecraft.Snbt;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public abstract class MinecraftServer : ISwitchable
    {
        protected MinecraftServer(string serverPath, string serverAddress)
        {
            if (string.IsNullOrEmpty(serverPath))
                throw new ArgumentException($"“{nameof(serverPath)}”不能为 null 或空。", nameof(serverPath));
            if (string.IsNullOrEmpty(serverAddress))
                throw new ArgumentException($"“{nameof(serverAddress)}”不能为 null 或空。", nameof(serverAddress));

            ServerDirectory = new(serverPath);

            Dictionary<string, string> properties = ServerDirectory.ReadServerProperties();
            if (properties["enable-rcon"] != "true")
                throw new InvalidOperationException("需要在 server.properties 中将 enable-rcon 设置为 true");
            if (!ushort.TryParse(properties["server-port"], out var serverPort))
                throw new InvalidOperationException($"需要在 server.properties 中为 server-port 设置一个 {ushort.MinValue} 到 {ushort.MaxValue} 之间的有效端口");
            if (!ushort.TryParse(properties["rcon.port"], out var rconPort))
                throw new InvalidOperationException($"需要在 server.properties 中为 rcon.port 设置一个 {ushort.MinValue} 到 {ushort.MaxValue} 之间的有效端口");
            if (string.IsNullOrEmpty(properties["rcon.password"]))
                throw new InvalidOperationException($"需要在 server.properties 中为 rcon.password 设置一个非空密码");

            ServerAddress = serverAddress;
            ServerPort = serverPort;
            RconPort = rconPort;
            RconPassword = properties["rcon.password"];
        }

        public string ServerAddress { get; }

        public ushort ServerPort { get; }

        public ushort RconPort { get; }

        public string RconPassword { get; }

        public MinecraftServerDirectory ServerDirectory { get; }

        public abstract ITextListener TextListener { get; }

        public abstract ILogListener LogListener { get; }

        public abstract ServerLogParser LogParser { get; }

        public abstract ServerCommandHelper CommandHelper { get; }

        public abstract ICommandSender CommandSender { get; }

        public abstract MinecraftServerMode Mode { get; }

        public abstract bool Connected { get; }

        public abstract bool Runing { get; }

        public bool PingServer(out TimeSpan time)
        {
            return Ping(ServerAddress, ServerPort, out time);
        }

        public bool PingRcon(out TimeSpan time)
        {
            return Ping(ServerAddress, RconPort, out time);
        }

        private static bool Ping(string hostname, int port, out TimeSpan time)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            TcpClient client = new();
            try
            {
                client.Connect(hostname, port);
            }
            catch
            {
                time = TimeSpan.Zero;
                return false;
            }
            finally
            {
                if (client.Connected)
                    client.Close();
            }
            stopwatch.Stop();
            time = stopwatch.Elapsed;
            return true;
        }

        public virtual void WaitForServerStartup()
        {
            while (!PingServer(out var _))
            {
                Thread.Sleep(1000);
            }
        }

        public virtual void WaitForRconStartup()
        {
            while (!PingRcon(out var _))
            {
                Thread.Sleep(1000);
            }
        }

        public void EnableConsoleOutput()
        {
            TextListener.WriteLineText += TextListener_WriteLineText;
        }

        public void DisableConsoleOutput()
        {
            TextListener.WriteLineText -= TextListener_WriteLineText;
        }

        private void TextListener_WriteLineText(ITextListener sender, QuanLib.Core.Event.TextEventArgs e)
        {
            Console.WriteLine(e.Text);
        }

        public abstract void WaitForConnected();

        public abstract void Start();

        public abstract void Stop();
    }
}
