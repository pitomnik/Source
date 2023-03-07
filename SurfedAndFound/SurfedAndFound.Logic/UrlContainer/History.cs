using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SurfedAndFound.Logic.UrlContainer
{
	public class History : IndexData
	{
		private const string historyFolder = "History.IE5";

		protected override string[] GetIndexFiles()
		{
			string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.History), historyFolder);
			string[] files = Directory.GetFiles(folder, indexFileName, SearchOption.AllDirectories);

			return files;
		}
	}
}
