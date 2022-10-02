using System;
using System.IO;

namespace Framework
{
    public enum LogLevel
    {
        Error,
        Warning,
        Info,
        Dbg1,
        Dbg2,
        Dbg3,
		None
    }

    public static class Log
    {
        private static bool active;
        private static StreamWriter writer;
        private static LogLevel level;

        public static void Init(string filePath, LogLevel logLevel)
        {
            active = logLevel != LogLevel.None;
            if (active)
            {
                level = logLevel;
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                var path = Path.Combine(filePath, DateTime.Now.ToString("yyyy_MM_dd") + ".log");
                writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
                writer.WriteLine("----------------------------------------------------------------------------------------");
                writer.WriteLine("start at " + DateTime.Now.ToString("HH:mm:ss"));
                writer.WriteLine("");
                writer.Flush();
            }
        }

        public static void Out(LogLevel logLevel, string text)
        {
			if (active)
			{
				if (logLevel <= level)
				{
                    text = $"{DateTime.Now.ToString("HH:mm:ss.fff")} | {logLevel} | {text}";
					writer.WriteLine(text);
					writer.Flush();
				}
			}
        }

        public static void Close()
        {
            if (active)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }
    }
}
