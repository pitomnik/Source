using System;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Diagnostics;
using System.Collections.Generic;

namespace SurfedAndFound.Shared.Data
{
	#region DataProvider Enum

	/// <summary>
	/// Data providers supported by <see cref="GenericDal"/>.
	/// </summary>
	public enum DataProvider
	{
		/// <summary>
		/// SQL data provider.
		/// </summary>
		Sql,
		/// <summary>
		/// ODBC data provider.
		/// </summary>
		Odbc,
		/// <summary>
		/// OLEDB data provider.
		/// </summary>
		OleDb,
		/// <summary>
		/// SQLite data provider.
		/// </summary>
		SQLite,
	}

	#endregion

	#region GenericDal Class

	/// <summary>
	/// Represents generic data access layer for various data providers.
	/// </summary>
	public class GenericDal
	{
		#region Private Members

		private readonly DataProvider provider;
		private IDbConnection connection;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="GenericDal"/> class.
		/// </summary>
		/// <param name="connectString">Connection string to data source.</param>
		/// <param name="provider">Data provider <see cref="DataProvider"/>.</param>
		public GenericDal(string connectString, DataProvider provider)
		{
			this.provider = provider;

			switch (provider)
			{
				case DataProvider.Sql:
					connection = new SqlConnection(connectString);
					break;
				case DataProvider.Odbc:
					connection = new OdbcConnection(connectString);
					break;
				case DataProvider.OleDb:
					connection = new OleDbConnection(connectString);
					break;
				case DataProvider.SQLite:
					connection = new SQLiteConnection(connectString);
					break;
				default:
					throw new NotImplementedException(String.Format("\"{0}\" data provider isn't implemented."));
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Calls ExecuteReader method of IDbCommand with specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>The query result.</returns>
		public virtual IDataReader ExecuteReader(string query)
		{
			return ExecuteReader(query, null);
		}

		/// <summary>
		/// Calls ExecuteReader method of IDbCommand with specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The query result.</returns>
		public virtual IDataReader ExecuteReader(string query, params object[] parameters)
		{
			PrepareConnection();

			IDataReader reader;

			using (IDbCommand command = CreateCommand(query))
			{
				FillParameters(command, parameters);
				reader = command.ExecuteReader();
			}

			return reader;
		}

		/// <summary>
		/// Calls ExecuteNonQuery method of IDbCommand with specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Affected rows count.</returns>
		public virtual int ExecuteNonQuery(string query)
		{
			return ExecuteNonQuery(query, null);
		}

		/// <summary>
		/// Calls ExecuteNonQuery method of IDbCommand with specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Affected rows count.</returns>
		public virtual int ExecuteNonQuery(string query, params object[] parameters)
		{
			PrepareConnection();

			int count;

			using (IDbCommand command = CreateCommand(query))
			{
				FillParameters(command, parameters);
				count = command.ExecuteNonQuery();
			}

			return count;
		}

		/// <summary>
		/// Calls ExecuteScalar method of IDbCommand with specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>The scalar result of query.</returns>
		public virtual object ExecuteScalar(string query)
		{
			return ExecuteScalar(query, null);
		}

		/// <summary>
		/// Calls ExecuteScalar method of IDbCommand with specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The scalar result of query.</returns>
		public virtual object ExecuteScalar(string query, params object[] parameters)
		{
			PrepareConnection();

			object result;

			using (IDbCommand command = CreateCommand(query))
			{
				FillParameters(command, parameters);
				result = command.ExecuteScalar();
			}

			return result;
		}

		/// <summary>
		/// Closes the underlying connection.
		/// </summary>
		public void Close()
		{
			if (connection.State != ConnectionState.Closed)
			{
				connection.Close();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Opens the connection if closed.
		/// </summary>
		/// <returns>Current connection state.</returns>
		private ConnectionState PrepareConnection()
		{
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
			}

			return connection.State;
		}

		/// <summary>
		/// Creates the command on underlying connection.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Created command.</returns>
		private IDbCommand CreateCommand(string query)
		{
			IDbCommand command = connection.CreateCommand();

			command.CommandText = query;

			return command;
		}

		/// <summary>
		/// Creates the data parameter.
		/// </summary>
		/// <returns>Data provider.</returns>
		private IDataParameter CreateParameter()
		{
			IDataParameter parameter;

			switch (provider)
			{
				case DataProvider.Sql:
					parameter = new SqlParameter();
					break;
				case DataProvider.Odbc:
					parameter = new OdbcParameter();
					break;
				case DataProvider.OleDb:
					parameter = new OleDbParameter();
					break;
				case DataProvider.SQLite:
					parameter = new SQLiteParameter();
					break;
				default:
					throw new NotImplementedException(String.Format("\"{0}\" data provider isn't implemented."));
			}

			return parameter;
		}

		/// <summary>
		/// Fills the command parameters.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="parameters">The parameters.</param>
		private void FillParameters(IDbCommand command, params object[] parameters)
		{
			if (parameters == null)
			{
				return;
			}

			for (int i = 0; i < parameters.Length; i++)
			{
				IDataParameter parameter = CreateParameter();

				parameter.Value = parameters[i];

				command.Parameters.Add(parameter);
			}
		}

		#endregion
	}

	#endregion
}
