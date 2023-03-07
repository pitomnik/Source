using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace SurfedAndFound.Logic.Managers
{
	public class ErrorArgs : EventArgs
	{
		private readonly Exception error;
		private bool accepted;

		public ErrorArgs(Exception error)
		{
			this.error = error;
		}

		public Exception Error
		{
			[DebuggerNonUserCode]
			get { return error; }
		}

		public bool Accepted
		{
			[DebuggerNonUserCode]
			get { return accepted; }
			[DebuggerNonUserCode]
			set { accepted = value; }
		}
	}
}
