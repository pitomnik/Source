using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ToCoLo
{
	internal class Logger
	{
		#region Constants

		private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss,fff";

		#endregion

		#region Private Members

		private static readonly Logger instance = new Logger();

		private readonly TextWriterTraceListener listener;

		#endregion

		#region Constructors

		/// <summary>
		/// Prevents a default instance of the <see cref="Logger"/> class from being created.
		/// </summary>
		private Logger()
		{
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName);

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string fileName = Path.ChangeExtension(Application.ProductName, ".log");

			FilePath = Path.Combine(folderName, fileName);

			if (File.Exists(FilePath))
			{
				try
				{
					File.Delete(FilePath);
				}
				catch (Exception ex)
				{
					Trace.TraceError(ex.ToString());
				}
			}

			listener = new TextWriterTraceListener(FilePath, Application.ProductName);
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static Logger Instance
		{
			[DebuggerNonUserCode]
			get { return Logger.instance; }
		}

		/// <summary>
		/// Gets or sets the path of the file.
		/// </summary>
		/// <value>
		/// The path of the file.
		/// </value>
		public string FilePath
		{
			private set;
			get;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Writes the specified error.
		/// </summary>
		/// <param name="error">The error.</param>
		public void Write(Exception error)
		{
			Write(error.ToString());
		}

		/// <summary>
		/// Writes the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		public void Write(string format, params object[] args)
		{
			Write(String.Format(format, args));
		}

		/// <summary>
		/// Writes the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Write(string message)
		{
			listener.WriteLine(message, DateTime.Now.ToString(DateTimeFormat));

			listener.Flush();
		}

		#endregion
	}
}
