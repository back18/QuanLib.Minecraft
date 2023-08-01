using FFMediaToolkit;
using QuanLib.Minecraft;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.SystemApplications;
using QuanLib.Minecraft.Fabric;
using QuanLib.Minecraft.Vectors;

namespace MCBS.ConsoleTerminal
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            FFmpegLoader.FFmpegPath = PathManager.FFmpeg_Dir;
            FFmpegLoader.LoadFFmpeg();
            ConfigManager.LoadAll();
            SystemResourcesManager.LoadAll();
            MinecraftResourcesManager.LoadAll();

            //ForgeServer server = new("D:\\程序\\HMCL\\forge-1.19.2-43.2.8-new", "127.0.0.1");
            FabricServer server = new("D:\\程序\\HMCL\\fabric-server-mc.1.20.1-loader.0.14.21-launcher.0.11.2", "127.0.0.1");
            //FabricServer server = new("C:\\Users\\Administrator\\Desktop\\fabric-server-mc.1.20.1-loader.0.14.21-launcher.0.11.2", "127.0.0.1");
            server.OnServerStarting += () => Console.WriteLine("开始启动");
            server.OnPreparingLeveling += (name) => Console.WriteLine("开始加载存档：" + name);
            server.OnPreparingLevelDone += (time) => Console.WriteLine("加载存档耗时：" + time);
            server.OnServerStoping += () => Console.WriteLine("开始终止");
            server.OnServerStopped += () => Console.WriteLine("完成终止");
            server.OnServerStartFail += (err) => Console.WriteLine("启动失败：\n" + err);
            server.OnServerCrash += (guid) => Console.WriteLine("崩溃报告UUID：" + guid);
            server.OnPlayerJoined += (player) => Console.WriteLine("玩家登录：" + player.Name);
            server.OnPlayerLeft += (player, meg) => Console.WriteLine("玩家离开：" + player?.Name);
            server.OnPlayerSendChatMessage += (meg) => Console.WriteLine("玩家消息" + meg);
            server.OnRconRunning += (ipport) => Console.WriteLine("RCON启动：" + ipport);
            server.OnRconStopped += () => Console.WriteLine("RCON终止");
            //server.OnWriteLog += (log) => Console.WriteLine(log + "\n");

            //ServeLauncher launcher = server.CreatNewServerProcess(new("java", "D:\\程序\\HMCL\\forge-1.19.2-43.2.8", "@libraries/net/minecraftforge/forge/1.19.2-43.2.8/win_args.txt", addonArgs: new string[] { "%*" }));
            //launcher.OnServerProcessStart += () => Console.WriteLine("进程启动");
            //launcher.OnServerProcessExit += () => Console.WriteLine("进程终止");
            //launcher.OnServerProcessRestart += () => Console.WriteLine("进程重启");
            //Task.Run(() =>
            //{
            //    launcher.Start();
            //});

            server.ConnectExistingServer();

            var outs = server.CommandHelper.GetPlayersSbnt("@a", "XpSeed");

            MCOS os = new(server);
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Services.ServicesAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Desktop.DesktopAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Settings.SettingsAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.TaskManager.TaskManagerAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer.FileExplorerAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.ImageViewer.ImageViewerAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer.VideoPlayerAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Mspaint.MspaintAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.DataScreen.DataScreenAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.DialogBox.DialogBoxAppInfo());
            os.ApplicationManager.ApplicationList.Add(new Test01AppInfo());
            os.ApplicationManager.ApplicationList.Add(new Test02AppInfo());
            os.ApplicationManager.ApplicationList.Add(new Test03AppInfo());

            //MCOS.BlockTextureCollection.BuildMapCache(screen.ScreenFacing);
            //Console.WriteLine("完成，按回车继续");
            //Console.ReadLine();

            os.Start();

            while (true)
            {
                string? read = Console.ReadLine();
                if (read is null)
                    continue;
                string output = server.CommandHelper.SendCommandAsync(read).Result;
                Console.WriteLine(output);
            }
        }
    }
}
