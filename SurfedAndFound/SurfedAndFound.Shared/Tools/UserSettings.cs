using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Data;

namespace SurfedAndFound.Shared.Tools
{
	public sealed class UserSettings
	{
		#region Constants

		private const string insertCommand = "INSERT OR REPLACE INTO Settings (Key, Value) VALUES (?, ?)";
		private const string selectCommand = "SELECT Value FROM Settings WHERE Key = ?";

		#endregion

		#region Public Methods

		public static T Read<T>(string name, T defaultValue)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Argument can't be null or empty.", "name");
			}

			object obj = SqliteDal.Instance.ExecuteScalar(selectCommand, name);
			T value;

			if (obj == null)
			{
				value = defaultValue;
			}
			else
			{
				if (defaultValue is bool)
				{
					obj = Convert.ToInt32(obj) == 1;
				}
				
				value = (T)Convert.ChangeType(obj, typeof(T));
			}

			return value;
		}

		public static void Write<T>(string name, T value)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Argument can't be null or empty.", "name");
			}

			int affected = SqliteDal.Instance.ExecuteNonQuery(insertCommand, name, value);

			Debug.Assert(affected == 1, String.Format("Unexpected count of affected rows: {0}.", affected));
		}

		#endregion
	}
}
