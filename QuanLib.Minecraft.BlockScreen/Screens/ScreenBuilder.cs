#define TryCatch

using static QuanLib.Minecraft.BlockScreen.Config.ConfigManager;
using CoreRCON.Parsers.Standard;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.Data;
using QuanLib.Minecraft.Selectors;
using QuanLib.Minecraft.Vector;
using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using QuanLib.Minecraft.BlockScreen.Logging;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public class ScreenBuilder
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        public ScreenBuilder()
        {
            _contexts = new();

            Enable = true;
        }

        private readonly Dictionary<string, ScreenBuildContext> _contexts;

        public bool Enable { get; set; }

        public void Handle()
        {
            ServerCommandHelper command = MCOS.Instance.MinecraftServer.CommandHelper;
            foreach (var context in _contexts.ToArray())
            {
                switch (context.Value.BuildState)
                {
                    case ScreenBuildState.Timedout:
                        context.Value.Screen?.Clear();
                        command.SendChatMessage(new GenericSelector(context.Key), "[屏幕构建器] 操作超时，已取消本次屏幕创建", TextColor.Red);
                        _contexts.Remove(context.Key);
                        break;
                    case ScreenBuildState.Canceled:
                        context.Value.Screen?.Clear();
                        command.SendChatMessage(new GenericSelector(context.Key), "[屏幕构建器] 已取消本次屏幕创建", TextColor.Red);
                        _contexts.Remove(context.Key);
                        break;
                    case ScreenBuildState.Completed:
                        Screen? screen = context.Value.Screen;
                        if (screen is null)
                        {
                            command.SendChatMessage(new GenericSelector(context.Key), "[屏幕构建器] 未知错误，创建失败", TextColor.Red);
                        }
                        else
                        {
                            if (MCOS.Instance.ScreenManager.Items.Count >= ScreenConfig.MaxCount)
                            {
                                screen.Clear();
                                command.SendChatMessage(new GenericSelector(context.Key), $"[屏幕构建器] 当前屏幕数量达到最大数量限制{ScreenConfig.MaxCount}个，无法继续创建屏幕", TextColor.Red);
                            }
                            else
                            {
#if TryCatch
                                try
                                {
#endif
                                    MCOS.Instance.LoadScreen(screen);
                                    command.SendChatMessage(new GenericSelector(context.Key), "[屏幕构建器] 已完成本次屏幕创建");
#if TryCatch
                                }
                                catch (Exception ex)
                                {
                                    LOGGER.Error($"屏幕“{screen}”无法加载", ex);
                                    command.SendChatMessage(new GenericSelector(context.Key), $"[屏幕构建器] 屏幕构建失败: {ex.GetType()}: {ex.Message}", TextColor.Red);
                                }
#endif
                            }
                        }
                        _contexts.Remove(context.Key);
                        break;
                }
            }

            if (!Enable || MCOS.Instance.ScreenManager.Items.Count >= ScreenConfig.MaxCount)
                return;

            Dictionary<string, Item> items = command.GetAllPlayerSelectedItem();
            foreach (var item in items)
            {
                if (_contexts.ContainsKey(item.Key))
                    continue;

                if (item.Value.Tag is not null &&
                item.Value.Tag.TryGetValue("display", out var display) &&
                display is Dictionary<string, object> displayTag &&
                displayTag.TryGetValue("Name", out var name) &&
                name is string nameString)
                {
                    JObject nameJson;
                    try
                    {
                        nameJson = JObject.Parse(nameString);
                    }
                    catch
                    {
                        continue;
                    }

                    string? text = nameJson["text"]?.Value<string>();
                    if (text is null || text != ScreenConfig.ScreenBuildItemName)
                        continue;

                    if (ScreenConfig.ScreenBuildOperatorList.Count != 0 && !ScreenConfig.ScreenBuildOperatorList.Contains(item.Key))
                    {
                        command.SendActionbarTitle(new GenericSelector(item.Key), $"[屏幕构建器] 错误：你没有权限创建屏幕", TextColor.Red);
                        continue;
                    }

                    _contexts.Add(item.Key, new(item.Key));
                    command.SendChatMessage(new GenericSelector(item.Key), "[屏幕构建器] 已载入屏幕创建程序");
                }
            }

            foreach (var context in _contexts.Values)
            {
                context.Handle();
            }
        }
    }
}
