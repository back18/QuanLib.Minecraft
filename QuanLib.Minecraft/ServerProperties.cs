using QuanLib.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ServerProperties
    {
        public const string ENABLE_JMX_MONITORING = "enable-jmx-monitoring";
        public const string RCON_PORT = "rcon.port";
        public const string LEVEL_SEED = "level-seed";
        public const string GAMEMODE = "gamemode";
        public const string ENABLE_COMMAND_BLOCK = "enable-command-block";
        public const string ENABLE_QUERY = "enable-query";
        public const string GENERATOR_SETTINGS = "generator-settings";
        public const string ENFORCE_SECURE_PROFILE = "enforce-secure-profile";
        public const string LEVEL_NAME = "level-name";
        public const string MOTD = "motd";
        public const string QUERY_PORT = "query.port";
        public const string PVP = "pvp";
        public const string GENERATE_STRUCTURES = "generate-structures";
        public const string MAX_CHAINED_NEIGHBOR_UPDATES = "max-chained-neighbor-updates";
        public const string DIFFICULTY = "difficulty";
        public const string NETWORK_COMPRESSION_THRESHOLD = "network-compression-threshold";
        public const string MAX_TICK_TIME = "max-tick-time";
        public const string REQUIRE_RESOURCE_PACK = "require-resource-pack";
        public const string USE_NATIVE_TRANSPORT = "use-native-transport";
        public const string MAX_PLAYERS = "max-players";
        public const string ONLINE_MODE = "online-mode";
        public const string ENABLE_STATUS = "enable-status";
        public const string ALLOW_FLIGHT = "allow-flight";
        public const string INITIAL_DISABLED_PACKS = "initial-disabled-packs";
        public const string BROADCAST_RCON_TO_OPS = "broadcast-rcon-to-ops";
        public const string VIEW_DISTANCE = "view-distance";
        public const string SERVER_IP = "server-ip";
        public const string RESOURCE_PACK_PROMPT = "resource-pack-prompt";
        public const string ALLOW_NETHER = "allow-nether";
        public const string SERVER_PORT = "server-port";
        public const string ENABLE_RCON = "enable-rcon";
        public const string SYNC_CHUNK_WRITES = "sync-chunk-writes";
        public const string OP_PERMISSION_LEVEL = "op-permission-level";
        public const string PREVENT_PROXY_CONNECTIONS = "prevent-proxy-connections";
        public const string HIDE_ONLINE_PLAYERS = "hide-online-players";
        public const string RESOURCE_PACK = "resource-pack";
        public const string ENTITY_BROADCAST_RANGE_PERCENTAGE = "entity-broadcast-range-percentage";
        public const string SIMULATION_DISTANCE = "simulation-distance";
        public const string RCON_PASSWORD = "rcon.password";
        public const string PLAYER_IDLE_TIMEOUT = "player-idle-timeout";
        public const string FORCE_GAMEMODE = "force-gamemode";
        public const string RATE_LIMIT = "rate-limit";
        public const string HARDCORE = "hardcore";
        public const string WHITE_LIST = "white-list";
        public const string BROADCAST_CONSOLE_TO_OPS = "broadcast-console-to-ops";
        public const string SPAWN_NPCS = "spawn-npcs";
        public const string SPAWN_ANIMALS = "spawn-animals";
        public const string LOG_IPS = "log-ips";
        public const string FUNCTION_PERMISSION_LEVEL = "function-permission-level";
        public const string INITIAL_ENABLED_PACKS = "initial-enabled-packs";
        public const string LEVEL_TYPE = "level-type";
        public const string TEXT_FILTERING_CONFIG = "text-filtering-config";
        public const string SPAWN_MONSTERS = "spawn-monsters";
        public const string ENFORCE_WHITELIST = "enforce-whitelist";
        public const string SPAWN_PROTECTION = "spawn-protection";
        public const string RESOURCE_PACK_SHA1 = "resource-pack-sha1";
        public const string MAX_WORLD_SIZE = "max-world-size";

        static ServerProperties()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".SystemResource.server.properties") ?? throw new InvalidOperationException();
            string text = stream.ToUtf8Text();
            Dictionary<string, string> dictionary = Parse(text);
            DefaultProperties = new(dictionary, false);
        }

        public ServerProperties(IDictionary<string, string> dictionary, bool applyDefault = true)
        {
            ArgumentNullException.ThrowIfNull(dictionary, nameof(dictionary));

            if (applyDefault)
            {
                EnableJmxMonitoring = bool.Parse(GetValueOrDefault(dictionary, ENABLE_JMX_MONITORING, DefaultProperties.EnableJmxMonitoring.ToString()));
                RconPort = int.Parse(GetValueOrDefault(dictionary, RCON_PORT, DefaultProperties.RconPort.ToString()));
                LevelSeed = GetValueOrDefault(dictionary, LEVEL_SEED, DefaultProperties.LevelSeed.ToString());
                Gamemode = GetValueOrDefault(dictionary, GAMEMODE, DefaultProperties.Gamemode.ToString());
                EnableCommandBlock = bool.Parse(GetValueOrDefault(dictionary, ENABLE_COMMAND_BLOCK, DefaultProperties.EnableCommandBlock.ToString()));
                EnableQuery = bool.Parse(GetValueOrDefault(dictionary, ENABLE_QUERY, DefaultProperties.EnableQuery.ToString()));
                GeneratorSettings = GetValueOrDefault(dictionary, GENERATOR_SETTINGS, DefaultProperties.GeneratorSettings.ToString());
                EnforceSecureProfile = bool.Parse(GetValueOrDefault(dictionary, ENFORCE_SECURE_PROFILE, DefaultProperties.EnforceSecureProfile.ToString()));
                LevelName = GetValueOrDefault(dictionary, LEVEL_NAME, DefaultProperties.LevelName.ToString());
                Motd = GetValueOrDefault(dictionary, MOTD, DefaultProperties.Motd.ToString());
                QueryPort = int.Parse(GetValueOrDefault(dictionary, QUERY_PORT, DefaultProperties.QueryPort.ToString()));
                PvP = bool.Parse(GetValueOrDefault(dictionary, PVP, DefaultProperties.PvP.ToString()));
                GenerateStructures = bool.Parse(GetValueOrDefault(dictionary, GENERATE_STRUCTURES, DefaultProperties.GenerateStructures.ToString()));
                MaxChainedNeighborUpdates = int.Parse(GetValueOrDefault(dictionary, MAX_CHAINED_NEIGHBOR_UPDATES, DefaultProperties.MaxChainedNeighborUpdates.ToString()));
                Difficulty = GetValueOrDefault(dictionary, DIFFICULTY, DefaultProperties.Difficulty.ToString());
                NetworkCompressionThreshold = int.Parse(GetValueOrDefault(dictionary, NETWORK_COMPRESSION_THRESHOLD, DefaultProperties.NetworkCompressionThreshold.ToString()));
                MaxTickTime = int.Parse(GetValueOrDefault(dictionary, MAX_TICK_TIME, DefaultProperties.MaxTickTime.ToString()));
                RequireResourcePack = bool.Parse(GetValueOrDefault(dictionary, REQUIRE_RESOURCE_PACK, DefaultProperties.RequireResourcePack.ToString()));
                UseNativeTransport = bool.Parse(GetValueOrDefault(dictionary, USE_NATIVE_TRANSPORT, DefaultProperties.UseNativeTransport.ToString()));
                MaxPlayers = int.Parse(GetValueOrDefault(dictionary, MAX_PLAYERS, DefaultProperties.MaxPlayers.ToString()));
                OnlineMode = bool.Parse(GetValueOrDefault(dictionary, ONLINE_MODE, DefaultProperties.OnlineMode.ToString()));
                EnableStatus = bool.Parse(GetValueOrDefault(dictionary, ENABLE_STATUS, DefaultProperties.EnableStatus.ToString()));
                AllowFlight = bool.Parse(GetValueOrDefault(dictionary, ALLOW_FLIGHT, DefaultProperties.AllowFlight.ToString()));
                InitialDisabledPacks = GetValueOrDefault(dictionary, INITIAL_DISABLED_PACKS, DefaultProperties.InitialDisabledPacks.ToString());
                BroadcastRconToOps = bool.Parse(GetValueOrDefault(dictionary, BROADCAST_RCON_TO_OPS, DefaultProperties.BroadcastRconToOps.ToString()));
                ViewDistance = int.Parse(GetValueOrDefault(dictionary, VIEW_DISTANCE, DefaultProperties.ViewDistance.ToString()));
                ServerIP = GetValueOrDefault(dictionary, SERVER_IP, DefaultProperties.ServerIP.ToString());
                ResourcePackPrompt = GetValueOrDefault(dictionary, RESOURCE_PACK_PROMPT, DefaultProperties.ResourcePackPrompt.ToString());
                AllowNether = bool.Parse(GetValueOrDefault(dictionary, ALLOW_NETHER, DefaultProperties.AllowNether.ToString()));
                ServerPort = int.Parse(GetValueOrDefault(dictionary, SERVER_PORT, DefaultProperties.ServerPort.ToString()));
                EnableRcon = bool.Parse(GetValueOrDefault(dictionary, ENABLE_RCON, DefaultProperties.EnableRcon.ToString()));
                SyncChunkWrites = bool.Parse(GetValueOrDefault(dictionary, SYNC_CHUNK_WRITES, DefaultProperties.SyncChunkWrites.ToString()));
                OpPermissionLevel = int.Parse(GetValueOrDefault(dictionary, OP_PERMISSION_LEVEL, DefaultProperties.OpPermissionLevel.ToString()));
                PreventProxyConnections = bool.Parse(GetValueOrDefault(dictionary, PREVENT_PROXY_CONNECTIONS, DefaultProperties.PreventProxyConnections.ToString()));
                HideOnlinePlayers = bool.Parse(GetValueOrDefault(dictionary, HIDE_ONLINE_PLAYERS, DefaultProperties.HideOnlinePlayers.ToString()));
                ResourcePack = GetValueOrDefault(dictionary, RESOURCE_PACK, DefaultProperties.ResourcePack.ToString());
                EntityBroadcastRangePercentage = int.Parse(GetValueOrDefault(dictionary, ENTITY_BROADCAST_RANGE_PERCENTAGE, DefaultProperties.EntityBroadcastRangePercentage.ToString()));
                SimulationDistance = int.Parse(GetValueOrDefault(dictionary, SIMULATION_DISTANCE, DefaultProperties.SimulationDistance.ToString()));
                RconPassword = GetValueOrDefault(dictionary, RCON_PASSWORD, DefaultProperties.RconPassword.ToString());
                PlayerIdleTimeout = int.Parse(GetValueOrDefault(dictionary, PLAYER_IDLE_TIMEOUT, DefaultProperties.PlayerIdleTimeout.ToString()));
                ForceGamemode = bool.Parse(GetValueOrDefault(dictionary, FORCE_GAMEMODE, DefaultProperties.ForceGamemode.ToString()));
                RateLimit = int.Parse(GetValueOrDefault(dictionary, RATE_LIMIT, DefaultProperties.RateLimit.ToString()));
                Hardcore = bool.Parse(GetValueOrDefault(dictionary, HARDCORE, DefaultProperties.Hardcore.ToString()));
                WhiteList = bool.Parse(GetValueOrDefault(dictionary, WHITE_LIST, DefaultProperties.WhiteList.ToString()));
                BroadcastConsoleToOps = bool.Parse(GetValueOrDefault(dictionary, BROADCAST_CONSOLE_TO_OPS, DefaultProperties.BroadcastConsoleToOps.ToString()));
                SpawnNpcs = bool.Parse(GetValueOrDefault(dictionary, SPAWN_NPCS, DefaultProperties.SpawnNpcs.ToString()));
                SpawnAnimals = bool.Parse(GetValueOrDefault(dictionary, SPAWN_ANIMALS, DefaultProperties.SpawnAnimals.ToString()));
                LogIps = bool.Parse(GetValueOrDefault(dictionary, LOG_IPS, DefaultProperties.LogIps.ToString()));
                FunctionPermissionLevel = int.Parse(GetValueOrDefault(dictionary, FUNCTION_PERMISSION_LEVEL, DefaultProperties.FunctionPermissionLevel.ToString()));
                InitialEnabledPacks = GetValueOrDefault(dictionary, INITIAL_ENABLED_PACKS, DefaultProperties.InitialEnabledPacks.ToString());
                LevelType = GetValueOrDefault(dictionary, LEVEL_TYPE, DefaultProperties.LevelType.ToString());
                TextFilteringConfig = GetValueOrDefault(dictionary, TEXT_FILTERING_CONFIG, DefaultProperties.TextFilteringConfig.ToString());
                SpawnMonsters = bool.Parse(GetValueOrDefault(dictionary, SPAWN_MONSTERS, DefaultProperties.SpawnMonsters.ToString()));
                EnforceWhitelist = bool.Parse(GetValueOrDefault(dictionary, ENFORCE_WHITELIST, DefaultProperties.EnforceWhitelist.ToString()));
                SpawnProtection = int.Parse(GetValueOrDefault(dictionary, SPAWN_PROTECTION, DefaultProperties.SpawnProtection.ToString()));
                ResourcePackSha1 = GetValueOrDefault(dictionary, RESOURCE_PACK_SHA1, DefaultProperties.ResourcePackSha1.ToString());
                MaxWorldSize = int.Parse(GetValueOrDefault(dictionary, MAX_WORLD_SIZE, DefaultProperties.MaxWorldSize.ToString()));
            }
            else
            {
                EnableJmxMonitoring = bool.Parse(dictionary[ENABLE_JMX_MONITORING]);
                RconPort = int.Parse(dictionary[RCON_PORT]);
                LevelSeed = dictionary[LEVEL_SEED];
                Gamemode = dictionary[GAMEMODE];
                EnableCommandBlock = bool.Parse(dictionary[ENABLE_COMMAND_BLOCK]);
                EnableQuery = bool.Parse(dictionary[ENABLE_QUERY]);
                GeneratorSettings = dictionary[GENERATOR_SETTINGS];
                EnforceSecureProfile = bool.Parse(dictionary[ENFORCE_SECURE_PROFILE]);
                LevelName = dictionary[LEVEL_NAME];
                Motd = dictionary[MOTD];
                QueryPort = int.Parse(dictionary[QUERY_PORT]);
                PvP = bool.Parse(dictionary[PVP]);
                GenerateStructures = bool.Parse(dictionary[GENERATE_STRUCTURES]);
                MaxChainedNeighborUpdates = int.Parse(dictionary[MAX_CHAINED_NEIGHBOR_UPDATES]);
                Difficulty = dictionary[DIFFICULTY];
                NetworkCompressionThreshold = int.Parse(dictionary[NETWORK_COMPRESSION_THRESHOLD]);
                MaxTickTime = int.Parse(dictionary[MAX_TICK_TIME]);
                RequireResourcePack = bool.Parse(dictionary[REQUIRE_RESOURCE_PACK]);
                UseNativeTransport = bool.Parse(dictionary[USE_NATIVE_TRANSPORT]);
                MaxPlayers = int.Parse(dictionary[MAX_PLAYERS]);
                OnlineMode = bool.Parse(dictionary[ONLINE_MODE]);
                EnableStatus = bool.Parse(dictionary[ENABLE_STATUS]);
                AllowFlight = bool.Parse(dictionary[ALLOW_FLIGHT]);
                InitialDisabledPacks = dictionary[INITIAL_DISABLED_PACKS];
                BroadcastRconToOps = bool.Parse(dictionary[BROADCAST_RCON_TO_OPS]);
                ViewDistance = int.Parse(dictionary[VIEW_DISTANCE]);
                ServerIP = dictionary[SERVER_IP];
                ResourcePackPrompt = dictionary[RESOURCE_PACK_PROMPT];
                AllowNether = bool.Parse(dictionary[ALLOW_NETHER]);
                ServerPort = int.Parse(dictionary[SERVER_PORT]);
                EnableRcon = bool.Parse(dictionary[ENABLE_RCON]);
                SyncChunkWrites = bool.Parse(dictionary[SYNC_CHUNK_WRITES]);
                OpPermissionLevel = int.Parse(dictionary[OP_PERMISSION_LEVEL]);
                PreventProxyConnections = bool.Parse(dictionary[PREVENT_PROXY_CONNECTIONS]);
                HideOnlinePlayers = bool.Parse(dictionary[HIDE_ONLINE_PLAYERS]);
                ResourcePack = dictionary[RESOURCE_PACK];
                EntityBroadcastRangePercentage = int.Parse(dictionary[ENTITY_BROADCAST_RANGE_PERCENTAGE]);
                SimulationDistance = int.Parse(dictionary[SIMULATION_DISTANCE]);
                RconPassword = dictionary[RCON_PASSWORD];
                PlayerIdleTimeout = int.Parse(dictionary[PLAYER_IDLE_TIMEOUT]);
                ForceGamemode = bool.Parse(dictionary[FORCE_GAMEMODE]);
                RateLimit = int.Parse(dictionary[RATE_LIMIT]);
                Hardcore = bool.Parse(dictionary[HARDCORE]);
                WhiteList = bool.Parse(dictionary[WHITE_LIST]);
                BroadcastConsoleToOps = bool.Parse(dictionary[BROADCAST_CONSOLE_TO_OPS]);
                SpawnNpcs = bool.Parse(dictionary[SPAWN_NPCS]);
                SpawnAnimals = bool.Parse(dictionary[SPAWN_ANIMALS]);
                LogIps = bool.Parse(dictionary[LOG_IPS]);
                FunctionPermissionLevel = int.Parse(dictionary[FUNCTION_PERMISSION_LEVEL]);
                InitialEnabledPacks = dictionary[INITIAL_ENABLED_PACKS];
                LevelType = dictionary[LEVEL_TYPE];
                TextFilteringConfig = dictionary[TEXT_FILTERING_CONFIG];
                SpawnMonsters = bool.Parse(dictionary[SPAWN_MONSTERS]);
                EnforceWhitelist = bool.Parse(dictionary[ENFORCE_WHITELIST]);
                SpawnProtection = int.Parse(dictionary[SPAWN_PROTECTION]);
                ResourcePackSha1 = dictionary[RESOURCE_PACK_SHA1];
                MaxWorldSize = int.Parse(dictionary[MAX_WORLD_SIZE]);
            }

            static string GetValueOrDefault(IDictionary<string, string> dictionary, string key, string defValue)
            {
                if (dictionary.TryGetValue(key, out var value))
                    return value;
                else
                    return defValue;
            }
        }

        public static readonly ServerProperties DefaultProperties;

        #region Properties

        [Properties(ENABLE_JMX_MONITORING)]
        public bool EnableJmxMonitoring { get; }

        [Properties(RCON_PORT)]
        public int RconPort { get; }

        [Properties(LEVEL_SEED)]
        public string LevelSeed { get; }

        [Properties(GAMEMODE)]
        public string Gamemode { get; }

        [Properties(ENABLE_COMMAND_BLOCK)]
        public bool EnableCommandBlock { get; }

        [Properties(ENABLE_QUERY)]
        public bool EnableQuery { get; }

        [Properties(GENERATOR_SETTINGS)]
        public string GeneratorSettings { get; }

        [Properties(ENFORCE_SECURE_PROFILE)]
        public bool EnforceSecureProfile { get; }

        [Properties(LEVEL_NAME)]
        public string LevelName { get; }

        [Properties(MOTD)]
        public string Motd { get; }

        [Properties(QUERY_PORT)]
        public int QueryPort { get; }

        [Properties(PVP)]
        public bool PvP { get; }

        [Properties(GENERATE_STRUCTURES)]
        public bool GenerateStructures { get; }

        [Properties(MAX_CHAINED_NEIGHBOR_UPDATES)]
        public int MaxChainedNeighborUpdates { get; }

        [Properties(DIFFICULTY)]
        public string Difficulty { get; }

        [Properties(NETWORK_COMPRESSION_THRESHOLD)]
        public int NetworkCompressionThreshold { get; }

        [Properties(MAX_TICK_TIME)]
        public int MaxTickTime { get; }

        [Properties(REQUIRE_RESOURCE_PACK)]
        public bool RequireResourcePack { get; }

        [Properties(USE_NATIVE_TRANSPORT)]
        public bool UseNativeTransport { get; }

        [Properties(MAX_PLAYERS)]
        public int MaxPlayers { get; }

        [Properties(ONLINE_MODE)]
        public bool OnlineMode { get; }

        [Properties(ENABLE_STATUS)]
        public bool EnableStatus { get; }

        [Properties(ALLOW_FLIGHT)]
        public bool AllowFlight { get; }

        [Properties(INITIAL_DISABLED_PACKS)]
        public string InitialDisabledPacks { get; }

        [Properties(BROADCAST_RCON_TO_OPS)]
        public bool BroadcastRconToOps { get; }

        [Properties(VIEW_DISTANCE)]
        public int ViewDistance { get; }

        [Properties(SERVER_IP)]
        public string ServerIP { get; }

        [Properties(RESOURCE_PACK_PROMPT)]
        public string ResourcePackPrompt { get; }

        [Properties(ALLOW_NETHER)]
        public bool AllowNether { get; }

        [Properties(SERVER_PORT)]
        public int ServerPort { get; }

        [Properties(ENABLE_RCON)]
        public bool EnableRcon { get; }

        [Properties(SYNC_CHUNK_WRITES)]
        public bool SyncChunkWrites { get; }

        [Properties(OP_PERMISSION_LEVEL)]
        public int OpPermissionLevel { get; }

        [Properties(PREVENT_PROXY_CONNECTIONS)]
        public bool PreventProxyConnections { get; }

        [Properties(HIDE_ONLINE_PLAYERS)]
        public bool HideOnlinePlayers { get; }

        [Properties(RESOURCE_PACK)]
        public string ResourcePack { get; }

        [Properties(ENTITY_BROADCAST_RANGE_PERCENTAGE)]
        public int EntityBroadcastRangePercentage { get; }

        [Properties(SIMULATION_DISTANCE)]
        public int SimulationDistance { get; }

        [Properties(RCON_PASSWORD)]
        public string RconPassword { get; }

        [Properties(PLAYER_IDLE_TIMEOUT)]
        public int PlayerIdleTimeout { get; }

        [Properties(FORCE_GAMEMODE)]
        public bool ForceGamemode { get; }

        [Properties(RATE_LIMIT)]
        public int RateLimit { get; }

        [Properties(HARDCORE)]
        public bool Hardcore { get; }

        [Properties(WHITE_LIST)]
        public bool WhiteList { get; }

        [Properties(BROADCAST_CONSOLE_TO_OPS)]
        public bool BroadcastConsoleToOps { get; }

        [Properties(SPAWN_NPCS)]
        public bool SpawnNpcs { get; }

        [Properties(SPAWN_ANIMALS)]
        public bool SpawnAnimals { get; }

        [Properties(LOG_IPS)]
        public bool LogIps { get; }

        [Properties(FUNCTION_PERMISSION_LEVEL)]
        public int FunctionPermissionLevel { get; }

        [Properties(INITIAL_ENABLED_PACKS)]
        public string InitialEnabledPacks { get; }

        [Properties(LEVEL_TYPE)]
        public string LevelType { get; }

        [Properties(TEXT_FILTERING_CONFIG)]
        public string TextFilteringConfig { get; }

        [Properties(SPAWN_MONSTERS)]
        public bool SpawnMonsters { get; }

        [Properties(ENFORCE_WHITELIST)]
        public bool EnforceWhitelist { get; }

        [Properties(SPAWN_PROTECTION)]
        public int SpawnProtection { get; }

        [Properties(RESOURCE_PACK_SHA1)]
        public string ResourcePackSha1 { get; }

        [Properties(MAX_WORLD_SIZE)]
        public int MaxWorldSize { get; }

        #endregion

        public override string ToString()
        {
            string format = "{0}={1}{2}";
            StringBuilder sb = new();
            sb.AppendFormat(format, ENABLE_JMX_MONITORING, EnableJmxMonitoring, Environment.NewLine);
            sb.AppendFormat(format, RCON_PORT, RconPort, Environment.NewLine);
            sb.AppendFormat(format, LEVEL_SEED, LevelSeed, Environment.NewLine);
            sb.AppendFormat(format, GAMEMODE, Gamemode, Environment.NewLine);
            sb.AppendFormat(format, ENABLE_COMMAND_BLOCK, EnableCommandBlock, Environment.NewLine);
            sb.AppendFormat(format, ENABLE_QUERY, EnableQuery, Environment.NewLine);
            sb.AppendFormat(format, GENERATOR_SETTINGS, GeneratorSettings, Environment.NewLine);
            sb.AppendFormat(format, ENFORCE_SECURE_PROFILE, EnforceSecureProfile, Environment.NewLine);
            sb.AppendFormat(format, LEVEL_NAME, LevelName, Environment.NewLine);
            sb.AppendFormat(format, MOTD, Motd, Environment.NewLine);
            sb.AppendFormat(format, QUERY_PORT, QueryPort, Environment.NewLine);
            sb.AppendFormat(format, PVP, PvP, Environment.NewLine);
            sb.AppendFormat(format, GENERATE_STRUCTURES, GenerateStructures, Environment.NewLine);
            sb.AppendFormat(format, MAX_CHAINED_NEIGHBOR_UPDATES, MaxChainedNeighborUpdates, Environment.NewLine);
            sb.AppendFormat(format, DIFFICULTY, Difficulty, Environment.NewLine);
            sb.AppendFormat(format, NETWORK_COMPRESSION_THRESHOLD, NetworkCompressionThreshold, Environment.NewLine);
            sb.AppendFormat(format, MAX_TICK_TIME, MaxTickTime, Environment.NewLine);
            sb.AppendFormat(format, REQUIRE_RESOURCE_PACK, RequireResourcePack, Environment.NewLine);
            sb.AppendFormat(format, USE_NATIVE_TRANSPORT, UseNativeTransport, Environment.NewLine);
            sb.AppendFormat(format, MAX_PLAYERS, MaxPlayers, Environment.NewLine);
            sb.AppendFormat(format, ONLINE_MODE, OnlineMode, Environment.NewLine);
            sb.AppendFormat(format, ENABLE_STATUS, EnableStatus, Environment.NewLine);
            sb.AppendFormat(format, ALLOW_FLIGHT, AllowFlight, Environment.NewLine);
            sb.AppendFormat(format, INITIAL_DISABLED_PACKS, InitialDisabledPacks, Environment.NewLine);
            sb.AppendFormat(format, BROADCAST_RCON_TO_OPS, BroadcastRconToOps, Environment.NewLine);
            sb.AppendFormat(format, VIEW_DISTANCE, ViewDistance, Environment.NewLine);
            sb.AppendFormat(format, SERVER_IP, ServerIP, Environment.NewLine);
            sb.AppendFormat(format, RESOURCE_PACK_PROMPT, ResourcePackPrompt, Environment.NewLine);
            sb.AppendFormat(format, ALLOW_NETHER, AllowNether, Environment.NewLine);
            sb.AppendFormat(format, SERVER_PORT, ServerPort, Environment.NewLine);
            sb.AppendFormat(format, ENABLE_RCON, EnableRcon, Environment.NewLine);
            sb.AppendFormat(format, SYNC_CHUNK_WRITES, SyncChunkWrites, Environment.NewLine);
            sb.AppendFormat(format, OP_PERMISSION_LEVEL, OpPermissionLevel, Environment.NewLine);
            sb.AppendFormat(format, PREVENT_PROXY_CONNECTIONS, PreventProxyConnections, Environment.NewLine);
            sb.AppendFormat(format, HIDE_ONLINE_PLAYERS, HideOnlinePlayers, Environment.NewLine);
            sb.AppendFormat(format, RESOURCE_PACK, ResourcePack, Environment.NewLine);
            sb.AppendFormat(format, ENTITY_BROADCAST_RANGE_PERCENTAGE, EntityBroadcastRangePercentage, Environment.NewLine);
            sb.AppendFormat(format, SIMULATION_DISTANCE, SimulationDistance, Environment.NewLine);
            sb.AppendFormat(format, RCON_PASSWORD, RconPassword, Environment.NewLine);
            sb.AppendFormat(format, PLAYER_IDLE_TIMEOUT, PlayerIdleTimeout, Environment.NewLine);
            sb.AppendFormat(format, FORCE_GAMEMODE, ForceGamemode, Environment.NewLine);
            sb.AppendFormat(format, RATE_LIMIT, RateLimit, Environment.NewLine);
            sb.AppendFormat(format, HARDCORE, Hardcore, Environment.NewLine);
            sb.AppendFormat(format, WHITE_LIST, WhiteList, Environment.NewLine);
            sb.AppendFormat(format, BROADCAST_CONSOLE_TO_OPS, BroadcastConsoleToOps, Environment.NewLine);
            sb.AppendFormat(format, SPAWN_NPCS, SpawnNpcs, Environment.NewLine);
            sb.AppendFormat(format, SPAWN_ANIMALS, SpawnAnimals, Environment.NewLine);
            sb.AppendFormat(format, LOG_IPS, LogIps, Environment.NewLine);
            sb.AppendFormat(format, FUNCTION_PERMISSION_LEVEL, FunctionPermissionLevel, Environment.NewLine);
            sb.AppendFormat(format, INITIAL_ENABLED_PACKS, InitialEnabledPacks, Environment.NewLine);
            sb.AppendFormat(format, LEVEL_TYPE, LevelType, Environment.NewLine);
            sb.AppendFormat(format, TEXT_FILTERING_CONFIG, TextFilteringConfig, Environment.NewLine);
            sb.AppendFormat(format, SPAWN_MONSTERS, SpawnMonsters, Environment.NewLine);
            sb.AppendFormat(format, ENFORCE_WHITELIST, EnforceWhitelist, Environment.NewLine);
            sb.AppendFormat(format, SPAWN_PROTECTION, SpawnProtection, Environment.NewLine);
            sb.AppendFormat(format, RESOURCE_PACK_SHA1, ResourcePackSha1, Environment.NewLine);
            sb.AppendFormat(format, MAX_WORLD_SIZE, MaxWorldSize, Environment.NewLine);
            sb.Length -= Environment.NewLine.Length;

            string result = sb.ToString();
            result = result.Replace("=True", "=true");
            result = result.Replace("=False", "=false");

            return result;
        }

        public static Dictionary<string, string> Parse(string text)
        {
            Dictionary<string, string> dictionary = new();
            if (string.IsNullOrEmpty(text))
                return dictionary;

            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (line.StartsWith('#'))
                    continue;

                char separator = '=';
                int index = line.IndexOf(separator);
                if (index == -1)
                    continue;

                string key = line[..index];
                string value = line[(index + 1)..];
                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}
