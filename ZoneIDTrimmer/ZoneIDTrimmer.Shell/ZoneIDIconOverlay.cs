using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using SharpShell.Interop;
using SharpShell.SharpIconOverlayHandler;
using ZoneIDTrimmer.Shared;

namespace ZoneIDTrimmer.Shell
{
    [ComVisible(true)]
    public class ZoneIDIconOverlay : SharpIconOverlayHandler
    {
        private const string DefaultPattern = "*.*";

        private readonly Lazy<PathTools> _pathTools;
        private string _scanFilesPattern;
        private int? _scanFilesLimit;

        public ZoneIDIconOverlay()
        {
            _pathTools = new Lazy<PathTools>(() =>
                {
                    var ignoreFolders = ConfigManager.Instance.ReadValue(ConfigKeys.IgnoreFolders, String.Empty);
                    var ignoreFiles = ConfigManager.Instance.ReadValue(ConfigKeys.IgnoreFiles, String.Empty);

                    return new PathTools(ignoreFolders, ignoreFiles);
                });
        }

        private string ScanFilesPattern
        {
            get
            {
                if (_scanFilesPattern == null)
                {
                    _scanFilesPattern = GetScanFilesPattern();
                }
                return _scanFilesPattern;
            }
        }

        private int ScanFilesLimit
        {
            get
            {
                if (!_scanFilesLimit.HasValue)
                {
                    _scanFilesLimit = GetScanFilesLimit();
                }
                return _scanFilesLimit.Value;
            }
        }
        
        protected override bool CanShowOverlay(string path, FILE_ATTRIBUTE attributes)
        {
            try
            {
                return IsMemberOf(path);
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error("IsMemberOf failed.", ex);
            }

            return false;
        }

        protected override Icon GetOverlayIcon()
        {
            return Shared.Properties.Resources.ZoneID;
        }

        protected override int GetPriority()
        {
            return 0;
        }

        public bool IsMemberOf(string path)
        {
            var normalizedPath = PathTools.Normalize(path);

            if (File.Exists(normalizedPath))
            {
                return IsFileMemberOf(normalizedPath);
            }
            
            if (Directory.Exists(normalizedPath))
            {
                return IsFolderMemberOf(normalizedPath);
            }

            LogMaster.Instance.WarnFormat("Path not found: '{0}'.", path);

            return false;
        }

        private string GetScanFilesPattern()
        {
            var pattern = ConfigManager.Instance.ReadValue(ConfigKeys.ScanFilesPattern, DefaultPattern);

            if (String.IsNullOrEmpty(pattern)) pattern = DefaultPattern;

            return pattern;
        }

        private int GetScanFilesLimit()
        {
            var limit = ConfigManager.Instance.ReadValue(ConfigKeys.ScanFilesLimit, -1);

            if (limit == -1) limit = int.MaxValue;

            return limit;
        }

        private bool IsFileMemberOf(string path)
        {
            LogMaster.Instance.DebugFormat("Scanning file '{0}'.", path);

            if (_pathTools.Value.IsPathIgnored(path))
            {
                LogMaster.Instance.DebugFormat("File '{0}' in ignore list.", path);
                return false;
            }

            var isMemberOf = false;

            try
            {
                isMemberOf = ZoneIDHelper.ZoneIDExists(path);
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error(String.Format("Failed to scan file '{0}'.", path), ex);
            }

            return isMemberOf;
        }

        private bool IsFolderMemberOf(string path)
        {
            if (ScanFilesLimit == 0)
            {
                return false;
            }

            LogMaster.Instance.DebugFormat("Scanning folder '{0}'.", path);

            if (_pathTools.Value.IsFolderIgnored(path))
            {
                LogMaster.Instance.DebugFormat("Folder '{0}' in ignore list.", path);
                return false;
            }

            var isMemberOf = false;

            try
            {
                var files = Directory.GetFiles(path, ScanFilesPattern, SearchOption.TopDirectoryOnly);

                for (int i = 0; i < files.Length && i < ScanFilesLimit; i++)
                {
                    if (IsFileMemberOf(files[i]))
                    {
                        isMemberOf = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMaster.Instance.Error(String.Format("Failed to scan folder '{0}'.", path), ex);
            }

            return isMemberOf;
        }
    }
}
