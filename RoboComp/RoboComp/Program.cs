using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace RoboComp
{
	static class Program
	{
		#region Private Members

		private static readonly string log = Application.ProductName + ".log";
		private static readonly TextWriterTraceListener listener = new TextWriterTraceListener(log);

		#endregion

		#region Entry Point

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if (File.Exists(log))
			{
				File.Delete(log);
			}

			Trace.AutoFlush = true;
			Trace.Listeners.Add(listener);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		#endregion
	}
}
