using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace DiscoLights.Shared
{
	#region ConfigKey Class

	internal sealed class ConfigKey
	{
		public const string LightSize = "LightSize";
		public const string DeviceNumber = "DeviceNumber";
		public const string AlwaysOnTop = "AlwaysOnTop";
		public const string Orientation = "Orientation";
		public const string Enabled = "Enabled";
		public const string WindowLeft = "WindowLeft";
		public const string WindowTop = "WindowTop";
		public const string NoiseLevel = "NoiseLevel";
	}

	#endregion

	#region AppConfig Class

	internal class AppConfig
	{
		#region Private Members

		private static readonly AppConfig instance = new AppConfig();
		private readonly Configuration configuration;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AppConfig"/> class.
		/// </summary>
		private AppConfig()
		{
			Assembly assembly = Assembly.GetEntryAssembly();

			if (assembly != null) // may occur in unit testing
			{
                string appName = assembly.GetName().Name;
				string configName = Path.ChangeExtension(appName, ".config");
                string appLocation = AppDomain.CurrentDomain.BaseDirectory;
                string dataLocation = Environment.ExpandEnvironmentVariables("%APPDATA%");
                string appDataLocation = Path.Combine(dataLocation, appName);
                string appConfigPath = Path.Combine(appLocation, configName);
                string userConfigPath = Path.Combine(appDataLocation, configName);
                string configPath = userConfigPath;

                if (!File.Exists(userConfigPath))
                {
                    try
                    {
                        File.Copy(appConfigPath, userConfigPath);
                    }
                    catch
                    {
                        configPath = appConfigPath;
                    }
                }

				AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath);
			}

			configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static AppConfig Instance
		{
			[DebuggerNonUserCode]
			get { return instance; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Reads the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public T ReadValue<T>(string key)
		{
			return ReadValue<T>(key, default(T));
		}

		/// <summary>
		/// Reads the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		public T ReadValue<T>(string key, T defaultValue)
		{
			ValidateKey(key);

			T value = defaultValue;
			KeyValueConfigurationElement element = configuration.AppSettings.Settings[key];

			if (element != null)
			{
				try
				{
					if (defaultValue is Enum)
					{
						value = (T)Enum.Parse(typeof(T), element.Value);
					}
					else
					{
						value = (T)Convert.ChangeType(element.Value, typeof(T), CultureInfo.InvariantCulture);
					}
				}
				catch (Exception ex)
				{
					LogMaster.Instance.Error(String.Format("Failed to parse value '{0}' of key '{1}'.", element.Value, key), ex);
				}
			}

			return value;
		}

		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void WriteValue(string key, object value)
		{
			WriteValue(key, value == null ? null : value.ToString());
		}

		/// <summary>
		/// Writes the value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void WriteValue(string key, string value)
		{
			ValidateKey(key);

			KeyValueConfigurationElement element = configuration.AppSettings.Settings[key];

			if (element == null)
			{
				configuration.AppSettings.Settings.Add(key, value);
			}
			else
			{
				element.Value = value;
			}

			try
			{
				configuration.Save(ConfigurationSaveMode.Modified);

				ConfigurationManager.RefreshSection("appSettings");
			}
			catch (Exception ex)
			{
				LogMaster.Instance.Error(String.Format("Failed to save value '{0}' of key '{1}'.", value, key), ex);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Validates the key.
		/// </summary>
		/// <param name="key">The key.</param>
		private void ValidateKey(string key)
		{
			if (String.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Argument can't be null or empty.", "key");
			}
		}

		#endregion
	}

	#endregion
}
