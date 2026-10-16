using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace CribbageRoyale.Server.Logging
{
    public static class ApplicationLogging
    {
        private const string LogFolderName = "Logs";
        private const string LogFileName = "cribbage-royale-server-.log";
        private const int RetainedFileCount = 7;
        private const string OutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}";

        private static ILoggerFactory _loggerFactory;

        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    BuildLogFilePath(),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: RetainedFileCount,
                    outputTemplate: OutputTemplate)
                .CreateLogger();

            _loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));
        }

        public static ILogger CreateLogger<TCategory>()
        {
            if (_loggerFactory == null)
            {
                Initialize();
            }

            return _loggerFactory.CreateLogger<TCategory>();
        }

        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }

        private static string BuildLogFilePath()
        {
            string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFolderName);
            return Path.Combine(logFolder, LogFileName);
        }
    }
}