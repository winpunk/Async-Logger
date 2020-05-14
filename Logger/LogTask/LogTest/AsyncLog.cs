namespace LogTest
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    public class AsyncLog : ILog
    {
        private Thread _runThread;

        //  For thread-safe operation better to use BlockingCollection<>.
        private BlockingCollection<LogLine> _logLines = new BlockingCollection<LogLine>();

        private string _filepath = "";

        // Header for every new file created.
        private static readonly string _header = "Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine;

        private bool _isExit = false;

        private bool _isQuitWithFlush = false;

        private DateTime _currentDate = DateTime.Now;

        public AsyncLog()
        {
            try
            {
                //  No need for checking if directory exists, "CreateDirectory" does it for you.
                Directory.CreateDirectory(@"C:\LogTest");

                _filepath = @"C:\LogTest\Log" + _currentDate.ToString("yyyyMMdd HHmmss fff") + ".log";

                //  It is better to use StreamWriter with using statement (AutoFlush by default - false, for better performance).
                using (StreamWriter _writer = new StreamWriter(_filepath, true))
                {
                    _writer.Write(_header);
                }

                //  Starting new Thread to write logs Asynchronously.
                _runThread = new Thread(WriterLoop);
                _runThread.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
                return;
            }
        }

        private void WriterLoop()
        {
            while (!_isExit)
            {
                try
                {
                    //  For thread-safe operation better to use BlockingCollection<>. Also it automatically removes item from a list.
                    while (_logLines.TryTake(out var logLine) && !_isExit)
                    {
                        // Moved code for checking if it is a new day to a function. For better readability and testing.
                        MidnightCrossingCheck(DateTime.Now);

                        // Write log using StreamWriter.
                        using (StreamWriter _writer = new StreamWriter(_filepath, true))
                        {
                            _writer.Write(logLine.TimeStampText() + logLine.LineText());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.Message);
                }

                if (_isQuitWithFlush == true && _logLines.Count == 0) _isExit = true;
            }
        }

        // Passing DateTime for testing purposes.
        private void MidnightCrossingCheck(DateTime dateTimeNow)
        {
            // You have to subtract Dates to get result in Days.
            if ((dateTimeNow.Date - _currentDate.Date).Days != 0)
            {
                _currentDate = dateTimeNow;

                _filepath = @"C:\LogTest\Log" + _currentDate.ToString("yyyyMMdd HHmmss fff") + ".log";

                try
                {
                    using (StreamWriter _writer = new StreamWriter(_filepath, true))
                    {
                        _writer.Write(_header);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception: " + ex.Message);
                }
            }
        }

        public void StopWithoutFlush()
        {
            _isExit = true;
        }

        public void StopWithFlush()
        {
            _isQuitWithFlush = true;
        }

        public void Write(string text)
        {
            // With Write just add log to the queue
            try
            {
                _logLines.TryAdd(new LogLine(text, DateTime.Now));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }

        }
    }
}