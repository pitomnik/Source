using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using SurfedAndFound.Shared.Win32;

namespace SurfedAndFound.Logic.UrlContainer
{
	public class Favorites : IUrlContainer
	{
		#region Constants

		private const string urlFilePattern = "*.url";
		private const string urlSectionName = "InternetShortcut";
		private const string urlKeyName = "URL";
		private const int urlMaxLength = 2083;

		#endregion

		#region IUrlContainer Members

		public List<string> GetUrlList()
		{
			List<string> list = new List<string>();
			string homePath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
			string[] files = Directory.GetFiles(homePath, urlFilePattern, SearchOption.AllDirectories);

			foreach (string file in files)
			{
				StringBuilder sb = new StringBuilder(urlMaxLength);
				int rc = Kernel32.GetPrivateProfileString(urlSectionName, urlKeyName, String.Empty, sb, sb.Capacity, file);
				string url = rc > 0 ? sb.ToString() : null;

				if (!String.IsNullOrEmpty(url))
				{
					list.Add(url);
				}
			}

			return list;
		}

		#endregion
	}
}
