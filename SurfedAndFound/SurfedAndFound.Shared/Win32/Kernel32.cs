using System;
using System.Text;
using System.Runtime.InteropServices;

namespace SurfedAndFound.Shared.Win32
{
	public sealed class Kernel32
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
	}
}
