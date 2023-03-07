using System;
using System.Diagnostics;
using log4net;
using log4net.Config;
using log4net.Core;

namespace WOW64Menu
{
	public class LogMaster
	{
		#region Private Members

		private static readonly ILog log;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="LogMaster"/> class.
		/// </summary>
		static LogMaster()
		{
			XmlConfigurator.Configure();

			log = LogManager.GetLogger(typeof(LogMaster));
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static ILog Instance
		{
			[DebuggerNonUserCode]
			get { return log; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Traces the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Trace(string format, params object[] args)
		{
			Trace(String.Format(format, args));
		}

		/// <summary>
		/// Traces the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Trace(string message)
		{
			log.Logger.Log(typeof(LogMaster), Level.Trace, message, null);
		}

		#endregion
	}
}
