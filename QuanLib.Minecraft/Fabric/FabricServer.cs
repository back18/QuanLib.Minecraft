using QuanLib.Minecraft.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Fabric
{
    public class FabricServer : MinecraftServer
    {
        public FabricServer(string serverPath, string serverAddress) : base(serverPath, serverAddress)
        {
            OnWriteLog += LogListener_OnWriteLog;
            OnServerStarting += () => { };
            OnPreparingLeveling += (obj) => { };
            OnPreparingLevelDone += (obj) => { };
            OnServerStartFail += (obj) => { };
            OnServerCrash += (obj) => { };
            OnServerStoping += () => { };
            OnServerStopped += () => { };
            OnPlayerJoined += (obj) => { };
            OnPlayerLeft += (arg1, arg2) => { };
            OnPlayerSendChatMessage += (obj) => { };
            OnRconRunning += (obj) => { };
            OnRconStopped += () => { };
        }

        public override event Action OnServerStarting;

        public override event Action<string> OnPreparingLeveling;

        public override event Action<TimeSpan> OnPreparingLevelDone;

        public override event Action<string> OnServerStartFail;

        public override event Action<Guid> OnServerCrash;

        public override event Action OnServerStoping;

        public override event Action OnServerStopped;

        public override event Action<PlayerLoginInfo> OnPlayerJoined;

        public override event Action<PlayerLoginInfo?, string> OnPlayerLeft;

        public override event Action<ChatMessage> OnPlayerSendChatMessage;

        public override event Action<IPEndPoint> OnRconRunning;

        public override event Action OnRconStopped;

        private void LogListener_OnWriteLog(MinecraftLog log)
        {
            if (log.Message.StartsWith("RCON running") && IPEndPoint.TryParse(log.Message.Split(' ')[^1], out var ipPort))
                OnRconRunning.Invoke(ipPort);
        }
    }
}
