using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance.Extensions
{
    public class MicrosoftLoggerAdapter : ILogger
    {
        public MicrosoftLoggerAdapter(Core.ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _logger = logger;
        }

        private readonly Core.ILogger _logger;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace or LogLevel.Debug => _logger.IsDebugEnabled,
                LogLevel.Information => _logger.IsInfoEnabled,
                LogLevel.Warning => _logger.IsWarnEnabled,
                LogLevel.Error => _logger.IsErrorEnabled,
                LogLevel.Critical => _logger.IsFatalEnabled,
                LogLevel.None => false,
                _ => false
            };
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            ArgumentNullException.ThrowIfNull(formatter, nameof(formatter));
            string message = formatter.Invoke(state, null);

            if (string.IsNullOrEmpty(message) && exception is null)
                return;

            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    _logger.Debug(message, exception);
                    break;
                case LogLevel.Information:
                    _logger.Info(message, exception);
                    break;
                case LogLevel.Warning:
                    _logger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    _logger.Error(message, exception);
                    break;
                case LogLevel.Critical:
                    _logger.Fatal(message, exception);
                    break;
                case LogLevel.None:
                    break;
                default:
                    _logger.Debug(message, exception);
                    break;
            }
        }
    }
}
