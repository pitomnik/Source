using System;
using System.Diagnostics;

namespace PhotosTree
{
	public sealed class Formats
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string[] List = new[] { "%year%", "%month%", "%monthname%", "%week%", "%day%", "%hour%", "%minute%" };

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string Year = List[0];
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string Month = List[1];
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string MonthName = List[2];
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string Week = List[3];
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string Day = List[4];
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string Hour = List[5];
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string Minute = List[6];

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly string Default = String.Format("{0}\\{1}", Year, MonthName);
	}
}
