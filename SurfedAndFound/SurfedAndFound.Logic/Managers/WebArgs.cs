using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Types;

namespace SurfedAndFound.Logic.Managers
{
	public class WebArgs : EventArgs
	{
		#region Private Members

		private readonly WebPage page;
		private readonly Exception error;

		#endregion

		#region Constructors

		public WebArgs(WebPage page, Exception error)
		{
			this.page = page;
			this.error = error;
		}

		#endregion

		#region Public Members

		public WebPage Page
		{
			[DebuggerNonUserCode]
			get { return page; }
		}

		public Exception Error
		{
			[DebuggerNonUserCode]
			get { return error; }
		}

		#endregion
	}
}
