using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WOW64Menu
{
	public sealed class Win32
	{
		#region Constants

		public const int WM_COPYDATA = 0x4A;
		public static readonly IntPtr CDS_ID = new IntPtr(32);

		#endregion

		#region Types

		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public int cbData;
			[MarshalAs(UnmanagedType.LPTStr)]
			public string lpData;
		}

		#endregion

		#region External Methods

		[DllImport("User32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("User32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("User32.dll", EntryPoint = "SendMessage", SetLastError = true)]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("User32.dll", EntryPoint = "SendMessage", SetLastError = true)]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

		#endregion

		#region Public Methods

		/// <summary>
		/// Sends the string message.
		/// </summary>
		/// <param name="handle">The handle.</param>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		public static int SendStringMessage(IntPtr handle, string message)
		{
			SetForegroundWindow(handle);

			COPYDATASTRUCT cds = CreateCopyDataStruct(message);
			int result = SendMessage(handle, WM_COPYDATA, 0, ref cds);

			return result;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates the copy data struct.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns></returns>
		private static COPYDATASTRUCT CreateCopyDataStruct(string message)
		{
			byte[] data = Encoding.Unicode.GetBytes(message + '\0');
			COPYDATASTRUCT cds = new COPYDATASTRUCT();

			cds.dwData = (IntPtr)CDS_ID;
			cds.cbData = data.Length;
			cds.lpData = message;

			return cds;
		}

		#endregion
	}
}
