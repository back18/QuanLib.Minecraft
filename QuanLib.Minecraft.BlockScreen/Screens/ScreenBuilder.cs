using CoreRCON.Parsers.Standard;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.BlockScreen.Config;
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

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public class ScreenBuilder
    {
        public ScreenBuilder()
        {
            _contexts = new();
        }

        private readonly Dictionary<string, ScreenBuildContext> _contexts;

        public void Handle()
        {
            ServerCommandHelper command = MCOS.Instance.MinecraftServer.CommandHelper;
            foreach (var context in _contexts.ToArray())
            {
                switch (_contexts[context.Key].BuildState)
                {
                    case ScreenBuildState.Timedout:
                        command.SendChatMessage(new PlayerSelector(context.Key), "[屏幕构建器] 操作超时，已取消本次屏幕创建");
                        context.Value.Screen?.Clear();
                        _contexts.Remove(context.Key);
                        break;
                    case ScreenBuildState.Canceled:
                        command.SendChatMessage(new PlayerSelector(context.Key), "[屏幕构建器] 你已取消本次屏幕创建");
                        context.Value.Screen?.Clear();
                        _contexts.Remove(context.Key);
                        break;
                    case ScreenBuildState.Completed:
                        Screen? screen = context.Value.Screen;
                        if (screen is null)
                        {
                            command.SendChatMessage(new PlayerSelector(context.Key), "[屏幕构建器] 未知错误，创建失败");
                        }
                        else
                        {
                            command.SendChatMessage(new PlayerSelector(context.Key), "[屏幕构建器] 已完成本次屏幕创建");
                            MCOS.Instance.ScreenManager.ScreenList.Add(screen);
                        }
                        _contexts.Remove(context.Key);
                        break;
                }
            }

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
                    if (text is null || text != "创建屏幕")
                        continue;

                    command.SendChatMessage(new PlayerSelector(item.Key), "[屏幕构建器] 已载入屏幕创建程序");
                    _contexts.Add(item.Key, new(item.Key));
                }
            }

            foreach (var context in _contexts.Values)
            {
                context.Handle();
            }
        }
    }
}
