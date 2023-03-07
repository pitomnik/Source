using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SurfedAndFound.Shared.Tools;

namespace SurfedAndFound.Tool
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				ProgramInfo.Instance.Locate();
			}
			catch (Exception ex)
			{
				//TODO: Use exception message box.
				MessageBox.Show(ex.Message);
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}