using Cassia;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MultiUserK9
{
	internal class ProfileManager
	{
		#region Constants

		//TODO: Use configuration file.
		private const string serviceName = "bckwfs";
		private const string profileName = "k9profile";

		#endregion

		#region Private Members

		private static readonly ProfileManager _instance = new ProfileManager();
		private readonly Dictionary<int, string> _sessions;
		private readonly ServiceManager _service;
		private readonly IUserProfile _profile;

		#endregion

		#region Contructors

		/// <summary>
		/// Prevents a default instance of the <see cref="ProfileManager"/> class from being created.
		/// </summary>
		private ProfileManager()
		{
			_sessions = new Dictionary<int, string>();
			_service = new ServiceManager(serviceName);
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static ProfileManager Instance
		{
			[DebuggerNonUserCode]
			get { return _instance; }
		}

		#endregion

		#region Public Methods

		public string FindUserName(int sessionId)
		{
			string userName = null;

			if (!_sessions.TryGetValue(sessionId, out userName) ||
				String.IsNullOrEmpty(userName))
			{
				EnumerateSessions();

				_sessions.TryGetValue(sessionId, out userName);
			}

			return userName;
		}

		public void EnumerateSessions()
		{
			ITerminalServicesManager manager = new TerminalServicesManager();

			using (ITerminalServer server = manager.GetLocalServer())
			{
				server.Open();

				_sessions.Clear();

				foreach (ITerminalServicesSession session in server.GetSessions())
				{
					_sessions.Add(session.SessionId, session.UserName);
				}
			}
		}

		public string FindProfileLocation()
		{
			string keyPath = @"System\CurrentControlSet\Services\" + serviceName;
			RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath);
			string location = null;

			if (key != null)
			{
				string value = key.GetValue("ImagePath") as string;

				location = Path.GetDirectoryName(value);

				key.Close();
			}

			return location;
		}

		#endregion
	}
}
