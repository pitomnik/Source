using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Types;

namespace SurfedAndFound.UI
{
	public class ResultArgs : EventArgs
	{
		private readonly WebInfo info;

		public ResultArgs(WebInfo info)
		{
			this.info = info;
		}

		public WebInfo Info
		{
			[DebuggerNonUserCode]
			get { return info; }
		} 
	}
}
