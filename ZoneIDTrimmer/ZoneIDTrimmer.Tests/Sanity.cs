using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpShell;
using ZoneIDTrimmer.Shared;
using ZoneIDTrimmer.Shell;

namespace ZoneIDTrimmer.Tests
{
    [TestClass]
    public class Sanity : Trimmer
    {
        private readonly List<Action> _cleanupActions = new List<Action>();

        [TestMethod]
        public void ShellContextMenuServerType()
        {
            var contextMenu = new ZoneIDContextMenu();

            Assert.AreEqual(ServerType.ShellContextMenu, contextMenu.ServerType);
        }

        [TestMethod]
        public void ShellIconOverlayServerType()
        {
            var iconOverlay = new ZoneIDIconOverlay();

            Assert.AreEqual(ServerType.ShellIconOverlayHandler, iconOverlay.ServerType);
        }

        [TestMethod]
        public void ZoneIDIconOverlayFileIsMemberOf()
        {
            IEnumerable<string> files;

            CreateDirectory(1, out files);
            
            var target = new ZoneIDIconOverlay();

            Assert.IsTrue(target.IsMemberOf(files.Single()));
        }

        [TestMethod]
        public void ZoneIDIconOverlayFolderIsMemberOf()
        {
            var path = CreateDirectory(1);
            var target = new ZoneIDIconOverlay();

            Assert.IsTrue(target.IsMemberOf(path));
        }

        [TestMethod]
        public void TrimSingleFile()
        {
            IEnumerable<string> files;
            var path = CreateDirectory(1, out files);

            AddPathCleanup(path);

            ExecuteTrimmer(files);

            AssertResult(path);
        }

        [TestMethod]
        public void TrimMultipleFiles()
        {
            IEnumerable<string> files;
            var path = CreateDirectory(100, out files);

            AddPathCleanup(path);

            ExecuteTrimmer(files);

            AssertResult(path);
        }

        [TestMethod]
        public void TrimSingleDirectory()
        {
            var path = CreateDirectory(100);

            AddPathCleanup(path);

            ExecuteTrimmer(path);

            AssertResult(path);
        }

        [TestMethod]
        public void TrimMultipleDirectories()
        {
            var paths = new List<string>();

            for (var i = 0; i < 100; i++)
            {
                var path = CreateDirectory(1);

                AddPathCleanup(path);

                paths.Add(path);
            }

            ExecuteTrimmer(paths);

            AssertResult(paths);
        }

        [TestMethod]
        public void ConfigurationMergeString()
        {
            var key = CreateUniqueString();
            var value = CreateUniqueString();
            var readValue = ConfigManager.Instance.ReadValue(key, value);

            Assert.AreEqual(value, readValue);
        }

        [TestMethod]
        public void ConfigurationMergeNumber()
        {
            var key = CreateUniqueString();
            var value = CreateUniqueNumber();
            var readValue = ConfigManager.Instance.ReadValue(key, value);

            Assert.AreEqual(value, readValue);
        }

        [TestMethod]
        public void IgnoreFileCaseInsensitive()
        {
            var file1 = CreateUniqueString();
            var file2 = CreateUniqueString();
            var path1 = Path.Combine(@"C:\Temp", file1);
            var path2 = Path.Combine(@"C:\Temp", file1);
            var ignoreFiles = file1 + Path.PathSeparator + file2;
            var pathTools = new PathTools(String.Empty, ignoreFiles);

            Assert.AreEqual(0, pathTools.IgnoreFoldersList.Count());
            Assert.AreEqual(2, pathTools.IgnoreFilesList.Count());

            Assert.IsTrue(pathTools.IgnoreFilesList.Contains(file1));
            Assert.IsTrue(pathTools.IgnoreFilesList.Contains(file2));

            Assert.IsTrue(pathTools.IsFileIgnored(path1.ToLower()));
            Assert.IsTrue(pathTools.IsFileIgnored(path1.ToUpper()));
            Assert.IsTrue(pathTools.IsFileIgnored(path2.ToLower()));
            Assert.IsTrue(pathTools.IsFileIgnored(path2.ToUpper()));
        }

        [TestMethod]
        public void IgnoreFolderCaseInsensitive()
        {
            var folder1 = CreateUniqueString();
            var folder2 = CreateUniqueString();
            var path1 = Path.Combine(@"C:\Temp", folder1);
            var path2 = Path.Combine(@"C:\Temp", folder2);
            var ignoreFolders = folder1 + Path.PathSeparator + folder2;
            var pathTools = new PathTools(ignoreFolders, String.Empty);

            Assert.AreEqual(2, pathTools.IgnoreFoldersList.Count());
            Assert.AreEqual(0, pathTools.IgnoreFilesList.Count());

            Assert.IsTrue(pathTools.IgnoreFoldersList.Contains(folder1));
            Assert.IsTrue(pathTools.IgnoreFoldersList.Contains(folder2));

            Assert.IsTrue(pathTools.IsFolderIgnored(path1.ToLower()));
            Assert.IsTrue(pathTools.IsFolderIgnored(path1.ToUpper()));
            Assert.IsTrue(pathTools.IsFolderIgnored(path2.ToLower()));
            Assert.IsTrue(pathTools.IsFolderIgnored(path2.ToUpper()));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (var cleanupAction in _cleanupActions)
            {
                try
                {
                    cleanupAction();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
            }
        }

        private void AddPathCleanup(string path)
        {
            if (File.Exists(path))
            {
                _cleanupActions.Add(() => File.Delete(path));
            }
            else if (Directory.Exists(path))
            {
                _cleanupActions.Add(() => Directory.Delete(path, true));
            }
        }
    }
}
