using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace InterProcessObject
{
	public class InterProcessSingleton<T> : IDisposable where T : new()
	{
		#region Constants

		private const string memoryScope = "Local"; // or "Global"
		private const int memorySize = 1024 * 1024;

		#endregion

		#region Private Members

		private readonly string name;
		private readonly Mutex mutex;
		private readonly SharedMemory memory;
		private readonly BinaryFormatter formatter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly InterProcessSingleton<T> instance = new InterProcessSingleton<T>();

		#endregion

		#region Constructor(s)

		/// <summary>
		/// Initializes a new instance of the <see cref="InterProcessSingleton&lt;T&gt;"/> class.
		/// </summary>
		private InterProcessSingleton()
		{
			name = Path.Combine(memoryScope, typeof(T).ToString());
			mutex = new Mutex(false, name + ".Lock");
			memory = new SharedMemory(name, memorySize, true);
			if (memory.Handle == IntPtr.Zero)
			{
				memory = new SharedMemory(name, memorySize, false);
			}
			formatter = new BinaryFormatter();
		}

		~InterProcessSingleton()
		{
			Dispose();
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static InterProcessSingleton<T> Instance
		{
			[DebuggerNonUserCode]
			get { return instance; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the wrapper.
		/// </summary>
		/// <returns></returns>
		public InstanceWrapper<T> GetWrapper()
		{
			AcquireLock();

			T state = LoadState();

			return new InstanceWrapper<T>(state);
		}

		/// <summary>
		/// Loads the state.
		/// </summary>
		/// <returns></returns>
		internal T LoadState()
		{
			T state;
			byte[] data;

			data = memory.Read();

			if (data.Length == 0)
			{
				state = new T();
			}
			else
			{
				using (MemoryStream stream = new MemoryStream(data))
				{
					state = (T)formatter.Deserialize(stream);
					stream.Close();
				}
			}

			return state;
		}

		/// <summary>
		/// Saves the state.
		/// </summary>
		/// <param name="state">The state.</param>
		internal void SaveState(T state)
		{
			byte[] data;

			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, state);

				data = stream.ToArray();

				stream.Close();
			}

			memory.Write(data);
		}

		/// <summary>
		/// Acquires the lock.
		/// </summary>
		internal void AcquireLock()
		{
			mutex.WaitOne();
		}

		/// <summary>
		/// Releases the lock.
		/// </summary>
		internal void ReleaseLock()
		{
			mutex.ReleaseMutex();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			mutex.Close();
			memory.Dispose();

			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
