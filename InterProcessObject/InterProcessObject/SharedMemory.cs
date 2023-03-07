using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace InterProcessObject
{
	public sealed class SharedMemory : IDisposable
	{
		#region Private Members

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly int headerSize = sizeof(int);
		private IntPtr fileHandle, mapHandle;

		#endregion

		#region Constructor(s)

		/// <summary>
		/// Initializes a new instance of the <see cref="SharedMemory"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="size">The size.</param>
		/// <param name="existing">If set to <c>true</c> use existing.</param>
		public SharedMemory(string name, uint size, bool existing)
		{
			fileHandle = existing ? Win32.OpenFileMapping(Win32.FileRights.ReadWrite, false, name) :
				fileHandle = Win32.CreateFileMapping(Win32.InvalidHandle, IntPtr.Zero, Win32.FileProtection.ReadWrite, 0, size, name);

			if (fileHandle != IntPtr.Zero)
			{
				mapHandle = Win32.MapViewOfFile(fileHandle, Win32.FileRights.ReadWrite, 0, 0, 0);
			}
		}

		~SharedMemory()
		{
			Dispose();
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the handle.
		/// </summary>
		/// <value>The handle.</value>
		public IntPtr Handle
		{
			[DebuggerNonUserCode]
			get { return mapHandle; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the bytes count.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static int GetBytesCount(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			return headerSize + data.Length;
		}

		/// <summary>
		/// Writes the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public void Write(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			byte[] buffer = new byte[headerSize + data.Length];
			byte[] header = BitConverter.GetBytes(buffer.Length);

			Buffer.BlockCopy(header, 0, buffer, 0, headerSize);
			Buffer.BlockCopy(data, 0, buffer, headerSize, data.Length);

			Marshal.Copy(buffer, 0, mapHandle, buffer.Length);
		}

		/// <summary>
		/// Reads this instance.
		/// </summary>
		/// <returns></returns>
		public byte[] Read()
		{
			byte[] header = new byte[headerSize];

			Marshal.Copy(mapHandle, header, 0, headerSize);

			int dataSize = BitConverter.ToInt32(header, 0);
			byte[] buffer = new byte[headerSize + dataSize];
			byte[] data = new byte[dataSize];

			Marshal.Copy(mapHandle, buffer, 0, buffer.Length);
			Buffer.BlockCopy(buffer, headerSize, data, 0, dataSize);

			return data;
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (mapHandle != IntPtr.Zero)
			{
				Win32.UnmapViewOfFile(mapHandle);
				mapHandle = IntPtr.Zero;
			}

			if (fileHandle != IntPtr.Zero)
			{
				Win32.CloseHandle(fileHandle);
				fileHandle = IntPtr.Zero;
			}

			GC.SuppressFinalize(this);
		}

		#endregion
	}
}