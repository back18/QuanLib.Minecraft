#define TryCatch

using FFMediaToolkit;
using log4net.Core;
using QuanLib.Minecraft;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.Logging;
using QuanLib.Minecraft.BlockScreen.SystemApplications;
using QuanLib.Minecraft.Data;
using System.Text;

namespace MCBS.ConsoleTerminal
{
    public static class Program
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        private static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "MainThread";
            LOGGER.Info("Starting!");

            Terminal terminal = new();
            Task.Run(() => terminal.Start());

#if TryCatch
            try
            {
#endif
                ConfigManager.LoadAll();
                SystemResourcesManager.LoadAll();
                MinecraftResourcesManager.LoadAll();
                TextureManager.Load(MCOS.MainDirectory.SystemResources.Textures.Control);
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    FFmpegLoader.FFmpegPath = MCOS.MainDirectory.FFmpeg.Directory;
                else
                    FFmpegLoader.FFmpegPath = "/usr/lib/";
#if TryCatch
            }
            catch (Exception ex)
            {
                LOGGER.Fatal("无法完成初始化", ex);
                Console.ReadLine();
                return;
            }
#endif

            MinecraftServer server;
            MCOS mcos;

#if TryCatch
            try
            {
#endif
                MinecraftConfig config = ConfigManager.MinecraftConfig;
                switch (config.MinecraftServerMode)
                {
                    case MinecraftServerMode.RconConnect:
                        LOGGER.Info($"将以RCON连接模式绑定到Minecraft服务器\n服务端路径: {config.ServerPath}\n服务器地址: {config.ServerAddress}");
                        server = new RconConnectServer(config.ServerPath, config.ServerAddress);
                        break;
                    case MinecraftServerMode.ManagedProcess:
                        LOGGER.Info($"将以托管进程模式绑定到Minecraft服务器\n服务端路径: {config.ServerPath}\n服务器地址: {config.ServerAddress}\nJava路径: {config.JavaPath}\n 启动参数: {config.LaunchArguments}");
                        server = new ManagedProcessServer(config.ServerPath, config.ServerAddress, new CustomServerLaunchArguments(config.JavaPath, config.LaunchArguments));
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                Task.Run(() => server.Start());
#if TryCatch
            }
            catch (Exception ex)
            {
                LOGGER.Fatal("无法绑定到Minecraft服务器", ex);
                Console.ReadLine();
                return;
            }
#endif

#if TryCatch
            try
            {
#endif
                LOGGER.Info("开始初始化MCOS");
                mcos = MCOS.Load(server);
                LOGGER.Info("MCOS初始化完成");
#if TryCatch
            }
            catch (Exception ex)
            {
                LOGGER.Fatal("无法初始化MCOS", ex);
                Console.ReadLine();
                return;
            }
#endif

            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Services.ServicesAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Desktop.DesktopAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Settings.SettingsAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.ScreenManager.ScreenManagerAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.TaskManager.TaskManagerAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer.FileExplorerAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Notepad.NotepadAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Album.AlbumAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Drawing.DrawingAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer.VideoPlayerAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.DataScreen.DataScreenAppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new Test01AppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new Test02AppInfo());
            mcos.ApplicationManager.ApplicationList.Add(new Test03AppInfo());

            LOGGER.Info("正在等待Minecraft服务器启动...");
            server.WaitForConnected();
            LOGGER.Info("成功连接到Minecraft服务器");

#if TryCatch
            try
            {
#endif
                mcos.Start();
#if TryCatch
            }
            catch (Exception ex)
            {
                LOGGER.Fatal("MCOS运行时出现意外异常", ex);
                Console.ReadLine();
                return;
            }
#endif
        }
    }
}
