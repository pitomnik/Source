using System;
using System.Diagnostics;

namespace PhotosTree
{
	public class TreeBuilderEventArgs : EventArgs
	{
		#region Private Methods

		private readonly int total;
		private readonly int done;
		private readonly string path;
		private readonly string error;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeBuilderEventArgs"/> class.
		/// </summary>
		/// <param name="total">The total.</param>
		/// <param name="done">The done.</param>
		/// <param name="path">The path.</param>
		public TreeBuilderEventArgs(int total, int done, string path)
			: this(total, done, path, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeBuilderEventArgs"/> class.
		/// </summary>
		/// <param name="total">The total.</param>
		/// <param name="done">The done.</param>
		/// <param name="path">The path.</param>
		/// <param name="error">The error.</param>
		public TreeBuilderEventArgs(int total, int done, string path, string error)
		{
			this.total = total;
			this.done = done;
			this.path = path;
			this.error = error;
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the total.
		/// </summary>
		/// <value>The total.</value>
		public int Total
		{
			[DebuggerNonUserCode]
			get { return total; }
		} 

		/// <summary>
		/// Gets the done.
		/// </summary>
		/// <value>The done.</value>
		public int Done
		{
			[DebuggerNonUserCode]
			get { return done; }
		} 

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <value>The path.</value>
		public string Path
		{
			[DebuggerNonUserCode]
			get { return path; }
		}

		/// <summary>
		/// Gets the error.
		/// </summary>
		/// <value>The error.</value>
		public string Error
		{
			[DebuggerNonUserCode]
			get { return error; }
		} 

		#endregion
	}
}
