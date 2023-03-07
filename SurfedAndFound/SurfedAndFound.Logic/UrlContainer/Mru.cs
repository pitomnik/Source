using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Win32;

namespace SurfedAndFound.Logic.UrlContainer
{
	public class Mru : IUrlContainer
	{
		private const string path = @"Software\Microsoft\Internet Explorer\TypedURLs";

		#region IUrlContainer Members

		public List<string> GetUrlList()
		{
			List<string> list = new List<string>();
			RegistryKey key = Registry.CurrentUser.OpenSubKey(path);

			if (key != null)
			{
				try
				{
					string[] valueNames = key.GetValueNames();

					foreach (string name in valueNames)
					{
						string value = (string)key.GetValue(name);

						list.Add(value);
					}
				}
				finally
				{
				}
			}

			return list;
		}

		#endregion
	}
}
