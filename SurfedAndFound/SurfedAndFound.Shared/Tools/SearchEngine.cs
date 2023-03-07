using System;
using System.Text;
using System.Collections.Generic;

namespace SurfedAndFound.Shared.Tools
{
	public sealed class SearchEngine
	{
		#region Public Methods

		public static bool Search(string text, string query, bool ignoreCase)
		{
			bool found;

			if (String.IsNullOrEmpty(text))
			{
				found = false;
			}
			else
			{
				int index = ignoreCase ? text.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) : text.IndexOf(query);

				found = index > -1;
			}

			return found;
		}

		#endregion
	}
}
