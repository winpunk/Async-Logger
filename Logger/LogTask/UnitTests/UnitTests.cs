using LogTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void IsAnythingIsWrittenToLogFile()
        {
            var mockFileSystem = new MockFileSystem();

            var loggerAgent = new LoggerAgent(mockFileSystem, new LogFile());

            loggerAgent.IsQuitWithFlush = true;
            loggerAgent.logLines.TryAdd(new LogLine("TEST", DateTime.Now), 10);
            loggerAgent.LoggerLoop();

            var fileNames = mockFileSystem.Directory.GetFiles(@"C:\LogTest\");
            string text = mockFileSystem.File.ReadAllText(fileNames.First());

            Assert.AreEqual(true, text.Contains("TEST"));
        }

        [TestMethod]
        public void IsNewFileCreatedIfNewDay()
        {
            var mockFileSystem = new MockFileSystem();
            var newDayChecker = new NewDayChecker(DateTime.Now, mockFileSystem);
            var logFile = new LogFile();
            StreamWriter blankStreamWriter = null;

            var loggerAgent = new LoggerAgent(mockFileSystem, logFile);

            loggerAgent.IsQuitWithFlush = true;
            loggerAgent.logLines.TryAdd(new LogLine("TEST", DateTime.Now), 10);
            loggerAgent.LoggerLoop();

            var tommorow = DateTime.Now.AddDays(1);
            var fakeDateTime = new DateTime(tommorow.Year, tommorow.Month, tommorow.Day, 0, 0, 1);
            newDayChecker.Check(fakeDateTime, logFile, ref blankStreamWriter);

            bool isNewFileCreated = false;

            string[] filePaths = mockFileSystem.Directory.GetFiles(@"C:\LogTest\");

            foreach (var filePath in filePaths)
            {
                if (filePath.Contains(fakeDateTime.ToString("yyyyMMdd")))
                {
                    isNewFileCreated = true;
                }
            }

            Assert.IsTrue(isNewFileCreated);
        }

        [TestMethod]
        public void IsStopWithFlushWorks()
        {
            MockFileSystem mockFileSystem = new MockFileSystem();

            ILog asyncFileLogger = new AsyncFileLogger(mockFileSystem, new LogFile());

            for (int i = 0; i < 50; i++)
            {
                asyncFileLogger.Write("Number with Flush: " + i.ToString());
            }

            asyncFileLogger.StopWithFlush();

            var fileNames = mockFileSystem.Directory.GetFiles(@"C:\LogTest\");
            string fileText = mockFileSystem.File.ReadAllText(fileNames.First());

            //Assert.AreEqual("Number with Flush: 49", fileText);
            Assert.IsTrue(fileText.Contains("Number with Flush: 49"));

            int numberOfLines = mockFileSystem.File.ReadAllLines(fileNames.First()).Count();

            Assert.AreEqual(51, numberOfLines);
        }

        [TestMethod]
        public void IsStopWithoutFlushWorks()
        {
            var mockFileSystem = new MockFileSystem();

            var asyncFileLogger = new AsyncFileLogger(mockFileSystem, new LogFile());

            for (int i = 0; i < 100; i++)
            {
                asyncFileLogger.Write("Number without Flush: " + i.ToString());
            }

            asyncFileLogger.StopWithoutFlush();

            var fileNames = mockFileSystem.Directory.GetFiles(@"C:\LogTest\");
            string fileText = mockFileSystem.File.ReadAllText(fileNames.First());

            Assert.IsTrue(!fileText.Contains("Number without Flush: 99"));

            int numberOfLines = mockFileSystem.File.ReadAllLines(fileNames.First()).Count();

            Assert.IsTrue(numberOfLines < 100);
            Assert.IsTrue(numberOfLines > 1);
        }
    }
}