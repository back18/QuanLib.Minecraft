using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace QuanLib.Minecraft
{
    public class ServerProperties
    {
        static ServerProperties()
        {
            _propertyInfos = [];
            foreach (PropertyInfo propertyInfo in typeof(ServerProperties).GetProperties())
            {
                if (Attribute.GetCustomAttribute(propertyInfo, typeof(PropertyAttribute)) is PropertyAttribute attribute)
                    _propertyInfos.Add(attribute.Name, propertyInfo);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".SystemResource.server.properties") ?? throw new InvalidOperationException();
            using StreamReader streamReader = new(stream, Encoding.UTF8);
            string serverProperties = streamReader.ReadToEnd();
            Dictionary<string, string> properties = Parse(serverProperties);
            DefaultProperties = Load(properties, false);
        }

        private ServerProperties() { }

        private static readonly Dictionary<string, PropertyInfo> _propertyInfos;
        public static readonly ServerProperties DefaultProperties;

        public const string ACCEPTS_TRANSFERS = "accepts-transfers";
        public const string ALLOW_FLIGHT = "allow-flight";
        public const string BROADCAST_CONSOLE_TO_OPS = "broadcast-console-to-ops";
        public const string BROADCAST_RCON_TO_OPS = "broadcast-rcon-to-ops";
        public const string BUG_REPORT_LINK = "bug-report-link";
        public const string DIFFICULTY = "difficulty";
        public const string ENABLE_CODE_OF_CONDUCT = "enable-code-of-conduct";
        public const string ENABLE_JMX_MONITORING = "enable-jmx-monitoring";
        public const string ENABLE_QUERY = "enable-query";
        public const string ENABLE_RCON = "enable-rcon";
        public const string ENABLE_STATUS = "enable-status";
        public const string ENFORCE_SECURE_PROFILE = "enforce-secure-profile";
        public const string ENFORCE_WHITELIST = "enforce-whitelist";
        public const string ENTITY_BROADCAST_RANGE_PERCENTAGE = "entity-broadcast-range-percentage";
        public const string FORCE_GAMEMODE = "force-gamemode";
        public const string FUNCTION_PERMISSION_LEVEL = "function-permission-level";
        public const string GAMEMODE = "gamemode";
        public const string GENERATE_STRUCTURES = "generate-structures";
        public const string GENERATOR_SETTINGS = "generator-settings";
        public const string HARDCORE = "hardcore";
        public const string HIDE_ONLINE_PLAYERS = "hide-online-players";
        public const string INITIAL_DISABLED_PACKS = "initial-disabled-packs";
        public const string INITIAL_ENABLED_PACKS = "initial-enabled-packs";
        public const string LEVEL_NAME = "level-name";
        public const string LEVEL_SEED = "level-seed";
        public const string LEVEL_TYPE = "level-type";
        public const string LOG_IPS = "log-ips";
        public const string MANAGEMENT_SERVER_ALLOWED_ORIGINS = "management-server-allowed-origins";
        public const string MANAGEMENT_SERVER_ENABLED = "management-server-enabled";
        public const string MANAGEMENT_SERVER_HOST = "management-server-host";
        public const string MANAGEMENT_SERVER_PORT = "management-server-port";
        public const string MANAGEMENT_SERVER_SECRET = "management-server-secret";
        public const string MANAGEMENT_SERVER_TLS_ENABLED = "management-server-tls-enabled";
        public const string MANAGEMENT_SERVER_TLS_KEYSTORE = "management-server-tls-keystore";
        public const string MANAGEMENT_SERVER_TLS_KEYSTORE_PASSWORD = "management-server-tls-keystore-password";
        public const string MAX_CHAINED_NEIGHBOR_UPDATES = "max-chained-neighbor-updates";
        public const string MAX_PLAYERS = "max-players";
        public const string MAX_TICK_TIME = "max-tick-time";
        public const string MAX_WORLD_SIZE = "max-world-size";
        public const string MOTD = "motd";
        public const string NETWORK_COMPRESSION_THRESHOLD = "network-compression-threshold";
        public const string ONLINE_MODE = "online-mode";
        public const string OP_PERMISSION_LEVEL = "op-permission-level";
        public const string PAUSE_WHEN_EMPTY_SECONDS = "pause-when-empty-seconds";
        public const string PLAYER_IDLE_TIMEOUT = "player-idle-timeout";
        public const string PREVENT_PROXY_CONNECTIONS = "prevent-proxy-connections";
        public const string QUERY_PORT = "query.port";
        public const string RATE_LIMIT = "rate-limit";
        public const string RCON_PASSWORD = "rcon.password";
        public const string RCON_PORT = "rcon.port";
        public const string REGION_FILE_COMPRESSION = "region-file-compression";
        public const string REQUIRE_RESOURCE_PACK = "require-resource-pack";
        public const string RESOURCE_PACK = "resource-pack";
        public const string RESOURCE_PACK_ID = "resource-pack-id";
        public const string RESOURCE_PACK_PROMPT = "resource-pack-prompt";
        public const string RESOURCE_PACK_SHA1 = "resource-pack-sha1";
        public const string SERVER_IP = "server-ip";
        public const string SERVER_PORT = "server-port";
        public const string SIMULATION_DISTANCE = "simulation-distance";
        public const string SPAWN_PROTECTION = "spawn-protection";
        public const string STATUS_HEARTBEAT_INTERVAL = "status-heartbeat-interval";
        public const string SYNC_CHUNK_WRITES = "sync-chunk-writes";
        public const string TEXT_FILTERING_CONFIG = "text-filtering-config";
        public const string TEXT_FILTERING_VERSION = "text-filtering-version";
        public const string USE_NATIVE_TRANSPORT = "use-native-transport";
        public const string VIEW_DISTANCE = "view-distance";
        public const string WHITE_LIST = "white-list";

        //以下的属性在 Minecraft 1.21 版本之后被移除，但仍然保留在 ServerProperties 类中以兼容旧版本的服务器属性文件
        public const string ENABLE_COMMAND_BLOCK = "enable-command-block";
        public const string ALLOW_NETHER = "allow-nether";
        public const string SPAWN_ANIMALS = "spawn-animals";
        public const string SPAWN_MONSTERS = "spawn-monsters";
        public const string SPAWN_NPCS = "spawn-npcs";
        public const string PVP = "pvp";

        [Property(ACCEPTS_TRANSFERS)] public bool AcceptsTransfers { get; private set; }
        [Property(ALLOW_FLIGHT)] public bool AllowFlight { get; private set; }
        [Property(BROADCAST_CONSOLE_TO_OPS)] public bool BroadcastConsoleToOps { get; private set; }
        [Property(BROADCAST_RCON_TO_OPS)] public bool BroadcastRconToOps { get; private set; }
        [Property(BUG_REPORT_LINK)] public string BugReportLink { get; private set; } = string.Empty;
        [Property(DIFFICULTY)] public string Difficulty { get; private set; } = string.Empty;
        [Property(ENABLE_CODE_OF_CONDUCT)] public bool EnableCodeOfConduct { get; private set; }
        [Property(ENABLE_JMX_MONITORING)] public bool EnableJmxMonitoring { get; private set; }
        [Property(ENABLE_QUERY)] public bool EnableQuery { get; private set; }
        [Property(ENABLE_RCON)] public bool EnableRcon { get; private set; }
        [Property(ENABLE_STATUS)] public bool EnableStatus { get; private set; }
        [Property(ENFORCE_SECURE_PROFILE)] public bool EnforceSecureProfile { get; private set; }
        [Property(ENFORCE_WHITELIST)] public bool EnforceWhitelist { get; private set; }
        [Property(ENTITY_BROADCAST_RANGE_PERCENTAGE)] public int EntityBroadcastRangePercentage { get; private set; }
        [Property(FORCE_GAMEMODE)] public bool ForceGamemode { get; private set; }
        [Property(FUNCTION_PERMISSION_LEVEL)] public int FunctionPermissionLevel { get; private set; }
        [Property(GAMEMODE)] public string Gamemode { get; private set; } = string.Empty;
        [Property(GENERATE_STRUCTURES)] public bool GenerateStructures { get; private set; }
        [Property(GENERATOR_SETTINGS)] public string GeneratorSettings { get; private set; } = string.Empty;
        [Property(HARDCORE)] public bool Hardcore { get; private set; }
        [Property(HIDE_ONLINE_PLAYERS)] public bool HideOnlinePlayers { get; private set; }
        [Property(INITIAL_DISABLED_PACKS)] public string InitialDisabledPacks { get; private set; } = string.Empty;
        [Property(INITIAL_ENABLED_PACKS)] public string InitialEnabledPacks { get; private set; } = string.Empty;
        [Property(LEVEL_NAME)] public string LevelName { get; private set; } = string.Empty;
        [Property(LEVEL_SEED)] public string LevelSeed { get; private set; } = string.Empty;
        [Property(LEVEL_TYPE)] public string LevelType { get; private set; } = string.Empty;
        [Property(LOG_IPS)] public bool LogIps { get; private set; }
        [Property(MANAGEMENT_SERVER_ALLOWED_ORIGINS)] public string ManagementServerAllowedOrigins { get; private set; } = string.Empty;
        [Property(MANAGEMENT_SERVER_ENABLED)] public bool ManagementServerEnabled { get; private set; }
        [Property(MANAGEMENT_SERVER_HOST)] public string ManagementServerHost { get; private set; } = string.Empty;
        [Property(MANAGEMENT_SERVER_PORT)] public int ManagementServerPort { get; private set; }
        [Property(MANAGEMENT_SERVER_SECRET)] public string ManagementServerSecret { get; private set; } = string.Empty;
        [Property(MANAGEMENT_SERVER_TLS_ENABLED)] public bool ManagementServerTlsEnabled { get; private set; }
        [Property(MANAGEMENT_SERVER_TLS_KEYSTORE)] public string ManagementServerTlsKeystore { get; private set; } = string.Empty;
        [Property(MANAGEMENT_SERVER_TLS_KEYSTORE_PASSWORD)] public string ManagementServerTlsKeystorePassword { get; private set; } = string.Empty;
        [Property(MAX_CHAINED_NEIGHBOR_UPDATES)] public int MaxChainedNeighborUpdates { get; private set; }
        [Property(MAX_PLAYERS)] public int MaxPlayers { get; private set; }
        [Property(MAX_TICK_TIME)] public int MaxTickTime { get; private set; }
        [Property(MAX_WORLD_SIZE)] public int MaxWorldSize { get; private set; }
        [Property(MOTD)] public string Motd { get; private set; } = string.Empty;
        [Property(NETWORK_COMPRESSION_THRESHOLD)] public int NetworkCompressionThreshold { get; private set; }
        [Property(ONLINE_MODE)] public bool OnlineMode { get; private set; }
        [Property(OP_PERMISSION_LEVEL)] public int OpPermissionLevel { get; private set; }
        [Property(PAUSE_WHEN_EMPTY_SECONDS)] public int PauseWhenEmptySeconds { get; private set; }
        [Property(PLAYER_IDLE_TIMEOUT)] public int PlayerIdleTimeout { get; private set; }
        [Property(PREVENT_PROXY_CONNECTIONS)] public bool PreventProxyConnections { get; private set; }
        [Property(QUERY_PORT)] public int QueryPort { get; private set; }
        [Property(RATE_LIMIT)] public int RateLimit { get; private set; }
        [Property(RCON_PASSWORD)] public string RconPassword { get; private set; } = string.Empty;
        [Property(RCON_PORT)] public int RconPort { get; private set; }
        [Property(REGION_FILE_COMPRESSION)] public string RegionFileCompression { get; private set; } = string.Empty;
        [Property(REQUIRE_RESOURCE_PACK)] public bool RequireResourcePack { get; private set; }
        [Property(RESOURCE_PACK)] public string ResourcePack { get; private set; } = string.Empty;
        [Property(RESOURCE_PACK_ID)] public string ResourcePackId { get; private set; } = string.Empty;
        [Property(RESOURCE_PACK_PROMPT)] public string ResourcePackPrompt { get; private set; } = string.Empty;
        [Property(RESOURCE_PACK_SHA1)] public string ResourcePackSha1 { get; private set; } = string.Empty;
        [Property(SERVER_IP)] public string ServerIp { get; private set; } = string.Empty;
        [Property(SERVER_PORT)] public int ServerPort { get; private set; }
        [Property(SIMULATION_DISTANCE)] public int SimulationDistance { get; private set; }
        [Property(SPAWN_PROTECTION)] public int SpawnProtection { get; private set; }
        [Property(STATUS_HEARTBEAT_INTERVAL)] public int StatusHeartbeatInterval { get; private set; }
        [Property(SYNC_CHUNK_WRITES)] public bool SyncChunkWrites { get; private set; }
        [Property(TEXT_FILTERING_CONFIG)] public string TextFilteringConfig { get; private set; } = string.Empty;
        [Property(TEXT_FILTERING_VERSION)] public int TextFilteringVersion { get; private set; }
        [Property(USE_NATIVE_TRANSPORT)] public bool UseNativeTransport { get; private set; }
        [Property(VIEW_DISTANCE)] public int ViewDistance { get; private set; }
        [Property(WHITE_LIST)] public bool WhiteList { get; private set; }

        //以下的属性在 Minecraft 1.21 版本之后被移除，但仍然保留在 ServerProperties 类中以兼容旧版本的服务器属性文件
        [Property(ENABLE_COMMAND_BLOCK)] public bool EnableCommandBlock { get; private set; }
        [Property(ALLOW_NETHER)] public bool AllowNether { get; private set; }
        [Property(SPAWN_ANIMALS)] public bool SpawnAnimals { get; private set; }
        [Property(SPAWN_MONSTERS)] public bool SpawnMonsters { get; private set; }
        [Property(SPAWN_NPCS)] public bool SpawnNpcs { get; private set; }
        [Property(PVP)] public bool Pvp { get; private set; }

        public object? GetPropertyValue(string propertyName)
        {
            ArgumentException.ThrowIfNullOrEmpty(propertyName, nameof(propertyName));

            if (!_propertyInfos.TryGetValue(propertyName, out var propertyInfo))
                return null;

            return propertyInfo.GetValue(this);
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            object? value = GetPropertyValue(propertyName)
                ?? throw new InvalidOperationException($"Property '{propertyName}' does not exist.");

            if (value is not T typedValue)
                throw new InvalidCastException($"Property '{propertyName}' is not of type {typeof(T).Name}.");

            return typedValue;
        }

        public static ServerProperties Load(IReadOnlyDictionary<string, string> properties, bool useDefaultValues = true)
        {
            ArgumentNullException.ThrowIfNull(properties, nameof(properties));

            Func<string, bool> boolParser = bool.Parse;
            Func<string, int> intParser = int.Parse;
            ServerProperties serverProperties = new();

            foreach (var item in _propertyInfos)
            {
                string propertyName = item.Key;
                PropertyInfo propertyInfo = item.Value;

                if (properties.TryGetValue(propertyName, out var propertyValue))
                {
                    object? value;
                    if (propertyInfo.PropertyType.Equals(typeof(string)))
                    {
                        value = propertyValue;
                    }
                    else
                    {
                        try
                        {
                            if (propertyInfo.PropertyType.Equals(typeof(int)))
                                value = intParser.Invoke(propertyValue);
                            else if (propertyInfo.PropertyType.Equals(typeof(bool)))
                                value = boolParser.Invoke(propertyValue);
                            else if (useDefaultValues)
                                value = DefaultProperties.GetPropertyValue(propertyName);
                            else
                                continue;
                        }
                        catch (Exception ex) when (ex is ArgumentException or FormatException)
                        {
                            Debug.WriteLine("{0}.{1}: 无法将 \"{2}\" 解析为 \"{3}\" 类型", nameof(ServerProperties), propertyName, propertyValue, ObjectFormatter.Format(propertyInfo.PropertyType));
                            Debug.WriteLine(ex);

                            if (useDefaultValues)
                                value = DefaultProperties.GetPropertyValue(propertyName);
                            else
                                continue;
                        }
                    }

                    propertyInfo.SetValue(serverProperties, value);
                }
                else if (useDefaultValues)
                {
                    propertyInfo.SetValue(serverProperties, DefaultProperties.GetPropertyValue(propertyName));
                }
                else
                {
                    continue;
                }
            }

            return serverProperties;
        }

        public static Dictionary<string, string> Parse(string serverProperties)
        {
            Dictionary<string, string> properties = [];
            if (string.IsNullOrEmpty(serverProperties))
                return properties;

            string[] lines = serverProperties.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith('#'))
                    continue;

                int index = line.IndexOf('=');
                if (index == -1)
                    throw new FormatException($"Invalid line in server properties: '{line}'");

                string key = line[..index];
                string value = line[(index + 1)..];
                properties.Add(key, value);
            }

            return properties;
        }
    }
}
