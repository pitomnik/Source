using System;
using System.IO;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Tools;
using SurfedAndFound.Shared.Properties;

namespace SurfedAndFound.Shared.Data
{
	public sealed class SqliteDal : GenericDal
	{
		#region Private Members

		private static string fileLocation;
		private static string filePath;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static object syncRoot = new object();
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly SqliteDal instance = CreateInstance();

		#endregion

		#region Constructor(s)

		public SqliteDal(string connectString)
			: base(connectString, DataProvider.SQLite)
		{
		}

		#endregion

		#region Public Members

		public static SqliteDal Instance
		{
			[DebuggerNonUserCode]
			get { return instance; }
		}

		public string FilePath
		{
			[DebuggerNonUserCode]
			get { return filePath; }
		}

		#endregion

		#region Public Methods

		public void CreateFile()
		{
			Close();

			if (!Directory.Exists(fileLocation))
			{
				Directory.CreateDirectory(fileLocation);
			}

			File.WriteAllBytes(filePath, Resources.Template);
		}

		public void DeleteFile()
		{
			Close();

			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}

		public void VerifyFile()
		{
			if (!File.Exists(filePath))
			{
				CreateFile();
			}
		}

		#endregion

		#region Overriden Methods

		public override IDataReader ExecuteReader(string query)
		{
			VerifyFile();

			IDataReader reader;

			lock (syncRoot)
			{
				reader = base.ExecuteReader(query);
			}

			return reader;
		}

		public override IDataReader ExecuteReader(string query, params object[] parameters)
		{
			VerifyFile();

			IDataReader reader;

			lock (syncRoot)
			{
				reader = base.ExecuteReader(query, parameters);
			}

			return reader;
		}

		public override int ExecuteNonQuery(string query)
		{
			VerifyFile();

			int count;

			lock (syncRoot)
			{
				count = base.ExecuteNonQuery(query);
			}

			return count;
		}

		public override int ExecuteNonQuery(string query, params object[] parameters)
		{
			VerifyFile();

			int count;

			lock (syncRoot)
			{
				count = base.ExecuteNonQuery(query, parameters);
			}

			return count;
		}

		public override object ExecuteScalar(string query)
		{
			VerifyFile();

			object result;

			lock (syncRoot)
			{
				result = base.ExecuteScalar(query);
			}

			return result;
		}

		public override object ExecuteScalar(string query, params object[] parameters)
		{
			VerifyFile();

			object result;

			lock (syncRoot)
			{
				result = base.ExecuteScalar(query, parameters);
			}

			return result;
		}

		#endregion

		#region Private Methods

		private static SqliteDal CreateInstance()
		{
			fileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProgramInfo.Instance.Name);
			filePath = Path.Combine(fileLocation, ProgramInfo.Instance.Name + ".db");

			string connectString = String.Format(@"Data Source={0};Version=3;Compress=True;", filePath);

			return new SqliteDal(connectString);
		}

		#endregion
	}
}
