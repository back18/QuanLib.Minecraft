using log4net.Core;
using QuanLib;
using QuanLib.CommandLine;
using QuanLib.CommandLine.ConsoleCommand;
using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Level = QuanLib.CommandLine.Level;

namespace MCBS.ConsoleTerminal
{
    public class Terminal : ISwitchable
    {
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        public Terminal()
        {
            CommandSystem = new(new(Level.Root));
            RegisterCommands();

            _runing = false;
        }

        private bool _runing;

        public bool Runing => _runing;

        public CommandSystem CommandSystem { get; }

        public void Start()
        {
            if (_runing)
                return;
            _runing = true;

            LOGGER.Info("命令行终端已开始运行");

            while (_runing)
            {
                string? input = Console.ReadLine();
                if (_runing == false)
                {
                    break;
                }
                if (!MCOS.IsLoaded)
                {
                    Console.WriteLine("【MCBS控制台】系统未加载，控制台输入已被禁用");
                    continue;
                }
                if (input is null)
                {
                    continue;
                }

                switch (input)
                {
                    case "help":
                        Console.WriteLine("【MCBS控制台】mcconsole--------Minecraft控制台");
                        Console.WriteLine("【MCBS控制台】commandsystem----可视化命令系统");
                        Console.WriteLine("【MCBS控制台】timer------------MSPT实时计时器");
                        Console.WriteLine("【MCBS控制台】stop-------------终止系统并退出程序");
                        break;
                    case "mcconsole":
                    case "mcc":
                        Console.WriteLine("【MCBS控制台】已进入Minecraft控制台");
                        LogUtil.DisableConsoleOutput();
                        MCOS.Instance.MinecraftServer.EnableConsoleOutput();
                        while (true)
                        {
                            string? input2 = Console.ReadLine();
                            if (string.IsNullOrEmpty(input2))
                                break;

                            string output2 = MCOS.Instance.MinecraftServer.CommandHelper.SendCommand(input2);
                            Console.WriteLine(output2);
                        }
                        MCOS.Instance.MinecraftServer.DisableConsoleOutput();
                        LogUtil.EnableConsoleOutput();
                        Console.WriteLine("【MCBS控制台】已退出Minecraft控制台");
                        break;
                    case "commandsystem":
                    case "cs":
                        Console.WriteLine("【MCBS控制台】已进入可视化命令系统");
                        LogUtil.DisableConsoleOutput();
                        CommandSystem.Start();
                        LogUtil.EnableConsoleOutput();
                        Console.WriteLine("【MCBS控制台】已退出可视化命令系统");
                        break;
                    case "timer":
                        Console.WriteLine("【MCBS控制台】已进入MSPT实时计时器");
                        LogUtil.DisableConsoleOutput();
                        Console.CursorVisible = false;
                        bool run = true;
                        Task.Run(() =>
                        {
                            string empty = new(' ', 32);
                            int lines = 14;
                            for (int i = 0; i < lines; i++)
                                Console.WriteLine(empty);
                            while (run)
                            {
                                Console.CursorTop -= lines;
                                for (int i = 0; i < lines; i++)
                                    Console.WriteLine(empty);
                                Console.CursorTop -= lines;
                                Console.WriteLine(MCOS.Instance.SystemTimer.ToString(QuanLib.Minecraft.BlockScreen.Timer.Duration.Tick20));
                                Console.WriteLine($"帧: {MCOS.Instance.FrameCount}");
                                Console.WriteLine($"滞后: {MCOS.Instance.LagFrameCount}");
                                Thread.Sleep(50);
                            }
                        });
                        while (true)
                        {
                            if (Console.KeyAvailable)
                            {
                                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                                {
                                    run = false;
                                    break;
                                }
                            }
                            else
                            {
                                Thread.Sleep(10);
                            }
                        }
                        Console.CursorVisible = true;
                        LogUtil.EnableConsoleOutput();
                        Console.WriteLine("【MCBS控制台】已退出MSPT实时计时器");
                        break;
                    case "stop":
                        if (MCOS.Instance.Runing)
                        {
                            MCOS.Instance.Stop();
                            _runing = false;
                        }
                        else
                        {
                            Console.WriteLine("【MCBS控制台】系统未开始运行，因此无法关闭");
                        }
                        break;
                    default:
                        Console.WriteLine("【MCBS控制台】未知或不完整命令，输入“help”可查看可用命令列表");
                        break;
                }
            }

            _runing = false;
            LOGGER.Info("命令行终端已终止运行");
        }

        public void Stop()
        {
            _runing = false;
        }

        private void RegisterCommands()
        {
            CommandSystem.Pool.AddCommand(new(new("application list"), CommandFunc.GetFunc(GetApplicationList)));
            CommandSystem.Pool.AddCommand(new(new("screen list"), CommandFunc.GetFunc(GetScreenList)));
            CommandSystem.Pool.AddCommand(new(new("screen close"), CommandFunc.GetFunc(CloseScreen)));
            CommandSystem.Pool.AddCommand(new(new("screen builder"), CommandFunc.GetFunc(SetScreenBuilderEnable)));
            CommandSystem.Pool.AddCommand(new(new("process list"), CommandFunc.GetFunc(GetProcessList)));
            CommandSystem.Pool.AddCommand(new(new("form list"), CommandFunc.GetFunc(GetFormList)));
            CommandSystem.Pool.AddCommand(new(new("frame count"), CommandFunc.GetFunc(GetFrameCount)));
        }

        #region commands

        private static string GetApplicationList()
        {
            var list = MCOS.Instance.ApplicationManager.Items;
            StringBuilder sb = new();
            sb.AppendLine($"当前已加载{list.Count}个应用程序，应用程序列表：");
            foreach (var appInfo in list.Values)
                sb.AppendLine(appInfo.ToString());

            return sb.ToString().TrimEnd();
        }

        private static string GetScreenList()
        {
            var list = MCOS.Instance.ScreenManager.Items;
            StringBuilder sb = new();
            sb.AppendLine($"当前已加载{list.Count}个屏幕，屏幕列表：");
            foreach (var context in list.Values)
                sb.AppendLine(context.ToString());

            return sb.ToString().TrimEnd();
        }

        private static string CloseScreen(int id)
        {
            if (MCOS.Instance.ScreenManager.Items.TryGetValue(id, out var context))
            {
                context.CloseScreen();
                return $"屏幕“{context}”已关闭";
            }
            else
            {
                return "未知的屏幕ID";
            }
        }

        private static string SetScreenBuilderEnable(bool enable)
        {
            MCOS.Instance.ScreenManager.ScreenBuilder.Enable = enable;

            if (enable)
                return "屏幕构造器已启用";
            else
                return "屏幕构造器已禁用";
        }

        private static string GetProcessList()
        {
            var list = MCOS.Instance.ProcessManager.Items;
            StringBuilder sb = new();
            sb.AppendLine($"当前已启动{list.Count}个进程，进程列表：");
            foreach (var context in list.Values)
                sb.AppendLine(context.ToString());

            return sb.ToString().TrimEnd();
        }

        private static string GetFormList()
        {
            var list = MCOS.Instance.FormManager.Items;
            StringBuilder sb = new();
            sb.AppendLine($"当前已打开{list.Count}个窗体，窗体列表：");
            foreach (var context in list.Values)
                sb.AppendLine(context.ToString());

            return sb.ToString().TrimEnd();
        }

        private static int GetFrameCount()
        {
            return MCOS.Instance.FrameCount;
        }

        #endregion
    }
}
