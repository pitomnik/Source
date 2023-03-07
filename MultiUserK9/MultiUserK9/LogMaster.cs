using log4net;
using log4net.Config;

namespace MultiUserK9
{
	internal class LogMaster
	{
		#region Constructors

		/// <summary>
		/// Initializes the <see cref="LogMaster"/> class.
		/// </summary>
		static LogMaster()
		{
			XmlConfigurator.Configure();

			Instance = LogManager.GetLogger(typeof(LogMaster));
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static ILog Instance
		{
			private set;
			get;
		}

		#endregion
	}
}
