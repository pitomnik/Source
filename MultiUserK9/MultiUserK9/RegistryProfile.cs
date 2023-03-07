using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace MultiUserK9
{
	internal class RegistryProfile : IUserProfile
	{
		#region Constants

		public const string SourceKey = @"SOFTWARE\Blue Coat Systems\K9";
		public const string TargetKey = @"SOFTWARE\Gasanov.NET\MultiUserK9";
		private static readonly string[] ValueNames = new[] { "pbup", "psum" };

		#endregion

		#region Private Members

		private static readonly IUserProfile _instance = new RegistryProfile();

		#endregion

		#region Constructors

		public RegistryProfile()
		{

		}

		#endregion

		#region Public Members

		public static IUserProfile Instance
		{
			[DebuggerNonUserCode]
			get { return _instance;  }
		}

		#endregion

		#region IUserProfile Members

		public bool HasDefaultProfileBackup()
		{
			return AreValuesExist(TargetKey);
		}

		public void BackupDefaultProfile()
		{
			CopyValues(SourceKey, TargetKey);
		}

		public void RestoreDefaultProfile()
		{
			CopyValues(TargetKey, SourceKey);
		}

		public bool HasUserProfileBackup(string name)
		{
			ValidateName(name);

			return AreValuesExist(Path.Combine(TargetKey, name));
		}

		public void BackupUserProfile(string name)
		{
			ValidateName(name);

			CopyValues(SourceKey, Path.Combine(TargetKey, name));
		}

		public void RestoreUserProfile(string name)
		{
			ValidateName(name);

			CopyValues(Path.Combine(TargetKey, name), SourceKey);
		}

		#endregion

		#region Private Methods

		private void ValidateName(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Argument can't be empty.", "name");
			}
		}

		private bool AreValuesExist(string path)
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey(path);

			if (key == null)
			{
				return false;
			}

			string[] names = key.GetValueNames();

			if (names.Length == 0)
			{
				return false;
			}

			foreach (string valueName in ValueNames)
			{
				if (!Array.Exists(names, x => x.Equals(valueName)))
				{
					return false;
				}
			}

			return true;
		}

		private void CopyValues(string source, string target)
		{
			RegistryKey sourceKey = Registry.LocalMachine.OpenSubKey(source);

			if (sourceKey == null)
			{
				throw new Exception(String.Format("Key '{0}' not found.", source));
			}

			RegistryKey targetKey = Registry.LocalMachine.CreateSubKey(target);

			try
			{
				CopyValues(sourceKey, targetKey);
			}
			finally
			{
				sourceKey.Close();
				targetKey.Close();
			}
		}

		private void CopyValues(RegistryKey source, RegistryKey target)
		{
			if (source.ValueCount == 0)
			{
				throw new Exception(String.Format("Key '{0}' is empty.", source));
			}

			foreach (string valueName in ValueNames)
			{
				RegistryValueKind valueKind = source.GetValueKind(valueName);

				if (valueKind == RegistryValueKind.Unknown)
				{
					continue;
				}

				object value = source.GetValue(valueName);

				target.SetValue(valueName, value, valueKind);
			}
		}

		#endregion
	}
}
