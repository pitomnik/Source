using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

namespace ToCoLo
{
	static class Program
	{
		private static Context context;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            ProcessWindowStyle windowStyle;
            if (!Enum.TryParse(ConfigurationManager.AppSettings["WindowStyle"], out windowStyle))
            {
                windowStyle = ProcessWindowStyle.Normal;
            }
            int waitInterval = Convert.ToInt32(ConfigurationManager.AppSettings["WaitInterval"] ?? "100");
			int dialogRetries = Convert.ToInt32(ConfigurationManager.AppSettings["DialogRetries"] ?? "10");
			int numberRetries = Convert.ToInt32(ConfigurationManager.AppSettings["NumberRetries"] ?? "5");
			int titleRetries = Convert.ToInt32(ConfigurationManager.AppSettings["TitleRetries"] ?? "3");

            context = new Context(windowStyle, waitInterval, dialogRetries, numberRetries, titleRetries);

			context.Finish += new EventHandler(Context_Finish);

			Application.Idle += new EventHandler(Application_Idle);

			Application.Run(context);
		}

		static void Application_Idle(object sender, EventArgs e)
		{
			Application.Idle -= Application_Idle;

			try
			{
				context.Begin();
			}
			catch (Exception ex)
			{
				Logger.Instance.Write(ex);

				MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		static void Context_Finish(object sender, EventArgs e)
		{
			context.Finish -= Context_Finish;

			Application.Exit();
		}
	}
}
