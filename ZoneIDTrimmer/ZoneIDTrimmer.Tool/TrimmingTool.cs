using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZoneIDTrimmer.Report;
using ZoneIDTrimmer.Shared;

namespace ZoneIDTrimmer.Tool
{
    internal class TrimmingTool : ApplicationContext
    {
        private const string HomepageUrl = "http://www.gasanov.net";
        private const string DonationUrl = HomepageUrl + "/Donation.asp";

        private static readonly object Locker = new object();

        private readonly IToolView _toolView;
        private readonly IEnumerable<string> _paths;
        private readonly ReportBuilder _reportBuilder;
        private readonly List<IPathObserver> _pathObservers;
        private readonly PathTools _pathTools;
        private int _taskCount;

        public TrimmingTool(IToolView toolView, IEnumerable<string> paths)
        {
            _toolView = toolView;

            _toolView.OnCreate += ToolViewOnCreate;
            _toolView.OnDonate += ToolViewOnDonate;
            _toolView.OnClose += ToolViewOnClose;
            _toolView.OnHomepageClick += ToolViewOnHomepageClick;
            _toolView.OnDetailsClick += ToolViewOnDetailsClick;

            _paths = paths;

            _reportBuilder = new ReportBuilder(Application.ProductName);

            _pathObservers = new List<IPathObserver> { _reportBuilder };

            if (toolView is IPathObserver)
            {
                _pathObservers.Add((IPathObserver)toolView);
            }

            new WindowsFormsSynchronizationContext().Post(ShowView, null);

            var ignoreFolders = ConfigManager.Instance.ReadValue(ConfigKeys.IgnoreFolders, String.Empty);
            var ignoreFiles = ConfigManager.Instance.ReadValue(ConfigKeys.IgnoreFiles, String.Empty);

            _pathTools = new PathTools(ignoreFolders, ignoreFiles);
        }

        private void ShowView(object state)
        {
            var title = String.Concat(Application.ProductName, " ", Application.ProductVersion);
            var autoClose = ConfigManager.Instance.ReadValue(ConfigKeys.AutoClose, false);
            var autoCloseDelay = ConfigManager.Instance.ReadValue(ConfigKeys.AutoCloseDelay, 0);

            try
            {
                _toolView.Show(title, autoClose, TimeSpan.FromSeconds(autoCloseDelay));
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error("Failed to show dialog.", ex);

                Application.Exit();
            }

            ConfigManager.Instance.WriteValue(ConfigKeys.AutoClose, _toolView.AutoClose);
            ConfigManager.Instance.WriteValue(ConfigKeys.AutoCloseDelay, _toolView.AutoCloseDelay.TotalSeconds);
        }

        private void StartTask(string path)
        {
            Task.Factory.StartNew(TrimPath, path);

            Interlocked.Increment(ref _taskCount);
        }

        private void TrimPath(object state)
        {
            var path = (string)state;

            _toolView.NotifyTrimStart();

            if (File.Exists(path))
            {
                TrimFile(path);
            }
            else if (Directory.Exists(path))
            {
                TrimFolder(path);
            }
            else
            {
                LogMaster.Instance.WarnFormat("Path not found: '{0}'.", path);
            }

            if (Interlocked.Decrement(ref _taskCount) == 0)
            {
                _toolView.NotifyTrimFinish();

                ZoneIDHelper.NotifyShell();
            }
        }

        private void TrimFolder(string path)
        {
            LogMaster.Instance.DebugFormat("Trimming folder '{0}'.", path);

            if (_pathTools.IsFolderIgnored(path))
            {
                NotifyPathObservers(TrimEvent.Ignored, PathType.Folder, path);

                LogMaster.Instance.DebugFormat("Folder '{0}' in ignore list.", path);

                return;
            }

            var dirs = GetFolders(path);

            if (dirs == null) return;

            foreach (var dir in dirs)
            {
                try
                {
                    TrimFolder(dir);
                }
                catch (Exception ex)
                {
                    NotifyPathObservers(TrimEvent.Failed, PathType.Folder, path);

                    LogMaster.Instance.Error(String.Format("Failed to trim folder '{0}'.", dir), ex);
                }
            }

            var files = GetFiles(path);

            if (files == null) return;

            var trimEvents = new List<TrimEvent>();

            foreach (var file in files)
            {
                var eventType = TrimFile(file);

                trimEvents.Add(eventType);
            }

            if (trimEvents.Contains(TrimEvent.Trimmed))
            {
                NotifyPathObservers(TrimEvent.Trimmed, PathType.Folder, path);
            }
            else
            {
                NotifyPathObservers(TrimEvent.Scanned, PathType.Folder, path);
            }
        }

