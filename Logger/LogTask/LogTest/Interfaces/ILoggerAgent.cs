using System.Collections.Concurrent;

namespace LogTest
{
    public interface ILoggerAgent
    {
        BlockingCollection<LogLine> logLines { get; set; }
        bool IsExit { get; set; }
        bool IsDoneWritting { get; set; }
        bool IsQuitWithFlush { get; set; }

        void LoggerLoop();
    }
}