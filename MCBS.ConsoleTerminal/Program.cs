using QuanLib.Minecraft;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.SystemApplications;
using QuanLib.Minecraft.Fabric;
using QuanLib.Minecraft.Vectors;

namespace MCBS.ConsoleTerminal
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            //var bdf = BDF.BdfFont.Load("unifont-15.0.04.bdf");

            //FontData font = bdf['你'];
            //bool[,] data = font.GetBitMap();
            //for (int y = 0; y < font.Height; y++)
            //{
            //    for (int x = 0; x < font.Width; x++)
            //        Console.Write(data[x, y] ? '■' : '□');
            //    Console.WriteLine();
            //}

            //Dictionary<int, int> counts = new();
            //foreach (var f in bdf)
            //{
            //    counts.TryAdd(f.Value.Height, 0);
            //    counts[f.Value.Height]++;
            //}

            //foreach (var count in  counts)
            //{
            //    Console.WriteLine($"{count.Key}有{count.Value}ge");
            //}

            //var blockTexture = BlockTextureCollection.Load("D:\\程序\\HMCL\\1.19.2\\assets\\minecraft");

            //AccelerationEngine temp = new("127.0.0.1", 0, 12345);
            //temp.Start();
            //List<WorldPixel> pixels = new();
            //pixels.Add(new(new(0, 100, 0), "minecraft:stone"));
            //pixels.Add(new(new(0, 101, 0), "minecraft:cobblestone"));
            //pixels.Add(new(new(0, 102, 0), "minecraft:stone"));
            //pixels.Add(new(new(0, 103, 0), "minecraft:gold_block"));
            //pixels.Add(new(new(0, 104, 0), "minecraft:diamond_block"));
            //byte[] bytes = AccelerationEngine.DataPacket.ToDataPacket(pixels).ToBytes();
            //temp.SendData(bytes);

            Screen screen1 = Screen.CreateScreen(new(0, 100, 0), new(0, 0, -100));

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

            //Screen screen = new(new(50, 206, 100), Facing.Zm, Facing.Ym, 256, 144);
            //Screen screen = new(new(250, 206, 300), Facing.Xm, Facing.Ym, 256, 144);
            //Screen screen = new(new(-207, 62, -160), Facing.Zm, Facing.Xp, 256, 256);
            //Screen screen = new(new(64, 62, -192), Facing.Xp, Facing.Zp, 128, 128);

            Screen screen = new(new(440, 206, -90), Facing.Xm, Facing.Ym, 256, 144);
            AccelerationEngine ae = new("127.0.0.1", 0, 12345);
            ScreenInputReader cursor = new("snowball_mouse");
            MCOS os = new(server, screen, cursor, ae);

            //screen.Start();
            //screen.Stop();

            //MCOS.BlockTextureCollection.BuildMapCache(screen.ScreenFacing);
            //Console.WriteLine("完成，按回车继续");
            //Console.ReadLine();

            //Dictionary<MinecraftColor, string> _ids = new();
            //for (int i = 0; i < 16; i++)
            //{
            //    MinecraftColor color = (MinecraftColor)i;
            //    _ids.Add(color, $"minecraft:{CommandUtil.MinecraftColorToString(color)}_concrete");
            //}
            //foreach (var id in _ids.Values)
            //{
            //    BlockTexture texture = MCOS.BlockTextureCollection[id];
            //    var color = texture.AverageColors[Facing.Yp];
            //    Console.WriteLine($"{id}\t{Color.FromRgba(color.R, color.G, color.B, color.A)}");
            //}

            os.Initialize();
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.Services.ServicesAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.Desktop.DesktopAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.Settings.SettingsAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.TaskManager.TaskManagerAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer.FileExplorerAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.ImageViewer.ImageViewerAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer.VideoPlayerAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.Mspaint.MspaintAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.DataScreen.DataScreenAppInfo());
            os.RegisterApp(new QuanLib.Minecraft.BlockScreen.SystemApplications.DialogBox.DialogBoxAppInfo());
            os.RegisterApp(new Test01AppInfo());
            os.RegisterApp(new Test02AppInfo());
            os.RegisterApp(new Test03AppInfo());
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
