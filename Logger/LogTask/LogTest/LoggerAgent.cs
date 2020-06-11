using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;

namespace LogTest
{
    public class LoggerAgent : ILoggerAgent
    {
        private readonly IFileSystem _fileSystem;
        private StreamWriter _writer;
        public BlockingCollection<LogLine> logLines { get; set; } = new BlockingCollection<LogLine>();
        private NewDayChecker _newDayChecker;
        private ILogFile _logFile;
        public bool IsExit { get; set; } = false;
        public bool IsDoneWritting { get; set; } = false;
        public bool IsQuitWithFlush { get; set; } = false;

        private string _filePath = @"C:\LogTest\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log";

        public LoggerAgent(IFileSystem fileSystem, ILogFile logFile)
        {
            _logFile = logFile;
            _newDayChecker = new NewDayChecker(DateTime.Now, fileSystem);
            _fileSystem = fileSystem;
        }

        public void LoggerLoop()
        {
            try
            {
                if (_logFile.CreateNewDirectory(_fileSystem))
                {
                    _writer = _fileSystem.File.CreateText(_filePath);
                    _writer.AutoFlush = true;
                }
                   

                using (_writer)
                {
                    _logFile.CreateNewFile(_writer);

                    while (!IsExit)
                    {
                        while (logLines.TryTake(out var logLine, 10) && !IsExit)
                        {
                            _newDayChecker.Check(DateTime.Now, _logFile, ref _writer);

                            _logFile.WriteLog(_writer, logLine.TimeStampText() + logLine.LineText());
                        }

                        if (IsQuitWithFlush == true && logLines.Count == 0) IsExit = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
            finally
            {
                IsDoneWritting = true;
            }
        }
    }
}