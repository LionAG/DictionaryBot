using DictionaryBot.Components.Enums;
using DictionaryBot.Components.Extensions;
using Discord;

namespace DictionaryBot.Components
{

    internal class LogData
    {
        public string Source { get; init; }
        public string Message { get; init; }
        public LogImportance Importance { get; init; }

        public LogData(string Source, string Message, LogImportance Importance)
        {
            this.Source = Source;
            this.Message = Message;
            this.Importance = Importance;
        }
    }

    internal class Logger
    {
        public static bool IncludeTime { get; set; } = true;

        private static ConsoleColor GetConsoleColor(LogImportance importance)
        {
            switch (importance)
            {
                case LogImportance.Error: return ConsoleColor.DarkRed;
                case LogImportance.Warning: return ConsoleColor.DarkYellow;
                case LogImportance.Critical: return ConsoleColor.Red;
                case LogImportance.Debug: return ConsoleColor.Blue;
                case LogImportance.Verbose: return ConsoleColor.White;
                case LogImportance.Info: return ConsoleColor.Green;
                default: break;
            }

            return ConsoleColor.White;
        }

        private static string GetLogTypeName(LogImportance importance)
        {
            return (Enum.GetName(typeof(LogSeverity), importance) ?? "UNKNOWN").ToUpperInvariant();
        }

        private static void PrintLog(LogData logData)
        {
            if (IncludeTime)
                Console.Write($"[{DateTime.Now}] ");

            Console.Write("[");

            Console.ForegroundColor = GetConsoleColor(logData.Importance);
            Console.Write(GetLogTypeName(logData.Importance));
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write($"]");
            Console.Write(new string(' ', logData.Importance.GetAlignCount()));

            Console.WriteLine($"{logData.Source} -> {logData.Message}");
        }

        public static void Log(LogData logData)
        {
            PrintLog(logData);
        }

        public static void Log(LogMessage Message)
        {
            PrintLog(new(Message.Source, Message.Message, (LogImportance)Message.Severity));
        }
    }
}
