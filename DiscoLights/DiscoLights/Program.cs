using System;
using System.Threading;
using System.Windows.Forms;
using DiscoLights.Properties;

namespace DiscoLights
{
	static class Program
	{
		#region Entry Point

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;
			Application.ApplicationExit += Application_ApplicationExit;

			LogMaster.Instance.InfoFormat("Application entry point.");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		#endregion

		#region Private Events

		/// <summary>
		/// Handles the UnhandledException event of the CurrentDomain control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			HandleUnhandledException(e.ExceptionObject as Exception);
		}

		/// <summary>
		/// Handles the ThreadException event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Threading.ThreadExceptionEventArgs"/> instance containing the event data.</param>
		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			HandleUnhandledException(e.Exception);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles the unhandled exception.
		/// </summary>
		/// <param name="ex">The ex.</param>
		private static void HandleUnhandledException(Exception ex)
		{
			LogMaster.Instance.Error("An unhandled exception occurred.", ex);

			if (MessageBox.Show(Resources.UnexpectedError, Application.ProductName,
				MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				Application.Restart();
			}
			else
			{
				Environment.Exit(0);
			}
		}

		/// <summary>
		/// Handles the ApplicationExit event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private static void Application_ApplicationExit(object sender, EventArgs e)
		{
			LogMaster.Instance.InfoFormat("Application exit event.");
		}

		#endregion
	}
}
