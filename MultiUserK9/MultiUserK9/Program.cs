using System.ServiceProcess;

namespace MultiUserK9
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new ProfileService() 
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
