using System;
using System.Diagnostics;

namespace SurfedAndFound.Shared.Types
{
	#region SearchOptions Struct

	public struct SearchOptions
	{
		#region Public Members

		public LookIn LookInFlags;
		public SearchIn SearchInFlags;

		#endregion

		#region Public Methods

		public bool HasFlag(LookIn flag)
		{
			return (LookInFlags & flag) != LookIn.None;
		}

		public bool HasFlag(SearchIn flag)
		{
			return (SearchInFlags & flag) != SearchIn.None;
		}

		public void Validate()
		{
			if (LookInFlags == LookIn.None || SearchInFlags == SearchIn.None)
			{
				throw new ArgumentException("Options are not properly set.", "options");
			}
		}

		#endregion
	}

	#endregion

	#region LookIn Enum

	[Flags]
	public enum LookIn
	{
		None = 0x0,
		Favorites = 0x1,
		History = 0x2,
		Recent = 0x4,
		Cookies = 0x8,
	}

	#endregion

	#region SearchIn Enum

	[Flags]
	public enum SearchIn
	{
		None = 0x0,
		Text = 0x1,
		Title = 0x2,
		Address = 0x4,
	}

	#endregion
}
