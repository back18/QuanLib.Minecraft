using FFMediaToolkit;
using QuanLib.Minecraft;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.Config;
using QuanLib.Minecraft.BlockScreen.SystemApplications;
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

            //FFmpegLoader.FFmpegPath = PathManager.FFmpeg_Dir;
            //FFmpegLoader.LoadFFmpeg();
            ConfigManager.LoadAll();
            SystemResourcesManager.LoadAll();
            MinecraftResourcesManager.LoadAll();

            MinecraftConfig config = ConfigManager.MinecraftConfig;
            MinecraftServer server = config.MinecraftServerMode switch
            {
                MinecraftServerMode.RconConnect => new RconConnectServer(config.ServerPath, config.ServerAddress),
                MinecraftServerMode.ManagedProcess => new ManagedProcessServer(config.ServerPath, config.ServerAddress, new CustomServerLaunchArguments(config.JavaPath, config.LaunchArguments)),
                _ => throw new InvalidOperationException(),
            };
            Task.Run(() => server.Start());
            server.WaitForConnected();

            MCOS os = new(server);
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Services.ServicesAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Desktop.DesktopAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Settings.SettingsAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.TaskManager.TaskManagerAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer.FileExplorerAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.ImageViewer.ImageViewerAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer.VideoPlayerAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.Drawing.DrawingAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.DataScreen.DataScreenAppInfo());
            os.ApplicationManager.ApplicationList.Add(new QuanLib.Minecraft.BlockScreen.SystemApplications.DialogBox.DialogBoxAppInfo());
            os.ApplicationManager.ApplicationList.Add(new Test01AppInfo());
            os.ApplicationManager.ApplicationList.Add(new Test02AppInfo());
            os.ApplicationManager.ApplicationList.Add(new Test03AppInfo());

            //MCOS.BlockTextureCollection.BuildMapCache(screen.ScreenFacing);
            //Console.WriteLine("完成，按回车继续");
            //Console.ReadLine();

            os.Start();

            //while (true)
            //{
            //    string? read = Console.ReadLine();
            //    if (read is null)
            //        continue;
            //    string output = server.CommandHelper.SendCommand(read);
            //    Console.WriteLine(output);
            //}
        }
    }
}
