//#define TryCatch

using FFMediaToolkit;
using log4net.Core;
using QuanLib.Minecraft;
using QuanLib.Minecraft.API;
using QuanLib.Minecraft.API.Instance;
using QuanLib.Minecraft.API.Packet;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.Logging;
using QuanLib.Minecraft.BlockScreen.Screens;
using QuanLib.Minecraft.BlockScreen.SystemApplications;
using QuanLib.Minecraft.Instance;
using QuanLib.Minecraft.ResourcePack.Language;
using System.Net;
using System.Text;

namespace MCBS.ConsoleTerminal
{
    public static class Program
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        private static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "MainThread";
            ConfigManager.CreateIfNotExists();
            LOGGER.Info("Starting!");

            Terminal terminal = new();
            terminal.Start();

#if TryCatch
            try
            {
#endif
                LOGGER.Info("开始加载资源文件");
                ConfigManager.LoadAll();
                SystemResourcesManager.LoadAll();
                MinecraftResourcesManager.LoadAll();
                TextureManager.Load(MCOS.MainDirectory.SystemResources.Textures.Control);
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    FFmpegLoader.FFmpegPath = MCOS.MainDirectory.FFmpeg.FullPath;
                else
                    FFmpegLoader.FFmpegPath = "/usr/lib/";
#if TryCatch
            }
            catch (Exception ex)
            {
                LOGGER.Fatal("无法完成资源文件的加载", ex);
                Exit();
                return;
            }
#endif

            MinecraftInstance minecraftInstance;
            MCOS mcos;

#if TryCatch
            try
            {
#endif
                MinecraftConfig config = ConfigManager.MinecraftConfig;
                switch (config.InstanceType)
                {
                    case InstanceTypes.CLIENT:
                        if (config.MinecraftMode == InstanceKeys.MCAPI)
                            minecraftInstance = new McapiMinecraftClient(config.MinecraftPath, config.ServerAddress, config.McapiPort, config.McapiPassword);
                        else
                            throw new InvalidOperationException();
                        break;
                    case InstanceTypes.SERVER:
                        minecraftInstance = config.MinecraftMode switch
                        {
                            InstanceKeys.RCON => new RconMinecraftServer(config.MinecraftPath, config.ServerAddress),
                            InstanceKeys.CONSOLE => new ConsoleMinecraftServer(config.MinecraftPath, config.ServerAddress, new GenericServerLaunchArguments(config.JavaPath, config.LaunchArguments)),
                            InstanceKeys.HYBRID => new HybridMinecraftServer(config.MinecraftPath, config.ServerAddress, new GenericServerLaunchArguments(config.JavaPath, config.LaunchArguments)),
                            InstanceKeys.MCAPI => new McapiMinecraftServer(config.MinecraftPath, config.ServerAddress, config.McapiPort, config.McapiPassword),
                            _ => throw new InvalidOperationException(),
                        };
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                minecraftInstance.Start();
                Thread.Sleep(1000);
#if TryCatch
            }
            catch (Exception ex)
            {
                LOGGER.Fatal("无法绑定到Minecraft实例", ex);
                Exit();
                return;
            }
#endif

            mcos = MCOS.LoadInstance(minecraftInstance);
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Services.ServicesAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Desktop.DesktopAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Settings.SettingsAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.ScreenManager.ScreenManagerAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.TaskManager.TaskManagerAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer.FileExplorerAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Notepad.NotepadAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Album.AlbumAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Drawing.DrawingAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer.VideoPlayerAppInfo());
            mcos.ApplicationManager.Items.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.DataScreen.DataScreenAppInfo());
            mcos.ApplicationManager.Items.Add(new Test01AppInfo());
            mcos.ApplicationManager.Items.Add(new Test02AppInfo());
            mcos.ApplicationManager.Items.Add(new Test03AppInfo());
            mcos.Start();
            mcos.WaitForStop();

            Exit();
            return;

            void Exit()
            {
                terminal.Stop();
                LOGGER.Info("按下回车键退出...");
                terminal.WaitForStop();
            }
        }
    }
}
