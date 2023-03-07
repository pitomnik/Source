using System;
using System.Diagnostics;

namespace SurfedAndFound.Shared.Types
{
	public class WebInfo
	{
		#region Private Members

		private readonly string url;
		private string title;
		private bool found;

		#endregion

		#region Constructors

		public WebInfo(string url)
		{
			this.url = url;
		}

		#endregion

		#region Public Members

		public string Url
		{
			[DebuggerNonUserCode]
			get { return url; }
		}

		public string Title
		{
			[DebuggerNonUserCode]
			get { return title; }
			[DebuggerNonUserCode]
			set { title = value; }
		}

		public bool Found
		{
			[DebuggerNonUserCode]
			get { return found; }
			[DebuggerNonUserCode]
			set { found = value; }
		}

		#endregion

		#region Public Methods

		public override string ToString()
		{
			if (String.IsNullOrEmpty(title))
			{
				return url;
			}
			else
			{
				return title;
			}
		}

		#endregion
	}
}
