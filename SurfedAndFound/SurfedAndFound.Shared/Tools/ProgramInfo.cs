using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;

using Microsoft.Win32;

namespace SurfedAndFound.Shared.Tools
{
	public class ProgramInfo
	{
		#region Private Members

		private static readonly ProgramInfo instance = new ProgramInfo();

		private readonly string name, company, copyright, version;
		private string installDir, configFile;
		private bool setup;

		#endregion

		#region Constructors

		private ProgramInfo()
		{
			Assembly assembly = GetType().Assembly;
			
			name = (assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0] as AssemblyProductAttribute).Product;
			company = (assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true)[0] as AssemblyCompanyAttribute).Company;
			copyright = (assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0] as AssemblyCopyrightAttribute).Copyright;
			version = assembly.GetName().Version.ToString();

			setup = false;
		}

		#endregion

		#region Public Members

		public static ProgramInfo Instance
		{
			[DebuggerNonUserCode]
			get { return instance; }
		}

		public string Name
		{
			[DebuggerNonUserCode]
			get { return name; }
		}

		public string Company
		{
			[DebuggerNonUserCode]
			get { return company; }
		}

		public string Copyright
		{
			[DebuggerNonUserCode]
			get { return copyright; }
		}

		public string Version
		{
			[DebuggerNonUserCode]
			get { return version.ToString(); }
		}

		public string InstallDir
		{
			[DebuggerNonUserCode]
			get
			{
				if (setup)
				{
					return installDir;
				}
				else
				{
					throw new InvalidOperationException("Path wasn't located.");
				}
			}
		}

		public string ConfigFile
		{
			[DebuggerNonUserCode]
			get
			{
				if (setup)
				{
					return configFile;
				}
				else
				{
					throw new InvalidOperationException("Path wasn't located.");
				}
			}
		}

		#endregion

		public void Locate()
		{
			string folder = String.Format("{0}\\{1}", company, name);
			string defaultDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), folder);
			RegistryKey key = Registry.LocalMachine.OpenSubKey(Path.Combine("SOFTWARE", folder));

			if (key == null)
			{
				installDir = defaultDir;
			}
			else
			{
				installDir = (string)key.GetValue("InstallDir", defaultDir);
			}

			if (Directory.Exists(installDir))
			{
				configFile = Path.Combine(installDir, String.Format("{0}.config", name));
			}
			else
			{
				throw new DirectoryNotFoundException(String.Format("Path not found: '{0}'.", installDir));
			}

			if (!File.Exists(configFile))
			{
				throw new FileNotFoundException(String.Format("Path not found: '{0}'.", configFile));
			}
			
			AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configFile);

			setup = true;
		}
	}
}
