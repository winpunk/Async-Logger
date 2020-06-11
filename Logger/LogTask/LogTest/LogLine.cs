namespace LogTest
{
    using System;

    public class LogLine
    {
        public LogLine(String text, DateTime timeStamp)
        {
            Text = text;
            TimeStamp = timeStamp;
        }

        public string Text { get; set; }

        public virtual DateTime TimeStamp { get; set; }

        public virtual string LineText()
        {
            return ("\t" + Text + ". " + Environment.NewLine);
        }

        public virtual string TimeStampText()
        {
            return (TimeStamp.ToString("yyyy-MM-dd HH:mm:ss:fff").PadRight(25, ' '));
        }
    }
}