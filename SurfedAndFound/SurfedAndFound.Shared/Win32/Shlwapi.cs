using System;
using System.Text;
using System.Runtime.InteropServices;

namespace SurfedAndFound.Shared.Win32
{
	public sealed class Shlwapi
	{
		[DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool PathMakePretty(ref StringBuilder pszPath);
	}
}
