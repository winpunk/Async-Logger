namespace LogTest
{
    public interface ILog
    {
        // Stop the logging. If any outstanding logs theses will not be written to Log
        void StopWithoutFlush();

        ///Stop the logging. The call will not return until all logs have been written to Log.
        void StopWithFlush();

        // Write a message to the Log.
        void Write(string text);
    }
}