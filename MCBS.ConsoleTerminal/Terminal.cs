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

            LOGGER.Info("命令行终端已启动");

            while (_runing)
            {
                string? input = Console.ReadLine();
                if (!MCOS.IsLoaded)
                {
                    Console.WriteLine("【MCBS控制台】MCOS未加载，控制台输入已被禁用");
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
                        Console.WriteLine("【MCBS控制台】stop-------------关闭MCOS并退出程序");
                        break;
                    case "mcconsole":
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
                            int top = Console.CursorTop;
                            while (run)
                            {
                                string empty = new(' ', 16);
                                Console.SetCursorPosition(0, top);
                                for (int i = 0; i < 13; i++)
                                    Console.WriteLine(empty);
                                Console.SetCursorPosition(0, top);
                                Console.WriteLine(MCOS.Instance.SystemTimer.ToString(QuanLib.Minecraft.BlockScreen.Timer.Duration.Tick20));
                                Console.WriteLine($"帧: {MCOS.Instance.FrameCount}");
                                Console.WriteLine($"滞后: {MCOS.Instance.LagFrameCount}");
                                Thread.Sleep(50);
                            }
                        });
                        while (true)
                        {
                            if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                            {
                                run = false;
                                break;
                            }
                        }
                        Console.CursorVisible = true;
                        LogUtil.EnableConsoleOutput();
                        Console.WriteLine("【MCBS控制台】已退出MSPT实时计时器");
                        break;
                    case "stop":
                        MCOS.Instance.Stop();
                        break;
                    default:
                        Console.WriteLine("【MCBS控制台】未知或不完整命令，输入“help”可查看可用命令列表");
                        break;
                }
            }
        }

        public void Stop()
        {
            _runing = false;
        }

        private void RegisterCommands()
        {
            CommandSystem.Pool.AddCommand(new(new("screen lsit"), CommandFunc.GetFunc(GetScreenList)));
            CommandSystem.Pool.AddCommand(new(new("process lsit"), CommandFunc.GetFunc(GetProcessList)));
            CommandSystem.Pool.AddCommand(new(new("form lsit"), CommandFunc.GetFunc(GetFormList)));
            CommandSystem.Pool.AddCommand(new(new("frame count"), CommandFunc.GetFunc(GetFrameCount)));
        }

        private static string GetScreenList()
        {
            var list = MCOS.Instance.ScreenManager.ScreenList;
            StringBuilder sb = new();
            sb.AppendLine($"当前已加载{list.Count}个屏幕，屏幕列表：");
            foreach (var context in list.Values)
                sb.AppendLine(context.ToString());
            sb.Length--;

            return sb.ToString();
        }

        private static string GetProcessList()
        {
            var list = MCOS.Instance.ProcessManager.ProcessList;
            StringBuilder sb = new();
            sb.AppendLine($"当前已启动{list.Count}个进程，进程列表：");
            foreach (var context in list.Values)
                sb.AppendLine(context.ToString());
            sb.Length--;

            return sb.ToString();
        }

        private static string GetFormList()
        {
            var list = MCOS.Instance.FormManager.FormList;
            StringBuilder sb = new();
            sb.AppendLine($"当前已打开{list.Count}个窗体，窗体列表：");
            foreach (var context in list.Values)
                sb.AppendLine(context.ToString());
            sb.Length--;

            return sb.ToString();
        }

        private static int GetFrameCount()
        {
            return MCOS.Instance.FrameCount;
        }
    }
}
