﻿using System;
using Microsoft.Extensions.Logging;

namespace Martiscoin.Utilities
{
    /// <summary>
    /// Extension methods for classes and interfaces related to logging.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Converts <see cref="Microsoft.Extensions.Logging.LogLevel"/> to <see cref="NLog.LogLevel"/>.
        /// </summary>
        /// <param name="logLevel">Log level value to convert.</param>
        /// <returns>NLog value of the log level.</returns>
        public static NLog.LogLevel ToNLogLevel(this LogLevel logLevel)
        {
            NLog.LogLevel res = NLog.LogLevel.Trace;

            switch (logLevel)
            {
                case LogLevel.Trace: res = NLog.LogLevel.Trace; break;
                case LogLevel.Debug: res = NLog.LogLevel.Debug; break;
                case LogLevel.Information: res = NLog.LogLevel.Info; break;
                case LogLevel.Warning: res = NLog.LogLevel.Warn; break;
                case LogLevel.Error: res = NLog.LogLevel.Error; break;
                case LogLevel.Critical: res = NLog.LogLevel.Fatal; break;
            }

            return res;
        }

        /// <summary>
        /// Converts a string to a <see cref="NLog.LogLevel"/>.
        /// </summary>
        /// <param name="logLevel">Log level value to convert.</param>
        /// <returns>NLog value of the log level.</returns>
        public static NLog.LogLevel ToNLogLevel(this string logLevel)
        {
            logLevel = logLevel.ToLowerInvariant();

            switch (logLevel)
            {
                case "trace":
                    return NLog.LogLevel.Trace;

                case "debug":
                    return NLog.LogLevel.Debug;

                case "info":
                case "information":
                    return NLog.LogLevel.Info;

                case "warn":
                case "warning":
                    return NLog.LogLevel.Warn;

                case "error":
                    return NLog.LogLevel.Error;

                case "fatal":
                case "critical":
                case "crit":
                    return NLog.LogLevel.Fatal;

                case "off":
                    return NLog.LogLevel.Off;

                default:
                    throw new Exception($"Failed converting {logLevel} to a member of NLog.LogLevel.");
            }
        }
    }
}