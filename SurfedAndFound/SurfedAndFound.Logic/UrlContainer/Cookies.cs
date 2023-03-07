using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SurfedAndFound.Logic.UrlContainer
{
	public class Cookies : IndexData
	{
		protected override string[] GetIndexFiles()
		{
			string dir = Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
			string[] files = Directory.GetFiles(dir, indexFileName);

			return files;
		}
	}
}
