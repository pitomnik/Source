using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;

namespace SurfedAndFound.Shared.Tools
{
	public class AppConfig
	{
		#region Keys Class

		public sealed class Keys
		{
			public const string RequestTimeout = "RequestTimeout";
			public const string MaxContentLength = "MaxContentLength";
			public const string ContentTypeFilter = "ContentTypeFilter";
		}

		#endregion

		#region Public Methods

		public static T Read<T>(string keyName, T defaultValue)
		{
			string s = ConfigurationManager.AppSettings[keyName];
			T value;

			if (s == null)
			{
				value = defaultValue;
			}
			else
			{
				value = (T)Convert.ChangeType(s, typeof(T));
			}

			return value;
		}

		#endregion
	}
}
