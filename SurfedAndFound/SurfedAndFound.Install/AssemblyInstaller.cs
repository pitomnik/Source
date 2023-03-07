using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.ComponentModel;

namespace SurfedAndFound.Install
{
	[RunInstaller(true)]
	public class AssemblyInstaller : Installer
	{
		#region Constants

		private const string assemblyNamesList = "SurfedAndFound.Msie";
		private const string assemblyNameFormat = "{0}, Version={1}, Culture=neutral, PublicKeyToken=9fafe63f40dd416e";

		#endregion

		#region Private Members

		private readonly string installerPath;

		#endregion

		#region Constructors

		public AssemblyInstaller()
		{
			installerPath = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe");
		}

		#endregion

		#region Public Methods

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
		}

		public override void Commit(IDictionary savedState)
		{
			base.Commit(savedState);

			InstallAssemblies();
		}

		public override void Rollback(IDictionary savedState)
		{
			base.Rollback(savedState);
		}

		#endregion

		#region Private Methods

		private void InstallAssemblies()
		{
			string[] assemblyNames = assemblyNamesList.Split(',');

			foreach (string assemblyName in assemblyNames)
			{
				string fullName = String.Format(assemblyNameFormat, assemblyName, GetType().Assembly.GetName().Version);
				
				InstallAssembly(fullName);
			}
		}

		private bool InstallAssembly(string assemblyName)
		{
			Process process = new Process();

			process.StartInfo.FileName = installerPath;
			process.StartInfo.Arguments = "install \"" + assemblyName + "\"";
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

			process.Start();

			bool success = process.WaitForExit(10000);

			if (!process.HasExited)
			{
				process.Kill();
			}

			if (success)
			{
				success = process.ExitCode == 0;
			}

			process.Close();
			process.Dispose();

			return success;
		}

		#endregion
	}
}
