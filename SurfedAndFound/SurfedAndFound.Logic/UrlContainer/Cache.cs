using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

using SurfedAndFound.Shared.Data;
using SurfedAndFound.Shared.Types;

namespace SurfedAndFound.Logic.UrlContainer
{
	public class Cache : IUrlContainer
	{
		#region Tables Class

		private sealed class Tables
		{
			public const string Cache = "Cache";
		}

		#endregion

		#region Columns Class

		private sealed class Columns
		{
			public const string URL = "URL";
			public const string Date = "Date";
			public const string Title = "Title";
			public const string Text = "Text";
		}

		#endregion

		#region Operators Class

		private sealed class Operators
		{
			public const string OR = "OR";
			public const string LIKE = "LIKE";
		}

		#endregion

		#region Constants

		private const string countCommand = "SELECT COUNT(*) FROM " + Tables.Cache;
		private const string selectCommand = "SELECT * FROM " + Tables.Cache;
		private const string insertCommand = "INSERT OR REPLACE INTO " + Tables.Cache + " (" + Columns.URL + ", " + Columns.Date + ", " + Columns.Title + ", " + Columns.Text + ") VALUES (?, ?, ?, ?)";
		private const string deleteCommand = "DELETE FROM " + Tables.Cache + " WHERE " + Columns.URL + " = ?";
		private const string searchCommand = "SELECT " + Columns.URL + ", " + Columns.Title + " FROM " + Tables.Cache + " WHERE ";

		private const string likePattern = "{0} " + Operators.LIKE + " '%{1}%'";

		#endregion

		#region Constructors

		public Cache()
		{
		}

		#endregion

		#region IUrlContainer Members

		public List<string> GetUrlList()
		{
			List<string> urls = new List<string>();
			IDataReader reader = SqliteDal.Instance.ExecuteReader(selectCommand);

			try
			{
				while (reader.Read())
				{
					urls.Add(reader.GetString(0));
				}
			}
			finally
			{
				reader.Close();
				reader.Dispose();
			}

			return urls;
		}

		#endregion

		#region Public Members

		public void Add(WebPage page)
		{
			if (page == null)
			{
				throw new ArgumentException("Argument can't be null.", "page");
			}

			int affected = SqliteDal.Instance.ExecuteNonQuery(insertCommand, page.Url, DateTime.Now, page.Title, page.Text);
		}

		public void Remove(string url)
		{
			if (String.IsNullOrEmpty(url))
			{
				throw new ArgumentException("Argument can't be null or empty.", "url");
			}

			int affected = SqliteDal.Instance.ExecuteNonQuery(deleteCommand, url);
		}

		public Dictionary<string, string> Search(string query, SearchOptions options)
		{
			options.Validate();

			Dictionary<string, string> urls = new Dictionary<string, string>();
			string normalQuery = NormalizeQuery(query);
			StringBuilder where = new StringBuilder();
			string or = " " + Operators.OR + " ";
			string like = likePattern + or;

			if (options.HasFlag(SearchIn.Address))
			{
				where.AppendFormat(like, Columns.URL, query);
			}
			if (options.HasFlag(SearchIn.Title))
			{
				where.AppendFormat(like, Columns.Title, query);
			}
			if (options.HasFlag(SearchIn.Text))
			{
				where.AppendFormat(like, Columns.Text, query);
			}

			if (where.Length > 0)
			{
				where.Length -= or.Length;
			}

			IDataReader reader = SqliteDal.Instance.ExecuteReader(searchCommand + where);

			try
			{
				int urlIndex = reader.GetOrdinal(Columns.URL);
				int titleIndex = reader.GetOrdinal(Columns.Title);

				while (reader.Read())
				{
					string url = reader.GetString(urlIndex);
					string title = reader.IsDBNull(titleIndex) ? null : reader.GetString(titleIndex);

					urls[url] = title;
				}
			}
			finally
			{
				reader.Close();
				reader.Dispose();
			}

			return urls;
		}

		#endregion

		#region Public Methods

		public long GetCount()
		{
			object count = SqliteDal.Instance.ExecuteScalar(countCommand);

			return count == null ? 0 : (long)count;
		}

		#endregion

		#region Private Methods

		private string NormalizeQuery(string query)
		{
			return query.Replace("'", "''");
		}

		#endregion
	}
}
