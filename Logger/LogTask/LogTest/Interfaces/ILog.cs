namespace LogTest
{
    public interface ILog
    {
        void StopWithoutFlush();

        void StopWithFlush();

        void Write(string text);
    }
}