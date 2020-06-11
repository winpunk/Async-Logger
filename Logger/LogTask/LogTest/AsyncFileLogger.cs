namespace LogTest
{
    using System;
    using System.Diagnostics;
    using System.IO.Abstractions;
    using System.Threading;

    public class AsyncFileLogger : ILog
    {
        private Thread _runThread;

        private ILoggerAgent _loggerAgent;

        public AsyncFileLogger() : this(new FileSystem(), new LogFile())
        {
        }

        public AsyncFileLogger(IFileSystem fileSystem, ILogFile logFile)
        {            
            _loggerAgent = new LoggerAgent(fileSystem, logFile);

            _runThread = new Thread(_loggerAgent.LoggerLoop);
            _runThread.Start();
        }

        public void StopWithoutFlush()
        {
            _loggerAgent.IsExit = true;
        }

        public void StopWithFlush()
        {
            _loggerAgent.IsQuitWithFlush = true;

            while (!_loggerAgent.IsDoneWritting)
            {
                Thread.Sleep(1);
            }
        }

        public void Write(string text)
        {
            try
            {
                _loggerAgent.logLines.TryAdd(new LogLine(text, DateTime.Now), 10);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}