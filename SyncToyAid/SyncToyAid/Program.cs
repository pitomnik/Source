using SyncToyAid.View;
using SyncToyAid.Logic;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SyncToyAid
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		/// <summary>
		/// Handles the UnhandledException event of the CurrentDomain control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;

			if (ex != null)
			{
				//LogMaster.Instance.Fatal("Fatal exception occured.", ex);
            }

			MessageBox.Show("A fatal exception occured.");
		}

		/// <summary>
		/// Handles the AssemblyResolve event of the CurrentDomain control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
		/// <returns></returns>
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (!args.Name.Contains(SyncToyInfo.LibraryName))
			{
				if (File.Exists(SyncToyInfo.Instance.LibraryPath))
				{
					return Assembly.LoadFrom(SyncToyInfo.Instance.LibraryPath);
				}
				else
				{
					MessageBox.Show("SyncToy not installed.");
					Environment.Exit(1);
				}
			}

			return null;
		}
	}
}
