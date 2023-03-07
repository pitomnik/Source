using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ToCoLo
{
	internal class Win32
	{
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(IntPtr hWndParent, WindowEnumProc lpEnumFunc, IntPtr lParam);
		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr GetParent(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int maxCount);
		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern bool SetWindowText(IntPtr hWnd, string lpString);

		public delegate bool WindowEnumProc(IntPtr hWnd, IntPtr lParam);
	}
}
