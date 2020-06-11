using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;

namespace LogTest
{
    public class LogFile : ILogFile
    {
        private static readonly string _header = "Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine;

        public LogFile()
        {
        }

        public bool CreateNewDirectory(IFileSystem fileSystem)
        {
            try
            {
                fileSystem.Directory.CreateDirectory(@"C:\LogTest");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
                return false;
            }

            return true;
        }

        public void CreateNewFile(StreamWriter streamWriter)
        {
            streamWriter.Write(_header);
        }

        public void WriteLog(StreamWriter streamWriter, string log)
        {
            streamWriter.Write(log);
        }
    }
}