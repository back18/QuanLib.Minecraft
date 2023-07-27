using QuanLib.Minecraft.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Forge
{
    public class ForgeServer : MinecraftServer
    {
        public ForgeServer(string serverPath, string serverAddress) : base(serverPath, serverAddress)
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
            switch (log.Type)
            {
                case "cpw.mods.modlauncher.Launcher/MODLAUNCHER":
                case "cp.mo.mo.Launcher/MODLAUNCHER":
                    if (log.Message.StartsWith("ModLauncher running"))
                        OnServerStarting.Invoke();
                    break;
                case "minecraft/DedicatedServer":
                case "net.minecraft.server.dedicated.DedicatedServer/":
                    if (log.Message.StartsWith("Preparing level"))
                    {
                        Match match = Regex.Match(log.Message, "\"([^\"]*)\"");
                        if (match.Success)
                        {
                            OnPreparingLeveling.Invoke(match.Groups[1].Value);
                        }
                    }
                    else if (log.Message.StartsWith("Done"))
                    {
                        Match match = Regex.Match(log.Message, @"\((.*?)\)");
                        if (match.Success)
                        {
                            _ = TimeSpan.TryParse(match.Groups[1].Value, out var result);
                            OnPreparingLevelDone.Invoke(result);
                        }
                    }
                    break;
                case "net.minecraft.server.Main/FATAL":
                    if (log.Message.StartsWith("Failed to start the minecraft server"))
                        OnServerStartFail.Invoke(log.Message);
                    break;
                case "net.minecraftforge.common.ForgeMod/":
                    if (log.Message.StartsWith("Preparing crash report with UUID"))
                    {
                        _ = Guid.TryParse(log.Message.Split(' ')[^1], out var result);
                        OnServerCrash.Invoke(result);
                    }
                    break;
                case "minecraft/PlayerList":
                case "net.minecraft.server.players.PlayerList/":
                    if (PlayerLoginInfo.TryParse(log.Message, out var loginInfo1))
                    {
                        OnPlayerJoined.Invoke(loginInfo1);
                    }
                    break;
                case "minecraft/ServerGamePacketListenerImpl":
                case "net.minecraft.server.network.ServerGamePacketListenerImpl/":
                    Match match2 = Regex.Match(log.Message, @"^(?<name>\w+) lost connection: (?<reason>\w+)$");
                    if (match2.Success)
                    {
                        string name = match2.Groups["name"].Value;
                        string reason = match2.Groups["reason"].Value;
                        LoginInfos.TryGetValue(name, out var loginInfo2);
                        OnPlayerLeft.Invoke(loginInfo2, reason);
                    }
                    break;
                case "minecraft/MinecraftServer":
                case "net.minecraft.server.MinecraftServer":
                case "net.minecraft.server.MinecraftServer/":
                    if (log.Message.StartsWith("Stopping server"))
                        OnServerStoping.Invoke();
                    else if (log.Message.StartsWith("ThreadedAnvilChunkStorage: All dimensions are saved"))
                        OnServerStopped.Invoke();
                    Match match3 = Regex.Match(log.Message, @"<(.*?)>\s*(.*)");
                    if (match3.Success)
                    {
                        string name = match3.Groups[1].Value.Trim();
                        string message = match3.Groups[2].Value.Trim();
                        OnPlayerSendChatMessage.Invoke(new(name, message));
                    }
                    break;
                case "minecraft/RconThread":
                case "net.minecraft.server.rcon.thread.RconThread/":
                    if (log.Message.StartsWith("RCON running") && IPEndPoint.TryParse(log.Message.Split(' ')[^1], out var ipPort))
                        OnRconRunning.Invoke(ipPort);
                    break;
                case "minecraft/GenericThread":
                case "net.minecraft.server.rcon.thread.GenericThread/":
                    if (log.Message == "Thread RCON Listener stopped")
                        OnRconStopped.Invoke();
                    break;
            }
        }

        public TPS GetTps()
        {
            string output = CommandHelper.SendCommand("forge tps");
            return TPS.Parse(output.Split('\n', StringSplitOptions.RemoveEmptyEntries)[^1]);
        }

        public TPS[] GetAllDimensionTps()
        {
            string output = CommandHelper.SendCommand("forge tps");
            string[] lines = output.Split("\n", StringSplitOptions.RemoveEmptyEntries)[..^1];
            List<TPS> result = new();
            foreach (var line in lines)
            {
                result.Add(TPS.Parse(line));
            }
            return result.OrderByDescending(t => t.Mspt).ToArray();
        }
    }
}
