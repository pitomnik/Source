using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using log4net;
using log4net.Core;
using log4net.Config;
using log4net.Repository;

namespace SurfedAndFound.Shared.Tools
{
	public class LogMaster
	{
		#region Private Members

		private static readonly ILog log;

		#endregion

		#region Constructors

		static LogMaster()
		{
			try
			{
				if (File.Exists(ProgramInfo.Instance.ConfigFile))
				{
					FileInfo configInfo = new FileInfo(ProgramInfo.Instance.ConfigFile);

					XmlConfigurator.ConfigureAndWatch(configInfo);
				}
				else
				{
					XmlConfigurator.Configure();
				}
			}
			catch (Exception ex)
			{
				Debug.Assert(false, ex.Message);

				XmlConfigurator.Configure();
			}

			log = LogManager.GetLogger(typeof(LogMaster));
		}

		#endregion

		#region Public Members

		public static ILog Instance
		{
			[DebuggerNonUserCode]
			get { return log; }
		}

		#endregion

		#region Public Methods

		public static void Trace(string format, params object[] args)
		{
			Trace(String.Format(format, args));
		}

		public static void Trace(string message)
		{
			log.Logger.Log(typeof(LogMaster), Level.Trace, message, null);
		}

		#endregion
	}
}
