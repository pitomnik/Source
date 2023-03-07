using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trinet.Core.IO.Ntfs;
using ZoneIDTrimmer.Shared;

namespace ZoneIDTrimmer.Tests
{
    public class Trimmer
    {
        private const string StreamName = "Zone.Identifier";
        private const string Filename = @"ZoneIDTrimmer.Tool.exe";
        private const string OutputPath = @"..\..\..\ZoneIDTrimmer.Tool\bin\";

        static Trimmer()
        {
            var testPath = AppDomain.CurrentDomain.BaseDirectory;
            var tokens = testPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            
            BaseDirectory = Path.Combine(OutputPath, tokens.Last());

            ExecutablePath = Path.Combine(BaseDirectory, Filename);
        }

        protected static string BaseDirectory
        {
            get;
            private set;
        }

        protected static string ExecutablePath
        {
            get;
            private set;
        }

        protected Assembly ExecutableAssembly
        {
            get
            {
                var fullPath = Path.GetFullPath(ExecutablePath);

                return Assembly.LoadFile(fullPath);
            }
        }

        protected string CreateDirectory(int fileCount)
        {
            IEnumerable<string> files;

            return CreateDirectory(fileCount, out files);
        }

        protected string CreateDirectory(int fileCount, out IEnumerable<string> fileList)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CreateRandomFolderName());

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            fileList = CreateFiles(path, fileCount);

            return path;
        }

        protected bool IsProcesRunning()
        {
            var processName = Path.GetFileNameWithoutExtension(Filename);

            return Process.GetProcessesByName(processName).Length > 0;
        }

        protected void ExecuteTrimmer(string path)
        {
            Assert.IsFalse(IsProcesRunning());

            var autoClose = ConfigManager.Instance.ReadValue(ConfigKeys.AutoClose, false);
            var autoCloseDelay = ConfigManager.Instance.ReadValue(ConfigKeys.AutoCloseDelay, 0);

            ConfigManager.Instance.WriteValue(ConfigKeys.AutoClose, true);
            ConfigManager.Instance.WriteValue(ConfigKeys.AutoCloseDelay, 3);

            try
            {
                using (var process = Process.Start(ExecutablePath, path))
                {
                    Assert.IsNotNull(process);

                    process.WaitForExit();

                    Assert.AreEqual(0, process.ExitCode);
                }
            }
            finally
            {
                ConfigManager.Instance.WriteValue(ConfigKeys.AutoClose, autoClose);
                ConfigManager.Instance.WriteValue(ConfigKeys.AutoCloseDelay, autoCloseDelay);
            }
        }

        protected void ExecuteTrimmer(IEnumerable<string> paths)
        {
            var arguments = "\"" + String.Join("\" \"", paths) + "\"";

            ExecuteTrimmer(arguments);
        }

        protected void AssertResult(string path)
        {
            if (File.Exists(path))
            {
                Assert.IsFalse(AssertFile(path), path);
            }
            else if (Directory.Exists(path))
            {
                Assert.AreEqual(0, AssertDirectory(path), path);
            }
            else
            {
                Assert.Fail("Path '{0}' not found.", path);
            }
        }

        protected void AssertResult(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                AssertResult(path);
            }
        }

        protected string CreateUniqueString()
        {
            return Guid.NewGuid().ToString("N");
        }

        protected long CreateUniqueNumber()
        {
            var guid = Guid.NewGuid();

            return BitConverter.ToInt64(guid.ToByteArray(), 0);
        }

        private string CreateRandomFileName()
        {
            return Path.GetRandomFileName();
        }

        private string CreateRandomFolderName()
        {
            var filename = CreateRandomFileName();

            return Path.GetFileNameWithoutExtension(filename);
        }

        private FileInfo CreateRandomFile(string path)
        {
            var fullPath = Path.Combine(path, CreateRandomFileName());

            File.WriteAllText(fullPath, Guid.NewGuid().ToString());

            return new FileInfo(fullPath);
        }

        private IEnumerable<string> CreateFiles(string path, int fileCount)
        {
            var fileList = new List<string>();
            var random = new Random(DateTime.Now.Millisecond);

            for (var i = 0; i < fileCount; i++)
            {
                var file = CreateRandomFile(path);
                var dataStream = file.GetAlternateDataStream(StreamName);

                using (var fileStream = dataStream.OpenWrite())
                {
                    var zoneId = Convert.ToString(random.Next(0, 1000));
                    var byteArray = Encoding.ASCII.GetBytes(zoneId);

                    fileStream.Write(byteArray, 0, byteArray.Length);

                    fileStream.Flush();
                }

                fileList.Add(file.FullName);
            }

            return fileList;
        }

        private bool AssertFile(string path)
        {
            var fileInfo = new FileInfo(path);
            var dataStream = fileInfo.GetAlternateDataStream(StreamName);

            return dataStream.Exists;
        }

        private int AssertDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            var count = 0;

            foreach (var file in files)
            {
                if (AssertFile(file))
                {
                    count += 1;
                }
            }

            return count;
        }
    }
}
