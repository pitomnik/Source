using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Demo.Properties;

namespace SurfedAndFound.Demo
{
	static class Program
	{
		private const string demoUrl = "http://www.gasanov.net/SurfedAndFoundDemo.htm";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if (CheckConnection(demoUrl))
			{
				Application.Run(new DemoForm(demoUrl));
			}
			else
			{
				MessageBox.Show(String.Format(Resources.WebpageNotAvailable, demoUrl));
			}
		}

		private static bool CheckConnection(string url)
		{
			bool success;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(demoUrl);

			request.Timeout = 2 * 1000;

			try
			{
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					success = response.StatusCode == HttpStatusCode.OK;

					response.Close();
				}
			}
			catch
			{
				success = false;
			}

			return success;
		}
	}
}