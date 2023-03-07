using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Tools;
using SurfedAndFound.Logic.UrlContainer;

namespace SurfedAndFound.Logic.UrlContainer
{
	/// <summary>
	/// http://128.175.24.251/forensics/index_dat1.htm
	/// </summary>
	public abstract class IndexData : IUrlContainer
	{
		#region Constants

		protected const string indexFileName = "index.dat";

		private const string urlMarker = "URL";
		private const char urlSymbol = '@';
		private const int urlOffset = 101; // 105 from U

		#endregion

		#region IUrlContainer Members

		public List<string> GetUrlList()
		{
			List<string> list = new List<string>();
			string[] files = GetIndexFiles();

			foreach (string file in files)
			{
				try
				{
					list.AddRange(GetUrlList(file));
				}
				catch (Exception ex)
				{
					LogMaster.Instance.Error(String.Format("Failed to process '{0}'.", file), ex);
				}
			}

			return list;
		}

		#endregion

		#region Private Methods

		protected abstract string[] GetIndexFiles();

		private List<string> GetUrlList(string path)
		{
			List<string> list = new List<string>();
			FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			int readByte = 0;

			try
			{
				while (readByte != -1)
				{
					bool markerFound = false;

					readByte = stream.ReadByte();

					if (readByte == urlMarker[0])
					{
						markerFound = true;

						for (int i = 1; i < urlMarker.Length; i++)
						{
							readByte = stream.ReadByte();

							markerFound = readByte != -1 && readByte == urlMarker[i];

							if (!markerFound)
							{
								break;
							}
						}

						if (markerFound)
						{
							StringBuilder sb = new StringBuilder();

							stream.Position += urlOffset;

							readByte = stream.ReadByte();

							bool charFound = false;

							while (readByte > 0)
							{
								if (charFound)
								{
									sb.Append((char)readByte);
								}
								else
								{
									charFound = readByte == urlSymbol;
								}

								readByte = stream.ReadByte();
							}

							string url = sb.ToString();

							if (url.StartsWith(":"))
							{
								int index = url.IndexOf(":", 1);

								if (index != -1)
								{
									url = url.Substring(index + 1).TrimStart(' ');
								}
							}

							list.Add(url);
						}
					}
				}
			}
			finally
			{
				stream.Close();
				stream.Dispose();
			}

			return list;
		}

		#endregion
	}
}
