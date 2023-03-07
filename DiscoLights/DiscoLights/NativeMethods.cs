using System;
using System.Runtime.InteropServices;

namespace DiscoLights
{
	internal sealed class NativeMethods
	{
		#region MessageBoxCheckFlags Enum

		public enum MessageBoxCheckFlags : uint
		{
			MB_OK = 0x00000000,
			MB_OKCANCEL = 0x00000001,
			MB_YESNO = 0x00000004,
			MB_ICONHAND = 0x00000010,
			MB_ICONQUESTION = 0x00000020,
			MB_ICONEXCLAMATION = 0x00000030,
			MB_ICONINFORMATION = 0x00000040,
		}

		#endregion

		#region Constants

		public const int WM_NCLBUTTONDOWN = 0x00A1;
		public const int HTCAPTION = 0x2;

		#endregion

		#region Public Methods

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		[DllImport("shlwapi.dll", EntryPoint = "#185", BestFitMapping = false)]
		public static extern int SHMessageBoxCheck(IntPtr hWnd, [MarshalAs(UnmanagedType.LPStr)] string pszText, [MarshalAs(UnmanagedType.LPStr)] string pszTitle, MessageBoxCheckFlags uType, int iDefault, [MarshalAs(UnmanagedType.LPStr)] string pszRegVal);

		#endregion
	}
}
