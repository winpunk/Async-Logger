namespace LogTest
{
    using System;

    // This object is for storing and formating log text.
    public class LogLine
    {
        public LogLine(String text, DateTime timeStamp)
        {
            Text = text;
            TimeStamp = timeStamp;
        }


        // The text to be display in logline.
        public string Text { get; set; }

        // The Timestamp is initialized when the log is added. Can be overridden.
        public virtual DateTime TimeStamp { get; set; }


        // Returns a formatted line. Can be overridden.
        public virtual string LineText()
        {
            // No need for StringBuilder, concatenation is as fast as StringBuilder in this case.
            return ("\t" + Text + ". " + Environment.NewLine);
        }

        // Returns a formatted line. Can be overridden.
        public virtual string TimeStampText()
        {
            return (TimeStamp.ToString("yyyy-MM-dd HH:mm:ss:fff").PadRight(25, ' '));
        }
    }
}