using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Logging
{
    public static class LogUtil
    {
        static LogUtil()
        {
            if (!File.Exists(MCOS.MainDirectory.Configs.Log4Net))
            {
                using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("QuanLib.Minecraft.BlockScreen.Config.Default.log4net.xml") ?? throw new IndexOutOfRangeException();
                using FileStream fileStream = new(MCOS.MainDirectory.Configs.Log4Net, FileMode.Create);
                stream.CopyTo(fileStream);
                Console.WriteLine($"配置文件“{MCOS.MainDirectory.Configs.Log4Net}”不存在，已创建默认配置文件");
            }
            XmlConfigurator.Configure(new FileInfo(MCOS.MainDirectory.Configs.Log4Net));
            _repository = (Hierarchy)LogManager.GetRepository();
            _console = new();
            PatternLayout layout = new("[%date{HH:mm:ss}] [%t/%p] [%c]: %m%n");
            layout.ActivateOptions();
            _console.Layout = layout;
            _console.ActivateOptions();
            MainLogger = GetLogger("McbsMain");
        }

        private static readonly Hierarchy _repository;

        private static readonly ConsoleAppender _console;

        public static LogImpl MainLogger { get; }

        public static LogImpl GetLogger(string name)
        {
            Logger logger = _repository.LoggerFactory.CreateLogger(_repository, name);
            logger.Hierarchy = _repository;
            logger.Parent = _repository.Root;
            logger.Level = Level.All;
            logger.Additivity = false;
            logger.AddAppender(_console);

            return new(logger);
        }

        public static void EnableConsoleOutput()
        {
            _console.Threshold = Level.All;
        }

        public static void DisableConsoleOutput()
        {
            _console.Threshold = Level.Off;
        }
    }
}
