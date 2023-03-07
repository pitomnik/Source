using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace InterProcessObject
{
	internal sealed class Win32
	{
		#region FileProtection Enum

		public enum FileProtection : uint
		{
			ReadOnly = 2,
			ReadWrite = 4,
		}

		#endregion

		#region FileRights Enum

		public enum FileRights : uint
		{
			Read = 4,
			Write = 2,
			ReadWrite = Read | Write,
		}

		#endregion

		#region Public Members

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static readonly IntPtr InvalidHandle = new IntPtr(-1);

		#endregion

		#region Public Methods

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, FileProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenFileMapping(FileRights dwDesiredAccess, bool bInheritHandle, string lpName);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, FileRights dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
		[DllImport("kernel32.dll")]
		public static extern bool UnmapViewOfFile(IntPtr map);
		[DllImport("kernel32.dll")]
		public static extern int CloseHandle(IntPtr hObject);

		#endregion
	}
}
