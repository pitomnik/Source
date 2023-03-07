using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace SyncToyAid.Logic
{
	internal class SyncToyInfo
	{
		#region Constants

		public const string ProgramName = "SyncToy.exe";
		public const string LibraryName = "SyncToyEngine.dll";
		public const string ConfigName = "SyncToyDirPairs.bin";

		private static readonly RegistryKey installDirRegistryHive = Registry.CurrentUser;
		private const string installDirRegistryKey = @"Software\Microsoft\SyncToy";
		private const string installDirRegistryValue = "InstallLocation";
		private const string configFile = @"Microsoft\SyncToy\2.0\" + ConfigName;
		private static readonly string configHomeXpAndEarlier = @"%USERPROFILE%\Local Settings\Application Data";
		private static readonly string configHomeVistaAndAbove = "%LOCALAPPDATA%";

		#endregion

		#region Private Members

		private readonly string programPath;
		private readonly string libraryPath;
		private readonly string configPath;
		
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly SyncToyInfo Instance = new SyncToyInfo();

		#endregion

		#region Constructor(s)

		/// <summary>
		/// Initializes a new instance of the <see cref="SyncToyInfo"/> class.
		/// </summary>
		private SyncToyInfo()
		{
			string installDir = String.Empty;
			RegistryKey key = installDirRegistryHive.OpenSubKey(installDirRegistryKey);

			if (key != null)
			{
				object value = key.GetValue(installDirRegistryValue);

				if (value != null)
				{
					installDir = (string)value;
				}

				key.Close();
			}

			programPath = Path.Combine(installDir, ProgramName);
			libraryPath = Path.Combine(installDir, LibraryName);

			string configDir = Environment.OSVersion.Version.Major > 5 ? configHomeVistaAndAbove : configHomeXpAndEarlier;

			configPath = Path.Combine(Environment.ExpandEnvironmentVariables(configDir), configFile);
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the config path.
		/// </summary>
		/// <value>The config path.</value>
		public string ConfigPath
		{
			[DebuggerNonUserCode]
			get { return configPath; }
		}

		/// <summary>
		/// Gets the program path.
		/// </summary>
		/// <value>The program path.</value>
		public string ProgramPath
		{
			[DebuggerNonUserCode]
			get { return programPath; } 
		}

		/// <summary>
		/// Gets the library path.
		/// </summary>
		/// <value>The library path.</value>
		public string LibraryPath
		{
			[DebuggerNonUserCode]
			get { return libraryPath; }
		}

		#endregion
	}
}