        private TrimEvent TrimFile(string path)
        {
            LogMaster.Instance.DebugFormat("Trimming file '{0}'.", path);

            if (_pathTools.IsFileIgnored(path))
            {
                NotifyPathObservers(TrimEvent.Ignored, PathType.File, path);

                LogMaster.Instance.DebugFormat("File '{0}' in ignore list.", path);

                return TrimEvent.Ignored;
            }

            TrimEvent trimEvent;

            try
            {
                if (ZoneIDHelper.ZoneIDExists(path))
                {
                    ZoneIDHelper.DeleteZoneID(path);

                    trimEvent = TrimEvent.Trimmed;

                    LogMaster.Instance.Debug("Zone ID trimmed successfully.");
                }
                else
                {
                    trimEvent = TrimEvent.Scanned;

                    LogMaster.Instance.DebugFormat("Zone ID not found.");
                }
            }
            catch (Exception ex)
            {
                trimEvent = TrimEvent.Failed;

                LogMaster.Instance.Error(String.Format("Failed to trim file '{0}'.", path), ex);
            }

            NotifyPathObservers(trimEvent, PathType.File, path);

            return trimEvent;
        }

        private IEnumerable<string> GetFolders(string path)
        {
            try
            {
                return Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                NotifyPathObservers(TrimEvent.Failed, PathType.Folder, path);

                LogMaster.Instance.Error(String.Format("Failed to get directories in '{0}'.", path), ex);
            }

            return null;
        }

        private IEnumerable<string> GetFiles(string path)
        {
            try
            {
                return Directory.GetFiles(path);
            }
            catch (Exception ex)
            {
                NotifyPathObservers(TrimEvent.Failed, PathType.Folder, path);

                LogMaster.Instance.Error(String.Format("Failed to get files in '{0}'.", path), ex);
            }

            return null;
        }

        private void ToolViewOnCreate(object sender, EventArgs e)
        {
            foreach (var path in _paths)
            {
                var normalaziedPath = PathTools.Normalize(path);

                LogMaster.Instance.DebugFormat("Normalized path: '{0}'.", normalaziedPath);

                StartTask(normalaziedPath);
            }
        }

        private void ToolViewOnDonate(object sender, EventArgs e)
        {
            OpenWebPage(DonationUrl);
        }

        private void ToolViewOnClose(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ToolViewOnHomepageClick(object sender, EventArgs e)
        {
            OpenWebPage(HomepageUrl);
        }

        private void ToolViewOnDetailsClick(object sender, EventArgs e)
        {
            ShowDetails();
        }

        private void NotifyPathObservers(TrimEvent trimEvent, PathType pathType, string path)
        {
            var lockTaken = false;

            if (_taskCount > 1)
            {
                Monitor.Enter(Locker, ref lockTaken);
            }

            try
            {
                _pathObservers.ForEach(x => x.Notify(trimEvent, pathType, path));
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error("Failed to notify observers.", ex);
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(Locker);
                }
            }
        }

        private void OpenWebPage(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error(String.Format("Failed to open Web page '{0}'.", url), ex);
            }
        }

        private void ShowDetails()
        {
            try
            {
                var time = DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                var name = String.Concat(Application.ProductName, time, ".htm");
                var path = Path.Combine(Path.GetTempPath(), name);
                string text;

                lock (Locker)
                {
                    text = _reportBuilder.ToString();
                }

                File.WriteAllText(path, text, Encoding.UTF8);

                Process.Start(path);
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error("Failed to show details.", ex);
            }
        }
    }
}
