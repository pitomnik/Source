using System;
using System.ServiceProcess;

namespace MultiUserK9
{
	public partial class ProfileService : ServiceBase
	{
		public ProfileService()
		{
			InitializeComponent();

			CanShutdown = true;
			CanPauseAndContinue = true;
			CanHandleSessionChangeEvent = true;
		}

		protected override void OnStart(string[] args)
		{
			LogMaster.Instance.Info("Service started.");

			try
			{
				if (!RegistryProfile.Instance.HasDefaultProfileBackup())
				{
					RegistryProfile.Instance.BackupDefaultProfile();
				}
			}
			catch (Exception ex)
			{
				LogMaster.Instance.Error("Failed to backup original copy.", ex);
			}

			try
			{
				ProfileManager.Instance.EnumerateSessions();
			}
			catch (Exception ex)
			{
				LogMaster.Instance.Error("Failed to enumerate sessions.", ex);
			}
		}

		protected override void OnStop()
		{
			LogMaster.Instance.Info("Service stopped.");
		}

		protected override void OnSessionChange(SessionChangeDescription changeDescription)
		{
			base.OnSessionChange(changeDescription);

			string userName = null;

			try
			{
				userName = ProfileManager.Instance.FindUserName(changeDescription.SessionId);
			}
			catch (Exception ex)
			{
				LogMaster.Instance.Error(String.Format("Failed to get session #{0} user.", changeDescription.SessionId), ex);
			}

			LogMaster.Instance.InfoFormat("{0} - ID: {1} Name: '{2}'.", changeDescription.Reason, changeDescription.SessionId, userName);

			if (!String.IsNullOrEmpty(userName))
			{
				try
				{
					HandleSessionChange(changeDescription, userName);
				}
				catch (Exception ex)
				{
					LogMaster.Instance.Error(String.Format("Failed to handle session #{0} change.", changeDescription.SessionId), ex);
				}
			}
		}

		private void HandleSessionChange(SessionChangeDescription changeDescription, string userName)
		{
			switch (changeDescription.Reason)
			{
				case SessionChangeReason.SessionLogon:
				case SessionChangeReason.SessionUnlock:
				case SessionChangeReason.ConsoleConnect:
				case SessionChangeReason.RemoteConnect:
					ProfileManager.Instance.EnumerateSessions();
					LogMaster.Instance.Info("User profile restore started.");
					if (RegistryProfile.Instance.HasUserProfileBackup(userName))
					{
						RegistryProfile.Instance.RestoreUserProfile(userName);
					}
					else
					{
						RegistryProfile.Instance.RestoreDefaultProfile();
					}
					LogMaster.Instance.Info("User profile restore ended.");
					break;
				case SessionChangeReason.SessionLogoff:
				case SessionChangeReason.SessionLock:
				case SessionChangeReason.ConsoleDisconnect:
				case SessionChangeReason.RemoteDisconnect:
					LogMaster.Instance.Info("User profile backup started.");
					RegistryProfile.Instance.BackupUserProfile(userName);
					LogMaster.Instance.Info("User profile backup ended.");
					break;
			}
		}
	}
}
