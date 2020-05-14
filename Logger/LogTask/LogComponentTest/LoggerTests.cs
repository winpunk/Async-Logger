using LogTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace LogComponentTest
{
    [TestClass]
    public class LogComponentTests
    {
        [TestMethod]

        // For testing if a call to Ilog will end up in writing something.
        public void TestWrite()
        {
            var sut = new AsyncLog();

            // Writing with flush.
            sut.Write("TEST");
            sut.StopWithFlush();

            // Checking if last modified file contains "TEST" text.
            var filenames = Directory.GetFiles(@"C:\LogTest\").OrderByDescending(f => new FileInfo(f).LastWriteTime);
            string text = System.IO.File.ReadAllText(filenames.First());

            Assert.AreEqual(true, text.Contains("TEST"));
        }

        [TestMethod]

        // For testing if new files are created if midnight is crossed
        public void TestMidnightFileCreation()
        {
            bool isNewFileCreated = false;
            var sut = new AsyncLog();

            // Writing with flush.
            sut.Write("TESTMidnight");
            sut.StopWithFlush();

            // Creating a fake DateTime for testing.
            var tommorow = DateTime.Now.AddDays(1);
            var fakeDateTime = new DateTime(tommorow.Year, tommorow.Month, tommorow.Day, 0, 0, 1);

            // Using PrivateObject to access private method - "MidnightCrossingCheck".
            PrivateObject obj = new PrivateObject(sut);
            obj.Invoke("MidnightCrossingCheck", fakeDateTime);

            // Check if new file with fake date is created.
            string[] filePaths = Directory.GetFiles(@"C:\LogTest\").OrderBy(f => new FileInfo(f).LastWriteTime).ToArray();

            foreach (var filePath in filePaths)
            {
                if (filePath.Contains(fakeDateTime.ToString("yyyyMMdd")))
                {
                    isNewFileCreated = true;
                }
            }

            // Check if new file is created.
            Assert.AreEqual(true, isNewFileCreated);
        }

        [TestMethod]

        // For testing if the stop behavior is working as it should.
        public void TestStopBehavior()
        {
            var sut = new AsyncLog();

            // Write text as in Application program.
            for (int i = 0; i < 15; i++)
            {
                sut.Write("Number with Flush: " + i.ToString());
            }

            // Stop with flush and wait it to write everything.
            sut.StopWithFlush();
            Thread.Sleep(20);

            // Read last modified file, count the lines and check if line count is good.
            var filenames = Directory.GetFiles(@"C:\LogTest\").OrderByDescending(f => new FileInfo(f).LastWriteTime);
            int numberOfLines = System.IO.File.ReadAllLines(filenames.First()).Count();

            Assert.AreEqual(16, numberOfLines);


            // Repeat the same for write without flush.
            var sut2 = new AsyncLog();

            for (int i = 50; i > 0; i--)
            {
                sut2.Write("Number with No flush: " + i.ToString());
            }

            sut2.StopWithoutFlush();
            Thread.Sleep(50);

            filenames = Directory.GetFiles(@"C:\LogTest\").OrderByDescending(f => new FileInfo(f).LastWriteTime);
            numberOfLines = System.IO.File.ReadAllLines(filenames.First()).Count();

            Assert.AreEqual(true, (numberOfLines < 52));
        }
    }
}