using System;
using System.Diagnostics;
using System.IO;
using TSManager.Core.Attributes;

namespace TSManager
{
    public class Logs
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
            LogAndSave(text, LogLevel.Info, "[Info]", ConsoleColor.Yellow);
        }
        public static void Error(object text)
        {
            LogAndSave(text, LogLevel.Error, "[Error]", ConsoleColor.Red);
        }
        public static void Fatal(object text)
        {
            LogAndSave(text, LogLevel.Fatal, "[Fatal]", ConsoleColor.Red);
        }
        public static void Warn(object text)
        {
            LogAndSave(text, LogLevel.Info, "[Warn]", ConsoleColor.DarkYellow);
        }
        public static void Success(object text)
        {
            LogAndSave(text, LogLevel.Info, "[Success]", ConsoleColor.Green);
        }
        public static void LogAndSave(object message, LogLevel level, string prefix = "[Log]", ConsoleColor color = DefaultColor)
        {
            var caller = new StackFrame(2).GetMethod().DeclaringType.Namespace;
            if (level >= DisplayLogLevel)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"[{caller}] {prefix} {message}");
                Console.ForegroundColor = DefaultColor;
            }
            File.AppendAllText(LogFileName, $"{DateTime.Now:HH:mm:ss} - <{caller}> {prefix} {message}{Environment.NewLine}");
        }
    }
}
