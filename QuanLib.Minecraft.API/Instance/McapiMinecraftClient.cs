using QuanLib.Core;
using QuanLib.Minecraft.API.Packet;
using QuanLib.Minecraft.Command.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Instance
{
    public class McapiMinecraftClient : Minecraft.Instance.MinecraftClient, IMcapiInstance
    {
        public McapiMinecraftClient(string clientPath, string serverAddress, ushort mcapiPort, string mcapiPassword) : base(clientPath)
        {
            if (string.IsNullOrEmpty(serverAddress))
                throw new ArgumentException($"“{nameof(serverAddress)}”不能为 null 或空。", nameof(serverAddress));
            if (string.IsNullOrEmpty(mcapiPassword))
                throw new ArgumentException($"“{nameof(mcapiPassword)}”不能为 null 或空。", nameof(mcapiPassword));

            ServerAddress = IPAddress.Parse(serverAddress);
            McapiPort = mcapiPort;
            McapiPassword = mcapiPassword;
            McapiClient = new(ServerAddress, McapiPort);
            McapiCommandSender = new(McapiClient);
            CommandSender = new(McapiCommandSender, McapiCommandSender);
        }

        public IPAddress ServerAddress { get; }

        public ushort McapiPort { get; }

        public string McapiPassword { get; }

        public McapiClient McapiClient { get; }

        public McapiCommandSender McapiCommandSender { get; }

        public override CommandSender CommandSender { get; }

        public override string InstanceKey => IMcapiInstance.INSTANCE_KEY;

        protected override void Run()
        {
            LogFileListener.Start();
            McapiClient.Start();
            McapiClient.LoginAsync(McapiPassword).Wait();

            Task.WaitAll(LogFileListener.WaitForStopAsync(), McapiClient.WaitForStopAsync());
        }

        protected override void DisposeUnmanaged()
        {
            LogFileListener.Stop();
            McapiClient.Stop();
        }

        public override bool TestConnection()
        {
            return NetworkUtil.TestTcpConnection(ServerAddress, McapiPort);
        }
    }
}
