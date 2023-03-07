namespace MultiUserK9
{
	internal interface IUserProfile
	{
		bool HasDefaultProfileBackup();
		void BackupDefaultProfile();
		void RestoreDefaultProfile();
		bool HasUserProfileBackup(string name);
		void BackupUserProfile(string name);
		void RestoreUserProfile(string name);
	}
}
