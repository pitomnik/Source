using System;
using System.Text;
using System.Runtime.InteropServices;

namespace SurfedAndFound.Shared.Win32
{
	public sealed class User32
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
	}
}
