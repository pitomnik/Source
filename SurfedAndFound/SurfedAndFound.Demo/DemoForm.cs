using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;

namespace SurfedAndFound.Demo
{
	public partial class DemoForm : Form
	{
		public DemoForm(string uri)
		{
			InitializeComponent();

			demoBrowser.Navigate(uri);
		}

		private void DemoForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				Close();
			}
		}

		private void demoBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				Close();
			}
		}
	}
}