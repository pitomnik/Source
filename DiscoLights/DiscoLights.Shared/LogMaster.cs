using System.Diagnostics;
using log4net;
using log4net.Config;

namespace DiscoLights
{
	internal class LogMaster
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
	}
}
