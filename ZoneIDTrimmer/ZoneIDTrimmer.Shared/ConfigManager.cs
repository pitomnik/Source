using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace ZoneIDTrimmer.Shared
{
    public sealed class ConfigKeys
    {
        public const string ScanFilesPattern = "ScanFilesPattern";
        public const string ScanFilesLimit = "ScanFilesLimit";
        public const string IgnoreFolders = "IgnoreFolders";
        public const string IgnoreFiles = "IgnoreFiles";
        public const string AutoClose = "AutoClose";
        public const string AutoCloseDelay = "AutoCloseDelay";
        public const string MenuIconSize = "MenuIconSize";
    }

    public class ConfigManager
    {
        public const string DataFolderName = "ZoneIDTrimmer";
        public const string ConfigFileName = "ZoneIDTrimmer.Tool.exe.config";
        public const string InstallDirKey = @"SOFTWARE\Gasanov.net\ZoneIDTrimmer";
        public const string InstallDirName = "InstallDir";

        private static readonly Lazy<ConfigManager> LazyInstance = new Lazy<ConfigManager>(() => new ConfigManager());

        private Configuration _configuration;

        private ConfigManager()
        {
            LoadConfiguration();
        }

        public static ConfigManager Instance
        {
            [DebuggerNonUserCode]
            get { return LazyInstance.Value; }
        }

        public string ConfigFile
        {
            [DebuggerNonUserCode]
            get { return _configuration.FilePath; }
        }

        public T ReadValue<T>(string key, T defaultValue)
        {
            var value = defaultValue;
            var element = _configuration.AppSettings.Settings[key];

            if (element == null)
            {
                _configuration.AppSettings.Settings.Add(key, Convert.ToString(defaultValue));
                _configuration.Save(ConfigurationSaveMode.Modified);
            }
            else
            {
                try
                {
                    value = (T)Convert.ChangeType(element.Value, typeof(T));
                }
                catch (Exception ex)
                {
                    LogMaster.Instance.Warn(String.Format("Failed to convert value '{0}' of key '{1}.", element.Value, key), ex);
                }
            }

            return value;
        }

        public void WriteValue<T>(string key, T value)
        {
            try
            {
                InternalWriteValue(key, value);
            }
            catch (ConfigurationException)
            {
                LoadConfiguration();
                InternalWriteValue(key, value);
            }
        }

        private void LoadConfiguration()
        {
            try
            {
                var appDataPath = Environment.ExpandEnvironmentVariables("%APPDATA%");
                var userConfigFile = Path.Combine(appDataPath, DataFolderName, ConfigFileName);

                if (!File.Exists(userConfigFile))
                {
                    var installDir = GetInstallDir();
                    var appConfigFile = Path.Combine(installDir, ConfigFileName);

                    if (File.Exists(appConfigFile))
                    {
                        if (!Directory.Exists(appDataPath))
                        {
                            Directory.CreateDirectory(appDataPath);
                        }

                        File.Copy(appConfigFile, userConfigFile);
                    }
                }

                var fileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = userConfigFile
                };

                _configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }
            catch
            {
                _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
        }

        private string GetInstallDir()
        {
            string installDir = null;

            using (var key = Registry.LocalMachine.OpenSubKey(InstallDirKey))
            {
                if (key != null)
                {
                    installDir = key.GetValue(InstallDirName) as string;
                }
            }

            return installDir;
        }

        private void InternalWriteValue<T>(string key, T value)
        {
            _configuration.AppSettings.Settings[key].Value = Convert.ToString(value);
            _configuration.Save(ConfigurationSaveMode.Modified);
        }
    }
}
