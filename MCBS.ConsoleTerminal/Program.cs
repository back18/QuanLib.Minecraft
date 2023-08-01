using FFMediaToolkit;
using QuanLib.Minecraft;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.SystemApplications;
using QuanLib.Minecraft.Fabric;
using QuanLib.Minecraft.Files;
using QuanLib.Minecraft.Vectors;
using System.Text;

namespace MCBS.ConsoleTerminal
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            //LogFileListener listener = new("D:\\程序\\HMCL\\服务端\\logs\\latest.log");
            ////listener.WriteLog += (sender, e) => Console.WriteLine('\n' + e.MinecraftLog.ToString());
            //ServerLogParser logHelper = new(listener);
            //logHelper.Starting += (sender, e) => Console.WriteLine("开始启动");
            //logHelper.PreparingLevel += (sender, e) => Console.WriteLine("开始加载地图：" + e.Text);
            //logHelper.Started += (sender, e) => Console.WriteLine("启动完成");
            //logHelper.Stopping += (sender, e) => Console.WriteLine("正在停止");
            //logHelper.Stopped +=  (sender, e) => Console.WriteLine("已停止");
            //logHelper.FailToStart += (sender, e) => Console.WriteLine("无法启动：" + e.Text);
            //logHelper.Crashed += (sender, e) => Console.WriteLine("已崩溃：" + e.Guid);
            //logHelper.RconRunning += (sender, e) => Console.WriteLine("RCON启动：" + e.IPEndPoint);
            //logHelper.RconStopped += (sender, e) => Console.WriteLine("RCON停止");
            //logHelper.PlayerJoined += (sender, e) => Console.WriteLine("玩家登录：" + e.PlayerLoginInfo.Name);
            //logHelper.PlayerLeft += (sender, e) => Console.WriteLine("玩家离开：" + e.PlayerLeftInfo.Name);
            //logHelper.PlayerSendChatMessage += (sender, e) => Console.WriteLine("玩家消息：" + e.ChatMessage);
            //listener.Start();

            FFmpegLoader.FFmpegPath = PathManager.FFmpeg_Dir;
            FFmpegLoader.LoadFFmpeg();
            ConfigManager.LoadAll();
            SystemResourcesManager.LoadAll();
            MinecraftResourcesManager.LoadAll();

            MinecraftServer server = new("D:\\程序\\HMCL\\fabric-server-mc.1.20.1-loader.0.14.21-launcher.0.11.2", "127.0.0.1");

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
