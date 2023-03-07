using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using SurfedAndFound.Shared.Types;

namespace SurfedAndFound.Logic.Managers
{
	public class SearchArgs : EventArgs
	{
		#region Private Members

		private readonly WebInfo info;
		private readonly int totalCount;
		private readonly int currentCount;

		#endregion

		#region Constructors

		public SearchArgs(WebInfo info, int totalCount, int currentCount)
		{
			this.info = info;
			this.totalCount = totalCount;
			this.currentCount = currentCount;
		}

		#endregion

		#region Public Members

		public WebInfo Info
		{
			[DebuggerNonUserCode]
			get { return info; }
		}

		public int TotalCount
		{
			[DebuggerNonUserCode]
			get { return totalCount; }
		}

		public int CurrentCount
		{
			[DebuggerNonUserCode]
			get { return currentCount; }
		}

		#endregion
	}
}
