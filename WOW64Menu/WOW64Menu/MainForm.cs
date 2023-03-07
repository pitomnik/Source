using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using GongSolutions.Shell;
using WOW64Menu.Properties;

namespace WOW64Menu
{
	public partial class MainForm : Form
	{
		#region Constants

		private const string titleFormat = "{0} Host";

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MainForm"/> class.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
			InitializeWindow();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Initializes the window.
		/// </summary>
		private void InitializeWindow()
		{
			Width = 0;
			Height = 0;
			
			Text = CreateTitle();
		}

		#endregion

		#region Overriden Methods

		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case Win32.WM_COPYDATA:
					string data = null;
					Win32.COPYDATASTRUCT cds = (Win32.COPYDATASTRUCT)m.GetLParam(typeof(Win32.COPYDATASTRUCT));

					if (cds.dwData == Win32.CDS_ID)
					{
						data = cds.lpData;

						if (!String.IsNullOrEmpty(data))
						{
							string[] args = Program.StringToArgs(data);

							TryShowContextMenuAsync(args);
						}
					}
					break;
			}

			base.WndProc(ref m);
		}

		#endregion

		#region Public Methods

		public static string CreateTitle()
		{
			return String.Format(titleFormat, Application.ProductName);
		}

		/// <summary>
		/// Shows the context menu.
		/// </summary>
		/// <param name="files">The files.</param>
		public void ShowContextMenu(string[] files)
		{
			if (files == null)
			{
				throw new ArgumentNullException("files");
			}

			List<ShellItem> items = new List<ShellItem>();

			for (int i = 0; i < files.Length; i++)
			{
				string file = files[i];

				if (ValidatePath(ref file))
				{
					items.Add(new ShellItem(file));
				}
			}

			ShellContextMenu menu = new ShellContextMenu(items.ToArray());

			menu.ShowContextMenu(Cursor.Position);
		}

		/// <summary>
		/// Tries the show context menu async.
		/// </summary>
		/// <param name="files">The files.</param>
		public void TryShowContextMenuAsync(string[] files)
		{
			BeginInvoke(new Action<string[]>(TryShowContextMenu), new object[] { files });
		}

		/// <summary>
		/// Tries the show context menu.
		/// </summary>
		/// <param name="files">The files.</param>
		public void TryShowContextMenu(string[] files)
		{
			if (files == null || files.Length == 0)
			{
				return;
			}

			try
			{
				ShowContextMenu(files);
			}
			catch (Exception ex)
			{
				Program.LogFileList(files);

				LogMaster.Instance.Error("Failed creating context menu.", ex);
#if DEBUG
				Debugger.Launch();
#endif
				MessageBox.Show(this, Resources.CouldNotShowMenu + Environment.NewLine + ex.Message,
					Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Validates the path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns></returns>
		public bool ValidatePath(ref string path)
		{
			if (String.IsNullOrEmpty(path))
			{
				return false;
			}

			path = path.Trim('\"');

			if (path.Length == 2 && path[1] == ':')
			{
				path += "\\";
			}

			return true;
		}

		#endregion
	}
}
