using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace QuanLib.Minecraft
{
    public class ClientOptions
    {
        static ClientOptions()
        {
            _propertyInfos = [];
            foreach (PropertyInfo propertyInfo in typeof(ClientOptions).GetProperties())
            {
                if (Attribute.GetCustomAttribute(propertyInfo, typeof(OptionAttribute)) is OptionAttribute attribute)
                    _propertyInfos.Add(attribute.Name, propertyInfo);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".SystemResource.options.txt") ?? throw new InvalidOperationException();
            using StreamReader streamReader = new(stream, Encoding.UTF8);
            string clientOptions = streamReader.ReadToEnd();
            Dictionary<string, string> properties = Parse(clientOptions);
            DefaultOptions = Load(properties, false);
        }

        private ClientOptions() { }

        private static readonly Dictionary<string, PropertyInfo> _propertyInfos;
        public static readonly ClientOptions DefaultOptions;

        // 版本
        public const string VERSION = "version";

        // 图形设置
        public const string AO = "ao";
        public const string BIOME_BLEND_RADIUS = "biomeBlendRadius";
        public const string CHUNK_SECTION_FADE_IN_TIME = "chunkSectionFadeInTime";
        public const string CUTOUT_LEAVES = "cutoutLeaves";
        public const string ENABLE_VSYNC = "enableVsync";
        public const string ENTITY_DISTANCE_SCALING = "entityDistanceScaling";
        public const string ENTITY_SHADOWS = "entityShadows";
        public const string FORCE_UNICODE_FONT = "forceUnicodeFont";
        public const string JAPANESE_GLYPH_VARIANTS = "japaneseGlyphVariants";
        public const string FOV = "fov";
        public const string FOV_EFFECT_SCALE = "fovEffectScale";
        public const string DARKNESS_EFFECT_SCALE = "darknessEffectScale";
        public const string GLINT_SPEED = "glintSpeed";
        public const string GLINT_STRENGTH = "glintStrength";
        public const string GRAPHICS_PRESET = "graphicsPreset";
        public const string PRIORITIZE_CHUNK_UPDATES = "prioritizeChunkUpdates";
        public const string FULLSCREEN = "fullscreen";
        public const string EXCLUSIVE_FULLSCREEN = "exclusiveFullscreen";
        public const string GAMMA = "gamma";
        public const string GUI_SCALE = "guiScale";
        public const string MAX_ANISOTROPY_BIT = "maxAnisotropyBit";
        public const string TEXTURE_FILTERING = "textureFiltering";
        public const string MAX_FPS = "maxFps";
        public const string IMPROVED_TRANSPARENCY = "improvedTransparency";
        public const string INACTIVITY_FPS_LIMIT = "inactivityFpsLimit";
        public const string MIPMAP_LEVELS = "mipmapLevels";
        public const string NARRATOR = "narrator";
        public const string PARTICLES = "particles";
        public const string REDUCED_DEBUG_INFO = "reducedDebugInfo";
        public const string RENDER_CLOUDS = "renderClouds";
        public const string CLOUD_RANGE = "cloudRange";
        public const string RENDER_DISTANCE = "renderDistance";
        public const string SIMULATION_DISTANCE = "simulationDistance";
        public const string SCREEN_EFFECT_SCALE = "screenEffectScale";
        public const string SOUND_DEVICE = "soundDevice";
        public const string VIGNETTE = "vignette";
        public const string WEATHER_RADIUS = "weatherRadius";

        // 控制设置
        public const string AUTO_JUMP = "autoJump";
        public const string ROTATE_WITH_MINECART = "rotateWithMinecart";
        public const string OPERATOR_ITEMS_TAB = "operatorItemsTab";
        public const string AUTO_SUGGESTIONS = "autoSuggestions";
        public const string DISCRETE_MOUSE_SCROLL = "discrete_mouse_scroll";
        public const string INVERT_X_MOUSE = "invertXMouse";
        public const string INVERT_Y_MOUSE = "invertYMouse";
        public const string REALMS_NOTIFICATIONS = "realmsNotifications";
        public const string DIRECTIONAL_AUDIO = "directionalAudio";
        public const string TOUCHSCREEN = "touchscreen";
        public const string BOB_VIEW = "bobView";
        public const string TOGGLE_CROUCH = "toggleCrouch";
        public const string TOGGLE_SPRINT = "toggleSprint";
        public const string TOGGLE_ATTACK = "toggleAttack";
        public const string TOGGLE_USE = "toggleUse";
        public const string SPRINT_WINDOW = "sprintWindow";
        public const string MOUSE_SENSITIVITY = "mouseSensitivity";
        public const string MOUSE_WHEEL_SENSITIVITY = "mouseWheelSensitivity";
        public const string RAW_MOUSE_INPUT = "rawMouseInput";
        public const string ALLOW_CURSOR_CHANGES = "allowCursorChanges";
        public const string MAIN_HAND = "mainHand";
        public const string ATTACK_INDICATOR = "attackIndicator";

        // 聊天设置
        public const string CHAT_COLORS = "chatColors";
        public const string CHAT_LINKS = "chatLinks";
        public const string CHAT_LINKS_PROMPT = "chatLinksPrompt";
        public const string SHOW_SUBTITLES = "showSubtitles";
        public const string CHAT_VISIBILITY = "chatVisibility";
        public const string CHAT_OPACITY = "chatOpacity";
        public const string CHAT_LINE_SPACING = "chatLineSpacing";
        public const string TEXT_BACKGROUND_OPACITY = "textBackgroundOpacity";
        public const string BACKGROUND_FOR_CHAT_ONLY = "backgroundForChatOnly";
        public const string CHAT_HEIGHT_FOCUSED = "chatHeightFocused";
        public const string CHAT_DELAY = "chatDelay";
        public const string CHAT_HEIGHT_UNFOCUSED = "chatHeightUnfocused";
        public const string CHAT_SCALE = "chatScale";
        public const string CHAT_WIDTH = "chatWidth";
        public const string NOTIFICATION_DISPLAY_TIME = "notificationDisplayTime";
        public const string ONLY_SHOW_SECURE_CHAT = "onlyShowSecureChat";
        public const string SAVE_CHAT_DRAFTS = "saveChatDrafts";

        // 其他设置
        public const string DARK_MOJANG_STUDIOS_BACKGROUND = "darkMojangStudiosBackground";
        public const string HIDE_LIGHTNING_FLASHES = "hideLightningFlashes";
        public const string HIDE_SPLASH_TEXTS = "hideSplashTexts";
        public const string DAMAGE_TILT_STRENGTH = "damageTiltStrength";
        public const string HIGH_CONTRAST = "highContrast";
        public const string HIGH_CONTRAST_BLOCK_OUTLINE = "highContrastBlockOutline";
        public const string NARRATOR_HOTKEY = "narratorHotkey";
        public const string RESOURCE_PACKS = "resourcePacks";
        public const string INCOMPATIBLE_RESOURCE_PACKS = "incompatibleResourcePacks";
        public const string LAST_SERVER = "lastServer";
        public const string LANG = "lang";
        public const string HIDE_SERVER_ADDRESS = "hideServerAddress";
        public const string ADVANCED_ITEM_TOOLTIPS = "advancedItemTooltips";
        public const string PAUSE_ON_LOST_FOCUS = "pauseOnLostFocus";
        public const string OVERRIDE_WIDTH = "overrideWidth";
        public const string OVERRIDE_HEIGHT = "overrideHeight";
        public const string USE_NATIVE_TRANSPORT = "useNativeTransport";
        public const string TUTORIAL_STEP = "tutorialStep";
        public const string GL_DEBUG_VERBOSITY = "glDebugVerbosity";
        public const string SKIP_MULTIPLAYER_WARNING = "skipMultiplayerWarning";
        public const string HIDE_MATCHED_NAMES = "hideMatchedNames";
        public const string JOINED_FIRST_SERVER = "joinedFirstServer";
        public const string SYNC_CHUNK_WRITES = "syncChunkWrites";
        public const string SHOW_AUTOSAVE_INDICATOR = "showAutosaveIndicator";
        public const string ALLOW_SERVER_LISTING = "allowServerListing";
        public const string PANORAMA_SCROLL_SPEED = "panoramaScrollSpeed";
        public const string TELEMETRY_OPT_IN_EXTRA = "telemetryOptInExtra";
        public const string ONBOARD_ACCESSIBILITY = "onboardAccessibility";
        public const string MENU_BACKGROUND_BLURRINESS = "menuBackgroundBlurriness";
        public const string STARTED_CLEANLY = "startedCleanly";
        public const string MUSIC_TOAST = "musicToast";
        public const string MUSIC_FREQUENCY = "musicFrequency";

        //版本
        [Option(VERSION)] public int Version { get; private set; }

        // 图形设置
        [Option(AO)] public bool Ao { get; private set; }
        [Option(BIOME_BLEND_RADIUS)] public int BiomeBlendRadius { get; private set; }
        [Option(CHUNK_SECTION_FADE_IN_TIME)] public double ChunkSectionFadeInTime { get; private set; }
        [Option(CUTOUT_LEAVES)] public bool CutoutLeaves { get; private set; }
        [Option(ENABLE_VSYNC)] public bool EnableVsync { get; private set; }
        [Option(ENTITY_DISTANCE_SCALING)] public double EntityDistanceScaling { get; private set; }
        [Option(ENTITY_SHADOWS)] public bool EntityShadows { get; private set; }
        [Option(FORCE_UNICODE_FONT)] public bool ForceUnicodeFont { get; private set; }
        [Option(JAPANESE_GLYPH_VARIANTS)] public bool JapaneseGlyphVariants { get; private set; }
        [Option(FOV)] public double Fov { get; private set; }
        [Option(FOV_EFFECT_SCALE)] public double FovEffectScale { get; private set; }
        [Option(DARKNESS_EFFECT_SCALE)] public double DarknessEffectScale { get; private set; }
        [Option(GLINT_SPEED)] public double GlintSpeed { get; private set; }
        [Option(GLINT_STRENGTH)] public double GlintStrength { get; private set; }
        [Option(GRAPHICS_PRESET)] public string GraphicsPreset { get; private set; } = string.Empty;
        [Option(PRIORITIZE_CHUNK_UPDATES)] public int PrioritizeChunkUpdates { get; private set; }
        [Option(FULLSCREEN)] public bool Fullscreen { get; private set; }
        [Option(EXCLUSIVE_FULLSCREEN)] public bool ExclusiveFullscreen { get; private set; }
        [Option(GAMMA)] public double Gamma { get; private set; }
        [Option(GUI_SCALE)] public int GuiScale { get; private set; }
        [Option(MAX_ANISOTROPY_BIT)] public int MaxAnisotropyBit { get; private set; }
        [Option(TEXTURE_FILTERING)] public int TextureFiltering { get; private set; }
        [Option(MAX_FPS)] public int MaxFps { get; private set; }
        [Option(IMPROVED_TRANSPARENCY)] public bool ImprovedTransparency { get; private set; }
        [Option(INACTIVITY_FPS_LIMIT)] public string InactivityFpsLimit { get; private set; } = string.Empty;
        [Option(MIPMAP_LEVELS)] public int MipmapLevels { get; private set; }
        [Option(NARRATOR)] public int Narrator { get; private set; }
        [Option(PARTICLES)] public int Particles { get; private set; }
        [Option(REDUCED_DEBUG_INFO)] public bool ReducedDebugInfo { get; private set; }
        [Option(RENDER_CLOUDS)] public string RenderClouds { get; private set; } = string.Empty;
        [Option(CLOUD_RANGE)] public int CloudRange { get; private set; }
        [Option(RENDER_DISTANCE)] public int RenderDistance { get; private set; }
        [Option(SIMULATION_DISTANCE)] public int SimulationDistance { get; private set; }
        [Option(SCREEN_EFFECT_SCALE)] public double ScreenEffectScale { get; private set; }
        [Option(SOUND_DEVICE)] public string SoundDevice { get; private set; } = string.Empty;
        [Option(VIGNETTE)] public bool Vignette { get; private set; }
        [Option(WEATHER_RADIUS)] public int WeatherRadius { get; private set; }

        // 控制设置
        [Option(AUTO_JUMP)] public bool AutoJump { get; private set; }
        [Option(ROTATE_WITH_MINECART)] public bool RotateWithMinecart { get; private set; }
        [Option(OPERATOR_ITEMS_TAB)] public bool OperatorItemsTab { get; private set; }
        [Option(AUTO_SUGGESTIONS)] public bool AutoSuggestions { get; private set; }
        [Option(DISCRETE_MOUSE_SCROLL)] public bool DiscreteMouseScroll { get; private set; }
        [Option(INVERT_X_MOUSE)] public bool InvertXMouse { get; private set; }
        [Option(INVERT_Y_MOUSE)] public bool InvertYMouse { get; private set; }
        [Option(REALMS_NOTIFICATIONS)] public bool RealmsNotifications { get; private set; }
        [Option(DIRECTIONAL_AUDIO)] public bool DirectionalAudio { get; private set; }
        [Option(TOUCHSCREEN)] public bool Touchscreen { get; private set; }
        [Option(BOB_VIEW)] public bool BobView { get; private set; }
        [Option(TOGGLE_CROUCH)] public bool ToggleCrouch { get; private set; }
        [Option(TOGGLE_SPRINT)] public bool ToggleSprint { get; private set; }
        [Option(TOGGLE_ATTACK)] public bool ToggleAttack { get; private set; }
        [Option(TOGGLE_USE)] public bool ToggleUse { get; private set; }
        [Option(SPRINT_WINDOW)] public int SprintWindow { get; private set; }
        [Option(MOUSE_SENSITIVITY)] public double MouseSensitivity { get; private set; }
        [Option(MOUSE_WHEEL_SENSITIVITY)] public double MouseWheelSensitivity { get; private set; }
        [Option(RAW_MOUSE_INPUT)] public bool RawMouseInput { get; private set; }
        [Option(ALLOW_CURSOR_CHANGES)] public bool AllowCursorChanges { get; private set; }
        [Option(MAIN_HAND)] public string MainHand { get; private set; } = string.Empty;
        [Option(ATTACK_INDICATOR)] public int AttackIndicator { get; private set; }

        // 聊天设置
        [Option(CHAT_COLORS)] public bool ChatColors { get; private set; }
        [Option(CHAT_LINKS)] public bool ChatLinks { get; private set; }
        [Option(CHAT_LINKS_PROMPT)] public bool ChatLinksPrompt { get; private set; }
        [Option(SHOW_SUBTITLES)] public bool ShowSubtitles { get; private set; }
        [Option(CHAT_VISIBILITY)] public int ChatVisibility { get; private set; }
        [Option(CHAT_OPACITY)] public double ChatOpacity { get; private set; }
        [Option(CHAT_LINE_SPACING)] public double ChatLineSpacing { get; private set; }
        [Option(TEXT_BACKGROUND_OPACITY)] public double TextBackgroundOpacity { get; private set; }
        [Option(BACKGROUND_FOR_CHAT_ONLY)] public bool BackgroundForChatOnly { get; private set; }
        [Option(CHAT_HEIGHT_FOCUSED)] public double ChatHeightFocused { get; private set; }
        [Option(CHAT_DELAY)] public double ChatDelay { get; private set; }
        [Option(CHAT_HEIGHT_UNFOCUSED)] public double ChatHeightUnfocused { get; private set; }
        [Option(CHAT_SCALE)] public double ChatScale { get; private set; }
        [Option(CHAT_WIDTH)] public double ChatWidth { get; private set; }
        [Option(NOTIFICATION_DISPLAY_TIME)] public double NotificationDisplayTime { get; private set; }
        [Option(ONLY_SHOW_SECURE_CHAT)] public bool OnlyShowSecureChat { get; private set; }
        [Option(SAVE_CHAT_DRAFTS)] public bool SaveChatDrafts { get; private set; }

        // 其他设置
        [Option(DARK_MOJANG_STUDIOS_BACKGROUND)] public bool DarkMojangStudiosBackground { get; private set; }
        [Option(HIDE_LIGHTNING_FLASHES)] public bool HideLightningFlashes { get; private set; }
        [Option(HIDE_SPLASH_TEXTS)] public bool HideSplashTexts { get; private set; }
        [Option(DAMAGE_TILT_STRENGTH)] public double DamageTiltStrength { get; private set; }
        [Option(HIGH_CONTRAST)] public bool HighContrast { get; private set; }
        [Option(HIGH_CONTRAST_BLOCK_OUTLINE)] public bool HighContrastBlockOutline { get; private set; }
        [Option(NARRATOR_HOTKEY)] public bool NarratorHotkey { get; private set; }
        [Option(RESOURCE_PACKS)] public ReadOnlyCollection<string> ResourcePacks { get; private set; } = ReadOnlyCollection<string>.Empty;
        [Option(INCOMPATIBLE_RESOURCE_PACKS)] public ReadOnlyCollection<string> IncompatibleResourcePacks { get; private set; } = ReadOnlyCollection<string>.Empty;
        [Option(LAST_SERVER)] public string LastServer { get; private set; } = string.Empty;
        [Option(LANG)] public string Lang { get; private set; } = string.Empty;
        [Option(HIDE_SERVER_ADDRESS)] public bool HideServerAddress { get; private set; }
        [Option(ADVANCED_ITEM_TOOLTIPS)] public bool AdvancedItemTooltips { get; private set; }
        [Option(PAUSE_ON_LOST_FOCUS)] public bool PauseOnLostFocus { get; private set; }
        [Option(OVERRIDE_WIDTH)] public int OverrideWidth { get; private set; }
        [Option(OVERRIDE_HEIGHT)] public int OverrideHeight { get; private set; }
        [Option(USE_NATIVE_TRANSPORT)] public bool UseNativeTransport { get; private set; }
        [Option(TUTORIAL_STEP)] public string TutorialStep { get; private set; } = string.Empty;
        [Option(GL_DEBUG_VERBOSITY)] public int GlDebugVerbosity { get; private set; }
        [Option(SKIP_MULTIPLAYER_WARNING)] public bool SkipMultiplayerWarning { get; private set; }
        [Option(HIDE_MATCHED_NAMES)] public bool HideMatchedNames { get; private set; }
        [Option(JOINED_FIRST_SERVER)] public bool JoinedFirstServer { get; private set; }
        [Option(SYNC_CHUNK_WRITES)] public bool SyncChunkWrites { get; private set; }
        [Option(SHOW_AUTOSAVE_INDICATOR)] public bool ShowAutosaveIndicator { get; private set; }
        [Option(ALLOW_SERVER_LISTING)] public bool AllowServerListing { get; private set; }
        [Option(PANORAMA_SCROLL_SPEED)] public double PanoramaScrollSpeed { get; private set; }
        [Option(TELEMETRY_OPT_IN_EXTRA)] public bool TelemetryOptInExtra { get; private set; }
        [Option(ONBOARD_ACCESSIBILITY)] public bool OnboardAccessibility { get; private set; }
        [Option(MENU_BACKGROUND_BLURRINESS)] public int MenuBackgroundBlurriness { get; private set; }
        [Option(STARTED_CLEANLY)] public bool StartedCleanly { get; private set; }
        [Option(MUSIC_TOAST)] public string MusicToast { get; private set; } = string.Empty;
        [Option(MUSIC_FREQUENCY)] public string MusicFrequency { get; private set; } = string.Empty;

        public object? GetOptionValue(string optionName)
        {
            ArgumentException.ThrowIfNullOrEmpty(optionName, nameof(optionName));

            if (!_propertyInfos.TryGetValue(optionName, out var propertyInfo))
                return null;

            return propertyInfo.GetValue(this);
        }

        public T GetOptionValue<T>(string optionName) where T : notnull
        {
            object? value = GetOptionValue(optionName)
                ?? throw new InvalidOperationException($"Option '{optionName}' does not exist.");

            if (value is not T typedValue)
                throw new InvalidCastException($"Option '{optionName}' is not of type {typeof(T).Name}.");

            return typedValue;
        }

        public static ClientOptions Load(IReadOnlyDictionary<string, string> options, bool useDefaultValues = true)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            Func<string, bool> boolParser = bool.Parse;
            Func<string, int> intParser = int.Parse;
            Func<string, double> doubleParser = double.Parse;
            ClientOptions clientOptions = new();

            foreach (var item in _propertyInfos)
            {
                string optionName = item.Key;
                PropertyInfo propertyInfo = item.Value;

                if (options.TryGetValue(optionName, out var optionValue))
                {
                    object? value;
                    if (propertyInfo.PropertyType.Equals(typeof(ReadOnlyCollection<string>)))
                    {
                        if (optionValue == "[]")
                        {
                            value = ReadOnlyCollection<string>.Empty;
                        }
                        else
                        {
                            string pattern = @"""([^""]*)""";
                            MatchCollection matches = Regex.Matches(optionValue, pattern);
                            var results = matches.Select(m => m.Groups[1].Value);
                            value = results.ToArray().AsReadOnly();
                        }
                    }
                    else if (propertyInfo.PropertyType.Equals(typeof(string)))
                    {
                        value = optionValue;
                    }
                    else
                    {
                        try
                        {
                            if (propertyInfo.PropertyType.Equals(typeof(double)))
                                value = doubleParser.Invoke(optionValue);
                            else if (propertyInfo.PropertyType.Equals(typeof(int)))
                                value = intParser.Invoke(optionValue);
                            else if (propertyInfo.PropertyType.Equals(typeof(bool)))
                                value = boolParser.Invoke(optionValue);
                            else if (useDefaultValues)
                                value = DefaultOptions.GetOptionValue(optionName);
                            else
                                continue;
                        }
                        catch (Exception ex) when (ex is ArgumentException or FormatException)
                        {
                            Debug.WriteLine("{0}.{1}: 无法将 \"{2}\" 解析为 \"{3}\" 类型", nameof(ClientOptions), optionName, optionValue, ObjectFormatter.Format(propertyInfo.PropertyType));
                            Debug.WriteLine(ex);

                            if (useDefaultValues)
                                value = DefaultOptions.GetOptionValue(optionName);
                            else
                                continue;
                        }
                    }

                    propertyInfo.SetValue(clientOptions, value);
                }
                else if (useDefaultValues)
                {
                    propertyInfo.SetValue(clientOptions, DefaultOptions.GetOptionValue(optionName));
                }
                else
                {
                    continue;
                }
            }

            return clientOptions;
        }

        public static Dictionary<string, string> Parse(string optionsText)
        {
            Dictionary<string, string> options = [];
            if (string.IsNullOrEmpty(optionsText))
                return options;

            string[] lines = optionsText.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith('#'))
                    continue;

                int index = line.IndexOf(':');
                if (index == -1)
                    throw new FormatException($"Invalid line in options: '{line}'");

                string key = line[..index];
                string value = line[(index + 1)..];

                if (value.Length >= 2 && value.StartsWith('\"') && value.EndsWith('\"'))
                    value = value[1..^1];

                options.Add(key, value);
            }

            return options;
        }
    }
}
