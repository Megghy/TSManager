using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TSManager.Core.Attributes;

namespace TSManager
{
    public class Logger
    {
        public enum LogLevel
        {
            Text,
            Info,
            Warn,
            Error,
            Fatal
        }
        public static string LogFileName => Path.Combine(Data.LogPath, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        public const ConsoleColor DefaultColor = ConsoleColor.Gray;
        public static LogLevel DisplayLogLevel { get; set; } = LogLevel.Text;
        public static void Text(object text)
        {
            LogAndSave(text, LogLevel.Text);
        }
        public static void Info(object text)
        {
            LogAndSave(text, LogLevel.Info, LogLevel.Info.ToString(), ConsoleColor.Yellow);
        }
        public static void Error(object text)
        {
            LogAndSave(text, LogLevel.Error, LogLevel.Error.ToString(), ConsoleColor.Red);
        }
        public static void Fatal(object text)
        {
            LogAndSave(text, LogLevel.Fatal, LogLevel.Fatal.ToString(), ConsoleColor.DarkRed);
        }
        public static void Warn(object text)
        {
            LogAndSave(text, LogLevel.Info, LogLevel.Warn.ToString(), ConsoleColor.DarkYellow);
        }
        public static void Success(object text)
        {
            LogAndSave(text, LogLevel.Info, "Success", ConsoleColor.Green);
        }
        private static readonly ConcurrentQueue<string> _queue = new();
        [AutoStart]
        private static void SaveLogTask()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_queue.TryDequeue(out var text))
                        File.AppendAllText(LogFileName, text + Environment.NewLine);
                    else
                        Thread.Sleep(1);
                }
            });
        }
        public static void LogAndSave(object message, LogLevel level, string prefix = "Log", ConsoleColor color = DefaultColor)
        {
            var caller = new StackFrame(2).GetMethod().DeclaringType.Namespace;
            if (level >= DisplayLogLevel)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"[{prefix}] <{caller}> - {message}");
                Console.ForegroundColor = DefaultColor;
            }
            _queue.Enqueue($"{DateTime.Now:HH:mm:ss} - [{prefix}] <{caller}> {message}{Environment.NewLine}");
        }
    }
}
