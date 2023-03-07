using System;
using System.Runtime.InteropServices;

namespace PhotosTree.PhotoCopiers
{
	public class HardLinkCopier : PhotoCopier
	{
		#region External Methods

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

		#endregion

		#region PhotoCopier Members

		/// <summary>
		/// Copies the specified source.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="target">The target.</param>
		protected override void Copy(string source, string target)
		{
			bool success = CreateHardLink(target, source, IntPtr.Zero);

			if (!success)
			{
				int hr = Marshal.GetHRForLastWin32Error();

				throw Marshal.GetExceptionForHR(hr);
			}
		}

		#endregion
	}
}
