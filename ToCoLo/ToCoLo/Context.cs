using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ToCoLo
{
	internal class Context : ApplicationContext
	{
		#region Constants

		private const string FileName32 = "TOTALCMD.EXE";
		private const string FileName64 = "TOTALCMD64.EXE";
		private const string WindowClass = "TNASTYNAGSCREEN";
		private const string WindowName = "Total Commander";

		#endregion

		#region Private Members

		private event EventHandler onFinish;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Context"/> class.
		/// </summary>
        /// <param name="windowStyle">The window style.</param>
        /// <param name="waitInterval">The wait interval.</param>
        /// <param name="dialogRetries">The dialog retries.</param>
        /// <param name="numberRetries">The number retries.</param>
        /// <param name="titleRetries">The title retries.</param>
        public Context(ProcessWindowStyle windowStyle, int waitInterval, int dialogRetries, int numberRetries, int titleRetries)
		{
            WindowStyle = windowStyle;
            if (waitInterval > 0)
			{
				WaitInterval = waitInterval;
			}
			if (dialogRetries> 0)
			{
				DialogRetries = dialogRetries;
			}
			if (numberRetries > 0)
			{
				NumberRetries = numberRetries;
			}
			if (titleRetries > 0)
			{
				TitleRetries = titleRetries;
			}
		}

		#endregion

		#region Public Members

        /// <summary>
        /// Gets or sets the window style.
        /// </summary>
        /// <value>
        /// The window style.
        /// </value>
        public ProcessWindowStyle WindowStyle
        {
            private set;
            get;
        }

		/// <summary>
		/// Gets or sets the wait interval.
		/// </summary>
		/// <value>
		/// The wait interval.
		/// </value>
		public int WaitInterval
		{
			private set;
			get;
		}

		/// <summary>
		/// Gets or sets the dialog retries.
		/// </summary>
		/// <value>
		/// The dialog retries.
		/// </value>
		public int DialogRetries
		{
			private set;
			get;
		}

		/// <summary>
		/// Gets or sets the number retries.
		/// </summary>
		/// <value>
		/// The number retries.
		/// </value>
		public int NumberRetries
		{
			private set;
			get;
		}

		/// <summary>
		/// Gets or sets the title retries.
		/// </summary>
		/// <value>
		/// The title retries.
		/// </value>
		public int TitleRetries
		{
			private set;
			get;
		}

		/// <summary>
		/// Occurs when context finish.
		/// </summary>
		public event EventHandler Finish
		{
			[DebuggerNonUserCode]
			add { onFinish += value; }
			[DebuggerNonUserCode]
			remove { onFinish -= value; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Begins this context.
		/// </summary>
		public void Begin()
		{
			string file = FindFile();

			if (file == null)
			{
				Logger.Instance.Write("File not found.");
			}
			else
			{
				Logger.Instance.Write("File - {0}.", file);

				string name = Path.GetFileNameWithoutExtension(file);
				Process[] processes = FindUserProcess(name);

				Logger.Instance.Write("Process count - {0}.", processes.Length);

                ProcessStartInfo psi = new ProcessStartInfo();

                psi.FileName = file;
                psi.WindowStyle = WindowStyle;

                Process process = Process.Start(psi);

				if (processes.Length == 0 || processes.Length == 1)
				{
					int retryCount = processes.Length == 0 ? DialogRetries : 1;
					IntPtr hDialog = DoWithRetries<IntPtr, IntPtr>(FindDialog, IntPtr.Zero, IntPtr.Zero, retryCount);

					if (hDialog == IntPtr.Zero)
					{
						Logger.Instance.Write("Dialog not found.");
					}
					else
					{
						Logger.Instance.Write("Dialog handle - {0}.", hDialog);

						bool result = DoWithRetries<IntPtr, bool>(FindNumber, hDialog, true, NumberRetries);

						if (result)
						{
							Logger.Instance.Write("Number not found.");
						}
					}

					IntPtr hParent = DoWithRetries<Process, IntPtr>(FindTitle, processes.Length == 0 ? process : processes[0], IntPtr.Zero, TitleRetries);

					if (hParent == IntPtr.Zero)
					{
						Logger.Instance.Write("Title not found.");
					}
					else
					{
						UpdateTitle(hParent);
					}
				}
			}

			if (onFinish != null)
			{
				onFinish(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Does action the with retries.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="action">The action.</param>
		/// <param name="argument">The argument.</param>
		/// <param name="emptyValue">The empty value.</param>
		/// <param name="retryCount">The retry count.</param>
		/// <returns></returns>
		private TResult DoWithRetries<T, TResult>(Func<T, TResult> action, T argument, TResult emptyValue, int retryCount)
		{
			for (int i = 0; i < retryCount; i++)
			{
				TResult result = action(argument);

				if (!emptyValue.Equals(result))
				{
					return result;
				}

				Thread.Sleep(WaitInterval);
			}

			return emptyValue;
		}

		/// <summary>
		/// Finds the file.
		/// </summary>
		/// <returns></returns>
		private string FindFile()
		{
			if (File.Exists(FileName32))
			{
				return FileName32;
			}
			else if (File.Exists(FileName64))
			{
				return FileName64;
			}

			return null;
		}

		/// <summary>
		/// Finds the dialog.
		/// </summary>
		/// <param name="hParent">The h parent.</param>
		/// <returns></returns>
		private IntPtr FindDialog(IntPtr hParent)
		{
			Logger.Instance.Write("Looking for dialog...");

			IntPtr hDialog = Win32.FindWindow(WindowClass, WindowName);

			return hDialog;
		}

		/// <summary>
		/// Finds the number.
		/// </summary>
		/// <param name="hDialog">The h dialog.</param>
		/// <returns></returns>
		private bool FindNumber(IntPtr hDialog)
		{
			Logger.Instance.Write("Looking for number...");

			bool result = Win32.EnumChildWindows(hDialog, WindowEnum, hDialog);

			return result;
		}

		/// <summary>
		/// Finds the title.
		/// </summary>
		/// <param name="process">The process.</param>
		/// <returns></returns>
		private IntPtr FindTitle(Process process)
		{
			Logger.Instance.Write("Looking for title...");

			process.Refresh();

			if (process.MainWindowHandle == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}

			int length = Win32.GetWindowTextLength(process.MainWindowHandle);

			if (length == 0)
			{
				return IntPtr.Zero;
			}
			else
			{
				Logger.Instance.Write("Title length - {0}.", length);
			}

			return process.MainWindowHandle;
		}

		/// <summary>
		/// Enumerates windows.
		/// </summary>
		/// <param name="hWnd">The handle.</param>
		/// <param name="lParam">The L param.</param>
		/// <returns></returns>
		private bool WindowEnum(IntPtr hWnd, IntPtr lParam)
		{
			const int NumberLength = 1;
			bool @continue = true;

			if (Win32.IsWindowVisible(hWnd))
			{
				StringBuilder sb = new StringBuilder(NumberLength + 1);
				int length = Win32.GetWindowTextLength(hWnd);

				if (length == NumberLength)
				{
					Win32.GetWindowText(hWnd, sb, sb.Capacity);

					string s = sb.ToString();
					int number;

					if (int.TryParse(sb.ToString(), out number) &&
						number >= 1 && number <= 3)
					{
						Logger.Instance.Write("Number found - {0}.", number);

						Win32.SetForegroundWindow(lParam);

						SendKeys.SendWait(s);

						@continue = false;
					}
				}
			}

			return @continue;
		}

		/// <summary>
		/// Finds the user process.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private Process[] FindUserProcess(string name)
		{
			Process curentProcess = Process.GetCurrentProcess();
			Process[] allProcesses = Process.GetProcessesByName(name);
			Process[] userProcesses = Array.FindAll(allProcesses, x => x.SessionId == curentProcess.SessionId);

			return userProcesses;
		}

		/// <summary>
		/// Updates the title.
		/// </summary>
		/// <param name="hParent">The parent handle.</param>
		private void UpdateTitle(IntPtr hParent)
		{
			const int MaxLength = 512;
			StringBuilder sb = new StringBuilder(MaxLength);
			int length = Win32.GetWindowText(hParent, sb, sb.Capacity);
			string title = sb.ToString();
			int index = title.LastIndexOf("-");

			if (index > 0)
			{
				title = title.Substring(0, index).TrimEnd();

				Win32.SetWindowText(hParent, title);
			}
		}

		#endregion
	}
}
