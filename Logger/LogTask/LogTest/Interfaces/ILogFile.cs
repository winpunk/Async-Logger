using System.IO;
using System.IO.Abstractions;

namespace LogTest
{
    public interface ILogFile
    {
        bool CreateNewFile(StreamWriter streamWriter);

        void WriteLog(StreamWriter streamWriter, string log);

        bool CreateNewDirectory(IFileSystem fileSystem);
    }
}