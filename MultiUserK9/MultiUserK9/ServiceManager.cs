using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace MultiUserK9
{
	internal class ServiceManager
	{
		#region Contructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceManager"/> class.
		/// </summary>
		/// <param name="serviceName">Name of the service.</param>
		/// <exception cref="System.ArgumentException">Argument can't be empty.;serviceName</exception>
		public ServiceManager(string serviceName)
		{
			if (String.IsNullOrEmpty(serviceName))
			{
				throw new ArgumentException("Argument can't be empty.", "serviceName");
			}

			Controller = new ServiceController(serviceName);
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the controller.
		/// </summary>
		/// <value>
		/// The controller.
		/// </value>
		public ServiceController Controller
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a value indicating whether this instance can start.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance can start; otherwise, <c>false</c>.
		/// </value>
		public bool CanStart
		{
			[DebuggerNonUserCode]
			get
			{
				return Controller.Status != ServiceControllerStatus.Running &&
					Controller.Status != ServiceControllerStatus.StartPending;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance can stop.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance can stop; otherwise, <c>false</c>.
		/// </value>
		public bool CanStop
		{
			[DebuggerNonUserCode]
			get
			{
				return Controller.Status != ServiceControllerStatus.Stopped &&
					Controller.Status != ServiceControllerStatus.StopPending;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			if (Controller.Status == ServiceControllerStatus.Running)
			{
				return;
			}

			if (Controller.Status == ServiceControllerStatus.StartPending)
			{
				Controller.WaitForStatus(ServiceControllerStatus.Running);
				return;
			}

			Controller.Start();
			Controller.WaitForStatus(ServiceControllerStatus.Running);
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			if (Controller.Status == ServiceControllerStatus.Stopped)
			{
				return;
			}

			if (Controller.Status == ServiceControllerStatus.StopPending)
			{
				Controller.WaitForStatus(ServiceControllerStatus.Stopped);
				return;
			}

			Controller.Stop();
			Controller.WaitForStatus(ServiceControllerStatus.Stopped);
		}

		/// <summary>
		/// Restarts this instance.
		/// </summary>
		public void Restart()
		{
			Start();
			Stop();
		}

		#endregion
	}
}
