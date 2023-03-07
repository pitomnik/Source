using System;
using System.Diagnostics;

namespace InterProcessObject
{
	public class InstanceWrapper<T> : IDisposable where T : new()
	{
		#region Private Members

		private readonly T instance;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="InstanceWrapper&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="instance">The instance.</param>
		public InstanceWrapper(T instance)
		{
			this.instance = instance;
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public T Instance
		{
			[DebuggerNonUserCode]
			get { return instance; }
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			InterProcessSingleton<T>.Instance.SaveState(instance);
			InterProcessSingleton<T>.Instance.ReleaseLock();
		}

		#endregion
	}
}
