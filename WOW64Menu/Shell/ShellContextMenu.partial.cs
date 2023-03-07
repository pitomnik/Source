using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GongSolutions.Shell.Interop;

namespace GongSolutions.Shell
{
	public partial class ShellContextMenu
	{
		#region Constants

		private const uint MF_STRING = 0x00000000;
		private const uint MF_BYPOSITION = 0x00000400;
		private const uint MF_SEPARATOR = 0x00000800;

		private const int cmdAbout = 0x100;
		private const int cmdOnWeb = 0x101;
		private const int cmdDonate = 0x102;

		#endregion

		#region External Methods

		[DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern bool InsertMenu(IntPtr hMenu, uint uPosition, uint uFlags, uint uIDNewItem, string lpNewItem);

		#endregion

		#region Public Methods

		/// <summary>
		/// Shows the context menu.
		/// </summary>
		/// <param name="pos">The pos.</param>
		public void ShowContextMenu(Point pos)
		{
			using (ContextMenu menu = new ContextMenu())
			{
				Populate(menu);

				InsertMenu(menu.Handle, 0, MF_BYPOSITION | MF_SEPARATOR, 0, String.Empty);
				InsertMenu(menu.Handle, 0, MF_BYPOSITION | MF_STRING, cmdDonate, String.Format("Donate to {0}", Application.ProductName));
				InsertMenu(menu.Handle, 0, MF_BYPOSITION | MF_STRING, cmdOnWeb, String.Format("{0} on Web", Application.ProductName));
				InsertMenu(menu.Handle, 0, MF_BYPOSITION | MF_STRING, cmdAbout, String.Format("About {0}", Application.ProductName));

				int command = User32.TrackPopupMenuEx(menu.Handle,
					TPM.TPM_RETURNCMD, pos.X, pos.Y, m_MessageWindow.Handle,
					IntPtr.Zero);
				
				if (command > 0)
				{
					switch (command)
					{
						case cmdAbout:
							InvokeAboutMenu();
							break;
						case cmdOnWeb:
							InvokeOnWebMenu();
							break;
						case cmdDonate:
							InvokeDonateMenu();
							break;
						default:
							InvokeCommand(command - m_CmdFirst);
							break;
					}
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Invokes the about menu.
		/// </summary>
		private void InvokeAboutMenu()
		{
			try
			{
				string copyright = (Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute).Copyright;

				MessageBox.Show(String.Format("{0} * Version {1}{2}{3}", Application.ProductName, Application.ProductVersion, Environment.NewLine, copyright),
					Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch
			{
#if DEBUG
				Debugger.Launch();
#endif
			}
		}

		/// <summary>
		/// Invokes the on web menu.
		/// </summary>
		private void InvokeOnWebMenu()
		{
			try
			{
				Process.Start("http://www.gasanov.net/WOW64Menu.asp");
			}
			catch
			{
#if DEBUG
				Debugger.Launch();
#endif
			}
		}

		/// <summary>
		/// Invokes the donate menu.
		/// </summary>
		private void InvokeDonateMenu()
		{
			try
			{
				Process.Start("http://www.gasanov.net/Donation.asp");
			}
			catch
			{
#if DEBUG
				Debugger.Launch();
#endif
			}
		}

		#endregion
	}
}
