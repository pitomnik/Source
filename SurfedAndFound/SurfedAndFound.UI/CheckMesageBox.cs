using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Win32;
using Microsoft.SqlServer.MessageBox;

using SurfedAndFound.Shared.Tools;

namespace SurfedAndFound.UI
{
	public sealed class CheckMesageBox
	{
		#region Message Enum

		public enum Message
		{
			EmptyCache,
		}

		#endregion

		#region Constants

		private const string registryKey = "DontShow";

		private static readonly RegistryKey registryHive;
		private static readonly string registryPath;

		#endregion

		#region Constructors

		static CheckMesageBox()
		{
			registryHive = Registry.CurrentUser;
			registryPath = String.Format(@"Software\{0}\{1}\{2}", ProgramInfo.Instance.Company, ProgramInfo.Instance.Name, registryKey);
		}

		#endregion

		#region Public Methods

		public static DialogResult Show(Message message, IWin32Window owner, string text, string caption, ExceptionMessageBoxButtons buttons, Icon icon)
		{
			ExceptionMessageBox box = new ExceptionMessageBox();

			box.CheckBoxRegistryKey = registryHive.CreateSubKey(registryPath);
			box.CheckBoxRegistryValue = message.ToString();

			box.Text = text;
			box.Caption = caption;

			box.Buttons = buttons;
			box.CustomSymbol = icon.ToBitmap();

			box.ShowToolBar = false;
			box.ShowCheckBox = true;

			DialogResult result = box.Show(owner);

			return result;
		}

		public static void Clear(Message message)
		{
			registryHive.DeleteSubKey(Path.Combine(registryKey, message.ToString()), false);
		}

		public static void ClearAll()
		{
			registryHive.DeleteSubKey(registryPath, false);
		}

		#endregion
	}
}
