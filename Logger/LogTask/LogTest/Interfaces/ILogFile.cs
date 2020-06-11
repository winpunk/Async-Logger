using System.IO;
using System.IO.Abstractions;

namespace LogTest
{
    public interface ILogFile
    {
        void CreateNewFile(StreamWriter streamWriter);

        void WriteLog(StreamWriter streamWriter, string log);

        bool CreateNewDirectory(IFileSystem fileSystem);
    }
}