using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WOW64Menu
{
	static class Program
	{
		#region Constants

		private const char argsSeparator = '|';

		#endregion

		#region Entry Point

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			string title = MainForm.CreateTitle();
			IntPtr handle = Win32.FindWindow(null, title);

			if (handle == IntPtr.Zero)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				MainForm form = new MainForm();

				form.Activated += OnFormActivated;

				Application.Run(form);
			}
			else if (args.Length > 0)
			{
				Win32.SendStringMessage(handle, args[0]);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Logs the command line.
		/// </summary>
		public static void LogCommandLine()
		{
			LogMaster.Instance.Info(Environment.CommandLine);
		}

		/// <summary>
		/// Logs the file list.
		/// </summary>
		/// <param name="files">The files.</param>
		public static void LogFileList(string[] files)
		{
			for (int i = 0; i < files.Length; i++)
			{
				LogMaster.Instance.InfoFormat("{0}.\t'{1}'.", i + 1, files[i]);
			}
		}

		/// <summary>
		/// Strings to args.
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns></returns>
		public static string[] StringToArgs(string s)
		{
			if (s == null) return null;

			return s.Split(argsSeparator);
		}

		#endregion

		#region Private Events

		/// <summary>
		/// Called on unhandled exception.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			LogCommandLine();

			LogMaster.Instance.Fatal("An unexpected error occured.", e.ExceptionObject as Exception);
		}

		/// <summary>
		/// Called on form activated.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void OnFormActivated(object sender, EventArgs e)
		{
			MainForm form = sender as MainForm;

			if (form == null) return;

			form.Hide();

			form.Activated -= OnFormActivated;

			string[] args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
			{
				Win32.SendStringMessage(form.Handle, args[1]);
			}
		}

		#endregion
	}
}
