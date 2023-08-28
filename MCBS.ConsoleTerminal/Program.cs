#define TryCatch

using FFMediaToolkit;
using log4net.Core;
using QuanLib.Minecraft;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.Logging;
using QuanLib.Minecraft.BlockScreen.Screens;
using QuanLib.Minecraft.BlockScreen.SystemApplications;
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
            Task terminalTask = Task.Run(() => terminal.Start());

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
                Exit();
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
                switch (config.ServerMode)
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
                Exit();
                return;
            }
#endif

#if TryCatch
            try
            {
#endif
                LOGGER.Info("系统开始初始化");
                mcos = MCOS.Load(server);
                LOGGER.Info("系统初始化完成");
#if TryCatch
            }
            catch (Exception ex)
            {
                LOGGER.Fatal("系统无法完成初始化", ex);
                Exit();
                return;
            }
#endif

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

            LOGGER.Info("正在等待Minecraft服务器启动...");
            server.WaitForConnected();
            LOGGER.Info("成功连接到Minecraft服务器");

            bool boo = server.CommandHelper.TryGetInteractionData("5cdf0980-3a61-4fab-9bc5-79a393d8128b", out var result);

            mcos.Start();

            Exit();
            return;

            void Exit()
            {
                terminal.Stop();
                LOGGER.Info("按下回车键退出...");
                terminalTask.Wait();
            }
        }
    }
}
