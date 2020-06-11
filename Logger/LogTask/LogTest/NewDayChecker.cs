using System;
using System.IO;
using System.IO.Abstractions;

namespace LogTest
{
    public class NewDayChecker
    {
        private DateTime _currentDate;
        private IFileSystem _fileSystem;

        public NewDayChecker(DateTime currentDate, IFileSystem fileSystem)
        {
            _currentDate = currentDate;
            _fileSystem = fileSystem;
        }

        public void Check(DateTime dateTimeNow, ILogFile logFile, ref StreamWriter writer)
        {
            if ((dateTimeNow.Date - _currentDate.Date).Days != 0)
            {
                _currentDate = dateTimeNow;

                writer = _fileSystem.File.CreateText(@"C:\LogTest\Log" + _currentDate.ToString("yyyyMMdd HHmmss fff") + ".log");               

                logFile.CreateNewFile(writer);
            }
        }
    }
}