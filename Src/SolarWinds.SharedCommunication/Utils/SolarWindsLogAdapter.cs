using System;
using Microsoft.Extensions.Logging;
using SolarWinds.Logging;

namespace SolarWinds.SharedCommunication.Utils
{
    /// <summary>
    /// A class for the log adapter
    /// </summary>
    public class SolarWindsLogAdapter : ILogger
    {
        public static ILogger CreateLogger() => new SolarWindsLogAdapter();

        Log _log = new Log();

        /// <summary>
        /// A method that does logging based on log level, event identifier, state, thrown exception and a formatter func.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"> log level.</param>
        /// <param name="eventId"> event identifier.</param>
        /// <param name="state"> state.</param>
        /// <param name="exception"> thrown exception.</param>
        /// <param name="formatter"> formatter func</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);
            if (!string.IsNullOrEmpty(message)
                || exception != null)
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        _log.Fatal(message, exception);
                        break;

                    case LogLevel.Debug:
                        _log.Debug(message, exception);
                        break;

                    case LogLevel.Error:
                        _log.Error(message, exception);
                        break;

                    case LogLevel.Information:
                        _log.Info(message, exception);
                        break;

                    case LogLevel.Warning:
                        _log.Warn(message, exception);
                        break;

                    case LogLevel.Trace:
                        _log.Trace(message, exception);
                        break;

                    default:
                        _log.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                        _log.Info(message, exception);
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if the log level is enabled.
        /// </summary>
        /// <param name="logLevel">Log level.</param>
        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return _log.IsFatalEnabled;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    return _log.IsDebugEnabled;
                case LogLevel.Error:
                    return _log.IsErrorEnabled;
                case LogLevel.Information:
                    return _log.IsInfoEnabled;
                case LogLevel.Warning:
                    return _log.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        /// <summary>
        /// This method needs to be implemented.
        /// </summary>
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException(
                "BeginScope intentionally not supported. Will be provided with final version of logger adapter. If blocking now - method can be altered to return empty IDisposable");
        }
    }
}
